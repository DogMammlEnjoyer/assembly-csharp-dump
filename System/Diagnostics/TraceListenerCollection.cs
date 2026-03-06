using System;
using System.Collections;

namespace System.Diagnostics
{
	/// <summary>Provides a thread-safe list of <see cref="T:System.Diagnostics.TraceListener" /> objects.</summary>
	public class TraceListenerCollection : IList, ICollection, IEnumerable
	{
		internal TraceListenerCollection()
		{
			this.list = new ArrayList(1);
		}

		/// <summary>Gets or sets the <see cref="T:System.Diagnostics.TraceListener" /> at the specified index.</summary>
		/// <param name="i">The zero-based index of the <see cref="T:System.Diagnostics.TraceListener" /> to get from the list.</param>
		/// <returns>A <see cref="T:System.Diagnostics.TraceListener" /> with the specified index.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value is <see langword="null" />.</exception>
		public TraceListener this[int i]
		{
			get
			{
				return (TraceListener)this.list[i];
			}
			set
			{
				this.InitializeListener(value);
				this.list[i] = value;
			}
		}

		/// <summary>Gets the first <see cref="T:System.Diagnostics.TraceListener" /> in the list with the specified name.</summary>
		/// <param name="name">The name of the <see cref="T:System.Diagnostics.TraceListener" /> to get from the list.</param>
		/// <returns>The first <see cref="T:System.Diagnostics.TraceListener" /> in the list with the given <see cref="P:System.Diagnostics.TraceListener.Name" />. This item returns <see langword="null" /> if no <see cref="T:System.Diagnostics.TraceListener" /> with the given name can be found.</returns>
		public TraceListener this[string name]
		{
			get
			{
				foreach (object obj in this)
				{
					TraceListener traceListener = (TraceListener)obj;
					if (traceListener.Name == name)
					{
						return traceListener;
					}
				}
				return null;
			}
		}

		/// <summary>Gets the number of listeners in the list.</summary>
		/// <returns>The number of listeners in the list.</returns>
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		/// <summary>Adds a <see cref="T:System.Diagnostics.TraceListener" /> to the list.</summary>
		/// <param name="listener">A <see cref="T:System.Diagnostics.TraceListener" /> to add to the list.</param>
		/// <returns>The position at which the new listener was inserted.</returns>
		public int Add(TraceListener listener)
		{
			this.InitializeListener(listener);
			object critSec = TraceInternal.critSec;
			int result;
			lock (critSec)
			{
				result = this.list.Add(listener);
			}
			return result;
		}

		/// <summary>Adds an array of <see cref="T:System.Diagnostics.TraceListener" /> objects to the list.</summary>
		/// <param name="value">An array of <see cref="T:System.Diagnostics.TraceListener" /> objects to add to the list.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public void AddRange(TraceListener[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < value.Length; i++)
			{
				this.Add(value[i]);
			}
		}

