using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-194752, false)]
    internal class C2SBlankPacket : Packet
    {
        protected override byte[] Bytify()
        {
            return new byte[0];
        }
        public override void Execute(IClientConnection sender)
        {
        }
        public C2SBlankPacket()
        {

        }
        public C2SBlankPacket(byte[] bytes)
        {
        }

        public static bool IsAllowedSize(int size)
        {
            return size == 0;
        }
    }
}