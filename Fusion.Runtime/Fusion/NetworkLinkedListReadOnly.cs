using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public ref struct NetworkLinkedListReadOnly<T>
	{
		internal unsafe NetworkLinkedListReadOnly(byte* data, int capacity, IElementReaderWriter<T> rw)
		{
			Assert.Check(Native.IsPointerAligned((void*)data, 4));
			this._rw = rw;
			this._data = (int*)data;
			this._capacity = capacity;
			this._stride = rw.GetElementWordCount() + 2;
		}

		private unsafe int Head
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._data[1];
			}
		}

		private unsafe int Tail
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._data[2];
			}
		}

		public unsafe int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return *this._data;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._capacity;
			}
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Read(this.GetEntryByListIndex(index));
			}
		}

		private unsafe int* GetEntryByListIndex(int listIndex)
		{
			int num = listIndex;
			int i = this.Head;
			while (i != 0)
			{
				int* ptr = this.Entry(i);
				bool flag = listIndex == 0;
				if (flag)
				{
					return ptr;
				}
				i = ptr[1];
				listIndex--;
			}
			throw new IndexOutOfRangeException(num.ToString());
		}

		public bool Contains(T value)
		{
			return this.Contains(value, EqualityComparer<T>.Default);
		}

		public unsafe bool Contains(T value, IEqualityComparer<T> comparer)
		{
			int* ptr;
			for (int i = this.Head; i != 0; i = ptr[1])
			{
				ptr = this.Entry(i);
				T x = this.Read(ptr);
				bool flag = comparer.Equals(x, value);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public T Get(int index)
		{
			return this.Read(this.GetEntryByListIndex(index));
		}

		public int IndexOf(T value)
		{
			return this.IndexOf(value, EqualityComparer<T>.Default);
		}

		public int IndexOf(T value, IEqualityComparer<T> equalityComparer)
		{
			for (int i = 0; i < this.Capacity; i++)
			{
				bool flag = equalityComparer.Equals(this.Read(this.GetEntryByListIndex(i)), value);
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe int* Entry(int index)
		{
			Assert.Check(index >= 1 && index <= this._capacity);
			return this._data + 3 + this._stride * (index - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe T Read(int* entry)
		{
			return this._rw.Read((byte*)(entry + 2), 0);
		}

		private const int COUNT = 0;

		private const int HEAD = 1;

		private const int TAIL = 2;

		private const int PREV = 0;

		private const int NEXT = 1;

		private const int INVALID = 0;

		private const int OFFSET = 1;

		public const int ELEMENT_WORDS = 2;

		public const int META_WORDS = 3;

		private unsafe int* _data;

		private int _stride;

		private int _capacity;

		private IElementReaderWriter<T> _rw;
	}
}
