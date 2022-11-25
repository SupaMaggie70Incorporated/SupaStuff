using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Net.Packets.BuiltIn
{
    /// <summary>
    /// This packet is to tell the server to keep the connection alive if no other packets were sent recently
    /// </summary>
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

        public static new bool IsAllowedSize(int size)
        {
            return size == 0;
        }
    }
}