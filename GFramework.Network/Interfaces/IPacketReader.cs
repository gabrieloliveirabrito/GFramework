using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    using Bases;

    public interface IPacketReader<TPacket>
        where TPacket : BasePacket
    {
        ulong ID { get; }

        bool Read(TPacket packet);
        void Execute();
    }
}
