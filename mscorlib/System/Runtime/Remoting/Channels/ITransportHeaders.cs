using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	/// <summary>Stores a collection of headers used in the channel sinks.</summary>
	[ComVisible(true)]
	public interface ITransportHeaders
	{
		/// <summary>Gets or sets a transport header associated with the given key.</summary>
		/// <param name="key">The key the requested transport header is associated with.</param>
		/// <returns>A transport header associated with the given key.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		object this[object key]
		{
			get;
			set;
		}

		/// <summary>Returns a <see cref="T:System.Collections.IEnumerator" /> that iterates over all entries in the <see cref="T:System.Runtime.Remoting.Channels.ITransportHeaders" /> object.</summary>
		/// <returns>A <see cref="T:System.Collections.IEnumerator" /> that iterates over all entries in the <see cref="T:System.Runtime.Remoting.Channels.ITransportHeaders" /> object.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		IEnumerator GetEnumerator();
	}
}
