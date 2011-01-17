using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ArenaClient
{
    interface IAsyncClient
    {
        void Connect(string ipAddr, int port);
        void Connect(IPEndPoint remoteEP);
        void Send(byte[] data);
        void Send(string message);
        void Disconnect();
        bool Connected { get; }
    }
}
