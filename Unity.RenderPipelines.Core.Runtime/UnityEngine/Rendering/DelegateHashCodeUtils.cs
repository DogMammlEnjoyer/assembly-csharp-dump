using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	internal static class DelegateHashCodeUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetFuncHashCode(Delegate del)
		{
			int hashCode = RuntimeHelpers.GetHashCode(del.Method);
			bool flag;
			if (!DelegateHashCodeUtils.s_MethodHashCodeToSkipTargetHashMap.Value.TryGetValue(hashCode, out flag))
			{
				bool flag2;
				if (del.Target != null)
				{
					Type declaringType = del.Method.DeclaringType;
					flag2 = (declaringType != null && declaringType.IsNestedPrivate && Attribute.IsDefined(del.Method.DeclaringType, typeof(CompilerGeneratedAttribute), false));
				}
				else
				{
					flag2 = true;
				}
				flag = flag2;
				DelegateHashCodeUtils.s_MethodHashCodeToSkipTargetHashMap.Value[hashCode] = flag;
			}
			if (!flag)
			{
				return hashCode ^ RuntimeHelpers.GetHashCode(del.Target);
			}
			return hashCode;
		}

		internal static int GetTotalCacheCount()
		{
			return DelegateHashCodeUtils.s_MethodHashCodeToSkipTargetHashMap.Value.Count;
		}

		internal static void ClearCache()
		{
			DelegateHashCodeUtils.s_MethodHashCodeToSkipTargetHashMap.Value.Clear();
		}

		private static readonly Lazy<Dictionary<int, bool>> s_MethodHashCodeToSkipTargetHashMap = new Lazy<Dictionary<int, bool>>(() => new Dictionary<int, bool>(64));
	}
}
