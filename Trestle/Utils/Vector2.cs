namespace Trestle.Utils
{
    public class Vector2
    {
        public Vector2(int x, int z)
        {
            X = x;
            Z = z;
        }

        public int X { get; set; }
        public int Z { get; set; }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new(a.X - b.X, a.Z - b.Z);
        }
        
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Z == b.Z;
        }
        
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a.X == b.X && a.Z == b.Z);
        }

        public string ToString()
        {
            return $"({X}, {Z})";
        }
    }
}