using Trestle.Levels.Attributes;
using Trestle.Levels.Enums;

namespace Trestle.Levels.Biomes
{
    [Biome(BiomeType.Plains)]
    public class Plains : Biome
    {
        public override string Precipitation => "rain";

        public override float Depth => 0.125f;
        
        public override float Temperature => 0.800000011920929f;

        public override float Scale => 0.05000000074505806f;

        public override float Downfall => 0.4000000059604645f;

        public override string Category => "plains";

        public override BiomeEffects Effects => new (7907327, 329011, 12638463, 4159204);
    }
}