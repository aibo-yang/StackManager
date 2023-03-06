using Common.Communication;


namespace StackManager.Context.PLC
{

    class PLCPalletCylinderInfo
    {
        public ushort PositioningCylinder1 { get; set; }
        public ushort PositioningCylinder2 { get; set; }
        public ushort PushCylinder1 { get; set; }
        public ushort PushCylinder2 { get; set; }
    }

    class PLCRollerCylinderInfo 
    {
        public ushort RollerCylinder1 { get; set; } 
        public ushort RollerCylinder2 { get; set; }
        public ushort DGMotorCylinder { get; set; } 
    }

    class PalletCylinderStatus : DeviceData
    {
        public PLCPalletCylinderInfo[] Infos { get; private set; }
        public PLCRollerCylinderInfo Roller { get; private set; }

        public PalletCylinderStatus(int address, int dataCount, int dataSize) : base(address, dataSize, 2, true, 1000)
        {
            Infos = new PLCPalletCylinderInfo[dataCount-1];
            for (int i = 0; i < Infos.Length; i++)
            {
                Infos[i] = new PLCPalletCylinderInfo();
            }

            Roller = new PLCRollerCylinderInfo();
        }

        public override void ToBuffer()
        {
            var pos = 0;
            for (int i = 0; i < Infos.Length; i++)
            {
                var item = Infos[i];
                ByteUtil.SetUShortAt(MainBuffer, pos, item.PositioningCylinder1);
                pos += 2;
                ByteUtil.SetUShortAt(MainBuffer, pos, item.PositioningCylinder2);
                pos += 2;
                ByteUtil.SetUShortAt(MainBuffer, pos, item.PushCylinder1);
                pos += 2;
                ByteUtil.SetUShortAt(MainBuffer, pos, item.PushCylinder2);
                pos += 2;
            }
        }

        public override void ToEntity()
        {
            var pos = 0;
            for (int i = 0; i < Infos.Length; i++)
            {
                var item1 = Infos[i];
                item1.PositioningCylinder1 = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
                item1.PositioningCylinder2 = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
                item1.PushCylinder1 = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
                item1.PushCylinder2 = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
            }

            var item2 = Roller;
            item2.RollerCylinder1 = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            item2.RollerCylinder2 = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
            item2.DGMotorCylinder = ByteUtil.GetUShortAt(MainBuffer, pos);
            pos += 2;
        }
    }

    class PalletCylinders : PalletCylinderStatus
    {
        public PalletCylinders() : base((int)DataAddress.PalletCylinders, 14, 55)
        {
        }
    }

}