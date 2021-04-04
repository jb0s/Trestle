using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;
using Ionic.Zlib;
using Trestle.Enums;
using Trestle.Networking;

namespace Trestle.Utils
{
public class MinecraftStream : IDisposable
    {
        private readonly Client _client;
        public byte[] BufferedData = new byte[4096];
        private int _lastByte;
        public int Size = 0;
        
        public MinecraftStream(Client client)
        {
            _client = client;
        }
        
        public MinecraftStream(byte[] data)
        {
            BufferedData = data;
        }

        public MinecraftStream()
        {
        }
        
        public void SetDataSize(int size)
        {
            Array.Resize(ref BufferedData, size);
            Size = size;
        }

        public void Dispose()
        {
            BufferedData = null;
            _lastByte = 0;
        }
        
        public int ReadByte()
		{
			var returnData = BufferedData[_lastByte];
			_lastByte++;
			return returnData;
		}

		public byte[] Read(int length)
		{
			var buffered = new byte[length];
			Array.Copy(BufferedData, _lastByte, buffered, 0, length);
			_lastByte += length;
			return buffered;
		}


		public int ReadInt()
		{
			var dat = Read(4);
			var value = BitConverter.ToInt32(dat, 0);
			return IPAddress.NetworkToHostOrder(value);
		}

		public float ReadFloat()
		{
			var almost = Read(4);
			var f = BitConverter.ToSingle(almost, 0);
			return NetworkToHostOrder(f);
		}

		public bool ReadBool()
		{
			var answer = ReadByte();
			return answer == 1;
		}

		public double ReadDouble()
		{
			var almostValue = Read(8);
			return NetworkToHostOrder(almostValue);
		}

