using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GFramework.Network.Bases
{
    using Enums;
    using EventArgs.Client;
    using EventArgs.Server;
    using Interfaces;

    public abstract class BaseServerWrapper<TServer, TClient, TClientWrapper, TPacket>
        where TServer : IServer<TServer, TClient, TPacket>
        where TClient : class, IClient<TClient, TPacket>, new()
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
    {
        private IDictionary<IPEndPoint, TClientWrapper> clients;

        public TServer Socket { get; private set; }
        public IEnumerable<TClientWrapper> Clients { get => clients.Values; }

        public BaseServerWrapper(int port) : this(IPAddress.Any, port)
        {

        }

        public BaseServerWrapper(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {

        }

        public BaseServerWrapper(IPEndPoint endpoint)
        {
            Socket = (TServer)Activator.CreateInstance(typeof(TServer), endpoint);
            Initialize();
        }

        private void Initialize()
        {
            clients = new Dictionary<IPEndPoint, TClientWrapper>();

            Socket.OnClientConnected += Socket_OnClientConnected;
            Socket.OnServerOpened += Socket_OnServerOpened;
            Socket.OnServerClosed += Socket_OnServerClosed;
            Socket.OnServerError += Socket_OnServerError;
        }

        private void Socket_OnClientConnected(object sender, ClientConnectedEventArgs<TServer, TClient, TPacket> e)
        {
            lock (clients)
            {
                var wrapper = Activator.CreateInstance<TClientWrapper>();

                e.Client.OnDisconnected += Client_OnDisconnected;
                clients[e.Client.EndPoint] = wrapper;

                wrapper.Initialize(e.Client);
                OnClientConnected(wrapper);
            }
        }

        private void Client_OnDisconnected(object sender, ClientDisconnectedEventArgs<TClient, TPacket> e)
        {
            lock (clients)
            {
                if (clients.TryGetValue(e.Client.EndPoint, out TClientWrapper client))
                {
                    OnClientDisconnect(client, e.Reason);
                    clients.Remove(e.Client.EndPoint);
                }
            }
        }

        private void Socket_OnServerOpened(object sender, ServerOpenedEventArgs<TServer, TClient, TPacket> e)
        {
            OnServerOpened();
        }

        private void Socket_OnServerClosed(object sender, ServerClosedEventArgs<TServer, TClient, TPacket> e)
        {
            lock (clients)
            {
                clients.Clear();
            }
            OnServerClosed();
        }

        private void Socket_OnServerError(object sender, ServerErrorEventArgs<TServer, TClient, TPacket> e)
        {
            OnServerError(e.Method, e.Exception);
        }

        protected virtual void OnServerOpened() { }
        protected virtual void OnServerClosed() { }
        protected virtual void OnServerError(string method, Exception exception) { }
        protected virtual void OnClientConnected(TClientWrapper client) { }
        protected virtual void OnClientDisconnect(TClientWrapper client, DisconnectReason reason) { }

        public bool Open() => Socket.Open();
        public bool Close() => Socket.Close();

        public void SendToAll<TPacketWriter>(TPacketWriter writer)
            where TPacketWriter : BasePacketWriter<TClient, TClientWrapper, TPacket>
        {
            lock (clients)
            {
                foreach (var client in clients.Values)
                    client.Send(writer);
            }
        }
    }
}
