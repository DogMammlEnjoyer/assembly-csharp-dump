using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace System.Data
{
	internal static class Win32NativeMethods
	{
		internal static bool IsTokenRestrictedWrapper(IntPtr token)
		{
			bool result;
			uint num = SNINativeMethodWrapper.UnmanagedIsTokenRestricted(token, out result);
			if (num != 0U)
			{
				Marshal.ThrowExceptionForHR((int)num);
			}
			return result;
		}
	}
}
