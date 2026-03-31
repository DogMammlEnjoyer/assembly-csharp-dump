using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class ToHashSet
	{
		internal static UniTask<HashSet<TSource>> ToHashSetAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)
		{
			ToHashSet.<ToHashSetAsync>d__0<TSource> <ToHashSetAsync>d__;
			<ToHashSetAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<HashSet<TSource>>.Create();
			<ToHashSetAsync>d__.source = source;
			<ToHashSetAsync>d__.comparer = comparer;
			<ToHashSetAsync>d__.cancellationToken = cancellationToken;
			<ToHashSetAsync>d__.<>1__state = -1;
			<ToHashSetAsync>d__.<>t__builder.Start<ToHashSet.<ToHashSetAsync>d__0<TSource>>(ref <ToHashSetAsync>d__);
			return <ToHashSetAsync>d__.<>t__builder.Task;
		}
	}
}
