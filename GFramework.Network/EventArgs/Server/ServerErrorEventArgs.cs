﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Server
{
    using Bases;
    using Interfaces;

    public class ServerErrorEventArgs<TServer, TClient, TPacket> : BaseServerEventArgs<TServer, TClient, TPacket>
        where TServer : IBaseServer<TServer, TClient, TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        public string Method { get; private set; }
        public Exception Exception { get; private set; }

        public ServerErrorEventArgs(TServer server, string method, Exception exception) : base(server)
        {
            Method = method;
            Exception = exception;
        }
    }
}
