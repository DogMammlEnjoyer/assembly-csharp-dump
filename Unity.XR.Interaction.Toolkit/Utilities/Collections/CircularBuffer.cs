using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Collections
{
	internal class CircularBuffer<T>
	{
		public CircularBuffer(int capacity)
		{
			this.m_Buffer = new T[capacity];
			this.m_Start = 0;
			this.m_Count = 0;
		}

		public int count
		{
			get
			{
				return this.m_Count;
			}
		}

		public int capacity
		{
			get
			{
				return this.m_Buffer.Length;
			}
		}

		public void Add(T item)
		{
			int num = (this.m_Start + this.m_Count) % this.m_Buffer.Length;
			this.m_Buffer[num] = item;
			if (this.m_Count < this.m_Buffer.Length)
			{
				this.m_Count++;
				return;
			}
			this.m_Start = (this.m_Start + 1) % this.m_Buffer.Length;
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_Count)
				{
					throw new IndexOutOfRangeException();
				}
				return this.m_Buffer[(this.m_Start + index) % this.m_Buffer.Length];
			}
		}

		public void Clear()
		{
			this.m_Start = 0;
			this.m_Count = 0;
		}

		private readonly T[] m_Buffer;

		private int m_Start;

		private int m_Count;
	}
}
