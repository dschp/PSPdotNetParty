using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
//using System.Net.Sockets;
using MHPTunnelG;

namespace ArenaServer
{
    interface IClientState
    {
        IServerConnection Connection { get; set; }
        byte[] Buffer { get; }
    }

    interface IServer<Type> where Type : IClientState
    {
        void StartListening(IPEndPoint localEP, IServerHandler<Type> handler);
        void StopListening();
    }

    interface IServerConnection
    {
        IPEndPoint RemoteEndPoint { get; }
        void Send(string message);
        void Send(byte[] data, int count);
    }

    interface IServerHandler<Type> where Type : IClientState
    {
        Type CreateState(IServerConnection connection);
        void DisposeState(Type state);
        bool ProcessIncomingData(Type state, PacketData data);
    }
}
