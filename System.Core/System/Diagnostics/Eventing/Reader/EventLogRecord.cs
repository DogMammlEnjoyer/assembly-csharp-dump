using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains the properties of an event instance for an event that is received from an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogReader" /> object. The event properties provide information about the event such as the name of the computer where the event was logged and the time that the event was created.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogRecord : EventRecord
	{
		internal EventLogRecord()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the globally unique identifier (GUID) for the activity in process for which the event is involved. This allows consumers to group related activities.</summary>
		/// <returns>Returns a GUID value.</returns>
		public override Guid? ActivityId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a placeholder (bookmark) that corresponds to this event. This can be used as a placeholder in a stream of events.</summary>
		/// <returns>Returns a <see cref="T:System.Diagnostics.Eventing.Reader.EventBookmark" /> object.</returns>
		public override EventBookmark Bookmark
		{
			[SecuritySafeCritical]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the name of the event log or the event log file in which the event is stored.</summary>
		/// <returns>Returns a string that contains the name of the event log or the event log file in which the event is stored.</returns>
		public string ContainerLog
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the identifier for this event. All events with this identifier value represent the same type of event.</summary>
		/// <returns>Returns an integer value. This value can be null.</returns>
		public override int Id
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the keyword mask of the event. Get the value of the <see cref="P:System.Diagnostics.Eventing.Reader.EventLogRecord.KeywordsDisplayNames" /> property to get the name of the keywords used in this mask.</summary>
		/// <returns>Returns a long value. This value can be null.</returns>
		public override long? Keywords
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the display names of the keywords used in the keyword mask for this event.</summary>
		/// <returns>Returns an enumerable collection of strings that contain the display names of the keywords used in the keyword mask for this event.</returns>
		public override IEnumerable<string> KeywordsDisplayNames
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the level of the event. The level signifies the severity of the event. For the name of the level, get the value of the <see cref="P:System.Diagnostics.Eventing.Reader.EventLogRecord.LevelDisplayName" /> property.</summary>
		/// <returns>Returns a byte value. This value can be null.</returns>
		public override byte? Level
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the display name of the level for this event.</summary>
		/// <returns>Returns a string that contains the display name of the level for this event.</returns>
		public override string LevelDisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the name of the event log where this event is logged.</summary>
		/// <returns>Returns a string that contains a name of the event log that contains this event.</returns>
		public override string LogName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the name of the computer on which this event was logged.</summary>
		/// <returns>Returns a string that contains the name of the computer on which this event was logged.</returns>
		public override string MachineName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a list of query identifiers that this event matches. This event matches a query if the query would return this event.</summary>
		/// <returns>Returns an enumerable collection of integer values.</returns>
		public IEnumerable<int> MatchedQueryIds
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the opcode of the event. The opcode defines a numeric value that identifies the activity or a point within an activity that the application was performing when it raised the event. For the name of the opcode, get the value of the <see cref="P:System.Diagnostics.Eventing.Reader.EventLogRecord.OpcodeDisplayName" /> property.</summary>
		/// <returns>Returns a short value. This value can be null.</returns>
		public override short? Opcode
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the display name of the opcode for this event.</summary>
		/// <returns>Returns a string that contains the display name of the opcode for this event.</returns>
		public override string OpcodeDisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the process identifier for the event provider that logged this event.</summary>
		/// <returns>Returns an integer value. This value can be null.</returns>
		public override int? ProcessId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the user-supplied properties of the event.</summary>
		/// <returns>Returns a list of <see cref="T:System.Diagnostics.Eventing.Reader.EventProperty" /> objects.</returns>
		public override IList<EventProperty> Properties
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the globally unique identifier (GUID) of the event provider that published this event.</summary>
		/// <returns>Returns a GUID value. This value can be null.</returns>
		public override Guid? ProviderId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the name of the event provider that published this event.</summary>
		/// <returns>Returns a string that contains the name of the event provider that published this event.</returns>
		public override string ProviderName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets qualifier numbers that are used for event identification.</summary>
		/// <returns>Returns an integer value. This value can be null.</returns>
		public override int? Qualifiers
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the event record identifier of the event in the log.</summary>
		/// <returns>Returns a long value. This value can be null.</returns>
		public override long? RecordId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a globally unique identifier (GUID) for a related activity in a process for which an event is involved.</summary>
		/// <returns>Returns a GUID value. This value can be null.</returns>
		public override Guid? RelatedActivityId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets a task identifier for a portion of an application or a component that publishes an event. A task is a 16-bit value with 16 top values reserved. This type allows any value between 0x0000 and 0xffef to be used. For the name of the task, get the value of the <see cref="P:System.Diagnostics.Eventing.Reader.EventLogRecord.TaskDisplayName" /> property.</summary>
		/// <returns>Returns an integer value. This value can be null.</returns>
		public override int? Task
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the display name of the task for the event.</summary>
		/// <returns>Returns a string that contains the display name of the task for the event.</returns>
		public override string TaskDisplayName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the thread identifier for the thread that the event provider is running in.</summary>
		/// <returns>Returns an integer value. This value can be null.</returns>
		public override int? ThreadId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the time, in <see cref="T:System.DateTime" /> format, that the event was created.</summary>
		/// <returns>Returns a <see cref="T:System.DateTime" /> value. The value can be null.</returns>
		public override DateTime? TimeCreated
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the security descriptor of the user whose context is used to publish the event.</summary>
		/// <returns>Returns a <see cref="T:System.Security.Principal.SecurityIdentifier" /> value.</returns>
		public override SecurityIdentifier UserId
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the version number for the event.</summary>
		/// <returns>Returns a byte value. This value can be null.</returns>
		public override byte? Version
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Releases the unmanaged resources used by this object, and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///       <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		[SecuritySafeCritical]
		protected override void Dispose(bool disposing)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the event message in the current locale.</summary>
		/// <returns>Returns a string that contains the event message in the current locale.</returns>
		public override string FormatDescription()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Gets the event message, replacing variables in the message with the specified values.</summary>
		/// <param name="values">The values used to replace variables in the event message. Variables are represented by %n, where n is a number.</param>
		/// <returns>Returns a string that contains the event message in the current locale.</returns>
		public override string FormatDescription(IEnumerable<object> values)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Gets the enumeration of the values of the user-supplied event properties, or the results of XPath-based data if the event has XML representation.</summary>
		/// <param name="propertySelector">Selects the property values to return.</param>
		/// <returns>Returns a list of objects.</returns>
		public IList<object> GetPropertyValues(EventLogPropertySelector propertySelector)
		{
			ThrowStub.ThrowNotSupportedException();
			return 0;
		}

		/// <summary>Gets the XML representation of the event. All of the event properties are represented in the event's XML. The XML conforms to the event schema.</summary>
		/// <returns>Returns a string that contains the XML representation of the event.</returns>
		[SecuritySafeCritical]
		public override string ToXml()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
