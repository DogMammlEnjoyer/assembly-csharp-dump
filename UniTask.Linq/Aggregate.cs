using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Aggregate
	{
		internal static UniTask<TSource> AggregateAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TSource, TSource> accumulator, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAsync>d__0<TSource> <AggregateAsync>d__;
			<AggregateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<AggregateAsync>d__.source = source;
			<AggregateAsync>d__.accumulator = accumulator;
			<AggregateAsync>d__.cancellationToken = cancellationToken;
			<AggregateAsync>d__.<>1__state = -1;
			<AggregateAsync>d__.<>t__builder.Start<Aggregate.<AggregateAsync>d__0<TSource>>(ref <AggregateAsync>d__);
			return <AggregateAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAsync>d__1<TSource, TAccumulate> <AggregateAsync>d__;
			<AggregateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TAccumulate>.Create();
			<AggregateAsync>d__.source = source;
			<AggregateAsync>d__.seed = seed;
			<AggregateAsync>d__.accumulator = accumulator;
			<AggregateAsync>d__.cancellationToken = cancellationToken;
			<AggregateAsync>d__.<>1__state = -1;
			<AggregateAsync>d__.<>t__builder.Start<Aggregate.<AggregateAsync>d__1<TSource, TAccumulate>>(ref <AggregateAsync>d__);
			return <AggregateAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TResult> AggregateAsync<TSource, TAccumulate, TResult>(IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAsync>d__2<TSource, TAccumulate, TResult> <AggregateAsync>d__;
			<AggregateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<AggregateAsync>d__.source = source;
			<AggregateAsync>d__.seed = seed;
			<AggregateAsync>d__.accumulator = accumulator;
			<AggregateAsync>d__.resultSelector = resultSelector;
			<AggregateAsync>d__.cancellationToken = cancellationToken;
			<AggregateAsync>d__.<>1__state = -1;
			<AggregateAsync>d__.<>t__builder.Start<Aggregate.<AggregateAsync>d__2<TSource, TAccumulate, TResult>>(ref <AggregateAsync>d__);
			return <AggregateAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TSource> AggregateAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TSource, UniTask<TSource>> accumulator, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAwaitAsync>d__3<TSource> <AggregateAwaitAsync>d__;
			<AggregateAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<AggregateAwaitAsync>d__.source = source;
			<AggregateAwaitAsync>d__.accumulator = accumulator;
			<AggregateAwaitAsync>d__.cancellationToken = cancellationToken;
			<AggregateAwaitAsync>d__.<>1__state = -1;
			<AggregateAwaitAsync>d__.<>t__builder.Start<Aggregate.<AggregateAwaitAsync>d__3<TSource>>(ref <AggregateAwaitAsync>d__);
			return <AggregateAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, UniTask<TAccumulate>> accumulator, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAwaitAsync>d__4<TSource, TAccumulate> <AggregateAwaitAsync>d__;
			<AggregateAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TAccumulate>.Create();
			<AggregateAwaitAsync>d__.source = source;
			<AggregateAwaitAsync>d__.seed = seed;
			<AggregateAwaitAsync>d__.accumulator = accumulator;
			<AggregateAwaitAsync>d__.cancellationToken = cancellationToken;
			<AggregateAwaitAsync>d__.<>1__state = -1;
			<AggregateAwaitAsync>d__.<>t__builder.Start<Aggregate.<AggregateAwaitAsync>d__4<TSource, TAccumulate>>(ref <AggregateAwaitAsync>d__);
			return <AggregateAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TResult> AggregateAwaitAsync<TSource, TAccumulate, TResult>(IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, UniTask<TAccumulate>> accumulator, Func<TAccumulate, UniTask<TResult>> resultSelector, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAwaitAsync>d__5<TSource, TAccumulate, TResult> <AggregateAwaitAsync>d__;
			<AggregateAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<AggregateAwaitAsync>d__.source = source;
			<AggregateAwaitAsync>d__.seed = seed;
			<AggregateAwaitAsync>d__.accumulator = accumulator;
			<AggregateAwaitAsync>d__.resultSelector = resultSelector;
			<AggregateAwaitAsync>d__.cancellationToken = cancellationToken;
			<AggregateAwaitAsync>d__.<>1__state = -1;
			<AggregateAwaitAsync>d__.<>t__builder.Start<Aggregate.<AggregateAwaitAsync>d__5<TSource, TAccumulate, TResult>>(ref <AggregateAwaitAsync>d__);
			return <AggregateAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TSource> AggregateAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TSource, CancellationToken, UniTask<TSource>> accumulator, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAwaitWithCancellationAsync>d__6<TSource> <AggregateAwaitWithCancellationAsync>d__;
			<AggregateAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<AggregateAwaitWithCancellationAsync>d__.source = source;
			<AggregateAwaitWithCancellationAsync>d__.accumulator = accumulator;
			<AggregateAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AggregateAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AggregateAwaitWithCancellationAsync>d__.<>t__builder.Start<Aggregate.<AggregateAwaitWithCancellationAsync>d__6<TSource>>(ref <AggregateAwaitWithCancellationAsync>d__);
			return <AggregateAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TAccumulate> AggregateAwaitWithCancellationAsync<TSource, TAccumulate>(IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>> accumulator, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAwaitWithCancellationAsync>d__7<TSource, TAccumulate> <AggregateAwaitWithCancellationAsync>d__;
			<AggregateAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TAccumulate>.Create();
			<AggregateAwaitWithCancellationAsync>d__.source = source;
			<AggregateAwaitWithCancellationAsync>d__.seed = seed;
			<AggregateAwaitWithCancellationAsync>d__.accumulator = accumulator;
			<AggregateAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AggregateAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AggregateAwaitWithCancellationAsync>d__.<>t__builder.Start<Aggregate.<AggregateAwaitWithCancellationAsync>d__7<TSource, TAccumulate>>(ref <AggregateAwaitWithCancellationAsync>d__);
			return <AggregateAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<TResult> AggregateAwaitWithCancellationAsync<TSource, TAccumulate, TResult>(IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>> accumulator, Func<TAccumulate, CancellationToken, UniTask<TResult>> resultSelector, CancellationToken cancellationToken)
		{
			Aggregate.<AggregateAwaitWithCancellationAsync>d__8<TSource, TAccumulate, TResult> <AggregateAwaitWithCancellationAsync>d__;
			<AggregateAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<AggregateAwaitWithCancellationAsync>d__.source = source;
			<AggregateAwaitWithCancellationAsync>d__.seed = seed;
			<AggregateAwaitWithCancellationAsync>d__.accumulator = accumulator;
			<AggregateAwaitWithCancellationAsync>d__.resultSelector = resultSelector;
			<AggregateAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AggregateAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AggregateAwaitWithCancellationAsync>d__.<>t__builder.Start<Aggregate.<AggregateAwaitWithCancellationAsync>d__8<TSource, TAccumulate, TResult>>(ref <AggregateAwaitWithCancellationAsync>d__);
			return <AggregateAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
