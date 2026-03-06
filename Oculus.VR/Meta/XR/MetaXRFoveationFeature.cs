using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.Features;

namespace Meta.XR
{
	public class MetaXRFoveationFeature : OpenXRFeature
	{
		protected override void OnSessionCreate(ulong xrSession)
		{
			MetaXRFoveationFeature._xrSession = xrSession;
		}

		public static OVRManager.FoveatedRenderingLevel foveatedRenderingLevel
		{
			get
			{
				uint result;
				MetaXRFoveationFeature.FBGetFoveationLevel(out result);
				return (OVRManager.FoveatedRenderingLevel)result;
			}
			set
			{
				if (value == OVRManager.FoveatedRenderingLevel.HighTop)
				{
					MetaXRFoveationFeature._foveatedRenderingLevel = 3U;
				}
				else
				{
					MetaXRFoveationFeature._foveatedRenderingLevel = (uint)value;
				}
				MetaXRFoveationFeature.FBSetFoveationLevel(MetaXRFoveationFeature._xrSession, MetaXRFoveationFeature._foveatedRenderingLevel, 0f, MetaXRFoveationFeature._useDynamicFoveation);
			}
		}

		public static bool useDynamicFoveatedRendering
		{
			get
			{
				uint num;
				MetaXRFoveationFeature.FBGetFoveationLevel(out num);
				return num > 0U;
			}
			set
			{
				if (value)
				{
					MetaXRFoveationFeature._useDynamicFoveation = 1U;
				}
				else
				{
					MetaXRFoveationFeature._useDynamicFoveation = 0U;
				}
				MetaXRFoveationFeature.FBSetFoveationLevel(MetaXRFoveationFeature._xrSession, MetaXRFoveationFeature._foveatedRenderingLevel, 0f, MetaXRFoveationFeature._useDynamicFoveation);
			}
		}

		[DllImport("UnityOpenXR")]
		private static extern void FBSetFoveationLevel(ulong session, uint level, float verticalOffset, uint dynamic);

		[DllImport("UnityOpenXR")]
		private static extern void FBGetFoveationLevel(out uint level);

		[DllImport("UnityOpenXR")]
		private static extern void FBGetFoveationDynamic(out uint dynamic);

		public const string extensionList = "XR_FB_foveation XR_FB_foveation_configuration XR_FB_foveation_vulkan ";

		public const string featureId = "com.meta.openxr.feature.foveation";

		private static ulong _xrSession;

		private static uint _foveatedRenderingLevel;

		private static uint _useDynamicFoveation;
	}
}
