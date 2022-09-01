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
        public override void Execute(IClientConnection sender)
        {
            if (SupaMath.ByteArraysEqual(bytes, NetMain.ServerInstance.GetPassword()))
            {
                sender.FinishAuth();
                NetMain.ServerLogger.Log(sender.GetAddress() + " finished authorizing!");
            }
            else
            {
                NetMain.ServerLogger.Log(sender.GetAddress() + " entered the wrong password! It was " + Encoding.ASCII.GetString(NetMain.ServerInstance.GetPassword()) + " but they thought it was " + Encoding.ASCII.GetString(bytes));
                sender.Dispose();
            }
        }
        public C2SWelcomePacket()
        {
            bytes = NetMain.ClientInstance.Password;
        }
        public C2SWelcomePacket(byte[] bytes)
        {
            this.bytes = bytes;
        }
        public static bool IsAllowedSize(int size)
        {
            return size == NetMain.ServerInstance.GetPassword().Length;
        }
    }
}