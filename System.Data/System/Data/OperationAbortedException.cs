using System;
using System.Runtime.Serialization;
using Unity;

namespace System.Data
{
	/// <summary>This exception is thrown when an ongoing operation is aborted by the user.</summary>
	[Serializable]
	public sealed class OperationAbortedException : SystemException
	{
		private OperationAbortedException(string message, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2146232010;
		}

		private OperationAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal static OperationAbortedException Aborted(Exception inner)
		{
			OperationAbortedException result;
			if (inner == null)
			{
				result = new OperationAbortedException(SR.GetString("Operation aborted."), null);
			}
			else
			{
				result = new OperationAbortedException(SR.GetString("Operation aborted due to an exception (see InnerException for details)."), inner);
			}
			return result;
		}

		internal OperationAbortedException()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
