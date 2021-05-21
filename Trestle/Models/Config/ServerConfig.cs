using System.Text.Json.Serialization;

namespace Trestle.Models.Config
{
    public class ServerConfig
    {
        [JsonPropertyName("logging")]
        public LoggingConfig Logging { get; set; } = new();
    }
}