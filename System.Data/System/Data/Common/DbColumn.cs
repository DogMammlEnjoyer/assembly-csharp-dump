using System;

namespace System.Data.Common
{
	/// <summary>Represents a column within a data source.</summary>
	public abstract class DbColumn
	{
		/// <summary>Gets a nullable boolean value that indicates whether <see langword="DBNull" /> values are allowed in this column, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether <see langword="DBNull" /> values are allowed in this column, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if <see langword="DBNull" /> values are allowed in this column; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? AllowDBNull { get; protected set; }

		/// <summary>Gets the catalog name associated with the data source; otherwise, <see langword="null" /> if no value is set. Can be set to either the catalog name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The catalog name associated with the data source; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string BaseCatalogName { get; protected set; }

		/// <summary>Gets the base column name; otherwise, <see langword="null" /> if no value is set. Can be set to either the column name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The base column name; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string BaseColumnName { get; protected set; }

		/// <summary>Gets the schema name associated with the data source; otherwise, <see langword="null" /> if no value is set. Can be set to either the schema name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The schema name associated with the data source; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string BaseSchemaName { get; protected set; }

		/// <summary>Gets the server name associated with the column; otherwise, <see langword="null" /> if no value is set. Can be set to either the server name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The server name associated with the column; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string BaseServerName { get; protected set; }

		/// <summary>Gets the table name in the schema; otherwise, <see langword="null" /> if no value is set. Can be set to either the table name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The table name in the schema; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string BaseTableName { get; protected set; }

		/// <summary>Gets the name of the column. Can be set to the column name when overridden in a derived class.</summary>
		/// <returns>The name of the column.</returns>
		public string ColumnName { get; protected set; }

		/// <summary>Gets the column position (ordinal) in the datasource row; otherwise, <see langword="null" /> if no value is set. Can be set to either an <see langword="int32" /> value to specify the column position or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>An <see langword="int32" /> value for column ordinal; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public int? ColumnOrdinal { get; protected set; }

		/// <summary>Gets the column size; otherwise, <see langword="null" /> if no value is set. Can be set to either an <see langword="int32" /> value to specify the column size or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>An <see langword="int32" /> value for column size; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public int? ColumnSize { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column is aliased, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column is aliased, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column is aliased; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsAliased { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether values in this column are automatically incremented, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether values in this column are automatically incremented, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if values in this column are automatically incremented; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsAutoIncrement { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column is an expression, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column is an expression, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column is an expression; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsExpression { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column is hidden, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column is hidden, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column is hidden; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsHidden { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column is an identity, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column is an identity, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column is an identity; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsIdentity { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column is a key, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column is a key, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column is a key; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsKey { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column contains long data, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column contains long data, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column contains long data; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsLong { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether this column is read-only, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether this column is read-only, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if this column is read-only; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsReadOnly { get; protected set; }

		/// <summary>Gets a nullable boolean value that indicates whether a unique constraint applies to this column, or returns <see langword="null" /> if no value is set. Can be set to either <see langword="true" /> or <see langword="false" /> indicating whether a unique constraint applies to this column, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) when overridden in a derived class.</summary>
		/// <returns>Returns <see langword="true" /> if a unique constraint applies to this column; otherwise, <see langword="false" />. If no value is set, returns a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public bool? IsUnique { get; protected set; }

		/// <summary>Gets the numeric precision of the column data; otherwise, <see langword="null" /> if no value is set. Can be set to either an <see langword="int32" /> value to specify the numeric precision of the column data or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>An <see langword="int32" /> value that specifies the precision of the column data, if the data is numeric; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public int? NumericPrecision { get; protected set; }

		/// <summary>Gets a nullable <see langword="int32" /> value that either returns <see langword="null" /> or the numeric scale of the column data. Can be set to either <see langword="null" /> or an <see langword="int32" /> value for the numeric scale of the column data when overridden in a derived class.</summary>
		/// <returns>A null reference (<see langword="Nothing" /> in Visual Basic) if no value is set; otherwise, a <see langword="int32" /> value that specifies the scale of the column data, if the data is numeric.</returns>
		public int? NumericScale { get; protected set; }

		/// <summary>Gets the assembly-qualified name of the <see cref="T:System.Type" /> object that represents the type of data in the column; otherwise, <see langword="null" /> if no value is set. Can be set to either the assembly-qualified name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The assembly-qualified name of the <see cref="T:System.Type" /> object that represents the type of data in the column; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string UdtAssemblyQualifiedName { get; protected set; }

