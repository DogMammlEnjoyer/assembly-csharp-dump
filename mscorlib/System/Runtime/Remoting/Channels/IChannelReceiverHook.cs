using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	/// <summary>Indicates that the implementing channel wants to hook into the outside listener service.</summary>
	[ComVisible(true)]
	public interface IChannelReceiverHook
	{
		/// <summary>Gets the type of listener to hook into.</summary>
		/// <returns>The type of listener to hook into (for example, "http").</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		string ChannelScheme { get; }

		/// <summary>Gets the channel sink chain that the current channel is using.</summary>
		/// <returns>The channel sink chain that the current channel is using.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		IServerChannelSink ChannelSinkChain { get; }

		/// <summary>Gets a Boolean value that indicates whether <see cref="T:System.Runtime.Remoting.Channels.IChannelReceiverHook" /> needs to be hooked into the outside listener service.</summary>
		/// <returns>A Boolean value that indicates whether <see cref="T:System.Runtime.Remoting.Channels.IChannelReceiverHook" /> needs to be hooked into the outside listener service.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		bool WantsToListen { get; }

		/// <summary>Adds a URI on which the channel hook will listen.</summary>
		/// <param name="channelUri">A URI on which the channel hook will listen.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		void AddHookChannelUri(string channelUri);
	}
}
