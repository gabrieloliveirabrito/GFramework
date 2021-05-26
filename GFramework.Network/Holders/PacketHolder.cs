using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Holders
{
    using Enums;
    using Bases;

    public class PacketHolder
    {
        public PacketHolder()
        {
            Header = new byte[Constants.PacketHeaderSize];
            Chunk = new byte[Constants.PacketChunkLength];

            HeaderHandled = 0;
            BufferHandled = 0;
        }

        public PacketHolder(BasePacket packet) : this()
        {
            Packet = packet;

            var lengthBuffer = BitConverter.GetBytes(packet.Length);
            var idBuffer = BitConverter.GetBytes(packet.ID);

            Header[0] = (byte)PacketEvent.Receive;
            System.Buffer.BlockCopy(lengthBuffer, 0, Header, 1, lengthBuffer.Length);
            System.Buffer.BlockCopy(idBuffer, 0, Header, 1 + lengthBuffer.Length, idBuffer.Length);

            Buffer = packet.Data;
        }

        public BasePacket Packet { get; set; }

        public byte[] Header { get; set; }
        public int HeaderHandled { get; set; }

        public PacketEvent Event => (PacketEvent)Header[0];
        public long Length => BitConverter.ToInt64(Header, 1);
        public ulong ID => BitConverter.ToUInt64(Header, 1 + sizeof(long));

        public byte[] Chunk { get; set; }
        public int ChunkHandled { get; set; }

        public byte[] Buffer { get; set; } 
        public int BufferHandled { get; set; }
    }
}
