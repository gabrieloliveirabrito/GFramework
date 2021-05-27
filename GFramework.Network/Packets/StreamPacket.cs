using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace GFramework.Network.Packets
{
    using Bases;
    using Factories;

    public class StreamPacket : BasePacket
    {
        private MemoryStream memory;
        private BinaryReader reader;
        private BinaryWriter writer;

        public override byte[] Data
        {
            get => memory.ToArray();
            set
            {
                Clear();

                memory.Write(value, 0, value.Length);
                memory.Position = 0;
            }
        }

        public override long Length
        {
            get => memory.Length;
            protected set => memory.SetLength(value);
        }

        public StreamPacket(ulong id) : base(id)
        {
            Clear();
        }

        public StreamPacket(ulong id, byte[] buffer) : base(id)
        {
            memory = new MemoryStream(buffer);
            memory.Position = 0;

            reader = new BinaryReader(memory);
            writer = new BinaryWriter(memory);
        }

        private bool disposed = false;
        public override void Dispose()
        {
            if (!disposed)
            {
                Clear();
                Data = null;
                Data = new byte[0];
            }
            disposed = true;
        }

        public override void Clear()
        {
            memory = new MemoryStream();
            reader = new BinaryReader(memory);
            writer = new BinaryWriter(memory);
        }

        public override void Reset()
        {
            memory.Position = 0;
        }

        public override bool ReadBoolean() => reader.ReadBoolean();
        public override byte ReadByte() => reader.ReadByte();
        public override byte[] ReadBytes(int length) => reader.ReadBytes(length);

        public override decimal ReadDecimal()
        {
            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
                bits[i] = ReadInt();

            return new decimal(bits);
        }

        public override double ReadDouble() => reader.ReadDouble();
        public override float ReadFloat() => reader.ReadSingle();
        public override int ReadInt() => reader.ReadInt32();
        public override long ReadLong() => reader.ReadInt64();
        public override short ReadShort() => reader.ReadInt16();
        public override string ReadString() => reader.ReadString();
        public override uint ReadUInt() => reader.ReadUInt32();
        public override ulong ReadULong() => reader.ReadUInt64();
        public override ushort ReadUShort() => reader.ReadUInt16();
        public override DateTime ReadDateTime() => new DateTime(reader.ReadInt64());

        public override void WriteBoolean(bool data) => writer.Write(data);
        public override void WriteByte(byte data) => writer.Write(data);
        public override void WriteBytes(byte[] data) => writer.Write(data);

        public override void WriteDecimal(decimal data)
        {
            int[] bits = decimal.GetBits(data);
            for (int i = 0; i < 4; i++)
                WriteInt(bits[i]);
        }

        public override void WriteDouble(double data) => writer.Write(data);
        public override void WriteFloat(float data) => writer.Write(data);
        public override void WriteInt(int data) => writer.Write(data);
        public override void WriteLong(long data) => writer.Write(data);
        public override void WriteShort(short data) => writer.Write(data);
        public override void WriteString(string data) => writer.Write(data);
        public override void WriteUInt(uint data) => writer.Write(data);
        public override void WriteULong(ulong data) => writer.Write(data);
        public override void WriteUShort(ushort data) => writer.Write(data);
        public override void WriteDateTime(DateTime data) => writer.Write(data.Ticks);
    }
}
