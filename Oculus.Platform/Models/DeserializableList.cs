using System;
using System.Collections;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class DeserializableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this._Data.Count;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return ((ICollection<!0>)this._Data).IsReadOnly;
			}
		}

		public int IndexOf(T obj)
		{
			return this._Data.IndexOf(obj);
		}

		public T this[int index]
		{
			get
			{
				return this._Data[index];
			}
			set
			{
				this._Data[index] = value;
			}
		}

		public void Add(T item)
		{
			this._Data.Add(item);
		}

		public void Clear()
		{
			this._Data.Clear();
		}

		public bool Contains(T item)
		{
			return this._Data.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this._Data.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._Data.GetEnumerator();
		}

		public void Insert(int index, T item)
		{
			this._Data.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return this._Data.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this._Data.RemoveAt(index);
		}

		private IEnumerator GetEnumerator1()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator1();
		}

		[Obsolete("Use IList interface on the DeserializableList object instead.", false)]
		public List<T> Data
		{
			get
			{
				return this._Data;
			}
		}

		public bool HasNextPage
		{
			get
			{
				return !string.IsNullOrEmpty(this.NextUrl);
			}
		}

		public bool HasPreviousPage
		{
			get
			{
				return !string.IsNullOrEmpty(this.PreviousUrl);
			}
		}

		public string NextUrl
		{
			get
			{
				return this._NextUrl;
			}
		}

		public string PreviousUrl
		{
			get
			{
				return this._PreviousUrl;
			}
		}

		protected List<T> _Data;

		protected string _NextUrl;

		protected string _PreviousUrl;
	}
}
