using System;

namespace Modio.Errors
{
	public class RateLimitError : Error
	{
		internal RateLimitError(RateLimitErrorCode code, int retryAfterSeconds) : base((ErrorCode)code)
		{
			this.RetryAfterSeconds = retryAfterSeconds;
		}

		public readonly int RetryAfterSeconds;
	}
}
