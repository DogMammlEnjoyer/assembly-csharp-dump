using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features
{
	[Serializable]
	public abstract class OpenXRFeature : ScriptableObject
	{
		internal bool failedInitialization { get; private set; }

		internal static bool requiredFeatureFailed { get; private set; }

		public bool enabled
		{
			get
			{
				return this.m_enabled && (OpenXRLoaderBase.Instance == null || !this.failedInitialization);
			}
			set
			{
				if (this.enabled == value)
				{
					return;
				}
				if (OpenXRLoaderBase.Instance != null)
				{
					Debug.LogError("OpenXRFeature.enabled cannot be changed while OpenXR is running");
					return;
				}
				this.m_enabled = value;
				this.OnEnabledChange();
			}
		}

		protected static IntPtr xrGetInstanceProcAddr
		{
			get
			{
				return OpenXRFeature.Internal_GetProcAddressPtr(false);
			}
		}

		protected internal virtual IntPtr HookGetInstanceProcAddr(IntPtr func)
		{
			return func;
		}

		protected internal virtual void OnSubsystemCreate()
		{
		}

		protected internal virtual void OnSubsystemStart()
		{
		}

		protected internal virtual void OnSubsystemStop()
		{
		}

		protected internal virtual void OnSubsystemDestroy()
		{
		}

		protected internal virtual bool OnInstanceCreate(ulong xrInstance)
		{
			return true;
		}

		protected internal virtual void OnSystemChange(ulong xrSystem)
		{
		}

		protected internal virtual void OnSessionCreate(ulong xrSession)
		{
		}

		protected internal virtual void OnAppSpaceChange(ulong xrSpace)
		{
		}

		protected internal virtual void OnSessionStateChange(int oldState, int newState)
		{
		}

		protected internal virtual void OnSessionBegin(ulong xrSession)
		{
		}

		protected internal virtual void OnSessionEnd(ulong xrSession)
		{
		}

		protected internal virtual void OnSessionExiting(ulong xrSession)
		{
		}

		protected internal virtual void OnSessionDestroy(ulong xrSession)
		{
		}

		protected internal virtual void OnInstanceDestroy(ulong xrInstance)
		{
		}

		protected internal virtual void OnSessionLossPending(ulong xrSession)
		{
		}

		protected internal virtual void OnInstanceLossPending(ulong xrInstance)
		{
		}

		protected internal virtual void OnFormFactorChange(int xrFormFactor)
		{
		}

		protected internal virtual void OnViewConfigurationTypeChange(int xrViewConfigurationType)
		{
		}

		protected internal virtual void OnEnvironmentBlendModeChange(XrEnvironmentBlendMode xrEnvironmentBlendMode)
		{
		}

		protected internal virtual void OnEnabledChange()
		{
		}

		protected static string PathToString(ulong path)
		{
			IntPtr ptr;
			if (!OpenXRFeature.Internal_PathToStringPtr(path, out ptr))
			{
				return null;
			}
			return Marshal.PtrToStringAnsi(ptr);
		}

		protected static ulong StringToPath(string str)
		{
			ulong result;
			if (!OpenXRFeature.Internal_StringToPath(str, out result))
			{
				return 0UL;
			}
			return result;
		}

		protected static ulong GetCurrentInteractionProfile(ulong userPath)
		{
			ulong result;
			if (!OpenXRFeature.Internal_GetCurrentInteractionProfile(userPath, out result))
			{
				return 0UL;
			}
			return result;
		}

		protected static ulong GetCurrentInteractionProfile(string userPath)
		{
			return OpenXRFeature.GetCurrentInteractionProfile(OpenXRFeature.StringToPath(userPath));
		}

		protected static ulong GetCurrentAppSpace()
		{
			ulong result;
			if (!OpenXRFeature.Internal_GetAppSpace(out result))
			{
				return 0UL;
			}
			return result;
		}

		protected static int GetViewConfigurationTypeForRenderPass(int renderPassIndex)
		{
			return OpenXRFeature.Internal_GetViewTypeFromRenderIndex(renderPassIndex);
		}

		protected static void SetEnvironmentBlendMode(XrEnvironmentBlendMode xrEnvironmentBlendMode)
		{
			OpenXRFeature.Internal_SetEnvironmentBlendMode(xrEnvironmentBlendMode);
		}

		protected static XrEnvironmentBlendMode GetEnvironmentBlendMode()
		{
			return OpenXRFeature.Internal_GetEnvironmentBlendMode();
		}

		protected void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : ISubsystemDescriptor where TSubsystem : ISubsystem
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				Debug.LogError("CreateSubsystem called before loader was initialized");
				return;
			}
			OpenXRLoaderBase.Instance.CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);
		}

		protected void StartSubsystem<T>() where T : class, ISubsystem
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				Debug.LogError("StartSubsystem called before loader was initialized");
				return;
			}
			OpenXRLoaderBase.Instance.StartSubsystem<T>();
		}

		protected void StopSubsystem<T>() where T : class, ISubsystem
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				Debug.LogError("StopSubsystem called before loader was initialized");
				return;
			}
			OpenXRLoaderBase.Instance.StopSubsystem<T>();
		}

		protected void DestroySubsystem<T>() where T : class, ISubsystem
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				Debug.LogError("DestroySubsystem called before loader was initialized");
				return;
			}
			OpenXRLoaderBase.Instance.DestroySubsystem<T>();
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void Awake()
		{
		}

		internal static bool ReceiveLoaderEvent(OpenXRLoaderBase loader, OpenXRFeature.LoaderEvent e)
		{
			OpenXRSettings instance = OpenXRSettings.Instance;
			if (instance == null)
			{
				return true;
			}
			foreach (OpenXRFeature openXRFeature in instance.features)
			{
				if (!(openXRFeature == null) && openXRFeature.enabled)
				{
					switch (e)
					{
					case OpenXRFeature.LoaderEvent.SubsystemCreate:
						openXRFeature.OnSubsystemCreate();
						break;
					case OpenXRFeature.LoaderEvent.SubsystemDestroy:
						openXRFeature.OnSubsystemDestroy();
						break;
					case OpenXRFeature.LoaderEvent.SubsystemStart:
						openXRFeature.OnSubsystemStart();
						break;
					case OpenXRFeature.LoaderEvent.SubsystemStop:
						openXRFeature.OnSubsystemStop();
						break;
					default:
						throw new ArgumentOutOfRangeException("e", e, null);
					}
				}
			}
			return true;
		}

		internal static void ReceiveNativeEvent(OpenXRFeature.NativeEvent e, ulong payload)
		{
			if (null == OpenXRSettings.Instance)
			{
				return;
			}
			foreach (OpenXRFeature openXRFeature in OpenXRSettings.Instance.features)
			{
				if (!(openXRFeature == null) && openXRFeature.enabled)
				{
					switch (e)
					{
					case OpenXRFeature.NativeEvent.XrSetupConfigValues:
						openXRFeature.OnFormFactorChange(OpenXRFeature.Internal_GetFormFactor());
						openXRFeature.OnEnvironmentBlendModeChange(OpenXRFeature.Internal_GetEnvironmentBlendMode());
						openXRFeature.OnViewConfigurationTypeChange(OpenXRFeature.Internal_GetViewConfigurationType());
						break;
					case OpenXRFeature.NativeEvent.XrSystemIdChanged:
						openXRFeature.OnSystemChange(payload);
						break;
					case OpenXRFeature.NativeEvent.XrInstanceChanged:
						openXRFeature.failedInitialization = !openXRFeature.OnInstanceCreate(payload);
						OpenXRFeature.requiredFeatureFailed |= (openXRFeature.required && openXRFeature.failedInitialization);
						break;
					case OpenXRFeature.NativeEvent.XrSessionChanged:
						openXRFeature.OnSessionCreate(payload);
						break;
					case OpenXRFeature.NativeEvent.XrBeginSession:
						openXRFeature.OnSessionBegin(payload);
						break;
					case OpenXRFeature.NativeEvent.XrSessionStateChanged:
					{
						int oldState;
						int newState;
						OpenXRFeature.Internal_GetSessionState(out oldState, out newState);
						openXRFeature.OnSessionStateChange(oldState, newState);
						break;
					}
					case OpenXRFeature.NativeEvent.XrChangedSpaceApp:
						openXRFeature.OnAppSpaceChange(payload);
						break;
					case OpenXRFeature.NativeEvent.XrEndSession:
						openXRFeature.OnSessionEnd(payload);
						break;
					case OpenXRFeature.NativeEvent.XrDestroySession:
						openXRFeature.OnSessionDestroy(payload);
						break;
					case OpenXRFeature.NativeEvent.XrDestroyInstance:
						openXRFeature.OnInstanceDestroy(payload);
						break;
					case OpenXRFeature.NativeEvent.XrExiting:
						openXRFeature.OnSessionExiting(payload);
						break;
					case OpenXRFeature.NativeEvent.XrLossPending:
						openXRFeature.OnSessionLossPending(payload);
						break;
					case OpenXRFeature.NativeEvent.XrInstanceLossPending:
						openXRFeature.OnInstanceLossPending(payload);
						break;
					}
				}
			}
		}

		internal static void Initialize()
		{
			OpenXRFeature.requiredFeatureFailed = false;
			OpenXRSettings instance = OpenXRSettings.Instance;
			if (instance == null || instance.features == null)
			{
				return;
			}
			foreach (OpenXRFeature openXRFeature in instance.features)
			{
				if (openXRFeature != null)
				{
					openXRFeature.failedInitialization = false;
				}
			}
		}

		internal static void HookGetInstanceProcAddr()
		{
			IntPtr func = OpenXRFeature.Internal_GetProcAddressPtr(true);
			OpenXRSettings instance = OpenXRSettings.Instance;
			if (instance != null && instance.features != null)
			{
				for (int i = instance.features.Length - 1; i >= 0; i--)
				{
					OpenXRFeature openXRFeature = instance.features[i];
					if (!(openXRFeature == null) && openXRFeature.enabled)
					{
						func = openXRFeature.HookGetInstanceProcAddr(func);
					}
				}
			}
			OpenXRFeature.Internal_SetProcAddressPtrAndLoadStage1(func);
		}

		protected ulong GetAction(InputAction inputAction)
		{
			return OpenXRInput.GetActionHandle(inputAction, null);
		}

		protected ulong GetAction(InputDevice device, InputFeatureUsage usage)
		{
			return OpenXRInput.GetActionHandle(device, usage);
		}

		protected ulong GetAction(InputDevice device, string usageName)
		{
			return OpenXRInput.GetActionHandle(device, usageName);
		}

		protected internal static ulong RegisterStatsDescriptor(string statName, OpenXRFeature.StatFlags statFlags)
		{
			return OpenXRFeature.runtime_RegisterStatsDescriptor(statName, statFlags);
		}

		protected internal static void SetStatAsFloat(ulong statId, float value)
		{
			OpenXRFeature.runtime_SetStatAsFloat(statId, value);
		}

		protected internal static void SetStatAsUInt(ulong statId, uint value)
		{
			OpenXRFeature.runtime_SetStatAsUInt(statId, value);
		}

		[DllImport("UnityOpenXR", EntryPoint = "Internal_PathToString")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_PathToStringPtr(ulong pathId, out IntPtr path);

		[DllImport("UnityOpenXR")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_StringToPath([MarshalAs(UnmanagedType.LPStr)] string str, out ulong pathId);

		[DllImport("UnityOpenXR")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetCurrentInteractionProfile(ulong pathId, out ulong interactionProfile);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetFormFactor")]
		private static extern int Internal_GetFormFactor();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetViewConfigurationType")]
		private static extern int Internal_GetViewConfigurationType();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetViewTypeFromRenderIndex")]
		private static extern int Internal_GetViewTypeFromRenderIndex(int renderPassIndex);

		[DllImport("UnityOpenXR", EntryPoint = "OpenXRInputProvider_GetXRSession")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_GetXRSession(out ulong xrSession);

		[DllImport("UnityOpenXR", EntryPoint = "session_GetSessionState")]
		private static extern void Internal_GetSessionState(out int oldState, out int newState);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetEnvironmentBlendMode")]
		private static extern XrEnvironmentBlendMode Internal_GetEnvironmentBlendMode();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetEnvironmentBlendMode")]
		private static extern void Internal_SetEnvironmentBlendMode(XrEnvironmentBlendMode xrEnvironmentBlendMode);

		[DllImport("UnityOpenXR", EntryPoint = "OpenXRInputProvider_GetAppSpace")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_GetAppSpace(out ulong appSpace);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetProcAddressPtr")]
		internal static extern IntPtr Internal_GetProcAddressPtr([MarshalAs(UnmanagedType.I1)] bool loaderDefault);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetProcAddressPtrAndLoadStage1")]
		internal static extern void Internal_SetProcAddressPtrAndLoadStage1(IntPtr func);

		[DllImport("UnityOpenXR")]
		internal static extern ulong runtime_RegisterStatsDescriptor(string statName, OpenXRFeature.StatFlags statFlags);

		[DllImport("UnityOpenXR")]
		internal static extern void runtime_SetStatAsFloat(ulong statId, float value);

		[DllImport("UnityOpenXR")]
		internal static extern void runtime_SetStatAsUInt(ulong statId, uint value);

		[FormerlySerializedAs("enabled")]
		[HideInInspector]
		[SerializeField]
		private bool m_enabled;

		[HideInInspector]
		[SerializeField]
		internal string nameUi;

		[HideInInspector]
		[SerializeField]
		internal string version;

		[HideInInspector]
		[SerializeField]
		internal string featureIdInternal;

		[HideInInspector]
		[SerializeField]
		internal string openxrExtensionStrings;

		[HideInInspector]
		[SerializeField]
		internal string company;

		[HideInInspector]
		[SerializeField]
		internal int priority;

		[HideInInspector]
		[SerializeField]
		internal bool required;

		[NonSerialized]
		internal bool internalFieldsUpdated;

		private const string Library = "UnityOpenXR";

		internal enum LoaderEvent
		{
			SubsystemCreate,
			SubsystemDestroy,
			SubsystemStart,
			SubsystemStop
		}

		internal enum NativeEvent
		{
			XrSetupConfigValues,
			XrSystemIdChanged,
			XrInstanceChanged,
			XrSessionChanged,
			XrBeginSession,
			XrSessionStateChanged,
			XrChangedSpaceApp,
			XrEndSession,
			XrDestroySession,
			XrDestroyInstance,
			XrIdle,
			XrReady,
			XrSynchronized,
			XrVisible,
			XrFocused,
			XrStopping,
			XrExiting,
			XrLossPending,
			XrInstanceLossPending,
			XrRestartRequested,
			XrRequestRestartLoop,
			XrRequestGetSystemLoop
		}

		[Flags]
		protected internal enum StatFlags
		{
			StatOptionNone = 0,
			ClearOnUpdate = 1,
			All = 1
		}
	}
}
