using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class WhereAwait<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public WhereAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new WhereAwait<TSource>._WhereAwait(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<bool>> predicate;

		private sealed class _WhereAwait : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _WhereAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken)
			{
				this.source = source;
				this.predicate = predicate;
				this.cancellationToken = cancellationToken;
				this.moveNextAction = new Action(this.MoveNext);
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				if (this.state == -2)
				{
					return default(UniTask<bool>);
				}
				this.completionSource.Reset();
				this.MoveNext();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void MoveNext()
			{
				for (;;)
				{
					try
					{
						switch (this.state)
						{
						case -1:
							this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
							break;
						case 0:
							break;
						case 1:
							goto IL_7F;
						case 2:
							goto IL_E5;
						default:
							goto IL_11A;
						}
						this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
						if (!this.awaiter.IsCompleted)
						{
							this.state = 1;
							this.awaiter.UnsafeOnCompleted(this.moveNextAction);
							return;
						}
						IL_7F:
						if (!this.awaiter.GetResult())
						{
							break;
						}
						this.Current = this.enumerator.Current;
						this.awaiter2 = this.predicate(this.Current).GetAwaiter();
						if (!this.awaiter2.IsCompleted)
						{
							this.state = 2;
							this.awaiter2.UnsafeOnCompleted(this.moveNextAction);
							return;
						}
						IL_E5:
						if (this.awaiter2.GetResult())
						{
							goto IL_130;
						}
						this.state = 0;
						continue;
					}
					catch (Exception error)
					{
						this.state = -2;
						this.completionSource.TrySetException(error);
						return;
					}
					break;
				}
				IL_11A:
				this.state = -2;
				this.completionSource.TrySetResult(false);
				return;
				IL_130:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, UniTask<bool>> predicate;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private UniTask<bool>.Awaiter awaiter2;

			private Action moveNextAction;
		}
	}
}
