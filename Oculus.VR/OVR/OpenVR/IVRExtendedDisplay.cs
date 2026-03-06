using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public struct IVRExtendedDisplay
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRExtendedDisplay._GetWindowBounds GetWindowBounds;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRExtendedDisplay._GetEyeOutputViewport GetEyeOutputViewport;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRExtendedDisplay._GetDXGIOutputInfo GetDXGIOutputInfo;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetWindowBounds(ref int pnX, ref int pnY, ref uint pnWidth, ref uint pnHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetEyeOutputViewport(EVREye eEye, ref uint pnX, ref uint pnY, ref uint pnWidth, ref uint pnHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetDXGIOutputInfo(ref int pnAdapterIndex, ref int pnAdapterOutputIndex);
	}
}
