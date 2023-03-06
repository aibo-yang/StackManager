using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Communication.HTTP;
using Common.Toolkits.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Context.MES;
using StackManager.Repositories;
using StackManager.UI;

namespace StackManager.Workers
{
    class UpdateViewWorker : BackgroundWorker
    {
        readonly ILogger<UpdateViewWorker> logger;
        readonly IEventAggregator eventAggregator;
        readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        readonly IRepository<Box> boxRepository;
        readonly IRepository<Pallet> palletRepository;
        readonly IRepository<Flowline> flowlineRepository;
        readonly IRepository<DeviceCategory> deviceCategoryRepository;
        readonly IRepository<SlaveDevice> slavedeviceRepository;
        readonly IRepository<DeviceAlarm> deviceAlarmRepository;
        readonly IRepository<Setting> settingRepository;

        readonly List<DeviceInfo> curDeviceInfos = new ();
        Setting setting;

        object pqmLock = new object();

        int hourLog = DateTime.Now.Hour;

        public UpdateViewWorker(ILogger<UpdateViewWorker> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.boxRepository = this.unitOfWork.GetRepository<Box>();
            this.palletRepository = this.unitOfWork.GetRepository<Pallet>();
            this.flowlineRepository = this.unitOfWork.GetRepository<Flowline>();
            this.deviceCategoryRepository = this.unitOfWork.GetRepository<DeviceCategory>();
            this.deviceAlarmRepository = this.unitOfWork.GetRepository<DeviceAlarm>();
            this.settingRepository = this.unitOfWork.GetRepository<Setting>();
            this.slavedeviceRepository = this.unitOfWork.GetRepository<SlaveDevice>();
            this.setting = this.settingRepository.TrackingQuery().SingleOrDefault();
            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                mapper.Map(x.Getter<Setting>(), this.setting);
            }, ThreadOption.BackgroundThread, true, x => x.EventType == EventType.ProfileChanged);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                lock (pqmLock)
                {
                    var infos = new List<PQMReportInfo>();

                    var status = 0;
                    var statusCode = 0;

                    var param = x.Getter<object>();
                    if (param is DeviceStats deviceStats)
                    {
                        status = PlcToPqmStatus((int)deviceStats.Status);
                        statusCode = deviceStats.AlarmCode;

                        var device = curDeviceInfos.SingleOrDefault(x => x.Code == deviceStats.DeviceCategory.Code);
                        


                        if (device == null)
                        {
                            return;
                        }

                        var passQty = 0;
                        if (deviceStats.DeviceCategory.DeviceType == DeviceType.ElevatorLoad)
                        {
                            passQty = flowlineRepository
                                .NoTrackingQuery()
                                .IgnoreQueryFilters()
                                .Include(x => x.Elevator)
                                .Where(x => x.Elevator.Code == deviceStats.DeviceCategory.Code).Sum(x => x.Boxes.Count);
                        }
                        else if (deviceStats.DeviceCategory.DeviceType == DeviceType.Pallet)
                        {
                            passQty = palletRepository
                                .NoTrackingQuery()
                                .IgnoreQueryFilters()
                                .Where(x => x.Code == deviceStats.DeviceCategory.Code)
                                .Sum(x => x.Boxes.Count);

                        }
                        else if (deviceStats.DeviceCategory.DeviceType == DeviceType.ElevatorUnload || deviceStats.DeviceCategory.DeviceType == DeviceType.Robot)
                        {
                            passQty = boxRepository.NoTrackingQuery().IgnoreQueryFilters().Count();
                        }

                        if (passQty == 0)
                        {
                            return;
                        }

                        var pqmInfo = new PQMReportInfo
                        {
                            InterfaceID = deviceStats.DeviceCategory.Code,
                            Status = status,
                            StatusCode = statusCode.ToString(),
                            PassQty = passQty,
                            FailQty = 0,
                            ErrorCnt = 0,
                            ErrorTimes = double.Parse((device.AlarmTime / 1.0).ToString("0.00")),
                            CycleTime = double.Parse((device.CycleTime / 10.0).ToString("0.00")),
                            RunningTime = double.Parse((device.ActiveTime / 1.0).ToString("0.00")),
                            WaitingTime = double.Parse((device.WaitingTime / 1.0).ToString("0.00")),
                            SelfCheck = 1,
                            InputQty = passQty,
                            Barcode = "",
                            Model = "",
                            //CollectDate = DateTime.Now.ToString()
                        };

                        infos.Add(pqmInfo);

                        //从设备发送
                        var slavedevices = slavedeviceRepository
                                            .NoTrackingQuery()
                                            .IgnoreQueryFilters()
                                            .Where(x => x.MasterCode == deviceStats.DeviceCategory.Code).ToList();
                        foreach (var slavedevice in slavedevices)
                        {
                            infos.Add(new PQMReportInfo
                            {
                                InterfaceID = slavedevice.SlaveCode,
                                Status = status,
                                StatusCode = statusCode.ToString(),
                                PassQty = passQty,
                                FailQty = 0,
                                ErrorCnt = 0,
                                ErrorTimes = double.Parse((device.AlarmTime / 1.0).ToString("0.00")),
                                CycleTime = double.Parse((device.CycleTime / 10.0).ToString("0.00")),
                                RunningTime = double.Parse((device.ActiveTime / 1.0).ToString("0.00")),
                                WaitingTime = double.Parse((device.WaitingTime / 1.0).ToString("0.00")),
                                SelfCheck = 1,
                                InputQty = passQty,
                                Barcode = "",
                                Model = "",
                                //CollectDate = DateTime.Now.ToString()
                            });
                        }
                    }
                    else if (param is Box box)
                    {
                        DeviceInfo device = null;

                        if (box.Status == BoxStatus.Caching)
                        {
                            device = curDeviceInfos.SingleOrDefault(x => x.Code == box.Flowline.Elevator.Code);   //提升机
                        }
                        else if (box.Status == BoxStatus.Stacked)
                        {
                            device = curDeviceInfos.SingleOrDefault(x => x.DeviceType == DeviceType.Pallet && x.Index == (box.Flowline.Index + 1));  //栈板号
                        }

                        if (device == null)
                        {
                            return;
                        }

                        var passQty = 0;
                        if (box.Status == BoxStatus.Caching)
                        {
                            passQty = flowlineRepository
                                .NoTrackingQuery()
                                .IgnoreQueryFilters()
                                .Include(x => x.Elevator)
                                .Where(x => x.Elevator.Code == device.Code).Sum(x => x.Boxes.Count);
                        }
                        else if (box.Status == BoxStatus.Stacked)
                        {
                            passQty = palletRepository.NoTrackingQuery()
                                .IgnoreQueryFilters()
                                .Where(x => x.Index + 1 == device.Index)
                                .Sum(x => x.Boxes.Count);
                        }

                        if (passQty == 0)
                        {
                            return;
                        }

                        status = PlcToPqmStatus((int)device.Status);

                        var pqmInfo = new PQMReportInfo
                        {
                            InterfaceID = device.Code,
                            Status = status,
                            StatusCode = "0",
                            PassQty = passQty,
                            FailQty = 0,
                            ErrorCnt = 0,
                            ErrorTimes = double.Parse((device.AlarmTime / 1.0).ToString("0.00")),
                            CycleTime = double.Parse((device.CycleTime / 10.0).ToString("0.00")),
                            RunningTime = double.Parse((device.ActiveTime / 1.0).ToString("0.00")),
                            WaitingTime = double.Parse((device.WaitingTime / 1.0).ToString("0.00")),
                            SelfCheck = 1,
                            InputQty = passQty,
                            Barcode = box.Code,
                            Model = box.ProductCategory.Name,
                            //CollectDate = DateTime.Now.ToString()
                        };

                        infos.Add(pqmInfo);
                        #region 从设备发送
                        var slavedevices = slavedeviceRepository
                                          .NoTrackingQuery()
                                          .IgnoreQueryFilters()
                                          .Where(x => x.MasterCode == device.Code).ToList();
                        foreach (var slavedevice in slavedevices)
                        {
                            infos.Add(new PQMReportInfo
                            {
                                InterfaceID = slavedevice.SlaveCode,
                                Status = status,
                                StatusCode = statusCode.ToString(),
                                PassQty = passQty,
                                FailQty = 0,
                                ErrorCnt = 0,
                                ErrorTimes = double.Parse((device.AlarmTime / 1.0).ToString("0.00")),
                                CycleTime = double.Parse((device.CycleTime / 10.0).ToString("0.00")),
                                RunningTime = double.Parse((device.ActiveTime / 1.0).ToString("0.00")),
                                WaitingTime = double.Parse((device.WaitingTime / 1.0).ToString("0.00")),
                                SelfCheck = 1,
                                InputQty = passQty,
                                Barcode = "",
                                Model = "",
                                //CollectDate = DateTime.Now.ToString()
                            });
                        }
                        #endregion
                        #region 从设备发送
                        List<SlaveDevice> slavedevices1 = null;
                        #endregion
                        if (box.Status == BoxStatus.Stacked)
                        {
                            var devices = deviceCategoryRepository.NoTrackingQuery()
                                .Where(x => x.DeviceType == DeviceType.Robot || x.DeviceType == DeviceType.ElevatorUnload || x.DeviceType == DeviceType.Pallet)
                                .ToList();

                            foreach (var dev in devices)
                            {
                                if (dev.DeviceType == DeviceType.Robot || dev.DeviceType == DeviceType.ElevatorUnload)
                                {
                                    passQty = boxRepository.NoTrackingQuery().IgnoreQueryFilters().Count();
                                }
                                else if (dev.DeviceType == DeviceType.Pallet)
                                {
                                    passQty = palletRepository.NoTrackingQuery().Where(x => x.Index == (box.Flowline.Index)).Sum(x => x.Boxes.Count);
                                }

                                if (passQty == 0)
                                {
                                    return;
                                }

                                device = curDeviceInfos.SingleOrDefault(x => x.Code == dev.Code);
                                status = PlcToPqmStatus((int)device.Status);

                                #region 从设备发送
                                slavedevices1 = slavedeviceRepository
                                          .NoTrackingQuery()
                                          .IgnoreQueryFilters()
                                          .Where(x => x.MasterCode == dev.Code).ToList();
                                #endregion
                                infos.Add(new PQMReportInfo
                                {
                                    InterfaceID = dev.Code,
                                    Status = status,
                                    StatusCode = "0",
                                    PassQty = passQty,
                                    FailQty = 0,
                                    ErrorCnt = 0,
                                    ErrorTimes = double.Parse((device.AlarmTime / 1.0).ToString("0.00")),
                                    CycleTime = double.Parse((device.CycleTime / 10.0).ToString("0.00")),
                                    RunningTime = double.Parse((device.ActiveTime / 1.0).ToString("0.00")),
                                    WaitingTime = double.Parse((device.WaitingTime / 1.0).ToString("0.00")),
                                    SelfCheck = 1,
                                    InputQty = passQty,
                                    Barcode = box.Code,
                                    Model = box.ProductCategory.Name,
                                    //CollectDate = DateTime.Now.ToString()
                                });

                                #region 从设备发送
                                foreach (var slavedevice in slavedevices1)
                                {
                                    infos.Add(new PQMReportInfo
                                    {
                                        InterfaceID = slavedevice.SlaveCode,
                                        Status = status,
                                        StatusCode = statusCode.ToString(),
                                        PassQty = passQty,
                                        FailQty = 0,
                                        ErrorCnt = 0,
                                        ErrorTimes = double.Parse((device.AlarmTime / 1.0).ToString("0.00")),
                                        CycleTime = double.Parse((device.CycleTime / 10.0).ToString("0.00")),
                                        RunningTime = double.Parse((device.ActiveTime / 1.0).ToString("0.00")),
                                        WaitingTime = double.Parse((device.WaitingTime / 1.0).ToString("0.00")),
                                        SelfCheck = 1,
                                        InputQty = passQty,
                                        Barcode = "",
                                        Model = "",
                                        //CollectDate = DateTime.Now.ToString()
                                    });
                                }
                                #endregion
                            }

                        }
                    }

                    foreach (var info in infos)
                    {
                        try
                        {
                            var res = HttpRequest.PostJsonAsync($"{setting.PqmUri}", "[" + JsonConvert.SerializeObject(info) + "]").Result;
                            logger.LogInformation("PQM CONTENT:" + JsonConvert.SerializeObject(info).ToString());
                            logger.LogInformation($"PQM上报成功: {res}");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "PQM上报失败");
                            break;
                        }
                    }
                }
            }, ThreadOption.BackgroundThread, true, x => x.EventType == EventType.ReportPQM);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 从7:30开始统计
            var curDate = DateTime.Now.Date.AddDays(-1).AddHours(7).AddMinutes(30);
            var idx = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000);

                lock (pqmLock)
                {
                    if ((DateTime.Now - curDate).TotalSeconds > 24 * 60 * 60)
                    {
                        curDate = curDate.AddDays(1);
                    }

                    var curHour = DateTime.Now.Hour;
                    if (hourLog != curHour)
                    { 
                        // 开始查询

                        hourLog = curHour;
                    }

                    // curDate = DateTime.Now.AddDays(-100);

                    var orderInfos = new List<OrderInfo>();
                    var deviceInfos = new List<DeviceInfo>();
                    var lineInfos = new List<LineInfo>();
                    var cacheInfos = new List<CacheInfo>();
                    var palletInfos = new List<PalletInfo>();
                    var elevatorInfos = new List<ElevatorInfo>();
                    var alarmInfos = new List<AlarmInfo>();

                    var infos = new Dictionary<int, object>()
                {
                    { 0, orderInfos },
                    { 1, deviceInfos },
                    { 2, lineInfos },
                    { 3, elevatorInfos },
                    { 4, cacheInfos },
                    { 5, palletInfos },
                    { 6, alarmInfos },
                };

                    try
                    {
                        unitOfWork.ClearDbContext();

                        #region 订单信息
                        var orders = (boxRepository.TrackingQuery()
                            .IgnoreQueryFilters()
                            .Include(x => x.Flowline)
                            .ThenInclude(x => x.Boxes)
                            .ThenInclude(x => x.ProductCategory)
                            .Where(x => x.LastUpdated > curDate)
                            .OrderBy(x => x.Flowline.Index)
                            .ToList())
                            .GroupBy(x => x.OrderNo);

                        foreach (var grpOrder in orders)
                        {
                            var grplines = grpOrder.GroupBy(x => x.Flowline.Name);
                            foreach (var grpLine in grplines)
                            {
                                var grpBoxs = grpLine.GroupBy(x => x.ProductCategory.Name);

                                foreach (var grpBox in grpBoxs)
                                {
                                    var order = grpLine.OrderBy(x => x.LastUpdated).First();

                                    var orderInfo = new OrderInfo
                                    {
                                        Name = grpOrder.Key,
                                        LineName = grpLine.Key,
                                        Type = grpBox.First().ProductCategory.Name,
                                        Count = grpBox.Count(),
                                        StartDateTime = order.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"),
                                    };
                                    orderInfos.Add(orderInfo);
                                }
                            }
                        }

                        #endregion

                        #region 设备状态
                        var devices = deviceCategoryRepository.NoTrackingQuery()
                            .Include(x => x.Statistics.Where(x => x.LastUpdated > curDate)
                            .OrderBy(x => x.LastUpdated))
                            .ToList();

                        foreach (var device in devices)
                        {
                            var deviceInfo = new DeviceInfo
                            {
                                Index = device.Index,
                                Name = device.Name,
                                Code = device.Code,
                                DeviceType = device.DeviceType,
                            };

                            var stats = device.Statistics.LastOrDefault();
                            if (stats == null)
                            {
                                continue;
                            }

                            deviceInfo.Status = (int)stats.Status;
                            deviceInfo.CycleTime = stats.CycleTime;
                            deviceInfo.CycleTime = stats.CycleTime;

                            // 计算稼动率
                            var totalTime = (DateTime.Now - curDate).TotalSeconds;
                            // var totalTime = 1000;
                            var activeTime = 0d;
                            var alarmTime = 0d;
                            DateTime? activeStart = null;
                            DateTime? alarmStart = null;

                            foreach (var ds in device.Statistics)
                            {
                                if (ds.IsActivity)
                                {
                                    if (activeStart == null)
                                    {
                                        activeStart = ds.LastUpdated;
                                    }
                                }
                                else
                                {
                                    if (activeStart != null)
                                    {
                                        activeTime += (ds.LastUpdated - activeStart.Value).TotalSeconds;
                                        activeStart = null;
                                    }
                                }

                                if (ds.AlarmCode != 0)
                                {
                                    if (alarmStart == null)
                                    {
                                        alarmStart = ds.LastUpdated;
                                    }
                                }
                                else
                                {
                                    if (alarmStart != null)
                                    {
                                        alarmTime += (ds.LastUpdated - alarmStart.Value).TotalSeconds;
                                        alarmStart = null;
                                    }
                                }
                            }

                            if (activeStart != null)
                            {
                                activeTime += (DateTime.Now - activeStart.Value).TotalSeconds;
                            }

                            if (alarmStart != null)
                            {
                                alarmTime += (DateTime.Now - alarmStart.Value).TotalSeconds;
                            }

                            deviceInfo.ActiveTime = activeTime;
                            deviceInfo.AlarmTime = alarmTime;
                            deviceInfo.WaitingTime = totalTime - activeTime - alarmTime;
                            deviceInfo.Efficency = Math.Round(activeTime / totalTime, 2);
                            deviceInfos.Add(deviceInfo);

                            var di = curDeviceInfos.SingleOrDefault(x => x.Name == deviceInfo.Name);
                            if (di == null)
                            {
                                curDeviceInfos.Add(deviceInfo);
                            }
                            else
                            {
                                di.Code = deviceInfo.Code;
                                di.Status = deviceInfo.Status;
                                di.CycleTime = deviceInfo.CycleTime;
                                di.Efficency = deviceInfo.Efficency;
                                di.ActiveTime = deviceInfo.ActiveTime;
                                di.AlarmTime = deviceInfo.AlarmTime;
                            }
                        }
                        #endregion

                        #region 产线信息
                        var flowlines =flowlineRepository.NoTrackingQuery()
                            .IgnoreQueryFilters()
                            .Include(x => x.Boxes.Where(x => x.LastUpdated > curDate))
                            .ToList();


                        foreach (var flowline in flowlines)
                        {
                            var lineInfo = new LineInfo
                            {
                                Index = flowline.Index + 1,
                                Name = flowline.Name,
                                CurrentCount = flowline.Boxes.Count,
                            };
                            lineInfos.Add(lineInfo);
                        }

                        var maxCount = lineInfos.Max(x => x.CurrentCount);
                        foreach (var line in lineInfos)
                        {
                            line.MaxCount = maxCount;
                        }
                        #endregion

                        #region 提升机
                        var elevators =deviceCategoryRepository.NoTrackingQuery()
                            .IgnoreQueryFilters()
                            .Include(x => x.Flowlines)
                            .ThenInclude(x => x.Boxes.Where(x => x.LastUpdated > curDate))
                            .Where(x => x.DeviceType == DeviceType.ElevatorLoad)
                            .ToList();


                        foreach (var elevator in elevators)
                        {
                            var elevatorInfo = new ElevatorInfo
                            {
                                Index = elevator.Index + 1,
                                Name = elevator.Name,
                                CurrentCount = elevator.Flowlines.Sum(x => x.Boxes.Count)
                            };

                            elevatorInfos.Add(elevatorInfo);
                        }
                        #endregion

                        #region 缓存信息
                        var caches =boxRepository.NoTrackingQuery()
                            .Include(x => x.ProductCategory)
                            .Where(x => x.Status == BoxStatus.Caching)
                            .ToList();

                        var grpCaches = caches.GroupBy(x => x.ProductCategory.Name);

                        idx = 0;
                        foreach (var grpCache in grpCaches)
                        {
                            var box = grpCache.First();
                            var cacheInfo = new CacheInfo
                            {
                                Index = ++idx,
                                Name = grpCache.Key,
                                Count = grpCache.Count()
                            };

                            cacheInfos.Add(cacheInfo);
                        }
                        #endregion

                        #region 栈板信息
                        var flowlinePallets =flowlineRepository.NoTrackingQuery()
                            .Include(x => x.Pallets)
                            .ThenInclude(x => x.Boxes)
                            .ThenInclude(x => x.ProductCategory)
                            .ToList();

                        idx = 0;
                        foreach (var flowline in flowlinePallets)
                        {
                            var pallet = flowline.Pallets.SingleOrDefault();

                            var palletInfo = new PalletInfo
                            {
                                Index = ++idx,
                                Name = flowline.Name,
                                IsExist = (pallet != null),
                            };

                            if (pallet != null && pallet.Boxes.Any())
                            {
                                palletInfo.Type = pallet.Boxes.First().ProductCategory.Name;
                                palletInfo.MaxCount = pallet.Boxes.First().ProductCategory.PalletBoxCount;
                                palletInfo.CurrentCount = pallet.Boxes.Where(x=>x.Status == BoxStatus.Stacked).ToList().Count;
                            }

                            palletInfos.Add(palletInfo);
                        }
                        #endregion

                        #region 报警信息
                        var alarms = deviceAlarmRepository.NoTrackingQuery()
                            .Include(x => x.AlarmCategory)
                            .ThenInclude(x => x.DeviceCategory)
                            .Where(x => x.IsRaised)
                            .OrderByDescending(x => x.LastUpdated)
                            .Take(20)
                            .ToList();


                        idx = 0;
                        foreach (var alarm in alarms)
                        {
                            if (alarm.AlarmCategory != null && alarm.AlarmCategory.Description != null)
                            {
                                var alarmInfo = new AlarmInfo
                                {
                                    Index = ++idx,
                                    Name = alarm.AlarmCategory.Name,
                                    StationName = alarm.AlarmCategory.DeviceCategory.Name,
                                    Message = alarm.AlarmCategory.Message,
                                    DateTime = alarm.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"),
                                };
                                alarmInfos.Add(alarmInfo);
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"{nameof(UpdateViewWorker)}");
                    }
                    finally
                    {
                        var ec = new EventContext { EventType = EventType.UpdateView };
                        ec.Setter(infos);

                        eventAggregator.GetEvent<EventHub>().Publish(ec);
                    }
                }
            }
        }

        int PlcToPqmStatus(int status)
        {
            if (status == 1) // 准备
            {
                return 3;
            }
            else if (status == 2) // 运行
            {
                return 0;
            }
            else if (status == 3) // 寸动
            {
                return 2;
            }
            else if (status == 4) // 手动
            {
                return 2;
            }
            else if (status == 5) // 故障
            {
                return 1;
            }
            else if (status == 6) // 未满足
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }
    }
}
