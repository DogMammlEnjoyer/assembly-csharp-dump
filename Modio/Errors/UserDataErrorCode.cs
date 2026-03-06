using System;

namespace Modio.Errors
{
	public enum UserDataErrorCode : long
	{
		NONE,
		UNKNOWN = -2147483648L,
		INVALID_USER = -2147483602L,
		BLOB_MISSING
	}
}
