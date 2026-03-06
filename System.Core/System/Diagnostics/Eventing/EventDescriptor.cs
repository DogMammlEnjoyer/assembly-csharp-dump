using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing
{
	/// <summary>Contains the metadata that defines an event.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct EventDescriptor
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.EventDescriptor" /> class.</summary>
		/// <param name="id">The event identifier.</param>
		/// <param name="version">Version of the event. The version indicates a revision to the event definition. You can use this member and the Id member to identify a unique event.</param>
		/// <param name="channel">Defines a potential target for the event.</param>
		/// <param name="level">Specifies the level of detail included in the event.</param>
		/// <param name="opcode">Operation being performed at the time the event is written.</param>
		/// <param name="task">Identifies a logical component of the application that is writing the event.</param>
		/// <param name="keywords">Bit mask that specifies the event category. The keyword can contain one or more provider-defined keywords, standard keywords, or both.</param>
		public EventDescriptor(int id, byte version, byte channel, byte level, byte opcode, int task, long keywords)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Retrieves the channel value from the event descriptor.</summary>
		/// <returns>The channel that defines a potential target for the event.</returns>
		public byte Channel
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Retrieves the event identifier value from the event descriptor.</summary>
		/// <returns>The event identifier.</returns>
		public int EventId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Retrieves the keyword value from the event descriptor.</summary>
		/// <returns>The keyword, which is a bit mask, that specifies the event category.</returns>
		public long Keywords
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		/// <summary>Retrieves the level value from the event descriptor.</summary>
		/// <returns>The level of detail included in the event.</returns>
		public byte Level
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Retrieves the operation code value from the event descriptor.</summary>
		/// <returns>The operation being performed at the time the event is written.</returns>
		public byte Opcode
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Retrieves the task value from the event descriptor.</summary>
		/// <returns>The task that identifies the logical component of the application that is writing the event.</returns>
		public int Task
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Retrieves the version value from the event descriptor.</summary>
		/// <returns>The version of the event. </returns>
		public byte Version
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}
	}
}
