using System;
using System.Text;

namespace System.Collections.Specialized
{
	/// <summary>Provides a simple structure that stores Boolean values and small integers in 32 bits of memory.</summary>
	public struct BitVector32
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Specialized.BitVector32" /> structure containing the data represented in an integer.</summary>
		/// <param name="data">An integer representing the data of the new <see cref="T:System.Collections.Specialized.BitVector32" />.</param>
		public BitVector32(int data)
		{
			this._data = (uint)data;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Specialized.BitVector32" /> structure containing the data represented in an existing <see cref="T:System.Collections.Specialized.BitVector32" /> structure.</summary>
		/// <param name="value">A <see cref="T:System.Collections.Specialized.BitVector32" /> structure that contains the data to copy.</param>
		public BitVector32(BitVector32 value)
		{
			this._data = value._data;
		}

		/// <summary>Gets or sets the state of the bit flag indicated by the specified mask.</summary>
		/// <param name="bit">A mask that indicates the bit to get or set.</param>
		/// <returns>
		///   <see langword="true" /> if the specified bit flag is on (1); otherwise, <see langword="false" />.</returns>
		public bool this[int bit]
		{
			get
			{
				return ((ulong)this._data & (ulong)((long)bit)) == (ulong)bit;
			}
			set
			{
				if (value)
				{
					this._data |= (uint)bit;
					return;
				}
				this._data &= (uint)(~(uint)bit);
			}
		}

		/// <summary>Gets or sets the value stored in the specified <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</summary>
		/// <param name="section">A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> that contains the value to get or set.</param>
		/// <returns>The value stored in the specified <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</returns>
		public int this[BitVector32.Section section]
		{
			get
			{
				return (int)((this._data & (uint)((uint)section.Mask << (int)section.Offset)) >> (int)section.Offset);
			}
			set
			{
				value <<= (int)section.Offset;
				int num = (65535 & (int)section.Mask) << (int)section.Offset;
				this._data = ((this._data & (uint)(~(uint)num)) | (uint)(value & num));
			}
		}

		/// <summary>Gets the value of the <see cref="T:System.Collections.Specialized.BitVector32" /> as an integer.</summary>
		/// <returns>The value of the <see cref="T:System.Collections.Specialized.BitVector32" /> as an integer.</returns>
		public int Data
		{
			get
			{
				return (int)this._data;
			}
		}

		private static short CountBitsSet(short mask)
		{
			short num = 0;
			while ((mask & 1) != 0)
			{
				num += 1;
				mask = (short)(mask >> 1);
			}
			return num;
		}

		/// <summary>Creates the first mask in a series of masks that can be used to retrieve individual bits in a <see cref="T:System.Collections.Specialized.BitVector32" /> that is set up as bit flags.</summary>
		/// <returns>A mask that isolates the first bit flag in the <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
		public static int CreateMask()
		{
			return BitVector32.CreateMask(0);
		}

		/// <summary>Creates an additional mask following the specified mask in a series of masks that can be used to retrieve individual bits in a <see cref="T:System.Collections.Specialized.BitVector32" /> that is set up as bit flags.</summary>
		/// <param name="previous">The mask that indicates the previous bit flag.</param>
		/// <returns>A mask that isolates the bit flag following the one that <paramref name="previous" /> points to in <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="previous" /> indicates the last bit flag in the <see cref="T:System.Collections.Specialized.BitVector32" />.</exception>
		public static int CreateMask(int previous)
		{
			if (previous == 0)
			{
				return 1;
			}
			if (previous == -2147483648)
			{
				throw new InvalidOperationException("Bit vector is full.");
			}
			return previous << 1;
		}

		private static short CreateMaskFromHighValue(short highValue)
		{
			short num = 16;
			while (((int)highValue & 32768) == 0)
			{
				num -= 1;
				highValue = (short)(highValue << 1);
			}
			ushort num2 = 0;
			while (num > 0)
			{
				num -= 1;
				num2 = (ushort)(num2 << 1);
				num2 |= 1;
			}
			return (short)num2;
		}

		/// <summary>Creates the first <see cref="T:System.Collections.Specialized.BitVector32.Section" /> in a series of sections that contain small integers.</summary>
		/// <param name="maxValue">A 16-bit signed integer that specifies the maximum value for the new <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</param>
		/// <returns>A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> that can hold a number from zero to <paramref name="maxValue" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="maxValue" /> is less than 1.</exception>
		public static BitVector32.Section CreateSection(short maxValue)
		{
			return BitVector32.CreateSectionHelper(maxValue, 0, 0);
		}

		/// <summary>Creates a new <see cref="T:System.Collections.Specialized.BitVector32.Section" /> following the specified <see cref="T:System.Collections.Specialized.BitVector32.Section" /> in a series of sections that contain small integers.</summary>
		/// <param name="maxValue">A 16-bit signed integer that specifies the maximum value for the new <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</param>
		/// <param name="previous">The previous <see cref="T:System.Collections.Specialized.BitVector32.Section" /> in the <see cref="T:System.Collections.Specialized.BitVector32" />.</param>
		/// <returns>A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> that can hold a number from zero to <paramref name="maxValue" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="maxValue" /> is less than 1.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="previous" /> includes the final bit in the <see cref="T:System.Collections.Specialized.BitVector32" />.  
		/// -or-  
		/// <paramref name="maxValue" /> is greater than the highest value that can be represented by the number of bits after <paramref name="previous" />.</exception>
		public static BitVector32.Section CreateSection(short maxValue, BitVector32.Section previous)
		{
			return BitVector32.CreateSectionHelper(maxValue, previous.Mask, previous.Offset);
		}

		private static BitVector32.Section CreateSectionHelper(short maxValue, short priorMask, short priorOffset)
		{
			if (maxValue < 1)
			{
				throw new ArgumentException(SR.Format("Argument {0} should be larger than {1}.", "maxValue", 1), "maxValue");
			}
			short num = priorOffset + BitVector32.CountBitsSet(priorMask);
			if (num >= 32)
			{
				throw new InvalidOperationException("Bit vector is full.");
			}
			return new BitVector32.Section(BitVector32.CreateMaskFromHighValue(maxValue), num);
		}

		/// <summary>Determines whether the specified object is equal to the <see cref="T:System.Collections.Specialized.BitVector32" />.</summary>
		/// <param name="o">The object to compare with the current <see cref="T:System.Collections.Specialized.BitVector32" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is equal to the <see cref="T:System.Collections.Specialized.BitVector32" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			return o is BitVector32 && this._data == ((BitVector32)o)._data;
		}

