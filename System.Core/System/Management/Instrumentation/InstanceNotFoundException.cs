using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>The exception thrown to indicate that no instances are returned by a provider.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class InstanceNotFoundException : InstrumentationException
	{
		/// <summary>Initializes a new instance of the InstanceNotFoundException class.</summary>
		public InstanceNotFoundException()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the InstanceNotFoundException class with the specified serialization information and streaming context.</summary>
		/// <param name="info">The SerializationInfo that contains all the data required to serialize the exception.</param>
		/// <param name="context">The StreamingContext that specifies the source and destination of the stream.</param>
		protected InstanceNotFoundException(SerializationInfo info, StreamingContext context)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the InstanceNotFoundException class with its message string set to message.</summary>
		/// <param name="message">A string that contains the error message that explains the reason for the exception.</param>
		public InstanceNotFoundException(string message)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the InstanceNotFoundException class with the specified error message and the inner exception.</summary>
		/// <param name="message">A string that contains the error message that explains the reason for the exception.</param>
		/// <param name="innerException">The Exception that caused the current exception to be thrown.</param>
		public InstanceNotFoundException(string message, Exception innerException)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
