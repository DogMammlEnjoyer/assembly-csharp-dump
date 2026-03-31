using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class DistinctUntilChanged<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public DistinctUntilChanged(IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			this.source = source;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new DistinctUntilChanged<TSource>._DistinctUntilChanged(this.source, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly IEqualityComparer<TSource> comparer;

		private sealed class _DistinctUntilChanged : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _DistinctUntilChanged(IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken)
			{
				this.source = source;
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
							goto IL_132;
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
							goto IL_F0;
						case 1:
							goto IL_F0;
						default:
							goto IL_132;
						}
						if (this.awaiter.GetResult())
						{
							this.Current = this.enumerator.Current;
							goto IL_162;
						}
						break;
						IL_F0:
						if (this.awaiter.GetResult())
						{
							TSource tsource = this.enumerator.Current;
							if (!this.comparer.Equals(this.Current, tsource))
							{
								this.Current = tsource;
								goto IL_162;
							}
							this.state = 0;
							continue;
						}
						IL_132:;
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
				IL_162:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly IEqualityComparer<TSource> comparer;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private Action moveNextAction;
		}
	}
}
