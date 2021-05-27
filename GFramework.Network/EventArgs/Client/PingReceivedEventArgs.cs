using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Interfaces;
    using Bases;

    public class PingReceivedEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public DateTime SentAt { get; private set; }

        public PingReceivedEventArgs(TClient client, DateTime sentAt) : base(client)
        {
            SentAt = sentAt;
        }
    }
}
