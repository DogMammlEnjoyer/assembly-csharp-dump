using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Provides a type converter to convert 64-bit unsigned integer objects to and from other representations.</summary>
	public class UInt64Converter : BaseNumberConverter
	{
		internal override Type TargetType
		{
			get
			{
				return typeof(ulong);
			}
		}

		internal override object FromString(string value, int radix)
		{
			return Convert.ToUInt64(value, radix);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return ulong.Parse(value, NumberStyles.Integer, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((ulong)value).ToString("G", formatInfo);
		}
	}
}
