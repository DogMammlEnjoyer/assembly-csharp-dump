using System;

namespace Modio.Errors
{
	public enum ModValidationErrorCode : long
	{
		NONE,
		UNKNOWN = -2147483648L,
		NO_FILES_FOUND_FOR_MOD = -2147483563L,
		MOD_DIRECTORY_NOT_FOUND,
		MD5DOES_NOT_MATCH
	}
}
