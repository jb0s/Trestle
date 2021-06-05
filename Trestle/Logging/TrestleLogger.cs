using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Trestle.Logging
{
    public class TrestleLogger : ILogger
    {
        private readonly string _name;
        private readonly TrestleLoggerConfiguration _config;

        public TrestleLogger(string name, TrestleLoggerConfiguration config)
            => (_name, _config) = (name, config);
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                string formattedName = null;

                // If the name contains dots, we assume it has a full namespace.
                // We don't want long names in the console, so we trim it to only the class name.
                if (_name.Contains('.'))
                    formattedName = _name.Split('.').LastOrDefault();
                
                // Timestamp
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{DateTime.Now}] ");
                
                // Prefix and message
                // If `formattedName` == null, we use `_name` instead.
                Console.ForegroundColor = _config.LogLevels[logLevel];
                Console.Write($"[{formattedName ?? _name}: {logLevel}] ");
                Console.Write($"{formatter(state, exception)}\n");
                
                Console.ForegroundColor = originalColor;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
            => _config.LogLevels.ContainsKey(logLevel);

        public IDisposable BeginScope<TState>(TState state)
            => default;
    }
}