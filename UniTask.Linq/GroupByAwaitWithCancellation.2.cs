using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public GroupByAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.elementSelector = elementSelector;
			this.resultSelector = resultSelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation(this.source, this.keySelector, this.elementSelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, CancellationToken, UniTask<TKey>> keySelector;

		private readonly Func<TSource, CancellationToken, UniTask<TElement>> elementSelector;

		private readonly Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupByAwaitWithCancellation : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _GroupByAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			{
				this.source = source;
				this.keySelector = keySelector;
				this.elementSelector = elementSelector;
				this.resultSelector = resultSelector;
				this.comparer = comparer;
				this.cancellationToken = cancellationToken;
			}

			public TResult Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (this.groupEnumerator == null)
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
				GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation.<CreateLookup>d__15 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation.<CreateLookup>d__15>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					if (this.groupEnumerator.MoveNext())
					{
						IGrouping<TKey, TElement> grouping = this.groupEnumerator.Current;
						this.awaiter = this.resultSelector(grouping.Key, grouping, this.cancellationToken).GetAwaiter();
						if (this.awaiter.IsCompleted)
						{
							GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation.ResultSelectCore(this);
						}
						else
						{
							this.awaiter.SourceOnCompleted(GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation.ResultSelectCoreDelegate, this);
						}
					}
					else
					{
						this.completionSource.TrySetResult(false);
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
				}
			}

			private static void ResultSelectCore(object state)
			{
				GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation groupByAwaitWithCancellation = (GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation)state;
				TResult value;
				if (groupByAwaitWithCancellation.TryGetResult<TResult>(groupByAwaitWithCancellation.awaiter, out value))
				{
					groupByAwaitWithCancellation.Current = value;
					groupByAwaitWithCancellation.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				if (this.groupEnumerator != null)
				{
					this.groupEnumerator.Dispose();
				}
				return default(UniTask);
			}

			private static readonly Action<object> ResultSelectCoreDelegate = new Action<object>(GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>._GroupByAwaitWithCancellation.ResultSelectCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, CancellationToken, UniTask<TKey>> keySelector;

			private readonly Func<TSource, CancellationToken, UniTask<TElement>> elementSelector;

			private readonly Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private IEnumerator<IGrouping<TKey, TElement>> groupEnumerator;

			private UniTask<TResult>.Awaiter awaiter;
		}
	}
}
