using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Parallel;
using Unity;

namespace System.Linq
{
	/// <summary>Represents a parallel sequence.</summary>
	public class ParallelQuery : IEnumerable
	{
		internal ParallelQuery(QuerySettings specifiedSettings)
		{
			this._specifiedSettings = specifiedSettings;
		}

		internal QuerySettings SpecifiedQuerySettings
		{
			get
			{
				return this._specifiedSettings;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ParallelQuery<TCastTo> Cast<TCastTo>()
		{
			throw new NotSupportedException();
		}

		[ExcludeFromCodeCoverage]
		internal virtual ParallelQuery<TCastTo> OfType<TCastTo>()
		{
			throw new NotSupportedException();
		}

		[ExcludeFromCodeCoverage]
		internal virtual IEnumerator GetEnumeratorUntyped()
		{
			throw new NotSupportedException();
		}

		/// <summary>Returns an enumerator that iterates through the sequence.</summary>
		/// <returns>An enumerator that iterates through the sequence.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumeratorUntyped();
		}

		internal ParallelQuery()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private QuerySettings _specifiedSettings;
	}
}
