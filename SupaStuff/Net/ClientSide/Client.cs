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
        public TcpClient tcpClient { get; private set; }
        public NetworkStream stream { get; private set; }
        public bool IsLocal { get; private set; }
        public bool IsActive { get; internal set; }
        public IClientConnection localConnection { get; private set; }
        public PacketStream packetStream { get; private set; }
        public readonly int port;

        public byte[] Password;
        /// <summary>
        /// Create a client and attempt to connect to server
        /// </summary>
        /// <param name="ip"></param>
        public Client(IPAddress ip, int port, byte[] password)
        {
            IsLocal = false;
            IsActive = true;
            //External client
            NetMain.ClientInstance = this;
            //New client to connect with
            tcpClient = new TcpClient();
            this.port = port;
            Password = password;
            /*
            //How long to wait
            TimeSpan timeSpan = TimeSpan.FromSeconds(1);
            //Try to connect to server
            var result = tcpClient.BeginConnect(ip, Server.port, null, null);
            //Wait a second, if it hasnt worked it cant connect
            var success = result.AsyncWaitHandle.WaitOne(timeSpan);
            if (!success)
            {
                return;
            }
            */
            tcpClient.Connect(new IPEndPoint(ip, port));
            //Get the stream
            stream = tcpClient.GetStream();
            packetStream = new PacketStream(stream, false, () => { Dispose(); return false; });
            packetStream.OnDisconnected += Dispose;
            NetMain.ClientLogger.Log("Client started!");
            SendPacket(new C2SWelcomePacket());

        }
        /// <summary>
        /// Create a local client connection
        /// </summary>
        /// <param name="localconnection"></param>
        public Client(IClientConnection localconnection)
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
                else packetStream.SendPacket(packet);
            }
            catch
            {
                Dispose();
                NetMain.ClientLogger.Log("Failed sending a packet because the connection is closed");
            }
        }
        public void RecievePacket(Packet packet)
        {
            packet.Execute(null);
        }
        /// <summary>
        /// Try to recieve and write packets
        /// </summary>
        public void Update()
        {
            if (!IsLocal)
            {
                try
                {
                    packetStream.Update();
                    if (!tcpClient.Connected) Dispose();
                }
                catch
                {
                    NetMain.ClientLogger.Log("Failed updating for unknown reason, closing");
                    Dispose();
                }
            }
        }
        public delegate void OnMessage(Packet packet);
        public event OnMessage onMessage;
        /// <summary>
        /// GC assister
        /// </summary>
        public void Dispose()
        {
            if (!IsActive) return;
            IsActive = false;
            NetMain.ClientLogger.Log("Client closed!");
            NetMain.ClientInstance = null;
            stream.Close();
            stream.Dispose();
            tcpClient.Close();
            tcpClient.Dispose();
            packetStream.Dispose();
            DisposeEvent();
        }
        public event Action OnDispose;
        void DisposeEvent()
        {
            if (OnDispose != null) OnDispose.Invoke();
        }
        public void Disconnect()
        {
            SendPacket(new C2SDisconnectPacket());
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