﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Bases;
    using Interfaces;

    public class ClientConnectedEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public ClientConnectedEventArgs(TClient client) : base(client)
        {
        }
    }
}