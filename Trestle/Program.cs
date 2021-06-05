using System;
using System.Linq;
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
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => {})
                .ConfigureLogging(builder => 
                    builder.ClearProviders()
                        .AddTrestleLogger())
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