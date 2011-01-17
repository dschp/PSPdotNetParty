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

namespace PspDotNetParty.Server
{
    public class AsyncTcpServer<Type> : IServer<Type>
        where Type : IClientState
    {
        Socket listeningTcpSocket = null;

        AsyncCallback asyncAcceptCallback;

        IServerHandler<Type> handler;

        public AsyncTcpServer()
        {
            asyncAcceptCallback = new AsyncCallback(AcceptCallback);
        }

        public void StartListening(IPEndPoint localEP, IServerHandler<Type> handler)
        {
            // stop if started
            StopListening();

            this.handler = handler;

            // Create a TCP/IP socket.
            listeningTcpSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listeningTcpSocket.Bind(localEP);
                listeningTcpSocket.Listen(100);

                Console.WriteLine("TCP : Listening on {0}", listeningTcpSocket.LocalEndPoint);

                AcceptNext();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                StopListening();
                throw ex;
            }
        }

        public void StopListening()
        {
            if (listeningTcpSocket != null)
            {
                listeningTcpSocket.Close();
                listeningTcpSocket = null;
            }
            handler = null;
        }

        void AcceptNext()
        {
            if (listeningTcpSocket == null || !listeningTcpSocket.IsBound) return;

            //Console.WriteLine("TCP AcceptNext");

            // Start an asynchronous socket to listen for connections.
            //Console.WriteLine("TCP : クライアントからの接続を待っています...");
            listeningTcpSocket.BeginAccept(asyncAcceptCallback, null);
        }

        void AcceptCallback(IAsyncResult ar)
        {
            //Console.WriteLine("TCP AcceptCallback");

            // Get the socket that handles the client request.
            Socket socket = null;
            try
            {
                socket = listeningTcpSocket.EndAccept(ar);
                //Console.WriteLine("TCP : 接続されました: {0}", socket.RemoteEndPoint);

                TcpConnection conn = new TcpConnection(socket, handler);

                conn.state = handler.CreateState(conn);

                if (conn.state != null) conn.ReadNext();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //if (socket != null) socket.Dispose();
            }

            AcceptNext();
        }

        class TcpConnection : IServerConnection
        {
            IServerHandler<Type> handler;
            Socket socket;
            public Type state;

            AsyncCallback asyncReadCalback;
            AsyncCallback asyncSendCallback;

            public TcpConnection(Socket socket, IServerHandler<Type> handler)
            {
                this.socket = socket;
                this.handler = handler;

                asyncReadCalback = new AsyncCallback(ReadCallback);
                asyncSendCallback = new AsyncCallback(SendCallback);
            }

            public IPEndPoint RemoteEndPoint { get { return (IPEndPoint)socket.RemoteEndPoint; } }

            internal void ReadNext()
            {
                //Console.WriteLine("Read");
                try
                {
                    socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, asyncReadCalback, state);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    DisconnectClient(state);
                }
            }

            void ReadCallback(IAsyncResult ar)
            {
                //Console.WriteLine("TCP ReadCallback");

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                try
                {
                    // Read data from the client socket. 
                    int bytesRead = socket.EndReceive(ar);
                    bool sessionContinue = false;
                    if (bytesRead > 0)
                    {
                        PacketData data = new PacketData(state.Buffer, bytesRead);
                        sessionContinue = handler.ProcessIncomingData(state, data);
                    }
                    else
                    {
                        //Console.WriteLine("bytesRead = 0, which means the client had closed. " + RemoteEndPoint);
                        sessionContinue = false;
                    }

                    if (sessionContinue)
                        ReadNext();
                    else
                        DisconnectClient(state);
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    DisconnectClient(state);
                }
            }

            public void Send(byte[] data, int count)
            {
                //Console.WriteLine("TCP Send: Binary length={0}", count);
                if (socket == null || !socket.Connected) return;

                // Begin sending the data to the remote device.
                try
                {
                    socket.BeginSend(data, 0, count, 0, asyncSendCallback, null);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    //DisconnectClient(state);
                }
            }

            public void Send(string message)
            {
                //Console.WriteLine("Send: {0}", message);
                if (socket == null || !socket.Connected) return;

                // Convert the string data to byte data using ASCII encoding.
                byte[] data = Encoding.UTF8.GetBytes(message + Protocol1Constants.MESSAGE_SEPARATOR);

                // Begin sending the data to the remote device.
                try
                {
                    socket.BeginSend(data, 0, data.Length, 0, asyncSendCallback, null);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(e.ToString());
                    //DisconnectClient(state);
                }
            }

            void SendCallback(IAsyncResult ar)
            {
                //Console.WriteLine("TCP SendCallback");

                // Retrieve the socket from the state object.
                Type state = (Type)ar.AsyncState;
                //Socket socket = state.Socket;
                try
                {
                    // Complete sending the data to the remote device.
                    int bytesSent = socket.EndSend(ar);
                    //Console.WriteLine("TCP Sent {0} bytes to {1}", bytesSent, socket.RemoteEndPoint);
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    //DisconnectClient(state);
                }
            }

            void DisconnectClient(Type state)
            {
                //Console.WriteLine("TCP DisconnectClient");

                if (socket == null || state == null) return;

                try
                {
                    handler.DisposeState(state);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                state = default(Type);

                if (socket != null)
                {
                    try
                    {
                        //Console.WriteLine("切断します: " + socket.RemoteEndPoint);
                        //state.Socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }

                    socket = null;
                }
            }
        }
    }
}
