using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Holders
{
    using Enums;
    using Bases;

    public class EventHolder
    {
        public EventHolder()
        {
            EventBuffer = new byte[1];
            EventHandled = 0;
        }

        public EventHolder(PacketEvent packetEvent) : this()
        {
            EventBuffer[0] = (byte)packetEvent;
        }

        public byte[] EventBuffer { get; set; }
        public int EventHandled { get; set; }

        public PacketEvent Event => (PacketEvent)EventBuffer[0];
        public BasePacket Packet { get; set; }
    }
}
