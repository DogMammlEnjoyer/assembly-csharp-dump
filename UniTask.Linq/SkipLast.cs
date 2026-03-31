using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipLast<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipLast(IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			this.source = source;
			this.count = count;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipLast<TSource>._SkipLast(this.source, this.count, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly int count;

		private sealed class _SkipLast : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _SkipLast(IUniTaskAsyncEnumerable<TSource> source, int count, CancellationToken cancellationToken)
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
						SkipLast<TSource>._SkipLast.MoveNextCore(this);
						if (!this.continueNext)
						{
							goto IL_55;
						}
						this.continueNext = false;
					}
					this.awaiter.SourceOnCompleted(SkipLast<TSource>._SkipLast.MoveNextCoreDelegate, this);
					IL_55:;
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				SkipLast<TSource>._SkipLast skipLast = (SkipLast<TSource>._SkipLast)state;
				bool flag;
				if (skipLast.TryGetResult<bool>(skipLast.awaiter, out flag))
				{
					if (!flag)
					{
						skipLast.continueNext = false;
						skipLast.completionSource.TrySetResult(false);
						return;
					}
					if (skipLast.queue.Count == skipLast.count)
					{
						skipLast.continueNext = false;
						TSource value = skipLast.queue.Dequeue();
						skipLast.Current = value;
						skipLast.queue.Enqueue(skipLast.enumerator.Current);
						skipLast.completionSource.TrySetResult(true);
						return;
					}
					skipLast.queue.Enqueue(skipLast.enumerator.Current);
					if (!skipLast.continueNext)
					{
						skipLast.SourceMoveNext();
						return;
					}
				}
				else
				{
					skipLast.continueNext = false;
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(SkipLast<TSource>._SkipLast.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly int count;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private Queue<TSource> queue;

			private bool continueNext;
		}
	}
}
