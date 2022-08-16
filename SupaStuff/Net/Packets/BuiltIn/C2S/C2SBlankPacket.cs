using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-194752,false,false)]
    internal class C2SBlankPacket : Packet
    {
        public override byte[] Bytify()
        {
            return new byte[0];
        }
        public override void Execute(ClientConnection sender)
        {
        }
        public C2SBlankPacket() : base(null)
        {

        }
        public C2SBlankPacket(byte[] bytes) : base(null)
        {
        }

        public static bool IsAllowedSize(int size)
        {
            return size == 0;
        }
    }
}