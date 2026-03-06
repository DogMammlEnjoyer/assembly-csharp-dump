using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains an event task that is defined in an event provider. The task identifies a portion of an application or a component that publishes an event. A task is a 16-bit value with 16 top values reserved.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventTask
	{
		internal EventTask()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the localized name for the event task.</summary>
		/// <returns>Returns a string that contains the localized name for the event task.</returns>
		public string DisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the event globally unique identifier (GUID) associated with the task. </summary>
		/// <returns>Returns a GUID value.</returns>
		public Guid EventGuid
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(Guid);
			}
		}

		/// <summary>Gets the non-localized name of the event task.</summary>
		/// <returns>Returns a string that contains the non-localized name of the event task.</returns>
		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the numeric value associated with the task.</summary>
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
