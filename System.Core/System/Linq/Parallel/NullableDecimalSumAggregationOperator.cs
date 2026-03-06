using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class NullableDecimalSumAggregationOperator : InlinedAggregationOperator<decimal?, decimal?, decimal?>
	{
		internal NullableDecimalSumAggregationOperator(IEnumerable<decimal?> child) : base(child)
		{
		}

		protected override decimal? InternalAggregate(ref Exception singularExceptionToThrow)
		{
			decimal? result;
			using (IEnumerator<decimal?> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				decimal num = 0.0m;
				while (enumerator.MoveNext())
				{
					decimal d = num;
					result = enumerator.Current;
					num = d + result.GetValueOrDefault();
				}
				result = new decimal?(num);
			}
			return result;
		}

		protected override QueryOperatorEnumerator<decimal?, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<decimal?, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new NullableDecimalSumAggregationOperator.NullableDecimalSumAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class NullableDecimalSumAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<decimal?>
		{
			internal NullableDecimalSumAggregationOperatorEnumerator(QueryOperatorEnumerator<decimal?, TKey> source, int partitionIndex, CancellationToken cancellationToken) : base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref decimal? currentElement)
			{
				decimal? num = null;
				TKey tkey = default(TKey);
				QueryOperatorEnumerator<decimal?, TKey> source = this._source;
				if (source.MoveNext(ref num, ref tkey))
				{
					decimal num2 = 0.0m;
					int num3 = 0;
					do
					{
						if ((num3++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						num2 += num.GetValueOrDefault();
					}
					while (source.MoveNext(ref num, ref tkey));
					currentElement = new decimal?(num2);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<decimal?, TKey> _source;
		}
	}
}
