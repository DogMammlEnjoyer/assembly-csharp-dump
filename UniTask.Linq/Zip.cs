using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Zip<TFirst, TSecond, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public Zip(IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			this.first = first;
			this.second = second;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Zip<TFirst, TSecond, TResult>._Zip(this.first, this.second, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TFirst> first;

		private readonly IUniTaskAsyncEnumerable<TSecond> second;

		private readonly Func<TFirst, TSecond, TResult> resultSelector;

		private sealed class _Zip : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _Zip(IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector, CancellationToken cancellationToken)
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
					Zip<TFirst, TSecond, TResult>._Zip.FirstMoveNextCore(this);
				}
				else
				{
					this.firstAwaiter.SourceOnCompleted(Zip<TFirst, TSecond, TResult>._Zip.firstMoveNextCoreDelegate, this);
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void FirstMoveNextCore(object state)
			{
				Zip<TFirst, TSecond, TResult>._Zip zip = (Zip<TFirst, TSecond, TResult>._Zip)state;
				bool flag;
				if (zip.TryGetResult<bool>(zip.firstAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							zip.secondAwaiter = zip.secondEnumerator.MoveNextAsync().GetAwaiter();
						}
						catch (Exception error)
						{
							zip.completionSource.TrySetException(error);
							return;
						}
						if (zip.secondAwaiter.IsCompleted)
						{
							Zip<TFirst, TSecond, TResult>._Zip.SecondMoveNextCore(zip);
							return;
						}
						zip.secondAwaiter.SourceOnCompleted(Zip<TFirst, TSecond, TResult>._Zip.secondMoveNextCoreDelegate, zip);
						return;
					}
					else
					{
						zip.completionSource.TrySetResult(false);
					}
				}
			}

			private static void SecondMoveNextCore(object state)
			{
				Zip<TFirst, TSecond, TResult>._Zip zip = (Zip<TFirst, TSecond, TResult>._Zip)state;
				bool flag;
				if (zip.TryGetResult<bool>(zip.secondAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							zip.Current = zip.resultSelector(zip.firstEnumerator.Current, zip.secondEnumerator.Current);
						}
						catch (Exception error)
						{
							zip.completionSource.TrySetException(error);
						}
						if (zip.cancellationToken.IsCancellationRequested)
						{
							zip.completionSource.TrySetCanceled(zip.cancellationToken);
							return;
						}
						zip.completionSource.TrySetResult(true);
						return;
					}
					else
					{
						zip.completionSource.TrySetResult(false);
					}
				}
			}

			public UniTask DisposeAsync()
			{
				Zip<TFirst, TSecond, TResult>._Zip.<DisposeAsync>d__18 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<Zip<TFirst, TSecond, TResult>._Zip.<DisposeAsync>d__18>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> firstMoveNextCoreDelegate = new Action<object>(Zip<TFirst, TSecond, TResult>._Zip.FirstMoveNextCore);

			private static readonly Action<object> secondMoveNextCoreDelegate = new Action<object>(Zip<TFirst, TSecond, TResult>._Zip.SecondMoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TFirst> first;

			private readonly IUniTaskAsyncEnumerable<TSecond> second;

			private readonly Func<TFirst, TSecond, TResult> resultSelector;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TFirst> firstEnumerator;

			private IUniTaskAsyncEnumerator<TSecond> secondEnumerator;

			private UniTask<bool>.Awaiter firstAwaiter;

			private UniTask<bool>.Awaiter secondAwaiter;
		}
	}
}
