using GFramework.Bases;
using GFramework.Factories;
using GFramework.Network.Bases;
using GFramework.Network.Enums;
using GFramework.Network.EventArgs.Client;
using GFramework.Network.EventArgs.Server;
using GFramework.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GFramework.Network
{
    public class AsyncTCPServer<TPacket> : IAsyncServer<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>
        where TPacket : BasePacket
    {
        private BaseLogger logger;
        private IPEndPoint endPoint;
        private List<AsyncTCPClient<TPacket>> clients;
        private Task wrapperTask;
        private CancellationTokenSource cancellationToken;

        public event EventHandler<ServerOpenedEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>> OnServerOpened;
        public event EventHandler<ServerClosedEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>> OnServerClosed;
        public event EventHandler<ServerErrorEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>> OnServerError;
        public event EventHandler<ClientConnectedEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>> OnClientConnected;

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

        public AsyncTCPClient<TPacket>[] Clients
        {
            get => clients.ToArray();
        }

        private uint maximumClients = Constants.MaximumActiveConnections;
        public uint MaximumClients
        {
            get => maximumClients;
            set
            {
                if (Listening)
                    throw new InvalidOperationException("Cannot change the EndPoint of server while is listening.");
                maximumClients = value;
            }
        }

        public AsyncTCPServer(int port) : this(IPAddress.Any, port)
        {

        }

        public AsyncTCPServer(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {

        }

        public AsyncTCPServer(IPEndPoint endpoint)
        {
            logger = LoggerFactory.GetLogger(this);
            clients = new List<AsyncTCPClient<TPacket>>();
            endPoint = endpoint;
        }

        public async Task<bool> Open()
        {
            try
            {
                if (Listening)
                    throw new InvalidOperationException("Cannot start the socket because is already listening!");

                Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket.Bind(EndPoint);
                Socket.Listen(1);

                cancellationToken = new CancellationTokenSource();
                wrapperTask = new Task(WrapperProc, cancellationToken.Token, TaskCreationOptions.LongRunning);
                wrapperTask.Start();

                InvokeOnServerOpened();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("Open", ex);

                return false;
            }
        }

        public async Task<bool> Close()
        {
            try
            {
                await DisconnectAll();
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();

                InvokeOnServerClosed();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("Close", ex);

                return false;
            }
        }

        protected void InvokeOnServerOpened()
        {
            if (OnServerOpened != null)
                OnServerOpened(this, new ServerOpenedEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>(this));
        }

        protected void InvokeOnServerClosed()
        {
            if (OnServerClosed != null)
                OnServerClosed(this, new ServerClosedEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>(this));
        }

        protected void InvokeOnServerError(string method, Exception exception)
        {
            if (OnServerError != null)
                OnServerError(this, new ServerErrorEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>(this, method, exception));
        }

        protected void InvokeOnClientConnected(AsyncTCPClient<TPacket> client)
        {
            if (OnClientConnected != null)
                OnClientConnected(this, new ClientConnectedEventArgs<AsyncTCPServer<TPacket>, AsyncTCPClient<TPacket>, TPacket>(this, client));
        }

        private void Client_OnDisconnected(object sender, ClientDisconnectedEventArgs<AsyncTCPClient<TPacket>, TPacket> e)
        {
            if (e.Reason != DisconnectReason.ServerShutdown)
                clients.Remove(e.Client);
        }

        public TPacket CreatePacket(ulong id)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id);
        }

        public async Task SendToAll(TPacket packet)
        {
            try
            {
                foreach (AsyncTCPClient<TPacket> client in Clients)
                    await client.Send(packet);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("SendToAll", ex);
            }
        }

        public async Task DisconnectAll()
        {
            try
            {
                foreach (AsyncTCPClient<TPacket> client in Clients)
                    await client.Disconnect();//DisconnectReason.ServerShutdown);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnServerError("DisconnectAll", ex);
            }
        }

        

        private async void WrapperProc()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    var socket = await Socket.AcceptAsync();
                    var client = new AsyncTCPClient<TPacket>(socket);
                    client.Initialize();

                    InvokeOnClientConnected(client);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogFatal(ex);
                }
                finally
                {
                }
            }

            await DisconnectAll();
        }
    }
}