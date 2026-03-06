using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Security.Cryptography
{
	/// <summary>Represents a collection of <see cref="T:System.Security.Cryptography.AsnEncodedData" /> objects. This class cannot be inherited.</summary>
	public sealed class AsnEncodedDataCollection : ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> class.</summary>
		public AsnEncodedDataCollection()
		{
			this._list = new List<AsnEncodedData>();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> class and adds an <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object to the collection.</summary>
		/// <param name="asnEncodedData">The <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object to add to the collection.</param>
		public AsnEncodedDataCollection(AsnEncodedData asnEncodedData) : this()
		{
			this._list.Add(asnEncodedData);
		}

		/// <summary>Adds an <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object to the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</summary>
		/// <param name="asnEncodedData">The <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object to add to the collection.</param>
		/// <returns>The index of the added <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asnEncodedData" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">Neither of the OIDs are <see langword="null" /> and the OIDs do not match.
		/// -or-
		/// One of the OIDs is <see langword="null" /> and the OIDs do not match.</exception>
		public int Add(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			int count = this._list.Count;
			this._list.Add(asnEncodedData);
			return count;
		}

		/// <summary>Removes an <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object from the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</summary>
		/// <param name="asnEncodedData">The <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asnEncodedData" /> is <see langword="null" />.</exception>
		public void Remove(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			this._list.Remove(asnEncodedData);
		}

		/// <summary>Gets an <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object from the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</summary>
		/// <param name="index">The location in the collection.</param>
		/// <returns>An <see cref="T:System.Security.Cryptography.AsnEncodedData" /> object.</returns>
		public AsnEncodedData this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

		/// <summary>Gets the number of <see cref="T:System.Security.Cryptography.AsnEncodedData" /> objects in a collection.</summary>
		/// <returns>The number of <see cref="T:System.Security.Cryptography.AsnEncodedData" /> objects.</returns>
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		/// <summary>Returns an <see cref="T:System.Security.Cryptography.AsnEncodedDataEnumerator" /> object that can be used to navigate the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.AsnEncodedDataEnumerator" /> object.</returns>
		public AsnEncodedDataEnumerator GetEnumerator()
		{
			return new AsnEncodedDataEnumerator(this);
		}

		/// <summary>Returns an <see cref="T:System.Security.Cryptography.AsnEncodedDataEnumerator" /> object that can be used to navigate the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.AsnEncodedDataEnumerator" /> object that can be used to navigate the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Copies the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object into an array.</summary>
		/// <param name="array">The array that the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object is to be copied into.</param>
		/// <param name="index">The location where the copy operation starts.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is a multidimensional array, which is not supported by this method.
		/// -or-
		/// The length for <paramref name="index" /> is invalid.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length for <paramref name="index" /> is out of range.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (this.Count > array.Length - index)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index);
				index++;
			}
		}

		/// <summary>Copies the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object into an array.</summary>
		/// <param name="array">The array that the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object is to be copied into.</param>
		/// <param name="index">The location where the copy operation starts.</param>
		public void CopyTo(AsnEncodedData[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			this._list.CopyTo(array, index);
		}

		/// <summary>Gets a value that indicates whether access to the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object is thread safe.</summary>
		/// <returns>
		///   <see langword="false" /> in all cases.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</summary>
		/// <returns>An object used to synchronize access to the <see cref="T:System.Security.Cryptography.AsnEncodedDataCollection" /> object.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private readonly List<AsnEncodedData> _list;
	}
}
