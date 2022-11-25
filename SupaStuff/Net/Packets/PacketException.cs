using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupaStuff.Net.Packets
{
    public class PacketException : Exception
    {
        public PacketException() : base("Packet exception occured!")
        {
        }
        public PacketException(string message) : base(message)
        {
        }
        public override string ToString()
        {
            return Message;
        }
    }
}