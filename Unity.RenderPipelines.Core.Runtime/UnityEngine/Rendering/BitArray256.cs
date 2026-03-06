using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	[Serializable]
	public struct BitArray256 : IBitArray
	{
		public uint capacity
		{
			get
			{
				return 256U;
			}
		}

		public bool allFalse
		{
			get
			{
				return this.data1 == 0UL && this.data2 == 0UL && this.data3 == 0UL && this.data4 == 0UL;
			}
		}

		public bool allTrue
		{
			get
			{
				return this.data1 == ulong.MaxValue && this.data2 == ulong.MaxValue && this.data3 == ulong.MaxValue && this.data4 == ulong.MaxValue;
			}
		}

		public string humanizedData
		{
			get
			{
				return Regex.Replace(string.Format("{0, " + 64U.ToString() + "}", Convert.ToString((long)this.data4, 2)).Replace(' ', '0'), ".{8}", "$0.") + Regex.Replace(string.Format("{0, " + 64U.ToString() + "}", Convert.ToString((long)this.data3, 2)).Replace(' ', '0'), ".{8}", "$0.") + Regex.Replace(string.Format("{0, " + 64U.ToString() + "}", Convert.ToString((long)this.data2, 2)).Replace(' ', '0'), ".{8}", "$0.") + Regex.Replace(string.Format("{0, " + 64U.ToString() + "}", Convert.ToString((long)this.data1, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');
			}
		}

		public bool this[uint index]
		{
			get
			{
				return BitArrayUtilities.Get256(index, this.data1, this.data2, this.data3, this.data4);
			}
			set
			{
				BitArrayUtilities.Set256(index, ref this.data1, ref this.data2, ref this.data3, ref this.data4, value);
			}
		}

		public BitArray256(ulong initValue1, ulong initValue2, ulong initValue3, ulong initValue4)
		{
			this.data1 = initValue1;
			this.data2 = initValue2;
			this.data3 = initValue3;
			this.data4 = initValue4;
		}

		public BitArray256(IEnumerable<uint> bitIndexTrue)
		{
			this.data1 = (this.data2 = (this.data3 = (this.data4 = 0UL)));
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
				else if (num < 128U)
				{
					this.data2 |= 1UL << (int)(num - 64U);
				}
				else if (num < 192U)
				{
					this.data3 |= 1UL << (int)(num - 128U);
				}
				else if (num < this.capacity)
				{
					this.data4 |= 1UL << (int)(num - 192U);
				}
			}
		}

		public static BitArray256 operator ~(BitArray256 a)
		{
			return new BitArray256(~a.data1, ~a.data2, ~a.data3, ~a.data4);
		}

		public static BitArray256 operator |(BitArray256 a, BitArray256 b)
		{
			return new BitArray256(a.data1 | b.data1, a.data2 | b.data2, a.data3 | b.data3, a.data4 | b.data4);
		}

		public static BitArray256 operator &(BitArray256 a, BitArray256 b)
		{
			return new BitArray256(a.data1 & b.data1, a.data2 & b.data2, a.data3 & b.data3, a.data4 & b.data4);
		}

		public IBitArray BitAnd(IBitArray other)
		{
			return this & (BitArray256)other;
		}

		public IBitArray BitOr(IBitArray other)
		{
			return this | (BitArray256)other;
		}

		public IBitArray BitNot()
		{
			return ~this;
		}

		public static bool operator ==(BitArray256 a, BitArray256 b)
		{
			return a.data1 == b.data1 && a.data2 == b.data2 && a.data3 == b.data3 && a.data4 == b.data4;
		}

		public static bool operator !=(BitArray256 a, BitArray256 b)
		{
			return a.data1 != b.data1 || a.data2 != b.data2 || a.data3 != b.data3 || a.data4 != b.data4;
		}

		public override bool Equals(object obj)
		{
			if (obj is BitArray256)
			{
				BitArray256 bitArray = (BitArray256)obj;
				if (this.data1.Equals(bitArray.data1) && this.data2.Equals(bitArray.data2) && this.data3.Equals(bitArray.data3))
				{
					return this.data4.Equals(bitArray.data4);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((1870826326 * -1521134295 + this.data1.GetHashCode()) * -1521134295 + this.data2.GetHashCode()) * -1521134295 + this.data3.GetHashCode()) * -1521134295 + this.data4.GetHashCode();
		}

		[SerializeField]
		private ulong data1;

		[SerializeField]
		private ulong data2;

		[SerializeField]
		private ulong data3;

		[SerializeField]
		private ulong data4;
	}
}
