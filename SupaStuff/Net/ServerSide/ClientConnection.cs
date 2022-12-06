using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using SupaStuff.Net.Packets;
using SupaStuff.Net.Packets.BuiltIn;
using SupaStuff.Math;


namespace SupaStuff.Net.ServerSide
{
    public class ClientConnection<T> : IClientConnection
    {
        public Server<T> ConnectedServer;

        public T Data = default(T);

        public TcpClient TcpClient;
        protected NetworkStream Stream;
        public bool Active { get; protected set; }
        public PacketStream PacketStream { get; protected set; }
        public IPAddress Address { get; protected set; }

        public virtual IPAddress GetAddress() => Address;
        public virtual bool IsLocal() => false;
        public virtual bool IsActive() => Active;
        public virtual void FinishAuth() => FinishedAuth = true;
        public virtual bool AuthFinished() => FinishedAuth;
        public IServer GetServer() => ConnectedServer;

        internal DateTime ConnectionStartTime;
        internal bool FinishedAuth = false;

        public ClientConnection(TcpClient tcpClient)
        {
            ConnectedServer = NetMain.ServerInstance as Server<T>;
            Active = true;
            this.TcpClient = tcpClient;
            tcpClient.NoDelay = false;
            Stream = tcpClient.GetStream();
            PacketStream = new PacketStream(Stream, true);
            PacketStream.clientConnection = this;
            PacketStream.OnDisconnected += () =>
            {
                NetMain.ServerLogger.Log("Kicking " + Address + " because they already disconnected");
                Dispose();
            };
            Address = (tcpClient.Client.RemoteEndPoint as IPEndPoint).Address;
            ConnectionStartTime = DateTime.UtcNow;
        }
        protected ClientConnection()
        {
            Active = true;
        }

        /// <summary>
        /// Send a packet only if it's remote 
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public virtual void SendPacket(Packet packet)
        {
            PacketStream.SendPacket(packet);
        }
        public virtual void RecievePacket(Packet packet)
        {
            PacketStream.HandleIncomingPacket(packet);
        }
        public virtual void Update()
        {
            if (!Active)
            {
                Dispose();
                return;
            }

            PacketStream.Update();
            if (!FinishedAuth)
            {
                if (SupaMath.TimeSince(ConnectionStartTime) > 10)
                {
                    NetMain.ServerLogger.Warn("Shutting down connection to " + Address + " because they were unable to authorize themselves");
                    Dispose();
                }
            }
        }
        /// <summary>
        /// Kick the client from the server with a message
        /// </summary>
        /// <param name="message">
        /// The message to be sent
        /// </param>
        public virtual void Dispose()
        {
            if (!Active) return;
            Active = false;
            NetMain.ServerLogger.Log("Connection to client " + Address + " terminated");
            ConnectedServer.Kick(this);
            TcpClient?.Dispose();
            Stream?.Close();
            Stream?.Dispose();
            PacketStream?.Dispose();
            Active = false;
            DisposeEvent();
        }
        public event Action OnDispose;
        void DisposeEvent()
        {
            if (OnDispose != null) OnDispose.Invoke();
        }
        public virtual void Kick(string message)
        {
            SendPacket(new S2CKickPacket(message));
        }
        public virtual void Kick()
        {
            SendPacket(new S2CKickPacket());
        }
    }
}