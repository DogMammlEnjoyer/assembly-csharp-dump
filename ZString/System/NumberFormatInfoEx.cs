using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class NumberFormatInfoEx
	{
		[NullableContext(1)]
		internal static bool HasInvariantNumberSigns(this NumberFormatInfo info)
		{
			return info.PositiveSign == "+" && info.NegativeSign == "-";
		}
	}
}
