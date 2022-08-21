
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
    public class LocalClientConnection : ClientConnection
    {
        public ClientSide.Client client { get; internal set; }
        private LocalClientConnection()
        {
            this.IsLocal = true;
            address = new IPAddress(new byte[] {127,0,0,1});
            client = new ClientSide.Client(this);
            NetMain.ClientLogger.Log("Local client initialized");
        }
        internal static LocalClientConnection LocalClient()
        {
            return new LocalClientConnection();
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
            if (!IsActive) return;
            IsActive = false;
            try
            {
                Server.Instance.connections.Remove(this);
            }
            catch { }
        }
    }
}