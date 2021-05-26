﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Bases;
    using Interfaces;

    public class PacketReceivedEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public BasePacket Packet { get; private set; }

        public PacketReceivedEventArgs(TClient client, BasePacket packet) : base(client)
        {
            Packet = packet;
        }
    }
}
