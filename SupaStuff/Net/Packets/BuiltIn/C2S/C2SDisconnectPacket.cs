using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-5662131, false)]
    internal sealed class C2SDisconnectPacket : Packet
    {
        public readonly string Message;
        const string defaultMessage = "We are leaving now, goodbye!";
        protected override byte[] Bytify()
        {
            return Encoding.ASCII.GetBytes(Message);
        }
        public override void Execute(IClientConnection sender)
        {
            NetMain.ServerLogger.Log("Client " + sender.GetAddress().ToString() + " disconnected from server for:\n    " + Message);
            sender.Dispose();
        }
        public C2SDisconnectPacket(byte[] bytes)
        {
            Message = Encoding.ASCII.GetString(bytes);
        }
        public C2SDisconnectPacket(string message = defaultMessage)
        {
            this.Message = message;
        }
        public static new bool IsAllowedSize(int size)
        {
            return size < 1024;
        }
    }
}