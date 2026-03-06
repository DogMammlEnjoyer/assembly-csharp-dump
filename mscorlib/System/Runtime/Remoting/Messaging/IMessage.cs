using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	/// <summary>Contains communication data sent between cooperating message sinks.</summary>
	[ComVisible(true)]
	public interface IMessage
	{
		/// <summary>Gets an <see cref="T:System.Collections.IDictionary" /> that represents a collection of the message's properties.</summary>
		/// <returns>A dictionary that represents a collection of the message's properties.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		IDictionary Properties { get; }
	}
}
