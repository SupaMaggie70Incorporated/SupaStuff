using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using UnityEngine;

using SupaStuff.Net.ServerSide;


namespace SupaStuff.Unity
{
    [UnitySpecific]
    public class ServerInstance<T> : MonoBehaviour
    {
        public static ServerInstance<T> Instance;
        public Server<T> server;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        private void FixedUpdate()
        {
            if (server != null && server.Active) server.Update();
        }
        public void InitializeDedicated(int maxConnections, int port, byte[] password)
        {
            server = new Server<T>(maxConnections, port, password);
            server.LocalConnection.Dispose();
        }
        public void InitializeLocal(int maxConnections, int port, byte[] password)
        {
            server = new Server<T>(maxConnections, port, password);
            ClientInstance<T>.Instance.InitializeLocal(server.LocalConnection);
        }
    }
}
