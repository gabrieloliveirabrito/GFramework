using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GFramework.Network.Interfaces
{
    using Bases;
    using EventArgs.Client;

    public interface IClient<TClient, TPacket> : IBaseClient<TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        bool Connect();
        bool Disconnect();

        void Send(TPacket packet);
        void Ping();

        TPacket CreatePacket(ulong id);
    }
}
