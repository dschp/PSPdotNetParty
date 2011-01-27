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

namespace ArenaServer
{
    class Program
    {
        public static int Main(String[] args)
        {
            Console.WriteLine("{0}  アリーナサーバー バージョン: {1}", ApplicationConstants.APP_NAME, ApplicationConstants.VERSION);

            IniParser p = new IniParser();

            int port;
            if (!int.TryParse(p.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_PORT), out port))
                port = 30000;

            int maxUsers;
            if (!int.TryParse(p.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_MAX_USERS), out maxUsers))
                maxUsers = 20;

            int maxRooms;
            if (!int.TryParse(p.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_MAX_ROOMS), out maxRooms))
                maxRooms = 3;

            int idleTimeout;
            if (!int.TryParse(p.GetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_IDLEPLAYER_TIMEOUT), out idleTimeout))
                idleTimeout = -1;

            switch (args.Length)
            {
                case 2:
                    maxUsers = Int32.Parse(args[1]);
                    port = Int32.Parse(args[0]);
                    break;
                case 1:
                    port = Int32.Parse(args[0]);
                    break;
                default:
                    break;
            }

            p.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_PORT, port.ToString());
            p.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_MAX_USERS, maxUsers.ToString());
            p.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_MAX_ROOMS, maxRooms.ToString());
            p.SetSetting(IniConstants.SECTION_SETTINGS, IniConstants.SERVER_IDLEPLAYER_TIMEOUT, idleTimeout.ToString());
            p.SaveSettings();

            Protocol1ArenaEngine engine = new Protocol1ArenaEngine();
            try
            {
                engine.MaxPlayers = maxUsers;
                engine.MaxRooms = maxRooms;
                engine.IdlePlayerTimeout = idleTimeout * 10000000;

                engine.StartListening(new IPEndPoint(IPAddress.Any, port));

                while (true)
                {
                    //Console.Write(">");
                    string cmd = Console.ReadLine();
                    //if (cmd == "") break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            finally
            {
                engine.StopListening();
            }

            return 0;
        }

        static void BasicUdpCommunication()
        {
            UdpClient listener = new UdpClient(30000); // 受信port
            listener.DontFragment = true;
            listener.EnableBroadcast = true;

            while (true)
            {
                IPEndPoint remoteEP = null;

                byte[] recvByte = listener.Receive(ref remoteEP);
                String msg = "receive: " + Encoding.UTF8.GetString(recvByte) +
                          " (form" + remoteEP.Address + ":" + remoteEP.Port + ")";
                Console.WriteLine(msg);

                byte[] data = Encoding.UTF8.GetBytes(msg);
                listener.Send(data, data.Length, remoteEP);
                Console.WriteLine(remoteEP);
            }
        }
    }
}
