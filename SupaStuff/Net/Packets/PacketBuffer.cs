using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SupaStuff.Net.Packets
{
    public class PacketBuffer
    {
        public bool isReadonly;
        MemoryStream stream;
        public byte[] readBuffer
        {
            get; private set;
        }
        
        public int index
        {
            get; private set;
        }

        internal PacketBuffer(byte[] buffer)
        {
            isReadonly = true;
            this.readBuffer = buffer;
            index = 0;
        }
        public PacketBuffer()
        {
            isReadonly = false;
            stream = new MemoryStream();
        }
        public byte[] ToArray()
        {
            return stream.ToArray();
        }

        public byte ReadByte()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            return readBuffer[index++];
        }
        public void WriteByte(byte val)
        {
            if (isReadonly) throw new Exception("You are trying to write to a readonly only buffer!");
            stream.WriteByte(val);
        }
        public void ReadBytes(byte[] array,int offset, int count)
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            for (int i = 0; i < count; i++)
            {
                array[i] = readBuffer[i + index + offset];
            }
            index += count;
        }
        public void WriteBytes(byte[] array, int offset, int count)
        {
            stream.Write(array, offset, count);
        }
        public string ReadString()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            int length = ReadInt();
            string val = Encoding.UTF8.GetString(readBuffer, index, length);
            index += length;
            return val;
        }
        public void WriteString(string val)
        {
            if (isReadonly) throw new Exception("You are trying to write to a readonly only buffer!");
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public short ReadShort()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            short val = BitConverter.ToInt16(readBuffer, index);
            index += 2;
            return val;
        }
        public void WriteShort(int val)
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public int ReadInt()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            int val = BitConverter.ToInt32(readBuffer, index);
            index += 4;
            return val;
        }
        public void WriteInt(int val)
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public long ReadLong()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            long val = BitConverter.ToInt64(readBuffer, index);
            index += 8;
            return val;
        }
        public void WriteLong(long val)
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public float ReadFloat()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            float val = BitConverter.ToSingle(readBuffer, index);
            index += 4;
            return val;
        }
        public void WriteFloat(float val)
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
        public double ReadDouble()
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            double val = BitConverter.ToDouble(readBuffer, index);
            index += 8;
            return val;
        }
        public void WriteDouble(float val)
        {
            if (!isReadonly) throw new Exception("You are trying to read a write only buffer!");
            byte[] bytes = BitConverter.GetBytes(val);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
