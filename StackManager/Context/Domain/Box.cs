using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 箱子
    /// </summary>
    class Box : IEntity
    {
        [Display(Name = "箱子编码"), MaxLength(200)]
        public string Code { get; set; }

        [Display(Name = "箱子状态")]
        public BoxStatus Status { get; set; }

        [Display(Name = "是否满箱")]
        public bool BoxIsFull { get; set; } = false;

        [Display(Name = "是否满栈板")]
        public bool PalletIsFull { get; set; } = false;

        [Display(Name = "栈板编号"), MaxLength(200)]
        public string PalletNo { get; set; }

        [Display(Name = "订单编号"), MaxLength(200)]
        public string OrderNo { get; set; }

        [Display(Name = "当前箱子产品数量")]
        public int ProdcutCount { get; set; }

        [Display(Name = "当前栈板箱子数量")]
        public int BoxCount { get; set; }

        [Display(Name = "产品类型")]
        public ProductCategory ProductCategory { get; set; }

        [Display(Name = "所属栈板")]
        public Pallet Pallet { get; set; }

        [Display(Name = "所属产线")]
        public Flowline Flowline { get; set; }
    }

    enum BoxStatus : int
    {
        [Display(Name = "缓存入站")]
        Caching,
        [Display(Name = "缓存出站")]
        Caching_Leave,

        [Display(Name = "准备码垛")]
        Stacking,   

        [Display(Name = "码垛完成")]
        Stacked,

        [Display(Name = "进缓存不码垛")]
        NoStackCached,

        [Display(Name = "进缓存码垛")]
        StackCached,

        [Display(Name = "箱子异常")]
        Exception = 10,
    }
}
