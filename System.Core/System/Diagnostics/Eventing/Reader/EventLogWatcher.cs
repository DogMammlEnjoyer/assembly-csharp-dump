using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Allows you to subscribe to incoming events. Each time a desired event is published to an event log, the <see cref="E:System.Diagnostics.Eventing.Reader.EventLogWatcher.EventRecordWritten" /> event is raised, and the method that handles this event will be executed. </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogWatcher : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.Reader.EventLogWatcher" /> class by specifying an event query.</summary>
		/// <param name="eventQuery">Specifies a query for the event subscription. When an event is logged that matches the criteria expressed in the query, then the <see cref="E:System.Diagnostics.Eventing.Reader.EventLogWatcher.EventRecordWritten" /> event is raised. </param>
		public EventLogWatcher(EventLogQuery eventQuery)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.Reader.EventLogWatcher" /> class by specifying an event query and a bookmark that is used as starting position for the query.</summary>
		/// <param name="eventQuery">Specifies a query for the event subscription. When an event is logged that matches the criteria expressed in the query, then the <see cref="E:System.Diagnostics.Eventing.Reader.EventLogWatcher.EventRecordWritten" /> event is raised.</param>
		/// <param name="bookmark">The bookmark (placeholder) used as a starting position in the event log or stream of events. Only events that have been logged after the bookmark event will be returned by the query.</param>
		public EventLogWatcher(EventLogQuery eventQuery, EventBookmark bookmark)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.Reader.EventLogWatcher" /> class by specifying an event query, a bookmark that is used as starting position for the query, and a Boolean value that determines whether to read the events that already exist in the event log.</summary>
		/// <param name="eventQuery">Specifies a query for the event subscription. When an event is logged that matches the criteria expressed in the query, then the <see cref="E:System.Diagnostics.Eventing.Reader.EventLogWatcher.EventRecordWritten" /> event is raised.</param>
		/// <param name="bookmark">The bookmark (placeholder) used as a starting position in the event log or stream of events. Only events that have been logged after the bookmark event will be returned by the query.</param>
		/// <param name="readExistingEvents">A Boolean value that determines whether to read the events that already exist in the event log. If this value is <see langword="true" />, then the existing events are read and if this value is <see langword="false" />, then the existing events are not read.</param>
		public EventLogWatcher(EventLogQuery eventQuery, EventBookmark bookmark, bool readExistingEvents)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.Reader.EventLogWatcher" /> class by specifying the name or path to an event log.</summary>
		/// <param name="path">The path or name of the event log monitor for events. If any event is logged in this event log, then the <see cref="E:System.Diagnostics.Eventing.Reader.EventLogWatcher.EventRecordWritten" /> event is raised.</param>
		public EventLogWatcher(string path)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Determines whether this object starts delivering events to the event delegate.</summary>
		/// <returns>Returns <see langword="true" /> when this object can deliver events to the event delegate, and returns <see langword="false" /> when this object has stopped delivery.</returns>
		public bool Enabled
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Allows setting a delegate (event handler method) that gets called every time an event is published that matches the criteria specified in the event query for this object. </summary>
		public event EventHandler<EventRecordWrittenEventArgs> EventRecordWritten
		{
			add
			{
				ThrowStub.ThrowNotSupportedException();
			}
			remove
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Releases all the resources used by this object.</summary>
		public void Dispose()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Releases the unmanaged resources used by this object, and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///       <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
