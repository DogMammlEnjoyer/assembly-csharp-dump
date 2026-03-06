using System;
using System.Collections;

namespace System.Data.SqlClient
{
	/// <summary>Collection of <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> objects that inherits from <see cref="T:System.Collections.CollectionBase" />.</summary>
	public sealed class SqlBulkCopyColumnMappingCollection : CollectionBase
	{
		internal SqlBulkCopyColumnMappingCollection()
		{
		}

		internal bool ReadOnly { get; set; }

		/// <summary>Gets the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object at the specified index.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> to find.</param>
		/// <returns>A <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object.</returns>
		public SqlBulkCopyColumnMapping this[int index]
		{
			get
			{
				return (SqlBulkCopyColumnMapping)base.List[index];
			}
		}

		/// <summary>Adds the specified mapping to the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMappingCollection" />.</summary>
		/// <param name="bulkCopyColumnMapping">The <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object that describes the mapping to be added to the collection.</param>
		/// <returns>A <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object.</returns>
		public SqlBulkCopyColumnMapping Add(SqlBulkCopyColumnMapping bulkCopyColumnMapping)
		{
			this.AssertWriteAccess();
			if ((string.IsNullOrEmpty(bulkCopyColumnMapping.SourceColumn) && bulkCopyColumnMapping.SourceOrdinal == -1) || (string.IsNullOrEmpty(bulkCopyColumnMapping.DestinationColumn) && bulkCopyColumnMapping.DestinationOrdinal == -1))
			{
				throw SQL.BulkLoadNonMatchingColumnMapping();
			}
			base.InnerList.Add(bulkCopyColumnMapping);
			return bulkCopyColumnMapping;
		}

		/// <summary>Creates a new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> and adds it to the collection, using column names to specify both source and destination columns.</summary>
		/// <param name="sourceColumn">The name of the source column within the data source.</param>
		/// <param name="destinationColumn">The name of the destination column within the destination table.</param>
		/// <returns>A column mapping.</returns>
		public SqlBulkCopyColumnMapping Add(string sourceColumn, string destinationColumn)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumn, destinationColumn));
		}

		/// <summary>Creates a new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> and adds it to the collection, using an ordinal for the source column and a string for the destination column.</summary>
		/// <param name="sourceColumnIndex">The ordinal position of the source column within the data source.</param>
		/// <param name="destinationColumn">The name of the destination column within the destination table.</param>
		/// <returns>A column mapping.</returns>
		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, string destinationColumn)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumn));
		}

		/// <summary>Creates a new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> and adds it to the collection, using a column name to describe the source column and an ordinal to specify the destination column.</summary>
		/// <param name="sourceColumn">The name of the source column within the data source.</param>
		/// <param name="destinationColumnIndex">The ordinal position of the destination column within the destination table.</param>
		/// <returns>A column mapping.</returns>
		public SqlBulkCopyColumnMapping Add(string sourceColumn, int destinationColumnIndex)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumn, destinationColumnIndex));
		}

		/// <summary>Creates a new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> and adds it to the collection, using ordinals to specify both source and destination columns.</summary>
		/// <param name="sourceColumnIndex">The ordinal position of the source column within the data source.</param>
		/// <param name="destinationColumnIndex">The ordinal position of the destination column within the destination table.</param>
		/// <returns>A column mapping.</returns>
		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, int destinationColumnIndex)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumnIndex));
		}

		private void AssertWriteAccess()
		{
			if (this.ReadOnly)
			{
				throw SQL.BulkLoadMappingInaccessible();
			}
		}

		/// <summary>Clears the contents of the collection.</summary>
		public new void Clear()
		{
			this.AssertWriteAccess();
			base.Clear();
		}

		/// <summary>Gets a value indicating whether a specified <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object exists in the collection.</summary>
		/// <param name="value">A valid <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified mapping exists in the collection; otherwise <see langword="false" />.</returns>
		public bool Contains(SqlBulkCopyColumnMapping value)
		{
			return base.InnerList.Contains(value);
		}

		/// <summary>Copies the elements of the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMappingCollection" /> to an array of <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> items, starting at a particular index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> array that is the destination of the elements copied from <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMappingCollection" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		public void CopyTo(SqlBulkCopyColumnMapping[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		internal void CreateDefaultMapping(int columnCount)
		{
			for (int i = 0; i < columnCount; i++)
			{
				base.InnerList.Add(new SqlBulkCopyColumnMapping(i, i));
			}
		}

		/// <summary>Gets the index of the specified <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object.</summary>
		/// <param name="value">The <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object for which to search.</param>
		/// <returns>The zero-based index of the column mapping, or -1 if the column mapping is not found in the collection.</returns>
		public int IndexOf(SqlBulkCopyColumnMapping value)
		{
			return base.InnerList.IndexOf(value);
		}

		/// <summary>Insert a new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> at the index specified.</summary>
		/// <param name="index">Integer value of the location within the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMappingCollection" /> at which to insert the new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" />.</param>
		/// <param name="value">
		///   <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object to be inserted in the collection.</param>
		public void Insert(int index, SqlBulkCopyColumnMapping value)
		{
			this.AssertWriteAccess();
			base.InnerList.Insert(index, value);
		}

		/// <summary>Removes the specified <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> element from the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMappingCollection" />.</summary>
		/// <param name="value">
		///   <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object to be removed from the collection.</param>
		public void Remove(SqlBulkCopyColumnMapping value)
		{
			this.AssertWriteAccess();
			base.InnerList.Remove(value);
		}

		/// <summary>Removes the mapping at the specified index from the collection.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object to be removed from the collection.</param>
		public new void RemoveAt(int index)
		{
			this.AssertWriteAccess();
			base.RemoveAt(index);
		}

		internal void ValidateCollection()
		{
			foreach (object obj in base.InnerList)
			{
				SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = (SqlBulkCopyColumnMapping)obj;
				SqlBulkCopyColumnMappingCollection.MappingSchema mappingSchema = (sqlBulkCopyColumnMapping.SourceOrdinal != -1) ? ((sqlBulkCopyColumnMapping.DestinationOrdinal != -1) ? SqlBulkCopyColumnMappingCollection.MappingSchema.OrdinalsOrdinals : SqlBulkCopyColumnMappingCollection.MappingSchema.OrdinalsNames) : ((sqlBulkCopyColumnMapping.DestinationOrdinal != -1) ? SqlBulkCopyColumnMappingCollection.MappingSchema.NemesOrdinals : SqlBulkCopyColumnMappingCollection.MappingSchema.NamesNames);
				if (this._mappingSchema == SqlBulkCopyColumnMappingCollection.MappingSchema.Undefined)
				{
					this._mappingSchema = mappingSchema;
				}
				else if (this._mappingSchema != mappingSchema)
				{
					throw SQL.BulkLoadMappingsNamesOrOrdinalsOnly();
				}
			}
		}

		private SqlBulkCopyColumnMappingCollection.MappingSchema _mappingSchema;

		private enum MappingSchema
		{
			Undefined,
			NamesNames,
			NemesOrdinals,
			OrdinalsNames,
			OrdinalsOrdinals
		}
	}
}
