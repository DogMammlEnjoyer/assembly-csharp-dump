using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Data.Common
{
	/// <summary>Contains a generic column mapping for an object that inherits from <see cref="T:System.Data.Common.DataAdapter" />. This class cannot be inherited.</summary>
	[TypeConverter(typeof(DataColumnMapping.DataColumnMappingConverter))]
	public sealed class DataColumnMapping : MarshalByRefObject, IColumnMapping, ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DataColumnMapping" /> class.</summary>
		public DataColumnMapping()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DataColumnMapping" /> class with the specified source column name and <see cref="T:System.Data.DataSet" /> column name to map to.</summary>
		/// <param name="sourceColumn">The case-sensitive column name from a data source.</param>
		/// <param name="dataSetColumn">The column name, which is not case sensitive, from a <see cref="T:System.Data.DataSet" /> to map to.</param>
		public DataColumnMapping(string sourceColumn, string dataSetColumn)
		{
			this.SourceColumn = sourceColumn;
			this.DataSetColumn = dataSetColumn;
		}

		/// <summary>Gets or sets the name of the column within the <see cref="T:System.Data.DataSet" /> to map to.</summary>
		/// <returns>The name of the column within the <see cref="T:System.Data.DataSet" /> to map to. The name is not case sensitive.</returns>
		[DefaultValue("")]
		public string DataSetColumn
		{
			get
			{
				return this._dataSetColumnName ?? string.Empty;
			}
			set
			{
				this._dataSetColumnName = value;
			}
		}

		internal DataColumnMappingCollection Parent
		{
			get
			{
				return this._parent;
			}
			set
			{
				this._parent = value;
			}
		}

		/// <summary>Gets or sets the name of the column within the data source to map from. The name is case-sensitive.</summary>
		/// <returns>The case-sensitive name of the column in the data source.</returns>
		[DefaultValue("")]
		public string SourceColumn
		{
			get
			{
				return this._sourceColumnName ?? string.Empty;
			}
			set
			{
				if (this.Parent != null && ADP.SrcCompare(this._sourceColumnName, value) != 0)
				{
					this.Parent.ValidateSourceColumn(-1, value);
				}
				this._sourceColumnName = value;
			}
		}

		/// <summary>Creates a new object that is a copy of the current instance.</summary>
		/// <returns>A copy of the current object.</returns>
		object ICloneable.Clone()
		{
			return new DataColumnMapping
			{
				_sourceColumnName = this._sourceColumnName,
				_dataSetColumnName = this._dataSetColumnName
			};
		}

		/// <summary>Gets a <see cref="T:System.Data.DataColumn" /> from the given <see cref="T:System.Data.DataTable" /> using the <see cref="T:System.Data.MissingSchemaAction" /> and the <see cref="P:System.Data.Common.DataColumnMapping.DataSetColumn" /> property.</summary>
		/// <param name="dataTable">The <see cref="T:System.Data.DataTable" /> to get the column from.</param>
		/// <param name="dataType">The <see cref="T:System.Type" /> of the data column.</param>
		/// <param name="schemaAction">One of the <see cref="T:System.Data.MissingSchemaAction" /> values.</param>
		/// <returns>A data column.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataColumn GetDataColumnBySchemaAction(DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
		{
			return DataColumnMapping.GetDataColumnBySchemaAction(this.SourceColumn, this.DataSetColumn, dataTable, dataType, schemaAction);
		}

		/// <summary>A static version of <see cref="M:System.Data.Common.DataColumnMapping.GetDataColumnBySchemaAction(System.Data.DataTable,System.Type,System.Data.MissingSchemaAction)" /> that can be called without instantiating a <see cref="T:System.Data.Common.DataColumnMapping" /> object.</summary>
		/// <param name="sourceColumn">The case-sensitive column name from a data source.</param>
		/// <param name="dataSetColumn">The column name, which is not case sensitive, from a <see cref="T:System.Data.DataSet" /> to map to.</param>
		/// <param name="dataTable">An instance of <see cref="T:System.Data.DataTable" />.</param>
		/// <param name="dataType">The data type for the column being mapped.</param>
		/// <param name="schemaAction">Determines the action to take when existing <see cref="T:System.Data.DataSet" /> schema does not match incoming data.</param>
		/// <returns>A <see cref="T:System.Data.DataColumn" /> object.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataColumn GetDataColumnBySchemaAction(string sourceColumn, string dataSetColumn, DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
		{
			if (dataTable == null)
			{
				throw ADP.ArgumentNull("dataTable");
			}
			if (string.IsNullOrEmpty(dataSetColumn))
			{
				return null;
			}
			DataColumnCollection columns = dataTable.Columns;
			int num = columns.IndexOf(dataSetColumn);
			if (0 > num || num >= columns.Count)
			{
				return DataColumnMapping.CreateDataColumnBySchemaAction(sourceColumn, dataSetColumn, dataTable, dataType, schemaAction);
			}
			DataColumn dataColumn = columns[num];
			if (!string.IsNullOrEmpty(dataColumn.Expression))
			{
				throw ADP.ColumnSchemaExpression(sourceColumn, dataSetColumn);
			}
			if (null == dataType || dataType.IsArray == dataColumn.DataType.IsArray)
			{
				return dataColumn;
			}
			throw ADP.ColumnSchemaMismatch(sourceColumn, dataType, dataColumn);
		}

		internal static DataColumn CreateDataColumnBySchemaAction(string sourceColumn, string dataSetColumn, DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
		{
			if (string.IsNullOrEmpty(dataSetColumn))
			{
				return null;
			}
			switch (schemaAction)
			{
			case MissingSchemaAction.Add:
			case MissingSchemaAction.AddWithKey:
				return new DataColumn(dataSetColumn, dataType);
			case MissingSchemaAction.Ignore:
				return null;
			case MissingSchemaAction.Error:
				throw ADP.ColumnSchemaMissing(dataSetColumn, dataTable.TableName, sourceColumn);
			default:
				throw ADP.InvalidMissingSchemaAction(schemaAction);
			}
		}

		/// <summary>Converts the current <see cref="P:System.Data.Common.DataColumnMapping.SourceColumn" /> name to a string.</summary>
		/// <returns>The current <see cref="P:System.Data.Common.DataColumnMapping.SourceColumn" /> name as a string.</returns>
		public override string ToString()
		{
			return this.SourceColumn;
		}

		private DataColumnMappingCollection _parent;

		private string _dataSetColumnName;

		private string _sourceColumnName;

		internal sealed class DataColumnMappingConverter : ExpandableObjectConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return typeof(InstanceDescriptor) == destinationType || base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (null == destinationType)
				{
					throw ADP.ArgumentNull("destinationType");
				}
				if (typeof(InstanceDescriptor) == destinationType && value is DataColumnMapping)
				{
					DataColumnMapping dataColumnMapping = (DataColumnMapping)value;
					object[] arguments = new object[]
					{
						dataColumnMapping.SourceColumn,
						dataColumnMapping.DataSetColumn
					};
					Type[] types = new Type[]
					{
						typeof(string),
						typeof(string)
					};
					return new InstanceDescriptor(typeof(DataColumnMapping).GetConstructor(types), arguments);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
