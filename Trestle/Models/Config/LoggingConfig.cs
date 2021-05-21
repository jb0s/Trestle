using Trestle.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Trestle.Models.Config
{
    public class LoggingConfig
    {
        [JsonPropertyName("verbosity")] 
        public LogLevel Verbosity { get; set; } = Debug.IsDebugBuild() ? LogLevel.Debug : LogLevel.Information;
    }
}