		/// <summary>Gets the type of data stored in the column. Can be set to a <see cref="T:System.Type" /> object that represents the type of data in the column when overridden in a derived class.</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents the type of data the column contains.</returns>
		public Type DataType { get; protected set; }

		/// <summary>Gets the name of the data type; otherwise, <see langword="null" /> if no value is set. Can be set to either the data type name or <see langword="null" /> when overridden in a derived class.</summary>
		/// <returns>The name of the data type; otherwise, a null reference (<see langword="Nothing" /> in Visual Basic) if no value is set.</returns>
		public string DataTypeName { get; protected set; }

		/// <summary>Gets the object based on the column property name.</summary>
		/// <param name="property">The column property name.</param>
		/// <returns>The object based on the column property name.</returns>
		public virtual object this[string property]
		{
			get
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(property);
				if (num <= 2477638934U)
				{
					if (num <= 1067318116U)
					{
						if (num <= 687909556U)
						{
							if (num != 405521230U)
							{
								if (num == 687909556U)
								{
									if (property == "ColumnOrdinal")
									{
										return this.ColumnOrdinal;
									}
								}
							}
							else if (property == "DataTypeName")
							{
								return this.DataTypeName;
							}
						}
						else if (num != 720006947U)
						{
							if (num != 1005639113U)
							{
								if (num == 1067318116U)
								{
									if (property == "ColumnName")
									{
										return this.ColumnName;
									}
								}
							}
							else if (property == "IsHidden")
							{
								return this.IsHidden;
							}
						}
						else if (property == "IsLong")
						{
							return this.IsLong;
						}
					}
					else if (num <= 2215472237U)
					{
						if (num != 1154057342U)
						{
							if (num != 1309233724U)
							{
								if (num == 2215472237U)
								{
									if (property == "DataType")
									{
										return this.DataType;
									}
								}
							}
							else if (property == "IsKey")
							{
								return this.IsKey;
							}
						}
						else if (property == "ColumnSize")
						{
							return this.ColumnSize;
						}
					}
					else if (num != 2239129947U)
					{
						if (num != 2380251540U)
						{
							if (num == 2477638934U)
							{
								if (property == "IsUnique")
								{
									return this.IsUnique;
								}
							}
						}
						else if (property == "NumericPrecision")
						{
							return this.NumericPrecision;
						}
					}
					else if (property == "IsExpression")
					{
						return this.IsExpression;
					}
				}
				else if (num <= 3042527364U)
				{
					if (num <= 2711511624U)
					{
						if (num != 2504653387U)
						{
							if (num != 2586490225U)
							{
								if (num == 2711511624U)
								{
									if (property == "BaseServerName")
									{
										return this.BaseServerName;
									}
								}
							}
							else if (property == "UdtAssemblyQualifiedName")
							{
								return this.UdtAssemblyQualifiedName;
							}
						}
						else if (property == "IsIdentity")
						{
							return this.IsIdentity;
						}
					}
					else if (num != 2741140585U)
					{
						if (num != 2757192823U)
						{
							if (num == 3042527364U)
							{
								if (property == "BaseCatalogName")
								{
									return this.BaseCatalogName;
								}
							}
						}
						else if (property == "BaseTableName")
						{
							return this.BaseTableName;
						}
					}
					else if (property == "BaseColumnName")
					{
						return this.BaseColumnName;
					}
				}
				else if (num <= 3656290791U)
				{
					if (num != 3115085976U)
					{
						if (num != 3173893005U)
						{
							if (num == 3656290791U)
							{
								if (property == "IsReadOnly")
								{
									return this.IsReadOnly;
								}
							}
						}
						else if (property == "AllowDBNull")
						{
							return this.AllowDBNull;
						}
					}
					else if (property == "BaseSchemaName")
					{
						return this.BaseSchemaName;
					}
				}
				else if (num != 3912158903U)
				{
					if (num != 3938522122U)
					{
						if (num == 4233439846U)
						{
							if (property == "IsAliased")
							{
								return this.IsAliased;
							}
						}
					}
					else if (property == "NumericScale")
					{
						return this.NumericScale;
					}
				}
				else if (property == "IsAutoIncrement")
				{
					return this.IsAutoIncrement;
				}
				return null;
			}
		}
	}
}
