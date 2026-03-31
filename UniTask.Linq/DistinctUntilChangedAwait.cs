using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class DistinctUntilChangedAwait<TSource, TKey> : IUniTaskAsyncEnumerable<TSource>
	{
		public DistinctUntilChangedAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new DistinctUntilChangedAwait<TSource, TKey>._DistinctUntilChangedAwait(this.source, this.keySelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<TKey>> keySelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _DistinctUntilChangedAwait : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _DistinctUntilChangedAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			{
				this.source = source;
				this.keySelector = keySelector;
				this.comparer = comparer;
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
						case -3:
							break;
						case -2:
							goto IL_19E;
						case -1:
							this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
							this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
							if (!this.awaiter.IsCompleted)
							{
								this.state = -3;
								this.awaiter.UnsafeOnCompleted(this.moveNextAction);
								return;
							}
							break;
						case 0:
							this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
							if (!this.awaiter.IsCompleted)
							{
								this.state = 1;
								this.awaiter.UnsafeOnCompleted(this.moveNextAction);
								return;
							}
							goto IL_F4;
						case 1:
							goto IL_F4;
						case 2:
							goto IL_15D;
						default:
							goto IL_19E;
						}
						if (this.awaiter.GetResult())
						{
							this.Current = this.enumerator.Current;
							goto IL_1D0;
						}
						break;
						IL_F4:
						if (!this.awaiter.GetResult())
						{
							break;
						}
						this.enumeratorCurrent = this.enumerator.Current;
						this.awaiter2 = this.keySelector(this.enumeratorCurrent).GetAwaiter();
						if (!this.awaiter2.IsCompleted)
						{
							this.state = 2;
							this.awaiter2.UnsafeOnCompleted(this.moveNextAction);
							return;
						}
						IL_15D:
						TKey result = this.awaiter2.GetResult();
						if (!this.comparer.Equals(this.prev, result))
						{
							this.prev = result;
							this.Current = this.enumeratorCurrent;
							goto IL_1D0;
						}
						this.state = 0;
						continue;
						IL_19E:;
					}
					catch (Exception error)
					{
						this.state = -2;
						this.completionSource.TrySetException(error);
						return;
					}
					break;
				}
				this.state = -2;
				this.completionSource.TrySetResult(false);
				return;
				IL_1D0:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, UniTask<TKey>> keySelector;

			private readonly IEqualityComparer<TKey> comparer;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private UniTask<TKey>.Awaiter awaiter2;

			private Action moveNextAction;

			private TSource enumeratorCurrent;

			private TKey prev;
		}
	}
}
