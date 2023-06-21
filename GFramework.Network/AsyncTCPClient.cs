using GFramework.Bases;
using GFramework.Factories;
using GFramework.Network.Bases;
using GFramework.Network.Enums;
using GFramework.Network.EventArgs.Client;
using GFramework.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GFramework.Network
{
    public static class AsyncTCPClientExtensions
    {
        public static async Task SendAsync<TClientWrapper, TPacket, TPacketWriter>(this BaseClientWrapper<AsyncTCPClient<TPacket>, TClientWrapper, TPacket> clientWrapper, TPacketWriter writer)
        where TClientWrapper : BaseClientWrapper<AsyncTCPClient<TPacket>, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
        where TPacketWriter : BasePacketWriter<AsyncTCPClient<TPacket>, TClientWrapper, TPacket>
        {
            var packet = clientWrapper.Socket.CreatePacket(writer.ID);
            if (writer.Write(packet))
                await clientWrapper.Socket.Send(packet);
        }
    }

    public class AsyncTCPClient<TPacket> : IAsyncClient<AsyncTCPClient<TPacket>, TPacket>
        where TPacket : BasePacket
    {
        private BaseLogger logger;
        private ManualResetEvent sendLock;
        private IPEndPoint endPoint;
        private Task wrapperTask;
        private CancellationTokenSource cancellationToken;
        private NetworkStream stream;
        private byte[] receiveBuffer = new byte[Constants.MaxPacketLength];
        private byte[] sendBuffer = new byte[Constants.MaxPacketLength];
        private byte[] receiveChunk = new byte[Constants.PacketChunkLength];
        private byte[] sendChunk = new byte[Constants.PacketChunkLength];
        private int receivePosition = 0, sendPosition = 0;

        public event EventHandler<ClientConnectedEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnConnected;
        public event EventHandler<ClientDisconnectedEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnDisconnected;
        public event EventHandler<PacketReceivedEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnPacketReceived;
        public event EventHandler<PacketSentEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnPacketSent;
        public event EventHandler<ClientErrorEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnClientError;
        public event EventHandler<PingSentEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnPingSent;
        public event EventHandler<PingReceivedEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnPingReceived;
        public event EventHandler<PongSentEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnPongSent;
        public event EventHandler<PongReceivedEventArgs<AsyncTCPClient<TPacket>, TPacket>> OnPongReceived;

        public IPEndPoint EndPoint
        {
            get => endPoint;
            set
            {
                if (Connected)
                    throw new InvalidOperationException("Cannot change the EndPoint of client while is connected.");
                endPoint = value;
            }
        }

        public Socket Socket
        {
            get; private set;
        }

        public bool Connected
        {
            get => Socket != null && Socket.Connected;
        }

        public bool DebugPackets { get; set; }

        public AsyncTCPClient()
        {
            logger = LoggerFactory.GetLogger(this);
            sendLock = new ManualResetEvent(true);
        }

        public AsyncTCPClient(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {

        }

        public AsyncTCPClient(IPEndPoint endpoint) : this()
        {
            endPoint = endpoint;
        }

        protected internal AsyncTCPClient(Socket socket) : this()
        {
            EndPoint = (IPEndPoint)socket.RemoteEndPoint;
            Socket = socket;
        }

        protected internal void Initialize()
        {
            InvokeOnConnected();

            stream = new NetworkStream(Socket);
            cancellationToken = new CancellationTokenSource();

            wrapperTask = new Task(SocketProc, cancellationToken.Token, TaskCreationOptions.LongRunning);
            wrapperTask.Start();
        }

        #region Event Invokers
        protected internal void InvokeOnConnected()
        {
            if (OnConnected != null)
                OnConnected(this, new ClientConnectedEventArgs<AsyncTCPClient<TPacket>, TPacket>(this));
        }

        protected internal void InvokeOnDisconnected(DisconnectReason reason)
        {
            if (OnDisconnected != null)
                OnDisconnected(this, new ClientDisconnectedEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, reason));
        }

        protected internal void InvokeOnClientError(string methodName, Exception ex)
        {
            if (OnClientError != null)
                OnClientError(this, new ClientErrorEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, methodName, ex));
        }

        protected internal void InvokeOnPacketReceived(TPacket packet)
        {
            if (OnPacketReceived != null)
                OnPacketReceived(this, new PacketReceivedEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, packet));
        }

        protected internal void InvokeOnPacketSent(TPacket packet)
        {
            if (OnPacketSent != null)
                OnPacketSent(this, new PacketSentEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, packet));
        }

        protected internal void InvokeOnPingSent(DateTime sentAt)
        {
            if (OnPingSent != null)
                OnPingSent(this, new PingSentEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, sentAt));
        }

        protected internal void InvokeOnPingReceived(DateTime sentAt)
        {
            if (OnPingReceived != null)
                OnPingReceived(this, new PingReceivedEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, sentAt));
        }

        protected internal void InvokeOnPongSent(DateTime sentAt, DateTime receivedAt)
        {
            if (OnPongSent != null)
                OnPongSent(this, new PongSentEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, sentAt, receivedAt));
        }

        protected internal void InvokeOnPongReceived(DateTime sentAt, DateTime receivedAt)
        {
            if (OnPongReceived != null)
                OnPongReceived(this, new PongReceivedEventArgs<AsyncTCPClient<TPacket>, TPacket>(this, sentAt, receivedAt));
        }

        public async Task<bool> Connect()
        {
            try
            {
                if (Connected)
                    throw new InvalidOperationException("Cannot start the socket because is already connected!");

                Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await Socket.ConnectAsync(EndPoint);

                Initialize();

                return true;
            }
            catch (Exception ex)
            {
                InvokeOnClientError(nameof(Connect), ex);
                return false;
            }
        }

        public async Task<bool> Disconnect()
        {
            if (!Connected) return true;

            try
            {
                stream.Close();
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();

                return true;
            }
            catch (Exception ex)
            {
                InvokeOnClientError(nameof(Connect), ex);
                return false;
            }
        }

        public async Task Send(TPacket packet)
        {
            try
            {
                sendLock.WaitOne();

                var idBuffer = BitConverter.GetBytes(packet.ID);
                var sizeBuffer = BitConverter.GetBytes(packet.Data.Length);

                Buffer.BlockCopy(idBuffer, 0, sendBuffer, 0, idBuffer.Length);
                Buffer.BlockCopy(sizeBuffer, 0, sendBuffer, idBuffer.Length, sizeBuffer.Length);
                Buffer.BlockCopy(packet.Data, 0, sendBuffer, idBuffer.Length + sizeBuffer.Length, packet.Data.Length);

                int toSent = idBuffer.Length + sizeBuffer.Length + packet.Data.Length;
                sendPosition = 0;

                while (toSent > sendPosition)
                {
                    Buffer.BlockCopy(sendBuffer, sendPosition, sendChunk, 0, sendChunk.Length);
                    //sendPosition += await Socket.SendAsync(new ArraySegment<byte>(sendBuffer), SocketFlags.None);
                    await stream.WriteAsync(sendChunk, 0, sendChunk.Length);
                    sendPosition += sendChunk.Length;
                }

                sendLock.Set();
                InvokeOnPacketSent(packet);
            }
            catch (Exception ex)
            {
                InvokeOnClientError(nameof(Send), ex);
            }
        }

        public Task Ping()
        {
            var packet = CreatePacket(0x0);
            packet.WriteDateTime(DateTime.Now);

            return Send(packet);
        }

        public TPacket CreatePacket(ulong id)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id);
        }

        protected internal TPacket CreatePacket(ulong id, byte[] data)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id, data);
        }

        private async Task<bool> Disconnect(DisconnectReason reason, bool notify = false)
        {
            try
            {
                if (reason == DisconnectReason.MaximumClientsReached && notify)
                {
                    //BeginSendEvent(PacketEvent.MaximumClientsReached);
                }
                else
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Disconnect(false);
                }

                await Task.Delay(100);

                if (wrapperTask != null)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        cancellationToken.Cancel();
                }

                InvokeOnDisconnected(reason);
                return true;
            }
            catch (Exception ex)
            {
                InvokeOnClientError(nameof(Disconnect), ex);
                return false;
            }
        }

        private async void SocketProc()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    if (!await ReadBuffer(Constants.PacketHeaderSize))
                    {
                        await Disconnect(DisconnectReason.ConnectionFailed, false);
                        return;
                    }

                    var id = BitConverter.ToUInt64(receiveBuffer, 0);
                    var packet = CreatePacket(id);
                    var size = BitConverter.ToInt32(receiveBuffer, sizeof(ulong));

                    if (!await ReadBuffer(size))
                    {
                        await Disconnect(DisconnectReason.ConnectionFailed, false);
                        return;
                    }

                    packet.Data = receiveBuffer;
                    InvokeOnPacketReceived(packet);
                }
                catch (Exception ex)
                {
                    InvokeOnClientError(nameof(SocketProc), ex);
                }
            }
        }

        private async Task<bool> ReadBuffer(int length)
        {
            receivePosition = 0;
            int received = 0;

            while (receivePosition < length)
            {
                received = await stream.ReadAsync(receiveChunk, 0, receiveChunk.Length);
                if (received == 0)
                    return false;

                Buffer.BlockCopy(receiveChunk, 0, receiveBuffer, receivePosition, received);
                receivePosition += received;
            }

            return receivePosition > 0 && receivePosition == length;
        }
        #endregion
    }
}
