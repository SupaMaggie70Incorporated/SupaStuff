using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


using UnityEngine;


using SupaStuff.Net.ClientSide;
using SupaStuff.Net.ServerSide;
using SupaStuff.Net.Packets;
using static System.Net.Mime.MediaTypeNames;
using SupaStuff.Net;
using SupaStuff.Util;

namespace SupaStuff.Unity
{
    [UnitySpecific]
    public abstract class ClientInstance<T> : MonoBehaviour
    {
        public static ClientInstance<T> Instance = null;
        public Client client = null;
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
            if (client == null) return;
            if (client.IsActive) client.Update();
            if (client.Disposed)
            {
                OnFailedConnection();
                client = null;
            }
        }
        public virtual void Initialize(IPAddress address, int port, byte[] password)
        {
            if(address == null)
            {
                throw new ArgumentException("Address cannot be null!");
            }
            else if(port < 0 || port > ushort.MaxValue)
            {
                throw new ArgumentException("Port must be in the range of [0,65535]!");
            }
            if(password == null) password = new byte[0];
            client = new Client(address, port, password);
            client.OnConnected += () =>
            {
                client.OnDispose += () => { NetMain.ClientLogger.Log("Connected"); };
            };
        }
        internal protected virtual void InitializeLocal(LocalClientConnection<T> connection)
        {
            NetMain.ClientLogger.Log("Started local client");
            client = connection.Client;
        }
        protected virtual void OnFailedConnection()
        {
            NetMain.ClientLogger.Warn("Connection failed");
            client.OnConnected -= OnConnected;
            client.OnDispose -= OnFailedConnection;
            client = null;
        }
        protected virtual void OnConnected()
        {
            Debug.Log("Client Successfully Connected!");
            Instance.client.OnConnected -= OnConnected;
            Instance.client.OnDispose -= OnFailedConnection;
        }
    }
}
