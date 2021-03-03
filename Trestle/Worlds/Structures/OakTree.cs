using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds.Structures
{
    public class OakTree : Structure
    {
        public override string Name => "OakTree";
        public override int Height => 10;

        public override Block[] Blocks
        {
            get
            {
                return new[]
                {
                    // Logs
                    new Block(Material.Log) { Coordinates = new Vector3(0, 1, 0) },
                    new Block(Material.Log) { Coordinates = new Vector3(0, 2, 0) },
                    new Block(Material.Log) { Coordinates = new Vector3(0, 3, 0) },
                    new Block(Material.Log) { Coordinates = new Vector3(0, 4, 0) },
                    new Block(Material.Log) { Coordinates = new Vector3(0, 5, 0) },

                    // Outer leaves
                    new Block(Material.Leaves) { Coordinates = new Vector3(-2, 3, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-2, 4, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-2, 3, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-2, 4, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-2, 3, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-2, 4, -1) },
                    
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 3, -2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 4, -2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 3, -2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 4, -2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 3, -2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 4, -2) },
                    
                    new Block(Material.Leaves) { Coordinates = new Vector3(2, 3, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(2, 4, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(2, 3, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(2, 4, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(2, 3, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(2, 4, 1) },
                    
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 3, 2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 4, 2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 3, 2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 4, 2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 3, 2) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 4, 2) },
                    
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 5, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 5, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 5, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 5, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 5, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 5, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 5, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 5, 0) },
                    
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 6, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 6, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 6, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 6, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 6, 1) },
                    
                    // Leaves inside
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 4, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 4, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 4, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 4, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 4, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 4, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 4, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 4, 0) },
                    
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 3, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 3, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 3, 1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 3, 0) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(1, 3, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(0, 3, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 3, -1) },
                    new Block(Material.Leaves) { Coordinates = new Vector3(-1, 3, 0) }
                };
            }
        }
    }
}