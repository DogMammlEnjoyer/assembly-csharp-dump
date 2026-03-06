using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing
{
	/// <summary>A listener for <see cref="T:System.Diagnostics.TraceSource" /> that writes events to the ETW subsytem. </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventProviderTraceListener : TraceListener
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.EventProviderTraceListener" /> class using the specified provider identifier.</summary>
		/// <param name="providerId">A unique string <see cref="T:System.Guid" /> that identifies the provider.</param>
		public EventProviderTraceListener(string providerId)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.EventProviderTraceListener" /> class using the specified provider identifier and name of the listener.</summary>
		/// <param name="providerId">A unique string <see cref="T:System.Guid" /> that identifies the provider.</param>
		/// <param name="name">Name of the listener.</param>
		public EventProviderTraceListener(string providerId, string name)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Eventing.EventProviderTraceListener" /> class using the specified provider identifier, name of the listener, and delimiter.</summary>
		/// <param name="providerId">A unique string <see cref="T:System.Guid" /> that identifies the provider.</param>
		/// <param name="name">Name of the listener.</param>
		/// <param name="delimiter">Delimiter used to delimit the event data. (For more details, see the <see cref="P:System.Diagnostics.Eventing.EventProviderTraceListener.Delimiter" /> property.)</param>
		public EventProviderTraceListener(string providerId, string name, string delimiter)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets and sets the delimiter used to delimit the event data that is written to the ETW subsystem.</summary>
		/// <returns>The delimiter used to delimit the event data. The default delimiter is a comma.</returns>
		public string Delimiter
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

		/// <summary>When overridden in a derived class, writes the specified message to the listener you create in the derived class.</summary>
		/// <param name="message">A message to write.</param>
		public sealed override void Write(string message)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.</summary>
		/// <param name="message">A message to write. </param>
		public sealed override void WriteLine(string message)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
