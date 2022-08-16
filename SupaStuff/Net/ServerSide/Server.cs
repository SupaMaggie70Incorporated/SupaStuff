using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using SupaStuff.Net.Packets;

//Temporary fix xd
using Main = SupaStuff.Net.NetMain;

namespace SupaStuff.Net.ServerSide
{
    public class Server : IDisposable
    {
        public static Server Instance { get; private set; }
        public TcpListener listener { get; private set; }
        public int port = 12345;
        public bool IsActive { get; private set; }
        public LocalClientConnection localConnection { get; private set; }
        public List<ClientConnection> connections { get; private set; }
        public readonly int maxConnections;
        public readonly byte[] Password;
        public Server(int maxConnections,int port,byte[] password)
        {
this.Password = password;
this.port = port;
            IsActive = true;
            Instance = this;
            this.maxConnections = maxConnections;
            connections = new List<ClientConnection>(maxConnections);
            listener = new TcpListener(Main.host, port);
            StartListening();
            Main.ServerLogger.Log("Server started");
            listener.BeginAcceptTcpClient(new System.AsyncCallback(ClientConnected), null);
            Main.ServerLogger.Log("Accepting tcp client");
            localConnection = ClientConnection.LocalClient();
            connections.Add(localConnection);
            Main.NetLogger.Log("Server started!");
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
            for (int i = 0; i < connections.Count; i++)
            {
                ClientConnection connection = connections[i];
                if(connection == null)
                {
                    connections.RemoveAt(i);
                    i--;
                }
                else if(!IsActive)
                {
                    Main.ServerLogger.Log("Kicking " + connection.address + " because they should've already been kicked");
                    connections[i].Dispose();
                    connections.RemoveAt(i);
                    i--;
                    continue;
                }
                try { 
                    connection.Update();
                }
                catch
                {
                    Main.ServerLogger.Log("Kicking " + connection + " because they were unable to update properly");
                    connection.Dispose();
                }
            }
        }
        private void ClientConnected(System.IAsyncResult ar)
        {
            try
            {
                ClientConnection connection = new ClientConnection(listener.EndAcceptTcpClient(ar));
                Main.ServerLogger.Log("Attempted connection from " + connection.address + "!");
                if (connections.Count + 1 < maxConnections)
                {
                    connections.Add(connection);
                    ClientConnectedEvent(connection);
                }
                else
                {
                    Main.ServerLogger.Log("Rejected connection from " + connection.address + " because we already have the max number of concurrent connections, " + maxConnections + "!");
                    connection.Dispose();
                }
                listener.BeginAcceptTcpClient(new System.AsyncCallback(ClientConnected), null);
            }catch
            {
                if(this != null)
                {
                    Dispose();
                }
            }
        }
        public void Dispose()
        {
if(!IsActive) return;
IsActive = false;
//List<ClientConnection> connections = this.connections;
//this.connections = null;
            foreach (var connection in connections)
            {
                try
                {
                    Main.ServerLogger.Log("Closing connection to " + connection.address + " because we are shutting down the server");
                    connection.Dispose();
                }
                catch
                {

                }
            }
            listener.Stop();
            Instance = null;
            connections.Clear();
            Main.NetLogger.Log("Closing server");
        }
        public event Action<ClientConnection> OnClientConnected;
        private void ClientConnectedEvent(ClientConnection connection)
        {
            if (OnClientConnected == null) return;
            OnClientConnected.Invoke(connection);
        }
        public void SendToAll(Packet packet)
        { 
            foreach(ClientConnection connection in connections) 
            {
                connection.SendPacket(packet);
            }
        }
        /// <summary>
        /// Kick someone, after sending packet with the message
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        public void Kick(ClientConnection connection,string message)
        {
            Main.NetLogger.Log("Kicking " + connection.address + " for reason: " + message);
            connection.Kick(message);
        }
        /// <summary>
        /// Instantly kick someone
        /// </summary>
        /// <param name="connection"></param>
        public void Kick(ClientConnection connection)
        {
            Main.ServerLogger.Log("Kicking " + connection.address + " because we want to kick him idk");
            connection.Dispose();
            
        }
        /// <summary>
        /// Create a new local connection to the server
        /// </summary>
        /// <returns></returns>
        public LocalClientConnection MakeLocalConnection()
        {
            if (connections.Count + 1== maxConnections) return null;
            LocalClientConnection connection = LocalClientConnection.LocalClient();
            connections.Add(connection);
            return connection;
        }

    }
}