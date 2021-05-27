using GFramework.Network;
using GFramework.Network.Bases;
using GFramework.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.PacketWriters
{
    public abstract class ChatClientWriter : BasePacketWriter<TCPClient<StreamPacket>, ChatClientWrapper, StreamPacket>
    {

    }
}
