using System;

namespace SouthPointe.Serialization.MessagePack
{
	internal static class CustomExtensions
	{
		internal static bool IsNullable(this Type type)
		{
			return type.IsValueType && Nullable.GetUnderlyingType(type) != null;
		}
	}
}
