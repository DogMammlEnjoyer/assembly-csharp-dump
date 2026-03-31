using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Pairwise<TSource> : IUniTaskAsyncEnumerable<ValueTuple<TSource, TSource>>
	{
		public Pairwise(IUniTaskAsyncEnumerable<TSource> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<ValueTuple<TSource, TSource>> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Pairwise<TSource>._Pairwise(this.source, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private sealed class _Pairwise : MoveNextSource, IUniTaskAsyncEnumerator<ValueTuple<TSource, TSource>>, IUniTaskAsyncDisposable
		{
			public _Pairwise(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
			{
				this.source = source;
				this.cancellationToken = cancellationToken;
			}

			public ValueTuple<TSource, TSource> Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.enumerator == null)
				{
					this.isFirst = true;
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
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
						Pairwise<TSource>._Pairwise.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(Pairwise<TSource>._Pairwise.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				Pairwise<TSource>._Pairwise pairwise = (Pairwise<TSource>._Pairwise)state;
				bool flag;
				if (pairwise.TryGetResult<bool>(pairwise.awaiter, out flag))
				{
					if (flag)
					{
						if (pairwise.isFirst)
						{
							pairwise.isFirst = false;
							pairwise.prev = pairwise.enumerator.Current;
							pairwise.SourceMoveNext();
							return;
						}
						TSource item = pairwise.prev;
						pairwise.prev = pairwise.enumerator.Current;
						pairwise.Current = new ValueTuple<TSource, TSource>(item, pairwise.prev);
						pairwise.completionSource.TrySetResult(true);
						return;
					}
					else
					{
						pairwise.completionSource.TrySetResult(false);
					}
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(Pairwise<TSource>._Pairwise.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private TSource prev;

			private bool isFirst;
		}
	}
}
