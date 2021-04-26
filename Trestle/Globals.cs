using System;
using System.IO;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Worlds;
using Trestle.Entity;
using Trestle.Commands;
using Trestle.Networking;
using System.Collections.Generic;
using System.Security.Cryptography;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle
{
    public class Globals
    {
        /// <summary>
        /// Provides random number generation for the server.
        /// </summary>
        public static Random Random;

        /// <summary>
        /// The protocol version ID that the server accepts.
        /// </summary>
        internal static int ProtocolVersion = 340;
        
        /// <summary>
        /// The version name for Trestle.
        /// </summary>
        internal static string ProtocolName = "Trestle 1.12.2";
        
        /// <summary>
        /// The client version name.
        /// </summary>
        internal static string OfficialProtocolName = "Minecraft 1.12.2";
        
        /// <summary>
        /// In charge of managing worlds.
        /// </summary>
        internal static WorldManager WorldManager = null;
        
        /// <summary>
        /// In charge of managing commands.
        /// </summary>
        internal static CommandManager CommandManager = null;

        /// <summary>
        /// Server encryption key.
        /// </summary>
        internal static RSAParameters ServerKey;

        /// <summary>
        /// The server listener.
        /// Listens to incoming connections, sends packets from server to client, and reads packets vice versa.
        /// </summary>
        internal static Listener ServerListener = null;
        
        /// <summary>
        /// Internal counter for the highest entity ID.
        /// </summary>
        private static int _entityId;

        /// <summary>
        /// Generates a unique identifier for an entity.
        /// </summary>
        internal static int GetEntityId()
            => ++_entityId;
    }
}