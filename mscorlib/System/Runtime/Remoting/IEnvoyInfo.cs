using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting
{
	/// <summary>Provides envoy information.</summary>
	[ComVisible(true)]
	public interface IEnvoyInfo
	{
		/// <summary>Gets or sets the list of envoys that were contributed by the server context and object chains when the object was marshaled.</summary>
		/// <returns>A chain of envoy sinks.</returns>
		IMessageSink EnvoySinks { get; set; }
	}
}
