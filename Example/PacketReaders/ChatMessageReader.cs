using GFramework.Factories;
using GFramework.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.PacketReaders
{
    public class ChatMessageReader : ChatClientReader
    {
        public override ulong ID => ulong.MaxValue;

        string message;
        public override bool Read(StreamPacket packet)
        {
            message = packet.ReadString();
            return !string.IsNullOrEmpty(message);
        }

        public override void Execute()
        {
            LoggerFactory.GetLogger(this).LogSuccess("Received message: {0}", message);
        }
    }
}
