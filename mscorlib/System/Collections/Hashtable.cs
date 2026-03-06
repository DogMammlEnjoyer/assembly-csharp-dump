using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace System.Collections
{
	/// <summary>Represents a collection of key/value pairs that are organized based on the hash code of the key.</summary>
	[DebuggerTypeProxy(typeof(Hashtable.HashtableDebugView))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class Hashtable : IDictionary, ICollection, IEnumerable, ISerializable, IDeserializationCallback, ICloneable
	{
		private static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
		{
			get
			{
				return LazyInitializer.EnsureInitialized<ConditionalWeakTable<object, SerializationInfo>>(ref Hashtable.s_serializationInfoTable);
			}
		}

		/// <summary>Gets or sets the object that can dispense hash codes.</summary>
		/// <returns>The object that can dispense hash codes.</returns>
		/// <exception cref="T:System.ArgumentException">The property is set to a value, but the hash table was created using an <see cref="T:System.Collections.IEqualityComparer" />.</exception>
		[Obsolete("Please use EqualityComparer property.")]
		protected IHashCodeProvider hcp
		{
			get
			{
				if (this._keycomparer is CompatibleComparer)
				{
					return ((CompatibleComparer)this._keycomparer).HashCodeProvider;
				}
				if (this._keycomparer == null)
				{
					return null;
				}
				throw new ArgumentException("The usage of IKeyComparer and IHashCodeProvider/IComparer interfaces cannot be mixed; use one or the other.");
			}
			set
			{
				if (this._keycomparer is CompatibleComparer)
				{
					CompatibleComparer compatibleComparer = (CompatibleComparer)this._keycomparer;
					this._keycomparer = new CompatibleComparer(value, compatibleComparer.Comparer);
					return;
				}
				if (this._keycomparer == null)
				{
					this._keycomparer = new CompatibleComparer(value, null);
					return;
				}
				throw new ArgumentException("The usage of IKeyComparer and IHashCodeProvider/IComparer interfaces cannot be mixed; use one or the other.");
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Collections.IComparer" /> to use for the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>The <see cref="T:System.Collections.IComparer" /> to use for the <see cref="T:System.Collections.Hashtable" />.</returns>
		/// <exception cref="T:System.ArgumentException">The property is set to a value, but the hash table was created using an <see cref="T:System.Collections.IEqualityComparer" />.</exception>
		[Obsolete("Please use KeyComparer properties.")]
		protected IComparer comparer
		{
			get
			{
				if (this._keycomparer is CompatibleComparer)
				{
					return ((CompatibleComparer)this._keycomparer).Comparer;
				}
				if (this._keycomparer == null)
				{
					return null;
				}
				throw new ArgumentException("The usage of IKeyComparer and IHashCodeProvider/IComparer interfaces cannot be mixed; use one or the other.");
			}
			set
			{
				if (this._keycomparer is CompatibleComparer)
				{
					CompatibleComparer compatibleComparer = (CompatibleComparer)this._keycomparer;
					this._keycomparer = new CompatibleComparer(compatibleComparer.HashCodeProvider, value);
					return;
				}
				if (this._keycomparer == null)
				{
					this._keycomparer = new CompatibleComparer(null, value);
					return;
				}
				throw new ArgumentException("The usage of IKeyComparer and IHashCodeProvider/IComparer interfaces cannot be mixed; use one or the other.");
			}
		}

		/// <summary>Gets the <see cref="T:System.Collections.IEqualityComparer" /> to use for the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>The <see cref="T:System.Collections.IEqualityComparer" /> to use for the <see cref="T:System.Collections.Hashtable" />.</returns>
		/// <exception cref="T:System.ArgumentException">The property is set to a value, but the hash table was created using an <see cref="T:System.Collections.IHashCodeProvider" /> and an <see cref="T:System.Collections.IComparer" />.</exception>
		protected IEqualityComparer EqualityComparer
		{
			get
			{
				return this._keycomparer;
			}
		}

		internal Hashtable(bool trash)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the default initial capacity, load factor, hash code provider, and comparer.</summary>
		public Hashtable() : this(0, 1f)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the specified initial capacity, and the default load factor, hash code provider, and comparer.</summary>
		/// <param name="capacity">The approximate number of elements that the <see cref="T:System.Collections.Hashtable" /> object can initially contain.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.</exception>
		public Hashtable(int capacity) : this(capacity, 1f)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the specified initial capacity and load factor, and the default hash code provider and comparer.</summary>
		/// <param name="capacity">The approximate number of elements that the <see cref="T:System.Collections.Hashtable" /> object can initially contain.</param>
		/// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.  
		/// -or-  
		/// <paramref name="loadFactor" /> is less than 0.1.  
		/// -or-  
		/// <paramref name="loadFactor" /> is greater than 1.0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="capacity" /> is causing an overflow.</exception>
		public Hashtable(int capacity, float loadFactor)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", "Non-negative number required.");
			}
			if (loadFactor < 0.1f || loadFactor > 1f)
			{
				throw new ArgumentOutOfRangeException("loadFactor", SR.Format("Load factor needs to be between 0.1 and 1.0.", 0.1, 1.0));
			}
			this._loadFactor = 0.72f * loadFactor;
			double num = (double)((float)capacity / this._loadFactor);
			if (num > 2147483647.0)
			{
				throw new ArgumentException("Hashtable's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.", "capacity");
			}
			int num2 = (num > 3.0) ? HashHelpers.GetPrime((int)num) : 3;
			this._buckets = new Hashtable.bucket[num2];
			this._loadsize = (int)(this._loadFactor * (float)num2);
			this._isWriterInProgress = false;
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the specified initial capacity, load factor, and <see cref="T:System.Collections.IEqualityComparer" /> object.</summary>
		/// <param name="capacity">The approximate number of elements that the <see cref="T:System.Collections.Hashtable" /> object can initially contain.</param>
		/// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
		/// <param name="equalityComparer">The <see cref="T:System.Collections.IEqualityComparer" /> object that defines the hash code provider and the comparer to use with the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider and the default comparer. The default hash code provider is each key's implementation of <see cref="M:System.Object.GetHashCode" /> and the default comparer is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.  
		/// -or-  
		/// <paramref name="loadFactor" /> is less than 0.1.  
		/// -or-  
		/// <paramref name="loadFactor" /> is greater than 1.0.</exception>
		public Hashtable(int capacity, float loadFactor, IEqualityComparer equalityComparer) : this(capacity, loadFactor)
		{
			this._keycomparer = equalityComparer;
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the default initial capacity and load factor, and the specified hash code provider and comparer.</summary>
		/// <param name="hcp">The <see cref="T:System.Collections.IHashCodeProvider" /> object that supplies the hash codes for all keys in the <see cref="T:System.Collections.Hashtable" /> object.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider, which is each key's implementation of <see cref="M:System.Object.GetHashCode" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> object to use to determine whether two keys are equal.  
		///  -or-  
		///  <see langword="null" /> to use the default comparer, which is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		[Obsolete("Please use Hashtable(IEqualityComparer) instead.")]
		public Hashtable(IHashCodeProvider hcp, IComparer comparer) : this(0, 1f, hcp, comparer)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the default initial capacity and load factor, and the specified <see cref="T:System.Collections.IEqualityComparer" /> object.</summary>
		/// <param name="equalityComparer">The <see cref="T:System.Collections.IEqualityComparer" /> object that defines the hash code provider and the comparer to use with the <see cref="T:System.Collections.Hashtable" /> object.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider and the default comparer. The default hash code provider is each key's implementation of <see cref="M:System.Object.GetHashCode" /> and the default comparer is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		public Hashtable(IEqualityComparer equalityComparer) : this(0, 1f, equalityComparer)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the specified initial capacity, hash code provider, comparer, and the default load factor.</summary>
		/// <param name="capacity">The approximate number of elements that the <see cref="T:System.Collections.Hashtable" /> object can initially contain.</param>
		/// <param name="hcp">The <see cref="T:System.Collections.IHashCodeProvider" /> object that supplies the hash codes for all keys in the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider, which is each key's implementation of <see cref="M:System.Object.GetHashCode" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> object to use to determine whether two keys are equal.  
		///  -or-  
		///  <see langword="null" /> to use the default comparer, which is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.</exception>
		[Obsolete("Please use Hashtable(int, IEqualityComparer) instead.")]
		public Hashtable(int capacity, IHashCodeProvider hcp, IComparer comparer) : this(capacity, 1f, hcp, comparer)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the specified initial capacity and <see cref="T:System.Collections.IEqualityComparer" />, and the default load factor.</summary>
		/// <param name="capacity">The approximate number of elements that the <see cref="T:System.Collections.Hashtable" /> object can initially contain.</param>
		/// <param name="equalityComparer">The <see cref="T:System.Collections.IEqualityComparer" /> object that defines the hash code provider and the comparer to use with the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider and the default comparer. The default hash code provider is each key's implementation of <see cref="M:System.Object.GetHashCode" /> and the default comparer is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.</exception>
		public Hashtable(int capacity, IEqualityComparer equalityComparer) : this(capacity, 1f, equalityComparer)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Hashtable" /> class by copying the elements from the specified dictionary to the new <see cref="T:System.Collections.Hashtable" /> object. The new <see cref="T:System.Collections.Hashtable" /> object has an initial capacity equal to the number of elements copied, and uses the default load factor, hash code provider, and comparer.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> object to copy to a new <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		public Hashtable(IDictionary d) : this(d, 1f)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Hashtable" /> class by copying the elements from the specified dictionary to the new <see cref="T:System.Collections.Hashtable" /> object. The new <see cref="T:System.Collections.Hashtable" /> object has an initial capacity equal to the number of elements copied, and uses the specified load factor, and the default hash code provider and comparer.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> object to copy to a new <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="loadFactor" /> is less than 0.1.  
		/// -or-  
		/// <paramref name="loadFactor" /> is greater than 1.0.</exception>
		public Hashtable(IDictionary d, float loadFactor) : this(d, loadFactor, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Hashtable" /> class by copying the elements from the specified dictionary to the new <see cref="T:System.Collections.Hashtable" /> object. The new <see cref="T:System.Collections.Hashtable" /> object has an initial capacity equal to the number of elements copied, and uses the default load factor, and the specified hash code provider and comparer. This API is obsolete. For an alternative, see <see cref="M:System.Collections.Hashtable.#ctor(System.Collections.IDictionary,System.Collections.IEqualityComparer)" />.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> object to copy to a new <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <param name="hcp">The <see cref="T:System.Collections.IHashCodeProvider" /> object that supplies the hash codes for all keys in the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider, which is each key's implementation of <see cref="M:System.Object.GetHashCode" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> object to use to determine whether two keys are equal.  
		///  -or-  
		///  <see langword="null" /> to use the default comparer, which is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		[Obsolete("Please use Hashtable(IDictionary, IEqualityComparer) instead.")]
		public Hashtable(IDictionary d, IHashCodeProvider hcp, IComparer comparer) : this(d, 1f, hcp, comparer)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Hashtable" /> class by copying the elements from the specified dictionary to a new <see cref="T:System.Collections.Hashtable" /> object. The new <see cref="T:System.Collections.Hashtable" /> object has an initial capacity equal to the number of elements copied, and uses the default load factor and the specified <see cref="T:System.Collections.IEqualityComparer" /> object.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> object to copy to a new <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <param name="equalityComparer">The <see cref="T:System.Collections.IEqualityComparer" /> object that defines the hash code provider and the comparer to use with the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider and the default comparer. The default hash code provider is each key's implementation of <see cref="M:System.Object.GetHashCode" /> and the default comparer is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		public Hashtable(IDictionary d, IEqualityComparer equalityComparer) : this(d, 1f, equalityComparer)
		{
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class using the specified initial capacity, load factor, hash code provider, and comparer.</summary>
		/// <param name="capacity">The approximate number of elements that the <see cref="T:System.Collections.Hashtable" /> object can initially contain.</param>
		/// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
		/// <param name="hcp">The <see cref="T:System.Collections.IHashCodeProvider" /> object that supplies the hash codes for all keys in the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider, which is each key's implementation of <see cref="M:System.Object.GetHashCode" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> object to use to determine whether two keys are equal.  
		///  -or-  
		///  <see langword="null" /> to use the default comparer, which is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.  
		/// -or-  
		/// <paramref name="loadFactor" /> is less than 0.1.  
		/// -or-  
		/// <paramref name="loadFactor" /> is greater than 1.0.</exception>
		[Obsolete("Please use Hashtable(int, float, IEqualityComparer) instead.")]
		public Hashtable(int capacity, float loadFactor, IHashCodeProvider hcp, IComparer comparer) : this(capacity, loadFactor)
		{
			if (hcp != null || comparer != null)
			{
				this._keycomparer = new CompatibleComparer(hcp, comparer);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Hashtable" /> class by copying the elements from the specified dictionary to the new <see cref="T:System.Collections.Hashtable" /> object. The new <see cref="T:System.Collections.Hashtable" /> object has an initial capacity equal to the number of elements copied, and uses the specified load factor, hash code provider, and comparer.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> object to copy to a new <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
		/// <param name="hcp">The <see cref="T:System.Collections.IHashCodeProvider" /> object that supplies the hash codes for all keys in the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider, which is each key's implementation of <see cref="M:System.Object.GetHashCode" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> object to use to determine whether two keys are equal.  
		///  -or-  
		///  <see langword="null" /> to use the default comparer, which is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="loadFactor" /> is less than 0.1.  
		/// -or-  
		/// <paramref name="loadFactor" /> is greater than 1.0.</exception>
		[Obsolete("Please use Hashtable(IDictionary, float, IEqualityComparer) instead.")]
		public Hashtable(IDictionary d, float loadFactor, IHashCodeProvider hcp, IComparer comparer) : this((d != null) ? d.Count : 0, loadFactor, hcp, comparer)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d", "Dictionary cannot be null.");
			}
			IDictionaryEnumerator enumerator = d.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.Add(enumerator.Key, enumerator.Value);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Hashtable" /> class by copying the elements from the specified dictionary to the new <see cref="T:System.Collections.Hashtable" /> object. The new <see cref="T:System.Collections.Hashtable" /> object has an initial capacity equal to the number of elements copied, and uses the specified load factor and <see cref="T:System.Collections.IEqualityComparer" /> object.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> object to copy to a new <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <param name="loadFactor">A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides the best performance. The result is the maximum ratio of elements to buckets.</param>
		/// <param name="equalityComparer">The <see cref="T:System.Collections.IEqualityComparer" /> object that defines the hash code provider and the comparer to use with the <see cref="T:System.Collections.Hashtable" />.  
		///  -or-  
		///  <see langword="null" /> to use the default hash code provider and the default comparer. The default hash code provider is each key's implementation of <see cref="M:System.Object.GetHashCode" /> and the default comparer is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="loadFactor" /> is less than 0.1.  
		/// -or-  
		/// <paramref name="loadFactor" /> is greater than 1.0.</exception>
		public Hashtable(IDictionary d, float loadFactor, IEqualityComparer equalityComparer) : this((d != null) ? d.Count : 0, loadFactor, equalityComparer)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d", "Dictionary cannot be null.");
			}
			IDictionaryEnumerator enumerator = d.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.Add(enumerator.Key, enumerator.Value);
			}
		}

		/// <summary>Initializes a new, empty instance of the <see cref="T:System.Collections.Hashtable" /> class that is serializable using the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> objects.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Hashtable" /> object.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Hashtable" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		protected Hashtable(SerializationInfo info, StreamingContext context)
		{
			Hashtable.SerializationInfoTable.Add(this, info);
		}

		private uint InitHash(object key, int hashsize, out uint seed, out uint incr)
		{
			uint num = (uint)(this.GetHash(key) & int.MaxValue);
			seed = num;
			incr = 1U + seed * 101U % (uint)(hashsize - 1);
			return num;
		}

		/// <summary>Adds an element with the specified key and value into the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be <see langword="null" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Hashtable" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Hashtable" /> is read-only.  
		///  -or-  
		///  The <see cref="T:System.Collections.Hashtable" /> has a fixed size.</exception>
		public virtual void Add(object key, object value)
		{
			this.Insert(key, value, true);
		}

		/// <summary>Removes all elements from the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Hashtable" /> is read-only.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public virtual void Clear()
		{
			if (this._count == 0 && this._occupancy == 0)
			{
				return;
			}
			this._isWriterInProgress = true;
			for (int i = 0; i < this._buckets.Length; i++)
			{
				this._buckets[i].hash_coll = 0;
				this._buckets[i].key = null;
				this._buckets[i].val = null;
			}
			this._count = 0;
			this._occupancy = 0;
			this.UpdateVersion();
			this._isWriterInProgress = false;
		}

		/// <summary>Creates a shallow copy of the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>A shallow copy of the <see cref="T:System.Collections.Hashtable" />.</returns>
		public virtual object Clone()
		{
			Hashtable.bucket[] buckets = this._buckets;
			Hashtable hashtable = new Hashtable(this._count, this._keycomparer);
			hashtable._version = this._version;
			hashtable._loadFactor = this._loadFactor;
			hashtable._count = 0;
			int i = buckets.Length;
			while (i > 0)
			{
				i--;
				object key = buckets[i].key;
				if (key != null && key != buckets)
				{
					hashtable[key] = buckets[i].val;
				}
			}
			return hashtable;
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.Hashtable" /> contains a specific key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Hashtable" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Hashtable" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public virtual bool Contains(object key)
		{
			return this.ContainsKey(key);
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.Hashtable" /> contains a specific key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Hashtable" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Hashtable" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public virtual bool ContainsKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key cannot be null.");
			}
			Hashtable.bucket[] buckets = this._buckets;
			uint num2;
			uint num3;
			uint num = this.InitHash(key, buckets.Length, out num2, out num3);
			int num4 = 0;
			int num5 = (int)(num2 % (uint)buckets.Length);
			for (;;)
			{
				Hashtable.bucket bucket = buckets[num5];
				if (bucket.key == null)
				{
					break;
				}
				if ((long)(bucket.hash_coll & 2147483647) == (long)((ulong)num) && this.KeyEquals(bucket.key, key))
				{
					return true;
				}
				num5 = (int)(((long)num5 + (long)((ulong)num3)) % (long)((ulong)buckets.Length));
				if (bucket.hash_coll >= 0 || ++num4 >= buckets.Length)
				{
					return false;
				}
			}
			return false;
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.Hashtable" /> contains a specific value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Hashtable" />. The value can be <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Hashtable" /> contains an element with the specified <paramref name="value" />; otherwise, <see langword="false" />.</returns>
		public virtual bool ContainsValue(object value)
		{
			if (value == null)
			{
				int num = this._buckets.Length;
				while (--num >= 0)
				{
					if (this._buckets[num].key != null && this._buckets[num].key != this._buckets && this._buckets[num].val == null)
					{
						return true;
					}
				}
			}
			else
			{
				int num2 = this._buckets.Length;
				while (--num2 >= 0)
				{
					object val = this._buckets[num2].val;
					if (val != null && val.Equals(value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void CopyKeys(Array array, int arrayIndex)
		{
			Hashtable.bucket[] buckets = this._buckets;
			int num = buckets.Length;
			while (--num >= 0)
			{
				object key = buckets[num].key;
				if (key != null && key != this._buckets)
				{
					array.SetValue(key, arrayIndex++);
				}
			}
		}

		private void CopyEntries(Array array, int arrayIndex)
		{
			Hashtable.bucket[] buckets = this._buckets;
			int num = buckets.Length;
			while (--num >= 0)
			{
				object key = buckets[num].key;
				if (key != null && key != this._buckets)
				{
					DictionaryEntry dictionaryEntry = new DictionaryEntry(key, buckets[num].val);
					array.SetValue(dictionaryEntry, arrayIndex++);
				}
			}
		}

		/// <summary>Copies the <see cref="T:System.Collections.Hashtable" /> elements to a one-dimensional <see cref="T:System.Array" /> instance at the specified index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the <see cref="T:System.Collections.DictionaryEntry" /> objects copied from <see cref="T:System.Collections.Hashtable" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="arrayIndex" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// The number of elements in the source <see cref="T:System.Collections.Hashtable" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.Hashtable" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public virtual void CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Array cannot be null.");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number required.");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			this.CopyEntries(array, arrayIndex);
		}

		internal virtual KeyValuePairs[] ToKeyValuePairsArray()
		{
			KeyValuePairs[] array = new KeyValuePairs[this._count];
			int num = 0;
			Hashtable.bucket[] buckets = this._buckets;
			int num2 = buckets.Length;
			while (--num2 >= 0)
			{
				object key = buckets[num2].key;
				if (key != null && key != this._buckets)
				{
					array[num++] = new KeyValuePairs(key, buckets[num2].val);
				}
			}
			return array;
		}

		private void CopyValues(Array array, int arrayIndex)
		{
			Hashtable.bucket[] buckets = this._buckets;
			int num = buckets.Length;
			while (--num >= 0)
			{
				object key = buckets[num].key;
				if (key != null && key != this._buckets)
				{
					array.SetValue(buckets[num].val, arrayIndex++);
				}
			}
		}

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <param name="key">The key whose value to get or set.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, attempting to get it returns <see langword="null" />, and attempting to set it creates a new element using the specified key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Hashtable" /> is read-only.  
		///  -or-  
		///  The property is set, <paramref name="key" /> does not exist in the collection, and the <see cref="T:System.Collections.Hashtable" /> has a fixed size.</exception>
		public virtual object this[object key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key", "Key cannot be null.");
				}
				Hashtable.bucket[] buckets = this._buckets;
				uint num2;
				uint num3;
				uint num = this.InitHash(key, buckets.Length, out num2, out num3);
				int num4 = 0;
				int num5 = (int)(num2 % (uint)buckets.Length);
				Hashtable.bucket bucket;
				for (;;)
				{
					SpinWait spinWait = default(SpinWait);
					for (;;)
					{
						int version = this._version;
						bucket = buckets[num5];
						if (!this._isWriterInProgress && version == this._version)
						{
							break;
						}
						spinWait.SpinOnce();
					}
					if (bucket.key == null)
					{
						break;
					}
					if ((long)(bucket.hash_coll & 2147483647) == (long)((ulong)num) && this.KeyEquals(bucket.key, key))
					{
						goto Block_5;
					}
					num5 = (int)(((long)num5 + (long)((ulong)num3)) % (long)((ulong)buckets.Length));
					if (bucket.hash_coll >= 0 || ++num4 >= buckets.Length)
					{
						goto IL_CA;
					}
				}
				return null;
				Block_5:
				return bucket.val;
				IL_CA:
				return null;
			}
			set
			{
				this.Insert(key, value, false);
			}
		}

		private void expand()
		{
			int newsize = HashHelpers.ExpandPrime(this._buckets.Length);
			this.rehash(newsize);
		}

		private void rehash()
		{
			this.rehash(this._buckets.Length);
		}

		private void UpdateVersion()
		{
			this._version++;
		}

		private void rehash(int newsize)
		{
			this._occupancy = 0;
			Hashtable.bucket[] array = new Hashtable.bucket[newsize];
			for (int i = 0; i < this._buckets.Length; i++)
			{
				Hashtable.bucket bucket = this._buckets[i];
				if (bucket.key != null && bucket.key != this._buckets)
				{
					int hashcode = bucket.hash_coll & int.MaxValue;
					this.putEntry(array, bucket.key, bucket.val, hashcode);
				}
			}
			this._isWriterInProgress = true;
			this._buckets = array;
			this._loadsize = (int)(this._loadFactor * (float)newsize);
			this.UpdateVersion();
			this._isWriterInProgress = false;
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Hashtable.HashtableEnumerator(this, 3);
		}

		/// <summary>Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> that iterates through the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator" /> for the <see cref="T:System.Collections.Hashtable" />.</returns>
		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new Hashtable.HashtableEnumerator(this, 3);
		}

		/// <summary>Returns the hash code for the specified key.</summary>
		/// <param name="key">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
		/// <returns>The hash code for <paramref name="key" />.</returns>
		/// <exception cref="T:System.NullReferenceException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		protected virtual int GetHash(object key)
		{
			if (this._keycomparer != null)
			{
				return this._keycomparer.GetHashCode(key);
			}
			return key.GetHashCode();
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Hashtable" /> is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Hashtable" /> is read-only; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Hashtable" /> has a fixed size.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Hashtable" /> has a fixed size; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.Hashtable" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.Hashtable" /> is synchronized (thread safe); otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Compares a specific <see cref="T:System.Object" /> with a specific key in the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <param name="item">The <see cref="T:System.Object" /> to compare with <paramref name="key" />.</param>
		/// <param name="key">The key in the <see cref="T:System.Collections.Hashtable" /> to compare with <paramref name="item" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="item" /> and <paramref name="key" /> are equal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="item" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="key" /> is <see langword="null" />.</exception>
		protected virtual bool KeyEquals(object item, object key)
		{
			if (this._buckets == item)
			{
				return false;
			}
			if (item == key)
			{
				return true;
			}
			if (this._keycomparer != null)
			{
				return this._keycomparer.Equals(item, key);
			}
			return item != null && item.Equals(key);
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> containing the keys in the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> containing the keys in the <see cref="T:System.Collections.Hashtable" />.</returns>
		public virtual ICollection Keys
		{
			get
			{
				if (this._keys == null)
				{
					this._keys = new Hashtable.KeyCollection(this);
				}
				return this._keys;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> containing the values in the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> containing the values in the <see cref="T:System.Collections.Hashtable" />.</returns>
		public virtual ICollection Values
		{
			get
			{
				if (this._values == null)
				{
					this._values = new Hashtable.ValueCollection(this);
				}
				return this._values;
			}
		}

		private void Insert(object key, object nvalue, bool add)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key cannot be null.");
			}
			if (this._count >= this._loadsize)
			{
				this.expand();
			}
			else if (this._occupancy > this._loadsize && this._count > 100)
			{
				this.rehash();
			}
			uint num2;
			uint num3;
			uint num = this.InitHash(key, this._buckets.Length, out num2, out num3);
			int num4 = 0;
			int num5 = -1;
			int num6 = (int)(num2 % (uint)this._buckets.Length);
			for (;;)
			{
				if (num5 == -1 && this._buckets[num6].key == this._buckets && this._buckets[num6].hash_coll < 0)
				{
					num5 = num6;
				}
				if (this._buckets[num6].key == null || (this._buckets[num6].key == this._buckets && ((long)this._buckets[num6].hash_coll & (long)((ulong)-2147483648)) == 0L))
				{
					break;
				}
				if ((long)(this._buckets[num6].hash_coll & 2147483647) == (long)((ulong)num) && this.KeyEquals(this._buckets[num6].key, key))
				{
					goto Block_12;
				}
				if (num5 == -1 && this._buckets[num6].hash_coll >= 0)
				{
					Hashtable.bucket[] buckets = this._buckets;
					int num7 = num6;
					buckets[num7].hash_coll = (buckets[num7].hash_coll | int.MinValue);
					this._occupancy++;
				}
				num6 = (int)(((long)num6 + (long)((ulong)num3)) % (long)((ulong)this._buckets.Length));
				if (++num4 >= this._buckets.Length)
				{
					goto Block_16;
				}
			}
			if (num5 != -1)
			{
				num6 = num5;
			}
			this._isWriterInProgress = true;
			this._buckets[num6].val = nvalue;
			this._buckets[num6].key = key;
			Hashtable.bucket[] buckets2 = this._buckets;
			int num8 = num6;
			buckets2[num8].hash_coll = (buckets2[num8].hash_coll | (int)num);
			this._count++;
			this.UpdateVersion();
			this._isWriterInProgress = false;
			return;
			Block_12:
			if (add)
			{
				throw new ArgumentException(SR.Format("Item has already been added. Key in dictionary: '{0}'  Key being added: '{1}'", this._buckets[num6].key, key));
			}
			this._isWriterInProgress = true;
			this._buckets[num6].val = nvalue;
			this.UpdateVersion();
			this._isWriterInProgress = false;
			return;
			Block_16:
			if (num5 != -1)
			{
				this._isWriterInProgress = true;
				this._buckets[num5].val = nvalue;
				this._buckets[num5].key = key;
				Hashtable.bucket[] buckets3 = this._buckets;
				int num9 = num5;
				buckets3[num9].hash_coll = (buckets3[num9].hash_coll | (int)num);
				this._count++;
				this.UpdateVersion();
				this._isWriterInProgress = false;
				return;
			}
			throw new InvalidOperationException("Hashtable insert failed. Load factor too high. The most common cause is multiple threads writing to the Hashtable simultaneously.");
		}

		private void putEntry(Hashtable.bucket[] newBuckets, object key, object nvalue, int hashcode)
		{
			uint num = (uint)(1 + hashcode * 101 % (newBuckets.Length - 1));
			int num2 = hashcode % newBuckets.Length;
			while (newBuckets[num2].key != null && newBuckets[num2].key != this._buckets)
			{
				if (newBuckets[num2].hash_coll >= 0)
				{
					int num3 = num2;
					newBuckets[num3].hash_coll = (newBuckets[num3].hash_coll | int.MinValue);
					this._occupancy++;
				}
				num2 = (int)(((long)num2 + (long)((ulong)num)) % (long)((ulong)newBuckets.Length));
			}
			newBuckets[num2].val = nvalue;
			newBuckets[num2].key = key;
			int num4 = num2;
			newBuckets[num4].hash_coll = (newBuckets[num4].hash_coll | hashcode);
		}

		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Hashtable" /> is read-only.  
		///  -or-  
		///  The <see cref="T:System.Collections.Hashtable" /> has a fixed size.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public virtual void Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key cannot be null.");
			}
			uint num2;
			uint num3;
			uint num = this.InitHash(key, this._buckets.Length, out num2, out num3);
			int num4 = 0;
			int num5 = (int)(num2 % (uint)this._buckets.Length);
			for (;;)
			{
				Hashtable.bucket bucket = this._buckets[num5];
				if ((long)(bucket.hash_coll & 2147483647) == (long)((ulong)num) && this.KeyEquals(bucket.key, key))
				{
					break;
				}
				num5 = (int)(((long)num5 + (long)((ulong)num3)) % (long)((ulong)this._buckets.Length));
				if (bucket.hash_coll >= 0 || ++num4 >= this._buckets.Length)
				{
					return;
				}
			}
			this._isWriterInProgress = true;
			Hashtable.bucket[] buckets = this._buckets;
			int num6 = num5;
			buckets[num6].hash_coll = (buckets[num6].hash_coll & int.MinValue);
			if (this._buckets[num5].hash_coll != 0)
			{
				this._buckets[num5].key = this._buckets;
			}
			else
			{
				this._buckets[num5].key = null;
			}
			this._buckets[num5].val = null;
			this._count--;
			this.UpdateVersion();
			this._isWriterInProgress = false;
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.Hashtable" />.</returns>
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

		/// <summary>Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Hashtable" />.</returns>
		public virtual int Count
		{
			get
			{
				return this._count;
			}
		}

		/// <summary>Returns a synchronized (thread-safe) wrapper for the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <param name="table">The <see cref="T:System.Collections.Hashtable" /> to synchronize.</param>
		/// <returns>A synchronized (thread-safe) wrapper for the <see cref="T:System.Collections.Hashtable" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="table" /> is <see langword="null" />.</exception>
		public static Hashtable Synchronized(Hashtable table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			return new Hashtable.SyncHashtable(table);
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:System.Collections.Hashtable" />.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Hashtable" />.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Hashtable" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified.</exception>
		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			object syncRoot = this.SyncRoot;
			lock (syncRoot)
			{
				int version = this._version;
				info.AddValue("LoadFactor", this._loadFactor);
				info.AddValue("Version", this._version);
				IEqualityComparer keycomparer = this._keycomparer;
				if (keycomparer == null)
				{
					info.AddValue("Comparer", null, typeof(IComparer));
					info.AddValue("HashCodeProvider", null, typeof(IHashCodeProvider));
				}
				else if (keycomparer is CompatibleComparer)
				{
					CompatibleComparer compatibleComparer = keycomparer as CompatibleComparer;
					info.AddValue("Comparer", compatibleComparer.Comparer, typeof(IComparer));
					info.AddValue("HashCodeProvider", compatibleComparer.HashCodeProvider, typeof(IHashCodeProvider));
				}
				else
				{
					info.AddValue("KeyComparer", keycomparer, typeof(IEqualityComparer));
				}
				info.AddValue("HashSize", this._buckets.Length);
				object[] array = new object[this._count];
				object[] array2 = new object[this._count];
				this.CopyKeys(array, 0);
				this.CopyValues(array2, 0);
				info.AddValue("Keys", array, typeof(object[]));
				info.AddValue("Values", array2, typeof(object[]));
				if (this._version != version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
			}
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
		/// <param name="sender">The source of the deserialization event.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Hashtable" /> is invalid.</exception>
		public virtual void OnDeserialization(object sender)
		{
			if (this._buckets != null)
			{
				return;
			}
			SerializationInfo serializationInfo;
			Hashtable.SerializationInfoTable.TryGetValue(this, out serializationInfo);
			if (serializationInfo == null)
			{
				throw new SerializationException("OnDeserialization method was called while the object was not being deserialized.");
			}
			int num = 0;
			IComparer comparer = null;
			IHashCodeProvider hashCodeProvider = null;
			object[] array = null;
			object[] array2 = null;
			SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string name = enumerator.Name;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num2 <= 1613443821U)
				{
					if (num2 != 891156946U)
					{
						if (num2 != 1228509323U)
						{
							if (num2 == 1613443821U)
							{
								if (name == "Keys")
								{
									array = (object[])serializationInfo.GetValue("Keys", typeof(object[]));
								}
							}
						}
						else if (name == "KeyComparer")
						{
							this._keycomparer = (IEqualityComparer)serializationInfo.GetValue("KeyComparer", typeof(IEqualityComparer));
						}
					}
					else if (name == "Comparer")
					{
						comparer = (IComparer)serializationInfo.GetValue("Comparer", typeof(IComparer));
					}
				}
				else if (num2 <= 2484309429U)
				{
					if (num2 != 2370642523U)
					{
						if (num2 == 2484309429U)
						{
							if (name == "HashCodeProvider")
							{
								hashCodeProvider = (IHashCodeProvider)serializationInfo.GetValue("HashCodeProvider", typeof(IHashCodeProvider));
							}
						}
					}
					else if (name == "Values")
					{
						array2 = (object[])serializationInfo.GetValue("Values", typeof(object[]));
					}
				}
				else if (num2 != 3356145248U)
				{
					if (num2 == 3483216242U)
					{
						if (name == "LoadFactor")
						{
							this._loadFactor = serializationInfo.GetSingle("LoadFactor");
						}
					}
				}
				else if (name == "HashSize")
				{
					num = serializationInfo.GetInt32("HashSize");
				}
			}
			this._loadsize = (int)(this._loadFactor * (float)num);
			if (this._keycomparer == null && (comparer != null || hashCodeProvider != null))
			{
				this._keycomparer = new CompatibleComparer(hashCodeProvider, comparer);
			}
			this._buckets = new Hashtable.bucket[num];
			if (array == null)
			{
				throw new SerializationException("The keys for this dictionary are missing.");
			}
			if (array2 == null)
			{
				throw new SerializationException("The values for this dictionary are missing.");
			}
			if (array.Length != array2.Length)
			{
				throw new SerializationException("The keys and values arrays have different sizes.");
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					throw new SerializationException("One of the serialized keys is null.");
				}
				this.Insert(array[i], array2[i], true);
			}
			this._version = serializationInfo.GetInt32("Version");
			Hashtable.SerializationInfoTable.Remove(this);
		}

		internal const int HashPrime = 101;

		private const int InitialSize = 3;

		private const string LoadFactorName = "LoadFactor";

		private const string VersionName = "Version";

		private const string ComparerName = "Comparer";

		private const string HashCodeProviderName = "HashCodeProvider";

		private const string HashSizeName = "HashSize";

		private const string KeysName = "Keys";

		private const string ValuesName = "Values";

		private const string KeyComparerName = "KeyComparer";

		private Hashtable.bucket[] _buckets;

		private int _count;

		private int _occupancy;

		private int _loadsize;

		private float _loadFactor;

		private volatile int _version;

		private volatile bool _isWriterInProgress;

		private ICollection _keys;

		private ICollection _values;

		private IEqualityComparer _keycomparer;

		private object _syncRoot;

		private static ConditionalWeakTable<object, SerializationInfo> s_serializationInfoTable;

		private struct bucket
		{
			public object key;

			public object val;

			public int hash_coll;
		}

		[Serializable]
		private class KeyCollection : ICollection, IEnumerable
		{
			internal KeyCollection(Hashtable hashtable)
			{
				this._hashtable = hashtable;
			}

			public virtual void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number required.");
				}
				if (array.Length - arrayIndex < this._hashtable._count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				this._hashtable.CopyKeys(array, arrayIndex);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return new Hashtable.HashtableEnumerator(this._hashtable, 1);
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return this._hashtable.IsSynchronized;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return this._hashtable.SyncRoot;
				}
			}

			public virtual int Count
			{
				get
				{
					return this._hashtable._count;
				}
			}

			private Hashtable _hashtable;
		}

		[Serializable]
		private class ValueCollection : ICollection, IEnumerable
		{
			internal ValueCollection(Hashtable hashtable)
			{
				this._hashtable = hashtable;
			}

			public virtual void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number required.");
				}
				if (array.Length - arrayIndex < this._hashtable._count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				this._hashtable.CopyValues(array, arrayIndex);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return new Hashtable.HashtableEnumerator(this._hashtable, 2);
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return this._hashtable.IsSynchronized;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return this._hashtable.SyncRoot;
				}
			}

			public virtual int Count
			{
				get
				{
					return this._hashtable._count;
				}
			}

			private Hashtable _hashtable;
		}

		[Serializable]
		private class SyncHashtable : Hashtable, IEnumerable
		{
			internal SyncHashtable(Hashtable table) : base(false)
			{
				this._table = table;
			}

			internal SyncHashtable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				throw new PlatformNotSupportedException();
			}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new PlatformNotSupportedException();
			}

			public override int Count
			{
				get
				{
					return this._table.Count;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this._table.IsReadOnly;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return this._table.IsFixedSize;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object this[object key]
			{
				get
				{
					return this._table[key];
				}
				set
				{
					object syncRoot = this._table.SyncRoot;
					lock (syncRoot)
					{
						this._table[key] = value;
					}
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this._table.SyncRoot;
				}
			}

			public override void Add(object key, object value)
			{
				object syncRoot = this._table.SyncRoot;
				lock (syncRoot)
				{
					this._table.Add(key, value);
				}
			}

			public override void Clear()
			{
				object syncRoot = this._table.SyncRoot;
				lock (syncRoot)
				{
					this._table.Clear();
				}
			}

			public override bool Contains(object key)
			{
				return this._table.Contains(key);
			}

			public override bool ContainsKey(object key)
			{
				if (key == null)
				{
					throw new ArgumentNullException("key", "Key cannot be null.");
				}
				return this._table.ContainsKey(key);
			}

			public override bool ContainsValue(object key)
			{
				object syncRoot = this._table.SyncRoot;
				bool result;
				lock (syncRoot)
				{
					result = this._table.ContainsValue(key);
				}
				return result;
			}

			public override void CopyTo(Array array, int arrayIndex)
			{
				object syncRoot = this._table.SyncRoot;
				lock (syncRoot)
				{
					this._table.CopyTo(array, arrayIndex);
				}
			}

			public override object Clone()
			{
				object syncRoot = this._table.SyncRoot;
				object result;
				lock (syncRoot)
				{
					result = Hashtable.Synchronized((Hashtable)this._table.Clone());
				}
				return result;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._table.GetEnumerator();
			}

			public override IDictionaryEnumerator GetEnumerator()
			{
				return this._table.GetEnumerator();
			}

			public override ICollection Keys
			{
				get
				{
					object syncRoot = this._table.SyncRoot;
					ICollection keys;
					lock (syncRoot)
					{
						keys = this._table.Keys;
					}
					return keys;
				}
			}

			public override ICollection Values
			{
				get
				{
					object syncRoot = this._table.SyncRoot;
					ICollection values;
					lock (syncRoot)
					{
						values = this._table.Values;
					}
					return values;
				}
			}

			public override void Remove(object key)
			{
				object syncRoot = this._table.SyncRoot;
				lock (syncRoot)
				{
					this._table.Remove(key);
				}
			}

			public override void OnDeserialization(object sender)
			{
			}

			internal override KeyValuePairs[] ToKeyValuePairsArray()
			{
				return this._table.ToKeyValuePairsArray();
			}

			protected Hashtable _table;
		}

		[Serializable]
		private class HashtableEnumerator : IDictionaryEnumerator, IEnumerator, ICloneable
		{
			internal HashtableEnumerator(Hashtable hashtable, int getObjRetType)
			{
				this._hashtable = hashtable;
				this._bucket = hashtable._buckets.Length;
				this._version = hashtable._version;
				this._current = false;
				this._getObjectRetType = getObjRetType;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			public virtual object Key
			{
				get
				{
					if (!this._current)
					{
						throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
					}
					return this._currentKey;
				}
			}

			public virtual bool MoveNext()
			{
				if (this._version != this._hashtable._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				while (this._bucket > 0)
				{
					this._bucket--;
					object key = this._hashtable._buckets[this._bucket].key;
					if (key != null && key != this._hashtable._buckets)
					{
						this._currentKey = key;
						this._currentValue = this._hashtable._buckets[this._bucket].val;
						this._current = true;
						return true;
					}
				}
				this._current = false;
				return false;
			}

			public virtual DictionaryEntry Entry
			{
				get
				{
					if (!this._current)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return new DictionaryEntry(this._currentKey, this._currentValue);
				}
			}

			public virtual object Current
			{
				get
				{
					if (!this._current)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					if (this._getObjectRetType == 1)
					{
						return this._currentKey;
					}
					if (this._getObjectRetType == 2)
					{
						return this._currentValue;
					}
					return new DictionaryEntry(this._currentKey, this._currentValue);
				}
			}

			public virtual object Value
			{
				get
				{
					if (!this._current)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._currentValue;
				}
			}

			public virtual void Reset()
			{
				if (this._version != this._hashtable._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._current = false;
				this._bucket = this._hashtable._buckets.Length;
				this._currentKey = null;
				this._currentValue = null;
			}

			private Hashtable _hashtable;

			private int _bucket;

			private int _version;

			private bool _current;

			private int _getObjectRetType;

			private object _currentKey;

			private object _currentValue;

			internal const int Keys = 1;

			internal const int Values = 2;

			internal const int DictEntry = 3;
		}

		internal class HashtableDebugView
		{
			public HashtableDebugView(Hashtable hashtable)
			{
				if (hashtable == null)
				{
					throw new ArgumentNullException("hashtable");
				}
				this._hashtable = hashtable;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePairs[] Items
			{
				get
				{
					return this._hashtable.ToKeyValuePairsArray();
				}
			}

			private Hashtable _hashtable;
		}
	}
}
