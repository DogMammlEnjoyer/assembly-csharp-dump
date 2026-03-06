using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	[Serializable]
	public struct BitArray8 : IBitArray
	{
		public uint capacity
		{
			get
			{
				return 8U;
			}
		}

		public bool allFalse
		{
			get
			{
				return this.data == 0;
			}
		}

		public bool allTrue
		{
			get
			{
				return this.data == byte.MaxValue;
			}
		}

		public string humanizedData
		{
			get
			{
				return string.Format("{0, " + this.capacity.ToString() + "}", Convert.ToString(this.data, 2)).Replace(' ', '0');
			}
		}

		public bool this[uint index]
		{
			get
			{
				return BitArrayUtilities.Get8(index, this.data);
			}
			set
			{
				BitArrayUtilities.Set8(index, ref this.data, value);
			}
		}

		public BitArray8(byte initValue)
		{
			this.data = initValue;
		}

		public BitArray8(IEnumerable<uint> bitIndexTrue)
		{
			this.data = 0;
			if (bitIndexTrue == null)
			{
				return;
			}
			for (int i = bitIndexTrue.Count<uint>() - 1; i >= 0; i--)
			{
				uint num = bitIndexTrue.ElementAt(i);
				if (num < this.capacity)
				{
					this.data |= (byte)(1 << (int)num);
				}
			}
		}

		public static BitArray8 operator ~(BitArray8 a)
		{
			return new BitArray8(~a.data);
		}

		public static BitArray8 operator |(BitArray8 a, BitArray8 b)
		{
			return new BitArray8(a.data | b.data);
		}

		public static BitArray8 operator &(BitArray8 a, BitArray8 b)
		{
			return new BitArray8(a.data & b.data);
		}

		public IBitArray BitAnd(IBitArray other)
		{
			return this & (BitArray8)other;
		}

		public IBitArray BitOr(IBitArray other)
		{
			return this | (BitArray8)other;
		}

		public IBitArray BitNot()
		{
			return ~this;
		}

		public static bool operator ==(BitArray8 a, BitArray8 b)
		{
			return a.data == b.data;
		}

		public static bool operator !=(BitArray8 a, BitArray8 b)
		{
			return a.data != b.data;
		}

		public override bool Equals(object obj)
		{
			if (obj is BitArray8)
			{
				BitArray8 bitArray = (BitArray8)obj;
				return bitArray.data == this.data;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 1768953197 + this.data.GetHashCode();
		}

		[SerializeField]
		private byte data;
	}
}
