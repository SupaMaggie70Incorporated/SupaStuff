using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-521341, true)]
    internal sealed class S2CKickPacket : Packet
    {
        public string Message;
        const string defaultMessage = "You have been kicked from the server!";
        protected override byte[] Bytify()
        {
            return Encoding.ASCII.GetBytes(Message);
        }
        public override void Execute(IClientConnection sender)
        {
            NetMain.ClientLogger.Log("You have been kicked from the server for:\n    " + Message);
            NetMain.ClientInstance?.Dispose();
        }
        public S2CKickPacket(byte[] bytes)
        {
            Message = Encoding.ASCII.GetString(bytes);
        }
        public S2CKickPacket(string message = defaultMessage)
        {
            this.Message = message;
        }
        public static new bool IsAllowedSize(int size)
        {
            return size < 1024;
        }
    }
}