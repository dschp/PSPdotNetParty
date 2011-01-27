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
using PspDotNetParty.Server;
using System.Threading;

namespace ArenaServer
{
    class TunnelState : IClientState
    {
        public const int BUFFER_SIZE = 2000;

        byte[] _buffer = new byte[BUFFER_SIZE];
        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        IServerConnection _connection;
        public IServerConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public TunnelState(IServerConnection conn)
        {
            this._connection = conn;
        }
    }

    delegate bool MessageHandler(PlayerState state, string message);

    class PlayerState : IClientState
    {
        public const int BUFFER_SIZE = 2000;

        byte[] _buffer = new byte[BUFFER_SIZE];
        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        IServerConnection _connection;
        public IServerConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        int _udpTunnelPort = 0;
        public int UdpTunnelPort
        {
            set
            {
                _udpTunnelPort = value;
                if (value == 0)
                    _cachedRemoteTunnelEndPoint = null;
                else
                    _cachedRemoteTunnelEndPoint = new IPEndPoint(_connection.RemoteEndPoint.Address, _udpTunnelPort);
            }
        }

        IPEndPoint _cachedRemoteTunnelEndPoint;
        public IPEndPoint RemoteTunnelEndPoint
        {
            get { return _cachedRemoteTunnelEndPoint; }
        }

        public Dictionary<string, MessageHandler> MessageProcessors;
        public Room CurrentRoom;

        public string Name = null;
        public long Ping = 0;
        public string Ssid = null;

        public long LastActTicks;

        public PlayerState(IServerConnection connection)
        {
            _connection = connection;
        }
    }

    class Room
    {
        PlayerState _master = null;

        public PlayerState Master { get { return _master; } }

        int _maxOtherPlayers = 0;
        int _maxPlayers = 0;
        public int MaxPlayers
        {
            get { return _maxPlayers; }
            set
            {
                _maxPlayers = value;
                if (_master == null)
                    _maxOtherPlayers = value;
                else
                    _maxOtherPlayers = value - 1;
            }
        }
        public int PlayerCount
        {
            get { return OtherPlayers.Count + (Master != null ? 1 : 0); }
        }

        public string Title = string.Empty;
        public string Description = string.Empty;
        public string Password = string.Empty;

        List<PlayerState> OtherPlayers = new List<PlayerState>();
        ReaderWriterLockSlim otherPlayersRWLock = new ReaderWriterLockSlim();

        public Room(PlayerState master = null)
        {
            if (master != null)
            {
                this._master = master;
                master.CurrentRoom = this;
            }
        }

        public virtual bool AddPlayer(PlayerState state)
        {
            otherPlayersRWLock.EnterWriteLock();
            try
            {
                if (OtherPlayers.Count < _maxOtherPlayers)
                {
                    OtherPlayers.Add(state);
                    return true;
                }
                else { return false; }
            }
            catch (Exception) { throw; }
            finally { otherPlayersRWLock.ExitWriteLock(); }
        }

        public virtual void RemovePlayer(PlayerState state)
        {
            otherPlayersRWLock.EnterWriteLock();
            try
            {
                OtherPlayers.Remove(state);
            }
            catch (Exception) { throw; }
            finally { otherPlayersRWLock.ExitWriteLock(); }

        }

        public void ForEach(ForEachPlayerAction action, PlayerState skip = null)
        {
            if (Master != null && Master != skip)
                action(Master);

            otherPlayersRWLock.EnterReadLock();
            try
            {
                foreach (PlayerState s in OtherPlayers)
                    if (s != skip) action(s);
            }
            catch (Exception) { throw; }
            finally { otherPlayersRWLock.ExitReadLock(); }
        }

        public List<PlayerState> GetPlayerList(PlayerState exclude = null)
        {
            List<PlayerState> list = new List<PlayerState>(PlayerCount);
            if (Master != null && Master != exclude)
                list.Add(Master);
            otherPlayersRWLock.EnterReadLock();
            try
            {
                foreach (PlayerState s in OtherPlayers)
                    if (s != exclude)
                        list.Add(s);
            }
            catch (Exception) { throw; }
            finally { otherPlayersRWLock.ExitReadLock(); }
            return list;
        }
    }

    class LobbyRoom : Room
    {
        public override bool AddPlayer(PlayerState state)
        {
            Interlocked.Exchange(ref state.LastActTicks, DateTime.UtcNow.Ticks);
            return base.AddPlayer(state);
        }
    }

    delegate void ForEachPlayerAction(PlayerState state);
}
