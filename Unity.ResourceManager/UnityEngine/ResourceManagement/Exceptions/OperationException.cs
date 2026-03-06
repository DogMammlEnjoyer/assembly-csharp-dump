using System;

namespace UnityEngine.ResourceManagement.Exceptions
{
	public class OperationException : Exception
	{
		public OperationException(string message, Exception innerException = null) : base(message, innerException)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} : {1}\n{2}", base.GetType().Name, base.Message, base.InnerException);
		}
	}
}
