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

    public interface IClient<TClient, TPacket>
        where TClient : IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        IPEndPoint EndPoint { get; set; }
        bool Connected { get; }

        event EventHandler<ClientConnectedEventArgs<TClient, TPacket>> OnConnected;
        event EventHandler<ClientDisconnectedEventArgs<TClient, TPacket>> OnDisconnected;
        event EventHandler<PacketReceivedEventArgs<TClient, TPacket>> OnPacketReceived;
        event EventHandler<PacketSentEventArgs<TClient, TPacket>> OnPacketSent;
        event EventHandler<ClientErrorEventArgs<TClient, TPacket>> OnClientError;

        bool Connect();
        bool Disconnect();

        void Send(BasePacket packet);
    }
}
