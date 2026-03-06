using System;
using System.Globalization;

namespace System.ComponentModel
{
	/// <summary>Provides a type converter to convert 32-bit signed integer objects to and from other representations.</summary>
	public class Int32Converter : BaseNumberConverter
	{
		internal override Type TargetType
		{
			get
			{
				return typeof(int);
			}
		}

		internal override object FromString(string value, int radix)
		{
			return Convert.ToInt32(value, radix);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return int.Parse(value, NumberStyles.Integer, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((int)value).ToString("G", formatInfo);
		}
	}
}
