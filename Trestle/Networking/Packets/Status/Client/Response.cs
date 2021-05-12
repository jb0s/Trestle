using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;

namespace Trestle.Networking.Packets.Status.Client
{
    [ClientBound(StatusPacket.Response)]
    public class Response : Packet
    {
        [Field]
        public string JsonResponse { get; set; } = JsonSerializer.Serialize(new ServerListResponse());
        
         public class ServerListResponse
        {
            [JsonPropertyName("version")] 
            public ServerListVersion Version { get; set; } = new();

            [JsonPropertyName("players")] 
            public ServerListPlayers Players { get; set; } = new(); }

        public class ServerListVersion
        {
            [JsonPropertyName("name")] 
            public string Name { get; set; } = "Trestle 1.12.2";

            [JsonPropertyName("protocol")] 
            public int Protocol { get; set; } = 320;
        }
        
        public class ServerListPlayers
        {
            [JsonPropertyName("max")] 
            public int Max { get; set; } = 20;

            [JsonPropertyName("online")] 
            public int Online { get; set; } = 0;
        }
    }
}