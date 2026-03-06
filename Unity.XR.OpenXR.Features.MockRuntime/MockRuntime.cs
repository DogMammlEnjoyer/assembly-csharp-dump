using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.Mock
{
	public class MockRuntime : OpenXRFeature
	{
		public static event MockRuntime.ScriptEventDelegate onScriptEvent;

		public static MockRuntime Instance
		{
			get
			{
				return OpenXRSettings.Instance.GetFeature<MockRuntime>();
			}
		}

		[MonoPInvokeCallback(typeof(MockRuntime.ScriptEventDelegate))]
		private static void ReceiveScriptEvent(MockRuntime.ScriptEvent evt, ulong param)
		{
			MockRuntime.ScriptEventDelegate scriptEventDelegate = MockRuntime.onScriptEvent;
			if (scriptEventDelegate == null)
			{
				return;
			}
			scriptEventDelegate(evt, param);
		}

		[MonoPInvokeCallback(typeof(MockRuntime.BeforeFunctionDelegate))]
		private static XrResult BeforeFunctionCallback(string function)
		{
			MockRuntime.BeforeFunctionDelegate beforeFunctionCallback = MockRuntime.GetBeforeFunctionCallback(function);
			if (beforeFunctionCallback == null)
			{
				return XrResult.Success;
			}
			return beforeFunctionCallback(function);
		}

		[MonoPInvokeCallback(typeof(MockRuntime.BeforeFunctionDelegate))]
		private static void AfterFunctionCallback(string function, XrResult result)
		{
			MockRuntime.AfterFunctionDelegate afterFunctionCallback = MockRuntime.GetAfterFunctionCallback(function);
			if (afterFunctionCallback == null)
			{
				return;
			}
			afterFunctionCallback(function, result);
		}

		public static void SetFunctionCallback(string function, MockRuntime.BeforeFunctionDelegate beforeCallback, MockRuntime.AfterFunctionDelegate afterCallback)
		{
			if (beforeCallback != null)
			{
				if (MockRuntime.s_BeforeFunctionCallbacks == null)
				{
					MockRuntime.s_BeforeFunctionCallbacks = new Dictionary<string, MockRuntime.BeforeFunctionDelegate>();
				}
				MockRuntime.s_BeforeFunctionCallbacks[function] = beforeCallback;
			}
			else if (MockRuntime.s_BeforeFunctionCallbacks != null)
			{
				MockRuntime.s_BeforeFunctionCallbacks.Remove(function);
				if (MockRuntime.s_BeforeFunctionCallbacks.Count == 0)
				{
					MockRuntime.s_BeforeFunctionCallbacks = null;
				}
			}
			if (afterCallback != null)
			{
				if (MockRuntime.s_AfterFunctionCallbacks == null)
				{
					MockRuntime.s_AfterFunctionCallbacks = new Dictionary<string, MockRuntime.AfterFunctionDelegate>();
				}
				MockRuntime.s_AfterFunctionCallbacks[function] = afterCallback;
			}
			else if (MockRuntime.s_AfterFunctionCallbacks != null)
			{
				MockRuntime.s_AfterFunctionCallbacks.Remove(function);
				if (MockRuntime.s_AfterFunctionCallbacks.Count == 0)
				{
					MockRuntime.s_AfterFunctionCallbacks = null;
				}
			}
			MockRuntime.MockRuntime_RegisterFunctionCallbacks((MockRuntime.s_BeforeFunctionCallbacks != null) ? new MockRuntime.BeforeFunctionDelegate(MockRuntime.BeforeFunctionCallback) : null, (MockRuntime.s_AfterFunctionCallbacks != null) ? new MockRuntime.AfterFunctionDelegate(MockRuntime.AfterFunctionCallback) : null);
		}

		public static void SetFunctionCallback(string function, MockRuntime.BeforeFunctionDelegate beforeCallback)
		{
			MockRuntime.SetFunctionCallback(function, beforeCallback, MockRuntime.GetAfterFunctionCallback(function));
		}

		public static void SetFunctionCallback(string function, MockRuntime.AfterFunctionDelegate afterCallback)
		{
			MockRuntime.SetFunctionCallback(function, MockRuntime.GetBeforeFunctionCallback(function), afterCallback);
		}

		public static MockRuntime.BeforeFunctionDelegate GetBeforeFunctionCallback(string function)
		{
			if (MockRuntime.s_BeforeFunctionCallbacks == null)
			{
				return null;
			}
			MockRuntime.BeforeFunctionDelegate result;
			if (!MockRuntime.s_BeforeFunctionCallbacks.TryGetValue(function, out result))
			{
				return null;
			}
			return result;
		}

		public static MockRuntime.AfterFunctionDelegate GetAfterFunctionCallback(string function)
		{
			if (MockRuntime.s_AfterFunctionCallbacks == null)
			{
				return null;
			}
			MockRuntime.AfterFunctionDelegate result;
			if (!MockRuntime.s_AfterFunctionCallbacks.TryGetValue(function, out result))
			{
				return null;
			}
			return result;
		}

		public static void ClearFunctionCallbacks()
		{
			MockRuntime.s_BeforeFunctionCallbacks = null;
			MockRuntime.s_AfterFunctionCallbacks = null;
			MockRuntime.MockRuntime_RegisterFunctionCallbacks(null, null);
		}

		public static void ResetDefaults()
		{
			MockRuntime.onScriptEvent = null;
			MockRuntime.ClearFunctionCallbacks();
		}

		protected internal override void OnInstanceDestroy(ulong instance)
		{
			MockRuntime.ClearFunctionCallbacks();
		}

		[DllImport("mock_api", EntryPoint = "MockRuntime_HookCreateInstance")]
		public static extern IntPtr HookCreateInstance(IntPtr func);

		[DllImport("mock_api", EntryPoint = "MockRuntime_SetKeepFunctionCallbacks")]
		public static extern void SetKeepFunctionCallbacks([MarshalAs(UnmanagedType.I1)] bool value);

		[DllImport("mock_api", EntryPoint = "MockRuntime_SetView")]
		public static extern void SetViewPose(XrViewConfigurationType viewConfigurationType, int viewIndex, Vector3 position, Quaternion orientation, Vector4 fov);

		[DllImport("mock_api", EntryPoint = "MockRuntime_SetViewState")]
		public static extern void SetViewState(XrViewConfigurationType viewConfigurationType, XrViewStateFlags viewStateFlags);

		[DllImport("mock_api", EntryPoint = "MockRuntime_SetReferenceSpace")]
		public static extern void SetSpace(XrReferenceSpaceType referenceSpace, Vector3 position, Quaternion orientation, XrSpaceLocationFlags locationFlags);

		[DllImport("mock_api", EntryPoint = "MockRuntime_SetActionSpace")]
		public static extern void SetSpace(ulong actionHandle, Vector3 position, Quaternion orientation, XrSpaceLocationFlags locationFlags);

		[DllImport("mock_api", EntryPoint = "MockRuntime_RegisterScriptEventCallback")]
		private static extern XrResult Internal_RegisterScriptEventCallback(MockRuntime.ScriptEventDelegate callback);

		[DllImport("mock_api", EntryPoint = "MockRuntime_TransitionToState")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_TransitionToState(XrSessionState state, [MarshalAs(UnmanagedType.I1)] bool forceTransition);

		[DllImport("mock_api", EntryPoint = "MockRuntime_GetSessionState")]
		private static extern XrSessionState Internal_GetSessionState();

		[DllImport("mock_api", EntryPoint = "MockRuntime_RequestExitSession")]
		public static extern void RequestExitSession();

		[DllImport("mock_api", EntryPoint = "MockRuntime_CauseInstanceLoss")]
		public static extern void CauseInstanceLoss();

		[DllImport("mock_api", EntryPoint = "MockRuntime_CauseUserPresenceChange")]
		public static extern void CauseUserPresenceChange([MarshalAs(UnmanagedType.U1)] bool hasUserPresent);

		[DllImport("mock_api", EntryPoint = "MockRuntime_SetReferenceSpaceBounds")]
		internal static extern void SetReferenceSpaceBounds(XrReferenceSpaceType referenceSpace, Vector2 bounds);

		[DllImport("mock_api", EntryPoint = "MockRuntime_GetEndFrameStats")]
		internal static extern void GetEndFrameStats(out int primaryLayerCount, out int secondaryLayerCount);

		[DllImport("mock_api", EntryPoint = "MockRuntime_ActivateSecondaryView")]
		internal static extern void ActivateSecondaryView(XrViewConfigurationType viewConfigurationType, [MarshalAs(UnmanagedType.I1)] bool activate);

		[DllImport("mock_api")]
		private static extern void MockRuntime_RegisterFunctionCallbacks(MockRuntime.BeforeFunctionDelegate hookBefore, MockRuntime.AfterFunctionDelegate hookAfter);

		[DllImport("mock_api", EntryPoint = "MockRuntime_MetaPerformanceMetrics_SeedCounterOnce_Float")]
		internal static extern void MetaPerformanceMetrics_SeedCounterOnce_Float(string xrPathString, float value, uint unit);

		[DllImport("mock_api", EntryPoint = "MockRuntime_PerformanceSettings_CauseNotification")]
		internal static extern void PerformanceSettings_CauseNotification(PerformanceDomain domain, PerformanceSubDomain subDomain, PerformanceNotificationLevel level);

		[DllImport("mock_api", EntryPoint = "MockRuntime_PerformanceSettings_GetPerformanceLevelHint")]
		internal static extern PerformanceLevelHint PerformanceSettings_GetPerformanceLevelHint(PerformanceDomain domain);

		internal static bool IsAndroidThreadTypeRegistered(uint threadType)
		{
			return false;
		}

		internal static ulong GetRegisteredAndroidThreadsCount()
		{
			return 0UL;
		}

		internal void AddTestHookGetInstanceProcAddr(Func<IntPtr, IntPtr> nativeFunctionHook)
		{
			this.MockFunctionInterceptor = nativeFunctionHook;
		}

		internal void ClearTestHookGetInstanceProcAddr()
		{
			this.MockFunctionInterceptor = null;
		}

		private static Dictionary<string, MockRuntime.AfterFunctionDelegate> s_AfterFunctionCallbacks;

		private static Dictionary<string, MockRuntime.BeforeFunctionDelegate> s_BeforeFunctionCallbacks;

		public const string featureId = "com.unity.openxr.feature.mockruntime";

		public bool ignoreValidationErrors;

		private const string extLib = "mock_api";

		internal Func<IntPtr, IntPtr> MockFunctionInterceptor;

		public enum ScriptEvent
		{
			Unknown,
			EndFrame,
			HapticImpulse,
			HapticStop
		}

		public delegate void ScriptEventDelegate(MockRuntime.ScriptEvent evt, ulong param);

		public delegate XrResult BeforeFunctionDelegate(string functionName);

		public delegate void AfterFunctionDelegate(string functionName, XrResult result);
	}
}
