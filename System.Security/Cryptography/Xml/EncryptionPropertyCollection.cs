using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents a collection of <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> classes used in XML encryption. This class cannot be inherited.</summary>
	public sealed class EncryptionPropertyCollection : IList, ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> class.</summary>
		public EncryptionPropertyCollection()
		{
			this._props = new ArrayList();
		}

		/// <summary>Returns an enumerator that iterates through an <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through an <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
		public IEnumerator GetEnumerator()
		{
			return this._props.GetEnumerator();
		}

		/// <summary>Gets the number of elements contained in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		/// <returns>The number of elements contained in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
		public int Count
		{
			get
			{
				return this._props.Count;
			}
		}

		/// <summary>Adds an item to the <see cref="T:System.Collections.IList" />.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to add to the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> uses an incorrect object type.</exception>
		int IList.Add(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._props.Add(value);
		}

		/// <summary>Adds an <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		/// <param name="value">An <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to add to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
		/// <returns>The position at which the new element is inserted.</returns>
		public int Add(EncryptionProperty value)
		{
			return this._props.Add(value);
		}

		/// <summary>Removes all items from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		public void Clear()
		{
			this._props.Clear();
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> uses an incorrect object type.</exception>
		bool IList.Contains(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._props.Contains(value);
		}

		/// <summary>Determines whether the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object contains a specific <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</summary>
		/// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to locate in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object is found in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object; otherwise, <see langword="false" />.</returns>
		public bool Contains(EncryptionProperty value)
		{
			return this._props.Contains(value);
		}

		/// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to locate in the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> uses an incorrect object type.</exception>
		int IList.IndexOf(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._props.IndexOf(value);
		}

		/// <summary>Determines the index of a specific item in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		/// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to locate in the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
		/// <returns>The index of <paramref name="value" /> if found in the collection; otherwise, -1.</returns>
		public int IndexOf(EncryptionProperty value)
		{
			return this._props.IndexOf(value);
		}

		/// <summary>Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to insert into the <see cref="T:System.Collections.IList" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> uses an incorrect object type.</exception>
		void IList.Insert(int index, object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			this._props.Insert(index, value);
		}

		/// <summary>Inserts an <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object into the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object at the specified position.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
		/// <param name="value">An <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to insert into the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
		public void Insert(int index, EncryptionProperty value)
		{
			this._props.Insert(index, value);
		}

		/// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to remove from the <see cref="T:System.Collections.IList" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="value" /> uses an incorrect object type.</exception>
		void IList.Remove(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			this._props.Remove(value);
		}

		/// <summary>Removes the first occurrence of a specific <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		/// <param name="value">The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to remove from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</param>
		public void Remove(EncryptionProperty value)
		{
			this._props.Remove(value);
		}

		/// <summary>Removes the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to remove.</param>
		public void RemoveAt(int index)
		{
			this._props.RemoveAt(index);
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object has a fixed size.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object has a fixed size; otherwise, <see langword="false" />.</returns>
		public bool IsFixedSize
		{
			get
			{
				return this._props.IsFixedSize;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is read-only; otherwise, <see langword="false" />.</returns>
		public bool IsReadOnly
		{
			get
			{
				return this._props.IsReadOnly;
			}
		}

		/// <summary>Returns the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</summary>
		/// <param name="index">The index of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to return.</param>
		/// <returns>The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</returns>
		public EncryptionProperty Item(int index)
		{
			return (EncryptionProperty)this._props[index];
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</summary>
		/// <param name="index">The index of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to return.</param>
		/// <returns>The <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object at the specified index.</returns>
		[IndexerName("ItemOf")]
		public EncryptionProperty this[int index]
		{
			get
			{
				return (EncryptionProperty)((IList)this)[index];
			}
			set
			{
				((IList)this)[index] = value;
			}
		}

		/// <summary>Gets the element at the specified index.</summary>
		/// <param name="index">The <see cref="T:System.Object" /> to remove from the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>The element at the specified index.</returns>
		object IList.this[int index]
		{
			get
			{
				return this._props[index];
			}
			set
			{
				if (!(value is EncryptionProperty))
				{
					throw new ArgumentException("Type of input object is invalid.", "value");
				}
				this._props[index] = value;
			}
		}

		/// <summary>Copies the elements of the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object to an array, starting at a particular array index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> object that is the destination of the elements copied from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		public void CopyTo(Array array, int index)
		{
			this._props.CopyTo(array, index);
		}

		/// <summary>Copies the elements of the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object to an array of <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> objects, starting at a particular array index.</summary>
		/// <param name="array">The one-dimensional array of  <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> objects that is the destination of the elements copied from the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		public void CopyTo(EncryptionProperty[] array, int index)
		{
			this._props.CopyTo(array, index);
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
		public object SyncRoot
		{
			get
			{
				return this._props.SyncRoot;
			}
		}

		/// <summary>Gets a value that indicates whether access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
		public bool IsSynchronized
		{
			get
			{
				return this._props.IsSynchronized;
			}
		}

		private ArrayList _props;
	}
}
