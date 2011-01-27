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

namespace PspDotNetParty
{
    public sealed class ApplicationConstants
    {
        //private static string _applicationName;
        public const string APP_NAME = "PSP.NetParty";

        private static string _versionNumber;
        public static string VERSION
        {
            get
            {
                if (_versionNumber == null) _versionNumber = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                return _versionNumber;
            }
        }
    }

    public sealed class IniConstants
    {
        public const string SECTION_SETTINGS = "Settings";

        public const string CLIENT_LOGIN_NAME = "UserName";
        //public const string CLIENT_SERVER_ADDRESS = "ServerAddress";

        public const string CLIENT_WINDOW_WIDTH = "WindowWidth";
        public const string CLIENT_WINDOW_HEIGHT = "WindowHeight";

        public const string CLIENT_SERVER_LIST = "ServerList";
        public const string CLIENT_SERVER_HISTORY = "ServerHistory";

        public const string SERVER_PORT = "Port";
        public const string SERVER_MAX_USERS = "MaxUsers";
        public const string SERVER_MAX_ROOMS = "MaxRooms";
        public const string SERVER_IDLEPLAYER_TIMEOUT = "IdlePlayerTimeout";
    }

    public sealed class Protocol1Constants
    {
        public const char MESSAGE_SEPARATOR = '\t';

        public const string SERVER_ARENA = "SERVER_ARENA";
        public const string SERVER_PORTAL = "SERVER_PORTAL";
        public const string COMMAND_VERSION = "VERSION";
        public const string PROTOCOL_NUMBER = "3";

        public const string COMMAND_LOGIN = "LI";
        public const string COMMAND_LOGOUT = "LO";
        public const string COMMAND_CHAT = "CH"; //"CHAT ";
        public const string COMMAND_USER_UPDATE = "UU"; //"USER_UPDATE ";
        public const string COMMAND_ROOM_CREATE = "RC"; //"ROOM_CREATE ";
        public const string COMMAND_ROOM_UPDATE = "RU"; // "ROOM_UPDATE ";
        public const string COMMAND_ROOM_DELETE = "RD"; //"ROOM_DELETE ";
        public const string COMMAND_ROOM_ENTER = "RE"; //"ROOM_ENTER ";
        public const string COMMAND_ROOM_EXIT = "RX"; //"ROOM_EXIT ";
        public const string COMMAND_ROOM_KICK_PLAYER = "RK"; //"ROOM_KICK ";
        public const string COMMAND_SERVER_STATUS = "SS";
        public const string COMMAND_ADMIN_NOTIFY = "AN";

        public const string COMMAND_INFORM_TUNNEL_UDP_PORT = "IU";

        public const string COMMAND_PING = "PG";
        public const string COMMAND_PINGBACK = "PGBK";
        public const string COMMAND_INFORM_PING = "IP";

        public const string NOTIFY_USER_ENTERED = "NUE";
        public const string NOTIFY_USER_EXITED = "NUX";
        public const string NOTIFY_USER_LIST = "NUL";
        public const string NOTIFY_ROOM_CREATED = "NRC";
        public const string NOTIFY_ROOM_DELETED = "NRD";
        public const string NOTIFY_ROOM_LIST = "NRL";
        public const string NOTIFY_ROOM_UPDATED = "NRU";
        //public const string NOTIFY_ROOM_PLAYER_KICKED = "NRK";

        public const string NOTIFY_ROOM_PASSWORD_REQUIRED = "NRPR";

        public const string ERROR_VERSION_MISMATCH = "ERR_VER_MIS";

        public const string ERROR_LOGIN_DUPLICATED_NAME = "ERR_LI_DUP";
        public const string ERROR_LOGIN_BEYOND_CAPACITY = "ERR_LI_CAP";

        public const string ERROR_ROOM_CREATE_BEYOND_LIMIT = "ERR_RC_LIM";
        public const string ERROR_ROOM_ENTER_PASSWORD_FAIL = "ERR_RE_PWF";
        public const string ERROR_ROOM_ENTER_BEYOND_CAPACITY = "ERR_RE_CAP";
    }
}
