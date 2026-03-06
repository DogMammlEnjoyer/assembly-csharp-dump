using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	/// <summary>Represents a collection of <see cref="T:System.Security.Principal.IdentityReference" /> objects and provides a means of converting sets of <see cref="T:System.Security.Principal.IdentityReference" />-derived objects to <see cref="T:System.Security.Principal.IdentityReference" />-derived types.</summary>
	[ComVisible(false)]
	public class IdentityReferenceCollection : IEnumerable, ICollection<IdentityReference>, IEnumerable<IdentityReference>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> class with zero items in the collection.</summary>
		public IdentityReferenceCollection()
		{
			this._list = new ArrayList();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> class by using the specified initial size.</summary>
		/// <param name="capacity">The initial number of items in the collection. The value of <paramref name="capacity" /> is a hint only; it is not necessarily the maximum number of items created.</param>
		public IdentityReferenceCollection(int capacity)
		{
			this._list = new ArrayList(capacity);
		}

		/// <summary>Gets the number of items in the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</summary>
		/// <returns>The number of <see cref="T:System.Security.Principal.IdentityReference" /> objects in the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</returns>
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection is read-only.</summary>
		/// <returns>Always returns <see langword="false" />.</returns>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Sets or gets the node at the specified index of the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</summary>
		/// <param name="index">The zero-based index in the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</param>
		/// <returns>The <see cref="T:System.Security.Principal.IdentityReference" /> at the specified index in the collection. If <paramref name="index" /> is greater than or equal to the number of nodes in the collection, the return value is <see langword="null" />.</returns>
		public IdentityReference this[int index]
		{
			get
			{
				if (index >= this._list.Count)
				{
					return null;
				}
				return (IdentityReference)this._list[index];
			}
			set
			{
				this._list[index] = value;
			}
		}

		/// <summary>Adds an <see cref="T:System.Security.Principal.IdentityReference" /> object to the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</summary>
		/// <param name="identity">The <see cref="T:System.Security.Principal.IdentityReference" /> object to add to the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="identity" /> is <see langword="null" />.</exception>
		public void Add(IdentityReference identity)
		{
			this._list.Add(identity);
		}

		/// <summary>Clears all <see cref="T:System.Security.Principal.IdentityReference" /> objects from the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</summary>
		public void Clear()
		{
			this._list.Clear();
		}

		/// <summary>Indicates whether the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection contains the specified <see cref="T:System.Security.Principal.IdentityReference" /> object.</summary>
		/// <param name="identity">The <see cref="T:System.Security.Principal.IdentityReference" /> object to check for.</param>
		/// <returns>
		///   <see langword="true" /> if the collection contains the specified object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="identity" /> is <see langword="null" />.</exception>
		public bool Contains(IdentityReference identity)
		{
			using (IEnumerator enumerator = this._list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((IdentityReference)enumerator.Current).Equals(identity))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>Copies the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection to an <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> array, starting at the specified index.</summary>
		/// <param name="array">An <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> array object to which the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection is to be copied.</param>
		/// <param name="offset">The zero-based index in <paramref name="array" /> where the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection is to be copied.</param>
		public void CopyTo(IdentityReference[] array, int offset)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets an enumerator that can be used to iterate through the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</summary>
		/// <returns>An enumerator for the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</returns>
		public IEnumerator<IdentityReference> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets an enumerator that can be used to iterate through the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</summary>
		/// <returns>An enumerator for the <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <summary>Removes the specified <see cref="T:System.Security.Principal.IdentityReference" /> object from the collection.</summary>
		/// <param name="identity">The <see cref="T:System.Security.Principal.IdentityReference" /> object to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object was removed from the collection.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="identity" /> is <see langword="null" />.</exception>
		public bool Remove(IdentityReference identity)
		{
			foreach (object obj in this._list)
			{
				IdentityReference identityReference = (IdentityReference)obj;
				if (identityReference.Equals(identity))
				{
					this._list.Remove(identityReference);
					return true;
				}
			}
			return false;
		}

		/// <summary>Converts the objects in the collection to the specified type. Calling this method is the same as calling <see cref="M:System.Security.Principal.IdentityReferenceCollection.Translate(System.Type,System.Boolean)" /> with the second parameter set to <see langword="false" />, which means that exceptions will not be thrown for items that fail conversion.</summary>
		/// <param name="targetType">The type to which items in the collection are being converted.</param>
		/// <returns>A <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection that represents the converted contents of the original collection.</returns>
		public IdentityReferenceCollection Translate(Type targetType)
		{
			throw new NotImplementedException();
		}

		/// <summary>Converts the objects in the collection to the specified type and uses the specified fault tolerance to handle or ignore errors associated with a type not having a conversion mapping.</summary>
		/// <param name="targetType">The type to which items in the collection are being converted.</param>
		/// <param name="forceSuccess">A Boolean value that determines how conversion errors are handled.  
		///  If <paramref name="forceSuccess" /> is <see langword="true" />, conversion errors due to a mapping not being found for the translation result in a failed conversion and exceptions being thrown.  
		///  If <paramref name="forceSuccess" /> is <see langword="false" />, types that failed to convert due to a mapping not being found for the translation are copied without being converted into the collection being returned.</param>
		/// <returns>A <see cref="T:System.Security.Principal.IdentityReferenceCollection" /> collection that represents the converted contents of the original collection.</returns>
		public IdentityReferenceCollection Translate(Type targetType, bool forceSuccess)
		{
			throw new NotImplementedException();
		}

		private ArrayList _list;
	}
}
