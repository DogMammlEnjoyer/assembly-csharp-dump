using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public class OpenVR
	{
		public static uint InitInternal(ref EVRInitError peError, EVRApplicationType eApplicationType)
		{
			return OpenVRInterop.InitInternal(ref peError, eApplicationType);
		}

		public static uint InitInternal2(ref EVRInitError peError, EVRApplicationType eApplicationType, string pchStartupInfo)
		{
			return OpenVRInterop.InitInternal2(ref peError, eApplicationType, pchStartupInfo);
		}

		public static void ShutdownInternal()
		{
			OpenVRInterop.ShutdownInternal();
		}

		public static bool IsHmdPresent()
		{
			return OpenVRInterop.IsHmdPresent();
		}

		public static bool IsRuntimeInstalled()
		{
			return OpenVRInterop.IsRuntimeInstalled();
		}

		public static string GetStringForHmdError(EVRInitError error)
		{
			return Marshal.PtrToStringAnsi(OpenVRInterop.GetStringForHmdError(error));
		}

		public static IntPtr GetGenericInterface(string pchInterfaceVersion, ref EVRInitError peError)
		{
			return OpenVRInterop.GetGenericInterface(pchInterfaceVersion, ref peError);
		}

		public static bool IsInterfaceVersionValid(string pchInterfaceVersion)
		{
			return OpenVRInterop.IsInterfaceVersionValid(pchInterfaceVersion);
		}

		public static uint GetInitToken()
		{
			return OpenVRInterop.GetInitToken();
		}

		private static uint VRToken { get; set; }

		private static OpenVR.COpenVRContext OpenVRInternal_ModuleContext
		{
			get
			{
				if (OpenVR._OpenVRInternal_ModuleContext == null)
				{
					OpenVR._OpenVRInternal_ModuleContext = new OpenVR.COpenVRContext();
				}
				return OpenVR._OpenVRInternal_ModuleContext;
			}
		}

		public static CVRSystem System
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRSystem();
			}
		}

		public static CVRChaperone Chaperone
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRChaperone();
			}
		}

		public static CVRChaperoneSetup ChaperoneSetup
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRChaperoneSetup();
			}
		}

		public static CVRCompositor Compositor
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRCompositor();
			}
		}

		public static CVROverlay Overlay
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VROverlay();
			}
		}

		public static CVRRenderModels RenderModels
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRRenderModels();
			}
		}

		public static CVRExtendedDisplay ExtendedDisplay
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRExtendedDisplay();
			}
		}

		public static CVRSettings Settings
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRSettings();
			}
		}

		public static CVRApplications Applications
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRApplications();
			}
		}

		public static CVRScreenshots Screenshots
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRScreenshots();
			}
		}

		public static CVRTrackedCamera TrackedCamera
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRTrackedCamera();
			}
		}

		public static CVRInput Input
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRInput();
			}
		}

		public static CVRSpatialAnchors SpatialAnchors
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRSpatialAnchors();
			}
		}

		public static CVRSystem Init(ref EVRInitError peError, EVRApplicationType eApplicationType = EVRApplicationType.VRApplication_Scene, string pchStartupInfo = "")
		{
			try
			{
				OpenVR.VRToken = OpenVR.InitInternal2(ref peError, eApplicationType, pchStartupInfo);
			}
			catch (EntryPointNotFoundException)
			{
				OpenVR.VRToken = OpenVR.InitInternal(ref peError, eApplicationType);
			}
			OpenVR.OpenVRInternal_ModuleContext.Clear();
			if (peError != EVRInitError.None)
			{
				return null;
			}
			if (!OpenVR.IsInterfaceVersionValid("IVRSystem_019"))
			{
				OpenVR.ShutdownInternal();
				peError = EVRInitError.Init_InterfaceNotFound;
				return null;
			}
			return OpenVR.System;
		}

		public static void Shutdown()
		{
			OpenVR.ShutdownInternal();
		}

		public const uint k_nDriverNone = 4294967295U;

		public const uint k_unMaxDriverDebugResponseSize = 32768U;

		public const uint k_unTrackedDeviceIndex_Hmd = 0U;

		public const uint k_unMaxTrackedDeviceCount = 64U;

		public const uint k_unTrackedDeviceIndexOther = 4294967294U;

		public const uint k_unTrackedDeviceIndexInvalid = 4294967295U;

		public const ulong k_ulInvalidPropertyContainer = 0UL;

		public const uint k_unInvalidPropertyTag = 0U;

		public const ulong k_ulInvalidDriverHandle = 0UL;

		public const uint k_unFloatPropertyTag = 1U;

		public const uint k_unInt32PropertyTag = 2U;

		public const uint k_unUint64PropertyTag = 3U;

		public const uint k_unBoolPropertyTag = 4U;

		public const uint k_unStringPropertyTag = 5U;

		public const uint k_unHmdMatrix34PropertyTag = 20U;

		public const uint k_unHmdMatrix44PropertyTag = 21U;

		public const uint k_unHmdVector3PropertyTag = 22U;

		public const uint k_unHmdVector4PropertyTag = 23U;

		public const uint k_unHiddenAreaPropertyTag = 30U;

		public const uint k_unPathHandleInfoTag = 31U;

		public const uint k_unActionPropertyTag = 32U;

		public const uint k_unInputValuePropertyTag = 33U;

		public const uint k_unWildcardPropertyTag = 34U;

		public const uint k_unHapticVibrationPropertyTag = 35U;

		public const uint k_unSkeletonPropertyTag = 36U;

		public const uint k_unSpatialAnchorPosePropertyTag = 40U;

		public const uint k_unOpenVRInternalReserved_Start = 1000U;

		public const uint k_unOpenVRInternalReserved_End = 10000U;

		public const uint k_unMaxPropertyStringSize = 32768U;

		public const ulong k_ulInvalidActionHandle = 0UL;

		public const ulong k_ulInvalidActionSetHandle = 0UL;

		public const ulong k_ulInvalidInputValueHandle = 0UL;

		public const uint k_unControllerStateAxisCount = 5U;

		public const ulong k_ulOverlayHandleInvalid = 0UL;

		public const uint k_unScreenshotHandleInvalid = 0U;

		public const string IVRSystem_Version = "IVRSystem_019";

		public const string IVRExtendedDisplay_Version = "IVRExtendedDisplay_001";

		public const string IVRTrackedCamera_Version = "IVRTrackedCamera_003";

		public const uint k_unMaxApplicationKeyLength = 128U;

		public const string k_pch_MimeType_HomeApp = "vr/home";

		public const string k_pch_MimeType_GameTheater = "vr/game_theater";

		public const string IVRApplications_Version = "IVRApplications_006";

		public const string IVRChaperone_Version = "IVRChaperone_003";

		public const string IVRChaperoneSetup_Version = "IVRChaperoneSetup_005";

		public const string IVRCompositor_Version = "IVRCompositor_022";

		public const uint k_unVROverlayMaxKeyLength = 128U;

		public const uint k_unVROverlayMaxNameLength = 128U;

		public const uint k_unMaxOverlayCount = 64U;

		public const uint k_unMaxOverlayIntersectionMaskPrimitivesCount = 32U;

		public const string IVROverlay_Version = "IVROverlay_018";

		public const string k_pch_Controller_Component_GDC2015 = "gdc2015";

		public const string k_pch_Controller_Component_Base = "base";

		public const string k_pch_Controller_Component_Tip = "tip";

		public const string k_pch_Controller_Component_HandGrip = "handgrip";

		public const string k_pch_Controller_Component_Status = "status";

		public const string IVRRenderModels_Version = "IVRRenderModels_006";

		public const uint k_unNotificationTextMaxSize = 256U;

		public const string IVRNotifications_Version = "IVRNotifications_002";

		public const uint k_unMaxSettingsKeyLength = 128U;

		public const string IVRSettings_Version = "IVRSettings_002";

		public const string k_pch_SteamVR_Section = "steamvr";

		public const string k_pch_SteamVR_RequireHmd_String = "requireHmd";

		public const string k_pch_SteamVR_ForcedDriverKey_String = "forcedDriver";

		public const string k_pch_SteamVR_ForcedHmdKey_String = "forcedHmd";

		public const string k_pch_SteamVR_DisplayDebug_Bool = "displayDebug";

		public const string k_pch_SteamVR_DebugProcessPipe_String = "debugProcessPipe";

		public const string k_pch_SteamVR_DisplayDebugX_Int32 = "displayDebugX";

		public const string k_pch_SteamVR_DisplayDebugY_Int32 = "displayDebugY";

		public const string k_pch_SteamVR_SendSystemButtonToAllApps_Bool = "sendSystemButtonToAllApps";

		public const string k_pch_SteamVR_LogLevel_Int32 = "loglevel";

		public const string k_pch_SteamVR_IPD_Float = "ipd";

		public const string k_pch_SteamVR_Background_String = "background";

		public const string k_pch_SteamVR_BackgroundUseDomeProjection_Bool = "backgroundUseDomeProjection";

		public const string k_pch_SteamVR_BackgroundCameraHeight_Float = "backgroundCameraHeight";

		public const string k_pch_SteamVR_BackgroundDomeRadius_Float = "backgroundDomeRadius";

		public const string k_pch_SteamVR_GridColor_String = "gridColor";

		public const string k_pch_SteamVR_PlayAreaColor_String = "playAreaColor";

		public const string k_pch_SteamVR_ShowStage_Bool = "showStage";

		public const string k_pch_SteamVR_ActivateMultipleDrivers_Bool = "activateMultipleDrivers";

		public const string k_pch_SteamVR_DirectMode_Bool = "directMode";

		public const string k_pch_SteamVR_DirectModeEdidVid_Int32 = "directModeEdidVid";

		public const string k_pch_SteamVR_DirectModeEdidPid_Int32 = "directModeEdidPid";

		public const string k_pch_SteamVR_UsingSpeakers_Bool = "usingSpeakers";

		public const string k_pch_SteamVR_SpeakersForwardYawOffsetDegrees_Float = "speakersForwardYawOffsetDegrees";

		public const string k_pch_SteamVR_BaseStationPowerManagement_Bool = "basestationPowerManagement";

		public const string k_pch_SteamVR_NeverKillProcesses_Bool = "neverKillProcesses";

		public const string k_pch_SteamVR_SupersampleScale_Float = "supersampleScale";

		public const string k_pch_SteamVR_AllowAsyncReprojection_Bool = "allowAsyncReprojection";

		public const string k_pch_SteamVR_AllowReprojection_Bool = "allowInterleavedReprojection";

		public const string k_pch_SteamVR_ForceReprojection_Bool = "forceReprojection";

		public const string k_pch_SteamVR_ForceFadeOnBadTracking_Bool = "forceFadeOnBadTracking";

		public const string k_pch_SteamVR_DefaultMirrorView_Int32 = "defaultMirrorView";

		public const string k_pch_SteamVR_ShowMirrorView_Bool = "showMirrorView";

		public const string k_pch_SteamVR_MirrorViewGeometry_String = "mirrorViewGeometry";

		public const string k_pch_SteamVR_StartMonitorFromAppLaunch = "startMonitorFromAppLaunch";

		public const string k_pch_SteamVR_StartCompositorFromAppLaunch_Bool = "startCompositorFromAppLaunch";

		public const string k_pch_SteamVR_StartDashboardFromAppLaunch_Bool = "startDashboardFromAppLaunch";

		public const string k_pch_SteamVR_StartOverlayAppsFromDashboard_Bool = "startOverlayAppsFromDashboard";

		public const string k_pch_SteamVR_EnableHomeApp = "enableHomeApp";

		public const string k_pch_SteamVR_CycleBackgroundImageTimeSec_Int32 = "CycleBackgroundImageTimeSec";

		public const string k_pch_SteamVR_RetailDemo_Bool = "retailDemo";

		public const string k_pch_SteamVR_IpdOffset_Float = "ipdOffset";

		public const string k_pch_SteamVR_AllowSupersampleFiltering_Bool = "allowSupersampleFiltering";

		public const string k_pch_SteamVR_SupersampleManualOverride_Bool = "supersampleManualOverride";

		public const string k_pch_SteamVR_EnableLinuxVulkanAsync_Bool = "enableLinuxVulkanAsync";

		public const string k_pch_SteamVR_AllowDisplayLockedMode_Bool = "allowDisplayLockedMode";

		public const string k_pch_SteamVR_HaveStartedTutorialForNativeChaperoneDriver_Bool = "haveStartedTutorialForNativeChaperoneDriver";

		public const string k_pch_SteamVR_ForceWindows32bitVRMonitor = "forceWindows32BitVRMonitor";

		public const string k_pch_SteamVR_DebugInput = "debugInput";

		public const string k_pch_SteamVR_LegacyInputRebinding = "legacyInputRebinding";

		public const string k_pch_SteamVR_DebugInputBinding = "debugInputBinding";

		public const string k_pch_SteamVR_InputBindingUIBlock = "inputBindingUI";

		public const string k_pch_SteamVR_RenderCameraMode = "renderCameraMode";

		public const string k_pch_Lighthouse_Section = "driver_lighthouse";

		public const string k_pch_Lighthouse_DisableIMU_Bool = "disableimu";

		public const string k_pch_Lighthouse_DisableIMUExceptHMD_Bool = "disableimuexcepthmd";

		public const string k_pch_Lighthouse_UseDisambiguation_String = "usedisambiguation";

		public const string k_pch_Lighthouse_DisambiguationDebug_Int32 = "disambiguationdebug";

		public const string k_pch_Lighthouse_PrimaryBasestation_Int32 = "primarybasestation";

		public const string k_pch_Lighthouse_DBHistory_Bool = "dbhistory";

		public const string k_pch_Lighthouse_EnableBluetooth_Bool = "enableBluetooth";

		public const string k_pch_Lighthouse_PowerManagedBaseStations_String = "PowerManagedBaseStations";

		public const string k_pch_Null_Section = "driver_null";

		public const string k_pch_Null_SerialNumber_String = "serialNumber";

		public const string k_pch_Null_ModelNumber_String = "modelNumber";

		public const string k_pch_Null_WindowX_Int32 = "windowX";

		public const string k_pch_Null_WindowY_Int32 = "windowY";

		public const string k_pch_Null_WindowWidth_Int32 = "windowWidth";

		public const string k_pch_Null_WindowHeight_Int32 = "windowHeight";

		public const string k_pch_Null_RenderWidth_Int32 = "renderWidth";

		public const string k_pch_Null_RenderHeight_Int32 = "renderHeight";

		public const string k_pch_Null_SecondsFromVsyncToPhotons_Float = "secondsFromVsyncToPhotons";

		public const string k_pch_Null_DisplayFrequency_Float = "displayFrequency";

		public const string k_pch_UserInterface_Section = "userinterface";

		public const string k_pch_UserInterface_StatusAlwaysOnTop_Bool = "StatusAlwaysOnTop";

		public const string k_pch_UserInterface_MinimizeToTray_Bool = "MinimizeToTray";

		public const string k_pch_UserInterface_Screenshots_Bool = "screenshots";

		public const string k_pch_UserInterface_ScreenshotType_Int = "screenshotType";

		public const string k_pch_Notifications_Section = "notifications";

		public const string k_pch_Notifications_DoNotDisturb_Bool = "DoNotDisturb";

		public const string k_pch_Keyboard_Section = "keyboard";

		public const string k_pch_Keyboard_TutorialCompletions = "TutorialCompletions";

		public const string k_pch_Keyboard_ScaleX = "ScaleX";

		public const string k_pch_Keyboard_ScaleY = "ScaleY";

		public const string k_pch_Keyboard_OffsetLeftX = "OffsetLeftX";

		public const string k_pch_Keyboard_OffsetRightX = "OffsetRightX";

		public const string k_pch_Keyboard_OffsetY = "OffsetY";

		public const string k_pch_Keyboard_Smoothing = "Smoothing";

		public const string k_pch_Perf_Section = "perfcheck";

		public const string k_pch_Perf_HeuristicActive_Bool = "heuristicActive";

		public const string k_pch_Perf_NotifyInHMD_Bool = "warnInHMD";

		public const string k_pch_Perf_NotifyOnlyOnce_Bool = "warnOnlyOnce";

		public const string k_pch_Perf_AllowTimingStore_Bool = "allowTimingStore";

		public const string k_pch_Perf_SaveTimingsOnExit_Bool = "saveTimingsOnExit";

		public const string k_pch_Perf_TestData_Float = "perfTestData";

		public const string k_pch_Perf_LinuxGPUProfiling_Bool = "linuxGPUProfiling";

		public const string k_pch_CollisionBounds_Section = "collisionBounds";

		public const string k_pch_CollisionBounds_Style_Int32 = "CollisionBoundsStyle";

		public const string k_pch_CollisionBounds_GroundPerimeterOn_Bool = "CollisionBoundsGroundPerimeterOn";

		public const string k_pch_CollisionBounds_CenterMarkerOn_Bool = "CollisionBoundsCenterMarkerOn";

		public const string k_pch_CollisionBounds_PlaySpaceOn_Bool = "CollisionBoundsPlaySpaceOn";

		public const string k_pch_CollisionBounds_FadeDistance_Float = "CollisionBoundsFadeDistance";

		public const string k_pch_CollisionBounds_ColorGammaR_Int32 = "CollisionBoundsColorGammaR";

		public const string k_pch_CollisionBounds_ColorGammaG_Int32 = "CollisionBoundsColorGammaG";

		public const string k_pch_CollisionBounds_ColorGammaB_Int32 = "CollisionBoundsColorGammaB";

		public const string k_pch_CollisionBounds_ColorGammaA_Int32 = "CollisionBoundsColorGammaA";

		public const string k_pch_Camera_Section = "camera";

		public const string k_pch_Camera_EnableCamera_Bool = "enableCamera";

		public const string k_pch_Camera_EnableCameraInDashboard_Bool = "enableCameraInDashboard";

		public const string k_pch_Camera_EnableCameraForCollisionBounds_Bool = "enableCameraForCollisionBounds";

		public const string k_pch_Camera_EnableCameraForRoomView_Bool = "enableCameraForRoomView";

		public const string k_pch_Camera_BoundsColorGammaR_Int32 = "cameraBoundsColorGammaR";

		public const string k_pch_Camera_BoundsColorGammaG_Int32 = "cameraBoundsColorGammaG";

		public const string k_pch_Camera_BoundsColorGammaB_Int32 = "cameraBoundsColorGammaB";

		public const string k_pch_Camera_BoundsColorGammaA_Int32 = "cameraBoundsColorGammaA";

		public const string k_pch_Camera_BoundsStrength_Int32 = "cameraBoundsStrength";

		public const string k_pch_Camera_RoomViewMode_Int32 = "cameraRoomViewMode";

		public const string k_pch_audio_Section = "audio";

		public const string k_pch_audio_OnPlaybackDevice_String = "onPlaybackDevice";

		public const string k_pch_audio_OnRecordDevice_String = "onRecordDevice";

		public const string k_pch_audio_OnPlaybackMirrorDevice_String = "onPlaybackMirrorDevice";

		public const string k_pch_audio_OffPlaybackDevice_String = "offPlaybackDevice";

		public const string k_pch_audio_OffRecordDevice_String = "offRecordDevice";

		public const string k_pch_audio_VIVEHDMIGain = "viveHDMIGain";

		public const string k_pch_Power_Section = "power";

		public const string k_pch_Power_PowerOffOnExit_Bool = "powerOffOnExit";

		public const string k_pch_Power_TurnOffScreensTimeout_Float = "turnOffScreensTimeout";

		public const string k_pch_Power_TurnOffControllersTimeout_Float = "turnOffControllersTimeout";

		public const string k_pch_Power_ReturnToWatchdogTimeout_Float = "returnToWatchdogTimeout";

		public const string k_pch_Power_AutoLaunchSteamVROnButtonPress = "autoLaunchSteamVROnButtonPress";

		public const string k_pch_Power_PauseCompositorOnStandby_Bool = "pauseCompositorOnStandby";

		public const string k_pch_Dashboard_Section = "dashboard";

		public const string k_pch_Dashboard_EnableDashboard_Bool = "enableDashboard";

		public const string k_pch_Dashboard_ArcadeMode_Bool = "arcadeMode";

		public const string k_pch_Dashboard_EnableWebUI = "webUI";

		public const string k_pch_Dashboard_EnableWebUIDevTools = "webUIDevTools";

		public const string k_pch_Dashboard_EnableWebUIDashboardReplacement = "webUIDashboard";

		public const string k_pch_modelskin_Section = "modelskins";

		public const string k_pch_Driver_Enable_Bool = "enable";

		public const string k_pch_WebInterface_Section = "WebInterface";

		public const string k_pch_WebInterface_WebEnable_Bool = "WebEnable";

		public const string k_pch_WebInterface_WebPort_String = "WebPort";

		public const string k_pch_TrackingOverride_Section = "TrackingOverrides";

		public const string k_pch_App_BindingAutosaveURLSuffix_String = "AutosaveURL";

		public const string k_pch_App_BindingCurrentURLSuffix_String = "CurrentURL";

		public const string k_pch_App_NeedToUpdateAutosaveSuffix_Bool = "NeedToUpdateAutosave";

		public const string k_pch_App_ActionManifestURL_String = "ActionManifestURL";

		public const string k_pch_Trackers_Section = "trackers";

		public const string IVRScreenshots_Version = "IVRScreenshots_001";

		public const string IVRResources_Version = "IVRResources_001";

		public const string IVRDriverManager_Version = "IVRDriverManager_001";

		public const uint k_unMaxActionNameLength = 64U;

		public const uint k_unMaxActionSetNameLength = 64U;

		public const uint k_unMaxActionOriginCount = 16U;

		public const string IVRInput_Version = "IVRInput_004";

		public const ulong k_ulInvalidIOBufferHandle = 0UL;

		public const string IVRIOBuffer_Version = "IVRIOBuffer_001";

		public const uint k_ulInvalidSpatialAnchorHandle = 0U;

		public const string IVRSpatialAnchors_Version = "IVRSpatialAnchors_001";

		private const string FnTable_Prefix = "FnTable:";

		private static OpenVR.COpenVRContext _OpenVRInternal_ModuleContext;

		private class COpenVRContext
		{
			public COpenVRContext()
			{
				this.Clear();
			}

			public void Clear()
			{
				this.m_pVRSystem = null;
				this.m_pVRChaperone = null;
				this.m_pVRChaperoneSetup = null;
				this.m_pVRCompositor = null;
				this.m_pVROverlay = null;
				this.m_pVRRenderModels = null;
				this.m_pVRExtendedDisplay = null;
				this.m_pVRSettings = null;
				this.m_pVRApplications = null;
				this.m_pVRScreenshots = null;
				this.m_pVRTrackedCamera = null;
				this.m_pVRInput = null;
				this.m_pVRSpatialAnchors = null;
			}

			private void CheckClear()
			{
				if (OpenVR.VRToken != OpenVR.GetInitToken())
				{
					this.Clear();
					OpenVR.VRToken = OpenVR.GetInitToken();
				}
			}

			public CVRSystem VRSystem()
			{
				this.CheckClear();
				if (this.m_pVRSystem == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSystem_019", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRSystem = new CVRSystem(genericInterface);
					}
				}
				return this.m_pVRSystem;
			}

			public CVRChaperone VRChaperone()
			{
				this.CheckClear();
				if (this.m_pVRChaperone == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperone_003", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRChaperone = new CVRChaperone(genericInterface);
					}
				}
				return this.m_pVRChaperone;
			}

			public CVRChaperoneSetup VRChaperoneSetup()
			{
				this.CheckClear();
				if (this.m_pVRChaperoneSetup == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperoneSetup_005", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRChaperoneSetup = new CVRChaperoneSetup(genericInterface);
					}
				}
				return this.m_pVRChaperoneSetup;
			}

			public CVRCompositor VRCompositor()
			{
				this.CheckClear();
				if (this.m_pVRCompositor == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRCompositor_022", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRCompositor = new CVRCompositor(genericInterface);
					}
				}
				return this.m_pVRCompositor;
			}

			public CVROverlay VROverlay()
			{
				this.CheckClear();
				if (this.m_pVROverlay == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVROverlay_018", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVROverlay = new CVROverlay(genericInterface);
					}
				}
				return this.m_pVROverlay;
			}

			public CVRRenderModels VRRenderModels()
			{
				this.CheckClear();
				if (this.m_pVRRenderModels == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRRenderModels_006", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRRenderModels = new CVRRenderModels(genericInterface);
					}
				}
				return this.m_pVRRenderModels;
			}

			public CVRExtendedDisplay VRExtendedDisplay()
			{
				this.CheckClear();
				if (this.m_pVRExtendedDisplay == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRExtendedDisplay_001", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRExtendedDisplay = new CVRExtendedDisplay(genericInterface);
					}
				}
				return this.m_pVRExtendedDisplay;
			}

			public CVRSettings VRSettings()
			{
				this.CheckClear();
				if (this.m_pVRSettings == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSettings_002", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRSettings = new CVRSettings(genericInterface);
					}
				}
				return this.m_pVRSettings;
			}

			public CVRApplications VRApplications()
			{
				this.CheckClear();
				if (this.m_pVRApplications == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRApplications_006", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRApplications = new CVRApplications(genericInterface);
					}
				}
				return this.m_pVRApplications;
			}

			public CVRScreenshots VRScreenshots()
			{
				this.CheckClear();
				if (this.m_pVRScreenshots == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRScreenshots_001", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRScreenshots = new CVRScreenshots(genericInterface);
					}
				}
				return this.m_pVRScreenshots;
			}

			public CVRTrackedCamera VRTrackedCamera()
			{
				this.CheckClear();
				if (this.m_pVRTrackedCamera == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRTrackedCamera_003", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRTrackedCamera = new CVRTrackedCamera(genericInterface);
					}
				}
				return this.m_pVRTrackedCamera;
			}

			public CVRInput VRInput()
			{
				this.CheckClear();
				if (this.m_pVRInput == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRInput_004", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRInput = new CVRInput(genericInterface);
					}
				}
				return this.m_pVRInput;
			}

			public CVRSpatialAnchors VRSpatialAnchors()
			{
				this.CheckClear();
				if (this.m_pVRSpatialAnchors == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSpatialAnchors_001", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRSpatialAnchors = new CVRSpatialAnchors(genericInterface);
					}
				}
				return this.m_pVRSpatialAnchors;
			}

			private CVRSystem m_pVRSystem;

			private CVRChaperone m_pVRChaperone;

			private CVRChaperoneSetup m_pVRChaperoneSetup;

			private CVRCompositor m_pVRCompositor;

			private CVROverlay m_pVROverlay;

			private CVRRenderModels m_pVRRenderModels;

			private CVRExtendedDisplay m_pVRExtendedDisplay;

			private CVRSettings m_pVRSettings;

			private CVRApplications m_pVRApplications;

			private CVRScreenshots m_pVRScreenshots;

			private CVRTrackedCamera m_pVRTrackedCamera;

			private CVRInput m_pVRInput;

			private CVRSpatialAnchors m_pVRSpatialAnchors;
		}
	}
}
