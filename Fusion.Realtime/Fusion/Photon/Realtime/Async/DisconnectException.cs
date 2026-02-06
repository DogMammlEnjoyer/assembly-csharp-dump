using System;

namespace Fusion.Photon.Realtime.Async
{
	internal class DisconnectException : Exception
	{
		public DisconnectException(DisconnectCause cause) : base(cause.ToString())
		{
			this.Cause = cause;
		}

		public DisconnectCause Cause;
	}
}
