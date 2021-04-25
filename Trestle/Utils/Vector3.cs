using System;

namespace Trestle.Utils
{
    public struct Vector3
    {
        public double X;
		public double Y;
		public double Z;

		public Vector3(double value)
		{
			X = Y = Z = value;
		}

		public Vector3(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		/// Finds the distance of this vector from Vector3.Zero
		/// </summary>
		public double Distance 
			=> DistanceTo(Constants.Vector3.Zero);

		public bool Equals(Vector3 other)
		{
			return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
		}

		/// <summary>
		/// Truncates the decimal component of each part of this Vector3.
		/// </summary>
		public Vector3 Floor()
		{
			return new Vector3(Math.Floor(X), Math.Floor(Y), Math.Floor(Z));
		}

		public Vector3 Normalize()
		{
			return new Vector3(X/Distance, Y/Distance, Z/Distance);
		}

		/// <summary>
		/// Calculates the distance between two Vector3 objects.
		/// </summary>
		public double DistanceTo(Vector3 other)
		{
			return Math.Sqrt(Square(other.X - X) +
			                 Square(other.Y - Y) +
			                 Square(other.Z - Z));
		}

		/// <summary>
		/// Calculates the square of a num.
		/// </summary>
		private double Square(double num)
		{
			return num*num;
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				Math.Min(value1.X, value2.X),
				Math.Min(value1.Y, value2.Y),
				Math.Min(value1.Z, value2.Z)
				);
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				Math.Max(value1.X, value2.X),
				Math.Max(value1.Y, value2.Y),
				Math.Max(value1.Z, value2.Z)
				);
		}

		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(Vector3 a, Vector3 b)
		{
			return a.Equals(b);
		}

		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X + b.X,
				a.Y + b.Y,
				a.Z + b.Z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X - b.X,
				a.Y - b.Y,
				a.Z - b.Z);
		}

		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(
				-a.X,
				-a.Y,
				-a.Z);
		}

		public static Vector3 operator *(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X*b.X,
				a.Y*b.Y,
				a.Z*b.Z);
		}

		public static Vector3 operator /(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X/b.X,
				a.Y/b.Y,
				a.Z/b.Z);
		}

		public static Vector3 operator %(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X%b.X, a.Y%b.Y, a.Z%b.Z);
		}

		public static Vector3 operator +(Vector3 a, double b)
		{
			return new Vector3(
				a.X + b,
				a.Y + b,
				a.Z + b);
		}

		public static Vector3 operator -(Vector3 a, double b)
		{
			return new Vector3(
				a.X - b,
				a.Y - b,
				a.Z - b);
		}

		public static Vector3 operator *(Vector3 a, double b)
		{
			return new Vector3(
				a.X*b,
				a.Y*b,
				a.Z*b);
		}

		public static Vector3 operator /(Vector3 a, double b)
		{
			return new Vector3(
				a.X/b,
				a.Y/b,
				a.Z/b);
		}

		public static Vector3 operator %(Vector3 a, double b)
		{
			return new Vector3(a.X%b, a.Y%b, a.Y%b);
		}

		public static Vector3 operator +(double a, Vector3 b)
		{
			return new Vector3(
				a + b.X,
				a + b.Y,
				a + b.Z);
		}

		public static Vector3 operator -(double a, Vector3 b)
		{
			return new Vector3(
				a - b.X,
				a - b.Y,
				a - b.Z);
		}

		public static Vector3 operator *(double a, Vector3 b)
		{
			return new Vector3(
				a*b.X,
				a*b.Y,
				a*b.Z);
		}

		public static Vector3 operator /(double a, Vector3 b)
		{
			return new Vector3(
				a/b.X,
				a/b.Y,
				a/b.Z);
		}

		public static Vector3 operator %(double a, Vector3 b)
		{
			return new Vector3(a%b.X, a%b.Y, a%b.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Vector3)) return false;
			return Equals((Vector3) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = X.GetHashCode();
				result = (result*397) ^ Y.GetHashCode();
				result = (result*397) ^ Z.GetHashCode();
				return result;
			}
		}

		public Location ToLocation()
		{
			return new Location(X, Y, Z);
		}

		public string ToString()
			=> $"({X}, {Y}, {Z})";
    }
}