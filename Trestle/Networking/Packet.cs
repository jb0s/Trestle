using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Trestle.Nbt.Tags;
using System.Threading.Tasks;
using Trestle.Configuration.Service;
using Trestle.Networking.Attributes;
using Trestle.Networking.Services;
using Trestle.Utils;

namespace Trestle.Networking
{
    public class Packet : IDisposable
    {
        public Client Client { get; internal set; }

        public IClientService ClientService { get; internal set; }
        
        public IMojangService MojangService { get; internal set; }
        
        public IConfigService ConfigService { get; internal set; }

        public virtual async Task Handle()
        {
        }
        
        internal void Initialize(Client client, IClientService clientService, IMojangService mojangService, IConfigService configService)
        {
            Client = client;
            ClientService = clientService;
            MojangService = mojangService;
            ConfigService = configService;
        }

        internal byte[] Serialize()
        {
            var buffer = new NettyStream();
            
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
                var field = property.GetCustomAttribute<FieldAttribute>(false);
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
                    case Uuid data:
                        buffer.WriteUuid(data);
                        break;
                    case Location data:
                        buffer.WriteDouble(data.X);
                        buffer.WriteDouble(data.Y);
                        buffer.WriteDouble(data.Z);
                        
                        buffer.WriteFloat(data.Yaw);
                        buffer.WriteFloat(data.Pitch);
                        break;
                    case NbtCompound data:
                        buffer.Write(data.ToArray());
                        break;
                    default:
                        var message = $"Unable to parse field '{property.Name}' of type '{property.PropertyType}'";
                        throw new Exception(message);
                }
            }

            var final = buffer.ToArray();
            
            buffer = new NettyStream();
            buffer.WriteVarInt(final.Length);
            buffer.Write(final);
            
            return buffer.ToArray();
        }

        internal void Deserialize(NettyStream stream)
        {
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
                    { typeof(byte), () => property.SetValue(this, (byte)stream.ReadByte()) },
                    { typeof(byte[]), () =>
                    {
                        var length = stream.ReadVarInt();
                        var bytes = new byte[length];

                        for(var i = 0; i < length; i++)
                            bytes[i] = (byte)stream.ReadByte();
                    
                        property.SetValue(this, bytes);
                    } },
                    { typeof(ushort), () => property.SetValue(this, stream.ReadUShort()) },
                    { typeof(short), () => property.SetValue(this, stream.ReadShort()) },
                    { typeof(int), () => property.SetValue(this, isVarInt ? stream.ReadVarInt() : stream.ReadInt()) },
                    { typeof(long), () => property.SetValue(this, stream.ReadLong()) },
                    { typeof(float), () => property.SetValue(this, stream.ReadFloat()) },
                    { typeof(double), () => property.SetValue(this, stream.ReadDouble()) },
                    { typeof(bool), () => property.SetValue(this, stream.ReadBool()) },
                    { typeof(string), () => property.SetValue(this, stream.ReadString()) },
                };
                
                if (@switch.ContainsKey(field.OverrideType != null ? field.OverrideType : property.PropertyType))
                    @switch[field.OverrideType != null ? field.OverrideType : property.PropertyType]();
            }
        }

        public void Dispose()
        {
            Client = null;
            ClientService = null;
            MojangService = null;
            ConfigService = null;
        }
    }
}