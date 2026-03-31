using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SelectIntAwaitWithCancellation<TSource, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public SelectIntAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<TResult>> selector)
		{
			this.source = source;
			this.selector = selector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SelectIntAwaitWithCancellation<TSource, TResult>._SelectAwaitWithCancellation(this.source, this.selector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, CancellationToken, UniTask<TResult>> selector;

		private sealed class _SelectAwaitWithCancellation : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _SelectAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<TResult>> selector, CancellationToken cancellationToken)
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
						goto IL_7E;
					case 2:
						goto IL_EF;
					default:
						goto IL_11E;
					}
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (!this.awaiter.IsCompleted)
					{
						this.state = 1;
						this.awaiter.UnsafeOnCompleted(this.moveNextAction);
						return;
					}
					IL_7E:
					if (!this.awaiter.GetResult())
					{
						goto IL_11E;
					}
					Func<TSource, int, CancellationToken, UniTask<TResult>> func = this.selector;
					TSource arg = this.enumerator.Current;
					int num = this.index;
					this.index = checked(num + 1);
					this.awaiter2 = func(arg, num, this.cancellationToken).GetAwaiter();
					if (!this.awaiter2.IsCompleted)
					{
						this.state = 2;
						this.awaiter2.UnsafeOnCompleted(this.moveNextAction);
						return;
					}
					IL_EF:
					this.Current = this.awaiter2.GetResult();
					goto IL_134;
				}
				catch (Exception error)
				{
					this.state = -2;
					this.completionSource.TrySetException(error);
					return;
				}
				IL_11E:
				this.state = -2;
				this.completionSource.TrySetResult(false);
				return;
				IL_134:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, int, CancellationToken, UniTask<TResult>> selector;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private UniTask<TResult>.Awaiter awaiter2;

			private Action moveNextAction;

			private int index;
		}
	}
}
