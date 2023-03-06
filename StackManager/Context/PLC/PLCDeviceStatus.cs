using Common.Communication;

namespace StackManager.Context.PLC
{
    class PLCDeviceInfo
    {
        public ushort Status { get; set; }
        public ushort IsActivity { get; set; }
        public ushort CycleTime { get; set; }
        public ushort Alarm { get; set; }
    }

    class PLCDeviceStatus : DeviceData
    {
        public PLCDeviceInfo[] Infos { get; private set; }

        public PLCDeviceStatus(int address, int dataCount,  int dataSize) : base(address, dataSize, 2, true, 1000)
        {
            Infos = new PLCDeviceInfo[dataCount];
            for (int i = 0; i < Infos.Length; i++)
            {
                Infos[i] = new PLCDeviceInfo();
            }
        }

        public override void ToBuffer()
        {
            var pos = 0;
            for (int i = 0; i < Infos.Length; i++)
            {
                var item = Infos[i];
                ByteUtil.SetUShortAt(MainBuffer, pos, item.Status);
                pos += 2;
                ByteUtil.SetUShortAt(MainBuffer, pos, item.IsActivity);
                pos += 2;
                ByteUtil.SetUShortAt(MainBuffer, pos, item.CycleTime);
                pos += 2;
                ByteUtil.SetUShortAt(MainBuffer, pos, item.Alarm);
                pos += 2;
            }
        }

        public override void ToEntity()
        {
            var pos = 0;
            for (int i = 0; i < Infos.Length; i++)
            {
                var item = Infos[i];
                item.Status = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
                item.IsActivity = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
                item.CycleTime = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
                item.Alarm = ByteUtil.GetUShortAt(MainBuffer, pos);
                pos += 2;
            }
        }
    }

    class FlowlineDevices : PLCDeviceStatus
    {
        public FlowlineDevices() : base((int)DataAddress.FlowlineDevices, 6, 24)
        {
        }
    }

    class StackingDevices : PLCDeviceStatus
    {
        public StackingDevices() : base((int)DataAddress.StackingDevices, 11, 44)
        {
        }
    }
}
