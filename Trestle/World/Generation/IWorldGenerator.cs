using System.Collections.Concurrent;
using System.IO;
using Trestle.Utils;

namespace Trestle.World.Generation
{
    public interface IWorldGenerator
    {
        public ConcurrentDictionary<Vector2, ChunkColumn> Chunks { get; }
        
        ChunkColumn GenerateChunkColumn(Vector2 coordinates);
        Location GetSpawnPoint();

        public ChunkColumn LoadChunkFromSave(Vector2 location, string fileLocation)
        {
            if (Chunks.ContainsKey(location))
                return Chunks[location];
            
            byte[] data = File.ReadAllBytes(fileLocation);

            var chunk = new ChunkColumn(data);
            Chunks.TryAdd(location, chunk);
            return chunk;
        }
        
        void Initialize();
    }
}