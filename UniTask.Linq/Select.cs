using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Select<TSource, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public Select(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			this.source = source;
			this.selector = selector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Select<TSource, TResult>._Select(this.source, this.selector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, TResult> selector;

		private sealed class _Select : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _Select(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken)
			{
				this.source = source;
				this.selector = selector;
				this.cancellationToken = cancellationToken;
				this.moveNextAction = new Action(this.MoveNext);
			}

			public TResult Current { get; private set; }

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
						goto IL_77;
					default:
						goto IL_A2;
					}
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (!this.awaiter.IsCompleted)
					{
						this.state = 1;
						this.awaiter.UnsafeOnCompleted(this.moveNextAction);
						return;
					}
					IL_77:
					if (this.awaiter.GetResult())
					{
						this.Current = this.selector(this.enumerator.Current);
						goto IL_D2;
					}
					IL_A2:;
				}
				catch (Exception error)
				{
					this.state = -2;
					this.completionSource.TrySetException(error);
					return;
				}
				this.state = -2;
				this.completionSource.TrySetResult(false);
				return;
				IL_D2:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, TResult> selector;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private Action moveNextAction;
		}
	}
}
