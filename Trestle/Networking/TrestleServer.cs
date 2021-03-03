using System;
using System.Diagnostics;
using System.Threading;
using Trestle.Worlds;
using Trestle.Worlds.Standard;

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
            
            Logger.Info("Loading configuration..");
            Config.Load();
            Logger.Info("Loaded configuration!");

            InitGlobals();

            try
            {
                new Thread(() => Globals.ServerListener.Start()).Start();
                //new Thread(() => new ConsoleCommandHandler().WaitForCommand()).Start();
            }
            catch (Exception ex)
            {
                UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
            }
            
            loadStopwatch.Stop();
            Logger.Info($"Initialized in {loadStopwatch.ElapsedMilliseconds}ms!");
        }

        private void InitGlobals()
        {
            Globals.Random = new Random();
            Globals.Registry = new Registry.Registry();
            Globals.ServerListener = new Listener();
          //  Globals.ClientManager = new ClientManager();
          //  Globals.CommandManager = new CommandManager();
          //  Globals.ServerKey = PacketCryptography.GenerateKeyPair();
            Globals.WorldManager = new WorldManager(new StandardWorld("world"));
        }
        
        /// <summary>
        /// Fired when an unhandled exception occurs.
        /// We don't want the server to fully shut down when something goes wrong.
        /// </summary>
        private void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            Logger.Error($"An unhandled exception occurred: {e.Message}");
        }
    }
}