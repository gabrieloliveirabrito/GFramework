using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    using Bases;

    public interface IPacketWriter<TPacket>
        where TPacket : BasePacket
    {
        ulong ID { get; }

        bool Write(TPacket packet);
    }
}
