using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Net.NetworkInformation
{
	/// <summary>Stores a set of <see cref="T:System.Net.IPAddress" /> types.</summary>
	public class IPAddressCollection : ICollection<IPAddress>, IEnumerable<IPAddress>, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkInformation.IPAddressCollection" /> class.</summary>
		protected internal IPAddressCollection()
		{
		}

		/// <summary>Copies the elements in this collection to a one-dimensional array of type <see cref="T:System.Net.IPAddress" />.</summary>
		/// <param name="array">A one-dimensional array that receives a copy of the collection.</param>
		/// <param name="offset">The zero-based index in <paramref name="array" /> at which the copy begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		///
		///  -or-  
		///  The number of elements in this <see cref="T:System.Net.NetworkInformation.IPAddressCollection" /> is greater than the available space from <paramref name="offset" /> to the end of the destination <paramref name="array" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The elements in this <see cref="T:System.Net.NetworkInformation.IPAddressCollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public virtual void CopyTo(IPAddress[] array, int offset)
		{
			this.addresses.CopyTo(array, offset);
		}

		/// <summary>Gets the number of <see cref="T:System.Net.IPAddress" /> types in this collection.</summary>
		/// <returns>An <see cref="T:System.Int32" /> value that contains the number of <see cref="T:System.Net.IPAddress" /> types in this collection.</returns>
		public virtual int Count
		{
			get
			{
				return this.addresses.Count;
			}
		}

		/// <summary>Gets a value that indicates whether access to this collection is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> in all cases.</returns>
		public virtual bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>Throws a <see cref="T:System.NotSupportedException" /> because this operation is not supported for this collection.</summary>
		/// <param name="address">The object to be added to the collection.</param>
		public virtual void Add(IPAddress address)
		{
			throw new NotSupportedException(SR.GetString("The collection is read-only."));
		}

		internal void InternalAdd(IPAddress address)
		{
			this.addresses.Add(address);
		}

		/// <summary>Checks whether the collection contains the specified <see cref="T:System.Net.IPAddress" /> object.</summary>
		/// <param name="address">The <see cref="T:System.Net.IPAddress" /> object to be searched in the collection.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.IPAddress" /> object exists in the collection; otherwise, <see langword="false" />.</returns>
		public virtual bool Contains(IPAddress address)
		{
			return this.addresses.Contains(address);
		}

		/// <summary>Returns an object that can be used to iterate through this collection.</summary>
		/// <returns>An object that implements the <see cref="T:System.Collections.IEnumerator" /> interface and provides access to the <see cref="T:System.Net.NetworkInformation.IPAddressCollection" /> types in this collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns an object that can be used to iterate through this collection.</summary>
		/// <returns>An object that implements the <see cref="T:System.Collections.IEnumerator" /> interface and provides access to the <see cref="T:System.Net.NetworkInformation.IPAddressCollection" /> types in this collection.</returns>
		public virtual IEnumerator<IPAddress> GetEnumerator()
		{
			return this.addresses.GetEnumerator();
		}

		/// <summary>Gets the <see cref="T:System.Net.IPAddress" /> at the specific index of the collection.</summary>
		/// <param name="index">The index of interest.</param>
		/// <returns>The <see cref="T:System.Net.IPAddress" /> at the specific index in the collection.</returns>
		public virtual IPAddress this[int index]
		{
			get
			{
				return this.addresses[index];
			}
		}

		/// <summary>Throws a <see cref="T:System.NotSupportedException" /> because this operation is not supported for this collection.</summary>
		/// <param name="address">The object to be removed.</param>
		/// <returns>Always throws a <see cref="T:System.NotSupportedException" />.</returns>
		public virtual bool Remove(IPAddress address)
		{
			throw new NotSupportedException(SR.GetString("The collection is read-only."));
		}

		/// <summary>Throws a <see cref="T:System.NotSupportedException" /> because this operation is not supported for this collection.</summary>
		public virtual void Clear()
		{
			throw new NotSupportedException(SR.GetString("The collection is read-only."));
		}

		private Collection<IPAddress> addresses = new Collection<IPAddress>();
	}
}
