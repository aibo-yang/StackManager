using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Communication;
using Common.Toolkits.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Context.PLC;
using StackManager.Repositories;

namespace StackManager.Workers
{
    class MockPLCRequestWorker : BackgroundWorker
    {
        readonly ILogger<MockPLCRequestWorker> logger;
        readonly IEventAggregator eventAggregator;
        readonly IUnitOfWork unitOfWork;

        FlowlineRequest flowlineRequest = null;
        FlowlineResponse flowlineResponse = null;

        StackingRequest stackingRequest = null;
        StackingResponse stackingResponse = null;

        FlowlineDevices flowlineDevices = null;
        StackingDevices stackingDevices = null;

        readonly DeviceData[] dbContext;

        readonly IRepository<Box> boxRepository;
        readonly IRepository<Pallet> palletRepository;
        readonly IRepository<Flowline> flowlineRepository;

        public MockPLCRequestWorker(ILogger<MockPLCRequestWorker> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.boxRepository = this.unitOfWork.GetRepository<Box>();
            this.palletRepository = this.unitOfWork.GetRepository<Pallet>();
            this.flowlineRepository = this.unitOfWork.GetRepository<Flowline>();

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                flowlineRequest = x.Getter<FlowlineRequest>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.FlowlineRequest);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                flowlineResponse = x.Getter<FlowlineResponse>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.FlowlineResponse);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                stackingRequest = x.Getter<StackingRequest>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.StackingRequest);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                stackingResponse = x.Getter<StackingResponse>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.StackingResponse);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                flowlineDevices = x.Getter<FlowlineDevices>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.FlowlineDevices);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                stackingDevices = x.Getter<StackingDevices>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.StackingDevices);

            this.dbContext = new DeviceData[]
            {
                new FlowlineRequest(),
                new FlowlineResponse(),
                new StackingRequest(),
                new StackingResponse(),
                new FlowlineDevices(),
                new StackingDevices(),
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rd = new Random();

            var logFlowlineDatetimes = new DateTime[13];
            for (int i = 0; i < logFlowlineDatetimes.Length; i++)
            {
                logFlowlineDatetimes[i] = DateTime.Now;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(300);

                if (flowlineRequest == null)
                {
                    foreach (var db in dbContext)
                    {
                        var ec = new EventContext
                        {
                            EventId = db.Address,
                        };
                        ec.Setter(db);
                        eventAggregator.GetEvent<EventHub>().Publish(ec);
                    }
                    continue;
                }

                if (flowlineRequest == null || flowlineResponse == null || stackingRequest == null || stackingResponse == null || flowlineDevices == null || stackingDevices == null)
                {
                    continue;
                }

                try
                {
                    unitOfWork.ClearDbContext();

                    #region 栈板
                    for (int i = 0; i < stackingRequest.NewPalletRequests.Length; i++)
                    {
                        if (stackingRequest.NewPalletRequests[i] == false && stackingResponse.NewPalletResults[i] == false)
                        {
                            var flowline = await flowlineRepository.TrackingQuery()
                            .Include(x => x.Pallets)
                            .SingleOrDefaultAsync(x => x.Index == i);

                            if (!flowline.Pallets.Any())
                            {
                                stackingRequest.NewPalletRequests[i] = true;
                            }
                        }
                        else if (stackingRequest.NewPalletRequests[i] == true && stackingResponse.NewPalletResults[i] == true)
                        {
                            stackingRequest.NewPalletRequests[i] = false;
                        }
                    }
                    #endregion

                    #region 产线
                    for (int i = 0; i < flowlineRequest.ScanRequests.Length; i++)
                    {
                        if (flowlineRequest.ScanRequests[i] == 0 && flowlineResponse.ScanResults[i] == 0)
                        {
                            var flowline = await flowlineRepository.TrackingQuery()
                                //.Include(x => x.ProductCategory)
                                .Include(x=>x.Pallets)
                                .SingleOrDefaultAsync(x => x.Index == i);

                            if (flowline.Pallets == null || flowline.Pallets.Count != 1)
                            {
                                continue;
                            }

                            // flowlineRequest.ScanRequests[i] = 1;

                            if ((DateTime.Now - logFlowlineDatetimes[i]).TotalSeconds > 10)
                            {
                                logFlowlineDatetimes[i] = DateTime.Now;
                                flowlineRequest.ScanRequests[i] = 1;
                            }
                        }
                        else if (flowlineRequest.ScanRequests[i] == 1 && flowlineResponse.ScanResults[i] == 1)
                        {
                            flowlineRequest.ScanRequests[i] = 0;
                        }
                    }
                    #endregion

                    #region 码垛
                    if (stackingRequest.StowStartRequest == 0 && stackingResponse.StowStartResult.Done == 0)
                    {
                        var box = await boxRepository.TrackingQuery()
                            .Where(x=>x.Status == BoxStatus.Caching)
                            .FirstOrDefaultAsync();

                        if (box != null)
                        {
                            stackingRequest.StowStartRequest = 1;
                        }
                    }
                    else if (stackingRequest.StowStartRequest == 1 && stackingResponse.StowStartResult.Done != 0)
                    {
                        stackingRequest.StowStartRequest = 0;
                    }
                    #endregion
                    
                    #region 状态
                    foreach (var info in flowlineDevices.Infos)
                    {
                        var v = rd.Next(0, 10);
                        info.Status = (ushort)(v >= 3 ? 3 : v);
                        info.IsActivity = (ushort)(v >= 3 ? 1 : 0);
                        info.CycleTime = (ushort)(v * 100);
                        info.Alarm = 0;
                    }

                    foreach (var info in stackingDevices.Infos)
                    {
                        var v = rd.Next(0, 10);
                        info.Status = (ushort)(v >= 3 ? 3 : v);
                        info.IsActivity = (ushort)(v >= 3 ? 1 : 0);
                        info.CycleTime = (ushort)(v * 100);
                        info.Alarm = 0;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{nameof(MockPLCRequestWorker)}");
                }
            }
        }
    }
}
