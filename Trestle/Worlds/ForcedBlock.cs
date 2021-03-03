using Trestle.Enums;
using Trestle.Worlds.Structures;

namespace Trestle.Utils
{
    public class ForcedBlock
    {
        public Vector3 Coordinates;
        public Material Block;

        public ForcedBlock(Vector3 coordinates, Material block)
        {
            Coordinates = coordinates;
            Block = block;
        }
    }
}