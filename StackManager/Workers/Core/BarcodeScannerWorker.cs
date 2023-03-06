using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Communication;
using Common.Communication.TCP;
using Common.Toolkits.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Repositories;

namespace StackManager.Workers
{
    class BarcodeScannerWorker : BackgroundWorker
    {
        readonly ILogger<BarcodeScannerWorker> logger;
        readonly IEventAggregator eventAggregator;
        readonly IUnitOfWork unitOfWork;
        readonly IRepository<Box> boxRepository;

        object _lock = new object();
        Random rd = new Random();

        TCPClient[] scanners = new TCPClient[6];

#if MOCK_ENABLE
        int barcodeIndex = 0;
#endif 
        public BarcodeScannerWorker(ILogger<BarcodeScannerWorker> logger, 
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.boxRepository = this.unitOfWork.GetRepository<Box>();

            for (int i = 0; i < scanners.Length; i++)
            {
                scanners[i] = new TCPClient($"192.168.10.{81 + i}", 51236, i == 4 ? OutputScannerReceivedHandler : InputScannerReceivedHandler);
            }

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x=>
            {
                var idx = x.Getter<int>();
                if (idx < 0 || idx > 4)
                {
                    return;
                }

#if !MOCK_ENABLE
                if (scanners[idx].Connected)
                {
                    scanners[idx].Send(ByteUtil.GetAsciiBytesFromString((idx == 4) ? $"LON" : $"LON"));
                }
                else
                {
                    scanners[idx].Connect();
                    Thread.Sleep(500);
                    scanners[idx].Send(ByteUtil.GetAsciiBytesFromString((idx == 4) ? $"LON" : $"LON"));
                }
#else
                lock (_lock)
                {
                    var barcode = string.Empty;
                    if (idx != 8)
                    {
                        // barcode = $"00{++barcodeIndex:D5}";
                        if (idx < 5)
                        {
                            barcode = $"{(idx * 2 + rd.Next(0, 2)):D2}{++barcodeIndex:D5}";
                        }
                        else
                        {
                            barcode = $"{(5 + idx):D2}{++barcodeIndex:D5}";
                        }
                    }
                    else
                    {
                        unitOfWork.ClearDbContext();
                        var box = boxRepository.TrackingQuery()
                            .Where(x => x.Status == BoxStatus.Caching)
                            .OrderBy(x => x.LastUpdated)
                            // .OrderBy(x => x.Code)
                            .FirstOrDefault();

                        barcode = box.Code;
                    }

                    var ec = new EventContext { EventType = idx != 8 ? EventType.InputScannerResponse : EventType.OutputScannerResponse };
                    ec.Setter(barcode);
                    this.eventAggregator.GetEvent<EventHub>().Publish(ec);
                }
#endif
            }, ThreadOption.BackgroundThread, true, x=>x.EventType == EventType.ScannerRequest);
        } 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < scanners.Length; i++)
                    {
                        //if (i == 7)
                        //{
                        //    continue;
                        //}

#if MOCK_ENABLE
                        continue;

#else
                        if (!scanners[i].Connected)
                        {
                            scanners[i].Connect();
                        }
#endif
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{nameof(BarcodeScannerWorker)}");
                }

                await Task.Delay(2000);
            }
        }

        private void InputScannerReceivedHandler(SocketContext sc)
        {
            ScannerReceived(sc, true);
        }

        private void OutputScannerReceivedHandler(SocketContext sc)
        {
            ScannerReceived(sc, false);
        }

        private void ScannerReceived(SocketContext sc, bool isInput)
        {
            if (sc.SockStatus == SockStatus.Contected)
            {
                logger.LogInformation($"[{sc.Address}] Connected");
            }
            else if (sc.SockStatus == SockStatus.Disconnected)
            {
                logger.LogInformation($"[{sc.Address}] Disconnected");
            }
            else if (sc.SockStatus == SockStatus.DataReceived)
            {
                var barcode = ByteUtil.GetStringFromAsciiBytes(sc.Buffer, 0, sc.BufferLength);
                
                //barcode = barcode.Replace("\r", "");
                //barcode = barcode.Replace("\n", "");

                var regex = new Regex(@"(NBA\d+)");
                var match = regex.Match(barcode);
                if (match.Success)
                {
                    barcode = match.Groups[1].Value;
                }
                else
                {
                    barcode = string.Empty;
                }

                var ec = new EventContext { EventType = isInput ? EventType.InputScannerResponse : EventType.OutputScannerResponse };
                logger.LogInformation($"{barcode}");
                ec.Setter(barcode);
                this.eventAggregator.GetEvent<EventHub>().Publish(ec);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            for (int i = 0; i < scanners.Length; i++)
            {
                if (scanners[i].Connected)
                {
                    scanners[i].Disconnect();
                }
            }
        }
    }
}
