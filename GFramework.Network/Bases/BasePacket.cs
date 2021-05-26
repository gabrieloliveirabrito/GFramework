using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Bases
{
    public abstract class BasePacket : IDisposable
    {
        public ulong ID { get; set; }
        public abstract byte[] Data { get; set; }

        public virtual long Length { get; protected set; }

        public BasePacket(ulong id)
        {
            ID = id;
        }

        public BasePacket(ulong id, byte[] data) : this(id)
        {
            Buffer.BlockCopy(data, 0, Data, 0, data.Length);
            Length = data.Length;
        }

        public abstract void Dispose();
        public abstract void Clear();
        public abstract void Reset();

        public abstract byte ReadByte();
        public abstract byte[] ReadBytes(int length);
        public abstract short ReadShort();
        public abstract int ReadInt();
        public abstract long ReadLong();
        public abstract ushort ReadUShort();
        public abstract uint ReadUInt();
        public abstract ulong ReadULong();
        public abstract decimal ReadDecimal();
        public abstract float ReadFloat();
        public abstract double ReadDouble();
        public abstract bool ReadBoolean();
        public abstract string ReadString();

        public abstract void WriteByte(byte data);
        public abstract void WriteBytes(byte[] data);
        public abstract void WriteShort(short data);
        public abstract void WriteInt(int data);
        public abstract void WriteLong(long data);
        public abstract void WriteUShort(ushort data);
        public abstract void WriteUInt(uint data);
        public abstract void WriteULong(ulong data);
        public abstract void WriteDecimal(decimal data);
        public abstract void WriteFloat(float data);
        public abstract void WriteDouble(double data);
        public abstract void WriteBoolean(bool data);
        public abstract void WriteString(string data);
    }
}
