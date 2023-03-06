using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Toolkits.Workers;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Repositories;

namespace StackManager.Workers
{
    class TimerUpdateWorker : BackgroundWorker
    {
        private readonly ILogger<TimerUpdateWorker> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        readonly IRepository<Setting> settingRepository;

        Setting setting;

        public TimerUpdateWorker(ILogger<TimerUpdateWorker> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.settingRepository = this.unitOfWork.GetRepository<Setting>();
            this.setting = this.settingRepository.TrackingQuery().SingleOrDefault();

            var ctx = new EventContext
            {
                EventType = EventType.ProfileChanged
            };
            ctx.Setter(setting);
            eventAggregator.GetEvent<EventHub>().Publish(ctx);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Start");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // bool firstRun = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                //if (firstRun)
                //{
                //    firstRun = false;
                //}

                eventAggregator.GetEvent<EventHub>().Publish(new EventContext
                {
                    EventType = EventType.TimerUpdated,
                });

                var newSetting = settingRepository.NoTrackingQuery().SingleOrDefault();
                if (!Enumerable.SequenceEqual(newSetting.RowVersion, setting.RowVersion))
                {
                    mapper.Map(newSetting, setting);
                    var ctx = new EventContext
                    {
                        EventType = EventType.ProfileChanged
                    };
                    ctx.Setter(setting);
                    eventAggregator.GetEvent<EventHub>().Publish(ctx);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stop");
            return base.StopAsync(cancellationToken);
        }
    }
}