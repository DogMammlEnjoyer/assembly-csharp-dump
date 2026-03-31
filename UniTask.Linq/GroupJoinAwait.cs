using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupJoinAwait<TOuter, TInner, TKey, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public GroupJoinAwait(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
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
			return new GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait(this.outer, this.inner, this.outerKeySelector, this.innerKeySelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TOuter> outer;

		private readonly IUniTaskAsyncEnumerable<TInner> inner;

		private readonly Func<TOuter, UniTask<TKey>> outerKeySelector;

		private readonly Func<TInner, UniTask<TKey>> innerKeySelector;

		private readonly Func<TOuter, IEnumerable<TInner>, UniTask<TResult>> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupJoinAwait : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _GroupJoinAwait(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
					this.CreateLookup().Forget();
				}
				else
				{
					this.SourceMoveNext();
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private UniTaskVoid CreateLookup()
			{
				GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.<CreateLookup>d__22 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.<CreateLookup>d__22>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (this.awaiter.IsCompleted)
					{
						GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait groupJoinAwait = (GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait)state;
				bool flag;
				if (groupJoinAwait.TryGetResult<bool>(groupJoinAwait.awaiter, out flag))
				{
					if (flag)
					{
						try
						{
							groupJoinAwait.outerValue = groupJoinAwait.enumerator.Current;
							groupJoinAwait.outerKeyAwaiter = groupJoinAwait.outerKeySelector(groupJoinAwait.outerValue).GetAwaiter();
							if (groupJoinAwait.outerKeyAwaiter.IsCompleted)
							{
								GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.OuterKeySelectCore(groupJoinAwait);
							}
							else
							{
								groupJoinAwait.outerKeyAwaiter.SourceOnCompleted(GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.OuterKeySelectCoreDelegate, groupJoinAwait);
							}
							return;
						}
						catch (Exception error)
						{
							groupJoinAwait.completionSource.TrySetException(error);
							return;
						}
					}
					groupJoinAwait.completionSource.TrySetResult(false);
				}
			}

			private static void OuterKeySelectCore(object state)
			{
				GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait groupJoinAwait = (GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait)state;
				TKey key;
				if (groupJoinAwait.TryGetResult<TKey>(groupJoinAwait.outerKeyAwaiter, out key))
				{
					try
					{
						IEnumerable<TInner> arg = groupJoinAwait.lookup[key];
						groupJoinAwait.resultAwaiter = groupJoinAwait.resultSelector(groupJoinAwait.outerValue, arg).GetAwaiter();
						if (groupJoinAwait.resultAwaiter.IsCompleted)
						{
							GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.ResultSelectCore(groupJoinAwait);
						}
						else
						{
							groupJoinAwait.resultAwaiter.SourceOnCompleted(GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.ResultSelectCoreDelegate, groupJoinAwait);
						}
					}
					catch (Exception error)
					{
						groupJoinAwait.completionSource.TrySetException(error);
					}
				}
			}

			private static void ResultSelectCore(object state)
			{
				GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait groupJoinAwait = (GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait)state;
				TResult value;
				if (groupJoinAwait.TryGetResult<TResult>(groupJoinAwait.resultAwaiter, out value))
				{
					groupJoinAwait.Current = value;
					groupJoinAwait.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.MoveNextCore);

			private static readonly Action<object> ResultSelectCoreDelegate = new Action<object>(GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.ResultSelectCore);

			private static readonly Action<object> OuterKeySelectCoreDelegate = new Action<object>(GroupJoinAwait<TOuter, TInner, TKey, TResult>._GroupJoinAwait.OuterKeySelectCore);

			private readonly IUniTaskAsyncEnumerable<TOuter> outer;

			private readonly IUniTaskAsyncEnumerable<TInner> inner;

			private readonly Func<TOuter, UniTask<TKey>> outerKeySelector;

			private readonly Func<TInner, UniTask<TKey>> innerKeySelector;

			private readonly Func<TOuter, IEnumerable<TInner>, UniTask<TResult>> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private ILookup<TKey, TInner> lookup;

			private IUniTaskAsyncEnumerator<TOuter> enumerator;

			private TOuter outerValue;

			private UniTask<bool>.Awaiter awaiter;

			private UniTask<TKey>.Awaiter outerKeyAwaiter;

			private UniTask<TResult>.Awaiter resultAwaiter;
		}
	}
}
