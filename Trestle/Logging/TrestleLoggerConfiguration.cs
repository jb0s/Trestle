using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Trestle.Logging
{
    public class TrestleLoggerConfiguration
    {
        public int EventId { get; set; }
        public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new();
    }
}