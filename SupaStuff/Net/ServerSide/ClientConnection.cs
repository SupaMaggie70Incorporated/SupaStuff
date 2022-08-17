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
    public class ClientConnection : IDisposable
    {
        protected TcpClient tcpClient;
        protected NetworkStream stream;
        public bool IsLocal { get; protected set; }
        protected List<Packet> packetsToWrite = new List<Packet>();
        public bool IsActive { get; private set; }
        protected HandshakeStage handshakeStage = HandshakeStage.unstarted;
        public PacketStream packetStream { get; protected set; }
        public IPAddress address { get; protected set; }

        internal DateTime connectionStarted;
        internal bool finishAuth = false;
        public ClientConnection(IAsyncResult ar)
        {
            tcpClient = Server.Instance.listener.EndAcceptTcpClient(ar);
        }
        public ClientConnection(TcpClient tcpClient)
        {
            IsActive = true;
            IsLocal = false;
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
            IsActive = true;
        }
        internal static LocalClientConnection LocalClient()
        {
            return LocalClientConnection.LocalClient();
        }
        public delegate void OnMessage(Packet packet);
        public event OnMessage onMessage;
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
            if (!IsActive) Console.WriteLine("Wait, why am I still being updated?");
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
            if (!IsActive) return;
            IsActive = false;
            NetMain.ServerLogger.Log("Connection to client " + address + " terminated");
            try
            {
                if (Server.Instance.IsActive)
                    Server.Instance.connections.Remove(this);
            }
            catch
            {

            }
            if (IsLocal)
            {
            }
            else
            {
                tcpClient.Dispose();
                stream.Close();
                stream.Dispose();
                packetStream.Dispose();
            }
            IsActive = false;
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