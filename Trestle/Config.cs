using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Trestle
{
    public class Config
    {
        internal static string Seed = "default";
        internal static bool UseCompression = false;
        internal static int CompressionThreshold = 999999999;
        internal static bool OnlineMode = true;
        internal static bool EncryptionEnabled = true;
        internal static byte MaxPlayers = 10;
        internal static int Port = 25565;
        internal static int MaxMissedKeepAlives = 5;
        internal static string TabListHeader = "";
        internal static string TabListFooter = "";
        
        public static bool DisplayPacketErrors = false;
        public static bool Debug = true;
        internal static string Motd = "A Minecraft server - \u00A7bPowered by Trestle!";

        public static bool ReportExceptionsToClient = true;
        
        public static string ConfigFile = "server.properties";

        public static void Load()
        {
            if (!File.Exists(ConfigFile))
                File.WriteAllLines(ConfigFile, Constants.Config.ConfigDefault);

            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(ConfigFile))
                if(!row.StartsWith("#") && !string.IsNullOrEmpty(row))
                    data.Add(row.Split('=')[0], string.Join("=",row.Split('=').Skip(1).ToArray()));

            if (data.ContainsKey("motd")) Motd = data["motd"];
            if (data.ContainsKey("seed")) Seed = data["seed"];
            if (data.ContainsKey("max_players")) MaxPlayers = byte.Parse(data["max_players"]);
            if (data.ContainsKey("online_mode")) OnlineMode = bool.Parse(data["online_mode"]);
            if (data.ContainsKey("port")) Port = int.Parse(data["port"]);
            if (data.ContainsKey("max_missed_keep_alives")) MaxMissedKeepAlives = int.Parse(data["max_missed_keep_alives"]);
            if (data.ContainsKey("tab_list_header")) TabListHeader = data["tab_list_header"];
            if (data.ContainsKey("tab_list_footer")) TabListFooter = data["tab_list_footer"];

            Logger.Debug($"Using Configuration:\n" +
                         $"    Port: {Port}\n"+
                         $"    Max Players: {MaxPlayers}\n"+
                         $"    Online Mode: {OnlineMode}\n"+
                         $"    Seed: {Seed}\n"+
                         $"    MOTD: {Motd}\n"+
                         $"    Max Missed KeepAlives: {MaxMissedKeepAlives}");
        }
    }
}