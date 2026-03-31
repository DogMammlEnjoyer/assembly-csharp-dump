using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class SequenceEqual
	{
		internal static UniTask<bool> SequenceEqualAsync<TSource>(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)
		{
			SequenceEqual.<SequenceEqualAsync>d__0<TSource> <SequenceEqualAsync>d__;
			<SequenceEqualAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<SequenceEqualAsync>d__.first = first;
			<SequenceEqualAsync>d__.second = second;
			<SequenceEqualAsync>d__.comparer = comparer;
			<SequenceEqualAsync>d__.cancellationToken = cancellationToken;
			<SequenceEqualAsync>d__.<>1__state = -1;
			<SequenceEqualAsync>d__.<>t__builder.Start<SequenceEqual.<SequenceEqualAsync>d__0<TSource>>(ref <SequenceEqualAsync>d__);
			return <SequenceEqualAsync>d__.<>t__builder.Task;
		}
	}
}
