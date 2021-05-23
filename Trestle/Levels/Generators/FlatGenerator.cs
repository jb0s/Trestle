using Trestle.Levels.Chunks;
using Trestle.Levels.Enums;
using Trestle.Utils;

namespace Trestle.Levels.Generators
{
    public class FlatGenerator : Generator
    {
        public override BiomeType[] Biomes => new[] {BiomeType.Plains};

        public override Chunk CreateChunk(Vector2 coordinates)
        {
            if (Chunks.ContainsKey(coordinates))
                return Chunks[coordinates];
            
            var chunk = new Chunk(coordinates);
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < Chunk.WIDTH; x++)
                {
                    for (var z = 0; z < Chunk.DEPTH; z++)
                    {
                        
                    }
                }
            }

            Chunks[coordinates] = chunk;
            return chunk;
        }
    }
}