using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using UnityEngine;

using SupaStuff.Net.ServerSide;
using SupaStuff.Net;

namespace SupaStuff.Unity
{
    [UnitySpecific]
    public abstract class ServerInstance<T> : MonoBehaviour
    {
        public static ServerInstance<T> Instance;
        public Server<T> server;
        protected virtual void Awake()
        {
            Util.Logger.SetUnity();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                if (!NetMain.Initialized) NetMain.Init();
            }
            else
            {
                Destroy(this);
            }
        }
        protected virtual void FixedUpdate()
        {
            if (server != null && server.Active) server.Update();
        }
        public virtual void InitializeDedicated(int maxConnections, int port, byte[] password)
        {
            server = new Server<T>(maxConnections, port, password);
            server.LocalConnection.Dispose();
        }
        public virtual void InitializeLocal(int maxConnections, int port, byte[] password)
        {
            server = new Server<T>(maxConnections, port, password);
            server.OnClientConnected += (IClientConnection connection) => OnClientConnected(connection as ClientConnection<T>);
            ClientInstance<T>.Instance.InitializeLocal(server.LocalConnection);
        }
        public abstract void OnClientConnected(ClientConnection<T> connection);
    }
}
