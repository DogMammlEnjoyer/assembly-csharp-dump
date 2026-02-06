using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	public static class CollectionExtensions
	{
		public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
		{
			foreach (T item in ts)
			{
				collection.Add(item);
			}
		}

		public static void CopyStringKeepDelimiterAtEnd(this HashSet<string> hash, string str, char delimiter)
		{
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			int i = 0;
			int num = 0;
			int length = str.Length;
			while (i < length)
			{
				if (str[i] == delimiter)
				{
					hash.Add(str.Substring(num, i - num));
					num = i + 1;
				}
				i++;
			}
		}
	}
}
