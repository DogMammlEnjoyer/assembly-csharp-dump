using System;
using System.Collections.Generic;
using System.Linq;
using Meta.XR;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-add-camera-rig/#configure-settings")]
public class OVRManager : MonoBehaviour, OVRMixedRealityCaptureConfiguration
{
	public static OVRManager instance { get; private set; }

	public static OVRDisplay display { get; private set; }

	public static OVRTracker tracker { get; private set; }

	public static OVRBoundary boundary { get; private set; }

	public static OVRRuntimeSettings runtimeSettings { get; private set; }

	public static OVRProfile profile
	{
		get
		{
			if (OVRManager._profile == null)
			{
				OVRManager._profile = new OVRProfile();
			}
			return OVRManager._profile;
		}
	}

	public static event Action HMDAcquired;

	public static event Action HMDLost;

	public static event Action HMDMounted;

	public static event Action HMDUnmounted;

	public static event Action VrFocusAcquired;

	public static event Action VrFocusLost;

	public static event Action InputFocusAcquired;

	public static event Action InputFocusLost;

	public static event Action AudioOutChanged;

	public static event Action AudioInChanged;

	public static event Action TrackingAcquired;

	public static event Action TrackingLost;

	public static event Action<float, float> DisplayRefreshRateChanged;

	public static event Action<ulong, bool, OVRSpace, Guid> SpatialAnchorCreateComplete;

	public static event Action<ulong, bool, OVRSpace, Guid, OVRPlugin.SpaceComponentType, bool> SpaceSetComponentStatusComplete;

	public static event Action<ulong> SpaceQueryResults;

	public static event Action<ulong, bool> SpaceQueryComplete;

	public static event Action<ulong, OVRSpace, bool, Guid> SpaceSaveComplete;

	public static event Action<ulong, bool, Guid, OVRPlugin.SpaceStorageLocation> SpaceEraseComplete;

	public static event Action<ulong, OVRSpatialAnchor.OperationResult> ShareSpacesComplete;

	public static event Action<ulong, OVRSpatialAnchor.OperationResult> SpaceListSaveComplete;

	public static event Action<ulong, bool> SceneCaptureComplete;

	public static event Action<int> PassthroughLayerResumed;

	public static event Action<OVRPlugin.BoundaryVisibility> BoundaryVisibilityChanged;

	public static event Action<OVRManager.TrackingOrigin, OVRPose?> TrackingOriginChangePending;

	[Obsolete]
	public static event Action HSWDismissed;

	public static bool isHmdPresent
	{
		get
		{
			if (OVRManager._isHmdPresentCacheFrame != Time.frameCount)
			{
				OVRManager._isHmdPresentCacheFrame = Time.frameCount;
				OVRManager._isHmdPresent = OVRNodeStateProperties.IsHmdPresent();
			}
			return OVRManager._isHmdPresent;
		}
	}

	public static string audioOutId
	{
		get
		{
			return OVRPlugin.audioOutId;
		}
	}

	public static string audioInId
	{
		get
		{
			return OVRPlugin.audioInId;
		}
	}

	public static bool hasVrFocus
	{
		get
		{
			if (!OVRManager._hasVrFocusCached)
			{
				OVRManager._hasVrFocusCached = true;
				OVRManager._hasVrFocus = OVRPlugin.hasVrFocus;
			}
			return OVRManager._hasVrFocus;
		}
		private set
		{
			OVRManager._hasVrFocusCached = true;
			OVRManager._hasVrFocus = value;
		}
	}

	public static bool hasInputFocus
	{
		get
		{
			return OVRPlugin.hasInputFocus;
		}
	}

	public bool chromatic
	{
		get
		{
			return OVRManager.isHmdPresent && OVRPlugin.chromatic;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.chromatic = value;
		}
	}

