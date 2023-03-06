using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Communication.TCP
{
    public class TCPServer
    {
        private readonly Socket server;

        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 2000;
        public int ServerBacklog { get; set; } = 100;
        public Action<SocketContext> ReceivedCallback { get; set; }

        ManualResetEvent acceptedSignal = null;
        CancellationTokenSource cts = null;

        public TCPServer()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Start()
        {
            bool result = false;

            acceptedSignal = new ManualResetEvent(false);
            cts = new CancellationTokenSource();

            var endPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
            try
            {
                server.Bind(endPoint);
                server.Listen(ServerBacklog);
                Task.Run(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        acceptedSignal.Reset();
                        server.BeginAccept(new AsyncCallback(AcceptedCallbackAsync), server);
                        acceptedSignal.WaitOne();
                    }
                }, cts.Token);

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public void Stop()
        {
            cts.Cancel();

            try
            {
                server.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                server.Close();
            }
        }

        private void AcceptedCallbackAsync(IAsyncResult ar)
        {
            try
            {
                var sock = ((Socket)ar.AsyncState).EndAccept(ar);

                var sc = new SocketContext
                {
                    Socket = sock,
                    SockStatus = SockStatus.Contected
                };

                ReceivedCallback?.Invoke(sc);
                sock.BeginReceive(sc.Buffer, 0, sc.BufferMaxSize, 0, new AsyncCallback(ReceivedCallbackAsync), sc);
                acceptedSignal.Set();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                cts.Cancel();
                var sc = new SocketContext
                {
                    Socket = (Socket)ar.AsyncState,
                    SockStatus = SockStatus.Disconnected
                };
                ReceivedCallback?.Invoke(sc);
            }
        }

        private void ReceivedCallbackAsync(IAsyncResult ar)
        {
            var sc = (SocketContext)ar.AsyncState;
            var sock = sc.Socket;

            int byteRead = 0;
            try
            {
                byteRead = sc.Socket.EndReceive(ar);
                if (byteRead > 0)
                {
                    sc.SockStatus = SockStatus.DataReceived;
                    sc.BufferLength = byteRead;
                    ReceivedCallback?.Invoke(sc);
                    sc.Socket.BeginReceive(sc.Buffer, 0, sc.BufferMaxSize, 0, new AsyncCallback(ReceivedCallbackAsync), sc);
                }
                else
                {
                    sc.SockStatus = SockStatus.Disconnected;
                    ReceivedCallback?.Invoke(sc);
                }
            }
            catch
            {
                sc.SockStatus = SockStatus.Disconnected;
                ReceivedCallback?.Invoke(sc);
            }
        }

        private void SentCallbackAsync(IAsyncResult ar)
        {
            var state = new SocketContext
            {
                Socket = (Socket)ar.AsyncState
            };

            try
            {
                int byteSent = state.Socket.EndSend(ar);
            }
            catch (Exception)
            {
                state.SockStatus = SockStatus.Disconnected;
                ReceivedCallback?.Invoke(state);
            }
        }

        public void Send(SocketContext state)
        {
            try
            {
                state.Socket.BeginSend(state.Buffer, 0, state.BufferLength, 0, new AsyncCallback(SentCallbackAsync), state.Socket);
            }
            catch
            {
                state.SockStatus = SockStatus.Disconnected;
                ReceivedCallback?.Invoke(state);
            }
        }
    }
}
