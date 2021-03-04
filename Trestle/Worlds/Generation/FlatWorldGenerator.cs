using Trestle.Utils;
using System.Collections.Concurrent;

namespace Trestle.Worlds.Generation
{
    public class FlatWorldGenerator : Worlds.IWorldGenerator
    {
        private ConcurrentDictionary<ChunkLocation, ChunkColumn> Chunks { get; }
			 
        public FlatWorldGenerator()
        {
            Chunks = new ConcurrentDictionary<ChunkLocation, ChunkColumn>();
        }

        public void Initialize()
        {
        }

        public Location GetSpawnPoint()
            => new(0.5, 4f, 0.5f);

        public ChunkColumn GenerateChunk(ChunkLocation location)
            => Chunks.GetOrAdd(location, CreateChunk);

        private static ChunkColumn CreateChunk(ChunkLocation chunkCoordinates)
        {
            ChunkColumn column = new(chunkCoordinates);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < ChunkColumn.WIDTH_DEPTH; x++)
                {
                    for (int z = 0; z < ChunkColumn.WIDTH_DEPTH; z++)
                    {
                        if (y == 0) column.SetBlockId(x, y, z, 7); //bedrock
                        else if (y == 1 || y == 2) column.SetBlockId(x, y, z, 3); //Dirt
                        else if (y == 3) column.SetBlockId(x, y, z, 2); //Grass
                    }
                }
            }

            column.SetBlockId(0, 5, 0, 41);
            column.SetBlockId(0, 5, 1, 41);
            column.SetBlockId(0, 5, 2, 7);

            return column;
        }
    }
}