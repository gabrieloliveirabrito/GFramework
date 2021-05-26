using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Server
{
    using Bases;
    using Interfaces;

    public class ServerOpenedEventArgs<TServer, TClient, TPacket> : BaseServerEventArgs<TServer, TClient, TPacket>
        where TServer : IServer<TServer, TClient, TPacket>
        where TClient : IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public ServerOpenedEventArgs(TServer server) : base(server)
        {

        }
    }
}
