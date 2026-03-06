using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class DecimalAverageAggregationOperator : InlinedAggregationOperator<decimal, Pair<decimal, long>, decimal>
	{
		internal DecimalAverageAggregationOperator(IEnumerable<decimal> child) : base(child)
		{
		}

		protected override decimal InternalAggregate(ref Exception singularExceptionToThrow)
		{
			decimal result;
			using (IEnumerator<Pair<decimal, long>> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
					result = 0m;
				}
				else
				{
					Pair<decimal, long> pair = enumerator.Current;
					while (enumerator.MoveNext())
					{
						decimal first = pair.First;
						Pair<decimal, long> pair2 = enumerator.Current;
						pair.First = first + pair2.First;
						long second = pair.Second;
						pair2 = enumerator.Current;
						pair.Second = checked(second + pair2.Second);
					}
					result = pair.First / pair.Second;
				}
			}
			return result;
		}

		protected override QueryOperatorEnumerator<Pair<decimal, long>, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<decimal, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new DecimalAverageAggregationOperator.DecimalAverageAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class DecimalAverageAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<Pair<decimal, long>>
		{
			internal DecimalAverageAggregationOperatorEnumerator(QueryOperatorEnumerator<decimal, TKey> source, int partitionIndex, CancellationToken cancellationToken) : base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref Pair<decimal, long> currentElement)
			{
				decimal num = 0.0m;
				long num2 = 0L;
				QueryOperatorEnumerator<decimal, TKey> source = this._source;
				decimal d = 0m;
				TKey tkey = default(TKey);
				if (source.MoveNext(ref d, ref tkey))
				{
					int num3 = 0;
					do
					{
						if ((num3++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						num += d;
						checked
						{
							num2 += 1L;
						}
					}
					while (source.MoveNext(ref d, ref tkey));
					currentElement = new Pair<decimal, long>(num, num2);
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
