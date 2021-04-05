using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Ionic.Zlib;
using Trestle.Commands;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking;
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
        
        public static void BroadcastChat(string message)
            => BroadcastChat(new MessageComponent(message), ChatMessageType.ChatBox);
        
        public static void BroadcastChat(MessageComponent message, ChatMessageType chattype)
            => WorldManager.BroadcastChat(message, chattype);

        internal static int GetEntityId()
            => ++_entityId;
    }
}