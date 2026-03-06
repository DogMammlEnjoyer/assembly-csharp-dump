using System;
using System.Collections.ObjectModel;
using Unity;

namespace System.Diagnostics.Tracing
{
	/// <summary>Provides data for the <see cref="M:System.Diagnostics.Tracing.EventListener.OnEventWritten(System.Diagnostics.Tracing.EventWrittenEventArgs)" /> callback.</summary>
	public class EventWrittenEventArgs : EventArgs
	{
		internal EventWrittenEventArgs(EventSource eventSource)
		{
			this.EventSource = eventSource;
		}

		/// <summary>Gets the activity ID on the thread that the event was written to.</summary>
		/// <returns>The activity ID on the thread that the event was written to.</returns>
		public Guid ActivityId
		{
			get
			{
				return EventSource.CurrentThreadActivityId;
			}
		}

		/// <summary>Gets the channel for the event.</summary>
		/// <returns>The channel for the event.</returns>
		public EventChannel Channel
		{
			get
			{
				return EventChannel.None;
			}
		}

		/// <summary>Gets the event identifier.</summary>
		/// <returns>The event identifier.</returns>
		public int EventId { get; internal set; }

		public long OSThreadId { get; internal set; }

		public DateTime TimeStamp { get; internal set; }

		/// <summary>Gets the name of the event.</summary>
		/// <returns>The name of the event.</returns>
		public string EventName { get; internal set; }

		/// <summary>Gets the event source object.</summary>
		/// <returns>The event source object.</returns>
		public EventSource EventSource { get; private set; }

		/// <summary>Gets the keywords for the event.</summary>
		/// <returns>The keywords for the event.</returns>
		public EventKeywords Keywords
		{
			get
			{
				return EventKeywords.None;
			}
		}

		/// <summary>Gets the level of the event.</summary>
		/// <returns>The level of the event.</returns>
		public EventLevel Level
		{
			get
			{
				return EventLevel.LogAlways;
			}
		}

		/// <summary>Gets the message for the event.</summary>
		/// <returns>The message for the event.</returns>
		public string Message { get; internal set; }

		/// <summary>Gets the operation code for the event.</summary>
		/// <returns>The operation code for the event.</returns>
		public EventOpcode Opcode
		{
			get
			{
				return EventOpcode.Info;
			}
		}

		/// <summary>Gets the payload for the event.</summary>
		/// <returns>The payload for the event.</returns>
		public ReadOnlyCollection<object> Payload { get; internal set; }

		/// <summary>Returns a list of strings that represent the property names of the event.</summary>
		/// <returns>Returns <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</returns>
		public ReadOnlyCollection<string> PayloadNames { get; internal set; }

		/// <summary>Gets the identifier of an activity that is related to the activity represented by the current instance.</summary>
		/// <returns>The identifier of the related activity, or <see cref="F:System.Guid.Empty" /> if there is no related activity.</returns>
		public Guid RelatedActivityId { get; internal set; }

		/// <summary>Returns the tags specified in the call to the <see cref="M:System.Diagnostics.Tracing.EventSource.Write(System.String,System.Diagnostics.Tracing.EventSourceOptions)" /> method.</summary>
		/// <returns>Returns <see cref="T:System.Diagnostics.Tracing.EventTags" />.</returns>
		public EventTags Tags
		{
			get
			{
				return EventTags.None;
			}
		}

		/// <summary>Gets the task for the event.</summary>
		/// <returns>The task for the event.</returns>
		public EventTask Task
		{
			get
			{
				return EventTask.None;
			}
		}

		/// <summary>Gets the version of the event.</summary>
		/// <returns>The version of the event.</returns>
		public byte Version
		{
			get
			{
				return 0;
			}
		}

		internal EventWrittenEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
