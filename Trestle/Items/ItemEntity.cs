using Trestle.Utils;
using Trestle.Worlds;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Entity
{
    public class ItemEntity : Entity
    {
        public ItemStack Item { get; private set; }
        public int PickupDelay { get; set; }
        public int TimeToLive { get; set; }

        public ItemEntity(World world, ItemStack item) : base(2, world)
        {
            Item = item;

            PickupDelay = 10;
            TimeToLive = 6000;
        }

        public override void SpawnEntity()
        {
            base.SpawnEntity();

            foreach (var player in World.Players.Values)
            {
                var spawnedBy = player.Client;
                spawnedBy.SendPacket(new SpawnObject(EntityId, 2, (int)Location.X, (int)Location.Y, (int)Location.Z, 0, 0, 1));

                using (MinecraftStream stream = new MinecraftStream())
                {
                    stream.WriteByte((5 << 5 | 10 & 0x1F) & 0xFF);
                    stream.WriteShort((short) (Item.ItemId != 0 ? Item.ItemId : 1));
                    stream.WriteByte(1);
                    stream.WriteShort(Item.Metadata);
                    stream.WriteByte(0); // nbt tingz :sparkles:
                    stream.WriteByte(127);
                    
                    spawnedBy.SendPacket(new EntityMetadata(EntityId, stream.Data));
                }
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
            TimeToLive--;

            if (TimeToLive <= 0)
            {
                DespawnEntity();
                return;
            }
            
            foreach (var player in World.Players.Values)
            {
                if (player.Location.DistanceTo(Location) <= 1.8)
                {
                    PickupDelay--;

                    if (PickupDelay <= 0)
                    {
                        // Add the item to the player's inventory
                        player.Inventory.AddItem(Item.ItemId, Item.Metadata);
                    
                        // Send the pickup animation packets.
                        // The DespawnEntity is actually overridden to send the "item floating to player" animation before despawning.
                        player.Client.SendPacket(new SoundEffect("random.pop", player.Location.ToVector3(), 1f, (byte)Globals.Random.Next(40, 100)));
                        DespawnEntity(player);
                    }

                    break;
                }
                
                PickupDelay = 10;
            }
        }
    }
}