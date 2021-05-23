using Trestle.Nbt.Tags;

namespace Trestle.Levels.Biomes
{
    public class Biome
    {
        public virtual string Precipitation { get; }
        
        public virtual float Depth { get; }
        
        public virtual float Temperature { get; }
        
        public virtual float Scale { get; }
        
        public virtual float Downfall { get; }
        
        public virtual string Category { get; }
        
        public virtual BiomeEffects Effects { get; }

        public NbtCompound GetNbt()
            => new()
            {
                new NbtString("precipitation", Precipitation),
                new NbtFloat("depth", Depth),
                new NbtFloat("temperature", Temperature),
                new NbtFloat("scale", Scale),
                new NbtFloat("downfall", Downfall),
                new NbtString("category", Category),
                new NbtCompound("effects")
                {
                    new NbtInt("sky_color", Effects.SkyColor),
                    new NbtInt("water_fog_color", Effects.WaterFogColor),
                    new NbtInt("fog_color", Effects.FogColor),
                    new NbtInt("foilage_color", Effects.FoilageColor),
                }
            };
    }

    public class BiomeEffects
    {
        public int SkyColor { get; }
        
        public int WaterFogColor { get; }
        
        public int FogColor { get; }
        
        public int FoilageColor { get; }
        
        public BiomeEffects(int skyColor, int waterFogColor, int fogColor, int foilageColor)
        {
            SkyColor = skyColor;
            WaterFogColor = waterFogColor;
            FogColor = fogColor;
            FoilageColor = foilageColor;
        }

    }
}