using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class ForEach
	{
		public static UniTask ForEachAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken)
		{
			ForEach.<ForEachAsync>d__0<TSource> <ForEachAsync>d__;
			<ForEachAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ForEachAsync>d__.source = source;
			<ForEachAsync>d__.action = action;
			<ForEachAsync>d__.cancellationToken = cancellationToken;
			<ForEachAsync>d__.<>1__state = -1;
			<ForEachAsync>d__.<>t__builder.Start<ForEach.<ForEachAsync>d__0<TSource>>(ref <ForEachAsync>d__);
			return <ForEachAsync>d__.<>t__builder.Task;
		}

		public static UniTask ForEachAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Action<TSource, int> action, CancellationToken cancellationToken)
		{
			ForEach.<ForEachAsync>d__1<TSource> <ForEachAsync>d__;
			<ForEachAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ForEachAsync>d__.source = source;
			<ForEachAsync>d__.action = action;
			<ForEachAsync>d__.cancellationToken = cancellationToken;
			<ForEachAsync>d__.<>1__state = -1;
			<ForEachAsync>d__.<>t__builder.Start<ForEach.<ForEachAsync>d__1<TSource>>(ref <ForEachAsync>d__);
			return <ForEachAsync>d__.<>t__builder.Task;
		}

		public static UniTask ForEachAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> action, CancellationToken cancellationToken)
		{
			ForEach.<ForEachAwaitAsync>d__2<TSource> <ForEachAwaitAsync>d__;
			<ForEachAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ForEachAwaitAsync>d__.source = source;
			<ForEachAwaitAsync>d__.action = action;
			<ForEachAwaitAsync>d__.cancellationToken = cancellationToken;
			<ForEachAwaitAsync>d__.<>1__state = -1;
			<ForEachAwaitAsync>d__.<>t__builder.Start<ForEach.<ForEachAwaitAsync>d__2<TSource>>(ref <ForEachAwaitAsync>d__);
			return <ForEachAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask ForEachAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask> action, CancellationToken cancellationToken)
		{
			ForEach.<ForEachAwaitAsync>d__3<TSource> <ForEachAwaitAsync>d__;
			<ForEachAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ForEachAwaitAsync>d__.source = source;
			<ForEachAwaitAsync>d__.action = action;
			<ForEachAwaitAsync>d__.cancellationToken = cancellationToken;
			<ForEachAwaitAsync>d__.<>1__state = -1;
			<ForEachAwaitAsync>d__.<>t__builder.Start<ForEach.<ForEachAwaitAsync>d__3<TSource>>(ref <ForEachAwaitAsync>d__);
			return <ForEachAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask ForEachAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> action, CancellationToken cancellationToken)
		{
			ForEach.<ForEachAwaitWithCancellationAsync>d__4<TSource> <ForEachAwaitWithCancellationAsync>d__;
			<ForEachAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ForEachAwaitWithCancellationAsync>d__.source = source;
			<ForEachAwaitWithCancellationAsync>d__.action = action;
			<ForEachAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<ForEachAwaitWithCancellationAsync>d__.<>1__state = -1;
			<ForEachAwaitWithCancellationAsync>d__.<>t__builder.Start<ForEach.<ForEachAwaitWithCancellationAsync>d__4<TSource>>(ref <ForEachAwaitWithCancellationAsync>d__);
			return <ForEachAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask ForEachAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask> action, CancellationToken cancellationToken)
		{
			ForEach.<ForEachAwaitWithCancellationAsync>d__5<TSource> <ForEachAwaitWithCancellationAsync>d__;
			<ForEachAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ForEachAwaitWithCancellationAsync>d__.source = source;
			<ForEachAwaitWithCancellationAsync>d__.action = action;
			<ForEachAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<ForEachAwaitWithCancellationAsync>d__.<>1__state = -1;
			<ForEachAwaitWithCancellationAsync>d__.<>t__builder.Start<ForEach.<ForEachAwaitWithCancellationAsync>d__5<TSource>>(ref <ForEachAwaitWithCancellationAsync>d__);
			return <ForEachAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
