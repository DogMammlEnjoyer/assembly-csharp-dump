using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	public struct ReadOnlyArray<TValue> : IReadOnlyList<TValue>, IEnumerable<TValue>, IEnumerable, IReadOnlyCollection<TValue>
	{
		public ReadOnlyArray(TValue[] array)
		{
			this.m_Array = array;
			this.m_StartIndex = 0;
			this.m_Length = ((array != null) ? array.Length : 0);
		}

		public ReadOnlyArray(TValue[] array, int index, int length)
		{
			this.m_Array = array;
			this.m_StartIndex = index;
			this.m_Length = length;
		}

		public TValue[] ToArray()
		{
			TValue[] array = new TValue[this.m_Length];
			if (this.m_Length > 0)
			{
				Array.Copy(this.m_Array, this.m_StartIndex, array, 0, this.m_Length);
			}
			return array;
		}

		public int IndexOf(Predicate<TValue> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			for (int i = 0; i < this.m_Length; i++)
			{
				if (predicate(this.m_Array[this.m_StartIndex + i]))
				{
					return i;
				}
			}
			return -1;
		}

		public ReadOnlyArray<TValue>.Enumerator GetEnumerator()
		{
			return new ReadOnlyArray<TValue>.Enumerator(this.m_Array, this.m_StartIndex, this.m_Length);
		}

		IEnumerator<TValue> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public static implicit operator ReadOnlyArray<TValue>(TValue[] array)
		{
			return new ReadOnlyArray<TValue>(array);
		}

		public int Count
		{
			get
			{
				return this.m_Length;
			}
		}

		public TValue this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (this.m_Array == null)
				{
					throw new InvalidOperationException();
				}
				return this.m_Array[this.m_StartIndex + index];
			}
		}

		internal TValue[] m_Array;

		internal int m_StartIndex;

		internal int m_Length;

		public struct Enumerator : IEnumerator<!0>, IEnumerator, IDisposable
		{
			internal Enumerator(TValue[] array, int index, int length)
			{
				this.m_Array = array;
				this.m_IndexStart = index - 1;
				this.m_IndexEnd = index + length;
				this.m_Index = this.m_IndexStart;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.m_Index < this.m_IndexEnd)
				{
					this.m_Index++;
				}
				return this.m_Index != this.m_IndexEnd;
			}

			public void Reset()
			{
				this.m_Index = this.m_IndexStart;
			}

			public TValue Current
			{
				get
				{
					if (this.m_Index == this.m_IndexEnd)
					{
						throw new InvalidOperationException("Iterated beyond end");
					}
					return this.m_Array[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private readonly TValue[] m_Array;

			private readonly int m_IndexStart;

			private readonly int m_IndexEnd;

			private int m_Index;
		}
	}
}
