using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network
{
    public static class Constants
    {
        public const int MaxPacketLength = 1024 * 100;
        public const int MaxIncomingConnections = 128;
        public const int PacketHeaderSize = 1 + sizeof(long) + sizeof(ulong);
        public const int PacketChunkLength = 1024 * 50;
    }
}
