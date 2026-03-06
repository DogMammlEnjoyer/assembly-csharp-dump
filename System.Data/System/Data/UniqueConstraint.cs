using System;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Data
{
	/// <summary>Represents a restriction on a set of columns in which all values must be unique.</summary>
	[DefaultProperty("ConstraintName")]
	public class UniqueConstraint : Constraint
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the specified name and <see cref="T:System.Data.DataColumn" />.</summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="column">The <see cref="T:System.Data.DataColumn" /> to constrain.</param>
		public UniqueConstraint(string name, DataColumn column)
		{
			this.Create(name, new DataColumn[]
			{
				column
			});
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the specified <see cref="T:System.Data.DataColumn" />.</summary>
		/// <param name="column">The <see cref="T:System.Data.DataColumn" /> to constrain.</param>
		public UniqueConstraint(DataColumn column)
		{
			this.Create(null, new DataColumn[]
			{
				column
			});
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the specified name and array of <see cref="T:System.Data.DataColumn" /> objects.</summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="columns">The array of <see cref="T:System.Data.DataColumn" /> objects to constrain.</param>
		public UniqueConstraint(string name, DataColumn[] columns)
		{
			this.Create(name, columns);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the given array of <see cref="T:System.Data.DataColumn" /> objects.</summary>
		/// <param name="columns">The array of <see cref="T:System.Data.DataColumn" /> objects to constrain.</param>
		public UniqueConstraint(DataColumn[] columns)
		{
			this.Create(null, columns);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the specified name, an array of <see cref="T:System.Data.DataColumn" /> objects to constrain, and a value specifying whether the constraint is a primary key.</summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="columnNames">An array of <see cref="T:System.Data.DataColumn" /> objects to constrain.</param>
		/// <param name="isPrimaryKey">
		///   <see langword="true" /> to indicate that the constraint is a primary key; otherwise, <see langword="false" />.</param>
		[Browsable(false)]
		public UniqueConstraint(string name, string[] columnNames, bool isPrimaryKey)
		{
			this._constraintName = name;
			this._columnNames = columnNames;
			this._bPrimaryKey = isPrimaryKey;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the specified name, the <see cref="T:System.Data.DataColumn" /> to constrain, and a value specifying whether the constraint is a primary key.</summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="column">The <see cref="T:System.Data.DataColumn" /> to constrain.</param>
		/// <param name="isPrimaryKey">
		///   <see langword="true" /> to indicate that the constraint is a primary key; otherwise, <see langword="false" />.</param>
		public UniqueConstraint(string name, DataColumn column, bool isPrimaryKey)
		{
			DataColumn[] columns = new DataColumn[]
			{
				column
			};
			this._bPrimaryKey = isPrimaryKey;
			this.Create(name, columns);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the <see cref="T:System.Data.DataColumn" /> to constrain, and a value specifying whether the constraint is a primary key.</summary>
		/// <param name="column">The <see cref="T:System.Data.DataColumn" /> to constrain.</param>
		/// <param name="isPrimaryKey">
		///   <see langword="true" /> to indicate that the constraint is a primary key; otherwise, <see langword="false" />.</param>
		public UniqueConstraint(DataColumn column, bool isPrimaryKey)
		{
			DataColumn[] columns = new DataColumn[]
			{
				column
			};
			this._bPrimaryKey = isPrimaryKey;
			this.Create(null, columns);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with the specified name, an array of <see cref="T:System.Data.DataColumn" /> objects to constrain, and a value specifying whether the constraint is a primary key.</summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="columns">An array of <see cref="T:System.Data.DataColumn" /> objects to constrain.</param>
		/// <param name="isPrimaryKey">
		///   <see langword="true" /> to indicate that the constraint is a primary key; otherwise, <see langword="false" />.</param>
		public UniqueConstraint(string name, DataColumn[] columns, bool isPrimaryKey)
		{
			this._bPrimaryKey = isPrimaryKey;
			this.Create(name, columns);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.UniqueConstraint" /> class with an array of <see cref="T:System.Data.DataColumn" /> objects to constrain, and a value specifying whether the constraint is a primary key.</summary>
		/// <param name="columns">An array of <see cref="T:System.Data.DataColumn" /> objects to constrain.</param>
		/// <param name="isPrimaryKey">
		///   <see langword="true" /> to indicate that the constraint is a primary key; otherwise, <see langword="false" />.</param>
		public UniqueConstraint(DataColumn[] columns, bool isPrimaryKey)
		{
			this._bPrimaryKey = isPrimaryKey;
			this.Create(null, columns);
		}

		internal string[] ColumnNames
		{
			get
			{
				return this._key.GetColumnNames();
			}
		}

		internal Index ConstraintIndex
		{
			get
			{
				return this._constraintIndex;
			}
		}

		[Conditional("DEBUG")]
		private void AssertConstraintAndKeyIndexes()
		{
			DataColumn[] array = new DataColumn[this._constraintIndex._indexFields.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this._constraintIndex._indexFields[i].Column;
			}
		}

		internal void ConstraintIndexClear()
		{
			if (this._constraintIndex != null)
			{
				this._constraintIndex.RemoveRef();
				this._constraintIndex = null;
			}
		}

		internal void ConstraintIndexInitialize()
		{
			if (this._constraintIndex == null)
			{
				this._constraintIndex = this._key.GetSortIndex();
				this._constraintIndex.AddRef();
			}
		}

		internal override void CheckState()
		{
			this.NonVirtualCheckState();
		}

		private void NonVirtualCheckState()
		{
			this._key.CheckState();
		}

		internal override void CheckCanAddToCollection(ConstraintCollection constraints)
		{
		}

		internal override bool CanBeRemovedFromCollection(ConstraintCollection constraints, bool fThrowException)
		{
			if (!this.Equals(constraints.Table._primaryKey))
			{
				ParentForeignKeyConstraintEnumerator parentForeignKeyConstraintEnumerator = new ParentForeignKeyConstraintEnumerator(this.Table.DataSet, this.Table);
				while (parentForeignKeyConstraintEnumerator.GetNext())
				{
					ForeignKeyConstraint foreignKeyConstraint = parentForeignKeyConstraintEnumerator.GetForeignKeyConstraint();
					if (this._key.ColumnsEqual(foreignKeyConstraint.ParentKey))
					{
						if (!fThrowException)
						{
							return false;
						}
						throw ExceptionBuilder.NeededForForeignKeyConstraint(this, foreignKeyConstraint);
					}
				}
				return true;
			}
			if (!fThrowException)
			{
				return false;
			}
			throw ExceptionBuilder.RemovePrimaryKey(constraints.Table);
		}

		internal override bool CanEnableConstraint()
		{
			return !this.Table.EnforceConstraints || this.ConstraintIndex.CheckUnique();
		}

		internal override bool IsConstraintViolated()
		{
			bool result = false;
			Index constraintIndex = this.ConstraintIndex;
			if (constraintIndex.HasDuplicates)
			{
				object[] uniqueKeyValues = constraintIndex.GetUniqueKeyValues();
				for (int i = 0; i < uniqueKeyValues.Length; i++)
				{
					Range range = constraintIndex.FindRecords((object[])uniqueKeyValues[i]);
					if (1 < range.Count)
					{
						DataRow[] rows = constraintIndex.GetRows(range);
						string text = ExceptionBuilder.UniqueConstraintViolationText(this._key.ColumnsReference, (object[])uniqueKeyValues[i]);
						for (int j = 0; j < rows.Length; j++)
						{
							rows[j].RowError = text;
							foreach (DataColumn column in this._key.ColumnsReference)
							{
								rows[j].SetColumnError(column, text);
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		internal override void CheckConstraint(DataRow row, DataRowAction action)
		{
			if (this.Table.EnforceConstraints && (action == DataRowAction.Add || action == DataRowAction.Change || (action == DataRowAction.Rollback && row._tempRecord != -1)) && row.HaveValuesChanged(this.ColumnsReference) && this.ConstraintIndex.IsKeyRecordInIndex(row.GetDefaultRecord()))
			{
				object[] columnValues = row.GetColumnValues(this.ColumnsReference);
				throw ExceptionBuilder.ConstraintViolation(this.ColumnsReference, columnValues);
			}
		}

		internal override bool ContainsColumn(DataColumn column)
		{
			return this._key.ContainsColumn(column);
		}

		internal override Constraint Clone(DataSet destination)
		{
			return this.Clone(destination, false);
		}

		internal override Constraint Clone(DataSet destination, bool ignorNSforTableLookup)
		{
			int num;
			if (ignorNSforTableLookup)
			{
				num = destination.Tables.IndexOf(this.Table.TableName);
			}
			else
			{
				num = destination.Tables.IndexOf(this.Table.TableName, this.Table.Namespace, false);
			}
			if (num < 0)
			{
				return null;
			}
			DataTable dataTable = destination.Tables[num];
			int num2 = this.ColumnsReference.Length;
			DataColumn[] array = new DataColumn[num2];
			for (int i = 0; i < num2; i++)
			{
				DataColumn dataColumn = this.ColumnsReference[i];
				num = dataTable.Columns.IndexOf(dataColumn.ColumnName);
				if (num < 0)
				{
					return null;
				}
				array[i] = dataTable.Columns[num];
			}
			UniqueConstraint uniqueConstraint = new UniqueConstraint(this.ConstraintName, array);
			foreach (object key in base.ExtendedProperties.Keys)
			{
				uniqueConstraint.ExtendedProperties[key] = base.ExtendedProperties[key];
			}
			return uniqueConstraint;
		}

		internal UniqueConstraint Clone(DataTable table)
		{
			int num = this.ColumnsReference.Length;
			DataColumn[] array = new DataColumn[num];
			for (int i = 0; i < num; i++)
			{
				DataColumn dataColumn = this.ColumnsReference[i];
				int num2 = table.Columns.IndexOf(dataColumn.ColumnName);
				if (num2 < 0)
				{
					return null;
				}
				array[i] = table.Columns[num2];
			}
			UniqueConstraint uniqueConstraint = new UniqueConstraint(this.ConstraintName, array);
			foreach (object key in base.ExtendedProperties.Keys)
			{
				uniqueConstraint.ExtendedProperties[key] = base.ExtendedProperties[key];
			}
			return uniqueConstraint;
		}

		/// <summary>Gets the array of columns that this constraint affects.</summary>
		/// <returns>An array of <see cref="T:System.Data.DataColumn" /> objects.</returns>
		[ReadOnly(true)]
		public virtual DataColumn[] Columns
		{
			get
			{
				return this._key.ToArray();
			}
		}

		internal DataColumn[] ColumnsReference
		{
			get
			{
				return this._key.ColumnsReference;
			}
		}

		/// <summary>Gets a value indicating whether or not the constraint is on a primary key.</summary>
		/// <returns>
		///   <see langword="true" />, if the constraint is on a primary key; otherwise, <see langword="false" />.</returns>
		public bool IsPrimaryKey
		{
			get
			{
				return this.Table != null && this == this.Table._primaryKey;
			}
		}

		private void Create(string constraintName, DataColumn[] columns)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i].Computed)
				{
					throw ExceptionBuilder.ExpressionInConstraint(columns[i]);
				}
			}
			this._key = new DataKey(columns, true);
			this.ConstraintName = constraintName;
			this.NonVirtualCheckState();
		}

		/// <summary>Compares this constraint to a second to determine if both are identical.</summary>
		/// <param name="key2">The object to which this <see cref="T:System.Data.UniqueConstraint" /> is compared.</param>
		/// <returns>
		///   <see langword="true" />, if the contraints are equal; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object key2)
		{
			return key2 is UniqueConstraint && this.Key.ColumnsEqual(((UniqueConstraint)key2).Key);
		}

		/// <summary>Gets the hash code of this instance of the <see cref="T:System.Data.UniqueConstraint" /> object.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal override bool InCollection
		{
			set
			{
				base.InCollection = value;
				if (this._key.ColumnsReference.Length == 1)
				{
					this._key.ColumnsReference[0].InternalUnique(value);
				}
			}
		}

		internal DataKey Key
		{
			get
			{
				return this._key;
			}
		}

		/// <summary>Gets the table to which this constraint belongs.</summary>
		/// <returns>The <see cref="T:System.Data.DataTable" /> to which the constraint belongs.</returns>
		[ReadOnly(true)]
		public override DataTable Table
		{
			get
			{
				if (this._key.HasValue)
				{
					return this._key.Table;
				}
				return null;
			}
		}

		private DataKey _key;

		private Index _constraintIndex;

		internal bool _bPrimaryKey;

		internal string _constraintName;

		internal string[] _columnNames;
	}
}
