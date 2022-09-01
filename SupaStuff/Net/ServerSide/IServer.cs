using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Net.ServerSide
{
    public interface IServer : IDisposable
    {
        void StartListening();
        void StopListening();
        void Update();
        event Action<IClientConnection> OnClientConnected;
        void SendToAll(Packets.Packet packet);
        void Kick(IClientConnection connection,string message);
        LocalClientConnection MakeLocalConnection();
        bool IsActive();
        byte[] GetPassword();
        LocalClientConnection GetLocalConnection();
        List<IClientConnection> GetConnections();
    }
}
