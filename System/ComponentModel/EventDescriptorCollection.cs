using System;
using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel
{
	/// <summary>Represents a collection of <see cref="T:System.ComponentModel.EventDescriptor" /> objects.</summary>
	public class EventDescriptorCollection : ICollection, IEnumerable, IList
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EventDescriptorCollection" /> class with the given array of <see cref="T:System.ComponentModel.EventDescriptor" /> objects.</summary>
		/// <param name="events">An array of type <see cref="T:System.ComponentModel.EventDescriptor" /> that provides the events for this collection.</param>
		public EventDescriptorCollection(EventDescriptor[] events)
		{
			if (events == null)
			{
				this._events = Array.Empty<EventDescriptor>();
			}
			else
			{
				this._events = events;
				this.Count = events.Length;
			}
			this._eventsOwned = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EventDescriptorCollection" /> class with the given array of <see cref="T:System.ComponentModel.EventDescriptor" /> objects. The collection is optionally read-only.</summary>
		/// <param name="events">An array of type <see cref="T:System.ComponentModel.EventDescriptor" /> that provides the events for this collection.</param>
		/// <param name="readOnly">
		///   <see langword="true" /> to specify a read-only collection; otherwise, <see langword="false" />.</param>
		public EventDescriptorCollection(EventDescriptor[] events, bool readOnly) : this(events)
		{
			this._readOnly = readOnly;
		}

		private EventDescriptorCollection(EventDescriptor[] events, int eventCount, string[] namedSort, IComparer comparer)
		{
			this._eventsOwned = false;
			if (namedSort != null)
			{
				this._namedSort = (string[])namedSort.Clone();
			}
			this._comparer = comparer;
			this._events = events;
			this.Count = eventCount;
			this._needSort = true;
		}

		/// <summary>Gets the number of event descriptors in the collection.</summary>
		/// <returns>The number of event descriptors in the collection.</returns>
		public int Count { get; private set; }

		/// <summary>Gets or sets the event with the specified index number.</summary>
		/// <param name="index">The zero-based index number of the <see cref="T:System.ComponentModel.EventDescriptor" /> to get or set.</param>
		/// <returns>The <see cref="T:System.ComponentModel.EventDescriptor" /> with the specified index number.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">
		///   <paramref name="index" /> is not a valid index for <see cref="P:System.ComponentModel.EventDescriptorCollection.Item(System.Int32)" />.</exception>
		public virtual EventDescriptor this[int index]
		{
			get
			{
				if (index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				this.EnsureEventsOwned();
				return this._events[index];
			}
		}

		/// <summary>Gets or sets the event with the specified name.</summary>
		/// <param name="name">The name of the <see cref="T:System.ComponentModel.EventDescriptor" /> to get or set.</param>
		/// <returns>The <see cref="T:System.ComponentModel.EventDescriptor" /> with the specified name, or <see langword="null" /> if the event does not exist.</returns>
		public virtual EventDescriptor this[string name]
		{
			get
			{
				return this.Find(name, false);
			}
		}

		/// <summary>Adds an <see cref="T:System.ComponentModel.EventDescriptor" /> to the end of the collection.</summary>
		/// <param name="value">An <see cref="T:System.ComponentModel.EventDescriptor" /> to add to the collection.</param>
		/// <returns>The position of the <see cref="T:System.ComponentModel.EventDescriptor" /> within the collection.</returns>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public int Add(EventDescriptor value)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException();
			}
			this.EnsureSize(this.Count + 1);
			EventDescriptor[] events = this._events;
			int count = this.Count;
			this.Count = count + 1;
			events[count] = value;
			return this.Count - 1;
		}

		/// <summary>Removes all objects from the collection.</summary>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void Clear()
		{
			if (this._readOnly)
			{
				throw new NotSupportedException();
			}
			this.Count = 0;
		}

		/// <summary>Returns whether the collection contains the given <see cref="T:System.ComponentModel.EventDescriptor" />.</summary>
		/// <param name="value">The <see cref="T:System.ComponentModel.EventDescriptor" /> to find within the collection.</param>
		/// <returns>
		///   <see langword="true" /> if the collection contains the <paramref name="value" /> parameter given; otherwise, <see langword="false" />.</returns>
		public bool Contains(EventDescriptor value)
		{
			return this.IndexOf(value) >= 0;
		}

		/// <summary>Copies the elements of the collection to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from collection. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureEventsOwned();
			Array.Copy(this._events, 0, array, index, this.Count);
		}

		private void EnsureEventsOwned()
		{
			if (!this._eventsOwned)
			{
				this._eventsOwned = true;
				if (this._events != null)
				{
					EventDescriptor[] array = new EventDescriptor[this.Count];
					Array.Copy(this._events, 0, array, 0, this.Count);
					this._events = array;
				}
			}
			if (this._needSort)
			{
				this._needSort = false;
				this.InternalSort(this._namedSort);
			}
		}

		private void EnsureSize(int sizeNeeded)
		{
			if (sizeNeeded <= this._events.Length)
			{
				return;
			}
			if (this._events.Length == 0)
			{
				this.Count = 0;
				this._events = new EventDescriptor[sizeNeeded];
				return;
			}
			this.EnsureEventsOwned();
			EventDescriptor[] array = new EventDescriptor[Math.Max(sizeNeeded, this._events.Length * 2)];
			Array.Copy(this._events, 0, array, 0, this.Count);
			this._events = array;
		}

		/// <summary>Gets the description of the event with the specified name in the collection.</summary>
		/// <param name="name">The name of the event to get from the collection.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> if you want to ignore the case of the event; otherwise, <see langword="false" />.</param>
		/// <returns>The <see cref="T:System.ComponentModel.EventDescriptor" /> with the specified name, or <see langword="null" /> if the event does not exist.</returns>
		public virtual EventDescriptor Find(string name, bool ignoreCase)
		{
			EventDescriptor result = null;
			if (ignoreCase)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (string.Equals(this._events[i].Name, name, StringComparison.OrdinalIgnoreCase))
					{
						result = this._events[i];
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.Count; j++)
				{
					if (string.Equals(this._events[j].Name, name, StringComparison.Ordinal))
					{
						result = this._events[j];
						break;
					}
				}
			}
			return result;
		}

		/// <summary>Returns the index of the given <see cref="T:System.ComponentModel.EventDescriptor" />.</summary>
		/// <param name="value">The <see cref="T:System.ComponentModel.EventDescriptor" /> to find within the collection.</param>
		/// <returns>The index of the given <see cref="T:System.ComponentModel.EventDescriptor" /> within the collection.</returns>
		public int IndexOf(EventDescriptor value)
		{
			return Array.IndexOf<EventDescriptor>(this._events, value, 0, this.Count);
		}

		/// <summary>Inserts an <see cref="T:System.ComponentModel.EventDescriptor" /> to the collection at a specified index.</summary>
		/// <param name="index">The index within the collection in which to insert the <paramref name="value" /> parameter.</param>
		/// <param name="value">An <see cref="T:System.ComponentModel.EventDescriptor" /> to insert into the collection.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void Insert(int index, EventDescriptor value)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException();
			}
			this.EnsureSize(this.Count + 1);
			if (index < this.Count)
			{
				Array.Copy(this._events, index, this._events, index + 1, this.Count - index);
			}
			this._events[index] = value;
			int count = this.Count;
			this.Count = count + 1;
		}

		/// <summary>Removes the specified <see cref="T:System.ComponentModel.EventDescriptor" /> from the collection.</summary>
		/// <param name="value">The <see cref="T:System.ComponentModel.EventDescriptor" /> to remove from the collection.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void Remove(EventDescriptor value)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException();
			}
			int num = this.IndexOf(value);
			if (num != -1)
			{
				this.RemoveAt(num);
			}
		}

		/// <summary>Removes the <see cref="T:System.ComponentModel.EventDescriptor" /> at the specified index from the collection.</summary>
		/// <param name="index">The index of the <see cref="T:System.ComponentModel.EventDescriptor" /> to remove.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void RemoveAt(int index)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException();
			}
			if (index < this.Count - 1)
			{
				Array.Copy(this._events, index + 1, this._events, index, this.Count - index - 1);
			}
			this._events[this.Count - 1] = null;
			int count = this.Count;
			this.Count = count - 1;
		}

		/// <summary>Gets an enumerator for this <see cref="T:System.ComponentModel.EventDescriptorCollection" />.</summary>
		/// <returns>An enumerator that implements <see cref="T:System.Collections.IEnumerator" />.</returns>
		public IEnumerator GetEnumerator()
		{
			if (this._events.Length == this.Count)
			{
				return this._events.GetEnumerator();
			}
			return new EventDescriptorCollection.ArraySubsetEnumerator(this._events, this.Count);
		}

		/// <summary>Sorts the members of this <see cref="T:System.ComponentModel.EventDescriptorCollection" />, using the default sort for this collection, which is usually alphabetical.</summary>
		/// <returns>The new <see cref="T:System.ComponentModel.EventDescriptorCollection" />.</returns>
		public virtual EventDescriptorCollection Sort()
		{
			return new EventDescriptorCollection(this._events, this.Count, this._namedSort, this._comparer);
		}

		/// <summary>Sorts the members of this <see cref="T:System.ComponentModel.EventDescriptorCollection" />, given a specified sort order.</summary>
		/// <param name="names">An array of strings describing the order in which to sort the <see cref="T:System.ComponentModel.EventDescriptor" /> objects in the collection.</param>
		/// <returns>The new <see cref="T:System.ComponentModel.EventDescriptorCollection" />.</returns>
		public virtual EventDescriptorCollection Sort(string[] names)
		{
			return new EventDescriptorCollection(this._events, this.Count, names, this._comparer);
		}

		/// <summary>Sorts the members of this <see cref="T:System.ComponentModel.EventDescriptorCollection" />, given a specified sort order and an <see cref="T:System.Collections.IComparer" />.</summary>
		/// <param name="names">An array of strings describing the order in which to sort the <see cref="T:System.ComponentModel.EventDescriptor" /> objects in the collection.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.IComparer" /> to use to sort the <see cref="T:System.ComponentModel.EventDescriptor" /> objects in this collection.</param>
		/// <returns>The new <see cref="T:System.ComponentModel.EventDescriptorCollection" />.</returns>
		public virtual EventDescriptorCollection Sort(string[] names, IComparer comparer)
		{
			return new EventDescriptorCollection(this._events, this.Count, names, comparer);
		}

		/// <summary>Sorts the members of this <see cref="T:System.ComponentModel.EventDescriptorCollection" />, using the specified <see cref="T:System.Collections.IComparer" />.</summary>
		/// <param name="comparer">An <see cref="T:System.Collections.IComparer" /> to use to sort the <see cref="T:System.ComponentModel.EventDescriptor" /> objects in this collection.</param>
		/// <returns>The new <see cref="T:System.ComponentModel.EventDescriptorCollection" />.</returns>
		public virtual EventDescriptorCollection Sort(IComparer comparer)
		{
			return new EventDescriptorCollection(this._events, this.Count, this._namedSort, comparer);
		}

		/// <summary>Sorts the members of this <see cref="T:System.ComponentModel.EventDescriptorCollection" />. The specified order is applied first, followed by the default sort for this collection, which is usually alphabetical.</summary>
		/// <param name="names">An array of strings describing the order in which to sort the <see cref="T:System.ComponentModel.EventDescriptor" /> objects in this collection.</param>
		protected void InternalSort(string[] names)
		{
			if (this._events.Length == 0)
			{
				return;
			}
			this.InternalSort(this._comparer);
			if (names != null && names.Length != 0)
			{
				List<EventDescriptor> list = new List<EventDescriptor>(this._events);
				int num = 0;
				int num2 = this._events.Length;
				for (int i = 0; i < names.Length; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						EventDescriptor eventDescriptor = list[j];
						if (eventDescriptor != null && eventDescriptor.Name.Equals(names[i]))
						{
							this._events[num++] = eventDescriptor;
							list[j] = null;
							break;
						}
					}
				}
				for (int k = 0; k < num2; k++)
				{
					if (list[k] != null)
					{
						this._events[num++] = list[k];
					}
				}
			}
		}

		/// <summary>Sorts the members of this <see cref="T:System.ComponentModel.EventDescriptorCollection" />, using the specified <see cref="T:System.Collections.IComparer" />.</summary>
		/// <param name="sorter">A comparer to use to sort the <see cref="T:System.ComponentModel.EventDescriptor" /> objects in this collection.</param>
		protected void InternalSort(IComparer sorter)
		{
			if (sorter == null)
			{
				TypeDescriptor.SortDescriptorArray(this);
				return;
			}
			Array.Sort(this._events, sorter);
		}

		/// <summary>Gets a value indicating whether access to the collection is synchronized.</summary>
		/// <returns>
		///   <see langword="true" /> if access to the collection is synchronized; otherwise, <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
		/// <returns>An object that can be used to synchronize access to the collection.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the number of elements contained in the collection.</summary>
		/// <returns>The number of elements contained in the collection.</returns>
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Gets or sets the element at the specified index.</summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		/// <exception cref="T:System.IndexOutOfRangeException">
		///   <paramref name="index" /> is less than 0.  
		/// -or-  
		/// <paramref name="index" /> is equal to or greater than <see cref="P:System.ComponentModel.EventDescriptorCollection.Count" />.</exception>
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (this._readOnly)
				{
					throw new NotSupportedException();
				}
				if (index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				this.EnsureEventsOwned();
				this._events[index] = (EventDescriptor)value;
			}
		}

		/// <summary>Adds an item to the collection.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to add to the collection.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		int IList.Add(object value)
		{
			return this.Add((EventDescriptor)value);
		}

		/// <summary>Determines whether the collection contains a specific value.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the collection.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Object" /> is found in the collection; otherwise, <see langword="false" />.</returns>
		bool IList.Contains(object value)
		{
			return this.Contains((EventDescriptor)value);
		}

		/// <summary>Removes all the items from the collection.</summary>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		void IList.Clear()
		{
			this.Clear();
		}

		/// <summary>Determines the index of a specific item in the collection.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the collection.</param>
		/// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
		int IList.IndexOf(object value)
		{
			return this.IndexOf((EventDescriptor)value);
		}

		/// <summary>Inserts an item to the collection at the specified index.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to insert into the collection.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		void IList.Insert(int index, object value)
		{
			this.Insert(index, (EventDescriptor)value);
		}

		/// <summary>Removes the first occurrence of a specific object from the collection.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to remove from the collection.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		void IList.Remove(object value)
		{
			this.Remove((EventDescriptor)value);
		}

		/// <summary>Removes the item at the specified index.</summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		/// <summary>Gets a value indicating whether the collection is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection is read-only; otherwise, <see langword="false" />.</returns>
		bool IList.IsReadOnly
		{
			get
			{
				return this._readOnly;
			}
		}

		/// <summary>Gets a value indicating whether the collection has a fixed size.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection has a fixed size; otherwise, <see langword="false" />.</returns>
		bool IList.IsFixedSize
		{
			get
			{
				return this._readOnly;
			}
		}

		private EventDescriptor[] _events;

		private string[] _namedSort;

		private readonly IComparer _comparer;

		private bool _eventsOwned;

		private bool _needSort;

		private readonly bool _readOnly;

		/// <summary>Specifies an empty collection to use, rather than creating a new one with no items. This <see langword="static" /> field is read-only.</summary>
		public static readonly EventDescriptorCollection Empty = new EventDescriptorCollection(null, true);

		private class ArraySubsetEnumerator : IEnumerator
		{
			public ArraySubsetEnumerator(Array array, int count)
			{
				this._array = array;
				this._total = count;
				this._current = -1;
			}

			public bool MoveNext()
			{
				if (this._current < this._total - 1)
				{
					this._current++;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this._current = -1;
			}

			public object Current
			{
				get
				{
					if (this._current == -1)
					{
						throw new InvalidOperationException();
					}
					return this._array.GetValue(this._current);
				}
			}

			private readonly Array _array;

			private readonly int _total;

			private int _current;
		}
	}
}
