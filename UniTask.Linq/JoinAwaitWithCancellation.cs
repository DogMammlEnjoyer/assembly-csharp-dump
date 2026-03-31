using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public JoinAwaitWithCancellation(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			this.outer = outer;
			this.inner = inner;
			this.outerKeySelector = outerKeySelector;
			this.innerKeySelector = innerKeySelector;
			this.resultSelector = resultSelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation(this.outer, this.inner, this.outerKeySelector, this.innerKeySelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TOuter> outer;

		private readonly IUniTaskAsyncEnumerable<TInner> inner;

		private readonly Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector;

		private readonly Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector;

		private readonly Func<TOuter, TInner, CancellationToken, UniTask<TResult>> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _JoinAwaitWithCancellation : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _JoinAwaitWithCancellation(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			{
				this.outer = outer;
				this.inner = inner;
				this.outerKeySelector = outerKeySelector;
				this.innerKeySelector = innerKeySelector;
				this.resultSelector = resultSelector;
				this.comparer = comparer;
				this.cancellationToken = cancellationToken;
			}

			public TResult Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (this.lookup == null)
				{
					this.CreateInnerHashSet().Forget();
				}
				else
				{
					this.SourceMoveNext();
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private UniTaskVoid CreateInnerHashSet()
			{
				JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.<CreateInnerHashSet>d__24 <CreateInnerHashSet>d__;
				<CreateInnerHashSet>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateInnerHashSet>d__.<>4__this = this;
				<CreateInnerHashSet>d__.<>1__state = -1;
				<CreateInnerHashSet>d__.<>t__builder.Start<JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.<CreateInnerHashSet>d__24>(ref <CreateInnerHashSet>d__);
				return <CreateInnerHashSet>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					for (;;)
					{
						if (this.valueEnumerator != null)
						{
							if (this.valueEnumerator.MoveNext())
							{
								break;
							}
							this.valueEnumerator.Dispose();
							this.valueEnumerator = null;
						}
						this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
						if (!this.awaiter.IsCompleted)
						{
							goto IL_C6;
						}
						this.continueNext = true;
						JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.MoveNextCore(this);
						if (!this.continueNext)
						{
							goto IL_D7;
						}
						this.continueNext = false;
					}
					this.resultAwaiter = this.resultSelector(this.currentOuterValue, this.valueEnumerator.Current, this.cancellationToken).GetAwaiter();
					if (this.resultAwaiter.IsCompleted)
					{
						JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.ResultSelectCore(this);
					}
					else
					{
						this.resultAwaiter.SourceOnCompleted(JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.ResultSelectCoreDelegate, this);
					}
					return;
					IL_C6:
					this.awaiter.SourceOnCompleted(JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.MoveNextCoreDelegate, this);
					IL_D7:;
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation joinAwaitWithCancellation = (JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation)state;
				bool flag;
				if (!joinAwaitWithCancellation.TryGetResult<bool>(joinAwaitWithCancellation.awaiter, out flag))
				{
					joinAwaitWithCancellation.continueNext = false;
					return;
				}
				if (!flag)
				{
					joinAwaitWithCancellation.continueNext = false;
					joinAwaitWithCancellation.completionSource.TrySetResult(false);
					return;
				}
				joinAwaitWithCancellation.currentOuterValue = joinAwaitWithCancellation.enumerator.Current;
				joinAwaitWithCancellation.outerKeyAwaiter = joinAwaitWithCancellation.outerKeySelector(joinAwaitWithCancellation.currentOuterValue, joinAwaitWithCancellation.cancellationToken).GetAwaiter();
				if (joinAwaitWithCancellation.outerKeyAwaiter.IsCompleted)
				{
					JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.OuterSelectCore(joinAwaitWithCancellation);
					return;
				}
				joinAwaitWithCancellation.continueNext = false;
				joinAwaitWithCancellation.outerKeyAwaiter.SourceOnCompleted(JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.OuterSelectCoreDelegate, joinAwaitWithCancellation);
			}

			private static void OuterSelectCore(object state)
			{
				JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation joinAwaitWithCancellation = (JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation)state;
				TKey key;
				if (!joinAwaitWithCancellation.TryGetResult<TKey>(joinAwaitWithCancellation.outerKeyAwaiter, out key))
				{
					joinAwaitWithCancellation.continueNext = false;
					return;
				}
				joinAwaitWithCancellation.valueEnumerator = joinAwaitWithCancellation.lookup[key].GetEnumerator();
				if (joinAwaitWithCancellation.continueNext)
				{
					return;
				}
				joinAwaitWithCancellation.SourceMoveNext();
			}

			private static void ResultSelectCore(object state)
			{
				JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation joinAwaitWithCancellation = (JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation)state;
				TResult value;
				if (joinAwaitWithCancellation.TryGetResult<TResult>(joinAwaitWithCancellation.resultAwaiter, out value))
				{
					joinAwaitWithCancellation.Current = value;
					joinAwaitWithCancellation.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				if (this.valueEnumerator != null)
				{
					this.valueEnumerator.Dispose();
				}
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.MoveNextCore);

			private static readonly Action<object> OuterSelectCoreDelegate = new Action<object>(JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.OuterSelectCore);

			private static readonly Action<object> ResultSelectCoreDelegate = new Action<object>(JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._JoinAwaitWithCancellation.ResultSelectCore);

			private readonly IUniTaskAsyncEnumerable<TOuter> outer;

			private readonly IUniTaskAsyncEnumerable<TInner> inner;

			private readonly Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector;

			private readonly Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector;

			private readonly Func<TOuter, TInner, CancellationToken, UniTask<TResult>> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private ILookup<TKey, TInner> lookup;

			private IUniTaskAsyncEnumerator<TOuter> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private TOuter currentOuterValue;

			private IEnumerator<TInner> valueEnumerator;

			private UniTask<TResult>.Awaiter resultAwaiter;

			private UniTask<TKey>.Awaiter outerKeyAwaiter;

			private bool continueNext;
		}
	}
}
