using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class SingleOperator
	{
		public static UniTask<TSource> SingleAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			SingleOperator.<SingleAsync>d__0<TSource> <SingleAsync>d__;
			<SingleAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<SingleAsync>d__.source = source;
			<SingleAsync>d__.cancellationToken = cancellationToken;
			<SingleAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<SingleAsync>d__.<>1__state = -1;
			<SingleAsync>d__.<>t__builder.Start<SingleOperator.<SingleAsync>d__0<TSource>>(ref <SingleAsync>d__);
			return <SingleAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> SingleAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			SingleOperator.<SingleAsync>d__1<TSource> <SingleAsync>d__;
			<SingleAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<SingleAsync>d__.source = source;
			<SingleAsync>d__.predicate = predicate;
			<SingleAsync>d__.cancellationToken = cancellationToken;
			<SingleAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<SingleAsync>d__.<>1__state = -1;
			<SingleAsync>d__.<>t__builder.Start<SingleOperator.<SingleAsync>d__1<TSource>>(ref <SingleAsync>d__);
			return <SingleAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> SingleAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			SingleOperator.<SingleAwaitAsync>d__2<TSource> <SingleAwaitAsync>d__;
			<SingleAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<SingleAwaitAsync>d__.source = source;
			<SingleAwaitAsync>d__.predicate = predicate;
			<SingleAwaitAsync>d__.cancellationToken = cancellationToken;
			<SingleAwaitAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<SingleAwaitAsync>d__.<>1__state = -1;
			<SingleAwaitAsync>d__.<>t__builder.Start<SingleOperator.<SingleAwaitAsync>d__2<TSource>>(ref <SingleAwaitAsync>d__);
			return <SingleAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> SingleAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			SingleOperator.<SingleAwaitWithCancellationAsync>d__3<TSource> <SingleAwaitWithCancellationAsync>d__;
			<SingleAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<SingleAwaitWithCancellationAsync>d__.source = source;
			<SingleAwaitWithCancellationAsync>d__.predicate = predicate;
			<SingleAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SingleAwaitWithCancellationAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<SingleAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SingleAwaitWithCancellationAsync>d__.<>t__builder.Start<SingleOperator.<SingleAwaitWithCancellationAsync>d__3<TSource>>(ref <SingleAwaitWithCancellationAsync>d__);
			return <SingleAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
