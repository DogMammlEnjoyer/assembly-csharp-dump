using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal abstract class OrderedAsyncEnumerable<TElement> : IUniTaskOrderedAsyncEnumerable<TElement>, IUniTaskAsyncEnumerable<TElement>
	{
		public OrderedAsyncEnumerable(IUniTaskAsyncEnumerable<TElement> source)
		{
			this.source = source;
		}

		public IUniTaskOrderedAsyncEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedAsyncEnumerable<TElement, TKey>(this.source, keySelector, comparer, descending, this);
		}

		public IUniTaskOrderedAsyncEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, UniTask<TKey>> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedAsyncEnumerableAwait<TElement, TKey>(this.source, keySelector, comparer, descending, this);
		}

		public IUniTaskOrderedAsyncEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, CancellationToken, UniTask<TKey>> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedAsyncEnumerableAwaitWithCancellation<TElement, TKey>(this.source, keySelector, comparer, descending, this);
		}

		internal abstract AsyncEnumerableSorter<TElement> GetAsyncEnumerableSorter(AsyncEnumerableSorter<TElement> next, CancellationToken cancellationToken);

		public IUniTaskAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new OrderedAsyncEnumerable<TElement>._OrderedAsyncEnumerator(this, cancellationToken);
		}

		protected readonly IUniTaskAsyncEnumerable<TElement> source;

		private class _OrderedAsyncEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<TElement>, IUniTaskAsyncDisposable
		{
			public _OrderedAsyncEnumerator(OrderedAsyncEnumerable<TElement> parent, CancellationToken cancellationToken)
			{
				this.parent = parent;
				this.cancellationToken = cancellationToken;
			}

			public TElement Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.map == null)
				{
					this.completionSource.Reset();
					this.CreateSortSource().Forget();
					return new UniTask<bool>(this, this.completionSource.Version);
				}
				if (this.index < this.buffer.Length)
				{
					TElement[] array = this.buffer;
					int[] array2 = this.map;
					int num = this.index;
					this.index = num + 1;
					this.Current = array[array2[num]];
					return CompletedTasks.True;
				}
				return CompletedTasks.False;
			}

			private UniTaskVoid CreateSortSource()
			{
				OrderedAsyncEnumerable<TElement>._OrderedAsyncEnumerator.<CreateSortSource>d__11 <CreateSortSource>d__;
				<CreateSortSource>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<CreateSortSource>d__.<>4__this = this;
				<CreateSortSource>d__.<>1__state = -1;
				<CreateSortSource>d__.<>t__builder.Start<OrderedAsyncEnumerable<TElement>._OrderedAsyncEnumerator.<CreateSortSource>d__11>(ref <CreateSortSource>d__);
				return <CreateSortSource>d__.<>t__builder.Task;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			protected readonly OrderedAsyncEnumerable<TElement> parent;

			private CancellationToken cancellationToken;

			private TElement[] buffer;

			private int[] map;

			private int index;
		}
	}
}
