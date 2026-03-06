using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Data
{
	/// <summary>Contains a default <see cref="T:System.Data.DataViewSettingCollection" /> for each <see cref="T:System.Data.DataTable" /> in a <see cref="T:System.Data.DataSet" />.</summary>
	public class DataViewManager : MarshalByValueComponent, IBindingList, IList, ICollection, IEnumerable, ITypedList
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataViewManager" /> class.</summary>
		public DataViewManager() : this(null, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataViewManager" /> class for the specified <see cref="T:System.Data.DataSet" />.</summary>
		/// <param name="dataSet">The name of the <see cref="T:System.Data.DataSet" /> to use.</param>
		public DataViewManager(DataSet dataSet) : this(dataSet, false)
		{
		}

		internal DataViewManager(DataSet dataSet, bool locked)
		{
			GC.SuppressFinalize(this);
			this._dataSet = dataSet;
			if (this._dataSet != null)
			{
				this._dataSet.Tables.CollectionChanged += this.TableCollectionChanged;
				this._dataSet.Relations.CollectionChanged += this.RelationCollectionChanged;
			}
			this._locked = locked;
			this._item = new DataViewManagerListItemTypeDescriptor(this);
			this._dataViewSettingsCollection = new DataViewSettingCollection(this);
		}

		/// <summary>Gets or sets the <see cref="T:System.Data.DataSet" /> to use with the <see cref="T:System.Data.DataViewManager" />.</summary>
		/// <returns>The <see cref="T:System.Data.DataSet" /> to use.</returns>
		[DefaultValue(null)]
		public DataSet DataSet
		{
			get
			{
				return this._dataSet;
			}
			set
			{
				if (value == null)
				{
					throw ExceptionBuilder.SetFailed("DataSet to null");
				}
				if (this._locked)
				{
					throw ExceptionBuilder.SetDataSetFailed();
				}
				if (this._dataSet != null)
				{
					if (this._nViews > 0)
					{
						throw ExceptionBuilder.CanNotSetDataSet();
					}
					this._dataSet.Tables.CollectionChanged -= this.TableCollectionChanged;
					this._dataSet.Relations.CollectionChanged -= this.RelationCollectionChanged;
				}
				this._dataSet = value;
				this._dataSet.Tables.CollectionChanged += this.TableCollectionChanged;
				this._dataSet.Relations.CollectionChanged += this.RelationCollectionChanged;
				this._dataViewSettingsCollection = new DataViewSettingCollection(this);
				this._item.Reset();
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataViewSettingCollection" /> for each <see cref="T:System.Data.DataTable" /> in the <see cref="T:System.Data.DataSet" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataViewSettingCollection" /> for each <see langword="DataTable" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewSettingCollection DataViewSettings
		{
			get
			{
				return this._dataViewSettingsCollection;
			}
		}

		/// <summary>Gets or sets a value that is used for code persistence.</summary>
		/// <returns>A value that is used for code persistence.</returns>
		public string DataViewSettingCollectionString
		{
			get
			{
				if (this._dataSet == null)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<DataViewSettingCollectionString>");
				foreach (object obj in this._dataSet.Tables)
				{
					DataTable dataTable = (DataTable)obj;
					DataViewSetting dataViewSetting = this._dataViewSettingsCollection[dataTable];
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "<{0} Sort=\"{1}\" RowFilter=\"{2}\" RowStateFilter=\"{3}\"/>", new object[]
					{
						dataTable.EncodedTableName,
						dataViewSetting.Sort,
						dataViewSetting.RowFilter,
						dataViewSetting.RowStateFilter
					});
				}
				stringBuilder.Append("</DataViewSettingCollectionString>");
				return stringBuilder.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(value));
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
				xmlTextReader.Read();
				if (xmlTextReader.Name != "DataViewSettingCollectionString")
				{
					throw ExceptionBuilder.SetFailed("DataViewSettingCollectionString");
				}
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType == XmlNodeType.Element)
					{
						string tableName = XmlConvert.DecodeName(xmlTextReader.LocalName);
						if (xmlTextReader.MoveToAttribute("Sort"))
						{
							this._dataViewSettingsCollection[tableName].Sort = xmlTextReader.Value;
						}
						if (xmlTextReader.MoveToAttribute("RowFilter"))
						{
							this._dataViewSettingsCollection[tableName].RowFilter = xmlTextReader.Value;
						}
						if (xmlTextReader.MoveToAttribute("RowStateFilter"))
						{
							this._dataViewSettingsCollection[tableName].RowStateFilter = (DataViewRowState)Enum.Parse(typeof(DataViewRowState), xmlTextReader.Value);
						}
					}
				}
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</summary>
		/// <returns>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			DataViewManagerListItemTypeDescriptor[] array = new DataViewManagerListItemTypeDescriptor[1];
			((ICollection)this).CopyTo(array, 0);
			return array.GetEnumerator();
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.Count" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.ICollection.Count" />.</returns>
		int ICollection.Count
		{
			get
			{
				return 1;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.IList.IsReadOnly" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.IList.IsReadOnly" />.</returns>
		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.IList.IsFixedSize" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, <see langword="false" />.</returns>
		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		/// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			array.SetValue(new DataViewManagerListItemTypeDescriptor(this), index);
		}

		/// <summary>Gets or sets the element at the specified index.</summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		object IList.this[int index]
		{
			get
			{
				return this._item;
			}
			set
			{
				throw ExceptionBuilder.CannotModifyCollection();
			}
		}

		/// <summary>Adds an item to the <see cref="T:System.Collections.IList" />.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to add to the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		int IList.Add(object value)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.Clear" />.</summary>
		void IList.Clear()
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, <see langword="false" />.</returns>
		bool IList.Contains(object value)
		{
			return value == this._item;
		}

		/// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
		int IList.IndexOf(object value)
		{
			if (value != this._item)
			{
				return -1;
			}
			return 1;
		}

		/// <summary>Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to insert into the <see cref="T:System.Collections.IList" />.</param>
		void IList.Insert(int index, object value)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		/// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to remove from the <see cref="T:System.Collections.IList" />.</param>
		void IList.Remove(object value)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		/// <summary>Removes the <see cref="T:System.Collections.IList" /> item at the specified index.</summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		void IList.RemoveAt(int index)
		{
			throw ExceptionBuilder.CannotModifyCollection();
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowNew" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowNew" />.</returns>
		bool IBindingList.AllowNew
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.AddNew" />.</summary>
		/// <returns>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.AddNew" />.</returns>
		object IBindingList.AddNew()
		{
			throw DataViewManager.s_notSupported;
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowEdit" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowEdit" />.</returns>
		bool IBindingList.AllowEdit
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowRemove" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowRemove" />.</returns>
		bool IBindingList.AllowRemove
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsChangeNotification" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsChangeNotification" />.</returns>
		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsSearching" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsSearching" />.</returns>
		bool IBindingList.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" />.</returns>
		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.IsSorted" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.IsSorted" />.</returns>
		bool IBindingList.IsSorted
		{
			get
			{
				throw DataViewManager.s_notSupported;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortProperty" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortProperty" />.</returns>
		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw DataViewManager.s_notSupported;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortDirection" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortDirection" />.</returns>
		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw DataViewManager.s_notSupported;
			}
		}

		/// <summary>Occurs after a row is added to or deleted from a <see cref="T:System.Data.DataView" />.</summary>
		public event ListChangedEventHandler ListChanged;

		/// <summary>Adds the <see cref="T:System.ComponentModel.PropertyDescriptor" /> to the indexes used for searching.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to add to the indexes used for searching.</param>
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		/// <summary>Sorts the list based on a <see cref="T:System.ComponentModel.PropertyDescriptor" /> and a <see cref="T:System.ComponentModel.ListSortDirection" />.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to sort by.</param>
		/// <param name="direction">One of the <see cref="T:System.ComponentModel.ListSortDirection" /> values.</param>
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw DataViewManager.s_notSupported;
		}

		/// <summary>Returns the index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor" />.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to search on.</param>
		/// <param name="key">The value of the property parameter to search for.</param>
		/// <returns>The index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor" />.</returns>
		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw DataViewManager.s_notSupported;
		}

		/// <summary>Removes the <see cref="T:System.ComponentModel.PropertyDescriptor" /> from the indexes used for searching.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to remove from the indexes used for searching.</param>
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		/// <summary>Removes any sort applied using <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" />.</summary>
		void IBindingList.RemoveSort()
		{
			throw DataViewManager.s_notSupported;
		}

		/// <summary>Returns the name of the list.</summary>
		/// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects, for which the list name is returned. This can be <see langword="null" />.</param>
		/// <returns>The name of the list.</returns>
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			DataSet dataSet = this.DataSet;
			if (dataSet == null)
			{
				throw ExceptionBuilder.CanNotUseDataViewManager();
			}
			if (listAccessors == null || listAccessors.Length == 0)
			{
				return dataSet.DataSetName;
			}
			DataTable dataTable = dataSet.FindTable(null, listAccessors, 0);
			if (dataTable != null)
			{
				return dataTable.TableName;
			}
			return string.Empty;
		}

		/// <summary>Returns the <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties on each item used to bind data.</summary>
		/// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects to find in the collection as bindable. This can be <see langword="null" />.</param>
		/// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties on each item used to bind data.</returns>
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			DataSet dataSet = this.DataSet;
			if (dataSet == null)
			{
				throw ExceptionBuilder.CanNotUseDataViewManager();
			}
			if (listAccessors == null || listAccessors.Length == 0)
			{
				return ((ICustomTypeDescriptor)new DataViewManagerListItemTypeDescriptor(this)).GetProperties();
			}
			DataTable dataTable = dataSet.FindTable(null, listAccessors, 0);
			if (dataTable != null)
			{
				return dataTable.GetPropertyDescriptorCollection(null);
			}
			return new PropertyDescriptorCollection(null);
		}

		/// <summary>Creates a <see cref="T:System.Data.DataView" /> for the specified <see cref="T:System.Data.DataTable" />.</summary>
		/// <param name="table">The name of the <see cref="T:System.Data.DataTable" /> to use in the <see cref="T:System.Data.DataView" />.</param>
		/// <returns>A <see cref="T:System.Data.DataView" /> object.</returns>
		public DataView CreateDataView(DataTable table)
		{
			if (this._dataSet == null)
			{
				throw ExceptionBuilder.CanNotUseDataViewManager();
			}
			DataView dataView = new DataView(table);
			dataView.SetDataViewManager(this);
			return dataView;
		}

		/// <summary>Raises the <see cref="E:System.Data.DataViewManager.ListChanged" /> event.</summary>
		/// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs" /> that contains the event data.</param>
		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			try
			{
				ListChangedEventHandler listChanged = this.ListChanged;
				if (listChanged != null)
				{
					listChanged(this, e);
				}
			}
			catch (Exception e2) when (ADP.IsCatchableExceptionType(e2))
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e2);
			}
		}

		/// <summary>Raises a <see cref="E:System.Data.DataTableCollection.CollectionChanged" /> event when a <see cref="T:System.Data.DataTable" /> is added to or removed from the <see cref="T:System.Data.DataTableCollection" />.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.CollectionChangeEventArgs" /> that contains the event data.</param>
		protected virtual void TableCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			PropertyDescriptor propDesc = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataTablePropertyDescriptor((DataTable)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, propDesc) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataTablePropertyDescriptor((DataTable)e.Element)) : null)));
		}

		/// <summary>Raises a <see cref="E:System.Data.DataRelationCollection.CollectionChanged" /> event when a <see cref="T:System.Data.DataRelation" /> is added to or removed from the <see cref="T:System.Data.DataRelationCollection" />.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.CollectionChangeEventArgs" /> that contains the event data.</param>
		protected virtual void RelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataRelationPropertyDescriptor propDesc = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, propDesc) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : null)));
		}

		private DataViewSettingCollection _dataViewSettingsCollection;

		private DataSet _dataSet;

		private DataViewManagerListItemTypeDescriptor _item;

		private bool _locked;

		internal int _nViews;

		private static NotSupportedException s_notSupported = new NotSupportedException();
	}
}
