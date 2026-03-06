using System;

namespace System.Data.Common
{
	/// <summary>Describes the column metadata of the schema for a database table.</summary>
	public static class SchemaTableColumn
	{
		/// <summary>Specifies the name of the column in the schema table.</summary>
		public static readonly string ColumnName = "ColumnName";

		/// <summary>Specifies the ordinal of the column.</summary>
		public static readonly string ColumnOrdinal = "ColumnOrdinal";

		/// <summary>Specifies the size of the column.</summary>
		public static readonly string ColumnSize = "ColumnSize";

		/// <summary>Specifies the precision of the column data, if the data is numeric.</summary>
		public static readonly string NumericPrecision = "NumericPrecision";

		/// <summary>Specifies the scale of the column data, if the data is numeric.</summary>
		public static readonly string NumericScale = "NumericScale";

		/// <summary>Specifies the type of data in the column.</summary>
		public static readonly string DataType = "DataType";

		/// <summary>Specifies the provider-specific data type of the column.</summary>
		public static readonly string ProviderType = "ProviderType";

		/// <summary>Specifies the non-versioned provider-specific data type of the column.</summary>
		public static readonly string NonVersionedProviderType = "NonVersionedProviderType";

		/// <summary>Specifies whether this column contains long data.</summary>
		public static readonly string IsLong = "IsLong";

		/// <summary>Specifies whether value <see langword="DBNull" /> is allowed.</summary>
		public static readonly string AllowDBNull = "AllowDBNull";

		/// <summary>Specifies whether this column is aliased.</summary>
		public static readonly string IsAliased = "IsAliased";

		/// <summary>Specifies whether this column is an expression.</summary>
		public static readonly string IsExpression = "IsExpression";

		/// <summary>Specifies whether this column is a key for the table.</summary>
		public static readonly string IsKey = "IsKey";

		/// <summary>Specifies whether a unique constraint applies to this column.</summary>
		public static readonly string IsUnique = "IsUnique";

		/// <summary>Specifies the name of the schema in the schema table.</summary>
		public static readonly string BaseSchemaName = "BaseSchemaName";

		/// <summary>Specifies the name of the table in the schema table.</summary>
		public static readonly string BaseTableName = "BaseTableName";

		/// <summary>Specifies the name of the column in the schema table.</summary>
		public static readonly string BaseColumnName = "BaseColumnName";
	}
}
