using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.Features;

namespace Meta.XR
{
	public class MetaXRSubsampledLayout : OpenXRFeature
	{
		protected override bool OnInstanceCreate(ulong xrInstance)
		{
			MetaXRSubsampledLayout.MetaSetSubsampledLayout(base.enabled);
			return true;
		}

		[DllImport("UnityOpenXR")]
		private static extern void MetaSetSubsampledLayout(bool enabled);

		public const string extensionName = "XR_META_vulkan_swapchain_create_info";

		public const string featureId = "com.meta.openxr.feature.subsampledLayout";
	}
}
