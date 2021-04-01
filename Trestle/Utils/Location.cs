using System;

namespace Trestle.Utils
{
    public class Location
    {
        public Location(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Location(Location location)
        {
            X = location.X;
            Y = location.Y;
            Z = location.Z;
            HeadYaw = location.HeadYaw;
            Yaw = location.Yaw;
            Pitch = location.Pitch;
            OnGround = location.OnGround;
        }
        
        public byte HeadYaw { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public double DistanceTo(Location other)
        {
            return Math.Sqrt(Square(other.X - X) +
                             Square(other.Y - Y) +
                             Square(other.Z - Z));
        }

        private double Square(double num)
        {
            return num*num;
        }

        public string GetString()
        {
            return "X: " + X + ", Y: " + Y + ", Z: " + Z + ", Yaw: " + Yaw + ", Pitch: " + Pitch;
        }

        public Location Clone()
        {
            return new Location(X, Y, Z) {Yaw = Yaw, Pitch = Pitch, OnGround = OnGround};
        }
    }
}