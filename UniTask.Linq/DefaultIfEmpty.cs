using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class DefaultIfEmpty<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public DefaultIfEmpty(IUniTaskAsyncEnumerable<TSource> source, TSource defaultValue)
		{
			this.source = source;
			this.defaultValue = defaultValue;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new DefaultIfEmpty<TSource>._DefaultIfEmpty(this.source, this.defaultValue, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly TSource defaultValue;

		private sealed class _DefaultIfEmpty : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _DefaultIfEmpty(IUniTaskAsyncEnumerable<TSource> source, TSource defaultValue, CancellationToken cancellationToken)
			{
				this.source = source;
				this.defaultValue = defaultValue;
				this.cancellationToken = cancellationToken;
				this.iteratingState = DefaultIfEmpty<TSource>._DefaultIfEmpty.IteratingState.Empty;
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (this.iteratingState == DefaultIfEmpty<TSource>._DefaultIfEmpty.IteratingState.Completed)
				{
					return CompletedTasks.False;
				}
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
				}
				this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
				if (this.awaiter.IsCompleted)
				{
					DefaultIfEmpty<TSource>._DefaultIfEmpty.MoveNextCore(this);
				}
				else
				{
					this.awaiter.SourceOnCompleted(DefaultIfEmpty<TSource>._DefaultIfEmpty.MoveNextCoreDelegate, this);
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void MoveNextCore(object state)
			{
				DefaultIfEmpty<TSource>._DefaultIfEmpty defaultIfEmpty = (DefaultIfEmpty<TSource>._DefaultIfEmpty)state;
				bool flag;
				if (defaultIfEmpty.TryGetResult<bool>(defaultIfEmpty.awaiter, out flag))
				{
					if (flag)
					{
						defaultIfEmpty.iteratingState = DefaultIfEmpty<TSource>._DefaultIfEmpty.IteratingState.Iterating;
						defaultIfEmpty.Current = defaultIfEmpty.enumerator.Current;
						defaultIfEmpty.completionSource.TrySetResult(true);
						return;
					}
					if (defaultIfEmpty.iteratingState == DefaultIfEmpty<TSource>._DefaultIfEmpty.IteratingState.Empty)
					{
						defaultIfEmpty.iteratingState = DefaultIfEmpty<TSource>._DefaultIfEmpty.IteratingState.Completed;
						defaultIfEmpty.Current = defaultIfEmpty.defaultValue;
						defaultIfEmpty.completionSource.TrySetResult(true);
						return;
					}
					defaultIfEmpty.completionSource.TrySetResult(false);
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(DefaultIfEmpty<TSource>._DefaultIfEmpty.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly TSource defaultValue;

			private CancellationToken cancellationToken;

			private DefaultIfEmpty<TSource>._DefaultIfEmpty.IteratingState iteratingState;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private enum IteratingState : byte
			{
				Empty,
				Iterating,
				Completed
			}
		}
	}
}
