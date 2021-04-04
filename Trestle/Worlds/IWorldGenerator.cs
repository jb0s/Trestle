using Trestle.Utils;

namespace Trestle.Worlds
{
    public interface IWorldGenerator
    {
        void Initialize();
        
        ChunkColumn GenerateChunk(ChunkLocation location);

        Location GetSpawnPoint()
            => null;
    }
}