using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Events;

namespace UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings
{
	public class XrPerformanceSettingsFeature : OpenXRFeature
	{
		public static event UnityAction<PerformanceChangeNotification> OnXrPerformanceChangeNotification;

		public static bool SetPerformanceLevelHint(PerformanceDomain domain, PerformanceLevelHint level)
		{
			return OpenXRRuntime.IsExtensionEnabled("XR_EXT_performance_settings") && XrPerformanceSettingsFeature.NativeApi.xr_performance_settings_setPerformanceLevel(domain, level);
		}

		protected internal override bool OnInstanceCreate(ulong xrInstance)
		{
			return base.OnInstanceCreate(xrInstance) && OpenXRRuntime.IsExtensionEnabled("XR_EXT_performance_settings") && XrPerformanceSettingsFeature.NativeApi.xr_performance_settings_setEventCallback(new XrPerformanceSettingsFeature.NativeApi.XrPerformanceNotificationDelegate(XrPerformanceSettingsFeature.OnXrPerformanceNotificationCallback));
		}

		[MonoPInvokeCallback(typeof(XrPerformanceSettingsFeature.NativeApi.XrPerformanceNotificationDelegate))]
		private static void OnXrPerformanceNotificationCallback(PerformanceChangeNotification notification)
		{
			UnityAction<PerformanceChangeNotification> onXrPerformanceChangeNotification = XrPerformanceSettingsFeature.OnXrPerformanceChangeNotification;
			if (onXrPerformanceChangeNotification == null)
			{
				return;
			}
			onXrPerformanceChangeNotification(notification);
		}

		public const string featureId = "com.unity.openxr.feature.extension.performance_settings";

		public const string extensionString = "XR_EXT_performance_settings";

		internal static class NativeApi
		{
			[DllImport("UnityOpenXR")]
			[return: MarshalAs(UnmanagedType.U1)]
			internal static extern bool xr_performance_settings_setEventCallback(XrPerformanceSettingsFeature.NativeApi.XrPerformanceNotificationDelegate callback);

			[DllImport("UnityOpenXR")]
			[return: MarshalAs(UnmanagedType.U1)]
			internal static extern bool xr_performance_settings_setPerformanceLevel(PerformanceDomain domain, PerformanceLevelHint level);

			internal delegate void XrPerformanceNotificationDelegate(PerformanceChangeNotification notification);
		}
	}
}
