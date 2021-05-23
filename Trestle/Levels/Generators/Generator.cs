using System;
using System.Collections.Concurrent;
using Trestle.Levels.Chunks;
using Trestle.Levels.Enums;
using Trestle.Utils;

namespace Trestle.Levels.Generators
{
    public class Generator
    {
        public virtual BiomeType[] Biomes { get; }
        
        public ConcurrentDictionary<Vector2, Chunk> Chunks = new();

        public virtual Chunk CreateChunk(Vector2 coordinates)
            => throw new NotSupportedException();
    }
}