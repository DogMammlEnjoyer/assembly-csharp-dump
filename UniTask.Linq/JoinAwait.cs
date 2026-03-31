using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class JoinAwait<TOuter, TInner, TKey, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public JoinAwait(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
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
			return new JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait(this.outer, this.inner, this.outerKeySelector, this.innerKeySelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TOuter> outer;

		private readonly IUniTaskAsyncEnumerable<TInner> inner;

		private readonly Func<TOuter, UniTask<TKey>> outerKeySelector;

		private readonly Func<TInner, UniTask<TKey>> innerKeySelector;

		private readonly Func<TOuter, TInner, UniTask<TResult>> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _JoinAwait : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _JoinAwait(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
				JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.<CreateInnerHashSet>d__24 <CreateInnerHashSet>d__;
				<CreateInnerHashSet>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateInnerHashSet>d__.<>4__this = this;
				<CreateInnerHashSet>d__.<>1__state = -1;
				<CreateInnerHashSet>d__.<>t__builder.Start<JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.<CreateInnerHashSet>d__24>(ref <CreateInnerHashSet>d__);
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
							goto IL_C0;
						}
						this.continueNext = true;
						JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.MoveNextCore(this);
						if (!this.continueNext)
						{
							goto IL_D1;
						}
						this.continueNext = false;
					}
					this.resultAwaiter = this.resultSelector(this.currentOuterValue, this.valueEnumerator.Current).GetAwaiter();
					if (this.resultAwaiter.IsCompleted)
					{
						JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.ResultSelectCore(this);
					}
					else
					{
						this.resultAwaiter.SourceOnCompleted(JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.ResultSelectCoreDelegate, this);
					}
					return;
					IL_C0:
					this.awaiter.SourceOnCompleted(JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.MoveNextCoreDelegate, this);
					IL_D1:;
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait joinAwait = (JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait)state;
				bool flag;
				if (!joinAwait.TryGetResult<bool>(joinAwait.awaiter, out flag))
				{
					joinAwait.continueNext = false;
					return;
				}
				if (!flag)
				{
					joinAwait.continueNext = false;
					joinAwait.completionSource.TrySetResult(false);
					return;
				}
				joinAwait.currentOuterValue = joinAwait.enumerator.Current;
				joinAwait.outerKeyAwaiter = joinAwait.outerKeySelector(joinAwait.currentOuterValue).GetAwaiter();
				if (joinAwait.outerKeyAwaiter.IsCompleted)
				{
					JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.OuterSelectCore(joinAwait);
					return;
				}
				joinAwait.continueNext = false;
				joinAwait.outerKeyAwaiter.SourceOnCompleted(JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.OuterSelectCoreDelegate, joinAwait);
			}

			private static void OuterSelectCore(object state)
			{
				JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait joinAwait = (JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait)state;
				TKey key;
				if (!joinAwait.TryGetResult<TKey>(joinAwait.outerKeyAwaiter, out key))
				{
					joinAwait.continueNext = false;
					return;
				}
				joinAwait.valueEnumerator = joinAwait.lookup[key].GetEnumerator();
				if (joinAwait.continueNext)
				{
					return;
				}
				joinAwait.SourceMoveNext();
			}

			private static void ResultSelectCore(object state)
			{
				JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait joinAwait = (JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait)state;
				TResult value;
				if (joinAwait.TryGetResult<TResult>(joinAwait.resultAwaiter, out value))
				{
					joinAwait.Current = value;
					joinAwait.completionSource.TrySetResult(true);
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.MoveNextCore);

			private static readonly Action<object> OuterSelectCoreDelegate = new Action<object>(JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.OuterSelectCore);

			private static readonly Action<object> ResultSelectCoreDelegate = new Action<object>(JoinAwait<TOuter, TInner, TKey, TResult>._JoinAwait.ResultSelectCore);

			private readonly IUniTaskAsyncEnumerable<TOuter> outer;

			private readonly IUniTaskAsyncEnumerable<TInner> inner;

			private readonly Func<TOuter, UniTask<TKey>> outerKeySelector;

			private readonly Func<TInner, UniTask<TKey>> innerKeySelector;

			private readonly Func<TOuter, TInner, UniTask<TResult>> resultSelector;

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
