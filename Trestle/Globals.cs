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

        public static void UnregisterPlayer(Client client)
        {
            // Send a global chat message announcing that the player has left :(
            BroadcastChat($"{ChatColor.Yellow}{client.Username} left the game");
            
            // Remove the player from the world (despawns it from other clients)
            client.Player.World.RemovePlayer(client.Player);
            
            // Remove the player from the tab list
            BroadcastPacket(new PlayerListItem(true, Mojang.GetProfileById(client.Player.Uuid)));
        }
        
        public static void BroadcastChat(string message)
            => BroadcastChat(new MessageComponent(message), ChatMessageType.ChatBox);
        
        public static void BroadcastChat(MessageComponent message, ChatMessageType chattype)
            => WorldManager.BroadcastChat(message, chattype);

        internal static int GetEntityId()
            => ++_entityId;
    }
}