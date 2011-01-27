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
using System.Net.NetworkInformation;
using System.Threading;
using PspDotNetParty;
using PspDotNetParty.Server;
using System.IO;

namespace ArenaServer
{
    class Protocol1ArenaEngine
    {
        IServer<PlayerState> ChatServer;
        SessionHandler ChatHandler;

        AsyncUdpServer<TunnelState> TunnelServer;
        TunnelHandler MyTunnelHandler;

        public Protocol1ArenaEngine()
        {
            ChatServer = new AsyncTcpServer<PlayerState>();
            ChatHandler = new SessionHandler(this);

            TunnelServer = new AsyncUdpServer<TunnelState>();
            MyTunnelHandler = new TunnelHandler(this);

            MaxPlayers = 4;
            MaxRooms = 1;
        }

        bool EngineIsActice = false;

        public void StartListening(IPEndPoint localEP)
        {
            Console.WriteLine("プロトコル: {0}", Protocol1Constants.PROTOCOL_NUMBER);

            ChatServer.StartListening(localEP, ChatHandler);
            TunnelServer.StartListening(localEP, MyTunnelHandler);

            EngineIsActice = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendServerStatusToAllLoginPlayersLoop));
        }

        public void StopListening()
        {
            EngineIsActice = false;

            ChatServer.StopListening();
            TunnelServer.StopListening();

            lock (PlayersByName)
                PlayersByName.Clear();
            lock (PlayersByTunnelEndPoint)
                PlayersByTunnelEndPoint.Clear();
            lock (PlayersByMacAddress)
                PlayersByMacAddress.Clear();
            lock (PlayRooms)
                PlayRooms.Clear();
        }

        Dictionary<string, PlayerState> PlayersByName = new Dictionary<string, PlayerState>();
        Dictionary<string, PlayerState> PlayersByTunnelEndPoint = new Dictionary<string, PlayerState>();
        Dictionary<string, PlayerState> PlayersByMacAddress = new Dictionary<string, PlayerState>();

        Dictionary<string, Room> PlayRooms = new Dictionary<string, Room>();

        Room Lobby = new Room();

        public int MaxPlayers
        {
            get { return Lobby.MaxPlayers; }
            set { if (value < 1) throw new ArgumentOutOfRangeException(); Lobby.MaxPlayers = value; }
        }

        int _maxRooms;
        public int MaxRooms
        {
            get { return _maxRooms; }
            set { if (value < 1) throw new ArgumentOutOfRangeException(); _maxRooms = value; }
        }

        void SendServerStatusToAllLoginPlayersLoop(object o)
        {
            StringBuilder sb = new StringBuilder();
            ForEachPlayerAction PrintStateAction = new ForEachPlayerAction(
                delegate(PlayerState p)
                {
                    Console.WriteLine(p.Name);
                });
            while (EngineIsActice)
            {
                sb.Append(Protocol1Constants.COMMAND_SERVER_STATUS);
                sb.AppendFormat(" {0} {1} {2} {3}",
                    PlayersByName.Count, MaxPlayers, PlayRooms.Count, MaxRooms);

                string message = sb.ToString();

                lock (PlayersByName)
                    try
                    {
                        foreach (KeyValuePair<string, PlayerState> p in PlayersByName)
                        {
                            p.Value.Connection.Send(message);
                        }
                    }
                    catch (Exception) { continue; }

                sb.Clear();
                Thread.Sleep(2000);

                //Console.WriteLine("[Lobby]");
                //Lobby.ForEach(PrintStateAction);

                //lock (PlayRooms)
                //    foreach (KeyValuePair<string, Room> p in PlayRooms)
                //    {
                //        Console.WriteLine("[{0}]", p.Key);
                //        p.Value.ForEach(PrintStateAction);
                //    }
                //lock (PlayersByMacAddress)
                //    foreach (KeyValuePair<string, PlayerState> p in PlayersByMacAddress)
                //        Console.WriteLine("Mac: [{0}] => {1}", p.Key, p.Value);
            }
        }

        static void AppendNotifyUserList(StringBuilder sb, Room room)
        {
            sb.Append(Protocol1Constants.NOTIFY_USER_LIST).Append(' ');
            room.ForEach(new ForEachPlayerAction(
                delegate(PlayerState p)
                {
                    sb.Append(p.Name).Append(' ');
                }));
            sb.Remove(sb.Length - 1, 1);
        }

        void AppendNotifyRoomList(StringBuilder sb)
        {
            sb.Append(Protocol1Constants.NOTIFY_ROOM_LIST).Append(' ');
            lock (PlayRooms)
                foreach (KeyValuePair<string, Room> p in PlayRooms)
                {
                    Room room = p.Value;
                    sb.Append(p.Key).Append(' ');
                    sb.Append(room.MaxPlayers).Append(' ');
                    sb.Append(room.PlayerCount).Append(' ');
                    sb.Append(room.Password != string.Empty ? 'Y' : 'N').Append(' ');
                    sb.Append(room.Title).Append(' ');
                }
            sb.Remove(sb.Length - 1, 1);
        }

        void AppendNotifyRoomUpdate(StringBuilder sb, Room room)
        {
            sb.Append(Protocol1Constants.NOTIFY_ROOM_UPDATED).Append(' ');
            sb.Append(room.Master.Name).Append(' ');
            sb.Append(room.MaxPlayers).Append(' ');
            sb.Append(room.PlayerCount).Append(' ');
            sb.Append(room.Password != string.Empty ? 'Y' : 'N').Append(' ');
            sb.Append(room.Title);
        }

        void MoveToLobby(PlayerState user)
        {
            if (user.CurrentRoom == Lobby) return;

            user.CurrentRoom.RemovePlayer(user);

            Lobby.AddPlayer(user);
            user.CurrentRoom = Lobby;
        }

        class SessionHandler : IServerHandler<PlayerState>
        {
            Protocol1ArenaEngine Engine;

            Dictionary<string, MessageHandler> FirstStageHandlers = new Dictionary<string, MessageHandler>();
            Dictionary<string, MessageHandler> SecondStageHandlers = new Dictionary<string, MessageHandler>();
            Dictionary<string, MessageHandler> ThirdStageHandlers = new Dictionary<string, MessageHandler>();

            public SessionHandler(Protocol1ArenaEngine engine)
            {
                this.Engine = engine;

                FirstStageHandlers[Protocol1Constants.COMMAND_VERSION] = new MessageHandler(VersionMatchHandler);
                SecondStageHandlers[Protocol1Constants.COMMAND_LOGIN] = new MessageHandler(LoginHandler);

                ThirdStageHandlers[Protocol1Constants.COMMAND_INFORM_TUNNEL_UDP_PORT] = new MessageHandler(InformUdpEndPointHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_LOGOUT] = new MessageHandler(LogoutHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_CHAT] = new MessageHandler(ChatHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_PING] = new MessageHandler(PingHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_INFORM_PING] = new MessageHandler(InformPingHandler);
                //ThirdStageHandlers[Protocol1Constants.COMMAND_USER_UPDATE] = new MessageHandler(UserUpdateHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_ROOM_CREATE] = new MessageHandler(RoomCreateHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_ROOM_UPDATE] = new MessageHandler(RoomUpdateHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_ROOM_DELETE] = new MessageHandler(RoomDeleteHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_ROOM_ENTER] = new MessageHandler(RoomEnterHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_ROOM_EXIT] = new MessageHandler(RoomExitHandler);
                ThirdStageHandlers[Protocol1Constants.COMMAND_ROOM_KICK_PLAYER] = new MessageHandler(RoomKickUserHandler);
                //ThirdStageHandlers[COMMAND_ADMIN_NOTIFY] = new MessageHandler(AdminNotifyHandler);
            }

            public PlayerState CreateState(IServerConnection connection)
            {
                PlayerState state = new PlayerState(connection);
                state.MessageProcessors = FirstStageHandlers;

                string ipaddr = connection.RemoteEndPoint.Address.ToString();

                return state;
            }

            public void DisposeState(PlayerState playerToBeDisposed)
            {
                Room currentRoom;
                if (playerToBeDisposed.CurrentRoom == null) return;

                currentRoom = playerToBeDisposed.CurrentRoom;
                playerToBeDisposed.CurrentRoom = null;

                lock (Engine.PlayersByName)
                    Engine.PlayersByName.Remove(playerToBeDisposed.Name);

                IPEndPoint remoteEP = playerToBeDisposed.RemoteTunnelEndPoint;
                if (remoteEP != null)
                {
                    lock (Engine.PlayersByTunnelEndPoint)
                        Engine.PlayersByTunnelEndPoint.Remove(remoteEP.ToString());
                }

                if (playerToBeDisposed == currentRoom.Master)
                {
                    string message;
                    List<PlayerState> otherPlayers = currentRoom.GetPlayerList(playerToBeDisposed);

                    lock (Engine.PlayRooms)
                        Engine.PlayRooms.Remove(playerToBeDisposed.Name);
                    foreach (PlayerState p in otherPlayers)
                        Engine.MoveToLobby(p);

                    StringBuilder sb = new StringBuilder();

                    // send notify other members enter to lobby members
                    sb.Append(Protocol1Constants.NOTIFY_ROOM_DELETED).Append(' ').Append(playerToBeDisposed.Name);

                    foreach (PlayerState p in otherPlayers)
                    {
                        if (p == playerToBeDisposed) continue;

                        sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                        sb.Append(Protocol1Constants.NOTIFY_USER_ENTERED).Append(' ').Append(p.Name);
                    }

                    message = sb.ToString();

                    Engine.Lobby.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(message);
                        }));

                    sb.Clear();
                    // send notify room deletion to old room members
                    sb.Append(Protocol1Constants.COMMAND_ROOM_DELETE);
                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);

                    AppendNotifyUserList(sb, Engine.Lobby);
                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);

                    Engine.AppendNotifyRoomList(sb);

                    message = sb.ToString();

                    foreach (PlayerState p in otherPlayers)
                        p.Connection.Send(message);
                }
                else
                {
                    currentRoom.RemovePlayer(playerToBeDisposed);

                    // send notify to all room members
                    string notify = Protocol1Constants.NOTIFY_USER_EXITED + " " + playerToBeDisposed.Name;
                    currentRoom.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(notify);
                        }));

                    if (currentRoom != Engine.Lobby)
                    {
                        StringBuilder sb = new StringBuilder();
                        // send notify room update to lobby members
                        Engine.AppendNotifyRoomUpdate(sb, currentRoom);
                        notify = sb.ToString();
                        Engine.Lobby.ForEach(new ForEachPlayerAction(
                            delegate(PlayerState p)
                            {
                                p.Connection.Send(notify);
                            }));
                    }
                }
            }

            public bool ProcessIncomingData(PlayerState state, PacketData data)
            {
                IServerConnection socket = state.Connection;
                bool sessionContinue = false;

                foreach (string message in data.Messages)
                {
                    //Console.WriteLine(message);

                    int commandEndIndex = message.IndexOf(' ');
                    string command, argument;
                    if (commandEndIndex > 0)
                    {
                        command = message.Substring(0, commandEndIndex);
                        argument = message.Substring(commandEndIndex + 1);
                    }
                    else
                    {
                        command = message;
                        argument = string.Empty;
                    }

                    try
                    {
                        MessageHandler handler = state.MessageProcessors[command];
                        sessionContinue = handler(state, argument);
                    }
                    catch (KeyNotFoundException)
                    {
                        sessionContinue = false;
                    }

                    if (!sessionContinue) break;
                }

                return sessionContinue;
            }

            bool VersionMatchHandler(PlayerState state, string message)
            {
                string version = message;
                if (version == Protocol1Constants.PROTOCOL_NUMBER)
                {
                    //version match, so go on
                    state.MessageProcessors = SecondStageHandlers;
                    state.Connection.Send(Protocol1Constants.SERVER_ARENA);
                    return true;
                }
                else
                {
                    //Console.WriteLine("バージョンが一致しませんので切断します");
                    state.Connection.Send(Protocol1Constants.ERROR_VERSION_MISMATCH + " " + Protocol1Constants.PROTOCOL_NUMBER);
                    return true;
                }
            }

            bool LoginHandler(PlayerState state, string message)
            {
                //string[] args = message.Split(' ');
                string name = message; //args[0];

                state.Name = name;

                if (name.Length == 0)
                {
                    return false;
                }
                else if (Engine.PlayersByName.ContainsKey(name))
                {
                    //Console.WriteLine("同名のユーザーが存在するので接続を拒否します: {0}", state.Connection.RemoteEndPoint);
                    state.Connection.Send(Protocol1Constants.ERROR_LOGIN_DUPLICATED_NAME);
                    return false;
                }

                // TODO: password auth
                // login succeed
                lock (Engine.PlayersByName)
                    if (Engine.PlayersByName.Count < Engine.MaxPlayers)
                    {
                        Engine.PlayersByName[name] = state;
                        state.MessageProcessors = ThirdStageHandlers;
                    }
                    else
                    {
                        //Console.WriteLine("最大人数を超えたので接続を拒否します: {0}", state.Connection.RemoteEndPoint);
                        state.Connection.Send(Protocol1Constants.ERROR_LOGIN_BEYOND_CAPACITY);
                        return false;
                    }

                Engine.Lobby.AddPlayer(state);
                state.CurrentRoom = Engine.Lobby;

                Engine.Lobby.ForEach(new ForEachPlayerAction(
                    delegate(PlayerState p)
                    {   // send notify user enter
                        if (p != state)
                            p.Connection.Send(Protocol1Constants.NOTIFY_USER_ENTERED + " " + state.Name);
                    }));

                StringBuilder sb = new StringBuilder();

                sb.Append(Protocol1Constants.COMMAND_LOGIN);
                sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);

                AppendNotifyUserList(sb, Engine.Lobby);
                sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);

                Engine.AppendNotifyRoomList(sb);

                state.Connection.Send(sb.ToString());

                return true;
            }

            bool LogoutHandler(PlayerState user, string message)
            {
                return false;
            }

            bool InformUdpEndPointHandler(PlayerState user, string udpRemotePort)
            {
                try
                {
                    user.UdpTunnelPort = int.Parse(udpRemotePort);
                    string remoteEP = user.Connection.RemoteEndPoint.Address + ":" + udpRemotePort;
                    //Console.WriteLine("Informed UDP IP Address: {0} -> {1}", user.Name, remoteEP);
                    Engine.PlayersByTunnelEndPoint[remoteEP] = user;

                    user.Connection.Send(Protocol1Constants.COMMAND_INFORM_TUNNEL_UDP_PORT);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(e.ToString());
                }
                return true;
            }

            bool ChatHandler(PlayerState user, string message)
            {
                string chatMessage = Protocol1Constants.COMMAND_CHAT + " <" + user.Name + "> " + message;

                user.CurrentRoom.ForEach(new ForEachPlayerAction(
                    delegate(PlayerState p)
                    {
                        p.Connection.Send(chatMessage);
                    }));

                return true;
            }

            bool PingHandler(PlayerState state, string message)
            {
                state.Connection.Send(Protocol1Constants.COMMAND_PINGBACK + " " + message);
                return true;
            }

            bool InformPingHandler(PlayerState state, string message)
            {
                state.CurrentRoom.ForEach(new ForEachPlayerAction(
                    delegate(PlayerState p)
                    {
                        if (p != state)
                            p.Connection.Send(Protocol1Constants.COMMAND_INFORM_PING + " " + state.Name + " " + message);
                    }));
                return true;
            }

            bool UserUpdateHandler(PlayerState user, string message)
            {
                string userInfo = message;
                //Console.WriteLine("User Info; {0}", userInfo);
                // Update info

                //MessageSender sender = new MessageSender();
                //sender.skip = user;
                //sender.message = "UL " + ConcatUserList(user.CurrentRoom);

                //user.CurrentRoom.ForEach(new ForEachPlayerAction(sender.SendMessage));

                return true;
            }

            bool RoomCreateHandler(PlayerState user, string message)
            {
                Room newRoom = null;
                lock (Engine.PlayRooms)
                    if (user.CurrentRoom == Engine.Lobby && Engine.PlayRooms.Count < Engine.MaxRooms)
                    {
                        newRoom = new Room(user);
                        newRoom.MaxPlayers = 4;
                        Engine.PlayRooms[user.Name] = newRoom;
                    }
                if (newRoom != null)
                {
                    // RC {maxusers} {title} "{description}" "{password}"
                    string[] args = message.Split(' ');

                    if (args.Length != 4) return true;

                    int maxplayers;
                    if (!int.TryParse(args[0], out maxplayers))
                        maxplayers = 4;

                    newRoom.MaxPlayers = maxplayers;
                    newRoom.Title = args[1];
                    newRoom.Description = args[2].Substring(1, args[2].Length - 2);
                    newRoom.Password = args[3].Substring(1, args[3].Length - 2);

                    Engine.Lobby.RemovePlayer(user);
                    user.Connection.Send(Protocol1Constants.COMMAND_ROOM_CREATE);

                    StringBuilder sb = new StringBuilder();
                    // send notify room created to lobby member
                    // send notify user exit to lobby members
                    sb.Append(Protocol1Constants.NOTIFY_ROOM_CREATED).Append(' ').Append(user.Name);
                    sb.Append(' ').Append(newRoom.MaxPlayers).Append(' ').Append(newRoom.PlayerCount);
                    sb.Append(' ').Append(newRoom.Password != string.Empty ? 'Y' : 'N').Append(' ').Append(newRoom.Title);
                    //sb.Append(Protocol1Constants.NOTIFY_USER_EXITED).Append(" ").Append(user.Name);
                    string notify = sb.ToString();

                    Engine.Lobby.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(notify);
                        }));
                }
                else
                {
                    user.Connection.Send(Protocol1Constants.ERROR_ROOM_CREATE_BEYOND_LIMIT);
                }
                return true;
            }

            bool RoomUpdateHandler(PlayerState user, string message)
            {
                if (user == user.CurrentRoom.Master)
                {
                    Room room = user.CurrentRoom;

                    // "RU {maxusers} {title} {description} {password}"
                    string[] args = message.Split(' ');
                    if (args.Length != 4) return true;

                    int maxplayers;
                    if (!int.TryParse(args[0], out maxplayers))
                        maxplayers = 4;

                    room.MaxPlayers = maxplayers;
                    room.Title = args[1];
                    room.Description = args[2].Substring(1, args[2].Length - 2);
                    room.Password = args[3].Substring(1, args[3].Length - 2);

                    user.Connection.Send(Protocol1Constants.COMMAND_ROOM_UPDATE);

                    StringBuilder sb = new StringBuilder();
                    Engine.AppendNotifyRoomUpdate(sb, room);
                    // send notify room update to lobby member
                    // send notify user exit to lobby members
                    string notify = sb.ToString();

                    Engine.Lobby.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(notify);
                        }));

                    sb.Append(' ').Append(room.Description);
                    notify = sb.ToString();
                    room.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            if (p != user)
                                p.Connection.Send(notify);
                        }));
                }
                return true;
            }

            bool RoomDeleteHandler(PlayerState user, string message)
            {
                bool deleteSuccess = false;
                lock (Engine.PlayRooms)
                {
                    if (Engine.PlayRooms.ContainsKey(user.Name))// user == user.CurrentRoom.Master)
                    {
                        Engine.PlayRooms.Remove(user.Name);
                        deleteSuccess = true;
                    }
                }

                if (deleteSuccess)
                {
                    List<PlayerState> list = user.CurrentRoom.GetPlayerList();

                    StringBuilder sb = new StringBuilder();

                    // send notify room deleted to lobby members
                    sb.Append(Protocol1Constants.NOTIFY_ROOM_DELETED).Append(' ').Append(user.Name);

                    foreach (PlayerState p in list)
                    {
                        // send notify deleted room users enter to lobby members
                        sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                        sb.Append(Protocol1Constants.NOTIFY_USER_ENTERED).Append(' ').Append(p.Name);
                    }

                    message = sb.ToString();

                    Engine.Lobby.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(message);
                        }));

                    foreach (PlayerState p in list)
                        Engine.MoveToLobby(p);


                    sb.Clear();
                    // send user list to deleting room members
                    // send room list to deleting room members
                    sb.Append(Protocol1Constants.COMMAND_ROOM_DELETE);
                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                    AppendNotifyUserList(sb, Engine.Lobby);
                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);

                    Engine.AppendNotifyRoomList(sb);

                    message = sb.ToString();

                    foreach (PlayerState p in list)
                        p.Connection.Send(message);
                }
                return true;
            }

            bool RoomEnterHandler(PlayerState user, string message)
            {
                if (user.CurrentRoom == Engine.Lobby)
                {
                    string[] args = message.Split(' ');
                    string masterName = args[0];
                    string password = args.Length == 1 ? null : args[1];
                    if (Engine.PlayRooms.ContainsKey(masterName))
                    {
                        string notify;
                        Room room = Engine.PlayRooms[masterName];
                        if (room.Password != string.Empty)
                        {
                            if (password == null)
                            {
                                user.Connection.Send(Protocol1Constants.NOTIFY_ROOM_PASSWORD_REQUIRED + " " + masterName);
                                return true;
                            }
                            else if (password != room.Password)
                            {
                                user.Connection.Send(Protocol1Constants.ERROR_ROOM_ENTER_PASSWORD_FAIL);
                                return true;
                            }
                        }

                        if (room.AddPlayer(user))
                        {
                            Engine.Lobby.RemovePlayer(user);
                            user.CurrentRoom = room;

                            StringBuilder sb = new StringBuilder();
                            // send room member list to user
                            sb.Append(Protocol1Constants.COMMAND_ROOM_ENTER).Append(' ').Append(masterName);
                            sb.Append(' ').Append(room.MaxPlayers).Append(' ').Append(room.Title).Append(' ').Append(room.Description);
                            sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                            AppendNotifyUserList(sb, room);

                            user.Connection.Send(sb.ToString());

                            // send notify user enter to room members
                            notify = Protocol1Constants.NOTIFY_USER_ENTERED + " " + user.Name;
                            room.ForEach(new ForEachPlayerAction(
                                delegate(PlayerState p)
                                {
                                    if (p != user)
                                        p.Connection.Send(notify);
                                }));

                            sb.Clear();
                            // send notify user exit to lobby members
                            // send notify room update to lobby members
                            sb.Append(Protocol1Constants.NOTIFY_USER_EXITED).Append(' ').Append(user.Name);
                            sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                            Engine.AppendNotifyRoomUpdate(sb, room);

                            notify = sb.ToString();
                            Engine.Lobby.ForEach(new ForEachPlayerAction(
                                delegate(PlayerState p)
                                {
                                    p.Connection.Send(notify);
                                }));
                        }
                        else
                        {
                            user.Connection.Send(Protocol1Constants.ERROR_ROOM_ENTER_BEYOND_CAPACITY);
                        }
                    }
                }
                return true;
            }

            bool RoomExitHandler(PlayerState user, string message)
            {
                if (user != user.CurrentRoom.Master)
                {
                    string notify;
                    StringBuilder sb = new StringBuilder();

                    Room oldRoom = user.CurrentRoom;
                    oldRoom.RemovePlayer(user);

                    // send notify user exit to room members
                    notify = Protocol1Constants.NOTIFY_USER_EXITED + " " + user.Name;
                    oldRoom.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(notify);
                        }));

                    // send notify user enter to lobby members
                    // send notify room update to lobby members
                    sb.Append(Protocol1Constants.NOTIFY_USER_ENTERED).Append(' ').Append(user.Name);
                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                    Engine.AppendNotifyRoomUpdate(sb, oldRoom);

                    notify = sb.ToString();
                    Engine.Lobby.ForEach(new ForEachPlayerAction(
                        delegate(PlayerState p)
                        {
                            p.Connection.Send(notify);
                        }));

                    user.CurrentRoom = Engine.Lobby;
                    Engine.Lobby.AddPlayer(user);

                    sb.Clear();
                    // send room list and user list to this player
                    sb.Append(Protocol1Constants.COMMAND_ROOM_EXIT);

                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                    AppendNotifyUserList(sb, Engine.Lobby);

                    sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                    Engine.AppendNotifyRoomList(sb);

                    user.Connection.Send(sb.ToString());
                }
                return true;
            }

            bool RoomKickUserHandler(PlayerState user, string message)
            {
                if (user == user.CurrentRoom.Master)
                {
                    string name = message;
                    if (name != user.Name && Engine.PlayersByName.ContainsKey(name))
                    {
                        PlayerState kickedUser = Engine.PlayersByName[name];
                        if (kickedUser.CurrentRoom != user.CurrentRoom) return true;

                        Engine.MoveToLobby(kickedUser);

                        string notify;
                        StringBuilder sb = new StringBuilder();

                        // send notify user enter to lobby members
                        sb.Append(Protocol1Constants.NOTIFY_USER_ENTERED);
                        sb.Append(' ').Append(kickedUser.Name);
                        sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                        Engine.AppendNotifyRoomUpdate(sb, user.CurrentRoom);

                        notify = sb.ToString();
                        Engine.Lobby.ForEach(new ForEachPlayerAction(
                            delegate(PlayerState p)
                            {
                                if (p != kickedUser)
                                    p.Connection.Send(notify);
                            }));

                        sb.Clear();
                        // send notify user exit to room members
                        // send notify room update to lobby members
                        sb.Append(Protocol1Constants.COMMAND_ROOM_KICK_PLAYER);
                        sb.Append(" ").Append(kickedUser.Name);

                        notify = sb.ToString();
                        user.CurrentRoom.ForEach(new ForEachPlayerAction(
                            delegate(PlayerState p)
                            {
                                p.Connection.Send(notify);
                            }));

                        sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                        AppendNotifyUserList(sb, Engine.Lobby);
                        sb.Append(Protocol1Constants.MESSAGE_SEPARATOR);
                        Engine.AppendNotifyRoomList(sb);

                        kickedUser.Connection.Send(sb.ToString());
                    }
                }
                return true;
            }
        }

        class TunnelHandler : IServerHandler<TunnelState>
        {
            Protocol1ArenaEngine Engine;

            public TunnelHandler(Protocol1ArenaEngine engine)
            {
                this.Engine = engine;
            }

            public TunnelState CreateState(IServerConnection connection)
            {
                TunnelState state = new TunnelState(connection);
                return state;
            }

            public void DisposeState(TunnelState state)
            {
                Engine.PlayersByTunnelEndPoint.Remove(state.Connection.RemoteEndPoint.ToString());
            }


            public bool ProcessIncomingData(TunnelState tunnelState, PacketData data)
            {
                string ipaddress = tunnelState.Connection.RemoteEndPoint.ToString();
                //Console.WriteLine(ipaddress);

                if (!Engine.PlayersByTunnelEndPoint.ContainsKey(ipaddress))
                {
                    //Console.WriteLine("This is from anonymous");

                    IServerConnection connection = tunnelState.Connection;
                    //Console.WriteLine("Inform client about its external port: {0}", connection.RemoteEndPoint);
                    connection.Send(connection.RemoteEndPoint.Port.ToString());

                    return true;
                }
                PlayerState player = Engine.PlayersByTunnelEndPoint[ipaddress];
                Room room = player.CurrentRoom;

                if (room == Engine.Lobby) return true;

                byte[] packet = data.RawBytes;
                //Console.WriteLine(BitConverter.ToString(packet));
                //Console.WriteLine();
                //Console.WriteLine(packet.Length);

                if (!Utility.IsPspPacket(packet, Utility.HEADER_OFFSET))
                {
                    //Console.WriteLine("This is not PSP Packet");
                    return true;
                }

                byte[] buff = new byte[6];
                Array.Copy(packet, Utility.HEADER_OFFSET, buff, 0, 6);
                PhysicalAddress destMac = new PhysicalAddress(buff);
                Array.Copy(packet, Utility.HEADER_OFFSET + 6, buff, 0, 6);
                PhysicalAddress srcMac = new PhysicalAddress(buff);

                string destMacStr = destMac.ToString();
                string srcMacStr = srcMac.ToString();

                lock (Engine.PlayersByMacAddress)
                    Engine.PlayersByMacAddress[srcMacStr] = player;

                //Console.WriteLine("Tick = {0}", BitConverter.ToUInt64(packet, 0));
                //Console.WriteLine("Tunnel packet: {0} -> {1} size={2}", srcMacStr, destMacStr, packet.Length - Utility.HEADER_OFFSET);

                if (destMac.Equals(Utility.GetBroadcastAddress()))
                {
                    //Console.WriteLine("Broadcast packet from {0}", player.Name);
                    // send to other players in room except me
                    room.ForEach((ForEachPlayerAction)delegate(PlayerState sendTo)
                    {
                        IPEndPoint remoteEP = sendTo.RemoteTunnelEndPoint;
                        if (remoteEP != null)
                        {
                            //Console.WriteLine("Broadcasting from {0} to {1}", player.Name, sendTo.Name);
                            Engine.TunnelServer.Send(remoteEP, packet, packet.Length);
                        }
                        else
                        {
                            //Console.WriteLine("But {0} doesn't have UDP tunnel port;", sendTo.Name);
                        }
                    }, player);
                }
                else
                {
                    // send to specific player
                    if (Engine.PlayersByMacAddress.ContainsKey(destMacStr))
                    {
                        PlayerState sendTo = Engine.PlayersByMacAddress[destMacStr];
                        if (sendTo.CurrentRoom != room)
                        {
                            //Console.WriteLine("Players are not in the same room: {0} [{1}] => {2} [{3}]",
                            //    player.Name, srcMacStr, sendTo.Name, destMacStr);
                            return true;
                        }

                        //Console.WriteLine("Tunnel: {0} [{1}] => {2} [{3}]", player.Name, srcMacStr, sendTo.Name, destMacStr);
                        IPEndPoint remoteEP = sendTo.RemoteTunnelEndPoint;
                        if (remoteEP != null)
                        {
                            Engine.TunnelServer.Send(remoteEP, packet, packet.Length);
                        }
                        else
                        {
                            //Console.WriteLine("But {0} doesn't have UDP tunnel port;", sendTo.Name);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Player not found from MAC Address: {0}", destMac.ToString());
                    }
                }
                return false;
            }
        }
    }
}