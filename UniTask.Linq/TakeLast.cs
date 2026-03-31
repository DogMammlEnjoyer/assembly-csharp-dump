using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeLast<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeLast(IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			this.source = source;
			this.count = count;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TakeLast<TSource>._TakeLast(this.source, this.count, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly int count;

		private sealed class _TakeLast : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _TakeLast(IUniTaskAsyncEnumerable<TSource> source, int count, CancellationToken cancellationToken)
			{
				this.source = source;
				this.count = count;
				this.cancellationToken = cancellationToken;
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
					this.queue = new Queue<TSource>();
				}
				this.completionSource.Reset();
				this.SourceMoveNext();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void SourceMoveNext()
			{
				if (!this.iterateCompleted)
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
							TakeLast<TSource>._TakeLast.MoveNextCore(this);
							if (!this.continueNext)
							{
								goto IL_99;
							}
							this.continueNext = false;
						}
						this.awaiter.SourceOnCompleted(TakeLast<TSource>._TakeLast.MoveNextCoreDelegate, this);
						IL_99:;
					}
					catch (Exception error)
					{
						this.completionSource.TrySetException(error);
					}
					return;
				}
				if (this.queue.Count > 0)
				{
					this.Current = this.queue.Dequeue();
					this.completionSource.TrySetResult(true);
					return;
				}
				this.completionSource.TrySetResult(false);
			}

			private static void MoveNextCore(object state)
			{
				TakeLast<TSource>._TakeLast takeLast = (TakeLast<TSource>._TakeLast)state;
				bool flag;
				if (takeLast.TryGetResult<bool>(takeLast.awaiter, out flag))
				{
					if (!flag)
					{
						takeLast.continueNext = false;
						takeLast.iterateCompleted = true;
						takeLast.SourceMoveNext();
						return;
					}
					if (takeLast.queue.Count < takeLast.count)
					{
						takeLast.queue.Enqueue(takeLast.enumerator.Current);
						if (!takeLast.continueNext)
						{
							takeLast.SourceMoveNext();
							return;
						}
					}
					else
					{
						takeLast.queue.Dequeue();
						takeLast.queue.Enqueue(takeLast.enumerator.Current);
						if (!takeLast.continueNext)
						{
							takeLast.SourceMoveNext();
							return;
						}
					}
				}
				else
				{
					takeLast.continueNext = false;
				}
			}

			public UniTask DisposeAsync()
			{
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(TakeLast<TSource>._TakeLast.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly int count;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private Queue<TSource> queue;

			private bool iterateCompleted;

			private bool continueNext;
		}
	}
}
