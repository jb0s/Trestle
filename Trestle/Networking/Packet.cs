using System;
using System.Linq;
using System.Reflection;
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

        public void DeserializePacket(DataBuffer buffer)
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
                else
                {
                    throw new Exception("Unable to parse field.");
                }
            }
        }
    }
}