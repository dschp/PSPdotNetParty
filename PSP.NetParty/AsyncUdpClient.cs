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

namespace PspDotNetParty.Client
{
    public class AsyncUdpClient : IAsyncClient
    {
        IAsyncClientHandler handler = null;

        UdpClient client = null;

        int _localPort = 0;
        public int LocalPort { set { _localPort = value; } }

        IPEndPoint remoteEndPoint;

        AsyncCallback ReadCallback;
        AsyncCallback SendCallback;

        public AsyncUdpClient(IAsyncClientHandler handler)
        {
            this.handler = handler;

            ReadCallback = new AsyncCallback(AsyncReadCallback);
            SendCallback = new AsyncCallback(AsyncSendCallback);
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
                    handler.DisconnectCallback();
                }
                catch (Exception)
                { }

                try
                {
                    client.Close();
                }
                catch (Exception) { }

                client = null;
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
