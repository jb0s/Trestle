using System;

namespace Trestle.Utils
{
    public struct Vector2
    {
        public int X, Z;

        public Vector2(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
            => new(a.X - b.X, a.Z - b.Z);
        
        public static bool operator ==(Vector2 a, Vector2 b)
            => a.X == b.X && a.Z == b.Z;
        
        public static bool operator !=(Vector2 a, Vector2 b)
            => !(a.X == b.X && a.Z == b.Z);

        public string ToString()
            => $"({X}, {Z})";

        public static Vector2 ToChunkLocation(Location location)
            => new((int)Math.Floor(location.X) >> 4, (int)Math.Floor(location.Z) >> 4);
    }
}