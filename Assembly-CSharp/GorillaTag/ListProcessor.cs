using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	public class ListProcessor<T>
	{
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		public InAction<T> ItemProcessor
		{
			get
			{
				return this.m_itemProcessorDelegate;
			}
			set
			{
				this.m_itemProcessorDelegate = value;
			}
		}

		public ListProcessor() : this(10, null)
		{
		}

		public ListProcessor(int capacity, InAction<T> itemProcessorDelegate = null)
		{
			this.m_list = new List<T>(capacity);
			this.m_currentIndex = -1;
			this.m_listCount = -1;
			this.m_itemProcessorDelegate = itemProcessorDelegate;
		}

		public virtual void Add(in T item)
		{
			this.m_listCount++;
			this.m_list.Add(item);
		}

		public virtual bool Remove(in T item)
		{
			int num = this.m_list.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			if (num < this.m_currentIndex)
			{
				this.m_currentIndex--;
			}
			this.m_listCount--;
			this.m_list.RemoveAt(num);
			return true;
		}

		public void Clear()
		{
			this.m_list.Clear();
			this.m_currentIndex = -1;
			this.m_listCount = -1;
		}

		public bool Contains(in T item)
		{
			return this.m_list.Contains(item);
		}

		public virtual void ProcessListSafe()
		{
			this.ProcessListSafe(this.m_itemProcessorDelegate);
		}

		public virtual void ProcessListSafe(InAction<T> customDelegate)
		{
			if (customDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				try
				{
					T t = this.m_list[this.m_currentIndex];
					customDelegate(t);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
				this.m_currentIndex++;
			}
		}

		public virtual void ProcessList()
		{
			this.ProcessList(this.m_itemProcessorDelegate);
		}

		public virtual void ProcessList(InAction<T> customDelegate)
		{
			if (customDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				T t = this.m_list[this.m_currentIndex];
				customDelegate(t);
				this.m_currentIndex++;
			}
		}

		public IReadOnlyList<T> GetReadonlyList()
		{
			return this.m_list;
		}

		protected readonly List<T> m_list;

		protected int m_currentIndex;

		protected int m_listCount;

		protected InAction<T> m_itemProcessorDelegate;
	}
}
