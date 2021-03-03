using System;

namespace Trestle
{
    public class Constants
    {
        public static class Config
        {
            public static readonly string[] ConfigDefault = new string[]
            {
                "# This server is powered by Trestle - " + DateTime.Now.ToString(),
                "port=25565",
                "max_players=10",
                "world_name=world",
                "online_mode=false",
                "seed=" + new Random().Next(999999999),
                "motd=\u00A7bA Minecraft Server - Powered by Trestle!"
            };
        }
        
        public static class Vector3
        {
            public static readonly Trestle.Utils.Vector3 Zero = new (0);
            public static readonly Trestle.Utils.Vector3 One = new (1);

            public static readonly Trestle.Utils.Vector3 Up = new (0, 1, 0);
            public static readonly Trestle.Utils.Vector3 Down = new (0, -1, 0);
            public static readonly Trestle.Utils.Vector3 Left = new (-1, 0, 0);
            public static readonly Trestle.Utils.Vector3 Right = new (1, 0, 0);
            public static readonly Trestle.Utils.Vector3 Back = new (0, 0, -1);
            public static readonly Trestle.Utils.Vector3 Front = new (0, 0, 1);

            public static readonly Trestle.Utils.Vector3 East = new (1, 0, 0);
            public static readonly Trestle.Utils.Vector3 West = new (-1, 0, 0);
            public static readonly Trestle.Utils.Vector3 North = new (0, 0, -1);
            public static readonly Trestle.Utils.Vector3 South = new (0, 0, 1);
        }

        public static class Hitbox
        {
            public const int CORNER_COUNT = 0;
        }
    }
}