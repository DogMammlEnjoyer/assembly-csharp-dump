using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Provides data for the <see cref="E:System.Diagnostics.Tracing.EventListener.EventSourceCreated" /> event.</summary>
	public class EventSourceCreatedEventArgs : EventArgs
	{
		/// <summary>Get the event source that is attaching to the listener.</summary>
		/// <returns>The event source that is attaching to the listener.</returns>
		public EventSource EventSource { get; internal set; }
	}
}
