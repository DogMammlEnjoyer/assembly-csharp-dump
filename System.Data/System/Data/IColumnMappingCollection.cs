using System;
using System.Collections;

namespace System.Data
{
	/// <summary>Contains a collection of DataColumnMapping objects, and is implemented by the <see cref="T:System.Data.Common.DataColumnMappingCollection" />, which is used in common by .NET Framework data providers.</summary>
	public interface IColumnMappingCollection : IList, ICollection, IEnumerable
	{
		/// <summary>Gets or sets the <see cref="T:System.Data.IColumnMapping" /> object with the specified <see langword="SourceColumn" /> name.</summary>
		/// <param name="index">The <see langword="SourceColumn" /> name of the <see langword="IColumnMapping" /> object to find.</param>
		/// <returns>The <see langword="IColumnMapping" /> object with the specified <see langword="SourceColumn" /> name.</returns>
		object this[string index]
		{
			get;
			set;
		}

		/// <summary>Adds a ColumnMapping object to the ColumnMapping collection using the source column and <see cref="T:System.Data.DataSet" /> column names.</summary>
		/// <param name="sourceColumnName">The case-sensitive name of the source column.</param>
		/// <param name="dataSetColumnName">The name of the <see cref="T:System.Data.DataSet" /> column.</param>
		/// <returns>The ColumnMapping object that was added to the collection.</returns>
		IColumnMapping Add(string sourceColumnName, string dataSetColumnName);

		/// <summary>Gets a value indicating whether the <see cref="T:System.Data.Common.DataColumnMappingCollection" /> contains a <see cref="T:System.Data.Common.DataColumnMapping" /> object with the specified source column name.</summary>
		/// <param name="sourceColumnName">The case-sensitive name of the source column.</param>
		/// <returns>
		///   <see langword="true" /> if a <see cref="T:System.Data.Common.DataColumnMapping" /> object with the specified source column name exists, otherwise <see langword="false" />.</returns>
		bool Contains(string sourceColumnName);

		/// <summary>Gets the ColumnMapping object with the specified <see cref="T:System.Data.DataSet" /> column name.</summary>
		/// <param name="dataSetColumnName">The name of the <see cref="T:System.Data.DataSet" /> column within the collection.</param>
		/// <returns>The ColumnMapping object with the specified <see langword="DataSet" /> column name.</returns>
		IColumnMapping GetByDataSetColumn(string dataSetColumnName);

		/// <summary>Gets the location of the <see cref="T:System.Data.Common.DataColumnMapping" /> object with the specified source column name. The name is case-sensitive.</summary>
		/// <param name="sourceColumnName">The case-sensitive name of the source column.</param>
		/// <returns>The zero-based location of the <see langword="DataColumnMapping" /> object with the specified source column name.</returns>
		int IndexOf(string sourceColumnName);

		/// <summary>Removes the <see cref="T:System.Data.IColumnMapping" /> object with the specified <see cref="P:System.Data.IColumnMapping.SourceColumn" /> name from the collection.</summary>
		/// <param name="sourceColumnName">The case-sensitive <see langword="SourceColumn" /> name.</param>
		/// <exception cref="T:System.IndexOutOfRangeException">A <see cref="T:System.Data.Common.DataColumnMapping" /> object does not exist with the specified <see langword="SourceColumn" /> name.</exception>
		void RemoveAt(string sourceColumnName);
	}
}
