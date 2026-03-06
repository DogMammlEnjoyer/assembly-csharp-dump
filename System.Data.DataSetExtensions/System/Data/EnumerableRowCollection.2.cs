using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace System.Data
{
	/// <summary>Represents a collection of <see cref="T:System.Data.DataRow" /> objects returned from a query.</summary>
	/// <typeparam name="TRow">The type of objects in the source sequence, typically <see cref="T:System.Data.DataRow" />.</typeparam>
	public class EnumerableRowCollection<TRow> : EnumerableRowCollection, IEnumerable<TRow>, IEnumerable
	{
		internal override Type ElementType
		{
			get
			{
				return typeof(TRow);
			}
		}

		internal IEnumerable<TRow> EnumerableRows
		{
			get
			{
				return this._enumerableRows;
			}
		}

		internal override DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		internal EnumerableRowCollection(IEnumerable<TRow> enumerableRows, bool isDataViewable, DataTable table)
		{
			this._enumerableRows = enumerableRows;
			if (isDataViewable)
			{
				this._table = table;
			}
			this._listOfPredicates = new List<Func<TRow, bool>>();
			this._sortExpression = new SortExpressionBuilder<TRow>();
		}

		internal EnumerableRowCollection(DataTable table)
		{
			this._table = table;
			this._enumerableRows = table.Rows.Cast<TRow>();
			this._listOfPredicates = new List<Func<TRow, bool>>();
			this._sortExpression = new SortExpressionBuilder<TRow>();
		}

		internal EnumerableRowCollection(EnumerableRowCollection<TRow> source, IEnumerable<TRow> enumerableRows, Func<TRow, TRow> selector)
		{
			this._enumerableRows = enumerableRows;
			this._selector = selector;
			if (source != null)
			{
				if (source._selector == null)
				{
					this._table = source._table;
				}
				this._listOfPredicates = new List<Func<TRow, bool>>(source._listOfPredicates);
				this._sortExpression = source._sortExpression.Clone();
				return;
			}
			this._listOfPredicates = new List<Func<TRow, bool>>();
			this._sortExpression = new SortExpressionBuilder<TRow>();
		}

		/// <summary>Returns an enumerator for the collection of <see cref="T:System.Data.DataRow" /> objects.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to traverse the collection of <see cref="T:System.Data.DataRow" /> objects.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns an enumerator for the collection of contained row objects.</summary>
		/// <returns>A strongly-typed <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to traverse the collection of <paramref name="TRow" /> objects.</returns>
		public IEnumerator<TRow> GetEnumerator()
		{
			return this._enumerableRows.GetEnumerator();
		}

		internal void AddPredicate(Func<TRow, bool> pred)
		{
			this._listOfPredicates.Add(pred);
		}

		internal void AddSortExpression<TKey>(Func<TRow, TKey> keySelector, bool isDescending, bool isOrderBy)
		{
			this.AddSortExpression<TKey>(keySelector, Comparer<TKey>.Default, isDescending, isOrderBy);
		}

		internal void AddSortExpression<TKey>(Func<TRow, TKey> keySelector, IComparer<TKey> comparer, bool isDescending, bool isOrderBy)
		{
			DataSetUtil.CheckArgumentNull<Func<TRow, TKey>>(keySelector, "keySelector");
			DataSetUtil.CheckArgumentNull<IComparer<TKey>>(comparer, "comparer");
			this._sortExpression.Add((TRow input) => keySelector(input), (object val1, object val2) => (isDescending ? -1 : 1) * comparer.Compare((TKey)((object)val1), (TKey)((object)val2)), isOrderBy);
		}

		internal EnumerableRowCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly DataTable _table;

		private readonly IEnumerable<TRow> _enumerableRows;

		private readonly List<Func<TRow, bool>> _listOfPredicates;

		private readonly SortExpressionBuilder<TRow> _sortExpression;

		private readonly Func<TRow, TRow> _selector;
	}
}
