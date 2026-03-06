using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Defines the standard operation codes that the event source attaches to events.</summary>
	public enum EventOpcode
	{
		/// <summary>An informational event.</summary>
		Info,
		/// <summary>An event that is published when an application starts a new transaction or activity. This operation code can be embedded within another transaction or activity when multiple events that have the <see cref="F:System.Diagnostics.Tracing.EventOpcode.Start" /> code follow each other without an intervening event that has a <see cref="F:System.Diagnostics.Tracing.EventOpcode.Stop" /> code.</summary>
		Start,
		/// <summary>An event that is published when an activity or a transaction in an application ends. The event corresponds to the last unpaired event that has a <see cref="F:System.Diagnostics.Tracing.EventOpcode.Start" /> operation code.</summary>
		Stop,
		/// <summary>A trace collection start event.</summary>
		DataCollectionStart,
		/// <summary>A trace collection stop event.</summary>
		DataCollectionStop,
		/// <summary>An extension event.</summary>
		Extension,
		/// <summary>An event that is published after an activity in an application replies to an event.</summary>
		Reply,
		/// <summary>An event that is published after an activity in an application resumes from a suspended state. The event should follow an event that has the <see cref="F:System.Diagnostics.Tracing.EventOpcode.Suspend" /> operation code.</summary>
		Resume,
		/// <summary>An event that is published when an activity in an application is suspended.</summary>
		Suspend,
		/// <summary>An event that is published when one activity in an application transfers data or system resources to another activity.</summary>
		Send,
		/// <summary>An event that is published when one activity in an application receives data.</summary>
		Receive = 240
	}
}
