using System;
using System.Linq;
using Trestle.Entities.Players;
using Trestle.Nbt.Tags;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;
using Trestle.Networking;
using Trestle.Worlds.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.JoinGame)]
    public class JoinGame : Packet
    {
        [Field] 
        public int EntityId { get; set; }

        [Field]
        public bool IsHardcore { get; set; } = false;
        
        [Field(typeof(byte))] 
        public GameMode GameMode { get; set; } = GameMode.Survival;

        [Field(typeof(byte))] 
        public GameMode PreviousGameMode { get; set; } = GameMode.Creative;

        [Field]
        public string[] WorldNames { get; set; }
        
        [Field]
        public NbtCompound DimensionCodec { get; set; }
        
        [Field]
        public NbtCompound Dimension { get; set; }

        [Field] 
        public string WorldName { get; set; } = "minecraft:overworld";
        
        [Field]
        public long HashedSeed { get; set; }

        [Field] 
        [VarInt] 
        public int MaxPlayers { get; set; } = 20;

        [Field] 
        [VarInt] 
        public int ViewDistance { get; set; } = 16;

        [Field]
        public bool ShowReducedDebugInfo { get; set; } = false;

        [Field]
        public bool EnableRespawnScreen { get; set; } = true;

        [Field]
        public bool IsDebug { get; set; } = false;

        [Field] 
        public bool IsFlat { get; set; } = true;

        public JoinGame(Player player)
        {
            EntityId = player.Id;
            GameMode = GameMode.Survival;

            WorldNames = new [] {"minecraft:overworld"};

            HashedSeed = 0;

            DimensionCodec = new NbtCompound("")
            {
                new NbtCompound("minecraft:dimension_type")
                {
                    new NbtString("type", "minecraft:dimension_type"),
                    new NbtList("value")
                    {
                        new NbtCompound()
                        {
                            new NbtString("name", "minecraft:overworld"),
                            new NbtInt("id", 0),
                            new NbtCompound("element")
                            {
                                new NbtByte("piglin_safe", 0),
                                new NbtByte("natural", 1),
                                new NbtFloat("ambient_light", 1),
                                new NbtString("infiniburn", "minecraft:infiniburn_overworld"),
                                new NbtByte("respawn_anchor_works", 1),
                                new NbtByte("has_skylight", 1),
                                new NbtByte("bed_works", 1),
                                new NbtString("effects", "minecraft:overworld"),
                                new NbtByte("has_raids", 1),
                                new NbtInt("logical_height", 256),
                                new NbtDouble("coordinate_scale", 1),
                                new NbtByte("ultrawarm", 0),
                                new NbtByte("has_ceiling", 0),
                            }
                        }
                    }
                },
                new NbtCompound("minecraft:worldgen/biome")
                {
                    new NbtString("type", "minecraft:worldgen/biome"),
                    new NbtList("value")
                    {
                        new NbtCompound()
                        {
                            new NbtString("name", "minecraft:plains"),
                            new NbtInt("id", 0),
                            new NbtCompound("element")
                            {
                                new NbtString("precipitation", "rain"),
                                new NbtFloat("depth", 1),
                                new NbtFloat("temperature", 1),
                                new NbtFloat("scale", 0),
                                new NbtFloat("downfall", 0),
                                new NbtString("category", "plains"),
                                new NbtCompound("effects")
                                {
                                    new NbtInt("sky_color", 8364543),
                                    new NbtInt("water_fog_color", 8364543),
                                    new NbtInt("fog_color", 8364543),
                                    new NbtInt("water_color", 8364543),
                                    new NbtCompound("mood_sound")
                                    {
                                        new NbtInt("tick_delay", 6000),
                                        new NbtDouble("offset", 2),
                                        new NbtString("sound", "minecraft:ambient.cave"),
                                        new NbtInt("block_search_extent", 8)
                                    }
                                }
                            }
                        }
                    }
                }
            };
            
            Dimension = new NbtCompound("")
            {
                new NbtByte("piglin_safe", 0),
                new NbtByte("natural", 1),
                new NbtFloat("ambient_light", 1),
                new NbtString("infiniburn", "minecraft:infiniburn_overworld"),
                new NbtByte("respawn_anchor_works", 1),
                new NbtByte("has_skylight", 1),
                new NbtByte("bed_works", 1),
                new NbtString("effects", "minecraft:overworld"),
                new NbtByte("has_raids", 1),
                new NbtInt("logical_height", 256),
                new NbtDouble("coordinate_scale", 1),
                new NbtByte("ultrawarm", 0),
                new NbtByte("has_ceiling", 0),
            };
        }
    }
}