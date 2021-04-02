using System.Buffers.Text;
using Trestle.Enums;

namespace Trestle.Worlds.TestWorld
{
    public class StandardWorld : World
    {
        public StandardWorld() : base("world", new StandardWorldGenerator())
        {
        }
    }
}