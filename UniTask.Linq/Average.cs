using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Average
	{
		public static UniTask<double> AverageAsync(IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__0 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__0>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__1<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__1<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__2<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__2<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__3<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__3<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAsync(IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__4 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__4>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__5<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__5<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__6<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__6<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__7<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__7<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> AverageAsync(IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__8 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__8>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__9<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__9<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__10<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__10<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__11<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__11<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAsync(IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__12 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__12>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__13<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__13<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__14<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__14<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__15<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__15<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> AverageAsync(IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__16 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__16>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__17<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__17<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__18<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__18<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__19<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__19<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAsync(IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__20 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__20>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__21<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__21<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__22<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__22<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__23<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__23<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAsync(IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__24 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__24>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__25<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__25<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__26<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__26<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__27<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__27<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> AverageAsync(IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__28 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__28>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__29<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__29<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__30<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__30<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__31<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__31<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAsync(IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__32 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__32>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__33<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__33<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__34<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__34<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__35<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__35<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> AverageAsync(IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__36 <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__36>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> AverageAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAsync>d__37<TSource> <AverageAsync>d__;
			<AverageAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<AverageAsync>d__.source = source;
			<AverageAsync>d__.selector = selector;
			<AverageAsync>d__.cancellationToken = cancellationToken;
			<AverageAsync>d__.<>1__state = -1;
			<AverageAsync>d__.<>t__builder.Start<Average.<AverageAsync>d__37<TSource>>(ref <AverageAsync>d__);
			return <AverageAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> AverageAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitAsync>d__38<TSource> <AverageAwaitAsync>d__;
			<AverageAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<AverageAwaitAsync>d__.source = source;
			<AverageAwaitAsync>d__.selector = selector;
			<AverageAwaitAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitAsync>d__.<>1__state = -1;
			<AverageAwaitAsync>d__.<>t__builder.Start<Average.<AverageAwaitAsync>d__38<TSource>>(ref <AverageAwaitAsync>d__);
			return <AverageAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> AverageAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Average.<AverageAwaitWithCancellationAsync>d__39<TSource> <AverageAwaitWithCancellationAsync>d__;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<AverageAwaitWithCancellationAsync>d__.source = source;
			<AverageAwaitWithCancellationAsync>d__.selector = selector;
			<AverageAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<AverageAwaitWithCancellationAsync>d__.<>1__state = -1;
			<AverageAwaitWithCancellationAsync>d__.<>t__builder.Start<Average.<AverageAwaitWithCancellationAsync>d__39<TSource>>(ref <AverageAwaitWithCancellationAsync>d__);
			return <AverageAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
