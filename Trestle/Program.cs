using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Trestle.Networking.Services;

namespace Trestle
{
    class Program
    {
        static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder().ConfigureServices((host, services) =>
            {
                // Services
                services.AddSingleton<IPacketService, PacketService>();
                services.AddSingleton<IClientService, ClientService>();
                
                // Hosted Services
                services.AddHostedService<ListenerService>();
            });
    }
}