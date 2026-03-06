using System;

namespace System.ComponentModel.Design
{
	/// <summary>Provides a basic, component site-specific, key-value pair dictionary through a service that a designer can use to store user-defined data.</summary>
	public interface IDictionaryService
	{
		/// <summary>Gets the key corresponding to the specified value.</summary>
		/// <param name="value">The value to look up in the dictionary.</param>
		/// <returns>The associated key, or <see langword="null" /> if no key exists.</returns>
		object GetKey(object value);

		/// <summary>Gets the value corresponding to the specified key.</summary>
		/// <param name="key">The key to look up the value for.</param>
		/// <returns>The associated value, or <see langword="null" /> if no value exists.</returns>
		object GetValue(object key);

		/// <summary>Sets the specified key-value pair.</summary>
		/// <param name="key">An object to use as the key to associate the value with.</param>
		/// <param name="value">The value to store.</param>
		void SetValue(object key, object value);
	}
}
