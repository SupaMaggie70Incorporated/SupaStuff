using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Net.Packets;
using SupaStuff.Net.ServerSide;



namespace SupaStuff.Net.Example
{
    [APacket(1123453, false)]
    public class ExamplePacket : Packet
    {
        public short num;
        public override void Execute(IClientConnection sender)
        {
            NetMain.NetLogger.Log("Number: " + num.ToString());
            return;
        }
        protected override byte[] Bytify()
        {
            byte[] data = new byte[2];
            byte[] shortbytes = BitConverter.GetBytes(num);
            Buffer.BlockCopy(shortbytes, 0, data, 0, 2);
            return data;
        }
        public ExamplePacket(byte[] bytes)
        {
            num = BitConverter.ToInt16(bytes, 0);
        }
        public ExamplePacket(short num)
        {
            this.num = num;
        }
    }
}