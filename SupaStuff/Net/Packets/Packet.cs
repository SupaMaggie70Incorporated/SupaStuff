using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SupaStuff.Net.ClientSide;
using SupaStuff.Net.ServerSide;
using SupaStuff.Util;
using System.Net.Sockets;

namespace SupaStuff.Net.Packets
{
    public abstract class Packet : IDisposable
    {
        internal byte[] Data;
        public static bool IsAllowedSize(int size)
        {
            return true;
        }
        public static Packet GetPacket(int packetid, byte[] packetbytes, bool isS2C)
        {
            Packet packet = null;
            try
            {
                Func<byte[], Packet> func = PacketTypesFinder.GetConstructor(packetid, isS2C);
                if (func == null)
                {
                    throw new PacketException($"Invalid {(isS2C ? "S2C" : "C2S")} packet id: {packetid} does not match any {(isS2C ? "S2C" : "C2S")} ids!");
                }
                
                packet = func(packetbytes);
            }
            catch (Exception e)
            {
                NetMain.NetLogger.Error($"Error reconstructing packet of type {PacketTypesFinder.GetPacket(packetid, isS2C).FullName}. Error shown below.");
                NetMain.NetLogger.Error(e);
            }
            if (packet == null)
            { 
                throw new PacketException($"Packet constructor for packet type {(isS2C ? PacketTypesFinder.S2CTypes[packetid] : PacketTypesFinder.C2STypes[packetid]).Type.ToString()}");
            }
            return packet;
        }
        internal byte[] GenerateHeader()
        {
            try
            {
                Data = Bytify();
            }
            catch(Exception e)
            {
                NetMain.NetLogger.Error($"Error executing packet of type {GetType().FullName} as it threw an error shown below");
                NetMain.NetLogger.Error(e.ToString());
                Data = new byte[0];
            }
            byte[] arr = new byte[8];
            byte[] id = BitConverter.GetBytes(GetID());
            byte[] size = BitConverter.GetBytes(Data.Length);
            Buffer.BlockCopy(id, 0, arr, 0, 4);
            Buffer.BlockCopy(size, 0, arr, 4, 4);
            return arr;
        }
        protected abstract byte[] Bytify();
        public abstract void Execute(IClientConnection sender);

        public virtual int GetID()
        {
            return GetType().GetCustomAttribute<APacket>().PacketID;
        }
        public void Dispose()
        {
        }
        public static bool ExecutePacket(Packet packet, bool isS2C, IClientConnection sender)
        {
            try
            {
                packet.Execute(sender);
                return true;
            }
            catch(Exception e)
            {
                Logger logger = isS2C ? NetMain.ClientLogger : NetMain.ServerLogger;
                logger.Error($"Error executing packet of type {packet.GetType().FullName} as it threw an error shown below");
                logger.Error(e.ToString());
                return false;
                
            }
        }
    }
}