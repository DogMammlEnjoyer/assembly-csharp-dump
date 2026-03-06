using System;

namespace Modio.Errors
{
	public enum ArchiveErrorCode : long
	{
		NONE,
		UNKNOWN = -2147483648L,
		INVALID_HEADER = -2147483600L,
		UNSUPPORTED_COMPRESSION
	}
}
