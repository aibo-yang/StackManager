using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 栈板
    /// </summary>
    class Pallet : IEntity
    {
        [Display(Name = "栈板名称"), MaxLength(200)]
        public string Name { get; set; }

        [Display(Name = "栈板序号")]
        public int Index { get; set; }

        [Display(Name = "栈板编码"), MaxLength(200)]
        public string Code { get; set; }

        [Display(Name = "栈板状态")]
        public PalletStatus Status { get; set; }

        [Display(Name = "箱子数量")]
        public int BoxCount { get; set; }

        [Display(Name = "所有箱子")]
        public ICollection<Box> Boxes { get; set; } = new List<Box>();

        [Display(Name = "所属产线")]
        public Flowline Flowline { get; set; }
    }

    enum PalletStatus : int
    {
        Stacking,
        Stacked_OK,
        Stacked_NG,
    }
}
