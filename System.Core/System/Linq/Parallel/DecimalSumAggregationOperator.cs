using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class DecimalSumAggregationOperator : InlinedAggregationOperator<decimal, decimal, decimal>
	{
		internal DecimalSumAggregationOperator(IEnumerable<decimal> child) : base(child)
		{
		}

		protected override decimal InternalAggregate(ref Exception singularExceptionToThrow)
		{
			decimal result;
			using (IEnumerator<decimal> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				decimal num = 0.0m;
				while (enumerator.MoveNext())
				{
					decimal d = enumerator.Current;
					num += d;
				}
				result = num;
			}
			return result;
		}

		protected override QueryOperatorEnumerator<decimal, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<decimal, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new DecimalSumAggregationOperator.DecimalSumAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class DecimalSumAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<decimal>
		{
			internal DecimalSumAggregationOperatorEnumerator(QueryOperatorEnumerator<decimal, TKey> source, int partitionIndex, CancellationToken cancellationToken) : base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref decimal currentElement)
			{
				decimal d = 0m;
				TKey tkey = default(TKey);
				QueryOperatorEnumerator<decimal, TKey> source = this._source;
				if (source.MoveNext(ref d, ref tkey))
				{
					decimal num = 0.0m;
					int num2 = 0;
					do
					{
						if ((num2++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						num += d;
					}
					while (source.MoveNext(ref d, ref tkey));
					currentElement = num;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<decimal, TKey> _source;
		}
	}
}
