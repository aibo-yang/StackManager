using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using AutoMapper;
using Common.Toolkits.Extensions;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Repositories;
using StackManager.UI;

namespace StackManager.ViewModels
{
    class MainViewModel : BindableBase
    {
        private readonly ILogger<MainViewModel> logger;
        private readonly IDialogService dialogService;
        readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IRepository<Setting> settingRepository;

        readonly Random rand = new Random();

        List<SolidColorBrush> LinesBrush = new List<SolidColorBrush>();
        List<double> LinesCanvasLeft = new List<double>();
        List<Tuple<double,double>> PalletsPosition = new List<Tuple<double, double>>();

        Setting setting;

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private bool adminIsLogin;
        public bool AdminIsLogin
        {
            get { return adminIsLogin; }
            set { SetProperty(ref adminIsLogin, value); }
        }

        private int maxCount = 200;
        public int MaxCount
        {
            get { return maxCount; }
            set { SetProperty(ref maxCount, value); }
        }

        public ObservableCollection<DeviceInfo> Devices { get; } = new ObservableCollection<DeviceInfo>();
        public ObservableCollection<OrderInfo> Orders { get; } = new ObservableCollection<OrderInfo>();
        public ObservableCollection<LineInfo> Lines { get; } = new ObservableCollection<LineInfo>();
        public ObservableCollection<ElevatorInfo> Elevators { get; } = new ObservableCollection<ElevatorInfo>();
        public ObservableCollection<CacheInfo> Caches { get; } = new ObservableCollection<CacheInfo>();
        public ObservableCollection<PalletInfo> Pallets { get; } = new ObservableCollection<PalletInfo>();
        public ObservableCollection<AlarmInfo> Alarms { get; } = new ObservableCollection<AlarmInfo>();

        public DelegateCommand<string> ButtonCommands { get; private set; }

        public MainViewModel(ILogger<MainViewModel> logger,
            IDialogService dialogService,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.logger = logger;
            this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.settingRepository = this.unitOfWork.GetRepository<Setting>();

            setting = this.settingRepository.TrackingQuery().SingleOrDefault();
            Title = setting.Name;

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                mapper.Map(x.Getter<Setting>(), setting);
                Title = setting.Name;
            }, ThreadOption.UIThread, true, x => x.EventType == EventType.ProfileChanged);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(
                x => UpdateView(x),ThreadOption.UIThread, true, x => x.EventType == EventType.UpdateView);

            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            for (int i = 0; i < 6; i++)
            {
                LinesBrush.Add(new SolidColorBrush(Color.FromRgb((byte)rand.Next(100, 255), (byte)rand.Next(100, 255), (byte)rand.Next(100, 255))));
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        LinesCanvasLeft.Add(320 * (i / 2));
            //    }
            //    else
            //    {
            //        LinesCanvasLeft.Add(120 + 320 * (i / 2));
            //    }
            //},
            //LinesCanvasLeft[12] += 60;

            LinesCanvasLeft.Add(300);
            LinesCanvasLeft.Add(LinesCanvasLeft[0] + 450);
            LinesCanvasLeft.Add(LinesCanvasLeft[1] + 430);
            LinesCanvasLeft.Add(LinesCanvasLeft[1] + 550);
            LinesCanvasLeft.Add(LinesCanvasLeft[1] + 900);
            LinesCanvasLeft.Add(LinesCanvasLeft[1] + 1020);



            PalletsPosition.Add(new Tuple<double, double>(168, 1218));
            PalletsPosition.Add(new Tuple<double, double>(168, 995));
            PalletsPosition.Add(new Tuple<double, double>(168, 772));
            PalletsPosition.Add(new Tuple<double, double>(168, 549));
            PalletsPosition.Add(new Tuple<double, double>(168, 326));
            PalletsPosition.Add(new Tuple<double, double>(168, 103));
      
            //for (int i = 0; i < 14; i++)
            //{
            //    if (i <= 6)
            //    {
            //        PalletsPosition.Add(new Tuple<double, double>(0, 940 - (144 * i) - (i > 4 ? 70 : 0)));
            //    }
            //    else
            //    {
            //        PalletsPosition.Add(new Tuple<double, double>(244, 940 - (144 * (i-7)) - (i > 11 ? 70 : 0)));
            //    }
            //}

            //for (int i = 10; i < 13; i++)
            //{
            //    PalletsPosition[i] = PalletsPosition[i + 1];
            //}

            //PalletsPosition.RemoveAt(13);
        }

