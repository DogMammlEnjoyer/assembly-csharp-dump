using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{this.GetType().Name} {humanizedData}")]
	[Serializable]
	public struct BitArray32 : IBitArray
	{
		public uint capacity
		{
			get
			{
				return 32U;
			}
		}

		public bool allFalse
		{
			get
			{
				return this.data == 0U;
			}
		}

		public bool allTrue
		{
			get
			{
				return this.data == uint.MaxValue;
			}
		}

		private string humanizedVersion
		{
			get
			{
				return Convert.ToString((long)((ulong)this.data), 2);
			}
		}

		public string humanizedData
		{
			get
			{
				return Regex.Replace(string.Format("{0, " + this.capacity.ToString() + "}", Convert.ToString((long)((ulong)this.data), 2)).Replace(' ', '0'), ".{8}", "$0.").TrimEnd('.');
			}
		}

		public bool this[uint index]
		{
			get
			{
				return BitArrayUtilities.Get32(index, this.data);
			}
			set
			{
				BitArrayUtilities.Set32(index, ref this.data, value);
			}
		}

		public BitArray32(uint initValue)
		{
			this.data = initValue;
		}

		public BitArray32(IEnumerable<uint> bitIndexTrue)
		{
			this.data = 0U;
			if (bitIndexTrue == null)
			{
				return;
			}
			for (int i = bitIndexTrue.Count<uint>() - 1; i >= 0; i--)
			{
				uint num = bitIndexTrue.ElementAt(i);
				if (num < this.capacity)
				{
					this.data |= 1U << (int)num;
				}
			}
		}

		public IBitArray BitAnd(IBitArray other)
		{
			return this & (BitArray32)other;
		}

		public IBitArray BitOr(IBitArray other)
		{
			return this | (BitArray32)other;
		}

		public IBitArray BitNot()
		{
			return ~this;
		}

		public static BitArray32 operator ~(BitArray32 a)
		{
			return new BitArray32(~a.data);
		}

		public static BitArray32 operator |(BitArray32 a, BitArray32 b)
		{
			return new BitArray32(a.data | b.data);
		}

		public static BitArray32 operator &(BitArray32 a, BitArray32 b)
		{
			return new BitArray32(a.data & b.data);
		}

		public static bool operator ==(BitArray32 a, BitArray32 b)
		{
			return a.data == b.data;
		}

		public static bool operator !=(BitArray32 a, BitArray32 b)
		{
			return a.data != b.data;
		}

		public override bool Equals(object obj)
		{
			if (obj is BitArray32)
			{
				BitArray32 bitArray = (BitArray32)obj;
				return bitArray.data == this.data;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 1768953197 + this.data.GetHashCode();
		}

		[SerializeField]
		private uint data;
	}
}
