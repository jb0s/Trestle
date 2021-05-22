using System;
using System.IO;
using System.Net;
using System.Text;

namespace Trestle.Nbt
{
    public static class Extensions
    {
        public static void Write(this BinaryWriter writer, NbtType type)
            => writer.Write((byte) type);

        public static void WriteShort(this BinaryWriter writer, short data)
            => writer.Write(IPAddress.HostToNetworkOrder(data));
        
        public static void WriteInt(this BinaryWriter writer, int data)
            => writer.Write(IPAddress.HostToNetworkOrder(data));
        
        public static void WriteLong(this BinaryWriter writer, long data)
            => writer.Write(IPAddress.HostToNetworkOrder(data));

        public static void WriteFloat(this BinaryWriter writer, float data)
        {
            var bytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            
            writer.Write(bytes);
        }
        
        public static void WriteDouble(this BinaryWriter writer, double data)
        {
            var bytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            
            writer.Write(bytes);
        }

        public static void WriteString(this BinaryWriter writer, string data)
        {
            var str = Encoding.UTF8.GetBytes(data);
            
            writer.WriteShort((short)str.Length);
            writer.Write(str);
        }
    }
}