using Trestle.Utils;

namespace Trestle.Worlds
{
    public interface IWorldGenerator
    {
        public void Initialize();

        public Location GetSpawnPoint();
        
        public ChunkColumn GenerateChunk(ChunkLocation location);
    }
}