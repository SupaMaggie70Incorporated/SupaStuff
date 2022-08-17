using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;
using SupaStuff.Math;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-538927, false, false)]
    internal class C2SWelcomePacket : Packet
    {
        public readonly byte[] bytes;
        protected override byte[] Bytify()
        {
            NetMain.ClientLogger.Log("Bytifying password packet with password " + Encoding.ASCII.GetString(bytes));
            return bytes;
        }
        public override void Execute(ClientConnection sender)
        {
            if (SupaMath.ByteArraysEqual(bytes, Server.Instance.Password))
            {
                sender.finishAuth = true;
                NetMain.ServerLogger.Log(sender.address + " finished authorizing!");
            }
            else
            {
                NetMain.ServerLogger.Log(sender.address + " entered the wrong password! It was " + Encoding.ASCII.GetString(Server.Instance.Password) + " but they thought it was " + Encoding.ASCII.GetString(bytes));
                sender.Dispose();
            }
        }
        public C2SWelcomePacket()
        {
            bytes = Server.Instance.Password;
        }
        public C2SWelcomePacket(byte[] bytes)
        {
            this.bytes = bytes;
        }
        public static bool IsAllowedSize(int size)
        {
            return size == Server.Instance.Password.Length;
        }
    }
}