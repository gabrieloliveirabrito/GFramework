using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.Network.Packets
{
    using Bases;
    using Factories;

    public class BitPacketReader : BasePacket
    {
        public override byte[] Data { get; set; }
        public int Offset { get; set; }

        public BitPacketReader(ulong id) : base(id)
        {
            Data = new byte[Constants.MaxPacketLength];
        }

        public BitPacketReader(ulong id, byte[] buffer) : base(id, buffer)
        {

        }

        private bool disposed = false;
        public override void Dispose()
        {
            if(!disposed)
            {
                Clear();
                Data = null;
                Data = new byte[0];
            }
            disposed = true;
        }

        public override void Clear()
        {
            //Array.Clear(Data, 0, Length);
            Offset = 0;
            Length = 0;
        }

        public override void Reset()
        {
            Offset = 0;
        }

        public override bool ReadBoolean()
        {
            byte[] buffer = ReadBytes(sizeof(bool));
            return BitConverter.ToBoolean(buffer, 0);
        }

        public override byte ReadByte()
        {
            return Data[Offset += 1];
        }

        public override byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(Data, Offset, data, 0, length);

            //LoggerFactory.GetLogger<BitPacketReader>().LogInfo("Reading O:{0} B:{1}", Offset, BitConverter.ToString(data));

            Offset += length;
            return data;
        }

        public override decimal ReadDecimal()
        {
            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
                bits[i] = ReadInt();

            return new decimal(bits);
        }

        public override double ReadDouble()
        {
            byte[] buffer = ReadBytes(sizeof(double));
            return BitConverter.ToDouble(buffer, 0);
        }

        public override float ReadFloat()
        {
            byte[] buffer = ReadBytes(sizeof(float));
            return BitConverter.ToSingle(buffer, 0);
        }

        public override int ReadInt()
        {
            byte[] buffer = ReadBytes(sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public override long ReadLong()
        {
            byte[] buffer = ReadBytes(sizeof(long));
            return BitConverter.ToInt64(buffer, 0);
        }

        public override short ReadShort()
        {
            byte[] buffer = ReadBytes(sizeof(short));
            return BitConverter.ToInt16(buffer, 0);
        }

        public override string ReadString()
        {
            int length = ReadInt();
            byte[] buffer = ReadBytes(length);

            return Encoding.UTF8.GetString(buffer);
        }

        public override uint ReadUInt()
        {
            byte[] buffer = ReadBytes(sizeof(uint));
            return BitConverter.ToUInt32(buffer, 0);
        }

        public override ulong ReadULong()
        {
            byte[] buffer = ReadBytes(sizeof(ulong));
            return BitConverter.ToUInt64(buffer, 0);
        }

        public override ushort ReadUShort()
        {
            byte[] buffer = ReadBytes(sizeof(ushort));
            return BitConverter.ToUInt16(buffer, 0);
        }

        public override void WriteBoolean(bool data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteByte(byte data)
        {
            Data[Length] = data;
            Length += 1;
            Offset += 1;
        }

        public override void WriteBytes(byte[] data)
        {
            //LoggerFactory.GetLogger<BitPacketReader>().LogInfo("Writing O:{0} B:{1}", Offset, BitConverter.ToString(data));
            Buffer.BlockCopy(data, 0, Data, Offset, data.Length);
            Length += data.Length;
            Offset += data.Length;
        }

        public override void WriteDecimal(decimal data)
        {
            int[] bits = decimal.GetBits(data);
            for (int i = 0; i < 4; i++)
                WriteInt(bits[i]);
        }

        public override void WriteDouble(double data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteFloat(float data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteInt(int data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteLong(long data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteShort(short data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteString(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            WriteInt(buffer.Length);
            WriteBytes(buffer);
        }

        public override void WriteUInt(uint data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteULong(ulong data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }

        public override void WriteUShort(ushort data)
        {
            WriteBytes(BitConverter.GetBytes(data));
        }
    }
}
