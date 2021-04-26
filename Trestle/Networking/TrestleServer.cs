using System;
using System.IO;
using System.Linq;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Worlds;
using Trestle.Entity;
using System.Threading;
using Trestle.Commands;
using System.Diagnostics;
using Trestle.Worlds.Normal;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Networking
{
    public class TrestleServer
    {
        public static bool Stopped { get; private set; }
        
        private bool _ready = false;

        public TrestleServer()
        {
            Logger.Special("Welcome to Trestle!");
            
            var loadStopwatch = new Stopwatch();
            loadStopwatch.Start();
            
            // Properly close the server if something goes terribly wrong.
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledException;
            
            // Create folders for the players & world, etc
            InitializeFileStructure();
            
            Logger.Info("Loading configuration...");
            Config.Load();

            Logger.Info("Initializing...");
            InitializeGlobals();
            
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

            // Mark the server as ready and start the server loop.
            _ready = true;
            _ = ServerLoop();
            
            // Stop the loading timer and log the amount of time it took to start up.
            loadStopwatch.Stop();
            Logger.Info($"Done! {loadStopwatch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// This loop is in charge of sending KeepAlives to every client.
        /// Ticks itself indefinitely every 50ms when called.
        /// </summary>
        private async Task ServerLoop()
        {
            int keepAliveTicks = 0;
            
            var stopwatch = Stopwatch.StartNew();

            while (_ready)
            {
                await Task.Delay(50);

                keepAliveTicks++;
                if (keepAliveTicks > 50)
                {
                    int keepAliveId = DateTime.Now.Millisecond;

                    foreach (var client in Globals.ServerListener.Clients.Where(x => x.State == ClientState.Play))
                        client.ProcessKeepAlive(keepAliveId);

                    keepAliveTicks = 0;
                }
            }
        }

        /// <summary>
        /// Disconnects every client and closes the server.
        /// </summary>
        public static void Shutdown()
        {
            Logger.Info("Stopping server");

            Stopped = true;
            
            // Stop the listener.
            Globals.ServerListener.Stop();
            
            Environment.Exit(0);
        }
        
        #region Initialization

        /// <summary>
        /// Creates folders which will hold data for logs, players, worlds, etc.
        /// </summary>
        private void InitializeFileStructure()
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            if (!Directory.Exists("players"))
                Directory.CreateDirectory("players");
            
            if (!Directory.Exists("worlds"))
                Directory.CreateDirectory("worlds");
        }
        
        /// <summary>
        /// This function is in charge of initializing globals & managers.
        /// </summary>
        private void InitializeGlobals()  
        {
            Globals.Random = new Random();
            Globals.ServerListener = new Listener();
            Globals.WorldManager = new WorldManager();
            Globals.CommandManager = new CommandManager();
            Globals.ServerKey = PacketCryptography.GenerateKeyPair();
        }

        /// <summary>
        /// This function is in charge of generating / loading worlds upon startup.
        /// </summary>
        private void GenerateWorlds()
            => Globals.WorldManager.CreateWorld<NormalWorldGenerator>(WorldType.Normal); // Create the overworld.
        
        #endregion

        #region Packet broadcasting

        /// <summary>
        /// Broadcasts a packet to every registered player.
        /// </summary>
        /// <param name="packet"></param>
        public static void BroadcastPacket(Packet packet)
        {
            foreach (var world in Globals.WorldManager.Worlds.Values)
                world.BroadcastPacket(packet);
        }
        
        /// <summary>
        /// Broadcasts a chat message to every registered player.
        /// </summary>
        /// <param name="message"></param>
        public static void BroadcastChat(string message)
            => BroadcastChat(new MessageComponent(message), ChatMessageType.ChatBox);
        
        /// <summary>
        /// Broadcasts a message to every registered player.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        public static void BroadcastChat(MessageComponent message, ChatMessageType chatType)
            => Globals.WorldManager.BroadcastChat(message, chatType);

        #endregion

        #region Player management

        /// <summary>
        /// Returns an array of every online <see cref="Player"/> in the server.
        /// </summary>
        public static Player[] GetOnlinePlayers()
        {
            var players = new List<Player>();
            
            foreach (var world in Globals.WorldManager.Worlds.Values)
                foreach (var player in world.Players.Values)
                    players.Add(player);

            return players.ToArray();
        }

        /// <summary>
        /// This function is in charge of making sure that the players spawn for each other and appear in the tab list.
        /// Calling this function is required to put players in a world.
        /// Also broadcasts a join message to the server.
        /// </summary>
        /// <param name="client">Client that's joining the server.</param>
        public static void RegisterPlayer(Client client)
        {
            // Add every online player to the tab list for the new player.
            foreach (var player in GetOnlinePlayers())
                client.SendPacket(new PlayerListItem(false, Mojang.GetProfileById(player.Uuid)));
	        
            // Add the new player to every online player's tab list.
            BroadcastPacket(new PlayerListItem(false, Mojang.GetProfileById(client.Player.Uuid)));

            // Send the player to the world it's supposed to be in.
            // (We do this in here after the PlayerListItems because of packet orders and shit.)
            client.Player.SendToWorld(client.Player.World);
            
            // At the point of the list item packet being broadcasted, the player wasn't in any world yet, so the packet above 
            // will *not* be sent to them. We have to do it manually, as the player only gets moved to a world at the end of this function.
            client.SendPacket(new PlayerListItem(false, Mojang.GetProfileById(client.Player.Uuid)));
            
            // Send a global chat message announcing that the player's joined.
            var msg = Constants.SystemMessages.JoinMessages[Globals.Random.Next(0, Constants.SystemMessages.JoinMessages.Length)].Replace("{PLAYER}", $"{ChatColor.Aqua}{client.Username}{ChatColor.Gray}");
            BroadcastChat($"{ChatColor.DarkGray}[{ChatColor.Green}+{ChatColor.DarkGray}] {ChatColor.Gray}{msg}");

            // Send player header list and footer
            client.SendPacket(new PlayerListHeaderAndFooter(new MessageComponent(Config.TabListHeader), new MessageComponent(Config.TabListFooter)));
        }
        
        /// <summary>
        /// This function is in charge of cleaning up a disconnecting player's entity & data.
        /// Also broadcasts a quit message to the server.
        /// </summary>
        /// <param name="client"></param>
        public static void UnregisterPlayer(Client client)
        {
            try
            {
                client.Player.Save();
            }
            catch (Exception e)
            {
                Logger.Error($"Unable to save data for {client.Player.Username}\n{e}");
            }
            
            // Send a global chat message announcing that the player has left :(
            var msg = Constants.SystemMessages.LeaveMessages[Globals.Random.Next(0, Constants.SystemMessages.LeaveMessages.Length)].Replace("{PLAYER}", $"{ChatColor.Aqua}{client.Username}{ChatColor.Gray}");
            BroadcastChat($"{ChatColor.DarkGray}[{ChatColor.Red}-{ChatColor.DarkGray}] {ChatColor.Gray}{msg}");
            
            // Remove the player from the world (despawns it from other clients)
            client.Player.World.RemovePlayer(client.Player);
            
            // Remove the player from the tab list & despawn its entity
            BroadcastPacket(new PlayerListItem(true, Mojang.GetProfileById(client.Player.Uuid)));
            client.Player.World.BroadcastPacket(new DestroyEntities(new int[1] { client.Player.EntityId }));
        }

        #endregion

        #region Handlers

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

        #endregion
    }
}
