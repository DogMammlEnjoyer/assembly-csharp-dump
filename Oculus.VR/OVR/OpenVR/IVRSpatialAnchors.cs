using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVRSpatialAnchors
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSpatialAnchors._CreateSpatialAnchorFromDescriptor CreateSpatialAnchorFromDescriptor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSpatialAnchors._CreateSpatialAnchorFromPose CreateSpatialAnchorFromPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSpatialAnchors._GetSpatialAnchorPose GetSpatialAnchorPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSpatialAnchors._GetSpatialAnchorDescriptor GetSpatialAnchorDescriptor;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRSpatialAnchorError _CreateSpatialAnchorFromDescriptor(string pchDescriptor, ref uint pHandleOut);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRSpatialAnchorError _CreateSpatialAnchorFromPose(uint unDeviceIndex, ETrackingUniverseOrigin eOrigin, ref SpatialAnchorPose_t pPose, ref uint pHandleOut);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRSpatialAnchorError _GetSpatialAnchorPose(uint unHandle, ETrackingUniverseOrigin eOrigin, ref SpatialAnchorPose_t pPoseOut);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRSpatialAnchorError _GetSpatialAnchorDescriptor(uint unHandle, StringBuilder pchDescriptorOut, ref uint punDescriptorBufferLenInOut);
	}
}