	public bool monoscopic
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return this._monoscopic;
			}
			return OVRPlugin.monoscopic;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.monoscopic = value;
			this._monoscopic = value;
		}
	}

	public OVRPlugin.LayerSharpenType sharpenType
	{
		get
		{
			return this._sharpenType;
		}
		set
		{
			this._sharpenType = value;
			OVRPlugin.SetEyeBufferSharpenType(this._sharpenType);
		}
	}

	public OVRManager.ColorSpace colorGamut
	{
		get
		{
			return this._colorGamut;
		}
		set
		{
			this._colorGamut = value;
			OVRPlugin.SetClientColorDesc((OVRPlugin.ColorSpace)this._colorGamut);
		}
	}

	public OVRManager.ColorSpace nativeColorGamut
	{
		get
		{
			return (OVRManager.ColorSpace)OVRPlugin.GetHmdColorDesc();
		}
	}

	public bool enableDynamicResolution
	{
		get
		{
			return this._enableDynamicResolution;
		}
		set
		{
			this._enableDynamicResolution = value;
		}
	}

	[Obsolete("Deprecated. Use Dynamic Render Scaling instead.", false)]
	public static bool IsAdaptiveResSupportedByEngine()
	{
		return true;
	}

	public Vector3 headPoseRelativeOffsetRotation
	{
		get
		{
			return this._headPoseRelativeOffsetRotation;
		}
		set
		{
			OVRPlugin.Quatf quatf;
			OVRPlugin.Vector3f vector3f;
			if (OVRPlugin.GetHeadPoseModifier(out quatf, out vector3f))
			{
				quatf = Quaternion.Euler(value).ToQuatf();
				OVRPlugin.SetHeadPoseModifier(ref quatf, ref vector3f);
			}
			this._headPoseRelativeOffsetRotation = value;
		}
	}

	public Vector3 headPoseRelativeOffsetTranslation
	{
		get
		{
			return this._headPoseRelativeOffsetTranslation;
		}
		set
		{
			OVRPlugin.Quatf quatf;
			OVRPlugin.Vector3f v;
			if (OVRPlugin.GetHeadPoseModifier(out quatf, out v) && v.FromFlippedZVector3f() != value)
			{
				v = value.ToFlippedZVector3f();
				OVRPlugin.SetHeadPoseModifier(ref quatf, ref v);
			}
			this._headPoseRelativeOffsetTranslation = value;
		}
	}

	[HideInInspector]
	public static bool eyeFovPremultipliedAlphaModeEnabled
	{
		get
		{
			return OVRPlugin.eyeFovPremultipliedAlphaModeEnabled;
		}
		set
		{
			OVRPlugin.eyeFovPremultipliedAlphaModeEnabled = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.enableMixedReality
	{
		get
		{
			return this.enableMixedReality;
		}
		set
		{
			this.enableMixedReality = value;
		}
	}

	LayerMask OVRMixedRealityCaptureConfiguration.extraHiddenLayers
	{
		get
		{
			return this.extraHiddenLayers;
		}
		set
		{
			this.extraHiddenLayers = value;
		}
	}

	LayerMask OVRMixedRealityCaptureConfiguration.extraVisibleLayers
	{
		get
		{
			return this.extraVisibleLayers;
		}
		set
		{
			this.extraVisibleLayers = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.dynamicCullingMask
	{
		get
		{
			return this.dynamicCullingMask;
		}
		set
		{
			this.dynamicCullingMask = value;
		}
	}

	OVRManager.CompositionMethod OVRMixedRealityCaptureConfiguration.compositionMethod
	{
		get
		{
			return this.compositionMethod;
		}
		set
		{
			this.compositionMethod = value;
		}
	}

	Color OVRMixedRealityCaptureConfiguration.externalCompositionBackdropColorRift
	{
		get
		{
			return this.externalCompositionBackdropColorRift;
		}
		set
		{
			this.externalCompositionBackdropColorRift = value;
		}
	}

	Color OVRMixedRealityCaptureConfiguration.externalCompositionBackdropColorQuest
	{
		get
		{
			return this.externalCompositionBackdropColorQuest;
		}
		set
		{
			this.externalCompositionBackdropColorQuest = value;
		}
	}

	[Obsolete("Deprecated", false)]
	OVRManager.CameraDevice OVRMixedRealityCaptureConfiguration.capturingCameraDevice
	{
		get
		{
			return this.capturingCameraDevice;
		}
		set
		{
			this.capturingCameraDevice = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.flipCameraFrameHorizontally
	{
		get
		{
			return this.flipCameraFrameHorizontally;
		}
		set
		{
			this.flipCameraFrameHorizontally = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.flipCameraFrameVertically
	{
		get
		{
			return this.flipCameraFrameVertically;
		}
		set
		{
			this.flipCameraFrameVertically = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.handPoseStateLatency
	{
		get
		{
			return this.handPoseStateLatency;
		}
		set
		{
			this.handPoseStateLatency = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.sandwichCompositionRenderLatency
	{
		get
		{
			return this.sandwichCompositionRenderLatency;
		}
		set
		{
			this.sandwichCompositionRenderLatency = value;
		}
	}

	int OVRMixedRealityCaptureConfiguration.sandwichCompositionBufferedFrames
	{
		get
		{
			return this.sandwichCompositionBufferedFrames;
		}
		set
		{
			this.sandwichCompositionBufferedFrames = value;
		}
	}

	Color OVRMixedRealityCaptureConfiguration.chromaKeyColor
	{
		get
		{
			return this.chromaKeyColor;
		}
		set
		{
			this.chromaKeyColor = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.chromaKeySimilarity
	{
		get
		{
			return this.chromaKeySimilarity;
		}
		set
		{
			this.chromaKeySimilarity = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.chromaKeySmoothRange
	{
		get
		{
			return this.chromaKeySmoothRange;
		}
		set
		{
			this.chromaKeySmoothRange = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.chromaKeySpillRange
	{
		get
		{
			return this.chromaKeySpillRange;
		}
		set
		{
			this.chromaKeySpillRange = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.useDynamicLighting
	{
		get
		{
			return this.useDynamicLighting;
		}
		set
		{
			this.useDynamicLighting = value;
		}
	}

	[Obsolete("Deprecated", false)]
	OVRManager.DepthQuality OVRMixedRealityCaptureConfiguration.depthQuality
	{
		get
		{
			return this.depthQuality;
		}
		set
		{
			this.depthQuality = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.dynamicLightingSmoothFactor
	{
		get
		{
			return this.dynamicLightingSmoothFactor;
		}
		set
		{
			this.dynamicLightingSmoothFactor = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.dynamicLightingDepthVariationClampingValue
	{
		get
		{
			return this.dynamicLightingDepthVariationClampingValue;
		}
		set
		{
			this.dynamicLightingDepthVariationClampingValue = value;
		}
	}

	[Obsolete("Deprecated", false)]
	OVRManager.VirtualGreenScreenType OVRMixedRealityCaptureConfiguration.virtualGreenScreenType
	{
		get
		{
			return this.virtualGreenScreenType;
		}
		set
		{
			this.virtualGreenScreenType = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.virtualGreenScreenTopY
	{
		get
		{
			return this.virtualGreenScreenTopY;
		}
		set
		{
			this.virtualGreenScreenTopY = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.virtualGreenScreenBottomY
	{
		get
		{
			return this.virtualGreenScreenBottomY;
		}
		set
		{
			this.virtualGreenScreenBottomY = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.virtualGreenScreenApplyDepthCulling
	{
		get
		{
			return this.virtualGreenScreenApplyDepthCulling;
		}
		set
		{
			this.virtualGreenScreenApplyDepthCulling = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.virtualGreenScreenDepthTolerance
	{
		get
		{
			return this.virtualGreenScreenDepthTolerance;
		}
		set
		{
			this.virtualGreenScreenDepthTolerance = value;
		}
	}

	OVRManager.MrcActivationMode OVRMixedRealityCaptureConfiguration.mrcActivationMode
	{
		get
		{
			return this.mrcActivationMode;
		}
		set
		{
			this.mrcActivationMode = value;
		}
	}

	OVRManager.InstantiateMrcCameraDelegate OVRMixedRealityCaptureConfiguration.instantiateMixedRealityCameraGameObject
	{
		get
		{
			return this.instantiateMixedRealityCameraGameObject;
		}
		set
		{
			this.instantiateMixedRealityCameraGameObject = value;
		}
	}

	public bool isBoundaryVisibilitySuppressed { get; private set; }

	public OVRManager.XrApi xrApi
	{
		get
		{
			return (OVRManager.XrApi)OVRPlugin.nativeXrApi;
		}
	}

	public ulong xrInstance
	{
		get
		{
			return OVRPlugin.GetNativeOpenXRInstance();
		}
	}

	public ulong xrSession
	{
		get
		{
			return OVRPlugin.GetNativeOpenXRSession();
		}
	}

	public int vsyncCount
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 1;
			}
			return OVRPlugin.vsyncCount;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.vsyncCount = value;
		}
	}

	[Obsolete("Deprecated. Please use SystemInfo.batteryLevel", false)]
	public static float batteryLevel
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 1f;
			}
			return OVRPlugin.batteryLevel;
		}
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static float batteryTemperature
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 0f;
			}
			return OVRPlugin.batteryTemperature;
		}
	}

	[Obsolete("Deprecated. Please use SystemInfo.batteryStatus", false)]
	public static int batteryStatus
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return -1;
			}
			return (int)OVRPlugin.batteryStatus;
		}
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static float volumeLevel
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 0f;
			}
			return OVRPlugin.systemVolume;
		}
	}

	public static OVRManager.ProcessorPerformanceLevel suggestedCpuPerfLevel
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return OVRManager.ProcessorPerformanceLevel.PowerSavings;
			}
			return (OVRManager.ProcessorPerformanceLevel)OVRPlugin.suggestedCpuPerfLevel;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.suggestedCpuPerfLevel = (OVRPlugin.ProcessorPerformanceLevel)value;
		}
	}

	public static OVRManager.ProcessorPerformanceLevel suggestedGpuPerfLevel
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return OVRManager.ProcessorPerformanceLevel.PowerSavings;
			}
			return (OVRManager.ProcessorPerformanceLevel)OVRPlugin.suggestedGpuPerfLevel;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.suggestedGpuPerfLevel = (OVRPlugin.ProcessorPerformanceLevel)value;
		}
	}

	[Obsolete("Deprecated. Please use suggestedCpuPerfLevel", false)]
	public static int cpuLevel
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 2;
			}
			return OVRPlugin.cpuLevel;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.cpuLevel = value;
		}
	}

	[Obsolete("Deprecated. Please use suggestedGpuPerfLevel", false)]
	public static int gpuLevel
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 2;
			}
			return OVRPlugin.gpuLevel;
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				return;
			}
			OVRPlugin.gpuLevel = value;
		}
	}

	public static bool isPowerSavingActive
	{
		get
		{
			return OVRManager.isHmdPresent && OVRPlugin.powerSaving;
		}
	}

	public static OVRManager.EyeTextureFormat eyeTextureFormat
	{
		get
		{
			return (OVRManager.EyeTextureFormat)OVRPlugin.GetDesiredEyeTextureFormat();
		}
		set
		{
			OVRPlugin.SetDesiredEyeTextureFormat((OVRPlugin.EyeTextureFormat)value);
		}
	}

	public static bool eyeTrackedFoveatedRenderingSupported
	{
		get
		{
			return OVRManager.GetEyeTrackedFoveatedRenderingSupported();
		}
	}

	public static bool eyeTrackedFoveatedRenderingEnabled
	{
		get
		{
			return OVRManager.GetEyeTrackedFoveatedRenderingEnabled();
		}
		set
		{
			if (OVRManager.eyeTrackedFoveatedRenderingSupported)
			{
				if (value)
				{
					if (OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.EyeTracking))
					{
						OVRManager.SetEyeTrackedFoveatedRenderingEnabled(value);
						return;
					}
				}
				else
				{
					OVRManager.SetEyeTrackedFoveatedRenderingEnabled(value);
				}
			}
		}
	}

	protected static void OnPermissionGranted(string permissionId)
	{
		if (permissionId == OVRPermissionsRequester.GetPermissionId(OVRPermissionsRequester.Permission.EyeTracking))
		{
			OVRPermissionsRequester.PermissionGranted -= OVRManager.OnPermissionGranted;
			OVRManager.SetEyeTrackedFoveatedRenderingEnabled(true);
		}
	}

	public static OVRManager.FoveatedRenderingLevel foveatedRenderingLevel
	{
		get
		{
			return OVRManager.GetFoveatedRenderingLevel();
		}
		set
		{
			OVRManager.SetFoveatedRenderingLevel(value);
		}
	}

	public static bool fixedFoveatedRenderingSupported
	{
		get
		{
			return OVRManager.GetFixedFoveatedRenderingSupported();
		}
	}

	[Obsolete("Please use foveatedRenderingLevel instead", false)]
	public static OVRManager.FixedFoveatedRenderingLevel fixedFoveatedRenderingLevel
	{
		get
		{
			return (OVRManager.FixedFoveatedRenderingLevel)OVRPlugin.fixedFoveatedRenderingLevel;
		}
		set
		{
			OVRPlugin.fixedFoveatedRenderingLevel = (OVRPlugin.FixedFoveatedRenderingLevel)value;
		}
	}

	public static bool useDynamicFoveatedRendering
	{
		get
		{
			return OVRManager.GetDynamicFoveatedRenderingEnabled();
		}
		set
		{
			OVRManager.SetDynamicFoveatedRenderingEnabled(value);
		}
	}

	[Obsolete("Please use useDynamicFoveatedRendering instead", false)]
	public static bool useDynamicFixedFoveatedRendering
	{
		get
		{
			return OVRPlugin.useDynamicFixedFoveatedRendering;
		}
		set
		{
			OVRPlugin.useDynamicFixedFoveatedRendering = value;
		}
	}

	[Obsolete("Please use fixedFoveatedRenderingSupported instead", false)]
	public static bool tiledMultiResSupported
	{
		get
		{
			return OVRPlugin.tiledMultiResSupported;
		}
	}

	[Obsolete("Please use foveatedRenderingLevel instead", false)]
	public static OVRManager.TiledMultiResLevel tiledMultiResLevel
	{
		get
		{
			return (OVRManager.TiledMultiResLevel)OVRPlugin.tiledMultiResLevel;
		}
		set
		{
			OVRPlugin.tiledMultiResLevel = (OVRPlugin.TiledMultiResLevel)value;
		}
	}

	public static bool gpuUtilSupported
	{
		get
		{
			return OVRPlugin.gpuUtilSupported;
		}
	}

	public static float gpuUtilLevel
	{
		get
		{
			if (!OVRPlugin.gpuUtilSupported)
			{
				Debug.LogWarning("GPU Util is not supported");
			}
			return OVRPlugin.gpuUtilLevel;
		}
	}

	public static OVRManager.SystemHeadsetType systemHeadsetType
	{
		get
		{
			return (OVRManager.SystemHeadsetType)OVRPlugin.GetSystemHeadsetType();
		}
	}

	public static OVRManager.SystemHeadsetTheme systemHeadsetTheme
	{
		get
		{
			return OVRManager.GetSystemHeadsetTheme();
		}
	}

	private static OVRManager.SystemHeadsetTheme GetSystemHeadsetTheme()
	{
		if (!OVRManager._isSystemHeadsetThemeCached)
		{
			OVRManager._isSystemHeadsetThemeCached = true;
		}
		return OVRManager._cachedSystemHeadsetTheme;
	}

	public static void SetColorScaleAndOffset(Vector4 colorScale, Vector4 colorOffset, bool applyToAllLayers)
	{
		OVRManager.SetColorScaleAndOffset_Internal(colorScale, colorOffset, applyToAllLayers);
	}

	public static void SetOpenVRLocalPose(Vector3 leftPos, Vector3 rightPos, Quaternion leftRot, Quaternion rightRot)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			OVRInput.SetOpenVRLocalPose(leftPos, rightPos, leftRot, rightRot);
		}
	}

	public static OVRPose GetOpenVRControllerOffset(XRNode hand)
	{
		OVRPose identity = OVRPose.identity;
		if ((hand == XRNode.LeftHand || hand == XRNode.RightHand) && OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			int num = (hand == XRNode.LeftHand) ? 0 : 1;
			if (OVRInput.openVRControllerDetails[num].controllerType == OVRInput.OpenVRController.OculusTouch)
			{
				Vector3 vector = (hand == XRNode.LeftHand) ? OVRManager.OpenVRTouchRotationOffsetEulerLeft : OVRManager.OpenVRTouchRotationOffsetEulerRight;
				identity.orientation = Quaternion.Euler(vector.x, vector.y, vector.z);
				identity.position = ((hand == XRNode.LeftHand) ? OVRManager.OpenVRTouchPositionOffsetLeft : OVRManager.OpenVRTouchPositionOffsetRight);
			}
		}
		return identity;
	}

	public static void SetSpaceWarp(bool enabled)
	{
		Camera camera = OVRManager.FindMainCamera();
		if (enabled)
		{
			if (camera != null)
			{
				OVRManager.PrepareCameraForSpaceWarp(camera);
				OVRManager.m_lastSpaceWarpCamera = new WeakReference<Camera>(camera);
			}
		}
		else
		{
			Camera x;
			if (camera != null && OVRManager.m_lastSpaceWarpCamera != null && OVRManager.m_lastSpaceWarpCamera.TryGetTarget(out x) && x == camera)
			{
				camera.depthTextureMode = OVRManager.m_CachedDepthTextureMode;
			}
			OVRManager.m_AppSpaceTransform = null;
			OVRManager.m_lastSpaceWarpCamera = null;
		}
		OVRManager.SetSpaceWarp_Internal(enabled);
		OVRManager.m_SpaceWarpEnabled = enabled;
	}

	private static void PrepareCameraForSpaceWarp(Camera camera)
	{
		OVRManager.m_CachedDepthTextureMode = camera.depthTextureMode;
		camera.depthTextureMode |= (DepthTextureMode.Depth | DepthTextureMode.MotionVectors);
		OVRManager.m_AppSpaceTransform = camera.transform.parent;
	}

	public static bool GetSpaceWarp()
	{
		return OVRManager.m_SpaceWarpEnabled;
	}

	public OVRManager.TrackingOrigin trackingOriginType
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return this._trackingOriginType;
			}
			return (OVRManager.TrackingOrigin)OVRPlugin.GetTrackingOriginType();
		}
		set
		{
			if (!OVRManager.isHmdPresent)
			{
				this._trackingOriginType = value;
				return;
			}
			if (OVRPlugin.UnityOpenXR.Enabled)
			{
				if (OVRManager.GetCurrentInputSubsystem() == null)
				{
					Debug.LogError("InputSubsystem not found");
					return;
				}
				TrackingOriginModeFlags trackingOriginModeFlags = TrackingOriginModeFlags.Unknown;
				if (value == OVRManager.TrackingOrigin.EyeLevel)
				{
					trackingOriginModeFlags = TrackingOriginModeFlags.Device;
				}
				else if (value == OVRManager.TrackingOrigin.FloorLevel)
				{
					trackingOriginModeFlags = TrackingOriginModeFlags.Floor;
					OpenXRSettings.SetAllowRecentering(true, 1.5f);
				}
				else if (value == OVRManager.TrackingOrigin.Stage)
				{
					trackingOriginModeFlags = TrackingOriginModeFlags.Floor;
					OpenXRSettings.SetAllowRecentering(false, 1.5f);
				}
				if (trackingOriginModeFlags != TrackingOriginModeFlags.Unknown)
				{
					if (!OVRManager.GetCurrentInputSubsystem().TrySetTrackingOriginMode(trackingOriginModeFlags))
					{
						Debug.LogError(string.Format("Unable to set TrackingOrigin {0} to Unity Input Subsystem", trackingOriginModeFlags));
						return;
					}
					this._trackingOriginType = value;
					OpenXRSettings.RefreshRecenterSpace();
					return;
				}
			}
			if (OVRPlugin.SetTrackingOriginType((OVRPlugin.TrackingOrigin)value))
			{
				this._trackingOriginType = value;
			}
		}
	}

	public bool IsSimultaneousHandsAndControllersSupported
	{
		get
		{
			return OVRManager._readOnlyControllerDrivenHandPosesType != OVRManager.ControllerDrivenHandPosesType.None || this.launchSimultaneousHandsControllersOnStartup;
		}
	}

	public bool isSupportedPlatform { get; private set; }

	public bool isUserPresent
	{
		get
		{
			if (!OVRManager._isUserPresentCached)
			{
				OVRManager._isUserPresentCached = true;
				OVRManager._isUserPresent = OVRPlugin.userPresent;
			}
			return OVRManager._isUserPresent;
		}
		private set
		{
			OVRManager._isUserPresentCached = true;
			OVRManager._isUserPresent = value;
		}
	}

	public void RegisterEventListener(OVRManager.EventListener listener)
	{
		this.eventListeners.Add(listener);
	}

	public void DeregisterEventListener(OVRManager.EventListener listener)
	{
		this.eventListeners.Remove(listener);
	}

	public static Version utilitiesVersion
	{
		get
		{
			return OVRPlugin.wrapperVersion;
		}
	}

	public static Version pluginVersion
	{
		get
		{
			return OVRPlugin.version;
		}
	}

	public static Version sdkVersion
	{
		get
		{
			return OVRPlugin.nativeSDKVersion;
		}
	}

	private static bool MixedRealityEnabledFromCmd()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].ToLower() == "-mixedreality")
			{
				return true;
			}
		}
		return false;
	}

	private static bool UseDirectCompositionFromCmd()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].ToLower() == "-directcomposition")
			{
				return true;
			}
		}
		return false;
	}

	private static bool UseExternalCompositionFromCmd()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].ToLower() == "-externalcomposition")
			{
				return true;
			}
		}
		return false;
	}

	private static bool CreateMixedRealityCaptureConfigurationFileFromCmd()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].ToLower() == "-create_mrc_config")
			{
				return true;
			}
		}
		return false;
	}

	private static bool LoadMixedRealityCaptureConfigurationFileFromCmd()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].ToLower() == "-load_mrc_config")
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsUnityAlphaOrBetaVersion()
	{
		string unityVersion = Application.unityVersion;
		int num = unityVersion.Length - 1;
		while (num >= 0 && unityVersion[num] >= '0' && unityVersion[num] <= '9')
		{
			num--;
		}
		return num >= 0 && (unityVersion[num] == 'a' || unityVersion[num] == 'b');
	}

	private void Reset()
	{
		this.dynamicResolutionVersion = OVRManager.MaxDynamicResolutionVersion;
	}

	private void InitOVRManager()
	{
		using (OVRTelemetryMarker marker = new OVRTelemetryMarker(163069401, 0, -1L, null))
		{
			marker.AddSDKVersionAnnotation();
			if (OVRManager.instance != null)
			{
				base.enabled = false;
				Object.DestroyImmediate(this);
				marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
			}
			else
			{
				OVRManager.instance = this;
				OVRManager.runtimeSettings = OVRRuntimeSettings.GetRuntimeSettings();
				string[] array = new string[9];
				array[0] = "Unity v";
				array[1] = Application.unityVersion;
				array[2] = ", Oculus Utilities v";
				int num = 3;
				Version wrapperVersion = OVRPlugin.wrapperVersion;
				array[num] = ((wrapperVersion != null) ? wrapperVersion.ToString() : null);
				array[4] = ", OVRPlugin v";
				int num2 = 5;
				Version version = OVRPlugin.version;
				array[num2] = ((version != null) ? version.ToString() : null);
				array[6] = ", SDK v";
				int num3 = 7;
				Version nativeSDKVersion = OVRPlugin.nativeSDKVersion;
				array[num3] = ((nativeSDKVersion != null) ? nativeSDKVersion.ToString() : null);
				array[8] = ".";
				string message = string.Concat(array);
				if (OVRPlugin.version < OVRPlugin.wrapperVersion)
				{
					Debug.LogWarning(message);
					Debug.LogWarning("You are using an old version of OVRPlugin. Some features may not work correctly. You will be prompted to restart the Editor for any OVRPlugin changes.");
				}
				else
				{
					Debug.Log(message);
				}
				Debug.LogFormat("SystemHeadset {0}, API {1}", new object[]
				{
					OVRManager.systemHeadsetType.ToString(),
					this.xrApi.ToString()
				});
				if (this.xrApi == OVRManager.XrApi.OpenXR)
				{
					Debug.LogFormat("OpenXR instance 0x{0:X} session 0x{1:X}", new object[]
					{
						this.xrInstance,
						this.xrSession
					});
				}
				if (OVRManager.IsUnityAlphaOrBetaVersion())
				{
					Debug.LogWarning(OVRManager.UnityAlphaOrBetaVersionWarningMessage);
				}
				string text = GraphicsDeviceType.Direct3D11.ToString() + ", " + GraphicsDeviceType.Direct3D12.ToString();
				if (!text.Contains(SystemInfo.graphicsDeviceType.ToString()))
				{
					Debug.LogWarning("VR rendering requires one of the following device types: (" + text + "). Your graphics device: " + SystemInfo.graphicsDeviceType.ToString());
				}
				RuntimePlatform platform = Application.platform;
				if (platform == RuntimePlatform.Android || platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
				{
					this.isSupportedPlatform = true;
				}
				else
				{
					this.isSupportedPlatform = false;
				}
				if (!this.isSupportedPlatform)
				{
					Debug.LogWarning("This platform is unsupported");
					marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
				}
				else
				{
					this.enableMixedReality = false;
					if (!OVRManager.staticMixedRealityCaptureInitialized)
					{
						bool flag = OVRManager.LoadMixedRealityCaptureConfigurationFileFromCmd();
						bool flag2 = OVRManager.CreateMixedRealityCaptureConfigurationFileFromCmd();
						if (flag || flag2)
						{
							OVRMixedRealityCaptureSettings ovrmixedRealityCaptureSettings = ScriptableObject.CreateInstance<OVRMixedRealityCaptureSettings>();
							ovrmixedRealityCaptureSettings.ReadFrom(this);
							if (flag)
							{
								ovrmixedRealityCaptureSettings.CombineWithConfigurationFile();
								ovrmixedRealityCaptureSettings.ApplyTo(this);
							}
							if (flag2)
							{
								ovrmixedRealityCaptureSettings.WriteToConfigurationFile();
							}
							Object.Destroy(ovrmixedRealityCaptureSettings);
						}
						if (OVRManager.MixedRealityEnabledFromCmd())
						{
							this.enableMixedReality = true;
						}
						if (this.enableMixedReality)
						{
							Debug.Log("OVR: Mixed Reality mode enabled");
							if (OVRManager.UseDirectCompositionFromCmd())
							{
								Debug.Log("DirectionComposition deprecated. Fallback to ExternalComposition");
								this.compositionMethod = OVRManager.CompositionMethod.External;
							}
							if (OVRManager.UseExternalCompositionFromCmd())
							{
								this.compositionMethod = OVRManager.CompositionMethod.External;
							}
							Debug.Log("OVR: CompositionMethod : " + this.compositionMethod.ToString());
						}
					}
					OVRManager.StaticInitializeMixedRealityCapture(this);
					this.Initialize();
					this.InitPermissionRequest();
					marker.AddPoint(OVRTelemetryConstants.OVRManager.InitPermissionRequest);
					string format = "Current display frequency {0}, available frequencies [{1}]";
					object[] array2 = new object[2];
					array2[0] = OVRManager.display.displayFrequency;
					array2[1] = string.Join(", ", (from f in OVRManager.display.displayFrequenciesAvailable
					select f.ToString()).ToArray<string>());
					Debug.LogFormat(format, array2);
					if (this.resetTrackerOnLoad)
					{
						OVRManager.display.RecenterPose();
					}
					if (Debug.isDebugBuild)
					{
						if (base.GetComponent<OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer>() == null)
						{
							base.gameObject.AddComponent<OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer>();
						}
						OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer component = base.GetComponent<OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer>();
						component.listeningPort = this.profilerTcpPort;
						if (!component.enabled)
						{
							component.enabled = true;
						}
						OVRPlugin.SetDeveloperMode(OVRPlugin.Bool.True);
					}
					OVRManager.ColorSpace colorSpace = OVRManager.runtimeSettings.colorSpace;
					this.colorGamut = colorSpace;
					OVRPlugin.SetEyeBufferSharpenType(this._sharpenType);
					OVRPlugin.occlusionMesh = true;
					if (!OVRPlugin.SetSimultaneousHandsAndControllersEnabled(this.launchSimultaneousHandsControllersOnStartup))
					{
						Debug.Log("Failed to set multimodal hands and controllers mode!");
					}
					if (this.isInsightPassthroughEnabled)
					{
						OVRManager.InitializeInsightPassthrough();
						marker.AddPoint(OVRTelemetryConstants.OVRManager.InitializeInsightPassthrough);
					}
					if (!OVRPlugin.localDimmingSupported)
					{
						Debug.LogWarning("Local Dimming feature is not supported");
						this._localDimming = false;
					}
					else
					{
						OVRPlugin.localDimming = this._localDimming;
					}
					this.UpdateDynamicResolutionVersion();
					OVRManager.SystemHeadsetType systemHeadsetType = OVRManager.systemHeadsetType;
					if (systemHeadsetType - OVRManager.SystemHeadsetType.Oculus_Quest_2 <= 1)
					{
						this.minDynamicResolutionScale = this.quest2MinDynamicResolutionScale;
						this.maxDynamicResolutionScale = this.quest2MaxDynamicResolutionScale;
					}
					else
					{
						this.minDynamicResolutionScale = this.quest3MinDynamicResolutionScale;
						this.maxDynamicResolutionScale = this.quest3MaxDynamicResolutionScale;
					}
					this.InitializeBoundary();
					if (OVRPlugin.HandSkeletonVersion != OVRManager.runtimeSettings.HandSkeletonVersion)
					{
						OVRPlugin.SetHandSkeletonVersion(OVRManager.runtimeSettings.HandSkeletonVersion);
					}
					Debug.Log(string.Format("[OVRManager] Current hand skeleton version is {0}", OVRPlugin.HandSkeletonVersion));
					OpenXRSettings instance = OpenXRSettings.Instance;
					if (instance != null)
					{
						MetaXRSubsampledLayout feature = instance.GetFeature<MetaXRSubsampledLayout>();
						MetaXRSpaceWarp feature2 = instance.GetFeature<MetaXRSpaceWarp>();
						bool flag3 = false;
						if (feature != null)
						{
							flag3 = feature.enabled;
						}
						bool flag4 = false;
						if (feature2 != null)
						{
							flag4 = feature2.enabled;
						}
						Debug.Log(string.Format("OpenXR Meta Quest Runtime Settings:\nDepth Submission Mode - {0}\nRendering Mode - {1}\nOptimize Buffer Discards - {2}\nSymmetric Projection - {3}\nSubsampled Layout - {4}\nSpace Warp - {5}", new object[]
						{
							instance.depthSubmissionMode,
							instance.renderMode,
							instance.optimizeBufferDiscards,
							instance.symmetricProjection,
							flag3,
							flag4
						}));
					}
					OVRManager.OVRManagerinitialized = true;
				}
			}
		}
	}

	private void InitPermissionRequest()
	{
		HashSet<OVRPermissionsRequester.Permission> hashSet = new HashSet<OVRPermissionsRequester.Permission>();
		if (this.requestBodyTrackingPermissionOnStartup)
		{
			hashSet.Add(OVRPermissionsRequester.Permission.BodyTracking);
		}
		if (this.requestFaceTrackingPermissionOnStartup)
		{
			hashSet.Add(OVRPermissionsRequester.Permission.FaceTracking);
		}
		if (this.requestEyeTrackingPermissionOnStartup)
		{
			hashSet.Add(OVRPermissionsRequester.Permission.EyeTracking);
		}
		if (this.requestScenePermissionOnStartup)
		{
			hashSet.Add(OVRPermissionsRequester.Permission.Scene);
		}
		if (this.requestRecordAudioPermissionOnStartup)
		{
			hashSet.Add(OVRPermissionsRequester.Permission.RecordAudio);
		}
		OVRPermissionsRequester.Request(hashSet);
	}

	private void Awake()
	{
		if (OVRPlugin.initialized)
		{
			this.InitOVRManager();
		}
	}

	private void SetCurrentXRDevice()
	{
		XRDisplaySubsystem currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
		XRDisplaySubsystemDescriptor currentDisplaySubsystemDescriptor = OVRManager.GetCurrentDisplaySubsystemDescriptor();
		if (OVRPlugin.initialized)
		{
			OVRManager.loadedXRDevice = OVRManager.XRDevice.Oculus;
			return;
		}
		if (currentDisplaySubsystem == null || currentDisplaySubsystemDescriptor == null || !currentDisplaySubsystem.running)
		{
			OVRManager.loadedXRDevice = OVRManager.XRDevice.Unknown;
			return;
		}
		if (currentDisplaySubsystemDescriptor.id == OVRManager.OPENVR_UNITY_NAME_STR)
		{
			OVRManager.loadedXRDevice = OVRManager.XRDevice.OpenVR;
			return;
		}
		OVRManager.loadedXRDevice = OVRManager.XRDevice.Unknown;
	}

	public static XRDisplaySubsystem GetCurrentDisplaySubsystem()
	{
		if (OVRManager.s_displaySubsystems == null)
		{
			OVRManager.s_displaySubsystems = new List<XRDisplaySubsystem>();
		}
		SubsystemManager.GetSubsystems<XRDisplaySubsystem>(OVRManager.s_displaySubsystems);
		if (OVRManager.s_displaySubsystems.Count > 0)
		{
			return OVRManager.s_displaySubsystems[0];
		}
		return null;
	}

	public static XRDisplaySubsystemDescriptor GetCurrentDisplaySubsystemDescriptor()
	{
		if (OVRManager.s_displaySubsystemDescriptors == null)
		{
			OVRManager.s_displaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
		}
		SubsystemManager.GetSubsystemDescriptors<XRDisplaySubsystemDescriptor>(OVRManager.s_displaySubsystemDescriptors);
		if (OVRManager.s_displaySubsystemDescriptors.Count > 0)
		{
			return OVRManager.s_displaySubsystemDescriptors[0];
		}
		return null;
	}

	public static XRInputSubsystem GetCurrentInputSubsystem()
	{
		if (OVRManager.s_inputSubsystems == null)
		{
			OVRManager.s_inputSubsystems = new List<XRInputSubsystem>();
		}
		SubsystemManager.GetSubsystems<XRInputSubsystem>(OVRManager.s_inputSubsystems);
		if (OVRManager.s_inputSubsystems.Count > 0)
		{
			return OVRManager.s_inputSubsystems[0];
		}
		return null;
	}

	private void Initialize()
	{
		if (OVRManager.display == null)
		{
			OVRManager.display = new OVRDisplay();
		}
		if (OVRManager.tracker == null)
		{
			OVRManager.tracker = new OVRTracker();
		}
		if (OVRManager.boundary == null)
		{
			OVRManager.boundary = new OVRBoundary();
		}
		this.SetCurrentXRDevice();
	}

	private void Update()
	{
		if (!OVRManager.OVRManagerinitialized)
		{
			bool currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem() != null;
			XRDisplaySubsystemDescriptor currentDisplaySubsystemDescriptor = OVRManager.GetCurrentDisplaySubsystemDescriptor();
			if (!currentDisplaySubsystem || currentDisplaySubsystemDescriptor == null || !OVRPlugin.initialized)
			{
				return;
			}
			this.InitOVRManager();
		}
		this.SetCurrentXRDevice();
		if (OVRPlugin.shouldQuit)
		{
			Debug.Log("[OVRManager] OVRPlugin.shouldQuit detected");
			OVRManager.StaticShutdownMixedRealityCapture(OVRManager.instance);
			OVRManager.ShutdownInsightPassthrough();
			Application.Quit();
		}
		if (this.AllowRecenter && OVRPlugin.shouldRecenter)
		{
			OVRManager.display.RecenterPose();
		}
		if (this.trackingOriginType != this._trackingOriginType)
		{
			this.trackingOriginType = this._trackingOriginType;
		}
		OVRManager.tracker.isEnabled = this.usePositionTracking;
		OVRPlugin.rotation = this.useRotationTracking;
		OVRPlugin.useIPDInPositionTracking = this.useIPDInPositionTracking;
		if (this.monoscopic != this._monoscopic)
		{
			this.monoscopic = this._monoscopic;
		}
		if (this.headPoseRelativeOffsetRotation != this._headPoseRelativeOffsetRotation)
		{
			this.headPoseRelativeOffsetRotation = this._headPoseRelativeOffsetRotation;
		}
		if (this.headPoseRelativeOffsetTranslation != this._headPoseRelativeOffsetTranslation)
		{
			this.headPoseRelativeOffsetTranslation = this._headPoseRelativeOffsetTranslation;
		}
		if (OVRManager._wasHmdPresent && !OVRManager.isHmdPresent)
		{
			try
			{
				Debug.Log("[OVRManager] HMDLost event");
				if (OVRManager.HMDLost != null)
				{
					OVRManager.HMDLost();
				}
			}
			catch (Exception ex)
			{
				string str = "Caught Exception: ";
				Exception ex2 = ex;
				Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
			}
		}
		if (!OVRManager._wasHmdPresent && OVRManager.isHmdPresent)
		{
			try
			{
				Debug.Log("[OVRManager] HMDAcquired event");
				if (OVRManager.HMDAcquired != null)
				{
					OVRManager.HMDAcquired();
				}
			}
			catch (Exception ex3)
			{
				string str2 = "Caught Exception: ";
				Exception ex4 = ex3;
				Debug.LogError(str2 + ((ex4 != null) ? ex4.ToString() : null));
			}
		}
		OVRManager._wasHmdPresent = OVRManager.isHmdPresent;
		this.isUserPresent = OVRPlugin.userPresent;
		if (OVRManager._wasUserPresent && !this.isUserPresent)
		{
			try
			{
				Debug.Log("[OVRManager] HMDUnmounted event");
				if (OVRManager.HMDUnmounted != null)
				{
					OVRManager.HMDUnmounted();
				}
			}
			catch (Exception ex5)
			{
				string str3 = "Caught Exception: ";
				Exception ex6 = ex5;
				Debug.LogError(str3 + ((ex6 != null) ? ex6.ToString() : null));
			}
		}
		if (!OVRManager._wasUserPresent && this.isUserPresent)
		{
			try
			{
				Debug.Log("[OVRManager] HMDMounted event");
				if (OVRManager.HMDMounted != null)
				{
					OVRManager.HMDMounted();
				}
			}
			catch (Exception ex7)
			{
				string str4 = "Caught Exception: ";
				Exception ex8 = ex7;
				Debug.LogError(str4 + ((ex8 != null) ? ex8.ToString() : null));
			}
		}
		OVRManager._wasUserPresent = this.isUserPresent;
		OVRManager.hasVrFocus = OVRPlugin.hasVrFocus;
		if (OVRManager._hadVrFocus && !OVRManager.hasVrFocus)
		{
			try
			{
				Debug.Log("[OVRManager] VrFocusLost event");
				if (OVRManager.VrFocusLost != null)
				{
					OVRManager.VrFocusLost();
				}
			}
			catch (Exception ex9)
			{
				string str5 = "Caught Exception: ";
				Exception ex10 = ex9;
				Debug.LogError(str5 + ((ex10 != null) ? ex10.ToString() : null));
			}
		}
		if (!OVRManager._hadVrFocus && OVRManager.hasVrFocus)
		{
			try
			{
				Debug.Log("[OVRManager] VrFocusAcquired event");
				if (OVRManager.VrFocusAcquired != null)
				{
					OVRManager.VrFocusAcquired();
				}
			}
			catch (Exception ex11)
			{
				string str6 = "Caught Exception: ";
				Exception ex12 = ex11;
				Debug.LogError(str6 + ((ex12 != null) ? ex12.ToString() : null));
			}
		}
		OVRManager._hadVrFocus = OVRManager.hasVrFocus;
		bool hasInputFocus = OVRPlugin.hasInputFocus;
		if (OVRManager._hadInputFocus && !hasInputFocus)
		{
			try
			{
				Debug.Log("[OVRManager] InputFocusLost event");
				if (OVRManager.InputFocusLost != null)
				{
					OVRManager.InputFocusLost();
				}
			}
			catch (Exception ex13)
			{
				string str7 = "Caught Exception: ";
				Exception ex14 = ex13;
				Debug.LogError(str7 + ((ex14 != null) ? ex14.ToString() : null));
			}
		}
		if (!OVRManager._hadInputFocus && hasInputFocus)
		{
			try
			{
				Debug.Log("[OVRManager] InputFocusAcquired event");
				if (OVRManager.InputFocusAcquired != null)
				{
					OVRManager.InputFocusAcquired();
				}
			}
			catch (Exception ex15)
			{
				string str8 = "Caught Exception: ";
				Exception ex16 = ex15;
				Debug.LogError(str8 + ((ex16 != null) ? ex16.ToString() : null));
			}
		}
		OVRManager._hadInputFocus = hasInputFocus;
		string audioOutId = OVRPlugin.audioOutId;
		if (!OVRManager.prevAudioOutIdIsCached)
		{
			OVRManager.prevAudioOutId = audioOutId;
			OVRManager.prevAudioOutIdIsCached = true;
		}
		else if (audioOutId != OVRManager.prevAudioOutId)
		{
			try
			{
				Debug.Log("[OVRManager] AudioOutChanged event");
				if (OVRManager.AudioOutChanged != null)
				{
					OVRManager.AudioOutChanged();
				}
			}
			catch (Exception ex17)
			{
				string str9 = "Caught Exception: ";
				Exception ex18 = ex17;
				Debug.LogError(str9 + ((ex18 != null) ? ex18.ToString() : null));
			}
			OVRManager.prevAudioOutId = audioOutId;
		}
		string audioInId = OVRPlugin.audioInId;
		if (!OVRManager.prevAudioInIdIsCached)
		{
			OVRManager.prevAudioInId = audioInId;
			OVRManager.prevAudioInIdIsCached = true;
		}
		else if (audioInId != OVRManager.prevAudioInId)
		{
			try
			{
				Debug.Log("[OVRManager] AudioInChanged event");
				if (OVRManager.AudioInChanged != null)
				{
					OVRManager.AudioInChanged();
				}
			}
			catch (Exception ex19)
			{
				string str10 = "Caught Exception: ";
				Exception ex20 = ex19;
				Debug.LogError(str10 + ((ex20 != null) ? ex20.ToString() : null));
			}
			OVRManager.prevAudioInId = audioInId;
		}
		if (OVRManager.wasPositionTracked && !OVRManager.tracker.isPositionTracked)
		{
			try
			{
				Debug.Log("[OVRManager] TrackingLost event");
				if (OVRManager.TrackingLost != null)
				{
					OVRManager.TrackingLost();
				}
			}
			catch (Exception ex21)
			{
				string str11 = "Caught Exception: ";
				Exception ex22 = ex21;
				Debug.LogError(str11 + ((ex22 != null) ? ex22.ToString() : null));
			}
		}
		if (!OVRManager.wasPositionTracked && OVRManager.tracker.isPositionTracked)
		{
			try
			{
				Debug.Log("[OVRManager] TrackingAcquired event");
				if (OVRManager.TrackingAcquired != null)
				{
					OVRManager.TrackingAcquired();
				}
			}
			catch (Exception ex23)
			{
				string str12 = "Caught Exception: ";
				Exception ex24 = ex23;
				Debug.LogError(str12 + ((ex24 != null) ? ex24.ToString() : null));
			}
		}
		OVRManager.wasPositionTracked = OVRManager.tracker.isPositionTracked;
		OVRManager.display.Update();
		if (OVRManager._readOnlyControllerDrivenHandPosesType != this.controllerDrivenHandPosesType)
		{
			OVRManager._readOnlyControllerDrivenHandPosesType = this.controllerDrivenHandPosesType;
			switch (OVRManager._readOnlyControllerDrivenHandPosesType)
			{
			case OVRManager.ControllerDrivenHandPosesType.None:
				OVRPlugin.SetControllerDrivenHandPoses(false);
				OVRPlugin.SetControllerDrivenHandPosesAreNatural(false);
				break;
			case OVRManager.ControllerDrivenHandPosesType.ConformingToController:
				OVRPlugin.SetControllerDrivenHandPoses(true);
				OVRPlugin.SetControllerDrivenHandPosesAreNatural(false);
				break;
			case OVRManager.ControllerDrivenHandPosesType.Natural:
				OVRPlugin.SetControllerDrivenHandPoses(true);
				OVRPlugin.SetControllerDrivenHandPosesAreNatural(true);
				break;
			}
		}
		if (this._readOnlyWideMotionModeHandPosesEnabled != this.wideMotionModeHandPosesEnabled)
		{
			this._readOnlyWideMotionModeHandPosesEnabled = this.wideMotionModeHandPosesEnabled;
			OVRPlugin.SetWideMotionModeHandPoses(this._readOnlyWideMotionModeHandPosesEnabled);
		}
		OVRInput.Update();
		this.UpdateHMDEvents();
		OVRManager.StaticUpdateMixedRealityCapture(this, base.gameObject, this.trackingOriginType);
		OVRManager.UpdateInsightPassthrough(this.isInsightPassthroughEnabled);
		this.UpdateBoundary();
	}

	private unsafe void UpdateHMDEvents()
	{
		while (OVRPlugin.PollEvent(ref OVRManager.eventDataBuffer))
		{
			OVRPlugin.EventType eventType = OVRManager.eventDataBuffer.EventType;
			if (eventType <= OVRPlugin.EventType.ColocationSessionStopDiscoveryComplete)
			{
				if (eventType <= OVRPlugin.EventType.SpaceShareToGroupsComplete)
				{
					if (eventType != OVRPlugin.EventType.DisplayRefreshRateChanged)
					{
						switch (eventType)
						{
						case OVRPlugin.EventType.SpatialAnchorCreateComplete:
						{
							OVRDeserialize.SpatialAnchorCreateCompleteData spatialAnchorCreateCompleteData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpatialAnchorCreateCompleteData>(OVRManager.eventDataBuffer.EventData);
							OVRTask.SetResult<OVRAnchor>(spatialAnchorCreateCompleteData.RequestId, (spatialAnchorCreateCompleteData.Result >= 0) ? new OVRAnchor(spatialAnchorCreateCompleteData.Space, spatialAnchorCreateCompleteData.Uuid) : OVRAnchor.Null);
							Action<ulong, bool, OVRSpace, Guid> spatialAnchorCreateComplete = OVRManager.SpatialAnchorCreateComplete;
							if (spatialAnchorCreateComplete == null)
							{
								continue;
							}
							spatialAnchorCreateComplete(spatialAnchorCreateCompleteData.RequestId, spatialAnchorCreateCompleteData.Result >= 0, spatialAnchorCreateCompleteData.Space, spatialAnchorCreateCompleteData.Uuid);
							continue;
						}
						case OVRPlugin.EventType.SpaceSetComponentStatusComplete:
						{
							OVRDeserialize.SpaceSetComponentStatusCompleteData spaceSetComponentStatusCompleteData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceSetComponentStatusCompleteData>(OVRManager.eventDataBuffer.EventData);
							Action<ulong, bool, OVRSpace, Guid, OVRPlugin.SpaceComponentType, bool> spaceSetComponentStatusComplete = OVRManager.SpaceSetComponentStatusComplete;
							if (spaceSetComponentStatusComplete != null)
							{
								spaceSetComponentStatusComplete(spaceSetComponentStatusCompleteData.RequestId, spaceSetComponentStatusCompleteData.Result >= 0, spaceSetComponentStatusCompleteData.Space, spaceSetComponentStatusCompleteData.Uuid, spaceSetComponentStatusCompleteData.ComponentType, spaceSetComponentStatusCompleteData.Enabled != 0);
							}
							OVRTask.SetResult<bool>(spaceSetComponentStatusCompleteData.RequestId, spaceSetComponentStatusCompleteData.Result >= 0);
							OVRAnchor.OnSpaceSetComponentStatusComplete(spaceSetComponentStatusCompleteData);
							continue;
						}
						case OVRPlugin.EventType.SpaceQueryResults:
							if (OVRManager.SpaceQueryResults != null)
							{
								OVRDeserialize.SpaceQueryResultsData spaceQueryResultsData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceQueryResultsData>(OVRManager.eventDataBuffer.EventData);
								OVRManager.SpaceQueryResults(spaceQueryResultsData.RequestId);
								continue;
							}
							continue;
						case OVRPlugin.EventType.SpaceQueryComplete:
						{
							OVRDeserialize.SpaceQueryCompleteData spaceQueryCompleteData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceQueryCompleteData>(OVRManager.eventDataBuffer.EventData);
							Action<ulong, bool> spaceQueryComplete = OVRManager.SpaceQueryComplete;
							if (spaceQueryComplete != null)
							{
								spaceQueryComplete(spaceQueryCompleteData.RequestId, spaceQueryCompleteData.Result >= 0);
							}
							OVRAnchor.OnSpaceQueryComplete(spaceQueryCompleteData);
							continue;
						}
						case OVRPlugin.EventType.SpaceSaveComplete:
							if (OVRManager.SpaceSaveComplete != null)
							{
								OVRDeserialize.SpaceSaveCompleteData spaceSaveCompleteData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceSaveCompleteData>(OVRManager.eventDataBuffer.EventData);
								OVRManager.SpaceSaveComplete(spaceSaveCompleteData.RequestId, spaceSaveCompleteData.Space, spaceSaveCompleteData.Result >= 0, spaceSaveCompleteData.Uuid);
								continue;
							}
							continue;
						case OVRPlugin.EventType.SpaceEraseComplete:
						{
							OVRDeserialize.SpaceEraseCompleteData spaceEraseCompleteData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceEraseCompleteData>(OVRManager.eventDataBuffer.EventData);
							bool flag = spaceEraseCompleteData.Result >= 0;
							OVRAnchor.OnSpaceEraseComplete(spaceEraseCompleteData);
							Action<ulong, bool, Guid, OVRPlugin.SpaceStorageLocation> spaceEraseComplete = OVRManager.SpaceEraseComplete;
							if (spaceEraseComplete != null)
							{
								spaceEraseComplete(spaceEraseCompleteData.RequestId, flag, spaceEraseCompleteData.Uuid, spaceEraseCompleteData.Location);
							}
							OVRTask.SetResult<bool>(spaceEraseCompleteData.RequestId, flag);
							continue;
						}
						case OVRPlugin.EventType.SpaceShareResult:
						{
							OVRDeserialize.SpaceShareResultData spaceShareResultData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceShareResultData>(OVRManager.eventDataBuffer.EventData);
							OVRTask.SetResult<OVRResult<OVRAnchor.ShareResult>>(spaceShareResultData.RequestId, OVRResult.From<OVRAnchor.ShareResult>((OVRAnchor.ShareResult)spaceShareResultData.Result));
							Action<ulong, OVRSpatialAnchor.OperationResult> shareSpacesComplete = OVRManager.ShareSpacesComplete;
							if (shareSpacesComplete == null)
							{
								continue;
							}
							shareSpacesComplete(spaceShareResultData.RequestId, (OVRSpatialAnchor.OperationResult)spaceShareResultData.Result);
							continue;
						}
						case OVRPlugin.EventType.SpaceListSaveResult:
						{
							OVRDeserialize.SpaceListSaveResultData spaceListSaveResultData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceListSaveResultData>(OVRManager.eventDataBuffer.EventData);
							OVRAnchor.OnSpaceListSaveResult(spaceListSaveResultData);
							Action<ulong, OVRSpatialAnchor.OperationResult> spaceListSaveComplete = OVRManager.SpaceListSaveComplete;
							if (spaceListSaveComplete == null)
							{
								continue;
							}
							spaceListSaveComplete(spaceListSaveResultData.RequestId, (OVRSpatialAnchor.OperationResult)spaceListSaveResultData.Result);
							continue;
						}
						case OVRPlugin.EventType.SpaceShareToGroupsComplete:
						{
							OVRDeserialize.ShareSpacesToGroupsCompleteData shareSpacesToGroupsCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRAnchor.OnShareAnchorsToGroupsComplete(shareSpacesToGroupsCompleteData.RequestId, shareSpacesToGroupsCompleteData.Result);
							continue;
						}
						}
					}
					else
					{
						if (OVRManager.DisplayRefreshRateChanged != null)
						{
							OVRDeserialize.DisplayRefreshRateChangedData displayRefreshRateChangedData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.DisplayRefreshRateChangedData>(OVRManager.eventDataBuffer.EventData);
							OVRManager.DisplayRefreshRateChanged(displayRefreshRateChangedData.FromRefreshRate, displayRefreshRateChangedData.ToRefreshRate);
							continue;
						}
						continue;
					}
				}
				else
				{
					if (eventType == OVRPlugin.EventType.SceneCaptureComplete)
					{
						OVRDeserialize.SceneCaptureCompleteData sceneCaptureCompleteData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SceneCaptureCompleteData>(OVRManager.eventDataBuffer.EventData);
						Action<ulong, bool> sceneCaptureComplete = OVRManager.SceneCaptureComplete;
						if (sceneCaptureComplete != null)
						{
							sceneCaptureComplete(sceneCaptureCompleteData.RequestId, sceneCaptureCompleteData.Result >= 0);
						}
						OVRTask.SetResult<bool>(sceneCaptureCompleteData.RequestId, sceneCaptureCompleteData.Result >= 0);
						continue;
					}
					switch (eventType)
					{
					case OVRPlugin.EventType.SpaceDiscoveryResultsAvailable:
						OVRAnchor.OnSpaceDiscoveryResultsAvailable(OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceDiscoveryResultsData>(OVRManager.eventDataBuffer.EventData));
						continue;
					case OVRPlugin.EventType.SpaceDiscoveryComplete:
						OVRAnchor.OnSpaceDiscoveryComplete(OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpaceDiscoveryCompleteData>(OVRManager.eventDataBuffer.EventData));
						continue;
					case OVRPlugin.EventType.SpacesSaveResult:
					{
						OVRDeserialize.SpacesSaveResultData spacesSaveResultData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpacesSaveResultData>(OVRManager.eventDataBuffer.EventData);
						OVRAnchor.OnSaveSpacesResult(spacesSaveResultData);
						OVRTask.SetResult<OVRResult<OVRAnchor.SaveResult>>(spacesSaveResultData.RequestId, OVRResult.From<OVRAnchor.SaveResult>(spacesSaveResultData.Result));
						continue;
					}
					case OVRPlugin.EventType.SpacesEraseResult:
					{
						OVRDeserialize.SpacesEraseResultData spacesEraseResultData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.SpacesEraseResultData>(OVRManager.eventDataBuffer.EventData);
						OVRAnchor.OnEraseSpacesResult(spacesEraseResultData);
						OVRTask.SetResult<OVRResult<OVRAnchor.EraseResult>>(spacesEraseResultData.RequestId, OVRResult.From<OVRAnchor.EraseResult>(spacesEraseResultData.Result));
						continue;
					}
					default:
						switch (eventType)
						{
						case OVRPlugin.EventType.ColocationSessionStartAdvertisementComplete:
						{
							OVRDeserialize.StartColocationSessionAdvertisementCompleteData startColocationSessionAdvertisementCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionStartAdvertisementComplete(startColocationSessionAdvertisementCompleteData.RequestId, startColocationSessionAdvertisementCompleteData.Result, startColocationSessionAdvertisementCompleteData.AdvertisementUuid);
							continue;
						}
						case OVRPlugin.EventType.ColocationSessionAdvertisementComplete:
						{
							OVRDeserialize.ColocationSessionAdvertisementCompleteData colocationSessionAdvertisementCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionAdvertisementComplete(colocationSessionAdvertisementCompleteData.RequestId, colocationSessionAdvertisementCompleteData.Result);
							continue;
						}
						case OVRPlugin.EventType.ColocationSessionStopAdvertisementComplete:
						{
							OVRDeserialize.StopColocationSessionAdvertisementCompleteData stopColocationSessionAdvertisementCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionStopAdvertisementComplete(stopColocationSessionAdvertisementCompleteData.RequestId, stopColocationSessionAdvertisementCompleteData.Result);
							continue;
						}
						case OVRPlugin.EventType.ColocationSessionStartDiscoveryComplete:
						{
							OVRDeserialize.StartColocationSessionDiscoveryCompleteData startColocationSessionDiscoveryCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionStartDiscoveryComplete(startColocationSessionDiscoveryCompleteData.RequestId, startColocationSessionDiscoveryCompleteData.Result);
							continue;
						}
						case OVRPlugin.EventType.ColocationSessionDiscoveryResult:
						{
							OVRDeserialize.ColocationSessionDiscoveryResultData colocationSessionDiscoveryResultData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionDiscoveryResult(colocationSessionDiscoveryResultData.RequestId, colocationSessionDiscoveryResultData.AdvertisementUuid, colocationSessionDiscoveryResultData.AdvertisementMetadataCount, &colocationSessionDiscoveryResultData.AdvertisementMetadata.FixedElementField);
							continue;
						}
						case OVRPlugin.EventType.ColocationSessionDiscoveryComplete:
						{
							OVRDeserialize.ColocationSessionDiscoveryCompleteData colocationSessionDiscoveryCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionDiscoveryComplete(colocationSessionDiscoveryCompleteData.RequestId, colocationSessionDiscoveryCompleteData.Result);
							continue;
						}
						case OVRPlugin.EventType.ColocationSessionStopDiscoveryComplete:
						{
							OVRDeserialize.StopColocationSessionDiscoveryCompleteData stopColocationSessionDiscoveryCompleteData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
							OVRColocationSession.OnColocationSessionStopDiscoveryComplete(stopColocationSessionDiscoveryCompleteData.RequestId, stopColocationSessionDiscoveryCompleteData.Result);
							continue;
						}
						}
						break;
					}
				}
			}
			else if (eventType <= OVRPlugin.EventType.BoundaryVisibilityChanged)
			{
				if (eventType != OVRPlugin.EventType.PassthroughLayerResumed)
				{
					if (eventType == OVRPlugin.EventType.BoundaryVisibilityChanged)
					{
						OVRDeserialize.BoundaryVisibilityChangedData boundaryVisibilityChangedData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.BoundaryVisibilityChangedData>(OVRManager.eventDataBuffer.EventData);
						Action<OVRPlugin.BoundaryVisibility> boundaryVisibilityChanged = OVRManager.BoundaryVisibilityChanged;
						if (boundaryVisibilityChanged != null)
						{
							boundaryVisibilityChanged(boundaryVisibilityChangedData.BoundaryVisibility);
						}
						this.isBoundaryVisibilitySuppressed = (boundaryVisibilityChangedData.BoundaryVisibility == OVRPlugin.BoundaryVisibility.Suppressed);
						continue;
					}
				}
				else
				{
					if (OVRManager.PassthroughLayerResumed != null)
					{
						OVRDeserialize.PassthroughLayerResumedData passthroughLayerResumedData = OVRDeserialize.ByteArrayToStructure<OVRDeserialize.PassthroughLayerResumedData>(OVRManager.eventDataBuffer.EventData);
						OVRManager.PassthroughLayerResumed(passthroughLayerResumedData.LayerId);
						continue;
					}
					continue;
				}
			}
			else
			{
				if (eventType == OVRPlugin.EventType.CreateDynamicObjectTrackerResult)
				{
					OVRDeserialize.CreateDynamicObjectTrackerResultData createDynamicObjectTrackerResultData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
					OVRTask.SetResult<OVRResult<ulong, OVRPlugin.Result>>(OVRTask.GetId(createDynamicObjectTrackerResultData.Tracker, createDynamicObjectTrackerResultData.EventType), OVRResult<ulong, OVRPlugin.Result>.From(createDynamicObjectTrackerResultData.Tracker, createDynamicObjectTrackerResultData.Result));
					continue;
				}
				if (eventType == OVRPlugin.EventType.SetDynamicObjectTrackedClassesResult)
				{
					OVRDeserialize.SetDynamicObjectTrackedClassesResultData setDynamicObjectTrackedClassesResultData = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
					OVRTask.SetResult<OVRResult<OVRPlugin.Result>>(OVRTask.GetId(setDynamicObjectTrackedClassesResultData.Tracker, setDynamicObjectTrackedClassesResultData.EventType), OVRResult<OVRPlugin.Result>.From(setDynamicObjectTrackedClassesResultData.Result));
					continue;
				}
				if (eventType == OVRPlugin.EventType.ReferenceSpaceChangePending)
				{
					OVRDeserialize.EventDataReferenceSpaceChangePending eventDataReferenceSpaceChangePending = OVRManager.eventDataBuffer.MarshalEntireStructAs(Allocator.Temp);
					Action<OVRManager.TrackingOrigin, OVRPose?> trackingOriginChangePending = OVRManager.TrackingOriginChangePending;
					if (trackingOriginChangePending == null)
					{
						continue;
					}
					trackingOriginChangePending((OVRManager.TrackingOrigin)eventDataReferenceSpaceChangePending.ReferenceSpaceType, (eventDataReferenceSpaceChangePending.PoseValid == OVRPlugin.Bool.True) ? new OVRPose?(eventDataReferenceSpaceChangePending.PoseInPreviousSpace.ToOVRPose()) : null);
					continue;
				}
			}
			foreach (OVRManager.EventListener eventListener in this.eventListeners)
			{
				eventListener.OnEvent(OVRManager.eventDataBuffer);
			}
		}
	}

	public void UpdateDynamicResolutionVersion()
	{
		if (this.dynamicResolutionVersion == 0)
		{
			this.quest2MinDynamicResolutionScale = this.minDynamicResolutionScale;
			this.quest2MaxDynamicResolutionScale = this.maxDynamicResolutionScale;
			this.quest3MinDynamicResolutionScale = this.minDynamicResolutionScale;
			this.quest3MaxDynamicResolutionScale = this.maxDynamicResolutionScale;
		}
		this.dynamicResolutionVersion = OVRManager.MaxDynamicResolutionVersion;
	}

	public static Camera FindMainCamera()
	{
		Camera camera;
		if (OVRManager.lastFoundMainCamera != null && OVRManager.lastFoundMainCamera.TryGetTarget(out camera) && camera != null && camera.isActiveAndEnabled && camera.CompareTag("MainCamera"))
		{
			return camera;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("MainCamera");
		List<Camera> list = new List<Camera>(4);
		GameObject[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Camera component = array2[i].GetComponent<Camera>();
			if (component != null && component.enabled)
			{
				OVRCameraRig componentInParent = component.GetComponentInParent<OVRCameraRig>();
				if (componentInParent != null && componentInParent.trackingSpace != null)
				{
					list.Add(component);
				}
			}
		}
		Camera camera2;
		if (list.Count == 0)
		{
			camera2 = Camera.main;
		}
		else if (list.Count == 1)
		{
			camera2 = list[0];
		}
		else
		{
			if (!OVRManager.multipleMainCameraWarningPresented)
			{
				Debug.LogWarning("Multiple MainCamera found. Assume the real MainCamera is the camera with the least depth");
				OVRManager.multipleMainCameraWarningPresented = true;
			}
			list.Sort(delegate(Camera c0, Camera c1)
			{
				if (c0.depth < c1.depth)
				{
					return -1;
				}
				if (c0.depth <= c1.depth)
				{
					return 0;
				}
				return 1;
			});
			camera2 = list[0];
		}
		if (camera2 != null)
		{
			OVRManager.suppressUnableToFindMainCameraMessage = false;
		}
		else if (!OVRManager.suppressUnableToFindMainCameraMessage)
		{
			Debug.Log("[OVRManager] unable to find a valid camera");
			OVRManager.suppressUnableToFindMainCameraMessage = true;
		}
		OVRManager.lastFoundMainCamera = new WeakReference<Camera>(camera2);
		return camera2;
	}

	private void OnDisable()
	{
		OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer component = base.GetComponent<OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	private void LateUpdate()
	{
		OVRHaptics.Process();
		if (OVRManager.m_SpaceWarpEnabled)
		{
			Camera camera = OVRManager.FindMainCamera();
			if (camera != null)
			{
				Camera y = null;
				if (OVRManager.m_lastSpaceWarpCamera != null)
				{
					OVRManager.m_lastSpaceWarpCamera.TryGetTarget(out y);
				}
				if (camera != y)
				{
					Debug.Log("Main camera changed. Updating new camera for space warp.");
					OVRManager.PrepareCameraForSpaceWarp(camera);
					OVRManager.m_lastSpaceWarpCamera = new WeakReference<Camera>(camera);
				}
				Vector3 position = OVRManager.m_AppSpaceTransform.position;
				Quaternion rotation = OVRManager.m_AppSpaceTransform.rotation;
				Vector3 lossyScale = OVRManager.m_AppSpaceTransform.lossyScale;
				OVRManager.SetAppSpacePosition(position.x / lossyScale.x, position.y / lossyScale.y, position.z / lossyScale.z);
				OVRManager.SetAppSpaceRotation(rotation.x, rotation.y, rotation.z, rotation.w);
				return;
			}
			OVRManager.SetAppSpacePosition(0f, 0f, 0f);
			OVRManager.SetAppSpaceRotation(0f, 0f, 0f, 1f);
		}
	}

	private void FixedUpdate()
	{
		OVRInput.FixedUpdate();
	}

	private void OnDestroy()
	{
		Debug.Log("[OVRManager] OnDestroy");
		OVRManager.OVRManagerinitialized = false;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("[OVRManager] OnApplicationPause(true)");
			return;
		}
		Debug.Log("[OVRManager] OnApplicationPause(false)");
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			Debug.Log("[OVRManager] OnApplicationFocus(true)");
			return;
		}
		Debug.Log("[OVRManager] OnApplicationFocus(false)");
	}

	private void OnApplicationQuit()
	{
		Debug.Log("[OVRManager] OnApplicationQuit");
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public void ReturnToLauncher()
	{
		OVRManager.PlatformUIConfirmQuit();
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static void PlatformUIConfirmQuit()
	{
		if (!OVRManager.isHmdPresent)
		{
			return;
		}
		OVRPlugin.ShowUI(OVRPlugin.PlatformUI.ConfirmQuit);
	}

	public static void StaticInitializeMixedRealityCapture(OVRMixedRealityCaptureConfiguration configuration)
	{
		if (!OVRManager.staticMixedRealityCaptureInitialized)
		{
			OVRManager.staticMrcSettings = ScriptableObject.CreateInstance<OVRMixedRealityCaptureSettings>();
			OVRManager.staticMrcSettings.ReadFrom(configuration);
			OVRManager.staticPrevEnableMixedRealityCapture = false;
			OVRManager.staticMixedRealityCaptureInitialized = true;
			return;
		}
		OVRManager.staticMrcSettings.ApplyTo(configuration);
	}

	public static void StaticUpdateMixedRealityCapture(OVRMixedRealityCaptureConfiguration configuration, GameObject gameObject, OVRManager.TrackingOrigin trackingOrigin)
	{
		if (!OVRManager.staticMixedRealityCaptureInitialized)
		{
			return;
		}
		if (configuration.enableMixedReality)
		{
			Camera camera = OVRManager.FindMainCamera();
			if (camera != null)
			{
				if (!OVRManager.staticPrevEnableMixedRealityCapture)
				{
					OVRPlugin.SendEvent("mixed_reality_capture", "activated", "");
					Debug.Log("MixedRealityCapture: activate");
					OVRManager.staticPrevEnableMixedRealityCapture = true;
				}
				OVRMixedReality.Update(gameObject, camera, configuration, trackingOrigin);
				OVRManager.suppressDisableMixedRealityBecauseOfNoMainCameraWarning = false;
			}
			else if (!OVRManager.suppressDisableMixedRealityBecauseOfNoMainCameraWarning)
			{
				Debug.LogWarning("Main Camera is not set, Mixed Reality disabled");
				OVRManager.suppressDisableMixedRealityBecauseOfNoMainCameraWarning = true;
			}
		}
		else if (OVRManager.staticPrevEnableMixedRealityCapture)
		{
			Debug.Log("MixedRealityCapture: deactivate");
			OVRManager.staticPrevEnableMixedRealityCapture = false;
			OVRMixedReality.Cleanup();
		}
		OVRManager.staticMrcSettings.ReadFrom(configuration);
	}

	public static void StaticShutdownMixedRealityCapture(OVRMixedRealityCaptureConfiguration configuration)
	{
		if (OVRManager.staticMixedRealityCaptureInitialized)
		{
			Object.Destroy(OVRManager.staticMrcSettings);
			OVRManager.staticMrcSettings = null;
			OVRMixedReality.Cleanup();
			OVRManager.staticMixedRealityCaptureInitialized = false;
		}
	}

	private static bool PassthroughInitializedOrPending(OVRManager.PassthroughInitializationState state)
	{
		return state == OVRManager.PassthroughInitializationState.Pending || state == OVRManager.PassthroughInitializationState.Initialized;
	}

	private static bool InitializeInsightPassthrough()
	{
		if (OVRManager.PassthroughInitializedOrPending(OVRManager._passthroughInitializationState.Value))
		{
			return false;
		}
		OVRPlugin.InitializeInsightPassthrough();
		OVRPlugin.Result insightPassthroughInitializationState = OVRPlugin.GetInsightPassthroughInitializationState();
		if (insightPassthroughInitializationState < OVRPlugin.Result.Success)
		{
			OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Failed;
			Debug.LogError("Failed to initialize Insight Passthrough. Passthrough will be unavailable. Error " + insightPassthroughInitializationState.ToString() + ".");
		}
		else if (insightPassthroughInitializationState == OVRPlugin.Result.Success_Pending)
		{
			OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Pending;
		}
		else
		{
			OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Initialized;
		}
		return OVRManager.PassthroughInitializedOrPending(OVRManager._passthroughInitializationState.Value);
	}

	private static void ShutdownInsightPassthrough()
	{
		if (!OVRManager.PassthroughInitializedOrPending(OVRManager._passthroughInitializationState.Value))
		{
			OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Unspecified;
			return;
		}
		if (OVRPlugin.ShutdownInsightPassthrough())
		{
			OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Unspecified;
			return;
		}
		if (OVRPlugin.IsInsightPassthroughInitialized())
		{
			Debug.LogError("Failed to shut down passthrough. It may be still in use.");
			return;
		}
		OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Unspecified;
	}

	private static void UpdateInsightPassthrough(bool shouldBeEnabled)
	{
		if (shouldBeEnabled != OVRManager.PassthroughInitializedOrPending(OVRManager._passthroughInitializationState.Value))
		{
			if (!shouldBeEnabled)
			{
				OVRManager.ShutdownInsightPassthrough();
				return;
			}
			if (OVRManager._passthroughInitializationState.Value != OVRManager.PassthroughInitializationState.Failed)
			{
				OVRManager.InitializeInsightPassthrough();
				return;
			}
		}
		else if (OVRManager._passthroughInitializationState.Value == OVRManager.PassthroughInitializationState.Pending)
		{
			OVRPlugin.Result insightPassthroughInitializationState = OVRPlugin.GetInsightPassthroughInitializationState();
			if (insightPassthroughInitializationState == OVRPlugin.Result.Success)
			{
				OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Initialized;
				return;
			}
			if (insightPassthroughInitializationState < OVRPlugin.Result.Success)
			{
				OVRManager._passthroughInitializationState.Value = OVRManager.PassthroughInitializationState.Failed;
				Debug.LogError("Failed to initialize Insight Passthrough. Passthrough will be unavailable. Error " + insightPassthroughInitializationState.ToString() + ".");
			}
		}
	}

	private void InitializeBoundary()
	{
		OVRPlugin.BoundaryVisibility boundaryVisibility2;
		OVRPlugin.Result boundaryVisibility = OVRPlugin.GetBoundaryVisibility(out boundaryVisibility2);
		if (boundaryVisibility == OVRPlugin.Result.Success)
		{
			this.isBoundaryVisibilitySuppressed = (boundaryVisibility2 == OVRPlugin.BoundaryVisibility.Suppressed);
			return;
		}
		if (boundaryVisibility == OVRPlugin.Result.Failure_Unsupported || boundaryVisibility == OVRPlugin.Result.Failure_NotYetImplemented)
		{
			this.isBoundaryVisibilitySuppressed = false;
			this.shouldBoundaryVisibilityBeSuppressed = false;
			return;
		}
		Debug.LogWarning("Could not retrieve initial boundary visibility state. Defaulting to not suppressed.");
		this.isBoundaryVisibilitySuppressed = false;
	}

	private void UpdateBoundary()
	{
		if (this.shouldBoundaryVisibilityBeSuppressed == this.isBoundaryVisibilitySuppressed)
		{
			return;
		}
		if (!OVRManager.PassthroughInitializedOrPending(OVRManager._passthroughInitializationState.Value) || !this.isInsightPassthroughEnabled)
		{
			return;
		}
		OVRPlugin.Result result = OVRPlugin.RequestBoundaryVisibility(this.shouldBoundaryVisibilityBeSuppressed ? OVRPlugin.BoundaryVisibility.Suppressed : OVRPlugin.BoundaryVisibility.NotSuppressed);
		if (result == OVRPlugin.Result.Warning_BoundaryVisibilitySuppressionNotAllowed)
		{
			if (!this._updateBoundaryLogOnce)
			{
				this._updateBoundaryLogOnce = true;
				Debug.LogWarning("Cannot suppress boundary visibility as it's required to be on.");
				return;
			}
		}
		else if (result == OVRPlugin.Result.Success)
		{
			this._updateBoundaryLogOnce = false;
			this.isBoundaryVisibilitySuppressed = this.shouldBoundaryVisibilityBeSuppressed;
		}
	}

	public static bool IsMultimodalHandsControllersSupported()
	{
		return OVRPlugin.IsMultimodalHandsControllersSupported();
	}

	public static bool IsInsightPassthroughSupported()
	{
		return OVRPlugin.IsInsightPassthroughSupported();
	}

	public static OVRManager.PassthroughCapabilities GetPassthroughCapabilities()
	{
		if (OVRManager._passthroughCapabilities == null)
		{
			OVRPlugin.PassthroughCapabilities passthroughCapabilities = default(OVRPlugin.PassthroughCapabilities);
			if (!OVRPlugin.GetPassthroughCapabilities(ref passthroughCapabilities).IsSuccess())
			{
				passthroughCapabilities.Flags = OVRPlugin.GetPassthroughCapabilityFlags();
				passthroughCapabilities.MaxColorLutResolution = 64U;
			}
			OVRManager._passthroughCapabilities = new OVRManager.PassthroughCapabilities((passthroughCapabilities.Flags & OVRPlugin.PassthroughCapabilityFlags.Passthrough) == OVRPlugin.PassthroughCapabilityFlags.Passthrough, (passthroughCapabilities.Flags & OVRPlugin.PassthroughCapabilityFlags.Color) == OVRPlugin.PassthroughCapabilityFlags.Color, passthroughCapabilities.MaxColorLutResolution);
		}
		return OVRManager._passthroughCapabilities;
	}

	public static bool IsInsightPassthroughInitialized()
	{
		return OVRManager._passthroughInitializationState.Value == OVRManager.PassthroughInitializationState.Initialized;
	}

	public static bool HasInsightPassthroughInitFailed()
	{
		return OVRManager._passthroughInitializationState.Value == OVRManager.PassthroughInitializationState.Failed;
	}

	public static bool IsInsightPassthroughInitPending()
	{
		return OVRManager._passthroughInitializationState.Value == OVRManager.PassthroughInitializationState.Pending;
	}

	public static bool IsPassthroughRecommended()
	{
		OVRPlugin.PassthroughPreferences passthroughPreferences;
		OVRPlugin.GetPassthroughPreferences(out passthroughPreferences);
		return (passthroughPreferences.Flags & OVRPlugin.PassthroughPreferenceFlags.DefaultToActive) == OVRPlugin.PassthroughPreferenceFlags.DefaultToActive;
	}

	public static bool GetFixedFoveatedRenderingSupported()
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			string[] array = "XR_FB_foveation XR_FB_foveation_configuration XR_FB_foveation_vulkan ".Split(' ', StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				if (!OpenXRRuntime.IsExtensionEnabled(array[i]))
				{
					return false;
				}
			}
			return true;
		}
		return OVRPlugin.fixedFoveatedRenderingSupported;
	}

	public static OVRManager.FoveatedRenderingLevel GetFoveatedRenderingLevel()
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			return MetaXRFoveationFeature.foveatedRenderingLevel;
		}
		return (OVRManager.FoveatedRenderingLevel)OVRPlugin.foveatedRenderingLevel;
	}

	public static void SetFoveatedRenderingLevel(OVRManager.FoveatedRenderingLevel level)
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			MetaXRFoveationFeature.foveatedRenderingLevel = level;
			return;
		}
		OVRPlugin.foveatedRenderingLevel = (OVRPlugin.FoveatedRenderingLevel)level;
	}

	public static bool GetDynamicFoveatedRenderingEnabled()
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			return MetaXRFoveationFeature.useDynamicFoveatedRendering;
		}
		return OVRPlugin.useDynamicFoveatedRendering;
	}

	public static void SetDynamicFoveatedRenderingEnabled(bool enabled)
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			MetaXRFoveationFeature.useDynamicFoveatedRendering = enabled;
			return;
		}
		OVRPlugin.useDynamicFoveatedRendering = enabled;
	}

	public static bool GetEyeTrackedFoveatedRenderingSupported()
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			return MetaXREyeTrackedFoveationFeature.eyeTrackedFoveatedRenderingSupported;
		}
		return OVRPlugin.eyeTrackedFoveatedRenderingSupported;
	}

	public static bool GetEyeTrackedFoveatedRenderingEnabled()
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			return MetaXREyeTrackedFoveationFeature.eyeTrackedFoveatedRenderingEnabled;
		}
		return OVRPlugin.eyeTrackedFoveatedRenderingEnabled;
	}

	public static void SetEyeTrackedFoveatedRenderingEnabled(bool enabled)
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			MetaXREyeTrackedFoveationFeature.eyeTrackedFoveatedRenderingEnabled = enabled;
			return;
		}
		OVRPlugin.eyeTrackedFoveatedRenderingEnabled = enabled;
	}

	public static void SetSpaceWarp_Internal(bool enabled)
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			MetaXRSpaceWarp.SetSpaceWarp(enabled);
			return;
		}
		Debug.Log("Failed to set Space Warp. Current XR Loader does not support this feature.");
	}

	public static void SetAppSpacePosition(float x, float y, float z)
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			MetaXRSpaceWarp.SetAppSpacePosition(x, y, z);
			return;
		}
		Debug.Log("Failed to set Space Warp App Position. Current XR Loader does not support this feature.");
	}

	public static void SetAppSpaceRotation(float x, float y, float z, float w)
	{
		if (OVRManager.IsOpenXRLoaderActive())
		{
			MetaXRSpaceWarp.SetAppSpaceRotation(x, y, z, w);
			return;
		}
		Debug.Log("Failed to set Space Warp App Rotation. Current XR Loader does not support this feature.");
	}

	public static bool SetColorScaleAndOffset_Internal(Vector4 colorScale, Vector4 colorOffset, bool applyToAllLayers)
	{
		return OVRManager.IsOpenXRLoaderActive() && OVRPlugin.SetColorScaleAndOffset(colorScale, colorOffset, applyToAllLayers);
	}

	private static bool IsOpenXRLoaderActive()
	{
		return XRGeneralSettings.Instance.Manager.activeLoader as OpenXRLoader != null;
	}

	protected static OVRProfile _profile;

	protected IEnumerable<Camera> disabledCameras;

	private static int _isHmdPresentCacheFrame = -1;

	private static bool _isHmdPresent = false;

	private static bool _wasHmdPresent = false;

	private static bool _hasVrFocusCached = false;

	private static bool _hasVrFocus = false;

	private static bool _hadVrFocus = false;

	private static bool _hadInputFocus = true;

	[Header("Performance/Quality")]
	[SerializeField]
	[Tooltip("If true, both eyes will see the same image, rendered from the center eye pose, saving performance.")]
	private bool _monoscopic;

	[SerializeField]
	[Tooltip("The sharpen filter of the eye buffer. This amplifies contrast and fine details.")]
	private OVRPlugin.LayerSharpenType _sharpenType;

	[HideInInspector]
	private OVRManager.ColorSpace _colorGamut = OVRManager.ColorSpace.P3;

	[SerializeField]
	[HideInInspector]
	[Tooltip("Enable Dynamic Resolution. This will allocate render buffers to maxDynamicResolutionScale size and will change the viewport to adapt performance. Mobile only.")]
	private bool _enableDynamicResolution;

	[HideInInspector]
	public float minDynamicResolutionScale = 1f;

	[HideInInspector]
	public float maxDynamicResolutionScale = 1f;

	[SerializeField]
	[HideInInspector]
	public float quest2MinDynamicResolutionScale = 0.7f;

	[SerializeField]
	[HideInInspector]
	public float quest2MaxDynamicResolutionScale = 1.3f;

	[SerializeField]
	[HideInInspector]
	public float quest3MinDynamicResolutionScale = 0.7f;

	[SerializeField]
	[HideInInspector]
	public float quest3MaxDynamicResolutionScale = 1.6f;

	private const int _pixelStepPerFrame = 32;

	[Range(0.5f, 2f)]
	[HideInInspector]
	[Tooltip("Min RenderScale the app can reach under adaptive resolution mode")]
	[Obsolete("Deprecated. Use minDynamicRenderScale instead.", false)]
	public float minRenderScale = 0.7f;

	[Range(0.5f, 2f)]
	[HideInInspector]
	[Tooltip("Max RenderScale the app can reach under adaptive resolution mode")]
	[Obsolete("Deprecated. Use maxDynamicRenderScale instead.", false)]
	public float maxRenderScale = 1f;

	[SerializeField]
	[Tooltip("Set the relative offset rotation of head poses")]
	private Vector3 _headPoseRelativeOffsetRotation;

	[SerializeField]
	[Tooltip("Set the relative offset translation of head poses")]
	private Vector3 _headPoseRelativeOffsetTranslation;

	public int profilerTcpPort = 32419;

	[HideInInspector]
	public bool expandMixedRealityCapturePropertySheet;

	[HideInInspector]
	[Tooltip("If true, Mixed Reality mode will be enabled. It would be always set to false when the game is launching without editor")]
	public bool enableMixedReality;

	[HideInInspector]
	public OVRManager.CompositionMethod compositionMethod;

	[HideInInspector]
	[Tooltip("Extra hidden layers")]
	public LayerMask extraHiddenLayers;

	[HideInInspector]
	[Tooltip("Extra visible layers")]
	public LayerMask extraVisibleLayers;

	[HideInInspector]
	[Tooltip("Dynamic Culling Mask")]
	public bool dynamicCullingMask = true;

	[HideInInspector]
	[Tooltip("Backdrop color for Rift (External Compositon)")]
	public Color externalCompositionBackdropColorRift = Color.green;

	[HideInInspector]
	[Tooltip("Backdrop color for Quest (External Compositon)")]
	public Color externalCompositionBackdropColorQuest = Color.clear;

	[HideInInspector]
	[Tooltip("The camera device for direct composition")]
	[Obsolete("Deprecated", false)]
	public OVRManager.CameraDevice capturingCameraDevice;

	[HideInInspector]
	[Tooltip("Flip the camera frame horizontally")]
	[Obsolete("Deprecated", false)]
	public bool flipCameraFrameHorizontally;

	[HideInInspector]
	[Tooltip("Flip the camera frame vertically")]
	[Obsolete("Deprecated", false)]
	public bool flipCameraFrameVertically;

	[HideInInspector]
	[Tooltip("Delay the touch controller pose by a short duration (0 to 0.5 second) to match the physical camera latency")]
	[Obsolete("Deprecated", false)]
	public float handPoseStateLatency;

	[HideInInspector]
	[Tooltip("Delay the foreground / background image in the sandwich composition to match the physical camera latency. The maximum duration is sandwichCompositionBufferedFrames / {Game FPS}")]
	[Obsolete("Deprecated", false)]
	public float sandwichCompositionRenderLatency;

	[HideInInspector]
	[Tooltip("The number of frames are buffered in the SandWich composition. The more buffered frames, the more memory it would consume.")]
	[Obsolete("Deprecated", false)]
	public int sandwichCompositionBufferedFrames = 8;

	[HideInInspector]
	[Tooltip("Chroma Key Color")]
	[Obsolete("Deprecated", false)]
	public Color chromaKeyColor = Color.green;

	[HideInInspector]
	[Tooltip("Chroma Key Similarity")]
	[Obsolete("Deprecated", false)]
	public float chromaKeySimilarity = 0.6f;

	[HideInInspector]
	[Tooltip("Chroma Key Smooth Range")]
	[Obsolete("Deprecated", false)]
	public float chromaKeySmoothRange = 0.03f;

	[HideInInspector]
	[Tooltip("Chroma Key Spill Range")]
	[Obsolete("Deprecated", false)]
	public float chromaKeySpillRange = 0.06f;

	[HideInInspector]
	[Tooltip("Use dynamic lighting (Depth sensor required)")]
	[Obsolete("Deprecated", false)]
	public bool useDynamicLighting;

	[HideInInspector]
	[Tooltip("The quality level of depth image. The lighting could be more smooth and accurate with high quality depth, but it would also be more costly in performance.")]
	[Obsolete("Deprecated", false)]
	public OVRManager.DepthQuality depthQuality = OVRManager.DepthQuality.Medium;

	[HideInInspector]
	[Tooltip("Smooth factor in dynamic lighting. Larger is smoother")]
	[Obsolete("Deprecated", false)]
	public float dynamicLightingSmoothFactor = 8f;

	[HideInInspector]
	[Tooltip("The maximum depth variation across the edges. Make it smaller to smooth the lighting on the edges.")]
	[Obsolete("Deprecated", false)]
	public float dynamicLightingDepthVariationClampingValue = 0.001f;

	[HideInInspector]
	[Tooltip("Type of virutal green screen ")]
	[Obsolete("Deprecated", false)]
	public OVRManager.VirtualGreenScreenType virtualGreenScreenType;

	[HideInInspector]
	[Tooltip("Top Y of virtual green screen")]
	[Obsolete("Deprecated", false)]
	public float virtualGreenScreenTopY = 10f;

	[HideInInspector]
	[Tooltip("Bottom Y of virtual green screen")]
	[Obsolete("Deprecated", false)]
	public float virtualGreenScreenBottomY = -10f;

	[HideInInspector]
	[Tooltip("When using a depth camera (e.g. ZED), whether to use the depth in virtual green screen culling.")]
	[Obsolete("Deprecated", false)]
	public bool virtualGreenScreenApplyDepthCulling;

	[HideInInspector]
	[Tooltip("The tolerance value (in meter) when using the virtual green screen with a depth camera. Make it bigger if the foreground objects got culled incorrectly.")]
	[Obsolete("Deprecated", false)]
	public float virtualGreenScreenDepthTolerance = 0.2f;

	[HideInInspector]
	[Tooltip("(Quest-only) control if the mixed reality capture mode can be activated automatically through remote network connection.")]
	public OVRManager.MrcActivationMode mrcActivationMode;

	public OVRManager.InstantiateMrcCameraDelegate instantiateMixedRealityCameraGameObject;

	[HideInInspector]
	[Tooltip("Specify if simultaneous hands and controllers should be enabled. ")]
	public bool launchSimultaneousHandsControllersOnStartup;

	[HideInInspector]
	[Tooltip("Specify if Insight Passthrough should be enabled. Passthrough layers can only be used if passthrough is enabled.")]
	public bool isInsightPassthroughEnabled;

	[HideInInspector]
	public bool shouldBoundaryVisibilityBeSuppressed;

	private bool _updateBoundaryLogOnce;

	[SerializeField]
	[HideInInspector]
	internal bool requestBodyTrackingPermissionOnStartup;

	[SerializeField]
	[HideInInspector]
	internal bool requestFaceTrackingPermissionOnStartup;

	[SerializeField]
	[HideInInspector]
	internal bool requestEyeTrackingPermissionOnStartup;

	[SerializeField]
	[HideInInspector]
	internal bool requestScenePermissionOnStartup;

	[SerializeField]
	[HideInInspector]
	internal bool requestRecordAudioPermissionOnStartup;

	public static string OCULUS_UNITY_NAME_STR = "Oculus";

	public static string OPENVR_UNITY_NAME_STR = "OpenVR";

	public static OVRManager.XRDevice loadedXRDevice;

	private static bool _isSystemHeadsetThemeCached = false;

	private static OVRManager.SystemHeadsetTheme _cachedSystemHeadsetTheme = OVRManager.SystemHeadsetTheme.Dark;

	protected static Vector3 OpenVRTouchRotationOffsetEulerLeft = new Vector3(40f, 0f, 0f);

	protected static Vector3 OpenVRTouchRotationOffsetEulerRight = new Vector3(40f, 0f, 0f);

	protected static Vector3 OpenVRTouchPositionOffsetLeft = new Vector3(0.0075f, -0.005f, -0.0525f);

	protected static Vector3 OpenVRTouchPositionOffsetRight = new Vector3(-0.0075f, -0.005f, -0.0525f);

	protected static WeakReference<Camera> m_lastSpaceWarpCamera;

	protected static bool m_SpaceWarpEnabled;

	protected static Transform m_AppSpaceTransform;

	protected static DepthTextureMode m_CachedDepthTextureMode;

	[SerializeField]
	[Tooltip("Available only for devices that support local dimming. It improves visual quality with a better display contrast ratio, but at a minor GPU performance cost.")]
	private bool _localDimming = true;

	[Header("Tracking")]
	[SerializeField]
	[Tooltip("Defines the current tracking origin type.")]
	private OVRManager.TrackingOrigin _trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;

	[Tooltip("If true, head tracking will affect the position of each OVRCameraRig's cameras.")]
	public bool usePositionTracking = true;

	[HideInInspector]
	public bool useRotationTracking = true;

	[Tooltip("If true, the distance between the user's eyes will affect the position of each OVRCameraRig's cameras.")]
	public bool useIPDInPositionTracking = true;

	[Tooltip("If true, each scene load will cause the head pose to reset. This function only works on Rift.")]
	public bool resetTrackerOnLoad;

	[Tooltip("If true, the Reset View in the universal menu will cause the pose to be reset in PC VR. This should generally be enabled for applications with a stationary position in the virtual world and will allow the View Reset command to place the person back to a predefined location (such as a cockpit seat). Set this to false if you have a locomotion system because resetting the view would effectively teleport the player to potentially invalid locations.")]
	public bool AllowRecenter = true;

	[Tooltip("If true, rendered controller latency is reduced by several ms, as the left/right controllers will have their positions updated right before rendering.")]
	public bool LateControllerUpdate = true;

	[Tooltip("Late latching is a feature that can reduce rendered head/controller latency by a substantial amount. Before enabling, be sure to go over the documentation to ensure that the feature is used correctly. This feature must also be enabled through the Oculus XR Plugin settings.")]
	public bool LateLatching;

	private static OVRManager.ControllerDrivenHandPosesType _readOnlyControllerDrivenHandPosesType = OVRManager.ControllerDrivenHandPosesType.None;

	[Tooltip("Defines if hand poses can be populated by controller data.")]
	public OVRManager.ControllerDrivenHandPosesType controllerDrivenHandPosesType;

	[Tooltip("Allows the application to use simultaneous hands and controllers functionality. This option must be enabled at build time.")]
	public bool SimultaneousHandsAndControllersEnabled;

	[SerializeField]
	[HideInInspector]
	private bool _readOnlyWideMotionModeHandPosesEnabled;

	[Tooltip("Defines if hand poses can leverage algorithms to retrieve hand poses outside of the normal tracking area.")]
	public bool wideMotionModeHandPosesEnabled;

	private static bool _isUserPresentCached = false;

	private static bool _isUserPresent = false;

	private static bool _wasUserPresent = false;

	private static bool prevAudioOutIdIsCached = false;

	private static bool prevAudioInIdIsCached = false;

	private static string prevAudioOutId = string.Empty;

	private static string prevAudioInId = string.Empty;

	private static bool wasPositionTracked = false;

	private static OVRPlugin.EventDataBuffer eventDataBuffer = default(OVRPlugin.EventDataBuffer);

	private HashSet<OVRManager.EventListener> eventListeners = new HashSet<OVRManager.EventListener>();

	public static string UnityAlphaOrBetaVersionWarningMessage = "WARNING: It's not recommended to use Unity alpha/beta release in Oculus development. Use a stable release if you encounter any issue.";

	public static int MaxDynamicResolutionVersion = 1;

	[SerializeField]
	[HideInInspector]
	public int dynamicResolutionVersion;

	public static bool OVRManagerinitialized = false;

	private static List<XRDisplaySubsystem> s_displaySubsystems;

	private static List<XRDisplaySubsystemDescriptor> s_displaySubsystemDescriptors;

	private static List<XRInputSubsystem> s_inputSubsystems;

	private static bool multipleMainCameraWarningPresented = false;

	private static bool suppressUnableToFindMainCameraMessage = false;

	private static WeakReference<Camera> lastFoundMainCamera = null;

	public static bool staticMixedRealityCaptureInitialized = false;

	public static bool staticPrevEnableMixedRealityCapture = false;

	public static OVRMixedRealityCaptureSettings staticMrcSettings = null;

	private static bool suppressDisableMixedRealityBecauseOfNoMainCameraWarning = false;

	public static Action<bool> OnPassthroughInitializedStateChange;

	private static OVRManager.Observable<OVRManager.PassthroughInitializationState> _passthroughInitializationState = new OVRManager.Observable<OVRManager.PassthroughInitializationState>(OVRManager.PassthroughInitializationState.Unspecified, delegate(OVRManager.PassthroughInitializationState newValue)
	{
		Action<bool> onPassthroughInitializedStateChange = OVRManager.OnPassthroughInitializedStateChange;
		if (onPassthroughInitializedStateChange == null)
		{
			return;
		}
		onPassthroughInitializedStateChange(newValue == OVRManager.PassthroughInitializationState.Initialized);
	});

	private static OVRManager.PassthroughCapabilities _passthroughCapabilities;

	public enum XrApi
	{
		Unknown,
		CAPI,
		VRAPI,
		OpenXR
	}

	public enum TrackingOrigin
	{
		EyeLevel,
		FloorLevel,
		Stage,
		Stationary = 6
	}

	public enum EyeTextureFormat
	{
		Default,
		R16G16B16A16_FP = 2,
		R11G11B10_FP
	}

	public enum FoveatedRenderingLevel
	{
		Off,
		Low,
		Medium,
		High,
		HighTop
	}

	[Obsolete("Please use FoveatedRenderingLevel instead")]
	public enum FixedFoveatedRenderingLevel
	{
		Off,
		Low,
		Medium,
		High,
		HighTop
	}

	[Obsolete("Please use FoveatedRenderingLevel instead")]
	public enum TiledMultiResLevel
	{
		Off,
		LMSLow,
		LMSMedium,
		LMSHigh,
		LMSHighTop
	}

	public enum SystemHeadsetType
	{
		None,
		Oculus_Quest = 8,
		Oculus_Quest_2,
		Meta_Quest_Pro,
		Meta_Quest_3,
		Meta_Quest_3S,
		Placeholder_13,
		Placeholder_14,
		Placeholder_15,
		Placeholder_16,
		Placeholder_17,
		Placeholder_18,
		Placeholder_19,
		Placeholder_20,
		Rift_DK1 = 4096,
		Rift_DK2,
		Rift_CV1,
		Rift_CB,
		Rift_S,
		Oculus_Link_Quest,
		Oculus_Link_Quest_2,
		Meta_Link_Quest_Pro,
		Meta_Link_Quest_3,
		Meta_Link_Quest_3S,
		PC_Placeholder_4106,
		PC_Placeholder_4107,
		PC_Placeholder_4108,
		PC_Placeholder_4109,
		PC_Placeholder_4110,
		PC_Placeholder_4111,
		PC_Placeholder_4112,
		PC_Placeholder_4113
	}

	public enum SystemHeadsetTheme
	{
		Dark,
		Light
	}

	public enum XRDevice
	{
		Unknown,
		Oculus,
		OpenVR
	}

	public enum ColorSpace
	{
		Unknown,
		Unmanaged,
		Rec_2020,
		Rec_709,
		Rift_CV1,
		Rift_S,
		[InspectorName("Quest 1")]
		Quest,
		[InspectorName("DCI-P3 (Recommended)")]
		P3,
		Adobe_RGB
	}

	public enum ProcessorPerformanceLevel
	{
		PowerSavings,
		SustainedLow,
		SustainedHigh,
		Boost
	}

	public enum ControllerDrivenHandPosesType
	{
		None,
		ConformingToController,
		Natural
	}

	public interface EventListener
	{
		void OnEvent(OVRPlugin.EventDataBuffer eventData);
	}

	public enum CompositionMethod
	{
		External,
		[Obsolete("Deprecated. Direct composition is no longer supported", false)]
		Direct
	}

	[Obsolete("Deprecated", false)]
	public enum CameraDevice
	{
		WebCamera0,
		WebCamera1,
		ZEDCamera
	}

	[Obsolete("Deprecated", false)]
	public enum DepthQuality
	{
		Low,
		Medium,
		High
	}

	[Obsolete("Deprecated", false)]
	public enum VirtualGreenScreenType
	{
		Off,
		[Obsolete("Deprecated. This enum value will not be supported in OpenXR", false)]
		OuterBoundary,
		PlayArea
	}

	public enum MrcActivationMode
	{
		Automatic,
		Disabled
	}

	public enum MrcCameraType
	{
		Normal,
		Foreground,
		Background
	}

	public delegate GameObject InstantiateMrcCameraDelegate(GameObject mainCameraGameObject, OVRManager.MrcCameraType cameraType);

	private enum PassthroughInitializationState
	{
		Unspecified,
		Pending,
		Initialized,
		Failed
	}

	public class PassthroughCapabilities
	{
		public bool SupportsPassthrough { get; }

		public bool SupportsColorPassthrough { get; }

		public uint MaxColorLutResolution { get; }

		public PassthroughCapabilities(bool supportsPassthrough, bool supportsColorPassthrough, uint maxColorLutResolution)
		{
			this.SupportsPassthrough = supportsPassthrough;
			this.SupportsColorPassthrough = supportsColorPassthrough;
			this.MaxColorLutResolution = maxColorLutResolution;
		}
	}

	private class Observable<T>
	{
		public T Value
		{
			get
			{
				return this._value;
			}
			set
			{
				T value2 = this._value;
				this._value = value;
				if (this.OnChanged != null)
				{
					this.OnChanged(value);
				}
			}
		}

		public Observable()
		{
		}

		public Observable(T defaultValue)
		{
			this._value = defaultValue;
		}

		public Observable(T defaultValue, Action<T> callback) : this(defaultValue)
		{
			this.OnChanged = (Action<T>)Delegate.Combine(this.OnChanged, callback);
		}

		private T _value;

		public Action<T> OnChanged;
	}
}
