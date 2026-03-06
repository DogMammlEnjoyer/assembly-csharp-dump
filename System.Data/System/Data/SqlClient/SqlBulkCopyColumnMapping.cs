using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	/// <summary>Defines the mapping between a column in a <see cref="T:System.Data.SqlClient.SqlBulkCopy" /> instance's data source and a column in the instance's destination table.</summary>
	public sealed class SqlBulkCopyColumnMapping
	{
		/// <summary>Name of the column being mapped in the destination database table.</summary>
		/// <returns>The string value of the <see cref="P:System.Data.SqlClient.SqlBulkCopyColumnMapping.DestinationColumn" /> property.</returns>
		public string DestinationColumn
		{
			get
			{
				if (this._destinationColumnName != null)
				{
					return this._destinationColumnName;
				}
				return string.Empty;
			}
			set
			{
				this._destinationColumnOrdinal = (this._internalDestinationColumnOrdinal = -1);
				this._destinationColumnName = value;
			}
		}

		/// <summary>Ordinal value of the destination column within the destination table.</summary>
		/// <returns>The integer value of the <see cref="P:System.Data.SqlClient.SqlBulkCopyColumnMapping.DestinationOrdinal" /> property, or -1 if the property has not been set.</returns>
		public int DestinationOrdinal
		{
			get
			{
				return this._destinationColumnOrdinal;
			}
			set
			{
				if (value >= 0)
				{
					this._destinationColumnName = null;
					this._internalDestinationColumnOrdinal = value;
					this._destinationColumnOrdinal = value;
					return;
				}
				throw ADP.IndexOutOfRange(value);
			}
		}

		/// <summary>Name of the column being mapped in the data source.</summary>
		/// <returns>The string value of the <see cref="P:System.Data.SqlClient.SqlBulkCopyColumnMapping.SourceColumn" /> property.</returns>
		public string SourceColumn
		{
			get
			{
				if (this._sourceColumnName != null)
				{
					return this._sourceColumnName;
				}
				return string.Empty;
			}
			set
			{
				this._sourceColumnOrdinal = (this._internalSourceColumnOrdinal = -1);
				this._sourceColumnName = value;
			}
		}

		/// <summary>The ordinal position of the source column within the data source.</summary>
		/// <returns>The integer value of the <see cref="P:System.Data.SqlClient.SqlBulkCopyColumnMapping.SourceOrdinal" /> property.</returns>
		public int SourceOrdinal
		{
			get
			{
				return this._sourceColumnOrdinal;
			}
			set
			{
				if (value >= 0)
				{
					this._sourceColumnName = null;
					this._internalSourceColumnOrdinal = value;
					this._sourceColumnOrdinal = value;
					return;
				}
				throw ADP.IndexOutOfRange(value);
			}
		}

		/// <summary>Default constructor that initializes a new <see cref="T:System.Data.SqlClient.SqlBulkCopyColumnMapping" /> object.</summary>
		public SqlBulkCopyColumnMapping()
		{
			this._internalSourceColumnOrdinal = -1;
		}

		/// <summary>Creates a new column mapping, using column names to refer to source and destination columns.</summary>
		/// <param name="sourceColumn">The name of the source column within the data source.</param>
		/// <param name="destinationColumn">The name of the destination column within the destination table.</param>
		public SqlBulkCopyColumnMapping(string sourceColumn, string destinationColumn)
		{
			this.SourceColumn = sourceColumn;
			this.DestinationColumn = destinationColumn;
		}

		/// <summary>Creates a new column mapping, using a column ordinal to refer to the source column and a column name for the target column.</summary>
		/// <param name="sourceColumnOrdinal">The ordinal position of the source column within the data source.</param>
		/// <param name="destinationColumn">The name of the destination column within the destination table.</param>
		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, string destinationColumn)
		{
			this.SourceOrdinal = sourceColumnOrdinal;
			this.DestinationColumn = destinationColumn;
		}

		/// <summary>Creates a new column mapping, using a column name to refer to the source column and a column ordinal for the target column.</summary>
		/// <param name="sourceColumn">The name of the source column within the data source.</param>
		/// <param name="destinationOrdinal">The ordinal position of the destination column within the destination table.</param>
		public SqlBulkCopyColumnMapping(string sourceColumn, int destinationOrdinal)
		{
			this.SourceColumn = sourceColumn;
			this.DestinationOrdinal = destinationOrdinal;
		}

		/// <summary>Creates a new column mapping, using column ordinals to refer to source and destination columns.</summary>
		/// <param name="sourceColumnOrdinal">The ordinal position of the source column within the data source.</param>
		/// <param name="destinationOrdinal">The ordinal position of the destination column within the destination table.</param>
		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, int destinationOrdinal)
		{
			this.SourceOrdinal = sourceColumnOrdinal;
			this.DestinationOrdinal = destinationOrdinal;
		}

		internal string _destinationColumnName;

		internal int _destinationColumnOrdinal;

		internal string _sourceColumnName;

		internal int _sourceColumnOrdinal;

		internal int _internalDestinationColumnOrdinal;

		internal int _internalSourceColumnOrdinal;
	}
}
