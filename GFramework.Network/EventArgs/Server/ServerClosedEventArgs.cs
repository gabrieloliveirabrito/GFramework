using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Server
{
    using Bases;
    using Interfaces;

    public class ServerClosedEventArgs<TServer, TClient, TPacket> : BaseServerEventArgs<TServer, TClient, TPacket>
        where TServer : IBaseServer<TServer, TClient, TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        public ServerClosedEventArgs(TServer server) : base(server)
        {

        }
    }
}