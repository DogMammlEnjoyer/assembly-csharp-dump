using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVROverlayView
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlayView._AcquireOverlayView AcquireOverlayView;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlayView._ReleaseOverlayView ReleaseOverlayView;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlayView._PostOverlayEvent PostOverlayEvent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVROverlayView._IsViewingPermitted IsViewingPermitted;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _AcquireOverlayView(ulong ulOverlayHandle, ref VRNativeDevice_t pNativeDevice, ref VROverlayView_t pOverlayView, uint unOverlayViewSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVROverlayError _ReleaseOverlayView(ref VROverlayView_t pOverlayView);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _PostOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pvrEvent);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsViewingPermitted(ulong ulOverlayHandle);
	}
}
