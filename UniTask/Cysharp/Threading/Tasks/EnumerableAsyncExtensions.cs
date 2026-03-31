using System;
using System.Collections.Generic;
using System.Linq;

namespace Cysharp.Threading.Tasks
{
	public static class EnumerableAsyncExtensions
	{
		public static IEnumerable<UniTask> Select<T>(this IEnumerable<T> source, Func<T, UniTask> selector)
		{
			return source.Select(selector);
		}

		public static IEnumerable<UniTask<TR>> Select<T, TR>(this IEnumerable<T> source, Func<T, UniTask<TR>> selector)
		{
			return source.Select(selector);
		}

		public static IEnumerable<UniTask> Select<T>(this IEnumerable<T> source, Func<T, int, UniTask> selector)
		{
			return source.Select(selector);
		}

		public static IEnumerable<UniTask<TR>> Select<T, TR>(this IEnumerable<T> source, Func<T, int, UniTask<TR>> selector)
		{
			return source.Select(selector);
		}
	}
}
