using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeUntil<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeUntil(IUniTaskAsyncEnumerable<TSource> source, UniTask other, Func<CancellationToken, UniTask> other2)
		{
			this.source = source;
			this.other = other;
			this.other2 = other2;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this.other2 != null)
			{
				return new TakeUntil<TSource>._TakeUntil(this.source, this.other2(cancellationToken), cancellationToken);
			}
			return new TakeUntil<TSource>._TakeUntil(this.source, this.other, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly UniTask other;

		private readonly Func<CancellationToken, UniTask> other2;

		private sealed class _TakeUntil : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _TakeUntil(IUniTaskAsyncEnumerable<TSource> source, UniTask other, CancellationToken cancellationToken1)
			{
				this.source = source;
				this.cancellationToken1 = cancellationToken1;
				if (cancellationToken1.CanBeCanceled)
				{
					this.cancellationTokenRegistration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(TakeUntil<TSource>._TakeUntil.CancelDelegate1, this);
				}
				this.RunOther(other).Forget();
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				if (this.completed)
				{
					return CompletedTasks.False;
				}
				if (this.exception != null)
				{
					return UniTask.FromException<bool>(this.exception);
				}
				if (this.cancellationToken1.IsCancellationRequested)
				{
					return UniTask.FromCanceled<bool>(this.cancellationToken1);
				}
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken1);
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
						TakeUntil<TSource>._TakeUntil.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(TakeUntil<TSource>._TakeUntil.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				TakeUntil<TSource>._TakeUntil takeUntil = (TakeUntil<TSource>._TakeUntil)state;
				bool flag;
				if (takeUntil.TryGetResult<bool>(takeUntil.awaiter, out flag))
				{
					if (flag)
					{
						if (takeUntil.exception != null)
						{
							takeUntil.completionSource.TrySetException(takeUntil.exception);
							return;
						}
						if (takeUntil.cancellationToken1.IsCancellationRequested)
						{
							takeUntil.completionSource.TrySetCanceled(takeUntil.cancellationToken1);
							return;
						}
						takeUntil.Current = takeUntil.enumerator.Current;
						takeUntil.completionSource.TrySetResult(true);
						return;
					}
					else
					{
						takeUntil.completionSource.TrySetResult(false);
					}
				}
			}

			private UniTaskVoid RunOther(UniTask other)
			{
				TakeUntil<TSource>._TakeUntil.<RunOther>d__17 <RunOther>d__;
				<RunOther>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunOther>d__.<>4__this = this;
				<RunOther>d__.other = other;
				<RunOther>d__.<>1__state = -1;
				<RunOther>d__.<>t__builder.Start<TakeUntil<TSource>._TakeUntil.<RunOther>d__17>(ref <RunOther>d__);
				return <RunOther>d__.<>t__builder.Task;
			}

			private static void OnCanceled1(object state)
			{
				TakeUntil<TSource>._TakeUntil takeUntil = (TakeUntil<TSource>._TakeUntil)state;
				takeUntil.completionSource.TrySetCanceled(takeUntil.cancellationToken1);
			}

			public UniTask DisposeAsync()
			{
				this.cancellationTokenRegistration1.Dispose();
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> CancelDelegate1 = new Action<object>(TakeUntil<TSource>._TakeUntil.OnCanceled1);

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(TakeUntil<TSource>._TakeUntil.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken1;

			private CancellationTokenRegistration cancellationTokenRegistration1;

			private bool completed;

			private Exception exception;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;
		}
	}
}
