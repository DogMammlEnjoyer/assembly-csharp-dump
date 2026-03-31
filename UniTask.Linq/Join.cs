using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Join<TOuter, TInner, TKey, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public Join(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
			return new Join<TOuter, TInner, TKey, TResult>._Join(this.outer, this.inner, this.outerKeySelector, this.innerKeySelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TOuter> outer;

		private readonly IUniTaskAsyncEnumerable<TInner> inner;

		private readonly Func<TOuter, TKey> outerKeySelector;

		private readonly Func<TInner, TKey> innerKeySelector;

		private readonly Func<TOuter, TInner, TResult> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _Join : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _Join(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
				Join<TOuter, TInner, TKey, TResult>._Join.<CreateInnerHashSet>d__20 <CreateInnerHashSet>d__;
				<CreateInnerHashSet>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateInnerHashSet>d__.<>4__this = this;
				<CreateInnerHashSet>d__.<>1__state = -1;
				<CreateInnerHashSet>d__.<>t__builder.Start<Join<TOuter, TInner, TKey, TResult>._Join.<CreateInnerHashSet>d__20>(ref <CreateInnerHashSet>d__);
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
							goto IL_92;
						}
						this.continueNext = true;
						Join<TOuter, TInner, TKey, TResult>._Join.MoveNextCore(this);
						if (!this.continueNext)
						{
							goto IL_A3;
						}
						this.continueNext = false;
					}
					this.Current = this.resultSelector(this.currentOuterValue, this.valueEnumerator.Current);
					goto IL_B6;
					IL_92:
					this.awaiter.SourceOnCompleted(Join<TOuter, TInner, TKey, TResult>._Join.MoveNextCoreDelegate, this);
					IL_A3:;
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
				return;
				IL_B6:
				this.completionSource.TrySetResult(true);
			}

			private static void MoveNextCore(object state)
			{
				Join<TOuter, TInner, TKey, TResult>._Join join = (Join<TOuter, TInner, TKey, TResult>._Join)state;
				bool flag;
				if (!join.TryGetResult<bool>(join.awaiter, out flag))
				{
					join.continueNext = false;
					return;
				}
				if (!flag)
				{
					join.continueNext = false;
					join.completionSource.TrySetResult(false);
					return;
				}
				join.currentOuterValue = join.enumerator.Current;
				TKey key = join.outerKeySelector(join.currentOuterValue);
				join.valueEnumerator = join.lookup[key].GetEnumerator();
				if (join.continueNext)
				{
					return;
				}
				join.SourceMoveNext();
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(Join<TOuter, TInner, TKey, TResult>._Join.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TOuter> outer;

			private readonly IUniTaskAsyncEnumerable<TInner> inner;

			private readonly Func<TOuter, TKey> outerKeySelector;

			private readonly Func<TInner, TKey> innerKeySelector;

			private readonly Func<TOuter, TInner, TResult> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private ILookup<TKey, TInner> lookup;

			private IUniTaskAsyncEnumerator<TOuter> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private TOuter currentOuterValue;

			private IEnumerator<TInner> valueEnumerator;

			private bool continueNext;
		}
	}
}
