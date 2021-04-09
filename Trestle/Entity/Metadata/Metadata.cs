using System;
using System.Reflection;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Networking;
using Trestle.Utils;

namespace Trestle.Entity
{
    public class Metadata
    {
        [Field(typeof(byte))]
        [Index(0)]
        public EntityStatus Status { get; set; } = 0x00;

        [Field]
        [Index(1)]
        public int Air { get; set; } = 300;

        [Field]
        [Index(2)]
        public string CustonName { get; set; } = "";

        [Field]
        [Index(3)]
        public bool IsCustomNameVisible { get; set; } = false;

        [Field]
        [Index(4)]
        public bool IsSilent { get; set; } = false;

        [Field]
        [Index(5)]
        public bool HasNoGravity { get; set; } = false;

        public readonly Entity Entity;

        public Metadata(Entity entity)
        {
            Entity = entity;
        }
        
        public byte[] ToArray()
        {
            var buffer = new MinecraftStream();

            foreach (var property in GetType().GetProperties())
            {
                // Checks if the field is meant to be serialized
                var field = (FieldAttribute)property.GetCustomAttribute<FieldAttribute>(false);
                if (field == null)
                    continue;
                
                var index = property.GetCustomAttribute<IndexAttribute>(false);
                if (index == null)
                    continue;

                buffer.WriteByte((byte)index.Index);

                var value = property.GetValue(this);
                if (field.OverrideType != null)
                    value = Convert.ChangeType(value, field.OverrideType);
                
                switch (value)
                {
                    case byte data:
                        buffer.WriteVarInt(0);
                        buffer.WriteByte(data);
                        break;
                    case int data:
                        buffer.WriteVarInt(1);
                        buffer.WriteVarInt(data);
                        break;
                    case float data:
                        buffer.WriteVarInt(2);
                        buffer.WriteFloat(data);
                        break;
                    case string data:
                        buffer.WriteVarInt(3);
                        buffer.WriteString(data);
                        break;
                    case ItemStack data:
                        buffer.WriteVarInt(5);
                        
                        buffer.WriteShort(data.ItemId);
                        buffer.WriteByte(data.ItemCount);
                        buffer.WriteShort(data.ItemDamage);
                        buffer.WriteByte(data.NBT);
                        break;
                    case bool data:
                        buffer.WriteVarInt(6);
                        buffer.WriteBool(data);
                        break;
                    default:
                        var message = $"Unable to serialize field '{property.Name}' of type '{property.PropertyType}'";
                        //Client.Player?.Kick(new MessageComponent($"{ChatColor.Red}An error occured while serializing.\n\n{ChatColor.Reset}{message}"));
                        throw new Exception(message);
                        break;
                }
            }
            buffer.WriteByte(0xff);

            return buffer.Data;
        }
    }
}