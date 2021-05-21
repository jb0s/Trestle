using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Trestle.Configuration.Models;
using Trestle.Configuration.Service;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;

namespace Trestle.Networking.Packets.Status.Client
{
    [ClientBound(StatusPacket.Response)]
    public class Response : Packet
    {
        private IConfigService _configService;
        private static ServerConfig _config;
        
        [Field]
        public string JsonResponse { get; set; }

        public Response(IConfigService configService)
        {
            // TODO: Refactor this (possible race condition)
            _configService = configService;
            _config = configService.GetConfig().Result;
            JsonResponse = JsonSerializer.Serialize(new ServerListResponse());
        }
        
        public class ServerListResponse
        {
            [JsonPropertyName("version")] 
            public ServerListVersion Version { get; set; } = new();

            [JsonPropertyName("players")] 
            public ServerListPlayers Players { get; set; } = new(); 
            
            [JsonPropertyName("description")] 
            public ServerListDescription Description { get; set; } = new(); 
        }

        public class ServerListVersion
        {
            [JsonPropertyName("name")] 
            public string Name { get; set; } = "Trestle 1.12.2";

            [JsonPropertyName("protocol")] 
            public int Protocol { get; set; } = 340;
        }
        
        public class ServerListPlayers
        {
            [JsonPropertyName("max")] 
            public int Max { get; set; } = _config.Host.MaxPlayers;

            [JsonPropertyName("online")] 
            public int Online { get; set; } = 0;
        }

        public class ServerListDescription
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = _config.Host.Motd;
        }
    }
}