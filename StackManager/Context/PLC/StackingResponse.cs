using Common.Communication;

namespace StackManager.Context.PLC
{
    class StowStartResult
    {
        public ushort Done { get; set; }
        public ushort BoxType { get; set; }
        public ushort PalletIndex { get; set; }
        public ushort BoxLayer { get; set; }
        public ushort BoxRow { get; set; }
        public ushort BoxCol { get; set; }
        public ushort PalletRemove { get; set; }
        public ushort PalletException { get; set; }
        public ushort BoxHeight { get; set; }
        public ushort BoxWidth { get; set; }
        public ushort BoxLength { get; set; }
    }

    class StackingResponse : DeviceData
    {
        public bool[] NewPalletResults { get; set; } = new bool[6];
        public bool[] InitPalletResults { get; set; } = new bool[6];
        public StowStartResult StowStartResult { get; set; } = new StowStartResult();
        public ushort StowEndResult { get; set; }

        //新增个地址位
        public ushort PalletType { get; set; }
        public ushort LayoutType { get; set; }
        public ushort BoxBoard { get; set; }
        public ushort BoardResult { get; set; } 
        public ushort StackType { get; set; }
        public ushort CacheRegion { get; set; }

        public StackingResponse() : base((int)DataAddress.StackingResponse, 14, 2, false, 500)
        {
        }

        public override void ToBuffer()
        {
            ushort newPalletResults = 0;
            for (int i = 0; i < NewPalletResults.Length; i++)
            {
                newPalletResults = (ushort)ByteUtil.SetBitAt(newPalletResults, i, NewPalletResults[i]);
            }

            ushort initPalletResults = 0;
            for (int i = 0; i < NewPalletResults.Length; i++)
            {
                initPalletResults = (ushort)ByteUtil.SetBitAt(initPalletResults, i, InitPalletResults[i]);
            }

            var pos = 0;
            ByteUtil.SetUShortAt(MainBuffer, pos, newPalletResults);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, PalletType);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, LayoutType);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, BoxBoard);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, BoardResult);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StackType);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, CacheRegion);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, initPalletResults);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.Done);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxType);
            pos += 2; 
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.PalletIndex);
            pos += 2; 
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxLayer);
            pos += 2; 
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxRow);
            pos += 2; 
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxCol);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.PalletRemove);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.PalletException);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxHeight);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxWidth);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowStartResult.BoxLength);
            pos += 2;
            ByteUtil.SetUShortAt(MainBuffer, pos, StowEndResult);
            pos += 2;
        }

        public override void ToEntity()
        {
            var pos = 0;
            var newPalletResults = ByteUtil.GetShortAt(MainBuffer, pos);
            pos += 2;
            PalletType = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            LayoutType = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            BoxBoard = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            BoardResult = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StackType = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            CacheRegion = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            var initPalletResults = ByteUtil.GetShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.Done = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxType = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.PalletIndex = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxLayer = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxRow = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxCol = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.PalletRemove = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.PalletException = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxHeight = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxWidth = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowStartResult.BoxLength = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            StowEndResult = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;

            for (int i = 0; i < NewPalletResults.Length; i++)
            {
                NewPalletResults[i] = ByteUtil.GetBitAt(newPalletResults, i);
            }

            for (int i = 0; i < InitPalletResults.Length; i++)
            {
                InitPalletResults[i] = ByteUtil.GetBitAt(initPalletResults, i);
            }
        }
    }
}