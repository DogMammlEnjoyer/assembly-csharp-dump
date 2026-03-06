using System;
using System.ComponentModel;
using Unity;

namespace System.Data
{
	/// <summary>Represents a customized view of a <see cref="T:System.Data.DataRow" />.</summary>
	public class DataRowView : ICustomTypeDescriptor, IEditableObject, IDataErrorInfo, INotifyPropertyChanged
	{
		internal DataRowView(DataView dataView, DataRow row)
		{
			this._dataView = dataView;
			this._row = row;
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Data.DataRowView" /> is identical to the specified object.</summary>
		/// <param name="other">An <see cref="T:System.Object" /> to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="object" /> is a <see cref="T:System.Data.DataRowView" /> and it returns the same row as the current <see cref="T:System.Data.DataRowView" />; otherwise <see langword="false" />.</returns>
		public override bool Equals(object other)
		{
			return this == other;
		}

		/// <summary>Returns the hash code of the <see cref="T:System.Data.DataRow" /> object.</summary>
		/// <returns>A 32-bit signed integer hash code 1, which represents Boolean <see langword="true" /> if the value of this instance is nonzero; otherwise the integer zero, which represents Boolean <see langword="false" />.</returns>
		public override int GetHashCode()
		{
			return this.Row.GetHashCode();
		}

		/// <summary>Gets the <see cref="T:System.Data.DataView" /> to which this row belongs.</summary>
		/// <returns>The <see langword="DataView" /> to which this row belongs.</returns>
		public DataView DataView
		{
			get
			{
				return this._dataView;
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._row._objectID;
			}
		}

		/// <summary>Gets or sets a value in a specified column.</summary>
		/// <param name="ndx">The specified column.</param>
		/// <returns>The value of the column.</returns>
		public object this[int ndx]
		{
			get
			{
				return this.Row[ndx, this.RowVersionDefault];
			}
			set
			{
				if (!this._dataView.AllowEdit && !this.IsNew)
				{
					throw ExceptionBuilder.CanNotEdit();
				}
				this.SetColumnValue(this._dataView.Table.Columns[ndx], value);
			}
		}

		/// <summary>Gets or sets a value in a specified column.</summary>
		/// <param name="property">String that contains the specified column.</param>
		/// <returns>The value of the column.</returns>
		public object this[string property]
		{
			get
			{
				DataColumn dataColumn = this._dataView.Table.Columns[property];
				if (dataColumn != null)
				{
					return this.Row[dataColumn, this.RowVersionDefault];
				}
				if (this._dataView.Table.DataSet != null && this._dataView.Table.DataSet.Relations.Contains(property))
				{
					return this.CreateChildView(property);
				}
				throw ExceptionBuilder.PropertyNotFound(property, this._dataView.Table.TableName);
			}
			set
			{
				DataColumn dataColumn = this._dataView.Table.Columns[property];
				if (dataColumn == null)
				{
					throw ExceptionBuilder.SetFailed(property);
				}
				if (!this._dataView.AllowEdit && !this.IsNew)
				{
					throw ExceptionBuilder.CanNotEdit();
				}
				this.SetColumnValue(dataColumn, value);
			}
		}

		/// <summary>Gets the error message for the property with the given name.</summary>
		/// <param name="colName">The name of the property whose error message to get.</param>
		/// <returns>The error message for the property. The default is an empty string ("").</returns>
		string IDataErrorInfo.this[string colName]
		{
			get
			{
				return this.Row.GetColumnError(colName);
			}
		}

		/// <summary>Gets a message that describes any validation errors for the object.</summary>
		/// <returns>The validation error on the object.</returns>
		string IDataErrorInfo.Error
		{
			get
			{
				return this.Row.RowError;
			}
		}

		/// <summary>Gets the current version description of the <see cref="T:System.Data.DataRow" />.</summary>
		/// <returns>One of the <see cref="T:System.Data.DataRowVersion" /> values. Possible values for the <see cref="P:System.Data.DataRowView.RowVersion" /> property are <see langword="Default" />, <see langword="Original" />, <see langword="Current" />, and <see langword="Proposed" />.</returns>
		public DataRowVersion RowVersion
		{
			get
			{
				return this.RowVersionDefault & (DataRowVersion)(-1025);
			}
		}

		private DataRowVersion RowVersionDefault
		{
			get
			{
				return this.Row.GetDefaultRowVersion(this._dataView.RowStateFilter);
			}
		}

		internal int GetRecord()
		{
			return this.Row.GetRecordFromVersion(this.RowVersionDefault);
		}

		internal bool HasRecord()
		{
			return this.Row.HasVersion(this.RowVersionDefault);
		}

		internal object GetColumnValue(DataColumn column)
		{
			return this.Row[column, this.RowVersionDefault];
		}

		internal void SetColumnValue(DataColumn column, object value)
		{
			if (this._delayBeginEdit)
			{
				this._delayBeginEdit = false;
				this.Row.BeginEdit();
			}
			if (DataRowVersion.Original == this.RowVersionDefault)
			{
				throw ExceptionBuilder.SetFailed(column.ColumnName);
			}
			this.Row[column] = value;
		}

		/// <summary>Returns a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" /> with the specified <see cref="T:System.Data.DataRelation" /> and parent.</summary>
		/// <param name="relation">The <see cref="T:System.Data.DataRelation" /> object.</param>
		/// <param name="followParent">The parent object.</param>
		/// <returns>A <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" />.</returns>
		public DataView CreateChildView(DataRelation relation, bool followParent)
		{
			if (relation == null || relation.ParentKey.Table != this.DataView.Table)
			{
				throw ExceptionBuilder.CreateChildView();
			}
			RelatedView relatedView;
			if (!followParent)
			{
				int record = this.GetRecord();
				object[] keyValues = relation.ParentKey.GetKeyValues(record);
				relatedView = new RelatedView(relation.ChildColumnsReference, keyValues);
			}
			else
			{
				relatedView = new RelatedView(this, relation.ParentKey, relation.ChildColumnsReference);
			}
			relatedView.SetIndex("", DataViewRowState.CurrentRows, null);
			relatedView.SetDataViewManager(this.DataView.DataViewManager);
			return relatedView;
		}

		/// <summary>Returns a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" /> with the specified child <see cref="T:System.Data.DataRelation" />.</summary>
		/// <param name="relation">The <see cref="T:System.Data.DataRelation" /> object.</param>
		/// <returns>a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" />.</returns>
		public DataView CreateChildView(DataRelation relation)
		{
			return this.CreateChildView(relation, false);
		}

		/// <summary>Returns a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" /> with the specified <see cref="T:System.Data.DataRelation" /> name and parent.</summary>
		/// <param name="relationName">A string containing the <see cref="T:System.Data.DataRelation" /> name.</param>
		/// <param name="followParent">The parent</param>
		/// <returns>a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" />.</returns>
		public DataView CreateChildView(string relationName, bool followParent)
		{
			return this.CreateChildView(this.DataView.Table.ChildRelations[relationName], followParent);
		}

		/// <summary>Returns a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" /> with the specified child <see cref="T:System.Data.DataRelation" /> name.</summary>
		/// <param name="relationName">A string containing the <see cref="T:System.Data.DataRelation" /> name.</param>
		/// <returns>a <see cref="T:System.Data.DataView" /> for the child <see cref="T:System.Data.DataTable" />.</returns>
		public DataView CreateChildView(string relationName)
		{
			return this.CreateChildView(relationName, false);
		}

		/// <summary>Gets the <see cref="T:System.Data.DataRow" /> being viewed.</summary>
		/// <returns>The <see cref="T:System.Data.DataRow" /> being viewed by the <see cref="T:System.Data.DataRowView" />.</returns>
		public DataRow Row
		{
			get
			{
				return this._row;
			}
		}

		/// <summary>Begins an edit procedure.</summary>
		public void BeginEdit()
		{
			this._delayBeginEdit = true;
		}

		/// <summary>Cancels an edit procedure.</summary>
		public void CancelEdit()
		{
			DataRow row = this.Row;
			if (this.IsNew)
			{
				this._dataView.FinishAddNew(false);
			}
			else
			{
				row.CancelEdit();
			}
			this._delayBeginEdit = false;
		}

		/// <summary>Commits changes to the underlying <see cref="T:System.Data.DataRow" /> and ends the editing session that was begun with <see cref="M:System.Data.DataRowView.BeginEdit" />.  Use <see cref="M:System.Data.DataRowView.CancelEdit" /> to discard the changes made to the <see cref="T:System.Data.DataRow" />.</summary>
		public void EndEdit()
		{
			if (this.IsNew)
			{
				this._dataView.FinishAddNew(true);
			}
			else
			{
				this.Row.EndEdit();
			}
			this._delayBeginEdit = false;
		}

		/// <summary>Indicates whether a <see cref="T:System.Data.DataRowView" /> is new.</summary>
		/// <returns>
		///   <see langword="true" /> if the row is new; otherwise <see langword="false" />.</returns>
		public bool IsNew
		{
			get
			{
				return this._row == this._dataView._addNewRow;
			}
		}

		/// <summary>Indicates whether the row is in edit mode.</summary>
		/// <returns>
		///   <see langword="true" /> if the row is in edit mode; otherwise <see langword="false" />.</returns>
		public bool IsEdit
		{
			get
			{
				return this.Row.HasVersion(DataRowVersion.Proposed) || this._delayBeginEdit;
			}
		}

		/// <summary>Deletes a row.</summary>
		public void Delete()
		{
			this._dataView.Delete(this.Row);
		}

		/// <summary>Event that is raised when a <see cref="T:System.Data.DataRowView" /> property is changed.</summary>
		public event PropertyChangedEventHandler PropertyChanged;

		internal void RaisePropertyChangedEvent(string propName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		/// <summary>Returns a collection of custom attributes for this instance of a component.</summary>
		/// <returns>An AttributeCollection containing the attributes for this object.</returns>
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection(null);
		}

		/// <summary>Returns the class name of this instance of a component.</summary>
		/// <returns>The class name of this instance of a component.</returns>
		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		/// <summary>Returns the name of this instance of a component.</summary>
		/// <returns>The name of this instance of a component.</returns>
		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		/// <summary>Returns a type converter for this instance of a component.</summary>
		/// <returns>The type converter for this instance of a component.</returns>
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		/// <summary>Returns the default event for this instance of a component.</summary>
		/// <returns>The default event for this instance of a component.</returns>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		/// <summary>Returns the default property for this instance of a component.</summary>
		/// <returns>The default property for this instance of a component.</returns>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		/// <summary>Returns an editor of the specified type for this instance of a component.</summary>
		/// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the editor for this object.</param>
		/// <returns>An <see cref="T:System.Object" /> of the specified type that is the editor for this object, or <see langword="null" /> if the editor cannot be found.</returns>
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		/// <summary>Returns the events for this instance of a component.</summary>
		/// <returns>The events for this instance of a component.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		/// <summary>Returns the events for this instance of a component with specified attributes.</summary>
		/// <param name="attributes">The attributes</param>
		/// <returns>The events for this instance of a component.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		/// <summary>Returns the properties for this instance of a component.</summary>
		/// <returns>The properties for this instance of a component.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		/// <summary>Returns the properties for this instance of a component with specified attributes.</summary>
		/// <param name="attributes">The attributes.</param>
		/// <returns>The properties for this instance of a component.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			if (this._dataView.Table == null)
			{
				return DataRowView.s_zeroPropertyDescriptorCollection;
			}
			return this._dataView.Table.GetPropertyDescriptorCollection(attributes);
		}

		/// <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
		/// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the property whose owner is to be found.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents the owner of the specified property.</returns>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		internal DataRowView()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly DataView _dataView;

		private readonly DataRow _row;

		private bool _delayBeginEdit;

		private static readonly PropertyDescriptorCollection s_zeroPropertyDescriptorCollection = new PropertyDescriptorCollection(null);
	}
}
