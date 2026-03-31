using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class ZipAwaitWithCancellation<TFirst, TSecond, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public ZipAwaitWithCancellation(IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, UniTask<TResult>> resultSelector)
		{
			this.first = first;
			this.second = second;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation(this.first, this.second, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TFirst> first;

		private readonly IUniTaskAsyncEnumerable<TSecond> second;

		private readonly Func<TFirst, TSecond, CancellationToken, UniTask<TResult>> resultSelector;

		private sealed class _ZipAwaitWithCancellation : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _ZipAwaitWithCancellation(IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, UniTask<TResult>> resultSelector, CancellationToken cancellationToken)
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
					ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.FirstMoveNextCore(this);
				}
				else
				{
					this.firstAwaiter.SourceOnCompleted(ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.firstMoveNextCoreDelegate, this);
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void FirstMoveNextCore(object state)
			{
				ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation zipAwaitWithCancellation = (ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation)state;
				bool flag;
				if (zipAwaitWithCancellation.TryGetResult<bool>(zipAwaitWithCancellation.firstAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							zipAwaitWithCancellation.secondAwaiter = zipAwaitWithCancellation.secondEnumerator.MoveNextAsync().GetAwaiter();
						}
						catch (Exception error)
						{
							zipAwaitWithCancellation.completionSource.TrySetException(error);
							return;
						}
						if (zipAwaitWithCancellation.secondAwaiter.IsCompleted)
						{
							ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.SecondMoveNextCore(zipAwaitWithCancellation);
							return;
						}
						zipAwaitWithCancellation.secondAwaiter.SourceOnCompleted(ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.secondMoveNextCoreDelegate, zipAwaitWithCancellation);
						return;
					}
					else
					{
						zipAwaitWithCancellation.completionSource.TrySetResult(false);
					}
				}
			}

			private static void SecondMoveNextCore(object state)
			{
				ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation zipAwaitWithCancellation = (ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation)state;
				bool flag;
				if (zipAwaitWithCancellation.TryGetResult<bool>(zipAwaitWithCancellation.secondAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							zipAwaitWithCancellation.resultAwaiter = zipAwaitWithCancellation.resultSelector(zipAwaitWithCancellation.firstEnumerator.Current, zipAwaitWithCancellation.secondEnumerator.Current, zipAwaitWithCancellation.cancellationToken).GetAwaiter();
							if (zipAwaitWithCancellation.resultAwaiter.IsCompleted)
							{
								ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.ResultAwaitCore(zipAwaitWithCancellation);
							}
							else
							{
								zipAwaitWithCancellation.resultAwaiter.SourceOnCompleted(ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.resultAwaitCoreDelegate, zipAwaitWithCancellation);
							}
							return;
						}
						catch (Exception error)
						{
							zipAwaitWithCancellation.completionSource.TrySetException(error);
							return;
						}
					}
					zipAwaitWithCancellation.completionSource.TrySetResult(false);
				}
			}

			private static void ResultAwaitCore(object state)
			{
				ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation zipAwaitWithCancellation = (ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation)state;
				TResult value;
				if (zipAwaitWithCancellation.TryGetResult<TResult>(zipAwaitWithCancellation.resultAwaiter, out value))
				{
					zipAwaitWithCancellation.Current = value;
					if (zipAwaitWithCancellation.cancellationToken.IsCancellationRequested)
					{
						zipAwaitWithCancellation.completionSource.TrySetCanceled(zipAwaitWithCancellation.cancellationToken);
						return;
					}
					zipAwaitWithCancellation.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.<DisposeAsync>d__21 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.<DisposeAsync>d__21>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> firstMoveNextCoreDelegate = new Action<object>(ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.FirstMoveNextCore);

			private static readonly Action<object> secondMoveNextCoreDelegate = new Action<object>(ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.SecondMoveNextCore);

			private static readonly Action<object> resultAwaitCoreDelegate = new Action<object>(ZipAwaitWithCancellation<TFirst, TSecond, TResult>._ZipAwaitWithCancellation.ResultAwaitCore);

			private readonly IUniTaskAsyncEnumerable<TFirst> first;

			private readonly IUniTaskAsyncEnumerable<TSecond> second;

			private readonly Func<TFirst, TSecond, CancellationToken, UniTask<TResult>> resultSelector;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TFirst> firstEnumerator;

			private IUniTaskAsyncEnumerator<TSecond> secondEnumerator;

			private UniTask<bool>.Awaiter firstAwaiter;

			private UniTask<bool>.Awaiter secondAwaiter;

			private UniTask<TResult>.Awaiter resultAwaiter;
		}
	}
}
