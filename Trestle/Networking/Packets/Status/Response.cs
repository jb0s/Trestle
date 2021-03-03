using System.Text.Json;
using System.Text.Json.Serialization;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Status
{
    [ClientBound(StatusPacket.Response)]
    public class Response : Packet
    {
        [Field] 
        public string JSONResponse { get; set; } = JsonSerializer.Serialize(new ServerListResponse());

        public class ServerListResponse
        {
            [JsonPropertyName("version")] public ServerListVersion Version { get; set; } = new ServerListVersion();

            [JsonPropertyName("players")] public ServerListPlayers Players { get; set; } = new ServerListPlayers();

            [JsonPropertyName("description")] public MessageComponent Description { get; set; } = new MessageComponent(Config.Motd);
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

            [JsonPropertyName("online")] public int Online { get; set; } = 0;
            
            public ServerListPlayers()
            { }
            
            public ServerListPlayers(int max, int online)
            {
                Max = max;
                Online = online;
            }
        }
    }
}