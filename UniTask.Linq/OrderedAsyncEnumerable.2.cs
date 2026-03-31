using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class OrderedAsyncEnumerable<TElement, TKey> : OrderedAsyncEnumerable<TElement>
	{
		public OrderedAsyncEnumerable(IUniTaskAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, OrderedAsyncEnumerable<TElement> parent) : base(source)
		{
			this.keySelector = keySelector;
			this.comparer = comparer;
			this.descending = descending;
			this.parent = parent;
		}

		internal override AsyncEnumerableSorter<TElement> GetAsyncEnumerableSorter(AsyncEnumerableSorter<TElement> next, CancellationToken cancellationToken)
		{
			AsyncEnumerableSorter<TElement> asyncEnumerableSorter = new SyncSelectorAsyncEnumerableSorter<TElement, TKey>(this.keySelector, this.comparer, this.descending, next);
			if (this.parent != null)
			{
				asyncEnumerableSorter = this.parent.GetAsyncEnumerableSorter(asyncEnumerableSorter, cancellationToken);
			}
			return asyncEnumerableSorter;
		}

		private readonly Func<TElement, TKey> keySelector;

		private readonly IComparer<TKey> comparer;

		private readonly bool descending;

		private readonly OrderedAsyncEnumerable<TElement> parent;
	}
}
