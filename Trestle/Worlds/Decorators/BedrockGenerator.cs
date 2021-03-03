using System;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Worlds;
using Trestle.Worlds.Standard;

namespace Trestle.Worlds.Decorators
{
    public class BedrockDecorator : Decorator
    {
        public override void Decorate(Chunk chunk, Biome biome, int x, int z)
        {
            if (chunk.ForcedBlocks != null && chunk.ForcedBlocks.Count > 0)
            {
                for(int i = 0; i < chunk.ForcedBlocks.Count; i++)
                {
                    var structure = chunk.ForcedBlocks[i];
                    //chunk.SetBlock((int)structure.Coordinates.X, (int)structure.Coordinates.Y, (int)structure.Coordinates.Z, new Block(structure.Block));
                    
                    chunk.ForcedBlocks.RemoveAt(i);
                    chunk.ForcedBlocksHandled++;
                    
                    Console.WriteLine($"Forcing block in chunk at {chunk.X} {chunk.Z}");
                }
            }
            
            for (var y = 1; y < 6; y++)
                if (StandardWorldGenerator.GetRandomNumber(0, 5) == 1)
                    chunk.SetBlock(x, y, z, new Block(Material.Bedrock));
        }
    }
}