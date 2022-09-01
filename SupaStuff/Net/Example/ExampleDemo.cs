using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Net;
using SupaStuff.Net.ServerSide;


using Main = SupaStuff.Net.NetMain;

namespace SupaStuff.Net.Example
{
    public struct EmptyStruct
    {

    }
    public class ExampleDemo
    {
        public bool isRunning = true;
        public Server<EmptyStruct> testServer;
        public ClientSide.Client client;
        /// <summary>
        /// Example of how to use SupaStuff.Net
        /// 
        /// Launches local server and client
        /// Prints handshake stage to console
        /// Generates a test packet
        /// Prints to debug console
        /// Adds test packet to queue
        /// Prints when sent and received
        /// Decodes packet and prints
        /// Closes server and client
        /// </summary>

        public ExampleDemo()
        {
            const int port = 12345;
            byte[] password = new byte[] { 123, 45, 67, 89 };
            Main.Init();
            Main.NetLogger.Log("Initiating Scarry Black Window...");

            testServer = new Server<EmptyStruct>(4, port, password);
            /*
            testServer.OnClientConnected += (ClientConnection conn) => {
                conn.SendPacket(new ExamplePacket2());
            };
            */
            Main.NetLogger.Log("Starting Server at\n     " + NetMain.host.ToString() + ":" + testServer.port);


            Console.ReadKey();

            client = new ClientSide.Client(NetMain.host, port, password);
            Main.NetLogger.Log("Client Started");

            Task task = new Task(updateLoop);
            task.Start();

            Console.ReadKey();

            Console.ReadKey();
            client.SendPacket(new ExamplePacket(129));
            client.SendPacket(new ExamplePacket(129));

            Console.ReadKey();

            Main.NetLogger.Log("Closing Server..." + testServer.connections.Count);
            testServer.Dispose();
            isRunning = false;


            Console.ReadKey();


            Main.NetLogger.Log("Completed Successfully");

        }
        public void updateLoop()
        {
            while (isRunning)
            {
                client.Update();
                testServer.Update();
                Task.Delay(1000);
            }
        }


    }
}