using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting
{
	/// <summary>Provides custom channel information that is carried along with the <see cref="T:System.Runtime.Remoting.ObjRef" />.</summary>
	[ComVisible(true)]
	public interface IChannelInfo
	{
		/// <summary>Gets or sets the channel data for each channel.</summary>
		/// <returns>The channel data for each channel.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		object[] ChannelData { get; set; }
	}
}
