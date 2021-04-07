using System;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Entity
{
    public class ItemEntity : Entity
    {
        public ItemStack Item { get; private set; }
        public int PickupDelay { get; set; }
        public int TimeToLive { get; set; }

        public ItemEntity(World.World world, ItemStack item) : base(2, world)
        {
            Item = item;

            PickupDelay = 10;
            TimeToLive = 6000;

            Metadata = new ItemMetadata()
            {
                Item = new Slot(item)
            };
        }

        public override void SpawnEntity()
        {
            base.SpawnEntity();

            foreach (var player in World.Players.Values)
            {
                player.Client.SendPacket(new SpawnObject(this, Location, 2, 1));
                player.Client.SendPacket(new EntityMetadata(this));
            }
        }

        private void DespawnEntity(Player collector)
        {
            foreach (var player in World.Players.Values)
                player.Client.SendPacket(new CollectItem(this, collector));

            // Actually kill the entity now
            DespawnEntity();
        }
        
        public override void OnTick()
        {
            if (PickupDelay != 0)
                PickupDelay--;
            
            foreach (var player in World.Players.Values)
            {
                if (player.Location.DistanceTo(Location) <= 1.8 && PickupDelay == 0)
                {
                    // Add the item to the player's inventory
                    player.Inventory.AddItem(Item.ItemId, Item.Metadata, Item.ItemCount);
                    
                    // Send the pickup animation packets.
                    // The DespawnEntity is actually overridden to send the "item floating to player" animation before despawning.
                    player.Client.SendPacket(new NamedSoundEffect("entity.item.pickup", SoundCategory.Player, player.Location.ToVector3(), 1f, (byte)Globals.Random.Next(40, 100)));
                    DespawnEntity(player);
                    break;
                }
            }
        }
    }
}