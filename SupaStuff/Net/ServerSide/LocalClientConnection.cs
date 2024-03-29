
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
        internal Server<T> ConnectedServer;
        public ClientSide.Client Client { get; internal set; }
        public override IPAddress GetAddress() => new IPAddress(new byte[] { 127, 0, 0, 1 });
        public override bool IsLocal() => true;
        public override bool IsActive() => ConnectedServer.IsActive();
        public override bool AuthFinished() => true;
        public override void FinishAuth()
        {

        }

        public bool Disposed = false;
        protected LocalClientConnection(IServer server)
        {
            Client = new ClientSide.Client(this);
            NetMain.ClientLogger.Log("Local client initialized");
            this.ConnectedServer = server as Server<T>;
        }
        
        internal static LocalClientConnection<T> LocalClient(IServer server)
        {
            return new LocalClientConnection<T>(server);
        }
        public override void SendPacket(Packet packet)
        {
            Client.RecievePacket(packet);
        }
        public override void RecievePacket(Packet packet)
        {
            if(!Packet.ExecutePacket(packet, false, this))
            {
                Dispose();
                Client.Dispose();
            }
        }
        public override void Update()
        {
            return;
        }
        public override void Kick()
        {
            Dispose();
        }
        public override void Kick(string message)
        {
            Dispose();
        }
        public override void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            ConnectedServer?.Kick(this);
        }
    }
}