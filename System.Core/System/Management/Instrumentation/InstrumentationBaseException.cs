using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>Represents the base provider-related exception.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class InstrumentationBaseException : Exception
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Management.Instrumentation.InstrumentationBaseException" />. class. This is the default constructor.</summary>
		public InstrumentationBaseException()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Management.Instrumentation.InstrumentationBaseException" /> class with serialization information.</summary>
		/// <param name="info">The data that is required to serialize or deserialize an object.</param>
		/// <param name="context">Description of the source and destination of the specified serialized stream.</param>
		protected InstrumentationBaseException(SerializationInfo info, StreamingContext context)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Management.Instrumentation.InstrumentationBaseException" /> class with a message that describes the exception.</summary>
		/// <param name="message">Message that describes the exception.</param>
		public InstrumentationBaseException(string message)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new <see cref="T:System.Management.Instrumentation.InstrumentationBaseException" /> class with the specified string and exception.</summary>
		/// <param name="message">Message that describes the exception.</param>
		/// <param name="innerException">The Exception instance that caused the current exception.</param>
		public InstrumentationBaseException(string message, Exception innerException)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
