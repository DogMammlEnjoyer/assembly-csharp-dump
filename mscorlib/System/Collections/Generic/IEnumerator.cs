using System;

namespace System.Collections.Generic
{
	/// <summary>Supports a simple iteration over a generic collection.</summary>
	/// <typeparam name="T">The type of objects to enumerate.</typeparam>
	public interface IEnumerator<out T> : IDisposable, IEnumerator
	{
		/// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
		/// <returns>The element in the collection at the current position of the enumerator.</returns>
		T Current { get; }
	}
}
