using System.Buffers.Text;

namespace Trestle.Worlds.TestWorld
{
    public class TestWorld : World
    {
        public TestWorld() : base("world", new TestWorldGenerator())
        {
        }
    }
}