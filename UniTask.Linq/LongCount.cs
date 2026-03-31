using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class LongCount
	{
		internal static UniTask<long> LongCountAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			LongCount.<LongCountAsync>d__0<TSource> <LongCountAsync>d__;
			<LongCountAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<LongCountAsync>d__.source = source;
			<LongCountAsync>d__.cancellationToken = cancellationToken;
			<LongCountAsync>d__.<>1__state = -1;
			<LongCountAsync>d__.<>t__builder.Start<LongCount.<LongCountAsync>d__0<TSource>>(ref <LongCountAsync>d__);
			return <LongCountAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<long> LongCountAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
		{
			LongCount.<LongCountAsync>d__1<TSource> <LongCountAsync>d__;
			<LongCountAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<LongCountAsync>d__.source = source;
			<LongCountAsync>d__.predicate = predicate;
			<LongCountAsync>d__.cancellationToken = cancellationToken;
			<LongCountAsync>d__.<>1__state = -1;
			<LongCountAsync>d__.<>t__builder.Start<LongCount.<LongCountAsync>d__1<TSource>>(ref <LongCountAsync>d__);
			return <LongCountAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<long> LongCountAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken)
		{
			LongCount.<LongCountAwaitAsync>d__2<TSource> <LongCountAwaitAsync>d__;
			<LongCountAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<LongCountAwaitAsync>d__.source = source;
			<LongCountAwaitAsync>d__.predicate = predicate;
			<LongCountAwaitAsync>d__.cancellationToken = cancellationToken;
			<LongCountAwaitAsync>d__.<>1__state = -1;
			<LongCountAwaitAsync>d__.<>t__builder.Start<LongCount.<LongCountAwaitAsync>d__2<TSource>>(ref <LongCountAwaitAsync>d__);
			return <LongCountAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<long> LongCountAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken)
		{
			LongCount.<LongCountAwaitWithCancellationAsync>d__3<TSource> <LongCountAwaitWithCancellationAsync>d__;
			<LongCountAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<LongCountAwaitWithCancellationAsync>d__.source = source;
			<LongCountAwaitWithCancellationAsync>d__.predicate = predicate;
			<LongCountAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<LongCountAwaitWithCancellationAsync>d__.<>1__state = -1;
			<LongCountAwaitWithCancellationAsync>d__.<>t__builder.Start<LongCount.<LongCountAwaitWithCancellationAsync>d__3<TSource>>(ref <LongCountAwaitWithCancellationAsync>d__);
			return <LongCountAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
