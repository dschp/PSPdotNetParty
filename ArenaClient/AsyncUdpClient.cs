using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MHPTunnelG;

namespace ArenaClient
{
    class AsyncUdpClient : IAsyncClient
    {
        IAsyncClientHandler handler = null;

        UdpClient client = null;

        int _localPort = 0;
        public int LocalPort { set { _localPort = value; } }

        IPEndPoint remoteEndPoint;

        public AsyncUdpClient(IAsyncClientHandler handler)
        {
            this.handler = handler;
        }

        public void Connect(string ipAddr, int port)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ipAddr), port);
            Connect(remoteEP);
        }

        public bool Connected { get { return client != null; } }

        public void Connect(IPEndPoint remoteEP)
        {
            if (client != null) Disconnect();

            try
            {
                client = new UdpClient(_localPort);
                //client.Connect(remoteEP);
                remoteEndPoint = remoteEP;
                handler.ConnectCallback((IPEndPoint)client.Client.LocalEndPoint);

                ReadNext();
            }
            catch (Exception ex)
            {
                handler.Log(ex.ToString());
            }
        }

        public void Disconnect()
        {
            if (client != null)
            {
                try
                {
                    client.Close();
                }
                catch (Exception) { }

                client = null;
                handler.DisconnectCallback();
            }
        }

        void ReadNext()
        {
            if (client == null) return;

            try
            {
                client.BeginReceive(new AsyncCallback(AsyncReadCallback), null);
            }
            catch (Exception ex)
            {
                handler.Log(ex.ToString());
            }
        }

        void AsyncReadCallback(IAsyncResult ar)
        {
            try
            {
                IPEndPoint remoteEP = null;
                Byte[] bytes = client.EndReceive(ar, ref remoteEP);
                PacketData data = new PacketData(bytes, bytes.Length);

                handler.ReadCallback(data);

                ReadNext();
            }
            catch (NullReferenceException)
            {
                Disconnect();
            }
            catch (Exception ex)
            {
                handler.Log(ex.ToString());
                Disconnect();
            }
        }

        public void Send(byte[] data)
        {
            if (client == null) return;

            try
            {
                client.BeginSend(data, data.Length, remoteEndPoint, new AsyncCallback(AsyncSendCallback), client);
            }
            catch (Exception ex)
            {
                handler.Log(ex.ToString());
            }
        }

        public void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            Send(data);
        }

        void AsyncSendCallback(IAsyncResult ar)
        {
            try
            {
                int bytesSent = client.EndSend(ar);
                handler.SendCallback(bytesSent);
            }
            catch (Exception ex)
            {
                handler.Log(ex.ToString());
                Disconnect();
            }
        }
    }
}
