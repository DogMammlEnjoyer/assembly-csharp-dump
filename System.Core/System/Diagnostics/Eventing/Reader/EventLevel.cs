using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains an event level that is defined in an event provider. The level signifies the severity of the event.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventLevel
	{
		internal EventLevel()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the localized name for the event level. The name describes what severity level of events this level is used for.</summary>
		/// <returns>Returns a string that contains the localized name for the event level.</returns>
		public string DisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the non-localized name of the event level.</summary>
		/// <returns>Returns a string that contains the non-localized name of the event level.</returns>
		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the numeric value of the event level.</summary>
		/// <returns>Returns an integer value.</returns>
		public int Value
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}
	}
}
