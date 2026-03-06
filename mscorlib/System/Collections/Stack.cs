using System;
using System.Diagnostics;
using System.Threading;

namespace System.Collections
{
	/// <summary>Represents a simple last-in-first-out (LIFO) non-generic collection of objects.</summary>
	[DebuggerTypeProxy(typeof(Stack.StackDebugView))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class Stack : ICollection, IEnumerable, ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Stack" /> class that is empty and has the default initial capacity.</summary>
		public Stack()
		{
			this._array = new object[10];
			this._size = 0;
			this._version = 0;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Stack" /> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.</summary>
		/// <param name="initialCapacity">The initial number of elements that the <see cref="T:System.Collections.Stack" /> can contain.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="initialCapacity" /> is less than zero.</exception>
		public Stack(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity", "Non-negative number required.");
			}
			if (initialCapacity < 10)
			{
				initialCapacity = 10;
			}
			this._array = new object[initialCapacity];
			this._size = 0;
			this._version = 0;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Stack" /> class that contains elements copied from the specified collection and has the same initial capacity as the number of elements copied.</summary>
		/// <param name="col">The <see cref="T:System.Collections.ICollection" /> to copy elements from.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="col" /> is <see langword="null" />.</exception>
		public Stack(ICollection col) : this((col == null) ? 32 : col.Count)
		{
			if (col == null)
			{
				throw new ArgumentNullException("col");
			}
			foreach (object obj in col)
			{
				this.Push(obj);
			}
		}

		/// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Stack" />.</summary>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.Stack" />.</returns>
		public virtual int Count
		{
			get
			{
				return this._size;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.Stack" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" />, if access to the <see cref="T:System.Collections.Stack" /> is synchronized (thread safe); otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.Stack" />.</summary>
		/// <returns>An <see cref="T:System.Object" /> that can be used to synchronize access to the <see cref="T:System.Collections.Stack" />.</returns>
		public virtual object SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		/// <summary>Removes all objects from the <see cref="T:System.Collections.Stack" />.</summary>
		public virtual void Clear()
		{
			Array.Clear(this._array, 0, this._size);
			this._size = 0;
			this._version++;
		}

		/// <summary>Creates a shallow copy of the <see cref="T:System.Collections.Stack" />.</summary>
		/// <returns>A shallow copy of the <see cref="T:System.Collections.Stack" />.</returns>
		public virtual object Clone()
		{
			Stack stack = new Stack(this._size);
			stack._size = this._size;
			Array.Copy(this._array, 0, stack._array, 0, this._size);
			stack._version = this._version;
			return stack;
		}

		/// <summary>Determines whether an element is in the <see cref="T:System.Collections.Stack" />.</summary>
		/// <param name="obj">The object to locate in the <see cref="T:System.Collections.Stack" />. The value can be <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" />, if <paramref name="obj" /> is found in the <see cref="T:System.Collections.Stack" />; otherwise, <see langword="false" />.</returns>
		public virtual bool Contains(object obj)
		{
			int size = this._size;
			while (size-- > 0)
			{
				if (obj == null)
				{
					if (this._array[size] == null)
					{
						return true;
					}
				}
				else if (this._array[size] != null && this._array[size].Equals(obj))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Copies the <see cref="T:System.Collections.Stack" /> to an existing one-dimensional <see cref="T:System.Array" />, starting at the specified array index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Stack" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// The number of elements in the source <see cref="T:System.Collections.Stack" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.Stack" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public virtual void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Non-negative number required.");
			}
			if (array.Length - index < this._size)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			int i = 0;
			object[] array2 = array as object[];
			if (array2 != null)
			{
				while (i < this._size)
				{
					array2[i + index] = this._array[this._size - i - 1];
					i++;
				}
				return;
			}
			while (i < this._size)
			{
				array.SetValue(this._array[this._size - i - 1], i + index);
				i++;
			}
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Collections.Stack" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Collections.Stack" />.</returns>
		public virtual IEnumerator GetEnumerator()
		{
			return new Stack.StackEnumerator(this);
		}

		/// <summary>Returns the object at the top of the <see cref="T:System.Collections.Stack" /> without removing it.</summary>
		/// <returns>The <see cref="T:System.Object" /> at the top of the <see cref="T:System.Collections.Stack" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Stack" /> is empty.</exception>
		public virtual object Peek()
		{
			if (this._size == 0)
			{
				throw new InvalidOperationException("Stack empty.");
			}
			return this._array[this._size - 1];
		}

		/// <summary>Removes and returns the object at the top of the <see cref="T:System.Collections.Stack" />.</summary>
		/// <returns>The <see cref="T:System.Object" /> removed from the top of the <see cref="T:System.Collections.Stack" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Stack" /> is empty.</exception>
		public virtual object Pop()
		{
			if (this._size == 0)
			{
				throw new InvalidOperationException("Stack empty.");
			}
			this._version++;
			object[] array = this._array;
			int num = this._size - 1;
			this._size = num;
			object result = array[num];
			this._array[this._size] = null;
			return result;
		}

		/// <summary>Inserts an object at the top of the <see cref="T:System.Collections.Stack" />.</summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to push onto the <see cref="T:System.Collections.Stack" />. The value can be <see langword="null" />.</param>
		public virtual void Push(object obj)
		{
			if (this._size == this._array.Length)
			{
				object[] array = new object[2 * this._array.Length];
				Array.Copy(this._array, 0, array, 0, this._size);
				this._array = array;
			}
			object[] array2 = this._array;
			int size = this._size;
			this._size = size + 1;
			array2[size] = obj;
			this._version++;
		}

		/// <summary>Returns a synchronized (thread safe) wrapper for the <see cref="T:System.Collections.Stack" />.</summary>
		/// <param name="stack">The <see cref="T:System.Collections.Stack" /> to synchronize.</param>
		/// <returns>A synchronized wrapper around the <see cref="T:System.Collections.Stack" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stack" /> is <see langword="null" />.</exception>
		public static Stack Synchronized(Stack stack)
		{
			if (stack == null)
			{
				throw new ArgumentNullException("stack");
			}
			return new Stack.SyncStack(stack);
		}

		/// <summary>Copies the <see cref="T:System.Collections.Stack" /> to a new array.</summary>
		/// <returns>A new array containing copies of the elements of the <see cref="T:System.Collections.Stack" />.</returns>
		public virtual object[] ToArray()
		{
			if (this._size == 0)
			{
				return Array.Empty<object>();
			}
			object[] array = new object[this._size];
			for (int i = 0; i < this._size; i++)
			{
				array[i] = this._array[this._size - i - 1];
			}
			return array;
		}

		private object[] _array;

		private int _size;

		private int _version;

		[NonSerialized]
		private object _syncRoot;

		private const int _defaultCapacity = 10;

		[Serializable]
		private class SyncStack : Stack
		{
			internal SyncStack(Stack stack)
			{
				this._s = stack;
				this._root = stack.SyncRoot;
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this._root;
				}
			}

			public override int Count
			{
				get
				{
					object root = this._root;
					int count;
					lock (root)
					{
						count = this._s.Count;
					}
					return count;
				}
			}

			public override bool Contains(object obj)
			{
				object root = this._root;
				bool result;
				lock (root)
				{
					result = this._s.Contains(obj);
				}
				return result;
			}

			public override object Clone()
			{
				object root = this._root;
				object result;
				lock (root)
				{
					result = new Stack.SyncStack((Stack)this._s.Clone());
				}
				return result;
			}

			public override void Clear()
			{
				object root = this._root;
				lock (root)
				{
					this._s.Clear();
				}
			}

			public override void CopyTo(Array array, int arrayIndex)
			{
				object root = this._root;
				lock (root)
				{
					this._s.CopyTo(array, arrayIndex);
				}
			}

			public override void Push(object value)
			{
				object root = this._root;
				lock (root)
				{
					this._s.Push(value);
				}
			}

			public override object Pop()
			{
				object root = this._root;
				object result;
				lock (root)
				{
					result = this._s.Pop();
				}
				return result;
			}

			public override IEnumerator GetEnumerator()
			{
				object root = this._root;
				IEnumerator enumerator;
				lock (root)
				{
					enumerator = this._s.GetEnumerator();
				}
				return enumerator;
			}

			public override object Peek()
			{
				object root = this._root;
				object result;
				lock (root)
				{
					result = this._s.Peek();
				}
				return result;
			}

			public override object[] ToArray()
			{
				object root = this._root;
				object[] result;
				lock (root)
				{
					result = this._s.ToArray();
				}
				return result;
			}

			private Stack _s;

			private object _root;
		}

		[Serializable]
		private class StackEnumerator : IEnumerator, ICloneable
		{
			internal StackEnumerator(Stack stack)
			{
				this._stack = stack;
				this._version = this._stack._version;
				this._index = -2;
				this._currentElement = null;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			public virtual bool MoveNext()
			{
				if (this._version != this._stack._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (this._index == -2)
				{
					this._index = this._stack._size - 1;
					bool flag = this._index >= 0;
					if (flag)
					{
						this._currentElement = this._stack._array[this._index];
					}
					return flag;
				}
				if (this._index == -1)
				{
					return false;
				}
				int num = this._index - 1;
				this._index = num;
				bool flag2 = num >= 0;
				if (flag2)
				{
					this._currentElement = this._stack._array[this._index];
					return flag2;
				}
				this._currentElement = null;
				return flag2;
			}

			public virtual object Current
			{
				get
				{
					if (this._index == -2)
					{
						throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
					}
					if (this._index == -1)
					{
						throw new InvalidOperationException("Enumeration already finished.");
					}
					return this._currentElement;
				}
			}

			public virtual void Reset()
			{
				if (this._version != this._stack._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._index = -2;
				this._currentElement = null;
			}

			private Stack _stack;

			private int _index;

			private int _version;

			private object _currentElement;
		}

		internal class StackDebugView
		{
			public StackDebugView(Stack stack)
			{
				if (stack == null)
				{
					throw new ArgumentNullException("stack");
				}
				this._stack = stack;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public object[] Items
			{
				get
				{
					return this._stack.ToArray();
				}
			}

			private Stack _stack;
		}
	}
}
