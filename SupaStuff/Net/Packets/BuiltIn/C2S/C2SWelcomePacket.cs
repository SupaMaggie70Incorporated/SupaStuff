using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;
using SupaStuff.Math;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-538927, false)]
    internal class C2SWelcomePacket : Packet
    {
        public readonly byte[] Bytes;
        protected override byte[] Bytify()
        {
            return Bytes;
        }
        public override void Execute(IClientConnection sender)
        {
            if (SupaMath.ByteArraysEqual(Bytes, NetMain.ServerInstance.GetPassword()))
            {
                sender.FinishAuth();
                NetMain.ServerLogger.Log(sender.GetAddress() + " finished authorizing!");
            }
            else
            {
                NetMain.ServerLogger.Warn(sender.GetAddress() + " entered the wrong password! It was " + Encoding.ASCII.GetString(NetMain.ServerInstance.GetPassword()) + " but they thought it was " + Encoding.ASCII.GetString(Bytes));
                sender.Dispose();
            }
        }
        public C2SWelcomePacket()
        {
            Bytes = NetMain.ClientInstance.Password;
        }
        public C2SWelcomePacket(byte[] bytes)
        {
            this.Bytes = bytes;
        }
        public static new bool IsAllowedSize(int size)
        {
            return size == NetMain.ServerInstance.GetPassword().Length;
        }
    }
}