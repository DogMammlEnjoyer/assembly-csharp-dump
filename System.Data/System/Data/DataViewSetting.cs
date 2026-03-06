using System;
using System.ComponentModel;

namespace System.Data
{
	/// <summary>Represents the default settings for <see cref="P:System.Data.DataView.ApplyDefaultSort" />, <see cref="P:System.Data.DataView.DataViewManager" />, <see cref="P:System.Data.DataView.RowFilter" />, <see cref="P:System.Data.DataView.RowStateFilter" />, <see cref="P:System.Data.DataView.Sort" />, and <see cref="P:System.Data.DataView.Table" /> for DataViews created from the <see cref="T:System.Data.DataViewManager" />.</summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DataViewSetting
	{
		internal DataViewSetting()
		{
		}

		/// <summary>Gets or sets a value indicating whether to use the default sort.</summary>
		/// <returns>
		///   <see langword="true" /> if the default sort is used; otherwise <see langword="false" />.</returns>
		public bool ApplyDefaultSort
		{
			get
			{
				return this._applyDefaultSort;
			}
			set
			{
				if (this._applyDefaultSort != value)
				{
					this._applyDefaultSort = value;
				}
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataViewManager" /> that contains this <see cref="T:System.Data.DataViewSetting" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataViewManager" /> object.</returns>
		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return this._dataViewManager;
			}
		}

		internal void SetDataViewManager(DataViewManager dataViewManager)
		{
			if (this._dataViewManager != dataViewManager)
			{
				this._dataViewManager = dataViewManager;
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataTable" /> to which the <see cref="T:System.Data.DataViewSetting" /> properties apply.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> object.</returns>
		[Browsable(false)]
		public DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		internal void SetDataTable(DataTable table)
		{
			if (this._table != table)
			{
				this._table = table;
			}
		}

		/// <summary>Gets or sets the filter to apply in the <see cref="T:System.Data.DataView" />. See <see cref="P:System.Data.DataView.RowFilter" /> for a code sample using RowFilter.</summary>
		/// <returns>A string that contains the filter to apply.</returns>
		public string RowFilter
		{
			get
			{
				return this._rowFilter;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this._rowFilter != value)
				{
					this._rowFilter = value;
				}
			}
		}

		/// <summary>Gets or sets a value indicating whether to display Current, Deleted, Modified Current, ModifiedOriginal, New, Original, Unchanged, or no rows in the <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>A value that indicates which rows to display.</returns>
		public DataViewRowState RowStateFilter
		{
			get
			{
				return this._rowStateFilter;
			}
			set
			{
				if (this._rowStateFilter != value)
				{
					this._rowStateFilter = value;
				}
			}
		}

		/// <summary>Gets or sets a value indicating the sort to apply in the <see cref="T:System.Data.DataView" />.</summary>
		/// <returns>The sort to apply in the <see cref="T:System.Data.DataView" />.</returns>
		public string Sort
		{
			get
			{
				return this._sort;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this._sort != value)
				{
					this._sort = value;
				}
			}
		}

		private DataViewManager _dataViewManager;

		private DataTable _table;

		private string _sort = string.Empty;

		private string _rowFilter = string.Empty;

		private DataViewRowState _rowStateFilter = DataViewRowState.CurrentRows;

		private bool _applyDefaultSort;
	}
}
