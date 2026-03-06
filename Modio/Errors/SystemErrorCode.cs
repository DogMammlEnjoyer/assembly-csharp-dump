using System;

namespace Modio.Errors
{
	public enum SystemErrorCode : long
	{
		NONE,
		UNKNOWN = -2147483648L,
		UNKNOWN_SYSTEM_ERROR = -2147483586L
	}
}
