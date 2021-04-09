using Trestle.Attributes;
using Trestle.Items;
using Trestle.Utils;

namespace Trestle.Entity
{
    public class ItemMetadata : Metadata
    {
        [Field] 
        [Index(6)] 
        public ItemStack Item => ((ItemEntity)Entity).Item;
        
        public ItemMetadata(ItemEntity entity) : base(entity)
        {
            
        }
    }
}