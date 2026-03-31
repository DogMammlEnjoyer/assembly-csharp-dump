using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public CombineLatest(IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resultSelector)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.source4 = source4;
			this.source5 = source5;
			this.source6 = source6;
			this.source7 = source7;
			this.source8 = source8;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest(this.source1, this.source2, this.source3, this.source4, this.source5, this.source6, this.source7, this.source8, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<T1> source1;

		private readonly IUniTaskAsyncEnumerable<T2> source2;

		private readonly IUniTaskAsyncEnumerable<T3> source3;

		private readonly IUniTaskAsyncEnumerable<T4> source4;

		private readonly IUniTaskAsyncEnumerable<T5> source5;

		private readonly IUniTaskAsyncEnumerable<T6> source6;

		private readonly IUniTaskAsyncEnumerable<T7> source7;

		private readonly IUniTaskAsyncEnumerable<T8> source8;

		private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resultSelector;

		private class _CombineLatest : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _CombineLatest(IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resultSelector, CancellationToken cancellationToken)
			{
				this.source1 = source1;
				this.source2 = source2;
				this.source3 = source3;
				this.source4 = source4;
				this.source5 = source5;
				this.source6 = source6;
				this.source7 = source7;
				this.source8 = source8;
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
				if (this.completedCount == 8)
				{
					return CompletedTasks.False;
				}
				if (this.enumerator1 == null)
				{
					this.enumerator1 = this.source1.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator2 = this.source2.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator3 = this.source3.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator4 = this.source4.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator5 = this.source5.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator6 = this.source6.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator7 = this.source7.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator8 = this.source8.GetAsyncEnumerator(this.cancellationToken);
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
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed1(this);
						}
						else
						{
							this.awaiter1.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed1Delegate, this);
						}
					}
					if (!this.running2)
					{
						this.running2 = true;
						this.awaiter2 = this.enumerator2.MoveNextAsync().GetAwaiter();
						if (this.awaiter2.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed2(this);
						}
						else
						{
							this.awaiter2.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed2Delegate, this);
						}
					}
					if (!this.running3)
					{
						this.running3 = true;
						this.awaiter3 = this.enumerator3.MoveNextAsync().GetAwaiter();
						if (this.awaiter3.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed3(this);
						}
						else
						{
							this.awaiter3.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed3Delegate, this);
						}
					}
					if (!this.running4)
					{
						this.running4 = true;
						this.awaiter4 = this.enumerator4.MoveNextAsync().GetAwaiter();
						if (this.awaiter4.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed4(this);
						}
						else
						{
							this.awaiter4.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed4Delegate, this);
						}
					}
					if (!this.running5)
					{
						this.running5 = true;
						this.awaiter5 = this.enumerator5.MoveNextAsync().GetAwaiter();
						if (this.awaiter5.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed5(this);
						}
						else
						{
							this.awaiter5.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed5Delegate, this);
						}
					}
					if (!this.running6)
					{
						this.running6 = true;
						this.awaiter6 = this.enumerator6.MoveNextAsync().GetAwaiter();
						if (this.awaiter6.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed6(this);
						}
						else
						{
							this.awaiter6.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed6Delegate, this);
						}
					}
					if (!this.running7)
					{
						this.running7 = true;
						this.awaiter7 = this.enumerator7.MoveNextAsync().GetAwaiter();
						if (this.awaiter7.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed7(this);
						}
						else
						{
							this.awaiter7.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed7Delegate, this);
						}
					}
					if (!this.running8)
					{
						this.running8 = true;
						this.awaiter8 = this.enumerator8.MoveNextAsync().GetAwaiter();
						if (this.awaiter8.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed8(this);
						}
						else
						{
							this.awaiter8.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed8Delegate, this);
						}
					}
				}
				while (!this.running1 || !this.running2 || !this.running3 || !this.running4 || !this.running5 || !this.running6 || !this.running7 || !this.running8);
				this.syncRunning = false;
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void Completed1(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running1 = true;
					combineLatest.completedCount = 8;
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
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter1.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed1Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed2(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running2 = true;
					combineLatest.completedCount = 8;
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
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter2.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed2Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed3(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
				combineLatest.running3 = false;
				try
				{
					if (combineLatest.awaiter3.GetResult())
					{
						combineLatest.hasCurrent3 = true;
						combineLatest.current3 = combineLatest.enumerator3.Current;
					}
					else
					{
						combineLatest.running3 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running3 = true;
					combineLatest.completedCount = 8;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running3 = true;
					try
					{
						combineLatest.awaiter3 = combineLatest.enumerator3.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter3.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed3Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed4(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
				combineLatest.running4 = false;
				try
				{
					if (combineLatest.awaiter4.GetResult())
					{
						combineLatest.hasCurrent4 = true;
						combineLatest.current4 = combineLatest.enumerator4.Current;
					}
					else
					{
						combineLatest.running4 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running4 = true;
					combineLatest.completedCount = 8;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running4 = true;
					try
					{
						combineLatest.awaiter4 = combineLatest.enumerator4.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter4.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed4Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed5(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
				combineLatest.running5 = false;
				try
				{
					if (combineLatest.awaiter5.GetResult())
					{
						combineLatest.hasCurrent5 = true;
						combineLatest.current5 = combineLatest.enumerator5.Current;
					}
					else
					{
						combineLatest.running5 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running5 = true;
					combineLatest.completedCount = 8;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running5 = true;
					try
					{
						combineLatest.awaiter5 = combineLatest.enumerator5.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter5.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed5Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed6(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
				combineLatest.running6 = false;
				try
				{
					if (combineLatest.awaiter6.GetResult())
					{
						combineLatest.hasCurrent6 = true;
						combineLatest.current6 = combineLatest.enumerator6.Current;
					}
					else
					{
						combineLatest.running6 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running6 = true;
					combineLatest.completedCount = 8;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running6 = true;
					try
					{
						combineLatest.awaiter6 = combineLatest.enumerator6.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter6.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed6Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed7(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
				combineLatest.running7 = false;
				try
				{
					if (combineLatest.awaiter7.GetResult())
					{
						combineLatest.hasCurrent7 = true;
						combineLatest.current7 = combineLatest.enumerator7.Current;
					}
					else
					{
						combineLatest.running7 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running7 = true;
					combineLatest.completedCount = 8;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running7 = true;
					try
					{
						combineLatest.awaiter7 = combineLatest.enumerator7.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter7.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed7Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed8(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest)state;
				combineLatest.running8 = false;
				try
				{
					if (combineLatest.awaiter8.GetResult())
					{
						combineLatest.hasCurrent8 = true;
						combineLatest.current8 = combineLatest.enumerator8.Current;
					}
					else
					{
						combineLatest.running8 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 8)
						{
							goto IL_CB;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running8 = true;
					combineLatest.completedCount = 8;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running8 = true;
					try
					{
						combineLatest.awaiter8 = combineLatest.enumerator8.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 8;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter8.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed8Delegate, combineLatest);
				}
				return;
				IL_CB:
				combineLatest.completionSource.TrySetResult(false);
			}

			private bool TrySetResult()
			{
				if (this.hasCurrent1 && this.hasCurrent2 && this.hasCurrent3 && this.hasCurrent4 && this.hasCurrent5 && this.hasCurrent6 && this.hasCurrent7 && this.hasCurrent8)
				{
					this.result = this.resultSelector(this.current1, this.current2, this.current3, this.current4, this.current5, this.current6, this.current7, this.current8);
					this.completionSource.TrySetResult(true);
					return true;
				}
				return false;
			}

			public UniTask DisposeAsync()
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.<DisposeAsync>d__75 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.<DisposeAsync>d__75>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> Completed1Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed1);

			private static readonly Action<object> Completed2Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed2);

			private static readonly Action<object> Completed3Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed3);

			private static readonly Action<object> Completed4Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed4);

			private static readonly Action<object> Completed5Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed5);

			private static readonly Action<object> Completed6Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed6);

			private static readonly Action<object> Completed7Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed7);

			private static readonly Action<object> Completed8Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>._CombineLatest.Completed8);

			private const int CompleteCount = 8;

			private readonly IUniTaskAsyncEnumerable<T1> source1;

			private readonly IUniTaskAsyncEnumerable<T2> source2;

			private readonly IUniTaskAsyncEnumerable<T3> source3;

			private readonly IUniTaskAsyncEnumerable<T4> source4;

			private readonly IUniTaskAsyncEnumerable<T5> source5;

			private readonly IUniTaskAsyncEnumerable<T6> source6;

			private readonly IUniTaskAsyncEnumerable<T7> source7;

			private readonly IUniTaskAsyncEnumerable<T8> source8;

			private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resultSelector;

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

			private IUniTaskAsyncEnumerator<T3> enumerator3;

			private UniTask<bool>.Awaiter awaiter3;

			private bool hasCurrent3;

			private bool running3;

			private T3 current3;

			private IUniTaskAsyncEnumerator<T4> enumerator4;

			private UniTask<bool>.Awaiter awaiter4;

			private bool hasCurrent4;

			private bool running4;

			private T4 current4;

			private IUniTaskAsyncEnumerator<T5> enumerator5;

			private UniTask<bool>.Awaiter awaiter5;

			private bool hasCurrent5;

			private bool running5;

			private T5 current5;

			private IUniTaskAsyncEnumerator<T6> enumerator6;

			private UniTask<bool>.Awaiter awaiter6;

			private bool hasCurrent6;

			private bool running6;

			private T6 current6;

			private IUniTaskAsyncEnumerator<T7> enumerator7;

			private UniTask<bool>.Awaiter awaiter7;

			private bool hasCurrent7;

			private bool running7;

			private T7 current7;

			private IUniTaskAsyncEnumerator<T8> enumerator8;

			private UniTask<bool>.Awaiter awaiter8;

			private bool hasCurrent8;

			private bool running8;

			private T8 current8;

			private int completedCount;

			private bool syncRunning;

			private TResult result;
		}
	}
}
