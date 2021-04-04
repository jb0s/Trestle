using System;
using Trestle.Utils;
using System.Collections.Generic;

namespace Trestle.Entity
{
    public enum ContainmentType
    {
        Disjoint,
        Contains,
        Intersects
    }
    
    public class Hitbox
    {
        public Vector3 Max, Min;

        public Hitbox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public Hitbox(Hitbox hitbox)
        {
            Min = hitbox.Min;
            Max = hitbox.Max;
        }
        
        public double Height
            => Max.Y - Min.Y;

        public double Width
            => Max.X - Min.X;

        public double Depth
            => Max.Z - Min.Z;

        public bool Equals(Hitbox other)
            => (Min == other.Min) && (Max == other.Max);
        
        public ContainmentType Contains(Hitbox box)
        {
            // Test if all corner is on the same side of a face by just checking min and max.
            if (box.Max.X < Min.X
                || box.Min.X > Max.X
                || box.Max.Y < Min.Y
                || box.Min.Y > Max.Y
                || box.Max.Z < Min.Z
                || box.Min.Z > Max.Z)
                return ContainmentType.Disjoint;


            if (box.Min.X >= Min.X
                && box.Max.X <= Max.X
                && box.Min.Y >= Min.Y
                && box.Max.Y <= Max.Y
                && box.Min.Z >= Min.Z
                && box.Max.Z <= Max.Z)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }
        
        public bool Contains(Vector3 vec)
            => Min.X <= vec.X && vec.X <= Max.X &&
                   Min.Y <= vec.Y && vec.Y <= Max.Y &&
                   Min.Z <= vec.Z && vec.Z <= Max.Z;
        
        public static Hitbox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points), "You must provide points for the hitbox.");

            var empty = true;
            var vector2 = new Vector3(float.MaxValue);
            var vector1 = new Vector3(float.MinValue);
            foreach (var vector3 in points)
            {
                vector2 = Vector3.Min(vector2, vector3);
                vector1 = Vector3.Max(vector1, vector3);
                empty = false;
            }
            if (empty)
                throw new ArgumentException("The hitbox cannot be empty.");

            return new Hitbox(vector2, vector1);
        }
        
        public Hitbox OffsetBy(Vector3 offset)
            => new(Min + offset, Max + offset);

        public Vector3[] GetCorners()
        {
            return new[]
            {
                new Vector3(Min.X, Max.Y, Max.Z),
                new Vector3(Max.X, Max.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Min.X, Min.Y, Min.Z)
            };
        }
        
        public override bool Equals(object obj)
            => (obj is Hitbox hitbox) && Equals(hitbox);

        public override int GetHashCode()
            => Min.GetHashCode() + Max.GetHashCode();

        public bool Intersects(Hitbox box)
        {
            Intersects(ref box, out bool result);
            return result;
        }

        public void Intersects(ref Hitbox box, out bool result)
        {
            if ((Max.X >= box.Min.X) && (Min.X <= box.Max.X))
            {
                if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
                {
                    result = false;
                    return;
                }

                result = (Max.Z >= box.Min.Z) && (Min.Z <= box.Max.Z);
                return;
            }

            result = false;
        }

        public static Hitbox operator +(Hitbox a, double b)
            => new(a.Min - b, a.Max + b);

        public static bool operator ==(Hitbox a, Hitbox b)
            => a.Equals(b);

        public static bool operator !=(Hitbox a, Hitbox b)
            => !a.Equals(b);

        public override string ToString()
            => string.Format("{{Min:{0} Max:{1}}}", Min, Max);
    }
}