using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Collections.Generic
{
	/// <summary>Represents a set of values.To browse the .NET Framework source code for this type, see the Reference Source.</summary>
	/// <typeparam name="T">The type of elements in the hash set.</typeparam>
	[DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISet<T>, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class that is empty and uses the default equality comparer for the set type.</summary>
		public HashSet() : this(EqualityComparer<T>.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class that is empty and uses the specified equality comparer for the set type.</summary>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> implementation for the set type.</param>
		public HashSet(IEqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			this._comparer = comparer;
			this._lastIndex = 0;
			this._count = 0;
			this._freeList = -1;
			this._version = 0;
		}

		/// <summary>
		/// 			Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class that is empty, but has reserved space for <paramref name="capacity" /> items and uses the default equality comparer for the set type.
		/// 		</summary>
		/// <param name="capacity">The initial size of the <see cref="T:System.Collections.Generic.HashSet`1" /></param>
		public HashSet(int capacity) : this(capacity, EqualityComparer<T>.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class that uses the default equality comparer for the set type, contains elements copied from the specified collection, and has sufficient capacity to accommodate the number of elements copied.</summary>
		/// <param name="collection">The collection whose elements are copied to the new set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="collection" /> is <see langword="null" />.</exception>
		public HashSet(IEnumerable<T> collection) : this(collection, EqualityComparer<T>.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class that uses the specified equality comparer for the set type, contains elements copied from the specified collection, and has sufficient capacity to accommodate the number of elements copied.</summary>
		/// <param name="collection">The collection whose elements are copied to the new set.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> implementation for the set type.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="collection" /> is <see langword="null" />.</exception>
		public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			HashSet<T> hashSet = collection as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				this.CopyFrom(hashSet);
				return;
			}
			ICollection<T> collection2 = collection as ICollection<!0>;
			int capacity = (collection2 == null) ? 0 : collection2.Count;
			this.Initialize(capacity);
			this.UnionWith(collection);
			if (this._count > 0 && this._slots.Length / this._count > 3)
			{
				this.TrimExcess();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class with serialized data.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		protected HashSet(SerializationInfo info, StreamingContext context)
		{
			this._siInfo = info;
		}

		private void CopyFrom(HashSet<T> source)
		{
			int count = source._count;
			if (count == 0)
			{
				return;
			}
			int num = source._buckets.Length;
			if (HashHelpers.ExpandPrime(count + 1) >= num)
			{
				this._buckets = (int[])source._buckets.Clone();
				this._slots = (HashSet<T>.Slot[])source._slots.Clone();
				this._lastIndex = source._lastIndex;
				this._freeList = source._freeList;
			}
			else
			{
				int lastIndex = source._lastIndex;
				HashSet<T>.Slot[] slots = source._slots;
				this.Initialize(count);
				int num2 = 0;
				for (int i = 0; i < lastIndex; i++)
				{
					int hashCode = slots[i].hashCode;
					if (hashCode >= 0)
					{
						this.AddValue(num2, hashCode, slots[i].value);
						num2++;
					}
				}
				this._lastIndex = num2;
			}
			this._count = count;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="T:System.Collections.Generic.HashSet`1" /> class that uses the specified equality comparer for the set type, and has sufficient capacity to accommodate <paramref name="capacity" /> elements.
		/// 		</summary>
		/// <param name="capacity">The initial size of the <see cref="T:System.Collections.Generic.HashSet`1" /></param>
		/// <param name="comparer">
		/// 				The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or null (Nothing in Visual Basic) to use the default <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation for the set type.
		/// 			</param>
		public HashSet(int capacity, IEqualityComparer<T> comparer) : this(comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity > 0)
			{
				this.Initialize(capacity);
			}
		}

		void ICollection<!0>.Add(T item)
		{
			this.AddIfNotPresent(item);
		}

		/// <summary>Removes all elements from a <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		public void Clear()
		{
			if (this._lastIndex > 0)
			{
				Array.Clear(this._slots, 0, this._lastIndex);
				Array.Clear(this._buckets, 0, this._buckets.Length);
				this._lastIndex = 0;
				this._count = 0;
				this._freeList = -1;
			}
			this._version++;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.HashSet`1" /> object contains the specified element.</summary>
		/// <param name="item">The element to locate in the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object contains the specified element; otherwise, <see langword="false" />.</returns>
		public bool Contains(T item)
		{
			if (this._buckets != null)
			{
				int num = 0;
				int num2 = this.InternalGetHashCode(item);
				HashSet<T>.Slot[] slots = this._slots;
				for (int i = this._buckets[num2 % this._buckets.Length] - 1; i >= 0; i = slots[i].next)
				{
					if (slots[i].hashCode == num2 && this._comparer.Equals(slots[i].value, item))
					{
						return true;
					}
					if (num >= slots.Length)
					{
						throw new InvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
					}
					num++;
				}
			}
			return false;
		}

		/// <summary>Copies the elements of a <see cref="T:System.Collections.Generic.HashSet`1" /> object to an array, starting at the specified array index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.HashSet`1" /> object. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="arrayIndex" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="arrayIndex" /> is greater than the length of the destination <paramref name="array" />.</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex, this._count);
		}

		/// <summary>Removes the specified element from a <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		/// <param name="item">The element to remove.</param>
		/// <returns>
		///     <see langword="true" /> if the element is successfully found and removed; otherwise, <see langword="false" />.  This method returns <see langword="false" /> if <paramref name="item" /> is not found in the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</returns>
		public bool Remove(T item)
		{
			if (this._buckets != null)
			{
				int num = this.InternalGetHashCode(item);
				int num2 = num % this._buckets.Length;
				int num3 = -1;
				int num4 = 0;
				HashSet<T>.Slot[] slots = this._slots;
				for (int i = this._buckets[num2] - 1; i >= 0; i = slots[i].next)
				{
					if (slots[i].hashCode == num && this._comparer.Equals(slots[i].value, item))
					{
						if (num3 < 0)
						{
							this._buckets[num2] = slots[i].next + 1;
						}
						else
						{
							slots[num3].next = slots[i].next;
						}
						slots[i].hashCode = -1;
						if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
						{
							slots[i].value = default(T);
						}
						slots[i].next = this._freeList;
						this._count--;
						this._version++;
						if (this._count == 0)
						{
							this._lastIndex = 0;
							this._freeList = -1;
						}
						else
						{
							this._freeList = i;
						}
						return true;
					}
					if (num4 >= slots.Length)
					{
						throw new InvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
					}
					num4++;
					num3 = i;
				}
			}
			return false;
		}

		/// <summary>Gets the number of elements that are contained in a set.</summary>
		/// <returns>The number of elements that are contained in the set.</returns>
		public int Count
		{
			get
			{
				return this._count;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Returns an enumerator that iterates through a <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.HashSet`1.Enumerator" /> object for the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</returns>
		public HashSet<T>.Enumerator GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize a <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="info" /> is <see langword="null" />.</exception>
		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this._version);
			info.AddValue("Comparer", this._comparer, typeof(IComparer<T>));
			info.AddValue("Capacity", (this._buckets == null) ? 0 : this._buckets.Length);
			if (this._buckets != null)
			{
				T[] array = new T[this._count];
				this.CopyTo(array);
				info.AddValue("Elements", array, typeof(T[]));
			}
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
		/// <param name="sender">The source of the deserialization event.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Generic.HashSet`1" /> object is invalid.</exception>
		public virtual void OnDeserialization(object sender)
		{
			if (this._siInfo == null)
			{
				return;
			}
			int @int = this._siInfo.GetInt32("Capacity");
			this._comparer = (IEqualityComparer<T>)this._siInfo.GetValue("Comparer", typeof(IEqualityComparer<T>));
			this._freeList = -1;
			if (@int != 0)
			{
				this._buckets = new int[@int];
				this._slots = new HashSet<T>.Slot[@int];
				T[] array = (T[])this._siInfo.GetValue("Elements", typeof(T[]));
				if (array == null)
				{
					throw new SerializationException("The keys for this dictionary are missing.");
				}
				for (int i = 0; i < array.Length; i++)
				{
					this.AddIfNotPresent(array[i]);
				}
			}
			else
			{
				this._buckets = null;
			}
			this._version = this._siInfo.GetInt32("Version");
			this._siInfo = null;
		}

		/// <summary>Adds the specified element to a set.</summary>
		/// <param name="item">The element to add to the set.</param>
		/// <returns>
		///     <see langword="true" /> if the element is added to the <see cref="T:System.Collections.Generic.HashSet`1" /> object; <see langword="false" /> if the element is already present.</returns>
		public bool Add(T item)
		{
			return this.AddIfNotPresent(item);
		}

		/// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
		/// <param name="equalValue">The value to search for.</param>
		/// <param name="actualValue">The value from the set that the search found, or the default value of T when the search yielded no match.</param>
		/// <returns>A value indicating whether the search was successful.</returns>
		public bool TryGetValue(T equalValue, out T actualValue)
		{
			if (this._buckets != null)
			{
				int num = this.InternalIndexOf(equalValue);
				if (num >= 0)
				{
					actualValue = this._slots[num].value;
					return true;
				}
			}
			actualValue = default(T);
			return false;
		}

		/// <summary>Modifies the current <see cref="T:System.Collections.Generic.HashSet`1" /> object to contain all elements that are present in itself, the specified collection, or both.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T value in other)
			{
				this.AddIfNotPresent(value);
			}
		}

		/// <summary>Modifies the current <see cref="T:System.Collections.Generic.HashSet`1" /> object to contain only elements that are present in that object and in the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return;
			}
			if (other == this)
			{
				return;
			}
			ICollection<T> collection = other as ICollection<!0>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					this.Clear();
					return;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
				{
					this.IntersectWithHashSetWithSameEC(hashSet);
					return;
				}
			}
			this.IntersectWithEnumerable(other);
		}

		/// <summary>Removes all elements in the specified collection from the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		/// <param name="other">The collection of items to remove from the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return;
			}
			if (other == this)
			{
				this.Clear();
				return;
			}
			foreach (T item in other)
			{
				this.Remove(item);
			}
		}

		/// <summary>Modifies the current <see cref="T:System.Collections.Generic.HashSet`1" /> object to contain only elements that are present either in that object or in the specified collection, but not both.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				this.UnionWith(other);
				return;
			}
			if (other == this)
			{
				this.Clear();
				return;
			}
			HashSet<T> hashSet = other as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				this.SymmetricExceptWithUniqueHashSet(hashSet);
				return;
			}
			this.SymmetricExceptWithEnumerable(other);
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.HashSet`1" /> object is a subset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object is a subset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return true;
			}
			if (other == this)
			{
				return true;
			}
			HashSet<T> hashSet = other as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				return this._count <= hashSet.Count && this.IsSubsetOfHashSetWithSameEC(hashSet);
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, false);
			return elementCount.uniqueCount == this._count && elementCount.unfoundCount >= 0;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.HashSet`1" /> object is a proper subset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object is a proper subset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other == this)
			{
				return false;
			}
			ICollection<T> collection = other as ICollection<!0>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return false;
				}
				if (this._count == 0)
				{
					return collection.Count > 0;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
				{
					return this._count < hashSet.Count && this.IsSubsetOfHashSetWithSameEC(hashSet);
				}
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, false);
			return elementCount.uniqueCount == this._count && elementCount.unfoundCount > 0;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.HashSet`1" /> object is a superset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object is a superset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other == this)
			{
				return true;
			}
			ICollection<T> collection = other as ICollection<!0>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return true;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet) && hashSet.Count > this._count)
				{
					return false;
				}
			}
			return this.ContainsAllElements(other);
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.HashSet`1" /> object is a proper superset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object. </param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object is a proper superset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return false;
			}
			if (other == this)
			{
				return false;
			}
			ICollection<T> collection = other as ICollection<!0>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return true;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
				{
					return hashSet.Count < this._count && this.ContainsAllElements(hashSet);
				}
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, true);
			return elementCount.uniqueCount < this._count && elementCount.unfoundCount == 0;
		}

		/// <summary>Determines whether the current <see cref="T:System.Collections.Generic.HashSet`1" /> object and a specified collection share common elements.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object and <paramref name="other" /> share at least one common element; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			foreach (T item in other)
			{
				if (this.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.HashSet`1" /> object and the specified collection contain the same elements.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.HashSet`1" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Collections.Generic.HashSet`1" /> object is equal to <paramref name="other" />; otherwise, false.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="other" /> is <see langword="null" />.</exception>
		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other == this)
			{
				return true;
			}
			HashSet<T> hashSet = other as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				return this._count == hashSet.Count && this.ContainsAllElements(hashSet);
			}
			ICollection<T> collection = other as ICollection<!0>;
			if (collection != null && this._count == 0 && collection.Count > 0)
			{
				return false;
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, true);
			return elementCount.uniqueCount == this._count && elementCount.unfoundCount == 0;
		}

		/// <summary>Copies the elements of a <see cref="T:System.Collections.Generic.HashSet`1" /> object to an array.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.HashSet`1" /> object. The array must have zero-based indexing.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> is <see langword="null" />.</exception>
		public void CopyTo(T[] array)
		{
			this.CopyTo(array, 0, this._count);
		}

		/// <summary>Copies the specified number of elements of a <see cref="T:System.Collections.Generic.HashSet`1" /> object to an array, starting at the specified array index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.HashSet`1" /> object. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <param name="count">The number of elements to copy to <paramref name="array" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="arrayIndex" /> is less than 0.-or-
		///         <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="arrayIndex" /> is greater than the length of the destination <paramref name="array" />.-or-
		///         <paramref name="count" /> is greater than the available space from the <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Non negative number is required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "Non negative number is required.");
			}
			if (arrayIndex > array.Length || count > array.Length - arrayIndex)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			int num = 0;
			int num2 = 0;
			while (num2 < this._lastIndex && num < count)
			{
				if (this._slots[num2].hashCode >= 0)
				{
					array[arrayIndex + num] = this._slots[num2].value;
					num++;
				}
				num2++;
			}
		}

		/// <summary>Removes all elements that match the conditions defined by the specified predicate from a <see cref="T:System.Collections.Generic.HashSet`1" /> collection.</summary>
		/// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the elements to remove.</param>
		/// <returns>The number of elements that were removed from the <see cref="T:System.Collections.Generic.HashSet`1" /> collection.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="match" /> is <see langword="null" />.</exception>
		public int RemoveWhere(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			int num = 0;
			for (int i = 0; i < this._lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0)
				{
					T value = this._slots[i].value;
					if (match(value) && this.Remove(value))
					{
						num++;
					}
				}
			}
			return num;
		}

		/// <summary>Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> object that is used to determine equality for the values in the set.</summary>
		/// <returns>The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> object that is used to determine equality for the values in the set.</returns>
		public IEqualityComparer<T> Comparer
		{
			get
			{
				return this._comparer;
			}
		}

		public int EnsureCapacity(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			int num = (this._slots == null) ? 0 : this._slots.Length;
			if (num >= capacity)
			{
				return num;
			}
			if (this._buckets == null)
			{
				return this.Initialize(capacity);
			}
			int prime = HashHelpers.GetPrime(capacity);
			this.SetCapacity(prime);
			return prime;
		}

		/// <summary>Sets the capacity of a <see cref="T:System.Collections.Generic.HashSet`1" /> object to the actual number of elements it contains, rounded up to a nearby, implementation-specific value.</summary>
		public void TrimExcess()
		{
			if (this._count == 0)
			{
				this._buckets = null;
				this._slots = null;
				this._version++;
				return;
			}
			int prime = HashHelpers.GetPrime(this._count);
			HashSet<T>.Slot[] array = new HashSet<T>.Slot[prime];
			int[] array2 = new int[prime];
			int num = 0;
			for (int i = 0; i < this._lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0)
				{
					array[num] = this._slots[i];
					int num2 = array[num].hashCode % prime;
					array[num].next = array2[num2] - 1;
					array2[num2] = num + 1;
					num++;
				}
			}
			this._lastIndex = num;
			this._slots = array;
			this._buckets = array2;
			this._freeList = -1;
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEqualityComparer" /> object that can be used for equality testing of a <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		/// <returns>An <see cref="T:System.Collections.IEqualityComparer" /> object that can be used for deep equality testing of the <see cref="T:System.Collections.Generic.HashSet`1" /> object.</returns>
		public static IEqualityComparer<HashSet<T>> CreateSetComparer()
		{
			return new HashSetEqualityComparer<T>();
		}

		private int Initialize(int capacity)
		{
			int prime = HashHelpers.GetPrime(capacity);
			this._buckets = new int[prime];
			this._slots = new HashSet<T>.Slot[prime];
			return prime;
		}

		private void IncreaseCapacity()
		{
			int num = HashHelpers.ExpandPrime(this._count);
			if (num <= this._count)
			{
				throw new ArgumentException("HashSet capacity is too big.");
			}
			this.SetCapacity(num);
		}

		private void SetCapacity(int newSize)
		{
			HashSet<T>.Slot[] array = new HashSet<T>.Slot[newSize];
			if (this._slots != null)
			{
				Array.Copy(this._slots, 0, array, 0, this._lastIndex);
			}
			int[] array2 = new int[newSize];
			for (int i = 0; i < this._lastIndex; i++)
			{
				int num = array[i].hashCode % newSize;
				array[i].next = array2[num] - 1;
				array2[num] = i + 1;
			}
			this._slots = array;
			this._buckets = array2;
		}

		private bool AddIfNotPresent(T value)
		{
			if (this._buckets == null)
			{
				this.Initialize(0);
			}
			int num = this.InternalGetHashCode(value);
			int num2 = num % this._buckets.Length;
			int num3 = 0;
			HashSet<T>.Slot[] slots = this._slots;
			for (int i = this._buckets[num2] - 1; i >= 0; i = slots[i].next)
			{
				if (slots[i].hashCode == num && this._comparer.Equals(slots[i].value, value))
				{
					return false;
				}
				if (num3 >= slots.Length)
				{
					throw new InvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
				}
				num3++;
			}
			int num4;
			if (this._freeList >= 0)
			{
				num4 = this._freeList;
				this._freeList = slots[num4].next;
			}
			else
			{
				if (this._lastIndex == slots.Length)
				{
					this.IncreaseCapacity();
					slots = this._slots;
					num2 = num % this._buckets.Length;
				}
				num4 = this._lastIndex;
				this._lastIndex++;
			}
			slots[num4].hashCode = num;
			slots[num4].value = value;
			slots[num4].next = this._buckets[num2] - 1;
			this._buckets[num2] = num4 + 1;
			this._count++;
			this._version++;
			return true;
		}

		private void AddValue(int index, int hashCode, T value)
		{
			int num = hashCode % this._buckets.Length;
			this._slots[index].hashCode = hashCode;
			this._slots[index].value = value;
			this._slots[index].next = this._buckets[num] - 1;
			this._buckets[num] = index + 1;
		}

		private bool ContainsAllElements(IEnumerable<T> other)
		{
			foreach (T item in other)
			{
				if (!this.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsSubsetOfHashSetWithSameEC(HashSet<T> other)
		{
			foreach (T item in this)
			{
				if (!other.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		private void IntersectWithHashSetWithSameEC(HashSet<T> other)
		{
			for (int i = 0; i < this._lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0)
				{
					T value = this._slots[i].value;
					if (!other.Contains(value))
					{
						this.Remove(value);
					}
				}
			}
		}

		private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
		{
			int lastIndex = this._lastIndex;
			int num = BitHelper.ToIntArrayLength(lastIndex);
			BitHelper bitHelper;
			if (num <= 100)
			{
				bitHelper = new BitHelper(stackalloc int[checked(unchecked((UIntPtr)num) * 4)], num);
			}
			else
			{
				bitHelper = new BitHelper(new int[num], num);
			}
			foreach (T item in other)
			{
				int num2 = this.InternalIndexOf(item);
				if (num2 >= 0)
				{
					bitHelper.MarkBit(num2);
				}
			}
			for (int i = 0; i < lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0 && !bitHelper.IsMarked(i))
				{
					this.Remove(this._slots[i].value);
				}
			}
		}

		private int InternalIndexOf(T item)
		{
			int num = 0;
			int num2 = this.InternalGetHashCode(item);
			HashSet<T>.Slot[] slots = this._slots;
			for (int i = this._buckets[num2 % this._buckets.Length] - 1; i >= 0; i = slots[i].next)
			{
				if (slots[i].hashCode == num2 && this._comparer.Equals(slots[i].value, item))
				{
					return i;
				}
				if (num >= slots.Length)
				{
					throw new InvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
				}
				num++;
			}
			return -1;
		}

		private void SymmetricExceptWithUniqueHashSet(HashSet<T> other)
		{
			foreach (T t in other)
			{
				if (!this.Remove(t))
				{
					this.AddIfNotPresent(t);
				}
			}
		}

		private unsafe void SymmetricExceptWithEnumerable(IEnumerable<T> other)
		{
			int lastIndex = this._lastIndex;
			int num = BitHelper.ToIntArrayLength(lastIndex);
			BitHelper bitHelper;
			checked
			{
				BitHelper bitHelper2;
				if (num <= 50)
				{
					bitHelper = new BitHelper(stackalloc int[unchecked((UIntPtr)num) * 4], num);
					bitHelper2 = new BitHelper(stackalloc int[unchecked((UIntPtr)num) * 4], num);
				}
				else
				{
					bitHelper = new BitHelper(new int[num], num);
					bitHelper2 = new BitHelper(new int[num], num);
				}
				foreach (T value in other)
				{
					int num2 = 0;
					if (this.AddOrGetLocation(value, out num2))
					{
						bitHelper2.MarkBit(num2);
					}
					else if (num2 < lastIndex && !bitHelper2.IsMarked(num2))
					{
						bitHelper.MarkBit(num2);
					}
				}
			}
			for (int i = 0; i < lastIndex; i++)
			{
				if (bitHelper.IsMarked(i))
				{
					this.Remove(this._slots[i].value);
				}
			}
		}

		private bool AddOrGetLocation(T value, out int location)
		{
			int num = this.InternalGetHashCode(value);
			int num2 = num % this._buckets.Length;
			int num3 = 0;
			HashSet<T>.Slot[] slots = this._slots;
			for (int i = this._buckets[num2] - 1; i >= 0; i = slots[i].next)
			{
				if (slots[i].hashCode == num && this._comparer.Equals(slots[i].value, value))
				{
					location = i;
					return false;
				}
				if (num3 >= slots.Length)
				{
					throw new InvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
				}
				num3++;
			}
			int num4;
			if (this._freeList >= 0)
			{
				num4 = this._freeList;
				this._freeList = slots[num4].next;
			}
			else
			{
				if (this._lastIndex == slots.Length)
				{
					this.IncreaseCapacity();
					slots = this._slots;
					num2 = num % this._buckets.Length;
				}
				num4 = this._lastIndex;
				this._lastIndex++;
			}
			slots[num4].hashCode = num;
			slots[num4].value = value;
			slots[num4].next = this._buckets[num2] - 1;
			this._buckets[num2] = num4 + 1;
			this._count++;
			this._version++;
			location = num4;
			return true;
		}

		private unsafe HashSet<T>.ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
		{
			HashSet<T>.ElementCount result;
			if (this._count == 0)
			{
				int num = 0;
				using (IEnumerator<T> enumerator = other.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						!0 ! = enumerator.Current;
						num++;
					}
				}
				result.uniqueCount = 0;
				result.unfoundCount = num;
				return result;
			}
			int num2 = BitHelper.ToIntArrayLength(this._lastIndex);
			BitHelper bitHelper;
			if (num2 <= 100)
			{
				bitHelper = new BitHelper(stackalloc int[checked(unchecked((UIntPtr)num2) * 4)], num2);
			}
			else
			{
				bitHelper = new BitHelper(new int[num2], num2);
			}
			int num3 = 0;
			int num4 = 0;
			foreach (T item in other)
			{
				int num5 = this.InternalIndexOf(item);
				if (num5 >= 0)
				{
					if (!bitHelper.IsMarked(num5))
					{
						bitHelper.MarkBit(num5);
						num4++;
					}
				}
				else
				{
					num3++;
					if (returnIfUnfound)
					{
						break;
					}
				}
			}
			result.uniqueCount = num4;
			result.unfoundCount = num3;
			return result;
		}

		internal static bool HashSetEquals(HashSet<T> set1, HashSet<T> set2, IEqualityComparer<T> comparer)
		{
			if (set1 == null)
			{
				return set2 == null;
			}
			if (set2 == null)
			{
				return false;
			}
			if (!HashSet<T>.AreEqualityComparersEqual(set1, set2))
			{
				foreach (T x in set2)
				{
					bool flag = false;
					foreach (T y in set1)
					{
						if (comparer.Equals(x, y))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			if (set1.Count != set2.Count)
			{
				return false;
			}
			foreach (T item in set2)
			{
				if (!set1.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2)
		{
			return set1.Comparer.Equals(set2.Comparer);
		}

		private int InternalGetHashCode(T item)
		{
			if (item == null)
			{
				return 0;
			}
			return this._comparer.GetHashCode(item) & int.MaxValue;
		}

		private const int Lower31BitMask = 2147483647;

		private const int StackAllocThreshold = 100;

		private const int ShrinkThreshold = 3;

		private const string CapacityName = "Capacity";

		private const string ElementsName = "Elements";

		private const string ComparerName = "Comparer";

		private const string VersionName = "Version";

		private int[] _buckets;

		private HashSet<T>.Slot[] _slots;

		private int _count;

		private int _lastIndex;

		private int _freeList;

		private IEqualityComparer<T> _comparer;

		private int _version;

		private SerializationInfo _siInfo;

		internal struct ElementCount
		{
			internal int uniqueCount;

			internal int unfoundCount;
		}

		internal struct Slot
		{
			internal int hashCode;

			internal int next;

			internal T value;
		}

		/// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.HashSet`1" /> object.</summary>
		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(HashSet<T> set)
			{
				this._set = set;
				this._index = 0;
				this._version = set._version;
				this._current = default(T);
			}

			/// <summary>Releases all resources used by a <see cref="T:System.Collections.Generic.HashSet`1.Enumerator" /> object.</summary>
			public void Dispose()
			{
			}

			/// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.HashSet`1" /> collection.</summary>
			/// <returns>
			///     <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNext()
			{
				if (this._version != this._set._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				while (this._index < this._set._lastIndex)
				{
					if (this._set._slots[this._index].hashCode >= 0)
					{
						this._current = this._set._slots[this._index].value;
						this._index++;
						return true;
					}
					this._index++;
				}
				this._index = this._set._lastIndex + 1;
				this._current = default(T);
				return false;
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the <see cref="T:System.Collections.Generic.HashSet`1" /> collection at the current position of the enumerator.</returns>
			public T Current
			{
				get
				{
					return this._current;
				}
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the collection at the current position of the enumerator, as an <see cref="T:System.Object" />.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
			object IEnumerator.Current
			{
				get
				{
					if (this._index == 0 || this._index == this._set._lastIndex + 1)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this.Current;
				}
			}

			/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			void IEnumerator.Reset()
			{
				if (this._version != this._set._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._index = 0;
				this._current = default(T);
			}

			private HashSet<T> _set;

			private int _index;

			private int _version;

			private T _current;
		}
	}
}
