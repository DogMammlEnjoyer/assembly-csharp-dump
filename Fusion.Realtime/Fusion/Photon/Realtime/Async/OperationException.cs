using System;

namespace Fusion.Photon.Realtime.Async
{
	internal class OperationException : Exception
	{
		public OperationException(short errorCode, string message) : base(string.Format("{0} (ErrorCode: {1})", message, errorCode))
		{
			this.ErrorCode = errorCode;
		}

		public short ErrorCode;
	}
}
