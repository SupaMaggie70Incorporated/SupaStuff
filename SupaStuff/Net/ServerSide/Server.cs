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
        private TcpListener listener;
        public int Port {get; private set;}
        public bool Active { get; private set; }
        public LocalClientConnection<T> LocalConnection { get; private set; }
        internal List<IClientConnection> _connections;
        public List<IClientConnection> Connections { 
            get
            {
                return new List<IClientConnection>(_connections);
            }
            private set{
                _connections = value;
            }
        }
        public int MaxConnections { get; private set; }
        private byte[] _password;
        public byte[] Password {
            get 
            { 
                return (byte[])_password.Clone(); 
            }
            set
            {
                _password = value;
            }
        }
        public byte[] GetPassword() => Password;
        public bool IsActive() => Active;
        public IClientConnection GetLocalConnection() => LocalConnection;
        public List<IClientConnection> GetConnections() => _connections;
        private bool _listening;
        public bool Listening
        {
            get
            {
                return _listening;
            }
            set
            {
                if (value && !_listening) StartListening();
                else if (!value && _listening) StopListening();
            }
        }

        public Server(int maxConnections, int port, byte[] password)
        {
            this._password = password;
            this.Port = port;
            Active = true;
            NetMain.ServerInstance = this;
            this.MaxConnections = maxConnections;
            _connections = new List<IClientConnection>(maxConnections);
            listener = new TcpListener(NetMain.Host, port);
            _listening = false;
            NetMain.ServerLogger.Log("Server started");
            LocalConnection = LocalClientConnection<T>.LocalClient(this);
            _connections.Add(LocalConnection);
            NetMain.NetLogger.Log("Server started!");
        }
        ~Server()
        {
            Dispose();
        }
        public void StartListening()
        {
            listener.Start();
            _listening = true;
        }
        public void StopListening()
        {
            listener.Stop();
            _listening = false;
        }

        public void Update()
        {
            if (!Active) return;
            if(_listening)
            {
                while (listener.Pending())
                {
                    ClientConnected(new ClientConnection<T>(listener.AcceptTcpClient()));
                }
            }
            for (int i = 0; i < _connections.Count; i++)
            {
                IClientConnection connection = _connections[i];


                if (connection == null)
                {
                    _connections.RemoveAt(i);
                    i--;
                }
                else if (!Active)
                {
                    NetMain.ServerLogger.Log("Kicking " + connection.GetAddress() + " because they should've already been kicked, as the server is shut down");
                    _connections[i].Dispose();
                    
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
        internal void ClientConnected(ClientConnection<T> connection)
        {
            try
            {
                NetMain.ServerLogger.Log("Attempted connection from " + connection.Address + "!");
                if (_connections.Count + 1 < MaxConnections)
                {
                    _connections.Add(connection);
                    OnClientConnected?.Invoke(connection);
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
            GC.SuppressFinalize(this);
            //List<ClientConnection> connections = this.connections;
            //this.connections = null;
            foreach (IClientConnection connection in _connections)
            {
                if (connection == null || !connection.IsActive()) return;
                NetMain.ServerLogger.Log("Closing connection to " + connection.GetAddress() + " because we are shutting down the server");
                Kick(connection, "Server is shutting down.");

            }
            listener.Stop();
            
            _connections.Clear();
            NetMain.NetLogger.Log("Closing server");
        }
        public event Action<IClientConnection> OnClientConnected;
        public void SendToAll(Packet packet)
        {
            if (!IsActive()) return;
            for (int i = 0; i < _connections.Count; i++)
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
            foreach (IClientConnection conn in _connections)
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
            foreach(IClientConnection conn in _connections)
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
            if (_connections.Count + 1 == MaxConnections) return null;
            IClientConnection connection = LocalClientConnection<T>.LocalClient(this);
            _connections.Add(connection);
            return connection;
        }

    }
}