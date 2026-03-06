using System;

namespace Backtrace.Unity.Extensions
{
	public static class EnumExtensions
	{
		internal static bool HasFlag(this Enum variable, Enum value)
		{
			if (variable.GetType() != value.GetType())
			{
				throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
			}
			TypeCode typeCode = variable.GetTypeCode();
			if (typeCode == TypeCode.SByte || typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64)
			{
				return (Convert.ToInt64(variable) & Convert.ToInt64(value)) != 0L;
			}
			return (typeCode == TypeCode.Byte || typeCode == TypeCode.UInt16 || typeCode == TypeCode.UInt32 || typeCode == TypeCode.UInt64) && (Convert.ToUInt64(variable) & Convert.ToUInt64(value)) > 0UL;
		}

		public static bool HasAllFlags<T>(this T rawSource)
		{
			Enum @enum = rawSource as Enum;
			foreach (object obj in Enum.GetValues(typeof(T)))
			{
				if (!@enum.HasFlag(((T)((object)obj)) as Enum))
				{
					return false;
				}
			}
			return true;
		}
	}
}
