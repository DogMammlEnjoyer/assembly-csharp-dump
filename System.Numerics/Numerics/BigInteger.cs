using System;
using System.Diagnostics;
using System.Globalization;

namespace System.Numerics
{
	/// <summary>Represents an arbitrarily large signed integer.</summary>
	[Serializable]
	public readonly struct BigInteger : IFormattable, IComparable, IComparable<BigInteger>, IEquatable<BigInteger>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a 32-bit signed integer value.</summary>
		/// <param name="value">A 32-bit signed integer.</param>
		public BigInteger(int value)
		{
			if (value == -2147483648)
			{
				this = BigInteger.s_bnMinInt;
				return;
			}
			this._sign = value;
			this._bits = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using an unsigned 32-bit integer value.</summary>
		/// <param name="value">An unsigned 32-bit integer value.</param>
		[CLSCompliant(false)]
		public BigInteger(uint value)
		{
			if (value <= 2147483647U)
			{
				this._sign = (int)value;
				this._bits = null;
				return;
			}
			this._sign = 1;
			this._bits = new uint[1];
			this._bits[0] = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a 64-bit signed integer value.</summary>
		/// <param name="value">A 64-bit signed integer.</param>
		public BigInteger(long value)
		{
			if (-2147483648L < value && value <= 2147483647L)
			{
				this._sign = (int)value;
				this._bits = null;
				return;
			}
			if (value == -2147483648L)
			{
				this = BigInteger.s_bnMinInt;
				return;
			}
			ulong num;
			if (value < 0L)
			{
				num = (ulong)(-(ulong)value);
				this._sign = -1;
			}
			else
			{
				num = (ulong)value;
				this._sign = 1;
			}
			if (num <= (ulong)-1)
			{
				this._bits = new uint[1];
				this._bits[0] = (uint)num;
				return;
			}
			this._bits = new uint[2];
			this._bits[0] = (uint)num;
			this._bits[1] = (uint)(num >> 32);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure with an unsigned 64-bit integer value.</summary>
		/// <param name="value">An unsigned 64-bit integer.</param>
		[CLSCompliant(false)]
		public BigInteger(ulong value)
		{
			if (value <= 2147483647UL)
			{
				this._sign = (int)value;
				this._bits = null;
				return;
			}
			if (value <= (ulong)-1)
			{
				this._sign = 1;
				this._bits = new uint[1];
				this._bits[0] = (uint)value;
				return;
			}
			this._sign = 1;
			this._bits = new uint[2];
			this._bits[0] = (uint)value;
			this._bits[1] = (uint)(value >> 32);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a single-precision floating-point value.</summary>
		/// <param name="value">A single-precision floating-point value.</param>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is <see cref="F:System.Single.NaN" />, <see cref="F:System.Single.NegativeInfinity" />, or <see cref="F:System.Single.PositiveInfinity" />.</exception>
		public BigInteger(float value)
		{
			this = new BigInteger((double)value);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a double-precision floating-point value.</summary>
		/// <param name="value">A double-precision floating-point value.</param>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />.</exception>
		public BigInteger(double value)
		{
			if (!double.IsFinite(value))
			{
				if (double.IsInfinity(value))
				{
					throw new OverflowException("BigInteger cannot represent infinity.");
				}
				throw new OverflowException("The value is not a number.");
			}
			else
			{
				this._sign = 0;
				this._bits = null;
				int num;
				int num2;
				ulong num3;
				bool flag;
				NumericsHelpers.GetDoubleParts(value, out num, out num2, out num3, out flag);
				if (num3 == 0UL)
				{
					this = BigInteger.Zero;
					return;
				}
				if (num2 <= 0)
				{
					if (num2 <= -64)
					{
						this = BigInteger.Zero;
						return;
					}
					this = num3 >> -num2;
					if (num < 0)
					{
						this._sign = -this._sign;
						return;
					}
				}
				else if (num2 <= 11)
				{
					this = num3 << num2;
					if (num < 0)
					{
						this._sign = -this._sign;
						return;
					}
				}
				else
				{
					num3 <<= 11;
					num2 -= 11;
					int num4 = (num2 - 1) / 32 + 1;
					int num5 = num4 * 32 - num2;
					this._bits = new uint[num4 + 2];
					this._bits[num4 + 1] = (uint)(num3 >> num5 + 32);
					this._bits[num4] = (uint)(num3 >> num5);
					if (num5 > 0)
					{
						this._bits[num4 - 1] = (uint)num3 << 32 - num5;
					}
					this._sign = num;
				}
				return;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="value">A decimal number.</param>
		public BigInteger(decimal value)
		{
			int[] bits = decimal.GetBits(decimal.Truncate(value));
			int num = 3;
			while (num > 0 && bits[num - 1] == 0)
			{
				num--;
			}
			if (num == 0)
			{
				this = BigInteger.s_bnZeroInt;
				return;
			}
			if (num == 1 && bits[0] > 0)
			{
				this._sign = bits[0];
				this._sign *= (((bits[3] & int.MinValue) != 0) ? -1 : 1);
				this._bits = null;
				return;
			}
			this._bits = new uint[num];
			this._bits[0] = (uint)bits[0];
			if (num > 1)
			{
				this._bits[1] = (uint)bits[1];
			}
			if (num > 2)
			{
				this._bits[2] = (uint)bits[2];
			}
			this._sign = (((bits[3] & int.MinValue) != 0) ? -1 : 1);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using the values in a byte array.</summary>
		/// <param name="value">An array of byte values in little-endian order.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public BigInteger(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this = new BigInteger(new ReadOnlySpan<byte>(value), false, false);
		}

		public unsafe BigInteger(ReadOnlySpan<byte> value, bool isUnsigned = false, bool isBigEndian = false)
		{
			int num = value.Length;
			bool flag;
			if (num > 0)
			{
				byte b = isBigEndian ? (*value[0]) : (*value[num - 1]);
				flag = ((b & 128) != 0 && !isUnsigned);
				if (b == 0)
				{
					if (isBigEndian)
					{
						int num2 = 1;
						while (num2 < num && *value[num2] == 0)
						{
							num2++;
						}
						value = value.Slice(num2);
						num = value.Length;
					}
					else
					{
						num -= 2;
						while (num >= 0 && *value[num] == 0)
						{
							num--;
						}
						num++;
					}
				}
			}
			else
			{
				flag = false;
			}
			if (num == 0)
			{
				this._sign = 0;
				this._bits = null;
				return;
			}
			if (num <= 4)
			{
				this._sign = (flag ? -1 : 0);
				if (isBigEndian)
				{
					for (int i = 0; i < num; i++)
					{
						this._sign = (this._sign << 8 | (int)(*value[i]));
					}
				}
				else
				{
					for (int j = num - 1; j >= 0; j--)
					{
						this._sign = (this._sign << 8 | (int)(*value[j]));
					}
				}
				this._bits = null;
				if (this._sign < 0 && !flag)
				{
					this._bits = new uint[]
					{
						(uint)this._sign
					};
					this._sign = 1;
				}
				if (this._sign == -2147483648)
				{
					this = BigInteger.s_bnMinInt;
					return;
				}
			}
			else
			{
				int num3 = num % 4;
				int num4 = num / 4 + ((num3 == 0) ? 0 : 1);
				uint[] array = new uint[num4];
				int num5 = num - 1;
				int l;
				if (isBigEndian)
				{
					int k = num - 4;
					for (l = 0; l < num4 - ((num3 == 0) ? 0 : 1); l++)
					{
						for (int m = 0; m < 4; m++)
						{
							byte b2 = *value[k];
							array[l] = (array[l] << 8 | (uint)b2);
							k++;
						}
						k -= 8;
					}
				}
				else
				{
					int k = 3;
					for (l = 0; l < num4 - ((num3 == 0) ? 0 : 1); l++)
					{
						for (int n = 0; n < 4; n++)
						{
							byte b3 = *value[k];
							array[l] = (array[l] << 8 | (uint)b3);
							k--;
						}
						k += 8;
					}
				}
				if (num3 != 0)
				{
					if (flag)
					{
						array[num4 - 1] = uint.MaxValue;
					}
					if (isBigEndian)
					{
						for (int k = 0; k < num3; k++)
						{
							byte b4 = *value[k];
							array[l] = (array[l] << 8 | (uint)b4);
						}
					}
					else
					{
						for (int k = num5; k >= num - num3; k--)
						{
							byte b5 = *value[k];
							array[l] = (array[l] << 8 | (uint)b5);
						}
					}
				}
				if (flag)
				{
					NumericsHelpers.DangerousMakeTwosComplement(array);
					int num6 = array.Length - 1;
					while (num6 >= 0 && array[num6] == 0U)
					{
						num6--;
					}
					num6++;
					if (num6 == 1)
					{
						uint num7 = array[0];
						if (num7 == 1U)
						{
							this = BigInteger.s_bnMinusOneInt;
							return;
						}
						if (num7 == 2147483648U)
						{
							this = BigInteger.s_bnMinInt;
							return;
						}
						if (array[0] > 0U)
						{
							this._sign = (int)(uint.MaxValue * array[0]);
							this._bits = null;
							return;
						}
					}
					if (num6 != array.Length)
					{
						this._sign = -1;
						this._bits = new uint[num6];
						Array.Copy(array, 0, this._bits, 0, num6);
						return;
					}
					this._sign = -1;
					this._bits = array;
					return;
				}
				else
				{
					this._sign = 1;
					this._bits = array;
				}
			}
		}

		internal BigInteger(int n, uint[] rgu)
		{
			this._sign = n;
			this._bits = rgu;
		}

		internal BigInteger(uint[] value, bool negative)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.Length;
			while (num > 0 && value[num - 1] == 0U)
			{
				num--;
			}
			if (num == 0)
			{
				this = BigInteger.s_bnZeroInt;
				return;
			}
			if (num == 1 && value[0] < 2147483648U)
			{
				this._sign = (int)(negative ? (-(int)value[0]) : value[0]);
				this._bits = null;
				if (this._sign == -2147483648)
				{
					this = BigInteger.s_bnMinInt;
					return;
				}
			}
			else
			{
				this._sign = (negative ? -1 : 1);
				this._bits = new uint[num];
				Array.Copy(value, 0, this._bits, 0, num);
			}
		}

		private BigInteger(uint[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.Length;
			bool flag = num > 0 && (value[num - 1] & 2147483648U) == 2147483648U;
			while (num > 0 && value[num - 1] == 0U)
			{
				num--;
			}
			if (num == 0)
			{
				this = BigInteger.s_bnZeroInt;
				return;
			}
			if (num == 1)
			{
				if (value[0] < 0U && !flag)
				{
					this._bits = new uint[1];
					this._bits[0] = value[0];
					this._sign = 1;
					return;
				}
				if (2147483648U == value[0])
				{
					this = BigInteger.s_bnMinInt;
					return;
				}
				this._sign = (int)value[0];
				this._bits = null;
				return;
			}
			else if (!flag)
			{
				if (num != value.Length)
				{
					this._sign = 1;
					this._bits = new uint[num];
					Array.Copy(value, 0, this._bits, 0, num);
					return;
				}
				this._sign = 1;
				this._bits = value;
				return;
			}
			else
			{
				NumericsHelpers.DangerousMakeTwosComplement(value);
				int num2 = value.Length;
				while (num2 > 0 && value[num2 - 1] == 0U)
				{
					num2--;
				}
				if (num2 == 1 && value[0] > 0U)
				{
					if (value[0] == 1U)
					{
						this = BigInteger.s_bnMinusOneInt;
						return;
					}
					if (value[0] == 2147483648U)
					{
						this = BigInteger.s_bnMinInt;
						return;
					}
					this._sign = (int)(uint.MaxValue * value[0]);
					this._bits = null;
					return;
				}
				else
				{
					if (num2 != value.Length)
					{
						this._sign = -1;
						this._bits = new uint[num2];
						Array.Copy(value, 0, this._bits, 0, num2);
						return;
					}
					this._sign = -1;
					this._bits = value;
					return;
				}
			}
		}

		/// <summary>Gets a value that represents the number 0 (zero).</summary>
		/// <returns>An integer whose value is 0 (zero).</returns>
		public static BigInteger Zero
		{
			get
			{
				return BigInteger.s_bnZeroInt;
			}
		}

		/// <summary>Gets a value that represents the number one (1).</summary>
		/// <returns>An object whose value is one (1).</returns>
		public static BigInteger One
		{
			get
			{
				return BigInteger.s_bnOneInt;
			}
		}

		/// <summary>Gets a value that represents the number negative one (-1).</summary>
		/// <returns>An integer whose value is negative one (-1).</returns>
		public static BigInteger MinusOne
		{
			get
			{
				return BigInteger.s_bnMinusOneInt;
			}
		}

		/// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is a power of two.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of the <see cref="T:System.Numerics.BigInteger" /> object is a power of two; otherwise, <see langword="false" />.</returns>
		public bool IsPowerOfTwo
		{
			get
			{
				if (this._bits == null)
				{
					return (this._sign & this._sign - 1) == 0 && this._sign != 0;
				}
				if (this._sign != 1)
				{
					return false;
				}
				int num = this._bits.Length - 1;
				if ((this._bits[num] & this._bits[num] - 1U) != 0U)
				{
					return false;
				}
				while (--num >= 0)
				{
					if (this._bits[num] != 0U)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.Zero" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of the <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.Zero" />; otherwise, <see langword="false" />.</returns>
		public bool IsZero
		{
			get
			{
				return this._sign == 0;
			}
		}

		/// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.One" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of the <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.One" />; otherwise, <see langword="false" />.</returns>
		public bool IsOne
		{
			get
			{
				return this._sign == 1 && this._bits == null;
			}
		}

		/// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is an even number.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of the <see cref="T:System.Numerics.BigInteger" /> object is an even number; otherwise, <see langword="false" />.</returns>
		public bool IsEven
		{
			get
			{
				if (this._bits != null)
				{
					return (this._bits[0] & 1U) == 0U;
				}
				return (this._sign & 1) == 0;
			}
		}

		/// <summary>Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="T:System.Numerics.BigInteger" /> object.</summary>
		/// <returns>A number that indicates the sign of the <see cref="T:System.Numerics.BigInteger" /> object, as shown in the following table.  
		///   Number  
		///
		///   Description  
		///
		///   -1  
		///
		///   The value of this object is negative.  
		///
		///   0  
		///
		///   The value of this object is 0 (zero).  
		///
		///   1  
		///
		///   The value of this object is positive.</returns>
		public int Sign
		{
			get
			{
				return (this._sign >> 31) - (-this._sign >> 31);
			}
		}

		/// <summary>Converts the string representation of a number to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in the correct format.</exception>
		public static BigInteger Parse(string value)
		{
			return BigInteger.Parse(value, NumberStyles.Integer);
		}

		/// <summary>Converts the string representation of a number in a specified style to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
		/// <param name="value">A string that contains a number to convert.</param>
		/// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of <paramref name="value" />.</param>
		/// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="style" /> is not a <see cref="T:System.Globalization.NumberStyles" /> value.  
		/// -or-  
		/// <paramref name="style" /> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier" /> or <see cref="F:System.Globalization.NumberStyles.HexNumber" /> flag along with another value.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not comply with the input pattern specified by <see cref="T:System.Globalization.NumberStyles" />.</exception>
		public static BigInteger Parse(string value, NumberStyles style)
		{
			return BigInteger.Parse(value, style, NumberFormatInfo.CurrentInfo);
		}

		/// <summary>Converts the string representation of a number in a specified culture-specific format to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
		/// <param name="value">A string that contains a number to convert.</param>
		/// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value" />.</param>
		/// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in the correct format.</exception>
		public static BigInteger Parse(string value, IFormatProvider provider)
		{
			return BigInteger.Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
		}

		/// <summary>Converts the string representation of a number in a specified style and culture-specific format to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
		/// <param name="value">A string that contains a number to convert.</param>
		/// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of <paramref name="value" />.</param>
		/// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value" />.</param>
		/// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="style" /> is not a <see cref="T:System.Globalization.NumberStyles" /> value.  
		/// -or-  
		/// <paramref name="style" /> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier" /> or <see cref="F:System.Globalization.NumberStyles.HexNumber" /> flag along with another value.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not comply with the input pattern specified by <paramref name="style" />.</exception>
		public static BigInteger Parse(string value, NumberStyles style, IFormatProvider provider)
		{
			return BigNumber.ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
		}

		/// <summary>Tries to convert the string representation of a number to its <see cref="T:System.Numerics.BigInteger" /> equivalent, and returns a value that indicates whether the conversion succeeded.</summary>
		/// <param name="value">The string representation of a number.</param>
		/// <param name="result">When this method returns, contains the <see cref="T:System.Numerics.BigInteger" /> equivalent to the number that is contained in <paramref name="value" />, or zero (0) if the conversion fails. The conversion fails if the <paramref name="value" /> parameter is <see langword="null" /> or is not of the correct format. This parameter is passed uninitialized.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> was converted successfully; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public static bool TryParse(string value, out BigInteger result)
		{
			return BigInteger.TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
		}

		/// <summary>Tries to convert the string representation of a number in a specified style and culture-specific format to its <see cref="T:System.Numerics.BigInteger" /> equivalent, and returns a value that indicates whether the conversion succeeded.</summary>
		/// <param name="value">The string representation of a number. The string is interpreted using the style specified by <paramref name="style" />.</param>
		/// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="value" />. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Integer" />.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="value" />.</param>
		/// <param name="result">When this method returns, contains the <see cref="T:System.Numerics.BigInteger" /> equivalent to the number that is contained in <paramref name="value" />, or <see cref="P:System.Numerics.BigInteger.Zero" /> if the conversion failed. The conversion fails if the <paramref name="value" /> parameter is <see langword="null" /> or is not in a format that is compliant with <paramref name="style" />. This parameter is passed uninitialized.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="value" /> parameter was converted successfully; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="style" /> is not a <see cref="T:System.Globalization.NumberStyles" /> value.  
		/// -or-  
		/// <paramref name="style" /> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier" /> or <see cref="F:System.Globalization.NumberStyles.HexNumber" /> flag along with another value.</exception>
		public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out BigInteger result)
		{
			return BigNumber.TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public static BigInteger Parse(ReadOnlySpan<char> value, NumberStyles style = NumberStyles.Integer, IFormatProvider provider = null)
		{
			return BigNumber.ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
		}

		public static bool TryParse(ReadOnlySpan<char> value, out BigInteger result)
		{
			return BigNumber.TryParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
		}

		public static bool TryParse(ReadOnlySpan<char> value, NumberStyles style, IFormatProvider provider, out BigInteger result)
		{
			return BigNumber.TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		/// <summary>Compares two <see cref="T:System.Numerics.BigInteger" /> values and returns an integer that indicates whether the first value is less than, equal to, or greater than the second value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>A signed integer that indicates the relative values of <paramref name="left" /> and <paramref name="right" />, as shown in the following table.  
		///   Value  
		///
		///   Condition  
		///
		///   Less than zero  
		///
		///  <paramref name="left" /> is less than <paramref name="right" />.  
		///
		///   Zero  
		///
		///  <paramref name="left" /> equals <paramref name="right" />.  
		///
		///   Greater than zero  
		///
		///  <paramref name="left" /> is greater than <paramref name="right" />.</returns>
		public static int Compare(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right);
		}

		/// <summary>Gets the absolute value of a <see cref="T:System.Numerics.BigInteger" /> object.</summary>
		/// <param name="value">A number.</param>
		/// <returns>The absolute value of <paramref name="value" />.</returns>
		public static BigInteger Abs(BigInteger value)
		{
			if (!(value >= BigInteger.Zero))
			{
				return -value;
			}
			return value;
		}

		/// <summary>Adds two <see cref="T:System.Numerics.BigInteger" /> values and returns the result.</summary>
		/// <param name="left">The first value to add.</param>
		/// <param name="right">The second value to add.</param>
		/// <returns>The sum of <paramref name="left" /> and <paramref name="right" />.</returns>
		public static BigInteger Add(BigInteger left, BigInteger right)
		{
			return left + right;
		}

		/// <summary>Subtracts one <see cref="T:System.Numerics.BigInteger" /> value from another and returns the result.</summary>
		/// <param name="left">The value to subtract from (the minuend).</param>
		/// <param name="right">The value to subtract (the subtrahend).</param>
		/// <returns>The result of subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
		public static BigInteger Subtract(BigInteger left, BigInteger right)
		{
			return left - right;
		}

		/// <summary>Returns the product of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first number to multiply.</param>
		/// <param name="right">The second number to multiply.</param>
		/// <returns>The product of the <paramref name="left" /> and <paramref name="right" /> parameters.</returns>
		public static BigInteger Multiply(BigInteger left, BigInteger right)
		{
			return left * right;
		}

		/// <summary>Divides one <see cref="T:System.Numerics.BigInteger" /> value by another and returns the result.</summary>
		/// <param name="dividend">The value to be divided.</param>
		/// <param name="divisor">The value to divide by.</param>
		/// <returns>The quotient of the division.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="divisor" /> is 0 (zero).</exception>
		public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
		{
			return dividend / divisor;
		}

		/// <summary>Performs integer division on two <see cref="T:System.Numerics.BigInteger" /> values and returns the remainder.</summary>
		/// <param name="dividend">The value to be divided.</param>
		/// <param name="divisor">The value to divide by.</param>
		/// <returns>The remainder after dividing <paramref name="dividend" /> by <paramref name="divisor" />.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="divisor" /> is 0 (zero).</exception>
		public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
		{
			return dividend % divisor;
		}

		/// <summary>Divides one <see cref="T:System.Numerics.BigInteger" /> value by another, returns the result, and returns the remainder in an output parameter.</summary>
		/// <param name="dividend">The value to be divided.</param>
		/// <param name="divisor">The value to divide by.</param>
		/// <param name="remainder">When this method returns, contains a <see cref="T:System.Numerics.BigInteger" /> value that represents the remainder from the division. This parameter is passed uninitialized.</param>
		/// <returns>The quotient of the division.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="divisor" /> is 0 (zero).</exception>
		public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
		{
			bool flag = dividend._bits == null;
			bool flag2 = divisor._bits == null;
			if (flag && flag2)
			{
				remainder = dividend._sign % divisor._sign;
				return dividend._sign / divisor._sign;
			}
			if (flag)
			{
				remainder = dividend;
				return BigInteger.s_bnZeroInt;
			}
			if (flag2)
			{
				uint num;
				uint[] value = BigIntegerCalculator.Divide(dividend._bits, NumericsHelpers.Abs(divisor._sign), out num);
				remainder = (long)((dividend._sign < 0) ? (ulong.MaxValue * (ulong)num) : ((ulong)num));
				return new BigInteger(value, dividend._sign < 0 ^ divisor._sign < 0);
			}
			if (dividend._bits.Length < divisor._bits.Length)
			{
				remainder = dividend;
				return BigInteger.s_bnZeroInt;
			}
			uint[] value3;
			uint[] value2 = BigIntegerCalculator.Divide(dividend._bits, divisor._bits, out value3);
			remainder = new BigInteger(value3, dividend._sign < 0);
			return new BigInteger(value2, dividend._sign < 0 ^ divisor._sign < 0);
		}

		/// <summary>Negates a specified <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to negate.</param>
		/// <returns>The result of the <paramref name="value" /> parameter multiplied by negative one (-1).</returns>
		public static BigInteger Negate(BigInteger value)
		{
			return -value;
		}

		/// <summary>Returns the natural (base <see langword="e" />) logarithm of a specified number.</summary>
		/// <param name="value">The number whose logarithm is to be found.</param>
		/// <returns>The natural (base <see langword="e" />) logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The natural log of <paramref name="value" /> is out of range of the <see cref="T:System.Double" /> data type.</exception>
		public static double Log(BigInteger value)
		{
			return BigInteger.Log(value, 2.718281828459045);
		}

		/// <summary>Returns the logarithm of a specified number in a specified base.</summary>
		/// <param name="value">A number whose logarithm is to be found.</param>
		/// <param name="baseValue">The base of the logarithm.</param>
		/// <returns>The base <paramref name="baseValue" /> logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The log of <paramref name="value" /> is out of range of the <see cref="T:System.Double" /> data type.</exception>
		public static double Log(BigInteger value, double baseValue)
		{
			if (value._sign < 0 || baseValue == 1.0)
			{
				return double.NaN;
			}
			if (baseValue == double.PositiveInfinity)
			{
				if (!value.IsOne)
				{
					return double.NaN;
				}
				return 0.0;
			}
			else
			{
				if (baseValue == 0.0 && !value.IsOne)
				{
					return double.NaN;
				}
				if (value._bits == null)
				{
					return Math.Log((double)value._sign, baseValue);
				}
				ulong num = (ulong)value._bits[value._bits.Length - 1];
				ulong num2 = (ulong)((value._bits.Length > 1) ? value._bits[value._bits.Length - 2] : 0U);
				ulong num3 = (ulong)((value._bits.Length > 2) ? value._bits[value._bits.Length - 3] : 0U);
				int num4 = NumericsHelpers.CbitHighZero((uint)num);
				long num5 = (long)value._bits.Length * 32L - (long)num4;
				return Math.Log(num << 32 + num4 | num2 << num4 | num3 >> 32 - num4, baseValue) + (double)(num5 - 64L) / Math.Log(baseValue, 2.0);
			}
		}

		/// <summary>Returns the base 10 logarithm of a specified number.</summary>
		/// <param name="value">A number whose logarithm is to be found.</param>
		/// <returns>The base 10 logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The base 10 log of <paramref name="value" /> is out of range of the <see cref="T:System.Double" /> data type.</exception>
		public static double Log10(BigInteger value)
		{
			return BigInteger.Log(value, 10.0);
		}

		/// <summary>Finds the greatest common divisor of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value.</param>
		/// <param name="right">The second value.</param>
		/// <returns>The greatest common divisor of <paramref name="left" /> and <paramref name="right" />.</returns>
		public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right)
		{
			bool flag = left._bits == null;
			bool flag2 = right._bits == null;
			if (flag && flag2)
			{
				return BigIntegerCalculator.Gcd(NumericsHelpers.Abs(left._sign), NumericsHelpers.Abs(right._sign));
			}
			if (flag)
			{
				if (left._sign == 0)
				{
					return new BigInteger(right._bits, false);
				}
				return BigIntegerCalculator.Gcd(right._bits, NumericsHelpers.Abs(left._sign));
			}
			else if (flag2)
			{
				if (right._sign == 0)
				{
					return new BigInteger(left._bits, false);
				}
				return BigIntegerCalculator.Gcd(left._bits, NumericsHelpers.Abs(right._sign));
			}
			else
			{
				if (BigIntegerCalculator.Compare(left._bits, right._bits) < 0)
				{
					return BigInteger.GreatestCommonDivisor(right._bits, left._bits);
				}
				return BigInteger.GreatestCommonDivisor(left._bits, right._bits);
			}
		}

		private static BigInteger GreatestCommonDivisor(uint[] leftBits, uint[] rightBits)
		{
			if (rightBits.Length == 1)
			{
				uint right = BigIntegerCalculator.Remainder(leftBits, rightBits[0]);
				return BigIntegerCalculator.Gcd(rightBits[0], right);
			}
			if (rightBits.Length == 2)
			{
				uint[] array = BigIntegerCalculator.Remainder(leftBits, rightBits);
				ulong left = (ulong)rightBits[1] << 32 | (ulong)rightBits[0];
				ulong right2 = (ulong)array[1] << 32 | (ulong)array[0];
				return BigIntegerCalculator.Gcd(left, right2);
			}
			return new BigInteger(BigIntegerCalculator.Gcd(leftBits, rightBits), false);
		}

		/// <summary>Returns the larger of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>The <paramref name="left" /> or <paramref name="right" /> parameter, whichever is larger.</returns>
		public static BigInteger Max(BigInteger left, BigInteger right)
		{
			if (left.CompareTo(right) < 0)
			{
				return right;
			}
			return left;
		}

		/// <summary>Returns the smaller of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>The <paramref name="left" /> or <paramref name="right" /> parameter, whichever is smaller.</returns>
		public static BigInteger Min(BigInteger left, BigInteger right)
		{
			if (left.CompareTo(right) <= 0)
			{
				return left;
			}
			return right;
		}

		/// <summary>Performs modulus division on a number raised to the power of another number.</summary>
		/// <param name="value">The number to raise to the <paramref name="exponent" /> power.</param>
		/// <param name="exponent">The exponent to raise <paramref name="value" /> by.</param>
		/// <param name="modulus">The number by which to divide <paramref name="value" /> raised to the <paramref name="exponent" /> power.</param>
		/// <returns>The remainder after dividing <paramref name="value" />exponent by <paramref name="modulus" />.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="modulus" /> is zero.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="exponent" /> is negative.</exception>
		public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
		{
			if (exponent.Sign < 0)
			{
				throw new ArgumentOutOfRangeException("exponent", "The number must be greater than or equal to zero.");
			}
			bool flag = value._bits == null;
			bool flag2 = exponent._bits == null;
			if (modulus._bits == null)
			{
				uint num = (flag && flag2) ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) : (flag ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits, NumericsHelpers.Abs(modulus._sign)) : (flag2 ? BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) : BigIntegerCalculator.Pow(value._bits, exponent._bits, NumericsHelpers.Abs(modulus._sign))));
				return (long)((value._sign < 0 && !exponent.IsEven) ? (ulong.MaxValue * (ulong)num) : ((ulong)num));
			}
			return new BigInteger((flag && flag2) ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), modulus._bits) : (flag ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits, modulus._bits) : (flag2 ? BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent._sign), modulus._bits) : BigIntegerCalculator.Pow(value._bits, exponent._bits, modulus._bits))), value._sign < 0 && !exponent.IsEven);
		}

		/// <summary>Raises a <see cref="T:System.Numerics.BigInteger" /> value to the power of a specified value.</summary>
		/// <param name="value">The number to raise to the <paramref name="exponent" /> power.</param>
		/// <param name="exponent">The exponent to raise <paramref name="value" /> by.</param>
		/// <returns>The result of raising <paramref name="value" /> to the <paramref name="exponent" /> power.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="exponent" /> is negative.</exception>
		public static BigInteger Pow(BigInteger value, int exponent)
		{
			if (exponent < 0)
			{
				throw new ArgumentOutOfRangeException("exponent", "The number must be greater than or equal to zero.");
			}
			if (exponent == 0)
			{
				return BigInteger.s_bnOneInt;
			}
			if (exponent == 1)
			{
				return value;
			}
			bool flag = value._bits == null;
			if (flag)
			{
				if (value._sign == 1)
				{
					return value;
				}
				if (value._sign == -1)
				{
					if ((exponent & 1) == 0)
					{
						return BigInteger.s_bnOneInt;
					}
					return value;
				}
				else if (value._sign == 0)
				{
					return value;
				}
			}
			return new BigInteger(flag ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent)) : BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent)), value._sign < 0 && (exponent & 1) != 0);
		}

		/// <summary>Returns the hash code for the current <see cref="T:System.Numerics.BigInteger" /> object.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (this._bits == null)
			{
				return this._sign;
			}
			int num = this._sign;
			int num2 = this._bits.Length;
			while (--num2 >= 0)
			{
				num = NumericsHelpers.CombineHash(num, (int)this._bits[num2]);
			}
			return num;
		}

		/// <summary>Returns a value that indicates whether the current instance and a specified object have the same value.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="obj" /> argument is a <see cref="T:System.Numerics.BigInteger" /> object, and its value is equal to the value of the current <see cref="T:System.Numerics.BigInteger" /> instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is BigInteger && this.Equals((BigInteger)obj);
		}

		/// <summary>Returns a value that indicates whether the current instance and a signed 64-bit integer have the same value.</summary>
		/// <param name="other">The signed 64-bit integer value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the signed 64-bit integer and the current instance have the same value; otherwise, <see langword="false" />.</returns>
		public bool Equals(long other)
		{
			if (this._bits == null)
			{
				return (long)this._sign == other;
			}
			int num;
			if (((long)this._sign ^ other) < 0L || (num = this._bits.Length) > 2)
			{
				return false;
			}
			ulong num2 = (ulong)((other < 0L) ? (-(ulong)other) : other);
			if (num == 1)
			{
				return (ulong)this._bits[0] == num2;
			}
			return NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) == num2;
		}

		/// <summary>Returns a value that indicates whether the current instance and an unsigned 64-bit integer have the same value.</summary>
		/// <param name="other">The unsigned 64-bit integer to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the current instance and the unsigned 64-bit integer have the same value; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public bool Equals(ulong other)
		{
			if (this._sign < 0)
			{
				return false;
			}
			if (this._bits == null)
			{
				return (long)this._sign == (long)other;
			}
			int num = this._bits.Length;
			if (num > 2)
			{
				return false;
			}
			if (num == 1)
			{
				return (ulong)this._bits[0] == other;
			}
			return NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) == other;
		}

		/// <summary>Returns a value that indicates whether the current instance and a specified <see cref="T:System.Numerics.BigInteger" /> object have the same value.</summary>
		/// <param name="other">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Numerics.BigInteger" /> object and <paramref name="other" /> have the same value; otherwise, <see langword="false" />.</returns>
		public bool Equals(BigInteger other)
		{
			if (this._sign != other._sign)
			{
				return false;
			}
			if (this._bits == other._bits)
			{
				return true;
			}
			if (this._bits == null || other._bits == null)
			{
				return false;
			}
			int num = this._bits.Length;
			return num == other._bits.Length && BigInteger.GetDiffLength(this._bits, other._bits, num) == 0;
		}

		/// <summary>Compares this instance to a signed 64-bit integer and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the signed 64-bit integer.</summary>
		/// <param name="other">The signed 64-bit integer to compare.</param>
		/// <returns>A signed integer value that indicates the relationship of this instance to <paramref name="other" />, as shown in the following table.  
		///   Return value  
		///
		///   Description  
		///
		///   Less than zero  
		///
		///   The current instance is less than <paramref name="other" />.  
		///
		///   Zero  
		///
		///   The current instance equals <paramref name="other" />.  
		///
		///   Greater than zero  
		///
		///   The current instance is greater than <paramref name="other" />.</returns>
		public int CompareTo(long other)
		{
			if (this._bits == null)
			{
				return ((long)this._sign).CompareTo(other);
			}
			int num;
			if (((long)this._sign ^ other) < 0L || (num = this._bits.Length) > 2)
			{
				return this._sign;
			}
			ulong value = (ulong)((other < 0L) ? (-(ulong)other) : other);
			ulong num2 = (num == 2) ? NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) : ((ulong)this._bits[0]);
			return this._sign * num2.CompareTo(value);
		}

		/// <summary>Compares this instance to an unsigned 64-bit integer and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the unsigned 64-bit integer.</summary>
		/// <param name="other">The unsigned 64-bit integer to compare.</param>
		/// <returns>A signed integer that indicates the relative value of this instance and <paramref name="other" />, as shown in the following table.  
		///   Return value  
		///
		///   Description  
		///
		///   Less than zero  
		///
		///   The current instance is less than <paramref name="other" />.  
		///
		///   Zero  
		///
		///   The current instance equals <paramref name="other" />.  
		///
		///   Greater than zero  
		///
		///   The current instance is greater than <paramref name="other" />.</returns>
		[CLSCompliant(false)]
		public int CompareTo(ulong other)
		{
			if (this._sign < 0)
			{
				return -1;
			}
			if (this._bits == null)
			{
				return ((ulong)((long)this._sign)).CompareTo(other);
			}
			int num = this._bits.Length;
			if (num > 2)
			{
				return 1;
			}
			return ((num == 2) ? NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) : ((ulong)this._bits[0])).CompareTo(other);
		}

		/// <summary>Compares this instance to a second <see cref="T:System.Numerics.BigInteger" /> and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.</summary>
		/// <param name="other">The object to compare.</param>
		/// <returns>A signed integer value that indicates the relationship of this instance to <paramref name="other" />, as shown in the following table.  
		///   Return value  
		///
		///   Description  
		///
		///   Less than zero  
		///
		///   The current instance is less than <paramref name="other" />.  
		///
		///   Zero  
		///
		///   The current instance equals <paramref name="other" />.  
		///
		///   Greater than zero  
		///
		///   The current instance is greater than <paramref name="other" />.</returns>
		public int CompareTo(BigInteger other)
		{
			if ((this._sign ^ other._sign) < 0)
			{
				if (this._sign >= 0)
				{
					return 1;
				}
				return -1;
			}
			else if (this._bits == null)
			{
				if (other._bits != null)
				{
					return -other._sign;
				}
				if (this._sign < other._sign)
				{
					return -1;
				}
				if (this._sign <= other._sign)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				int num;
				int num2;
				if (other._bits == null || (num = this._bits.Length) > (num2 = other._bits.Length))
				{
					return this._sign;
				}
				if (num < num2)
				{
					return -this._sign;
				}
				int diffLength = BigInteger.GetDiffLength(this._bits, other._bits, num);
				if (diffLength == 0)
				{
					return 0;
				}
				if (this._bits[diffLength - 1] >= other._bits[diffLength - 1])
				{
					return this._sign;
				}
				return -this._sign;
			}
		}

		/// <summary>Compares this instance to a specified object and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>A signed integer that indicates the relationship of the current instance to the <paramref name="obj" /> parameter, as shown in the following table.  
		///   Return value  
		///
		///   Description  
		///
		///   Less than zero  
		///
		///   The current instance is less than <paramref name="obj" />.  
		///
		///   Zero  
		///
		///   The current instance equals <paramref name="obj" />.  
		///
		///   Greater than zero  
		///
		///   The current instance is greater than <paramref name="obj" />, or the <paramref name="obj" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="obj" /> is not a <see cref="T:System.Numerics.BigInteger" />.</exception>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is BigInteger))
			{
				throw new ArgumentException("The parameter must be a BigInteger.", "obj");
			}
			return this.CompareTo((BigInteger)obj);
		}

		/// <summary>Converts a <see cref="T:System.Numerics.BigInteger" /> value to a byte array.</summary>
		/// <returns>The value of the current <see cref="T:System.Numerics.BigInteger" /> object converted to an array of bytes.</returns>
		public byte[] ToByteArray()
		{
			return this.ToByteArray(false, false);
		}

		public byte[] ToByteArray(bool isUnsigned = false, bool isBigEndian = false)
		{
			int num = 0;
			return this.TryGetBytes(BigInteger.GetBytesMode.AllocateArray, default(Span<byte>), isUnsigned, isBigEndian, ref num);
		}

		public bool TryWriteBytes(Span<byte> destination, out int bytesWritten, bool isUnsigned = false, bool isBigEndian = false)
		{
			bytesWritten = 0;
			if (this.TryGetBytes(BigInteger.GetBytesMode.Span, destination, isUnsigned, isBigEndian, ref bytesWritten) == null)
			{
				bytesWritten = 0;
				return false;
			}
			return true;
		}

		internal bool TryWriteOrCountBytes(Span<byte> destination, out int bytesWritten, bool isUnsigned = false, bool isBigEndian = false)
		{
			bytesWritten = 0;
			return this.TryGetBytes(BigInteger.GetBytesMode.Span, destination, isUnsigned, isBigEndian, ref bytesWritten) != null;
		}

		public int GetByteCount(bool isUnsigned = false)
		{
			int result = 0;
			this.TryGetBytes(BigInteger.GetBytesMode.Count, default(Span<byte>), isUnsigned, false, ref result);
			return result;
		}

		private unsafe byte[] TryGetBytes(BigInteger.GetBytesMode mode, Span<byte> destination, bool isUnsigned, bool isBigEndian, ref int bytesWritten)
		{
			int sign = this._sign;
			if (sign == 0)
			{
				if (mode == BigInteger.GetBytesMode.AllocateArray)
				{
					return new byte[1];
				}
				if (mode == BigInteger.GetBytesMode.Count)
				{
					bytesWritten = 1;
					return null;
				}
				bytesWritten = 1;
				if (destination.Length != 0)
				{
					*destination[0] = 0;
					return BigInteger.s_success;
				}
				return null;
			}
			else
			{
				if (isUnsigned && sign < 0)
				{
					throw new OverflowException("Negative values do not have an unsigned representation.");
				}
				int num = 0;
				uint[] bits = this._bits;
				byte b;
				uint num2;
				if (bits == null)
				{
					b = ((sign < 0) ? byte.MaxValue : 0);
					num2 = (uint)sign;
				}
				else if (sign == -1)
				{
					b = byte.MaxValue;
					while (bits[num] == 0U)
					{
						num++;
					}
					num2 = ~bits[bits.Length - 1];
					if (bits.Length - 1 == num)
					{
						num2 += 1U;
					}
				}
				else
				{
					b = 0;
					num2 = bits[bits.Length - 1];
				}
				byte b2;
				int num3;
				if ((b2 = (byte)(num2 >> 24)) != b)
				{
					num3 = 3;
				}
				else if ((b2 = (byte)(num2 >> 16)) != b)
				{
					num3 = 2;
				}
				else if ((b2 = (byte)(num2 >> 8)) != b)
				{
					num3 = 1;
				}
				else
				{
					b2 = (byte)num2;
					num3 = 0;
				}
				bool flag = (b2 & 128) != (b & 128) && !isUnsigned;
				int num4 = num3 + 1 + (flag ? 1 : 0);
				if (bits != null)
				{
					num4 = checked(4 * (bits.Length - 1) + num4);
				}
				byte[] result;
				if (mode != BigInteger.GetBytesMode.AllocateArray)
				{
					if (mode == BigInteger.GetBytesMode.Count)
					{
						bytesWritten = num4;
						return null;
					}
					bytesWritten = num4;
					if (destination.Length < num4)
					{
						return null;
					}
					result = BigInteger.s_success;
				}
				else
				{
					destination = (result = new byte[num4]);
				}
				int num5 = isBigEndian ? (num4 - 1) : 0;
				int num6 = isBigEndian ? -1 : 1;
				if (bits != null)
				{
					for (int i = 0; i < bits.Length - 1; i++)
					{
						uint num7 = bits[i];
						if (sign == -1)
						{
							num7 = ~num7;
							if (i <= num)
							{
								num7 += 1U;
							}
						}
						*destination[num5] = (byte)num7;
						num5 += num6;
						*destination[num5] = (byte)(num7 >> 8);
						num5 += num6;
						*destination[num5] = (byte)(num7 >> 16);
						num5 += num6;
						*destination[num5] = (byte)(num7 >> 24);
						num5 += num6;
					}
				}
				*destination[num5] = (byte)num2;
				if (num3 != 0)
				{
					num5 += num6;
					*destination[num5] = (byte)(num2 >> 8);
					if (num3 != 1)
					{
						num5 += num6;
						*destination[num5] = (byte)(num2 >> 16);
						if (num3 != 2)
						{
							num5 += num6;
							*destination[num5] = (byte)(num2 >> 24);
						}
					}
				}
				if (flag)
				{
					num5 += num6;
					*destination[num5] = b;
				}
				return result;
			}
		}

		private uint[] ToUInt32Array()
		{
			if (this._bits == null && this._sign == 0)
			{
				return new uint[1];
			}
			uint[] array;
			uint num;
			if (this._bits == null)
			{
				array = new uint[]
				{
					(uint)this._sign
				};
				num = ((this._sign < 0) ? uint.MaxValue : 0U);
			}
			else if (this._sign == -1)
			{
				array = (uint[])this._bits.Clone();
				NumericsHelpers.DangerousMakeTwosComplement(array);
				num = uint.MaxValue;
			}
			else
			{
				array = this._bits;
				num = 0U;
			}
			int num2 = array.Length - 1;
			while (num2 > 0 && array[num2] == num)
			{
				num2--;
			}
			bool flag = (array[num2] & 2147483648U) != (num & 2147483648U);
			uint[] array2 = new uint[num2 + 1 + (flag ? 1 : 0)];
			Array.Copy(array, 0, array2, 0, num2 + 1);
			if (flag)
			{
				array2[array2.Length - 1] = num;
			}
			return array2;
		}

		/// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation.</summary>
		/// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value.</returns>
		public override string ToString()
		{
			return BigNumber.FormatBigInteger(this, null, NumberFormatInfo.CurrentInfo);
		}

		/// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation by using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value in the format specified by the <paramref name="provider" /> parameter.</returns>
		public string ToString(IFormatProvider provider)
		{
			return BigNumber.FormatBigInteger(this, null, NumberFormatInfo.GetInstance(provider));
		}

		/// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation by using the specified format.</summary>
		/// <param name="format">A standard or custom numeric format string.</param>
		/// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value in the format specified by the <paramref name="format" /> parameter.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid format string.</exception>
		public string ToString(string format)
		{
			return BigNumber.FormatBigInteger(this, format, NumberFormatInfo.CurrentInfo);
		}

		/// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation by using the specified format and culture-specific format information.</summary>
		/// <param name="format">A standard or custom numeric format string.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value as specified by the <paramref name="format" /> and <paramref name="provider" /> parameters.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid format string.</exception>
		public string ToString(string format, IFormatProvider provider)
		{
			return BigNumber.FormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider));
		}

		public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider provider = null)
		{
			return BigNumber.TryFormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider), destination, out charsWritten);
		}

		private static BigInteger Add(uint[] leftBits, int leftSign, uint[] rightBits, int rightSign)
		{
			bool flag = leftBits == null;
			bool flag2 = rightBits == null;
			if (flag && flag2)
			{
				return (long)leftSign + (long)rightSign;
			}
			if (flag)
			{
				return new BigInteger(BigIntegerCalculator.Add(rightBits, NumericsHelpers.Abs(leftSign)), leftSign < 0);
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Add(leftBits, NumericsHelpers.Abs(rightSign)), leftSign < 0);
			}
			if (leftBits.Length < rightBits.Length)
			{
				return new BigInteger(BigIntegerCalculator.Add(rightBits, leftBits), leftSign < 0);
			}
			return new BigInteger(BigIntegerCalculator.Add(leftBits, rightBits), leftSign < 0);
		}

		/// <summary>Subtracts a <see cref="T:System.Numerics.BigInteger" /> value from another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The value to subtract from (the minuend).</param>
		/// <param name="right">The value to subtract (the subtrahend).</param>
		/// <returns>The result of subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
		public static BigInteger operator -(BigInteger left, BigInteger right)
		{
			if (left._sign < 0 != right._sign < 0)
			{
				return BigInteger.Add(left._bits, left._sign, right._bits, -1 * right._sign);
			}
			return BigInteger.Subtract(left._bits, left._sign, right._bits, right._sign);
		}

		private static BigInteger Subtract(uint[] leftBits, int leftSign, uint[] rightBits, int rightSign)
		{
			bool flag = leftBits == null;
			bool flag2 = rightBits == null;
			if (flag && flag2)
			{
				return (long)leftSign - (long)rightSign;
			}
			if (flag)
			{
				return new BigInteger(BigIntegerCalculator.Subtract(rightBits, NumericsHelpers.Abs(leftSign)), leftSign >= 0);
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Subtract(leftBits, NumericsHelpers.Abs(rightSign)), leftSign < 0);
			}
			if (BigIntegerCalculator.Compare(leftBits, rightBits) < 0)
			{
				return new BigInteger(BigIntegerCalculator.Subtract(rightBits, leftBits), leftSign >= 0);
			}
			return new BigInteger(BigIntegerCalculator.Subtract(leftBits, rightBits), leftSign < 0);
		}

		/// <summary>Defines an implicit conversion of an unsigned byte to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		public static implicit operator BigInteger(byte value)
		{
			return new BigInteger((int)value);
		}

		/// <summary>Defines an implicit conversion of an 8-bit signed integer to a <see cref="T:System.Numerics.BigInteger" /> value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="M:System.Numerics.BigInteger.#ctor(System.Int32)" />.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		[CLSCompliant(false)]
		public static implicit operator BigInteger(sbyte value)
		{
			return new BigInteger((int)value);
		}

		/// <summary>Defines an implicit conversion of a signed 16-bit integer to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		public static implicit operator BigInteger(short value)
		{
			return new BigInteger((int)value);
		}

		/// <summary>Defines an implicit conversion of a 16-bit unsigned integer to a <see cref="T:System.Numerics.BigInteger" /> value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="M:System.Numerics.BigInteger.op_Implicit(System.Int32)~System.Numerics.BigInteger" />.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		[CLSCompliant(false)]
		public static implicit operator BigInteger(ushort value)
		{
			return new BigInteger((int)value);
		}

		/// <summary>Defines an implicit conversion of a signed 32-bit integer to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		public static implicit operator BigInteger(int value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an implicit conversion of a 32-bit unsigned integer to a <see cref="T:System.Numerics.BigInteger" /> value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="M:System.Numerics.BigInteger.op_Implicit(System.Int64)~System.Numerics.BigInteger" />.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		[CLSCompliant(false)]
		public static implicit operator BigInteger(uint value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an implicit conversion of a signed 64-bit integer to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		public static implicit operator BigInteger(long value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an implicit conversion of a 64-bit unsigned integer to a <see cref="T:System.Numerics.BigInteger" /> value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="T:System.Double" />.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		[CLSCompliant(false)]
		public static implicit operator BigInteger(ulong value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Single" /> value to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is <see cref="F:System.Single.NaN" />, <see cref="F:System.Single.PositiveInfinity" />, or <see cref="F:System.Single.NegativeInfinity" />.</exception>
		public static explicit operator BigInteger(float value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Double" /> value to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.PositiveInfinity" />, or <see cref="F:System.Double.NegativeInfinity" />.</exception>
		public static explicit operator BigInteger(double value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Decimal" /> object to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Numerics.BigInteger" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		public static explicit operator BigInteger(decimal value)
		{
			return new BigInteger(value);
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to an unsigned byte value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Byte" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static explicit operator byte(BigInteger value)
		{
			return checked((byte)((int)value));
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a signed 8-bit value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="T:System.Int16" />.</summary>
		/// <param name="value">The value to convert to a signed 8-bit value.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.SByte.MinValue" /> or is greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator sbyte(BigInteger value)
		{
			return checked((sbyte)((int)value));
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a 16-bit signed integer value.</summary>
		/// <param name="value">The value to convert to a 16-bit signed integer.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Int16.MinValue" /> or is greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static explicit operator short(BigInteger value)
		{
			return checked((short)((int)value));
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to an unsigned 16-bit integer value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="T:System.Int32" />.</summary>
		/// <param name="value">The value to convert to an unsigned 16-bit integer.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.UInt16.MinValue" /> or is greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator ushort(BigInteger value)
		{
			return checked((ushort)((int)value));
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a 32-bit signed integer value.</summary>
		/// <param name="value">The value to convert to a 32-bit signed integer.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Int32.MinValue" /> or is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static explicit operator int(BigInteger value)
		{
			if (value._bits == null)
			{
				return value._sign;
			}
			if (value._bits.Length > 1)
			{
				throw new OverflowException("Value was either too large or too small for an Int32.");
			}
			if (value._sign > 0)
			{
				return checked((int)value._bits[0]);
			}
			if (value._bits[0] > 2147483648U)
			{
				throw new OverflowException("Value was either too large or too small for an Int32.");
			}
			return (int)(-(int)value._bits[0]);
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to an unsigned 32-bit integer value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="T:System.Int64" />.</summary>
		/// <param name="value">The value to convert to an unsigned 32-bit integer.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.UInt32.MinValue" /> or is greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator uint(BigInteger value)
		{
			if (value._bits == null)
			{
				return checked((uint)value._sign);
			}
			if (value._bits.Length > 1 || value._sign < 0)
			{
				throw new OverflowException("Value was either too large or too small for a UInt32.");
			}
			return value._bits[0];
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a 64-bit signed integer value.</summary>
		/// <param name="value">The value to convert to a 64-bit signed integer.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Int64.MinValue" /> or is greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static explicit operator long(BigInteger value)
		{
			if (value._bits == null)
			{
				return (long)value._sign;
			}
			int num = value._bits.Length;
			if (num > 2)
			{
				throw new OverflowException("Value was either too large or too small for an Int64.");
			}
			ulong num2;
			if (num > 1)
			{
				num2 = NumericsHelpers.MakeUlong(value._bits[1], value._bits[0]);
			}
			else
			{
				num2 = (ulong)value._bits[0];
			}
			long num3 = (long)((value._sign > 0) ? num2 : (-(long)num2));
			if ((num3 > 0L && value._sign > 0) || (num3 < 0L && value._sign < 0))
			{
				return num3;
			}
			throw new OverflowException("Value was either too large or too small for an Int64.");
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to an unsigned 64-bit integer value.  
		///  This API is not CLS-compliant. The compliant alternative is <see cref="T:System.Double" />.</summary>
		/// <param name="value">The value to convert to an unsigned 64-bit integer.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.UInt64.MinValue" /> or is greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator ulong(BigInteger value)
		{
			if (value._bits == null)
			{
				return checked((ulong)value._sign);
			}
			int num = value._bits.Length;
			if (num > 2 || value._sign < 0)
			{
				throw new OverflowException("Value was either too large or too small for a UInt64.");
			}
			if (num > 1)
			{
				return NumericsHelpers.MakeUlong(value._bits[1], value._bits[0]);
			}
			return (ulong)value._bits[0];
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a single-precision floating-point value.</summary>
		/// <param name="value">The value to convert to a single-precision floating-point value.</param>
		/// <returns>An object that contains the closest possible representation of the <paramref name="value" /> parameter.</returns>
		public static explicit operator float(BigInteger value)
		{
			return (float)((double)value);
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a <see cref="T:System.Double" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Double" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		public static explicit operator double(BigInteger value)
		{
			int sign = value._sign;
			uint[] bits = value._bits;
			if (bits == null)
			{
				return (double)sign;
			}
			int num = bits.Length;
			if (num <= 32)
			{
				ulong num2 = (ulong)bits[num - 1];
				ulong num3 = (ulong)((num > 1) ? bits[num - 2] : 0U);
				ulong num4 = (ulong)((num > 2) ? bits[num - 3] : 0U);
				int num5 = NumericsHelpers.CbitHighZero((uint)num2);
				int exp = (num - 2) * 32 - num5;
				ulong man = num2 << 32 + num5 | num3 << num5 | num4 >> 32 - num5;
				return NumericsHelpers.GetDoubleFromParts(sign, exp, man);
			}
			if (sign == 1)
			{
				return double.PositiveInfinity;
			}
			return double.NegativeInfinity;
		}

		/// <summary>Defines an explicit conversion of a <see cref="T:System.Numerics.BigInteger" /> object to a <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="value">The value to convert to a <see cref="T:System.Decimal" />.</param>
		/// <returns>An object that contains the value of the <paramref name="value" /> parameter.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Decimal.MinValue" /> or greater than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static explicit operator decimal(BigInteger value)
		{
			if (value._bits == null)
			{
				return value._sign;
			}
			int num = value._bits.Length;
			if (num > 3)
			{
				throw new OverflowException("Value was either too large or too small for a Decimal.");
			}
			int lo = 0;
			int mid = 0;
			int hi = 0;
			if (num > 2)
			{
				hi = (int)value._bits[2];
			}
			if (num > 1)
			{
				mid = (int)value._bits[1];
			}
			if (num > 0)
			{
				lo = (int)value._bits[0];
			}
			return new decimal(lo, mid, hi, value._sign < 0, 0);
		}

		/// <summary>Performs a bitwise <see langword="And" /> operation on two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value.</param>
		/// <param name="right">The second value.</param>
		/// <returns>The result of the bitwise <see langword="And" /> operation.</returns>
		public static BigInteger operator &(BigInteger left, BigInteger right)
		{
			if (left.IsZero || right.IsZero)
			{
				return BigInteger.Zero;
			}
			if (left._bits == null && right._bits == null)
			{
				return left._sign & right._sign;
			}
			uint[] array = left.ToUInt32Array();
			uint[] array2 = right.ToUInt32Array();
			uint[] array3 = new uint[Math.Max(array.Length, array2.Length)];
			uint num = (left._sign < 0) ? uint.MaxValue : 0U;
			uint num2 = (right._sign < 0) ? uint.MaxValue : 0U;
			for (int i = 0; i < array3.Length; i++)
			{
				uint num3 = (i < array.Length) ? array[i] : num;
				uint num4 = (i < array2.Length) ? array2[i] : num2;
				array3[i] = (num3 & num4);
			}
			return new BigInteger(array3);
		}

		/// <summary>Performs a bitwise <see langword="Or" /> operation on two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value.</param>
		/// <param name="right">The second value.</param>
		/// <returns>The result of the bitwise <see langword="Or" /> operation.</returns>
		public static BigInteger operator |(BigInteger left, BigInteger right)
		{
			if (left.IsZero)
			{
				return right;
			}
			if (right.IsZero)
			{
				return left;
			}
			if (left._bits == null && right._bits == null)
			{
				return left._sign | right._sign;
			}
			uint[] array = left.ToUInt32Array();
			uint[] array2 = right.ToUInt32Array();
			uint[] array3 = new uint[Math.Max(array.Length, array2.Length)];
			uint num = (left._sign < 0) ? uint.MaxValue : 0U;
			uint num2 = (right._sign < 0) ? uint.MaxValue : 0U;
			for (int i = 0; i < array3.Length; i++)
			{
				uint num3 = (i < array.Length) ? array[i] : num;
				uint num4 = (i < array2.Length) ? array2[i] : num2;
				array3[i] = (num3 | num4);
			}
			return new BigInteger(array3);
		}

		/// <summary>Performs a bitwise exclusive <see langword="Or" /> (<see langword="XOr" />) operation on two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value.</param>
		/// <param name="right">The second value.</param>
		/// <returns>The result of the bitwise <see langword="Or" /> operation.</returns>
		public static BigInteger operator ^(BigInteger left, BigInteger right)
		{
			if (left._bits == null && right._bits == null)
			{
				return left._sign ^ right._sign;
			}
			uint[] array = left.ToUInt32Array();
			uint[] array2 = right.ToUInt32Array();
			uint[] array3 = new uint[Math.Max(array.Length, array2.Length)];
			uint num = (left._sign < 0) ? uint.MaxValue : 0U;
			uint num2 = (right._sign < 0) ? uint.MaxValue : 0U;
			for (int i = 0; i < array3.Length; i++)
			{
				uint num3 = (i < array.Length) ? array[i] : num;
				uint num4 = (i < array2.Length) ? array2[i] : num2;
				array3[i] = (num3 ^ num4);
			}
			return new BigInteger(array3);
		}

		/// <summary>Shifts a <see cref="T:System.Numerics.BigInteger" /> value a specified number of bits to the left.</summary>
		/// <param name="value">The value whose bits are to be shifted.</param>
		/// <param name="shift">The number of bits to shift <paramref name="value" /> to the left.</param>
		/// <returns>A value that has been shifted to the left by the specified number of bits.</returns>
		public static BigInteger operator <<(BigInteger value, int shift)
		{
			if (shift == 0)
			{
				return value;
			}
			if (shift == -2147483648)
			{
				return value >> int.MaxValue >> 1;
			}
			if (shift < 0)
			{
				return value >> -shift;
			}
			int num = shift / 32;
			int num2 = shift - num * 32;
			uint[] array;
			int num3;
			bool partsForBitManipulation = BigInteger.GetPartsForBitManipulation(ref value, out array, out num3);
			uint[] array2 = new uint[num3 + num + 1];
			if (num2 == 0)
			{
				for (int i = 0; i < num3; i++)
				{
					array2[i + num] = array[i];
				}
			}
			else
			{
				int num4 = 32 - num2;
				uint num5 = 0U;
				int j;
				for (j = 0; j < num3; j++)
				{
					uint num6 = array[j];
					array2[j + num] = (num6 << num2 | num5);
					num5 = num6 >> num4;
				}
				array2[j + num] = num5;
			}
			return new BigInteger(array2, partsForBitManipulation);
		}

		/// <summary>Shifts a <see cref="T:System.Numerics.BigInteger" /> value a specified number of bits to the right.</summary>
		/// <param name="value">The value whose bits are to be shifted.</param>
		/// <param name="shift">The number of bits to shift <paramref name="value" /> to the right.</param>
		/// <returns>A value that has been shifted to the right by the specified number of bits.</returns>
		public static BigInteger operator >>(BigInteger value, int shift)
		{
			if (shift == 0)
			{
				return value;
			}
			if (shift == -2147483648)
			{
				return value << int.MaxValue << 1;
			}
			if (shift < 0)
			{
				return value << -shift;
			}
			int num = shift / 32;
			int num2 = shift - num * 32;
			uint[] array;
			int num3;
			bool partsForBitManipulation = BigInteger.GetPartsForBitManipulation(ref value, out array, out num3);
			if (partsForBitManipulation)
			{
				if (shift >= 32 * num3)
				{
					return BigInteger.MinusOne;
				}
				uint[] array2 = new uint[num3];
				Array.Copy(array, 0, array2, 0, num3);
				array = array2;
				NumericsHelpers.DangerousMakeTwosComplement(array);
			}
			int num4 = num3 - num;
			if (num4 < 0)
			{
				num4 = 0;
			}
			uint[] array3 = new uint[num4];
			if (num2 == 0)
			{
				for (int i = num3 - 1; i >= num; i--)
				{
					array3[i - num] = array[i];
				}
			}
			else
			{
				int num5 = 32 - num2;
				uint num6 = 0U;
				for (int j = num3 - 1; j >= num; j--)
				{
					uint num7 = array[j];
					if (partsForBitManipulation && j == num3 - 1)
					{
						array3[j - num] = (num7 >> num2 | uint.MaxValue << num5);
					}
					else
					{
						array3[j - num] = (num7 >> num2 | num6);
					}
					num6 = num7 << num5;
				}
			}
			if (partsForBitManipulation)
			{
				NumericsHelpers.DangerousMakeTwosComplement(array3);
			}
			return new BigInteger(array3, partsForBitManipulation);
		}

		/// <summary>Returns the bitwise one's complement of a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="value">An integer value.</param>
		/// <returns>The bitwise one's complement of <paramref name="value" />.</returns>
		public static BigInteger operator ~(BigInteger value)
		{
			return -(value + BigInteger.One);
		}

		/// <summary>Negates a specified BigInteger value.</summary>
		/// <param name="value">The value to negate.</param>
		/// <returns>The result of the <paramref name="value" /> parameter multiplied by negative one (-1).</returns>
		public static BigInteger operator -(BigInteger value)
		{
			return new BigInteger(-value._sign, value._bits);
		}

		/// <summary>Returns the value of the <see cref="T:System.Numerics.BigInteger" /> operand. (The sign of the operand is unchanged.)</summary>
		/// <param name="value">An integer value.</param>
		/// <returns>The value of the <paramref name="value" /> operand.</returns>
		public static BigInteger operator +(BigInteger value)
		{
			return value;
		}

		/// <summary>Increments a <see cref="T:System.Numerics.BigInteger" /> value by 1.</summary>
		/// <param name="value">The value to increment.</param>
		/// <returns>The value of the <paramref name="value" /> parameter incremented by 1.</returns>
		public static BigInteger operator ++(BigInteger value)
		{
			return value + BigInteger.One;
		}

		/// <summary>Decrements a <see cref="T:System.Numerics.BigInteger" /> value by 1.</summary>
		/// <param name="value">The value to decrement.</param>
		/// <returns>The value of the <paramref name="value" /> parameter decremented by 1.</returns>
		public static BigInteger operator --(BigInteger value)
		{
			return value - BigInteger.One;
		}

		/// <summary>Adds the values of two specified <see cref="T:System.Numerics.BigInteger" /> objects.</summary>
		/// <param name="left">The first value to add.</param>
		/// <param name="right">The second value to add.</param>
		/// <returns>The sum of <paramref name="left" /> and <paramref name="right" />.</returns>
		public static BigInteger operator +(BigInteger left, BigInteger right)
		{
			if (left._sign < 0 != right._sign < 0)
			{
				return BigInteger.Subtract(left._bits, left._sign, right._bits, -1 * right._sign);
			}
			return BigInteger.Add(left._bits, left._sign, right._bits, right._sign);
		}

		/// <summary>Multiplies two specified <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="left">The first value to multiply.</param>
		/// <param name="right">The second value to multiply.</param>
		/// <returns>The product of <paramref name="left" /> and <paramref name="right" />.</returns>
		public static BigInteger operator *(BigInteger left, BigInteger right)
		{
			bool flag = left._bits == null;
			bool flag2 = right._bits == null;
			if (flag && flag2)
			{
				return (long)left._sign * (long)right._sign;
			}
			if (flag)
			{
				return new BigInteger(BigIntegerCalculator.Multiply(right._bits, NumericsHelpers.Abs(left._sign)), left._sign < 0 ^ right._sign < 0);
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Multiply(left._bits, NumericsHelpers.Abs(right._sign)), left._sign < 0 ^ right._sign < 0);
			}
			if (left._bits == right._bits)
			{
				return new BigInteger(BigIntegerCalculator.Square(left._bits), left._sign < 0 ^ right._sign < 0);
			}
			if (left._bits.Length < right._bits.Length)
			{
				return new BigInteger(BigIntegerCalculator.Multiply(right._bits, left._bits), left._sign < 0 ^ right._sign < 0);
			}
			return new BigInteger(BigIntegerCalculator.Multiply(left._bits, right._bits), left._sign < 0 ^ right._sign < 0);
		}

		/// <summary>Divides a specified <see cref="T:System.Numerics.BigInteger" /> value by another specified <see cref="T:System.Numerics.BigInteger" /> value by using integer division.</summary>
		/// <param name="dividend">The value to be divided.</param>
		/// <param name="divisor">The value to divide by.</param>
		/// <returns>The integral result of the division.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="divisor" /> is 0 (zero).</exception>
		public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
		{
			bool flag = dividend._bits == null;
			bool flag2 = divisor._bits == null;
			if (flag && flag2)
			{
				return dividend._sign / divisor._sign;
			}
			if (flag)
			{
				return BigInteger.s_bnZeroInt;
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Divide(dividend._bits, NumericsHelpers.Abs(divisor._sign)), dividend._sign < 0 ^ divisor._sign < 0);
			}
			if (dividend._bits.Length < divisor._bits.Length)
			{
				return BigInteger.s_bnZeroInt;
			}
			return new BigInteger(BigIntegerCalculator.Divide(dividend._bits, divisor._bits), dividend._sign < 0 ^ divisor._sign < 0);
		}

		/// <summary>Returns the remainder that results from division with two specified <see cref="T:System.Numerics.BigInteger" /> values.</summary>
		/// <param name="dividend">The value to be divided.</param>
		/// <param name="divisor">The value to divide by.</param>
		/// <returns>The remainder that results from the division.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="divisor" /> is 0 (zero).</exception>
		public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
		{
			bool flag = dividend._bits == null;
			bool flag2 = divisor._bits == null;
			if (flag && flag2)
			{
				return dividend._sign % divisor._sign;
			}
			if (flag)
			{
				return dividend;
			}
			if (flag2)
			{
				uint num = BigIntegerCalculator.Remainder(dividend._bits, NumericsHelpers.Abs(divisor._sign));
				return (long)((dividend._sign < 0) ? (ulong.MaxValue * (ulong)num) : ((ulong)num));
			}
			if (dividend._bits.Length < divisor._bits.Length)
			{
				return dividend;
			}
			return new BigInteger(BigIntegerCalculator.Remainder(dividend._bits, divisor._bits), dividend._sign < 0);
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator <(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than or equal to another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator <=(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) <= 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator >(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than or equal to another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator >=(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) >= 0;
		}

		/// <summary>Returns a value that indicates whether the values of two <see cref="T:System.Numerics.BigInteger" /> objects are equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(BigInteger left, BigInteger right)
		{
			return left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether two <see cref="T:System.Numerics.BigInteger" /> objects have different values.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(BigInteger left, BigInteger right)
		{
			return !left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than a 64-bit signed integer.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator <(BigInteger left, long right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than or equal to a 64-bit signed integer.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator <=(BigInteger left, long right)
		{
			return left.CompareTo(right) <= 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> is greater than a 64-bit signed integer value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator >(BigInteger left, long right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than or equal to a 64-bit signed integer value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator >=(BigInteger left, long right)
		{
			return left.CompareTo(right) >= 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and a signed long integer value are equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(BigInteger left, long right)
		{
			return left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and a 64-bit signed integer are not equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(BigInteger left, long right)
		{
			return !left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether a 64-bit signed integer is less than a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator <(long left, BigInteger right)
		{
			return right.CompareTo(left) > 0;
		}

		/// <summary>Returns a value that indicates whether a 64-bit signed integer is less than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator <=(long left, BigInteger right)
		{
			return right.CompareTo(left) >= 0;
		}

		/// <summary>Returns a value that indicates whether a 64-bit signed integer is greater than a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator >(long left, BigInteger right)
		{
			return right.CompareTo(left) < 0;
		}

		/// <summary>Returns a value that indicates whether a 64-bit signed integer is greater than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator >=(long left, BigInteger right)
		{
			return right.CompareTo(left) <= 0;
		}

		/// <summary>Returns a value that indicates whether a signed long integer value and a <see cref="T:System.Numerics.BigInteger" /> value are equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(long left, BigInteger right)
		{
			return right.Equals(left);
		}

		/// <summary>Returns a value that indicates whether a 64-bit signed integer and a <see cref="T:System.Numerics.BigInteger" /> value are not equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(long left, BigInteger right)
		{
			return !right.Equals(left);
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than a 64-bit unsigned integer.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator <(BigInteger left, ulong right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than or equal to a 64-bit unsigned integer.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator <=(BigInteger left, ulong right)
		{
			return left.CompareTo(right) <= 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than a 64-bit unsigned integer.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator >(BigInteger left, ulong right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than or equal to a 64-bit unsigned integer value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator >=(BigInteger left, ulong right)
		{
			return left.CompareTo(right) >= 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and an unsigned long integer value are equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator ==(BigInteger left, ulong right)
		{
			return left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and a 64-bit unsigned integer are not equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator !=(BigInteger left, ulong right)
		{
			return !left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether a 64-bit unsigned integer is less than a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator <(ulong left, BigInteger right)
		{
			return right.CompareTo(left) > 0;
		}

		/// <summary>Returns a value that indicates whether a 64-bit unsigned integer is less than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator <=(ulong left, BigInteger right)
		{
			return right.CompareTo(left) >= 0;
		}

		/// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than a 64-bit unsigned integer.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator >(ulong left, BigInteger right)
		{
			return right.CompareTo(left) < 0;
		}

		/// <summary>Returns a value that indicates whether a 64-bit unsigned integer is greater than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator >=(ulong left, BigInteger right)
		{
			return right.CompareTo(left) <= 0;
		}

		/// <summary>Returns a value that indicates whether an unsigned long integer value and a <see cref="T:System.Numerics.BigInteger" /> value are equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator ==(ulong left, BigInteger right)
		{
			return right.Equals(left);
		}

		/// <summary>Returns a value that indicates whether a 64-bit unsigned integer and a <see cref="T:System.Numerics.BigInteger" /> value are not equal.</summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool operator !=(ulong left, BigInteger right)
		{
			return !right.Equals(left);
		}

		private static bool GetPartsForBitManipulation(ref BigInteger x, out uint[] xd, out int xl)
		{
			if (x._bits == null)
			{
				if (x._sign < 0)
				{
					xd = new uint[]
					{
						(uint)(-(uint)x._sign)
					};
				}
				else
				{
					xd = new uint[]
					{
						(uint)x._sign
					};
				}
			}
			else
			{
				xd = x._bits;
			}
			xl = ((x._bits == null) ? 1 : x._bits.Length);
			return x._sign < 0;
		}

		internal static int GetDiffLength(uint[] rgu1, uint[] rgu2, int cu)
		{
			int num = cu;
			while (--num >= 0)
			{
				if (rgu1[num] != rgu2[num])
				{
					return num + 1;
				}
			}
			return 0;
		}

		[Conditional("DEBUG")]
		private void AssertValid()
		{
			uint[] bits = this._bits;
		}

		private const int knMaskHighBit = -2147483648;

		private const uint kuMaskHighBit = 2147483648U;

		private const int kcbitUint = 32;

		private const int kcbitUlong = 64;

		private const int DecimalScaleFactorMask = 16711680;

		private const int DecimalSignMask = -2147483648;

		internal readonly int _sign;

		internal readonly uint[] _bits;

		private static readonly BigInteger s_bnMinInt = new BigInteger(-1, new uint[]
		{
			2147483648U
		});

		private static readonly BigInteger s_bnOneInt = new BigInteger(1);

		private static readonly BigInteger s_bnZeroInt = new BigInteger(0);

		private static readonly BigInteger s_bnMinusOneInt = new BigInteger(-1);

		private static readonly byte[] s_success = Array.Empty<byte>();

		private enum GetBytesMode
		{
			AllocateArray,
			Count,
			Span
		}
	}
}
