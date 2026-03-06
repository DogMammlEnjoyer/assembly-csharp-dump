using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRHeadsetView
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._SetHeadsetViewSize SetHeadsetViewSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._GetHeadsetViewSize GetHeadsetViewSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._SetHeadsetViewMode SetHeadsetViewMode;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._GetHeadsetViewMode GetHeadsetViewMode;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._SetHeadsetViewCropped SetHeadsetViewCropped;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._GetHeadsetViewCropped GetHeadsetViewCropped;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._GetHeadsetViewAspectRatio GetHeadsetViewAspectRatio;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._SetHeadsetViewBlendRange SetHeadsetViewBlendRange;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRHeadsetView._GetHeadsetViewBlendRange GetHeadsetViewBlendRange;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetHeadsetViewSize(uint nWidth, uint nHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetHeadsetViewSize(ref uint pnWidth, ref uint pnHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetHeadsetViewMode(uint eHeadsetViewMode);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetHeadsetViewMode();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetHeadsetViewCropped(bool bCropped);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetHeadsetViewCropped();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetHeadsetViewAspectRatio();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetHeadsetViewBlendRange(float flStartPct, float flEndPct);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetHeadsetViewBlendRange(ref float pStartPct, ref float pEndPct);
	}
}
