using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	public static class EnumerableExtensions
	{
		public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> ts, Func<TValue, TKey> keyGetter) where TKey : struct, IComparable<TKey>
		{
			TValue result = default(TValue);
			TKey? tkey = null;
			foreach (TValue tvalue in ts)
			{
				TKey value = keyGetter(tvalue);
				if (tkey == null || value.CompareTo(tkey.Value) < 0)
				{
					result = tvalue;
					tkey = new TKey?(value);
				}
			}
			if (tkey == null)
			{
				throw new ArgumentException("Cannot calculate MinBy on an empty IEnumerable.");
			}
			return result;
		}

		public static IEnumerable<T> Peek<T>(this IEnumerable<T> ts, Action<T> action)
		{
			foreach (T t in ts)
			{
				action(t);
				yield return t;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
