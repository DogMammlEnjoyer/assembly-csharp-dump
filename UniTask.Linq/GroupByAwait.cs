using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class GroupByAwait<TSource, TKey, TElement> : IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>>
	{
		public GroupByAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.elementSelector = elementSelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<IGrouping<TKey, TElement>> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new GroupByAwait<TSource, TKey, TElement>._GroupByAwait(this.source, this.keySelector, this.elementSelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<TKey>> keySelector;

		private readonly Func<TSource, UniTask<TElement>> elementSelector;

		private readonly IEqualityComparer<TKey> comparer;

		private sealed class _GroupByAwait : MoveNextSource, IUniTaskAsyncEnumerator<IGrouping<TKey, TElement>>, IUniTaskAsyncDisposable
		{
			public _GroupByAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			{
				this.source = source;
				this.keySelector = keySelector;
				this.elementSelector = elementSelector;
				this.comparer = comparer;
				this.cancellationToken = cancellationToken;
			}

			public IGrouping<TKey, TElement> Current { get; private set; }

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
				GroupByAwait<TSource, TKey, TElement>._GroupByAwait.<CreateLookup>d__12 <CreateLookup>d__;
				<CreateLookup>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateLookup>d__.<>4__this = this;
				<CreateLookup>d__.<>1__state = -1;
				<CreateLookup>d__.<>t__builder.Start<GroupByAwait<TSource, TKey, TElement>._GroupByAwait.<CreateLookup>d__12>(ref <CreateLookup>d__);
				return <CreateLookup>d__.<>t__builder.Task;
			}

			private void SourceMoveNext()
			{
				try
				{
					if (this.groupEnumerator.MoveNext())
					{
						this.Current = this.groupEnumerator.Current;
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

			private readonly Func<TSource, UniTask<TKey>> keySelector;

			private readonly Func<TSource, UniTask<TElement>> elementSelector;

			private readonly IEqualityComparer<TKey> comparer;

			private CancellationToken cancellationToken;

			private IEnumerator<IGrouping<TKey, TElement>> groupEnumerator;
		}
	}
}
