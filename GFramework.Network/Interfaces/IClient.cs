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
        event EventHandler<ClientErrorEventArgs<TClient, TPacket>> OnClientError;
        event EventHandler<PacketSentEventArgs<TClient, TPacket>> OnPacketSent;
        event EventHandler<PacketReceivedEventArgs<TClient, TPacket>> OnPacketReceived;
        event EventHandler<PingSentEventArgs<TClient, TPacket>> OnPingSent;
        event EventHandler<PingReceivedEventArgs<TClient, TPacket>> OnPingReceived;
        event EventHandler<PongSentEventArgs<TClient, TPacket>> OnPongSent;
        event EventHandler<PongReceivedEventArgs<TClient, TPacket>> OnPongReceived;

        bool Connect();
        bool Disconnect();

        void Send(TPacket packet);
        void Ping();
    }
}
