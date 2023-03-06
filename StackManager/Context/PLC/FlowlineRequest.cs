using System.Collections.Generic;
using Common.Communication;

namespace StackManager.Context.PLC
{
    class FlowlineRequest : DeviceData
    {
        public ushort[] ScanRequests { get; set; } = new ushort[8];

        public FlowlineRequest() : base((int)DataAddress.FlowlineRequest, 8, 2, true, 500)
        {
        }

        public override void ToBuffer()
        {
        }

        public override void ToEntity()
        {
            var pos = 0;
            for (int i = 0; i < ScanRequests.Length; i++)
            {
                ScanRequests[i] = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
            }
        }
    }
}