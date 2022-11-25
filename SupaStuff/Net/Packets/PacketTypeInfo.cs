using System;
using System.Collections.Generic;
using System.Text;

namespace SupaStuff.Net.Packets
{
    internal struct PacketTypeInfo
    {
        public Type Type;
        public Func<byte[], Packet> Constructor;
        public Func<int, bool> IsRightLength;

    }
}