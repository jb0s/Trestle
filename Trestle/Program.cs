using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trestle.Configuration;
using Trestle.Configuration.Service;
using Trestle.Levels.Services;
using Trestle.Logging;
using Trestle.Networking.Services;
using Trestle.Utils;

namespace Trestle
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            
            ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Welcome to Trestle!");
            logger.LogInformation($"Version: v{Assembly.GetExecutingAssembly().GetName().Version}");
            logger.LogInformation($"Protocol: {Constants.PROTOCOL_NAME} ({Constants.PROTOCOL_VERSION})");
            
            // Log every registered service
            logger.LogDebug("Active services:");
            foreach (KeyValuePair<Type, ServiceDescriptor> servicePair in host.Services.GetAllServiceDescriptors())
            {
                ServiceDescriptor service = servicePair.Value;
                string serviceName = service.ServiceType.Name;
                
                if (service.ServiceType.FullName.Contains("Trestle") && !service.ServiceType.FullName.Contains("Microsoft"))
                    logger.LogDebug($"- {serviceName.Substring(1, serviceName.Length - 1)}");
            }
            
            // Start the host
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => {})
                .ConfigureLogging(builder => 
                    builder.ClearProviders()
                        .AddTrestleLogger(config =>
                        {
                            config.LogLevels.Add(LogLevel.Debug, ConsoleColor.DarkGray);
                            config.LogLevels.Add(LogLevel.Information, ConsoleColor.White);
                            config.LogLevels.Add(LogLevel.Warning, ConsoleColor.Yellow);
                            config.LogLevels.Add(LogLevel.Error, ConsoleColor.Red);
                            config.LogLevels.Add(LogLevel.Critical, ConsoleColor.DarkRed);
                        })
                        .AddFilter("Trestle", LogLevel.Debug))
                .ConfigureServices((host, services) =>
                {
                    // Register services
                    services.AddSingleton<ILevelService, LevelService>();
                    services.AddSingleton<IConfigService, ConfigService>();
                    services.AddSingleton<IMojangService, MojangService>();
                    services.AddSingleton<IPacketService, PacketService>();
                    services.AddSingleton<IClientService, ClientService>();
                    
                    // Register osted Services
                    services.AddHostedService<ListenerService>();
                    
                    // Disable Microsoft Lifetime console messages
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                });
    }
}