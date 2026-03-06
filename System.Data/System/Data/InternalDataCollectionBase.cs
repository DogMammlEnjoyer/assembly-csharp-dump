using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Data
{
	/// <summary>Provides the base functionality for creating collections.</summary>
	public class InternalDataCollectionBase : ICollection, IEnumerable
	{
		/// <summary>Gets the total number of elements in a collection.</summary>
		/// <returns>The total number of elements in a collection.</returns>
		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return this.List.Count;
			}
		}

		/// <summary>Copies all the elements of the current <see cref="T:System.Data.InternalDataCollectionBase" /> to a one-dimensional <see cref="T:System.Array" />, starting at the specified <see cref="T:System.Data.InternalDataCollectionBase" /> index.</summary>
		/// <param name="ar">The one-dimensional <see cref="T:System.Array" /> to copy the current <see cref="T:System.Data.InternalDataCollectionBase" /> object's elements into.</param>
		/// <param name="index">The destination <see cref="T:System.Array" /> index to start copying into.</param>
		public virtual void CopyTo(Array ar, int index)
		{
			this.List.CopyTo(ar, index);
		}

		/// <summary>Gets an <see cref="T:System.Collections.IEnumerator" /> for the collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the collection.</returns>
		public virtual IEnumerator GetEnumerator()
		{
			return this.List.GetEnumerator();
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.InternalDataCollectionBase" /> is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection is read-only; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.InternalDataCollectionBase" /> is synchonized.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection is synchronized; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		internal int NamesEqual(string s1, string s2, bool fCaseSensitive, CultureInfo locale)
		{
			if (fCaseSensitive)
			{
				if (string.Compare(s1, s2, false, locale) != 0)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (locale.CompareInfo.Compare(s1, s2, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) != 0)
				{
					return 0;
				}
				if (string.Compare(s1, s2, false, locale) != 0)
				{
					return -1;
				}
				return 1;
			}
		}

		/// <summary>Gets an object that can be used to synchronize the collection.</summary>
		/// <returns>The <see cref="T:System.Object" /> used to synchronize the collection.</returns>
		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Gets the items of the collection as a list.</summary>
		/// <returns>An <see cref="T:System.Collections.ArrayList" /> that contains the collection.</returns>
		protected virtual ArrayList List
		{
			get
			{
				return null;
			}
		}

		internal static readonly CollectionChangeEventArgs s_refreshEventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
	}
}
