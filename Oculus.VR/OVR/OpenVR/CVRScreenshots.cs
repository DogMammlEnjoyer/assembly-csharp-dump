using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRScreenshots
	{
		internal CVRScreenshots(IntPtr pInterface)
		{
			this.FnTable = (IVRScreenshots)Marshal.PtrToStructure(pInterface, typeof(IVRScreenshots));
		}

		public EVRScreenshotError RequestScreenshot(ref uint pOutScreenshotHandle, EVRScreenshotType type, string pchPreviewFilename, string pchVRFilename)
		{
			pOutScreenshotHandle = 0U;
			return this.FnTable.RequestScreenshot(ref pOutScreenshotHandle, type, pchPreviewFilename, pchVRFilename);
		}

		public EVRScreenshotError HookScreenshot(EVRScreenshotType[] pSupportedTypes)
		{
			return this.FnTable.HookScreenshot(pSupportedTypes, pSupportedTypes.Length);
		}

		public EVRScreenshotType GetScreenshotPropertyType(uint screenshotHandle, ref EVRScreenshotError pError)
		{
			return this.FnTable.GetScreenshotPropertyType(screenshotHandle, ref pError);
		}

		public uint GetScreenshotPropertyFilename(uint screenshotHandle, EVRScreenshotPropertyFilenames filenameType, StringBuilder pchFilename, uint cchFilename, ref EVRScreenshotError pError)
		{
			return this.FnTable.GetScreenshotPropertyFilename(screenshotHandle, filenameType, pchFilename, cchFilename, ref pError);
		}

		public EVRScreenshotError UpdateScreenshotProgress(uint screenshotHandle, float flProgress)
		{
			return this.FnTable.UpdateScreenshotProgress(screenshotHandle, flProgress);
		}

		public EVRScreenshotError TakeStereoScreenshot(ref uint pOutScreenshotHandle, string pchPreviewFilename, string pchVRFilename)
		{
			pOutScreenshotHandle = 0U;
			return this.FnTable.TakeStereoScreenshot(ref pOutScreenshotHandle, pchPreviewFilename, pchVRFilename);
		}

		public EVRScreenshotError SubmitScreenshot(uint screenshotHandle, EVRScreenshotType type, string pchSourcePreviewFilename, string pchSourceVRFilename)
		{
			return this.FnTable.SubmitScreenshot(screenshotHandle, type, pchSourcePreviewFilename, pchSourceVRFilename);
		}

		private IVRScreenshots FnTable;
	}
}
