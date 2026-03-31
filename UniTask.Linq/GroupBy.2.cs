using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupBy<TSource, TKey, TElement, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public GroupBy(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.elementSelector = elementSelector;
			this.resultSelector = resultSelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new GroupBy<TSource, TKey, TElement, TResult>._GroupBy(this.source, this.keySelector, this.elementSelector, this.resultSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, TKey> keySelector;

		private readonly Func<TSource, TElement> elementSelector;

		private readonly Func<TKey, IEnumerable<TElement>, TResult> resultSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupBy : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _GroupBy(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
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
				GroupBy<TSource, TKey, TElement, TResult>._GroupBy.<CreateLookup>d__13 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupBy<TSource, TKey, TElement, TResult>._GroupBy.<CreateLookup>d__13>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					if (this.groupEnumerator.MoveNext())
					{
						IGrouping<TKey, TElement> grouping = this.groupEnumerator.Current;
						this.Current = this.resultSelector(grouping.Key, grouping);
						this.completionSource.TrySetResult(true);
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

			public UniTask DisposeAsync()
			{
				if (this.groupEnumerator != null)
				{
					this.groupEnumerator.Dispose();
				}
				return default(UniTask);
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, TKey> keySelector;

			private readonly Func<TSource, TElement> elementSelector;

			private readonly Func<TKey, IEnumerable<TElement>, TResult> resultSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private IEnumerator<IGrouping<TKey, TElement>> groupEnumerator;
		}
	}
}
