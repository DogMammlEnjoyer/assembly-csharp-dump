using System;

namespace System.Collections.Specialized
{
	/// <summary>Creates collections that ignore the case in strings.</summary>
	public class CollectionsUtil
	{
		/// <summary>Creates a new case-insensitive instance of the <see cref="T:System.Collections.Hashtable" /> class with the default initial capacity.</summary>
		/// <returns>A new case-insensitive instance of the <see cref="T:System.Collections.Hashtable" /> class with the default initial capacity.</returns>
		public static Hashtable CreateCaseInsensitiveHashtable()
		{
			return new Hashtable(StringComparer.CurrentCultureIgnoreCase);
		}

		/// <summary>Creates a new case-insensitive instance of the <see cref="T:System.Collections.Hashtable" /> class with the specified initial capacity.</summary>
		/// <param name="capacity">The approximate number of entries that the <see cref="T:System.Collections.Hashtable" /> can initially contain.</param>
		/// <returns>A new case-insensitive instance of the <see cref="T:System.Collections.Hashtable" /> class with the specified initial capacity.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.</exception>
		public static Hashtable CreateCaseInsensitiveHashtable(int capacity)
		{
			return new Hashtable(capacity, StringComparer.CurrentCultureIgnoreCase);
		}

		/// <summary>Copies the entries from the specified dictionary to a new case-insensitive instance of the <see cref="T:System.Collections.Hashtable" /> class with the same initial capacity as the number of entries copied.</summary>
		/// <param name="d">The <see cref="T:System.Collections.IDictionary" /> to copy to a new case-insensitive <see cref="T:System.Collections.Hashtable" />.</param>
		/// <returns>A new case-insensitive instance of the <see cref="T:System.Collections.Hashtable" /> class containing the entries from the specified <see cref="T:System.Collections.IDictionary" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		public static Hashtable CreateCaseInsensitiveHashtable(IDictionary d)
		{
			return new Hashtable(d, StringComparer.CurrentCultureIgnoreCase);
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Collections.SortedList" /> class that ignores the case of strings.</summary>
		/// <returns>A new instance of the <see cref="T:System.Collections.SortedList" /> class that ignores the case of strings.</returns>
		public static SortedList CreateCaseInsensitiveSortedList()
		{
			return new SortedList(CaseInsensitiveComparer.Default);
		}
	}
}
