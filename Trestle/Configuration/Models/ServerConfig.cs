using System.Text.Json.Serialization;

namespace Trestle.Configuration.Models
{
    public class ServerConfig
    {
        [JsonPropertyName("host")]
        public HostConfig Host { get; set; } = new();
        
        [JsonPropertyName("world")]
        public WorldConfig World { get; set; } = new();
        
        [JsonPropertyName("logging")]
        public LoggingConfig Logging { get; set; } = new();
    }
}