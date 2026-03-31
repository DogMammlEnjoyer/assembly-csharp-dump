using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Distinct<TSource, TKey> : IUniTaskAsyncEnumerable<TSource>
	{
		public Distinct(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			this.source = source;
			this.keySelector = keySelector;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Distinct<TSource, TKey>._Distinct(this.source, this.keySelector, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, TKey> keySelector;

		private readonly IEqualityComparer<TKey> comparer;

		private class _Distinct : AsyncEnumeratorBase<TSource, TSource>
		{
			public _Distinct(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.set = new HashSet<TKey>(comparer);
				this.keySelector = keySelector;
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (!sourceHasCurrent)
				{
					result = false;
					return true;
				}
				TSource sourceCurrent = base.SourceCurrent;
				if (this.set.Add(this.keySelector(sourceCurrent)))
				{
					base.Current = sourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return false;
			}

			private readonly HashSet<TKey> set;

			private readonly Func<TSource, TKey> keySelector;
		}
	}
}
