using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	/// <summary>Represents the method that will handle processing of headers on the stream during deserialization.</summary>
	/// <param name="headers">The headers of the event.</param>
	/// <returns>A <see cref="T:System.Object" /> that conveys information about a remote function call.</returns>
	[ComVisible(true)]
	public delegate object HeaderHandler(Header[] headers);
}
