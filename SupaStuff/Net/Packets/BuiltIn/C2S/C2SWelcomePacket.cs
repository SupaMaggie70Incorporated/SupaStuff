using SupaStuff.Net.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;
using SupaStuff.Math;
//Temporary fix xd
using Main = SupaStuff.Net.NetMain;


namespace SupaStuff.Net.Packets.BuiltIn
{
    [APacket(-538927,false,false)]
    internal class C2SWelcomePacket : Packet
    {
        public readonly byte[] bytes;
        public override byte[] Bytify()
        {
            Main.ClientLogger.Log("Bytifying password packet with password " + Encoding.ASCII.GetString(bytes));
            return bytes;
        }
        public override void Execute(ClientConnection sender)
        {
            if(SupaMath.ByteArraysEqual(bytes,Server.Instance.Password))
            {
                sender.finishAuth = true;
                Main.ServerLogger.Log(sender.address + " finished authorizing!");
            }
            else
            {
                Main.ServerLogger.Log(sender.address + " entered the wrong password! It was " + Encoding.ASCII.GetString(Server.Instance.Password) + " but they thought it was " + Encoding.ASCII.GetString(bytes));
                sender.Dispose();
            }
        }
        public C2SWelcomePacket() : base(null)
        {
            bytes = Server.Instance.Password;
        }
        public C2SWelcomePacket(byte[] bytes) : base(null)
        {
            this.bytes = bytes;
        }
        public static bool IsAllowedSize(int size)
        {
            return size == Server.Instance.Password.Length;
        }
    }
}