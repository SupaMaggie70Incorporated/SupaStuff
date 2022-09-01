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
        internal Server<T> server;

        protected TcpClient tcpClient;
        protected NetworkStream stream;
        protected List<Packet> packetsToWrite = new List<Packet>();
        public bool isActive { get; protected set; }
        protected HandshakeStage handshakeStage = HandshakeStage.unstarted;
        public PacketStream packetStream { get; protected set; }
        public IPAddress address { get; protected set; }

        public IPAddress GetAddress() => address;
        public bool IsLocal() => false;
        public bool IsActive() => isActive;
        public void FinishAuth() => finishAuth = true;
        public bool AuthFinished() => finishAuth;
        public IServer GetServer() => server;

        internal DateTime connectionStarted;
        internal bool finishAuth = false;

        public ClientConnection(TcpClient tcpClient)
        {
            isActive = true;
            this.tcpClient = tcpClient;
            tcpClient.NoDelay = false;
            stream = tcpClient.GetStream();
            packetStream = new PacketStream(stream, true, () => false);
            packetStream.clientConnection = this;
            packetStream.OnDisconnected += () =>
            {
                NetMain.ServerLogger.Log("Kicking " + address + " because they kicked us first and we're mad");
                Dispose();
            };
            packetStream.logger = NetMain.ServerLogger;
            address = (tcpClient.Client.RemoteEndPoint as IPEndPoint).Address;
            connectionStarted = DateTime.UtcNow;
        }
        protected ClientConnection()
        {
            isActive = true;
        }
        public event Action<Packet> OnMessage;
        /// <summary>
        /// Send a packet only if it's remote 
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public virtual void SendPacket(Packet packet)
        {
            packetStream.SendPacket(packet);
        }
        public virtual void Update()
        {
            if (!isActive) Console.WriteLine("Wait, why am I still being updated?");
            packetStream.Update();
            if (!finishAuth)
            {
                if (SupaMath.TimeSince(connectionStarted) > 10)
                {
                    NetMain.ServerLogger.Log("Shutting down connection to " + address + " because they were unable to authorize themselves");
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
            if (!isActive) return;
            isActive = false;
            NetMain.ServerLogger.Log("Connection to client " + address + " terminated");
            server.Kick(this);
            tcpClient.Dispose();
            stream.Close();
            stream.Dispose();
            packetStream.Dispose();
            isActive = false;
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
    public enum HandshakeStage
    {
        unstarted,
        complete
    }
}