using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Enables users to write activation code for managed objects that extend <see cref="T:System.MarshalByRefObject" />.</summary>
	public interface ICustomFactory
	{
		/// <summary>Creates a new instance of the specified type.</summary>
		/// <param name="serverType">The type to activate.</param>
		/// <returns>A <see cref="T:System.MarshalByRefObject" /> associated with the specified type.</returns>
		MarshalByRefObject CreateInstance(Type serverType);
	}
}
