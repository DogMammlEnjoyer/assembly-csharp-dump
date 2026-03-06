using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	/// <summary>Provides required functions and properties for the receiver channels.</summary>
	[ComVisible(true)]
	public interface IChannelReceiver : IChannel
	{
		/// <summary>Gets the channel-specific data.</summary>
		/// <returns>The channel data.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		object ChannelData { get; }

		/// <summary>Returns an array of all the URLs for a URI.</summary>
		/// <param name="objectURI">The URI for which URLs are required.</param>
		/// <returns>An array of the URLs.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		string[] GetUrlsForUri(string objectURI);

		/// <summary>Instructs the current channel to start listening for requests.</summary>
		/// <param name="data">Optional initialization information.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		void StartListening(object data);

		/// <summary>Instructs the current channel to stop listening for requests.</summary>
		/// <param name="data">Optional state information for the channel.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		void StopListening(object data);
	}
}
