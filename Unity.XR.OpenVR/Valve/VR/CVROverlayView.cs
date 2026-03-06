using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVROverlayView
	{
		internal CVROverlayView(IntPtr pInterface)
		{
			this.FnTable = (IVROverlayView)Marshal.PtrToStructure(pInterface, typeof(IVROverlayView));
		}

		public EVROverlayError AcquireOverlayView(ulong ulOverlayHandle, ref VRNativeDevice_t pNativeDevice, ref VROverlayView_t pOverlayView, uint unOverlayViewSize)
		{
			return this.FnTable.AcquireOverlayView(ulOverlayHandle, ref pNativeDevice, ref pOverlayView, unOverlayViewSize);
		}

		public EVROverlayError ReleaseOverlayView(ref VROverlayView_t pOverlayView)
		{
			return this.FnTable.ReleaseOverlayView(ref pOverlayView);
		}

		public void PostOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pvrEvent)
		{
			this.FnTable.PostOverlayEvent(ulOverlayHandle, ref pvrEvent);
		}

		public bool IsViewingPermitted(ulong ulOverlayHandle)
		{
			return this.FnTable.IsViewingPermitted(ulOverlayHandle);
		}

		private IVROverlayView FnTable;
	}
}