		/// <summary>Serves as a hash function for the <see cref="T:System.Collections.Specialized.BitVector32" />.</summary>
		/// <returns>A hash code for the <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Returns a string that represents the specified <see cref="T:System.Collections.Specialized.BitVector32" />.</summary>
		/// <param name="value">The <see cref="T:System.Collections.Specialized.BitVector32" /> to represent.</param>
		/// <returns>A string that represents the specified <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
		public static string ToString(BitVector32 value)
		{
			StringBuilder stringBuilder = new StringBuilder(45);
			stringBuilder.Append("BitVector32{");
			int num = (int)value._data;
			for (int i = 0; i < 32; i++)
			{
				if (((long)num & (long)((ulong)-2147483648)) != 0L)
				{
					stringBuilder.Append('1');
				}
				else
				{
					stringBuilder.Append('0');
				}
				num <<= 1;
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Collections.Specialized.BitVector32" />.</summary>
		/// <returns>A string that represents the current <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
		public override string ToString()
		{
			return BitVector32.ToString(this);
		}

		private uint _data;

		/// <summary>Represents a section of the vector that can contain an integer number.</summary>
		public readonly struct Section
		{
			internal Section(short mask, short offset)
			{
				this._mask = mask;
				this._offset = offset;
			}

			/// <summary>Gets a mask that isolates this section within the <see cref="T:System.Collections.Specialized.BitVector32" />.</summary>
			/// <returns>A mask that isolates this section within the <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
			public short Mask
			{
				get
				{
					return this._mask;
				}
			}

			/// <summary>Gets the offset of this section from the start of the <see cref="T:System.Collections.Specialized.BitVector32" />.</summary>
			/// <returns>The offset of this section from the start of the <see cref="T:System.Collections.Specialized.BitVector32" />.</returns>
			public short Offset
			{
				get
				{
					return this._offset;
				}
			}

			/// <summary>Determines whether the specified object is the same as the current <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</summary>
			/// <param name="o">The object to compare with the current <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</param>
			/// <returns>
			///   <see langword="true" /> if the specified object is the same as the current <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object; otherwise, <see langword="false" />.</returns>
			public override bool Equals(object o)
			{
				return o is BitVector32.Section && this.Equals((BitVector32.Section)o);
			}

			/// <summary>Determines whether the specified <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object is the same as the current <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</summary>
			/// <param name="obj">The <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object to compare with the current <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</param>
			/// <returns>
			///   <see langword="true" /> if the <paramref name="obj" /> parameter is the same as the current <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object; otherwise <see langword="false" />.</returns>
			public bool Equals(BitVector32.Section obj)
			{
				return obj._mask == this._mask && obj._offset == this._offset;
			}

			/// <summary>Determines whether two specified <see cref="T:System.Collections.Specialized.BitVector32.Section" /> objects are equal.</summary>
			/// <param name="a">A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</param>
			/// <param name="b">A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</param>
			/// <returns>
			///   <see langword="true" /> if the <paramref name="a" /> and <paramref name="b" /> parameters represent the same <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object, otherwise, <see langword="false" />.</returns>
			public static bool operator ==(BitVector32.Section a, BitVector32.Section b)
			{
				return a.Equals(b);
			}

			/// <summary>Determines whether two <see cref="T:System.Collections.Specialized.BitVector32.Section" /> objects have different values.</summary>
			/// <param name="a">A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</param>
			/// <param name="b">A <see cref="T:System.Collections.Specialized.BitVector32.Section" /> object.</param>
			/// <returns>
			///   <see langword="true" /> if the <paramref name="a" /> and <paramref name="b" /> parameters represent different <see cref="T:System.Collections.Specialized.BitVector32.Section" /> objects; otherwise, <see langword="false" />.</returns>
			public static bool operator !=(BitVector32.Section a, BitVector32.Section b)
			{
				return !(a == b);
			}

			/// <summary>Serves as a hash function for the current <see cref="T:System.Collections.Specialized.BitVector32.Section" />, suitable for hashing algorithms and data structures, such as a hash table.</summary>
			/// <returns>A hash code for the current <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</returns>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			/// <summary>Returns a string that represents the specified <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</summary>
			/// <param name="value">The <see cref="T:System.Collections.Specialized.BitVector32.Section" /> to represent.</param>
			/// <returns>A string that represents the specified <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</returns>
			public static string ToString(BitVector32.Section value)
			{
				return string.Concat(new string[]
				{
					"Section{0x",
					Convert.ToString(value.Mask, 16),
					", 0x",
					Convert.ToString(value.Offset, 16),
					"}"
				});
			}

			/// <summary>Returns a string that represents the current <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</summary>
			/// <returns>A string that represents the current <see cref="T:System.Collections.Specialized.BitVector32.Section" />.</returns>
			public override string ToString()
			{
				return BitVector32.Section.ToString(this);
			}

			private readonly short _mask;

			private readonly short _offset;
		}
	}
}
