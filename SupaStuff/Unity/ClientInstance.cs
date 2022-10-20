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


namespace SupaStuff.Unity
{
    [UnitySpecific]
    public class ClientInstance<T> : MonoBehaviour
    {
        public static ClientInstance<T> Instance = null;
        public Client client = null;
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
            if (client != null && client.IsActive) client.Update();
        }
        public void Initialize(IPAddress address, int port, byte[] password)
        {
            client = new Client(address, port, password);
            client.OnConnected += () =>
            {
                client.packetStream.OnDisconnected += () => { Debug.Log("Disconnected"); };
            };
        }
        internal protected void InitializeLocal(LocalClientConnection<T> connection)
        {
            Debug.Log("Started local client");
            client = connection.client;
            Debug.Log("Finished");
        }
        public void OnFailedConnection()
        {
            Debug.Log("Client Failed to Connect!");
            client.OnConnected -= OnConnected;
            client.OnDispose -= OnFailedConnection;
            client = null;
        }
        public void OnConnected()
        {
            Debug.Log("Client Successfully Connected!");
            Instance.client.OnConnected -= OnConnected;
            Instance.client.OnDispose -= OnFailedConnection;
        }
    }
}
