using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data
{
	/// <summary>Represents the schema of a column in a <see cref="T:System.Data.DataTable" />.</summary>
	[DesignTimeVisible(false)]
	[DefaultProperty("ColumnName")]
	[ToolboxItem(false)]
	public class DataColumn : MarshalByValueComponent
	{
		/// <summary>Initializes a new instance of a <see cref="T:System.Data.DataColumn" /> class as type string.</summary>
		public DataColumn() : this(null, typeof(string), null, MappingType.Element)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataColumn" /> class, as type string, using the specified column name.</summary>
		/// <param name="columnName">A string that represents the name of the column to be created. If set to <see langword="null" /> or an empty string (""), a default name will be specified when added to the columns collection.</param>
		public DataColumn(string columnName) : this(columnName, typeof(string), null, MappingType.Element)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataColumn" /> class using the specified column name and data type.</summary>
		/// <param name="columnName">A string that represents the name of the column to be created. If set to <see langword="null" /> or an empty string (""), a default name will be specified when added to the columns collection.</param>
		/// <param name="dataType">A supported <see cref="P:System.Data.DataColumn.DataType" />.</param>
		/// <exception cref="T:System.ArgumentNullException">No <paramref name="dataType" /> was specified.</exception>
		public DataColumn(string columnName, Type dataType) : this(columnName, dataType, null, MappingType.Element)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataColumn" /> class using the specified name, data type, and expression.</summary>
		/// <param name="columnName">A string that represents the name of the column to be created. If set to <see langword="null" /> or an empty string (""), a default name will be specified when added to the columns collection.</param>
		/// <param name="dataType">A supported <see cref="P:System.Data.DataColumn.DataType" />.</param>
		/// <param name="expr">The expression used to create this column. For more information, see the <see cref="P:System.Data.DataColumn.Expression" /> property.</param>
		/// <exception cref="T:System.ArgumentNullException">No <paramref name="dataType" /> was specified.</exception>
		public DataColumn(string columnName, Type dataType, string expr) : this(columnName, dataType, expr, MappingType.Element)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataColumn" /> class using the specified name, data type, expression, and value that determines whether the column is an attribute.</summary>
		/// <param name="columnName">A string that represents the name of the column to be created. If set to <see langword="null" /> or an empty string (""), a default name will be specified when added to the columns collection.</param>
		/// <param name="dataType">A supported <see cref="P:System.Data.DataColumn.DataType" />.</param>
		/// <param name="expr">The expression used to create this column. For more information, see the <see cref="P:System.Data.DataColumn.Expression" /> property.</param>
		/// <param name="type">One of the <see cref="T:System.Data.MappingType" /> values.</param>
		/// <exception cref="T:System.ArgumentNullException">No <paramref name="dataType" /> was specified.</exception>
		public DataColumn(string columnName, Type dataType, string expr, MappingType type)
		{
			GC.SuppressFinalize(this);
			DataCommonEventSource.Log.Trace<int, string, string, MappingType>("<ds.DataColumn.DataColumn|API> {0}, columnName='{1}', expr='{2}', type={3}", this.ObjectID, columnName, expr, type);
			if (dataType == null)
			{
				throw ExceptionBuilder.ArgumentNull("dataType");
			}
			StorageType storageType = DataStorage.GetStorageType(dataType);
			if (DataStorage.ImplementsINullableValue(storageType, dataType))
			{
				throw ExceptionBuilder.ColumnTypeNotSupported();
			}
			this._columnName = (columnName ?? string.Empty);
			SimpleType simpleType = SimpleType.CreateSimpleType(storageType, dataType);
			if (simpleType != null)
			{
				this.SimpleType = simpleType;
			}
			this.UpdateColumnType(dataType, storageType);
			if (!string.IsNullOrEmpty(expr))
			{
				this.Expression = expr;
			}
			this._columnMapping = type;
		}

		private void UpdateColumnType(Type type, StorageType typeCode)
		{
			TypeLimiter.EnsureTypeIsAllowed(type, null);
			this._dataType = type;
			this._storageType = typeCode;
			if (StorageType.DateTime != typeCode)
			{
				this._dateTimeMode = DataSetDateTime.UnspecifiedLocal;
			}
			DataStorage.ImplementsInterfaces(typeCode, type, out this._isSqlType, out this._implementsINullable, out this._implementsIXMLSerializable, out this._implementsIChangeTracking, out this._implementsIRevertibleChangeTracking);
			if (!this._isSqlType && this._implementsINullable)
			{
				SqlUdtStorage.GetStaticNullForUdtType(type);
			}
		}

		/// <summary>Gets or sets a value that indicates whether null values are allowed in this column for rows that belong to the table.</summary>
		/// <returns>
		///   <see langword="true" /> if null values values are allowed; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		[DefaultValue(true)]
		public bool AllowDBNull
		{
			get
			{
				return this._allowNull;
			}
			set
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataColumn.set_AllowDBNull|API> {0}, {1}", this.ObjectID, value);
				try
				{
					if (this._allowNull != value)
					{
						if (this._table != null && !value && this._table.EnforceConstraints)
						{
							this.CheckNotAllowNull();
						}
						this._allowNull = value;
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
		}

		/// <summary>Gets or sets a value that indicates whether the column automatically increments the value of the column for new rows added to the table.</summary>
		/// <returns>
		///   <see langword="true" /> if the value of the column increments automatically; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The column is a computed column.</exception>
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool AutoIncrement
		{
			get
			{
				return this._autoInc != null && this._autoInc.Auto;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, bool>("<ds.DataColumn.set_AutoIncrement|API> {0}, {1}", this.ObjectID, value);
				if (this.AutoIncrement != value)
				{
					if (value)
					{
						if (this._expression != null)
						{
							throw ExceptionBuilder.AutoIncrementAndExpression();
						}
						if (!this.DefaultValueIsNull)
						{
							throw ExceptionBuilder.AutoIncrementAndDefaultValue();
						}
						if (!DataColumn.IsAutoIncrementType(this.DataType))
						{
							if (this.HasData)
							{
								throw ExceptionBuilder.AutoIncrementCannotSetIfHasData(this.DataType.Name);
							}
							this.DataType = typeof(int);
						}
					}
					this.AutoInc.Auto = value;
				}
			}
		}

		internal object AutoIncrementCurrent
		{
			get
			{
				if (this._autoInc == null)
				{
					return this.AutoIncrementSeed;
				}
				return this._autoInc.Current;
			}
			set
			{
				if (this.AutoIncrementSeed != BigIntegerStorage.ConvertToBigInteger(value, this.FormatProvider))
				{
					this.AutoInc.SetCurrent(value, this.FormatProvider);
				}
			}
		}

		internal AutoIncrementValue AutoInc
		{
			get
			{
				AutoIncrementValue result;
				if ((result = this._autoInc) == null)
				{
					result = (this._autoInc = ((this.DataType == typeof(BigInteger)) ? new AutoIncrementBigInteger() : new AutoIncrementInt64()));
				}
				return result;
			}
		}

		/// <summary>Gets or sets the starting value for a column that has its <see cref="P:System.Data.DataColumn.AutoIncrement" /> property set to <see langword="true" />. The default is 0.</summary>
		/// <returns>The starting value for the <see cref="P:System.Data.DataColumn.AutoIncrement" /> feature.</returns>
		[DefaultValue(0L)]
		public long AutoIncrementSeed
		{
			get
			{
				if (this._autoInc == null)
				{
					return 0L;
				}
				return this._autoInc.Seed;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, long>("<ds.DataColumn.set_AutoIncrementSeed|API> {0}, {1}", this.ObjectID, value);
				if (this.AutoIncrementSeed != value)
				{
					this.AutoInc.Seed = value;
				}
			}
		}

		/// <summary>Gets or sets the increment used by a column with its <see cref="P:System.Data.DataColumn.AutoIncrement" /> property set to <see langword="true" />.</summary>
		/// <returns>The number by which the value of the column is automatically incremented. The default is 1.</returns>
		/// <exception cref="T:System.ArgumentException">The value set is zero.</exception>
		[DefaultValue(1L)]
		public long AutoIncrementStep
		{
			get
			{
				if (this._autoInc == null)
				{
					return 1L;
				}
				return this._autoInc.Step;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, long>("<ds.DataColumn.set_AutoIncrementStep|API> {0}, {1}", this.ObjectID, value);
				if (this.AutoIncrementStep != value)
				{
					this.AutoInc.Step = value;
				}
			}
		}

		/// <summary>Gets or sets the caption for the column.</summary>
		/// <returns>The caption of the column. If not set, returns the <see cref="P:System.Data.DataColumn.ColumnName" /> value.</returns>
		public string Caption
		{
			get
			{
				if (this._caption == null)
				{
					return this._columnName;
				}
				return this._caption;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this._caption == null || string.Compare(this._caption, value, true, this.Locale) != 0)
				{
					this._caption = value;
				}
			}
		}

		private void ResetCaption()
		{
			if (this._caption != null)
			{
				this._caption = null;
			}
		}

		private bool ShouldSerializeCaption()
		{
			return this._caption != null;
		}

		/// <summary>Gets or sets the name of the column in the <see cref="T:System.Data.DataColumnCollection" />.</summary>
		/// <returns>The name of the column.</returns>
		/// <exception cref="T:System.ArgumentException">The property is set to <see langword="null" /> or an empty string and the column belongs to a collection.</exception>
		/// <exception cref="T:System.Data.DuplicateNameException">A column with the same name already exists in the collection. The name comparison is not case sensitive.</exception>
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public string ColumnName
		{
			get
			{
				return this._columnName;
			}
			set
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int, string>("<ds.DataColumn.set_ColumnName|API> {0}, '{1}'", this.ObjectID, value);
				try
				{
					if (value == null)
					{
						value = string.Empty;
					}
					if (string.Compare(this._columnName, value, true, this.Locale) != 0)
					{
						if (this._table != null)
						{
							if (value.Length == 0)
							{
								throw ExceptionBuilder.ColumnNameRequired();
							}
							this._table.Columns.RegisterColumnName(value, this);
							if (this._columnName.Length != 0)
							{
								this._table.Columns.UnregisterName(this._columnName);
							}
						}
						this.RaisePropertyChanging("ColumnName");
						this._columnName = value;
						this._encodedColumnName = null;
						if (this._table != null)
						{
							this._table.Columns.OnColumnPropertyChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
						}
					}
					else if (this._columnName != value)
					{
						this.RaisePropertyChanging("ColumnName");
						this._columnName = value;
						this._encodedColumnName = null;
						if (this._table != null)
						{
							this._table.Columns.OnColumnPropertyChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
						}
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
		}

		internal string EncodedColumnName
		{
			get
			{
				if (this._encodedColumnName == null)
				{
					this._encodedColumnName = XmlConvert.EncodeLocalName(this.ColumnName);
				}
				return this._encodedColumnName;
			}
		}

		internal IFormatProvider FormatProvider
		{
			get
			{
				if (this._table == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return this._table.FormatProvider;
			}
		}

		internal CultureInfo Locale
		{
			get
			{
				if (this._table == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return this._table.Locale;
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		/// <summary>Gets or sets an XML prefix that aliases the namespace of the <see cref="T:System.Data.DataTable" />.</summary>
		/// <returns>The XML prefix for the <see cref="T:System.Data.DataTable" /> namespace.</returns>
		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				return this._columnPrefix;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataColumn.set_Prefix|API> {0}, '{1}'", this.ObjectID, value);
				if (XmlConvert.DecodeName(value) == value && XmlConvert.EncodeName(value) != value)
				{
					throw ExceptionBuilder.InvalidPrefix(value);
				}
				this._columnPrefix = value;
			}
		}

		internal string GetColumnValueAsString(DataRow row, DataRowVersion version)
		{
			object value = this[row.GetRecordFromVersion(version)];
			if (DataStorage.IsObjectNull(value))
			{
				return null;
			}
			return this.ConvertObjectToXml(value);
		}

		internal bool Computed
		{
			get
			{
				return this._expression != null;
			}
		}

		internal DataExpression DataExpression
		{
			get
			{
				return this._expression;
			}
		}

		/// <summary>Gets or sets the type of data stored in the column.</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents the column data type.</returns>
		/// <exception cref="T:System.ArgumentException">The column already has data stored.</exception>
		[DefaultValue(typeof(string))]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ColumnTypeConverter))]
		public Type DataType
		{
			get
			{
				return this._dataType;
			}
			set
			{
				if (this._dataType != value)
				{
					if (this.HasData)
					{
						throw ExceptionBuilder.CantChangeDataType();
					}
					if (value == null)
					{
						throw ExceptionBuilder.NullDataType();
					}
					StorageType storageType = DataStorage.GetStorageType(value);
					if (DataStorage.ImplementsINullableValue(storageType, value))
					{
						throw ExceptionBuilder.ColumnTypeNotSupported();
					}
					if (this._table != null && this.IsInRelation())
					{
						throw ExceptionBuilder.ColumnsTypeMismatch();
					}
					if (storageType == StorageType.BigInteger && this._expression != null)
					{
						throw ExprException.UnsupportedDataType(value);
					}
					if (!this.DefaultValueIsNull)
					{
						try
						{
							if (this._defaultValue is BigInteger)
							{
								this._defaultValue = BigIntegerStorage.ConvertFromBigInteger((BigInteger)this._defaultValue, value, this.FormatProvider);
							}
							else if (typeof(BigInteger) == value)
							{
								this._defaultValue = BigIntegerStorage.ConvertToBigInteger(this._defaultValue, this.FormatProvider);
							}
							else if (typeof(string) == value)
							{
								this._defaultValue = this.DefaultValue.ToString();
							}
							else if (typeof(SqlString) == value)
							{
								this._defaultValue = SqlConvert.ConvertToSqlString(this.DefaultValue);
							}
							else if (typeof(object) != value)
							{
								this.DefaultValue = SqlConvert.ChangeTypeForDefaultValue(this.DefaultValue, value, this.FormatProvider);
							}
						}
						catch (InvalidCastException inner)
						{
							throw ExceptionBuilder.DefaultValueDataType(this.ColumnName, this.DefaultValue.GetType(), value, inner);
						}
						catch (FormatException inner2)
						{
							throw ExceptionBuilder.DefaultValueDataType(this.ColumnName, this.DefaultValue.GetType(), value, inner2);
						}
					}
					if (this.ColumnMapping == MappingType.SimpleContent && value == typeof(char))
					{
						throw ExceptionBuilder.CannotSetSimpleContentType(this.ColumnName, value);
					}
					this.SimpleType = SimpleType.CreateSimpleType(storageType, value);
					if (StorageType.String == storageType)
					{
						this._maxLength = -1;
					}
					this.UpdateColumnType(value, storageType);
					this.XmlDataType = null;
					if (this.AutoIncrement)
					{
						if (!DataColumn.IsAutoIncrementType(value))
						{
							this.AutoIncrement = false;
						}
						if (this._autoInc != null)
						{
							AutoIncrementValue autoInc = this._autoInc;
							this._autoInc = null;
							this.AutoInc.Auto = autoInc.Auto;
							this.AutoInc.Seed = autoInc.Seed;
							this.AutoInc.Step = autoInc.Step;
							if (this._autoInc.DataType == autoInc.DataType)
							{
								this._autoInc.Current = autoInc.Current;
								return;
							}
							if (autoInc.DataType == typeof(long))
							{
								this.AutoInc.Current = (long)autoInc.Current;
								return;
							}
							this.AutoInc.Current = (long)((BigInteger)autoInc.Current);
						}
					}
				}
			}
		}

		/// <summary>Gets or sets the <see langword="DateTimeMode" /> for the column.</summary>
		/// <returns>The <see cref="T:System.Data.DataSetDateTime" /> for the specified column.</returns>
		[DefaultValue(DataSetDateTime.UnspecifiedLocal)]
		[RefreshProperties(RefreshProperties.All)]
		public DataSetDateTime DateTimeMode
		{
			get
			{
				return this._dateTimeMode;
			}
			set
			{
				if (this._dateTimeMode != value)
				{
					if (this.DataType != typeof(DateTime) && value != DataSetDateTime.UnspecifiedLocal)
					{
						throw ExceptionBuilder.CannotSetDateTimeModeForNonDateTimeColumns();
					}
					switch (value)
					{
					case DataSetDateTime.Local:
					case DataSetDateTime.Utc:
						if (this.HasData)
						{
							throw ExceptionBuilder.CantChangeDateTimeMode(this._dateTimeMode, value);
						}
						break;
					case DataSetDateTime.Unspecified:
					case DataSetDateTime.UnspecifiedLocal:
						if (this._dateTimeMode != DataSetDateTime.Unspecified && this._dateTimeMode != DataSetDateTime.UnspecifiedLocal && this.HasData)
						{
							throw ExceptionBuilder.CantChangeDateTimeMode(this._dateTimeMode, value);
						}
						break;
					default:
						throw ExceptionBuilder.InvalidDateTimeMode(value);
					}
					this._dateTimeMode = value;
				}
			}
		}

		/// <summary>Gets or sets the default value for the column when you are creating new rows.</summary>
		/// <returns>A value appropriate to the column's <see cref="P:System.Data.DataColumn.DataType" />.</returns>
		/// <exception cref="T:System.InvalidCastException">When you are adding a row, the default value is not an instance of the column's data type.</exception>
		[TypeConverter(typeof(DefaultValueTypeConverter))]
		public object DefaultValue
		{
			get
			{
				if (this._defaultValue == DBNull.Value && this._implementsINullable)
				{
					if (this._storage != null)
					{
						this._defaultValue = this._storage._nullValue;
					}
					else if (this._isSqlType)
					{
						this._defaultValue = SqlConvert.ChangeTypeForDefaultValue(this._defaultValue, this._dataType, this.FormatProvider);
					}
					else if (this._implementsINullable)
					{
						PropertyInfo property = this._dataType.GetProperty("Null", BindingFlags.Static | BindingFlags.Public);
						if (property != null)
						{
							this._defaultValue = property.GetValue(null, null);
						}
					}
				}
				return this._defaultValue;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataColumn.set_DefaultValue|API> {0}", this.ObjectID);
				if (this._defaultValue == null || !this.DefaultValue.Equals(value))
				{
					if (this.AutoIncrement)
					{
						throw ExceptionBuilder.DefaultValueAndAutoIncrement();
					}
					object obj = (value == null) ? DBNull.Value : value;
					if (obj != DBNull.Value && this.DataType != typeof(object))
					{
						try
						{
							obj = SqlConvert.ChangeTypeForDefaultValue(obj, this.DataType, this.FormatProvider);
						}
						catch (InvalidCastException inner)
						{
							throw ExceptionBuilder.DefaultValueColumnDataType(this.ColumnName, obj.GetType(), this.DataType, inner);
						}
					}
					this._defaultValue = obj;
					this._defaultValueIsNull = (obj == DBNull.Value || (this.ImplementsINullable && DataStorage.IsObjectSqlNull(obj)));
				}
			}
		}

		internal bool DefaultValueIsNull
		{
			get
			{
				return this._defaultValueIsNull;
			}
		}

		internal void BindExpression()
		{
			this.DataExpression.Bind(this._table);
		}

		/// <summary>Gets or sets the expression used to filter rows, calculate the values in a column, or create an aggregate column.</summary>
		/// <returns>An expression to calculate the value of a column, or create an aggregate column. The return type of an expression is determined by the <see cref="P:System.Data.DataColumn.DataType" /> of the column.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Data.DataColumn.AutoIncrement" /> or <see cref="P:System.Data.DataColumn.Unique" /> property is set to <see langword="true" />.</exception>
		/// <exception cref="T:System.FormatException">When you are using the CONVERT function, the expression evaluates to a string, but the string does not contain a representation that can be converted to the type parameter.</exception>
		/// <exception cref="T:System.InvalidCastException">When you are using the CONVERT function, the requested cast is not possible. See the Conversion function in the following section for detailed information about possible casts.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">When you use the SUBSTRING function, the start argument is out of range.  
		///  -Or-  
		///  When you use the SUBSTRING function, the length argument is out of range.</exception>
		/// <exception cref="T:System.Exception">When you use the LEN function or the TRIM function, the expression does not evaluate to a string. This includes expressions that evaluate to <see cref="T:System.Char" />.</exception>
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public string Expression
		{
			get
			{
				if (this._expression != null)
				{
					return this._expression.Expression;
				}
				return "";
			}
			set
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int, string>("<ds.DataColumn.set_Expression|API> {0}, '{1}'", this.ObjectID, value);
				if (value == null)
				{
					value = string.Empty;
				}
				try
				{
					DataExpression dataExpression = null;
					if (value.Length > 0)
					{
						DataExpression dataExpression2 = new DataExpression(this._table, value, this._dataType);
						if (dataExpression2.HasValue)
						{
							dataExpression = dataExpression2;
						}
					}
					if (this._expression == null && dataExpression != null)
					{
						if (this.AutoIncrement || this.Unique)
						{
							throw ExceptionBuilder.ExpressionAndUnique();
						}
						if (this._table != null)
						{
							for (int i = 0; i < this._table.Constraints.Count; i++)
							{
								if (this._table.Constraints[i].ContainsColumn(this))
								{
									throw ExceptionBuilder.ExpressionAndConstraint(this, this._table.Constraints[i]);
								}
							}
						}
						bool readOnly = this.ReadOnly;
						try
						{
							this.ReadOnly = true;
						}
						catch (ReadOnlyException e)
						{
							ExceptionBuilder.TraceExceptionForCapture(e);
							this.ReadOnly = readOnly;
							throw ExceptionBuilder.ExpressionAndReadOnly();
						}
					}
					if (this._table != null)
					{
						if (dataExpression != null && dataExpression.DependsOn(this))
						{
							throw ExceptionBuilder.ExpressionCircular();
						}
						this.HandleDependentColumnList(this._expression, dataExpression);
						DataExpression expression = this._expression;
						this._expression = dataExpression;
						try
						{
							if (dataExpression == null)
							{
								for (int j = 0; j < this._table.RecordCapacity; j++)
								{
									this.InitializeRecord(j);
								}
							}
							else
							{
								this._table.EvaluateExpressions(this);
							}
							this._table.ResetInternalIndexes(this);
							this._table.EvaluateDependentExpressions(this);
							return;
						}
						catch (Exception e2) when (ADP.IsCatchableExceptionType(e2))
						{
							ExceptionBuilder.TraceExceptionForCapture(e2);
							try
							{
								this._expression = expression;
								this.HandleDependentColumnList(dataExpression, this._expression);
								if (expression == null)
								{
									for (int k = 0; k < this._table.RecordCapacity; k++)
									{
										this.InitializeRecord(k);
									}
								}
								else
								{
									this._table.EvaluateExpressions(this);
								}
								this._table.ResetInternalIndexes(this);
								this._table.EvaluateDependentExpressions(this);
							}
							catch (Exception e3) when (ADP.IsCatchableExceptionType(e3))
							{
								ExceptionBuilder.TraceExceptionWithoutRethrow(e3);
							}
							throw;
						}
					}
					this._expression = dataExpression;
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
		}

		/// <summary>Gets the collection of custom user information associated with a <see cref="T:System.Data.DataColumn" />.</summary>
		/// <returns>A <see cref="T:System.Data.PropertyCollection" /> of custom information.</returns>
		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				PropertyCollection result;
				if ((result = this._extendedProperties) == null)
				{
					result = (this._extendedProperties = new PropertyCollection());
				}
				return result;
			}
		}

		internal bool HasData
		{
			get
			{
				return this._storage != null;
			}
		}

		internal bool ImplementsINullable
		{
			get
			{
				return this._implementsINullable;
			}
		}

		internal bool ImplementsIChangeTracking
		{
			get
			{
				return this._implementsIChangeTracking;
			}
		}

		internal bool ImplementsIRevertibleChangeTracking
		{
			get
			{
				return this._implementsIRevertibleChangeTracking;
			}
		}

		internal bool IsCloneable
		{
			get
			{
				return this._storage._isCloneable;
			}
		}

		internal bool IsStringType
		{
			get
			{
				return this._storage._isStringType;
			}
		}

		internal bool IsValueType
		{
			get
			{
				return this._storage._isValueType;
			}
		}

		internal bool IsSqlType
		{
			get
			{
				return this._isSqlType;
			}
		}

		private void SetMaxLengthSimpleType()
		{
			if (this._simpleType != null)
			{
				this._simpleType.MaxLength = this._maxLength;
				if (this._simpleType.IsPlainString())
				{
					this._simpleType = null;
					return;
				}
				if (this._simpleType.Name != null && this.XmlDataType != null)
				{
					this._simpleType.ConvertToAnnonymousSimpleType();
					this.XmlDataType = null;
					return;
				}
			}
			else if (-1 < this._maxLength)
			{
				this.SimpleType = SimpleType.CreateLimitedStringType(this._maxLength);
			}
		}

		/// <summary>Gets or sets the maximum length of a text column.</summary>
		/// <returns>The maximum length of the column in characters. If the column has no maximum length, the value is -1 (default).</returns>
		[DefaultValue(-1)]
		public int MaxLength
		{
			get
			{
				return this._maxLength;
			}
			set
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataColumn.set_MaxLength|API> {0}, {1}", this.ObjectID, value);
				try
				{
					if (this._maxLength != value)
					{
						if (this.ColumnMapping == MappingType.SimpleContent)
						{
							throw ExceptionBuilder.CannotSetMaxLength2(this);
						}
						if (this.DataType != typeof(string) && this.DataType != typeof(SqlString))
						{
							throw ExceptionBuilder.HasToBeStringType(this);
						}
						int maxLength = this._maxLength;
						this._maxLength = Math.Max(value, -1);
						if ((maxLength < 0 || value < maxLength) && this._table != null && this._table.EnforceConstraints && !this.CheckMaxLength())
						{
							this._maxLength = maxLength;
							throw ExceptionBuilder.CannotSetMaxLength(this, value);
						}
						this.SetMaxLengthSimpleType();
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
		}

		/// <summary>Gets or sets the namespace of the <see cref="T:System.Data.DataColumn" />.</summary>
		/// <returns>The namespace of the <see cref="T:System.Data.DataColumn" />.</returns>
		/// <exception cref="T:System.ArgumentException">The namespace already has data.</exception>
		public string Namespace
		{
			get
			{
				if (this._columnUri != null)
				{
					return this._columnUri;
				}
				if (this.Table != null && this._columnMapping != MappingType.Attribute)
				{
					return this.Table.Namespace;
				}
				return string.Empty;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataColumn.set_Namespace|API> {0}, '{1}'", this.ObjectID, value);
				if (this._columnUri != value)
				{
					if (this._columnMapping != MappingType.SimpleContent)
					{
						this.RaisePropertyChanging("Namespace");
						this._columnUri = value;
						return;
					}
					if (value != this.Namespace)
					{
						throw ExceptionBuilder.CannotChangeNamespace(this.ColumnName);
					}
				}
			}
		}

		private bool ShouldSerializeNamespace()
		{
			return this._columnUri != null;
		}

		private void ResetNamespace()
		{
			this.Namespace = null;
		}

		/// <summary>Gets the (zero-based) position of the column in the <see cref="T:System.Data.DataColumnCollection" /> collection.</summary>
		/// <returns>The position of the column. Gets -1 if the column is not a member of a collection.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Ordinal
		{
			get
			{
				return this._ordinal;
			}
		}

		/// <summary>Changes the ordinal or position of the <see cref="T:System.Data.DataColumn" /> to the specified ordinal or position.</summary>
		/// <param name="ordinal">The specified ordinal.</param>
		public void SetOrdinal(int ordinal)
		{
			if (this._ordinal == -1)
			{
				throw ExceptionBuilder.ColumnNotInAnyTable();
			}
			if (this._ordinal != ordinal)
			{
				this._table.Columns.MoveTo(this, ordinal);
			}
		}

		internal void SetOrdinalInternal(int ordinal)
		{
			if (this._ordinal != ordinal)
			{
				if (this.Unique && this._ordinal != -1 && ordinal == -1)
				{
					UniqueConstraint uniqueConstraint = this._table.Constraints.FindKeyConstraint(this);
					if (uniqueConstraint != null)
					{
						this._table.Constraints.Remove(uniqueConstraint);
					}
				}
				if (this._sortIndex != null && -1 == ordinal)
				{
					this._sortIndex.RemoveRef();
					this._sortIndex.RemoveRef();
					this._sortIndex = null;
				}
				int ordinal2 = this._ordinal;
				this._ordinal = ordinal;
				if (ordinal2 == -1 && this._ordinal != -1 && this.Unique)
				{
					UniqueConstraint constraint = new UniqueConstraint(this);
					this._table.Constraints.Add(constraint);
				}
			}
		}

		/// <summary>Gets or sets a value that indicates whether the column allows for changes as soon as a row has been added to the table.</summary>
		/// <returns>
		///   <see langword="true" /> if the column is read only; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The property is set to <see langword="false" /> on a computed column.</exception>
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get
			{
				return this._readOnly;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, bool>("<ds.DataColumn.set_ReadOnly|API> {0}, {1}", this.ObjectID, value);
				if (this._readOnly != value)
				{
					if (!value && this._expression != null)
					{
						throw ExceptionBuilder.ReadOnlyAndExpression();
					}
					this._readOnly = value;
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Index SortIndex
		{
			get
			{
				if (this._sortIndex == null)
				{
					IndexField[] indexDesc = new IndexField[]
					{
						new IndexField(this, false)
					};
					this._sortIndex = this._table.GetIndex(indexDesc, DataViewRowState.CurrentRows, null);
					this._sortIndex.AddRef();
				}
				return this._sortIndex;
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataTable" /> to which the column belongs to.</summary>
		/// <returns>The <see cref="T:System.Data.DataTable" /> that the <see cref="T:System.Data.DataColumn" /> belongs to.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		internal void SetTable(DataTable table)
		{
			if (this._table != table)
			{
				if (this.Computed && (table == null || (!table.fInitInProgress && (table.DataSet == null || (!table.DataSet._fIsSchemaLoading && !table.DataSet._fInitInProgress)))))
				{
					this.DataExpression.Bind(table);
				}
				if (this.Unique && this._table != null)
				{
					UniqueConstraint uniqueConstraint = table.Constraints.FindKeyConstraint(this);
					if (uniqueConstraint != null)
					{
						table.Constraints.CanRemove(uniqueConstraint, true);
					}
				}
				this._table = table;
				this._storage = null;
			}
		}

		private DataRow GetDataRow(int index)
		{
			return this._table._recordManager[index];
		}

		internal object this[int record]
		{
			get
			{
				return this._storage.Get(record);
			}
			set
			{
				try
				{
					this._storage.Set(record, value);
				}
				catch (Exception ex)
				{
					ExceptionBuilder.TraceExceptionForCapture(ex);
					throw ExceptionBuilder.SetFailed(value, this, this.DataType, ex);
				}
				if (this.AutoIncrement && !this._storage.IsNull(record))
				{
					this.AutoInc.SetCurrentAndIncrement(this._storage.Get(record));
				}
				if (this.Computed)
				{
					DataRow dataRow = this.GetDataRow(record);
					if (dataRow != null)
					{
						dataRow.LastChangedColumn = this;
					}
				}
			}
		}

		internal void InitializeRecord(int record)
		{
			this._storage.Set(record, this.DefaultValue);
		}

		internal void SetValue(int record, object value)
		{
			try
			{
				this._storage.Set(record, value);
			}
			catch (Exception ex)
			{
				ExceptionBuilder.TraceExceptionForCapture(ex);
				throw ExceptionBuilder.SetFailed(value, this, this.DataType, ex);
			}
			DataRow dataRow = this.GetDataRow(record);
			if (dataRow != null)
			{
				dataRow.LastChangedColumn = this;
			}
		}

		internal void FreeRecord(int record)
		{
			this._storage.Set(record, this._storage._nullValue);
		}

		/// <summary>Gets or sets a value that indicates whether the values in each row of the column must be unique.</summary>
		/// <returns>
		///   <see langword="true" /> if the value must be unique; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The column is a calculated column.</exception>
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Unique
		{
			get
			{
				return this._unique;
			}
			set
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataColumn.set_Unique|API> {0}, {1}", this.ObjectID, value);
				try
				{
					if (this._unique != value)
					{
						if (value && this._expression != null)
						{
							throw ExceptionBuilder.UniqueAndExpression();
						}
						UniqueConstraint constraint = null;
						if (this._table != null)
						{
							if (value)
							{
								this.CheckUnique();
							}
							else
							{
								foreach (object obj in this.Table.Constraints)
								{
									UniqueConstraint uniqueConstraint = obj as UniqueConstraint;
									if (uniqueConstraint != null && uniqueConstraint.ColumnsReference.Length == 1 && uniqueConstraint.ColumnsReference[0] == this)
									{
										constraint = uniqueConstraint;
									}
								}
								this._table.Constraints.CanRemove(constraint, true);
							}
						}
						this._unique = value;
						if (this._table != null)
						{
							if (value)
							{
								UniqueConstraint constraint2 = new UniqueConstraint(this);
								this._table.Constraints.Add(constraint2);
							}
							else
							{
								this._table.Constraints.Remove(constraint);
							}
						}
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
		}

		internal void InternalUnique(bool value)
		{
			this._unique = value;
		}

		internal string XmlDataType { get; set; } = string.Empty;

		internal SimpleType SimpleType
		{
			get
			{
				return this._simpleType;
			}
			set
			{
				this._simpleType = value;
				if (value != null && value.CanHaveMaxLength())
				{
					this._maxLength = this._simpleType.MaxLength;
				}
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Data.MappingType" /> of the column.</summary>
		/// <returns>One of the <see cref="T:System.Data.MappingType" /> values.</returns>
		[DefaultValue(MappingType.Element)]
		public virtual MappingType ColumnMapping
		{
			get
			{
				return this._columnMapping;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, MappingType>("<ds.DataColumn.set_ColumnMapping|API> {0}, {1}", this.ObjectID, value);
				if (value != this._columnMapping)
				{
					if (value == MappingType.SimpleContent && this._table != null)
					{
						int num = 0;
						if (this._columnMapping == MappingType.Element)
						{
							num = 1;
						}
						if (this._dataType == typeof(char))
						{
							throw ExceptionBuilder.CannotSetSimpleContent(this.ColumnName, this._dataType);
						}
						if (this._table.XmlText != null && this._table.XmlText != this)
						{
							throw ExceptionBuilder.CannotAddColumn3();
						}
						if (this._table.ElementColumnCount > num)
						{
							throw ExceptionBuilder.CannotAddColumn4(this.ColumnName);
						}
					}
					this.RaisePropertyChanging("ColumnMapping");
					if (this._table != null)
					{
						if (this._columnMapping == MappingType.SimpleContent)
						{
							this._table._xmlText = null;
						}
						if (value == MappingType.Element)
						{
							DataTable table = this._table;
							int elementColumnCount = table.ElementColumnCount;
							table.ElementColumnCount = elementColumnCount + 1;
						}
						else if (this._columnMapping == MappingType.Element)
						{
							DataTable table2 = this._table;
							int elementColumnCount = table2.ElementColumnCount;
							table2.ElementColumnCount = elementColumnCount - 1;
						}
					}
					this._columnMapping = value;
					if (value == MappingType.SimpleContent)
					{
						this._columnUri = null;
						if (this._table != null)
						{
							this._table.XmlText = this;
						}
						this.SimpleType = null;
					}
				}
			}
		}

		internal event PropertyChangedEventHandler PropertyChanging;

		internal void CheckColumnConstraint(DataRow row, DataRowAction action)
		{
			if (this._table.UpdatingCurrent(row, action))
			{
				this.CheckNullable(row);
				this.CheckMaxLength(row);
			}
		}

		internal bool CheckMaxLength()
		{
			if (0 <= this._maxLength && this.Table != null && 0 < this.Table.Rows.Count)
			{
				foreach (object obj in this.Table.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					if (dataRow.HasVersion(DataRowVersion.Current) && this._maxLength < this.GetStringLength(dataRow.GetCurrentRecordNo()))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		internal void CheckMaxLength(DataRow dr)
		{
			if (0 <= this._maxLength && this._maxLength < this.GetStringLength(dr.GetDefaultRecord()))
			{
				throw ExceptionBuilder.LongerThanMaxLength(this);
			}
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		protected internal void CheckNotAllowNull()
		{
			if (this._storage == null)
			{
				return;
			}
			if (this._sortIndex != null)
			{
				if (this._sortIndex.IsKeyInIndex(this._storage._nullValue))
				{
					throw ExceptionBuilder.NullKeyValues(this.ColumnName);
				}
			}
			else
			{
				foreach (object obj in this._table.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					if (dataRow.RowState != DataRowState.Deleted)
					{
						if (!this._implementsINullable)
						{
							if (dataRow[this] == DBNull.Value)
							{
								throw ExceptionBuilder.NullKeyValues(this.ColumnName);
							}
						}
						else if (DataStorage.IsObjectNull(dataRow[this]))
						{
							throw ExceptionBuilder.NullKeyValues(this.ColumnName);
						}
					}
				}
			}
		}

		internal void CheckNullable(DataRow row)
		{
			if (!this.AllowDBNull && this._storage.IsNull(row.GetDefaultRecord()))
			{
				throw ExceptionBuilder.NullValues(this.ColumnName);
			}
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		protected void CheckUnique()
		{
			if (!this.SortIndex.CheckUnique())
			{
				throw ExceptionBuilder.NonUniqueValues(this.ColumnName);
			}
		}

		internal int Compare(int record1, int record2)
		{
			return this._storage.Compare(record1, record2);
		}

		internal bool CompareValueTo(int record1, object value, bool checkType)
		{
			if (this.CompareValueTo(record1, value) == 0)
			{
				Type type = value.GetType();
				Type type2 = this._storage.Get(record1).GetType();
				if (type == typeof(string) && type2 == typeof(string))
				{
					return string.CompareOrdinal((string)this._storage.Get(record1), (string)value) == 0;
				}
				if (type == type2)
				{
					return true;
				}
			}
			return false;
		}

		internal int CompareValueTo(int record1, object value)
		{
			return this._storage.CompareValueTo(record1, value);
		}

		internal object ConvertValue(object value)
		{
			return this._storage.ConvertValue(value);
		}

		internal void Copy(int srcRecordNo, int dstRecordNo)
		{
			this._storage.Copy(srcRecordNo, dstRecordNo);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal DataColumn Clone()
		{
			DataColumn dataColumn = (DataColumn)Activator.CreateInstance(base.GetType());
			dataColumn.SimpleType = this.SimpleType;
			dataColumn._allowNull = this._allowNull;
			if (this._autoInc != null)
			{
				dataColumn._autoInc = this._autoInc.Clone();
			}
			dataColumn._caption = this._caption;
			dataColumn.ColumnName = this.ColumnName;
			dataColumn._columnUri = this._columnUri;
			dataColumn._columnPrefix = this._columnPrefix;
			dataColumn.DataType = this.DataType;
			dataColumn._defaultValue = this._defaultValue;
			dataColumn._defaultValueIsNull = (this._defaultValue == DBNull.Value || (dataColumn.ImplementsINullable && DataStorage.IsObjectSqlNull(this._defaultValue)));
			dataColumn._columnMapping = this._columnMapping;
			dataColumn._readOnly = this._readOnly;
			dataColumn.MaxLength = this.MaxLength;
			dataColumn.XmlDataType = this.XmlDataType;
			dataColumn._dateTimeMode = this._dateTimeMode;
			if (this._extendedProperties != null)
			{
				foreach (object key in this._extendedProperties.Keys)
				{
					dataColumn.ExtendedProperties[key] = this._extendedProperties[key];
				}
			}
			return dataColumn;
		}

		internal DataRelation FindParentRelation()
		{
			DataRelation[] array = new DataRelation[this.Table.ParentRelations.Count];
			this.Table.ParentRelations.CopyTo(array, 0);
			foreach (DataRelation dataRelation in array)
			{
				DataKey childKey = dataRelation.ChildKey;
				if (childKey.ColumnsReference.Length == 1 && childKey.ColumnsReference[0] == this)
				{
					return dataRelation;
				}
			}
			return null;
		}

		internal object GetAggregateValue(int[] records, AggregateType kind)
		{
			if (this._storage != null)
			{
				return this._storage.Aggregate(records, kind);
			}
			if (kind != AggregateType.Count)
			{
				return DBNull.Value;
			}
			return 0;
		}

		private int GetStringLength(int record)
		{
			return this._storage.GetStringLength(record);
		}

		internal void Init(int record)
		{
			if (this.AutoIncrement)
			{
				object value = this._autoInc.Current;
				this._autoInc.MoveAfter();
				this._storage.Set(record, value);
				return;
			}
			this[record] = this._defaultValue;
		}

		internal static bool IsAutoIncrementType(Type dataType)
		{
			return dataType == typeof(int) || dataType == typeof(long) || dataType == typeof(short) || dataType == typeof(decimal) || dataType == typeof(BigInteger) || dataType == typeof(SqlInt32) || dataType == typeof(SqlInt64) || dataType == typeof(SqlInt16) || dataType == typeof(SqlDecimal);
		}

		private bool IsColumnMappingValid(StorageType typeCode, MappingType mapping)
		{
			return mapping == MappingType.Element || !DataStorage.IsTypeCustomType(typeCode);
		}

		internal bool IsCustomType
		{
			get
			{
				if (this._storage == null)
				{
					return DataStorage.IsTypeCustomType(this.DataType);
				}
				return this._storage._isCustomDefinedType;
			}
		}

		internal bool IsValueCustomTypeInstance(object value)
		{
			return DataStorage.IsTypeCustomType(value.GetType()) && !(value is Type);
		}

		internal bool ImplementsIXMLSerializable
		{
			get
			{
				return this._implementsIXMLSerializable;
			}
		}

		internal bool IsNull(int record)
		{
			return this._storage.IsNull(record);
		}

		internal bool IsInRelation()
		{
			DataRelationCollection dataRelationCollection = this._table.ParentRelations;
			for (int i = 0; i < dataRelationCollection.Count; i++)
			{
				if (dataRelationCollection[i].ChildKey.ContainsColumn(this))
				{
					return true;
				}
			}
			dataRelationCollection = this._table.ChildRelations;
			for (int j = 0; j < dataRelationCollection.Count; j++)
			{
				if (dataRelationCollection[j].ParentKey.ContainsColumn(this))
				{
					return true;
				}
			}
			return false;
		}

		internal bool IsMaxLengthViolated()
		{
			if (this.MaxLength < 0)
			{
				return true;
			}
			bool result = false;
			string text = null;
			foreach (object obj in this.Table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow.HasVersion(DataRowVersion.Current))
				{
					object obj2 = dataRow[this];
					if (!this._isSqlType)
					{
						if (obj2 != null && obj2 != DBNull.Value && ((string)obj2).Length > this.MaxLength)
						{
							if (text == null)
							{
								text = ExceptionBuilder.MaxLengthViolationText(this.ColumnName);
							}
							dataRow.RowError = text;
							dataRow.SetColumnError(this, text);
							result = true;
						}
					}
					else if (!DataStorage.IsObjectNull(obj2) && ((SqlString)obj2).Value.Length > this.MaxLength)
					{
						if (text == null)
						{
							text = ExceptionBuilder.MaxLengthViolationText(this.ColumnName);
						}
						dataRow.RowError = text;
						dataRow.SetColumnError(this, text);
						result = true;
					}
				}
			}
			return result;
		}

		internal bool IsNotAllowDBNullViolated()
		{
			Index sortIndex = this.SortIndex;
			DataRow[] rows = sortIndex.GetRows(sortIndex.FindRecords(DBNull.Value));
			for (int i = 0; i < rows.Length; i++)
			{
				string text = ExceptionBuilder.NotAllowDBNullViolationText(this.ColumnName);
				rows[i].RowError = text;
				rows[i].SetColumnError(this, text);
			}
			return rows.Length != 0;
		}

		internal void FinishInitInProgress()
		{
			if (this.Computed)
			{
				this.BindExpression();
			}
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <param name="pcevent">Parameter reference.</param>
		protected virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			PropertyChangedEventHandler propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
			{
				return;
			}
			propertyChanging(this, pcevent);
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <param name="name">Parameter reference.</param>
		protected internal void RaisePropertyChanging(string name)
		{
			this.OnPropertyChanging(new PropertyChangedEventArgs(name));
		}

		private void InsureStorage()
		{
			if (this._storage == null)
			{
				this._storage = DataStorage.CreateStorage(this, this._dataType, this._storageType);
			}
		}

		internal void SetCapacity(int capacity)
		{
			this.InsureStorage();
			this._storage.SetCapacity(capacity);
		}

		private bool ShouldSerializeDefaultValue()
		{
			return !this.DefaultValueIsNull;
		}

		internal void OnSetDataSet()
		{
		}

		/// <summary>Gets the <see cref="P:System.Data.DataColumn.Expression" /> of the column, if one exists.</summary>
		/// <returns>The <see cref="P:System.Data.DataColumn.Expression" /> value, if the property is set; otherwise, the <see cref="P:System.Data.DataColumn.ColumnName" /> property.</returns>
		public override string ToString()
		{
			if (this._expression != null)
			{
				return this.ColumnName + " + " + this.Expression;
			}
			return this.ColumnName;
		}

		internal object ConvertXmlToObject(string s)
		{
			this.InsureStorage();
			return this._storage.ConvertXmlToObject(s);
		}

		internal object ConvertXmlToObject(XmlReader xmlReader, XmlRootAttribute xmlAttrib)
		{
			this.InsureStorage();
			return this._storage.ConvertXmlToObject(xmlReader, xmlAttrib);
		}

		internal string ConvertObjectToXml(object value)
		{
			this.InsureStorage();
			return this._storage.ConvertObjectToXml(value);
		}

		internal void ConvertObjectToXml(object value, XmlWriter xmlWriter, XmlRootAttribute xmlAttrib)
		{
			this.InsureStorage();
			this._storage.ConvertObjectToXml(value, xmlWriter, xmlAttrib);
		}

		internal object GetEmptyColumnStore(int recordCount)
		{
			this.InsureStorage();
			return this._storage.GetEmptyStorageInternal(recordCount);
		}

		internal void CopyValueIntoStore(int record, object store, BitArray nullbits, int storeIndex)
		{
			this._storage.CopyValueInternal(record, store, nullbits, storeIndex);
		}

		internal void SetStorage(object store, BitArray nullbits)
		{
			this.InsureStorage();
			this._storage.SetStorageInternal(store, nullbits);
		}

		internal void AddDependentColumn(DataColumn expressionColumn)
		{
			if (this._dependentColumns == null)
			{
				this._dependentColumns = new List<DataColumn>();
			}
			this._dependentColumns.Add(expressionColumn);
			this._table.AddDependentColumn(expressionColumn);
		}

		internal void RemoveDependentColumn(DataColumn expressionColumn)
		{
			if (this._dependentColumns != null && this._dependentColumns.Contains(expressionColumn))
			{
				this._dependentColumns.Remove(expressionColumn);
			}
			this._table.RemoveDependentColumn(expressionColumn);
		}

		internal void HandleDependentColumnList(DataExpression oldExpression, DataExpression newExpression)
		{
			if (oldExpression != null)
			{
				foreach (DataColumn dataColumn in oldExpression.GetDependency())
				{
					dataColumn.RemoveDependentColumn(this);
					if (dataColumn._table != this._table)
					{
						this._table.RemoveDependentColumn(this);
					}
				}
				this._table.RemoveDependentColumn(this);
			}
			if (newExpression != null)
			{
				foreach (DataColumn dataColumn2 in newExpression.GetDependency())
				{
					dataColumn2.AddDependentColumn(this);
					if (dataColumn2._table != this._table)
					{
						this._table.AddDependentColumn(this);
					}
				}
				this._table.AddDependentColumn(this);
			}
		}

		private bool _allowNull = true;

		private string _caption;

		private string _columnName;

		private Type _dataType;

		private StorageType _storageType;

		internal object _defaultValue = DBNull.Value;

		private DataSetDateTime _dateTimeMode = DataSetDateTime.UnspecifiedLocal;

		private DataExpression _expression;

		private int _maxLength = -1;

		private int _ordinal = -1;

		private bool _readOnly;

		internal Index _sortIndex;

		internal DataTable _table;

		private bool _unique;

		internal MappingType _columnMapping = MappingType.Element;

		internal int _hashCode;

		internal int _errors;

		private bool _isSqlType;

		private bool _implementsINullable;

		private bool _implementsIChangeTracking;

		private bool _implementsIRevertibleChangeTracking;

		private bool _implementsIXMLSerializable;

		private bool _defaultValueIsNull = true;

		internal List<DataColumn> _dependentColumns;

		internal PropertyCollection _extendedProperties;

		private DataStorage _storage;

		private AutoIncrementValue _autoInc;

		internal string _columnUri;

		private string _columnPrefix = string.Empty;

		internal string _encodedColumnName;

		internal SimpleType _simpleType;

		private static int s_objectTypeCount;

		private readonly int _objectID = Interlocked.Increment(ref DataColumn.s_objectTypeCount);
	}
}
