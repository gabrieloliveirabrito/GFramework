using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using GFramework.Bases;
using GFramework.Factories;

namespace GFramework.Network
{
    using Bases;
    using Enums;
    using EventArgs.Client;
    using EventArgs.Server;
    using Interfaces;

    public class TCPServer<TPacket> : IServer<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>
        where TPacket : BasePacket
    {
        private BaseLogger logger;
        private IPEndPoint endPoint;
        private List<TCPClient<TPacket>> clients;

        public event EventHandler<ServerOpenedEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>> OnServerOpened;
        public event EventHandler<ServerClosedEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>> OnServerClosed;
        public event EventHandler<ServerErrorEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>> OnServerError;
        public event EventHandler<ClientConnectedEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>> OnClientConnected;

        public IPEndPoint EndPoint
        {
            get => endPoint;
            set
            {
                if (Listening)
                    throw new InvalidOperationException("Cannot change the EndPoint of server while is listening.");
                endPoint = value;
            }
        }

        public Socket Socket
        {
            get; private set;
        }

        public bool Listening
        {
            get => Socket != null && Socket.IsBound;
        }

        public TCPClient<TPacket>[] Clients
        {
            get => clients.ToArray(); 
        }

        public bool CloseOnError { get; set; }

        public TCPServer(int port) : this(IPAddress.Any, port)
        {

        }

        public TCPServer(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {
            
        }

        public TCPServer(IPEndPoint endpoint)
        {
            CloseOnError = false;

            logger = LoggerFactory.GetLogger(this);
            clients = new List<TCPClient<TPacket>>();
            endPoint = endpoint;
        }

        public bool Open()
        {
            try
            {
                if (Listening)
                    throw new InvalidOperationException("Cannot start the socket because is already listening!");

                Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket.Bind(EndPoint);
                Socket.Listen(1);

                BeginAccept();
                InvokeOnServerOpened();
                return true;
            }
            catch(Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("Open", ex);

                return false;
            }
        }

        public bool Close()
        {
            try
            {
                DisconnectAll();
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();

                InvokeOnServerClosed();
                return true;
            }
            catch(Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("Close", ex);

                return false;
            }
        }

        protected void InvokeOnServerOpened()
        {
            if (OnServerOpened != null)
                OnServerOpened(this, new ServerOpenedEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>(this));
        }

        protected void InvokeOnServerClosed()
        {
            if (OnServerClosed != null)
                OnServerClosed(this, new ServerClosedEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>(this));
        }

        protected void InvokeOnServerError(string method, Exception exception)
        {
            if (OnServerError != null)
                OnServerError(this, new ServerErrorEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>(this, method, exception));
        }

        protected void InvokeOnClientConnected(TCPClient<TPacket> client)
        {
            if (OnClientConnected != null)
                OnClientConnected(this, new ClientConnectedEventArgs<TCPServer<TPacket>, TCPClient<TPacket>, TPacket>(this, client));
        }

        private void BeginAccept()
        {
            Socket.BeginAccept(EndAccept, null);
        }

        private void EndAccept(IAsyncResult result)
        {
            try
            {
                Socket socket = Socket.EndAccept(result);

                //TODO: Disconnect by Maximum Clients

                TCPClient<TPacket> client = new TCPClient<TPacket>(socket);
                clients.Add(client);
                InvokeOnClientConnected(client);

                client.OnDisconnected += Client_OnDisconnected;
                client.Initialize();

                BeginAccept();
            }
            catch(Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("EndAccept", ex);
            }
        }

        private void Client_OnDisconnected(object sender, ClientDisconnectedEventArgs<TCPClient<TPacket>, TPacket> e)
        {
            if (e.Reason != DisconnectReason.ServerShutdown)
                clients.Remove(e.Client);
        }

        public TPacket CreatePacket(ulong id)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id);
        }

        public void SendToAll(BasePacket packet)
        {
            try
            {
                foreach (TCPClient<TPacket> client in Clients)
                    client.Send(packet);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("SendToAll", ex);
            }
        }

        public void DisconnectAll()
        {
            try
            {
                foreach (TCPClient<TPacket> client in Clients)
                    client.Disconnect(DisconnectReason.ServerShutdown);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("DisconnectAll", ex);
            }
        }
    }
}
