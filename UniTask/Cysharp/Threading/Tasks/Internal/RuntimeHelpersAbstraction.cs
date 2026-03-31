using System;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class RuntimeHelpersAbstraction
	{
		public static bool IsWellKnownNoReferenceContainsType<T>()
		{
			return RuntimeHelpersAbstraction.WellKnownNoReferenceContainsType<T>.IsWellKnownType;
		}

		private static bool WellKnownNoReferenceContainsTypeInitialize(Type t)
		{
			if (t.IsPrimitive)
			{
				return true;
			}
			if (t.IsEnum)
			{
				return true;
			}
			if (t == typeof(DateTime))
			{
				return true;
			}
			if (t == typeof(DateTimeOffset))
			{
				return true;
			}
			if (t == typeof(Guid))
			{
				return true;
			}
			if (t == typeof(decimal))
			{
				return true;
			}
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return RuntimeHelpersAbstraction.WellKnownNoReferenceContainsTypeInitialize(t.GetGenericArguments()[0]);
			}
			return t == typeof(Vector2) || t == typeof(Vector3) || t == typeof(Vector4) || t == typeof(Color) || t == typeof(Rect) || t == typeof(Bounds) || t == typeof(Quaternion) || t == typeof(Vector2Int) || t == typeof(Vector3Int);
		}

		private static class WellKnownNoReferenceContainsType<T>
		{
			public static readonly bool IsWellKnownType = RuntimeHelpersAbstraction.WellKnownNoReferenceContainsTypeInitialize(typeof(T));
		}
	}
}
