using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains an event opcode that is defined in an event provider. An opcode defines a numeric value that identifies the activity or a point within an activity that the application was performing when it raised the event.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventOpcode
	{
		internal EventOpcode()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the localized name for an event opcode.</summary>
		/// <returns>Returns a string that contains the localized name for an event opcode.</returns>
		public string DisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the non-localized name for an event opcode.</summary>
		/// <returns>Returns a string that contains the non-localized name for an event opcode.</returns>
		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the numeric value associated with the event opcode.</summary>
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
