using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GFramework.Bases;
using GFramework.Factories;
using GFramework.Network;
using GFramework.Network.Bases;
using GFramework.Network.Packets;
using GFramework.Network.Enums;
using Example.PacketReaders;

namespace Example
{
    public class ChatServerWrapper : BaseServerWrapper<TCPServer<StreamPacket>, TCPClient<StreamPacket>, ChatClientWrapper, StreamPacket>
    {
        BaseLogger logger;
        public ChatServerWrapper() : base(3210)
        {
            logger = LoggerFactory.GetLogger(this);
        }

        protected override void OnServerOpened()
        {
            logger.LogInfo("ChatServer opened");
        }

        protected override void OnServerClosed()
        {
            logger.LogWarning("ChatServer closed");
        }

        protected override void OnServerError(string method, Exception exception)
        {
            logger.LogError("Error occurred on method {0}", method);
        }

        protected override void OnClientConnected(ChatClientWrapper client)
        {
            Console.Title = $"Example - {Socket.Clients.Length}/{Socket.MaximumClients}";
            logger.LogInfo("Client {0} connected", client.Socket.EndPoint);
        }

        protected override void OnClientDisconnect(ChatClientWrapper client, DisconnectReason reason)
        {
            logger.LogWarning("Client {0} disconnected by reason: {1}", client.Socket.EndPoint, reason);
        }
    }
}
