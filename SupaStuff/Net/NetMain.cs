using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Util;
using System.Net;
using System.Net.Sockets;

using SupaStuff.Net.ClientSide;
using SupaStuff.Net.ServerSide;
namespace SupaStuff.Net
{
    public static class NetMain
    {
        private static bool Initialized = false;
        public static void GetHost()
        {
            var hosts = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in hosts.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Host = ip;
                    return;
                }
            }
        }
        public static IPAddress Host { get; private set; }
        public static Client ClientInstance { get; internal set; }
        public static IServer ServerInstance { get;internal set; }

        public static Logger NetLogger { get; private set; }
        public static Logger ServerLogger { get; private set; }
        public static Logger ClientLogger { get; private set; }
        public static void Init()
        {
            if(Initialized) return;
            NetLogger = Logger.GetLogger("Net/Main");
            ServerLogger = Logger.GetLogger("Net/Server");
            ClientLogger = Logger.GetLogger("Net/Client");
            Packets.PacketTypesFinder.GetTypes();
            GetHost();
            Initialized = true;
            NetLogger.Log("Setup SupaStuff for networking");
        }
        public static void Update()
        {
            ServerInstance?.Update();
            ClientInstance?.Update();
        }
    }
}