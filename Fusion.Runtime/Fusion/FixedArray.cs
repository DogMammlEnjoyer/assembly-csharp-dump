using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fusion
{
	[DebuggerDisplay("Length = {Length}")]
	public struct FixedArray<[IsUnmanaged] T> : IEnumerable<!0>, IEnumerable where T : struct, ValueType
	{
		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public unsafe T this[int index]
		{
			get
			{
				bool flag = index >= this._length;
				if (flag)
				{
					throw new IndexOutOfRangeException();
				}
				return ref this._array[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
		}

		public unsafe FixedArray(T* array, int length)
		{
			this._array = array;
			this._length = length;
		}

		public unsafe T[] ToArray()
		{
			T[] array = new T[this._length];
			for (int i = 0; i < this._length; i++)
			{
				array[i] = *this[i];
			}
			return array;
		}

		public unsafe void CopyTo(List<T> list)
		{
			for (int i = 0; i < this._length; i++)
			{
				list.Add(*this[i]);
			}
		}

		public unsafe void CopyTo(T[] array, bool throwIfOverflow = true)
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
				array[i] = *this[i];
			}
		}

		public override string ToString()
		{
			return this.ToListString();
		}

		public FixedArray<T>.Enumerator GetEnumerator()
		{
			return new FixedArray<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public unsafe void Clear()
		{
			for (int i = 0; i < this._length; i++)
			{
				*this[i] = default(T);
			}
		}

		public unsafe void CopyFrom(T[] source, int sourceOffset, int sourceCount)
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
				*this[i] = source[i + sourceOffset];
			}
		}

		public unsafe void CopyFrom(List<T> source, int sourceOffset, int sourceCount)
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
				*this[i] = source[i + sourceOffset];
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
					bool flag3 = FixedArray<T>._stringBuilderCached == null;
					if (flag3)
					{
						FixedArray<T>._stringBuilderCached = new StringBuilder();
					}
					else
					{
						FixedArray<T>._stringBuilderCached.Clear();
					}
					StringBuilder stringBuilderCached = FixedArray<T>._stringBuilderCached;
					int num = 0;
					for (;;)
					{
						stringBuilderCached.Append(this[num].ToString());
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

		private unsafe T* _array;

		private int _length;

		private static StringBuilder _stringBuilderCached;

		public struct Enumerator : IEnumerator<!0>, IEnumerator, IDisposable
		{
			public unsafe T Current
			{
				get
				{
					bool flag = this._index < this._array.Length;
					if (flag)
					{
						return *this._array[this._index];
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

			public Enumerator(FixedArray<T> array)
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
				this._array = default(FixedArray<T>);
				this._index = -1;
			}

			private int _index;

			private FixedArray<T> _array;
		}
	}
}
