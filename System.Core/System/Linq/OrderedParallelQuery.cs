using System;
using System.Collections.Generic;
using System.Linq.Parallel;
using Unity;

namespace System.Linq
{
	/// <summary>Represents a sorted, parallel sequence.</summary>
	/// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
	public class OrderedParallelQuery<TSource> : ParallelQuery<TSource>
	{
		internal OrderedParallelQuery(QueryOperator<TSource> sortOp) : base(sortOp.SpecifiedQuerySettings)
		{
			this._sortOp = sortOp;
		}

		internal QueryOperator<TSource> SortOperator
		{
			get
			{
				return this._sortOp;
			}
		}

		internal IOrderedEnumerable<TSource> OrderedEnumerable
		{
			get
			{
				return (IOrderedEnumerable<!0>)this._sortOp;
			}
		}

		/// <summary>Returns an enumerator that iterates through the sequence.</summary>
		/// <returns>An enumerator that iterates through the sequence.</returns>
		public override IEnumerator<TSource> GetEnumerator()
		{
			return this._sortOp.GetEnumerator();
		}

		internal OrderedParallelQuery()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private QueryOperator<TSource> _sortOp;
	}
}
