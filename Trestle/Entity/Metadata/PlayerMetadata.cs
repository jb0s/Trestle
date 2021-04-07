using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Entity
{
    public class PlayerMetadata : LivingMetadata
    {
        [Field(typeof(byte))] 
        [Index(13)] 
        public SkinParts SkinMask => ((Player)Entity).SkinParts;
        
        public PlayerMetadata(Player entity) : base(entity)
        {
            
        }
    }
}