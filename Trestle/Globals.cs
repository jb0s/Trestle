using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Ionic.Zlib;
using Trestle.Networking;
using Trestle.Worlds;

namespace Trestle
{
    public class Globals
    {
        public static Random Random;

        internal static int ProtocolVersion = 754;
        internal static string ProtocolName = "Trestle 1.16.5";
        internal static string OfficialProtocolName = "Minecraft 1.16.5";
        
        internal static WorldManager WorldManager = null;
        internal static RSAParameters ServerKey;

        internal static Listener ServerListener = null;
        internal static Registry.Registry Registry = null;

        public static void Initialize()
        {
            Random = new Random();
        }
        
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
    }
}