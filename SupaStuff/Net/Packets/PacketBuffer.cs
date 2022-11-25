using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SupaStuff.Net.Packets
{
    public class PacketBuffer
    {
        public bool IsReadonly;
        MemoryStream stream;
        public byte[] ReadBuffer
        {
            get; private set;
        }
        
        public int Index
        {
            get; private set;
        }

        internal PacketBuffer(byte[] buffer)
        {
            IsReadonly = true;
            this.ReadBuffer = buffer;
            Index = 0;
        }
        public PacketBuffer()
        {
            IsReadonly = false;
            stream = new MemoryStream();
        }
        public byte[] ToArray()
        {
            return stream.ToArray();
        }

        public byte ReadByte()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            return ReadBuffer[Index++];
        }
        public void WriteByte(byte val)
        {
            if (IsReadonly) throw new Exception("You are trying to write to a readonly only buffer!");
            stream.WriteByte(val);
        }
        public void ReadBytes(byte[] array,int offset, int count)
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            for (int i = 0; i < count; i++)
            {
                array[i] = ReadBuffer[i + Index + offset];
            }
            Index += count;
        }
        public void WriteBytes(byte[] array, int offset, int count)
        {
            stream.Write(array, offset, count);
        }
        public string ReadString()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            int length = ReadInt();
            string val = Encoding.UTF8.GetString(ReadBuffer, Index, length);
            Index += length;
            return val;
        }
        public void WriteString(string val)
        {
            if (IsReadonly) throw new Exception("You are trying to write to a readonly only buffer!");
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public short ReadShort()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            short val = BitConverter.ToInt16(ReadBuffer, Index);
            Index += 2;
            return val;
        }
        public void WriteShort(int val)
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public int ReadInt()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            int val = BitConverter.ToInt32(ReadBuffer, Index);
            Index += 4;
            return val;
        }
        public void WriteInt(int val)
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public long ReadLong()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            long val = BitConverter.ToInt64(ReadBuffer, Index);
            Index += 8;
            return val;
        }
        public void WriteLong(long val)
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public float ReadFloat()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            float val = BitConverter.ToSingle(ReadBuffer, Index);
            Index += 4;
            return val;
        }
        public void WriteFloat(float val)
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public double ReadDouble()
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            double val = BitConverter.ToDouble(ReadBuffer, Index);
            Index += 8;
            return val;
        }
        public void WriteDouble(float val)
        {
            if (!IsReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
