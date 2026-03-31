using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupJoin<TOuter, TInner, TKey, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public GroupJoin(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
			return new GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin(this.outer, this.inner, this.outerKeySelector, this.innerKeySelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TOuter> outer;

		private readonly IUniTaskAsyncEnumerable<TInner> inner;

		private readonly Func<TOuter, TKey> outerKeySelector;

		private readonly Func<TInner, TKey> innerKeySelector;

		private readonly Func<TOuter, IEnumerable<TInner>, TResult> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupJoin : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _GroupJoin(IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
				GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin.<CreateLookup>d__17 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin.<CreateLookup>d__17>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					if (this.awaiter.IsCompleted)
					{
						GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin.MoveNextCore(this);
					}
					else
					{
						this.awaiter.SourceOnCompleted(GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin.MoveNextCoreDelegate, this);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void MoveNextCore(object state)
			{
				GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin groupJoin = (GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin)state;
				bool flag;
				if (groupJoin.TryGetResult<bool>(groupJoin.awaiter, out flag))
				{
					if (flag)
					{
						TOuter touter = groupJoin.enumerator.Current;
						TKey key = groupJoin.outerKeySelector(touter);
						IEnumerable<TInner> arg = groupJoin.lookup[key];
						groupJoin.Current = groupJoin.resultSelector(touter, arg);
						groupJoin.completionSource.TrySetResult(true);
						return;
					}
					groupJoin.completionSource.TrySetResult(false);
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(GroupJoin<TOuter, TInner, TKey, TResult>._GroupJoin.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TOuter> outer;

			private readonly IUniTaskAsyncEnumerable<TInner> inner;

			private readonly Func<TOuter, TKey> outerKeySelector;

			private readonly Func<TInner, TKey> innerKeySelector;

			private readonly Func<TOuter, IEnumerable<TInner>, TResult> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private ILookup<TKey, TInner> lookup;

			private IUniTaskAsyncEnumerator<TOuter> enumerator;

			private UniTask<bool>.Awaiter awaiter;
		}
	}
}
