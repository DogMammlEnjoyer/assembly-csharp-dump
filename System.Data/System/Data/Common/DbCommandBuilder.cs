using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Common
{
	/// <summary>Automatically generates single-table commands used to reconcile changes made to a <see cref="T:System.Data.DataSet" /> with the associated database. This is an abstract class that can only be inherited.</summary>
	public abstract class DbCommandBuilder : Component
	{
		/// <summary>Specifies which <see cref="T:System.Data.ConflictOption" /> is to be used by the <see cref="T:System.Data.Common.DbCommandBuilder" />.</summary>
		/// <returns>Returns one of the <see cref="T:System.Data.ConflictOption" /> values describing the behavior of this <see cref="T:System.Data.Common.DbCommandBuilder" />.</returns>
		[DefaultValue(ConflictOption.CompareAllSearchableValues)]
		public virtual ConflictOption ConflictOption
		{
			get
			{
				return this._conflictDetection;
			}
			set
			{
				if (value - ConflictOption.CompareAllSearchableValues <= 2)
				{
					this._conflictDetection = value;
					return;
				}
				throw ADP.InvalidConflictOptions(value);
			}
		}

		/// <summary>Sets or gets the <see cref="T:System.Data.Common.CatalogLocation" /> for an instance of the <see cref="T:System.Data.Common.DbCommandBuilder" /> class.</summary>
		/// <returns>A <see cref="T:System.Data.Common.CatalogLocation" /> object.</returns>
		[DefaultValue(CatalogLocation.Start)]
		public virtual CatalogLocation CatalogLocation
		{
			get
			{
				return this._catalogLocation;
			}
			set
			{
				if (this._dbSchemaTable != null)
				{
					throw ADP.NoQuoteChange();
				}
				if (value - CatalogLocation.Start <= 1)
				{
					this._catalogLocation = value;
					return;
				}
				throw ADP.InvalidCatalogLocation(value);
			}
		}

		/// <summary>Sets or gets a string used as the catalog separator for an instance of the <see cref="T:System.Data.Common.DbCommandBuilder" /> class.</summary>
		/// <returns>A string indicating the catalog separator for use with an instance of the <see cref="T:System.Data.Common.DbCommandBuilder" /> class.</returns>
		[DefaultValue(".")]
		public virtual string CatalogSeparator
		{
			get
			{
				string catalogSeparator = this._catalogSeparator;
				if (catalogSeparator == null || 0 >= catalogSeparator.Length)
				{
					return ".";
				}
				return catalogSeparator;
			}
			set
			{
				if (this._dbSchemaTable != null)
				{
					throw ADP.NoQuoteChange();
				}
				this._catalogSeparator = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Data.Common.DbDataAdapter" /> object for which Transact-SQL statements are automatically generated.</summary>
		/// <returns>A <see cref="T:System.Data.Common.DbDataAdapter" /> object.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbDataAdapter DataAdapter
		{
			get
			{
				return this._dataAdapter;
			}
			set
			{
				if (this._dataAdapter != value)
				{
					this.RefreshSchema();
					if (this._dataAdapter != null)
					{
						this.SetRowUpdatingHandler(this._dataAdapter);
						this._dataAdapter = null;
					}
					if (value != null)
					{
						this.SetRowUpdatingHandler(value);
						this._dataAdapter = value;
					}
				}
			}
		}

		internal int ParameterNameMaxLength
		{
			get
			{
				return this._parameterNameMaxLength;
			}
		}

		internal string ParameterNamePattern
		{
			get
			{
				return this._parameterNamePattern;
			}
		}

		private string QuotedBaseTableName
		{
			get
			{
				return this._quotedBaseTableName;
			}
		}

		/// <summary>Gets or sets the beginning character or characters to use when specifying database objects (for example, tables or columns) whose names contain characters such as spaces or reserved tokens.</summary>
		/// <returns>The beginning character or characters to use. The default is an empty string.</returns>
		/// <exception cref="T:System.InvalidOperationException">This property cannot be changed after an insert, update, or delete command has been generated.</exception>
		[DefaultValue("")]
		public virtual string QuotePrefix
		{
			get
			{
				return this._quotePrefix ?? string.Empty;
			}
			set
			{
				if (this._dbSchemaTable != null)
				{
					throw ADP.NoQuoteChange();
				}
				this._quotePrefix = value;
			}
		}

		/// <summary>Gets or sets the ending character or characters to use when specifying database objects (for example, tables or columns) whose names contain characters such as spaces or reserved tokens.</summary>
		/// <returns>The ending character or characters to use. The default is an empty string.</returns>
		[DefaultValue("")]
		public virtual string QuoteSuffix
		{
			get
			{
				string quoteSuffix = this._quoteSuffix;
				if (quoteSuffix == null)
				{
					return string.Empty;
				}
				return quoteSuffix;
			}
			set
			{
				if (this._dbSchemaTable != null)
				{
					throw ADP.NoQuoteChange();
				}
				this._quoteSuffix = value;
			}
		}

		/// <summary>Gets or sets the character to be used for the separator between the schema identifier and any other identifiers.</summary>
		/// <returns>The character to be used as the schema separator.</returns>
		[DefaultValue(".")]
		public virtual string SchemaSeparator
		{
			get
			{
				string schemaSeparator = this._schemaSeparator;
				if (schemaSeparator == null || 0 >= schemaSeparator.Length)
				{
					return ".";
				}
				return schemaSeparator;
			}
			set
			{
				if (this._dbSchemaTable != null)
				{
					throw ADP.NoQuoteChange();
				}
				this._schemaSeparator = value;
			}
		}

		/// <summary>Specifies whether all column values in an update statement are included or only changed ones.</summary>
		/// <returns>
		///   <see langword="true" /> if the UPDATE statement generated by the <see cref="T:System.Data.Common.DbCommandBuilder" /> includes all columns; <see langword="false" /> if it includes only changed columns.</returns>
		[DefaultValue(false)]
		public bool SetAllValues
		{
			get
			{
				return this._setAllValues;
			}
			set
			{
				this._setAllValues = value;
			}
		}

		private DbCommand InsertCommand
		{
			get
			{
				return this._insertCommand;
			}
			set
			{
				this._insertCommand = value;
			}
		}

		private DbCommand UpdateCommand
		{
			get
			{
				return this._updateCommand;
			}
			set
			{
				this._updateCommand = value;
			}
		}

		private DbCommand DeleteCommand
		{
			get
			{
				return this._deleteCommand;
			}
			set
			{
				this._deleteCommand = value;
			}
		}

		private void BuildCache(bool closeConnection, DataRow dataRow, bool useColumnsForParameterNames)
		{
			if (this._dbSchemaTable != null && (!useColumnsForParameterNames || this._parameterNames != null))
			{
				return;
			}
			DataTable dataTable = null;
			DbCommand selectCommand = this.GetSelectCommand();
			DbConnection connection = selectCommand.Connection;
			if (connection == null)
			{
				throw ADP.MissingSourceCommandConnection();
			}
			try
			{
				if ((ConnectionState.Open & connection.State) == ConnectionState.Closed)
				{
					connection.Open();
				}
				else
				{
					closeConnection = false;
				}
				if (useColumnsForParameterNames)
				{
					DataTable schema = connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);
					if (schema.Rows.Count == 1)
					{
						this._parameterNamePattern = (schema.Rows[0][DbMetaDataColumnNames.ParameterNamePattern] as string);
						this._parameterMarkerFormat = (schema.Rows[0][DbMetaDataColumnNames.ParameterMarkerFormat] as string);
						object obj = schema.Rows[0][DbMetaDataColumnNames.ParameterNameMaxLength];
						this._parameterNameMaxLength = ((obj is int) ? ((int)obj) : 0);
						if (this._parameterNameMaxLength == 0 || this._parameterNamePattern == null || this._parameterMarkerFormat == null)
						{
							useColumnsForParameterNames = false;
						}
					}
					else
					{
						useColumnsForParameterNames = false;
					}
				}
				dataTable = this.GetSchemaTable(selectCommand);
			}
			finally
			{
				if (closeConnection)
				{
					connection.Close();
				}
			}
			if (dataTable == null)
			{
				throw ADP.DynamicSQLNoTableInfo();
			}
			this.BuildInformation(dataTable);
			this._dbSchemaTable = dataTable;
			DbSchemaRow[] dbSchemaRows = this._dbSchemaRows;
			string[] array = new string[dbSchemaRows.Length];
			for (int i = 0; i < dbSchemaRows.Length; i++)
			{
				if (dbSchemaRows[i] != null)
				{
					array[i] = dbSchemaRows[i].ColumnName;
				}
			}
			this._sourceColumnNames = array;
			if (useColumnsForParameterNames)
			{
				this._parameterNames = new DbCommandBuilder.ParameterNames(this, dbSchemaRows);
			}
			ADP.BuildSchemaTableInfoTableNames(array);
		}

		/// <summary>Returns the schema table for the <see cref="T:System.Data.Common.DbCommandBuilder" />.</summary>
		/// <param name="sourceCommand">The <see cref="T:System.Data.Common.DbCommand" /> for which to retrieve the corresponding schema table.</param>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that represents the schema for the specific <see cref="T:System.Data.Common.DbCommand" />.</returns>
		protected virtual DataTable GetSchemaTable(DbCommand sourceCommand)
		{
			DataTable schemaTable;
			using (IDataReader dataReader = sourceCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
			{
				schemaTable = dataReader.GetSchemaTable();
			}
			return schemaTable;
		}

		private void BuildInformation(DataTable schemaTable)
		{
			DbSchemaRow[] sortedSchemaRows = DbSchemaRow.GetSortedSchemaRows(schemaTable, false);
			if (sortedSchemaRows == null || sortedSchemaRows.Length == 0)
			{
				throw ADP.DynamicSQLNoTableInfo();
			}
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = null;
			for (int i = 0; i < sortedSchemaRows.Length; i++)
			{
				DbSchemaRow dbSchemaRow = sortedSchemaRows[i];
				string baseTableName = dbSchemaRow.BaseTableName;
				if (baseTableName == null || baseTableName.Length == 0)
				{
					sortedSchemaRows[i] = null;
				}
				else
				{
					string text5 = dbSchemaRow.BaseServerName;
					string text6 = dbSchemaRow.BaseCatalogName;
					string text7 = dbSchemaRow.BaseSchemaName;
					if (text5 == null)
					{
						text5 = string.Empty;
					}
					if (text6 == null)
					{
						text6 = string.Empty;
					}
					if (text7 == null)
					{
						text7 = string.Empty;
					}
					if (text4 == null)
					{
						text = text5;
						text2 = text6;
						text3 = text7;
						text4 = baseTableName;
					}
					else if (ADP.SrcCompare(text4, baseTableName) != 0 || ADP.SrcCompare(text3, text7) != 0 || ADP.SrcCompare(text2, text6) != 0 || ADP.SrcCompare(text, text5) != 0)
					{
						throw ADP.DynamicSQLJoinUnsupported();
					}
				}
			}
			if (text.Length == 0)
			{
				text = null;
			}
			if (text2.Length == 0)
			{
				text = null;
				text2 = null;
			}
			if (text3.Length == 0)
			{
				text = null;
				text2 = null;
				text3 = null;
			}
			if (text4 == null || text4.Length == 0)
			{
				throw ADP.DynamicSQLNoTableInfo();
			}
			CatalogLocation catalogLocation = this.CatalogLocation;
			string catalogSeparator = this.CatalogSeparator;
			string schemaSeparator = this.SchemaSeparator;
			string quotePrefix = this.QuotePrefix;
			string quoteSuffix = this.QuoteSuffix;
			if (!string.IsNullOrEmpty(quotePrefix) && -1 != text4.IndexOf(quotePrefix, StringComparison.Ordinal))
			{
				throw ADP.DynamicSQLNestedQuote(text4, quotePrefix);
			}
			if (!string.IsNullOrEmpty(quoteSuffix) && -1 != text4.IndexOf(quoteSuffix, StringComparison.Ordinal))
			{
				throw ADP.DynamicSQLNestedQuote(text4, quoteSuffix);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (CatalogLocation.Start == catalogLocation)
			{
				if (text != null)
				{
					stringBuilder.Append(ADP.BuildQuotedString(quotePrefix, quoteSuffix, text));
					stringBuilder.Append(catalogSeparator);
				}
				if (text2 != null)
				{
					stringBuilder.Append(ADP.BuildQuotedString(quotePrefix, quoteSuffix, text2));
					stringBuilder.Append(catalogSeparator);
				}
			}
			if (text3 != null)
			{
				stringBuilder.Append(ADP.BuildQuotedString(quotePrefix, quoteSuffix, text3));
				stringBuilder.Append(schemaSeparator);
			}
			stringBuilder.Append(ADP.BuildQuotedString(quotePrefix, quoteSuffix, text4));
			if (CatalogLocation.End == catalogLocation)
			{
				if (text != null)
				{
					stringBuilder.Append(catalogSeparator);
					stringBuilder.Append(ADP.BuildQuotedString(quotePrefix, quoteSuffix, text));
				}
				if (text2 != null)
				{
					stringBuilder.Append(catalogSeparator);
					stringBuilder.Append(ADP.BuildQuotedString(quotePrefix, quoteSuffix, text2));
				}
			}
			this._quotedBaseTableName = stringBuilder.ToString();
			this._hasPartialPrimaryKey = false;
			foreach (DbSchemaRow dbSchemaRow2 in sortedSchemaRows)
			{
				if (dbSchemaRow2 != null && (dbSchemaRow2.IsKey || dbSchemaRow2.IsUnique) && !dbSchemaRow2.IsLong && !dbSchemaRow2.IsRowVersion && dbSchemaRow2.IsHidden)
				{
					this._hasPartialPrimaryKey = true;
					break;
				}
			}
			this._dbSchemaRows = sortedSchemaRows;
		}

		private DbCommand BuildDeleteCommand(DataTableMapping mappings, DataRow dataRow)
		{
			DbCommand dbCommand = this.InitializeCommand(this.DeleteCommand);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			stringBuilder.Append("DELETE FROM ");
			stringBuilder.Append(this.QuotedBaseTableName);
			num = this.BuildWhereClause(mappings, dataRow, stringBuilder, dbCommand, num, false);
			dbCommand.CommandText = stringBuilder.ToString();
			DbCommandBuilder.RemoveExtraParameters(dbCommand, num);
			this.DeleteCommand = dbCommand;
			return dbCommand;
		}

		private DbCommand BuildInsertCommand(DataTableMapping mappings, DataRow dataRow)
		{
			DbCommand dbCommand = this.InitializeCommand(this.InsertCommand);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			string value = " (";
			stringBuilder.Append("INSERT INTO ");
			stringBuilder.Append(this.QuotedBaseTableName);
			DbSchemaRow[] dbSchemaRows = this._dbSchemaRows;
			string[] array = new string[dbSchemaRows.Length];
			for (int i = 0; i < dbSchemaRows.Length; i++)
			{
				DbSchemaRow dbSchemaRow = dbSchemaRows[i];
				if (dbSchemaRow != null && dbSchemaRow.BaseColumnName.Length != 0 && this.IncludeInInsertValues(dbSchemaRow))
				{
					object obj = null;
					string text = this._sourceColumnNames[i];
					if (mappings != null && dataRow != null)
					{
						DataColumn dataColumn = this.GetDataColumn(text, mappings, dataRow);
						if (dataColumn == null || (dbSchemaRow.IsReadOnly && dataColumn.ReadOnly))
						{
							goto IL_11E;
						}
						obj = this.GetColumnValue(dataRow, dataColumn, DataRowVersion.Current);
						if (!dbSchemaRow.AllowDBNull && (obj == null || Convert.IsDBNull(obj)))
						{
							goto IL_11E;
						}
					}
					stringBuilder.Append(value);
					value = ", ";
					stringBuilder.Append(this.QuotedColumn(dbSchemaRow.BaseColumnName));
					array[num] = this.CreateParameterForValue(dbCommand, this.GetBaseParameterName(i), text, DataRowVersion.Current, num, obj, dbSchemaRow, StatementType.Insert, false);
					num++;
				}
				IL_11E:;
			}
			if (num == 0)
			{
				stringBuilder.Append(" DEFAULT VALUES");
			}
			else
			{
				stringBuilder.Append(")");
				stringBuilder.Append(" VALUES ");
				stringBuilder.Append("(");
				stringBuilder.Append(array[0]);
				for (int j = 1; j < num; j++)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(array[j]);
				}
				stringBuilder.Append(")");
			}
			dbCommand.CommandText = stringBuilder.ToString();
			DbCommandBuilder.RemoveExtraParameters(dbCommand, num);
			this.InsertCommand = dbCommand;
			return dbCommand;
		}

		private DbCommand BuildUpdateCommand(DataTableMapping mappings, DataRow dataRow)
		{
			DbCommand dbCommand = this.InitializeCommand(this.UpdateCommand);
			StringBuilder stringBuilder = new StringBuilder();
			string value = " SET ";
			int num = 0;
			stringBuilder.Append("UPDATE ");
			stringBuilder.Append(this.QuotedBaseTableName);
			DbSchemaRow[] dbSchemaRows = this._dbSchemaRows;
			for (int i = 0; i < dbSchemaRows.Length; i++)
			{
				DbSchemaRow dbSchemaRow = dbSchemaRows[i];
				if (dbSchemaRow != null && dbSchemaRow.BaseColumnName.Length != 0 && this.IncludeInUpdateSet(dbSchemaRow))
				{
					object obj = null;
					string text = this._sourceColumnNames[i];
					if (mappings != null && dataRow != null)
					{
						DataColumn dataColumn = this.GetDataColumn(text, mappings, dataRow);
						if (dataColumn == null || (dbSchemaRow.IsReadOnly && dataColumn.ReadOnly))
						{
							goto IL_13F;
						}
						obj = this.GetColumnValue(dataRow, dataColumn, DataRowVersion.Current);
						if (!this.SetAllValues)
						{
							object columnValue = this.GetColumnValue(dataRow, dataColumn, DataRowVersion.Original);
							if (columnValue == obj || (columnValue != null && columnValue.Equals(obj)))
							{
								goto IL_13F;
							}
						}
					}
					stringBuilder.Append(value);
					value = ", ";
					stringBuilder.Append(this.QuotedColumn(dbSchemaRow.BaseColumnName));
					stringBuilder.Append(" = ");
					stringBuilder.Append(this.CreateParameterForValue(dbCommand, this.GetBaseParameterName(i), text, DataRowVersion.Current, num, obj, dbSchemaRow, StatementType.Update, false));
					num++;
				}
				IL_13F:;
			}
			bool flag = num == 0;
			num = this.BuildWhereClause(mappings, dataRow, stringBuilder, dbCommand, num, true);
			dbCommand.CommandText = stringBuilder.ToString();
			DbCommandBuilder.RemoveExtraParameters(dbCommand, num);
			this.UpdateCommand = dbCommand;
			if (!flag)
			{
				return dbCommand;
			}
			return null;
		}

		private int BuildWhereClause(DataTableMapping mappings, DataRow dataRow, StringBuilder builder, DbCommand command, int parameterCount, bool isUpdate)
		{
			string value = string.Empty;
			int num = 0;
			builder.Append(" WHERE ");
			builder.Append("(");
			DbSchemaRow[] dbSchemaRows = this._dbSchemaRows;
			for (int i = 0; i < dbSchemaRows.Length; i++)
			{
				DbSchemaRow dbSchemaRow = dbSchemaRows[i];
				if (dbSchemaRow != null && dbSchemaRow.BaseColumnName.Length != 0 && this.IncludeInWhereClause(dbSchemaRow, isUpdate))
				{
					builder.Append(value);
					value = " AND ";
					object value2 = null;
					string text = this._sourceColumnNames[i];
					string value3 = this.QuotedColumn(dbSchemaRow.BaseColumnName);
					if (mappings != null && dataRow != null)
					{
						value2 = this.GetColumnValue(dataRow, text, mappings, DataRowVersion.Original);
					}
					if (!dbSchemaRow.AllowDBNull)
					{
						builder.Append("(");
						builder.Append(value3);
						builder.Append(" = ");
						builder.Append(this.CreateParameterForValue(command, this.GetOriginalParameterName(i), text, DataRowVersion.Original, parameterCount, value2, dbSchemaRow, isUpdate ? StatementType.Update : StatementType.Delete, true));
						parameterCount++;
						builder.Append(")");
					}
					else
					{
						builder.Append("(");
						builder.Append("(");
						builder.Append(this.CreateParameterForNullTest(command, this.GetNullParameterName(i), text, DataRowVersion.Original, parameterCount, value2, dbSchemaRow, isUpdate ? StatementType.Update : StatementType.Delete, true));
						parameterCount++;
						builder.Append(" = 1");
						builder.Append(" AND ");
						builder.Append(value3);
						builder.Append(" IS NULL");
						builder.Append(")");
						builder.Append(" OR ");
						builder.Append("(");
						builder.Append(value3);
						builder.Append(" = ");
						builder.Append(this.CreateParameterForValue(command, this.GetOriginalParameterName(i), text, DataRowVersion.Original, parameterCount, value2, dbSchemaRow, isUpdate ? StatementType.Update : StatementType.Delete, true));
						parameterCount++;
						builder.Append(")");
						builder.Append(")");
					}
					if (this.IncrementWhereCount(dbSchemaRow))
					{
						num++;
					}
				}
			}
			builder.Append(")");
			if (num != 0)
			{
				return parameterCount;
			}
			if (isUpdate)
			{
				if (ConflictOption.CompareRowVersion == this.ConflictOption)
				{
					throw ADP.DynamicSQLNoKeyInfoRowVersionUpdate();
				}
				throw ADP.DynamicSQLNoKeyInfoUpdate();
			}
			else
			{
				if (ConflictOption.CompareRowVersion == this.ConflictOption)
				{
					throw ADP.DynamicSQLNoKeyInfoRowVersionDelete();
				}
				throw ADP.DynamicSQLNoKeyInfoDelete();
			}
		}

		private string CreateParameterForNullTest(DbCommand command, string parameterName, string sourceColumn, DataRowVersion version, int parameterCount, object value, DbSchemaRow row, StatementType statementType, bool whereClause)
		{
			DbParameter nextParameter = DbCommandBuilder.GetNextParameter(command, parameterCount);
			if (parameterName == null)
			{
				nextParameter.ParameterName = this.GetParameterName(1 + parameterCount);
			}
			else
			{
				nextParameter.ParameterName = parameterName;
			}
			nextParameter.Direction = ParameterDirection.Input;
			nextParameter.SourceColumn = sourceColumn;
			nextParameter.SourceVersion = version;
			nextParameter.SourceColumnNullMapping = true;
			nextParameter.Value = value;
			nextParameter.Size = 0;
			this.ApplyParameterInfo(nextParameter, row.DataRow, statementType, whereClause);
			nextParameter.DbType = DbType.Int32;
			nextParameter.Value = (ADP.IsNull(value) ? DbDataAdapter.s_parameterValueNullValue : DbDataAdapter.s_parameterValueNonNullValue);
			if (!command.Parameters.Contains(nextParameter))
			{
				command.Parameters.Add(nextParameter);
			}
			if (parameterName == null)
			{
				return this.GetParameterPlaceholder(1 + parameterCount);
			}
			return string.Format(CultureInfo.InvariantCulture, this._parameterMarkerFormat, parameterName);
		}

		private string CreateParameterForValue(DbCommand command, string parameterName, string sourceColumn, DataRowVersion version, int parameterCount, object value, DbSchemaRow row, StatementType statementType, bool whereClause)
		{
			DbParameter nextParameter = DbCommandBuilder.GetNextParameter(command, parameterCount);
			if (parameterName == null)
			{
				nextParameter.ParameterName = this.GetParameterName(1 + parameterCount);
			}
			else
			{
				nextParameter.ParameterName = parameterName;
			}
			nextParameter.Direction = ParameterDirection.Input;
			nextParameter.SourceColumn = sourceColumn;
			nextParameter.SourceVersion = version;
			nextParameter.SourceColumnNullMapping = false;
			nextParameter.Value = value;
			nextParameter.Size = 0;
			this.ApplyParameterInfo(nextParameter, row.DataRow, statementType, whereClause);
			if (!command.Parameters.Contains(nextParameter))
			{
				command.Parameters.Add(nextParameter);
			}
			if (parameterName == null)
			{
				return this.GetParameterPlaceholder(1 + parameterCount);
			}
			return string.Format(CultureInfo.InvariantCulture, this._parameterMarkerFormat, parameterName);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Data.Common.DbCommandBuilder" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DataAdapter = null;
			}
			base.Dispose(disposing);
		}

		private DataTableMapping GetTableMapping(DataRow dataRow)
		{
			DataTableMapping result = null;
			if (dataRow != null)
			{
				DataTable table = dataRow.Table;
				if (table != null)
				{
					DbDataAdapter dataAdapter = this.DataAdapter;
					if (dataAdapter != null)
					{
						result = dataAdapter.GetTableMapping(table);
					}
					else
					{
						string tableName = table.TableName;
						result = new DataTableMapping(tableName, tableName);
					}
				}
			}
			return result;
		}

		private string GetBaseParameterName(int index)
		{
			if (this._parameterNames != null)
			{
				return this._parameterNames.GetBaseParameterName(index);
			}
			return null;
		}

		private string GetOriginalParameterName(int index)
		{
			if (this._parameterNames != null)
			{
				return this._parameterNames.GetOriginalParameterName(index);
			}
			return null;
		}

		private string GetNullParameterName(int index)
		{
			if (this._parameterNames != null)
			{
				return this._parameterNames.GetNullParameterName(index);
			}
			return null;
		}

		private DbCommand GetSelectCommand()
		{
			DbCommand dbCommand = null;
			DbDataAdapter dataAdapter = this.DataAdapter;
			if (dataAdapter != null)
			{
				if (this._missingMappingAction == (MissingMappingAction)0)
				{
					this._missingMappingAction = dataAdapter.MissingMappingAction;
				}
				dbCommand = dataAdapter.SelectCommand;
			}
			if (dbCommand == null)
			{
				throw ADP.MissingSourceCommand();
			}
			return dbCommand;
		}

		/// <summary>Gets the automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform insertions at the data source.</summary>
		/// <returns>The automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform insertions.</returns>
		public DbCommand GetInsertCommand()
		{
			return this.GetInsertCommand(null, false);
		}

		/// <summary>Gets the automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform insertions at the data source, optionally using columns for parameter names.</summary>
		/// <param name="useColumnsForParameterNames">If <see langword="true" />, generate parameter names matching column names, if possible. If <see langword="false" />, generate @p1, @p2, and so on.</param>
		/// <returns>The automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform insertions.</returns>
		public DbCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			return this.GetInsertCommand(null, useColumnsForParameterNames);
		}

		internal DbCommand GetInsertCommand(DataRow dataRow, bool useColumnsForParameterNames)
		{
			this.BuildCache(true, dataRow, useColumnsForParameterNames);
			this.BuildInsertCommand(this.GetTableMapping(dataRow), dataRow);
			return this.InsertCommand;
		}

		/// <summary>Gets the automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform updates at the data source.</summary>
		/// <returns>The automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform updates.</returns>
		public DbCommand GetUpdateCommand()
		{
			return this.GetUpdateCommand(null, false);
		}

		/// <summary>Gets the automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform updates at the data source, optionally using columns for parameter names.</summary>
		/// <param name="useColumnsForParameterNames">If <see langword="true" />, generate parameter names matching column names, if possible. If <see langword="false" />, generate @p1, @p2, and so on.</param>
		/// <returns>The automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform updates.</returns>
		public DbCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			return this.GetUpdateCommand(null, useColumnsForParameterNames);
		}

		internal DbCommand GetUpdateCommand(DataRow dataRow, bool useColumnsForParameterNames)
		{
			this.BuildCache(true, dataRow, useColumnsForParameterNames);
			this.BuildUpdateCommand(this.GetTableMapping(dataRow), dataRow);
			return this.UpdateCommand;
		}

		/// <summary>Gets the automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform deletions at the data source.</summary>
		/// <returns>The automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform deletions.</returns>
		public DbCommand GetDeleteCommand()
		{
			return this.GetDeleteCommand(null, false);
		}

		/// <summary>Gets the automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform deletions at the data source, optionally using columns for parameter names.</summary>
		/// <param name="useColumnsForParameterNames">If <see langword="true" />, generate parameter names matching column names, if possible. If <see langword="false" />, generate @p1, @p2, and so on.</param>
		/// <returns>The automatically generated <see cref="T:System.Data.Common.DbCommand" /> object required to perform deletions.</returns>
		public DbCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			return this.GetDeleteCommand(null, useColumnsForParameterNames);
		}

		internal DbCommand GetDeleteCommand(DataRow dataRow, bool useColumnsForParameterNames)
		{
			this.BuildCache(true, dataRow, useColumnsForParameterNames);
			this.BuildDeleteCommand(this.GetTableMapping(dataRow), dataRow);
			return this.DeleteCommand;
		}

		private object GetColumnValue(DataRow row, string columnName, DataTableMapping mappings, DataRowVersion version)
		{
			return this.GetColumnValue(row, this.GetDataColumn(columnName, mappings, row), version);
		}

		private object GetColumnValue(DataRow row, DataColumn column, DataRowVersion version)
		{
			object result = null;
			if (column != null)
			{
				result = row[column, version];
			}
			return result;
		}

		private DataColumn GetDataColumn(string columnName, DataTableMapping tablemapping, DataRow row)
		{
			DataColumn result = null;
			if (!string.IsNullOrEmpty(columnName))
			{
				result = tablemapping.GetDataColumn(columnName, null, row.Table, this._missingMappingAction, MissingSchemaAction.Error);
			}
			return result;
		}

		private static DbParameter GetNextParameter(DbCommand command, int pcount)
		{
			DbParameter result;
			if (pcount < command.Parameters.Count)
			{
				result = command.Parameters[pcount];
			}
			else
			{
				result = command.CreateParameter();
			}
			return result;
		}

		private bool IncludeInInsertValues(DbSchemaRow row)
		{
			return !row.IsAutoIncrement && !row.IsHidden && !row.IsExpression && !row.IsRowVersion && !row.IsReadOnly;
		}

		private bool IncludeInUpdateSet(DbSchemaRow row)
		{
			return !row.IsAutoIncrement && !row.IsRowVersion && !row.IsHidden && !row.IsReadOnly;
		}

		private bool IncludeInWhereClause(DbSchemaRow row, bool isUpdate)
		{
			bool flag = this.IncrementWhereCount(row);
			if (!flag || !row.IsHidden)
			{
				if (!flag && ConflictOption.CompareAllSearchableValues == this.ConflictOption)
				{
					flag = (!row.IsLong && !row.IsRowVersion && !row.IsHidden);
				}
				return flag;
			}
			if (ConflictOption.CompareRowVersion == this.ConflictOption)
			{
				throw ADP.DynamicSQLNoKeyInfoRowVersionUpdate();
			}
			throw ADP.DynamicSQLNoKeyInfoUpdate();
		}

		private bool IncrementWhereCount(DbSchemaRow row)
		{
			ConflictOption conflictOption = this.ConflictOption;
			switch (conflictOption)
			{
			case ConflictOption.CompareAllSearchableValues:
			case ConflictOption.OverwriteChanges:
				return (row.IsKey || row.IsUnique) && !row.IsLong && !row.IsRowVersion;
			case ConflictOption.CompareRowVersion:
				return (((row.IsKey || row.IsUnique) && !this._hasPartialPrimaryKey) || row.IsRowVersion) && !row.IsLong;
			default:
				throw ADP.InvalidConflictOptions(conflictOption);
			}
		}

		/// <summary>Resets the <see cref="P:System.Data.Common.DbCommand.CommandTimeout" />, <see cref="P:System.Data.Common.DbCommand.Transaction" />, <see cref="P:System.Data.Common.DbCommand.CommandType" />, and <see cref="T:System.Data.UpdateRowSource" /> properties on the <see cref="T:System.Data.Common.DbCommand" />.</summary>
		/// <param name="command">The <see cref="T:System.Data.Common.DbCommand" /> to be used by the command builder for the corresponding insert, update, or delete command.</param>
		/// <returns>A <see cref="T:System.Data.Common.DbCommand" /> instance to use for each insert, update, or delete operation. Passing a null value allows the <see cref="M:System.Data.Common.DbCommandBuilder.InitializeCommand(System.Data.Common.DbCommand)" /> method to create a <see cref="T:System.Data.Common.DbCommand" /> object based on the Select command associated with the <see cref="T:System.Data.Common.DbCommandBuilder" />.</returns>
		protected virtual DbCommand InitializeCommand(DbCommand command)
		{
			if (command == null)
			{
				DbCommand selectCommand = this.GetSelectCommand();
				command = selectCommand.Connection.CreateCommand();
				command.CommandTimeout = selectCommand.CommandTimeout;
				command.Transaction = selectCommand.Transaction;
			}
			command.CommandType = CommandType.Text;
			command.UpdatedRowSource = UpdateRowSource.None;
			return command;
		}

		private string QuotedColumn(string column)
		{
			return ADP.BuildQuotedString(this.QuotePrefix, this.QuoteSuffix, column);
		}

		/// <summary>Given an unquoted identifier in the correct catalog case, returns the correct quoted form of that identifier, including properly escaping any embedded quotes in the identifier.</summary>
		/// <param name="unquotedIdentifier">The original unquoted identifier.</param>
		/// <returns>The quoted version of the identifier. Embedded quotes within the identifier are properly escaped.</returns>
		public virtual string QuoteIdentifier(string unquotedIdentifier)
		{
			throw ADP.NotSupported();
		}

		/// <summary>Clears the commands associated with this <see cref="T:System.Data.Common.DbCommandBuilder" />.</summary>
		public virtual void RefreshSchema()
		{
			this._dbSchemaTable = null;
			this._dbSchemaRows = null;
			this._sourceColumnNames = null;
			this._quotedBaseTableName = null;
			DbDataAdapter dataAdapter = this.DataAdapter;
			if (dataAdapter != null)
			{
				if (this.InsertCommand == dataAdapter.InsertCommand)
				{
					dataAdapter.InsertCommand = null;
				}
				if (this.UpdateCommand == dataAdapter.UpdateCommand)
				{
					dataAdapter.UpdateCommand = null;
				}
				if (this.DeleteCommand == dataAdapter.DeleteCommand)
				{
					dataAdapter.DeleteCommand = null;
				}
			}
			DbCommand dbCommand;
			if ((dbCommand = this.InsertCommand) != null)
			{
				dbCommand.Dispose();
			}
			if ((dbCommand = this.UpdateCommand) != null)
			{
				dbCommand.Dispose();
			}
			if ((dbCommand = this.DeleteCommand) != null)
			{
				dbCommand.Dispose();
			}
			this.InsertCommand = null;
			this.UpdateCommand = null;
			this.DeleteCommand = null;
		}

		private static void RemoveExtraParameters(DbCommand command, int usedParameterCount)
		{
			for (int i = command.Parameters.Count - 1; i >= usedParameterCount; i--)
			{
				command.Parameters.RemoveAt(i);
			}
		}

		/// <summary>Adds an event handler for the <see cref="E:System.Data.OleDb.OleDbDataAdapter.RowUpdating" /> event.</summary>
		/// <param name="rowUpdatingEvent">A <see cref="T:System.Data.Common.RowUpdatingEventArgs" /> instance containing information about the event.</param>
		protected void RowUpdatingHandler(RowUpdatingEventArgs rowUpdatingEvent)
		{
			if (rowUpdatingEvent == null)
			{
				throw ADP.ArgumentNull("rowUpdatingEvent");
			}
			try
			{
				if (rowUpdatingEvent.Status == UpdateStatus.Continue)
				{
					StatementType statementType = rowUpdatingEvent.StatementType;
					DbCommand dbCommand = (DbCommand)rowUpdatingEvent.Command;
					if (dbCommand != null)
					{
						switch (statementType)
						{
						case StatementType.Select:
							return;
						case StatementType.Insert:
							dbCommand = this.InsertCommand;
							break;
						case StatementType.Update:
							dbCommand = this.UpdateCommand;
							break;
						case StatementType.Delete:
							dbCommand = this.DeleteCommand;
							break;
						default:
							throw ADP.InvalidStatementType(statementType);
						}
						if (dbCommand != rowUpdatingEvent.Command)
						{
							dbCommand = (DbCommand)rowUpdatingEvent.Command;
							if (dbCommand != null && dbCommand.Connection == null)
							{
								DbDataAdapter dataAdapter = this.DataAdapter;
								DbCommand dbCommand2 = (dataAdapter != null) ? dataAdapter.SelectCommand : null;
								if (dbCommand2 != null)
								{
									dbCommand.Connection = dbCommand2.Connection;
								}
							}
						}
						else
						{
							dbCommand = null;
						}
					}
					if (dbCommand == null)
					{
						this.RowUpdatingHandlerBuilder(rowUpdatingEvent);
					}
				}
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				ADP.TraceExceptionForCapture(ex);
				rowUpdatingEvent.Status = UpdateStatus.ErrorsOccurred;
				rowUpdatingEvent.Errors = ex;
			}
		}

		private void RowUpdatingHandlerBuilder(RowUpdatingEventArgs rowUpdatingEvent)
		{
			DataRow row = rowUpdatingEvent.Row;
			this.BuildCache(false, row, false);
			DbCommand dbCommand;
			switch (rowUpdatingEvent.StatementType)
			{
			case StatementType.Insert:
				dbCommand = this.BuildInsertCommand(rowUpdatingEvent.TableMapping, row);
				break;
			case StatementType.Update:
				dbCommand = this.BuildUpdateCommand(rowUpdatingEvent.TableMapping, row);
				break;
			case StatementType.Delete:
				dbCommand = this.BuildDeleteCommand(rowUpdatingEvent.TableMapping, row);
				break;
			default:
				throw ADP.InvalidStatementType(rowUpdatingEvent.StatementType);
			}
			if (dbCommand == null)
			{
				if (row != null)
				{
					row.AcceptChanges();
				}
				rowUpdatingEvent.Status = UpdateStatus.SkipCurrentRow;
			}
			rowUpdatingEvent.Command = dbCommand;
		}

		/// <summary>Given a quoted identifier, returns the correct unquoted form of that identifier, including properly un-escaping any embedded quotes in the identifier.</summary>
		/// <param name="quotedIdentifier">The identifier that will have its embedded quotes removed.</param>
		/// <returns>The unquoted identifier, with embedded quotes properly un-escaped.</returns>
		public virtual string UnquoteIdentifier(string quotedIdentifier)
		{
			throw ADP.NotSupported();
		}

		/// <summary>Allows the provider implementation of the <see cref="T:System.Data.Common.DbCommandBuilder" /> class to handle additional parameter properties.</summary>
		/// <param name="parameter">A <see cref="T:System.Data.Common.DbParameter" /> to which the additional modifications are applied.</param>
		/// <param name="row">The <see cref="T:System.Data.DataRow" /> from the schema table provided by <see cref="M:System.Data.Common.DbDataReader.GetSchemaTable" />.</param>
		/// <param name="statementType">The type of command being generated; INSERT, UPDATE or DELETE.</param>
		/// <param name="whereClause">
		///   <see langword="true" /> if the parameter is part of the update or delete WHERE clause, <see langword="false" /> if it is part of the insert or update values.</param>
		protected abstract void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause);

		/// <summary>Returns the name of the specified parameter in the format of @p#. Use when building a custom command builder.</summary>
		/// <param name="parameterOrdinal">The number to be included as part of the parameter's name.</param>
		/// <returns>The name of the parameter with the specified number appended as part of the parameter name.</returns>
		protected abstract string GetParameterName(int parameterOrdinal);

		/// <summary>Returns the full parameter name, given the partial parameter name.</summary>
		/// <param name="parameterName">The partial name of the parameter.</param>
		/// <returns>The full parameter name corresponding to the partial parameter name requested.</returns>
		protected abstract string GetParameterName(string parameterName);

		/// <summary>Returns the placeholder for the parameter in the associated SQL statement.</summary>
		/// <param name="parameterOrdinal">The number to be included as part of the parameter's name.</param>
		/// <returns>The name of the parameter with the specified number appended.</returns>
		protected abstract string GetParameterPlaceholder(int parameterOrdinal);

		/// <summary>Registers the <see cref="T:System.Data.Common.DbCommandBuilder" /> to handle the <see cref="E:System.Data.OleDb.OleDbDataAdapter.RowUpdating" /> event for a <see cref="T:System.Data.Common.DbDataAdapter" />.</summary>
		/// <param name="adapter">The <see cref="T:System.Data.Common.DbDataAdapter" /> to be used for the update.</param>
		protected abstract void SetRowUpdatingHandler(DbDataAdapter adapter);

		private const string DeleteFrom = "DELETE FROM ";

		private const string InsertInto = "INSERT INTO ";

		private const string DefaultValues = " DEFAULT VALUES";

		private const string Values = " VALUES ";

		private const string Update = "UPDATE ";

		private const string Set = " SET ";

		private const string Where = " WHERE ";

		private const string SpaceLeftParenthesis = " (";

		private const string Comma = ", ";

		private const string Equal = " = ";

		private const string LeftParenthesis = "(";

		private const string RightParenthesis = ")";

		private const string NameSeparator = ".";

		private const string IsNull = " IS NULL";

		private const string EqualOne = " = 1";

		private const string And = " AND ";

		private const string Or = " OR ";

		private DbDataAdapter _dataAdapter;

		private DbCommand _insertCommand;

		private DbCommand _updateCommand;

		private DbCommand _deleteCommand;

		private MissingMappingAction _missingMappingAction;

		private ConflictOption _conflictDetection = ConflictOption.CompareAllSearchableValues;

		private bool _setAllValues;

		private bool _hasPartialPrimaryKey;

		private DataTable _dbSchemaTable;

		private DbSchemaRow[] _dbSchemaRows;

		private string[] _sourceColumnNames;

		private DbCommandBuilder.ParameterNames _parameterNames;

		private string _quotedBaseTableName;

		private CatalogLocation _catalogLocation = CatalogLocation.Start;

		private string _catalogSeparator = ".";

		private string _schemaSeparator = ".";

		private string _quotePrefix = string.Empty;

		private string _quoteSuffix = string.Empty;

		private string _parameterNamePattern;

		private string _parameterMarkerFormat;

		private int _parameterNameMaxLength;

		private class ParameterNames
		{
			internal ParameterNames(DbCommandBuilder dbCommandBuilder, DbSchemaRow[] schemaRows)
			{
				this._dbCommandBuilder = dbCommandBuilder;
				this._baseParameterNames = new string[schemaRows.Length];
				this._originalParameterNames = new string[schemaRows.Length];
				this._nullParameterNames = new string[schemaRows.Length];
				this._isMutatedName = new bool[schemaRows.Length];
				this._count = schemaRows.Length;
				this._parameterNameParser = new Regex(this._dbCommandBuilder.ParameterNamePattern, RegexOptions.ExplicitCapture | RegexOptions.Singleline);
				this.SetAndValidateNamePrefixes();
				this._adjustedParameterNameMaxLength = this.GetAdjustedParameterNameMaxLength();
				for (int i = 0; i < schemaRows.Length; i++)
				{
					if (schemaRows[i] != null)
					{
						bool flag = false;
						string text = schemaRows[i].ColumnName;
						if ((this._originalPrefix == null || !text.StartsWith(this._originalPrefix, StringComparison.OrdinalIgnoreCase)) && (this._isNullPrefix == null || !text.StartsWith(this._isNullPrefix, StringComparison.OrdinalIgnoreCase)))
						{
							if (text.IndexOf(' ') >= 0)
							{
								text = text.Replace(' ', '_');
								flag = true;
							}
							if (this._parameterNameParser.IsMatch(text) && text.Length <= this._adjustedParameterNameMaxLength)
							{
								this._baseParameterNames[i] = text;
								this._isMutatedName[i] = flag;
							}
						}
					}
				}
				this.EliminateConflictingNames();
				for (int j = 0; j < schemaRows.Length; j++)
				{
					if (this._baseParameterNames[j] != null)
					{
						if (this._originalPrefix != null)
						{
							this._originalParameterNames[j] = this._originalPrefix + this._baseParameterNames[j];
						}
						if (this._isNullPrefix != null && schemaRows[j].AllowDBNull)
						{
							this._nullParameterNames[j] = this._isNullPrefix + this._baseParameterNames[j];
						}
					}
				}
				this.ApplyProviderSpecificFormat();
				this.GenerateMissingNames(schemaRows);
			}

			private void SetAndValidateNamePrefixes()
			{
				if (this._parameterNameParser.IsMatch("IsNull_"))
				{
					this._isNullPrefix = "IsNull_";
				}
				else if (this._parameterNameParser.IsMatch("isnull"))
				{
					this._isNullPrefix = "isnull";
				}
				else if (this._parameterNameParser.IsMatch("ISNULL"))
				{
					this._isNullPrefix = "ISNULL";
				}
				else
				{
					this._isNullPrefix = null;
				}
				if (this._parameterNameParser.IsMatch("Original_"))
				{
					this._originalPrefix = "Original_";
					return;
				}
				if (this._parameterNameParser.IsMatch("original"))
				{
					this._originalPrefix = "original";
					return;
				}
				if (this._parameterNameParser.IsMatch("ORIGINAL"))
				{
					this._originalPrefix = "ORIGINAL";
					return;
				}
				this._originalPrefix = null;
			}

			private void ApplyProviderSpecificFormat()
			{
				for (int i = 0; i < this._baseParameterNames.Length; i++)
				{
					if (this._baseParameterNames[i] != null)
					{
						this._baseParameterNames[i] = this._dbCommandBuilder.GetParameterName(this._baseParameterNames[i]);
					}
					if (this._originalParameterNames[i] != null)
					{
						this._originalParameterNames[i] = this._dbCommandBuilder.GetParameterName(this._originalParameterNames[i]);
					}
					if (this._nullParameterNames[i] != null)
					{
						this._nullParameterNames[i] = this._dbCommandBuilder.GetParameterName(this._nullParameterNames[i]);
					}
				}
			}

			private void EliminateConflictingNames()
			{
				for (int i = 0; i < this._count - 1; i++)
				{
					string text = this._baseParameterNames[i];
					if (text != null)
					{
						for (int j = i + 1; j < this._count; j++)
						{
							if (ADP.CompareInsensitiveInvariant(text, this._baseParameterNames[j]))
							{
								int num = this._isMutatedName[j] ? j : i;
								this._baseParameterNames[num] = null;
							}
						}
					}
				}
			}

			internal void GenerateMissingNames(DbSchemaRow[] schemaRows)
			{
				for (int i = 0; i < this._baseParameterNames.Length; i++)
				{
					if (this._baseParameterNames[i] == null)
					{
						this._baseParameterNames[i] = this.GetNextGenericParameterName();
						this._originalParameterNames[i] = this.GetNextGenericParameterName();
						if (schemaRows[i] != null && schemaRows[i].AllowDBNull)
						{
							this._nullParameterNames[i] = this.GetNextGenericParameterName();
						}
					}
				}
			}

			private int GetAdjustedParameterNameMaxLength()
			{
				int num = Math.Max((this._isNullPrefix != null) ? this._isNullPrefix.Length : 0, (this._originalPrefix != null) ? this._originalPrefix.Length : 0) + this._dbCommandBuilder.GetParameterName("").Length;
				return this._dbCommandBuilder.ParameterNameMaxLength - num;
			}

			private string GetNextGenericParameterName()
			{
				bool flag;
				string parameterName;
				do
				{
					flag = false;
					this._genericParameterCount++;
					parameterName = this._dbCommandBuilder.GetParameterName(this._genericParameterCount);
					for (int i = 0; i < this._baseParameterNames.Length; i++)
					{
						if (ADP.CompareInsensitiveInvariant(this._baseParameterNames[i], parameterName))
						{
							flag = true;
							break;
						}
					}
				}
				while (flag);
				return parameterName;
			}

			internal string GetBaseParameterName(int index)
			{
				return this._baseParameterNames[index];
			}

			internal string GetOriginalParameterName(int index)
			{
				return this._originalParameterNames[index];
			}

			internal string GetNullParameterName(int index)
			{
				return this._nullParameterNames[index];
			}

			private const string DefaultOriginalPrefix = "Original_";

			private const string DefaultIsNullPrefix = "IsNull_";

			private const string AlternativeOriginalPrefix = "original";

			private const string AlternativeIsNullPrefix = "isnull";

			private const string AlternativeOriginalPrefix2 = "ORIGINAL";

			private const string AlternativeIsNullPrefix2 = "ISNULL";

			private string _originalPrefix;

			private string _isNullPrefix;

			private Regex _parameterNameParser;

			private DbCommandBuilder _dbCommandBuilder;

			private string[] _baseParameterNames;

			private string[] _originalParameterNames;

			private string[] _nullParameterNames;

			private bool[] _isMutatedName;

			private int _count;

			private int _genericParameterCount;

			private int _adjustedParameterNameMaxLength;
		}
	}
}
