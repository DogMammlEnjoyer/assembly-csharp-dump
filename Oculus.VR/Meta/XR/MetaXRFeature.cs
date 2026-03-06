using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Meta.XR
{
	public class MetaXRFeature : OpenXRFeature
	{
		public bool userPresent
		{
			get
			{
				return OVRPlugin.UnityOpenXR.Enabled && OVRPlugin.userPresent;
			}
		}

		protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
		{
			OVRPlugin.UnityOpenXR.Enabled = true;
			Debug.Log(string.Format("[MetaXRFeature] HookGetInstanceProcAddr: {0}", func));
			Debug.Log("[MetaXRFeature] SetClientVersion");
			OVRPlugin.UnityOpenXR.SetClientVersion();
			return OVRPlugin.UnityOpenXR.HookGetInstanceProcAddr(func);
		}

		protected override bool OnInstanceCreate(ulong xrInstance)
		{
			bool flag = false;
			string[] availableExtensions = OpenXRRuntime.GetAvailableExtensions();
			for (int i = 0; i < availableExtensions.Length; i++)
			{
				if (availableExtensions[i] == "XR_META_headset_id")
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Debug.Log("[MetaXRFeature] OpenXR runtime supports XR_META_headset_id extension. MetaXRFeature is enabled.");
			}
			else
			{
				string text = OpenXRRuntime.name.ToLower();
				if (!text.Contains("meta") && !text.Contains("oculus"))
				{
					Debug.LogWarningFormat("[MetaXRFeature] MetaXRFeature is disabled on non-Oculus/Meta OpenXR Runtime. Runtime name: {0}", new object[]
					{
						OpenXRRuntime.name
					});
					return false;
				}
			}
			Debug.Log(string.Format("[MetaXRFeature] OnInstanceCreate: {0}", xrInstance));
			bool flag2 = OVRPlugin.UnityOpenXR.OnInstanceCreate(xrInstance);
			if (!flag2)
			{
				Debug.LogWarning("[MetaXRFeature] OnInstanceCreate returned an error. If you are using Quest Link, please verify if it's started.");
			}
			return flag2;
		}

		protected override void OnInstanceDestroy(ulong xrInstance)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnInstanceDestroy: {0}", xrInstance));
			OVRPlugin.UnityOpenXR.OnInstanceDestroy(xrInstance);
		}

		protected override void OnSessionCreate(ulong xrSession)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnSessionCreate: {0}", xrSession));
			OVRPlugin.UnityOpenXR.OnSessionCreate(xrSession);
		}

		protected override void OnAppSpaceChange(ulong xrSpace)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnAppSpaceChange: {0}", xrSpace));
			int num = 0;
			if (OpenXRSettings.AllowRecentering)
			{
				num |= 1;
			}
			OVRPlugin.UnityOpenXR.OnAppSpaceChange2(xrSpace, num);
		}

		protected override void OnSessionStateChange(int oldState, int newState)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnSessionStateChange: {0} -> {1}", oldState, newState));
			OVRPlugin.UnityOpenXR.OnSessionStateChange(oldState, newState);
		}

		protected override void OnSessionBegin(ulong xrSession)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnSessionBegin: {0}", xrSession));
			OVRPlugin.UnityOpenXR.OnSessionBegin(xrSession);
		}

		protected override void OnSessionEnd(ulong xrSession)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnSessionEnd: {0}", xrSession));
			OVRPlugin.UnityOpenXR.OnSessionEnd(xrSession);
		}

		protected override void OnSessionExiting(ulong xrSession)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnSessionExiting: {0}", xrSession));
			OVRPlugin.UnityOpenXR.OnSessionExiting(xrSession);
		}

		protected override void OnSessionDestroy(ulong xrSession)
		{
			Debug.Log(string.Format("[MetaXRFeature] OnSessionDestroy: {0}", xrSession));
			OVRPlugin.UnityOpenXR.OnSessionDestroy(xrSession);
		}

		public const string featureId = "com.meta.openxr.feature.metaxr";
	}
}
