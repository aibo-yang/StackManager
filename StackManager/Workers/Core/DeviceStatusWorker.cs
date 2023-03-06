using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
    class DeviceStatusWorker : BackgroundWorker
    {
        readonly ILogger<DeviceStatusWorker> logger;
        readonly IEventAggregator eventAggregator;
        readonly IUnitOfWork unitOfWork;
        readonly IMapper mapper;

        readonly IRepository<AlarmCategory> alarmCategoryRepository;
        readonly IRepository<DeviceAlarm> alarmRepository;
        readonly IRepository<DeviceCategory> deviceCategoryRepository;
        readonly IRepository<DeviceStats> deviceStatsRepository;

        FlowlineDevices flowlineDevices = null;
        StackingDevices stackingDevices = null;

        public PLCDeviceInfo[] curFlowlineInfos;
        public PLCDeviceInfo[] curStackingInfos;

        string curlog = string.Empty;

        public DeviceStatusWorker(ILogger<DeviceStatusWorker> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.alarmCategoryRepository = this.unitOfWork.GetRepository<AlarmCategory>();
            this.alarmRepository = this.unitOfWork.GetRepository<DeviceAlarm>();
            this.deviceCategoryRepository = this.unitOfWork.GetRepository<DeviceCategory>();
            this.deviceStatsRepository = this.unitOfWork.GetRepository<DeviceStats>();

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                flowlineDevices = x.Getter<FlowlineDevices>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.FlowlineDevices);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                stackingDevices = x.Getter<StackingDevices>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.StackingDevices);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logList = new List<string>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var log = string.Join(" ", logList);
                if (log != curlog)
                {
                    curlog = log;
                    logger.LogInformation(curlog);
                    logList.Clear();
                }

                await Task.Delay(500);

                logList.Clear();
                logList.Add($"{nameof(DeviceStatusWorker)}:");
                logList.Add($"{flowlineDevices == null},{stackingDevices == null}");

                if (flowlineDevices == null || stackingDevices == null)
                {
                    continue;
                }

                try
                {
                    unitOfWork.ClearDbContext();

                    if (flowlineDevices != null)
                    {
                        if (curFlowlineInfos == null)
                        {
                            curFlowlineInfos = new PLCDeviceInfo[flowlineDevices.Infos.Length];
                            for (int i = 0; i < curFlowlineInfos.Length; i++)
                            {
                                curFlowlineInfos[i] = new PLCDeviceInfo();
                            }
                        }

                        await LogDeviceStatusAsync(curFlowlineInfos, flowlineDevices.Infos, true);
                    }

                    if (stackingDevices != null)
                    {
                        if (curStackingInfos == null)
                        {
                            curStackingInfos = new PLCDeviceInfo[stackingDevices.Infos.Length];
                            for (int i = 0; i < curStackingInfos.Length; i++)
                            {
                                curStackingInfos[i] = new PLCDeviceInfo();
                            }
                        }

                        await LogDeviceStatusAsync(curStackingInfos, stackingDevices.Infos, false);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{nameof(FlowlineWorker)}");
                }
            }
        }

        async Task LogDeviceStatusAsync(PLCDeviceInfo[] curDevices, PLCDeviceInfo[] devices, bool isFlowline)
        {
            if (devices == null)
            {
                return;
            }

            for (int i = 0; i < devices.Length; i++)
            {
                //if (i == 7)
                //{
                //    continue;
                //}

                if (curDevices[i].Status != devices[i].Status ||
                    curDevices[i].IsActivity != devices[i].IsActivity ||
                    curDevices[i].CycleTime != devices[i].CycleTime ||
                    curDevices[i].Alarm != devices[i].Alarm)
                {
                    // 记录状态
                    var deviceCategory = await deviceCategoryRepository.TrackingQuery()
                        .Where(x => x.Index == i && (isFlowline ? 
                            (x.DeviceType == DeviceType.ElevatorLoad || x.DeviceType == DeviceType.ElevatorUnload) : 
                            (x.DeviceType == DeviceType.Robot || x.DeviceType == DeviceType.Pallet)))
                        .FirstOrDefaultAsync();

                    var stats = new DeviceStats
                    {
                        Status = (DeviceStatus)devices[i].Status,
                        IsActivity = devices[i].IsActivity == 1,
                        CycleTime = devices[i].CycleTime,
                        AlarmCode = devices[i].Alarm,
                        DeviceCategory = deviceCategory,
                    };

                    // if (curDevices[i].Status != devices[i].Status || curDevices[i].Alarm != devices[i].Alarm)
                    {
                        // PQM上报
                        var ec = new EventContext
                        {
                            EventType = EventType.ReportPQM,
                        };
                        ec.Setter(stats);
                        this.eventAggregator.GetEvent<EventHub>().Publish(ec);
                    }

                    // 保存数据
                    await deviceStatsRepository.AddAsync(stats);
                    if (await unitOfWork.SaveChangesAsync(async entry =>
                    {
                        entry.Reload();
                        return await Task.FromResult(false);
                    }))
                    {
                        curDevices[i].Status = devices[i].Status;
                        curDevices[i].IsActivity = devices[i].IsActivity;
                        curDevices[i].CycleTime = devices[i].CycleTime;
                    }
                    else
                    {
                        logger.LogError($"保存统计数据错误");
                        continue;
                    }

                    if (curDevices[i].Alarm != devices[i].Alarm)
                    {
                        // 记录报警
                        var alarms = new List<DeviceAlarm>();
                        for (int j = 0; j < 16; j++)
                        {
                            var bit = ByteUtil.GetBitAt(devices[i].Alarm, j);
                            if (ByteUtil.GetBitAt(curDevices[i].Alarm, j) != bit)
                            {
                                var alarmCategory = await alarmCategoryRepository.TrackingQuery()
                                    .Include(x => x.DeviceCategory)
                                    .Where(x => x.Index == j && x.DeviceCategory.Index == i &&
                                    (isFlowline ?
                                        (x.DeviceCategory.DeviceType == DeviceType.ElevatorLoad || x.DeviceCategory.DeviceType == DeviceType.ElevatorUnload) :
                                        (x.DeviceCategory.DeviceType == DeviceType.Robot || x.DeviceCategory.DeviceType == DeviceType.Pallet)))
                                    .SingleOrDefaultAsync();

                                if (alarmCategory == null)
                                {
                                    logger.LogError($"报警位[{i},{j}]不存在");
                                }
                                else
                                {
                                    alarms.Add(new DeviceAlarm { AlarmCategory = alarmCategory, IsRaised = !bit });
                                }
                            }
                        }


                        if (alarms.Any())
                        {
                            await alarmRepository.AddRangeAsync(alarms.ToArray());
                            if (await unitOfWork.SaveChangesAsync(async entry =>
                            {
                                entry.Reload();
                                return await Task.FromResult(false);
                            }))
                            {
                                curDevices[i].Alarm = devices[i].Alarm;
                            }
                            else
                            {
                                logger.LogError($"保存报警数据错误");
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }
}
