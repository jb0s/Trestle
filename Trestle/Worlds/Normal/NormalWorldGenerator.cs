using System;
using System.Text.RegularExpressions;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds.Normal
{
    public class NormalWorldGenerator : WorldGenerator
    {
        private const double OverhangOffset = 32.0; //Old value: 32.0 || Changes the offset from the bottom ground.
        private const double BottomOffset = 42.0; //Old value: 96.0  || Changes the offset from y level 0
        private const double OverhangsMagnitude = 16.0; //Old value: 16.0
        private const double BottomsMagnitude = 32.0; //Old value: 32.0
        private const double OverhangScale = 128.0; //Old value: 128.0 || Changes the scale of the overhang.
        private const double Groundscale = 256.0; //Old value: 256.0   || Changes the scale of the ground.
        private const double Threshold = 0.1; //Old value: 0.0 || Cool value: -0.3 hehehe
        private const double BottomsFrequency = 0.5; //Original 0.5
        private const double BottomsAmplitude = 0.5; //Original 0.5
        private const double OverhangFrequency = 0.5; //Original 0.5
        private const double OverhangAmplitude = 0.5; //Original 0.5
        private const bool EnableOverhang = true; //Enable overhang?
        
        public override Location GetSpawnPoint()
            => new(0, 40, 0);

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
            
            var bottom = new SimplexOctaveGenerator("sex".GetHashCode(), 8);
			var overhang = new SimplexOctaveGenerator("sex".GetHashCode(), 8);
			overhang.SetScale(1/OverhangScale);
			bottom.SetScale(1/Groundscale);

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + column.Coordinates.X * 16;
                    float oz = z + column.Coordinates.Z * 16;

					var bottomHeight =
						(int)
							((bottom.Noise(ox, oz, BottomsFrequency, BottomsAmplitude)*BottomsMagnitude) + BottomOffset + 52);

					var maxHeight =
						(int)
							((overhang.Noise(ox, oz, OverhangFrequency, OverhangAmplitude)*OverhangsMagnitude) + bottomHeight +
							 OverhangOffset);
					maxHeight = Math.Max(1, maxHeight);

					for (var y = 0; y < maxHeight && y < 256; y++)
					{
						if (y == 0)
						{
							column.SetBlock(new Vector3(x, y, z), Material.Bedrock);
							continue;
						}

						if (y > bottomHeight)
						{
							//part where we do the overhangs
							if (EnableOverhang)
							{
								var density = overhang.Noise(ox, y, oz, OverhangFrequency, OverhangAmplitude);
								if (density > Threshold) column.SetBlock(new Vector3(x, y, z), Material.Stone);
							}
						}
						else
						{
                            column.SetBlock(new Vector3(x, y, z), Material.Stone);
						}
					}

					//Turn the blocks ontop into the correct material
					for (var y = 0; y < 255; y++)
					{
						if (column.GetBlock(new Vector3(x, y + 1, z)) == 0 && column.GetBlock(new Vector3(x, y, z)) == Material.Stone)
						{
                            column.SetBlock(new Vector3(x, y, z), Material.Grass);

                            column.SetBlock(new Vector3(x, y - 1, z), Material.Dirt);
                            column.SetBlock(new Vector3(x, y - 2, z), Material.Dirt);
						}
					}
                }
			}

            return column;
        }
    }
}