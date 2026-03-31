using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Last
	{
		public static UniTask<TSource> LastAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			Last.<LastAsync>d__0<TSource> <LastAsync>d__;
			<LastAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<LastAsync>d__.source = source;
			<LastAsync>d__.cancellationToken = cancellationToken;
			<LastAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<LastAsync>d__.<>1__state = -1;
			<LastAsync>d__.<>t__builder.Start<Last.<LastAsync>d__0<TSource>>(ref <LastAsync>d__);
			return <LastAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> LastAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			Last.<LastAsync>d__1<TSource> <LastAsync>d__;
			<LastAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<LastAsync>d__.source = source;
			<LastAsync>d__.predicate = predicate;
			<LastAsync>d__.cancellationToken = cancellationToken;
			<LastAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<LastAsync>d__.<>1__state = -1;
			<LastAsync>d__.<>t__builder.Start<Last.<LastAsync>d__1<TSource>>(ref <LastAsync>d__);
			return <LastAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> LastAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			Last.<LastAwaitAsync>d__2<TSource> <LastAwaitAsync>d__;
			<LastAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<LastAwaitAsync>d__.source = source;
			<LastAwaitAsync>d__.predicate = predicate;
			<LastAwaitAsync>d__.cancellationToken = cancellationToken;
			<LastAwaitAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<LastAwaitAsync>d__.<>1__state = -1;
			<LastAwaitAsync>d__.<>t__builder.Start<Last.<LastAwaitAsync>d__2<TSource>>(ref <LastAwaitAsync>d__);
			return <LastAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> LastAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			Last.<LastAwaitWithCancellationAsync>d__3<TSource> <LastAwaitWithCancellationAsync>d__;
			<LastAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<LastAwaitWithCancellationAsync>d__.source = source;
			<LastAwaitWithCancellationAsync>d__.predicate = predicate;
			<LastAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<LastAwaitWithCancellationAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<LastAwaitWithCancellationAsync>d__.<>1__state = -1;
			<LastAwaitWithCancellationAsync>d__.<>t__builder.Start<Last.<LastAwaitWithCancellationAsync>d__3<TSource>>(ref <LastAwaitWithCancellationAsync>d__);
			return <LastAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
