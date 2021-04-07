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
                "online_mode=true",
                "max_missed_keep_alives=5",
                "motd=A Minecraft server - \u00A7bPowered by Trestle!",
                "",
                "# World Settings",
                "seed=" + new Random().Next(999999999),
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

        public static class SystemMessages
        {
            public static readonly string[] JoinMessages = new string[]
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
        
            public static readonly string[] LeaveMessages = new string[]
            {
                "Leaving so soon, {PLAYER}?",
                "{PLAYER} will be missed.",
                "Don't let the door hit you on the way out, {PLAYER}.",
                "{PLAYER}.exe has stopped responding",
            };
        }
    }
}