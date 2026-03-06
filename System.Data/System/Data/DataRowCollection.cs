using System;
using System.Collections;
using Unity;

namespace System.Data
{
	/// <summary>Represents a collection of rows for a <see cref="T:System.Data.DataTable" />.</summary>
	public sealed class DataRowCollection : InternalDataCollectionBase
	{
		internal DataRowCollection(DataTable table)
		{
			this._list = new DataRowCollection.DataRowTree();
			base..ctor();
			this._table = table;
		}

		/// <summary>Gets the total number of <see cref="T:System.Data.DataRow" /> objects in this collection.</summary>
		/// <returns>The total number of <see cref="T:System.Data.DataRow" /> objects in this collection.</returns>
		public override int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		/// <summary>Gets the row at the specified index.</summary>
		/// <param name="index">The zero-based index of the row to return.</param>
		/// <returns>The specified <see cref="T:System.Data.DataRow" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index value is greater than the number of items in the collection.</exception>
		public DataRow this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

		/// <summary>Adds the specified <see cref="T:System.Data.DataRow" /> to the <see cref="T:System.Data.DataRowCollection" /> object.</summary>
		/// <param name="row">The <see cref="T:System.Data.DataRow" /> to add.</param>
		/// <exception cref="T:System.ArgumentNullException">The row is null.</exception>
		/// <exception cref="T:System.ArgumentException">The row either belongs to another table or already belongs to this table.</exception>
		/// <exception cref="T:System.Data.ConstraintException">The addition invalidates a constraint.</exception>
		/// <exception cref="T:System.Data.NoNullAllowedException">The addition tries to put a null in a <see cref="T:System.Data.DataColumn" /> where <see cref="P:System.Data.DataColumn.AllowDBNull" /> is false.</exception>
		public void Add(DataRow row)
		{
			this._table.AddRow(row, -1);
		}

		/// <summary>Inserts a new row into the collection at the specified location.</summary>
		/// <param name="row">The <see cref="T:System.Data.DataRow" /> to add.</param>
		/// <param name="pos">The (zero-based) location in the collection where you want to add the <see langword="DataRow" />.</param>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="pos" /> is less than 0.</exception>
		public void InsertAt(DataRow row, int pos)
		{
			if (pos < 0)
			{
				throw ExceptionBuilder.RowInsertOutOfRange(pos);
			}
			if (pos >= this._list.Count)
			{
				this._table.AddRow(row, -1);
				return;
			}
			this._table.InsertRow(row, -1, pos);
		}

		internal void DiffInsertAt(DataRow row, int pos)
		{
			if (pos < 0 || pos == this._list.Count)
			{
				this._table.AddRow(row, (pos > -1) ? (pos + 1) : -1);
				return;
			}
			if (this._table.NestedParentRelations.Length == 0)
			{
				this._table.InsertRow(row, pos + 1, (pos > this._list.Count) ? -1 : pos);
				return;
			}
			if (pos >= this._list.Count)
			{
				while (pos > this._list.Count)
				{
					this._list.Add(null);
					this._nullInList++;
				}
				this._table.AddRow(row, pos + 1);
				return;
			}
			if (this._list[pos] != null)
			{
				throw ExceptionBuilder.RowInsertTwice(pos, this._table.TableName);
			}
			this._list.RemoveAt(pos);
			this._nullInList--;
			this._table.InsertRow(row, pos + 1, pos);
		}

		/// <summary>Gets the index of the specified <see cref="T:System.Data.DataRow" /> object.</summary>
		/// <param name="row">The <see langword="DataRow" /> to search for.</param>
		/// <returns>The zero-based index of the row, or -1 if the row is not found in the collection.</returns>
		public int IndexOf(DataRow row)
		{
			if (row != null && row.Table == this._table && (row.RBTreeNodeId != 0 || row.RowState != DataRowState.Detached))
			{
				return this._list.IndexOf(row.RBTreeNodeId, row);
			}
			return -1;
		}

		internal DataRow AddWithColumnEvents(params object[] values)
		{
			DataRow dataRow = this._table.NewRow(-1);
			dataRow.ItemArray = values;
			this._table.AddRow(dataRow, -1);
			return dataRow;
		}

		/// <summary>Creates a row using specified values and adds it to the <see cref="T:System.Data.DataRowCollection" />.</summary>
		/// <param name="values">The array of values that are used to create the new row.</param>
		/// <returns>None.</returns>
		/// <exception cref="T:System.ArgumentException">The array is larger than the number of columns in the table.</exception>
		/// <exception cref="T:System.InvalidCastException">A value does not match its respective column type.</exception>
		/// <exception cref="T:System.Data.ConstraintException">Adding the row invalidates a constraint.</exception>
		/// <exception cref="T:System.Data.NoNullAllowedException">Trying to put a null in a column where <see cref="P:System.Data.DataColumn.AllowDBNull" /> is false.</exception>
		public DataRow Add(params object[] values)
		{
			int record = this._table.NewRecordFromArray(values);
			DataRow dataRow = this._table.NewRow(record);
			this._table.AddRow(dataRow, -1);
			return dataRow;
		}

		internal void ArrayAdd(DataRow row)
		{
			row.RBTreeNodeId = this._list.Add(row);
		}

