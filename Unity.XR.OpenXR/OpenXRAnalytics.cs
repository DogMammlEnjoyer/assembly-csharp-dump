using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR
{
	internal static class OpenXRAnalytics
	{
		private static bool Initialize()
		{
			if (OpenXRAnalytics.s_Initialized)
			{
				return true;
			}
			if (Analytics.RegisterEvent("openxr_initialize", 1000, 1000, "unity.openxr", "") != AnalyticsResult.Ok)
			{
				return false;
			}
			OpenXRAnalytics.s_Initialized = true;
			return true;
		}

		public static void SendInitializeEvent(bool success)
		{
			if (!OpenXRAnalytics.s_Initialized && !OpenXRAnalytics.Initialize())
			{
				return;
			}
			OpenXRAnalytics.SendPlayerAnalytics(OpenXRAnalytics.CreateInitializeEvent(success));
		}

		private static OpenXRAnalytics.InitializeEvent CreateInitializeEvent(bool success)
		{
			OpenXRAnalytics.InitializeEvent result = default(OpenXRAnalytics.InitializeEvent);
			result.success = success;
			result.runtime = OpenXRRuntime.name;
			result.runtime_version = OpenXRRuntime.version;
			result.plugin_version = OpenXRRuntime.pluginVersion;
			result.api_version = OpenXRRuntime.apiVersion;
			result.enabled_extensions = (from ext in OpenXRRuntime.GetEnabledExtensions()
			select string.Format("{0}_{1}", ext, OpenXRRuntime.GetExtensionVersion(ext))).ToArray<string>();
			result.available_extensions = (from ext in OpenXRRuntime.GetAvailableExtensions()
			select string.Format("{0}_{1}", ext, OpenXRRuntime.GetExtensionVersion(ext))).ToArray<string>();
			result.enabled_features = (from f in OpenXRSettings.Instance.features
			where f != null && f.enabled
			select f.GetType().FullName + "_" + f.version).ToArray<string>();
			result.failed_features = (from f in OpenXRSettings.Instance.features
			where f != null && f.failedInitialization
			select f.GetType().FullName + "_" + f.version).ToArray<string>();
			return result;
		}

		private static void SendPlayerAnalytics(OpenXRAnalytics.InitializeEvent data)
		{
			Analytics.SendEvent("openxr_initialize", data, 1, "");
		}

		private const int kMaxEventsPerHour = 1000;

		private const int kMaxNumberOfElements = 1000;

		private const string kVendorKey = "unity.openxr";

		private const string kEventInitialize = "openxr_initialize";

		private static bool s_Initialized;

		[Serializable]
		private struct InitializeEvent : IAnalytic.IData
		{
			public bool success;

			public string runtime;

			public string runtime_version;

			public string plugin_version;

			public string api_version;

			public string[] available_extensions;

			public string[] enabled_extensions;

			public string[] enabled_features;

			public string[] failed_features;
		}

		[AnalyticInfo("openxr_initialize", "unity.openxr", 1, 1000, 1000)]
		private class XrInitializeAnalytic : IAnalytic
		{
			public XrInitializeAnalytic(OpenXRAnalytics.InitializeEvent data)
			{
				this.data = new OpenXRAnalytics.InitializeEvent?(data);
			}

			public bool TryGatherData(out IAnalytic.IData data, [NotNullWhen(false)] out Exception error)
			{
				error = null;
				data = this.data;
				return data != null;
			}

			private OpenXRAnalytics.InitializeEvent? data;
		}
	}
}
