using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trestle.Configuration.Service;
using Trestle.Levels.Services;
using Trestle.Logging;
using Trestle.Networking.Services;

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
            logger.LogInformation($"Protocol: Minecraft 1.16.5");
            
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
                        }))
                .ConfigureServices((host, services) =>
                {
                    // Services
                    services.AddSingleton<ILevelService, LevelService>();
                    services.AddSingleton<IConfigService, ConfigService>();
                    services.AddSingleton<IMojangService, MojangService>();
                    services.AddSingleton<IPacketService, PacketService>();
                    services.AddSingleton<IClientService, ClientService>();
                    
                    // Hosted Services
                    services.AddHostedService<ListenerService>();
                    
                    // Disable Lifetime messages
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                });
    }
}