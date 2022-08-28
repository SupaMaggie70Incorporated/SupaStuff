using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Net.Packets
{
    internal struct PacketTypeInfo
    {
        public Type type;
        public Func<byte[], Packet> constructor;
        public Func<int, bool> isRightLength;

    }
}