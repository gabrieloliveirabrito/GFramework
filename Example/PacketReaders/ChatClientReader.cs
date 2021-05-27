using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GFramework.Network;
using GFramework.Network.Bases;
using GFramework.Network.Packets;

namespace Example.PacketReaders
{
    public abstract class ChatClientReader : BasePacketReader<TCPClient<StreamPacket>, ChatClientWrapper, StreamPacket>
    {
    }
}
