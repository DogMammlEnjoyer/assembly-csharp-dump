using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Fusion
{
	[DebuggerDisplay("Length = {Length}")]
	[DebuggerTypeProxy(typeof(NetworkArray<>.DebuggerProxy))]
	public struct NetworkArray<T> : IEnumerable<T>, IEnumerable, INetworkArray
	{
		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public T this[int index]
		{
			get
			{
				bool flag = index >= this._length;
				if (flag)
				{
					throw new IndexOutOfRangeException();
				}
				return this._readerWriter.Read(this._array, index);
			}
			set
			{
				bool flag = index >= this._length;
				if (flag)
				{
					throw new IndexOutOfRangeException();
				}
				this._readerWriter.Write(this._array, index, value);
			}
		}

		object INetworkArray.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)((object)value);
			}
		}

		public unsafe NetworkArray(byte* array, int length, IElementReaderWriter<T> readerWriter)
		{
			this._array = array;
			this._length = length;
			this._readerWriter = readerWriter;
		}

		public NetworkArrayReadOnly<T> ToReadOnly()
		{
			return new NetworkArrayReadOnly<T>(this._array, this._length, this._readerWriter);
		}

		public T Get(int index)
		{
			return this[index];
		}

		public T Set(int index, T value)
		{
			this[index] = value;
			return value;
		}

		internal ref T GetRef(int index)
		{
			bool flag = index >= this._length;
			if (flag)
			{
				throw new IndexOutOfRangeException();
			}
			return this._readerWriter.ReadRef(this._array, index);
		}

		public T[] ToArray()
		{
			T[] array = new T[this._length];
			for (int i = 0; i < this._length; i++)
			{
				array[i] = this[i];
			}
			return array;
		}

		public void CopyTo(List<T> list)
		{
			for (int i = 0; i < this._length; i++)
			{
				list.Add(this[i]);
			}
		}

		public void CopyTo(NetworkArray<T> array)
		{
			int length = array.Length;
			bool flag = array.Length > length;
			if (flag)
			{
				throw new ArgumentException(string.Format("Max array length {0}, got: {1}", this._length, length), "array");
			}
			for (int i = 0; i < length; i++)
			{
				array[i] = this[i];
			}
		}

		public void CopyTo(T[] array, bool throwIfOverflow = true)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			int num = array.Length;
			bool flag2 = array.Length > num;
			if (flag2)
			{
				if (throwIfOverflow)
				{
					throw new ArgumentException(string.Format("Max array length {0}, got: {1}", this._length, num), "array");
				}
				num = this._length;
			}
			for (int i = 0; i < num; i++)
			{
				array[i] = this[i];
			}
		}

		public override string ToString()
		{
			return this.ToListString();
		}

		public NetworkArray<T>.Enumerator GetEnumerator()
		{
			return new NetworkArray<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			for (int i = 0; i < this._length; i++)
			{
				this[i] = default(T);
			}
		}

		public void CopyFrom(T[] source, int sourceOffset, int sourceCount)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = sourceCount > this._length;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Max array length {0}, got: {1}", this._length, sourceCount), "source");
			}
			bool flag3 = source.Length < sourceOffset + sourceCount;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Source length is {0}, but offset ({1}) and count {2}) are out of bounds", sourceCount, sourceOffset, sourceCount), "sourceCount");
			}
			for (int i = 0; i < sourceCount; i++)
			{
				this[i] = source[i + sourceOffset];
			}
		}

		public void CopyFrom(List<T> source, int sourceOffset, int sourceCount)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = sourceCount > this._length;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Max array length {0}, got: {1}", this._length, sourceCount), "source");
			}
			bool flag3 = source.Count < sourceOffset + sourceCount;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Source length is {0}, but offset ({1}) and count {2}) are out of bounds", sourceCount, sourceOffset, sourceCount), "sourceCount");
			}
			for (int i = 0; i < sourceCount; i++)
			{
				this[i] = source[i + sourceOffset];
			}
		}

		private string ToListString()
		{
			bool flag = this._length == 0;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this._array == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = NetworkArray<T>._stringBuilderCached == null;
					if (flag3)
					{
						NetworkArray<T>._stringBuilderCached = new StringBuilder();
					}
					else
					{
						NetworkArray<T>._stringBuilderCached.Clear();
					}
					StringBuilder stringBuilderCached = NetworkArray<T>._stringBuilderCached;
					int num = 0;
					for (;;)
					{
						bool isValueType = typeof(T).IsValueType;
						if (isValueType)
						{
							StringBuilder stringBuilder = stringBuilderCached;
							T t = this.Get(num);
							stringBuilder.Append(t.ToString());
						}
						else
						{
							stringBuilderCached.Append(this.Get(num));
						}
						num++;
						bool flag4 = num == this._length;
						if (flag4)
						{
							break;
						}
						stringBuilderCached.Append("\n");
					}
					result = stringBuilderCached.ToString();
				}
			}
			return result;
		}

		public static implicit operator NetworkArrayReadOnly<T>(NetworkArray<T> value)
		{
			return new NetworkArrayReadOnly<T>(value._array, value.Length, value._readerWriter);
		}

		private unsafe byte* _array;

		private int _length;

		private IElementReaderWriter<T> _readerWriter;

		private static StringBuilder _stringBuilderCached;

		internal class DebuggerProxy
		{
			public DebuggerProxy(NetworkArray<T> array)
			{
				this._items = new Lazy<T[]>(() => (array._array == null) ? Array.Empty<T>() : array.ToArray());
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
			public T Current
			{
				get
				{
					bool flag = this._index < this._array.Length;
					if (flag)
					{
						return this._array[this._index];
					}
					throw new IndexOutOfRangeException();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public Enumerator(NetworkArray<T> array)
			{
				this._index = -1;
				this._array = array;
			}

			public bool MoveNext()
			{
				int num = this._index + 1;
				this._index = num;
				return num < this._array.Length;
			}

			public void Reset()
			{
				this._index = -1;
			}

			public void Dispose()
			{
				this._array = default(NetworkArray<T>);
				this._index = -1;
			}

			private int _index;

			private NetworkArray<T> _array;
		}
	}
}
