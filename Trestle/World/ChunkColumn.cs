using System;
using System.Collections.Generic;
using System.IO;
using fNbt;
using fNbt.Tags;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.World
{
    public class ChunkColumn
    {
        /// <summary>
        /// The width of the chunk column.
        /// </summary>
        public const int WIDTH = 16;
        
        /// <summary>
        /// The depth of the chunk column.
        /// </summary>
        public const int DEPTH = 16;
        
        /// <summary>
        /// The height of the chunk column.
        /// </summary>
        public const int HEIGHT = 256;

        public readonly Vector2 Coordinates;
        public readonly byte[] Heightmap;
        public bool IsDirty;
        
        private readonly Dictionary<Vector3, BlockEntity> _blockEntities;
        private readonly ChunkSection[] _sections;
        private readonly byte[] _biomes;
        
        private byte[] _cache;

        public ChunkColumn(Vector2 coordinates)
        {
            Coordinates = coordinates;
            Heightmap = new byte[WIDTH * DEPTH];

            _blockEntities = new Dictionary<Vector3, BlockEntity>();
            _sections = new ChunkSection[16];
            _biomes = new byte[WIDTH * DEPTH];

            for(int i = 0; i < _sections.Length; i++)
                _sections[i] = new ChunkSection();

            for(int i = 0; i < Heightmap.Length; i++)
                Heightmap[i] = 0;

            for(int i = 0; i < _biomes.Length; i++)
                _biomes[i] = 1;
        }

        public ChunkColumn(byte[] data)
        {
            try
            {
                using (var stream = new MinecraftStream(data))
                {
                    Coordinates = new Vector2(stream.ReadInt(), stream.ReadInt());
                    Heightmap = new byte[WIDTH * DEPTH];
                    _sections = new ChunkSection[16];
                    _biomes = new byte[WIDTH * DEPTH];

                    var isFullChunk = stream.ReadBool();

                    // Chunk Sections

                    var sectionBitmask = stream.ReadVarInt();
                    var sectionDataLength = stream.ReadVarInt();
                    var sectionData = stream.Read(sectionDataLength);

                    using (var sectionStream = new MinecraftStream(sectionData))
                    {
                        for (int i = 0; i < sectionBitmask; i++)
                            _sections[i] = new ChunkSection(sectionStream.Read(10756));

                        for (int i = sectionBitmask; i < _sections.Length; i++)
                            _sections[i] = new ChunkSection();

                        for (int i = 0; i < _biomes.Length; i++)
                            _biomes[i] = 1;
                    }

                    _blockEntities = new Dictionary<Vector3, BlockEntity>();
                    // Block Entities

                    var blockEntitiesLength = stream.ReadVarInt();
                    var blockEntityStream = new MemoryStream(stream.BufferedData);
                    blockEntityStream.Position = stream.Position;

                    _blockEntities = new Dictionary<Vector3, BlockEntity>(blockEntitiesLength);

                    for (var i = 0; i < blockEntitiesLength; i++)
                    {
                        var reader = new NbtReader(blockEntityStream);
                        var compound = (NbtCompound)reader.ReadAsTag();

                        var blockEntity = new BlockEntity(compound);
                        _blockEntities.Add(blockEntity.Position, blockEntity);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Utilities

        private ChunkSection GetSection(int y)
            => _sections[y >> 4];

        public void RefreshHeightmap()
        {
            for(int x = 0; x < WIDTH; x++)
                for(int z = 0; z < DEPTH; z++)
                    for(byte y = 127; y > 0; y--)
                        if (GetBlock(new Vector3(x, y, z)) != Material.Air)
                        {
                            Heightmap[(x << 4) + z] = (byte)(y + 1);
                            break;
                        }
        }

        public byte[] Export()
        {
            if (_cache != null)
                return _cache;

            byte[] sectionData;
            int sectionBitmask = 0;
            
            // Export the sections
            using (var stream = new MinecraftStream())
            {
                for(int i = 0; i < _sections.Length; i++)
                {
                    var section = _sections[i];
                    
                    if (section.IsEmpty)
                        continue;

                    sectionBitmask |= 1 << i;
                    
                    stream.Write(section.Export());
                }

                sectionData = stream.Data;
            }

            using (var stream = new MinecraftStream())
            {
                stream.WriteInt(Coordinates.X);
                stream.WriteInt(Coordinates.Z);
                
                stream.WriteBool(true); // Is full chunk
                
                stream.WriteVarInt(sectionBitmask); // Primary bitmask
                
                stream.WriteVarInt(sectionData.Length + 256);
                stream.Write(sectionData, 0, sectionData.Length);
                
                stream.Write(_biomes);
                
                stream.WriteVarInt(_blockEntities.Count);
                foreach(var (_, entity) in _blockEntities)
                    stream.Write(entity.Export());
                  
                _cache = stream.Data;
                return stream.Data;
            }
        }
        
        #endregion

        #region Blocks

        public Material GetBlock(Vector3 coordinates)
            => GetSection((int)coordinates.Y).GetBlock(new Vector3(coordinates.X, coordinates.Y - 16 * ((int)coordinates.Y >> 4), coordinates.Z));
        
        public byte GetBlockData(Vector3 coordinates)
            => GetSection((int)coordinates.Y).GetBlockData(new Vector3(coordinates.X, coordinates.Y - 16 * ((int)coordinates.Y >> 4), coordinates.Z));

        public void SetBlock(Vector3 coordinates, Material material)
        {
            GetSection((int)coordinates.Y).SetBlock(new Vector3(coordinates.X, coordinates.Y - 16 * ((int)coordinates.Y >> 4), coordinates.Z), material);

            IsDirty = true;
            _cache = null;
        }
        
        public void SetBlockData(Vector3 coordinates, byte metadata)
        {
            GetSection((int)coordinates.Y).SetBlockData(new Vector3(coordinates.X, coordinates.Y - 16 * ((int)coordinates.Y >> 4), coordinates.Z), metadata);

            IsDirty = true;
            _cache = null;
        }
        #endregion

        #region Block Entities

        public BlockEntity GetBlockEntity(Vector3 coordinates)
            => _blockEntities[coordinates];
        
        public BlockEntity SetBlockEntity(Vector3 coordinates, BlockEntity blockEntity)
            => _blockEntities[coordinates] = blockEntity;

        #endregion
    }
}