using Trestle.Entity;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Worlds
{
    public class Block : Item
    {
        public Vector3 Coordinates { get; set; }
        public bool IsReplacible { get; set; }
        public bool IsSolid { get; set; }
        public bool IsTransparent { get; set; }
        public int Durability { get; set; }
        public ItemStack[] Drops { get; set; }
        public bool IsBuildable { get; set; }

        internal Block(ushort id) : base(id, 0)
            => Initialize(id, 0);
        
        public Block() {} 

        public Block(Material material) : base((ushort)material, 0)
            => Initialize((ushort)material, 0);

        public Block(Item item) : base(item.Id, item.Metadata)
            => Initialize(item.Id, item.Metadata);
        
        public bool CanPlace(World world)
            => CanPlace(world, Coordinates);
        
        public void Initialize(ushort id, byte metadata)
        {
            Id = id;
            Durability = 100;
            Metadata = 0;
            Drops = new[] { new ItemStack(this, 1) };

            IsSolid = true;
            IsBuildable = true;
            IsReplacible = false;
            IsTransparent = false;

            FuelEfficiency = 0;
            IsBlock = true;
        }

        public void DoDrop(World world)
        {
            if (Drops == null) 
                return;
            
            foreach (var its in Drops)
            {
                new ItemEntity(world, its)
                {
                    Location = new Location(Coordinates.X, Coordinates.Y + 0.25, Coordinates.Z)
                }.SpawnEntity();
            }
        }

        protected virtual bool CanPlace(World world, Vector3 blockCoordinates)
            => world.GetBlock(blockCoordinates).IsReplacible;
        
        public virtual void BreakBlock(World world)
            => world.SetBlock(new Block(Material.Air) {Coordinates = Coordinates});

        public virtual bool PlaceBlock(World world, Player player, Vector3 blockCoordinates, BlockFace face, Vector3 mouseLocation)
            => false;

        public float GetHardness()
            => Durability / 5.0F;

        public virtual void OnTick(World world)
        {
        }

        public virtual void DoPhysics(World world)
        {
        }

        public Hitbox GetHitbox()
            => new(
                new Vector3(Coordinates.X, Coordinates.Y, Coordinates.Z),
                new Vector3(Coordinates.X + 1, Coordinates.Y + 1, Coordinates.Z + 1));
    }
}