using Trestle.Attributes;
using Trestle.Utils;

namespace Trestle.Entity
{
    public class ItemMetadata : Metadata
    {
        [Field] 
        [Index(6)] 
        public Slot Item => new(((ItemEntity)Entity).Item);
        
        public ItemMetadata(ItemEntity entity) : base(entity)
        {
            
        }
    }
}