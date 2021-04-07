﻿using System;
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
        [Field] 
        public string JsonResponse { get; set; } = JsonSerializer.Serialize(new ServerListResponse());

        public class ServerListResponse
        {
            [JsonPropertyName("version")] public ServerListVersion Version { get; set; } = new();

            [JsonPropertyName("players")] public ServerListPlayers Players { get; set; } = new();

            [JsonPropertyName("description")] public MessageComponent Description { get; set; } = new(Config.Motd);

            [JsonPropertyName("favicon")]
            public string Favicon { get; set; } =
                "data:image/png;base64,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";
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