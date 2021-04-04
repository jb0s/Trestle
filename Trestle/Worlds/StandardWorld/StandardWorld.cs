using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds.StandardWorld
{
    public class StandardWorld : World
    {
        public StandardWorld() : base("world", new StandardWorldGenerator())
        {
            SpawnPoint = new(Globals.Random.Next(-500, 500) + 0.5, 0, Globals.Random.Next(-500, 500) + 0.5);

            for(int y = 0; y < 256; y++)
            {
                if (GetBlock(new Vector3(SpawnPoint.X, y, SpawnPoint.Z)).Material == Material.Air)
                {
                    SpawnPoint.Y = y + 1;
                    break;
                }
            }
        }
    }
}