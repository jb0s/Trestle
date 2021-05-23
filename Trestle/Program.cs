using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trestle.Configuration.Service;
using Trestle.Levels.Services;
using Trestle.Networking.Services;

namespace Trestle
{
    class Program
    {
        private static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => {})
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
                });
    }
}