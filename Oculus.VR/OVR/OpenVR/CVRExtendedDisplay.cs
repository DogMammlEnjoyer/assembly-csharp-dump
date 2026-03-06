using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public class CVRExtendedDisplay
	{
		internal CVRExtendedDisplay(IntPtr pInterface)
		{
			this.FnTable = (IVRExtendedDisplay)Marshal.PtrToStructure(pInterface, typeof(IVRExtendedDisplay));
		}

		public void GetWindowBounds(ref int pnX, ref int pnY, ref uint pnWidth, ref uint pnHeight)
		{
			pnX = 0;
			pnY = 0;
			pnWidth = 0U;
			pnHeight = 0U;
			this.FnTable.GetWindowBounds(ref pnX, ref pnY, ref pnWidth, ref pnHeight);
		}

		public void GetEyeOutputViewport(EVREye eEye, ref uint pnX, ref uint pnY, ref uint pnWidth, ref uint pnHeight)
		{
			pnX = 0U;
			pnY = 0U;
			pnWidth = 0U;
			pnHeight = 0U;
			this.FnTable.GetEyeOutputViewport(eEye, ref pnX, ref pnY, ref pnWidth, ref pnHeight);
		}

		public void GetDXGIOutputInfo(ref int pnAdapterIndex, ref int pnAdapterOutputIndex)
		{
			pnAdapterIndex = 0;
			pnAdapterOutputIndex = 0;
			this.FnTable.GetDXGIOutputInfo(ref pnAdapterIndex, ref pnAdapterOutputIndex);
		}

		private IVRExtendedDisplay FnTable;
	}
}
