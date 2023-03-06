using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Communication;
using Common.Communication.Modbus;
using Common.Toolkits.Workers;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.Event;

namespace StackManager.Workers
{
    public abstract class PLCCommunicationWorker : BackgroundWorker
    {
        protected string ipAddress { get; }
        protected DeviceData[] dataArray { get; }

        private readonly ModbusTCP client;
        private readonly ILogger<PLCCommunicationWorker> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly string workerName;
        private readonly int deviceId;

        public PLCCommunicationWorker(ILogger<PLCCommunicationWorker> logger,
            IEventAggregator eventAggregator,
            string workerName,
            int deviceId,
            string ipAddress,
            DeviceData[] dataArray)
        {
            this.ipAddress = ipAddress;
            this.dataArray = dataArray;
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.workerName = workerName;
            this.deviceId = deviceId;

            client = new ModbusTCP(ipAddress);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (client.Connect() == 0)
            {
                logger.LogInformation($"[{ipAddress}] Connected");
            }
            else
            {
                logger.LogInformation($"[{ipAddress}] Disconnected");
            }

            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            if (client.Connected)
            {
                client.Disconnect();
            }

            logger.LogInformation($"[{ipAddress}] Disconnected");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rs = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!client.Connected)
                    {
                        await Task.Delay(10000, stoppingToken);
                        if (!client.Connected)
                        {
                            if (client.Connect() == 0)
                            {
                                logger.LogInformation($"[{ipAddress}] Connected");
                            }
                            else
                            {
                                logger.LogInformation($"[{ipAddress}] Disconnected");
                            }
                            continue;
                        }
                        // 这里会多次重复连接
                    }

                    foreach (var data in dataArray.Where(x => x.IsActived))
                    {
                        if (!data.InitOk)
                        {
                            rs = ReadDBBytes(data);
                            if (rs)
                            {
                                data.InitOk = true;

                                var ec = new EventContext
                                {
                                    EventId = data.Address,
                                    DeviceId = deviceId,
                                    Source = workerName,
                                };
                                ec.Setter(data);
                                eventAggregator.GetEvent<EventHub>().Publish(ec);
                            }
                        }
                        else
                        {
                            if (data.ReadOnly)
                            {
                                if ((DateTime.Now - data.LastUpdated).TotalMilliseconds >= data.ReadInterval)
                                {
                                    rs = ReadDBBytes(data);
                                    if (rs)
                                    {
                                        data.LastUpdated = DateTime.Now;
                                    }
                                }
                            }
                            else
                            {
                                rs = WriteDBBytes(data);
                                if (rs)
                                {
                                    data.LastUpdated = DateTime.Now;
                                }
                            }
                        }

                        if (!rs)
                        {
                            logger.LogError($"{workerName}[{data.Address},{data.Size}] Failed=>{rs}");
                            client.Disconnect();
                            break;
                        }

                        await Task.Delay(50, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{nameof(PLCCommunicationWorker)}");
                }
            }
        }

        private bool ReadDBBytes(DeviceData data)
        {
            var buffer = new byte[data.BufferSize];

            if (client.RegisterRead(data.Address, data.Size, buffer) != 0)
            {
                return false;
            }

            if (!Enumerable.SequenceEqual(buffer, data.MainBuffer))
            {
                if (data.ReadOnly)
                {
                    Array.Copy(buffer, data.MainBuffer, data.BufferSize);
                }
                else
                {
                    if (Enumerable.SequenceEqual(data.MainBuffer, data.WriteCache))
                    {
                        // 程序重启导致状态不同步,如果刚好在写入未成功的时候重启就无法判断
                        Array.Copy(buffer, data.MainBuffer, data.BufferSize);
                    }
                    else
                    {
                        // 通信断开导致状态不同步,继续写操作
                        Array.Copy(buffer, data.WriteCache, data.BufferSize);
                    }
                }

                data.ToEntity();
            }

            return true;
        }

        private bool WriteDBBytes(DeviceData data)
        {
            data.ToBuffer();
            if (Enumerable.SequenceEqual(data.MainBuffer, data.WriteCache))
            {
                return true;
            }

            if (client.RegisterWrite(data.Address, data.Size, data.MainBuffer) != 0)
            {
                return false;
            }

            Array.Copy(data.MainBuffer, data.WriteCache, data.BufferSize);
            return true;
        }
    }
}
