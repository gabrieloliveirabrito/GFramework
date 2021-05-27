using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Bases
{
    using Interfaces;

    public abstract class BasePacketWriter<TClient, TClientWrapper, TPacket>
        where TClient : class, IClient<TClient, TPacket>
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>
        where TPacket : BasePacket
    {
        public TClientWrapper Client { get; internal set; }
        public TClient Socket { get; internal set; }

        public abstract ulong ID { get; }
        public abstract bool Write(TPacket packet);
    }
}
