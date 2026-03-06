using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public struct IVRDebug
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDebug._EmitVrProfilerEvent EmitVrProfilerEvent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDebug._BeginVrProfilerEvent BeginVrProfilerEvent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDebug._FinishVrProfilerEvent FinishVrProfilerEvent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRDebug._DriverDebugRequest DriverDebugRequest;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRDebugError _EmitVrProfilerEvent(IntPtr pchMessage);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRDebugError _BeginVrProfilerEvent(ref ulong pHandleOut);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRDebugError _FinishVrProfilerEvent(ulong hHandle, IntPtr pchMessage);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _DriverDebugRequest(uint unDeviceIndex, IntPtr pchRequest, StringBuilder pchResponseBuffer, uint unResponseBufferSize);
	}
}
