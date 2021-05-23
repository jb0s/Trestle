using Trestle.Levels.Attributes;
using Trestle.Levels.Enums;

namespace Trestle.Levels.Dimensions
{
    [Dimension(DimensionType.Overworld)]
    public class Overworld : Dimension
    {
        public override bool IsPiglinSafe => false;
        public override bool IsNatural => true;
        public override float AmbientLight => 0;
        public override string Infiniburn => "minecraft:infiniburn_overworld";
        public override bool DoesRespawnAnchorWork => false;
        public override bool HasSkylight => true;
        public override bool DoesBedWork => true;
        public override string Effects => "minecraft:overworld";
        public override bool HasRaids => true;
        public override int LogicalHeight => 256;
        public override float CoordinateScale => 1;
        public override bool IsUltrawarm => false;
        public override bool HasCeiling => false;
    }
}