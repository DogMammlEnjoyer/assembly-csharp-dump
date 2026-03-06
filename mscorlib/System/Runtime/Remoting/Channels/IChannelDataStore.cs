using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	/// <summary>Stores channel data for the remoting channels.</summary>
	[ComVisible(true)]
	public interface IChannelDataStore
	{
		/// <summary>Gets an array of channel URIs to which the current channel maps.</summary>
		/// <returns>An array of channel URIs to which the current channel maps.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		string[] ChannelUris { get; }

		/// <summary>Gets or sets the data object associated with the specified key for the implementing channel.</summary>
		/// <param name="key">The key the data object is associated with.</param>
		/// <returns>The specified data object for the implementing channel.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		object this[object key]
		{
			get;
			set;
		}
	}
}
