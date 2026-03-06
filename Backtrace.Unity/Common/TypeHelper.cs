using System;
using System.Collections.Generic;

namespace Backtrace.Unity.Common
{
	public static class TypeHelper
	{
		public static bool IsNumeric(Type myType)
		{
			return TypeHelper.NumericTypes.Contains(Nullable.GetUnderlyingType(myType) ?? myType);
		}

		private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
		{
			typeof(int),
			typeof(double),
			typeof(decimal),
			typeof(long),
			typeof(short),
			typeof(sbyte),
			typeof(byte),
			typeof(ulong),
			typeof(ushort),
			typeof(uint),
			typeof(float)
		};
	}
}
