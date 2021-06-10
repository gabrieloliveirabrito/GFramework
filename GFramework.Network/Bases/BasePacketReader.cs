using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Bases
{
    using Interfaces;

    public abstract class BasePacketReader<TClient, TClientWrapper, TPacket> : IPacketReader<TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
    {
        public TClientWrapper Client { get; internal set; }
        public TClient Socket { get; internal set; }


        public virtual ulong ID { get; }
        public abstract bool Read(TPacket packet);
        public abstract void Execute();
    }

    public abstract class BasePacketReader<TClient, TClientWrapper, TPacket, TPacketID> : BasePacketReader<TClient, TClientWrapper, TPacket>, IPacketReader<TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
        where TPacketID : struct, Enum
    {
        public override ulong ID => Convert.ToUInt64(PacketID);
        public abstract TPacketID PacketID { get; }
    }
}
