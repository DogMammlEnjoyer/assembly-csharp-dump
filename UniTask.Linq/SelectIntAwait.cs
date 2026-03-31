using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SelectIntAwait<TSource, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public SelectIntAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<TResult>> selector)
		{
			this.source = source;
			this.selector = selector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SelectIntAwait<TSource, TResult>._SelectAwait(this.source, this.selector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, UniTask<TResult>> selector;

		private sealed class _SelectAwait : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _SelectAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<TResult>> selector, CancellationToken cancellationToken)
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
						goto IL_E9;
					default:
						goto IL_118;
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
						goto IL_118;
					}
					Func<TSource, int, UniTask<TResult>> func = this.selector;
					TSource arg = this.enumerator.Current;
					int num = this.index;
					this.index = checked(num + 1);
					this.awaiter2 = func(arg, num).GetAwaiter();
					if (!this.awaiter2.IsCompleted)
					{
						this.state = 2;
						this.awaiter2.UnsafeOnCompleted(this.moveNextAction);
						return;
					}
					IL_E9:
					this.Current = this.awaiter2.GetResult();
					goto IL_12E;
				}
				catch (Exception error)
				{
					this.state = -2;
					this.completionSource.TrySetException(error);
					return;
				}
				IL_118:
				this.state = -2;
				this.completionSource.TrySetResult(false);
				return;
				IL_12E:
				this.state = 0;
				this.completionSource.TrySetResult(true);
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, int, UniTask<TResult>> selector;

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
