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

    public interface IServer<TServer, TClient, TPacket>
        where TServer : IServer<TServer, TClient, TPacket>
        where TClient : IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        IPEndPoint EndPoint { get; set; }
        bool Listening { get; }
        TClient[] Clients { get; }

        event EventHandler<ServerOpenedEventArgs<TServer, TClient, TPacket>> OnServerOpened;
        event EventHandler<ServerClosedEventArgs<TServer, TClient, TPacket>> OnServerClosed;
        event EventHandler<ServerErrorEventArgs<TServer, TClient, TPacket>> OnServerError;
        event EventHandler<ClientConnectedEventArgs<TServer, TClient, TPacket>> OnClientConnected;

        bool Open();
        bool Close();

        void SendToAll(TPacket packet);
        void DisconnectAll();
    }
}