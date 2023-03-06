using AutoMapper;
using StackManager.Context.Domain;
using StackManager.UI;

namespace StackManager.Extensions
{
    public class AutoMapperExtension : MapperConfigurationExpression
    {
        public AutoMapperExtension()
        {
            CreateMap<Setting, Setting>();
            CreateMap<AlarmInfo, AlarmInfo>();
            CreateMap<CacheInfo, CacheInfo>();
            CreateMap<DeviceInfo, DeviceInfo>();
            CreateMap<ElevatorInfo, ElevatorInfo>();
            CreateMap<LineInfo, LineInfo>();
            CreateMap<OrderInfo, OrderInfo>();
            CreateMap<PalletInfo, PalletInfo>();
            CreateMap<RobotInfo, RobotInfo>();
        }
    }
}
