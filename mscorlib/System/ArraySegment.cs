using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics.Hashing;

namespace System
{
	/// <summary>Delimits a section of a one-dimensional array.</summary>
	/// <typeparam name="T">The type of the elements in the array segment.</typeparam>
	[Serializable]
	public readonly struct ArraySegment<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>
	{
		public static ArraySegment<T> Empty { get; } = new ArraySegment<T>(new T[0]);

		/// <summary>Initializes a new instance of the <see cref="T:System.ArraySegment`1" /> structure that delimits all the elements in the specified array.</summary>
		/// <param name="array">The array to wrap.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		public ArraySegment(T[] array)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			this._array = array;
			this._offset = 0;
			this._count = array.Length;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArraySegment`1" /> structure that delimits the specified range of the elements in the specified array.</summary>
		/// <param name="array">The array containing the range of elements to delimit.</param>
		/// <param name="offset">The zero-based index of the first element in the range.</param>
		/// <param name="count">The number of elements in the range.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> and <paramref name="count" /> do not specify a valid range in <paramref name="array" />.</exception>
		public ArraySegment(T[] array, int offset, int count)
		{
			if (array == null || offset > array.Length || count > array.Length - offset)
			{
				ThrowHelper.ThrowArraySegmentCtorValidationFailedExceptions(array, offset, count);
			}
			this._array = array;
			this._offset = offset;
			this._count = count;
		}

		/// <summary>Gets the original array containing the range of elements that the array segment delimits.</summary>
		/// <returns>The original array that was passed to the constructor, and that contains the range delimited by the <see cref="T:System.ArraySegment`1" />.</returns>
		public T[] Array
		{
			get
			{
				return this._array;
			}
		}

		/// <summary>Gets the position of the first element in the range delimited by the array segment, relative to the start of the original array.</summary>
		/// <returns>The position of the first element in the range delimited by the <see cref="T:System.ArraySegment`1" />, relative to the start of the original array.</returns>
		public int Offset
		{
			get
			{
				return this._offset;
			}
		}

		/// <summary>Gets the number of elements in the range delimited by the array segment.</summary>
		/// <returns>The number of elements in the range delimited by the <see cref="T:System.ArraySegment`1" />.</returns>
		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= this._count)
				{
					ThrowHelper.ThrowArgumentOutOfRange_IndexException();
				}
				return this._array[this._offset + index];
			}
			set
			{
				if (index >= this._count)
				{
					ThrowHelper.ThrowArgumentOutOfRange_IndexException();
				}
				this._array[this._offset + index] = value;
			}
		}

		public ArraySegment<T>.Enumerator GetEnumerator()
		{
			this.ThrowInvalidOperationIfDefault();
			return new ArraySegment<T>.Enumerator(this);
		}

		/// <summary>Returns the hash code for the current instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (this._array == null)
			{
				return 0;
			}
			return System.Numerics.Hashing.HashHelpers.Combine(System.Numerics.Hashing.HashHelpers.Combine(5381, this._offset), this._count) ^ this._array.GetHashCode();
		}

		public void CopyTo(T[] destination)
		{
			this.CopyTo(destination, 0);
		}

		public void CopyTo(T[] destination, int destinationIndex)
		{
			this.ThrowInvalidOperationIfDefault();
			System.Array.Copy(this._array, this._offset, destination, destinationIndex, this._count);
		}

		public void CopyTo(ArraySegment<T> destination)
		{
			this.ThrowInvalidOperationIfDefault();
			destination.ThrowInvalidOperationIfDefault();
			if (this._count > destination._count)
			{
				ThrowHelper.ThrowArgumentException_DestinationTooShort();
			}
			System.Array.Copy(this._array, this._offset, destination._array, destination._offset, this._count);
		}

		/// <summary>Determines whether the specified object is equal to the current instance.</summary>
		/// <param name="obj">The object to be compared with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is a <see cref="T:System.ArraySegment`1" /> structure and is equal to the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is ArraySegment<T> && this.Equals((ArraySegment<T>)obj);
		}

		/// <summary>Determines whether the specified <see cref="T:System.ArraySegment`1" /> structure is equal to the current instance.</summary>
		/// <param name="obj">The structure to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ArraySegment`1" /> structure is equal to the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(ArraySegment<T> obj)
		{
			return obj._array == this._array && obj._offset == this._offset && obj._count == this._count;
		}

		public ArraySegment<T> Slice(int index)
		{
			this.ThrowInvalidOperationIfDefault();
			if (index > this._count)
			{
				ThrowHelper.ThrowArgumentOutOfRange_IndexException();
			}
			return new ArraySegment<T>(this._array, this._offset + index, this._count - index);
		}

		public ArraySegment<T> Slice(int index, int count)
		{
			this.ThrowInvalidOperationIfDefault();
			if (index > this._count || count > this._count - index)
			{
				ThrowHelper.ThrowArgumentOutOfRange_IndexException();
			}
			return new ArraySegment<T>(this._array, this._offset + index, count);
		}

		public T[] ToArray()
		{
			this.ThrowInvalidOperationIfDefault();
			if (this._count == 0)
			{
				return ArraySegment<T>.Empty._array;
			}
			T[] array = new T[this._count];
			System.Array.Copy(this._array, this._offset, array, 0, this._count);
			return array;
		}

		/// <summary>Indicates whether two <see cref="T:System.ArraySegment`1" /> structures are equal.</summary>
		/// <param name="a">The  structure on the left side of the equality operator.</param>
		/// <param name="b">The structure on the right side of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(ArraySegment<T> a, ArraySegment<T> b)
		{
			return a.Equals(b);
		}

		/// <summary>Indicates whether two <see cref="T:System.ArraySegment`1" /> structures are unequal.</summary>
		/// <param name="a">The structure on the left side of the inequality operator.</param>
		/// <param name="b">The structure on the right side of the inequality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(ArraySegment<T> a, ArraySegment<T> b)
		{
			return !(a == b);
		}

		public static implicit operator ArraySegment<T>(T[] array)
		{
			if (array == null)
			{
				return default(ArraySegment<T>);
			}
			return new ArraySegment<T>(array);
		}

		T IList<!0>.this[int index]
		{
			get
			{
				this.ThrowInvalidOperationIfDefault();
				if (index < 0 || index >= this._count)
				{
					ThrowHelper.ThrowArgumentOutOfRange_IndexException();
				}
				return this._array[this._offset + index];
			}
			set
			{
				this.ThrowInvalidOperationIfDefault();
				if (index < 0 || index >= this._count)
				{
					ThrowHelper.ThrowArgumentOutOfRange_IndexException();
				}
				this._array[this._offset + index] = value;
			}
		}

		int IList<!0>.IndexOf(T item)
		{
			this.ThrowInvalidOperationIfDefault();
			int num = System.Array.IndexOf<T>(this._array, item, this._offset, this._count);
			if (num < 0)
			{
				return -1;
			}
			return num - this._offset;
		}

		void IList<!0>.Insert(int index, T item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void IList<!0>.RemoveAt(int index)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		T IReadOnlyList<!0>.this[int index]
		{
			get
			{
				this.ThrowInvalidOperationIfDefault();
				if (index < 0 || index >= this._count)
				{
					ThrowHelper.ThrowArgumentOutOfRange_IndexException();
				}
				return this._array[this._offset + index];
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<!0>.Add(T item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void ICollection<!0>.Clear()
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		bool ICollection<!0>.Contains(T item)
		{
			this.ThrowInvalidOperationIfDefault();
			return System.Array.IndexOf<T>(this._array, item, this._offset, this._count) >= 0;
		}

		bool ICollection<!0>.Remove(T item)
		{
			ThrowHelper.ThrowNotSupportedException();
			return false;
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns an enumerator that iterates through an array segment.</summary>
		/// <returns>An enumerator that can be used to iterate through the array segment.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void ThrowInvalidOperationIfDefault()
		{
			if (this._array == null)
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_NullArray);
			}
		}

		private readonly T[] _array;

		private readonly int _offset;

		private readonly int _count;

		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(ArraySegment<T> arraySegment)
			{
				this._array = arraySegment.Array;
				this._start = arraySegment.Offset;
				this._end = arraySegment.Offset + arraySegment.Count;
				this._current = arraySegment.Offset - 1;
			}

			public bool MoveNext()
			{
				if (this._current < this._end)
				{
					this._current++;
					return this._current < this._end;
				}
				return false;
			}

			public T Current
			{
				get
				{
					if (this._current < this._start)
					{
						ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumNotStarted();
					}
					if (this._current >= this._end)
					{
						ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumEnded();
					}
					return this._array[this._current];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				this._current = this._start - 1;
			}

			public void Dispose()
			{
			}

			private readonly T[] _array;

			private readonly int _start;

			private readonly int _end;

			private int _current;
		}
	}
}
