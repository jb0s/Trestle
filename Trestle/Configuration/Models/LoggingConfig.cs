using System.ComponentModel;
using Trestle.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Trestle.Configuration.Models
{
    public class LoggingConfig
    {
        [JsonPropertyName("verbosity")] 
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel Verbosity { get; set; }

        public LoggingConfig()
        {
            Verbosity = Debug.IsDebugBuild() ? LogLevel.Debug : LogLevel.Information;
        }
    }
}