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

            System.Buffer.BlockCopy(lengthBuffer, 0, Header, 0, lengthBuffer.Length);
            System.Buffer.BlockCopy(idBuffer, 0, Header, lengthBuffer.Length, idBuffer.Length);

            Buffer = packet.Data;
        }

        public BasePacket Packet { get; set; }
        public PacketEvent Event { get; set; }

        public byte[] Header { get; set; }
        public int HeaderHandled { get; set; }

        public long Length => BitConverter.ToInt64(Header, 0);
        public ulong ID => BitConverter.ToUInt64(Header, sizeof(long));

        public byte[] Chunk { get; set; }
        public int ChunkHandled { get; set; }

        public byte[] Buffer { get; set; } 
        public int BufferHandled { get; set; }
    }
}
