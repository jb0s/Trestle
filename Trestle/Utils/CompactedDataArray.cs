using System;

namespace Trestle.Utils
{
	// Credits to https://github.com/SharpMC/SharpMC-Rewritten
    public class CompactedDataArray
    {
        public long[] Backing { get; }
		public int Capacity { get; }
		public int BitsPerValue { get; }
		
		private readonly long _valueMask;

		public CompactedDataArray(int bitsPerValue, int capacity)
		{
			if (capacity < 0)
				throw new ArgumentException($"Capacity {capacity} must not be negative");
			
			if (bitsPerValue < 1)
				throw new ArgumentException($"BitsPerValue {bitsPerValue} must not be smaller than 1");
			
			if (bitsPerValue > 64)
				throw new ArgumentException($"BitsPerValue {bitsPerValue} must not be greater than 64");
			
			Backing = new long[(int) Math.Ceiling(bitsPerValue * capacity/64.0)];
			BitsPerValue = bitsPerValue;
			
			_valueMask = (1L << bitsPerValue) - 1L;
			Capacity = capacity;
		}

		public int this[int index]
		{
			get
			{
				CheckIndex(index);

				index *= BitsPerValue;
				int i0 = index >> 6;
				int i1 = index & 0x3f;

				long value = Backing[i0] >> i1;
				int i2 = i1 + BitsPerValue;
				
				// The value is divided over two long values
				if (i2 > 64)
					value |= Backing[++i0] << 64 - i1;

				return (int)(value & _valueMask);
			}
			set
			{
				CheckIndex(index);

				if (value < 0)
					throw new ArgumentException($"Value {value} must not be negative");
				
				if (value > _valueMask)
					throw new ArgumentException($"Value {value} must not be greater than {_valueMask}");

				index *= BitsPerValue;
				int i0 = index >> 6;
				int i1 = index & 0x3f;

				Backing[i0] = Backing[i0] & ~(_valueMask << i1) | (value & _valueMask) << i1;
				int i2 = i1 + BitsPerValue;
				
				// The value is divided over two long values
				if (i2 > 64)
				{
					i0++;
					Backing[i0] = Backing[i0] & ~(long)((1L << i2 - 64) - 1L) | (long)value >> 64 - i1;
				}
			}
		}

		private void CheckIndex(int index)
		{
			if (index < 0)
				throw new IndexOutOfRangeException($"Index {index} must not be negative");

			if (index >= Capacity)
				throw new IndexOutOfRangeException($"Index {index} must not be greater than {Capacity}");
		}
    }
}