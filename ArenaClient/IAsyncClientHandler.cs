using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MHPTunnelG;

namespace ArenaClient
{
    interface IAsyncClientHandler
    {
        void ConnectCallback(IPEndPoint localEndPoint);
        void ReadCallback(PacketData data);
        void SendCallback(int bytesSent);
        void DisconnectCallback();
        void Log(string message);
    }
}
