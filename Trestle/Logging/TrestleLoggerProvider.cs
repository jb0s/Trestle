using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Trestle.Logging
{
    public class TrestleLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable _onChangeToken;
        private TrestleLoggerConfiguration _config;
        
        private readonly ConcurrentDictionary<string, TrestleLogger> _loggers = new();

        public TrestleLoggerProvider(IOptionsMonitor<TrestleLoggerConfiguration> config)
        {
            _config = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _config = updatedConfig);
        }
        
        /// <summary>
        /// Creates a new logger.
        /// </summary>
        public ILogger CreateLogger(string categoryName)
            => _loggers.GetOrAdd(categoryName, name => new TrestleLogger(name, _config));

        /// <summary>
        /// Stops all loggers and disposes of resources.
        /// </summary>
        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken.Dispose();
        }
    }
}