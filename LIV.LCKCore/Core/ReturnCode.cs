using System;

namespace Liv.Lck.Core
{
	internal enum ReturnCode : uint
	{
		Ok,
		Error,
		Panic,
		InvalidArgument,
		BackendUnavailable,
		Uninitialized,
		BackendDataParsingError,
		BackendClientError,
		UserNotLoggedIn,
		NullPointer,
		LoginAttemptExpired,
		Fatal,
		RateLimiterBackoff
	}
}
