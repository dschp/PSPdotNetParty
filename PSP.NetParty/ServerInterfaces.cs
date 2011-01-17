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
//using System.Net.Sockets;
using PspDotNetParty;

namespace PspDotNetParty.Server
{
    public interface IClientState
    {
        IServerConnection Connection { get; set; }
        byte[] Buffer { get; }
    }

    public interface IServer<Type> where Type : IClientState
    {
        void StartListening(IPEndPoint localEP, IServerHandler<Type> handler);
        void StopListening();
    }

    public interface IServerConnection
    {
        IPEndPoint RemoteEndPoint { get; }
        void Send(string message);
        void Send(byte[] data, int count);
    }

    public interface IServerHandler<Type> where Type : IClientState
    {
        Type CreateState(IServerConnection connection);
        void DisposeState(Type state);
        bool ProcessIncomingData(Type state, PacketData data);
    }
}
