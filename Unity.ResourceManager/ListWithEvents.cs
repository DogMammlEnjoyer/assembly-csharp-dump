using System;
using System.Collections;
using System.Collections.Generic;

internal class ListWithEvents<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public event Action<T> OnElementAdded;

	public event Action<T> OnElementRemoved;

	private void InvokeAdded(T element)
	{
		Action<T> onElementAdded = this.OnElementAdded;
		if (onElementAdded == null)
		{
			return;
		}
		onElementAdded(element);
	}

	private void InvokeRemoved(T element)
	{
		Action<T> onElementRemoved = this.OnElementRemoved;
		if (onElementRemoved == null)
		{
			return;
		}
		onElementRemoved(element);
	}

	public T this[int index]
	{
		get
		{
			return this.m_List[index];
		}
		set
		{
			T element = this.m_List[index];
			this.m_List[index] = value;
			this.InvokeRemoved(element);
			this.InvokeAdded(value);
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
			return ((ICollection<T>)this.m_List).IsReadOnly;
		}
	}

	public void Add(T item)
	{
		this.m_List.Add(item);
		this.InvokeAdded(item);
	}

	public void Clear()
	{
		foreach (T element in this.m_List)
		{
			this.InvokeRemoved(element);
		}
		this.m_List.Clear();
	}

	public bool Contains(T item)
	{
		return this.m_List.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		this.m_List.CopyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return this.m_List.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return this.m_List.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		this.m_List.Insert(index, item);
		this.InvokeAdded(item);
	}

	public bool Remove(T item)
	{
		bool flag = this.m_List.Remove(item);
		if (flag)
		{
			this.InvokeRemoved(item);
		}
		return flag;
	}

	public void RemoveAt(int index)
	{
		T element = this.m_List[index];
		this.m_List.RemoveAt(index);
		this.InvokeRemoved(element);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)this.m_List).GetEnumerator();
	}

	private List<T> m_List = new List<T>();
}
