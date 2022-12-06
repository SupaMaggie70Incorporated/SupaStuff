using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using SupaStuff.Net.ServerSide;
using SupaStuff.Net.Packets;
using SupaStuff.Net.Packets.BuiltIn;



namespace SupaStuff.Net.ClientSide
{
    public class Client : IDisposable
    {
        public TcpClient TcpClient { get; private set; }
        public NetworkStream Stream { get; private set; }
        public bool IsLocal { get; private set; }
        public bool IsActive { get; internal set; }
        public IClientConnection localConnection { get; private set; }
        PacketStream packetStream;

        public IPAddress Address;
        public readonly int Port;

        public bool Disposed = false;

        public byte[] Password;

        private bool WasConnected = false;
        private Task ConnectionTask;

        /// <summary>
        /// Packets that were queued before connection completed
        /// </summary>
        private Queue<Packet> UnsentPackets = new Queue<Packet>();
        /// <summary>
        /// Create a client and attempt to connect to server
        /// </summary>
        /// <param name="ip"></param>
        public Client(IPAddress ip, int port, byte[] password)
        {
            IsLocal = false;
            IsActive = false;
            //External client
            NetMain.ClientInstance = this;
            //New client to connect with
            TcpClient = new TcpClient();
            this.Address = ip;
            this.Port = port;
            Password = password;
            NetMain.ClientLogger.Log("Client started!");
            ConnectionTask = TcpClient.ConnectAsync(ip, port);
        }
        public event Action OnConnected;
        /// <summary>
        /// Create a local client connection
        /// </summary>
        /// <param name="localconnection"></param>
        internal Client(IClientConnection localconnection)
        {
            //Local client
            IsLocal = true;
            IsActive = true;
            NetMain.ClientInstance = this;
            localConnection = localconnection;

        }
        /// <summary>
        /// Send a packet over the stream
        /// </summary>
        /// <param name="packet"></param>
        public void SendPacket(Packet packet)
        {
            try
            {
                if (IsLocal) localConnection.RecievePacket(packet);
                else
                {
                    if(IsActive)
                    {
                        packetStream.SendPacket(packet);
                    }
                    else if(!Disposed)
                    {
                        UnsentPackets.Enqueue(packet);
                    }
                }
            }
            catch
            {
                Dispose();
                NetMain.ClientLogger.Warn("Failed sending a packet because the connection is closed");
            }
        }
        public void CancelOngoingConnection()
        {
            if (ConnectionTask != null && ConnectionTask.Status == TaskStatus.Running) ConnectionTask.Dispose();
        }
        public void RecievePacket(Packet packet)
        {
            if (!Packet.ExecutePacket(packet, false, null))
            {
                Dispose();
            }
        }
        /// <summary>
        /// Try to recieve and write packets
        /// </summary>
        public void Update()
        {
            if(!WasConnected)
            {
                if (ConnectionTask.Status == TaskStatus.Faulted || ConnectionTask.Status == TaskStatus.Canceled)
                {
                    WasConnected = true;
                    Dispose();
                    return;
                }
                else if(ConnectionTask.Status == TaskStatus.RanToCompletion)
                {
                    WasConnected = true;
                    IsActive = true;
                    Stream = TcpClient.GetStream();
                    packetStream = new PacketStream(Stream, false);
                    packetStream.OnDisconnected += Dispose;
                    SendPacket(new C2SWelcomePacket());
                    NetMain.ClientLogger.Log("Client successfully connected!");
                    if (OnConnected != null) OnConnected.Invoke();

                    foreach (Packet packet in UnsentPackets)
                    {
                        packetStream.SendPacket(packet);
                    }
                    UnsentPackets?.Clear();
                    UnsentPackets = null;
                }
            }

            if (!IsActive) return;
            if (!IsLocal)
            {
                try
                {
                    packetStream.Update();
                    if (!TcpClient.Connected) Dispose();
                }
                catch
                {
                    NetMain.ClientLogger.Error("Failed updating for unknown reason, closing");
                    Dispose();
                }
            }
        }
        public event Action<Packet> OnMessage;
        /// <summary>
        /// GC assister
        /// </summary>
        public void Dispose()
        {
            CancelOngoingConnection();
            if (!IsActive) return;
            Disposed = true;
            IsActive = false;
            NetMain.ClientLogger.Log("Client closed!");
            NetMain.ClientInstance = null;
            Stream?.Close();
            TcpClient?.Close();
            packetStream?.Dispose();
            DisposeEvent();
            UnsentPackets?.Clear();
            UnsentPackets = null;
        }
        public event Action OnDispose;
        void DisposeEvent()
        {
            if (OnDispose != null) OnDispose.Invoke();
        }
        public void Disconnect(string message)
        {
            SendPacket(new C2SDisconnectPacket(message));
        }
        public void HardDisconnect()
        {
            Dispose();
        }
    }
}