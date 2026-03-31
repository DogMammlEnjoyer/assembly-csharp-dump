using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Any
	{
		internal static UniTask<bool> AnyAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			Any.<AnyAsync>d__0<TSource> <AnyAsync>d__;
			<AnyAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<AnyAsync>d__.source = source;
			<AnyAsync>d__.cancellationToken = cancellationToken;
			<AnyAsync>d__.<>1__state = -1;
			<AnyAsync>d__.<>t__builder.Start<Any.<AnyAsync>d__0<TSource>>(ref <AnyAsync>d__);
			return <AnyAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<bool> AnyAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
		{
			Any.<AnyAsync>d__1<TSource> <AnyAsync>d__;
			<AnyAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<AnyAsync>d__.source = source;
			<AnyAsync>d__.predicate = predicate;
			<AnyAsync>d__.cancellationToken = cancellationToken;
			<AnyAsync>d__.<>1__state = -1;
			<AnyAsync>d__.<>t__builder.Start<Any.<AnyAsync>d__1<TSource>>(ref <AnyAsync>d__);
			return <AnyAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<bool> AnyAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken)
		{
			Any.<AnyAwaitAsync>d__2<TSource> <AnyAwaitAsync>d__;
			<AnyAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<AnyAwaitAsync>d__.source = source;
			<AnyAwaitAsync>d__.predicate = predicate;
			<AnyAwaitAsync>d__.cancellationToken = cancellationToken;
			<AnyAwaitAsync>d__.<>1__state = -1;
			<AnyAwaitAsync>d__.<>t__builder.Start<Any.<AnyAwaitAsync>d__2<TSource>>(ref <AnyAwaitAsync>d__);
			return <AnyAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<bool> AnyAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken)
		{
			Any.<AnyAwaitWithCancellationAsync>d__3<TSource> <AnyAwaitWithCancellationAsync>d__;
			<AnyAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<AnyAwaitWithCancellationAsync>d__.source = source;
			<AnyAwaitWithCancellationAsync>d__.predicate = predicate;
			<AnyAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AnyAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AnyAwaitWithCancellationAsync>d__.<>t__builder.Start<Any.<AnyAwaitWithCancellationAsync>d__3<TSource>>(ref <AnyAwaitWithCancellationAsync>d__);
			return <AnyAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
