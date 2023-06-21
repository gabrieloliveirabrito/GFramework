using GFramework.Network.Bases;
using GFramework.Network.EventArgs.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    public interface IBaseClient<TClient, TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        event EventHandler<ClientConnectedEventArgs<TClient, TPacket>> OnConnected;
        event EventHandler<ClientDisconnectedEventArgs<TClient, TPacket>> OnDisconnected;
        event EventHandler<ClientErrorEventArgs<TClient, TPacket>> OnClientError;
        event EventHandler<PacketSentEventArgs<TClient, TPacket>> OnPacketSent;
        event EventHandler<PacketReceivedEventArgs<TClient, TPacket>> OnPacketReceived;
        event EventHandler<PingSentEventArgs<TClient, TPacket>> OnPingSent;
        event EventHandler<PingReceivedEventArgs<TClient, TPacket>> OnPingReceived;
        event EventHandler<PongSentEventArgs<TClient, TPacket>> OnPongSent;
        event EventHandler<PongReceivedEventArgs<TClient, TPacket>> OnPongReceived;
        
        IPEndPoint EndPoint { get; set; }
        bool Connected { get; }
    }
}
