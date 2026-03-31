using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Sum
	{
		public static UniTask<int> SumAsync(IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__0 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__0>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__1<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__1<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__2<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__2<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__3<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__3<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> SumAsync(IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__4 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__4>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__5<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__5<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__6<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__6<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__7<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__7<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> SumAsync(IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__8 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__8>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__9<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__9<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__10<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__10<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__11<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__11<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> SumAsync(IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__12 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__12>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__13<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__13<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__14<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__14<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__15<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__15<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> SumAsync(IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__16 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__16>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__17<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__17<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__18<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__18<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__19<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__19<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> SumAsync(IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__20 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__20>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__21<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__21<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__22<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__22<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__23<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__23<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> SumAsync(IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__24 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__24>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__25<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__25<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__26<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__26<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__27<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__27<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> SumAsync(IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__28 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__28>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__29<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__29<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__30<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__30<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__31<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__31<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> SumAsync(IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__32 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__32>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__33<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__33<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__34<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__34<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__35<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__35<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> SumAsync(IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__36 <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__36>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> SumAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAsync>d__37<TSource> <SumAsync>d__;
			<SumAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<SumAsync>d__.source = source;
			<SumAsync>d__.selector = selector;
			<SumAsync>d__.cancellationToken = cancellationToken;
			<SumAsync>d__.<>1__state = -1;
			<SumAsync>d__.<>t__builder.Start<Sum.<SumAsync>d__37<TSource>>(ref <SumAsync>d__);
			return <SumAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> SumAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitAsync>d__38<TSource> <SumAwaitAsync>d__;
			<SumAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<SumAwaitAsync>d__.source = source;
			<SumAwaitAsync>d__.selector = selector;
			<SumAwaitAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitAsync>d__.<>1__state = -1;
			<SumAwaitAsync>d__.<>t__builder.Start<Sum.<SumAwaitAsync>d__38<TSource>>(ref <SumAwaitAsync>d__);
			return <SumAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> SumAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Sum.<SumAwaitWithCancellationAsync>d__39<TSource> <SumAwaitWithCancellationAsync>d__;
			<SumAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<SumAwaitWithCancellationAsync>d__.source = source;
			<SumAwaitWithCancellationAsync>d__.selector = selector;
			<SumAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<SumAwaitWithCancellationAsync>d__.<>1__state = -1;
			<SumAwaitWithCancellationAsync>d__.<>t__builder.Start<Sum.<SumAwaitWithCancellationAsync>d__39<TSource>>(ref <SumAwaitWithCancellationAsync>d__);
			return <SumAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
