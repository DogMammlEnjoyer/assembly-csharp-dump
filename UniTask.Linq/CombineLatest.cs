using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class CombineLatest<T1, T2, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public CombineLatest(IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, Func<T1, T2, TResult> resultSelector)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new CombineLatest<T1, T2, TResult>._CombineLatest(this.source1, this.source2, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<T1> source1;

		private readonly IUniTaskAsyncEnumerable<T2> source2;

		private readonly Func<T1, T2, TResult> resultSelector;

		private class _CombineLatest : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _CombineLatest(IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, Func<T1, T2, TResult> resultSelector, CancellationToken cancellationToken)
			{
				this.source1 = source1;
				this.source2 = source2;
				this.resultSelector = resultSelector;
				this.cancellationToken = cancellationToken;
			}

			public TResult Current
			{
				get
				{
					return this.result;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.completedCount == 2)
				{
					return CompletedTasks.False;
				}
				if (this.enumerator1 == null)
				{
					this.enumerator1 = this.source1.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator2 = this.source2.GetAsyncEnumerator(this.cancellationToken);
				}
				this.completionSource.Reset();
				do
				{
					this.syncRunning = true;
					if (!this.running1)
					{
						this.running1 = true;
						this.awaiter1 = this.enumerator1.MoveNextAsync().GetAwaiter();
						if (this.awaiter1.IsCompleted)
						{
							CombineLatest<T1, T2, TResult>._CombineLatest.Completed1(this);
						}
						else
						{
							this.awaiter1.SourceOnCompleted(CombineLatest<T1, T2, TResult>._CombineLatest.Completed1Delegate, this);
						}
					}
					if (!this.running2)
					{
						this.running2 = true;
						this.awaiter2 = this.enumerator2.MoveNextAsync().GetAwaiter();
						if (this.awaiter2.IsCompleted)
						{
							CombineLatest<T1, T2, TResult>._CombineLatest.Completed2(this);
						}
						else
						{
							this.awaiter2.SourceOnCompleted(CombineLatest<T1, T2, TResult>._CombineLatest.Completed2Delegate, this);
						}
					}
				}
				while (!this.running1 || !this.running2);
				this.syncRunning = false;
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void Completed1(object state)
			{
				CombineLatest<T1, T2, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, TResult>._CombineLatest)state;
				combineLatest.running1 = false;
				try
				{
					if (combineLatest.awaiter1.GetResult())
					{
						combineLatest.hasCurrent1 = true;
						combineLatest.current1 = combineLatest.enumerator1.Current;
					}
					else
					{
						combineLatest.running1 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 2)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running1 = true;
					combineLatest.completedCount = 2;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running1 = true;
					try
					{
						combineLatest.awaiter1 = combineLatest.enumerator1.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 2;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter1.SourceOnCompleted(CombineLatest<T1, T2, TResult>._CombineLatest.Completed1Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed2(object state)
			{
				CombineLatest<T1, T2, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, TResult>._CombineLatest)state;
				combineLatest.running2 = false;
				try
				{
					if (combineLatest.awaiter2.GetResult())
					{
						combineLatest.hasCurrent2 = true;
						combineLatest.current2 = combineLatest.enumerator2.Current;
					}
					else
					{
						combineLatest.running2 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 2)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running2 = true;
					combineLatest.completedCount = 2;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running2 = true;
					try
					{
						combineLatest.awaiter2 = combineLatest.enumerator2.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 2;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter2.SourceOnCompleted(CombineLatest<T1, T2, TResult>._CombineLatest.Completed2Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private bool TrySetResult()
			{
				if (this.hasCurrent1 && this.hasCurrent2)
				{
					this.result = this.resultSelector(this.current1, this.current2);
					this.completionSource.TrySetResult(true);
					return true;
				}
				return false;
			}

			public UniTask DisposeAsync()
			{
				CombineLatest<T1, T2, TResult>._CombineLatest.<DisposeAsync>d__27 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<CombineLatest<T1, T2, TResult>._CombineLatest.<DisposeAsync>d__27>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> Completed1Delegate = new Action<object>(CombineLatest<T1, T2, TResult>._CombineLatest.Completed1);

			private static readonly Action<object> Completed2Delegate = new Action<object>(CombineLatest<T1, T2, TResult>._CombineLatest.Completed2);

			private const int CompleteCount = 2;

			private readonly IUniTaskAsyncEnumerable<T1> source1;

			private readonly IUniTaskAsyncEnumerable<T2> source2;

			private readonly Func<T1, T2, TResult> resultSelector;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<T1> enumerator1;

			private UniTask<bool>.Awaiter awaiter1;

			private bool hasCurrent1;

			private bool running1;

			private T1 current1;

			private IUniTaskAsyncEnumerator<T2> enumerator2;

			private UniTask<bool>.Awaiter awaiter2;

			private bool hasCurrent2;

			private bool running2;

			private T2 current2;

			private int completedCount;

			private bool syncRunning;

			private TResult result;
		}
	}
}
