using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Provides a type converter to convert 8-bit unsigned integer objects to and from a string.</summary>
	public class SByteConverter : BaseNumberConverter
	{
		internal override Type TargetType
		{
			get
			{
				return typeof(sbyte);
			}
		}

		internal override object FromString(string value, int radix)
		{
			return Convert.ToSByte(value, radix);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return sbyte.Parse(value, NumberStyles.Integer, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((sbyte)value).ToString("G", formatInfo);
		}
	}
}
