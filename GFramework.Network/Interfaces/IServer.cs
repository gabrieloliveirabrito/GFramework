using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    using Bases;
    using EventArgs.Server;

    public interface IServer<TServer, TClient, TPacket> : IBaseServer<TServer, TClient, TPacket>
        where TServer : IServer<TServer, TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        bool Open();
        bool Close();

        void SendToAll(TPacket packet);
        void DisconnectAll();
    }
}