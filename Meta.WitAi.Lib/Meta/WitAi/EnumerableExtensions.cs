using System;
using System.Collections.Generic;
using System.Linq;

namespace Meta.WitAi
{
	internal static class EnumerableExtensions
	{
		internal static bool Equivalent<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return (first == null && second == null) || (first != null && second != null && first.SequenceEqual(second));
		}
	}
}
