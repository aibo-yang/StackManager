using Common.Communication;

namespace StackManager.Context.PLC
{
    public class FlowlineResponse : DeviceData
    {
        public ushort[] ScanResults { get; set; } = new ushort[8];

        public FlowlineResponse() : base((int)DataAddress.FlowlineResponse, 8, 2, false, 500)
        {
        }

        public override void ToBuffer()
        {
            var pos = 0;
            for (int i = 0; i < ScanResults.Length; i++)
            {
                ByteUtil.SetUShortAt(MainBuffer, pos, ScanResults[i]);
                pos += 2;
            }
        }

        public override void ToEntity()
        {
            var pos = 0;
            for (int i = 0; i < ScanResults.Length; i++)
            {
                ScanResults[i] = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
            }
        }
    }
}