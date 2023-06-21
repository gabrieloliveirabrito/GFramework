using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    using Bases;
    using EventArgs.Server;

    public interface IBaseServer<TServer, TClient, TPacket>
        where TServer : IBaseServer<TServer, TClient, TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        IPEndPoint EndPoint { get; set; }
        bool Listening { get; }
        TClient[] Clients { get; }
        uint MaximumClients { get; set; }

        event EventHandler<ServerOpenedEventArgs<TServer, TClient, TPacket>> OnServerOpened;
        event EventHandler<ServerClosedEventArgs<TServer, TClient, TPacket>> OnServerClosed;
        event EventHandler<ServerErrorEventArgs<TServer, TClient, TPacket>> OnServerError;
        event EventHandler<ClientConnectedEventArgs<TServer, TClient, TPacket>> OnClientConnected;
    }
}