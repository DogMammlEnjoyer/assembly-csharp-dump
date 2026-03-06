using System;

namespace Oculus.Interaction
{
	public class RingBuffer<T>
	{
		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public int Capacity
		{
			get
			{
				return this._capacity;
			}
		}

		public RingBuffer(int capacity)
		{
			this._buffer = new T[capacity];
			this._capacity = capacity;
			this.Clear();
		}

		public void Clear()
		{
			this._head = -1;
			this._count = 0;
		}

		public void Add(T item)
		{
			this._head = (this._head + 1) % this._capacity;
			this._buffer[this._head] = item;
			if (this._count < this._capacity)
			{
				this._count++;
			}
		}

		public T this[int index]
		{
			get
			{
				if (this._count == 0)
				{
					throw new InvalidOperationException("The buffer is empty.");
				}
				return this._buffer[(index % this._count + this._count) % this._count];
			}
		}

		public T Peek(int offset = 0)
		{
			if (this._count == 0)
			{
				throw new InvalidOperationException("The buffer is empty.");
			}
			return this._buffer[((this._head + offset) % this._count + this._count) % this._count];
		}

		private readonly T[] _buffer;

		private readonly int _capacity;

		private int _head;

		private int _count;
	}
}
