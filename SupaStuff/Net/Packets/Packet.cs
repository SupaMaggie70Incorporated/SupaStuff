using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupaStuff.Net.Packets.Util;
using System.Reflection;
using SupaStuff.Net.ClientSide;
using SupaStuff.Net.ServerSide;
namespace SupaStuff.Net.Packets
{
    public abstract class Packet : IDisposable
    {
        internal byte[] data;
        public static bool IsAllowedSize(int size)
        {
            return true;
        }
        public static Packet GetPacket(byte[] contents, bool isS2C)
        {
            if (contents.Length < 8)
            {
                if (isS2C)
                {
                    throw new PacketException("Incomplete packet recieved!");
                }
                else
                {
                    throw new PacketException("Incomplete packet recieved!");
                }
                throw new PacketException($"Incomplete packet recieved from {(isS2C ? "server" : "client")}!");
            }
            int packetid = BitConverter.ToInt32(contents, 0);
            int size = BitConverter.ToInt32(contents, 4);
            byte[] bytes = new byte[contents.Length - 8];
            Buffer.BlockCopy(contents, 8, bytes, 0, contents.Length - 8);
            return GetPacket(packetid, bytes, isS2C);
        }
        public static Packet GetPacket(int packetid, byte[] packetbytes, bool isS2C)
        {
            Packet packet = null;
            try
            {
                Func<byte[], Packet> func = PacketTypesFinder.GetConstructor(packetid, isS2C);
                if (func == null)
                {
                    throw new PacketException($"Invalid {(isS2C ? "S2C" : "C2S")} packet id: {packetid.ToString()} does not match any {(isS2C ? "S2C" : "C2S")} ids!");
                }
                packet = func(packetbytes);
            }
            catch
            {

            }
            if (packet == null)
            {
                throw new PacketException("Constructor failure somehow idk");
            }
            return packet;
        }
        internal byte[] GenerateHeader()
        {
            byte[] arr = new byte[8];
            byte[] id = BitConverter.GetBytes(GetID());
            data = Bytify();
            byte[] size = BitConverter.GetBytes(data.Length);
            Buffer.BlockCopy(id, 0, arr, 0, 4);
            Buffer.BlockCopy(size, 0, arr, 4, 4);
            return arr;
        }
        protected abstract byte[] Bytify();
        public abstract void Execute(ClientConnection sender);
        public int GetID()
        {
            return GetType().GetCustomAttribute<APacket>().PacketID;
        }
        public void Dispose()
        {
        }
    }
}