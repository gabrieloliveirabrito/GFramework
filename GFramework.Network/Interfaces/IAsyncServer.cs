using GFramework.Network.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    public interface IAsyncServer<TServer, TClient, TPacket> : IBaseServer<TServer, TClient, TPacket>
        where TServer : IBaseServer<TServer, TClient, TPacket>
        where TClient : class, IAsyncClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        Task<bool> Open();
        Task<bool> Close();

        Task SendToAll(TPacket packet);
        Task DisconnectAll();
    }
}
