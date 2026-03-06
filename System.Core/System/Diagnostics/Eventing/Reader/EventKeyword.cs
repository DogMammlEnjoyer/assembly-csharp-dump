using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Represents a keyword for an event. Keywords are defined in an event provider and are used to group the event with other similar events (based on the usage of the events).</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventKeyword
	{
		internal EventKeyword()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the localized name of the keyword.</summary>
		/// <returns>Returns a string that contains a localized name for this keyword.</returns>
		public string DisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the non-localized name of the keyword.</summary>
		/// <returns>Returns a string that contains the non-localized name of this keyword.</returns>
		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the numeric value associated with the keyword.</summary>
		/// <returns>Returns a <see langword="long" /> value.</returns>
		public long Value
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}
	}
}
