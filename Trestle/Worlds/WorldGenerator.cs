using System;
using System.Collections.Concurrent;
using System.IO;
using Trestle.Utils;

namespace Trestle.Worlds
{
    public class WorldGenerator
    {
        public ConcurrentDictionary<Vector2, ChunkColumn> Chunks = new();

        public virtual ChunkColumn GenerateChunkColumn(Vector2 coordinates)
            => throw new Exception("This function is not to be called, it needs to be overridden.");

        public virtual ChunkColumn CreateAndCache(Vector2 chunkCoordinates)
            => Chunks.GetOrAdd(chunkCoordinates, GenerateChunkColumn);

        public virtual Location GetSpawnPoint()
            => new(0, 0, 0);
        
        public virtual void Initialize() {}
        
        public ChunkColumn LoadChunkFromSave(Vector2 location, string fileLocation)
        {
            if (Chunks.ContainsKey(location))
                return Chunks[location];
            
            byte[] data = File.ReadAllBytes(fileLocation);

            var chunk = new ChunkColumn(data);
            Chunks.TryAdd(location, chunk);
            return chunk;
        }

    }
}