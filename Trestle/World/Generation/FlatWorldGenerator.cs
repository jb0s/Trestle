using System.Collections.Concurrent;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.World.Generation
{
    public class FlatWorldGenerator : IWorldGenerator
    {
        public ConcurrentDictionary<Vector2, ChunkColumn> Chunks { get; }
			 
        public FlatWorldGenerator()
        {
            Chunks = new ConcurrentDictionary<Vector2, ChunkColumn>();
        }

        public ChunkColumn GenerateChunkColumn(Vector2 coordinates)
            => Chunks.GetOrAdd(coordinates, CreateChunk);

        public void Initialize()
        {
        }

        public Location GetSpawnPoint()
            => new(0.5, 4f, 0.5f);

        private static ChunkColumn CreateChunk(Vector2 chunkCoordinates)
        {
            ChunkColumn column = new ChunkColumn(chunkCoordinates);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < ChunkColumn.WIDTH; x++)
                {
                    for (int z = 0; z < ChunkColumn.DEPTH; z++)
                    {
                        if (y == 0) 
                            column.SetBlock(new Vector3(x, y, z), Material.Bedrock);
                        else if (y == 1 || y == 2) 
                            column.SetBlock(new Vector3(x, y, z), Material.Dirt);
                        else if (y == 3) 
                            column.SetBlock(new Vector3(x, y, z), Material.Grass);
                    }
                }
            }

            return column;
        }
    }
}