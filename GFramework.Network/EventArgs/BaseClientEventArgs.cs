using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.EventArgs
{
    using Bases;
    using Interfaces;

    public class BaseClientEventArgs<TClient, TPacket> : System.EventArgs
        where TClient : class, IClient<TClient, TPacket>
        where TPacket : BasePacket
    {
        public DateTime Time { get; set; }
        public TClient Client { get; set; }

        public BaseClientEventArgs(TClient client)
        {
            Time = DateTime.Now;
            Client = client;
        }
    }
}
