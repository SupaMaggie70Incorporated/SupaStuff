using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Util;
using System.Net;
using System.Net.Sockets;
namespace SupaStuff.Net
{
    public static class NetMain
    {
        public static void GetHost()
        {
            var hosts = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in hosts.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    host = ip;
                    return;
                }
            }
        }
        public static IPAddress host { get; private set; }

        public static Logger NetLogger { get; private set; }
        public static Logger ServerLogger { get; private set; }
        public static Logger ClientLogger { get; private set; }
        public static void Init()
        {
            NetLogger = Logger.GetLogger("Net/Main");
            ServerLogger = Logger.GetLogger("Net/Server");
            ClientLogger = Logger.GetLogger("Net/Client");
            Packets.Util.PacketTypesFinder.GetTypes();
            GetHost();
        }
        public static void Update()
        {
            if (ServerSide.Server.Instance != null)
            {
                ServerSide.Server.Instance.Update();
            }
            if (ClientSide.Client.Instance != null)
            {
                ClientSide.Client.Instance.Update();
            }
        }
    }
}