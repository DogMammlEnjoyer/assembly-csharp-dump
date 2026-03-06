using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public class ObservableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public event ListChangedEventHandler<T> ItemAdded;

		public event ListChangedEventHandler<T> ItemRemoved;

		public T this[int index]
		{
			get
			{
				return this.m_List[index];
			}
			set
			{
				this.OnEvent(this.ItemRemoved, index, this.m_List[index]);
				this.m_List[index] = value;
				this.OnEvent(this.ItemAdded, index, value);
			}
		}

		public int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ObservableList() : this(0, null)
		{
		}

		public ObservableList(int capacity, Comparison<T> comparison = null)
		{
			this.m_List = new List<T>(capacity);
			this.m_Comparison = comparison;
		}

		public ObservableList(IEnumerable<T> collection, Comparison<T> comparison = null)
		{
			this.m_List = new List<T>(collection);
			this.m_Comparison = comparison;
			this.Sort();
		}

		private void OnEvent(ListChangedEventHandler<T> e, int index, T item)
		{
			if (e != null)
			{
				e(this, new ListChangedEventArgs<T>(index, item));
			}
		}

		public bool Contains(T item)
		{
			return this.m_List.Contains(item);
		}

		public int IndexOf(T item)
		{
			return this.m_List.IndexOf(item);
		}

		public void Add(T item)
		{
			this.m_List.Add(item);
			this.Sort();
			this.OnEvent(this.ItemAdded, this.m_List.IndexOf(item), item);
		}

		public void Add(params T[] items)
		{
			foreach (T item in items)
			{
				this.Add(item);
			}
		}

		public void Insert(int index, T item)
		{
			this.m_List.Insert(index, item);
			this.Sort();
			this.OnEvent(this.ItemAdded, index, item);
		}

		public bool Remove(T item)
		{
			int index = this.m_List.IndexOf(item);
			bool flag = this.m_List.Remove(item);
			if (flag)
			{
				this.OnEvent(this.ItemRemoved, index, item);
			}
			return flag;
		}

		public int Remove(params T[] items)
		{
			if (items == null)
			{
				return 0;
			}
			int num = 0;
			foreach (T item in items)
			{
				num += (this.Remove(item) ? 1 : 0);
			}
			return num;
		}

		public void RemoveAt(int index)
		{
			T item = this.m_List[index];
			this.m_List.RemoveAt(index);
			this.OnEvent(this.ItemRemoved, index, item);
		}

		public void Clear()
		{
			while (this.Count > 0)
			{
				this.RemoveAt(this.Count - 1);
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_List.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.m_List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void Sort()
		{
			if (this.m_Comparison != null)
			{
				this.m_List.Sort(this.m_Comparison);
			}
		}

		private List<T> m_List;

		private readonly Comparison<T> m_Comparison;
	}
}
