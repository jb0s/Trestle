using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Trestle.Logging
{
    public class TrestleLoggerConfiguration
    {
        public int EventId { get; set; }

        public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new()
        {
            [LogLevel.Debug] = ConsoleColor.Gray,
            [LogLevel.Information] = ConsoleColor.White,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Critical] = ConsoleColor.DarkRed
        };
    }
}