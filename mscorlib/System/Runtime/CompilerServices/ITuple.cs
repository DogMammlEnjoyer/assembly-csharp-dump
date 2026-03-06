using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>Defines a general-purpose Tuple implementation that allows acccess to Tuple instance members without knowing the underlying Tuple type.</summary>
	public interface ITuple
	{
		/// <summary>Gets the number of elements in this <see langword="Tuple" /> instance.</summary>
		/// <returns>The number of elements in this <see langword="Tuple" /> instance.</returns>
		int Length { get; }

		/// <summary>Returns the value of the specified <see langword="Tuple" /> element.</summary>
		/// <param name="index">The index of the specified <see langword="Tuple" /> element. <paramref name="index" /> can range from 0 for <see langword="Item1" /> of the <see langword="Tuple" /> to one less than the number of elements in the <see langword="Tuple" />.</param>
		/// <returns>The value of the specified <see langword="Tuple" /> element.</returns>
		object this[int index]
		{
			get;
		}
	}
}