		internal void ArrayInsert(DataRow row, int pos)
		{
			row.RBTreeNodeId = this._list.Insert(pos, row);
		}

		internal void ArrayClear()
		{
			this._list.Clear();
		}

		internal void ArrayRemove(DataRow row)
		{
			if (row.RBTreeNodeId == 0)
			{
				throw ExceptionBuilder.InternalRBTreeError(RBTreeError.AttachedNodeWithZerorbTreeNodeId);
			}
			this._list.RBDelete(row.RBTreeNodeId);
			row.RBTreeNodeId = 0;
		}

		/// <summary>Gets the row specified by the primary key value.</summary>
		/// <param name="key">The primary key value of the <see cref="T:System.Data.DataRow" /> to find.</param>
		/// <returns>A <see cref="T:System.Data.DataRow" /> that contains the primary key value specified; otherwise a null value if the primary key value does not exist in the <see cref="T:System.Data.DataRowCollection" />.</returns>
		/// <exception cref="T:System.Data.MissingPrimaryKeyException">The table does not have a primary key.</exception>
		public DataRow Find(object key)
		{
			return this._table.FindByPrimaryKey(key);
		}

		/// <summary>Gets the row that contains the specified primary key values.</summary>
		/// <param name="keys">An array of primary key values to find. The type of the array is <see langword="Object" />.</param>
		/// <returns>A <see cref="T:System.Data.DataRow" /> object that contains the primary key values specified; otherwise a null value if the primary key value does not exist in the <see cref="T:System.Data.DataRowCollection" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">No row corresponds to that index value.</exception>
		/// <exception cref="T:System.Data.MissingPrimaryKeyException">The table does not have a primary key.</exception>
		public DataRow Find(object[] keys)
		{
			return this._table.FindByPrimaryKey(keys);
		}

		/// <summary>Clears the collection of all rows.</summary>
		/// <exception cref="T:System.Data.InvalidConstraintException">A <see cref="T:System.Data.ForeignKeyConstraint" /> is enforced on the <see cref="T:System.Data.DataRowCollection" />.</exception>
		public void Clear()
		{
			this._table.Clear(false);
		}

		/// <summary>Gets a value that indicates whether the primary key of any row in the collection contains the specified value.</summary>
		/// <param name="key">The value of the primary key to test for.</param>
		/// <returns>
		///   <see langword="true" /> if the collection contains a <see cref="T:System.Data.DataRow" /> with the specified primary key value; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Data.MissingPrimaryKeyException">The table does not have a primary key.</exception>
		public bool Contains(object key)
		{
			return this._table.FindByPrimaryKey(key) != null;
		}

		/// <summary>Gets a value that indicates whether the primary key columns of any row in the collection contain the values specified in the object array.</summary>
		/// <param name="keys">An array of primary key values to test for.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.DataRowCollection" /> contains a <see cref="T:System.Data.DataRow" /> with the specified key values; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Data.MissingPrimaryKeyException">The table does not have a primary key.</exception>
		public bool Contains(object[] keys)
		{
			return this._table.FindByPrimaryKey(keys) != null;
		}

		/// <summary>Copies all the <see cref="T:System.Data.DataRow" /> objects from the collection into the given array, starting at the given destination array index.</summary>
		/// <param name="ar">The one-dimensional array that is the destination of the elements copied from the <see langword="DataRowCollection" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in the array at which copying begins.</param>
		public override void CopyTo(Array ar, int index)
		{
			this._list.CopyTo(ar, index);
		}

		/// <summary>Copies all the <see cref="T:System.Data.DataRow" /> objects from the collection into the given array, starting at the given destination array index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see langword="DataRowCollection" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in the array at which copying begins.</param>
		public void CopyTo(DataRow[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		/// <summary>Gets an <see cref="T:System.Collections.IEnumerator" /> for this collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for this collection.</returns>
		public override IEnumerator GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		/// <summary>Removes the specified <see cref="T:System.Data.DataRow" /> from the collection.</summary>
		/// <param name="row">The <see cref="T:System.Data.DataRow" /> to remove.</param>
		public void Remove(DataRow row)
		{
			if (row == null || row.Table != this._table || -1L == row.rowID)
			{
				throw ExceptionBuilder.RowOutOfRange();
			}
			if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
			{
				row.Delete();
			}
			if (row.RowState != DataRowState.Detached)
			{
				row.AcceptChanges();
			}
		}

		/// <summary>Removes the row at the specified index from the collection.</summary>
		/// <param name="index">The index of the row to remove.</param>
		public void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		internal DataRowCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly DataTable _table;

		private readonly DataRowCollection.DataRowTree _list;

		internal int _nullInList;

		private sealed class DataRowTree : RBTree<DataRow>
		{
			internal DataRowTree() : base(TreeAccessMethod.INDEX_ONLY)
			{
			}

			protected override int CompareNode(DataRow record1, DataRow record2)
			{
				throw ExceptionBuilder.InternalRBTreeError(RBTreeError.CompareNodeInDataRowTree);
			}

			protected override int CompareSateliteTreeNode(DataRow record1, DataRow record2)
			{
				throw ExceptionBuilder.InternalRBTreeError(RBTreeError.CompareSateliteTreeNodeInDataRowTree);
			}
		}
	}
}
