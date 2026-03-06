using System;
using System.Globalization;

namespace System.Data.Common
{
	internal sealed class DbSchemaRow
	{
		internal static DbSchemaRow[] GetSortedSchemaRows(DataTable dataTable, bool returnProviderSpecificTypes)
		{
			DataColumn dataColumn = dataTable.Columns["SchemaMapping Unsorted Index"];
			if (dataColumn == null)
			{
				dataColumn = new DataColumn("SchemaMapping Unsorted Index", typeof(int));
				dataTable.Columns.Add(dataColumn);
			}
			int count = dataTable.Rows.Count;
			for (int i = 0; i < count; i++)
			{
				dataTable.Rows[i][dataColumn] = i;
			}
			DbSchemaTable schemaTable = new DbSchemaTable(dataTable, returnProviderSpecificTypes);
			DataRow[] array = dataTable.Select(null, "ColumnOrdinal ASC", DataViewRowState.CurrentRows);
			DbSchemaRow[] array2 = new DbSchemaRow[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = new DbSchemaRow(schemaTable, array[j]);
			}
			return array2;
		}

		internal DbSchemaRow(DbSchemaTable schemaTable, DataRow dataRow)
		{
			this._schemaTable = schemaTable;
			this._dataRow = dataRow;
		}

		internal DataRow DataRow
		{
			get
			{
				return this._dataRow;
			}
		}

		internal string ColumnName
		{
			get
			{
				object value = this._dataRow[this._schemaTable.ColumnName, DataRowVersion.Default];
				if (!Convert.IsDBNull(value))
				{
					return Convert.ToString(value, CultureInfo.InvariantCulture);
				}
				return string.Empty;
			}
		}

		internal int Size
		{
			get
			{
				object value = this._dataRow[this._schemaTable.Size, DataRowVersion.Default];
				if (!Convert.IsDBNull(value))
				{
					return Convert.ToInt32(value, CultureInfo.InvariantCulture);
				}
				return 0;
			}
		}

		internal string BaseColumnName
		{
			get
			{
				if (this._schemaTable.BaseColumnName != null)
				{
					object value = this._dataRow[this._schemaTable.BaseColumnName, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToString(value, CultureInfo.InvariantCulture);
					}
				}
				return string.Empty;
			}
		}

		internal string BaseServerName
		{
			get
			{
				if (this._schemaTable.BaseServerName != null)
				{
					object value = this._dataRow[this._schemaTable.BaseServerName, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToString(value, CultureInfo.InvariantCulture);
					}
				}
				return string.Empty;
			}
		}

		internal string BaseCatalogName
		{
			get
			{
				if (this._schemaTable.BaseCatalogName != null)
				{
					object value = this._dataRow[this._schemaTable.BaseCatalogName, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToString(value, CultureInfo.InvariantCulture);
					}
				}
				return string.Empty;
			}
		}

		internal string BaseSchemaName
		{
			get
			{
				if (this._schemaTable.BaseSchemaName != null)
				{
					object value = this._dataRow[this._schemaTable.BaseSchemaName, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToString(value, CultureInfo.InvariantCulture);
					}
				}
				return string.Empty;
			}
		}

		internal string BaseTableName
		{
			get
			{
				if (this._schemaTable.BaseTableName != null)
				{
					object value = this._dataRow[this._schemaTable.BaseTableName, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToString(value, CultureInfo.InvariantCulture);
					}
				}
				return string.Empty;
			}
		}

		internal bool IsAutoIncrement
		{
			get
			{
				if (this._schemaTable.IsAutoIncrement != null)
				{
					object value = this._dataRow[this._schemaTable.IsAutoIncrement, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsUnique
		{
			get
			{
				if (this._schemaTable.IsUnique != null)
				{
					object value = this._dataRow[this._schemaTable.IsUnique, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsRowVersion
		{
			get
			{
				if (this._schemaTable.IsRowVersion != null)
				{
					object value = this._dataRow[this._schemaTable.IsRowVersion, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsKey
		{
			get
			{
				if (this._schemaTable.IsKey != null)
				{
					object value = this._dataRow[this._schemaTable.IsKey, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsExpression
		{
			get
			{
				if (this._schemaTable.IsExpression != null)
				{
					object value = this._dataRow[this._schemaTable.IsExpression, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsHidden
		{
			get
			{
				if (this._schemaTable.IsHidden != null)
				{
					object value = this._dataRow[this._schemaTable.IsHidden, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsLong
		{
			get
			{
				if (this._schemaTable.IsLong != null)
				{
					object value = this._dataRow[this._schemaTable.IsLong, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				if (this._schemaTable.IsReadOnly != null)
				{
					object value = this._dataRow[this._schemaTable.IsReadOnly, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return false;
			}
		}

		internal Type DataType
		{
			get
			{
				if (this._schemaTable.DataType != null)
				{
					object obj = this._dataRow[this._schemaTable.DataType, DataRowVersion.Default];
					if (!Convert.IsDBNull(obj))
					{
						return (Type)obj;
					}
				}
				return null;
			}
		}

		internal bool AllowDBNull
		{
			get
			{
				if (this._schemaTable.AllowDBNull != null)
				{
					object value = this._dataRow[this._schemaTable.AllowDBNull, DataRowVersion.Default];
					if (!Convert.IsDBNull(value))
					{
						return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
					}
				}
				return true;
			}
		}

		internal int UnsortedIndex
		{
			get
			{
				return (int)this._dataRow[this._schemaTable.UnsortedIndex, DataRowVersion.Default];
			}
		}

		internal const string SchemaMappingUnsortedIndex = "SchemaMapping Unsorted Index";

		private DbSchemaTable _schemaTable;

		private DataRow _dataRow;
	}
}
