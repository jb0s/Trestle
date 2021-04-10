using System;
using System.IO;
using LibNoise.Model;
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
        
        private ChunkSection[] _sections;
        private byte[] _cache;
        private byte[] _biomes;
        
        public ChunkColumn(Vector2 coordinates)
        {
            Coordinates = coordinates;
            Heightmap = new byte[WIDTH * DEPTH];
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
            using (var stream = new MinecraftStream(data))
            {
                Coordinates = new Vector2(stream.ReadInt(), stream.ReadInt());
                Heightmap = new byte[WIDTH * DEPTH];
                _sections = new ChunkSection[16];
                _biomes = new byte[WIDTH * DEPTH];
                
                bool isFullChunk = stream.ReadBool();
                int sectionBitmask = stream.ReadVarInt();
                int sectionDataLength = stream.ReadVarInt();
                byte[] sectionData = stream.Read(sectionDataLength);

                using (var sectionStream = new MinecraftStream(sectionData))
                {
                    for(int i = 0; i < sectionBitmask; i++)
                        _sections[i] = new ChunkSection(sectionStream.Read(10756));

                    for(int i = sectionBitmask; i < _sections.Length; i++)
                        _sections[i] = new ChunkSection();
                    
                    for(int i = 0; i < _biomes.Length; i++)
                        _biomes[i] = 1;
                }
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
                
                stream.WriteVarInt(0); // No block entities yet

                _cache = stream.Data;
                return stream.Data;
            }
        }
        
        #endregion

        #region Block updating

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
    }
}