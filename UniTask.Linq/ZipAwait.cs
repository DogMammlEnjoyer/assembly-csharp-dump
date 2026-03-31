using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class ZipAwait<TFirst, TSecond, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public ZipAwait(IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, UniTask<TResult>> resultSelector)
		{
			this.first = first;
			this.second = second;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ZipAwait<TFirst, TSecond, TResult>._ZipAwait(this.first, this.second, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TFirst> first;

		private readonly IUniTaskAsyncEnumerable<TSecond> second;

		private readonly Func<TFirst, TSecond, UniTask<TResult>> resultSelector;

		private sealed class _ZipAwait : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _ZipAwait(IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, UniTask<TResult>> resultSelector, CancellationToken cancellationToken)
			{
				this.first = first;
				this.second = second;
				this.resultSelector = resultSelector;
				this.cancellationToken = cancellationToken;
			}

			public TResult Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.completionSource.Reset();
				if (this.firstEnumerator == null)
				{
					this.firstEnumerator = this.first.GetAsyncEnumerator(this.cancellationToken);
					this.secondEnumerator = this.second.GetAsyncEnumerator(this.cancellationToken);
				}
				this.firstAwaiter = this.firstEnumerator.MoveNextAsync().GetAwaiter();
				if (this.firstAwaiter.IsCompleted)
				{
					ZipAwait<TFirst, TSecond, TResult>._ZipAwait.FirstMoveNextCore(this);
				}
				else
				{
					this.firstAwaiter.SourceOnCompleted(ZipAwait<TFirst, TSecond, TResult>._ZipAwait.firstMoveNextCoreDelegate, this);
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void FirstMoveNextCore(object state)
			{
				ZipAwait<TFirst, TSecond, TResult>._ZipAwait zipAwait = (ZipAwait<TFirst, TSecond, TResult>._ZipAwait)state;
				bool flag;
				if (zipAwait.TryGetResult<bool>(zipAwait.firstAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							zipAwait.secondAwaiter = zipAwait.secondEnumerator.MoveNextAsync().GetAwaiter();
						}
						catch (Exception error)
						{
							zipAwait.completionSource.TrySetException(error);
							return;
						}
						if (zipAwait.secondAwaiter.IsCompleted)
						{
							ZipAwait<TFirst, TSecond, TResult>._ZipAwait.SecondMoveNextCore(zipAwait);
							return;
						}
						zipAwait.secondAwaiter.SourceOnCompleted(ZipAwait<TFirst, TSecond, TResult>._ZipAwait.secondMoveNextCoreDelegate, zipAwait);
						return;
					}
					else
					{
						zipAwait.completionSource.TrySetResult(false);
					}
				}
			}

			private static void SecondMoveNextCore(object state)
			{
				ZipAwait<TFirst, TSecond, TResult>._ZipAwait zipAwait = (ZipAwait<TFirst, TSecond, TResult>._ZipAwait)state;
				bool flag;
				if (zipAwait.TryGetResult<bool>(zipAwait.secondAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							zipAwait.resultAwaiter = zipAwait.resultSelector(zipAwait.firstEnumerator.Current, zipAwait.secondEnumerator.Current).GetAwaiter();
							if (zipAwait.resultAwaiter.IsCompleted)
							{
								ZipAwait<TFirst, TSecond, TResult>._ZipAwait.ResultAwaitCore(zipAwait);
							}
							else
							{
								zipAwait.resultAwaiter.SourceOnCompleted(ZipAwait<TFirst, TSecond, TResult>._ZipAwait.resultAwaitCoreDelegate, zipAwait);
							}
							return;
						}
						catch (Exception error)
						{
							zipAwait.completionSource.TrySetException(error);
							return;
						}
					}
					zipAwait.completionSource.TrySetResult(false);
				}
			}

			private static void ResultAwaitCore(object state)
			{
				ZipAwait<TFirst, TSecond, TResult>._ZipAwait zipAwait = (ZipAwait<TFirst, TSecond, TResult>._ZipAwait)state;
				TResult value;
				if (zipAwait.TryGetResult<TResult>(zipAwait.resultAwaiter, out value))
				{
					zipAwait.Current = value;
					if (zipAwait.cancellationToken.IsCancellationRequested)
					{
						zipAwait.completionSource.TrySetCanceled(zipAwait.cancellationToken);
						return;
					}
					zipAwait.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				ZipAwait<TFirst, TSecond, TResult>._ZipAwait.<DisposeAsync>d__21 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<ZipAwait<TFirst, TSecond, TResult>._ZipAwait.<DisposeAsync>d__21>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> firstMoveNextCoreDelegate = new Action<object>(ZipAwait<TFirst, TSecond, TResult>._ZipAwait.FirstMoveNextCore);

			private static readonly Action<object> secondMoveNextCoreDelegate = new Action<object>(ZipAwait<TFirst, TSecond, TResult>._ZipAwait.SecondMoveNextCore);

			private static readonly Action<object> resultAwaitCoreDelegate = new Action<object>(ZipAwait<TFirst, TSecond, TResult>._ZipAwait.ResultAwaitCore);

			private readonly IUniTaskAsyncEnumerable<TFirst> first;

			private readonly IUniTaskAsyncEnumerable<TSecond> second;

			private readonly Func<TFirst, TSecond, UniTask<TResult>> resultSelector;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TFirst> firstEnumerator;

			private IUniTaskAsyncEnumerator<TSecond> secondEnumerator;

			private UniTask<bool>.Awaiter firstAwaiter;

			private UniTask<bool>.Awaiter secondAwaiter;

			private UniTask<TResult>.Awaiter resultAwaiter;
		}
	}
}
