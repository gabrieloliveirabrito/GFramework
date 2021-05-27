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
using System.Net;

namespace Example
{
    public class ChatClient
    {
        BaseLogger logger;
        TCPClient<StreamPacket> client;

        public ChatClient()
        {
            logger = LoggerFactory.GetLogger<ChatClient>();
            client = new TCPClient<StreamPacket>(IPAddress.Parse("192.168.1.168"), 3210);

            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnClientError += Client_OnClientError;
            client.OnPacketReceived += Client_OnPacketReceived;
            client.OnPacketSent += Client_OnPacketSent;
            client.OnPingSent += Client_OnPingSent;
            client.OnPingReceived += Client_OnPingReceived;
            client.OnPongSent += Client_OnPongSent;
            client.OnPongReceived += Client_OnPongReceived;
        }

        public void Connect()
        {
            if (client.Connect())
                logger.LogSuccess("Connect request has been sent! Response on event!");
            else
                logger.LogError("Failed to sent connect request!");
        }

        public void Ping() => client.Ping();

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
            logger.LogInfo("Pong request has been received at {0}, sent at {1}, delay = {2}ms", e.SentAt, e.ReceivedAt, e.Ping);
        }

        private void Client_OnPongReceived(object sender, PongReceivedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            Console.Title = $"Example - Ping {e.Ping}ms";
            logger.LogInfo("Pong request has been received at {0}, sent at {1}, delay = {2}ms", e.SentAt, e.ReceivedAt, e.Ping);
        }

        public void SendMessage(string message)
        {
            var packet = client.CreatePacket(ulong.MaxValue);
            packet.WriteString(message);

            client.Send(packet);
        }

        private void Client_OnConnected(object sender, ClientConnectedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogSuccess("Client connected to {0} at {1}!", e.Client.EndPoint, e.Time);
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
            }
            else
                logger.LogWarning("Unknown packet!");
        }

        private void Client_OnDisconnected(object sender, ClientDisconnectedEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogWarning("Client {0} disconnected at {1} by reason: {2}", e.Client.EndPoint, e.Time, e.Reason);
        }

        private void Client_OnClientError(object sender, ClientErrorEventArgs<TCPClient<StreamPacket>, StreamPacket> e)
        {
            logger.LogError("Error occurred on method {0} at {1}", e.Method, e.Time);
        }
    }
}
