using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Trestle.Configuration.Models
{
    public class WorldConfig
    {
        [JsonPropertyName("seed")]
        public long Seed { get; set; }
        
        [JsonPropertyName("allow_nether")]
        public bool AllowNether { get; set; }
        
        [JsonPropertyName("allow_end")]
        public bool AllowEnd { get; set; }
        
        [JsonPropertyName("allow_mobs")]
        public bool AllowMobs { get; set; }
        
        [JsonPropertyName("is_hardcore")]
        public bool IsHardcore { get; set; }
        
        public WorldConfig()
        {
            Seed = new Random().Next(9999999) * 5;
            
            AllowNether = true;
            AllowEnd = true;
            AllowMobs = true;

            IsHardcore = false;
        }
    }
}