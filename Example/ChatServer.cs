using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GFramework.Bases;
using GFramework.Factories;
using GFramework.Network;
using GFramework.Network.Packets;
using GFramework.Network.EventArgs.Client;
using GFramework.Network.EventArgs.Server;

namespace Example
{
    public class ChatServer
    {
        BaseLogger logger;
        public TCPServer<StreamPacket> Server { get; private set; }

        public ChatServer()
        {
            logger = LoggerFactory.GetLogger<ChatServer>();
            Server = new TCPServer<StreamPacket>(3210);
            Server.OnClientConnected += Server_OnClientConnected;
            Server.OnServerOpened += Server_OnServerOpened;
            Server.OnServerClosed += Server_OnServerClosed;
            Server.OnServerError += Server_OnServerError;
            Server.MaximumClients = 1000;
        }

        public void Open() => Server.Open();

        public void SendMessage(string message)
        {
            if (Server.Clients.Length == 0)
                logger.LogWarning("No clients connected!");
            else
            {
                var packet = Server.CreatePacket(ulong.MaxValue);
                packet.WriteString(message);

                Server.SendToAll(packet);
            }
        }

        private void Server_OnServerOpened(object sender, ServerOpenedEventArgs<TCPServer<StreamPacket>, TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogInfo("ChatServer opened at {0}", e.Time);
        }

        private void Server_OnServerClosed(object sender, ServerClosedEventArgs<TCPServer<StreamPacket>, TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogWarning("ChatServer closed at {0}", e.Time);
        }

        private void Server_OnClientConnected(object sender, ClientConnectedEventArgs<TCPServer<StreamPacket>, TCPClient<StreamPacket>, StreamPacket> e)
        {
            Console.Title = $"Example - {Server.Clients.Length}/{Server.MaximumClients}";
            logger.LogInfo("Client {0} connected at {1}", e.Client.EndPoint, e.Time);

            e.Client.OnPacketReceived += Client_OnPacketReceived;
            e.Client.OnPacketSent += Client_OnPacketSent;
            e.Client.OnDisconnected += Client_OnDisconnected;
            e.Client.OnPingSent += Client_OnPingSent;
            e.Client.OnPingReceived += Client_OnPingReceived;
            e.Client.OnPongSent += Client_OnPongSent;
            e.Client.OnPongReceived += Client_OnPongReceived;

            //SendMessage($"Client {e.Client.EndPoint} connected!");
        }

        private void Client_OnPacketSent(object sender, PacketSentEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogInfo("Packet 0x{0:X16} has been sent to client {1} at {2}", e.Packet.ID, e.Client.EndPoint, e.Time);
        }

        private void Client_OnPacketReceived(object sender, PacketReceivedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogInfo("Packet 0x{0:X16} has been received from client {1} at {2}", e.Packet.ID, e.Client.EndPoint, e.Time);

            if (e.Packet.ID == ulong.MaxValue)
            {
                string message = e.Packet.ReadString();
                logger.LogSuccess("Received message: {0}", message);

                e.Packet.Reset();
                Server.SendToAll(e.Packet);
            }
            else
                logger.LogWarning("Unknown packet!");
        }

        private void Client_OnPingSent(object sender, PingSentEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogInfo("Ping request has been sent at {0}", e.SentAt);
        }

        private void Client_OnPingReceived(object sender, PingReceivedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogInfo("Ping request has been received at {0}", e.SentAt);
        }

        private void Client_OnPongSent(object sender, PongSentEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogInfo("Pong request has been sent at {0}, sent at {1}, delay = {2}ms", e.SentAt, e.ReceivedAt, e.Ping);
        }

        private void Client_OnPongReceived(object sender, PongReceivedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            Console.Title = $"Example - Ping {e.Ping}ms";
            logger.LogInfo("Pong request has been received at {0}, sent at {1}, delay = {2}ms", e.SentAt, e.ReceivedAt, e.Ping);
        }

        private void Client_OnDisconnected(object sender, ClientDisconnectedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogWarning("Client {0} disconnected at {1} by reason: {2}", e.Client.EndPoint, e.Time, e.Reason);
        }

        private void Server_OnServerError(object sender, ServerErrorEventArgs<TCPServer<StreamPacket>, TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogError("Error occurred on method {0} at {1}", e.Method, e.Time);
        }
    }
}
