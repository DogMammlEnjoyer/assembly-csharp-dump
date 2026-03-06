using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity;

namespace System.Text.RegularExpressions
{
	/// <summary>Returns the set of captured groups in a single match.</summary>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerProxy<Group>))]
	[Serializable]
	public class GroupCollection : IList<Group>, ICollection<Group>, IEnumerable<Group>, IEnumerable, IReadOnlyList<Group>, IReadOnlyCollection<Group>, IList, ICollection
	{
		internal GroupCollection(Match match, Hashtable caps)
		{
			this._match = match;
			this._captureMap = caps;
		}

		/// <summary>Gets a value that indicates whether the collection is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> in all cases.</returns>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>Returns the number of groups in the collection.</summary>
		/// <returns>The number of groups in the collection.</returns>
		public int Count
		{
			get
			{
				return this._match._matchcount.Length;
			}
		}

		/// <summary>Enables access to a member of the collection by integer index.</summary>
		/// <param name="groupnum">The zero-based index of the collection member to be retrieved.</param>
		/// <returns>The member of the collection specified by <paramref name="groupnum" />.</returns>
		public Group this[int groupnum]
		{
			get
			{
				return this.GetGroup(groupnum);
			}
		}

		/// <summary>Enables access to a member of the collection by string index.</summary>
		/// <param name="groupname">The name of a capturing group.</param>
		/// <returns>The member of the collection specified by <paramref name="groupname" />.</returns>
		public Group this[string groupname]
		{
			get
			{
				if (this._match._regex != null)
				{
					return this.GetGroup(this._match._regex.GroupNumberFromName(groupname));
				}
				return Group.s_emptyGroup;
			}
		}

		/// <summary>Provides an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that contains all <see cref="T:System.Text.RegularExpressions.Group" /> objects in the <see cref="T:System.Text.RegularExpressions.GroupCollection" />.</returns>
		public IEnumerator GetEnumerator()
		{
			return new GroupCollection.Enumerator(this);
		}

		IEnumerator<Group> IEnumerable<Group>.GetEnumerator()
		{
			return new GroupCollection.Enumerator(this);
		}

		private Group GetGroup(int groupnum)
		{
			if (this._captureMap != null)
			{
				int groupnum2;
				if (this._captureMap.TryGetValue(groupnum, out groupnum2))
				{
					return this.GetGroupImpl(groupnum2);
				}
			}
			else if (groupnum < this._match._matchcount.Length && groupnum >= 0)
			{
				return this.GetGroupImpl(groupnum);
			}
			return Group.s_emptyGroup;
		}

		private Group GetGroupImpl(int groupnum)
		{
			if (groupnum == 0)
			{
				return this._match;
			}
			if (this._groups == null)
			{
				this._groups = new Group[this._match._matchcount.Length - 1];
				for (int i = 0; i < this._groups.Length; i++)
				{
					string name = this._match._regex.GroupNameFromNumber(i + 1);
					this._groups[i] = new Group(this._match.Text, this._match._matches[i + 1], this._match._matchcount[i + 1], name);
				}
			}
			return this._groups[groupnum - 1];
		}

		/// <summary>Gets a value that indicates whether access to the <see cref="T:System.Text.RegularExpressions.GroupCollection" /> is synchronized (thread-safe).</summary>
		/// <returns>
		///   <see langword="false" /> in all cases.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Text.RegularExpressions.GroupCollection" />.</summary>
		/// <returns>A copy of the <see cref="T:System.Text.RegularExpressions.Match" /> object to synchronize.</returns>
		public object SyncRoot
		{
			get
			{
				return this._match;
			}
		}

		/// <summary>Copies all the elements of the collection to the given array beginning at the given index.</summary>
		/// <param name="array">The array the collection is to be copied into.</param>
		/// <param name="arrayIndex">The position in the destination array where the copying is to begin.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IndexOutOfRangeException">
		///   <paramref name="arrayIndex" /> is outside the bounds of <paramref name="array" />.  
		/// -or-  
		/// <paramref name="arrayIndex" /> plus <see cref="P:System.Text.RegularExpressions.GroupCollection.Count" /> is outside the bounds of <paramref name="array" />.</exception>
		public void CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = arrayIndex;
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], num);
				num++;
			}
		}

		public void CopyTo(Group[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			int num = arrayIndex;
			for (int i = 0; i < this.Count; i++)
			{
				array[num] = this[i];
				num++;
			}
		}

		int IList<Group>.IndexOf(Group item)
		{
			EqualityComparer<Group> @default = EqualityComparer<Group>.Default;
			for (int i = 0; i < this.Count; i++)
			{
				if (@default.Equals(this[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		void IList<Group>.Insert(int index, Group item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList<Group>.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		Group IList<Group>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		void ICollection<Group>.Add(Group item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void ICollection<Group>.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool ICollection<Group>.Contains(Group item)
		{
			return ((IList<Group>)this).IndexOf(item) >= 0;
		}

		bool ICollection<Group>.Remove(Group item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool IList.Contains(object value)
		{
			return value is Group && ((ICollection<Group>)this).Contains((Group)value);
		}

		int IList.IndexOf(object value)
		{
			if (!(value is Group))
			{
				return -1;
			}
			return ((IList<Group>)this).IndexOf((Group)value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		internal GroupCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly Match _match;

		private readonly Hashtable _captureMap;

		private Group[] _groups;

		private sealed class Enumerator : IEnumerator<Group>, IDisposable, IEnumerator
		{
			internal Enumerator(GroupCollection collection)
			{
				this._collection = collection;
				this._index = -1;
			}

			public bool MoveNext()
			{
				int count = this._collection.Count;
				if (this._index >= count)
				{
					return false;
				}
				this._index++;
				return this._index < count;
			}

			public Group Current
			{
				get
				{
					if (this._index < 0 || this._index >= this._collection.Count)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._collection[this._index];
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
				this._index = -1;
			}

			void IDisposable.Dispose()
			{
			}

			private readonly GroupCollection _collection;

			private int _index;
		}
	}
}
