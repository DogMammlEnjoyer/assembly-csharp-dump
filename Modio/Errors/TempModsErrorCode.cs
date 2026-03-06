using System;

namespace Modio.Errors
{
	public enum TempModsErrorCode : long
	{
		NONE,
		UNKNOWN = -2147483648L,
		CANT_INSTALL_TAINTED_MOD = -2147483550L
	}
}
