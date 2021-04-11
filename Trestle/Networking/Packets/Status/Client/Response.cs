using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Status.Client
{
    [ClientBound(StatusPacket.Response)]
    public class Response : Packet
    {
        // Serializing the response every single time the server is pinged is a perfect way to waste memory.
        // So, instead, we only serialize it once as we only need to serialize it once per server instance.
        private static string _cachedResponse = JsonSerializer.Serialize(new ServerListResponse());

        [Field]
        public string JsonResponse { get; set; } = _cachedResponse;

        public class ServerListResponse
        {
            [JsonPropertyName("version")] public ServerListVersion Version { get; set; } = new();

            [JsonPropertyName("players")] public ServerListPlayers Players { get; set; } = new();

            [JsonPropertyName("description")] public MessageComponent Description { get; set; } = new(Config.Motd);

            [JsonPropertyName("favicon")]
            public string Favicon { get; set; } = "";
        }

        public class ServerListVersion
        {
            [JsonPropertyName("name")] public string Name { get; set; } = Globals.ProtocolName;
            [JsonPropertyName("protocol")] public int Protocol { get; set; } = Globals.ProtocolVersion;

            public ServerListVersion()
            {
                
            }
            public ServerListVersion(string name, int protocol)
            {
                Name = name;
                Protocol = protocol;
            }
        }
        
        public class ServerListPlayers
        {
            [JsonPropertyName("max")] public int Max { get; set; } = Config.MaxPlayers;

            [JsonPropertyName("online")] public int Online { get; set; } = TrestleServer.GetOnlinePlayers().Length;

            [JsonPropertyName("sample")] public ServerListPlayer[] Players { get; set; }

            public ServerListPlayers()
            {
                var players = TrestleServer.GetOnlinePlayers();

                Players = new ServerListPlayer[players.Length];
                for(int i = 0; i < players.Length; i++)
                {
                    Players[i] = new ServerListPlayer(players[i]);
                }
            }
        }

        public class ServerListPlayer
        {
            [JsonPropertyName("name")] public string Username { get; set; }
            
            [JsonPropertyName("id")] public Guid Uuid { get; set; }
            
            public ServerListPlayer(Player player)
            {
                Username = player.Username;
                Uuid = Guid.Parse(player.Uuid);
            }
        }
    }
}