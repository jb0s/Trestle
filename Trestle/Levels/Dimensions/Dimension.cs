using System;
using Trestle.Nbt.Tags;

namespace Trestle.Levels.Dimensions
{
    public class Dimension
    {
        public virtual int Id { get; }
        public virtual string Name { get; }
        public virtual bool IsPiglinSafe { get; } 
        public virtual bool IsNatural { get; } 
        public virtual float AmbientLight { get; } 
        public virtual string Infiniburn { get; } 
        public virtual bool DoesRespawnAnchorWork { get; } 
        public virtual bool HasSkylight { get; } 
        public virtual bool DoesBedWork { get; } 
        public virtual string Effects { get; } 
        public virtual bool HasRaids { get; } 
        public virtual int LogicalHeight { get; } 
        public virtual float CoordinateScale { get; } 
        public virtual bool IsUltrawarm { get; } 
        public virtual bool HasCeiling { get; }

        public NbtCompound GetNbt(string elementName = "")
            => new (elementName)
            {
                new NbtByte("piglin_safe", Convert.ToByte(IsPiglinSafe)),
                new NbtByte("natural", Convert.ToByte(IsNatural)),
                new NbtFloat("ambient_light", AmbientLight),
                new NbtString("infiniburn", Infiniburn),
                new NbtByte("respawn_anchor_works", Convert.ToByte(DoesRespawnAnchorWork)),
                new NbtByte("has_skylight", Convert.ToByte(HasSkylight)),
                new NbtByte("bed_works", Convert.ToByte(DoesBedWork)),
                new NbtString("effects", Effects),
                new NbtByte("has_raids", Convert.ToByte(HasRaids)),
                new NbtInt("logical_height", LogicalHeight),
                new NbtFloat("coordinate_scale", CoordinateScale),
                new NbtByte("ultrawarm", Convert.ToByte(IsUltrawarm)),
                new NbtByte("has_ceiling", Convert.ToByte(HasCeiling)),
            };
    }
}