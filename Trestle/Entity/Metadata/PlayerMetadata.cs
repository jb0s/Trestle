using Trestle.Attributes;

namespace Trestle.Entity
{
    public class PlayerMetadata : LivingMetadata
    {
        [Field] 
        [Index(13)] 
        public byte SkinMask { get; set; } = 0x00;
    }
}