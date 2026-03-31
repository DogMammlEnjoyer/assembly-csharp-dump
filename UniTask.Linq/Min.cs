using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Min
	{
		public static UniTask<TSource> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__0<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__0<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TResult> MinAsync<TSource, TResult>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__1<TSource, TResult> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__1<TSource, TResult>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TResult> MinAwaitAsync<TSource, TResult>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TResult>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__2<TSource, TResult> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__2<TSource, TResult>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TResult> MinAwaitWithCancellationAsync<TSource, TResult>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TResult>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__3<TSource, TResult> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__3<TSource, TResult>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MinAsync(IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__4 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__4>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__5<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__5<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__6<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__6<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__7<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__7<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MinAsync(IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__8 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__8>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__9<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__9<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__10<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__10<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__11<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__11<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MinAsync(IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__12 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__12>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__13<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__13<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__14<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__14<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__15<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__15<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MinAsync(IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__16 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__16>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__17<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__17<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__18<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__18<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__19<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__19<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MinAsync(IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__20 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__20>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__21<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__21<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__22<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__22<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__23<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__23<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MinAsync(IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__24 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__24>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__25<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__25<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__26<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__26<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__27<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__27<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MinAsync(IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__28 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__28>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__29<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__29<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__30<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__30<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__31<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__31<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MinAsync(IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__32 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__32>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__33<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__33<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__34<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__34<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__35<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__35<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MinAsync(IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__36 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__36>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__37<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__37<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__38<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__38<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__39<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__39<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MinAsync(IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__40 <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__40>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MinAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken)
		{
			Min.<MinAsync>d__41<TSource> <MinAsync>d__;
			<MinAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MinAsync>d__.source = source;
			<MinAsync>d__.selector = selector;
			<MinAsync>d__.cancellationToken = cancellationToken;
			<MinAsync>d__.<>1__state = -1;
			<MinAsync>d__.<>t__builder.Start<Min.<MinAsync>d__41<TSource>>(ref <MinAsync>d__);
			return <MinAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MinAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitAsync>d__42<TSource> <MinAwaitAsync>d__;
			<MinAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MinAwaitAsync>d__.source = source;
			<MinAwaitAsync>d__.selector = selector;
			<MinAwaitAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitAsync>d__.<>1__state = -1;
			<MinAwaitAsync>d__.<>t__builder.Start<Min.<MinAwaitAsync>d__42<TSource>>(ref <MinAwaitAsync>d__);
			return <MinAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MinAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Min.<MinAwaitWithCancellationAsync>d__43<TSource> <MinAwaitWithCancellationAsync>d__;
			<MinAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MinAwaitWithCancellationAsync>d__.source = source;
			<MinAwaitWithCancellationAsync>d__.selector = selector;
			<MinAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MinAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MinAwaitWithCancellationAsync>d__.<>t__builder.Start<Min.<MinAwaitWithCancellationAsync>d__43<TSource>>(ref <MinAwaitWithCancellationAsync>d__);
			return <MinAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
