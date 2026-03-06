using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using AOT;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR
{
	public class OpenXRLoaderBase : XRLoaderHelper
	{
		internal static OpenXRLoaderBase Instance { get; private set; }

		internal OpenXRLoaderBase.LoaderState currentLoaderState { get; private set; }

		internal XRDisplaySubsystem displaySubsystem
		{
			get
			{
				return this.GetLoadedSubsystem<XRDisplaySubsystem>();
			}
		}

		internal XRInputSubsystem inputSubsystem
		{
			get
			{
				OpenXRLoaderBase instance = OpenXRLoaderBase.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.GetLoadedSubsystem<XRInputSubsystem>();
			}
		}

		private bool isInitialized
		{
			get
			{
				return this.currentLoaderState != OpenXRLoaderBase.LoaderState.Uninitialized && this.currentLoaderState != OpenXRLoaderBase.LoaderState.DeinitializeAttempted;
			}
		}

		private bool isStarted
		{
			get
			{
				return this.runningStates.Contains(this.currentLoaderState);
			}
		}

		private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
		{
			ulong section = DiagnosticReport.GetSection("Unhandled Exception Report");
			DiagnosticReport.AddSectionEntry(section, "Is Terminating", string.Format("{0}", args.IsTerminating));
			Exception ex = (Exception)args.ExceptionObject;
			DiagnosticReport.AddSectionEntry(section, "Message", ex.Message ?? "");
			DiagnosticReport.AddSectionEntry(section, "Source", ex.Source ?? "");
			DiagnosticReport.AddSectionEntry(section, "Stack Trace", "\n" + ex.StackTrace);
			DiagnosticReport.DumpReport("Uncaught Exception");
		}

		public override bool Initialize()
		{
			if (this.currentLoaderState == OpenXRLoaderBase.LoaderState.Initialized)
			{
				return true;
			}
			if (!this.validLoaderInitStates.Contains(this.currentLoaderState))
			{
				return false;
			}
			if (OpenXRLoaderBase.Instance != null)
			{
				Debug.LogError("Only one OpenXRLoader can be initialized at any given time");
				return false;
			}
			DiagnosticReport.StartReport();
			try
			{
				if (this.InitializeInternal())
				{
					return true;
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			this.Deinitialize();
			OpenXRLoaderBase.Instance = null;
			OpenXRAnalytics.SendInitializeEvent(false);
			return false;
		}

		private bool InitializeInternal()
		{
			OpenXRLoaderBase.Instance = this;
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.InitializeAttempted;
			OpenXRLoaderBase.Internal_SetSuccessfullyInitialized(false);
			OpenXRInput.RegisterLayouts();
			OpenXRFeature.Initialize();
			if (!this.LoadOpenXRSymbols())
			{
				Debug.LogError("Failed to load openxr runtime loader.");
				return false;
			}
			OpenXRSettings.Instance.features = (from f in OpenXRSettings.Instance.features
			where f != null
			orderby f.priority descending, f.nameUi
			select f).ToArray<OpenXRFeature>();
			OpenXRFeature.HookGetInstanceProcAddr();
			if (!OpenXRLoaderBase.Internal_InitializeSession())
			{
				return false;
			}
			this.RequestOpenXRFeatures();
			OpenXRLoaderBase.RegisterOpenXRCallbacks();
			if (null != OpenXRSettings.Instance)
			{
				OpenXRSettings.Instance.ApplySettings();
			}
			if (!this.CreateSubsystems())
			{
				return false;
			}
			if (OpenXRFeature.requiredFeatureFailed)
			{
				return false;
			}
			this.SetApplicationInfo();
			OpenXRAnalytics.SendInitializeEvent(true);
			OpenXRFeature.ReceiveLoaderEvent(this, OpenXRFeature.LoaderEvent.SubsystemCreate);
			OpenXRLoaderBase.DebugLogEnabledSpecExtensions();
			Application.onBeforeRender += this.ProcessOpenXRMessageLoop;
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.Initialized;
			return true;
		}

		private bool CreateSubsystems()
		{
			if (this.displaySubsystem == null)
			{
				this.CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(OpenXRLoaderBase.s_DisplaySubsystemDescriptors, "OpenXR Display");
				if (this.displaySubsystem == null)
				{
					return false;
				}
			}
			if (this.inputSubsystem == null)
			{
				this.CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(OpenXRLoaderBase.s_InputSubsystemDescriptors, "OpenXR Input");
				if (this.inputSubsystem == null)
				{
					return false;
				}
			}
			return true;
		}

		internal void ProcessOpenXRMessageLoop()
		{
			if (this.currentOpenXRState == OpenXRFeature.NativeEvent.XrIdle || this.currentOpenXRState == OpenXRFeature.NativeEvent.XrStopping || this.currentOpenXRState == OpenXRFeature.NativeEvent.XrExiting || this.currentOpenXRState == OpenXRFeature.NativeEvent.XrLossPending || this.currentOpenXRState == OpenXRFeature.NativeEvent.XrInstanceLossPending)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if ((double)realtimeSinceStartup - this.lastPollCheckTime < 0.1)
				{
					return;
				}
				this.lastPollCheckTime = (double)realtimeSinceStartup;
			}
			OpenXRLoaderBase.Internal_PumpMessageLoop();
		}

		public override bool Start()
		{
			if (this.currentLoaderState == OpenXRLoaderBase.LoaderState.Started)
			{
				return true;
			}
			if (!this.validLoaderStartStates.Contains(this.currentLoaderState))
			{
				return false;
			}
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.StartAttempted;
			if (!this.StartInternal())
			{
				this.Stop();
				return false;
			}
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.Started;
			return true;
		}

		private bool StartInternal()
		{
			if (!OpenXRLoaderBase.Internal_CreateSessionIfNeeded())
			{
				return false;
			}
			if (this.currentOpenXRState != OpenXRFeature.NativeEvent.XrReady || (this.currentLoaderState != OpenXRLoaderBase.LoaderState.StartAttempted && this.currentLoaderState != OpenXRLoaderBase.LoaderState.Started))
			{
				return true;
			}
			this.StartSubsystem<XRDisplaySubsystem>();
			XRDisplaySubsystem displaySubsystem = this.displaySubsystem;
			if (displaySubsystem != null && !displaySubsystem.running)
			{
				return false;
			}
			OpenXRLoaderBase.Internal_BeginSession();
			if (!this.actionSetsAttached)
			{
				OpenXRInput.AttachActionSets();
				this.actionSetsAttached = true;
			}
			XRDisplaySubsystem displaySubsystem2 = this.displaySubsystem;
			if (displaySubsystem2 != null && !displaySubsystem2.running)
			{
				this.StartSubsystem<XRDisplaySubsystem>();
			}
			XRInputSubsystem inputSubsystem = this.inputSubsystem;
			if (inputSubsystem != null && !inputSubsystem.running)
			{
				this.StartSubsystem<XRInputSubsystem>();
			}
			XRInputSubsystem inputSubsystem2 = this.inputSubsystem;
			bool flag = inputSubsystem2 != null && inputSubsystem2.running;
			XRDisplaySubsystem displaySubsystem3 = this.displaySubsystem;
			bool flag2 = displaySubsystem3 != null && displaySubsystem3.running;
			if (flag && flag2)
			{
				OpenXRFeature.ReceiveLoaderEvent(this, OpenXRFeature.LoaderEvent.SubsystemStart);
				return true;
			}
			return false;
		}

		public override bool Stop()
		{
			if (this.currentLoaderState == OpenXRLoaderBase.LoaderState.Stopped)
			{
				return true;
			}
			if (!this.validLoaderStopStates.Contains(this.currentLoaderState))
			{
				return false;
			}
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.StopAttempted;
			XRInputSubsystem inputSubsystem = this.inputSubsystem;
			object obj = inputSubsystem != null && inputSubsystem.running;
			XRDisplaySubsystem displaySubsystem = this.displaySubsystem;
			bool flag = displaySubsystem != null && displaySubsystem.running;
			object obj2 = obj;
			if ((obj2 | flag) != null)
			{
				OpenXRFeature.ReceiveLoaderEvent(this, OpenXRFeature.LoaderEvent.SubsystemStop);
			}
			if (obj2 != null)
			{
				this.StopSubsystem<XRInputSubsystem>();
			}
			if (flag)
			{
				this.StopSubsystem<XRDisplaySubsystem>();
			}
			this.StopInternal();
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.Stopped;
			return true;
		}

		private void StopInternal()
		{
			OpenXRLoaderBase.Internal_EndSession();
			this.ProcessOpenXRMessageLoop();
		}

		public override bool Deinitialize()
		{
			if (this.currentLoaderState == OpenXRLoaderBase.LoaderState.Uninitialized)
			{
				return true;
			}
			if (!this.validLoaderDeinitStates.Contains(this.currentLoaderState))
			{
				return false;
			}
			this.currentLoaderState = OpenXRLoaderBase.LoaderState.DeinitializeAttempted;
			bool result;
			try
			{
				OpenXRLoaderBase.Internal_RequestExitSession();
				Application.onBeforeRender -= this.ProcessOpenXRMessageLoop;
				this.ProcessOpenXRMessageLoop();
				OpenXRFeature.ReceiveLoaderEvent(this, OpenXRFeature.LoaderEvent.SubsystemDestroy);
				this.DestroySubsystem<XRInputSubsystem>();
				this.DestroySubsystem<XRDisplaySubsystem>();
				DiagnosticReport.DumpReport("System Shutdown");
				OpenXRLoaderBase.Internal_DestroySession();
				this.ProcessOpenXRMessageLoop();
				OpenXRLoaderBase.Internal_UnloadOpenXRLibrary();
				this.currentLoaderState = OpenXRLoaderBase.LoaderState.Uninitialized;
				this.actionSetsAttached = false;
				if (this.unhandledExceptionHandler != null)
				{
					AppDomain.CurrentDomain.UnhandledException -= this.unhandledExceptionHandler;
					this.unhandledExceptionHandler = null;
				}
				result = base.Deinitialize();
			}
			finally
			{
				OpenXRLoaderBase.Instance = null;
			}
			return result;
		}

		internal new void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) where TDescriptor : ISubsystemDescriptor where TSubsystem : ISubsystem
		{
			base.CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);
		}

		internal new void StartSubsystem<T>() where T : class, ISubsystem
		{
			base.StartSubsystem<T>();
		}

		internal new void StopSubsystem<T>() where T : class, ISubsystem
		{
			base.StopSubsystem<T>();
		}

		internal new void DestroySubsystem<T>() where T : class, ISubsystem
		{
			base.DestroySubsystem<T>();
		}

		private void SetApplicationInfo()
		{
			byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Application.version));
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse<byte>(array);
			}
			uint applicationVersionHash = BitConverter.ToUInt32(array, 0);
			OpenXRLoaderBase.Internal_SetApplicationInfo(Application.productName, Application.version, applicationVersionHash, Application.unityVersion);
		}

		internal static byte[] StringToWCHAR_T(string s)
		{
			return ((Environment.OSVersion.Platform == PlatformID.Unix) ? Encoding.UTF32 : Encoding.Unicode).GetBytes(s + "\0");
		}

		private bool LoadOpenXRSymbols()
		{
			return OpenXRLoaderBase.Internal_LoadOpenXRLibrary(OpenXRLoaderBase.StringToWCHAR_T("openxr_loader"));
		}

		private void RequestOpenXRFeatures()
		{
			OpenXRSettings instance = OpenXRSettings.Instance;
			if (instance == null || instance.features == null)
			{
				return;
			}
			this.featureLoggingInfo = new List<OpenXRLoaderBase.FeatureLoggingInfo>(instance.featureCount);
			foreach (OpenXRFeature openXRFeature in instance.features)
			{
				if (!(openXRFeature == null) && openXRFeature.enabled)
				{
					this.featureLoggingInfo.Add(new OpenXRLoaderBase.FeatureLoggingInfo(openXRFeature.nameUi, openXRFeature.version, openXRFeature.company, openXRFeature.openxrExtensionStrings));
					if (!string.IsNullOrEmpty(openXRFeature.openxrExtensionStrings))
					{
						foreach (string text in openXRFeature.openxrExtensionStrings.Split(' ', StringSplitOptions.None))
						{
							if (!string.IsNullOrWhiteSpace(text))
							{
								OpenXRLoaderBase.Internal_RequestEnableExtensionString(text);
							}
						}
					}
				}
			}
		}

		private void LogRequestedOpenXRFeatures()
		{
			OpenXRSettings instance = OpenXRSettings.Instance;
			if (instance == null || instance.features == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder("");
			StringBuilder stringBuilder2 = new StringBuilder("");
			uint num = 0U;
			uint num2 = 0U;
			foreach (OpenXRLoaderBase.FeatureLoggingInfo featureLoggingInfo in this.featureLoggingInfo)
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					"  ",
					featureLoggingInfo.m_nameUi,
					": Version=",
					featureLoggingInfo.m_version,
					", Company=\"",
					featureLoggingInfo.m_company,
					"\""
				}));
				if (!string.IsNullOrEmpty(featureLoggingInfo.m_openxrExtensionStrings))
				{
					stringBuilder.Append(", Extensions=\"" + featureLoggingInfo.m_openxrExtensionStrings + "\"");
					foreach (string text in featureLoggingInfo.m_openxrExtensionStrings.Split(' ', StringSplitOptions.None))
					{
						if (!string.IsNullOrWhiteSpace(text) && !OpenXRLoaderBase.Internal_IsExtensionEnabled(text))
						{
							num2 += 1U;
							stringBuilder2.Append(string.Concat(new string[]
							{
								"  ",
								text,
								": Feature=\"",
								featureLoggingInfo.m_nameUi,
								"\": Version=",
								featureLoggingInfo.m_version,
								", Company=\"",
								featureLoggingInfo.m_company,
								"\"\n"
							}));
						}
					}
				}
				stringBuilder.Append("\n");
			}
			ulong section = DiagnosticReport.GetSection("OpenXR Runtime Info");
			DiagnosticReport.AddSectionBreak(section);
			DiagnosticReport.AddSectionEntry(section, "Features requested to be enabled", string.Format("({0})\n{1}", num, stringBuilder.ToString()));
			DiagnosticReport.AddSectionBreak(section);
			DiagnosticReport.AddSectionEntry(section, "Requested feature extensions not supported by runtime", string.Format("({0})\n{1}", num2, stringBuilder2.ToString()));
		}

		private static void DebugLogEnabledSpecExtensions()
		{
			ulong section = DiagnosticReport.GetSection("OpenXR Runtime Info");
			DiagnosticReport.AddSectionBreak(section);
			string[] enabledExtensions = OpenXRRuntime.GetEnabledExtensions();
			StringBuilder stringBuilder = new StringBuilder(string.Format("({0})\n", enabledExtensions.Length));
			foreach (string text in enabledExtensions)
			{
				stringBuilder.Append(string.Format("  {0}: Version={1}\n", text, OpenXRRuntime.GetExtensionVersion(text)));
			}
			DiagnosticReport.AddSectionEntry(section, "Runtime extensions enabled", stringBuilder.ToString());
		}

		[MonoPInvokeCallback(typeof(OpenXRLoaderBase.ReceiveNativeEventDelegate))]
		private static void ReceiveNativeEvent(OpenXRFeature.NativeEvent e, ulong payload)
		{
			OpenXRLoaderBase instance = OpenXRLoaderBase.Instance;
			if (instance != null)
			{
				instance.currentOpenXRState = e;
			}
			if (e != OpenXRFeature.NativeEvent.XrBeginSession)
			{
				switch (e)
				{
				case OpenXRFeature.NativeEvent.XrReady:
					instance.StartInternal();
					break;
				case OpenXRFeature.NativeEvent.XrFocused:
					DiagnosticReport.DumpReport("System Startup Completed");
					break;
				case OpenXRFeature.NativeEvent.XrStopping:
					instance.StopInternal();
					break;
				case OpenXRFeature.NativeEvent.XrRestartRequested:
					OpenXRRestarter.Instance.ShutdownAndRestart();
					break;
				case OpenXRFeature.NativeEvent.XrRequestRestartLoop:
					Debug.Log("XR Initialization failed, will try to restart xr periodically.");
					OpenXRRestarter.Instance.PauseAndShutdownAndRestart();
					break;
				case OpenXRFeature.NativeEvent.XrRequestGetSystemLoop:
					OpenXRRestarter.Instance.PauseAndRetryInitialization();
					break;
				}
			}
			else
			{
				instance.LogRequestedOpenXRFeatures();
			}
			OpenXRFeature.ReceiveNativeEvent(e, payload);
			if ((instance == null || !instance.isStarted) && e != OpenXRFeature.NativeEvent.XrInstanceChanged)
			{
				return;
			}
			switch (e)
			{
			case OpenXRFeature.NativeEvent.XrExiting:
				OpenXRRestarter.Instance.Shutdown();
				return;
			case OpenXRFeature.NativeEvent.XrLossPending:
				OpenXRRestarter.Instance.ShutdownAndRestart();
				return;
			case OpenXRFeature.NativeEvent.XrInstanceLossPending:
				OpenXRRestarter.Instance.Shutdown();
				return;
			default:
				return;
			}
		}

		internal static void RegisterOpenXRCallbacks()
		{
			OpenXRLoaderBase.Internal_SetCallbacks(new OpenXRLoaderBase.ReceiveNativeEventDelegate(OpenXRLoaderBase.ReceiveNativeEvent));
		}

		[DllImport("UnityOpenXR", EntryPoint = "main_LoadOpenXRLibrary")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_LoadOpenXRLibrary(byte[] loaderPath);

		[DllImport("UnityOpenXR", EntryPoint = "main_UnloadOpenXRLibrary")]
		internal static extern void Internal_UnloadOpenXRLibrary();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetCallbacks")]
		private static extern void Internal_SetCallbacks(OpenXRLoaderBase.ReceiveNativeEventDelegate callback);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "NativeConfig_SetApplicationInfo")]
		private static extern void Internal_SetApplicationInfo(string applicationName, string applicationVersion, uint applicationVersionHash, string engineVersion);

		[DllImport("UnityOpenXR", EntryPoint = "session_RequestExitSession")]
		internal static extern void Internal_RequestExitSession();

		[DllImport("UnityOpenXR", EntryPoint = "session_InitializeSession")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_InitializeSession();

		[DllImport("UnityOpenXR", EntryPoint = "session_CreateSessionIfNeeded")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_CreateSessionIfNeeded();

		[DllImport("UnityOpenXR", EntryPoint = "session_BeginSession")]
		internal static extern void Internal_BeginSession();

		[DllImport("UnityOpenXR", EntryPoint = "session_EndSession")]
		internal static extern void Internal_EndSession();

		[DllImport("UnityOpenXR", EntryPoint = "session_DestroySession")]
		internal static extern void Internal_DestroySession();

		[DllImport("UnityOpenXR", EntryPoint = "messagepump_PumpMessageLoop")]
		private static extern void Internal_PumpMessageLoop();

		[DllImport("UnityOpenXR", EntryPoint = "session_SetSuccessfullyInitialized")]
		internal static extern void Internal_SetSuccessfullyInitialized([MarshalAs(UnmanagedType.I1)] bool value);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "unity_ext_RequestEnableExtensionString")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_RequestEnableExtensionString(string extensionString);

		[DllImport("UnityOpenXR", EntryPoint = "unity_ext_IsExtensionEnabled")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_IsExtensionEnabled(string extensionName);

		private List<OpenXRLoaderBase.FeatureLoggingInfo> featureLoggingInfo;

		private const double k_IdlePollingWaitTimeInSeconds = 0.1;

		private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();

		private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

		private List<OpenXRLoaderBase.LoaderState> validLoaderInitStates = new List<OpenXRLoaderBase.LoaderState>
		{
			OpenXRLoaderBase.LoaderState.Uninitialized,
			OpenXRLoaderBase.LoaderState.InitializeAttempted
		};

		private List<OpenXRLoaderBase.LoaderState> validLoaderStartStates = new List<OpenXRLoaderBase.LoaderState>
		{
			OpenXRLoaderBase.LoaderState.Initialized,
			OpenXRLoaderBase.LoaderState.StartAttempted,
			OpenXRLoaderBase.LoaderState.Stopped
		};

		private List<OpenXRLoaderBase.LoaderState> validLoaderStopStates = new List<OpenXRLoaderBase.LoaderState>
		{
			OpenXRLoaderBase.LoaderState.StartAttempted,
			OpenXRLoaderBase.LoaderState.Started,
			OpenXRLoaderBase.LoaderState.StopAttempted
		};

		private List<OpenXRLoaderBase.LoaderState> validLoaderDeinitStates = new List<OpenXRLoaderBase.LoaderState>
		{
			OpenXRLoaderBase.LoaderState.InitializeAttempted,
			OpenXRLoaderBase.LoaderState.Initialized,
			OpenXRLoaderBase.LoaderState.Stopped,
			OpenXRLoaderBase.LoaderState.DeinitializeAttempted
		};

		private List<OpenXRLoaderBase.LoaderState> runningStates = new List<OpenXRLoaderBase.LoaderState>
		{
			OpenXRLoaderBase.LoaderState.Initialized,
			OpenXRLoaderBase.LoaderState.StartAttempted,
			OpenXRLoaderBase.LoaderState.Started
		};

		private OpenXRFeature.NativeEvent currentOpenXRState;

		private bool actionSetsAttached;

		private UnhandledExceptionEventHandler unhandledExceptionHandler;

		internal bool DisableValidationChecksOnEnteringPlaymode;

		private double lastPollCheckTime;

		private const string LibraryName = "UnityOpenXR";

		private class FeatureLoggingInfo
		{
			public FeatureLoggingInfo(string nameUi, string version, string company, string extensionStrings)
			{
				this.m_nameUi = nameUi;
				this.m_version = version;
				this.m_company = company;
				this.m_openxrExtensionStrings = extensionStrings;
			}

			public string m_nameUi;

			public string m_version;

			public string m_company;

			public string m_openxrExtensionStrings;
		}

		internal enum LoaderState
		{
			Uninitialized,
			InitializeAttempted,
			Initialized,
			StartAttempted,
			Started,
			StopAttempted,
			Stopped,
			DeinitializeAttempted
		}

		internal delegate void ReceiveNativeEventDelegate(OpenXRFeature.NativeEvent e, ulong payload);
	}
}
