using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class DistinctAwait<TSource, TKey> : IUniTaskAsyncEnumerable<TSource>
	{
		public DistinctAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new DistinctAwait<TSource, TKey>._DistinctAwait(this.source, this.keySelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<TKey>> keySelector;

		private readonly IEqualityComparer<TKey> comparer;

		private class _DistinctAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, TKey>
		{
			public _DistinctAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.set = new HashSet<TKey>(comparer);
				this.keySelector = keySelector;
			}

			protected override UniTask<TKey> TransformAsync(TSource sourceCurrent)
			{
				return this.keySelector(sourceCurrent);
			}

			protected override bool TrySetCurrentCore(TKey awaitResult, out bool terminateIteration)
			{
				if (this.set.Add(awaitResult))
				{
					base.Current = base.SourceCurrent;
					terminateIteration = false;
					return true;
				}
				terminateIteration = false;
				return false;
			}

			private readonly HashSet<TKey> set;

			private readonly Func<TSource, UniTask<TKey>> keySelector;
		}
	}
}
