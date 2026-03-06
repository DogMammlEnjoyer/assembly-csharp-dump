using System;

namespace UnityEngine.XR.Management
{
	public class XRGeneralSettings : ScriptableObject
	{
		public XRManagerSettings Manager
		{
			get
			{
				return this.m_LoaderManagerInstance;
			}
			set
			{
				this.m_LoaderManagerInstance = value;
			}
		}

		public static XRGeneralSettings Instance
		{
			get
			{
				return XRGeneralSettings.s_RuntimeSettingsInstance;
			}
		}

		public XRManagerSettings AssignedSettings
		{
			get
			{
				return this.m_LoaderManagerInstance;
			}
		}

		public bool InitManagerOnStart
		{
			get
			{
				return this.m_InitManagerOnStart;
			}
		}

		private void Awake()
		{
			Debug.Log("XRGeneral Settings awakening...");
			XRGeneralSettings.s_RuntimeSettingsInstance = this;
			Application.quitting += XRGeneralSettings.Quit;
			Object.DontDestroyOnLoad(XRGeneralSettings.s_RuntimeSettingsInstance);
		}

		private static void Quit()
		{
			XRGeneralSettings instance = XRGeneralSettings.Instance;
			if (instance == null)
			{
				return;
			}
			instance.DeInitXRSDK();
		}

		private void Start()
		{
			this.StartXRSDK();
		}

		private void OnDestroy()
		{
			this.DeInitXRSDK();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void AttemptInitializeXRSDKOnLoad()
		{
			XRGeneralSettings instance = XRGeneralSettings.Instance;
			if (instance == null || !instance.InitManagerOnStart)
			{
				return;
			}
			instance.InitXRSDK();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		internal static void AttemptStartXRSDKOnBeforeSplashScreen()
		{
			XRGeneralSettings instance = XRGeneralSettings.Instance;
			if (instance == null || !instance.InitManagerOnStart)
			{
				return;
			}
			instance.StartXRSDK();
		}

		private void InitXRSDK()
		{
			if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.m_LoaderManagerInstance == null || !XRGeneralSettings.Instance.m_InitManagerOnStart)
			{
				return;
			}
			this.m_XRManager = XRGeneralSettings.Instance.m_LoaderManagerInstance;
			if (this.m_XRManager == null)
			{
				Debug.LogError("Assigned GameObject for XR Management loading is invalid. No XR Providers will be automatically loaded.");
				return;
			}
			this.m_XRManager.automaticLoading = false;
			this.m_XRManager.automaticRunning = false;
			this.m_XRManager.InitializeLoaderSync();
			this.m_ProviderIntialized = true;
		}

		private void StartXRSDK()
		{
			if (this.m_XRManager != null && this.m_XRManager.activeLoader != null)
			{
				this.m_XRManager.StartSubsystems();
				this.m_ProviderStarted = true;
			}
		}

		private void StopXRSDK()
		{
			if (this.m_XRManager != null && this.m_XRManager.activeLoader != null)
			{
				this.m_XRManager.StopSubsystems();
				this.m_ProviderStarted = false;
			}
		}

		private void DeInitXRSDK()
		{
			if (this.m_XRManager != null && this.m_XRManager.activeLoader != null)
			{
				this.m_XRManager.DeinitializeLoader();
				this.m_XRManager = null;
				this.m_ProviderIntialized = false;
			}
		}

		public static string k_SettingsKey = "com.unity.xr.management.loader_settings";

		internal static XRGeneralSettings s_RuntimeSettingsInstance = null;

		[SerializeField]
		internal XRManagerSettings m_LoaderManagerInstance;

		[SerializeField]
		[Tooltip("Toggling this on/off will enable/disable the automatic startup of XR at run time.")]
		internal bool m_InitManagerOnStart = true;

		private XRManagerSettings m_XRManager;

		private bool m_ProviderIntialized;

		private bool m_ProviderStarted;
	}
}
