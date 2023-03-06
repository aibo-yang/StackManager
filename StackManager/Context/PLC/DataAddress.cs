using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace StackManager.Context.PLC
{
    public enum DeviceId : int
    {
        [Display(Name = "流线区")]
        FlowlinePLC = 0,
        [Display(Name = "码垛区")]
        StackingPLC,
    }

    public class DataAddress
    {
        public const int FlowlineRequest = 8300;
        public const int StackingRequest = 8308;
        
        public const int FlowlineResponse = 8330;
        public const int StackingResponse = 8338;

        public const int FlowlineDevices = 8500;
        public const int StackingDevices = 8532;

        public const int PalletCylinders = 9200;
    }
}
