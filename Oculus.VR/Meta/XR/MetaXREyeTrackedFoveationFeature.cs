using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.Features;

namespace Meta.XR
{
	public class MetaXREyeTrackedFoveationFeature : OpenXRFeature
	{
		protected override void OnSessionCreate(ulong xrSession)
		{
			MetaXREyeTrackedFoveationFeature._xrSession = xrSession;
		}

		public static bool eyeTrackedFoveatedRenderingEnabled
		{
			get
			{
				bool result;
				MetaXREyeTrackedFoveationFeature.MetaGetFoveationEyeTracked(out result);
				return result;
			}
			set
			{
				MetaXREyeTrackedFoveationFeature.MetaSetFoveationEyeTracked(MetaXREyeTrackedFoveationFeature._xrSession, value);
			}
		}

		public static bool eyeTrackedFoveatedRenderingSupported
		{
			get
			{
				bool result;
				MetaXREyeTrackedFoveationFeature.MetaGetEyeTrackedFoveationSupported(out result);
				return result;
			}
		}

		[DllImport("UnityOpenXR")]
		private static extern void MetaSetFoveationEyeTracked(ulong session, bool isEyeTracked);

		[DllImport("UnityOpenXR")]
		private static extern void MetaGetFoveationEyeTracked(out bool isEyeTracked);

		[DllImport("UnityOpenXR")]
		private static extern void MetaGetEyeTrackedFoveationSupported(out bool supported);

		public const string extensionName = "XR_META_foveation_eye_tracked XR_FB_eye_tracking_social XR_META_vulkan_swapchain_create_info";

		public const string featureId = "com.meta.openxr.feature.eyetrackedfoveation";

		private static ulong _xrSession;
	}
}
