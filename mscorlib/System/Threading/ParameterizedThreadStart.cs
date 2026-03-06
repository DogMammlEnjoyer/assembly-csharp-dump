using System;

namespace System.Threading
{
	/// <summary>Represents the method that executes on a <see cref="T:System.Threading.Thread" />.</summary>
	/// <param name="obj">An object that contains data for the thread procedure.</param>
	public delegate void ParameterizedThreadStart(object obj);
}