		/// <summary>Adds the contents of another <see cref="T:System.Diagnostics.TraceListenerCollection" /> to the list.</summary>
		/// <param name="value">Another <see cref="T:System.Diagnostics.TraceListenerCollection" /> whose contents are added to the list.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public void AddRange(TraceListenerCollection value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int count = value.Count;
			for (int i = 0; i < count; i++)
			{
				this.Add(value[i]);
			}
		}

		/// <summary>Clears all the listeners from the list.</summary>
		public void Clear()
		{
			this.list = new ArrayList();
		}

		/// <summary>Checks whether the list contains the specified listener.</summary>
		/// <param name="listener">A <see cref="T:System.Diagnostics.TraceListener" /> to find in the list.</param>
		/// <returns>
		///   <see langword="true" /> if the listener is in the list; otherwise, <see langword="false" />.</returns>
		public bool Contains(TraceListener listener)
		{
			return ((IList)this).Contains(listener);
		}

		/// <summary>Copies a section of the current <see cref="T:System.Diagnostics.TraceListenerCollection" /> list to the specified array at the specified index.</summary>
		/// <param name="listeners">An array of type <see cref="T:System.Array" /> to copy the elements into.</param>
		/// <param name="index">The starting index number in the current list to copy from.</param>
		public void CopyTo(TraceListener[] listeners, int index)
		{
			((ICollection)this).CopyTo(listeners, index);
		}

		/// <summary>Gets an enumerator for this list.</summary>
		/// <returns>An enumerator of type <see cref="T:System.Collections.IEnumerator" />.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		internal void InitializeListener(TraceListener listener)
		{
			if (listener == null)
			{
				throw new ArgumentNullException("listener");
			}
			listener.IndentSize = TraceInternal.IndentSize;
			listener.IndentLevel = TraceInternal.IndentLevel;
		}

		/// <summary>Gets the index of the specified listener.</summary>
		/// <param name="listener">A <see cref="T:System.Diagnostics.TraceListener" /> to find in the list.</param>
		/// <returns>The index of the listener, if it can be found in the list; otherwise, -1.</returns>
		public int IndexOf(TraceListener listener)
		{
			return ((IList)this).IndexOf(listener);
		}

		/// <summary>Inserts the listener at the specified index.</summary>
		/// <param name="index">The position in the list to insert the new <see cref="T:System.Diagnostics.TraceListener" />.</param>
		/// <param name="listener">A <see cref="T:System.Diagnostics.TraceListener" /> to insert in the list.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="index" /> is not a valid index in the list.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="listener" /> is <see langword="null" />.</exception>
		public void Insert(int index, TraceListener listener)
		{
			this.InitializeListener(listener);
			object critSec = TraceInternal.critSec;
			lock (critSec)
			{
				this.list.Insert(index, listener);
			}
		}

		/// <summary>Removes from the collection the specified <see cref="T:System.Diagnostics.TraceListener" />.</summary>
		/// <param name="listener">A <see cref="T:System.Diagnostics.TraceListener" /> to remove from the list.</param>
		public void Remove(TraceListener listener)
		{
			((IList)this).Remove(listener);
		}

		/// <summary>Removes from the collection the first <see cref="T:System.Diagnostics.TraceListener" /> with the specified name.</summary>
		/// <param name="name">The name of the <see cref="T:System.Diagnostics.TraceListener" /> to remove from the list.</param>
		public void Remove(string name)
		{
			TraceListener traceListener = this[name];
			if (traceListener != null)
			{
				((IList)this).Remove(traceListener);
			}
		}

		/// <summary>Removes from the collection the <see cref="T:System.Diagnostics.TraceListener" /> at the specified index.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Diagnostics.TraceListener" /> to remove from the list.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="index" /> is not a valid index in the list.</exception>
		public void RemoveAt(int index)
		{
			object critSec = TraceInternal.critSec;
			lock (critSec)
			{
				this.list.RemoveAt(index);
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Diagnostics.TraceListener" /> at the specified index in the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</summary>
		/// <param name="index">The zero-based index of the <paramref name="value" /> to get.</param>
		/// <returns>The <see cref="T:System.Diagnostics.TraceListener" /> at the specified index.</returns>
		object IList.this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				TraceListener traceListener = value as TraceListener;
				if (traceListener == null)
				{
					throw new ArgumentException(SR.GetString("Only TraceListeners can be added to a TraceListenerCollection."), "value");
				}
				this.InitializeListener(traceListener);
				this.list[index] = traceListener;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Diagnostics.TraceListenerCollection" /> is read-only</summary>
		/// <returns>Always <see langword="false" />.</returns>
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Diagnostics.TraceListenerCollection" /> has a fixed size.</summary>
		/// <returns>Always <see langword="false" />.</returns>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>Adds a trace listener to the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</summary>
		/// <param name="value">The object to add to the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</param>
		/// <returns>The position into which the new trace listener was inserted.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="value" /> is not a <see cref="T:System.Diagnostics.TraceListener" />.</exception>
		int IList.Add(object value)
		{
			TraceListener traceListener = value as TraceListener;
			if (traceListener == null)
			{
				throw new ArgumentException(SR.GetString("Only TraceListeners can be added to a TraceListenerCollection."), "value");
			}
			this.InitializeListener(traceListener);
			object critSec = TraceInternal.critSec;
			int result;
			lock (critSec)
			{
				result = this.list.Add(value);
			}
			return result;
		}

		/// <summary>Determines whether the <see cref="T:System.Diagnostics.TraceListenerCollection" /> contains a specific object.</summary>
		/// <param name="value">The object to locate in the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Diagnostics.TraceListenerCollection" />; otherwise, <see langword="false" />.</returns>
		bool IList.Contains(object value)
		{
			return this.list.Contains(value);
		}

		/// <summary>Determines the index of a specific object in the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</summary>
		/// <param name="value">The object to locate in the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</param>
		/// <returns>The index of <paramref name="value" /> if found in the <see cref="T:System.Diagnostics.TraceListenerCollection" />; otherwise, -1.</returns>
		int IList.IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		/// <summary>Inserts a <see cref="T:System.Diagnostics.TraceListener" /> object at the specified position in the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
		/// <param name="value">The <see cref="T:System.Diagnostics.TraceListener" /> object to insert into the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> is not a <see cref="T:System.Diagnostics.TraceListener" /> object.</exception>
		void IList.Insert(int index, object value)
		{
			TraceListener traceListener = value as TraceListener;
			if (traceListener == null)
			{
				throw new ArgumentException(SR.GetString("Only TraceListeners can be added to a TraceListenerCollection."), "value");
			}
			this.InitializeListener(traceListener);
			object critSec = TraceInternal.critSec;
			lock (critSec)
			{
				this.list.Insert(index, value);
			}
		}

		/// <summary>Removes an object from the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</summary>
		/// <param name="value">The object to remove from the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</param>
		void IList.Remove(object value)
		{
			object critSec = TraceInternal.critSec;
			lock (critSec)
			{
				this.list.Remove(value);
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Diagnostics.TraceListenerCollection" />.</summary>
		/// <returns>The current <see cref="T:System.Diagnostics.TraceListenerCollection" /> object.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Diagnostics.TraceListenerCollection" /> is synchronized (thread safe).</summary>
		/// <returns>Always <see langword="true" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		/// <summary>Copies a section of the current <see cref="T:System.Diagnostics.TraceListenerCollection" /> to the specified array of <see cref="T:System.Diagnostics.TraceListener" /> objects.</summary>
		/// <param name="array">The one-dimensional array of <see cref="T:System.Diagnostics.TraceListener" /> objects that is the destination of the elements copied from the <see cref="T:System.Diagnostics.TraceListenerCollection" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			object critSec = TraceInternal.critSec;
			lock (critSec)
			{
				this.list.CopyTo(array, index);
			}
		}

		private ArrayList list;
	}
}
