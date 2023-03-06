using System;
using System.Text;

namespace Common.Communication
{
    public abstract class DeviceData
    {
        public bool IsActived { get; set; }
        public bool InitOk { get; set; } = false;
        public int Address { get; }
        public bool ReadOnly { get; } = true;
        public int ReadInterval { get; set; }
        public int Size { get; }
        public byte[] MainBuffer { get; }
        public byte[] WriteCache { get; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int BufferSize { get; }

        public DeviceData(int address, int dataSize, int dataByte, bool readOnly, int readInterval = 200, bool isActived = true)
        {
            this.Address = address;
            this.Size = dataSize;
            this.ReadOnly = readOnly;
            this.ReadInterval = readInterval;
            this.IsActived = isActived;

            BufferSize = this.Size * dataByte;
            this.MainBuffer = new byte[BufferSize];
            this.WriteCache = new byte[BufferSize];
        }

        public abstract void ToBuffer();

        public abstract void ToEntity();
    }
}
