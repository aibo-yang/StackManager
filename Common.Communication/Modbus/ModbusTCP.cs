using System;
using System.Collections.Generic;
using System.Linq;
using Common.Communication.TCP;

namespace Common.Communication.Modbus
{
    internal enum DataType : byte
    {
        // 读线圈
        CoilStatus = 1,
        // 读输入线圈
        InputStatus = 2,
        // 读保持寄存器
        HoldRegister = 3,
        // 读输入寄存器
        InputRegister = 4,
        // 写单个线圈
        WriteSingleCoil = 5,
        // 写单个寄存器
        WriteSingleRegister = 6,
        // 写多个线圈
        WriteMultiCoil = 15,
        // 写多个寄存器
        WriteMultiRegister = 16,
    }

    public class ModbusTCP : TCPClient
    {
        private uint transactionId = 0;
        public byte DeviceAddress { get; private set; }

        public ModbusTCP(string serverIp, int serverPort = 502, byte deviceAddress = 0 ) : base(serverIp, serverPort)
        {
            DeviceAddress = deviceAddress;
        }

        public int RegisterRead(int addr, int registerNumber, byte[] buffer)
        {
            if (buffer == null || buffer.Length < registerNumber * 2)
            {
                return (int)ResultCode.ArgumentError;
            }

            transactionId++;

            var cmd = new byte[]
            {
                ByteUtil.GetHi(transactionId),    // hi byte of transaction id
                ByteUtil.GetLo(transactionId),    // lo byte of transaction id
                0,                                  // protocol id
                0,                                  // protocol id
                0,                                  // hi byte of data length = 0 max 256
                6,                                  // lo byte of data length
                DeviceAddress,                      // slave address
                (byte)DataType.HoldRegister,        // function code
                ByteUtil.GetHi(addr),             // hi byte of register address
                ByteUtil.GetLo(addr),             // lo byte of register address
                ByteUtil.GetHi(registerNumber),   // hi byte of register no.
                ByteUtil.GetLo(registerNumber),   // lo byte of register no.
            };

            var receivedBuffer = new byte[520];

            var rc = SendWaitResponse(cmd, receivedBuffer, 8 + 1 + registerNumber * 2);
            if (rc != (int)ResultCode.Succeed)
            {
                return rc;
            }

            Array.Copy(receivedBuffer, 9, buffer, 0, registerNumber*2);

            if (receivedBuffer[7] != (byte)DataType.HoldRegister)
            {
                return (int)ResultCode.ReadFailed;
            }

            return (int)ResultCode.Succeed;
        }

        public int RegisterWrite(int address, int registerNumber, byte[] buffer)
        {
            if (buffer == null || buffer.Length < registerNumber * 2)
            { 
                return (int)ResultCode.ArgumentError;
            }

            transactionId++;

            var cmd = new byte[]
            {
                ByteUtil.GetHi(transactionId),       // hi byte of transaction id
                ByteUtil.GetLo(transactionId),       // lo byte of transaction id
                0,                                     // protocol id
                0,                                     // protocol id
                0,                                     // hi byte of data length = 0 max 256
                0,                                     // lo byte of data length
                DeviceAddress,                         // slave address
                (byte)DataType.WriteMultiRegister,     // function code
                ByteUtil.GetHi(address),             // hi byte of data address
                ByteUtil.GetLo(address),             // lo byte of data address
                ByteUtil.GetHi(registerNumber),      // hi byte of write register no.
                ByteUtil.GetLo(registerNumber),      // lo byte of write register no.
                ByteUtil.GetLo(registerNumber * 2),  // write byte length
            };
            cmd[5] = (byte)(7 + registerNumber * 2);

            var bufferList = new List<byte>(cmd);
            bufferList.AddRange(buffer.Take(registerNumber * 2));

            var receivedBuffer = new byte[12];

            var rc = SendWaitResponse(bufferList.ToArray(), receivedBuffer, receivedBuffer.Length);
            if (rc != (int)ResultCode.Succeed)
            {
                return rc;
            }

            if (receivedBuffer[7] != (byte)DataType.WriteMultiRegister)
            {
                return (int)ResultCode.WriteFailed;
            }

            return (int)ResultCode.Succeed;
        }
    }
}
