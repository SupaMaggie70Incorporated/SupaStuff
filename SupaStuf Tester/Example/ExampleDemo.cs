using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            testServer.StartListening();
            Main.NetLogger.Log("Starting Server at\n     " + NetMain.Host.ToString() + ":" + testServer.Port);


            Console.ReadKey();

            client = new ClientSide.Client(NetMain.Host, port, password);
            Console.WriteLine("Setup client");
            client.OnConnected += () => { Console.WriteLine("Connected"); };
            Console.WriteLine("Updating server & client");


            Console.ReadKey();


            testServer.Update();
            Console.WriteLine("Updated server");
            client.Update();
            Console.WriteLine("Updated client");

            Main.NetLogger.Log("Client Started");


            Console.ReadKey();

            testServer.Update();

            Console.ReadKey();

            client.Dispose();
            Main.NetLogger.Log("Client closing");

            Console.ReadKey();

            Main.NetLogger.Log("Closing Server..." + testServer.connections.Count);
            testServer.Dispose();
            isRunning = false;


            Console.ReadKey();


            Main.NetLogger.Log("Completed Successfully");

        }


    }
}