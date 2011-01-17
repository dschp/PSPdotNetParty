using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MHPTunnelG;

namespace ArenaClient
{
    class AsyncTcpClient : IAsyncClient
    {
        const int BUFFER_SIZE = 256;
        byte[] readBuffer = new byte[BUFFER_SIZE];

        IAsyncClientHandler handler = null;

        Socket _socket;
        protected Socket Socket
        {
            get { return _socket; }
            private set { _socket = value; }
        }

        public AsyncTcpClient(IAsyncClientHandler handler) { this.handler = handler; }

        public bool Connected
        {
            get
            {
                if (_socket == null) return false;
                return _socket.Connected;
            }
        }

        public void Connect(string ipaddr, int port)
        {
            //IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
            IPAddress ipAddress = IPAddress.Parse(ipaddr);// ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            Connect(remoteEP);
        }
        public void Connect(IPEndPoint remoteEP)
        {
            try
            {
                // Create a TCP/IP socket.
                _socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                // Connect to the remote endpoint.
                _socket.BeginConnect(
                    remoteEP,
                    new AsyncCallback(AsyncConnectCallback),
                    _socket);

                //connectDone.WaitOne();

            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }

        void AsyncConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Complete the connection.
                _socket.EndConnect(ar);

                //handler.ConnectCallback((IPEndPoint)socket.RemoteEndPoint);
                handler.ConnectCallback((IPEndPoint)_socket.LocalEndPoint);

                //Send(String.Format("VERSION {1}", this.Version));
                ReadNext();

                // UserUpdateLoop and PingLoop also have to start here.
            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
                catch (Exception) { }

                _socket = null;
                handler.DisconnectCallback();
            }
        }

        delegate void ProcessMessageDelegate(PacketData data);

        void ProcessMessage(PacketData data)
        {
            handler.ReadCallback(data);
        }

        void ReadNext()
        {
            if (_socket == null) return;

            try
            {
                // Begin receiving the data from the remote device.
                _socket.BeginReceive(
                    readBuffer,
                    0,
                    readBuffer.Length,
                    0,
                    new AsyncCallback(AsyncReceiveCallback),
                    new ProcessMessageDelegate(ProcessMessage));
            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }

        void AsyncReceiveCallback(IAsyncResult ar)
        {
            //handler.Log("ReceiveCallback");

            try
            {
                // Read data from the remote device.
                int bytesRead = _socket.EndReceive(ar);
                if (bytesRead == 0)
                {
                    Disconnect();
                }
                else
                {
                    PacketData data = new PacketData(readBuffer, bytesRead);

                    // Process message here
                    ProcessMessageDelegate process = (ProcessMessageDelegate)ar.AsyncState;
                    process(data);

                    ReadNext();
                }
            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }

        public void Send(byte[] data)
        {
            if (_socket == null) return;

            try
            {
                // Begin sending the data to the remote device.
                _socket.BeginSend(
                    data,
                    0,
                    data.Length,
                    0,
                    new AsyncCallback(AsyncSendCallback),
                    _socket);
            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }

        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data + '\t');
            Send(byteData);
        }

        void AsyncSendCallback(IAsyncResult ar)
        {
            //handler.Log("SendCallback");
            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = _socket.EndSend(ar);
                handler.SendCallback(bytesSent);
            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }
    }
}
