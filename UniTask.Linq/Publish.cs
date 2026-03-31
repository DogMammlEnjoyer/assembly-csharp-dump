using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Publish<TSource> : IConnectableUniTaskAsyncEnumerable<TSource>, IUniTaskAsyncEnumerable<TSource>
	{
		public Publish(IUniTaskAsyncEnumerable<TSource> source)
		{
			this.source = source;
			this.cancellationTokenSource = new CancellationTokenSource();
		}

		public IDisposable Connect()
		{
			if (this.connectedDisposable != null)
			{
				return this.connectedDisposable;
			}
			if (this.enumerator == null)
			{
				this.enumerator = this.source.GetAsyncEnumerator(this.cancellationTokenSource.Token);
			}
			this.ConsumeEnumerator().Forget();
			this.connectedDisposable = new Publish<TSource>.ConnectDisposable(this.cancellationTokenSource);
			return this.connectedDisposable;
		}

		private UniTaskVoid ConsumeEnumerator()
		{
			Publish<TSource>.<ConsumeEnumerator>d__8 <ConsumeEnumerator>d__;
			<ConsumeEnumerator>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<ConsumeEnumerator>d__.<>4__this = this;
			<ConsumeEnumerator>d__.<>1__state = -1;
			<ConsumeEnumerator>d__.<>t__builder.Start<Publish<TSource>.<ConsumeEnumerator>d__8>(ref <ConsumeEnumerator>d__);
			return <ConsumeEnumerator>d__.<>t__builder.Task;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Publish<TSource>._Publish(this, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly CancellationTokenSource cancellationTokenSource;

		private TriggerEvent<TSource> trigger;

		private IUniTaskAsyncEnumerator<TSource> enumerator;

		private IDisposable connectedDisposable;

		private bool isCompleted;

		private sealed class ConnectDisposable : IDisposable
		{
			public ConnectDisposable(CancellationTokenSource cancellationTokenSource)
			{
				this.cancellationTokenSource = cancellationTokenSource;
			}

			public void Dispose()
			{
				this.cancellationTokenSource.Cancel();
			}

			private readonly CancellationTokenSource cancellationTokenSource;
		}

		private sealed class _Publish : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable, ITriggerHandler<TSource>
		{
			public _Publish(Publish<TSource> parent, CancellationToken cancellationToken)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				this.parent = parent;
				this.cancellationToken = cancellationToken;
				if (cancellationToken.CanBeCanceled)
				{
					this.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(Publish<TSource>._Publish.CancelDelegate, this);
				}
				parent.trigger.Add(this);
			}

			public TSource Current { get; private set; }

			ITriggerHandler<TSource> ITriggerHandler<!0>.Prev { get; set; }

			ITriggerHandler<TSource> ITriggerHandler<!0>.Next { get; set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.parent.isCompleted)
				{
					return CompletedTasks.False;
				}
				this.completionSource.Reset();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void OnCanceled(object state)
			{
				Publish<TSource>._Publish publish = (Publish<TSource>._Publish)state;
				publish.completionSource.TrySetCanceled(publish.cancellationToken);
				publish.DisposeAsync().Forget();
			}

			public UniTask DisposeAsync()
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					this.cancellationTokenRegistration.Dispose();
					this.parent.trigger.Remove(this);
				}
				return default(UniTask);
			}

			public void OnNext(TSource value)
			{
				this.Current = value;
				this.completionSource.TrySetResult(true);
			}

			public void OnCanceled(CancellationToken cancellationToken)
			{
				this.completionSource.TrySetCanceled(cancellationToken);
			}

			public void OnCompleted()
			{
				this.completionSource.TrySetResult(false);
			}

			public void OnError(Exception ex)
			{
				this.completionSource.TrySetException(ex);
			}

			private static readonly Action<object> CancelDelegate = new Action<object>(Publish<TSource>._Publish.OnCanceled);

			private readonly Publish<TSource> parent;

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration cancellationTokenRegistration;

			private bool isDisposed;
		}
	}
}
