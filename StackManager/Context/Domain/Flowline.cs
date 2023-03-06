using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace StackManager.Context.Domain
{
    /// <summary>
    /// 产线类别
    /// </summary>
    class Flowline : IEntity
    {
        [Display(Name = "产线名称"), MaxLength(200)]
        public string Name { get; set; }

        [Display(Name = "产线序号")]
        public int Index { get; set; }

        [Display(Name = "提升机")]
        public DeviceCategory Elevator { get; set; }
        
        //[Display(Name = "产品类型")]
        //public ProductCategory ProductCategory { get; set; }

        //导航属性
        [Display(Name = "所有栈板")]
        public ICollection<Pallet> Pallets { get; set; }

        [Display(Name = "所有箱子")]
        public ICollection<Box> Boxes { get; set; }
    }
}
