using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	[Serializable]
	public struct BitArray128 : IBitArray
	{
		public uint capacity
		{
			get
			{
				return 128U;
			}
		}

		public bool allFalse
		{
			get
			{
				return this.data1 == 0UL && this.data2 == 0UL;
			}
		}

		public bool allTrue
		{
			get
			{
				return this.data1 == ulong.MaxValue && this.data2 == ulong.MaxValue;
			}
		}

		public string humanizedData
		{
			get
			{
				return Regex.Replace(string.Format("{0, " + 64U.ToString() + "}", Convert.ToString((long)this.data2, 2)).Replace(' ', '0'), ".{8}", "$0.") + Regex.Replace(string.Format("{0, " + 64U.ToString() + "}", Convert.ToString((long)this.data1, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');
			}
		}

		public bool this[uint index]
		{
			get
			{
				if (index >= 64U)
				{
					return (this.data2 & 1UL << (int)(index - 64U)) > 0UL;
				}
				return (this.data1 & 1UL << (int)index) > 0UL;
			}
			set
			{
				if (index < 64U)
				{
					this.data1 = (value ? (this.data1 | 1UL << (int)index) : (this.data1 & ~(1UL << (int)index)));
					return;
				}
				this.data2 = (value ? (this.data2 | 1UL << (int)(index - 64U)) : (this.data2 & ~(1UL << (int)(index - 64U))));
			}
		}

		public BitArray128(ulong initValue1, ulong initValue2)
		{
			this.data1 = initValue1;
			this.data2 = initValue2;
		}

		public BitArray128(IEnumerable<uint> bitIndexTrue)
		{
			this.data1 = (this.data2 = 0UL);
			if (bitIndexTrue == null)
			{
				return;
			}
			for (int i = bitIndexTrue.Count<uint>() - 1; i >= 0; i--)
			{
				uint num = bitIndexTrue.ElementAt(i);
				if (num < 64U)
				{
					this.data1 |= 1UL << (int)num;
				}
				else if (num < this.capacity)
				{
					this.data2 |= 1UL << (int)(num - 64U);
				}
			}
		}

		public static BitArray128 operator ~(BitArray128 a)
		{
			return new BitArray128(~a.data1, ~a.data2);
		}

		public static BitArray128 operator |(BitArray128 a, BitArray128 b)
		{
			return new BitArray128(a.data1 | b.data1, a.data2 | b.data2);
		}

		public static BitArray128 operator &(BitArray128 a, BitArray128 b)
		{
			return new BitArray128(a.data1 & b.data1, a.data2 & b.data2);
		}

		public IBitArray BitAnd(IBitArray other)
		{
			return this & (BitArray128)other;
		}

		public IBitArray BitOr(IBitArray other)
		{
			return this | (BitArray128)other;
		}

		public IBitArray BitNot()
		{
			return ~this;
		}

		public static bool operator ==(BitArray128 a, BitArray128 b)
		{
			return a.data1 == b.data1 && a.data2 == b.data2;
		}

		public static bool operator !=(BitArray128 a, BitArray128 b)
		{
			return a.data1 != b.data1 || a.data2 != b.data2;
		}

		public override bool Equals(object obj)
		{
			if (obj is BitArray128)
			{
				BitArray128 bitArray = (BitArray128)obj;
				if (this.data1.Equals(bitArray.data1))
				{
					return this.data2.Equals(bitArray.data2);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (1755735569 * -1521134295 + this.data1.GetHashCode()) * -1521134295 + this.data2.GetHashCode();
		}

		[SerializeField]
		private ulong data1;

		[SerializeField]
		private ulong data2;
	}
}
