using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Parallel;
using Unity;

namespace System.Linq
{
	/// <summary>Represents a parallel sequence.</summary>
	/// <typeparam name="TSource">The type of element in the source sequence.</typeparam>
	public class ParallelQuery<TSource> : ParallelQuery, IEnumerable<TSource>, IEnumerable
	{
		internal ParallelQuery(QuerySettings settings) : base(settings)
		{
		}

		internal sealed override ParallelQuery<TCastTo> Cast<TCastTo>()
		{
			return from elem in this
			select (TCastTo)((object)elem);
		}

		internal sealed override ParallelQuery<TCastTo> OfType<TCastTo>()
		{
			return from elem in this
			where elem is TCastTo
			select (TCastTo)((object)elem);
		}

		internal override IEnumerator GetEnumeratorUntyped()
		{
			return ((IEnumerable<TSource>)this).GetEnumerator();
		}

		/// <summary>Returns an enumerator that iterates through the sequence.</summary>
		/// <returns>An enumerator that iterates through the sequence.</returns>
		public virtual IEnumerator<TSource> GetEnumerator()
		{
			throw new NotSupportedException();
		}

		internal ParallelQuery()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
