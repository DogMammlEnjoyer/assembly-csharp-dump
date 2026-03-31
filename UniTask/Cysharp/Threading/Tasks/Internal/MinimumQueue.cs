using System;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal class MinimumQueue<T>
	{
		public MinimumQueue(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.array = new T[capacity];
			this.head = (this.tail = (this.size = 0));
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.size;
			}
		}

		public T Peek()
		{
			if (this.size == 0)
			{
				this.ThrowForEmptyQueue();
			}
			return this.array[this.head];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Enqueue(T item)
		{
			if (this.size == this.array.Length)
			{
				this.Grow();
			}
			this.array[this.tail] = item;
			this.MoveNext(ref this.tail);
			this.size++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Dequeue()
		{
			if (this.size == 0)
			{
				this.ThrowForEmptyQueue();
			}
			int num = this.head;
			T[] array = this.array;
			T result = array[num];
			array[num] = default(T);
			this.MoveNext(ref this.head);
			this.size--;
			return result;
		}

		private void Grow()
		{
			int num = (int)((long)this.array.Length * 200L / 100L);
			if (num < this.array.Length + 4)
			{
				num = this.array.Length + 4;
			}
			this.SetCapacity(num);
		}

		private void SetCapacity(int capacity)
		{
			T[] destinationArray = new T[capacity];
			if (this.size > 0)
			{
				if (this.head < this.tail)
				{
					Array.Copy(this.array, this.head, destinationArray, 0, this.size);
				}
				else
				{
					Array.Copy(this.array, this.head, destinationArray, 0, this.array.Length - this.head);
					Array.Copy(this.array, 0, destinationArray, this.array.Length - this.head, this.tail);
				}
			}
			this.array = destinationArray;
			this.head = 0;
			this.tail = ((this.size == capacity) ? 0 : this.size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MoveNext(ref int index)
		{
			int num = index + 1;
			if (num == this.array.Length)
			{
				num = 0;
			}
			index = num;
		}

		private void ThrowForEmptyQueue()
		{
			throw new InvalidOperationException("EmptyQueue");
		}

		private const int MinimumGrow = 4;

		private const int GrowFactor = 200;

		private T[] array;

		private int head;

		private int tail;

		private int size;
	}
}