        private void ButtonCommandsClick(string dialogName)
        {
            try
            {
                if (dialogName == "LoginView")
                {
                    dialogService.ShowDialog(dialogName, null, r =>
                    {
                        AdminIsLogin = r.Result == ButtonResult.OK;
                    });
                }
                else if (dialogName == "Logout")
                {
                    AdminIsLogin = false;
                }
                else
                {
                    dialogService.ShowDialog(dialogName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        private void UpdateView(EventContext ec)
        {
            lock (this)
            {
                var infos = ec.Getter<Dictionary<int, object>>();

                var orderInfos = infos[0] as List<OrderInfo>;
                var deviceInfos = infos[1] as List<DeviceInfo>;
                var lineInfos = infos[2] as List<LineInfo>;
                var elevatorInfos = infos[3] as List<ElevatorInfo>;
                var cacheInfos = infos[4] as List<CacheInfo>;
                var palletInfos = infos[5] as List<PalletInfo>;
                var alarmInfos = infos[6] as List<AlarmInfo>;

                if (!Orders.ChildrenIsEqual(orderInfos, (x1, x2) => x1.Name == x2.Name && x1.LineName == x2.LineName && x1.Type == x2.Type))
                {
                    Orders.Clear();
                    Orders.AddRange(orderInfos);
                }
                else
                {
                    foreach (var newOrder in orderInfos)
                    {
                        var order = Orders.Single(x => x.LineName == newOrder.LineName && x.Name == newOrder.Name && x.Type == newOrder.Type);
                        mapper.Map(newOrder, order);
                    }
                }

                if (!Devices.ChildrenIsEqual(deviceInfos, (x1, x2) => x1.Name == x2.Name))
                {
                    Devices.Clear();
                    Devices.AddRange(deviceInfos);
                }
                else
                {
                    foreach (var newDevice in deviceInfos)
                    {
                        var device = Devices.Single(x => x.Name == newDevice.Name);
                        mapper.Map(newDevice, device);
                    }
                }

                if (!Lines.ChildrenIsEqual(lineInfos, (x1, x2) => x1.Name == x2.Name))
                {
                    Lines.Clear();
                    Lines.AddRange(lineInfos);
                }
                else
                {
                    foreach (var newLine in lineInfos)
                    {
                        var line = Lines.Single(x => x.Index == newLine.Index);
                        mapper.Map(newLine, line);
                    }
                }

                for (int i = 0; i < Lines.Count; i++)
                {
                    Lines[i].LineBrush = LinesBrush[i];
                    Lines[i].CanvasLeft = LinesCanvasLeft[i];
                }

                if (!Elevators.ChildrenIsEqual(elevatorInfos, (x1, x2) => x1.Name == x2.Name))
                {
                    Elevators.Clear();
                    Elevators.AddRange(elevatorInfos);
                }
                else
                {
                    foreach (var newElevator in elevatorInfos)
                    {
                        var elevator = Elevators.Single(x => x.Index == newElevator.Index);
                        mapper.Map(newElevator, elevator);
                    }
                }

                if (!Caches.ChildrenIsEqual(cacheInfos, (x1, x2) => x1.Name == x2.Name))
                {
                    Caches.Clear();
                    Caches.AddRange(cacheInfos);
                }
                else
                {
                    foreach (var newCache in cacheInfos)
                    {
                        var cache = Caches.Single(x => x.Index == newCache.Index);
                        mapper.Map(newCache, cache);
                    }
                }

                if (!Pallets.ChildrenIsEqual(palletInfos, (x1, x2) => x1.Name == x2.Name))
                {
                    Pallets.Clear();
                    Pallets.AddRange(palletInfos);
                }
                else
                {
                    foreach (var newPallet in palletInfos)
                    {
                        var pallet = Pallets.Single(x => x.Index == newPallet.Index);
                        mapper.Map(newPallet, pallet);
                    }
                }

                for (int i = 0; i < Pallets.Count; i++)
                {
                    Pallets[i].PalletBrush = Pallets[i].IsExist ? Brushes.Transparent : Brushes.DarkGray;
                    Pallets[i].CanvasLeft = PalletsPosition[i].Item1;
                    Pallets[i].CanvasTop = PalletsPosition[i].Item2;
                }

                if (!Alarms.ChildrenIsEqual(alarmInfos, (x1, x2) => x1.Name == x2.Name))
                {
                    Alarms.Clear();
                    Alarms.AddRange(alarmInfos);
                }
                else
                {
                    foreach (var newAlarm in alarmInfos)
                    {
                        var alarm = Alarms.Single(x => x.Index == newAlarm.Index);
                        mapper.Map(newAlarm, alarm);
                    }
                }
            }
        }
    }
}
