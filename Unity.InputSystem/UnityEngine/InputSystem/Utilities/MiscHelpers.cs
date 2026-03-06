using System;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class MiscHelpers
	{
		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue result;
			if (!dictionary.TryGetValue(key, out result))
			{
				return default(TValue);
			}
			return result;
		}

		public static IEnumerable<TValue> EveryNth<TValue>(this IEnumerable<TValue> enumerable, int n, int start = 0)
		{
			int index = 0;
			foreach (TValue tvalue in enumerable)
			{
				if (index < start)
				{
					int num = index + 1;
					index = num;
				}
				else
				{
					if ((index - start) % n == 0)
					{
						yield return tvalue;
					}
					int num = index + 1;
					index = num;
				}
			}
			IEnumerator<TValue> enumerator = null;
			yield break;
			yield break;
		}

		public static int IndexOf<TValue>(this IEnumerable<TValue> enumerable, TValue value)
		{
			int num = 0;
			foreach (TValue x in enumerable)
			{
				if (EqualityComparer<TValue>.Default.Equals(x, value))
				{
					return num;
				}
				num++;
			}
			return -1;
		}
	}
}
