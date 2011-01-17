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
using System.Windows.Forms;
using PspDotNetParty;
using PspDotNetParty.Client;

namespace ArenaClient
{
    class Program
    {
        public static int Main(String[] args)
        {
            //BasicUdpCommunication();
            //return 0;

            //IRoomClient client;
            //client = new TcpRoomClient(new ConsoleHandler());
            //client = new UdpRoomClient(new ConsoleHandler());

            //StartClient(client);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ArenaClientForm());
            return 0;
        }

        class ConsoleHandler : IAsyncClientHandler
        {
            IPEndPoint localEndPoint;

            public void ConnectCallback(IPEndPoint localEndPoint)
            {
                Console.WriteLine("Socket connected from {0}", localEndPoint.ToString());
                this.localEndPoint = localEndPoint;
            }
            public void ReadCallback(PacketData message)
            {
                Console.WriteLine("Received message: {0}", message);
            }
            public void SendCallback(int bytesSent)
            {
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            public void DisconnectCallback()
            {
                Console.WriteLine("Socket disconnected: {0}", localEndPoint.ToString());
            }
            public void Log(string message)
            {
                Console.WriteLine(message);
            }
        }

        static void StartClient(IAsyncClient client)
        {
            try
            {
                client.Connect("127.0.0.1", 30000);

                while (true)
                {
                    Console.Write(">");
                    string cmd = Console.ReadLine();
                    if (cmd == "") break;

                    // Convert command to message
                    string msg = "CHAT " + cmd;

                    client.Send(msg);
                }

                client.Disconnect();
            }
            catch (Exception)
            {
                Console.ReadLine();
            }
        }

        static void BasicUdpCommunication()
        {
            UdpClient client = new UdpClient();//6000); // 受信port
            client.DontFragment = true;
            client.EnableBroadcast = true;

            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEP = new IPEndPoint(address, 30000);
            client.Connect(remoteEP);

            IPEndPoint localEP = (IPEndPoint)client.Client.LocalEndPoint;
            Console.WriteLine(localEP);

            try
            {
                while (true)
                {
                    Console.Write(">");
                    string cmd = Console.ReadLine();
                    if (cmd == "") break;

                    byte[] data = Encoding.UTF8.GetBytes(cmd);
                    client.Send(data, data.Length);

                    byte[] recvByte = client.Receive(ref remoteEP);
                    String msg = "receive: " + Encoding.UTF8.GetString(recvByte) +
                              " (form" + remoteEP.Address + ":" + remoteEP.Port + ")";
                    Console.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
