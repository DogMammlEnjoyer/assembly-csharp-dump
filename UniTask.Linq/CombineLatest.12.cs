using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public CombineLatest(IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, IUniTaskAsyncEnumerable<T12> source12, IUniTaskAsyncEnumerable<T13> source13, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> resultSelector)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.source3 = source3;
			this.source4 = source4;
			this.source5 = source5;
			this.source6 = source6;
			this.source7 = source7;
			this.source8 = source8;
			this.source9 = source9;
			this.source10 = source10;
			this.source11 = source11;
			this.source12 = source12;
			this.source13 = source13;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest(this.source1, this.source2, this.source3, this.source4, this.source5, this.source6, this.source7, this.source8, this.source9, this.source10, this.source11, this.source12, this.source13, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<T1> source1;

		private readonly IUniTaskAsyncEnumerable<T2> source2;

		private readonly IUniTaskAsyncEnumerable<T3> source3;

		private readonly IUniTaskAsyncEnumerable<T4> source4;

		private readonly IUniTaskAsyncEnumerable<T5> source5;

		private readonly IUniTaskAsyncEnumerable<T6> source6;

		private readonly IUniTaskAsyncEnumerable<T7> source7;

		private readonly IUniTaskAsyncEnumerable<T8> source8;

		private readonly IUniTaskAsyncEnumerable<T9> source9;

		private readonly IUniTaskAsyncEnumerable<T10> source10;

		private readonly IUniTaskAsyncEnumerable<T11> source11;

		private readonly IUniTaskAsyncEnumerable<T12> source12;

		private readonly IUniTaskAsyncEnumerable<T13> source13;

		private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> resultSelector;

		private class _CombineLatest : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _CombineLatest(IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, IUniTaskAsyncEnumerable<T12> source12, IUniTaskAsyncEnumerable<T13> source13, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> resultSelector, CancellationToken cancellationToken)
			{
				this.source1 = source1;
				this.source2 = source2;
				this.source3 = source3;
				this.source4 = source4;
				this.source5 = source5;
				this.source6 = source6;
				this.source7 = source7;
				this.source8 = source8;
				this.source9 = source9;
				this.source10 = source10;
				this.source11 = source11;
				this.source12 = source12;
				this.source13 = source13;
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
				if (this.completedCount == 13)
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
					this.enumerator9 = this.source9.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator10 = this.source10.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator11 = this.source11.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator12 = this.source12.GetAsyncEnumerator(this.cancellationToken);
					this.enumerator13 = this.source13.GetAsyncEnumerator(this.cancellationToken);
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
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed1(this);
						}
						else
						{
							this.awaiter1.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed1Delegate, this);
						}
					}
					if (!this.running2)
					{
						this.running2 = true;
						this.awaiter2 = this.enumerator2.MoveNextAsync().GetAwaiter();
						if (this.awaiter2.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed2(this);
						}
						else
						{
							this.awaiter2.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed2Delegate, this);
						}
					}
					if (!this.running3)
					{
						this.running3 = true;
						this.awaiter3 = this.enumerator3.MoveNextAsync().GetAwaiter();
						if (this.awaiter3.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed3(this);
						}
						else
						{
							this.awaiter3.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed3Delegate, this);
						}
					}
					if (!this.running4)
					{
						this.running4 = true;
						this.awaiter4 = this.enumerator4.MoveNextAsync().GetAwaiter();
						if (this.awaiter4.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed4(this);
						}
						else
						{
							this.awaiter4.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed4Delegate, this);
						}
					}
					if (!this.running5)
					{
						this.running5 = true;
						this.awaiter5 = this.enumerator5.MoveNextAsync().GetAwaiter();
						if (this.awaiter5.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed5(this);
						}
						else
						{
							this.awaiter5.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed5Delegate, this);
						}
					}
					if (!this.running6)
					{
						this.running6 = true;
						this.awaiter6 = this.enumerator6.MoveNextAsync().GetAwaiter();
						if (this.awaiter6.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed6(this);
						}
						else
						{
							this.awaiter6.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed6Delegate, this);
						}
					}
					if (!this.running7)
					{
						this.running7 = true;
						this.awaiter7 = this.enumerator7.MoveNextAsync().GetAwaiter();
						if (this.awaiter7.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed7(this);
						}
						else
						{
							this.awaiter7.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed7Delegate, this);
						}
					}
					if (!this.running8)
					{
						this.running8 = true;
						this.awaiter8 = this.enumerator8.MoveNextAsync().GetAwaiter();
						if (this.awaiter8.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed8(this);
						}
						else
						{
							this.awaiter8.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed8Delegate, this);
						}
					}
					if (!this.running9)
					{
						this.running9 = true;
						this.awaiter9 = this.enumerator9.MoveNextAsync().GetAwaiter();
						if (this.awaiter9.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed9(this);
						}
						else
						{
							this.awaiter9.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed9Delegate, this);
						}
					}
					if (!this.running10)
					{
						this.running10 = true;
						this.awaiter10 = this.enumerator10.MoveNextAsync().GetAwaiter();
						if (this.awaiter10.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed10(this);
						}
						else
						{
							this.awaiter10.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed10Delegate, this);
						}
					}
					if (!this.running11)
					{
						this.running11 = true;
						this.awaiter11 = this.enumerator11.MoveNextAsync().GetAwaiter();
						if (this.awaiter11.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed11(this);
						}
						else
						{
							this.awaiter11.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed11Delegate, this);
						}
					}
					if (!this.running12)
					{
						this.running12 = true;
						this.awaiter12 = this.enumerator12.MoveNextAsync().GetAwaiter();
						if (this.awaiter12.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed12(this);
						}
						else
						{
							this.awaiter12.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed12Delegate, this);
						}
					}
					if (!this.running13)
					{
						this.running13 = true;
						this.awaiter13 = this.enumerator13.MoveNextAsync().GetAwaiter();
						if (this.awaiter13.IsCompleted)
						{
							CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed13(this);
						}
						else
						{
							this.awaiter13.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed13Delegate, this);
						}
					}
				}
				while (!this.running1 || !this.running2 || !this.running3 || !this.running4 || !this.running5 || !this.running6 || !this.running7 || !this.running8 || !this.running9 || !this.running10 || !this.running11 || !this.running12 || !this.running13);
				this.syncRunning = false;
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void Completed1(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running1 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter1.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed1Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed2(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running2 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter2.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed2Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed3(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running3 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter3.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed3Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed4(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running4 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter4.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed4Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed5(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running5 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter5.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed5Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed6(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running6 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter6.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed6Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed7(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running7 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter7.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed7Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed8(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
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
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running8 = true;
					combineLatest.completedCount = 13;
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
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter8.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed8Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed9(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
				combineLatest.running9 = false;
				try
				{
					if (combineLatest.awaiter9.GetResult())
					{
						combineLatest.hasCurrent9 = true;
						combineLatest.current9 = combineLatest.enumerator9.Current;
					}
					else
					{
						combineLatest.running9 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running9 = true;
					combineLatest.completedCount = 13;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running9 = true;
					try
					{
						combineLatest.awaiter9 = combineLatest.enumerator9.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter9.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed9Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed10(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
				combineLatest.running10 = false;
				try
				{
					if (combineLatest.awaiter10.GetResult())
					{
						combineLatest.hasCurrent10 = true;
						combineLatest.current10 = combineLatest.enumerator10.Current;
					}
					else
					{
						combineLatest.running10 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running10 = true;
					combineLatest.completedCount = 13;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running10 = true;
					try
					{
						combineLatest.awaiter10 = combineLatest.enumerator10.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter10.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed10Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed11(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
				combineLatest.running11 = false;
				try
				{
					if (combineLatest.awaiter11.GetResult())
					{
						combineLatest.hasCurrent11 = true;
						combineLatest.current11 = combineLatest.enumerator11.Current;
					}
					else
					{
						combineLatest.running11 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running11 = true;
					combineLatest.completedCount = 13;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running11 = true;
					try
					{
						combineLatest.awaiter11 = combineLatest.enumerator11.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter11.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed11Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed12(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
				combineLatest.running12 = false;
				try
				{
					if (combineLatest.awaiter12.GetResult())
					{
						combineLatest.hasCurrent12 = true;
						combineLatest.current12 = combineLatest.enumerator12.Current;
					}
					else
					{
						combineLatest.running12 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running12 = true;
					combineLatest.completedCount = 13;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running12 = true;
					try
					{
						combineLatest.awaiter12 = combineLatest.enumerator12.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter12.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed12Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private static void Completed13(object state)
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest combineLatest = (CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest)state;
				combineLatest.running13 = false;
				try
				{
					if (combineLatest.awaiter13.GetResult())
					{
						combineLatest.hasCurrent13 = true;
						combineLatest.current13 = combineLatest.enumerator13.Current;
					}
					else
					{
						combineLatest.running13 = true;
						if (Interlocked.Increment(ref combineLatest.completedCount) == 13)
						{
							goto IL_D1;
						}
						return;
					}
				}
				catch (Exception error)
				{
					combineLatest.running13 = true;
					combineLatest.completedCount = 13;
					combineLatest.completionSource.TrySetException(error);
					return;
				}
				if (!combineLatest.TrySetResult())
				{
					if (combineLatest.syncRunning)
					{
						return;
					}
					combineLatest.running13 = true;
					try
					{
						combineLatest.awaiter13 = combineLatest.enumerator13.MoveNextAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						combineLatest.completedCount = 13;
						combineLatest.completionSource.TrySetException(error2);
						return;
					}
					combineLatest.awaiter13.SourceOnCompleted(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed13Delegate, combineLatest);
				}
				return;
				IL_D1:
				combineLatest.completionSource.TrySetResult(false);
			}

			private bool TrySetResult()
			{
				if (this.hasCurrent1 && this.hasCurrent2 && this.hasCurrent3 && this.hasCurrent4 && this.hasCurrent5 && this.hasCurrent6 && this.hasCurrent7 && this.hasCurrent8 && this.hasCurrent9 && this.hasCurrent10 && this.hasCurrent11 && this.hasCurrent12 && this.hasCurrent13)
				{
					this.result = this.resultSelector(this.current1, this.current2, this.current3, this.current4, this.current5, this.current6, this.current7, this.current8, this.current9, this.current10, this.current11, this.current12, this.current13);
					this.completionSource.TrySetResult(true);
					return true;
				}
				return false;
			}

			public UniTask DisposeAsync()
			{
				CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.<DisposeAsync>d__115 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.<DisposeAsync>d__115>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> Completed1Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed1);

			private static readonly Action<object> Completed2Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed2);

			private static readonly Action<object> Completed3Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed3);

			private static readonly Action<object> Completed4Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed4);

			private static readonly Action<object> Completed5Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed5);

			private static readonly Action<object> Completed6Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed6);

			private static readonly Action<object> Completed7Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed7);

			private static readonly Action<object> Completed8Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed8);

			private static readonly Action<object> Completed9Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed9);

			private static readonly Action<object> Completed10Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed10);

			private static readonly Action<object> Completed11Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed11);

			private static readonly Action<object> Completed12Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed12);

			private static readonly Action<object> Completed13Delegate = new Action<object>(CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>._CombineLatest.Completed13);

			private const int CompleteCount = 13;

			private readonly IUniTaskAsyncEnumerable<T1> source1;

			private readonly IUniTaskAsyncEnumerable<T2> source2;

			private readonly IUniTaskAsyncEnumerable<T3> source3;

			private readonly IUniTaskAsyncEnumerable<T4> source4;

			private readonly IUniTaskAsyncEnumerable<T5> source5;

			private readonly IUniTaskAsyncEnumerable<T6> source6;

			private readonly IUniTaskAsyncEnumerable<T7> source7;

			private readonly IUniTaskAsyncEnumerable<T8> source8;

			private readonly IUniTaskAsyncEnumerable<T9> source9;

			private readonly IUniTaskAsyncEnumerable<T10> source10;

			private readonly IUniTaskAsyncEnumerable<T11> source11;

			private readonly IUniTaskAsyncEnumerable<T12> source12;

			private readonly IUniTaskAsyncEnumerable<T13> source13;

			private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> resultSelector;

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

			private IUniTaskAsyncEnumerator<T9> enumerator9;

			private UniTask<bool>.Awaiter awaiter9;

			private bool hasCurrent9;

			private bool running9;

			private T9 current9;

			private IUniTaskAsyncEnumerator<T10> enumerator10;

			private UniTask<bool>.Awaiter awaiter10;

			private bool hasCurrent10;

			private bool running10;

			private T10 current10;

			private IUniTaskAsyncEnumerator<T11> enumerator11;

			private UniTask<bool>.Awaiter awaiter11;

			private bool hasCurrent11;

			private bool running11;

			private T11 current11;

			private IUniTaskAsyncEnumerator<T12> enumerator12;

			private UniTask<bool>.Awaiter awaiter12;

			private bool hasCurrent12;

			private bool running12;

			private T12 current12;

			private IUniTaskAsyncEnumerator<T13> enumerator13;

			private UniTask<bool>.Awaiter awaiter13;

			private bool hasCurrent13;

			private bool running13;

			private T13 current13;

			private int completedCount;

			private bool syncRunning;

			private TResult result;
		}
	}
}
