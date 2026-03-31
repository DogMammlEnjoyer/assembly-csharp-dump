using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupByAwait<TSource, TKey, TElement, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public GroupByAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.elementSelector = elementSelector;
			this.resultSelector = resultSelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait(this.source, this.keySelector, this.elementSelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<TKey>> keySelector;

		private readonly Func<TSource, UniTask<TElement>> elementSelector;

		private readonly Func<TKey, IEnumerable<TElement>, UniTask<TResult>> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupByAwait : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _GroupByAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
				GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait.<CreateLookup>d__15 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait.<CreateLookup>d__15>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					if (this.groupEnumerator.MoveNext())
					{
						IGrouping<TKey, TElement> grouping = this.groupEnumerator.Current;
						this.awaiter = this.resultSelector(grouping.Key, grouping).GetAwaiter();
						if (this.awaiter.IsCompleted)
						{
							GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait.ResultSelectCore(this);
						}
						else
						{
							this.awaiter.SourceOnCompleted(GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait.ResultSelectCoreDelegate, this);
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
				GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait groupByAwait = (GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait)state;
				TResult value;
				if (groupByAwait.TryGetResult<TResult>(groupByAwait.awaiter, out value))
				{
					groupByAwait.Current = value;
					groupByAwait.completionSource.TrySetResult(true);
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

			private static readonly Action<object> ResultSelectCoreDelegate = new Action<object>(GroupByAwait<TSource, TKey, TElement, TResult>._GroupByAwait.ResultSelectCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, UniTask<TKey>> keySelector;

			private readonly Func<TSource, UniTask<TElement>> elementSelector;

			private readonly Func<TKey, IEnumerable<TElement>, UniTask<TResult>> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private IEnumerator<IGrouping<TKey, TElement>> groupEnumerator;

			private UniTask<TResult>.Awaiter awaiter;
		}
	}
}
