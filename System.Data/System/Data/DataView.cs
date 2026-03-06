using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Threading;

namespace System.Data
{
	/// <summary>Represents a databindable, customized view of a <see cref="T:System.Data.DataTable" /> for sorting, filtering, searching, editing, and navigation. The <see cref="T:System.Data.DataView" /> does not store data, but instead represents a connected view of its corresponding <see cref="T:System.Data.DataTable" />. Changes to the <see cref="T:System.Data.DataView" />'s data will affect the <see cref="T:System.Data.DataTable" />. Changes to the <see cref="T:System.Data.DataTable" />'s data will affect all <see cref="T:System.Data.DataView" />s associated with it.</summary>
	[DefaultProperty("Table")]
	[DefaultEvent("PositionChanged")]
	public class DataView : MarshalByValueComponent, IBindingListView, IBindingList, IList, ICollection, IEnumerable, ITypedList, ISupportInitializeNotification, ISupportInitialize
	{
		internal DataView(DataTable table, bool locked)
		{
			GC.SuppressFinalize(this);
			DataCommonEventSource.Log.Trace<int, int, bool>("<ds.DataView.DataView|INFO> {0}, table={1}, locked={2}", this.ObjectID, (table != null) ? table.ObjectID : 0, locked);
			this._dvListener = new DataViewListener(this);
			this._locked = locked;
			this._table = table;
			this._dvListener.RegisterMetaDataEvents(this._table);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataView" /> class.</summary>
		public DataView() : this(null)
		{
			this.SetIndex2("", DataViewRowState.CurrentRows, null, true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataView" /> class with the specified <see cref="T:System.Data.DataTable" />.</summary>
		/// <param name="table">A <see cref="T:System.Data.DataTable" /> to add to the <see cref="T:System.Data.DataView" />.</param>
		public DataView(DataTable table) : this(table, false)
		{
			this.SetIndex2("", DataViewRowState.CurrentRows, null, true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataView" /> class with the specified <see cref="T:System.Data.DataTable" />, <see cref="P:System.Data.DataView.RowFilter" />, <see cref="P:System.Data.DataView.Sort" />, and <see cref="T:System.Data.DataViewRowState" />.</summary>
		/// <param name="table">A <see cref="T:System.Data.DataTable" /> to add to the <see cref="T:System.Data.DataView" />.</param>
		/// <param name="RowFilter">A <see cref="P:System.Data.DataView.RowFilter" /> to apply to the <see cref="T:System.Data.DataView" />.</param>
		/// <param name="Sort">A <see cref="P:System.Data.DataView.Sort" /> to apply to the <see cref="T:System.Data.DataView" />.</param>
		/// <param name="RowState">A <see cref="T:System.Data.DataViewRowState" /> to apply to the <see cref="T:System.Data.DataView" />.</param>
		public DataView(DataTable table, string RowFilter, string Sort, DataViewRowState RowState)
		{
			GC.SuppressFinalize(this);
			DataCommonEventSource.Log.Trace<int, int, string, string, DataViewRowState>("<ds.DataView.DataView|API> {0}, table={1}, RowFilter='{2}', Sort='{3}', RowState={4}", this.ObjectID, (table != null) ? table.ObjectID : 0, RowFilter, Sort, RowState);
			if (table == null)
			{
				throw ExceptionBuilder.CanNotUse();
			}
			this._dvListener = new DataViewListener(this);
			this._locked = false;
			this._table = table;
			this._dvListener.RegisterMetaDataEvents(this._table);
			if ((RowState & ~(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.ModifiedOriginal)) != DataViewRowState.None)
			{
				throw ExceptionBuilder.RecordStateRange();
			}
			if ((RowState & DataViewRowState.ModifiedOriginal) != DataViewRowState.None && (RowState & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
			{
				throw ExceptionBuilder.SetRowStateFilter();
			}
			if (Sort == null)
			{
				Sort = string.Empty;
			}
			if (RowFilter == null)
			{
				RowFilter = string.Empty;
			}
			DataExpression newRowFilter = new DataExpression(table, RowFilter);
			this.SetIndex(Sort, RowState, newRowFilter);
		}

		/// <summary>Sets or gets a value that indicates whether deletes are allowed.</summary>
		/// <returns>
		///   <see langword="true" />, if deletes are allowed; otherwise, <see langword="false" />.</returns>
		[DefaultValue(true)]
		public bool AllowDelete
		{
			get
			{
				return this._allowDelete;
			}
			set
			{
				if (this._allowDelete != value)
				{
					this._allowDelete = value;
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		/// <summary>Gets or sets a value that indicates whether to use the default sort. The default sort is (ascending) by all primary keys as specified by <see cref="P:System.Data.DataTable.PrimaryKey" />.</summary>
		/// <returns>
		///   <see langword="true" />, if the default sort is used; otherwise, <see langword="false" />.</returns>
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool ApplyDefaultSort
		{
			get
			{
				return this._applyDefaultSort;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, bool>("<ds.DataView.set_ApplyDefaultSort|API> {0}, {1}", this.ObjectID, value);
				if (this._applyDefaultSort != value)
				{
					this._comparison = null;
					this._applyDefaultSort = value;
					this.UpdateIndex(true);
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		/// <summary>Gets or sets a value that indicates whether edits are allowed.</summary>
		/// <returns>
		///   <see langword="true" />, if edits are allowed; otherwise, <see langword="false" />.</returns>
		[DefaultValue(true)]
		public bool AllowEdit
		{
			get
			{
				return this._allowEdit;
			}
			set
			{
				if (this._allowEdit != value)
				{
					this._allowEdit = value;
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		/// <summary>Gets or sets a value that indicates whether the new rows can be added by using the <see cref="M:System.Data.DataView.AddNew" /> method.</summary>
		/// <returns>
		///   <see langword="true" />, if new rows can be added; otherwise, <see langword="false" />.</returns>
		[DefaultValue(true)]
		public bool AllowNew
		{
			get
			{
				return this._allowNew;
			}
			set
			{
				if (this._allowNew != value)
				{
					this._allowNew = value;
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		/// <summary>Gets the number of records in the <see cref="T:System.Data.DataView" /> after <see cref="P:System.Data.DataView.RowFilter" /> and <see cref="P:System.Data.DataView.RowStateFilter" /> have been applied.</summary>
		/// <returns>The number of records in the <see cref="T:System.Data.DataView" />.</returns>
		[Browsable(false)]
		public int Count
		{
			get
			{
				return this._rowViewCache.Count;
			}
		}

		private int CountFromIndex
		{
			get
			{
				return ((this._index != null) ? this._index.RecordCount : 0) + ((this._addNewRow != null) ? 1 : 0);
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataViewManager" /> associated with this view.</summary>
		/// <returns>The <see langword="DataViewManager" /> that created this view. If this is the default <see cref="T:System.Data.DataView" /> for a <see cref="T:System.Data.DataTable" />, the <see langword="DataViewManager" /> property returns the default <see langword="DataViewManager" /> for the <see langword="DataSet" />. Otherwise, if the <see langword="DataView" /> was created without a <see langword="DataViewManager" />, this property is <see langword="null" />.</returns>
		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return this._dataViewManager;
			}
		}

		/// <summary>Gets a value that indicates whether the component is initialized.</summary>
		/// <returns>
		///   <see langword="true" /> to indicate the component has completed initialization; otherwise, <see langword="false" />.</returns>
		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return !this._fInitInProgress;
			}
		}

		/// <summary>Gets a value that indicates whether the data source is currently open and projecting views of data on the <see cref="T:System.Data.DataTable" />.</summary>
		/// <returns>
		///   <see langword="true" />, if the source is open; otherwise, <see langword="false" />.</returns>
		[Browsable(false)]
		protected bool IsOpen
		{
			get
			{
				return this._open;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets or sets the expression used to filter which rows are viewed in the <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>A string that specifies how rows are to be filtered.</returns>
		[DefaultValue("")]
		public virtual string RowFilter
		{
			get
			{
				DataExpression dataExpression = this._rowFilter as DataExpression;
				if (dataExpression != null)
				{
					return dataExpression.Expression;
				}
				return "";
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataView.set_RowFilter|API> {0}, '{1}'", this.ObjectID, value);
				if (this._fInitInProgress)
				{
					this._delayedRowFilter = value;
					return;
				}
				CultureInfo culture = (this._table != null) ? this._table.Locale : CultureInfo.CurrentCulture;
				if (this._rowFilter == null || string.Compare(this.RowFilter, value, false, culture) != 0)
				{
					DataExpression newRowFilter = new DataExpression(this._table, value);
					this.SetIndex(this._sort, this._recordStates, newRowFilter);
				}
			}
		}

		internal Predicate<DataRow> RowPredicate
		{
			get
			{
				DataView.RowPredicateFilter rowPredicateFilter = this.GetFilter() as DataView.RowPredicateFilter;
				if (rowPredicateFilter == null)
				{
					return null;
				}
				return rowPredicateFilter._predicateFilter;
			}
			set
			{
				if (this.RowPredicate != value)
				{
					this.SetIndex(this.Sort, this.RowStateFilter, (value != null) ? new DataView.RowPredicateFilter(value) : null);
				}
			}
		}

		/// <summary>Gets or sets the row state filter used in the <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>One of the <see cref="T:System.Data.DataViewRowState" /> values.</returns>
		[DefaultValue(DataViewRowState.CurrentRows)]
		public DataViewRowState RowStateFilter
		{
			get
			{
				return this._recordStates;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, DataViewRowState>("<ds.DataView.set_RowStateFilter|API> {0}, {1}", this.ObjectID, value);
				if (this._fInitInProgress)
				{
					this._delayedRecordStates = value;
					return;
				}
				if ((value & ~(DataViewRowState.Unchanged | DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.ModifiedOriginal)) != DataViewRowState.None)
				{
					throw ExceptionBuilder.RecordStateRange();
				}
				if ((value & DataViewRowState.ModifiedOriginal) != DataViewRowState.None && (value & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
				{
					throw ExceptionBuilder.SetRowStateFilter();
				}
				if (this._recordStates != value)
				{
					this.SetIndex(this._sort, value, this._rowFilter);
				}
			}
		}

		/// <summary>Gets or sets the sort column or columns, and sort order for the <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>A string that contains the column name followed by "ASC" (ascending) or "DESC" (descending). Columns are sorted ascending by default. Multiple columns can be separated by commas.</returns>
		[DefaultValue("")]
		public string Sort
		{
			get
			{
				if (this._sort.Length == 0 && this._applyDefaultSort && this._table != null && this._table._primaryIndex.Length != 0)
				{
					return this._table.FormatSortString(this._table._primaryIndex);
				}
				return this._sort;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				DataCommonEventSource.Log.Trace<int, string>("<ds.DataView.set_Sort|API> {0}, '{1}'", this.ObjectID, value);
				if (this._fInitInProgress)
				{
					this._delayedSort = value;
					return;
				}
				CultureInfo culture = (this._table != null) ? this._table.Locale : CultureInfo.CurrentCulture;
				if (string.Compare(this._sort, value, false, culture) != 0 || this._comparison != null)
				{
					this.CheckSort(value);
					this._comparison = null;
					this.SetIndex(value, this._recordStates, this._rowFilter);
				}
			}
		}

		internal Comparison<DataRow> SortComparison
		{
			get
			{
				return this._comparison;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataView.set_SortComparison|API> {0}", this.ObjectID);
				if (this._comparison != value)
				{
					this._comparison = value;
					this.SetIndex("", this._recordStates, this._rowFilter);
				}
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Gets or sets the source <see cref="T:System.Data.DataTable" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that provides the data for this view.</returns>
		[TypeConverter(typeof(DataTableTypeConverter))]
		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public DataTable Table
		{
			get
			{
				return this._table;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int, int>("<ds.DataView.set_Table|API> {0}, {1}", this.ObjectID, (value != null) ? value.ObjectID : 0);
				if (this._fInitInProgress && value != null)
				{
					this._delayedTable = value;
					return;
				}
				if (this._locked)
				{
					throw ExceptionBuilder.SetTable();
				}
				if (this._dataViewManager != null)
				{
					throw ExceptionBuilder.CanNotSetTable();
				}
				if (value != null && value.TableName.Length == 0)
				{
					throw ExceptionBuilder.CanNotBindTable();
				}
				if (this._table != value)
				{
					this._dvListener.UnregisterMetaDataEvents();
					this._table = value;
					if (this._table != null)
					{
						this._dvListener.RegisterMetaDataEvents(this._table);
					}
					this.SetIndex2("", DataViewRowState.CurrentRows, null, false);
					if (this._table != null)
					{
						this.OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, new DataTablePropertyDescriptor(this._table)));
					}
					this.OnListChanged(DataView.s_resetEventArgs);
				}
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.IList.Item(System.Int32)" />.</summary>
		/// <param name="recordIndex">An <see cref="T:System.Int32" /> value.</param>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.IList.Item(System.Int32)" />.</returns>
		object IList.this[int recordIndex]
		{
			get
			{
				return this[recordIndex];
			}
			set
			{
				throw ExceptionBuilder.SetIListObject();
			}
		}

		/// <summary>Gets a row of data from a specified table.</summary>
		/// <param name="recordIndex">The index of a record in the <see cref="T:System.Data.DataTable" />.</param>
		/// <returns>A <see cref="T:System.Data.DataRowView" /> of the row that you want.</returns>
		public DataRowView this[int recordIndex]
		{
			get
			{
				return this.GetRowView(this.GetRow(recordIndex));
			}
		}

		/// <summary>Adds a new row to the <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>A new <see cref="T:System.Data.DataRowView" /> object.</returns>
		public virtual DataRowView AddNew()
		{
			long scopeId = DataCommonEventSource.Log.EnterScope<int>("<ds.DataView.AddNew|API> {0}", this.ObjectID);
			DataRowView result;
			try
			{
				this.CheckOpen();
				if (!this.AllowNew)
				{
					throw ExceptionBuilder.AddNewNotAllowNull();
				}
				if (this._addNewRow != null)
				{
					this._rowViewCache[this._addNewRow].EndEdit();
				}
				this._addNewRow = this._table.NewRow();
				DataRowView dataRowView = new DataRowView(this, this._addNewRow);
				this._rowViewCache.Add(this._addNewRow, dataRowView);
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, this.IndexOf(dataRowView)));
				result = dataRowView;
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(scopeId);
			}
			return result;
		}

		/// <summary>Starts the initialization of a <see cref="T:System.Data.DataView" /> that is used on a form or used by another component. The initialization occurs at runtime.</summary>
		public void BeginInit()
		{
			this._fInitInProgress = true;
		}

		/// <summary>Ends the initialization of a <see cref="T:System.Data.DataView" /> that is used on a form or used by another component. The initialization occurs at runtime.</summary>
		public void EndInit()
		{
			if (this._delayedTable != null && this._delayedTable.fInitInProgress)
			{
				this._delayedTable._delayedViews.Add(this);
				return;
			}
			this._fInitInProgress = false;
			this._fEndInitInProgress = true;
			if (this._delayedTable != null)
			{
				this.Table = this._delayedTable;
				this._delayedTable = null;
			}
			if (this._delayedSort != null)
			{
				this.Sort = this._delayedSort;
				this._delayedSort = null;
			}
			if (this._delayedRowFilter != null)
			{
				this.RowFilter = this._delayedRowFilter;
				this._delayedRowFilter = null;
			}
			if (this._delayedRecordStates != (DataViewRowState)(-1))
			{
				this.RowStateFilter = this._delayedRecordStates;
				this._delayedRecordStates = (DataViewRowState)(-1);
			}
			this._fEndInitInProgress = false;
			this.SetIndex(this.Sort, this.RowStateFilter, this._rowFilter);
			this.OnInitialized();
		}

		private void CheckOpen()
		{
			if (!this.IsOpen)
			{
				throw ExceptionBuilder.NotOpen();
			}
		}

		private void CheckSort(string sort)
		{
			if (this._table == null)
			{
				throw ExceptionBuilder.CanNotUse();
			}
			if (sort.Length == 0)
			{
				return;
			}
			this._table.ParseSortString(sort);
		}

		/// <summary>Closes the <see cref="T:System.Data.DataView" />.</summary>
		protected void Close()
		{
			this._shouldOpen = false;
			this.UpdateIndex();
			this._dvListener.UnregisterMetaDataEvents();
		}

		/// <summary>Copies items into an array. Only for Web Forms Interfaces.</summary>
		/// <param name="array">array to copy into.</param>
		/// <param name="index">index to start at.</param>
		public void CopyTo(Array array, int index)
		{
			checked
			{
				if (this._index != null)
				{
					RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(0);
					while (enumerator.MoveNext())
					{
						int record = enumerator.Current;
						array.SetValue(this.GetRowView(record), index);
						index++;
					}
				}
				if (this._addNewRow != null)
				{
					array.SetValue(this._rowViewCache[this._addNewRow], index);
				}
			}
		}

		private void CopyTo(DataRowView[] array, int index)
		{
			checked
			{
				if (this._index != null)
				{
					RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(0);
					while (enumerator.MoveNext())
					{
						int record = enumerator.Current;
						array[index] = this.GetRowView(record);
						index++;
					}
				}
				if (this._addNewRow != null)
				{
					array[index] = this._rowViewCache[this._addNewRow];
				}
			}
		}

		/// <summary>Deletes a row at the specified index.</summary>
		/// <param name="index">The index of the row to delete.</param>
		public void Delete(int index)
		{
			this.Delete(this.GetRow(index));
		}

		internal void Delete(DataRow row)
		{
			if (row != null)
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int, int>("<ds.DataView.Delete|API> {0}, row={1}", this.ObjectID, row._objectID);
				try
				{
					this.CheckOpen();
					if (row == this._addNewRow)
					{
						this.FinishAddNew(false);
					}
					else
					{
						if (!this.AllowDelete)
						{
							throw ExceptionBuilder.CanNotDelete();
						}
						row.Delete();
					}
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
		}

		/// <summary>Disposes of the resources (other than memory) used by the <see cref="T:System.Data.DataView" /> object.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
			base.Dispose(disposing);
		}

		/// <summary>Finds a row in the <see cref="T:System.Data.DataView" /> by the specified sort key value.</summary>
		/// <param name="key">The object to search for.</param>
		/// <returns>The index of the row in the <see cref="T:System.Data.DataView" /> that contains the sort key value specified; otherwise -1 if the sort key value does not exist.</returns>
		public int Find(object key)
		{
			return this.FindByKey(key);
		}

		internal virtual int FindByKey(object key)
		{
			return this._index.FindRecordByKey(key);
		}

		/// <summary>Finds a row in the <see cref="T:System.Data.DataView" /> by the specified sort key values.</summary>
		/// <param name="key">An array of values, typed as <see cref="T:System.Object" />.</param>
		/// <returns>The index of the position of the first row in the <see cref="T:System.Data.DataView" /> that matches the sort key values specified; otherwise -1 if there are no matching sort key values.</returns>
		public int Find(object[] key)
		{
			return this.FindByKey(key);
		}

		internal virtual int FindByKey(object[] key)
		{
			return this._index.FindRecordByKey(key);
		}

		/// <summary>Returns an array of <see cref="T:System.Data.DataRowView" /> objects whose columns match the specified sort key value.</summary>
		/// <param name="key">The column value, typed as <see cref="T:System.Object" />, to search for.</param>
		/// <returns>An array of <see langword="DataRowView" /> objects whose columns match the specified sort key value; or, if no rows contain the specified sort key values, an empty <see langword="DataRowView" /> array.</returns>
		public DataRowView[] FindRows(object key)
		{
			return this.FindRowsByKey(new object[]
			{
				key
			});
		}

		/// <summary>Returns an array of <see cref="T:System.Data.DataRowView" /> objects whose columns match the specified sort key value.</summary>
		/// <param name="key">An array of column values, typed as <see cref="T:System.Object" />, to search for.</param>
		/// <returns>An array of <see langword="DataRowView" /> objects whose columns match the specified sort key value; or, if no rows contain the specified sort key values, an empty <see langword="DataRowView" /> array.</returns>
		public DataRowView[] FindRows(object[] key)
		{
			return this.FindRowsByKey(key);
		}

		internal virtual DataRowView[] FindRowsByKey(object[] key)
		{
			long scopeId = DataCommonEventSource.Log.EnterScope<int>("<ds.DataView.FindRows|API> {0}", this.ObjectID);
			DataRowView[] dataRowViewFromRange;
			try
			{
				Range range = this._index.FindRecords(key);
				dataRowViewFromRange = this.GetDataRowViewFromRange(range);
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(scopeId);
			}
			return dataRowViewFromRange;
		}

		internal DataRowView[] GetDataRowViewFromRange(Range range)
		{
			if (range.IsNull)
			{
				return Array.Empty<DataRowView>();
			}
			DataRowView[] array = new DataRowView[range.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this[i + range.Min];
			}
			return array;
		}

		internal void FinishAddNew(bool success)
		{
			DataCommonEventSource.Log.Trace<int, bool>("<ds.DataView.FinishAddNew|INFO> {0}, success={1}", this.ObjectID, success);
			DataRow addNewRow = this._addNewRow;
			if (success)
			{
				if (DataRowState.Detached == addNewRow.RowState)
				{
					this._table.Rows.Add(addNewRow);
				}
				else
				{
					addNewRow.EndEdit();
				}
			}
			if (addNewRow == this._addNewRow)
			{
				this._rowViewCache.Remove(this._addNewRow);
				this._addNewRow = null;
				if (!success)
				{
					addNewRow.CancelEdit();
				}
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, this.Count));
			}
		}

		/// <summary>Gets an enumerator for this <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for navigating through the list.</returns>
		public IEnumerator GetEnumerator()
		{
			DataRowView[] array = new DataRowView[this.Count];
			this.CopyTo(array, 0);
			return array.GetEnumerator();
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.IList.IsReadOnly" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.IList.IsReadOnly" />.</returns>
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Collections.IList.IsFixedSize" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.Collections.IList.IsFixedSize" />.</returns>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.Add(System.Object)" />.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> value.</param>
		/// <returns>For a description of this member, see <see cref="M:System.Collections.IList.Add(System.Object)" />.</returns>
		int IList.Add(object value)
		{
			if (value == null)
			{
				this.AddNew();
				return this.Count - 1;
			}
			throw ExceptionBuilder.AddExternalObject();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.Clear" />.</summary>
		void IList.Clear()
		{
			throw ExceptionBuilder.CanNotClear();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.Contains(System.Object)" />.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> value.</param>
		/// <returns>For a description of this member, see <see cref="M:System.Collections.IList.Contains(System.Object)" />.</returns>
		bool IList.Contains(object value)
		{
			return 0 <= this.IndexOf(value as DataRowView);
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.IndexOf(System.Object)" />.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> value.</param>
		/// <returns>For a description of this member, see <see cref="M:System.Collections.IList.IndexOf(System.Object)" />.</returns>
		int IList.IndexOf(object value)
		{
			return this.IndexOf(value as DataRowView);
		}

		internal int IndexOf(DataRowView rowview)
		{
			if (rowview != null)
			{
				if (this._addNewRow == rowview.Row)
				{
					return this.Count - 1;
				}
				DataRowView dataRowView;
				if (this._index != null && DataRowState.Detached != rowview.Row.RowState && this._rowViewCache.TryGetValue(rowview.Row, out dataRowView) && dataRowView == rowview)
				{
					return this.IndexOfDataRowView(rowview);
				}
			}
			return -1;
		}

		private int IndexOfDataRowView(DataRowView rowview)
		{
			return this._index.GetIndex(rowview.Row.GetRecordFromVersion(rowview.Row.GetDefaultRowVersion(this.RowStateFilter) & (DataRowVersion)(-1025)));
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.Insert(System.Int32,System.Object)" />.</summary>
		/// <param name="index">An <see cref="T:System.Int32" /> value.</param>
		/// <param name="value">An <see cref="T:System.Object" /> value to be inserted.</param>
		void IList.Insert(int index, object value)
		{
			throw ExceptionBuilder.InsertExternalObject();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.Remove(System.Object)" />.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> value.</param>
		void IList.Remove(object value)
		{
			int num = this.IndexOf(value as DataRowView);
			if (0 <= num)
			{
				((IList)this).RemoveAt(num);
				return;
			}
			throw ExceptionBuilder.RemoveExternalObject();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Collections.IList.RemoveAt(System.Int32)" />.</summary>
		/// <param name="index">An <see cref="T:System.Int32" /> value.</param>
		void IList.RemoveAt(int index)
		{
			this.Delete(index);
		}

		internal Index GetFindIndex(string column, bool keepIndex)
		{
			if (this._findIndexes == null)
			{
				this._findIndexes = new Dictionary<string, Index>();
			}
			Index index;
			if (this._findIndexes.TryGetValue(column, out index))
			{
				if (!keepIndex)
				{
					this._findIndexes.Remove(column);
					index.RemoveRef();
					if (index.RefCount == 1)
					{
						index.RemoveRef();
					}
				}
			}
			else if (keepIndex)
			{
				index = this._table.GetIndex(column, this._recordStates, this.GetFilter());
				this._findIndexes[column] = index;
				index.AddRef();
			}
			return index;
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowNew" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowNew" />.</returns>
		bool IBindingList.AllowNew
		{
			get
			{
				return this.AllowNew;
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.AddNew" />.</summary>
		/// <returns>The item added to the list.</returns>
		object IBindingList.AddNew()
		{
			return this.AddNew();
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowEdit" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowEdit" />.</returns>
		bool IBindingList.AllowEdit
		{
			get
			{
				return this.AllowEdit;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowRemove" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.AllowRemove" />.</returns>
		bool IBindingList.AllowRemove
		{
			get
			{
				return this.AllowDelete;
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
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" />.</returns>
		bool IBindingList.SupportsSorting
		{
			get
			{
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.IsSorted" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.IsSorted" />.</returns>
		bool IBindingList.IsSorted
		{
			get
			{
				return this.Sort.Length != 0;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortProperty" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortProperty" />.</returns>
		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				return this.GetSortProperty();
			}
		}

		internal PropertyDescriptor GetSortProperty()
		{
			if (this._table != null && this._index != null && this._index._indexFields.Length == 1)
			{
				return new DataColumnPropertyDescriptor(this._index._indexFields[0].Column);
			}
			return null;
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortDirection" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingList.SortDirection" />.</returns>
		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				if (this._index._indexFields.Length != 1 || !this._index._indexFields[0].IsDescending)
				{
					return ListSortDirection.Ascending;
				}
				return ListSortDirection.Descending;
			}
		}

		/// <summary>Occurs when the list managed by the <see cref="T:System.Data.DataView" /> changes.</summary>
		public event ListChangedEventHandler ListChanged
		{
			add
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataView.add_ListChanged|API> {0}", this.ObjectID);
				this._onListChanged = (ListChangedEventHandler)Delegate.Combine(this._onListChanged, value);
			}
			remove
			{
				DataCommonEventSource.Log.Trace<int>("<ds.DataView.remove_ListChanged|API> {0}", this.ObjectID);
				this._onListChanged = (ListChangedEventHandler)Delegate.Remove(this._onListChanged, value);
			}
		}

		/// <summary>Occurs when initialization of the <see cref="T:System.Data.DataView" /> is completed.</summary>
		public event EventHandler Initialized;

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.AddIndex(System.ComponentModel.PropertyDescriptor)" />.</summary>
		/// <param name="property">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> object.</param>
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			this.GetFindIndex(property.Name, true);
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" />.</summary>
		/// <param name="property">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> object.</param>
		/// <param name="direction">A <see cref="T:System.ComponentModel.ListSortDirection" /> object.</param>
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			this.Sort = this.CreateSortString(property, direction);
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.Find(System.ComponentModel.PropertyDescriptor,System.Object)" />.</summary>
		/// <param name="property">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> object.</param>
		/// <param name="key">An <see cref="T:System.Object" /> value.</param>
		/// <returns>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.Find(System.ComponentModel.PropertyDescriptor,System.Object)" />.</returns>
		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			if (property != null)
			{
				bool flag = false;
				Index index = null;
				try
				{
					if (this._findIndexes == null || !this._findIndexes.TryGetValue(property.Name, out index))
					{
						flag = true;
						index = this._table.GetIndex(property.Name, this._recordStates, this.GetFilter());
						index.AddRef();
					}
					Range range = index.FindRecords(key);
					if (!range.IsNull)
					{
						return this._index.GetIndex(index.GetRecord(range.Min));
					}
				}
				finally
				{
					if (flag && index != null)
					{
						index.RemoveRef();
						if (index.RefCount == 1)
						{
							index.RemoveRef();
						}
					}
				}
				return -1;
			}
			return -1;
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.RemoveIndex(System.ComponentModel.PropertyDescriptor)" />.</summary>
		/// <param name="property">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> object.</param>
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			this.GetFindIndex(property.Name, false);
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingList.RemoveSort" />.</summary>
		void IBindingList.RemoveSort()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.DataView.RemoveSort|API> {0}", this.ObjectID);
			this.Sort = string.Empty;
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingListView.ApplySort(System.ComponentModel.ListSortDescriptionCollection)" />.</summary>
		/// <param name="sorts">A <see cref="T:System.ComponentModel.ListSortDescriptionCollection" /> object.</param>
		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
			if (sorts == null)
			{
				throw ExceptionBuilder.ArgumentNull("sorts");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (object obj in ((IEnumerable)sorts))
			{
				ListSortDescription listSortDescription = (ListSortDescription)obj;
				if (listSortDescription == null)
				{
					throw ExceptionBuilder.ArgumentContainsNull("sorts");
				}
				PropertyDescriptor propertyDescriptor = listSortDescription.PropertyDescriptor;
				if (propertyDescriptor == null)
				{
					throw ExceptionBuilder.ArgumentNull("PropertyDescriptor");
				}
				if (!this._table.Columns.Contains(propertyDescriptor.Name))
				{
					throw ExceptionBuilder.ColumnToSortIsOutOfRange(propertyDescriptor.Name);
				}
				ListSortDirection sortDirection = listSortDescription.SortDirection;
				if (flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(this.CreateSortString(propertyDescriptor, sortDirection));
				if (!flag)
				{
					flag = true;
				}
			}
			this.Sort = stringBuilder.ToString();
		}

		private string CreateSortString(PropertyDescriptor property, ListSortDirection direction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			stringBuilder.Append(property.Name);
			stringBuilder.Append(']');
			if (ListSortDirection.Descending == direction)
			{
				stringBuilder.Append(" DESC");
			}
			return stringBuilder.ToString();
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.IBindingListView.RemoveFilter" />.</summary>
		void IBindingListView.RemoveFilter()
		{
			DataCommonEventSource.Log.Trace<int>("<ds.DataView.RemoveFilter|API> {0}", this.ObjectID);
			this.RowFilter = string.Empty;
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.Filter" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.Filter" />.</returns>
		string IBindingListView.Filter
		{
			get
			{
				return this.RowFilter;
			}
			set
			{
				this.RowFilter = value;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.SortDescriptions" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.SortDescriptions" />.</returns>
		ListSortDescriptionCollection IBindingListView.SortDescriptions
		{
			get
			{
				return this.GetSortDescriptions();
			}
		}

		internal ListSortDescriptionCollection GetSortDescriptions()
		{
			ListSortDescription[] array = Array.Empty<ListSortDescription>();
			if (this._table != null && this._index != null && this._index._indexFields.Length != 0)
			{
				array = new ListSortDescription[this._index._indexFields.Length];
				for (int i = 0; i < this._index._indexFields.Length; i++)
				{
					DataColumnPropertyDescriptor property = new DataColumnPropertyDescriptor(this._index._indexFields[i].Column);
					if (this._index._indexFields[i].IsDescending)
					{
						array[i] = new ListSortDescription(property, ListSortDirection.Descending);
					}
					else
					{
						array[i] = new ListSortDescription(property, ListSortDirection.Ascending);
					}
				}
			}
			return new ListSortDescriptionCollection(array);
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.SupportsAdvancedSorting" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.SupportsAdvancedSorting" />.</returns>
		bool IBindingListView.SupportsAdvancedSorting
		{
			get
			{
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.SupportsFiltering" />.</summary>
		/// <returns>For a description of this member, see <see cref="P:System.ComponentModel.IBindingListView.SupportsFiltering" />.</returns>
		bool IBindingListView.SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.ITypedList.GetListName(System.ComponentModel.PropertyDescriptor[])" />.</summary>
		/// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects.</param>
		/// <returns>For a description of this member, see <see cref="M:System.ComponentModel.ITypedList.GetListName(System.ComponentModel.PropertyDescriptor[])" />.</returns>
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (this._table != null)
			{
				if (listAccessors == null || listAccessors.Length == 0)
				{
					return this._table.TableName;
				}
				DataSet dataSet = this._table.DataSet;
				if (dataSet != null)
				{
					DataTable dataTable = dataSet.FindTable(this._table, listAccessors, 0);
					if (dataTable != null)
					{
						return dataTable.TableName;
					}
				}
			}
			return string.Empty;
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.ITypedList.GetItemProperties(System.ComponentModel.PropertyDescriptor[])" />.</summary>
		/// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects to find in the collection as bindable. This can be <see langword="null" />.</param>
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (this._table != null)
			{
				if (listAccessors == null || listAccessors.Length == 0)
				{
					return this._table.GetPropertyDescriptorCollection(null);
				}
				DataSet dataSet = this._table.DataSet;
				if (dataSet == null)
				{
					return new PropertyDescriptorCollection(null);
				}
				DataTable dataTable = dataSet.FindTable(this._table, listAccessors, 0);
				if (dataTable != null)
				{
					return dataTable.GetPropertyDescriptorCollection(null);
				}
			}
			return new PropertyDescriptorCollection(null);
		}

		internal virtual IFilter GetFilter()
		{
			return this._rowFilter;
		}

		private int GetRecord(int recordIndex)
		{
			if (this.Count <= recordIndex)
			{
				throw ExceptionBuilder.RowOutOfRange(recordIndex);
			}
			if (recordIndex != this._index.RecordCount)
			{
				return this._index.GetRecord(recordIndex);
			}
			return this._addNewRow.GetDefaultRecord();
		}

		internal DataRow GetRow(int index)
		{
			int count = this.Count;
			if (count <= index)
			{
				throw ExceptionBuilder.GetElementIndex(index);
			}
			if (index == count - 1 && this._addNewRow != null)
			{
				return this._addNewRow;
			}
			return this._table._recordManager[this.GetRecord(index)];
		}

		private DataRowView GetRowView(int record)
		{
			return this.GetRowView(this._table._recordManager[record]);
		}

		private DataRowView GetRowView(DataRow dr)
		{
			return this._rowViewCache[dr];
		}

		/// <summary>Occurs after a <see cref="T:System.Data.DataView" /> has been changed successfully.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs" /> that contains the event data.</param>
		protected virtual void IndexListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType != ListChangedType.Reset)
			{
				this.OnListChanged(e);
			}
			if (this._addNewRow != null && this._index.RecordCount == 0)
			{
				this.FinishAddNew(false);
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				this.OnListChanged(e);
			}
		}

		internal void IndexListChangedInternal(ListChangedEventArgs e)
		{
			this._rowViewBuffer.Clear();
			if (ListChangedType.ItemAdded == e.ListChangedType && this._addNewMoved != null && this._addNewMoved.NewIndex != this._addNewMoved.OldIndex)
			{
				ListChangedEventArgs addNewMoved = this._addNewMoved;
				this._addNewMoved = null;
				this.IndexListChanged(this, addNewMoved);
			}
			this.IndexListChanged(this, e);
		}

		internal void MaintainDataView(ListChangedType changedType, DataRow row, bool trackAddRemove)
		{
			DataRowView dataRowView = null;
			switch (changedType)
			{
			case ListChangedType.Reset:
				this.ResetRowViewCache();
				break;
			case ListChangedType.ItemAdded:
				if (trackAddRemove && this._rowViewBuffer.TryGetValue(row, out dataRowView))
				{
					this._rowViewBuffer.Remove(row);
				}
				if (row == this._addNewRow)
				{
					int newIndex = this.IndexOfDataRowView(this._rowViewCache[this._addNewRow]);
					this._addNewRow = null;
					this._addNewMoved = new ListChangedEventArgs(ListChangedType.ItemMoved, newIndex, this.Count - 1);
					return;
				}
				if (!this._rowViewCache.ContainsKey(row))
				{
					this._rowViewCache.Add(row, dataRowView ?? new DataRowView(this, row));
					return;
				}
				break;
			case ListChangedType.ItemDeleted:
				if (trackAddRemove)
				{
					this._rowViewCache.TryGetValue(row, out dataRowView);
					if (dataRowView != null)
					{
						this._rowViewBuffer.Add(row, dataRowView);
					}
				}
				this._rowViewCache.Remove(row);
				return;
			case ListChangedType.ItemMoved:
			case ListChangedType.ItemChanged:
			case ListChangedType.PropertyDescriptorAdded:
			case ListChangedType.PropertyDescriptorDeleted:
			case ListChangedType.PropertyDescriptorChanged:
				break;
			default:
				return;
			}
		}

		/// <summary>Raises the <see cref="E:System.Data.DataView.ListChanged" /> event.</summary>
		/// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs" /> that contains the event data.</param>
		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			DataCommonEventSource.Log.Trace<int, ListChangedType>("<ds.DataView.OnListChanged|INFO> {0}, ListChangedType={1}", this.ObjectID, e.ListChangedType);
			try
			{
				DataColumn dataColumn = null;
				string text = null;
				switch (e.ListChangedType)
				{
				case ListChangedType.ItemMoved:
				case ListChangedType.ItemChanged:
					if (0 <= e.NewIndex)
					{
						DataRow row = this.GetRow(e.NewIndex);
						if (row.HasPropertyChanged)
						{
							dataColumn = row.LastChangedColumn;
							text = ((dataColumn != null) ? dataColumn.ColumnName : string.Empty);
						}
					}
					break;
				}
				if (this._onListChanged != null)
				{
					if (dataColumn != null && e.NewIndex == e.OldIndex)
					{
						ListChangedEventArgs e2 = new ListChangedEventArgs(e.ListChangedType, e.NewIndex, new DataColumnPropertyDescriptor(dataColumn));
						this._onListChanged(this, e2);
					}
					else
					{
						this._onListChanged(this, e);
					}
				}
				if (text != null)
				{
					this[e.NewIndex].RaisePropertyChangedEvent(text);
				}
			}
			catch (Exception e3) when (ADP.IsCatchableExceptionType(e3))
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e3);
			}
		}

		private void OnInitialized()
		{
			EventHandler initialized = this.Initialized;
			if (initialized == null)
			{
				return;
			}
			initialized(this, EventArgs.Empty);
		}

		/// <summary>Opens a <see cref="T:System.Data.DataView" />.</summary>
		protected void Open()
		{
			this._shouldOpen = true;
			this.UpdateIndex();
			this._dvListener.RegisterMetaDataEvents(this._table);
		}

		/// <summary>Reserved for internal use only.</summary>
		protected void Reset()
		{
			if (this.IsOpen)
			{
				this._index.Reset();
			}
		}

		internal void ResetRowViewCache()
		{
			Dictionary<DataRow, DataRowView> dictionary = new Dictionary<DataRow, DataRowView>(this.CountFromIndex, DataView.DataRowReferenceComparer.s_default);
			if (this._index != null)
			{
				RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(0);
				while (enumerator.MoveNext())
				{
					int record = enumerator.Current;
					DataRow dataRow = this._table._recordManager[record];
					DataRowView value;
					if (!this._rowViewCache.TryGetValue(dataRow, out value))
					{
						value = new DataRowView(this, dataRow);
					}
					dictionary.Add(dataRow, value);
				}
			}
			if (this._addNewRow != null)
			{
				DataRowView value;
				this._rowViewCache.TryGetValue(this._addNewRow, out value);
				dictionary.Add(this._addNewRow, value);
			}
			this._rowViewCache = dictionary;
		}

		internal void SetDataViewManager(DataViewManager dataViewManager)
		{
			if (this._table == null)
			{
				throw ExceptionBuilder.CanNotUse();
			}
			if (this._dataViewManager != dataViewManager)
			{
				if (dataViewManager != null)
				{
					dataViewManager._nViews--;
				}
				this._dataViewManager = dataViewManager;
				if (dataViewManager != null)
				{
					dataViewManager._nViews++;
					DataViewSetting dataViewSetting = dataViewManager.DataViewSettings[this._table];
					try
					{
						this._applyDefaultSort = dataViewSetting.ApplyDefaultSort;
						DataExpression newRowFilter = new DataExpression(this._table, dataViewSetting.RowFilter);
						this.SetIndex(dataViewSetting.Sort, dataViewSetting.RowStateFilter, newRowFilter);
					}
					catch (Exception e) when (ADP.IsCatchableExceptionType(e))
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(e);
					}
					this._locked = true;
					return;
				}
				this.SetIndex("", DataViewRowState.CurrentRows, null);
			}
		}

		internal virtual void SetIndex(string newSort, DataViewRowState newRowStates, IFilter newRowFilter)
		{
			this.SetIndex2(newSort, newRowStates, newRowFilter, true);
		}

		internal void SetIndex2(string newSort, DataViewRowState newRowStates, IFilter newRowFilter, bool fireEvent)
		{
			DataCommonEventSource.Log.Trace<int, string, DataViewRowState>("<ds.DataView.SetIndex|INFO> {0}, newSort='{1}', newRowStates={2}", this.ObjectID, newSort, newRowStates);
			this._sort = newSort;
			this._recordStates = newRowStates;
			this._rowFilter = newRowFilter;
			if (this._fEndInitInProgress)
			{
				return;
			}
			if (fireEvent)
			{
				this.UpdateIndex(true);
			}
			else
			{
				this.UpdateIndex(true, false);
			}
			if (this._findIndexes != null)
			{
				Dictionary<string, Index> findIndexes = this._findIndexes;
				this._findIndexes = null;
				foreach (KeyValuePair<string, Index> keyValuePair in findIndexes)
				{
					keyValuePair.Value.RemoveRef();
				}
			}
		}

		/// <summary>Reserved for internal use only.</summary>
		protected void UpdateIndex()
		{
			this.UpdateIndex(false);
		}

		/// <summary>Reserved for internal use only.</summary>
		/// <param name="force">Reserved for internal use only.</param>
		protected virtual void UpdateIndex(bool force)
		{
			this.UpdateIndex(force, true);
		}

		internal void UpdateIndex(bool force, bool fireEvent)
		{
			long scopeId = DataCommonEventSource.Log.EnterScope<int, bool>("<ds.DataView.UpdateIndex|INFO> {0}, force={1}", this.ObjectID, force);
			try
			{
				if (this._open != this._shouldOpen || force)
				{
					this._open = this._shouldOpen;
					Index index = null;
					if (this._open && this._table != null)
					{
						if (this.SortComparison != null)
						{
							index = new Index(this._table, this.SortComparison, this._recordStates, this.GetFilter());
							index.AddRef();
						}
						else
						{
							index = this._table.GetIndex(this.Sort, this._recordStates, this.GetFilter());
						}
					}
					if (this._index != index)
					{
						if (this._index == null)
						{
							DataTable table = index.Table;
						}
						else
						{
							DataTable table2 = this._index.Table;
						}
						if (this._index != null)
						{
							this._dvListener.UnregisterListChangedEvent();
						}
						this._index = index;
						if (this._index != null)
						{
							this._dvListener.RegisterListChangedEvent(this._index);
						}
						this.ResetRowViewCache();
						if (fireEvent)
						{
							this.OnListChanged(DataView.s_resetEventArgs);
						}
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(scopeId);
			}
		}

		internal void ChildRelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataRelationPropertyDescriptor propDesc = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, propDesc) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : null)));
		}

		internal void ParentRelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataRelationPropertyDescriptor propDesc = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, propDesc) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataRelationPropertyDescriptor((DataRelation)e.Element)) : null)));
		}

		/// <summary>Occurs after a <see cref="T:System.Data.DataColumnCollection" /> has been changed successfully.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs" /> that contains the event data.</param>
		protected virtual void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			DataColumnPropertyDescriptor propDesc = null;
			this.OnListChanged((e.Action == CollectionChangeAction.Add) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, new DataColumnPropertyDescriptor((DataColumn)e.Element)) : ((e.Action == CollectionChangeAction.Refresh) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, propDesc) : ((e.Action == CollectionChangeAction.Remove) ? new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, new DataColumnPropertyDescriptor((DataColumn)e.Element)) : null)));
		}

		internal void ColumnCollectionChangedInternal(object sender, CollectionChangeEventArgs e)
		{
			this.ColumnCollectionChanged(sender, e);
		}

		/// <summary>Creates and returns a new <see cref="T:System.Data.DataTable" /> based on rows in an existing <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>A new <see cref="T:System.Data.DataTable" /> instance that contains the requested rows and columns.</returns>
		public DataTable ToTable()
		{
			return this.ToTable(null, false, Array.Empty<string>());
		}

		/// <summary>Creates and returns a new <see cref="T:System.Data.DataTable" /> based on rows in an existing <see cref="T:System.Data.DataView" />.</summary>
		/// <param name="tableName">The name of the returned <see cref="T:System.Data.DataTable" />.</param>
		/// <returns>A new <see cref="T:System.Data.DataTable" /> instance that contains the requested rows and columns.</returns>
		public DataTable ToTable(string tableName)
		{
			return this.ToTable(tableName, false, Array.Empty<string>());
		}

		/// <summary>Creates and returns a new <see cref="T:System.Data.DataTable" /> based on rows in an existing <see cref="T:System.Data.DataView" />.</summary>
		/// <param name="distinct">If <see langword="true" />, the returned <see cref="T:System.Data.DataTable" /> contains rows that have distinct values for all its columns. The default value is <see langword="false" />.</param>
		/// <param name="columnNames">A string array that contains a list of the column names to be included in the returned <see cref="T:System.Data.DataTable" />. The <see cref="T:System.Data.DataTable" /> contains the specified columns in the order they appear within this array.</param>
		/// <returns>A new <see cref="T:System.Data.DataTable" /> instance that contains the requested rows and columns.</returns>
		public DataTable ToTable(bool distinct, params string[] columnNames)
		{
			return this.ToTable(null, distinct, columnNames);
		}

		/// <summary>Creates and returns a new <see cref="T:System.Data.DataTable" /> based on rows in an existing <see cref="T:System.Data.DataView" />.</summary>
		/// <param name="tableName">The name of the returned <see cref="T:System.Data.DataTable" />.</param>
		/// <param name="distinct">If <see langword="true" />, the returned <see cref="T:System.Data.DataTable" /> contains rows that have distinct values for all its columns. The default value is <see langword="false" />.</param>
		/// <param name="columnNames">A string array that contains a list of the column names to be included in the returned <see cref="T:System.Data.DataTable" />. The <see langword="DataTable" /> contains the specified columns in the order they appear within this array.</param>
		/// <returns>A new <see cref="T:System.Data.DataTable" /> instance that contains the requested rows and columns.</returns>
		public DataTable ToTable(string tableName, bool distinct, params string[] columnNames)
		{
			DataCommonEventSource.Log.Trace<int, string, bool>("<ds.DataView.ToTable|API> {0}, TableName='{1}', distinct={2}", this.ObjectID, tableName, distinct);
			if (columnNames == null)
			{
				throw ExceptionBuilder.ArgumentNull("columnNames");
			}
			DataTable dataTable = new DataTable();
			dataTable.Locale = this._table.Locale;
			dataTable.CaseSensitive = this._table.CaseSensitive;
			dataTable.TableName = ((tableName != null) ? tableName : this._table.TableName);
			dataTable.Namespace = this._table.Namespace;
			dataTable.Prefix = this._table.Prefix;
			if (columnNames.Length == 0)
			{
				columnNames = new string[this.Table.Columns.Count];
				for (int i = 0; i < columnNames.Length; i++)
				{
					columnNames[i] = this.Table.Columns[i].ColumnName;
				}
			}
			int[] array = new int[columnNames.Length];
			List<object[]> list = new List<object[]>();
			for (int j = 0; j < columnNames.Length; j++)
			{
				DataColumn dataColumn = this.Table.Columns[columnNames[j]];
				if (dataColumn == null)
				{
					throw ExceptionBuilder.ColumnNotInTheUnderlyingTable(columnNames[j], this.Table.TableName);
				}
				dataTable.Columns.Add(dataColumn.Clone());
				array[j] = this.Table.Columns.IndexOf(dataColumn);
			}
			foreach (object obj in this)
			{
				DataRowView dataRowView = (DataRowView)obj;
				object[] array2 = new object[columnNames.Length];
				for (int k = 0; k < array.Length; k++)
				{
					array2[k] = dataRowView[array[k]];
				}
				if (!distinct || !this.RowExist(list, array2))
				{
					dataTable.Rows.Add(array2);
					list.Add(array2);
				}
			}
			return dataTable;
		}

		private bool RowExist(List<object[]> arraylist, object[] objectArray)
		{
			for (int i = 0; i < arraylist.Count; i++)
			{
				object[] array = arraylist[i];
				bool flag = true;
				for (int j = 0; j < objectArray.Length; j++)
				{
					flag &= array[j].Equals(objectArray[j]);
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Determines whether the specified <see cref="T:System.Data.DataView" /> instances are considered equal.</summary>
		/// <param name="view">The <see cref="T:System.Data.DataView" /> to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Data.DataView" /> instances are equal; otherwise, <see langword="false" />.</returns>
		public virtual bool Equals(DataView view)
		{
			return view != null && this.Table == view.Table && this.Count == view.Count && string.Equals(this.RowFilter, view.RowFilter, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Sort, view.Sort, StringComparison.OrdinalIgnoreCase) && this.SortComparison == view.SortComparison && this.RowPredicate == view.RowPredicate && this.RowStateFilter == view.RowStateFilter && this.DataViewManager == view.DataViewManager && this.AllowDelete == view.AllowDelete && this.AllowNew == view.AllowNew && this.AllowEdit == view.AllowEdit;
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		private DataViewManager _dataViewManager;

		private DataTable _table;

		private bool _locked;

		private Index _index;

		private Dictionary<string, Index> _findIndexes;

		private string _sort = string.Empty;

		private Comparison<DataRow> _comparison;

		private IFilter _rowFilter;

		private DataViewRowState _recordStates = DataViewRowState.CurrentRows;

		private bool _shouldOpen = true;

		private bool _open;

		private bool _allowNew = true;

		private bool _allowEdit = true;

		private bool _allowDelete = true;

		private bool _applyDefaultSort;

		internal DataRow _addNewRow;

		private ListChangedEventArgs _addNewMoved;

		private ListChangedEventHandler _onListChanged;

		internal static ListChangedEventArgs s_resetEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1);

		private DataTable _delayedTable;

		private string _delayedRowFilter;

		private string _delayedSort;

		private DataViewRowState _delayedRecordStates = (DataViewRowState)(-1);

		private bool _fInitInProgress;

		private bool _fEndInitInProgress;

		private Dictionary<DataRow, DataRowView> _rowViewCache = new Dictionary<DataRow, DataRowView>(DataView.DataRowReferenceComparer.s_default);

		private readonly Dictionary<DataRow, DataRowView> _rowViewBuffer = new Dictionary<DataRow, DataRowView>(DataView.DataRowReferenceComparer.s_default);

		private DataViewListener _dvListener;

		private static int s_objectTypeCount;

		private readonly int _objectID = Interlocked.Increment(ref DataView.s_objectTypeCount);

		private sealed class DataRowReferenceComparer : IEqualityComparer<DataRow>
		{
			private DataRowReferenceComparer()
			{
			}

			public bool Equals(DataRow x, DataRow y)
			{
				return x == y;
			}

			public int GetHashCode(DataRow obj)
			{
				return obj._objectID;
			}

			internal static readonly DataView.DataRowReferenceComparer s_default = new DataView.DataRowReferenceComparer();
		}

		private sealed class RowPredicateFilter : IFilter
		{
			internal RowPredicateFilter(Predicate<DataRow> predicate)
			{
				this._predicateFilter = predicate;
			}

			bool IFilter.Invoke(DataRow row, DataRowVersion version)
			{
				return this._predicateFilter(row);
			}

			internal readonly Predicate<DataRow> _predicateFilter;
		}
	}
}
