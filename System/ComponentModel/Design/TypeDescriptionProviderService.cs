using System;

namespace System.ComponentModel.Design
{
	/// <summary>Provides a type description provider for a specified type.</summary>
	public abstract class TypeDescriptionProviderService
	{
		/// <summary>Gets a type description provider for the specified object.</summary>
		/// <param name="instance">The object to get a type description provider for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> that corresponds with <paramref name="instance" />.</returns>
		public abstract TypeDescriptionProvider GetProvider(object instance);

		/// <summary>Gets a type description provider for the specified type.</summary>
		/// <param name="type">The type to get a type description provider for.</param>
		/// <returns>A <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> that corresponds with <paramref name="type" />.</returns>
		public abstract TypeDescriptionProvider GetProvider(Type type);
	}
}
