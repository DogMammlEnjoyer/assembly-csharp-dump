using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVRDriverManager
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDriverManager._GetDriverCount GetDriverCount;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDriverManager._GetDriverName GetDriverName;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDriverManager._GetDriverHandle GetDriverHandle;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetDriverCount();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetDriverName(uint nDriver, StringBuilder pchValue, uint unBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetDriverHandle(string pchDriverName);
	}
}
