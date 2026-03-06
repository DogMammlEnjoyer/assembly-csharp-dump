using System;
using System.Collections;
using Unity;

namespace System.Diagnostics
{
	/// <summary>Defines size and enumerators for a collection of <see cref="T:System.Diagnostics.EventLogEntry" /> instances.</summary>
	public class EventLogEntryCollection : ICollection, IEnumerable
	{
		internal EventLogEntryCollection(EventLogImpl impl)
		{
			this._impl = impl;
		}

		/// <summary>Gets the number of entries in the event log (that is, the number of elements in the <see cref="T:System.Diagnostics.EventLogEntry" /> collection).</summary>
		/// <returns>The number of entries currently in the event log.</returns>
		public int Count
		{
			get
			{
				return this._impl.EntryCount;
			}
		}

		/// <summary>Gets an entry in the event log, based on an index that starts at 0 (zero).</summary>
		/// <param name="index">The zero-based index that is associated with the event log entry.</param>
		/// <returns>The event log entry at the location that is specified by the <paramref name="index" /> parameter.</returns>
		public virtual EventLogEntry this[int index]
		{
			get
			{
				return this._impl[index];
			}
		}

		/// <summary>Gets a value that indicates whether access to the <see cref="T:System.Diagnostics.EventLogEntryCollection" /> is synchronized (thread-safe).</summary>
		/// <returns>
		///   <see langword="false" /> if access to the collection is not synchronized (thread-safe).</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Diagnostics.EventLogEntryCollection" /> object.</summary>
		/// <returns>An object that can be used to synchronize access to the collection.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Copies the elements of the <see cref="T:System.Diagnostics.EventLogEntryCollection" /> to an array of <see cref="T:System.Diagnostics.EventLogEntry" /> instances, starting at a particular array index.</summary>
		/// <param name="entries">The one-dimensional array of <see cref="T:System.Diagnostics.EventLogEntry" /> instances that is the destination of the elements copied from the collection. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in the array at which copying begins.</param>
		public void CopyTo(EventLogEntry[] entries, int index)
		{
			EventLogEntry[] entries2 = this._impl.GetEntries();
			Array.Copy(entries2, 0, entries, index, entries2.Length);
		}

		/// <summary>Supports a simple iteration over the <see cref="T:System.Diagnostics.EventLogEntryCollection" /> object.</summary>
		/// <returns>An object that can be used to iterate over the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return new EventLogEntryCollection.EventLogEntryEnumerator(this._impl);
		}

		/// <summary>Copies the elements of the collection to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements that are copied from the collection. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			EventLogEntry[] entries = this._impl.GetEntries();
			Array.Copy(entries, 0, array, index, entries.Length);
		}

		internal EventLogEntryCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly EventLogImpl _impl;

		private class EventLogEntryEnumerator : IEnumerator
		{
			internal EventLogEntryEnumerator(EventLogImpl impl)
			{
				this._impl = impl;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public EventLogEntry Current
			{
				get
				{
					if (this._currentEntry != null)
					{
						return this._currentEntry;
					}
					throw new InvalidOperationException("No current EventLog entry available, cursor is located before the first or after the last element of the enumeration.");
				}
			}

			public bool MoveNext()
			{
				this._currentIndex++;
				if (this._currentIndex >= this._impl.EntryCount)
				{
					this._currentEntry = null;
					return false;
				}
				this._currentEntry = this._impl[this._currentIndex];
				return true;
			}

			public void Reset()
			{
				this._currentIndex = -1;
			}

			private readonly EventLogImpl _impl;

			private int _currentIndex = -1;

			private EventLogEntry _currentEntry;
		}
	}
}
