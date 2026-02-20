using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fusion
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(NetworkLinkedList<>.DebuggerProxy))]
	public struct NetworkLinkedList<T> : IEnumerable<T>, IEnumerable, INetworkLinkedList
	{
		private unsafe int Head
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._data[1];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this._data[1] = value;
			}
		}

		private unsafe int Tail
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._data[2];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this._data[2] = value;
			}
		}

		public unsafe int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return *this._data;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private set
			{
				*this._data = value;
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.Write(this.GetEntryByListIndex(index), value);
			}
		}

		public unsafe NetworkLinkedList(byte* data, int capacity, IElementReaderWriter<T> rw)
		{
			Assert.Check(Native.IsPointerAligned((void*)data, 4));
			this._rw = rw;
			this._data = (int*)data;
			this._capacity = capacity;
			this._stride = rw.GetElementWordCount() + 2;
		}

		public unsafe NetworkLinkedList<T> Remap(void* list)
		{
			Assert.Check((void*)this._data);
			Assert.Check(this._capacity > 0);
			Assert.Check(this._rw);
			NetworkLinkedList<T> result = this;
			result._data = (int*)list;
			return result;
		}

		public unsafe void Clear()
		{
			Native.MemClear((void*)this._data, (3 + this._stride * this.Capacity) * 4);
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

		public T Set(int index, T value)
		{
			this.Write(this.GetEntryByListIndex(index), value);
			return value;
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

		public bool Remove(T value)
		{
			return this.Remove(value, EqualityComparer<T>.Default);
		}

		public unsafe bool Remove(T value, IEqualityComparer<T> equalityComparer)
		{
			int* ptr;
			for (int i = this.Head; i != 0; i = ptr[1])
			{
				ptr = this.Entry(i);
				T x = this.Read(ptr);
				bool flag = equalityComparer.Equals(x, value);
				if (flag)
				{
					this.RemoveEntry(ptr, i);
					return true;
				}
			}
			return false;
		}

		public unsafe void Add(T value)
		{
			Assert.Check((ulong)this.Count <= (ulong)((long)this.Capacity));
			bool flag = this.Count == this.Capacity;
			if (flag)
			{
				throw new InvalidOperationException("NetworkList is full");
			}
			int num;
			int* ptr = this.FindFreeEntry(out num);
			Assert.Check(ptr != null);
			int count = this.Count + 1;
			this.Count = count;
			this.Write(ptr, value);
			*ptr = this.Tail;
			ptr[1] = 0;
			bool flag2 = this.Tail != 0;
			if (flag2)
			{
				this.Entry(this.Tail)[1] = num;
				this.Tail = num;
			}
			else
			{
				this.Head = num;
				this.Tail = num;
			}
		}

		private unsafe int* FindFreeEntry(out int index)
		{
			for (int i = 0; i < this._capacity; i++)
			{
				int num = i + 1;
				bool flag = num != this.Head && num != this.Tail;
				if (flag)
				{
					int* ptr = this.Entry(num);
					bool flag2 = *ptr == 0;
					if (flag2)
					{
						index = num;
						return ptr;
					}
				}
			}
			Assert.AlwaysFail("No free entry");
			index = 0;
			return null;
		}

		private unsafe void RemoveEntry(int* entry, int entryIndex)
		{
			Assert.Check((ulong)this.Count <= (ulong)((long)this.Capacity));
			Assert.Check(this.Entry(entryIndex) == entry);
			bool flag = *entry != 0;
			if (flag)
			{
				this.Entry(*entry)[1] = entry[1];
			}
			bool flag2 = entry[1] != 0;
			if (flag2)
			{
				*this.Entry(entry[1]) = *entry;
			}
			bool flag3 = this.Tail == entryIndex;
			if (flag3)
			{
				this.Tail = *entry;
			}
			bool flag4 = this.Head == entryIndex;
			if (flag4)
			{
				this.Head = entry[1];
			}
			*entry = 0;
			entry[1] = 0;
			int count = this.Count - 1;
			this.Count = count;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void Write(int* entry, T value)
		{
			this._rw.Write((byte*)(entry + 2), 0, value);
		}

		public NetworkLinkedList<T>.Enumerator GetEnumerator()
		{
			return new NetworkLinkedList<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return new NetworkLinkedList<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<!0>)this).GetEnumerator();
		}

		void INetworkLinkedList.Add(object item)
		{
			this.Add((T)((object)item));
		}

		public const int ELEMENT_WORDS = 2;

		public const int META_WORDS = 3;

		private unsafe int* _data;

		private int _stride;

		private int _capacity;

		private IElementReaderWriter<T> _rw;

		private const int COUNT = 0;

		private const int HEAD = 1;

		private const int TAIL = 2;

		private const int PREV = 0;

		private const int NEXT = 1;

		private const int INVALID = 0;

		private const int OFFSET = 1;

		internal class DebuggerProxy
		{
			public DebuggerProxy(NetworkLinkedList<T> list)
			{
				this._items = new Lazy<T[]>(() => (list._data == null) ? Array.Empty<T>() : list.ToArray<T>());
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public T[] Items
			{
				get
				{
					return this._items.Value;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public Lazy<T[]> _items;
		}

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			internal Enumerator(NetworkLinkedList<T> list)
			{
				this._list = list;
				this._head = 0;
				this._first = true;
			}

			public unsafe bool MoveNext()
			{
				bool first = this._first;
				if (first)
				{
					this._first = false;
					this._head = this._list.Head;
				}
				else
				{
					bool flag = this._head == 0;
					if (flag)
					{
						return false;
					}
					this._head = this._list.Entry(this._head)[1];
				}
				return this._head > 0 && this._head <= this._list._capacity;
			}

			public void Reset()
			{
				this._head = 0;
			}

			public T Current
			{
				get
				{
					bool flag = this._head > 0 && this._head <= this._list._capacity;
					if (flag)
					{
						return this._list.Read(this._list.Entry(this._head));
					}
					throw new InvalidOperationException();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this._list = default(NetworkLinkedList<T>);
				this._head = -1;
			}

			private bool _first;

			private int _head;

			private NetworkLinkedList<T> _list;
		}
	}
}
