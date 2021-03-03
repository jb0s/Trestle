using System;
using System.Linq;
using Trestle.Entity;
using System.Collections.Generic;
using Trestle.Networking.Packets;

namespace Trestle.Worlds
{
    public class WorldManager
    {
        public World MainWorld { get; private set; }
        private Dictionary<string, World> SubWorlds { get; set; }
        
        public WorldManager(World mainWorld)
        {
            MainWorld = mainWorld;
            SubWorlds = new Dictionary<string, World>();
        }
        
        public World[] GetWorlds()
        {
            var worlds = SubWorlds.Values.ToList();
            worlds.Add(MainWorld);
            return worlds.ToArray();
        }

        public void AddLevel(string name, World wld)
        {
            Console.WriteLine("Initiating world: " + name);
            SubWorlds.Add(name, wld);
        }

        private World GetLevel(string name)
        {
            var d = (from wld in SubWorlds where wld.Key == name select wld.Value).FirstOrDefault();
            if (d != null) return d;
            return MainWorld;
        }

        public void TeleportToLevel(Player player, string world)
        {
            var wld = GetLevel(world);

            player.World.RemovePlayer(player);
            //player.World.BroadcastPlayerRemoval(player.Wrapper);

            player.World = wld;

            /*
            new Respawn(player.Wrapper)
            {
                Dimension = wld.Dimension,
                Difficulty = (byte) wld.Difficulty,
                GameMode = (byte) wld.DefaultGameMode
            }.Write();
*/
            player.HasSpawned = false;
            player.Location = wld.GetSpawnPoint();
            player.SendChunksForLocation(true);
        }

        public void TeleportToMain(Player player)
        {
            player.World.RemovePlayer(player);
            //player.World.BroadcastPlayerRemoval(player.Wrapper);

            player.World = MainWorld;

            /*
            new Respawn(player.Wrapper)
            {
                Dimension = 0,
                Difficulty = (byte) MainWorld.Difficulty,
                GameMode = (byte) MainWorld.DefaultGameMode
            }.Write();*/

            player.HasSpawned = false;
            player.Location = MainWorld.GetSpawnPoint();
            player.SendChunksForLocation(true);
        }

        public void SaveAllChunks()
        {
            foreach (World wld in GetWorlds())
            {
                wld.SaveChunks();
            }
            MainWorld.SaveChunks();
        }

        public Player[] GetAllPlayers()
        {
            List<Player> players = new List<Player>();
            foreach (World wld in GetWorlds())
            {
                players.AddRange(wld.OnlinePlayerArray);
            }
            players.AddRange(MainWorld.OnlinePlayerArray);
            return players.ToArray();
        }
    }
}