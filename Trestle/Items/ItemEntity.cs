using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Items
{
    public class ItemEntity : Entity.Entity
    {
        /// <summary>
        /// The item this entity defines as.
        /// </summary>
        public ItemStack Item { get; private set; }
        
        /// <summary>
        /// The amount of lifetime that has to pass until the item can be picked up.
        /// </summary>
        public int PickupDelay { get; set; }
        
        /// <summary>
        /// Ticks left until the entity despawns.
        /// </summary>
        public int TimeToLive { get; set; }

        public ItemEntity(Worlds.World world, ItemStack item) : base(EntityType.ItemStack, world)
        {
            Item = item;

            PickupDelay = 10;
            TimeToLive = 6000;

            Metadata = new ItemMetadata(this);
        }

        public override void SpawnEntity()
        {
            base.SpawnEntity();

            World.BroadcastPacket(new SpawnObject(this, Location, 2, 1));
            World.BroadcastPacket(new EntityMetadata(this));
        }

        private void DespawnEntity(Player collector)
        {
            World.BroadcastPacket(new CollectItem(this, collector));

            // Actually kill the entity now
            DespawnEntity();
        }
        
        public override void OnTick()
        {
            if (PickupDelay > 0)
                PickupDelay--;
            
            foreach (var player in World.Players.Values)
            {
                if (player.Location.DistanceTo(Location) <= 1.8 && PickupDelay <= 0 && !player.HealthManager.IsDead)
                {
                    // Add the item to the player's inventory
                    player.Inventory.AddItem(Item.ItemId, Item.ItemCount, Item.Metadata);
                    
                    // Send the pickup animation packets.
                    // The DespawnEntity is actually overridden to send the "item floating to player" animation before despawning.
                    DespawnEntity(player);
                    break;
                }
            }
        }
    }
}