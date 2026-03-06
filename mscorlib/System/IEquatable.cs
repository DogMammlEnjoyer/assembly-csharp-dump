using System;

namespace System
{
	/// <summary>Defines a generalized method that a value type or class implements to create a type-specific method for determining equality of instances.</summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	public interface IEquatable<T>
	{
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		bool Equals(T other);
	}
}
