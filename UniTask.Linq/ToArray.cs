using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class ToArray
	{
		internal static UniTask<TSource[]> ToArrayAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			ToArray.<ToArrayAsync>d__0<TSource> <ToArrayAsync>d__;
			<ToArrayAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource[]>.Create();
			<ToArrayAsync>d__.source = source;
			<ToArrayAsync>d__.cancellationToken = cancellationToken;
			<ToArrayAsync>d__.<>1__state = -1;
			<ToArrayAsync>d__.<>t__builder.Start<ToArray.<ToArrayAsync>d__0<TSource>>(ref <ToArrayAsync>d__);
			return <ToArrayAsync>d__.<>t__builder.Task;
		}
	}
}
