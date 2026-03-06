using System;

namespace System.Collections
{
	/// <summary>Enumerates the elements of a nongeneric dictionary.</summary>
	public interface IDictionaryEnumerator : IEnumerator
	{
		/// <summary>Gets the key of the current dictionary entry.</summary>
		/// <returns>The key of the current element of the enumeration.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator" /> is positioned before the first entry of the dictionary or after the last entry.</exception>
		object Key { get; }

		/// <summary>Gets the value of the current dictionary entry.</summary>
		/// <returns>The value of the current element of the enumeration.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator" /> is positioned before the first entry of the dictionary or after the last entry.</exception>
		object Value { get; }

		/// <summary>Gets both the key and the value of the current dictionary entry.</summary>
		/// <returns>A <see cref="T:System.Collections.DictionaryEntry" /> containing both the key and the value of the current dictionary entry.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator" /> is positioned before the first entry of the dictionary or after the last entry.</exception>
		DictionaryEntry Entry { get; }
	}
}
