using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;

namespace System.Data
{
	/// <summary>Represents a constraint that can be enforced on one or more <see cref="T:System.Data.DataColumn" /> objects.</summary>
	[DefaultProperty("ConstraintName")]
	[TypeConverter(typeof(ConstraintConverter))]
	public abstract class Constraint
	{
		/// <summary>The name of a constraint in the <see cref="T:System.Data.ConstraintCollection" />.</summary>
		/// <returns>The name of the <see cref="T:System.Data.Constraint" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="T:System.Data.Constraint" /> name is a null value or empty string.</exception>
		/// <exception cref="T:System.Data.DuplicateNameException">The <see cref="T:System.Data.ConstraintCollection" /> already contains a <see cref="T:System.Data.Constraint" /> with the same name (The comparison is not case-sensitive.).</exception>
		[DefaultValue("")]
		public virtual string ConstraintName
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (string.IsNullOrEmpty(value) && this.Table != null && this.InCollection)
				{
					throw ExceptionBuilder.NoConstraintName();
				}
				CultureInfo culture = (this.Table != null) ? this.Table.Locale : CultureInfo.CurrentCulture;
				if (string.Compare(this._name, value, true, culture) != 0)
				{
					if (this.Table != null && this.InCollection)
					{
						this.Table.Constraints.RegisterName(value);
						if (this._name.Length != 0)
						{
							this.Table.Constraints.UnregisterName(this._name);
						}
					}
					this._name = value;
					return;
				}
				if (string.Compare(this._name, value, false, culture) != 0)
				{
					this._name = value;
				}
			}
		}

		internal string SchemaName
		{
			get
			{
				if (!string.IsNullOrEmpty(this._schemaName))
				{
					return this._schemaName;
				}
				return this.ConstraintName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._schemaName = value;
				}
			}
		}

		internal virtual bool InCollection
		{
			get
			{
				return this._inCollection;
			}
			set
			{
				this._inCollection = value;
				this._dataSet = (value ? this.Table.DataSet : null);
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataTable" /> to which the constraint applies.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> to which the constraint applies.</returns>
		public abstract DataTable Table { get; }

		/// <summary>Gets the collection of user-defined constraint properties.</summary>
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

		internal abstract bool ContainsColumn(DataColumn column);

		internal abstract bool CanEnableConstraint();

		internal abstract Constraint Clone(DataSet destination);

		internal abstract Constraint Clone(DataSet destination, bool ignoreNSforTableLookup);

		internal void CheckConstraint()
		{
			if (!this.CanEnableConstraint())
			{
				throw ExceptionBuilder.ConstraintViolation(this.ConstraintName);
			}
		}

		internal abstract void CheckCanAddToCollection(ConstraintCollection constraint);

		internal abstract bool CanBeRemovedFromCollection(ConstraintCollection constraint, bool fThrowException);

		internal abstract void CheckConstraint(DataRow row, DataRowAction action);

		internal abstract void CheckState();

		/// <summary>Gets the <see cref="T:System.Data.DataSet" /> to which this constraint belongs.</summary>
		protected void CheckStateForProperty()
		{
			try
			{
				this.CheckState();
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				throw ExceptionBuilder.BadObjectPropertyAccess(ex.Message);
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataSet" /> to which this constraint belongs.</summary>
		/// <returns>The <see cref="T:System.Data.DataSet" /> to which the constraint belongs.</returns>
		[CLSCompliant(false)]
		protected virtual DataSet _DataSet
		{
			get
			{
				return this._dataSet;
			}
		}

		/// <summary>Sets the constraint's <see cref="T:System.Data.DataSet" />.</summary>
		/// <param name="dataSet">The <see cref="T:System.Data.DataSet" /> to which this constraint will belong.</param>
		protected internal void SetDataSet(DataSet dataSet)
		{
			this._dataSet = dataSet;
		}

		internal abstract bool IsConstraintViolated();

		/// <summary>Gets the <see cref="P:System.Data.Constraint.ConstraintName" />, if there is one, as a string.</summary>
		/// <returns>The string value of the <see cref="P:System.Data.Constraint.ConstraintName" />.</returns>
		public override string ToString()
		{
			return this.ConstraintName;
		}

		private string _schemaName = string.Empty;

		private bool _inCollection;

		private DataSet _dataSet;

		internal string _name = string.Empty;

		internal PropertyCollection _extendedProperties;
	}
}
