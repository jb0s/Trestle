using System;
using Trestle.Utils;

namespace Trestle.Worlds
{
    // Credits to https://github.com/NiclasOlofsson/MiNET
	public struct ChunkLocation : IEquatable<ChunkLocation>
	{
		public int X, Z;

		public ChunkLocation(int value)
		{
			X = Z = value;
		}

		public ChunkLocation(int x, int z)
		{
			X = x;
			Z = z;
		}

		public ChunkLocation(ChunkLocation v)
		{
			X = v.X;
			Z = v.Z;
		}

		public ChunkLocation(Location location)
		{
			X = ((int)Math.Floor(location.X)) >> 4;
			Z = ((int)Math.Floor(location.Z)) >> 4;
		}

		/// <summary>
		/// Converts this ChunkCoordinates to a string in the format &lt;x, z&gt;.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("<{0},{1}>", X, Z);
		}

		#region Math

		/// <summary>
		/// Calculates the distance between two ChunkCoordinates objects.
		/// </summary>
		public double DistanceTo(ChunkLocation other)
		{
			return Math.Sqrt(Square(other.X - X) +
							 Square(other.Z - Z));
		}

		/// <summary>
		/// Calculates the square of a num.
		/// </summary>
		private int Square(int num)
		{
			return num * num;
		}

		/// <summary>
		/// Finds the distance of this ChunkCoordinates from ChunkCoordinates.Zero
		/// </summary>
		public double Distance
		{
			get { return DistanceTo(Zero); }
		}

		public static ChunkLocation Min(ChunkLocation value1, ChunkLocation value2)
		{
			return new ChunkLocation(
				Math.Min(value1.X, value2.X),
				Math.Min(value1.Z, value2.Z)
				);
		}

		public static ChunkLocation Max(ChunkLocation value1, ChunkLocation value2)
		{
			return new ChunkLocation(
				Math.Max(value1.X, value2.X),
				Math.Max(value1.Z, value2.Z)
				);
		}

		#endregion

		#region Operators

		public static bool operator !=(ChunkLocation a, ChunkLocation b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(ChunkLocation a, ChunkLocation b)
		{
			return a.Equals(b);
		}

		public static ChunkLocation operator +(ChunkLocation a, ChunkLocation b)
		{
			return new ChunkLocation(a.X + b.X, a.Z + b.Z);
		}

		public static ChunkLocation operator -(ChunkLocation a, ChunkLocation b)
		{
			return new ChunkLocation(a.X - b.X, a.Z - b.Z);
		}

		public static ChunkLocation operator -(ChunkLocation a)
		{
			return new ChunkLocation(
				-a.X,
				-a.Z);
		}

		public static ChunkLocation operator *(ChunkLocation a, ChunkLocation b)
		{
			return new ChunkLocation(a.X * b.X, a.Z * b.Z);
		}

		public static ChunkLocation operator /(ChunkLocation a, ChunkLocation b)
		{
			return new ChunkLocation(a.X / b.X, a.Z / b.Z);
		}

		public static ChunkLocation operator %(ChunkLocation a, ChunkLocation b)
		{
			return new ChunkLocation(a.X % b.X, a.Z % b.Z);
		}

		public static ChunkLocation operator +(ChunkLocation a, int b)
		{
			return new ChunkLocation(a.X + b, a.Z + b);
		}

		public static ChunkLocation operator -(ChunkLocation a, int b)
		{
			return new ChunkLocation(a.X - b, a.Z - b);
		}

		public static ChunkLocation operator *(ChunkLocation a, int b)
		{
			return new ChunkLocation(a.X * b, a.Z * b);
		}

		public static ChunkLocation operator /(ChunkLocation a, int b)
		{
			return new ChunkLocation(a.X / b, a.Z / b);
		}

		public static ChunkLocation operator %(ChunkLocation a, int b)
		{
			return new ChunkLocation(a.X % b, a.Z % b);
		}

		public static ChunkLocation operator +(int a, ChunkLocation b)
		{
			return new ChunkLocation(a + b.X, a + b.Z);
		}

		public static ChunkLocation operator -(int a, ChunkLocation b)
		{
			return new ChunkLocation(a - b.X, a - b.Z);
		}

		public static ChunkLocation operator *(int a, ChunkLocation b)
		{
			return new ChunkLocation(a * b.X, a * b.Z);
		}

		public static ChunkLocation operator /(int a, ChunkLocation b)
		{
			return new ChunkLocation(a / b.X, a / b.Z);
		}

		public static ChunkLocation operator %(int a, ChunkLocation b)
		{
			return new ChunkLocation(a % b.X, a % b.Z);
		}

		//public static explicit operator ChunkCoordinates(BlockCoordinates a)
		//{
		//	return new ChunkCoordinates(a.X, a.Z);
		//}

		#endregion

		#region Constants

		public static readonly ChunkLocation Zero = new ChunkLocation(0);
		public static readonly ChunkLocation One = new ChunkLocation(1);

		public static readonly ChunkLocation Forward = new ChunkLocation(0, 1);
		public static readonly ChunkLocation Backward = new ChunkLocation(0, -1);
		public static readonly ChunkLocation Left = new ChunkLocation(-1, 0);
		public static readonly ChunkLocation Right = new ChunkLocation(1, 0);

		#endregion

		public bool Equals(ChunkLocation other)
		{
			return X == other.X && Z == other.Z;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is ChunkLocation && Equals((ChunkLocation)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Z;
			}
		}
	}
}