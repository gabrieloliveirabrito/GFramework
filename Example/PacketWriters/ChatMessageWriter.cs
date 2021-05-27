using GFramework.Network.Bases;
using GFramework.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.PacketWriters
{
    public class ChatMessageWriter : ChatClientWriter
    {
        public override ulong ID => ulong.MaxValue;

        public string Message { get; set; }

        public override bool Write(StreamPacket packet)
        {
            if (string.IsNullOrEmpty(Message))
                return false;

            packet.WriteString(Message);
            return true;
        }
    }
}
