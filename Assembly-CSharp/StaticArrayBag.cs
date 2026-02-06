using System;
using System.Collections.Generic;

public class StaticArrayBag<T>
{
	public T[] GetStaticArray(int size)
	{
		T[] array;
		if (!this.m_bag.ContainsKey(size))
		{
			array = new T[size];
			this.m_bag[size] = array;
		}
		else
		{
			array = this.m_bag[size];
		}
		return array;
	}

	private Dictionary<int, T[]> m_bag = new Dictionary<int, T[]>(1);
}
