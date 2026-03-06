using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Provides a type converter to convert 32-bit unsigned integer objects to and from various other representations.</summary>
	public class UInt32Converter : BaseNumberConverter
	{
		internal override Type TargetType
		{
			get
			{
				return typeof(uint);
			}
		}

		internal override object FromString(string value, int radix)
		{
			return Convert.ToUInt32(value, radix);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return uint.Parse(value, NumberStyles.Integer, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((uint)value).ToString("G", formatInfo);
		}
	}
}
