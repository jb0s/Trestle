using System;
using System.IO;
using System.Linq;
using System.Reflection;
using fNbt;
using Trestle.Attributes;
using Trestle.Utils;

namespace Trestle.Networking
{
    public class Packet
    {
        public Client Client { get; set; }
        
        public Packet()
        {
        }

        public Packet(Client client)
        {
            Client = client;
        }
        
        public virtual void HandlePacket()
        {
            Console.WriteLine("not special..");
        }

        public MinecraftStream SerializePacket()
        {
            var buffer = new MinecraftStream(Client);

            var attribute = (ClientBoundAttribute)GetType().GetCustomAttribute<ClientBoundAttribute>(false);
            if (attribute == null)
                throw new Exception("Packet is not ClientBound.");

            buffer.WriteVarInt(attribute.Id);
            foreach (var property in GetType().GetProperties())
            {
                if (property.GetCustomAttribute<FieldAttribute>(false) == null)
                    continue;
                
                if (property.GetCustomAttribute<VarIntAttribute>(false) != null)
                    buffer.WriteVarInt((int)property.GetValue(this));
                else if (property.PropertyType == typeof(Int16))
                    buffer.WriteShort((short)property.GetValue(this));
                else if (property.PropertyType == typeof(UInt16))
                    buffer.WriteUShort((ushort)property.GetValue(this));
                else if (property.PropertyType == typeof(byte))
                    buffer.WriteByte((byte) property.GetValue(this));
                else if (property.PropertyType == typeof(sbyte))
                    buffer.WriteByte(Convert.ToByte((sbyte)property.GetValue(this)));
                else if (property.PropertyType == typeof(Int32))
                    buffer.WriteInt((int)property.GetValue(this));
                else if (property.PropertyType == typeof(Int64))
                    buffer.WriteLong((long)property.GetValue(this));
                else if (property.PropertyType == typeof(float))
                    buffer.WriteFloat((float)property.GetValue(this));
                else if (property.PropertyType == typeof(double))
                    buffer.WriteDouble((double)property.GetValue(this));
                else if (property.PropertyType == typeof(String))
                    buffer.WriteString((string)property.GetValue(this));
                else if (property.PropertyType == typeof(Guid))
                    buffer.WriteUuid((Guid)property.GetValue(this));
                else if (property.PropertyType == typeof(string[]))
                {
                    var array = (string[]) property.GetValue(this);
                    buffer.WriteVarInt(array.Length);
                    
                    foreach(var thing in array)
                        buffer.WriteString(thing);
                }
                else if (property.PropertyType == typeof(byte[]))
                    buffer.Write((byte[])property.GetValue(this));
                else if (property.PropertyType == typeof(bool))
                    buffer.WriteBool((bool)property.GetValue(this));
                else if (property.PropertyType == typeof(NbtCompound))
                {
                    var stream = new MemoryStream();
                    new NbtFile((NbtCompound) property.GetValue(this)).SaveToStream(stream, NbtCompression.None);
                    buffer.Write(stream.ToArray());
                }
                else
                {
                    Client.Player?.Kick(new MessageComponent($"Unable to parse field '{property.Name}'."));
                    throw new Exception($"Unable to parse field '{property.Name}'.");
                }
            }
            return buffer;
        }

        public void DeserializePacket(MinecraftStream buffer)
        {
            foreach (var property in GetType().GetProperties())
            {
                if (property.GetCustomAttribute<FieldAttribute>(false) == null)
                    continue;
                
                if (property.GetCustomAttribute<VarIntAttribute>(false) != null)
                    property.SetValue(this, buffer.ReadVarInt());
                else if (property.PropertyType == typeof(Int16))
                    property.SetValue(this, buffer.ReadShort());
                else if (property.PropertyType == typeof(UInt16))
                    property.SetValue(this, buffer.ReadUShort());
                else if (property.PropertyType == typeof(Int32))
                    property.SetValue(this, buffer.ReadInt());
                else if (property.PropertyType == typeof(Int64))
                    property.SetValue(this, buffer.ReadLong());
                else if (property.PropertyType == typeof(String))
                    property.SetValue(this, buffer.ReadString());
                else if(property.PropertyType == typeof(Double))
                    property.SetValue(this, buffer.ReadDouble());
                else if(property.PropertyType == typeof(Boolean))
                    property.SetValue(this, buffer.ReadBool());
                else if(property.PropertyType == typeof(Single))
                    property.SetValue(this, buffer.ReadFloat());
                else if(property.PropertyType == typeof(Byte))
                    property.SetValue(this, (byte)buffer.ReadByte());
                else if(property.PropertyType == typeof(Vector3))
                {
                    long val = buffer.ReadLong();
                    property.SetValue(this, new Vector3(Convert.ToDouble(val >> 38), Convert.ToDouble((val >> 26) & 0xFFF), Convert.ToDouble(val << 38 >> 38)));
                }
                else if (property.PropertyType == typeof(Byte[]))
                {
                    var length = buffer.ReadVarInt();
                    byte[] bytes = new byte[length];

                    for(int i = 0; i < length; i++)
                    {
                        bytes[i] = (byte)buffer.ReadByte();
                    }
                    
                    property.SetValue(this, bytes);
                }
                else
                {
                    Client.Player?.Kick(new MessageComponent("Unable to parse field of type " + property.PropertyType));
                    throw new Exception("Unable to parse field of type " + property.PropertyType);
                }
            }
        }
    }
}