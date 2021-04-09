using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using fNbt;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Utils;

namespace Trestle.Networking
{
    public class Packet
    {
        public Client Client { get; set; }
        public Player Player => Client.Player;
        public World.World World => Player.World;
        public InventoryManager Inventory => Player.Inventory;
        
        public Packet()
        {
        }

        public Packet(Client client)
        {
            Client = client;
        }
        
        /// <summary>
        /// Virtual function for handling packets.
        /// </summary>
        public virtual void HandlePacket()
        {
            Console.WriteLine("not special..");
        }

        /// <summary>
        /// Serializes a packet.
        /// </summary>
        public MinecraftStream SerializePacket()
        {
            var buffer = new MinecraftStream(Client);
            
            // Checks if the Packet is ClientBound
            var clientBound = GetType().GetCustomAttribute<ClientBoundAttribute>(false);
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

                var value = property.GetValue(this);
                if (field.OverrideType != null)
                    value = Convert.ChangeType(value, field.OverrideType);
                
                // Finds the proper type for the field and then writes it to the buffer
                switch (value)
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
                    case Vector3 data:
                        var x = Convert.ToInt64(data.X);
                        var y = Convert.ToInt64(data.Y);
                        var z = Convert.ToInt64(data.Z);

                        var pos = ((x & 0x3FFFFFF) << 38) | ((y & 0xFFF) << 26) | (z & 0x3FFFFFF);
                        buffer.WriteLong(pos);
                        break;
                    case Velocity data:
                        buffer.WriteShort(data.X);
                        buffer.WriteShort(data.Y);
                        buffer.WriteShort(data.Z);
                        break;
                    case ItemStack data:
                        buffer.WriteShort(data.ItemId);

                        if (data.ItemId != -1)
                        {
                            buffer.WriteByte(data.ItemCount);
                            buffer.WriteShort(data.ItemDamage);
                            buffer.WriteByte(0);   
                        }
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

        /// <summary>
        /// Deserializes a packet.
        /// </summary>
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

                // Checks if the field is optional
                var isOptional = property.GetCustomAttribute<OptionalAttribute>(false) != null;

                
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
                    { typeof(ItemStack), () =>
                    {
                        var itemId = buffer.ReadShort();
                        if (itemId != -1)
                        {
                            var itemCount = buffer.ReadByte();
                            var itemDamage = buffer.ReadShort();
                            property.SetValue(this, new ItemStack(itemId, (byte)itemCount, itemDamage, 0));
                        }
                        else 
                            property.SetValue(this, new ItemStack(itemId, 0, 0, 0));
                    } },
                };

                try
                {
                    // Checks if the type is in the dictionary, and then executes it
                    if (@switch.ContainsKey(field.OverrideType != null ? field.OverrideType : property.PropertyType))
                        @switch[field.OverrideType != null ? field.OverrideType : property.PropertyType]();
                    else
                    {
                        var message = $"Unable to parse field '{property.Name}' of type '{property.PropertyType}'";
                        Client.Player?.Kick(new MessageComponent(
                            $"{ChatColor.Red}An error occured while deserializing.\n\n{ChatColor.Reset}{message}"));
                        throw new Exception(message);
                    }
                }
                catch (Exception e)
                {
                    if (isOptional)
                        return;
                    else
                        throw;
                }
            }
        }
    }
}