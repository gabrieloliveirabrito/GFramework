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
    using Holders;
    using Interfaces;
    using System.Threading;

    public class TCPClient<TPacket> : IClient<TCPClient<TPacket>, TPacket>
        where TPacket : BasePacket
    {
        private BaseLogger logger;
        private ManualResetEvent sendLock;
        private IPEndPoint endPoint;

        public event EventHandler<ClientConnectedEventArgs<TCPClient<TPacket>, TPacket>> OnConnected;
        public event EventHandler<ClientDisconnectedEventArgs<TCPClient<TPacket>, TPacket>> OnDisconnected;
        public event EventHandler<PacketReceivedEventArgs<TCPClient<TPacket>, TPacket>> OnPacketReceived;
        public event EventHandler<PacketSentEventArgs<TCPClient<TPacket>, TPacket>> OnPacketSent;
        public event EventHandler<ClientErrorEventArgs<TCPClient<TPacket>, TPacket>> OnClientError;
        public event EventHandler<PingSentEventArgs<TCPClient<TPacket>, TPacket>> OnPingSent;
        public event EventHandler<PingReceivedEventArgs<TCPClient<TPacket>, TPacket>> OnPingReceived;
        public event EventHandler<PongSentEventArgs<TCPClient<TPacket>, TPacket>> OnPongSent;
        public event EventHandler<PongReceivedEventArgs<TCPClient<TPacket>, TPacket>> OnPongReceived;

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

        public TCPClient(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {

        }

        public TCPClient(IPEndPoint endpoint)
        {
            logger = LoggerFactory.GetLogger(this);
            sendLock = new ManualResetEvent(true);
            endPoint = endpoint;
        }

        protected internal TCPClient(Socket socket)
        {
            logger = LoggerFactory.GetLogger(this);
            sendLock = new ManualResetEvent(true);

            EndPoint = (IPEndPoint)socket.RemoteEndPoint;
            Socket = socket;
        }

        #region Event Invokers
        protected internal void InvokeOnConnected()
        {
            if (OnConnected != null)
                OnConnected(this, new ClientConnectedEventArgs<TCPClient<TPacket>, TPacket>(this));
        }

        protected internal void InvokeOnDisconnected(DisconnectReason reason)
        {
            if (OnDisconnected != null)
                OnDisconnected(this, new ClientDisconnectedEventArgs<TCPClient<TPacket>, TPacket>(this, reason));
        }

        protected internal void InvokeOnClientError(string methodName, Exception ex)
        {
            if (OnClientError != null)
                OnClientError(this, new ClientErrorEventArgs<TCPClient<TPacket>, TPacket>(this, methodName, ex));
        }

        protected internal void InvokeOnPacketReceived(TPacket packet)
        {
            if (OnPacketReceived != null)
                OnPacketReceived(this, new PacketReceivedEventArgs<TCPClient<TPacket>, TPacket>(this, packet));
        }

        protected internal void InvokeOnPacketSent(TPacket packet)
        {
            if (OnPacketSent != null)
                OnPacketSent(this, new PacketSentEventArgs<TCPClient<TPacket>, TPacket>(this, packet));
        }

        protected internal void InvokeOnPingSent(DateTime sentAt)
        {
            if (OnPingSent != null)
                OnPingSent(this, new PingSentEventArgs<TCPClient<TPacket>, TPacket>(this, sentAt));
        }

        protected internal void InvokeOnPingReceived(DateTime sentAt)
        {
            if (OnPingReceived != null)
                OnPingReceived(this, new PingReceivedEventArgs<TCPClient<TPacket>, TPacket>(this, sentAt));
        }

        protected internal void InvokeOnPongSent(DateTime sentAt, DateTime receivedAt)
        {
            if (OnPongSent != null)
                OnPongSent(this, new PongSentEventArgs<TCPClient<TPacket>, TPacket>(this, sentAt, receivedAt));
        }

        protected internal void InvokeOnPongReceived(DateTime sentAt, DateTime receivedAt)
        {
            if (OnPongReceived != null)
                OnPongReceived(this, new PongReceivedEventArgs<TCPClient<TPacket>, TPacket>(this, sentAt, receivedAt));
        }
        #endregion

        public bool Connect()
        {
            try
            {
                if (Connected)
                    throw new InvalidOperationException("Cannot start the socket because is already connected!");

                Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket.BeginConnect(endPoint, EndConnect, null);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("Connect", ex);
                return false;
            }
        }

        public bool Disconnect()
        {
            return Disconnect(DisconnectReason.UserRequest);
        }

        public void Ping()
        {
            var packet = CreatePacket(0x0);
            packet.WriteDateTime(DateTime.Now);

            BeginSendEvent(PacketEvent.Ping, packet);
        }

        public TPacket CreatePacket(ulong id)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id);
        }

        protected internal TPacket CreatePacket(ulong id, byte[] data)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id, data);
        }

        public void Send(TPacket packet)
        {
            BeginSendEvent(PacketEvent.Receive, packet);
        }

        protected internal bool Disconnect(DisconnectReason reason, bool notify = false)
        {
            try
            {
                InvokeOnDisconnected(reason);

                if (reason == DisconnectReason.MaximumClientsReached && notify)
                {
                    BeginSendEvent(PacketEvent.MaximumClientsReached);
                }
                else
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Disconnect(false);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("Disconnect(reason)", ex);
                return false;
            }
        }

        private void EndConnect(IAsyncResult result)
        {
            try
            {
                Socket.EndConnect(result);
                Initialize();
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndConnect", ex);
                Disconnect(DisconnectReason.ConnectionFailed);
            }
        }

        protected internal void Initialize()
        {
            InvokeOnConnected();
            BeginReceiveEvent();
        }

        #region Receive Callbacks
        private void BeginReceiveEvent()
        {
            try
            {
                var holder = new EventHolder();
                Socket.BeginReceive(holder.EventBuffer, 0, holder.EventBuffer.Length, SocketFlags.None, EndReceiveEvent, holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginReceiveEvent", ex);

                Disconnect(DisconnectReason.Error);
            }
        }

        private void EndReceiveEvent(IAsyncResult ar)
        {
            try
            {
                EventHolder holder = (EventHolder)ar.AsyncState;
                int received = Socket.EndReceive(ar);

                if (DebugPackets)
                    logger.LogDebug("ER:{0} L:{1} R:{2} B:{3}", holder.EventHandled, holder.EventBuffer.Length, received, BitConverter.ToString(holder.EventBuffer, 0, holder.EventHandled + received));

                if (received == 0)
                {
                    Disconnect(DisconnectReason.ServerShutdown);
                }
                else
                {
                    holder.EventHandled += received;
                    if (holder.EventHandled < holder.EventBuffer.Length)
                    {
                        Socket.BeginReceive(holder.EventBuffer, holder.EventHandled, holder.EventBuffer.Length - holder.EventHandled, SocketFlags.None, EndReceiveEvent, holder);
                    }
                    else
                    {
                        switch (holder.Event)
                        {
                            case PacketEvent.Ping:
                            case PacketEvent.Pong:
                            case PacketEvent.Receive:
                                BeginReceiveHeader(holder.Event);
                                break;
                            case PacketEvent.ServerShutdown:
                                Disconnect(DisconnectReason.ServerShutdown);
                                break;
                            case PacketEvent.MaximumClientsReached:
                                Disconnect(DisconnectReason.MaximumClientsReached);
                                break;
                            case PacketEvent.Unknown:
                            default:
                                Disconnect(DisconnectReason.UnknownEvent);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndReceiveEvent", ex);

                Disconnect(DisconnectReason.Error);
            }
        }

        private void BeginReceiveHeader(PacketEvent packetEvent)
        {
            try
            {
                var holder = new PacketHolder();
                holder.Event = packetEvent;

                Socket.BeginReceive(holder.Header, 0, holder.Header.Length, SocketFlags.None, EndReceiveHeader, holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginReceiveHeader", ex);

                Disconnect(DisconnectReason.Error);
            }
        }

        private void EndReceiveHeader(IAsyncResult ar)
        {
            try
            {
                var holder = (PacketHolder)ar.AsyncState;
                var received = Socket.EndReceive(ar);

                if (DebugPackets)
                    logger.LogDebug("HR:{0} L:{1} R:{2} B:{3}", holder.HeaderHandled, holder.Header.Length, received, BitConverter.ToString(holder.Header, 0, holder.HeaderHandled + received));
                holder.HeaderHandled += received;

                if (received == 0)
                {
                    Disconnect(DisconnectReason.ServerShutdown);
                }
                else
                {
                    if (holder.HeaderHandled < holder.Header.Length)
                    {
                        Socket.BeginReceive(holder.Header, holder.HeaderHandled, holder.Header.Length - holder.HeaderHandled, SocketFlags.None, EndReceiveHeader, holder);
                    }
                    else
                    {
                        if (holder.Length > 0)
                        {
                            holder.Buffer = new byte[holder.Length];
                            BeginReceiveBuffer(holder);
                        }
                        else
                        {
                            var packet = CreatePacket(holder.ID);

                            InvokeOnPacketReceived(packet);
                            BeginReceiveEvent();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndReceiveHeader", ex);

                Disconnect(DisconnectReason.Error);
            }
        }

        private void BeginReceiveBuffer(PacketHolder holder)
        {
            try
            {
                long length = holder.Length - holder.ChunkHandled;
                if (length >= Constants.PacketChunkLength)
                    length = Constants.PacketChunkLength;

                holder.Chunk = new byte[length];

                BeginReceiveChunk(holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginReceiveBuffer", ex);

                Disconnect(DisconnectReason.Error);
            }
        }

        private void BeginReceiveChunk(PacketHolder holder)
        {
            try
            {
                Socket.BeginReceive(holder.Chunk, holder.ChunkHandled, holder.Chunk.Length - holder.ChunkHandled, SocketFlags.None, EndReceiveChunk, holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginReceiveChunk", ex);

                Disconnect(DisconnectReason.Error);
            }
        }

        private void EndReceiveChunk(IAsyncResult ar)
        {
            try
            {
                var holder = (PacketHolder)ar.AsyncState;
                var received = Socket.EndReceive(ar);

                if (DebugPackets)
                    logger.LogDebug("BC:{0} L:{1} R:{2} B:{3}", holder.ChunkHandled, holder.Chunk.Length, received, BitConverter.ToString(holder.Chunk, 0, holder.ChunkHandled + received));

                if (received == 0)
                {
                    Disconnect(DisconnectReason.ServerShutdown);
                }
                else
                {
                    Buffer.BlockCopy(holder.Chunk, 0, holder.Buffer, holder.BufferHandled, received);

                    holder.ChunkHandled += received;
                    if (holder.ChunkHandled < holder.Chunk.Length)
                    {
                        Socket.BeginReceive(holder.Chunk, holder.ChunkHandled, holder.Chunk.Length - holder.ChunkHandled, SocketFlags.None, EndReceiveChunk, holder);
                    }
                    else
                    {
                        if (DebugPackets)
                            logger.LogDebug("BR:{0} L:{1} R:{2} B:{3}", holder.BufferHandled, holder.Length, received, BitConverter.ToString(holder.Buffer, 0, holder.BufferHandled + received));

                        Buffer.BlockCopy(holder.Chunk, 0, holder.Buffer, holder.BufferHandled, holder.Chunk.Length);
                        holder.BufferHandled += holder.ChunkHandled;

                        if (holder.BufferHandled < holder.Length)
                        {
                            BeginReceiveBuffer(holder);
                        }
                        else
                        {
                            var packet = (TPacket)Activator.CreateInstance(typeof(TPacket), holder.ID, holder.Buffer);

                            switch (holder.Event)
                            {
                                case PacketEvent.Receive:
                                    InvokeOnPacketReceived(packet);
                                    break;
                                case PacketEvent.Ping:
                                    {
                                        var sendAt = packet.ReadDateTime();
                                        InvokeOnPingReceived(sendAt);

                                        packet.Clear();
                                        packet.WriteDateTime(sendAt);
                                        packet.WriteDateTime(DateTime.Now);

                                        BeginSendEvent(PacketEvent.Pong, packet);
                                    }
                                    break;
                                case PacketEvent.Pong:
                                    {
                                        var sentAt = packet.ReadDateTime();
                                        var receivedAt = packet.ReadDateTime();

                                        InvokeOnPongReceived(sentAt, receivedAt);
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException("Invalid PacketEvent handled!");
                            }
                            BeginReceiveEvent();
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndReceiveBuffer", ex);

                Disconnect(DisconnectReason.Error);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndReceiveBuffer", ex);

                BeginReceiveEvent();
            }
        }
        #endregion

        #region Send Callbacks
        private void BeginSendEvent(PacketEvent packetEvent, TPacket packet = null)
        {
            try
            {
                sendLock.WaitOne();

                var holder = new EventHolder(packetEvent);
                holder.Packet = packet;

                Socket.BeginSend(holder.EventBuffer, 0, holder.EventBuffer.Length, SocketFlags.None, EndSendEvent, holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginSendEvent", ex);
            }
        }

        private void EndSendEvent(IAsyncResult ar)
        {
            try
            {
                var holder = (EventHolder)ar.AsyncState;
                var sent = Socket.EndSend(ar);

                if (DebugPackets)
                    logger.LogDebug("ES:{0} L:{1} R:{2} B:{3}", holder.EventHandled, holder.EventBuffer.Length, sent, BitConverter.ToString(holder.EventBuffer, 0, holder.EventHandled + sent));

                holder.EventHandled += sent;
                if (holder.EventHandled < holder.EventBuffer.Length)
                {
                    Socket.BeginSend(holder.EventBuffer, holder.EventHandled, holder.EventHandled - holder.EventHandled, SocketFlags.None, EndSendEvent, holder);
                }
                else if (holder.Packet != null)
                {
                    var packetHolder = new PacketHolder(holder.Packet);
                    packetHolder.Event = holder.Event;

                    BeginSendHeader(packetHolder);
                }
                else
                {
                    if (holder.Event == PacketEvent.MaximumClientsReached)
                    {
                        Socket.Shutdown(SocketShutdown.Both);
                        Socket.Disconnect(false);
                    }

                    sendLock.Set();
                }
            }
            catch (Exception ex)
            {
                sendLock.Set();

                logger.LogFatal(ex);
                InvokeOnClientError("EndSendEvent", ex);
            }
        }

        private void BeginSendHeader(PacketHolder holder)
        {
            try
            {
                Socket.BeginSend(holder.Header, 0, holder.Header.Length, SocketFlags.None, EndSendHeader, holder);
            }
            catch (Exception ex)
            {
                sendLock.Set();

                logger.LogFatal(ex);
                InvokeOnClientError("BeginSendHeader", ex);
            }
        }

        private void EndSendHeader(IAsyncResult ar)
        {
            try
            {
                PacketHolder holder = (PacketHolder)ar.AsyncState;
                var sent = Socket.EndSend(ar);

                if (DebugPackets)
                    logger.LogDebug("HS:{0} L:{1} R:{2} B:{3}", holder.HeaderHandled, holder.Header.Length, sent, BitConverter.ToString(holder.Header, 0, holder.HeaderHandled + sent));
                holder.HeaderHandled += sent;
                if (holder.HeaderHandled < holder.Header.Length)
                {
                    Socket.BeginSend(holder.Header, holder.HeaderHandled, holder.Header.Length - holder.HeaderHandled, SocketFlags.None, EndSendHeader, holder);
                }
                else if (holder.Length == 0)
                {
                    sendLock.Set();
                    InvokeOnPacketSent((TPacket)holder.Packet);
                }
                else
                {
                    BeginSendBuffer(holder);
                }
            }
            catch (Exception ex)
            {
                sendLock.Set();

                logger.LogFatal(ex);
                InvokeOnClientError("EndSendHeader", ex);
            }
        }

        private void BeginSendBuffer(PacketHolder holder)
        {
            try
            {
                long length = holder.Length - holder.ChunkHandled;
                if (length >= Constants.PacketChunkLength)
                    length = Constants.PacketChunkLength;

                holder.Chunk = new byte[length];
                Buffer.BlockCopy(holder.Buffer, holder.BufferHandled, holder.Chunk, 0, Convert.ToInt32(length));

                BeginSendChunk(holder);
            }
            catch (Exception ex)
            {
                sendLock.Set();

                logger.LogFatal(ex);
                InvokeOnClientError("BeginSendBuffer", ex);
            }
        }

        private void BeginSendChunk(PacketHolder holder)
        {
            try
            {
                Socket.BeginSend(holder.Chunk, holder.ChunkHandled, holder.Chunk.Length - holder.ChunkHandled, SocketFlags.None, EndSendChunk, holder);
            }
            catch (Exception ex)
            {
                sendLock.Set();

                logger.LogFatal(ex);
                InvokeOnClientError("BeginSendEvent", ex);
            }
        }

        private void EndSendChunk(IAsyncResult ar)
        {
            try
            {
                PacketHolder holder = (PacketHolder)ar.AsyncState;
                var sent = Socket.EndSend(ar);

                if (DebugPackets)
                    logger.LogDebug("CS:{0} L:{1} R:{2} B:{3}", holder.ChunkHandled, holder.Chunk.Length, sent, BitConverter.ToString(holder.Chunk, 0, holder.ChunkHandled + sent));
                holder.ChunkHandled += sent;
                if (holder.ChunkHandled < holder.Chunk.Length)
                {
                    BeginSendChunk(holder);
                }
                else
                {
                    if (DebugPackets)
                        logger.LogDebug("BS:{0} L:{1} R:{2} B:{3}", holder.BufferHandled, holder.Buffer.Length, holder.ChunkHandled, BitConverter.ToString(holder.Buffer, 0, holder.BufferHandled + holder.ChunkHandled));

                    holder.BufferHandled += holder.ChunkHandled;
                    if (holder.BufferHandled < holder.Length)
                    {
                        BeginSendBuffer(holder);
                    }
                    else
                    {
                        sendLock.Set();

                        if (holder.Event == PacketEvent.Receive)
                        {
                            InvokeOnPacketSent((TPacket)holder.Packet);
                        }
                        else if (holder.Event == PacketEvent.Ping)
                        {
                            holder.Packet.Reset();
                            var sentAt = holder.Packet.ReadDateTime();

                            InvokeOnPingSent(sentAt);
                        }
                        else if (holder.Event == PacketEvent.Pong)
                        {
                            holder.Packet.Reset();
                            var sentAt = holder.Packet.ReadDateTime();
                            var receivedAt = holder.Packet.ReadDateTime();

                            InvokeOnPongSent(sentAt, receivedAt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sendLock.Set();

                logger.LogFatal(ex);
                InvokeOnClientError("EndSendChunk", ex);
            }
        }
        #endregion
    }
}