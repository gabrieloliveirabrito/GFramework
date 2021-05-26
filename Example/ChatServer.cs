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
        TCPServer<StreamPacketReader> server;

        public ChatServer()
        {
            logger = LoggerFactory.GetLogger<ChatServer>();
            server = new TCPServer<StreamPacketReader>(3210);
            server.OnClientConnected += Server_OnClientConnected;
            server.OnServerOpened += Server_OnServerOpened;
            server.OnServerClosed += Server_OnServerClosed;
            server.OnServerError += Server_OnServerError;
            server.Open();
        }

        public void SendMessage(string message)
        {
            var packet = server.CreatePacket(ulong.MaxValue);
            packet.WriteString(message);

            server.SendToAll(packet);
        }

        private void Server_OnServerOpened(object sender, ServerOpenedEventArgs<TCPServer<StreamPacketReader>, TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogInfo("ChatServer opened at {0}", e.Time);
        }

        private void Server_OnServerClosed(object sender, ServerClosedEventArgs<TCPServer<StreamPacketReader>, TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogWarning("ChatServer closed at {0}", e.Time);
        }

        private void Server_OnClientConnected(object sender, ClientConnectedEventArgs<TCPServer<StreamPacketReader>, TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogInfo("Client {0} connected at {1}", e.Client.EndPoint, e.Time);

            e.Client.OnPacketReceived += Client_OnPacketReceived;
            e.Client.OnPacketSent += Client_OnPacketSent;
            e.Client.OnDisconnected += Client_OnDisconnected;

            SendMessage($"Client {e.Client.EndPoint} connected!");
        }

        private void Client_OnPacketSent(object sender, PacketSentEventArgs<TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogInfo("Packet 0x{0:X16} has been sent to client {1} at {2}", e.Packet.ID, e.Client.EndPoint, e.Time);
        }

        private void Client_OnPacketReceived(object sender, PacketReceivedEventArgs<TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogInfo("Packet 0x{0:X16} has been received from client {1} at {2}", e.Packet.ID, e.Client.EndPoint, e.Time);

            if (e.Packet.ID == ulong.MaxValue)
            {
                string message = e.Packet.ReadString();
                logger.LogSuccess("Received message: {0}", message);

                e.Packet.Reset();
                server.SendToAll(e.Packet);
            }
            else
                logger.LogWarning("Unknown packet!");
        }

        private void Client_OnDisconnected(object sender, ClientDisconnectedEventArgs<TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogWarning("Client {0} disconnected at {1} by reason: {2}", e.Client.EndPoint, e.Time, e.Reason);
        }

        private void Server_OnServerError(object sender, ServerErrorEventArgs<TCPServer<StreamPacketReader>, TCPClient<StreamPacketReader>, StreamPacketReader> e)
        {
            logger.LogError("Error occurred on method {0} at {1}", e.Method, e.Time);
        }
    }
}
