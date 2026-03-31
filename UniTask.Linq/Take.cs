using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Take<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Take(IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			this.source = source;
			this.count = count;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Take<TSource>._Take(this.source, this.count, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly int count;

		private sealed class _Take : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _Take(IUniTaskAsyncEnumerable<TSource> source, int count, CancellationToken cancellationToken)
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
				}
				if (this.index >= this.count)
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
						Take<TSource>._Take.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(Take<TSource>._Take.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				Take<TSource>._Take take = (Take<TSource>._Take)state;
				bool flag;
				if (take.TryGetResult<bool>(take.awaiter, out flag))
				{
					if (flag)
					{
						take.index++;
						take.Current = take.enumerator.Current;
						take.completionSource.TrySetResult(true);
						return;
					}
					take.completionSource.TrySetResult(false);
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(Take<TSource>._Take.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly int count;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private int index;
		}
	}
}
