using System;
using System.Collections;

namespace System.Data
{
	/// <summary>Collects all parameters relevant to a Command object and their mappings to <see cref="T:System.Data.DataSet" /> columns, and is implemented by .NET Framework data providers that access data sources.</summary>
	public interface IDataParameterCollection : IList, ICollection, IEnumerable
	{
		/// <summary>Gets or sets the parameter at the specified index.</summary>
		/// <param name="parameterName">The name of the parameter to retrieve.</param>
		/// <returns>An <see cref="T:System.Object" /> at the specified index.</returns>
		object this[string parameterName]
		{
			get;
			set;
		}

		/// <summary>Gets a value indicating whether a parameter in the collection has the specified name.</summary>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the collection contains the parameter; otherwise, <see langword="false" />.</returns>
		bool Contains(string parameterName);

		/// <summary>Gets the location of the <see cref="T:System.Data.IDataParameter" /> within the collection.</summary>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <returns>The zero-based location of the <see cref="T:System.Data.IDataParameter" /> within the collection.</returns>
		int IndexOf(string parameterName);

		/// <summary>Removes the <see cref="T:System.Data.IDataParameter" /> from the collection.</summary>
		/// <param name="parameterName">The name of the parameter.</param>
		void RemoveAt(string parameterName);
	}
}
