using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Contains
	{
		internal static UniTask<bool> ContainsAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)
		{
			Contains.<ContainsAsync>d__0<TSource> <ContainsAsync>d__;
			<ContainsAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<ContainsAsync>d__.source = source;
			<ContainsAsync>d__.value = value;
			<ContainsAsync>d__.comparer = comparer;
			<ContainsAsync>d__.cancellationToken = cancellationToken;
			<ContainsAsync>d__.<>1__state = -1;
			<ContainsAsync>d__.<>t__builder.Start<Contains.<ContainsAsync>d__0<TSource>>(ref <ContainsAsync>d__);
			return <ContainsAsync>d__.<>t__builder.Task;
		}
	}
}
