using Trestle.Utils;

namespace Trestle.Worlds.Generation
{
    public interface IWorldGenerator
    {
        void Initialize();
        
        ChunkColumn GenerateChunk(ChunkLocation location);
        
        Location GetSpawnPoint();
    }
}