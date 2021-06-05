using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Trestle.Logging
{
    public static class TrestleLoggerExtensions
    {
        public static ILoggingBuilder AddTrestleLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, TrestleLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <TrestleLoggerConfiguration, TrestleLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddTrestleLogger(this ILoggingBuilder builder, Action<TrestleLoggerConfiguration> configure)
        {
            builder.AddTrestleLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}