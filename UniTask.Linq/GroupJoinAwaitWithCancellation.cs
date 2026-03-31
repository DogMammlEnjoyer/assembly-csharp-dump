using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public GroupJoinAwaitWithCancellation(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
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
			return new GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation(this.outer, this.inner, this.outerKeySelector, this.innerKeySelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TOuter> outer;

		private readonly IUniTaskAsyncEnumerable<TInner> inner;

		private readonly Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector;

		private readonly Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector;

		private readonly Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupJoinAwaitWithCancellation : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _GroupJoinAwaitWithCancellation(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
				GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.<CreateLookup>d__22 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.<CreateLookup>d__22>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (this.awaiter.IsCompleted)
					{
						GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation groupJoinAwaitWithCancellation = (GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation)state;
				bool flag;
				if (groupJoinAwaitWithCancellation.TryGetResult<bool>(groupJoinAwaitWithCancellation.awaiter, out flag))
				{
					if (flag)
					{
						try
						{
							groupJoinAwaitWithCancellation.outerValue = groupJoinAwaitWithCancellation.enumerator.Current;
							groupJoinAwaitWithCancellation.outerKeyAwaiter = groupJoinAwaitWithCancellation.outerKeySelector(groupJoinAwaitWithCancellation.outerValue, groupJoinAwaitWithCancellation.cancellationToken).GetAwaiter();
							if (groupJoinAwaitWithCancellation.outerKeyAwaiter.IsCompleted)
							{
								GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.OuterKeySelectCore(groupJoinAwaitWithCancellation);
							}
							else
							{
								groupJoinAwaitWithCancellation.outerKeyAwaiter.SourceOnCompleted(GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.OuterKeySelectCoreDelegate, groupJoinAwaitWithCancellation);
							}
							return;
						}
						catch (Exception error)
						{
							groupJoinAwaitWithCancellation.completionSource.TrySetException(error);
							return;
						}
					}
					groupJoinAwaitWithCancellation.completionSource.TrySetResult(false);
				}
			}

			private static void OuterKeySelectCore(object state)
			{
				GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation groupJoinAwaitWithCancellation = (GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation)state;
				TKey key;
				if (groupJoinAwaitWithCancellation.TryGetResult<TKey>(groupJoinAwaitWithCancellation.outerKeyAwaiter, out key))
				{
					try
					{
						IEnumerable<TInner> arg = groupJoinAwaitWithCancellation.lookup[key];
						groupJoinAwaitWithCancellation.resultAwaiter = groupJoinAwaitWithCancellation.resultSelector(groupJoinAwaitWithCancellation.outerValue, arg, groupJoinAwaitWithCancellation.cancellationToken).GetAwaiter();
						if (groupJoinAwaitWithCancellation.resultAwaiter.IsCompleted)
						{
							GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.ResultSelectCore(groupJoinAwaitWithCancellation);
						}
						else
						{
							groupJoinAwaitWithCancellation.resultAwaiter.SourceOnCompleted(GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.ResultSelectCoreDelegate, groupJoinAwaitWithCancellation);
						}
					}
					catch (Exception error)
					{
						groupJoinAwaitWithCancellation.completionSource.TrySetException(error);
					}
				}
			}

			private static void ResultSelectCore(object state)
			{
				GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation groupJoinAwaitWithCancellation = (GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation)state;
				TResult value;
				if (groupJoinAwaitWithCancellation.TryGetResult<TResult>(groupJoinAwaitWithCancellation.resultAwaiter, out value))
				{
					groupJoinAwaitWithCancellation.Current = value;
					groupJoinAwaitWithCancellation.completionSource.TrySetResult(true);
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.MoveNextCore);

			private static readonly Action<object> ResultSelectCoreDelegate = new Action<object>(GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.ResultSelectCore);

			private static readonly Action<object> OuterKeySelectCoreDelegate = new Action<object>(GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>._GroupJoinAwaitWithCancellation.OuterKeySelectCore);

			private readonly IUniTaskAsyncEnumerable<TOuter> outer;

			private readonly IUniTaskAsyncEnumerable<TInner> inner;

			private readonly Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector;

			private readonly Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector;

			private readonly Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>> resultSelector;

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
