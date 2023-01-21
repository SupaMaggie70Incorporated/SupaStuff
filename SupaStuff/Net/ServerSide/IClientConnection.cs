using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SupaStuff.Net.ServerSide
{
    public interface IClientConnection : IDisposable
    {
        IPAddress GetAddress();
        bool IsLocal();
        bool IsActive();
        
        void SendPacket(Packets.Packet packet);

        void Update();

        event Action OnDispose;

        void Kick(string message);

        void Kick();
        void RecievePacket(Packets.Packet packet);
        void FinishAuth();
        bool AuthFinished();
    }
}
