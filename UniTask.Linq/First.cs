using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class First
	{
		public static UniTask<TSource> FirstAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			First.<FirstAsync>d__0<TSource> <FirstAsync>d__;
			<FirstAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<FirstAsync>d__.source = source;
			<FirstAsync>d__.cancellationToken = cancellationToken;
			<FirstAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<FirstAsync>d__.<>1__state = -1;
			<FirstAsync>d__.<>t__builder.Start<First.<FirstAsync>d__0<TSource>>(ref <FirstAsync>d__);
			return <FirstAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> FirstAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			First.<FirstAsync>d__1<TSource> <FirstAsync>d__;
			<FirstAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<FirstAsync>d__.source = source;
			<FirstAsync>d__.predicate = predicate;
			<FirstAsync>d__.cancellationToken = cancellationToken;
			<FirstAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<FirstAsync>d__.<>1__state = -1;
			<FirstAsync>d__.<>t__builder.Start<First.<FirstAsync>d__1<TSource>>(ref <FirstAsync>d__);
			return <FirstAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> FirstAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			First.<FirstAwaitAsync>d__2<TSource> <FirstAwaitAsync>d__;
			<FirstAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<FirstAwaitAsync>d__.source = source;
			<FirstAwaitAsync>d__.predicate = predicate;
			<FirstAwaitAsync>d__.cancellationToken = cancellationToken;
			<FirstAwaitAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<FirstAwaitAsync>d__.<>1__state = -1;
			<FirstAwaitAsync>d__.<>t__builder.Start<First.<FirstAwaitAsync>d__2<TSource>>(ref <FirstAwaitAsync>d__);
			return <FirstAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TSource> FirstAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken, bool defaultIfEmpty)
		{
			First.<FirstAwaitWithCancellationAsync>d__3<TSource> <FirstAwaitWithCancellationAsync>d__;
			<FirstAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<FirstAwaitWithCancellationAsync>d__.source = source;
			<FirstAwaitWithCancellationAsync>d__.predicate = predicate;
			<FirstAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<FirstAwaitWithCancellationAsync>d__.defaultIfEmpty = defaultIfEmpty;
			<FirstAwaitWithCancellationAsync>d__.<>1__state = -1;
			<FirstAwaitWithCancellationAsync>d__.<>t__builder.Start<First.<FirstAwaitWithCancellationAsync>d__3<TSource>>(ref <FirstAwaitWithCancellationAsync>d__);
			return <FirstAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
