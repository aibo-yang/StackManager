using System.Collections.Generic;
using Common.Communication;

namespace StackManager.Context.PLC
{
    public class StackingRequest : DeviceData
    {
        public bool[] NewPalletRequests { get; set; } = new bool[6];
        public bool[] InitPalletRequests { get; set; } = new bool[6];
        public ushort StowStartRequest { get; set; }
        public ushort StowEndRequest { get; set; }

        public StackingRequest() : base((int)DataAddress.StackingRequest, 4, 2, true, 500)
        {
        }

        public override void ToBuffer()
        {
        }

        public override void ToEntity()
        {
            var pos = 0;
            var newPallets = ByteUtil.GetShortAt(MainBuffer, pos);
            pos += 2;
            var initPallets = ByteUtil.GetShortAt(MainBuffer, pos);
            pos += 2;
            StowStartRequest = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowEndRequest = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;

            for (int i = 0; i < NewPalletRequests.Length; i++)
            {
                NewPalletRequests[i] = ByteUtil.GetBitAt(newPallets, i);
                InitPalletRequests[i] = ByteUtil.GetBitAt(initPallets, i);
            }
        }
    }
}