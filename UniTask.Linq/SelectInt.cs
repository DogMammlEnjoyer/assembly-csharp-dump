using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SelectInt<TSource, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public SelectInt(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			this.source = source;
			this.selector = selector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SelectInt<TSource, TResult>._Select(this.source, this.selector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, TResult> selector;

		private sealed class _Select : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _Select(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, TResult> selector, CancellationToken cancellationToken)
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
						goto IL_7A;
					default:
						goto IL_B6;
					}
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (!this.awaiter.IsCompleted)
					{
						this.state = 1;
						this.awaiter.UnsafeOnCompleted(this.moveNextAction);
						return;
					}
					IL_7A:
					if (this.awaiter.GetResult())
					{
						Func<TSource, int, TResult> func = this.selector;
						TSource arg = this.enumerator.Current;
						int num = this.index;
						this.index = checked(num + 1);
						this.Current = func(arg, num);
						goto IL_E6;
					}
					IL_B6:;
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
				IL_E6:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, int, TResult> selector;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private Action moveNextAction;

			private int index;
		}
	}
}
