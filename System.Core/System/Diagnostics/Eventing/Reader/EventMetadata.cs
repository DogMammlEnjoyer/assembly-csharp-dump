using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains the metadata (properties and settings) for an event that is defined in an event provider. </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventMetadata
	{
		internal EventMetadata()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the description template associated with the event using the current thread locale for the description language.</summary>
		/// <returns>Returns a string that contains the description template associated with the event.</returns>
		public string Description
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the identifier of the event that is defined in the event provider.</summary>
		/// <returns>Returns a <see langword="long" /> value that is the event identifier.</returns>
		public long Id
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		/// <summary>Gets the keywords associated with the event that is defined in the even provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventKeyword" /> objects.</returns>
		public IEnumerable<EventKeyword> Keywords
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the level associated with the event that is defined in the event provider. The level defines the severity of the event.</summary>
		/// <returns>Returns an <see cref="T:System.Diagnostics.Eventing.Reader.EventLevel" /> object.</returns>
		public EventLevel Level
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a link to the event log that receives this event when the provider publishes this event.</summary>
		/// <returns>Returns a <see cref="T:System.Diagnostics.Eventing.Reader.EventLogLink" /> object.</returns>
		public EventLogLink LogLink
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the opcode associated with this event that is defined by an event provider. The opcode defines a numeric value that identifies the activity or a point within an activity that the application was performing when it raised the event.</summary>
		/// <returns>Returns a <see cref="T:System.Diagnostics.Eventing.Reader.EventOpcode" /> object.</returns>
		public EventOpcode Opcode
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the task associated with the event. A task identifies a portion of an application or a component that publishes an event. </summary>
		/// <returns>Returns a <see cref="T:System.Diagnostics.Eventing.Reader.EventTask" /> object.</returns>
		public EventTask Task
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the template string for the event. Templates are used to describe data that is used by a provider when an event is published. Templates optionally specify XML that provides the structure of an event. The XML allows values that the event publisher provides to be inserted during the rendering of an event.</summary>
		/// <returns>Returns a string that contains the template for the event.</returns>
		public string Template
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the version of the event that qualifies the event identifier.</summary>
		/// <returns>Returns a byte value.</returns>
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
