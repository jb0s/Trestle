using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds.Normal
{
    public class NormalWorldGenerator : WorldGenerator
    {
        public override Location GetSpawnPoint()
            => new(0, 4, 0);

        public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
        {
            // If there's a saved chunk, return that instead.
            if (Chunks.ContainsKey(chunkCoordinates))
                return Chunks[chunkCoordinates];
            
            return CreateChunk(chunkCoordinates);
        }
        
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