using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class BoxedPrimitives
	{
		internal static object Get(bool value)
		{
			if (!value)
			{
				return BoxedPrimitives.BooleanFalse;
			}
			return BoxedPrimitives.BooleanTrue;
		}

		internal static object Get(int value)
		{
			object result;
			switch (value)
			{
			case -1:
				result = BoxedPrimitives.Int32_M1;
				break;
			case 0:
				result = BoxedPrimitives.Int32_0;
				break;
			case 1:
				result = BoxedPrimitives.Int32_1;
				break;
			case 2:
				result = BoxedPrimitives.Int32_2;
				break;
			case 3:
				result = BoxedPrimitives.Int32_3;
				break;
			case 4:
				result = BoxedPrimitives.Int32_4;
				break;
			case 5:
				result = BoxedPrimitives.Int32_5;
				break;
			case 6:
				result = BoxedPrimitives.Int32_6;
				break;
			case 7:
				result = BoxedPrimitives.Int32_7;
				break;
			case 8:
				result = BoxedPrimitives.Int32_8;
				break;
			default:
				result = value;
				break;
			}
			return result;
		}

		internal static object Get(long value)
		{
			long num = value - -1L;
			if (num <= 9L)
			{
				switch ((uint)num)
				{
				case 0U:
					return BoxedPrimitives.Int64_M1;
				case 1U:
					return BoxedPrimitives.Int64_0;
				case 2U:
					return BoxedPrimitives.Int64_1;
				case 3U:
					return BoxedPrimitives.Int64_2;
				case 4U:
					return BoxedPrimitives.Int64_3;
				case 5U:
					return BoxedPrimitives.Int64_4;
				case 6U:
					return BoxedPrimitives.Int64_5;
				case 7U:
					return BoxedPrimitives.Int64_6;
				case 8U:
					return BoxedPrimitives.Int64_7;
				case 9U:
					return BoxedPrimitives.Int64_8;
				}
			}
			return value;
		}

		internal static object Get(decimal value)
		{
			if (!(value == 0m))
			{
				return value;
			}
			return BoxedPrimitives.DecimalZero;
		}

		internal static object Get(double value)
		{
			if (value == 0.0)
			{
				return BoxedPrimitives.DoubleZero;
			}
			if (double.IsInfinity(value))
			{
				if (!double.IsPositiveInfinity(value))
				{
					return BoxedPrimitives.DoubleNegativeInfinity;
				}
				return BoxedPrimitives.DoublePositiveInfinity;
			}
			else
			{
				if (double.IsNaN(value))
				{
					return BoxedPrimitives.DoubleNaN;
				}
				return value;
			}
		}

		internal static readonly object BooleanTrue = true;

		internal static readonly object BooleanFalse = false;

		internal static readonly object Int32_M1 = -1;

		internal static readonly object Int32_0 = 0;

		internal static readonly object Int32_1 = 1;

		internal static readonly object Int32_2 = 2;

		internal static readonly object Int32_3 = 3;

		internal static readonly object Int32_4 = 4;

		internal static readonly object Int32_5 = 5;

		internal static readonly object Int32_6 = 6;

		internal static readonly object Int32_7 = 7;

		internal static readonly object Int32_8 = 8;

		internal static readonly object Int64_M1 = -1L;

		internal static readonly object Int64_0 = 0L;

		internal static readonly object Int64_1 = 1L;

		internal static readonly object Int64_2 = 2L;

		internal static readonly object Int64_3 = 3L;

		internal static readonly object Int64_4 = 4L;

		internal static readonly object Int64_5 = 5L;

		internal static readonly object Int64_6 = 6L;

		internal static readonly object Int64_7 = 7L;

		internal static readonly object Int64_8 = 8L;

		private static readonly object DecimalZero = 0m;

		internal static readonly object DoubleNaN = double.NaN;

		internal static readonly object DoublePositiveInfinity = double.PositiveInfinity;

		internal static readonly object DoubleNegativeInfinity = double.NegativeInfinity;

		internal static readonly object DoubleZero = 0.0;
	}
}
