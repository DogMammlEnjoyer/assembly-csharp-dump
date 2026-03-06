using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies additional event schema information for an event.</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class EventAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Tracing.EventAttribute" /> class with the specified event identifier.</summary>
		/// <param name="eventId">The event identifier for the event.</param>
		public EventAttribute(int eventId)
		{
			this.EventId = eventId;
		}

		/// <summary>Gets or sets the identifier for the event.</summary>
		/// <returns>The event identifier. This value should be between 0 and 65535.</returns>
		public int EventId { get; private set; }

		/// <summary>Specifies the behavior of the start and stop events of an activity. An activity is the region of time in an app between the start and the stop.</summary>
		/// <returns>Returns <see cref="T:System.Diagnostics.Tracing.EventActivityOptions" />.</returns>
		public EventActivityOptions ActivityOptions { get; set; }

		/// <summary>Gets or sets the level for the event.</summary>
		/// <returns>One of the enumeration values that specifies the level for the event.</returns>
		public EventLevel Level { get; set; }

		/// <summary>Gets or sets the keywords for the event.</summary>
		/// <returns>A bitwise combination of the enumeration values.</returns>
		public EventKeywords Keywords { get; set; }

		/// <summary>Gets or sets the operation code for the event.</summary>
		/// <returns>One of the enumeration values that specifies the operation code.</returns>
		public EventOpcode Opcode { get; set; }

		/// <summary>Gets or sets an additional event log where the event should be written.</summary>
		/// <returns>An additional event log where the event should be written.</returns>
		public EventChannel Channel { get; set; }

		/// <summary>Gets or sets the message for the event.</summary>
		/// <returns>The message for the event.</returns>
		public string Message { get; set; }

		/// <summary>Gets or sets the task for the event.</summary>
		/// <returns>The task for the event.</returns>
		public EventTask Task { get; set; }

		/// <summary>Gets or sets the <see cref="T:System.Diagnostics.Tracing.EventTags" /> value for this <see cref="T:System.Diagnostics.Tracing.EventAttribute" /> object. An event tag is a user-defined value that is passed through when the event is logged.</summary>
		/// <returns>The <see cref="T:System.Diagnostics.Tracing.EventTags" /> value for this <see cref="T:System.Diagnostics.Tracing.EventAttribute" /> object. An event tag is a user-defined value that is passed through when the event is logged.</returns>
		public EventTags Tags { get; set; }

		/// <summary>Gets or sets the version of the event.</summary>
		/// <returns>The version of the event.</returns>
		public byte Version { get; set; }
	}
}
