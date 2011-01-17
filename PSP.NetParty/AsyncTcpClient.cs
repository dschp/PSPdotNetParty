/*
Copyright (C) 2010,2011 monte

This file is part of PSP NetParty.

PSP NetParty is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using PspDotNetParty;
using PspDotNetParty.Client;

namespace PspDotNetParty.Client
{
    public class AsyncTcpClient : IAsyncClient
    {
        const int BUFFER_SIZE = 1024;
        byte[] readBuffer = new byte[BUFFER_SIZE];

        IAsyncClientHandler handler = null;

        Socket _socket;
        protected Socket Socket
        {
            get { return _socket; }
            private set { _socket = value; }
        }

        AsyncCallback ConnectCallback;
        AsyncCallback ReadCallback;
        AsyncCallback SendCallback;

        public AsyncTcpClient(IAsyncClientHandler handler)
        {
            this.handler = handler;

            ConnectCallback = new AsyncCallback(AsyncConnectCallback);
            ReadCallback = new AsyncCallback(AsyncReadCallback);
            SendCallback = new AsyncCallback(AsyncSendCallback);
        }

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
                    ConnectCallback,
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
                    handler.DisconnectCallback();
                }
                catch (Exception) { }

                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
                catch (Exception) { }

                _socket = null;
            }
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
                    ReadCallback,
                    null);
            }
            catch (Exception e)
            {
                handler.Log(e.ToString());
                Disconnect();
            }
        }

        void AsyncReadCallback(IAsyncResult ar)
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
                    handler.ReadCallback(data);

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
                    SendCallback,
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
