using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class WhereInt<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public WhereInt(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new WhereInt<TSource>._Where(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, bool> predicate;

		private sealed class _Where : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _Where(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, CancellationToken cancellationToken)
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
							goto IL_7B;
						default:
							goto IL_CB;
						}
						this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
						if (!this.awaiter.IsCompleted)
						{
							this.state = 1;
							this.awaiter.UnsafeOnCompleted(this.moveNextAction);
							return;
						}
						IL_7B:
						if (this.awaiter.GetResult())
						{
							this.Current = this.enumerator.Current;
							Func<TSource, int, bool> func = this.predicate;
							TSource arg = this.Current;
							int num = this.index;
							this.index = checked(num + 1);
							if (func(arg, num))
							{
								goto IL_FB;
							}
							this.state = 0;
							continue;
						}
						IL_CB:;
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
				IL_FB:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, int, bool> predicate;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private Action moveNextAction;

			private int index;
		}
	}
}
