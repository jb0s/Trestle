using System;
using System.Linq;
using fNbt;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.JoinGame)]
    public class JoinGame : Packet
    {
        [Field] 
        public int EntityId { get; set; }

        [Field] 
        public bool IsHardcode { get; set; } = false;

        [Field] 
        public sbyte GameMode { get; set; } = 0x00;

        [Field] 
        public byte PreviousGameMode { get; set; } = 0x00;
        
        [Field]
        public string[] WorldNames { get; set; }
        
        [Field]
        public NbtCompound DimensionCodec { get; set; }
        
        [Field]
        public NbtCompound Dimension { get; set; }

        [Field]
        public string WorldName { get; set; }
        
        [Field]
        public long HashedSeed { get; set; }
        
        [Field]
        [VarInt]
        public int MaxPlayers { get; set; }

        [Field] 
        [VarInt] 
        public int ViewDistance { get; set; } = 16;

        [Field] 
        public bool ShowReducedDebugInfo { get; set; } = false;
        
        [Field] 
        public bool IsRespawnScreenEnabled { get; set; } = true;
        
        [Field] 
        public bool IsDebug { get; set; } = false;
        
        [Field] 
        public bool IsFlat { get; set; } = false;
        
        public JoinGame(Networking.Client client)
        {
            Client = client;

            EntityId = Client.Player.EntityId;
            GameMode = (sbyte)Client.Player.GameMode;
            WorldNames = Globals.WorldManager.GetWorlds().Select(x => x.Name).ToArray();

            DimensionCodec = new NbtCompound("")
            {
                new NbtCompound("minecraft:dimension_type")
                {
                    new NbtList("value", Globals.Registry.DimensionRegistry.Select(x => x.Serialize())),
                    new NbtString("type", "minecraft:dimension_type")
                },
                new NbtCompound("minecraft:worldgen/biome")
                {
                    new NbtList("value", Globals.Registry.BiomeRegistry.Select(x => x.Serialize())),
                    new NbtString("type", "minecraft:worldgen/biome")
                },
            };
            
            WorldName = WorldNames[0];
            Dimension = (NbtCompound)Globals.Registry.DimensionRegistry[0].Element.Serialize("");
            MaxPlayers = Config.MaxPlayers;
        }
    }
}