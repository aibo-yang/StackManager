using System.ComponentModel.DataAnnotations;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 产品类别
    /// </summary>
    class ProductCategory : IEntity
    {
        //0719
        [Display(Name = "序号")]
        public int Index { get; set; }

        [Display(Name = "产品名称"), MaxLength(200)]
        public string Name { get; set; }

        [Display(Name = "箱子料号"), MaxLength(200)]
        public string BoxCode { get; set; }

        [Display(Name = "PLC编码")]
        public ushort PLCCode { get; set; }

        [Display(Name = "生产速率")]
        public int Rate { get; set; }

        [Display(Name = "每箱个数")]
        public int BoxProductCount { get; set; } // 模拟用

        [Display(Name = "栈板箱数")]
        public int PalletBoxCount { get; set; }

        [Display(Name = "箱子行数")]
        public int BoxRow { get; set; }

        [Display(Name = "箱子列数")]
        public int BoxCol { get; set; }

        [Display(Name = "箱子高度")]
        public int BoxHeight { get; set; }

        [Display(Name = "箱子宽度")]
        public int BoxWidth { get; set; }

        [Display(Name = "箱子长度")]
        public int BoxLength { get; set; }

        [Display(Name = "栈板类型")]
        public ushort PalletType { get; set; }

        [Display(Name = "摆放方式")]//1横摆 2竖摆
        public ushort LayoutType { get; set; }

        [Display(Name = "是否盖板")]//1不盖板 2盖板
        public ushort BoxBoard { get; set; }

        [Display(Name = "产品缓存类型")] //1正常 2缓存（奇数）3缓存（偶数）
        public ushort StackType { get; set; }

        [Display(Name = "产品缓存区")]//1缓存区一 2缓存区二 3无缓存区
        public ushort CacheRegion { get; set; }
    }
}
