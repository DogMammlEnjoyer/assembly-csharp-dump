using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	/// <summary>Defines the interface for a message sink.</summary>
	[ComVisible(true)]
	public interface IMessageSink
	{
		/// <summary>Synchronously processes the given message.</summary>
		/// <param name="msg">The message to process.</param>
		/// <returns>A reply message in response to the request.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		IMessage SyncProcessMessage(IMessage msg);

		/// <summary>Asynchronously processes the given message.</summary>
		/// <param name="msg">The message to process.</param>
		/// <param name="replySink">The reply sink for the reply message.</param>
		/// <returns>An <see cref="T:System.Runtime.Remoting.Messaging.IMessageCtrl" /> interface that provides a way to control asynchronous messages after they have been dispatched.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink);

		/// <summary>Gets the next message sink in the sink chain.</summary>
		/// <returns>The next message sink in the sink chain.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		IMessageSink NextSink { get; }
	}
}
