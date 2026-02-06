using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusion
{
	internal class RingBuffer<T> : IEnumerable<!0>, IEnumerable where T : struct
	{
		public RingBuffer(int capacity) : this(capacity, new T[0])
		{
		}

		public RingBuffer(int capacity, T[] items)
		{
			bool flag = capacity < 1;
			if (flag)
			{
				throw new ArgumentException("Buffer cannot have negative or zero capacity.", "capacity");
			}
			bool flag2 = items == null;
			if (flag2)
			{
				throw new ArgumentNullException("items");
			}
			bool flag3 = items.Length > capacity;
			if (flag3)
			{
				throw new ArgumentException("Number of items exceeds buffer capacity.", "items");
			}
			this._buffer = new T[capacity];
			Array.Copy(items, this._buffer, items.Length);
			this._front = 0;
			this._count = items.Length;
		}

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
				return this._buffer.Length;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._count == 0;
			}
		}

		public bool IsFull
		{
			get
			{
				return this._count == this._buffer.Length;
			}
		}

		public T this[int index]
		{
			get
			{
				bool isEmpty = this.IsEmpty;
				if (isEmpty)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty.", index));
				}
				bool flag = index >= this._count;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer has {1} items.", index, this._count));
				}
				int num = this.InternalIndex(index);
				return this._buffer[num];
			}
			set
			{
				bool isEmpty = this.IsEmpty;
				if (isEmpty)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty.", index));
				}
				bool flag = index >= this._count;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer has {1} items.", index, this._count));
				}
				int num = this.InternalIndex(index);
				this._buffer[num] = value;
			}
		}

		public ref readonly T Front()
		{
			this.ThrowIfEmpty("Cannot access an empty buffer.");
			return ref this._buffer[this._front];
		}

		public ref T FrontMut()
		{
			this.ThrowIfEmpty("Cannot access an empty buffer.");
			return ref this._buffer[this._front];
		}

		public ref readonly T Get(int index)
		{
			bool isEmpty = this.IsEmpty;
			if (isEmpty)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty.", index));
			}
			bool flag = index >= this._count;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer has {1} items.", index, this._count));
			}
			int num = this.InternalIndex(index);
			return ref this._buffer[num];
		}

		public ref T GetMut(int index)
		{
			bool isEmpty = this.IsEmpty;
			if (isEmpty)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty.", index));
			}
			bool flag = index >= this._count;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer has {1} items.", index, this._count));
			}
			int num = this.InternalIndex(index);
			return ref this._buffer[num];
		}

		public ref readonly T Back()
		{
			this.ThrowIfEmpty("Cannot access an empty buffer.");
			int num = this.Decrement(this.BackIndex());
			return ref this._buffer[num];
		}

		public ref T BackMut()
		{
			this.ThrowIfEmpty("Cannot access an empty buffer.");
			int num = this.Decrement(this.BackIndex());
			return ref this._buffer[num];
		}

		public void PushBack(T item)
		{
			int num = this.BackIndex();
			bool isFull = this.IsFull;
			if (isFull)
			{
				this._buffer[num] = item;
				this._front = this.Increment(num);
			}
			else
			{
				this._buffer[num] = item;
				this._count++;
			}
		}

		public void PushFront(T item)
		{
			this._front = this.Decrement(this._front);
			bool isFull = this.IsFull;
			if (isFull)
			{
				this._buffer[this._front] = item;
			}
			else
			{
				this._buffer[this._front] = item;
				this._count++;
			}
		}

		public T PopBack()
		{
			this.ThrowIfEmpty("Cannot take items from an empty buffer.");
			int num = this.Decrement(this.BackIndex());
			T result = this._buffer[num];
			this._buffer[num] = default(T);
			this._count--;
			return result;
		}

		public T PopFront()
		{
			this.ThrowIfEmpty("Cannot take items from an empty buffer.");
			T result = this._buffer[this._front];
			this._buffer[this._front] = default(T);
			this._front = this.Increment(this._front);
			this._count--;
			return result;
		}

		public void Clear()
		{
			this._front = 0;
			this._count = 0;
			Array.Clear(this._buffer, 0, this._buffer.Length);
		}

		public IList<ArraySegment<T>> ToArraySegments()
		{
			return new ArraySegment<T>[]
			{
				this.SpanOne(),
				this.SpanTwo()
			};
		}

		public T[] ToArray()
		{
			T[] array = new T[this.Count];
			int num = 0;
			IList<ArraySegment<T>> list = this.ToArraySegments();
			foreach (ArraySegment<T> arraySegment in list)
			{
				Array.Copy(arraySegment.Array, arraySegment.Offset, array, num, arraySegment.Count);
				num += arraySegment.Count;
			}
			return array;
		}

		public IEnumerator<T> GetEnumerator()
		{
			RingBuffer<T>.<GetEnumerator>d__29 <GetEnumerator>d__ = new RingBuffer<T>.<GetEnumerator>d__29(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int FrontIndex()
		{
			return this._front;
		}

		private int BackIndex()
		{
			return (this._front + this._count) % this.Capacity;
		}

		private int InternalIndex(int index)
		{
			return (this._front + index) % this.Capacity;
		}

		private int Increment(int index)
		{
			Assert.Check(index >= 0 && index < this.Capacity);
			return (index == this.Capacity - 1) ? 0 : (index + 1);
		}

		private int Decrement(int index)
		{
			Assert.Check(index >= 0 && index < this.Capacity);
			return (index == 0) ? (this.Capacity - 1) : (index - 1);
		}

		private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
		{
			bool isEmpty = this.IsEmpty;
			if (isEmpty)
			{
				throw new InvalidOperationException(message);
			}
		}

		private ArraySegment<T> SpanOne()
		{
			int num = this.BackIndex();
			bool isEmpty = this.IsEmpty;
			ArraySegment<T> result;
			if (isEmpty)
			{
				result = new ArraySegment<T>(new T[0]);
			}
			else
			{
				bool flag = this._front < num;
				if (flag)
				{
					result = new ArraySegment<T>(this._buffer, this._front, num - this._front);
				}
				else
				{
					result = new ArraySegment<T>(this._buffer, this._front, this._buffer.Length - this._front);
				}
			}
			return result;
		}

		private ArraySegment<T> SpanTwo()
		{
			int num = this.BackIndex();
			bool isEmpty = this.IsEmpty;
			ArraySegment<T> result;
			if (isEmpty)
			{
				result = new ArraySegment<T>(new T[0]);
			}
			else
			{
				bool flag = this._front < num;
				if (flag)
				{
					result = new ArraySegment<T>(this._buffer, num, 0);
				}
				else
				{
					result = new ArraySegment<T>(this._buffer, 0, num);
				}
			}
			return result;
		}

		private readonly T[] _buffer;

		private int _front;

		private int _count;
	}
}
