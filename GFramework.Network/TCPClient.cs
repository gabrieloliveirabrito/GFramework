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

    public class TCPClient<TPacket> : IClient<TCPClient<TPacket>, TPacket>
        where TPacket : BasePacket
    {
        BaseLogger logger;

        private IPEndPoint endPoint;

        public event EventHandler<ClientConnectedEventArgs<TCPClient<TPacket>, TPacket>> OnConnected;
        public event EventHandler<ClientDisconnectedEventArgs<TCPClient<TPacket>, TPacket>> OnDisconnected;
        public event EventHandler<PacketReceivedEventArgs<TCPClient<TPacket>, TPacket>> OnPacketReceived;
        public event EventHandler<PacketSentEventArgs<TCPClient<TPacket>, TPacket>> OnPacketSent;
        public event EventHandler<ClientErrorEventArgs<TCPClient<TPacket>, TPacket>> OnClientError;

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

        public bool DisconnectOnError { get; set; }

        public TCPClient(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {

        }

        public TCPClient(IPEndPoint endpoint)
        {
            DisconnectOnError = false;
            logger = LoggerFactory.GetLogger(this);
            endPoint = endpoint;
        }

        protected internal TCPClient(Socket socket)
        {
            DisconnectOnError = false;
            EndPoint = (IPEndPoint)socket.RemoteEndPoint;
            Socket = socket;
        }

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

        public TPacket CreatePacket(ulong id)
        {
            return (TPacket)Activator.CreateInstance(typeof(TPacket), id);
        }

        public void Send(BasePacket packet)
        {
            var holder = new PacketHolder(packet);
            
            BeginSendHeader(holder);
        }

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

        protected internal void InvokeOnPacketReceived(BasePacket packet)
        {
            if (OnPacketReceived != null)
                OnPacketReceived(this, new PacketReceivedEventArgs<TCPClient<TPacket>, TPacket>(this, packet));
        }

        protected internal void InvokeOnPacketSent(BasePacket packet)
        {
            if (OnPacketSent != null)
                OnPacketSent(this, new PacketSentEventArgs<TCPClient<TPacket>, TPacket>(this, packet));
        }

        protected internal bool Disconnect(DisconnectReason reason)
        {
            try
            {
                InvokeOnDisconnected(reason);

                Socket.Shutdown(SocketShutdown.Both);
                Socket.Disconnect(false);

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
            logger = LoggerFactory.GetLogger<TCPClient<TPacket>>();
            InvokeOnConnected();
            BeginReceiveHeader();
        }

        private void BeginReceiveHeader()
        {
            try
            {
                var holder = new PacketHolder();
                Socket.BeginReceive(holder.Header, 0, holder.Header.Length, SocketFlags.None, EndReceiveHeader, holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginReceiveHeader", ex);

                if(DisconnectOnError)
                    Disconnect(DisconnectReason.Error);
            }
        }

        private void EndReceiveHeader(IAsyncResult ar)
        {
            try
            {
                var holder = (PacketHolder)ar.AsyncState;
                var received = Socket.EndReceive(ar);

                //logger.LogInfo("C:{0} R:{1} L:{2} HR:{3}", holder.HeaderHandled, received, holder.Header.Length, BitConverter.ToString(holder.Header, 0, holder.HeaderHandled + received));
                holder.HeaderHandled += received;

                if (holder.HeaderHandled < holder.Header.Length)
                {
                    Socket.BeginReceive(holder.Header, holder.HeaderHandled, holder.Header.Length - holder.HeaderHandled, SocketFlags.None, EndReceiveHeader, holder);
                }
                else
                {
                    switch (holder.Event)
                    {
                        case PacketEvent.Receive:
                            if(holder.Length > 0)
                                BeginReceiveBuffer(holder);
                            else
                            {
                                var packet = (BasePacket)Activator.CreateInstance(typeof(TPacket), holder.ID);

                                InvokeOnPacketReceived(packet);
                                BeginReceiveHeader();
                            }
                            break;
                        case PacketEvent.Ping:
                            break;
                        case PacketEvent.ServerShutdown:
                            Disconnect(DisconnectReason.ServerShutdown);
                            break;
                        case PacketEvent.Unknown:
                            Disconnect(DisconnectReason.Error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndReceiveHeader", ex);

                if (DisconnectOnError)
                    Disconnect(DisconnectReason.Error);
            }
        }

        private void BeginReceiveBuffer(PacketHolder holder)
        {
            try
            {
                holder.Buffer = new byte[holder.Length];
                Socket.BeginReceive(holder.Chunk, 0, Constants.PacketChunkLength, SocketFlags.None, EndReceiveBuffer, holder);
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("BeginReceiveBuffer", ex);

                if (DisconnectOnError)
                    Disconnect(DisconnectReason.Error);
            }
        }

        private void EndReceiveBuffer(IAsyncResult ar)
        {
            try
            {
                var holder = (PacketHolder)ar.AsyncState;
                var received = Socket.EndReceive(ar);
                //logger.LogInfo("C:{0} R:{1} L:{2} BR:{3}", holder.BufferHandled, received, holder.Length, BitConverter.ToString(holder.Buffer, 0, holder.BufferHandled + received));

                if (received == 0)
                {
                    Disconnect(DisconnectReason.ServerShutdown);
                }
                else
                {
                    Buffer.BlockCopy(holder.Chunk, 0, holder.Buffer, holder.BufferHandled, received);

                    holder.BufferHandled += received;
                    if (holder.BufferHandled < holder.Length)
                    {
                        Socket.BeginReceive(holder.Chunk, 0, Constants.PacketChunkLength, SocketFlags.None, EndReceiveBuffer, holder);
                    }
                    else
                    {
                        var packet = (BasePacket)Activator.CreateInstance(typeof(TPacket), holder.ID, holder.Buffer);

                        InvokeOnPacketReceived(packet);
                        BeginReceiveHeader();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogFatal(ex);
                InvokeOnClientError("EndReceiveBuffer", ex);

                if (DisconnectOnError)
                    Disconnect(DisconnectReason.Error);
            }
        }

        private void BeginSendHeader(PacketHolder holder)
        {
            Socket.BeginSend(holder.Header, 0, holder.Header.Length, SocketFlags.None, EndSendHeader, holder);
        }

        private void EndSendHeader(IAsyncResult ar)
        {
            PacketHolder holder = (PacketHolder)ar.AsyncState;
            var sent = Socket.EndSend(ar);
            //logger.LogInfo("C:{0} S:{1} L:{2} HS:{3}", holder.HeaderHandled, sent, holder.Header.Length, BitConverter.ToString(holder.Header, 0, holder.HeaderHandled + sent));

            holder.HeaderHandled += sent;
            if (holder.HeaderHandled < holder.Header.Length)
            {
                Socket.BeginSend(holder.Header, holder.HeaderHandled, holder.Header.Length - holder.HeaderHandled, SocketFlags.None, EndSendHeader, holder);
            }
            else
            {
                BeginSendBuffer(holder);
            }
        }

        private void BeginSendBuffer(PacketHolder holder)
        {
            long length = holder.Length - holder.ChunkHandled;
            if (length >= Constants.PacketChunkLength)
                length = Constants.PacketChunkLength;

            holder.Chunk = new byte[length];
            Buffer.BlockCopy(holder.Buffer, holder.BufferHandled, holder.Chunk, 0, Convert.ToInt32(length));

            BeginSendChunk(holder);
        }

        private void BeginSendChunk(PacketHolder holder)
        {
            Socket.BeginSend(holder.Chunk, holder.ChunkHandled, holder.Chunk.Length - holder.ChunkHandled, SocketFlags.None, EndSendChunk, holder);
        }

        private void EndSendChunk(IAsyncResult ar)
        {
            PacketHolder holder = (PacketHolder)ar.AsyncState;
            var sent = Socket.EndSend(ar);

            holder.ChunkHandled += sent;
            if(holder.ChunkHandled < holder.Chunk.Length)
            {
                BeginSendChunk(holder);
            }
            else
            {
                holder.BufferHandled += holder.ChunkHandled;
                if(holder.BufferHandled < holder.Length)
                {
                    BeginSendBuffer(holder);
                }
                else
                {
                    InvokeOnPacketSent(holder.Packet);
                }
            }
        }
    }
}