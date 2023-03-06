using System.Net.Sockets;

namespace Common.Communication
{
    public enum SockStatus
    {
        None = 0,
        Contected,
        Disconnected,
        DataSent,
        DataReceived,
    }

    public class SocketContext
    {
        public string Address { get; set; }
        public int Port { get; set; }

        public Socket Socket { get; set; }
        public SockStatus SockStatus { get; set; }
        
        public byte[] Buffer { get; }
        public int BufferMaxSize { get; }
        public int BufferLength { get; set; } = 0;

        public object Reserved { get; set; }

        public override string ToString()
        {
            return $"{Address}:{Port}";
        }

        public SocketContext()
        {
            BufferMaxSize = 255;
            Buffer = new byte[BufferMaxSize];
        }

        public SocketContext(int bufferMaxSize)
        {
            BufferMaxSize = bufferMaxSize;
            Buffer = new byte[BufferMaxSize];
        }
    }
}
