using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeUntilCanceled<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeUntilCanceled(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			this.source = source;
			this.cancellationToken = cancellationToken;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TakeUntilCanceled<TSource>._TakeUntilCanceled(this.source, this.cancellationToken, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly CancellationToken cancellationToken;

		private sealed class _TakeUntilCanceled : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _TakeUntilCanceled(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
			{
				this.source = source;
				this.cancellationToken1 = cancellationToken1;
				this.cancellationToken2 = cancellationToken2;
				if (cancellationToken1.CanBeCanceled)
				{
					this.cancellationTokenRegistration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(TakeUntilCanceled<TSource>._TakeUntilCanceled.CancelDelegate1, this);
				}
				if (cancellationToken1 != cancellationToken2 && cancellationToken2.CanBeCanceled)
				{
					this.cancellationTokenRegistration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(TakeUntilCanceled<TSource>._TakeUntilCanceled.CancelDelegate2, this);
				}
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				if (this.cancellationToken1.IsCancellationRequested)
				{
					this.isCanceled = true;
				}
				if (this.cancellationToken2.IsCancellationRequested)
				{
					this.isCanceled = true;
				}
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken2);
				}
				if (this.isCanceled)
				{
					return CompletedTasks.False;
				}
				this.completionSource.Reset();
				this.SourceMoveNext();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void SourceMoveNext()
			{
				try
				{
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (this.awaiter.IsCompleted)
					{
						TakeUntilCanceled<TSource>._TakeUntilCanceled.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(TakeUntilCanceled<TSource>._TakeUntilCanceled.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				TakeUntilCanceled<TSource>._TakeUntilCanceled takeUntilCanceled = (TakeUntilCanceled<TSource>._TakeUntilCanceled)state;
				bool flag;
				if (takeUntilCanceled.TryGetResult<bool>(takeUntilCanceled.awaiter, out flag))
				{
					if (flag)
					{
						if (takeUntilCanceled.isCanceled)
						{
							takeUntilCanceled.completionSource.TrySetResult(false);
							return;
						}
						takeUntilCanceled.Current = takeUntilCanceled.enumerator.Current;
						takeUntilCanceled.completionSource.TrySetResult(true);
						return;
					}
					else
					{
						takeUntilCanceled.completionSource.TrySetResult(false);
					}
				}
			}

			private static void OnCanceled1(object state)
			{
				TakeUntilCanceled<TSource>._TakeUntilCanceled takeUntilCanceled = (TakeUntilCanceled<TSource>._TakeUntilCanceled)state;
				if (!takeUntilCanceled.isCanceled)
				{
					takeUntilCanceled.cancellationTokenRegistration2.Dispose();
					takeUntilCanceled.completionSource.TrySetResult(false);
				}
			}

			private static void OnCanceled2(object state)
			{
				TakeUntilCanceled<TSource>._TakeUntilCanceled takeUntilCanceled = (TakeUntilCanceled<TSource>._TakeUntilCanceled)state;
				if (!takeUntilCanceled.isCanceled)
				{
					takeUntilCanceled.cancellationTokenRegistration1.Dispose();
					takeUntilCanceled.completionSource.TrySetResult(false);
				}
			}

			public UniTask DisposeAsync()
			{
				this.cancellationTokenRegistration1.Dispose();
				this.cancellationTokenRegistration2.Dispose();
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> CancelDelegate1 = new Action<object>(TakeUntilCanceled<TSource>._TakeUntilCanceled.OnCanceled1);

			private static readonly Action<object> CancelDelegate2 = new Action<object>(TakeUntilCanceled<TSource>._TakeUntilCanceled.OnCanceled2);

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(TakeUntilCanceled<TSource>._TakeUntilCanceled.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken1;

			private CancellationToken cancellationToken2;

			private CancellationTokenRegistration cancellationTokenRegistration1;

			private CancellationTokenRegistration cancellationTokenRegistration2;

			private bool isCanceled;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;
		}
	}
}
