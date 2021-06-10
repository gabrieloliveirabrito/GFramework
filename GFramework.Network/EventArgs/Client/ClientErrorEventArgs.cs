using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs.Client
{
    using Bases;
    using Interfaces;

    public class ClientErrorEventArgs<TClient, TPacket> : BaseClientEventArgs<TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        public string Method { get; private set; }
        public Exception Exception { get; private set; }

        public ClientErrorEventArgs(TClient client, string method, Exception exception) : base(client)
        {
            Method = method;
            Exception = exception;
        }
    }
}
