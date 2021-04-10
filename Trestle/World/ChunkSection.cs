using System;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.World
{
    /// <summary>
    /// A 16x16x16 cube contained in a <see cref="ChunkColumn"/>.
    /// </summary>
    public class ChunkSection
    {
        /// <summary>
        /// The width of the chunk section.
        /// </summary>
        public const int WIDTH = 16;
        
        /// <summary>
        /// The depth of the chunk section.
        /// </summary>
        public const int DEPTH = 16;
        
        /// <summary>
        /// The height of the chunk section.
        /// </summary>
        public const int HEIGHT = 16;
        
        /// <summary>
        /// The maximum capacity of blocks the section can hold.
        /// </summary>
        public const int CAPACITY = WIDTH * DEPTH * HEIGHT;

        /// <summary>
        /// A <see cref="CompactedDataArray"/> containing every block id present in the chunk section.
        /// </summary>
        public CompactedDataArray Types;
        
        /// <summary>
        /// A <see cref="NibbleArray"/> containing sky light info for every block in the chunk section.
        /// </summary>
        public NibbleArray SkyLight;
        
        /// <summary>
        /// A <see cref="NibbleArray"/> containing block light info for every block in the chunk section.
        /// </summary>
        public NibbleArray BlockLight;

        /// <summary>
        /// Is the chunk completely empty?
        /// </summary>
        public bool IsEmpty => _airBlocks == CAPACITY;
        
        private int _airBlocks;
        
        public ChunkSection()
        {
            SkyLight = new NibbleArray(CAPACITY);
            BlockLight = new NibbleArray(CAPACITY);

            Types = new CompactedDataArray(13, CAPACITY);

            for(int i = 0; i < CAPACITY; i++)
            {
                Types[i] = 0;
                SkyLight[i] = 0xff;
            }

            _airBlocks = CAPACITY;
        }
        
        public ChunkSection(byte[] data)
        {
            using (var stream = new MinecraftStream(data))
            {
                SkyLight = new NibbleArray(CAPACITY);
                BlockLight = new NibbleArray(CAPACITY);

                int bitsPerBlock = stream.ReadByte();
                int paletteLength = stream.ReadVarInt();
                Types = new CompactedDataArray(13, CAPACITY);
                long[] blocks = Types.Backing;

                int blockLength = stream.ReadVarInt();
                for(int i = 0; i < blocks.Length; i++)
                {
                    blocks[i] = stream.ReadLong();
                }

                BlockLight.Data = stream.Read(2048);
                SkyLight.Data = stream.Read(2048);

                RefreshAirBlockCount();
            }
        }

        #region Utilities

        /// <summary>
        /// Refresh the Air block counter.
        /// Results from <see cref="ChunkSection"/>.<see cref="IsEmpty"/> can be inaccurate if this is not regularly called.
        /// </summary>
        public void RefreshAirBlockCount()
        {
            int airCount = 0;
            
            for(int i = 0; i < CAPACITY; i++)
                if (Types[i] >> 4 == 0)
                    airCount++;

            _airBlocks = airCount;
        }

        /// <summary>
        /// Exports the <see cref="ChunkSection"/> into a <see cref="byte[]"/> array.
        /// </summary>
        public byte[] Export()
        {
            using (var stream = new MinecraftStream())
            {
                long[] blocks = Types.Backing;
                
                stream.WriteByte(13); // Bits per block
                stream.WriteVarInt(0); // Palette length

                stream.WriteVarInt(blocks.Length); // Amount of blocks
                
                for(int i = 0; i < blocks.Length; i++)
                    stream.WriteLong(blocks[i]); // Block data
                
                stream.Write(BlockLight.Data);
                stream.Write(SkyLight.Data);
                
                return stream.Data;
            }
        }
        
        #endregion

        #region Block updating

        /// <summary>
        /// Gets the <see cref="Material"/> of the block at `coordinates`.
        /// </summary>
        /// <param name="coordinates">The coordinates of the block to retrieve.</param>
        public Material GetBlock(Vector3 coordinates)
            => (Material)(Types[GetIndex(coordinates)] >> 4);

        /// <summary>
        /// Gets the block metadata of the block at `coordinates`.
        /// </summary>
        /// <param name="coordinates">The coordinates of the block to retrieve.</param>
        public byte GetBlockData(Vector3 coordinates)
            => (byte)(Types[GetIndex(coordinates)] & 15);
        
        /// <summary>
        /// Sets the <see cref="Material"/> of the block at `coordinates` to `material`.
        /// </summary>
        /// <param name="coordinates">The coordinates of the block to replace.</param>
        /// <param name="material">The material the block should become.</param>
        public void SetBlock(Vector3 coordinates, Material material)
        {
            int index = GetIndex(coordinates);
            int data = Types[index];
            int type = data >> 4;
            int metadata = data & 15;

            if (type == 0 && material != Material.Air)
                _airBlocks--;
            else if (type > 0 && material == Material.Air)
                _airBlocks++;

            Types[index] = (int)material << 4 | (metadata & 15);
        }

        /// <summary>
        /// Sets the block metadata of the block at `coordinates` to `metadata`.
        /// </summary>
        /// <param name="coordinates">The coordinates of the block you want to update.</param>
        /// <param name="metadata">The new metadata you want to apply to the block.</param>
        public void SetBlockData(Vector3 coordinates, byte metadata)
        {
            int index = GetIndex(coordinates);
            int data = Types[index];
            int type = data >> 4;

            Types[index] = type << 4 | (metadata & 15);
        }
        
        #endregion

        #region Private methods

        /// <summary>
        /// Gets the index for a block coordinates for use with the <see cref="ChunkSection"/>.<see cref="Types"/> array.
        /// </summary>
        /// <param name="coordinates">The coordinates of the block of which index to retrieve.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static int GetIndex(Vector3 coordinates)
        {
            if (coordinates.X < 0 || coordinates.Y < 0 || coordinates.Z < 0 || coordinates.X >= WIDTH || coordinates.Y >= HEIGHT || coordinates.Z >= DEPTH)
                throw new ArgumentOutOfRangeException("Coordinates are out of specified range (0 - 15)");

            return ((int)coordinates.Y << 8) | ((int)coordinates.Z << 4) | (int)coordinates.X;
        }

        #endregion
    }
}