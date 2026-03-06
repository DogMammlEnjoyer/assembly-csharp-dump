using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	[Serializable]
	public struct BitArray64 : IBitArray
	{
		public uint capacity
		{
			get
			{
				return 64U;
			}
		}

		public bool allFalse
		{
			get
			{
				return this.data == 0UL;
			}
		}

		public bool allTrue
		{
			get
			{
				return this.data == ulong.MaxValue;
			}
		}

		public string humanizedData
		{
			get
			{
				return Regex.Replace(string.Format("{0, " + this.capacity.ToString() + "}", Convert.ToString((long)this.data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');
			}
		}

		public bool this[uint index]
		{
			get
			{
				return BitArrayUtilities.Get64(index, this.data);
			}
			set
			{
				BitArrayUtilities.Set64(index, ref this.data, value);
			}
		}

		public BitArray64(ulong initValue)
		{
			this.data = initValue;
		}

		public BitArray64(IEnumerable<uint> bitIndexTrue)
		{
			this.data = 0UL;
			if (bitIndexTrue == null)
			{
				return;
			}
			for (int i = bitIndexTrue.Count<uint>() - 1; i >= 0; i--)
			{
				uint num = bitIndexTrue.ElementAt(i);
				if (num < this.capacity)
				{
					this.data |= 1UL << (int)num;
				}
			}
		}

		public static BitArray64 operator ~(BitArray64 a)
		{
			return new BitArray64(~a.data);
		}

		public static BitArray64 operator |(BitArray64 a, BitArray64 b)
		{
			return new BitArray64(a.data | b.data);
		}

		public static BitArray64 operator &(BitArray64 a, BitArray64 b)
		{
			return new BitArray64(a.data & b.data);
		}

		public IBitArray BitAnd(IBitArray other)
		{
			return this & (BitArray64)other;
		}

		public IBitArray BitOr(IBitArray other)
		{
			return this | (BitArray64)other;
		}

		public IBitArray BitNot()
		{
			return ~this;
		}

		public static bool operator ==(BitArray64 a, BitArray64 b)
		{
			return a.data == b.data;
		}

		public static bool operator !=(BitArray64 a, BitArray64 b)
		{
			return a.data != b.data;
		}

		public override bool Equals(object obj)
		{
			if (obj is BitArray64)
			{
				BitArray64 bitArray = (BitArray64)obj;
				return bitArray.data == this.data;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 1768953197 + this.data.GetHashCode();
		}

		[SerializeField]
		private ulong data;
	}
}
