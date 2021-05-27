using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace GFramework.Network.Bases
{
    using Interfaces;
    using Enums;
    using EventArgs.Client;

    public abstract class BaseClientWrapper<TClient, TClientWrapper, TPacket>
        where TClient : class, IClient<TClient, TPacket>
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>
        where TPacket : BasePacket
    {
        private Dictionary<ulong, BasePacketReader<TClient, TClientWrapper, TPacket>> packetReaders;
        public TClient Socket { get; private set; }

        public BaseClientWrapper(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {
            
        }

        public BaseClientWrapper(IPEndPoint endpoint)
        {
            packetReaders = new Dictionary<ulong, BasePacketReader<TClient, TClientWrapper, TPacket>>();

            Socket = (TClient)Activator.CreateInstance(typeof(TClient), endpoint);
            Initialize();
        }

        protected internal void Initialize(TClient socket = null)
        {
            if (socket != null)
                Socket = socket;

            Socket.OnConnected += Socket_OnConnected;
            Socket.OnDisconnected += Socket_OnDisconnected;
            Socket.OnClientError += Socket_OnSocketError;
            Socket.OnPacketReceived += Socket_OnPacketReceived;
            Socket.OnPacketReceived += Socket_OnPacketReceivedWrapper;
            Socket.OnPacketSent += Socket_OnPacketSent;
        }

        private void Socket_OnConnected(object sender, ClientConnectedEventArgs<TClient, TPacket> e) => OnConnected();
        private void Socket_OnDisconnected(object sender, ClientDisconnectedEventArgs<TClient, TPacket> e) => OnDisconnected(e.Reason);
        private void Socket_OnSocketError(object sender, ClientErrorEventArgs<TClient, TPacket> e) => OnSocketError(e.Method, e.Exception);
        private void Socket_OnPacketReceived(object sender, PacketReceivedEventArgs<TClient, TPacket> e) => OnPacketReceived(e.Packet);
        private void Socket_OnPacketSent(object sender, PacketSentEventArgs<TClient, TPacket> e) => OnPacketSent(e.Packet);

        private void Socket_OnPacketReceivedWrapper(object sender, PacketReceivedEventArgs<TClient, TPacket> e)
        {
            if (packetReaders.TryGetValue(e.Packet.ID, out BasePacketReader<TClient, TClientWrapper, TPacket> reader))
            {
                reader.Client = (TClientWrapper)this;
                reader.Socket = Socket;

                if (reader.Read(e.Packet))
                    reader.Execute();
            }
        }

        protected virtual void OnConnected() { }
        protected virtual void OnDisconnected(DisconnectReason reason) { }
        protected virtual void OnSocketError(string method, Exception exception) { }
        protected virtual void OnPacketReceived(TPacket packet) { }
        protected virtual void OnPacketSent(TPacket packet) { }

        public bool Connect() => Socket.Connect();
        public bool Disconnect() => Socket.Disconnect();

        public bool RegisterReader<TPacketReader>(TPacketReader reader)
            where TPacketReader : BasePacketReader<TClient, TClientWrapper, TPacket>
        {
            if (packetReaders.ContainsKey(reader.ID))
                return false;
            else
            {
                packetReaders[reader.ID] = reader;
                return true;
            }
        }

        public void Send<TPacketWriter>(TPacketWriter writer)
            where TPacketWriter : BasePacketWriter<TClient, TClientWrapper, TPacket>
        {
            writer.Client = (TClientWrapper)this;
            writer.Socket = Socket;

            var packet = Socket.CreatePacket(writer.ID);
            if (writer.Write(packet))
                Socket.Send(packet);
        }
    }
}
