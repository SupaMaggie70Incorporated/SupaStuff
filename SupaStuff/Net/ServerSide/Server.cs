using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using SupaStuff.Net.Packets;


namespace SupaStuff.Net.ServerSide
{
    public class Server<T> : IServer
    {
        public TcpListener listener { get; private set; }
        public int port = 12345;
        public bool isActive { get; private set; }
        public LocalClientConnection localConnection { get; private set; }
        public List<IClientConnection> connections { get; private set; }
        public readonly int maxConnections;
        public readonly byte[] Password;
        public byte[] GetPassword() => Password;
        public bool IsActive() => isActive;
        public LocalClientConnection GetLocalConnection() => localConnection;
        public List<IClientConnection> GetConnections() => connections;
        public Server(int maxConnections, int port, byte[] password)
        {
            this.Password = password;
            this.port = port;
            isActive = true;
            NetMain.ServerInstance = this;
            this.maxConnections = maxConnections;
            connections = new List<IClientConnection>(maxConnections);
            listener = new TcpListener(NetMain.host, port);
            StartListening();
            NetMain.ServerLogger.Log("Server started");
            listener.BeginAcceptTcpClient(new System.AsyncCallback(ClientConnected), null);
            NetMain.ServerLogger.Log("Accepting tcp client");
            localConnection = LocalClientConnection.LocalClient(this);
            connections.Add(localConnection);
            NetMain.NetLogger.Log("Server started!");
        }
        public void StartListening()
        {
            listener.Start();
        }
        public void StopListening()
        {
            listener.Stop();
        }

        public void Update()
        {
            if (!isActive) return;
            for (int i = 0; i < connections.Count; i++)
            {
                IClientConnection connection = connections[i];
                if (connection == null)
                {
                    connections.RemoveAt(i);
                    i--;
                }
                else if (!isActive)
                {
                    NetMain.ServerLogger.Log("Kicking " + connection.GetAddress() + " because they should've already been kicked");
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
        internal virtual void ClientConnected(IAsyncResult ar)
        {
            try
            {
                ClientConnection<T> connection = new ClientConnection<T>(listener.EndAcceptTcpClient(ar));
                NetMain.ServerLogger.Log("Attempted connection from " + connection.address + "!");
                if (connections.Count + 1 < maxConnections)
                {
                    connections.Add(connection);
                    ClientConnectedEvent(connection);
                }
                else
                {
                    NetMain.ServerLogger.Log("Rejected connection from " + connection.address + " because we already have the max number of concurrent connections, " + maxConnections + "!");
                    connection.Dispose();
                }
                listener.BeginAcceptTcpClient(new System.AsyncCallback(ClientConnected), null);
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
            if (!isActive) return;
            isActive = false;
            //List<ClientConnection> connections = this.connections;
            //this.connections = null;
            foreach (IClientConnection connection in connections)
            {
                if (connection == null || !connection.IsActive()) return;
                NetMain.ServerLogger.Log("Closing connection to " + connection.GetAddress() + " because we are shutting down the server");
                Kick(connection, "Server is shutting down.");

            }
            listener.Stop();
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
            foreach (IClientConnection connection in connections)
            {
                connection.SendPacket(packet);
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
            connections.Remove(connection);
        }
        /// <summary>
        /// Instantly kick someone
        /// </summary>
        /// <param name="connection"></param>
        public void Kick(IClientConnection connection)
        {
            NetMain.ServerLogger.Log("Kicking " + connection.GetAddress() + " because we want to kick him idk");
            connection.Dispose();
            connections.Remove(connection);

        }
        /// <summary>
        /// Create a new local connection to the server
        /// </summary>
        /// <returns></returns>
        public LocalClientConnection MakeLocalConnection()
        {
            if (connections.Count + 1 == maxConnections) return null;
            LocalClientConnection connection = LocalClientConnection.LocalClient(this);
            connections.Add(connection);
            return connection;
        }

    }
}