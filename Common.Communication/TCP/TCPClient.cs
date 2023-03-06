using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Common.Communication.TCP
{
    public class TCPClient
    {
        private Socket client;
        private readonly AutoResetEvent responseEvent = new (false);
        private readonly AutoResetEvent sentEvent = new (false);
        private bool connectPending = false;

        private byte[] responseBuffer;
        private readonly List<byte> responseBufferList = new List<byte>();
        private int responseByteLength = 0;

        public string ServerIp { get; private set; }
        public int ServerPort { get; private set; }
        public bool Connected { get; private set; } = false;
        public Action<SocketContext> ReceivedHandler { get; set; }

        #region 检查连接是否有效
        //public bool SocketConnected(Socket s)
        //{
        //    // Exit if socket is null
        //    if (s == null)
        //    {
        //        return false;
        //    }

        //    bool part1 = s.Poll(1000, SelectMode.SelectRead);
        //    bool part2 = (s.Available == 0);
        //    if (part1 && part2)
        //    { 
        //        return false;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            int sentBytesCount = s.Send(new byte[1], 1, 0);
        //            return sentBytesCount == 1;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}

        //private bool conncted = false;
        //public bool Conneted 
        //{
        //    get
        //    {
        //        bool ret;
        //        try
        //        {
        //            ret = !(client.Poll(1000, SelectMode.SelectRead) && client.Available == 0);
        //        }
        //        catch (SocketException)
        //        {
        //            ret = false;
        //        }
        //        return ret && conncted;
        //    }
        //    set
        //    {
        //        conncted = value;
        //    }
        //}
        #endregion

        public TCPClient(string serverIp, int serverPort)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
        }

        public TCPClient(string serverIp, int serverPort, Action<SocketContext> receivedHandler)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            ReceivedHandler = receivedHandler;
        }

        public int Connect(bool isWaitForFinished = true)
        {
            if (connectPending)
            {
                return (int)ResultCode.Pending;
            }

            var rc = (int)ResultCode.Succeed;

            try
            {
                connectPending = true;
                if (isWaitForFinished)
                {
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(IPAddress.Parse(ServerIp), ServerPort);
                    var sc = new SocketContext
                    {
                        Address = ServerIp,
                        Port = ServerPort,
                        Socket = client,
                        SockStatus = SockStatus.Contected
                    };
                    ReceivedHandler?.Invoke(sc);
                    client.BeginReceive(sc.Buffer, 0, sc.BufferMaxSize, 0, new AsyncCallback(ReceivedCallbackAsync), sc);
                    Connected = true;
                }
                else
                {
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.BeginConnect(IPAddress.Parse(ServerIp), ServerPort, new AsyncCallback(ConnetedCallbackAsync), client);
                }
            }
            catch (Exception ex)
            {
                SocketError(ex.Message);
                rc = (int)ResultCode.NotConnected;
            }
            finally
            {
                connectPending = false;
            }

            return rc;
        }

        public void Disconnect()
        {
            Connected = false;

            try
            {
                //client?.Shutdown(SocketShutdown.Both);
                client?.Disconnect(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                client?.Close();
            }
        }

        private int Send(SocketContext sc)
        {
            if (!Connected)
            {
                return (int)ResultCode.NotConnected;
            }

            try
            {
                sc.Socket = client;
                sc.Socket.BeginSend(sc.Buffer, 0, sc.BufferLength, 0, new AsyncCallback(SentCallbackAsync), sc);
            }
            catch (Exception ex)
            {
                SocketError($"{nameof(Send)}:{ex.Message}");
                return (int)ResultCode.WriteFailed;
            }

            return (int)ResultCode.Succeed;
        }

        public int Send(byte[] requestdBuffer, bool isWaitFinished = true, TimeSpan? timeout = null)
        {
            lock (sentEvent)
            {
                var sc = new SocketContext(requestdBuffer.Length)
                {
                    Socket = client,
                    BufferLength = requestdBuffer.Length,
                };
                Array.Copy(requestdBuffer, sc.Buffer, requestdBuffer.Length);

                responseBuffer = null;
                responseByteLength = 0;

                var rc = Send(sc);
                if (rc != (int)ResultCode.Succeed)
                {
                    return rc;
                }

                if (!isWaitFinished)
                {
                    return (int)ResultCode.Succeed;
                }

                if (!sentEvent.WaitOne(timeout ?? TimeSpan.FromSeconds(2)))
                {
                    return (int)ResultCode.Timeout;
                }

                return (int)ResultCode.Succeed;
            }
        }

        public int SendWaitResponse(byte[] requestdBuffer, byte[] responseBuffer, int responseByteLength, TimeSpan? timeout = null)
        {
            lock (sentEvent)
            {
                var sc = new SocketContext(requestdBuffer.Length)
                {
                    Socket = client,
                    BufferLength = requestdBuffer.Length,
                };
                Array.Copy(requestdBuffer, sc.Buffer, requestdBuffer.Length);

                this.responseBuffer = responseBuffer;
                this.responseByteLength = responseByteLength;
                responseBufferList.Clear();

                var rc = Send(sc);
                if (rc != (int)ResultCode.Succeed)
                {
                    return rc;
                }

                if (!sentEvent.WaitOne(TimeSpan.FromSeconds(2)))
                {
                    return (int)ResultCode.Timeout;
                }

                if (!responseEvent.WaitOne(timeout?? TimeSpan.FromSeconds(2)))
                {
                    return (int)ResultCode.Timeout;
                }

                return (int)ResultCode.Succeed;
            }
        }

        private void SocketError(string errorMessage)
        {
            Debug.WriteLine(errorMessage);

            if (ReceivedHandler != null)
            {
                var sc = new SocketContext
                {
                    Address = ServerIp,
                    Port = ServerPort,
                    Socket = client,
                    SockStatus = SockStatus.Disconnected
                };
                ReceivedHandler?.Invoke(sc);
            }

            if (Connected)
            {
                Disconnect();
            }
        }

        #region Callback
        private void ConnetedCallbackAsync(IAsyncResult ar)
        {
            if (ar.AsyncState is Socket sock && sock.Connected)
            {
                try
                {
                    sock.EndConnect(ar);
                    Connected = true;

                    var sc = new SocketContext
                    {
                        Address = ServerIp,
                        Port = ServerPort,
                        Socket = sock,
                        SockStatus = SockStatus.Contected
                    };
                    
                    ReceivedHandler?.Invoke(sc);
                    sock.BeginReceive(sc.Buffer, 0, sc.BufferMaxSize, 0, new AsyncCallback(ReceivedCallbackAsync), sc);
                }
                catch (Exception ex)
                {
                    SocketError($"{nameof(ConnetedCallbackAsync)}:{ex.Message}");
                }
            }
            else
            {
                SocketError($"{nameof(ConnetedCallbackAsync)}:connect failed");
            }
        }

        private void ReceivedCallbackAsync(IAsyncResult ar)
        {
            if (ar.AsyncState is not SocketContext sc || sc.Socket is not Socket socket)
            {
                SocketError($"{nameof(ReceivedCallbackAsync)}:type convert error.");
                return;
            }

            try
            {
                int receivedLength = socket.EndReceive(ar);
                if (receivedLength > 0)
                {
                    if (ReceivedHandler != null)
                    {
                        sc.BufferLength = receivedLength;
                        sc.SockStatus = SockStatus.DataReceived;
                        ReceivedHandler?.Invoke(sc);
                    }

                    if (responseBuffer != null)
                    {
                        responseByteLength -= receivedLength;
                        responseBufferList.AddRange(sc.Buffer.Take(receivedLength));
                        if (responseByteLength == 0)
                        {
                            Array.Copy(responseBufferList.ToArray(), responseBuffer, responseBufferList.Count);
                            responseEvent.Set();
                        }
                    }

                    sc.Socket.BeginReceive(sc.Buffer, 0, sc.BufferMaxSize, 0, new AsyncCallback(ReceivedCallbackAsync), sc);
                }
                else
                {
                    SocketError($"received data length is 0.");
                }
            }
            catch (Exception ex)
            {
                SocketError($"{nameof(ReceivedCallbackAsync)}:{ex.Message}");
            }
        }

        private void SentCallbackAsync(IAsyncResult ar)
        {
            if (ar.AsyncState is not SocketContext sc || sc.Socket is not Socket socket)
            {
                SocketError($"{nameof(ReceivedCallbackAsync)}:type convert error.");
                return;
            }

            try
            {
                var sentLength = socket.EndSend(ar);
                sc.BufferLength -= sentLength;
                if (sc.BufferLength == 0)
                {
                    if (ReceivedHandler != null)
                    {
                        sc.BufferLength = 0;
                        sc.SockStatus = SockStatus.DataSent;
                        ReceivedHandler?.Invoke(sc);
                    }
                    sentEvent.Set();
                }
                else if (sc.BufferLength < 0)
                {
                    SocketError($"{nameof(SentCallbackAsync)}:buffer length error");
                }
            }
            catch (Exception ex)
            {
                SocketError($"{nameof(SentCallbackAsync)}:{ex.Message}");
            }
        }
        #endregion
    }
}
