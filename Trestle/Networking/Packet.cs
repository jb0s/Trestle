using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using fNbt;
using Trestle.Attributes;
using Trestle.Enums;
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
            
            // Checks if the Packet is ClientBound
            var clientBound = (ClientBoundAttribute)GetType().GetCustomAttribute<ClientBoundAttribute>(false);
            if (clientBound == null)
                throw new Exception("Unable to Serialize packet, only ClientBound packets can be serialized.");
            
            // Writes the Packet Id first.
            buffer.WriteVarInt(clientBound.Id);

            // Iterates over all properties
            foreach (var property in GetType().GetProperties())
            {
                // Checks if the field is meant to be serialized
                var field = (FieldAttribute)property.GetCustomAttribute<FieldAttribute>(false);
                if (field == null)
                    continue;
                
                // Checks if the field has a VarInt override
                var isVarInt = property.GetCustomAttribute<VarIntAttribute>(false) != null;

                // Finds the proper type for the field and then writes it to the buffer
                switch (property.GetValue(this))
                {
                    case byte data:
                        buffer.WriteByte(data);
                        break;
                    case byte[] data:
                        buffer.Write(data);
                        break;
                    case sbyte data:
                        buffer.WriteByte(Convert.ToByte(data));
                        break;
                    case ushort data:
                        buffer.WriteUShort(data);
                        break;
                    case short data:
                        buffer.WriteShort(data);
                        break;
                    case int data:
                        if (isVarInt)
                            buffer.WriteVarInt(data);
                        else
                            buffer.WriteInt(data);
                        break;
                    case int[] data:
                        foreach (var num in data)
                            if (isVarInt)
                                buffer.WriteVarInt(num);
                            else
                                buffer.WriteInt(num);
                        break;
                    case long data:
                        buffer.WriteLong(data);
                        break;
                    case float data:
                        buffer.WriteFloat(data);
                        break;
                    case double data:
                        buffer.WriteDouble(data);
                        break;
                    case bool data:
                        buffer.WriteBool(data);
                        break;
                    case string data:
                        buffer.WriteString(data);
                        break;
                    case string[] data:
                        buffer.WriteVarInt(data.Length);
                    
                        foreach(var str in data)
                            buffer.WriteString(str);
                        break;
                    case Guid data:
                        buffer.WriteUuid(data);
                        break;
                    case NbtCompound data:
                        var stream = new MemoryStream();
                        new NbtFile(data).SaveToStream(stream, NbtCompression.None);
                        buffer.Write(stream.ToArray());
                        break;
                    default:
                        var message = $"Unable to parse field '{property.Name}' of type '{property.PropertyType}'";
                        Client.Player?.Kick(new MessageComponent($"{ChatColor.Red}An error occured while serializing.\n\n{ChatColor.Reset}{message}"));
                        throw new Exception(message);
                        break;
                }
            }
            
            return buffer;
        }

        public void DeserializePacket(MinecraftStream buffer)
        {
            // Iterates over all properties
            foreach (var property in GetType().GetProperties())
            {
                // Checks if the property should be deserialized
                var field = property.GetCustomAttribute<FieldAttribute>(false);
                if (field == null)
                    continue;

                // Checks if the field has a VarInt override
                var isVarInt = property.GetCustomAttribute<VarIntAttribute>(false) != null;
                
                // Dictionary of functions based on a type
                var @switch = new Dictionary<Type, Action> {
                    { typeof(byte), () => property.SetValue(this, (byte)buffer.ReadByte()) },
                    { typeof(byte[]), () =>
                    {
                        var length = buffer.ReadVarInt();
                        byte[] bytes = new byte[length];

                        for(int i = 0; i < length; i++)
                            bytes[i] = (byte)buffer.ReadByte();
                    
                        property.SetValue(this, bytes);
                    } },
                    { typeof(ushort), () => property.SetValue(this, buffer.ReadUShort()) },
                    { typeof(short), () => property.SetValue(this, buffer.ReadShort()) },
                    { typeof(int), () => property.SetValue(this, isVarInt ? buffer.ReadVarInt() : buffer.ReadInt()) },
                    { typeof(long), () => property.SetValue(this, buffer.ReadLong()) },
                    { typeof(float), () => property.SetValue(this, buffer.ReadFloat()) },
                    { typeof(double), () => property.SetValue(this, buffer.ReadDouble()) },
                    { typeof(bool), () => property.SetValue(this, buffer.ReadBool()) },
                    { typeof(string), () => property.SetValue(this, buffer.ReadString()) },
                    { typeof(Vector3), () =>
                    {
                        var val = buffer.ReadLong();
                        property.SetValue(this, new Vector3(
                            Convert.ToDouble(val >> 38), 
                            Convert.ToDouble((val >> 26) & 0xFFF), 
                            Convert.ToDouble(val << 38 >> 38)));
                    } },
                };
                
                // Checks if the type is in the dictionary, and then executes it
                if(@switch.ContainsKey(field.OverrideType != null ? field.OverrideType: property.PropertyType)) 
                    @switch[field.OverrideType != null ? field.OverrideType: property.PropertyType]();
                else
                {
                    var message = $"Unable to parse field '{property.Name}' of type '{property.PropertyType}'";
                    Client.Player?.Kick(new MessageComponent($"{ChatColor.Red}An error occured while deserializing.\n\n{ChatColor.Reset}{message}"));
                    throw new Exception(message);
                }
            }
        }
    }
}