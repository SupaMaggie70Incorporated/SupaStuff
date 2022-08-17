using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-521341, true, false)]
    internal sealed class S2CKickPacket : Packet
    {
        public string message;
        public const string defaultMessage = "You have been kicked from the server!";
        protected override byte[] Bytify()
        {
            return Encoding.ASCII.GetBytes(message);
        }
        public override void Execute(ClientConnection sender)
        {
            NetMain.ClientLogger.Log("You have been kicked from the server for:\n    " + message);
            ClientSide.Client.Instance?.Dispose();
        }
        public S2CKickPacket(byte[] bytes)
        {
            message = Encoding.ASCII.GetString(bytes);
        }
        public S2CKickPacket(string message = defaultMessage)
        {
            this.message = message;
        }
        public static bool IsAllowedSize(int size)
        {
            return size < 1024;
        }
    }
}