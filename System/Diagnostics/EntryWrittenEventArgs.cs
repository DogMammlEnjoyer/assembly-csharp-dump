using System;

namespace System.Diagnostics
{
	/// <summary>Provides data for the <see cref="E:System.Diagnostics.EventLog.EntryWritten" /> event.</summary>
	public class EntryWrittenEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.EntryWrittenEventArgs" /> class.</summary>
		public EntryWrittenEventArgs() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.EntryWrittenEventArgs" /> class with the specified event log entry.</summary>
		/// <param name="entry">An <see cref="T:System.Diagnostics.EventLogEntry" /> that represents the entry that was written.</param>
		public EntryWrittenEventArgs(EventLogEntry entry)
		{
			this.entry = entry;
		}

		/// <summary>Gets the event log entry that was written to the log.</summary>
		/// <returns>An <see cref="T:System.Diagnostics.EventLogEntry" /> that represents the entry that was written to the event log.</returns>
		public EventLogEntry Entry
		{
			get
			{
				return this.entry;
			}
		}

		private EventLogEntry entry;
	}
}
