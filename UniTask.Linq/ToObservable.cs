using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class ToObservable<T> : IObservable<T>
	{
		public ToObservable(IUniTaskAsyncEnumerable<T> source)
		{
			this.source = source;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			ToObservable<T>.CancellationTokenDisposable cancellationTokenDisposable = new ToObservable<T>.CancellationTokenDisposable();
			ToObservable<T>.RunAsync(this.source, observer, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		private static UniTaskVoid RunAsync(IUniTaskAsyncEnumerable<T> src, IObserver<T> observer, CancellationToken cancellationToken)
		{
			ToObservable<T>.<RunAsync>d__3 <RunAsync>d__;
			<RunAsync>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<RunAsync>d__.src = src;
			<RunAsync>d__.observer = observer;
			<RunAsync>d__.cancellationToken = cancellationToken;
			<RunAsync>d__.<>1__state = -1;
			<RunAsync>d__.<>t__builder.Start<ToObservable<T>.<RunAsync>d__3>(ref <RunAsync>d__);
			return <RunAsync>d__.<>t__builder.Task;
		}

		private readonly IUniTaskAsyncEnumerable<T> source;

		internal sealed class CancellationTokenDisposable : IDisposable
		{
			public CancellationToken Token
			{
				get
				{
					return this.cts.Token;
				}
			}

			public void Dispose()
			{
				if (!this.cts.IsCancellationRequested)
				{
					this.cts.Cancel();
				}
			}

			private readonly CancellationTokenSource cts = new CancellationTokenSource();
		}
	}
}
