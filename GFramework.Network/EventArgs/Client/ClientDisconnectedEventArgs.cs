using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Bases;
    using Enums;
    using Interfaces;

    public class ClientDisconnectedEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        public DisconnectReason Reason { get; private set; }

        public ClientDisconnectedEventArgs(TClient client, DisconnectReason reason) : base(client)
        {
            Reason = reason;
        }
    }
}
