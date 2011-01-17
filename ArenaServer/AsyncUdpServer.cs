using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MHPTunnelG;

namespace ArenaServer
{
    class AsyncUdpServer<Type> : IServer<Type>
        where Type : IClientState
    {
        UdpClient listeningClient;
        IServerHandler<Type> handler;

        AsyncCallback AsyncReadCallback;
        AsyncCallback AsyncSendCallback;

        Dictionary<IPEndPoint, UdpConnection> establishedConnections = new Dictionary<IPEndPoint, UdpConnection>();

        public AsyncUdpServer()
        {
            AsyncReadCallback = new AsyncCallback(ReadCallback);
        }

        public void StartListening(IPEndPoint localEP, IServerHandler<Type> handler)
        {
            // stop if started
            StopListening();

            this.handler = handler;

            listeningClient = new UdpClient(localEP);
            Console.WriteLine("UDP : Listening on {0}", localEP);

            ReadNext();
        }

        public void StopListening()
        {
            if (listeningClient != null)
            {
                listeningClient.Close();
                listeningClient = null;
            }
            handler = null;
            establishedConnections.Clear();
        }

        void ReadNext()
        {
            Console.WriteLine("UDP ReadNext");
            while (listeningClient != null)
                try
                {
                    listeningClient.BeginReceive(AsyncReadCallback, null);
                    break;
                }
                catch (NullReferenceException) { }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            Console.WriteLine("UDP ReadCallback");
            try
            {
                IPEndPoint remoteEP = null;
                byte[] bytes = listeningClient.EndReceive(ar, ref remoteEP);

                Console.WriteLine("Read {0} bytes from {1}", bytes.Length, remoteEP);

                UdpConnection conn;
                if (establishedConnections.ContainsKey(remoteEP))
                {
                    Console.WriteLine("UDP find established connection");
                    conn = establishedConnections[remoteEP];
                }
                else
                {
                    Console.WriteLine("UDP established connection not found");
                    conn = new UdpConnection(this, remoteEP);
                    conn.state = handler.CreateState(conn);

                    establishedConnections[remoteEP] = conn;
                }

                PacketData data = new PacketData(bytes, bytes.Length);
                bool sessionContinue = handler.ProcessIncomingData(conn.state, data);

                if (!sessionContinue)
                {
                    //establishedConnections.Remove(remoteEP);
                }
            }
            catch (NullReferenceException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            ReadNext();
        }

        public void Send(IPEndPoint remoteEP, string message)
        {
            if (listeningClient == null) return;
            Console.WriteLine("UDP Send: {0}", message);

            byte[] data = Encoding.UTF8.GetBytes(message);

            listeningClient.BeginSend(
                          data,
                          data.Length,
                          remoteEP,
                          AsyncSendCallback,
                          remoteEP);
        }

        public void Send(IPEndPoint remoteEP, byte[] bytes, int count)
        {
            if (listeningClient == null) return;
            Console.WriteLine("UDP Send: Binary length={0}", count);

            listeningClient.BeginSend(
                          bytes,
                          count,
                          remoteEP,
                          AsyncSendCallback,
                          remoteEP);
        }

        void SendCallback(IAsyncResult ar)
        {
            Console.WriteLine("UDP SendCallback");
            try
            {
                IPEndPoint remoteEP = (IPEndPoint)ar.AsyncState;
                int bytesSent = listeningClient.EndSend(ar);
                Console.WriteLine("Sent to [{1}] {0}", bytesSent, remoteEP);

                //listeningClient.BeginReceive(new AsyncCallback(ReadCallback), remoteEP);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        class UdpConnection : IServerConnection
        {
            AsyncUdpServer<Type> server;
            public Type state;

            AsyncCallback AsyncSendCallback;

            IPEndPoint _remoteEndPoint;
            public IPEndPoint RemoteEndPoint { get { return _remoteEndPoint; } }


            public UdpConnection(AsyncUdpServer<Type> server, IPEndPoint remoteEP)
            {
                this.server = server;
                this._remoteEndPoint = remoteEP;

                AsyncSendCallback = new AsyncCallback(SendCallback);
            }

            public void Send(string message)
            {
                if (server.listeningClient == null) return;
                Console.WriteLine("UDP Send: {0}", message);

                byte[] data = Encoding.UTF8.GetBytes(message);

                server.listeningClient.BeginSend(
                              data,
                              data.Length,
                              _remoteEndPoint,
                              AsyncSendCallback,
                              null);
            }

            public void Send(byte[] bytes, int count)
            {
                if (server.listeningClient == null) return;
                Console.WriteLine("UDP Send: Binary length={0}", count);

                server.listeningClient.BeginSend(
                              bytes,
                              count,
                              _remoteEndPoint,
                              AsyncSendCallback,
                              null);
            }

            void SendCallback(IAsyncResult ar)
            {
                Console.WriteLine("UDP SendCallback");
                try
                {
                    int bytesSent = server.listeningClient.EndSend(ar);
                    Console.WriteLine("Sent to [{1}] {0}", bytesSent, _remoteEndPoint);

                    //listeningClient.BeginReceive(new AsyncCallback(ReadCallback), remoteEP);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
