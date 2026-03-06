using System;
using System.Runtime.Serialization;

namespace UnityEngine.ResourceManagement.Exceptions
{
	public class ResourceManagerException : Exception
	{
		public ResourceManagerException()
		{
		}

		public ResourceManagerException(string message) : base(message)
		{
		}

		public ResourceManagerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ResourceManagerException(SerializationInfo message, StreamingContext context) : base(message, context)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} : {1}\n{2}", base.GetType().Name, base.Message, base.InnerException);
		}
	}
}
