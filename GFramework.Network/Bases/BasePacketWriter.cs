using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Bases
{
    using Interfaces;

    public abstract class BasePacketWriter<TClient, TClientWrapper, TPacket> : IPacketWriter<TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
    {
        public TClientWrapper Client { get; internal set; }
        public TClient Socket { get; internal set; }

        public abstract ulong ID { get; }
        public abstract bool Write(TPacket packet);
    }

    public abstract class BasePacketWriter<TClient, TClientWrapper, TPacket, TPacketID> : BasePacketWriter<TClient, TClientWrapper, TPacket>, IPacketWriter<TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
        where TPacketID : struct, Enum
    {
        public override ulong ID => Convert.ToUInt64(PacketID);

        public abstract TPacketID PacketID { get; }
    }
}
