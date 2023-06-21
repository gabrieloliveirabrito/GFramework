using GFramework.Network.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Interfaces
{
    public interface IAsyncClient<TClient, TPacket> : IBaseClient<TClient, TPacket>
        where TClient : class, IAsyncClient<TClient, TPacket>, new()
        where TPacket : BasePacket
    {
        Task<bool> Connect();
        Task<bool> Disconnect();

        Task Send(TPacket packet);

        Task Ping();

        TPacket CreatePacket(ulong id);
    }
}