		public int ReadVarInt()
		{
			var value = 0;
			var size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++*7);
				if (size > 5)
				{
					throw new IOException("VarInt too long");
				}
			}
			return value | ((b & 0x7F) << (size*7));
		}

		public long ReadVarLong()
		{
			var value = 0;
			var size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++*7);
				if (size > 10)
				{
					throw new IOException("VarLong too long");
				}
			}
			return value | ((b & 0x7F) << (size*7));
		}

		public short ReadShort()
		{
			var da = Read(2);
			var d = BitConverter.ToInt16(da, 0);
			return IPAddress.NetworkToHostOrder(d);
		}

		public ushort ReadUShort()
		{
			var da = Read(2);
			return NetworkToHostOrder(BitConverter.ToUInt16(da, 0));
		}

		public ushort[] ReadUShort(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return NetworkToHostOrder(us);
		}

		public ushort[] ReadUShortLocal(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public short[] ReadShortLocal(int count)
		{
			var us = new short[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public string ReadString()
		{
			var length = ReadVarInt();
			var stringValue = Read(length);

			return Encoding.UTF8.GetString(stringValue);
		}

		public long ReadLong()
		{
			var l = Read(8);
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(l, 0));
		}

		public Vector3 ReadPosition()
		{
			var val = ReadLong();
			var x = Convert.ToDouble(val >> 38);
			var y = Convert.ToDouble((val >> 26) & 0xFFF);
			var z = Convert.ToDouble(val << 38 >> 38);
			return new Vector3(x, y, z);
		}

		private double NetworkToHostOrder(byte[] data)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
			return BitConverter.ToDouble(data, 0);
		}

		private float NetworkToHostOrder(float network)
		{
			var bytes = BitConverter.GetBytes(network);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return BitConverter.ToSingle(bytes, 0);
		}

		private ushort[] NetworkToHostOrder(ushort[] network)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(network);
			return network;
		}

		private ushort NetworkToHostOrder(ushort network)
		{
			var net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt16(net, 0);
		}
		
		public byte[] Data
		{
			get { return _bffr.ToArray(); }
		}

		private readonly List<byte> _bffr = new List<byte>();

		public void Write(byte[] data, int offset, int length)
		{
			for (var i = 0; i < length; i++)
			{
				_bffr.Add(data[i + offset]);
			}
		}

		public void Write(byte[] data)
		{
			foreach (var i in data)
			{
				_bffr.Add(i);
			}
		}

		public void WritePosition(Vector3 position)
		{
			var x = Convert.ToInt64(position.X);
			var y = Convert.ToInt64(position.Y);
			var z = Convert.ToInt64(position.Z);
			var toSend = ((x & 0x3FFFFFF) << 38) | ((y & 0xFFF) << 26) | (z & 0x3FFFFFF);
			WriteLong(toSend);
		}

		public void WriteVarInt(int integer)
		{
			while ((integer & -128) != 0)
			{
				_bffr.Add((byte) (integer & 127 | 128));
				integer = (int) (((uint) integer) >> 7);
			}
			
			_bffr.Add((byte) integer);
		}

		public void WriteVarLong(long i)
		{
			var fuck = i;
			while ((fuck & ~0x7F) != 0)
			{
				_bffr.Add((byte) ((fuck & 0x7F) | 0x80));
				fuck >>= 7;
			}
			_bffr.Add((byte) fuck);
		}

		public void WriteInt(int data)
		{
			var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(buffer);
		}

		public void WriteString(string data)
		{
			var stringData = Encoding.UTF8.GetBytes(data);
			WriteVarInt(stringData.Length);
			Write(stringData);
		}

		public void WriteShort(short data)
		{
			var shortData = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(shortData);
		}

		public void WriteUShort(ushort data)
		{
			var uShortData = BitConverter.GetBytes(data);
			Write(uShortData);
		}

		public void WriteByte(byte data)
		{
			_bffr.Add(data);
		}

		public void WriteBool(bool data)
		{
			Write(BitConverter.GetBytes(data));
		}

		public void WriteDouble(double data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteFloat(float data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteLong(long data)
		{
			Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
		}

		public void WriteUuid(Guid guid)
		{
			var data = guid.ToString().Replace("-", "");
			var bigInteger = BigInteger.Parse(data, NumberStyles.HexNumber);
			Write(bigInteger.ToByteArray(false, true));
		}

		private byte[] GetVarIntBytes(int integer)
		{
			List<Byte> bytes = new List<byte>();
			while ((integer & -128) != 0)
			{
				bytes.Add((byte)(integer & 127 | 128));
				integer = (int)(((uint)integer) >> 7);
			}
			bytes.Add((byte)integer);
			return bytes.ToArray();
		}

		/// <summary>
		/// Flush all data to the TCPClient NetworkStream.
		/// </summary>
		public void FlushData(bool quee = false)
		{
			try
			{
				var allData = _bffr.ToArray();
				_bffr.Clear();
				
				if (Config.UseCompression && _client.State == ClientState.Play && _client.SetCompressionSend)
				{
					bool isOver = (allData.Length >= Config.CompressionThreshold);
					int dataLength = isOver ? allData.Length : 0;

					//Calculate length of 'Data Length'
					byte[] dLength = GetVarIntBytes(dataLength);

					//Create all data
					var compressedBytes = ZlibStream.CompressBuffer(allData);
					int packetlength = compressedBytes.Length + dLength.Length;
					var dataToSend = isOver ? compressedBytes : allData;

					var compressed = new MinecraftStream(_client);
					compressed.WriteVarInt(packetlength);
					compressed.WriteVarInt(dataLength);
					compressed.Write(dataToSend);

					Console.WriteLine();

					Console.WriteLine("Packet bigger than Threshold: " + isOver);
					Console.WriteLine("Packet info: ");

					Console.WriteLine("(Header) Packet Length: " + packetlength);
					Console.WriteLine("(Header) Data Length: " + dataLength);
					Console.WriteLine("Data Length " + dataToSend.Length);
					Console.WriteLine("Length difference: " + (packetlength - dataToSend.Length));

					Console.WriteLine();

					_client.AddToQueue(compressed.Data, quee);
				}
				else
				{
					WriteVarInt(allData.Length);
					var buffer = _bffr.ToArray();

					var data = new List<byte>();
					foreach (var i in buffer)
					{
						data.Add(i);
					}
					foreach (var i in allData)
					{
						data.Add(i);
					}
					_client.AddToQueue(data.ToArray(), quee);
				}
				_bffr.Clear();
			}
			catch (Exception ex)
			{
				//Globals.ClientManager.PacketError(_client, ex);
			}
		}

		private byte[] HostToNetworkOrder(double d)
		{
			var data = BitConverter.GetBytes(d);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(data);

			return data;
		}

		private byte[] HostToNetworkOrder(float host)
		{
			var bytes = BitConverter.GetBytes(host);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return bytes;
		}
    }
}