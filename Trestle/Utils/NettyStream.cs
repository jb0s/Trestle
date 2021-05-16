using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Trestle.Utils
{
    public class NettyStream : Stream
    {
        public override bool CanRead { get; }
        public override bool CanWrite { get; }
        public override bool CanSeek => false;

        public override long Position
        {
            get => _buffer.Position;
            set => _buffer.Position = value;
        }
        public override long Length => _buffer.Length;

        private readonly Stream _buffer;

        public NettyStream()
        {
            CanRead = true;
            CanWrite = true;
            
            _buffer = new MemoryStream();
        }
        
        public NettyStream(NetworkStream stream)
        {
            CanRead = true;
            CanWrite = false;

            _buffer = stream;
            
            var data = new byte[ReadVarInt()];
            Read(data, 0, data.Length);

            _buffer = new MemoryStream(data);
        }
        
        #region Reading

        public override int Read(byte[] buffer, int offset, int count)
            => _buffer.Read(buffer, offset, count);

        public byte[] Read(int length)
        {
            var buffer = new byte[length];
            Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public bool ReadBool()
            => ReadByte() == 1;
        
        public short ReadShort()
            => IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Read(2), 0));

        public ushort ReadUShort()
            => (ushort) ReadShort();
        
        public int ReadInt()
            => IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Read(4), 0));
        
        public int ReadVarInt()
        {
            int value = 0, size = 0, b;
            while (((b = ReadByte()) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++*7);
                if (size > 5)
                    throw new IOException("VarInt is too long.");
            }
            
            return value | ((b & 0x7F) << (size*7));
        }

        public long ReadLong()
            => IPAddress.NetworkToHostOrder(BitConverter.ToInt64(Read(8), 0));

        public float ReadFloat()
            => NetworkToHostOrder(BitConverter.ToSingle(Read(4), 0));
        
        public double ReadDouble()
            => NetworkToHostOrder(BitConverter.ToDouble(Read(8), 0));

        public string ReadString()
            => Encoding.UTF8.GetString(Read(ReadVarInt()));

        public Uuid ReadUuid()
            => new(Read(16));
        
        #endregion

        #region Writing

        public override void Write(byte[] buffer, int offset, int count)
            => _buffer.Write(buffer, offset, count);
        
        public void Write(byte[] buffer)
            => _buffer.Write(buffer, 0, buffer.Length);

        public void WriteBool(bool data)
            => Write(BitConverter.GetBytes(data));
        
        public void WriteShort(short data)
            => Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));

        public void WriteUShort(ushort data)
            => Write(BitConverter.GetBytes(data));
        
        public void WriteInt(int data)
            => Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
        
        public void WriteVarInt(int integer)
        {
            while ((integer & -128) != 0)
            {
                WriteByte((byte)(integer & 127 | 128));
                integer = (int) (((uint) integer) >> 7);
            }
            
            WriteByte((byte)integer);
        }
        
        public void WriteLong(long data)
            => Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
        
        public void WriteFloat(float data)
            => Write(BitConverter.GetBytes(NetworkToHostOrder(data)));
        
        public void WriteDouble(double data)
            => Write(BitConverter.GetBytes(NetworkToHostOrder(data)));

        public void WriteString(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            WriteVarInt(bytes.Length);
            Write(bytes);
        }

        public void WriteUuid(Uuid data)
            => Write(data.ToByteArray());
        
        #endregion
        
        #region Other

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotImplementedException();

        public override void SetLength(long value)
            => _buffer.SetLength(value);
        
        public override void Flush()
            => throw new NotImplementedException();

        public byte[] ToArray()
            => ((MemoryStream) _buffer).ToArray();
        
        private float NetworkToHostOrder(float data)
        {
            var bytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToSingle(bytes, 0);
        }
        
        private double NetworkToHostOrder(double data)
        {
            var bytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }

        #endregion
    }
}