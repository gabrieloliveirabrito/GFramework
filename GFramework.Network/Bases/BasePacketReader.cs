using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Bases
{
    public abstract class BasePacketReader : IDisposable
    {
        public uint ID { get; set; }
        public abstract byte[] Data { get; set; }

        public abstract int Offset { get; set; }
        public int Length { get; protected set; }

        public BasePacketReader(uint id)
        {
            ID = id;
            Data = new byte[Constants.MaxPacketLength];
            Length = 0;
            Offset = 0;
        }

        public BasePacketReader(uint id, byte[] data) : this(id)
        {
            Buffer.BlockCopy(Data, 0, data, 0, data.Length);
            Length = data.Length;
        }

        public abstract void Dispose();
        public abstract void Clear();

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
