using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class Max
	{
		public static UniTask<TSource> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__0<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TSource>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__0<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TResult> MaxAsync<TSource, TResult>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__1<TSource, TResult> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__1<TSource, TResult>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TResult> MaxAwaitAsync<TSource, TResult>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TResult>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__2<TSource, TResult> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__2<TSource, TResult>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<TResult> MaxAwaitWithCancellationAsync<TSource, TResult>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TResult>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__3<TSource, TResult> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<TResult>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__3<TSource, TResult>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MaxAsync(IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__4 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__4>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__5<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__5<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__6<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__6<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__7<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__7<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MaxAsync(IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__8 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__8>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__9<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__9<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__10<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__10<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__11<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__11<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MaxAsync(IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__12 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__12>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__13<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__13<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__14<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__14<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__15<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__15<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MaxAsync(IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__16 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__16>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__17<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__17<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__18<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__18<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__19<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__19<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MaxAsync(IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__20 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__20>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__21<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__21<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__22<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__22<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__23<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__23<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MaxAsync(IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__24 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__24>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__25<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__25<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__26<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__26<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<int?> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__27<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int?>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__27<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MaxAsync(IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__28 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__28>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__29<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__29<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__30<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__30<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<long?> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__31<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<long?>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__31<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MaxAsync(IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__32 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__32>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__33<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__33<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__34<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__34<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<float?> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__35<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<float?>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__35<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MaxAsync(IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__36 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__36>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__37<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__37<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__38<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__38<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<double?> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__39<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<double?>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__39<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MaxAsync(IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__40 <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__40>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MaxAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAsync>d__41<TSource> <MaxAsync>d__;
			<MaxAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MaxAsync>d__.source = source;
			<MaxAsync>d__.selector = selector;
			<MaxAsync>d__.cancellationToken = cancellationToken;
			<MaxAsync>d__.<>1__state = -1;
			<MaxAsync>d__.<>t__builder.Start<Max.<MaxAsync>d__41<TSource>>(ref <MaxAsync>d__);
			return <MaxAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MaxAwaitAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitAsync>d__42<TSource> <MaxAwaitAsync>d__;
			<MaxAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MaxAwaitAsync>d__.source = source;
			<MaxAwaitAsync>d__.selector = selector;
			<MaxAwaitAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitAsync>d__.<>1__state = -1;
			<MaxAwaitAsync>d__.<>t__builder.Start<Max.<MaxAwaitAsync>d__42<TSource>>(ref <MaxAwaitAsync>d__);
			return <MaxAwaitAsync>d__.<>t__builder.Task;
		}

		public static UniTask<decimal?> MaxAwaitWithCancellationAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken)
		{
			Max.<MaxAwaitWithCancellationAsync>d__43<TSource> <MaxAwaitWithCancellationAsync>d__;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<decimal?>.Create();
			<MaxAwaitWithCancellationAsync>d__.source = source;
			<MaxAwaitWithCancellationAsync>d__.selector = selector;
			<MaxAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<MaxAwaitWithCancellationAsync>d__.<>1__state = -1;
			<MaxAwaitWithCancellationAsync>d__.<>t__builder.Start<Max.<MaxAwaitWithCancellationAsync>d__43<TSource>>(ref <MaxAwaitWithCancellationAsync>d__);
			return <MaxAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}
	}
}
