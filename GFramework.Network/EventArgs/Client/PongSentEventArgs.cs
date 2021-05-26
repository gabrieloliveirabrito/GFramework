using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Interfaces;
    using Bases;

    public class PongSentEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public DateTime SentAt { get; private set; }
        public DateTime ReceivedAt { get; private set; }
        public double Ping => (ReceivedAt - SentAt).TotalMilliseconds;

        public PongSentEventArgs(TClient client, DateTime sentAt, DateTime receivedAt) : base(client)
        {
            SentAt = sentAt;
            ReceivedAt = receivedAt;
        }
    }
}
