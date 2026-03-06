using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
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

		public static string RuntimePath()
		{
			string result;
			try
			{
				uint num = 512U;
				uint num2 = 512U;
				StringBuilder stringBuilder = new StringBuilder((int)num);
				if (!OpenVRInterop.GetRuntimePath(stringBuilder, num, ref num2))
				{
					result = null;
				}
				else
				{
					result = stringBuilder.ToString();
				}
			}
			catch
			{
				result = OpenVRInterop.RuntimePath();
			}
			return result;
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

		public static CVRHeadsetView HeadsetView
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRHeadsetView();
			}
		}

		public static CVROverlay Overlay
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VROverlay();
			}
		}

		public static CVROverlayView OverlayView
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VROverlayView();
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

		public static CVRIOBuffer IOBuffer
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRIOBuffer();
			}
		}

		public static CVRSpatialAnchors SpatialAnchors
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRSpatialAnchors();
			}
		}

		public static CVRNotifications Notifications
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRNotifications();
			}
		}

		public static CVRDebug Debug
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRDebug();
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
			if (!OpenVR.IsInterfaceVersionValid("IVRSystem_022"))
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

		public const ulong k_ulSharedTextureIsNTHandle = 4294967296UL;

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

		public const uint k_unErrorPropertyTag = 6U;

		public const uint k_unDoublePropertyTag = 7U;

		public const uint k_unHmdMatrix34PropertyTag = 20U;

		public const uint k_unHmdMatrix44PropertyTag = 21U;

		public const uint k_unHmdVector3PropertyTag = 22U;

		public const uint k_unHmdVector4PropertyTag = 23U;

		public const uint k_unHmdVector2PropertyTag = 24U;

		public const uint k_unHmdQuadPropertyTag = 25U;

		public const uint k_unHiddenAreaPropertyTag = 30U;

		public const uint k_unPathHandleInfoTag = 31U;

		public const uint k_unActionPropertyTag = 32U;

		public const uint k_unInputValuePropertyTag = 33U;

		public const uint k_unWildcardPropertyTag = 34U;

		public const uint k_unHapticVibrationPropertyTag = 35U;

		public const uint k_unSkeletonPropertyTag = 36U;

		public const uint k_unSpatialAnchorPosePropertyTag = 40U;

		public const uint k_unJsonPropertyTag = 41U;

		public const uint k_unActiveActionSetPropertyTag = 42U;

		public const uint k_unOpenVRInternalReserved_Start = 1000U;

		public const uint k_unOpenVRInternalReserved_End = 10000U;

		public const uint k_unMaxPropertyStringSize = 32768U;

		public const ulong k_ulInvalidActionHandle = 0UL;

		public const ulong k_ulInvalidActionSetHandle = 0UL;

		public const ulong k_ulInvalidInputValueHandle = 0UL;

		public const uint k_unControllerStateAxisCount = 5U;

		public const ulong k_ulOverlayHandleInvalid = 0UL;

		public const uint k_unMaxDistortionFunctionParameters = 8U;

		public const uint k_unScreenshotHandleInvalid = 0U;

		public const string IVRSystem_Version = "IVRSystem_022";

		public const string IVRExtendedDisplay_Version = "IVRExtendedDisplay_001";

		public const string IVRTrackedCamera_Version = "IVRTrackedCamera_006";

		public const uint k_unMaxApplicationKeyLength = 128U;

		public const string k_pch_MimeType_HomeApp = "vr/home";

		public const string k_pch_MimeType_GameTheater = "vr/game_theater";

		public const string IVRApplications_Version = "IVRApplications_007";

		public const string IVRChaperone_Version = "IVRChaperone_004";

		public const string IVRChaperoneSetup_Version = "IVRChaperoneSetup_006";

		public const string IVRCompositor_Version = "IVRCompositor_026";

		public const uint k_unVROverlayMaxKeyLength = 128U;

		public const uint k_unVROverlayMaxNameLength = 128U;

		public const uint k_unMaxOverlayCount = 128U;

		public const uint k_unMaxOverlayIntersectionMaskPrimitivesCount = 32U;

		public const string IVROverlay_Version = "IVROverlay_024";

		public const string IVROverlayView_Version = "IVROverlayView_003";

		public const uint k_unHeadsetViewMaxWidth = 3840U;

		public const uint k_unHeadsetViewMaxHeight = 2160U;

		public const string k_pchHeadsetViewOverlayKey = "system.HeadsetView";

		public const string IVRHeadsetView_Version = "IVRHeadsetView_001";

		public const string k_pch_Controller_Component_GDC2015 = "gdc2015";

		public const string k_pch_Controller_Component_Base = "base";

		public const string k_pch_Controller_Component_Tip = "tip";

		public const string k_pch_Controller_Component_HandGrip = "handgrip";

		public const string k_pch_Controller_Component_Status = "status";

		public const string IVRRenderModels_Version = "IVRRenderModels_006";

		public const uint k_unNotificationTextMaxSize = 256U;

		public const string IVRNotifications_Version = "IVRNotifications_002";

		public const uint k_unMaxSettingsKeyLength = 128U;

		public const string IVRSettings_Version = "IVRSettings_003";

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

		public const string k_pch_SteamVR_TrackingLossColor_String = "trackingLossColor";

		public const string k_pch_SteamVR_ShowStage_Bool = "showStage";

		public const string k_pch_SteamVR_ActivateMultipleDrivers_Bool = "activateMultipleDrivers";

		public const string k_pch_SteamVR_UsingSpeakers_Bool = "usingSpeakers";

		public const string k_pch_SteamVR_SpeakersForwardYawOffsetDegrees_Float = "speakersForwardYawOffsetDegrees";

		public const string k_pch_SteamVR_BaseStationPowerManagement_Int32 = "basestationPowerManagement";

		public const string k_pch_SteamVR_ShowBaseStationPowerManagementTip_Int32 = "ShowBaseStationPowerManagementTip";

		public const string k_pch_SteamVR_NeverKillProcesses_Bool = "neverKillProcesses";

		public const string k_pch_SteamVR_SupersampleScale_Float = "supersampleScale";

		public const string k_pch_SteamVR_MaxRecommendedResolution_Int32 = "maxRecommendedResolution";

		public const string k_pch_SteamVR_MotionSmoothing_Bool = "motionSmoothing";

		public const string k_pch_SteamVR_MotionSmoothingOverride_Int32 = "motionSmoothingOverride";

		public const string k_pch_SteamVR_DisableAsyncReprojection_Bool = "disableAsync";

		public const string k_pch_SteamVR_ForceFadeOnBadTracking_Bool = "forceFadeOnBadTracking";

		public const string k_pch_SteamVR_DefaultMirrorView_Int32 = "mirrorView";

		public const string k_pch_SteamVR_ShowLegacyMirrorView_Bool = "showLegacyMirrorView";

		public const string k_pch_SteamVR_MirrorViewVisibility_Bool = "showMirrorView";

		public const string k_pch_SteamVR_MirrorViewDisplayMode_Int32 = "mirrorViewDisplayMode";

		public const string k_pch_SteamVR_MirrorViewEye_Int32 = "mirrorViewEye";

		public const string k_pch_SteamVR_MirrorViewGeometry_String = "mirrorViewGeometry";

		public const string k_pch_SteamVR_MirrorViewGeometryMaximized_String = "mirrorViewGeometryMaximized";

		public const string k_pch_SteamVR_PerfGraphVisibility_Bool = "showPerfGraph";

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

		public const string k_pch_SteamVR_DebugInputBinding = "debugInputBinding";

		public const string k_pch_SteamVR_DoNotFadeToGrid = "doNotFadeToGrid";

		public const string k_pch_SteamVR_RenderCameraMode = "renderCameraMode";

		public const string k_pch_SteamVR_EnableSharedResourceJournaling = "enableSharedResourceJournaling";

		public const string k_pch_SteamVR_EnableSafeMode = "enableSafeMode";

		public const string k_pch_SteamVR_PreferredRefreshRate = "preferredRefreshRate";

		public const string k_pch_SteamVR_LastVersionNotice = "lastVersionNotice";

		public const string k_pch_SteamVR_LastVersionNoticeDate = "lastVersionNoticeDate";

		public const string k_pch_SteamVR_HmdDisplayColorGainR_Float = "hmdDisplayColorGainR";

		public const string k_pch_SteamVR_HmdDisplayColorGainG_Float = "hmdDisplayColorGainG";

		public const string k_pch_SteamVR_HmdDisplayColorGainB_Float = "hmdDisplayColorGainB";

		public const string k_pch_SteamVR_CustomIconStyle_String = "customIconStyle";

		public const string k_pch_SteamVR_CustomOffIconStyle_String = "customOffIconStyle";

		public const string k_pch_SteamVR_CustomIconForceUpdate_String = "customIconForceUpdate";

		public const string k_pch_SteamVR_AllowGlobalActionSetPriority = "globalActionSetPriority";

		public const string k_pch_SteamVR_OverlayRenderQuality = "overlayRenderQuality_2";

		public const string k_pch_SteamVR_BlockOculusSDKOnOpenVRLaunchOption_Bool = "blockOculusSDKOnOpenVRLaunchOption";

		public const string k_pch_SteamVR_BlockOculusSDKOnAllLaunches_Bool = "blockOculusSDKOnAllLaunches";

		public const string k_pch_DirectMode_Section = "direct_mode";

		public const string k_pch_DirectMode_Enable_Bool = "enable";

		public const string k_pch_DirectMode_Count_Int32 = "count";

		public const string k_pch_DirectMode_EdidVid_Int32 = "edidVid";

		public const string k_pch_DirectMode_EdidPid_Int32 = "edidPid";

		public const string k_pch_Lighthouse_Section = "driver_lighthouse";

		public const string k_pch_Lighthouse_DisableIMU_Bool = "disableimu";

		public const string k_pch_Lighthouse_DisableIMUExceptHMD_Bool = "disableimuexcepthmd";

		public const string k_pch_Lighthouse_UseDisambiguation_String = "usedisambiguation";

		public const string k_pch_Lighthouse_DisambiguationDebug_Int32 = "disambiguationdebug";

		public const string k_pch_Lighthouse_PrimaryBasestation_Int32 = "primarybasestation";

		public const string k_pch_Lighthouse_DBHistory_Bool = "dbhistory";

		public const string k_pch_Lighthouse_EnableBluetooth_Bool = "enableBluetooth";

		public const string k_pch_Lighthouse_PowerManagedBaseStations_String = "PowerManagedBaseStations";

		public const string k_pch_Lighthouse_PowerManagedBaseStations2_String = "PowerManagedBaseStations2";

		public const string k_pch_Lighthouse_InactivityTimeoutForBaseStations_Int32 = "InactivityTimeoutForBaseStations";

		public const string k_pch_Lighthouse_EnableImuFallback_Bool = "enableImuFallback";

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

		public const string k_pch_WindowsMR_Section = "driver_holographic";

		public const string k_pch_UserInterface_Section = "userinterface";

		public const string k_pch_UserInterface_StatusAlwaysOnTop_Bool = "StatusAlwaysOnTop";

		public const string k_pch_UserInterface_MinimizeToTray_Bool = "MinimizeToTray";

		public const string k_pch_UserInterface_HidePopupsWhenStatusMinimized_Bool = "HidePopupsWhenStatusMinimized";

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

		public const string k_pch_Perf_PerfGraphInHMD_Bool = "perfGraphInHMD";

		public const string k_pch_Perf_AllowTimingStore_Bool = "allowTimingStore";

		public const string k_pch_Perf_SaveTimingsOnExit_Bool = "saveTimingsOnExit";

		public const string k_pch_Perf_TestData_Float = "perfTestData";

		public const string k_pch_Perf_GPUProfiling_Bool = "GPUProfiling";

		public const string k_pch_CollisionBounds_Section = "collisionBounds";

		public const string k_pch_CollisionBounds_Style_Int32 = "CollisionBoundsStyle";

		public const string k_pch_CollisionBounds_GroundPerimeterOn_Bool = "CollisionBoundsGroundPerimeterOn";

		public const string k_pch_CollisionBounds_CenterMarkerOn_Bool = "CollisionBoundsCenterMarkerOn";

		public const string k_pch_CollisionBounds_PlaySpaceOn_Bool = "CollisionBoundsPlaySpaceOn";

		public const string k_pch_CollisionBounds_FadeDistance_Float = "CollisionBoundsFadeDistance";

		public const string k_pch_CollisionBounds_WallHeight_Float = "CollisionBoundsWallHeight";

		public const string k_pch_CollisionBounds_ColorGammaR_Int32 = "CollisionBoundsColorGammaR";

		public const string k_pch_CollisionBounds_ColorGammaG_Int32 = "CollisionBoundsColorGammaG";

		public const string k_pch_CollisionBounds_ColorGammaB_Int32 = "CollisionBoundsColorGammaB";

		public const string k_pch_CollisionBounds_ColorGammaA_Int32 = "CollisionBoundsColorGammaA";

		public const string k_pch_CollisionBounds_EnableDriverImport = "enableDriverBoundsImport";

		public const string k_pch_Camera_Section = "camera";

		public const string k_pch_Camera_EnableCamera_Bool = "enableCamera";

		public const string k_pch_Camera_ShowOnController_Bool = "showOnController";

		public const string k_pch_Camera_EnableCameraForCollisionBounds_Bool = "enableCameraForCollisionBounds";

		public const string k_pch_Camera_RoomView_Int32 = "roomView";

		public const string k_pch_Camera_BoundsColorGammaR_Int32 = "cameraBoundsColorGammaR";

		public const string k_pch_Camera_BoundsColorGammaG_Int32 = "cameraBoundsColorGammaG";

		public const string k_pch_Camera_BoundsColorGammaB_Int32 = "cameraBoundsColorGammaB";

		public const string k_pch_Camera_BoundsColorGammaA_Int32 = "cameraBoundsColorGammaA";

		public const string k_pch_Camera_BoundsStrength_Int32 = "cameraBoundsStrength";

		public const string k_pch_Camera_RoomViewStyle_Int32 = "roomViewStyle";

		public const string k_pch_audio_Section = "audio";

		public const string k_pch_audio_SetOsDefaultPlaybackDevice_Bool = "setOsDefaultPlaybackDevice";

		public const string k_pch_audio_EnablePlaybackDeviceOverride_Bool = "enablePlaybackDeviceOverride";

		public const string k_pch_audio_PlaybackDeviceOverride_String = "playbackDeviceOverride";

		public const string k_pch_audio_PlaybackDeviceOverrideName_String = "playbackDeviceOverrideName";

		public const string k_pch_audio_SetOsDefaultRecordingDevice_Bool = "setOsDefaultRecordingDevice";

		public const string k_pch_audio_EnableRecordingDeviceOverride_Bool = "enableRecordingDeviceOverride";

		public const string k_pch_audio_RecordingDeviceOverride_String = "recordingDeviceOverride";

		public const string k_pch_audio_RecordingDeviceOverrideName_String = "recordingDeviceOverrideName";

		public const string k_pch_audio_EnablePlaybackMirror_Bool = "enablePlaybackMirror";

		public const string k_pch_audio_PlaybackMirrorDevice_String = "playbackMirrorDevice";

		public const string k_pch_audio_PlaybackMirrorDeviceName_String = "playbackMirrorDeviceName";

		public const string k_pch_audio_OldPlaybackMirrorDevice_String = "onPlaybackMirrorDevice";

		public const string k_pch_audio_ActiveMirrorDevice_String = "activePlaybackMirrorDevice";

		public const string k_pch_audio_EnablePlaybackMirrorIndependentVolume_Bool = "enablePlaybackMirrorIndependentVolume";

		public const string k_pch_audio_LastHmdPlaybackDeviceId_String = "lastHmdPlaybackDeviceId";

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

		public const string k_pch_Dashboard_Position = "position";

		public const string k_pch_Dashboard_DesktopScale = "desktopScale";

		public const string k_pch_Dashboard_DashboardScale = "dashboardScale";

		public const string k_pch_modelskin_Section = "modelskins";

		public const string k_pch_Driver_Enable_Bool = "enable";

		public const string k_pch_Driver_BlockedBySafemode_Bool = "blocked_by_safe_mode";

		public const string k_pch_Driver_LoadPriority_Int32 = "loadPriority";

		public const string k_pch_WebInterface_Section = "WebInterface";

		public const string k_pch_VRWebHelper_Section = "VRWebHelper";

		public const string k_pch_VRWebHelper_DebuggerEnabled_Bool = "DebuggerEnabled";

		public const string k_pch_VRWebHelper_DebuggerPort_Int32 = "DebuggerPort";

		public const string k_pch_TrackingOverride_Section = "TrackingOverrides";

		public const string k_pch_App_BindingAutosaveURLSuffix_String = "AutosaveURL";

		public const string k_pch_App_BindingLegacyAPISuffix_String = "_legacy";

		public const string k_pch_App_BindingSteamVRInputAPISuffix_String = "_steamvrinput";

		public const string k_pch_App_BindingCurrentURLSuffix_String = "CurrentURL";

		public const string k_pch_App_BindingPreviousURLSuffix_String = "PreviousURL";

		public const string k_pch_App_NeedToUpdateAutosaveSuffix_Bool = "NeedToUpdateAutosave";

		public const string k_pch_App_DominantHand_Int32 = "DominantHand";

		public const string k_pch_App_BlockOculusSDK_Bool = "blockOculusSDK";

		public const string k_pch_Trackers_Section = "trackers";

		public const string k_pch_DesktopUI_Section = "DesktopUI";

		public const string k_pch_LastKnown_Section = "LastKnown";

		public const string k_pch_LastKnown_HMDManufacturer_String = "HMDManufacturer";

		public const string k_pch_LastKnown_HMDModel_String = "HMDModel";

		public const string k_pch_DismissedWarnings_Section = "DismissedWarnings";

		public const string k_pch_Input_Section = "input";

		public const string k_pch_Input_LeftThumbstickRotation_Float = "leftThumbstickRotation";

		public const string k_pch_Input_RightThumbstickRotation_Float = "rightThumbstickRotation";

		public const string k_pch_Input_ThumbstickDeadzone_Float = "thumbstickDeadzone";

		public const string k_pch_GpuSpeed_Section = "GpuSpeed";

		public const string IVRScreenshots_Version = "IVRScreenshots_001";

		public const string IVRResources_Version = "IVRResources_001";

		public const string IVRDriverManager_Version = "IVRDriverManager_001";

		public const uint k_unMaxActionNameLength = 64U;

		public const uint k_unMaxActionSetNameLength = 64U;

		public const uint k_unMaxActionOriginCount = 16U;

		public const uint k_unMaxBoneNameLength = 32U;

		public const int k_nActionSetOverlayGlobalPriorityMin = 16777216;

		public const int k_nActionSetOverlayGlobalPriorityMax = 33554431;

		public const int k_nActionSetPriorityReservedMin = 33554432;

		public const string IVRInput_Version = "IVRInput_010";

		public const ulong k_ulInvalidIOBufferHandle = 0UL;

		public const string IVRIOBuffer_Version = "IVRIOBuffer_002";

		public const uint k_ulInvalidSpatialAnchorHandle = 0U;

		public const string IVRSpatialAnchors_Version = "IVRSpatialAnchors_001";

		public const string IVRDebug_Version = "IVRDebug_001";

		public const ulong k_ulDisplayRedirectContainer = 25769803779UL;

		public const string IVRProperties_Version = "IVRProperties_001";

		public const string k_pchPathUserHandRight = "/user/hand/right";

		public const string k_pchPathUserHandLeft = "/user/hand/left";

		public const string k_pchPathUserHandPrimary = "/user/hand/primary";

		public const string k_pchPathUserHandSecondary = "/user/hand/secondary";

		public const string k_pchPathUserHead = "/user/head";

		public const string k_pchPathUserGamepad = "/user/gamepad";

		public const string k_pchPathUserTreadmill = "/user/treadmill";

		public const string k_pchPathUserStylus = "/user/stylus";

		public const string k_pchPathDevices = "/devices";

		public const string k_pchPathDevicePath = "/device_path";

		public const string k_pchPathBestAliasPath = "/best_alias_path";

		public const string k_pchPathBoundTrackerAliasPath = "/bound_tracker_path";

		public const string k_pchPathBoundTrackerRole = "/bound_tracker_role";

		public const string k_pchPathPoseRaw = "/pose/raw";

		public const string k_pchPathPoseTip = "/pose/tip";

		public const string k_pchPathSystemButtonClick = "/input/system/click";

		public const string k_pchPathProximity = "/proximity";

		public const string k_pchPathControllerTypePrefix = "/controller_type/";

		public const string k_pchPathInputProfileSuffix = "/input_profile";

		public const string k_pchPathBindingNameSuffix = "/binding_name";

		public const string k_pchPathBindingUrlSuffix = "/binding_url";

		public const string k_pchPathBindingErrorSuffix = "/binding_error";

		public const string k_pchPathActiveActionSets = "/active_action_sets";

		public const string k_pchPathComponentUpdates = "/total_component_updates";

		public const string k_pchPathUserFootLeft = "/user/foot/left";

		public const string k_pchPathUserFootRight = "/user/foot/right";

		public const string k_pchPathUserShoulderLeft = "/user/shoulder/left";

		public const string k_pchPathUserShoulderRight = "/user/shoulder/right";

		public const string k_pchPathUserElbowLeft = "/user/elbow/left";

		public const string k_pchPathUserElbowRight = "/user/elbow/right";

		public const string k_pchPathUserKneeLeft = "/user/knee/left";

		public const string k_pchPathUserKneeRight = "/user/knee/right";

		public const string k_pchPathUserWaist = "/user/waist";

		public const string k_pchPathUserChest = "/user/chest";

		public const string k_pchPathUserCamera = "/user/camera";

		public const string k_pchPathUserKeyboard = "/user/keyboard";

		public const string k_pchPathClientAppKey = "/client_info/app_key";

		public const ulong k_ulInvalidPathHandle = 0UL;

		public const string IVRPaths_Version = "IVRPaths_001";

		public const string IVRBlockQueue_Version = "IVRBlockQueue_004";

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
				this.m_pVRHeadsetView = null;
				this.m_pVROverlay = null;
				this.m_pVROverlayView = null;
				this.m_pVRRenderModels = null;
				this.m_pVRExtendedDisplay = null;
				this.m_pVRSettings = null;
				this.m_pVRApplications = null;
				this.m_pVRScreenshots = null;
				this.m_pVRTrackedCamera = null;
				this.m_pVRInput = null;
				this.m_pVRIOBuffer = null;
				this.m_pVRSpatialAnchors = null;
				this.m_pVRNotifications = null;
				this.m_pVRDebug = null;
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSystem_022", ref evrinitError);
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperone_004", ref evrinitError);
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperoneSetup_006", ref evrinitError);
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRCompositor_026", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRCompositor = new CVRCompositor(genericInterface);
					}
				}
				return this.m_pVRCompositor;
			}

			public CVRHeadsetView VRHeadsetView()
			{
				this.CheckClear();
				if (this.m_pVRHeadsetView == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRHeadsetView_001", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRHeadsetView = new CVRHeadsetView(genericInterface);
					}
				}
				return this.m_pVRHeadsetView;
			}

			public CVROverlay VROverlay()
			{
				this.CheckClear();
				if (this.m_pVROverlay == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVROverlay_024", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVROverlay = new CVROverlay(genericInterface);
					}
				}
				return this.m_pVROverlay;
			}

			public CVROverlayView VROverlayView()
			{
				this.CheckClear();
				if (this.m_pVROverlayView == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVROverlayView_003", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVROverlayView = new CVROverlayView(genericInterface);
					}
				}
				return this.m_pVROverlayView;
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSettings_003", ref evrinitError);
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRApplications_007", ref evrinitError);
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRTrackedCamera_006", ref evrinitError);
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
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRInput_010", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRInput = new CVRInput(genericInterface);
					}
				}
				return this.m_pVRInput;
			}

			public CVRIOBuffer VRIOBuffer()
			{
				this.CheckClear();
				if (this.m_pVRIOBuffer == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRIOBuffer_002", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRIOBuffer = new CVRIOBuffer(genericInterface);
					}
				}
				return this.m_pVRIOBuffer;
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

			public CVRDebug VRDebug()
			{
				this.CheckClear();
				if (this.m_pVRDebug == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRDebug_001", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRDebug = new CVRDebug(genericInterface);
					}
				}
				return this.m_pVRDebug;
			}

			public CVRNotifications VRNotifications()
			{
				this.CheckClear();
				if (this.m_pVRNotifications == null)
				{
					EVRInitError evrinitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRNotifications_002", ref evrinitError);
					if (genericInterface != IntPtr.Zero && evrinitError == EVRInitError.None)
					{
						this.m_pVRNotifications = new CVRNotifications(genericInterface);
					}
				}
				return this.m_pVRNotifications;
			}

			private CVRSystem m_pVRSystem;

			private CVRChaperone m_pVRChaperone;

			private CVRChaperoneSetup m_pVRChaperoneSetup;

			private CVRCompositor m_pVRCompositor;

			private CVRHeadsetView m_pVRHeadsetView;

			private CVROverlay m_pVROverlay;

			private CVROverlayView m_pVROverlayView;

			private CVRRenderModels m_pVRRenderModels;

			private CVRExtendedDisplay m_pVRExtendedDisplay;

			private CVRSettings m_pVRSettings;

			private CVRApplications m_pVRApplications;

			private CVRScreenshots m_pVRScreenshots;

			private CVRTrackedCamera m_pVRTrackedCamera;

			private CVRInput m_pVRInput;

			private CVRIOBuffer m_pVRIOBuffer;

			private CVRSpatialAnchors m_pVRSpatialAnchors;

			private CVRNotifications m_pVRNotifications;

			private CVRDebug m_pVRDebug;
		}
	}
}
