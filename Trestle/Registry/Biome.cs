using System.Text.Json.Serialization;
using fNbt;
using Trestle.Serialization;

namespace Trestle.Registry
{
    public class Biome : INbtSerializable
    {
        [JsonPropertyName("precipitation")]
        public string Precipitation { get; set; } 

        [JsonPropertyName("effects")]
        public BiomeEffects Effects { get; set; } 

        [JsonPropertyName("depth")]
        public float Depth { get; set; } 

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } 

        [JsonPropertyName("scale")]
        public float Scale { get; set; } 

        [JsonPropertyName("downfall")]
        public float Downfall { get; set; } 

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("temperature_modifier")]
        public string TemperatureModifier { get; set; }

        public NbtTag Serialize(string tagName)
        {
            var compound = new NbtCompound(tagName)
            {
                new NbtString("precipitation", Precipitation),
                new NbtCompound("effects")
                {
                    new NbtInt("sky_color", Effects.SkyColor),
                    new NbtInt("water_fog_color", Effects.WaterFogColor),
                    new NbtInt("fog_color", Effects.FogColor),
                    new NbtInt("water_color", Effects.WaterColor),
                },
                new NbtFloat("depth", Depth),
                new NbtFloat("temperature", Temperature),
                new NbtFloat("scale", Scale),
                new NbtFloat("downfall", Downfall),
                new NbtString("category", Category)
            };
            
            if (TemperatureModifier != null)
                compound.Add(new NbtString("temperature_modifier", TemperatureModifier));
            return compound;
        }

        public void Deserialize(NbtTag value)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BiomeEffects
    {
        [JsonPropertyName("sky_color")]
        public int SkyColor { get; set; } 

        [JsonPropertyName("water_fog_color")]
        public int WaterFogColor { get; set; } 

        [JsonPropertyName("fog_color")]
        public int FogColor { get; set; } 

        [JsonPropertyName("water_color")]
        public int WaterColor { get; set; }
        
        [JsonPropertyName("mood_sound")]
        public MoodSound MoodSound { get; set; }
    }
    
    public class MoodSound    
    {
        [JsonPropertyName("tick_delay")]
        public int TickDelay { get; set; } 

        [JsonPropertyName("offset")]
        public int Offset { get; set; } 

        [JsonPropertyName("sound")]
        public string Sound { get; set; } 

        [JsonPropertyName("block_search_extent")]
        public int BlockSearchExtent { get; set; } 
    }
}