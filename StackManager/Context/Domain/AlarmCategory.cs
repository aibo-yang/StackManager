using System.ComponentModel.DataAnnotations;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 报警类别
    /// </summary>
    class AlarmCategory : IEntity
    {
        [Display(Name = "报警名称"), MaxLength(200)]
        public string Name { get; set; }

        [Display(Name = "报警序号")]
        public int Index { get; set; }

        [Display(Name = "报警信息"), MaxLength(200)]
        public string Message { get; set; }

        [Display(Name = "报警描述"), MaxLength(200)]
        public string Description { get; set; }

        [Display(Name = "所属设备")]
        public DeviceCategory DeviceCategory { get; set; }
    }
}
