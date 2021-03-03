using System;
using System.Text.Json.Serialization;
using fNbt;
using Trestle.Serialization;

namespace Trestle.Registry
{
    public class Dimension : INbtSerializable
    {
        [JsonPropertyName("piglin_safe")]
        public bool IsPiglinSafe { get; set; }
        
        [JsonPropertyName("natural")]
        public bool IsNatural { get; set; }
        
        [JsonPropertyName("ambient_light")]
        public float AmblientLight { get; set; }
        
        [JsonPropertyName("fixed_time")]
        public long? FixedTime { get; set; }
        
        [JsonPropertyName("infiniburn")]
        public string Infiniburn { get; set; }
        
        [JsonPropertyName("respawn_anchor_works")]
        public bool DoRespawnAnchorsWork { get; set; }
        
        [JsonPropertyName("has_skylight")]
        public bool HasSkylight { get; set; }
        
        [JsonPropertyName("bed_works")]
        public bool DoBedsWork { get; set; }
        
        [JsonPropertyName("effects")]
        public string Effects { get; set; }
        
        [JsonPropertyName("has_raids")]
        public bool HasRaids { get; set; }
        
        [JsonPropertyName("logical_height")]
        public int LogicalHeight { get; set; }
        
        [JsonPropertyName("coordinate_scale")]
        public float CoordinateScale { get; set; }
        
        [JsonPropertyName("ultrawarm")]
        public bool Ultrawarm { get; set; }
        
        [JsonPropertyName("has_ceiling")]
        public bool HasCeiling { get; set; }

        public NbtTag Serialize(string tagName)
        {
            var compound = new NbtCompound(tagName);
            compound.Add(new NbtByte("piglin_safe", Convert.ToByte(IsPiglinSafe)));
            compound.Add(new NbtByte("natural", Convert.ToByte(IsNatural)));
            compound.Add(new NbtFloat("ambient_light", AmblientLight));
            
            if (FixedTime != null)
                compound.Add(new NbtFloat("fixed_time", FixedTime.Value));

            compound.Add(new NbtString("infiniburn", Infiniburn));
            compound.Add(new NbtByte("respawn_anchor_works", Convert.ToByte(DoRespawnAnchorsWork)));
            compound.Add(new NbtByte("has_skylight", Convert.ToByte(HasSkylight)));
            compound.Add(new NbtByte("bed_works", Convert.ToByte(DoBedsWork)));
            compound.Add(new NbtString("effects", Effects));
            compound.Add(new NbtByte("has_raids", Convert.ToByte(HasRaids)));
            compound.Add(new NbtInt("logical_height", LogicalHeight));
            compound.Add(new NbtFloat("coordinate_scale", CoordinateScale));
            compound.Add(new NbtByte("ultrawarm", Convert.ToByte(Ultrawarm)));
            compound.Add(new NbtByte("has_ceiling", Convert.ToByte(HasCeiling)));
            
            return compound;
        }

        public void Deserialize(NbtTag value)
        {
            throw new NotImplementedException();
        }
    }
}