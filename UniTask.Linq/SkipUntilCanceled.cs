using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipUntilCanceled<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipUntilCanceled(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			this.source = source;
			this.cancellationToken = cancellationToken;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipUntilCanceled<TSource>._SkipUntilCanceled(this.source, this.cancellationToken, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly CancellationToken cancellationToken;

		private sealed class _SkipUntilCanceled : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _SkipUntilCanceled(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
			{
				this.source = source;
				this.cancellationToken1 = cancellationToken1;
				this.cancellationToken2 = cancellationToken2;
				if (cancellationToken1.CanBeCanceled)
				{
					this.cancellationTokenRegistration1 = cancellationToken1.RegisterWithoutCaptureExecutionContext(SkipUntilCanceled<TSource>._SkipUntilCanceled.CancelDelegate1, this);
				}
				if (cancellationToken1 != cancellationToken2 && cancellationToken2.CanBeCanceled)
				{
					this.cancellationTokenRegistration2 = cancellationToken2.RegisterWithoutCaptureExecutionContext(SkipUntilCanceled<TSource>._SkipUntilCanceled.CancelDelegate2, this);
				}
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				if (this.enumerator == null)
				{
					if (this.cancellationToken1.IsCancellationRequested)
					{
						this.isCanceled = 1;
					}
					if (this.cancellationToken2.IsCancellationRequested)
					{
						this.isCanceled = 1;
					}
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken2);
				}
				this.completionSource.Reset();
				if (this.isCanceled != 0)
				{
					this.SourceMoveNext();
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void SourceMoveNext()
			{
				try
				{
					for (;;)
					{
						this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
						if (!this.awaiter.IsCompleted)
						{
							break;
						}
						this.continueNext = true;
						SkipUntilCanceled<TSource>._SkipUntilCanceled.MoveNextCore(this);
						if (!this.continueNext)
						{
							goto IL_55;
						}
						this.continueNext = false;
					}
					this.awaiter.SourceOnCompleted(SkipUntilCanceled<TSource>._SkipUntilCanceled.MoveNextCoreDelegate, this);
					IL_55:;
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				SkipUntilCanceled<TSource>._SkipUntilCanceled skipUntilCanceled = (SkipUntilCanceled<TSource>._SkipUntilCanceled)state;
				bool flag;
				if (skipUntilCanceled.TryGetResult<bool>(skipUntilCanceled.awaiter, out flag))
				{
					if (flag)
					{
						skipUntilCanceled.Current = skipUntilCanceled.enumerator.Current;
						skipUntilCanceled.completionSource.TrySetResult(true);
						if (skipUntilCanceled.continueNext)
						{
							skipUntilCanceled.SourceMoveNext();
							return;
						}
					}
					else
					{
						skipUntilCanceled.completionSource.TrySetResult(false);
					}
				}
			}

			private static void OnCanceled1(object state)
			{
				SkipUntilCanceled<TSource>._SkipUntilCanceled skipUntilCanceled = (SkipUntilCanceled<TSource>._SkipUntilCanceled)state;
				if (skipUntilCanceled.isCanceled == 0 && Interlocked.Increment(ref skipUntilCanceled.isCanceled) == 1)
				{
					skipUntilCanceled.cancellationTokenRegistration2.Dispose();
					skipUntilCanceled.SourceMoveNext();
				}
			}

			private static void OnCanceled2(object state)
			{
				SkipUntilCanceled<TSource>._SkipUntilCanceled skipUntilCanceled = (SkipUntilCanceled<TSource>._SkipUntilCanceled)state;
				if (skipUntilCanceled.isCanceled == 0 && Interlocked.Increment(ref skipUntilCanceled.isCanceled) == 1)
				{
					skipUntilCanceled.cancellationTokenRegistration2.Dispose();
					skipUntilCanceled.SourceMoveNext();
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

			private static readonly Action<object> CancelDelegate1 = new Action<object>(SkipUntilCanceled<TSource>._SkipUntilCanceled.OnCanceled1);

			private static readonly Action<object> CancelDelegate2 = new Action<object>(SkipUntilCanceled<TSource>._SkipUntilCanceled.OnCanceled2);

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(SkipUntilCanceled<TSource>._SkipUntilCanceled.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken1;

			private CancellationToken cancellationToken2;

			private CancellationTokenRegistration cancellationTokenRegistration1;

			private CancellationTokenRegistration cancellationTokenRegistration2;

			private int isCanceled;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private bool continueNext;
		}
	}
}
