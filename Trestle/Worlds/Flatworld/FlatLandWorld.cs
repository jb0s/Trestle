namespace Trestle.Worlds.Flatworld
{
    public class FlatLandWorld : World
    {
        public FlatLandWorld() : base("world", new FlatWorldGenerator())
        {
        }
    }
}