using System;
using System.Text;

namespace Common.Communication
{
    public static class ByteUtil
    {
        public static byte GetHi(int value)
        {
            return GetByteAt(value, 1);
        }

        public static byte GetHi(uint value)
        {
            return GetByteAt(value, 1);
        }

        public static byte GetLo(int value)
        {
            return GetByteAt(value, 0);
        }

        public static byte GetLo(uint value)
        {
            return GetByteAt(value, 0);
        }

        public static int GetInt(byte hi, byte lo)
        {
            return (hi<<8) + lo;
        }

        #region GetByte
        public static byte GetByteAt(long value, int index)
        {
            return (byte)((value >> (index * 8)) & 0xff);
        }

        public static byte GetByteAt(ulong value, int index)
        {
            return (byte)((value >> (index * 8)) & 0xff);
        }

        public static byte GetByteAt(int value, int index)
        {
            return (byte)((value >> (index * 8)) & 0xff);
        }

        public static byte GetByteAt(uint value, int index)
        {
            return (byte)((value >> (index * 8)) & 0xff);
        }
        #endregion


        #region Byte Buffer
        public static string ToHexString(byte[] buffer)
        {
            var sb = new StringBuilder();
            foreach (var item in buffer)
            {
                sb.Append(string.Format("{0:X2} ", item));
            }
            return sb.ToString();
        }

        public static bool GetBitAt(int source, int pos)
        {
            return ((source >> pos) & 0x01) == 0x01;
        }

        public static int SetBitAt(int self, int pos, bool value)
        {
            if (value)
            {
                self |= (1 << pos);
            }
            else
            {
                self &= ~(1 << pos);
            }
            return self;
        }

        public static byte GetByteAt(byte[] buffer, int pos)
        {
            return buffer[pos];
        }

        public static void SetByteAt(byte[] buffer, int pos, byte value)
        {
            buffer[pos] = value;
        }

        public static ushort GetUShortAt(byte[] buffer, int pos)
        {
            return (ushort)((buffer[pos] << 8) | buffer[pos + 1]);
        }

        public static void SetUShortAt(byte[] buffer, int pos, ushort value)
        {
            buffer[pos] = (byte)(value >> 8);
            buffer[pos + 1] = (byte)(value & 0x00FF);
        }

        public static short GetShortAt(byte[] buffer, int pos)
        {
            return (short)((buffer[pos] << 8) | buffer[pos + 1]);
        }

        public static void SetShortAt(byte[] buffer, int pos, short value)
        {
            buffer[pos] = (byte)(value >> 8);
            buffer[pos + 1] = (byte)(value & 0x00FF);
        }

        public static int GetIntAt(byte[] buffer, int pos)
        {
            int result;
            result = buffer[pos];
            result <<= 8;
            result |= buffer[pos + 1];
            result <<= 8;
            result |= buffer[pos + 2];
            result <<= 8;
            result |= buffer[pos + 3];
            return result;
        }

        public static void SetIntAt(byte[] buffer, int pos, int value)
        {
            buffer[pos + 3] = (byte)(value & 0xFF);
            buffer[pos + 2] = (byte)((value >> 8) & 0xFF);
            buffer[pos + 1] = (byte)((value >> 16) & 0xFF);
            buffer[pos] = (byte)((value >> 24) & 0xFF);
        }

        public static uint GetUIntAt(byte[] buffer, int pos)
        {
            uint result;
            result = buffer[pos];
            result <<= 8;
            result |= buffer[pos + 1];
            result <<= 8;
            result |= buffer[pos + 2];
            result <<= 8;
            result |= buffer[pos + 3];
            return result;
        }

        public static void SetUIntAt(byte[] buffer, int pos, uint value)
        {
            buffer[pos + 3] = (byte)(value & 0xFF);
            buffer[pos + 2] = (byte)((value >> 8) & 0xFF);
            buffer[pos + 1] = (byte)((value >> 16) & 0xFF);
            buffer[pos] = (byte)((value >> 24) & 0xFF);
        }

        public static float GetRealAt(byte[] buffer, int pos)
        {
            UInt32 value = GetUIntAt(buffer, pos);
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static void SetRealAt(byte[] buffer, int pos, Single value)
        {
            byte[] floatArray = BitConverter.GetBytes(value);
            buffer[pos] = floatArray[3];
            buffer[pos + 1] = floatArray[2];
            buffer[pos + 2] = floatArray[1];
            buffer[pos + 3] = floatArray[0];
        }

        public static string GetSiemensStringAt(byte[] buffer, int pos)
        {
            int size = (int)((buffer[pos + 2] << 8) + buffer[pos + 3]);
            return Encoding.BigEndianUnicode.GetString(buffer, pos + 4, size * 2);
        }

        public static void SetSiemensStringAt(byte[] buffer, int pos, int maxLen, string value)
        {
            int size = value.Length;
            buffer[pos] = (byte)(maxLen >> 8);
            buffer[pos + 1] = (byte)maxLen;
            buffer[pos + 2] = (byte)(size >> 8);
            buffer[pos + 3] = (byte)size;
            Encoding.BigEndianUnicode.GetBytes(value, 0, size, buffer, pos + 4);
        }

        public static byte[] GetAsciiBytesFromString(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        public static string GetStringFromAsciiBytes(byte[] value, int index, int count)
        {
            return Encoding.ASCII.GetString(value, index, count);
        }
        #endregion
    }
}
