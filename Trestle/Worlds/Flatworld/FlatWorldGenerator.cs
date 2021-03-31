using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds.Flatworld
{
    public class FlatWorldGenerator : IWorldGenerator
    {
        public void Initialize()
        {
        }

        public ChunkColumn GenerateChunk(ChunkLocation location)
        {
            var chunk = new ChunkColumn {X = location.X, Z = location.Z};
            PopulateChunk(chunk);

            return chunk;
        }

        public Location GetSpawnPoint()
            => new(0, 1, 0);
        
        public int PopulateChunk(ChunkColumn chunk)
        {
            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = 0; y < 4; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(x, y, z, new Block(Material.Bedrock));
                        }
                        if (y == 1 || y == 2)
                        {
                            chunk.SetBlock(x, y, z, new Block(Material.Dirt));
                        }
                        if (y == 3)
                        {
                            chunk.SetBlock(x, y, z, new Block(Material.Grass));
                        }
                    }
                }
            }

            return 4;
        }
    }
}