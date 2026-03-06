using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVRScreenshots
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._RequestScreenshot RequestScreenshot;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._HookScreenshot HookScreenshot;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._GetScreenshotPropertyType GetScreenshotPropertyType;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._GetScreenshotPropertyFilename GetScreenshotPropertyFilename;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._UpdateScreenshotProgress UpdateScreenshotProgress;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._TakeStereoScreenshot TakeStereoScreenshot;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRScreenshots._SubmitScreenshot SubmitScreenshot;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRScreenshotError _RequestScreenshot(ref uint pOutScreenshotHandle, EVRScreenshotType type, string pchPreviewFilename, string pchVRFilename);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRScreenshotError _HookScreenshot([In] [Out] EVRScreenshotType[] pSupportedTypes, int numTypes);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRScreenshotType _GetScreenshotPropertyType(uint screenshotHandle, ref EVRScreenshotError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetScreenshotPropertyFilename(uint screenshotHandle, EVRScreenshotPropertyFilenames filenameType, StringBuilder pchFilename, uint cchFilename, ref EVRScreenshotError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRScreenshotError _UpdateScreenshotProgress(uint screenshotHandle, float flProgress);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRScreenshotError _TakeStereoScreenshot(ref uint pOutScreenshotHandle, string pchPreviewFilename, string pchVRFilename);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRScreenshotError _SubmitScreenshot(uint screenshotHandle, EVRScreenshotType type, string pchSourcePreviewFilename, string pchSourceVRFilename);
	}
}
