using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Net.Packets;
using SupaStuff.Net.ServerSide;

using Main = SupaStuff.Net.NetMain;

namespace SupaStuff.Net.Example
{
    [APacket(21245673, true)]
    public class ExamplePacket2 : Packet
    {
        protected override byte[] Bytify()
        {
            return new byte[0];
        }
        public override void Execute(ClientConnection sender)
        {
            Main.ServerLogger.Log("S2C packet recieved!");
        }
        public ExamplePacket2(byte[] bytes)
        {
        }
        public ExamplePacket2()
        {
        }
    }
}