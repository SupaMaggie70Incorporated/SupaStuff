using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-5662131, false, false)]
    internal sealed class C2SDisconnectPacket : Packet
    {
        public readonly string message;
        public const string defaultMessage = "We are leaving now, goodbye!";
        protected override byte[] Bytify()
        {
            return Encoding.ASCII.GetBytes(message);
        }
        public override void Execute(IClientConnection sender)
        {
            NetMain.ServerLogger.Log("Client " + sender.GetAddress().ToString() + " disconnected from server for:\n    " + message);
            sender.Dispose();
        }
        public C2SDisconnectPacket(byte[] bytes)
        {
            message = Encoding.ASCII.GetString(bytes);
        }
        public C2SDisconnectPacket(string message = defaultMessage)
        {
            this.message = message;
        }
        public static bool IsAllowedSize(int size)
        {
            return size < 1024;
        }
    }
}