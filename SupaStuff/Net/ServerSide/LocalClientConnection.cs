
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using SupaStuff.Net.Packets;



namespace SupaStuff.Net.ServerSide
{
    public class LocalClientConnection : IClientConnection
    {
        public ClientSide.Client client { get; internal set; }
        public IPAddress GetAddress() => new IPAddress(new byte[] { 127, 0, 0, 1 });
        public bool IsLocal() => true;
        public bool IsActive() => server.IsActive();
        public IServer server;
        public IServer GetServer() => server;
        public bool AuthFinished() => true;
        public void FinishAuth()
        {

        }
        protected LocalClientConnection(IServer server)
        {
            client = new ClientSide.Client(this);
            NetMain.ClientLogger.Log("Local client initialized");
            this.server = server;
        }
        
        internal static LocalClientConnection LocalClient(IServer server)
        {
            return new LocalClientConnection(server);
        }
        public void SendPacket(Packet packet)
        {
            client.RecievePacket(packet);
        }
        public event Action<Packet> OnMessage;
        public void RecievePacket(Packet packet)
        {
            if(OnMessage != null) OnMessage(packet);
            packet.Execute(this);
        }
        public void Update()
        {
            return;
        }
        public void Kick()
        {
        }
        public void Kick(string message)
        {
        }
        public event Action OnDispose;
        public void Dispose()
        {
            if(OnDispose != null) OnDispose();
            try
            {
                //server.Kick(this,"Kicked for local client terminated");
            }
            catch { }
        }
    }
}