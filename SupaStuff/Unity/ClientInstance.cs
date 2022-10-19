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
            Debug.Log("Started");
            client = new Client(address, port, password);
            client.packetStream.OnDisconnected += () => { Debug.Log("Disconnected"); };
            Debug.Log("Finished");
        }
        public void InitializeLocal(LocalClientConnection<T> connection)
        {
            Debug.Log("Started local client");
            client = connection.client;
            Debug.Log("Finished");
        }
    }
}
