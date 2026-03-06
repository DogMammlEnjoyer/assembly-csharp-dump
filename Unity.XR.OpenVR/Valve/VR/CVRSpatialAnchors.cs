using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRSpatialAnchors
	{
		internal CVRSpatialAnchors(IntPtr pInterface)
		{
			this.FnTable = (IVRSpatialAnchors)Marshal.PtrToStructure(pInterface, typeof(IVRSpatialAnchors));
		}

		public EVRSpatialAnchorError CreateSpatialAnchorFromDescriptor(string pchDescriptor, ref uint pHandleOut)
		{
			IntPtr intPtr = Utils.ToUtf8(pchDescriptor);
			pHandleOut = 0U;
			EVRSpatialAnchorError result = this.FnTable.CreateSpatialAnchorFromDescriptor(intPtr, ref pHandleOut);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRSpatialAnchorError CreateSpatialAnchorFromPose(uint unDeviceIndex, ETrackingUniverseOrigin eOrigin, ref SpatialAnchorPose_t pPose, ref uint pHandleOut)
		{
			pHandleOut = 0U;
			return this.FnTable.CreateSpatialAnchorFromPose(unDeviceIndex, eOrigin, ref pPose, ref pHandleOut);
		}

		public EVRSpatialAnchorError GetSpatialAnchorPose(uint unHandle, ETrackingUniverseOrigin eOrigin, ref SpatialAnchorPose_t pPoseOut)
		{
			return this.FnTable.GetSpatialAnchorPose(unHandle, eOrigin, ref pPoseOut);
		}

		public EVRSpatialAnchorError GetSpatialAnchorDescriptor(uint unHandle, StringBuilder pchDescriptorOut, ref uint punDescriptorBufferLenInOut)
		{
			punDescriptorBufferLenInOut = 0U;
			return this.FnTable.GetSpatialAnchorDescriptor(unHandle, pchDescriptorOut, ref punDescriptorBufferLenInOut);
		}

		private IVRSpatialAnchors FnTable;
	}
}
