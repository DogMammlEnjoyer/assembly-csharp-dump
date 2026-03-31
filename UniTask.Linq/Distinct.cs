using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Distinct<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Distinct(IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			this.source = source;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Distinct<TSource>._Distinct(this.source, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly IEqualityComparer<TSource> comparer;

		private class _Distinct : AsyncEnumeratorBase<TSource, TSource>
		{
			public _Distinct(IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.set = new HashSet<TSource>(comparer);
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (!sourceHasCurrent)
				{
					result = false;
					return true;
				}
				TSource sourceCurrent = base.SourceCurrent;
				if (this.set.Add(sourceCurrent))
				{
					base.Current = sourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return false;
			}

			private readonly HashSet<TSource> set;
		}
	}
}
