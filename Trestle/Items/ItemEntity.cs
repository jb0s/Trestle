using Trestle.Utils;
using Trestle.Worlds;

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

            Height = 0.25;
            Width = 0.25;
            Length = 0.25;

            PickupDelay = 10;
            TimeToLive = 6000;
        }

        private void DespawnEntity(Player source)
        {
            TickTimer.Stop();

            foreach (var i in World.OnlinePlayerArray)
            {
                var spawnedBy = i.Client;
                if (source != null)
                {
                    /*
                    new CollectItem(spawnedBy)
                    {
                        CollectorEntityId = source.EntityId,
                        EntityId = EntityId
                    }.Write();*/
                }

                /*
                new DestroyEntities(spawnedBy)
                {
                    EntityIds = new[] {EntityId}
                }.Write();*/
            }
            World.RemoveEntity(this);
        }

        public override void SpawnEntity()
        {
            World.AddEntity(this);
            foreach (var i in World.OnlinePlayerArray)
            {
                /*
                var spawnedBy = i.Wrapper;
                new SpawnObject(spawnedBy)
                {
                    EntityId = EntityId,
                    X = Location.X,
                    Y = Location.Y,
                    Z = Location.Z,
                    Type = ObjectType.ItemStack
                }.Write();

                new EntityMetadata(spawnedBy)
                {
                    EntityId = EntityId,
                    Type = ObjectType.ItemStack,
                    Data = Item
                }.Write();*/
            }
        }

        public override void OnTick()
        {
            TimeToLive--;

            if (TimeToLive <= 0)
            {
                DespawnEntity(null);
                return;
            }

            var players = World.OnlinePlayerArray;
            foreach (var player in players)
            {
                if (Location.DistanceTo(player.Location) <= 1.8)
                {
                    player.Inventory.AddItem(Item.ItemId, Item.Metadata);

                    DespawnEntity(player);
                    break;
                }
                /*
                new EntityTeleport(player.Wrapper)
                {
                    EntityId = EntityId,
                    Coordinates = Location.ToVector3(),
                    Yaw = (byte) Location.Yaw,
                    Pitch = (byte) Location.Pitch,
                    OnGround = true
                }.Write();*/
            }
        }
    }
}