using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Count
	{
		internal static UniTask<int> CountAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			Count.<CountAsync>d__0<TSource> <CountAsync>d__;
			<CountAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<CountAsync>d__.source = source;
			<CountAsync>d__.cancellationToken = cancellationToken;
			<CountAsync>d__.<>1__state = -1;
			<CountAsync>d__.<>t__builder.Start<Count.<CountAsync>d__0<TSource>>(ref <CountAsync>d__);
			return <CountAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<int> CountAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
		{
			Count.<CountAsync>d__1<TSource> <CountAsync>d__;
			<CountAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<CountAsync>d__.source = source;
			<CountAsync>d__.predicate = predicate;
			<CountAsync>d__.cancellationToken = cancellationToken;
			<CountAsync>d__.<>1__state = -1;
			<CountAsync>d__.<>t__builder.Start<Count.<CountAsync>d__1<TSource>>(ref <CountAsync>d__);
			return <CountAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<int> CountAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken)
		{
			Count.<CountAwaitAsync>d__2<TSource> <CountAwaitAsync>d__;
			<CountAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<CountAwaitAsync>d__.source = source;
			<CountAwaitAsync>d__.predicate = predicate;
			<CountAwaitAsync>d__.cancellationToken = cancellationToken;
			<CountAwaitAsync>d__.<>1__state = -1;
			<CountAwaitAsync>d__.<>t__builder.Start<Count.<CountAwaitAsync>d__2<TSource>>(ref <CountAwaitAsync>d__);
			return <CountAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<int> CountAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken)
		{
			Count.<CountAwaitWithCancellationAsync>d__3<TSource> <CountAwaitWithCancellationAsync>d__;
			<CountAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<CountAwaitWithCancellationAsync>d__.source = source;
			<CountAwaitWithCancellationAsync>d__.predicate = predicate;
			<CountAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<CountAwaitWithCancellationAsync>d__.<>1__state = -1;
			<CountAwaitWithCancellationAsync>d__.<>t__builder.Start<Count.<CountAwaitWithCancellationAsync>d__3<TSource>>(ref <CountAwaitWithCancellationAsync>d__);
			return <CountAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
