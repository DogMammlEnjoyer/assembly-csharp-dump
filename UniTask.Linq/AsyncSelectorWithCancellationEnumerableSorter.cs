using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class AsyncSelectorWithCancellationEnumerableSorter<TElement, TKey> : AsyncEnumerableSorter<TElement>
	{
		internal AsyncSelectorWithCancellationEnumerableSorter(Func<TElement, CancellationToken, UniTask<TKey>> keySelector, IComparer<TKey> comparer, bool descending, AsyncEnumerableSorter<TElement> next, CancellationToken cancellationToken)
		{
			this.keySelector = keySelector;
			this.comparer = comparer;
			this.descending = descending;
			this.next = next;
			this.cancellationToken = cancellationToken;
		}

		internal override UniTask ComputeKeysAsync(TElement[] elements, int count)
		{
			AsyncSelectorWithCancellationEnumerableSorter<TElement, TKey>.<ComputeKeysAsync>d__7 <ComputeKeysAsync>d__;
			<ComputeKeysAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ComputeKeysAsync>d__.<>4__this = this;
			<ComputeKeysAsync>d__.elements = elements;
			<ComputeKeysAsync>d__.count = count;
			<ComputeKeysAsync>d__.<>1__state = -1;
			<ComputeKeysAsync>d__.<>t__builder.Start<AsyncSelectorWithCancellationEnumerableSorter<TElement, TKey>.<ComputeKeysAsync>d__7>(ref <ComputeKeysAsync>d__);
			return <ComputeKeysAsync>d__.<>t__builder.Task;
		}

		internal override int CompareKeys(int index1, int index2)
		{
			int num = this.comparer.Compare(this.keys[index1], this.keys[index2]);
			if (num == 0)
			{
				if (this.next == null)
				{
					return index1 - index2;
				}
				return this.next.CompareKeys(index1, index2);
			}
			else
			{
				if (!this.descending)
				{
					return num;
				}
				return -num;
			}
		}

		private readonly Func<TElement, CancellationToken, UniTask<TKey>> keySelector;

		private readonly IComparer<TKey> comparer;

		private readonly bool descending;

		private readonly AsyncEnumerableSorter<TElement> next;

		private CancellationToken cancellationToken;

		private TKey[] keys;
	}
}
