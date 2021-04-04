using System;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds.StandardWorld
{
    public class StandardWorld : World
    {
        public StandardWorld() : base("world", new StandardWorldGenerator())
        {
            SpawnPoint = new Location(Globals.Random.Next(-500, 500), 0, Globals.Random.Next(-500, 500));
            SpawnPoint.X = 0;
            SpawnPoint.Z = 0;

            for(int y = 0; y < 256; y++)
            {
                if (GetBlock(new Vector3(SpawnPoint.X, y, SpawnPoint.Y)).Material == Material.Air)
                {
                    SpawnPoint.Y = y;
                    break;
                }
            }
        }
    }
}