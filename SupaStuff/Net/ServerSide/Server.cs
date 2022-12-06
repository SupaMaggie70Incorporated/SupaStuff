using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using SupaStuff.Net.Packets;
using SupaStuff.Net.Packets.BuiltIn;

namespace SupaStuff.Net.ServerSide
{
    public class Server<T> : IServer
    {
        public TcpListener Listener { get; private set; }
        public int Port = 12345;
        public bool Active { get; private set; }
        public LocalClientConnection<T> LocalConnection { get; private set; }
        public List<IClientConnection> connections { get; private set; }
        public readonly int MaxConnections;
        public readonly byte[] Password;
        public byte[] GetPassword() => Password;
        public bool IsActive() => Active;
        public IClientConnection GetLocalConnection() => LocalConnection;
        public List<IClientConnection> GetConnections() => connections;
        public bool Listening = false;
        public Server(int maxConnections, int port, byte[] password)
        {
            this.Password = password;
            this.Port = port;
            Active = true;
            NetMain.ServerInstance = this;
            this.MaxConnections = maxConnections;
            connections = new List<IClientConnection>(maxConnections);
            Listener = new TcpListener(NetMain.Host, port);
            NetMain.ServerLogger.Log("Server started");
            LocalConnection = LocalClientConnection<T>.LocalClient(this);
            connections.Add(LocalConnection);
            NetMain.NetLogger.Log("Server started!");
        }
        public void StartListening()
        {
            Listener.Start();
            Listening = true;
        }
        public void StopListening()
        {
            Listener.Stop();
            Listening = false;
        }

        public void Update()
        {
            if (!Active) return;
            if(Listening)
            {
                while (Listener.Pending())
                {
                    ClientConnected(new ClientConnection<T>(Listener.AcceptTcpClient()));
                }
            }
            for (int i = 0; i < connections.Count; i++)
            {
                IClientConnection connection = connections[i];


                if (connection == null)
                {
                    connections.RemoveAt(i);
                    i--;
                }
                else if (!Active)
                {
                    NetMain.ServerLogger.Log("Kicking " + connection.GetAddress() + " because they should've already been kicked, as the server is shut down");
                    connections[i].Dispose();
                    
                    i--;
                    continue;
                }
                try
                {
                    connection.Update();
                }
                catch
                {
                    NetMain.ServerLogger.Log("Kicking " + connection + " because they were unable to update properly");
                    connection.Dispose();
                }
            }
        }
        internal virtual void ClientConnected(ClientConnection<T> connection)
        {
            try
            {
                NetMain.ServerLogger.Log("Attempted connection from " + connection.Address + "!");
                if (connections.Count + 1 < MaxConnections)
                {
                    connections.Add(connection);
                    ClientConnectedEvent(connection);
                }
                else
                {
                    NetMain.ServerLogger.Warn("Rejected connection from " + connection.Address + " because we already have the max number of concurrent connections, " + MaxConnections + "!");
                    connection.SendPacket(new S2CKickPacket("The server is already full!"));
                    connection.Dispose();
                }
            }
            catch
            {
                if (this != null)
                {
                    Dispose();
                }
            }
        }
        public void Dispose()
        {
            if (!Active) return;
            Active = false;
            //List<ClientConnection> connections = this.connections;
            //this.connections = null;
            foreach (IClientConnection connection in connections)
            {
                if (connection == null || !connection.IsActive()) return;
                NetMain.ServerLogger.Log("Closing connection to " + connection.GetAddress() + " because we are shutting down the server");
                Kick(connection, "Server is shutting down.");

            }
            Listener.Stop();
            
            connections.Clear();
            NetMain.NetLogger.Log("Closing server");
        }
        public event Action<IClientConnection> OnClientConnected;
        private void ClientConnectedEvent(ClientConnection<T> connection)
        {
            if (OnClientConnected == null) return;
            OnClientConnected.Invoke(connection);
        }
        public void SendToAll(Packet packet)
        {
            if (!IsActive()) return;
            IClientConnection[] _connections = new IClientConnection[connections.Count];
            for (int i = 0; i < _connections.Length; i++)
            {
                _connections[i] = connections[i];
            }
            for (int i = 0; i < _connections.Length; i++)
            {
                IClientConnection connection = _connections[i];
                try
                {
                    connection.SendPacket(packet);
                }
                catch
                {
                    NetMain.ServerLogger.Warn($"We couldnt send a packet to {connection.GetAddress()} because the connection was closed");
                    Dispose();
                }
            }
        }
        /// <summary>
        /// Kick someone, after sending packet with the message
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        public void Kick(IClientConnection connection, string message)
        {
            NetMain.NetLogger.Log("Kicking " + connection.GetAddress() + " for reason: " + message);
            connection.Kick(message);
            foreach (IClientConnection conn in connections)
            {
                if (conn == connection)
                {
                    conn.Kick();
                    break;
                }
            }
        }
        /// <summary>
        /// Instantly kick someone
        /// </summary>
        /// <param name="connection"></param>
        public void Kick(IClientConnection connection)
        {
            NetMain.ServerLogger.Log("Kicking " + connection.GetAddress() + " without telling them");
            connection.Dispose();
            foreach(IClientConnection conn in connections)
            {
                if (conn == connection)
                {
                    conn.Kick();
                    break;
                }
            }

        }
        /// <summary>
        /// Create a new local connection to the server
        /// </summary>
        /// <returns></returns>
        public IClientConnection MakeLocalConnection()
        {
            if (connections.Count + 1 == MaxConnections) return null;
            IClientConnection connection = LocalClientConnection<T>.LocalClient(this);
            connections.Add(connection);
            return connection;
        }

    }
}