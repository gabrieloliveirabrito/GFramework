using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Enums
{
    public enum PacketEvent : byte
    {
        Unknown = 0x00,
        Receive = 0x01,
        Ping = 0x02,
        Pong = 0x03,
        ServerShutdown = 0x04,
        MaximumClientsReached = 0x5,
    }
}
