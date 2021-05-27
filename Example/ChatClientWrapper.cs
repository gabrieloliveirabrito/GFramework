using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using GFramework.Bases;
using GFramework.Factories;
using GFramework.Network;
using GFramework.Network.Bases;
using GFramework.Network.Packets;
using GFramework.Network.Enums;
using Example.PacketReaders;

namespace Example
{
    public class ChatClientWrapper : BaseClientWrapper<TCPClient<StreamPacket>, ChatClientWrapper, StreamPacket>
    {
        private BaseLogger logger;
        public ChatClientWrapper() : base(IPAddress.Parse("127.0.0.1"), 3210)
        {
            logger = LoggerFactory.GetLogger(this);
            RegisterReader(new ChatMessageReader());
        }

        protected override void OnConnected()
        {
            logger.LogSuccess("Client connected to {0}!", Socket.EndPoint);
        }

        protected override void OnDisconnected(DisconnectReason reason)
        {
            logger.LogWarning("Client {0} disconnected by reason: {1}", Socket.EndPoint, reason);
        }

        protected override void OnPacketReceived(StreamPacket packet)
        {
            logger.LogInfo("Packet 0x{0:X16} has been received from client {1}", packet.ID, Socket.EndPoint);
        }

        protected override void OnPacketSent(StreamPacket packet)
        {
            logger.LogInfo("Packet 0x{0:X16} has been sent to client {1}", packet.ID, Socket.EndPoint);
        }

        protected override void OnSocketError(string method, Exception exception)
        {
            logger.LogError("Error occurred on method {0}", method);
        }
    }
}
