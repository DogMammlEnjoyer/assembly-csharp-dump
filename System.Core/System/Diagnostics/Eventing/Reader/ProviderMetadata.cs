using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Contains static information about an event provider, such as the name and id of the provider, and the collection of events defined in the provider.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class ProviderMetadata : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.Reader.ProviderMetadata" /> class by specifying the name of the provider that you want to retrieve information about.</summary>
		/// <param name="providerName">The name of the event provider that you want to retrieve information about.</param>
		public ProviderMetadata(string providerName)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.Reader.ProviderMetadata" /> class by specifying the name of the provider that you want to retrieve information about, the event log service that the provider is registered with, and the language that you want to return the information in.</summary>
		/// <param name="providerName">The name of the event provider that you want to retrieve information about.</param>
		/// <param name="session">The <see cref="T:System.Diagnostics.Eventing.Reader.EventLogSession" /> object that specifies whether to get the provider information from a provider on the local computer or a provider on a remote computer.</param>
		/// <param name="targetCultureInfo">The culture that specifies the language that the information should be returned in.</param>
		public ProviderMetadata(string providerName, EventLogSession session, CultureInfo targetCultureInfo)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the localized name of the event provider.</summary>
		/// <returns>Returns a string that contains the localized name of the event provider.</returns>
		public string DisplayName
		{
			[SecurityCritical]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventMetadata" /> objects, each of which represents an event that is defined in the provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventMetadata" /> objects.</returns>
		public IEnumerable<EventMetadata> Events
		{
			[SecurityCritical]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the base of the URL used to form help requests for the events in this event provider.</summary>
		/// <returns>Returns a <see cref="T:System.Uri" /> value.</returns>
		public Uri HelpLink
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the globally unique identifier (GUID) for the event provider.</summary>
		/// <returns>Returns the GUID value for the event provider.</returns>
		public Guid Id
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(Guid);
			}
		}

		/// <summary>Gets an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventKeyword" /> objects, each of which represent an event keyword that is defined in the event provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventKeyword" /> objects.</returns>
		public IList<EventKeyword> Keywords
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventLevel" /> objects, each of which represent a level that is defined in the event provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventLevel" /> objects.</returns>
		public IList<EventLevel> Levels
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventLogLink" /> objects, each of which represent a link to an event log that is used by the event provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventLogLink" /> objects.</returns>
		public IList<EventLogLink> LogLinks
		{
			[SecurityCritical]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the path of the file that contains the message table resource that has the strings associated with the provider metadata.</summary>
		/// <returns>Returns a string that contains the path of the provider message file.</returns>
		public string MessageFilePath
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the unique name of the event provider.</summary>
		/// <returns>Returns a string that contains the unique name of the event provider.</returns>
		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventOpcode" /> objects, each of which represent an opcode that is defined in the event provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventOpcode" /> objects.</returns>
		public IList<EventOpcode> Opcodes
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets the path of the file that contains the message table resource that has the strings used for parameter substitutions in event descriptions.</summary>
		/// <returns>Returns a string that contains the path of the file that contains the message table resource that has the strings used for parameter substitutions in event descriptions.</returns>
		public string ParameterFilePath
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the path to the file that contains the metadata associated with the provider.</summary>
		/// <returns>Returns a string that contains the path to the file that contains the metadata associated with the provider.</returns>
		public string ResourceFilePath
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventTask" /> objects, each of which represent a task that is defined in the event provider.</summary>
		/// <returns>Returns an enumerable collection of <see cref="T:System.Diagnostics.Eventing.Reader.EventTask" /> objects.</returns>
		public IList<EventTask> Tasks
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
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
