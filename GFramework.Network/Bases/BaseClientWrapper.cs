﻿using System;
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
    using System.Reflection;
    using System.Dynamic;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class BaseClientWrapper<TClient, TClientWrapper, TPacket>
        where TClient : class, IBaseClient<TClient, TPacket>, new()
        where TClientWrapper : BaseClientWrapper<TClient, TClientWrapper, TPacket>, new()
        where TPacket : BasePacket
    {
        private Dictionary<ulong, Type> packetReaders;

        public TClient Socket { get; private set; }
        public IServiceProvider Services { get; set; }

        public BaseClientWrapper()
        {
            packetReaders = new Dictionary<ulong, Type>();

            Socket = new TClient();
            Initialize();
        }

        public BaseClientWrapper(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {

        }

        public BaseClientWrapper(IPEndPoint endpoint) : this()
        {
            Socket.EndPoint = endpoint;
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
            if (packetReaders.TryGetValue(e.Packet.ID, out Type readerType))
            {
                var reader = CreateInstance<BasePacketReader<TClient, TClientWrapper, TPacket>>(readerType);
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

        //public bool Connect() => Socket.Connect();
        //public bool Disconnect() => Socket.Disconnect();

        private T CreateInstance<T>(Type target)
        {
            object instance = null;
            if (Services == null)
                instance = Activator.CreateInstance(target);
            else
                instance = ActivatorUtilities.CreateInstance(Services, target);

            return instance == null ? default(T) : (T)instance;
        }

        public bool RegisterReader<TPacketReader>(ulong ID)
            where TPacketReader : BasePacketReader<TClient, TClientWrapper, TPacket>
        {
            if (packetReaders.ContainsKey(ID))
                return false;
            else
            {
                packetReaders[ID] = typeof(TPacketReader);
                return true;
            }
        }

        public bool RegisterReader<TBaseReader>(Assembly origin, ref int registered)
            where TBaseReader : BasePacketReader<TClient, TClientWrapper, TPacket>
        {
            registered = 0;
            foreach (var type in origin.GetTypes())
            {
                if (type.BaseType == typeof(TBaseReader))
                {
                    var reader = CreateInstance<TBaseReader>(type);
                    if (!packetReaders.ContainsKey(reader.ID))
                    {
                        packetReaders[reader.ID] = type;
                        registered++;
                    }
                }
            }

            return registered > 0;
        }

        /*public void Send<TPacketWriter>(TPacketWriter writer)
            where TPacketWriter : BasePacketWriter<TClient, TClientWrapper, TPacket>
        {
            if (!Socket.Connected) return;
            writer.Client = (TClientWrapper)this;
            writer.Socket = Socket;

            var packet = Socket.CreatePacket(writer.ID);
            if (writer.Write(packet))
                Socket.Send(packet);
        }*/
    }
}
