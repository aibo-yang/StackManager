using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StackManager.Context.Domain
{
    class SlaveDevice : IEntity
    {
       [Display(Name ="从设备编码"),MaxLength(200)]
       public string SlaveCode { get; set; }
        
       [Display(Name ="主设备编码")]
       public string MasterCode { get; set; }
    }
}
