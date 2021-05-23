using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Trestle.Nbt
{
    public class NbtTag
    {
        public string Name { get; set; }
        
        public NbtType Type { get; set; }
        
        public object Value { get; set; }

        public bool IsInList { get; set; } = false;

        public NbtTag(string name, NbtType type)
        {
            Name = name;
            Type = type;
        }
        
        public NbtTag(string name, NbtType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        
        public byte[] ToArray()
        {
            var stream = new MemoryStream();
            var buffer = new BinaryWriter(stream);

            if (!IsInList)
            {
                buffer.Write(Type);
                buffer.WriteString(Name);
            }

            switch (Value)
            {
                case byte data:
                    buffer.Write(data);
                    break;
                case short data:
                    buffer.WriteShort(data);
                    break;
                case int data:
                    buffer.WriteInt(data);
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
                case string data:
                    buffer.WriteString(data);
                    break;
                case List<long> data:
                    buffer.WriteInt(data.Count);
                    foreach (var num in data)
                        buffer.WriteLong(num);
                    break;
                case List<NbtTag> data:
                    if (Type == NbtType.List)
                    {
                        buffer.Write(data[0].Type);
                        buffer.WriteInt(data.Count);
                        foreach (var tag in data)
                        {
                            tag.IsInList = true;
                            buffer.Write(tag.ToArray());
                        }
                    }   
                    else if (Type == NbtType.Compound)
                    {
                        foreach (var tag in data)
                            buffer.Write(tag.ToArray());

                        buffer.Write(NbtType.End);   
                    }
                    break;
            }
            
            return stream.ToArray();
        }
    }
}