using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Bases;
    using Interfaces;

    public class PacketSentEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public TPacket Packet { get; private set; }

        public PacketSentEventArgs(TClient client, TPacket packet) : base(client)
        {
            Packet = packet;
        }
    }
}
