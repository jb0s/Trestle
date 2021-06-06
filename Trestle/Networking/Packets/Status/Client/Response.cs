using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Trestle.Configuration;
using Trestle.Configuration.Models;
using Trestle.Configuration.Service;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;

namespace Trestle.Networking.Packets.Status.Client
{
    [ClientBound(StatusPacket.Response)]
    public class Response : Packet
    {
        [Field]
        public string JsonResponse { get; set; }

        public Response(IConfigService configService)
            => JsonResponse = JsonSerializer.Serialize(new ServerListResponse(configService));
        
        public class ServerListResponse
        {
            [JsonPropertyName("version")] 
            public ServerListVersion Version { get; set; }

            [JsonPropertyName("players")] 
            public ServerListPlayers Players { get; set; }
            
            [JsonPropertyName("description")] 
            public ServerListDescription Description { get; set; }

            public ServerListResponse(IConfigService configService)
            {
                Version = new ServerListVersion();
                Players = new ServerListPlayers(configService);
                Description = new ServerListDescription(configService);
            }
        }

        public class ServerListVersion
        {
            [JsonPropertyName("name")] 
            public string Name { get; set; }

            [JsonPropertyName("protocol")] 
            public int Protocol { get; set; }

            public ServerListVersion()
            {
                Name = Constants.PROTOCOL_NAME;
                Protocol = Constants.PROTOCOL_VERSION;
            }
        }
        
        public class ServerListPlayers
        {
            [JsonPropertyName("max")] 
            public int Max { get; set; }

            [JsonPropertyName("online")] 
            public int Online { get; set; }

            public ServerListPlayers(IConfigService configService)
            {
                Max = configService.GetConfig().Host.MaxPlayers;
                
                // TODO: Add this
                Online = 0;
            }
        }

        public class ServerListDescription
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }

            public ServerListDescription(IConfigService configService)
            {
                Text = configService.GetConfig().Host.Motd;
            }
        }
    }
}