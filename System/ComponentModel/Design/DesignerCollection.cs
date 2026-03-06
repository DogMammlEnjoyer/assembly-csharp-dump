using System;
using System.Collections;

namespace System.ComponentModel.Design
{
	/// <summary>Represents a collection of designers.</summary>
	public class DesignerCollection : ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.DesignerCollection" /> class that contains the specified designers.</summary>
		/// <param name="designers">An array of <see cref="T:System.ComponentModel.Design.IDesignerHost" /> objects to store.</param>
		public DesignerCollection(IDesignerHost[] designers)
		{
			if (designers != null)
			{
				this._designers = new ArrayList(designers);
				return;
			}
			this._designers = new ArrayList();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.DesignerCollection" /> class that contains the specified set of designers.</summary>
		/// <param name="designers">A list that contains the collection of designers to add.</param>
		public DesignerCollection(IList designers)
		{
			this._designers = designers;
		}

		/// <summary>Gets the number of designers in the collection.</summary>
		/// <returns>The number of designers in the collection.</returns>
		public int Count
		{
			get
			{
				return this._designers.Count;
			}
		}

		/// <summary>Gets the designer at the specified index.</summary>
		/// <param name="index">The index of the designer to return.</param>
		/// <returns>The designer at the specified index.</returns>
		public virtual IDesignerHost this[int index]
		{
			get
			{
				return (IDesignerHost)this._designers[index];
			}
		}

		/// <summary>Gets a new enumerator for this collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that enumerates the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return this._designers.GetEnumerator();
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

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
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

		/// <summary>Copies the elements of the collection to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from collection. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this._designers.CopyTo(array, index);
		}

		/// <summary>Gets a new enumerator for this collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that enumerates the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private IList _designers;
	}
}
