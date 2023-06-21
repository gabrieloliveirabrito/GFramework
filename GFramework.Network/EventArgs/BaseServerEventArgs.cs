using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs
{
    using Bases;
    using Interfaces;

    public class BaseServerEventArgs<TServer, TClient, TPacket> : System.EventArgs
        where TServer : IBaseServer<TServer, TClient, TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        public DateTime Time { get; set; }
        public TServer Server { get; set; }

        public BaseServerEventArgs(TServer server)
        {
            Time = DateTime.Now;
            Server = server;
        }
    }
}
