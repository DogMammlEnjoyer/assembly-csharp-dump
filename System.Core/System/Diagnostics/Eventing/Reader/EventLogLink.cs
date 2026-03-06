using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Represents a link between an event provider and an event log that the provider publishes events into. This object cannot be instantiated.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventLogLink
	{
		internal EventLogLink()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the localized name of the event log.</summary>
		/// <returns>Returns a string that contains the localized name of the event log.</returns>
		public string DisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a Boolean value that determines whether the event log is imported, rather than defined in the event provider. An imported event log is defined in a different provider.</summary>
		/// <returns>Returns <see langword="true" /> if the event log is imported by the event provider, and returns <see langword="false" /> if the event log is not imported by the event provider.</returns>
		public bool IsImported
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Gets the non-localized name of the event log associated with this object.</summary>
		/// <returns>Returns a string that contains the non-localized name of the event log associated with this object.</returns>
		public string LogName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
