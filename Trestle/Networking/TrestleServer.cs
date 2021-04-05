using System;
using System.Linq;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.World;
using System.Threading;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Trestle.World.Generation;

namespace Trestle.Networking
{
    public class TrestleServer
    {
        private bool _ready = false;

        public TrestleServer()
        {
            Logger.Special("Welcome to Trestle!");
            
            var loadStopwatch = new Stopwatch();
            loadStopwatch.Start();
            
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledException;
            
            Logger.Info("Loading configuration...");
            Config.Load();

            Logger.Info("Initializing...");
            InitGlobals();
            
            Logger.Info("Loading worlds...");
            GenerateWorlds();
            
            try
            {
                new Thread(() => Globals.ServerListener.Start()).Start();
            }
            catch (Exception ex)
            {
                UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
            }

            _ready = true;
            _ = ServerLoop();
            
            loadStopwatch.Stop();
            Logger.Info($"Done! {loadStopwatch.ElapsedMilliseconds}ms");
        }

        private async Task ServerLoop()
        {
            var keepAliveTicks = 0;

            short iterationsPerSecond = 0;
            var stopwatch = Stopwatch.StartNew();

            while (_ready)
            {
                await Task.Delay(50);

                keepAliveTicks++;
                if (keepAliveTicks > 50)
                {
                    var keepAliveId = DateTime.Now.Millisecond;

                    foreach (var client in Globals.ServerListener.Clients.Where(x => x.State == ClientState.Play))
                        client.ProcessKeepAlive(keepAliveId);

                    keepAliveTicks = 0;
                }
            }
        }
        
        private void InitGlobals()
        {
            Globals.Random = new Random();
            Globals.ServerListener = new Listener();
            Globals.WorldManager = new WorldManager();
            Globals.ServerKey = PacketCryptography.GenerateKeyPair();
        }

        private void GenerateWorlds()
        {
            Globals.WorldManager.CreateWorld(WorldType.Normal);
        }

        /// <summary>
        /// Fired when an unhandled exception occurs.
        /// </summary>
        private void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            
            foreach (var client in Globals.ServerListener.Clients)
                client.Player?.Kick(new MessageComponent($"{ChatColor.Red}An unhandled exception occurred.\n\n{ChatColor.Reset}{e.Message}"));
            
            Logger.Error($"An unhandled exception occurred: {e.Message}");
        }
    }
}