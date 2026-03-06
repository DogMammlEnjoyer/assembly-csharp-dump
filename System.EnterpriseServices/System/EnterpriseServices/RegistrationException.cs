using System;
using System.Runtime.Serialization;

namespace System.EnterpriseServices
{
	/// <summary>The exception that is thrown when a registration error is detected.</summary>
	[Serializable]
	public sealed class RegistrationException : SystemException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.RegistrationException" /> class with a specified error message.</summary>
		/// <param name="msg">The message displayed to the client when the exception is thrown.</param>
		[MonoTODO]
		public RegistrationException(string msg) : base(msg)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.RegistrationException" /> class.</summary>
		public RegistrationException() : this("Registration error")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.RegistrationException" /> class with a specified error message and nested exception.</summary>
		/// <param name="msg">The message displayed to the client when the exception is thrown.</param>
		/// <param name="inner">The nested exception.</param>
		public RegistrationException(string msg, Exception inner) : base(msg, inner)
		{
		}

		/// <summary>Gets an array of <see cref="T:System.EnterpriseServices.RegistrationErrorInfo" /> objects that describe registration errors.</summary>
		/// <returns>The array of <see cref="T:System.EnterpriseServices.RegistrationErrorInfo" /> objects.</returns>
		public RegistrationErrorInfo[] ErrorInfo
		{
			get
			{
				return this.errorInfo;
			}
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the error information in <see cref="T:System.EnterpriseServices.RegistrationErrorInfo" />.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains serialized object data.</param>
		/// <param name="ctx">The contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="info" /> parameter is <see langword="null" />.</exception>
		[MonoTODO]
		public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
		{
			throw new NotImplementedException();
		}

		private RegistrationErrorInfo[] errorInfo;
	}
}
