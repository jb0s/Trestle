using Trestle.Utils;

namespace Trestle.Levels.Chunks
{
    public class Chunk
    {
        public const int WIDTH = 16;
        public const int HEIGHT = 256;
        public const int DEPTH = 16;
        
        public Vector2 Coordinates { get; }

        private readonly ChunkSection[] _sections = new ChunkSection[16];
        
        public Chunk(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }
    }
}