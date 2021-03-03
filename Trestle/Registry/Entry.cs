using System;
using System.Text.Json.Serialization;
using fNbt;
using Trestle.Serialization;

namespace Trestle.Registry
{
    public class Entry<T> where T : INbtSerializable 
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }   
        
        [JsonPropertyName("id")]
        public int Id { get; set; }   
        
        [JsonPropertyName("element")]
        public T Element { get; set; }

        public NbtTag Serialize()
        {
            var compound = new NbtCompound
            {
                new NbtString("name", Name),
                new NbtInt("id", Id),
                (NbtCompound)Element.Serialize("element")
            };
            
            return compound;
        }
        
        public NbtTag Serialize(string tagName)
        {
            var compound = new NbtCompound(tagName)
            {
                new NbtString("name", Name),
                new NbtInt("id", Id),
                (NbtCompound)Element.Serialize("element")
            };
            
            return compound;
        }
    }
}