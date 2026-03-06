using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains static information and configuration settings for an event log. Many of the configurations settings were defined by the event provider that created the log.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogConfiguration : IDisposable
	{
		/// <summary>Initializes a new <see cref="T:System.Diagnostics.Eventing.Reader.EventLogConfiguration" /> object by specifying the local event log for which to get information and configuration settings. </summary>
		/// <param name="logName">The name of the local event log for which to get information and configuration settings.</param>
		public EventLogConfiguration(string logName)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new <see cref="T:System.Diagnostics.Eventing.Reader.EventLogConfiguration" /> object by specifying the name of the log for which to get information and configuration settings. The log can be on the local computer or a remote computer, based on the event log session specified.</summary>
		/// <param name="logName">The name of the event log for which to get information and configuration settings.</param>
		/// <param name="session">The event log session used to determine the event log service that the specified log belongs to. The session is either connected to the event log service on the local computer or a remote computer.</param>
		[SecurityCritical]
		public EventLogConfiguration(string logName, EventLogSession session)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the flag that indicates if the event log is a classic event log. A classic event log is one that has its events defined in a .mc file instead of a manifest (.xml file) used by the event provider.</summary>
		/// <returns>Returns <see langword="true" /> if the event log is a classic log, and returns <see langword="false" /> if the event log is not a classic log.</returns>
		public bool IsClassicLog
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Gets or sets a Boolean value that determines whether the event log is enabled or disabled. An enabled log is one in which events can be logged, and a disabled log is one in which events cannot be logged.</summary>
		/// <returns>Returns <see langword="true" /> if the log is enabled, and returns <see langword="false" /> if the log is disabled.</returns>
		public bool IsEnabled
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

		/// <summary>Gets or sets the file directory path to the location of the file where the events are stored for the log.</summary>
		/// <returns>Returns a string that contains the path to the event log file.</returns>
		public string LogFilePath
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Gets an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogIsolation" /> value that specifies whether the event log is an application, system, or custom event log. </summary>
		/// <returns>Returns an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogIsolation" /> value.</returns>
		public EventLogIsolation LogIsolation
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return EventLogIsolation.Application;
			}
		}

		/// <summary>Gets or sets an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogMode" /> value that determines how events are handled when the event log becomes full.</summary>
		/// <returns>Returns an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogMode" /> value.</returns>
		public EventLogMode LogMode
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return EventLogMode.Circular;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Gets the name of the event log.</summary>
		/// <returns>Returns a string that contains the name of the event log.</returns>
		public string LogName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogType" /> value that determines the type of the event log.</summary>
		/// <returns>Returns an <see cref="T:System.Diagnostics.Eventing.Reader.EventLogType" /> value.</returns>
		public EventLogType LogType
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return EventLogType.Administrative;
			}
		}

		/// <summary>Gets or sets the maximum size, in bytes, that the event log file is allowed to be. When the file reaches this maximum size, it is considered full.</summary>
		/// <returns>Returns a long value that represents the maximum size, in bytes, that the event log file is allowed to be.</returns>
		public long MaximumSizeInBytes
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Gets the name of the event provider that created this event log.</summary>
		/// <returns>Returns a string that contains the name of the event provider that created this event log.</returns>
		public string OwningProviderName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the size of the buffer that the event provider uses for publishing events to the log.</summary>
		/// <returns>Returns an integer value that can be null.</returns>
		public int? ProviderBufferSize
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the control globally unique identifier (GUID) for the event log if the log is a debug log. If this log is not a debug log, this value will be null. </summary>
		/// <returns>Returns a GUID value or null.</returns>
		public Guid? ProviderControlGuid
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets or sets keyword mask used by the event provider.</summary>
		/// <returns>Returns a long value that can be null if the event provider did not define any keywords.</returns>
		public long? ProviderKeywords
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Gets the maximum latency time used by the event provider when publishing events to the log.</summary>
		/// <returns>Returns an integer value that can be null if no latency time was specified by the event provider.</returns>
		public int? ProviderLatency
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets or sets the maximum event level (which defines the severity of the event) that is allowed to be logged in the event log. This value is defined by the event provider.</summary>
		/// <returns>Returns an integer value that can be null if the maximum event level was not defined in the event provider.</returns>
		public int? ProviderLevel
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Gets the maximum number of buffers used by the event provider to publish events to the event log.</summary>
		/// <returns>Returns an integer value that is the maximum number of buffers used by the event provider to publish events to the event log. This value can be null.</returns>
		public int? ProviderMaximumNumberOfBuffers
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the minimum number of buffers used by the event provider to publish events to the event log.</summary>
		/// <returns>Returns an integer value that is the minimum number of buffers used by the event provider to publish events to the event log. This value can be null.</returns>
		public int? ProviderMinimumNumberOfBuffers
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets an enumerable collection of the names of all the event providers that can publish events to this event log.</summary>
		/// <returns>Returns an enumerable collection of strings that contain the event provider names.</returns>
		public IEnumerable<string> ProviderNames
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets or sets the security descriptor of the event log. The security descriptor defines the users and groups of users that can read and write to the event log.</summary>
		/// <returns>Returns a string that contains the security descriptor for the event log.</returns>
		public string SecurityDescriptor
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
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

		/// <summary>Saves the configuration settings that </summary>
		public void SaveChanges()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
