using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 设备类别
    /// </summary>
    class DeviceCategory : IEntity
    {
        [Display(Name = "设备名称"), MaxLength(200)]
        public string Name { get; set; }

        [Display(Name = "设备序号")]
        public int Index { get; set; }

        [Display(Name = "设备编码"), MaxLength(200)]
        public string Code { get; set; }

        [Display(Name = "设备类型")]
        public DeviceType DeviceType { get; set; }

        [Display(Name = "所属产线")]
        public ICollection<Flowline> Flowlines { get; set; }

        [Display(Name = "设备状态")]
        public ICollection<DeviceStats> Statistics { get; set; }

        [Display(Name = "从设备组")]
        public ICollection<SlaveDevice> SlaveDevices { get; set; }
    }

    enum DeviceType : int
    {
        ProductionLine,
        CacheLine,
        ElevatorLoad,
        ElevatorUnload,
        Robot,
        Pallet,
    }
}
