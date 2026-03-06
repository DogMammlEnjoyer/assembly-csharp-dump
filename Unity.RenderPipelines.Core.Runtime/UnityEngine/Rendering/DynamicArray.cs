using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("Size = {size} Capacity = {capacity}")]
	public class DynamicArray<T> where T : new()
	{
		public int size { get; protected set; }

		public int capacity
		{
			get
			{
				return this.m_Array.Length;
			}
		}

		public DynamicArray()
		{
			this.m_Array = new T[32];
			this.size = 0;
		}

		public DynamicArray(int size)
		{
			this.m_Array = new T[size];
			this.size = size;
		}

		public DynamicArray(int capacity, bool resize)
		{
			this.m_Array = new T[capacity];
			this.size = (resize ? capacity : 0);
		}

		public DynamicArray(DynamicArray<T> deepCopy)
		{
			this.m_Array = new T[deepCopy.size];
			this.size = deepCopy.size;
			Array.Copy(deepCopy.m_Array, this.m_Array, this.size);
		}

		public void Clear()
		{
			this.size = 0;
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) != -1;
		}

		public int Add(in T value)
		{
			int size = this.size;
			if (size >= this.m_Array.Length)
			{
				T[] array = new T[Math.Max(this.m_Array.Length * 2, 1)];
				Array.Copy(this.m_Array, array, this.m_Array.Length);
				this.m_Array = array;
			}
			this.m_Array[size] = value;
			int size2 = this.size;
			this.size = size2 + 1;
			this.BumpVersion();
			return size;
		}

		public unsafe void AddRange(DynamicArray<T> array)
		{
			int size = array.size;
			this.Reserve(this.size + size, true);
			for (int i = 0; i < size; i++)
			{
				T[] array2 = this.m_Array;
				int size2 = this.size;
				this.size = size2 + 1;
				array2[size2] = *array[i];
			}
			this.BumpVersion();
		}

		public void Insert(int index, T item)
		{
			if (index == this.size)
			{
				this.Add(item);
				return;
			}
			this.Resize(this.size + 1, true);
			Array.Copy(this.m_Array, index, this.m_Array, index + 1, this.size - index);
			this.m_Array[index] = item;
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num != -1)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			if (index != this.size - 1)
			{
				Array.Copy(this.m_Array, index + 1, this.m_Array, index, this.size - index - 1);
			}
			int size = this.size;
			this.size = size - 1;
			this.BumpVersion();
		}

		public void RemoveRange(int index, int count)
		{
			if (count == 0)
			{
				return;
			}
			Array.Copy(this.m_Array, index + count, this.m_Array, index, this.size - index - count);
			this.size -= count;
			this.BumpVersion();
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			int num = startIndex;
			while (num < this.size && count > 0)
			{
				if (match(this.m_Array[num]))
				{
					return num;
				}
				num++;
				count--;
			}
			return -1;
		}

		public int FindIndex(Predicate<T> match)
		{
			return this.FindIndex(0, this.size, match);
		}

		public int IndexOf(T item, int index, int count)
		{
			int num = index;
			while (num < this.size && count > 0)
			{
				if (this.m_Array[num].Equals(item))
				{
					return num;
				}
				num++;
				count--;
			}
			return -1;
		}

		public int IndexOf(T item, int index)
		{
			for (int i = index; i < this.size; i++)
			{
				if (this.m_Array[i].Equals(item))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(T item)
		{
			return this.IndexOf(item, 0);
		}

		public void Resize(int newSize, bool keepContent = false)
		{
			this.Reserve(newSize, keepContent);
			this.size = newSize;
			this.BumpVersion();
		}

		public void ResizeAndClear(int newSize)
		{
			if (newSize > this.m_Array.Length)
			{
				this.Reserve(newSize, false);
			}
			else
			{
				Array.Clear(this.m_Array, 0, newSize);
			}
			this.size = newSize;
			this.BumpVersion();
		}

		public void Reserve(int newCapacity, bool keepContent = false)
		{
			if (newCapacity > this.m_Array.Length)
			{
				if (keepContent)
				{
					T[] array = new T[newCapacity];
					Array.Copy(this.m_Array, array, this.m_Array.Length);
					this.m_Array = array;
					return;
				}
				this.m_Array = new T[newCapacity];
			}
		}

		public T this[int index]
		{
			get
			{
				return ref this.m_Array[index];
			}
		}

		[Obsolete("This is deprecated because it returns an incorrect value. It may returns an array with elements beyond the size. Please use Span/ReadOnly if you want safe raw access to the DynamicArray memory.", false)]
		public static implicit operator T[](DynamicArray<T> array)
		{
			return array.m_Array;
		}

		public static implicit operator ReadOnlySpan<T>(DynamicArray<T> array)
		{
			return new ReadOnlySpan<T>(array.m_Array, 0, array.size);
		}

		public static implicit operator Span<T>(DynamicArray<T> array)
		{
			return new Span<T>(array.m_Array, 0, array.size);
		}

		public DynamicArray<T>.Iterator GetEnumerator()
		{
			return new DynamicArray<T>.Iterator(this);
		}

		public DynamicArray<T>.RangeEnumerable SubRange(int first, int numItems)
		{
			return new DynamicArray<T>.RangeEnumerable
			{
				iterator = new DynamicArray<T>.RangeEnumerable.RangeIterator(this, first, numItems)
			};
		}

		protected internal void BumpVersion()
		{
		}

		protected T[] m_Array;

		public struct Iterator
		{
			public Iterator(DynamicArray<T> setOwner)
			{
				this.owner = setOwner;
				this.index = -1;
			}

			public ref T Current
			{
				get
				{
					return this.owner[this.index];
				}
			}

			public bool MoveNext()
			{
				this.index++;
				return this.index < this.owner.size;
			}

			public void Reset()
			{
				this.index = -1;
			}

			private readonly DynamicArray<T> owner;

			private int index;
		}

		public struct RangeEnumerable
		{
			public DynamicArray<T>.RangeEnumerable.RangeIterator GetEnumerator()
			{
				return this.iterator;
			}

			public DynamicArray<T>.RangeEnumerable.RangeIterator iterator;

			public struct RangeIterator
			{
				public RangeIterator(DynamicArray<T> setOwner, int first, int numItems)
				{
					this.owner = setOwner;
					this.first = first;
					this.index = first - 1;
					this.last = first + numItems;
				}

				public ref T Current
				{
					get
					{
						return this.owner[this.index];
					}
				}

				public bool MoveNext()
				{
					this.index++;
					return this.index < this.last;
				}

				public void Reset()
				{
					this.index = this.first - 1;
				}

				private readonly DynamicArray<T> owner;

				private int index;

				private int first;

				private int last;
			}
		}

		public delegate int SortComparer(T x, T y);
	}
}
