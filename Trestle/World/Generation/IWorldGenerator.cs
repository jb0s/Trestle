using Trestle.Utils;

namespace Trestle.World.Generation
{
    public interface IWorldGenerator
    {
        ChunkColumn GenerateChunkColumn(Vector2 coordinates);
        Location GetSpawnPoint();
        
        void Initialize();
    }
}