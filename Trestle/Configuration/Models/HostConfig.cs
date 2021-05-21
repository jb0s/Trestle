using System.Text.Json.Serialization;

namespace Trestle.Configuration.Models
{
    public class HostConfig
    {
        [JsonPropertyName("port")]
        public int Port { get; set; }
        
        [JsonPropertyName("online_mode")]
        public bool OnlineMode { get; set; }
        
        [JsonPropertyName("motd")]
        public string Motd { get; set; }
        
        [JsonPropertyName("max_players")]
        public int MaxPlayers { get; set; }
        
        [JsonPropertyName("allow_cheats")]
        public bool AllowCheats { get; set; }
        
        [JsonPropertyName("correct_players")]
        public bool CorrectPlayers { get; set; }

        public HostConfig()
        {
            Port = 25565;
            OnlineMode = true;

            Motd = "A Trestle server";
            MaxPlayers = 20;

            AllowCheats = false;
            CorrectPlayers = true;
        }
    }
}