using Trestle.Enums;
using Trestle.Utils;
using Trestle.Worlds;
using Trestle.Entity;

namespace Trestle.Blocks
{
    /// <summary>
    /// An item that can be placed in the game world.
    /// </summary>
    public class Block : Item
    {
        /// <summary>
        /// The coordinates of the block.
        /// </summary>
        public Vector3 Coordinates { get; set; }

        /// <summary>
        /// Can another block be placed in place of this one?
        /// </summary>
        public bool IsReplacable { get; set; }

        /// <summary>
        /// Is this block solid or can the player walk through it?
        /// </summary>
        public bool IsSolid { get; set; }

        /// <summary>
        /// How durable is this block?
        /// (Affects mining speed.)
        /// </summary>
        public float Durability { get; set; }

        /// <summary>
        /// What does this block drop when broken?
        /// </summary>
        public ItemStack[] Drops { get; set; }

        internal Block(Material material) : this((ushort)material)
        {
        }

        internal Block(ushort id) : base(id, 0)
        {
            Id = id;
            Durability = 0.5f;
            Metadata = 0;
            Drops = new[] {new ItemStack(this, 1)};
			
            IsSolid = true;
            IsReplacable = false;

            IsBlock = true;
        }

        /// <summary>
        /// Drops block loot.
        /// </summary>
        /// <param name="world"></param>
        public void Drop(World world)
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
        
        /// <summary>
        /// Gets the hardness of the block.
        /// </summary>
        /// <returns></returns>
        public float GetHardness()
            => Durability / 5f;

        /// <summary>
        /// Called every tick when its chunk is loaded.
        /// </summary>
        /// <param name="world"></param>
        public virtual void OnTick(World world)
        {
        }

        /// <summary>
        /// Starts a physics simulation.
        /// </summary>
        /// <param name="world"></param>
        public virtual void DoPhysics(World world)
        {
        }

        /// <summary>
        /// Breaks the block. (Does NOT drop loot, see <see cref="Drop(World)."/>)
        /// </summary>
        /// <param name="playerWorld"></param>
        public void BreakBlock(World playerWorld)
            => playerWorld.SetBlock(Material.Air, Coordinates);
    }
}