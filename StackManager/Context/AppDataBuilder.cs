using System;
using System.Collections.Generic;
using System.Linq;
using StackManager.Context.Domain;
using StackManager.Context.PLC;

namespace StackManager.Context
{
    class AppDataBuilder
    {
        public static void SeedData(AppDbContext context)
        {
            if (context.Settings.Any())
            {
                return;
            }

            var rd = new Random();

            #region 设置
            //  http://CNDGNMESWESP002:10101
            var setting = new Setting
            {
                Name = "集中码垛可视化信息平台",
                SerialNumber = "E222048142",
                MesUri = @"http://10.146.192.40:10101/TDC/DELTA_DEAL_TEST_DATA_I",
                MesSecret = "894A0F0DF84A4799E0530CCA940AC604",
                MesTokenId = "894A0F0DF8494799E0530CCA940AC604",
                PqmUri = @"http://10.146.192.29:8090/sensordata?sensorId=UploadMachineData",
                Password = "695F3A335A8181FE226702BE2336A1F1",
            };
            context.Settings.Add(setting);
            context.SaveChanges();
            #endregion

            #region 产品类别
            var productCategories = new ProductCategory[]
            {
                new ProductCategory {Index=1,Name="ADP-180MB PB",BoxCode="3512541100",PLCCode=1,PalletBoxCount=32,BoxRow=3,BoxCol=2,PalletType=1,LayoutType=1,BoxBoard=1,StackType=1,CacheRegion=3},
                new ProductCategory {Index=2,Name="ADP-370AF SB",BoxCode="3512541100",PLCCode=1,PalletBoxCount=32,BoxRow=3,BoxCol=2,PalletType=1,LayoutType=1,BoxBoard=1,StackType=2,CacheRegion=1},
                new ProductCategory {Index=3,Name="ADP-300BB TA",BoxCode="3512541100",PLCCode=2,PalletBoxCount=30,BoxRow=2,BoxCol=3,PalletType=1,LayoutType=1,BoxBoard=1,StackType=2,CacheRegion=2},
            };

            context.ProductCategories.AddRange(productCategories);
            context.SaveChanges();
            #endregion

            #region 从设备组
            var slavedevides = new SlaveDevice[]
           {
               //提升机
                new SlaveDevice { SlaveCode="E222048167", MasterCode="E222048142"},
                new SlaveDevice { SlaveCode="E222048172", MasterCode="E222048143"},
                new SlaveDevice { SlaveCode="E222048177", MasterCode="E222048144"},
                new SlaveDevice { SlaveCode="E222048182", MasterCode="E222048145"},
                //下料机
                new SlaveDevice { SlaveCode="E222048165", MasterCode="E222048150"},
                new SlaveDevice { SlaveCode="E222048168", MasterCode="E222048150"},
                new SlaveDevice { SlaveCode="E222048170", MasterCode="E222048150"},
                new SlaveDevice { SlaveCode="E222048173", MasterCode="E222048150"},
                new SlaveDevice { SlaveCode="E222048175", MasterCode="E222048150"},
                //机械手
                new SlaveDevice { SlaveCode="E222048166", MasterCode="E222048164"},
                new SlaveDevice { SlaveCode="E222048169", MasterCode="E222048164"},
                new SlaveDevice { SlaveCode="E222048171", MasterCode="E222048164"},
                new SlaveDevice { SlaveCode="E222048174", MasterCode="E222048164"},
                new SlaveDevice { SlaveCode="E222048176", MasterCode="E222048164"},
           };

            context.SlaveDevices.AddRange(slavedevides);
            context.SaveChanges();
            #endregion

            #region 线别和设备
            var flowlines = new Flowline[]
            {
                new Flowline { Index=0, Name="H3", },
                new Flowline { Index=1, Name="H4", },
                new Flowline { Index=2, Name="H5", },
                new Flowline { Index=3, Name="H6", },
                new Flowline { Index=4, Name="H7", },
                new Flowline { Index=5, Name="H8", },
            };
            context.Flowlines.AddRange(flowlines);
            context.SaveChanges();
            //20220713
            var loaders = new DeviceCategory[]
            {
                new DeviceCategory { Index=0, Name="提升机1", Code="E222048142", DeviceType= DeviceType.ElevatorLoad, Flowlines=flowlines.Where(x=>x.Index==0).ToList(),SlaveDevices=slavedevides.Where(x=>x.MasterCode=="E222048142").ToList()},
                new DeviceCategory { Index=1, Name="提升机2", Code="E222048143", DeviceType= DeviceType.ElevatorLoad, Flowlines=flowlines.Where(x=>x.Index==1).ToList(),SlaveDevices=slavedevides.Where(x=>x.MasterCode=="E222048143").ToList()},
                new DeviceCategory { Index=2, Name="提升机3", Code="E222048144", DeviceType= DeviceType.ElevatorLoad, Flowlines=flowlines.Where(x=>x.Index==2 || x.Index==3).ToList(),SlaveDevices=slavedevides.Where(x=>x.MasterCode=="E222048144").ToList()},
                new DeviceCategory { Index=3, Name="提升机4", Code="E222048145", DeviceType= DeviceType.ElevatorLoad, Flowlines=flowlines.Where(x=>x.Index==4 || x.Index==5).ToList(),SlaveDevices=slavedevides.Where(x=>x.MasterCode=="E222048145").ToList()},                            
            };

            context.DeviceCategories.AddRange(loaders);

            var unloader = new DeviceCategory { Index = 4, Name = "下料机", Code = "E222048145", DeviceType = DeviceType.ElevatorUnload,SlaveDevices = slavedevides.Where(x => x.MasterCode == "E222048150").ToList() };
            context.DeviceCategories.Add(unloader);

            var robot = new DeviceCategory { Index = 0, Name = "机械手", Code = "E222048160", DeviceType = DeviceType.Robot , SlaveDevices = slavedevides.Where(x => x.MasterCode == "E222048164").ToList() };
            context.DeviceCategories.Add(robot);

            var pallets = new List<DeviceCategory>();
            for (int i = 0; i < 6; i++)
            {
                pallets.Add(new DeviceCategory { Index = 1 + i, Name = $"码垛区{i+1}", Code = $"KSBA{300 + i}", DeviceType = DeviceType.Pallet });
            }
            context.DeviceCategories.AddRange(pallets);

            context.SaveChanges();
            #endregion

            #region 报警类别
            var alarms = new List<AlarmCategory>();
            foreach (var loader in loaders)
            {
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"00", Index = 0, Message = "00", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"01", Index = 1, Message = "提升电机故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"02", Index = 2, Message = "出料输送故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"03", Index = 3, Message = "移栽输送故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"04", Index = 4, Message = "链条断裂报警", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"05", Index = 5, Message = "旋转气缸故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"06", Index = 6, Message = "旋转上升气缸故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"07", Index = 7, Message = "急停", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"08", Index = 8, Message = "阻挡气缸故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"09", Index = 9, Message = "通信故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"10", Index = 10, Message = "扫码NG", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"11", Index = 11, Message = "MES请求NG", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"12", Index = 12, Message = "扫码内容与mes内容不一致请人工取走", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"13", Index = 13, Message = "定位气缸故障", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"14", Index = 14, Message = "扫码产品没有关联箱子类型请人工取走", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = loader, Name = $"15", Index = 15, Message = "15", Description = "", SoftDeleted = true });
            }

            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"00", Index = 0, Message = "00", Description = "", SoftDeleted = true });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"01", Index = 1, Message = "提升电机故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"02", Index = 2, Message = "出料输送故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"03", Index = 3, Message = "移栽输送故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"04", Index = 4, Message = "链条断裂报警", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"05", Index = 5, Message = "旋转气缸故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"06", Index = 6, Message = "旋转上升气缸故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"07", Index = 7, Message = "急停", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"08", Index = 8, Message = "阻挡气缸故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"09", Index = 9, Message = "通信故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"10", Index = 10, Message = "扫码NG", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"11", Index = 11, Message = "MES请求NG", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"12", Index = 12, Message = "扫码内容与mes内容不一致请人工取走", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"13", Index = 13, Message = "定位气缸故障", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"14", Index = 14, Message = "扫码产品没有关联箱子类型请人工取走", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = unloader, Name = $"15", Index = 15, Message = "15", Description = "", SoftDeleted = true });

            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"00", Index = 0, Message = "扫码NG", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"01", Index = 1, Message = "扫码成功非法上线产品人工拿走", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"02", Index = 2, Message = "扫码成功当前没有栈板", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"03", Index = 3, Message = "地轨伺服未使能", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"04", Index = 4, Message = "地轨伺服错误 ", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"05", Index = 5, Message = "地轨伺服未回原点", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"06", Index = 6, Message = "地轨伺服手自动不一致", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"07", Index = 7, Message = "地轨伺服正极限", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"08", Index = 8, Message = "地轨伺服负极限", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"09", Index = 9, Message = "真空吸未吸到料", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"10", Index = 10, Message = "破真空仍感应到料", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"11", Index = 11, Message = "机械手不在安全区域", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"12", Index = 12, Message = "左缓存流线有箱子机器人无法放箱子", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"13", Index = 13, Message = "右缓存流线有箱子机器人无法放箱子", Description = "", SoftDeleted = false });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"14", Index = 14, Message = "14", Description = "", SoftDeleted = true });
            alarms.Add(new AlarmCategory { DeviceCategory = robot, Name = $"15", Index = 15, Message = "15", Description = "", SoftDeleted = true });

            var idx = 0;
            foreach (var pallet in pallets)
            {
                idx++;
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"00", Index = 0, Message = $"栈板{idx}光幕被遮挡", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"01", Index = 1, Message = $"栈板{idx}未到位感应器", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"02", Index = 2, Message = $"栈板{idx}卷帘门未关闭", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"03", Index = 3, Message = $"栈板{idx}未准备好", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"04", Index = 4, Message = $"栈板{idx}满栈板", Description = "", SoftDeleted = false });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"05", Index = 5, Message = "05", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"06", Index = 6, Message = "06", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"07", Index = 7, Message = "07", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"08", Index = 8, Message = "08", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"09", Index = 9, Message = "09", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"10", Index = 10, Message = "10", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"11", Index = 11, Message = "11", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"12", Index = 12, Message = "12", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"13", Index = 13, Message = "13", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"14", Index = 14, Message = "14", Description = "", SoftDeleted = true });
                alarms.Add(new AlarmCategory { DeviceCategory = pallet, Name = $"15", Index = 15, Message = $"栈板{idx}气缸报警", Description = "", SoftDeleted = false });
            }

            context.AlarmCategories.AddRange(alarms);
            context.SaveChanges();
            #endregion
        }
    }
}