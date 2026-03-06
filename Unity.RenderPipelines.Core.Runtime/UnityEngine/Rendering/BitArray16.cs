using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	[Serializable]
	public struct BitArray16 : IBitArray
	{
		public uint capacity
		{
			get
			{
				return 16U;
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
				return this.data == ushort.MaxValue;
			}
		}

		public string humanizedData
		{
			get
			{
				return Regex.Replace(string.Format("{0, " + this.capacity.ToString() + "}", Convert.ToString((int)this.data, 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');
			}
		}

		public bool this[uint index]
		{
			get
			{
				return BitArrayUtilities.Get16(index, this.data);
			}
			set
			{
				BitArrayUtilities.Set16(index, ref this.data, value);
			}
		}

		public BitArray16(ushort initValue)
		{
			this.data = initValue;
		}

		public BitArray16(IEnumerable<uint> bitIndexTrue)
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
					this.data |= (ushort)(1 << (int)num);
				}
			}
		}

		public static BitArray16 operator ~(BitArray16 a)
		{
			return new BitArray16(~a.data);
		}

		public static BitArray16 operator |(BitArray16 a, BitArray16 b)
		{
			return new BitArray16(a.data | b.data);
		}

		public static BitArray16 operator &(BitArray16 a, BitArray16 b)
		{
			return new BitArray16(a.data & b.data);
		}

		public IBitArray BitAnd(IBitArray other)
		{
			return this & (BitArray16)other;
		}

		public IBitArray BitOr(IBitArray other)
		{
			return this | (BitArray16)other;
		}

		public IBitArray BitNot()
		{
			return ~this;
		}

		public static bool operator ==(BitArray16 a, BitArray16 b)
		{
			return a.data == b.data;
		}

		public static bool operator !=(BitArray16 a, BitArray16 b)
		{
			return a.data != b.data;
		}

		public override bool Equals(object obj)
		{
			if (obj is BitArray16)
			{
				BitArray16 bitArray = (BitArray16)obj;
				return bitArray.data == this.data;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 1768953197 + this.data.GetHashCode();
		}

		[SerializeField]
		private ushort data;
	}
}
