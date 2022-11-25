using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using SupaStuff.Util;


namespace SupaStuff.Net.Packets
{
    internal static class PacketTypesFinder
    {

        internal static Dictionary<int, PacketTypeInfo> C2STypes;
        internal static Dictionary<int, PacketTypeInfo> S2CTypes;
        /// <summary>
        /// Gets the classes with the APacket attribute to add to a list, for easier encoding and decoding
        /// </summary>
        public static void GetTypes()
        {
            C2STypes = new Dictionary<int, PacketTypeInfo>();
            S2CTypes = new Dictionary<int, PacketTypeInfo>();
            
            Type[] types =  TypeFinder.ReGetTypes();
            foreach (Type type in types)
            {
                APacket property = type.GetCustomAttribute<APacket>();
                if (property != null)
                {
                    if (property.IsS2C)
                    {
                        if (S2CTypes.ContainsKey(property.PacketID))
                        {
                            throw new PacketException($"Multiple S2C packets with the same id found! Fix this problem please!\nPackets sharing id:\n    {S2CTypes[property.PacketID].Type.FullName}\n    {type.FullName}");
                        }
                        ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(byte[]) });
                        Func<byte[], Packet> func = (byte[] bytes) => constructorInfo.Invoke(new object[] { bytes }) as Packet;
                        PacketTypeInfo info = new PacketTypeInfo();
                        info.Type = type;
                        info.Constructor = func;
                        info.IsRightLength = GetLengthFunc(type);
                        S2CTypes.Add(property.PacketID, info);
                    }
                    else
                    {
                        if (C2STypes.ContainsKey(property.PacketID))
                        {
                            throw new PacketException($"Multiple C2S packets with the same id found! Fix this problem please!\nPackets sharing id:\n    {S2CTypes[property.PacketID].Type.FullName}\n    {type.FullName}");
                        }
                        ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(byte[]) });
                        Func<byte[], Packet> func = (byte[] bytes) => constructorInfo.Invoke(new object[] { bytes }) as Packet;
                        Func<int, bool> lenFunc = GetLengthFunc(type);
                        PacketTypeInfo info = new PacketTypeInfo();
                        info.Type = type;
                        info.Constructor = func;
                        info.IsRightLength = GetLengthFunc(type);
                        C2STypes.Add(property.PacketID, info);
                    }
                }
            }
        }
        private static readonly Type[] argTypes = new Type[] { typeof(byte) };
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public static Func<int,bool> GetLengthFunc(Type type)
        {
            MethodInfo method = type.GetMethod("IsAllowedSize",flags,null,argTypes,null);
            if(method == null)
            {
                return Packet.IsAllowedSize;
            }
            return (int i) => (bool)(method.Invoke(null, new object[] { i }) as bool?);
        }
        [Obsolete]
        public static Type GetS2CPacket(int id)
        {
            return S2CTypes[id].Type;
        }
        [Obsolete]
        public static Type GetC2SPacket(int id)
        {
            return C2STypes[id].Type;
        }
        [Obsolete]
        public static Type GetPacket(int id, bool isS2C)
        {
            if (isS2C) return GetS2CPacket(id);
            else return GetC2SPacket(id);
        }
        public static Func<byte[],Packet> GetConstructor(int id,bool isS2C)
        {
            if (isS2C) return S2CTypes[id].Constructor;
            else return C2STypes[id].Constructor;
        }
    }
}