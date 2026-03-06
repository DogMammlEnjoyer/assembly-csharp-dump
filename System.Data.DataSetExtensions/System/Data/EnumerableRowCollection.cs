using System;
using System.Collections;

namespace System.Data
{
	/// <summary>Represents a collection of <see cref="T:System.Data.DataRow" /> objects returned from a LINQ to DataSet query. This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
	public abstract class EnumerableRowCollection : IEnumerable
	{
		internal abstract Type ElementType { get; }

		internal abstract DataTable Table { get; }

		internal EnumerableRowCollection()
		{
		}

		/// <summary>Returns an enumerator for the collection of <see cref="T:System.Data.DataRow" /> objects. This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to traverse the collection of <see cref="T:System.Data.DataRow" /> objects.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return null;
		}
	}
}
