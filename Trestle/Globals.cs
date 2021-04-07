using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Ionic.Zlib;
using Trestle.Commands;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using Trestle.World;

namespace Trestle
{
    public class Globals
    {
        public static Random Random;

        internal static int ProtocolVersion = 340;
        internal static string ProtocolName = "Trestle 1.12.2";
        internal static string OfficialProtocolName = "Minecraft 1.12.2";
        
        internal static WorldManager WorldManager = null;
        internal static CommandManager CommandManager = null;
        internal static RSAParameters ServerKey;

        internal static Listener ServerListener = null;
        private static int _entityId;

        private static string[] _joinMessages = new string[]
        {
            "{PLAYER} hopped into the server.",
            "{PLAYER} just joined the server - glhf!",
            "Welcome, {PLAYER}. We hope you brought pizza.",
            "Wild {PLAYER} appeared!",
            "Never gonna give {PLAYER} up, never gonna let {PLAYER} down.",
            "{PLAYER} just slid into the server.",
            "Where's {PLAYER}? In the server!",
            "Knock knock. Who's there? It's {PLAYER}.",
            "Cheers, love! {PLAYER}'s here!",
            "You made it, {PLAYER}!",
            "{PLAYER} bounces into the server.",
        };
        
        private static string[] _leaveMessages = new string[]
        {
            "Leaving so soon, {PLAYER}?",
            "{PLAYER} will be missed.",
            "Don't let the door hit you on the way out, {PLAYER}.",
            "{PLAYER}.exe has stopped responding",
        };

        public static byte[] Compress(byte[] input)
        {
            using (var output = new MemoryStream())
            {
                using (var zip = new GZipStream(output, CompressionMode.Compress))
                {
                    zip.Write(input, 0, input.Length);
                }
                return output.ToArray();
            }
        }

        public static byte[] Decompress(byte[] input)
        {
            using (var output = new MemoryStream(input))
            {
                using (var zip = new GZipStream(output, CompressionMode.Decompress))
                {
                    var bytes = new List<byte>();
                    var b = zip.ReadByte();
                    while (b != -1)
                    {
                        bytes.Add((byte) b);
                        b = zip.ReadByte();
                    }
                    return bytes.ToArray();
                }
            }
        }

        public static void BroadcastPacket(Packet packet)
        {
            foreach (var world in WorldManager.Worlds.Values)
                world.BroadcastPacket(packet);
        }

        public static Player[] GetOnlinePlayers()
        {
            var players = new List<Player>();
            
            foreach (var world in WorldManager.Worlds.Values)
                foreach (var player in world.Players.Values)
                    players.Add(player);

            return players.ToArray();
        }

        public static void RegisterPlayer(Client client)
        {
            // Add every online player to the tab list for the new player.
            foreach (var player in GetOnlinePlayers())
                client.SendPacket(new PlayerListItem(false, Mojang.GetProfileById(player.Uuid)));
	        
            // Add the new player to the tab list, too.
            client.SendPacket(new PlayerListItem(false, Mojang.GetProfileById(client.Player.Uuid)));
            BroadcastPacket(new PlayerListItem(false, Mojang.GetProfileById(client.Player.Uuid)));

            var msg = _joinMessages[Random.Next(0, _joinMessages.Length)].Replace("{PLAYER}", $"{ChatColor.Aqua}{client.Username}{ChatColor.Gray}");
            BroadcastChat($"{ChatColor.DarkGray}[{ChatColor.Green}+{ChatColor.DarkGray}] {ChatColor.Gray}{msg}");
        }
        
        public static void UnregisterPlayer(Client client)
        {
            // Send a global chat message announcing that the player has left :(
            var msg = _leaveMessages[Random.Next(0, _leaveMessages.Length)].Replace("{PLAYER}", $"{ChatColor.Aqua}{client.Username}{ChatColor.Gray}");
            BroadcastChat($"{ChatColor.DarkGray}[{ChatColor.Red}-{ChatColor.DarkGray}] {ChatColor.Gray}{msg}");
            
            // Remove the player from the world (despawns it from other clients)
            client.Player.World.RemovePlayer(client.Player);
            
            // Remove the player from the tab list & despawn its entity
            BroadcastPacket(new PlayerListItem(true, Mojang.GetProfileById(client.Player.Uuid)));
            client.Player.World.BroadcastPacket(new DestroyEntities(new int[1] { client.Player.EntityId }));
        }
        
        public static void BroadcastChat(string message)
            => BroadcastChat(new MessageComponent(message), ChatMessageType.ChatBox);
        
        public static void BroadcastChat(MessageComponent message, ChatMessageType chattype)
            => WorldManager.BroadcastChat(message, chattype);

        internal static int GetEntityId()
            => ++_entityId;
    }
}