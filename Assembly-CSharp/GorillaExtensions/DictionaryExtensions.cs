using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	public static class DictionaryExtensions
	{
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
		{
			TValue result;
			if (dict.TryGetValue(key, out result))
			{
				return result;
			}
			dict[key] = Activator.CreateInstance<TValue>();
			return dict[key];
		}
	}
}
