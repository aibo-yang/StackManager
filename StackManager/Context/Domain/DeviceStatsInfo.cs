using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 统计信息
    /// </summary>
    class DeviceStats : IEntity
    {
        [Display(Name = "设备状态")] // 0 无效
        public DeviceStatus Status { get; set; }

        [Display(Name = "是否产出")]
        public bool IsActivity { get; set; }

        [Display(Name = "循环时间")]
        public int CycleTime { get; set; }

        [Display(Name = "报警代码")]
        public int AlarmCode { get; set; }

        public DeviceCategory DeviceCategory { get; set; }
    }

    enum DeviceStatus : int
    {
        [Display(Name = "无效")]
        None = 0,
        [Display(Name = "准备")]
        Ready,
        [Display(Name = "运行")]
        Run,
        [Display(Name = "寸动")]
        Running,
        [Display(Name = "手动")]
        Manual,
        [Display(Name = "故障")]
        Alarm,
        [Display(Name = "运行条件未满足")]
        RunNot
    }
}
