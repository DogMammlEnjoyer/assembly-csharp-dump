using System;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	/// <summary>Provides methods for enabling and disabling events from event sources.</summary>
	public class EventListener : IDisposable
	{
		/// <summary>Gets a small non-negative number that represents the specified event source.</summary>
		/// <param name="eventSource">The event source to find the index for.</param>
		/// <returns>A small non-negative number that represents the specified event source.</returns>
		public static int EventSourceIndex(EventSource eventSource)
		{
			return 0;
		}

		/// <summary>Enables events for the specified event source that has the specified verbosity level or lower.</summary>
		/// <param name="eventSource">The event source to enable events for.</param>
		/// <param name="level">The level of events to enable.</param>
		public void EnableEvents(EventSource eventSource, EventLevel level)
		{
		}

		/// <summary>Enables events for the specified event source that has the specified verbosity level or lower, and matching keyword flags.</summary>
		/// <param name="eventSource">The event source to enable events for.</param>
		/// <param name="level">The level of events to enable.</param>
		/// <param name="matchAnyKeyword">The keyword flags necessary to enable the events.</param>
		public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword)
		{
		}

		/// <summary>Enables events for the specified event source that has the specified verbosity level or lower, matching event keyword flag, and matching arguments.</summary>
		/// <param name="eventSource">The event source to enable events for.</param>
		/// <param name="level">The level of events to enable.</param>
		/// <param name="matchAnyKeyword">The keyword flags necessary to enable the events.</param>
		/// <param name="arguments">The arguments to be matched to enable the events.</param>
		public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword, IDictionary<string, string> arguments)
		{
		}

		/// <summary>Disables all events for the specified event source.</summary>
		/// <param name="eventSource">The event source to disable events for.</param>
		public void DisableEvents(EventSource eventSource)
		{
		}

		/// <summary>Called for all existing event sources when the event listener is created and when a new event source is attached to the listener.</summary>
		/// <param name="eventSource">The event source.</param>
		protected internal virtual void OnEventSourceCreated(EventSource eventSource)
		{
		}

		/// <summary>Called whenever an event has been written by an event source for which the event listener has enabled events.</summary>
		/// <param name="eventData">The event arguments that describe the event.</param>
		protected internal virtual void OnEventWritten(EventWrittenEventArgs eventData)
		{
		}

		/// <summary>Releases the resources used by the current instance of the <see cref="T:System.Diagnostics.Tracing.EventListener" /> class.</summary>
		public virtual void Dispose()
		{
		}

		/// <summary>Occurs when an event source (<see cref="T:System.Diagnostics.Tracing.EventSource" /> object) is attached to the dispatcher.</summary>
		public event EventHandler<EventSourceCreatedEventArgs> EventSourceCreated;

		/// <summary>Occurs when an event has been written by an event source (<see cref="T:System.Diagnostics.Tracing.EventSource" /> object) for which the event listener has enabled events.</summary>
		public event EventHandler<EventWrittenEventArgs> EventWritten;
	}
}
