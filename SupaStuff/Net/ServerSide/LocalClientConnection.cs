
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
    public class LocalClientConnection<T> : ClientConnection<T>
    {
        public ClientSide.Client client { get; internal set; }
        public override IPAddress GetAddress() => new IPAddress(new byte[] { 127, 0, 0, 1 });
        public bool IsLocal() => true;
        public bool IsActive() => server.IsActive();
        public IServer GetServer() => server;
        public bool AuthFinished() => true;
        public void FinishAuth()
        {

        }
        protected LocalClientConnection(IServer server)
        {
            client = new ClientSide.Client(this);
            NetMain.ClientLogger.Log("Local client initialized");
            this.server = server as Server<T>;
        }
        
        internal static LocalClientConnection<T> LocalClient(IServer server)
        {
            return new LocalClientConnection<T>(server);
        }
        public override void SendPacket(Packet packet)
        {
            client.RecievePacket(packet);
        }
        public void RecievePacket(Packet packet)
        {
            packet.Execute(this); 
        }
        public override void Update()
        {
            return;
        }
        public override void Kick()
        {
        }
        public override void Kick(string message)
        {
        }
        public override void Dispose()
        { 

        }
    }
}