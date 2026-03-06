using System;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	/// <summary>Provides the arguments for the <see cref="M:System.Diagnostics.Tracing.EventSource.OnEventCommand(System.Diagnostics.Tracing.EventCommandEventArgs)" /> callback.</summary>
	public class EventCommandEventArgs : EventArgs
	{
		private EventCommandEventArgs()
		{
		}

		/// <summary>Gets the array of arguments for the callback.</summary>
		/// <returns>An array of callback arguments.</returns>
		public IDictionary<string, string> Arguments
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the command for the callback.</summary>
		/// <returns>The callback command.</returns>
		public EventCommand Command
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Disables the event that have the specified identifier.</summary>
		/// <param name="eventId">The identifier of the event to disable.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="eventId" /> is in range; otherwise, <see langword="false" />.</returns>
		public bool DisableEvent(int eventId)
		{
			return true;
		}

		/// <summary>Enables the event that has the specified identifier.</summary>
		/// <param name="eventId">The identifier of the event to enable.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="eventId" /> is in range; otherwise, <see langword="false" />.</returns>
		public bool EnableEvent(int eventId)
		{
			return true;
		}
	}
}
