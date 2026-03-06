using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

public static class OVRPlugin
{
	public static Version version
	{
		get
		{
			if (OVRPlugin._version == null)
			{
				try
				{
					string text = OVRPlugin.OVRP_1_1_0.ovrp_GetVersion();
					if (text != null)
					{
						text = text.Split('-', StringSplitOptions.None)[0];
						OVRPlugin._version = new Version(text);
					}
					else
					{
						OVRPlugin._version = OVRPlugin._versionZero;
					}
				}
				catch
				{
					OVRPlugin._version = OVRPlugin._versionZero;
				}
				if (OVRPlugin._version == OVRPlugin.OVRP_0_5_0.version)
				{
					OVRPlugin._version = OVRPlugin.OVRP_0_1_0.version;
				}
				if (OVRPlugin._version > OVRPlugin._versionZero && OVRPlugin._version < OVRPlugin.OVRP_1_3_0.version)
				{
					string[] array = new string[5];
					array[0] = "Oculus Utilities version ";
					int num = 1;
					Version version = OVRPlugin.wrapperVersion;
					array[num] = ((version != null) ? version.ToString() : null);
					array[2] = " is too new for OVRPlugin version ";
					array[3] = OVRPlugin._version.ToString();
					array[4] = ". Update to the latest version of Unity.";
					throw new PlatformNotSupportedException(string.Concat(array));
				}
			}
			return OVRPlugin._version;
		}
	}

	public static Version nativeSDKVersion
	{
		get
		{
			if (OVRPlugin._nativeSDKVersion == null)
			{
				try
				{
					string text = string.Empty;
					if (OVRPlugin.version >= OVRPlugin.OVRP_1_1_0.version)
					{
						text = OVRPlugin.OVRP_1_1_0.ovrp_GetNativeSDKVersion();
					}
					else
					{
						text = OVRPlugin._versionZero.ToString();
					}
					if (text != null)
					{
						text = text.Split('-', StringSplitOptions.None)[0];
						OVRPlugin._nativeSDKVersion = new Version(text);
					}
					else
					{
						OVRPlugin._nativeSDKVersion = OVRPlugin._versionZero;
					}
				}
				catch
				{
					OVRPlugin._nativeSDKVersion = OVRPlugin._versionZero;
				}
			}
			return OVRPlugin._nativeSDKVersion;
		}
	}

	public static bool IsSuccess(this OVRPlugin.Result result)
	{
		return result >= OVRPlugin.Result.Success;
	}

	public static void SetLogCallback2(OVRPlugin.LogCallback2DelegateType logCallback)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_70_0.version && OVRPlugin.OVRP_1_70_0.ovrp_SetLogCallback2(logCallback) != OVRPlugin.Result.Success)
		{
			Debug.LogWarning("OVRPlugin.SetLogCallback2() failed");
		}
	}

	public static bool IsPassthroughShape(OVRPlugin.OverlayShape shape)
	{
		return shape == OVRPlugin.OverlayShape.ReconstructionPassthrough || shape == OVRPlugin.OverlayShape.KeyboardHandsPassthrough || shape == OVRPlugin.OverlayShape.KeyboardMaskedHandsPassthrough || shape == OVRPlugin.OverlayShape.SurfaceProjectedPassthrough;
	}

	public static bool IsPositionValid(this OVRPlugin.SpaceLocationFlags value)
	{
		return (value & OVRPlugin.SpaceLocationFlags.PositionValid) > (OVRPlugin.SpaceLocationFlags)0UL;
	}

	public static bool IsOrientationValid(this OVRPlugin.SpaceLocationFlags value)
	{
		return (value & OVRPlugin.SpaceLocationFlags.OrientationValid) > (OVRPlugin.SpaceLocationFlags)0UL;
	}

	public static bool IsPositionTracked(this OVRPlugin.SpaceLocationFlags value)
	{
		return (value & OVRPlugin.SpaceLocationFlags.PositionTracked) > (OVRPlugin.SpaceLocationFlags)0UL;
	}

	public static bool IsOrientationTracked(this OVRPlugin.SpaceLocationFlags value)
	{
		return (value & OVRPlugin.SpaceLocationFlags.OrientationTracked) > (OVRPlugin.SpaceLocationFlags)0UL;
	}

	public static string GuidToUuidString(Guid guid)
	{
		string text = BitConverter.ToString(guid.ToByteArray()).Replace("-", "").ToLower();
		StringBuilder stringBuilder = new StringBuilder(36);
		for (int i = 0; i < 32; i++)
		{
			stringBuilder.Append(text[i]);
			if (i == 7 || i == 11 || i == 15 || i == 19)
			{
				stringBuilder.Append("-");
			}
		}
		return stringBuilder.ToString();
	}

	public static bool initialized
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetInitialized() == OVRPlugin.Bool.True;
		}
	}

	public static OVRPlugin.XrApi nativeXrApi
	{
		get
		{
			if (OVRPlugin._nativeXrApi == null)
			{
				OVRPlugin._nativeXrApi = new OVRPlugin.XrApi?(OVRPlugin.XrApi.Unknown);
				OVRPlugin.XrApi value;
				if (OVRPlugin.version >= OVRPlugin.OVRP_1_55_0.version && OVRPlugin.OVRP_1_55_0.ovrp_GetNativeXrApiType(out value) == OVRPlugin.Result.Success)
				{
					OVRPlugin._nativeXrApi = new OVRPlugin.XrApi?(value);
				}
			}
			return OVRPlugin._nativeXrApi.Value;
		}
	}

	public static bool chromatic
	{
		get
		{
			return !(OVRPlugin.version >= OVRPlugin.OVRP_1_7_0.version) || (OVRPlugin.initialized && OVRPlugin.OVRP_1_7_0.ovrp_GetAppChromaticCorrection() == OVRPlugin.Bool.True);
		}
		set
		{
			if (OVRPlugin.initialized && OVRPlugin.version >= OVRPlugin.OVRP_1_7_0.version)
			{
				OVRPlugin.OVRP_1_7_0.ovrp_SetAppChromaticCorrection(OVRPlugin.ToBool(value));
			}
		}
	}

	public static bool monoscopic
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetAppMonoscopic() == OVRPlugin.Bool.True;
		}
		set
		{
			if (OVRPlugin.initialized)
			{
				OVRPlugin.OVRP_1_1_0.ovrp_SetAppMonoscopic(OVRPlugin.ToBool(value));
			}
		}
	}

	public static bool rotation
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetTrackingOrientationEnabled() == OVRPlugin.Bool.True;
		}
		set
		{
			if (OVRPlugin.initialized)
			{
				OVRPlugin.OVRP_1_1_0.ovrp_SetTrackingOrientationEnabled(OVRPlugin.ToBool(value));
			}
		}
	}

	public static bool position
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetTrackingPositionEnabled() == OVRPlugin.Bool.True;
		}
		set
		{
			if (OVRPlugin.initialized)
			{
				OVRPlugin.OVRP_1_1_0.ovrp_SetTrackingPositionEnabled(OVRPlugin.ToBool(value));
			}
		}
	}

	public static bool useIPDInPositionTracking
	{
		get
		{
			return !OVRPlugin.initialized || !(OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version) || OVRPlugin.OVRP_1_6_0.ovrp_GetTrackingIPDEnabled() == OVRPlugin.Bool.True;
		}
		set
		{
			if (OVRPlugin.initialized && OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version)
			{
				OVRPlugin.OVRP_1_6_0.ovrp_SetTrackingIPDEnabled(OVRPlugin.ToBool(value));
			}
		}
	}

	public static bool positionSupported
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetTrackingPositionSupported() == OVRPlugin.Bool.True;
		}
	}

	public static bool positionTracked
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetNodePositionTracked(OVRPlugin.Node.EyeCenter) == OVRPlugin.Bool.True;
		}
	}

	public static bool powerSaving
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetSystemPowerSavingMode() == OVRPlugin.Bool.True;
		}
	}

	public static bool hmdPresent
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetNodePresent(OVRPlugin.Node.EyeCenter) == OVRPlugin.Bool.True;
		}
	}

	public static bool userPresent
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_1_0.ovrp_GetUserPresent() == OVRPlugin.Bool.True;
		}
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static bool headphonesPresent
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_3_0.ovrp_GetSystemHeadphonesPresent() == OVRPlugin.Bool.True;
		}
	}

	public static int recommendedMSAALevel
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version)
			{
				return OVRPlugin.OVRP_1_6_0.ovrp_GetSystemRecommendedMSAALevel();
			}
			return 2;
		}
	}

	public static OVRPlugin.SystemRegion systemRegion
	{
		get
		{
			if (OVRPlugin.initialized && OVRPlugin.version >= OVRPlugin.OVRP_1_5_0.version)
			{
				return OVRPlugin.OVRP_1_5_0.ovrp_GetSystemRegion();
			}
			return OVRPlugin.SystemRegion.Unspecified;
		}
	}

	public static string audioOutId
	{
		get
		{
			try
			{
				if (OVRPlugin._nativeAudioOutGuid == null)
				{
					OVRPlugin._nativeAudioOutGuid = new OVRPlugin.GUID();
				}
				IntPtr intPtr = OVRPlugin.OVRP_1_1_0.ovrp_GetAudioOutId();
				if (intPtr != IntPtr.Zero)
				{
					Marshal.PtrToStructure<OVRPlugin.GUID>(intPtr, OVRPlugin._nativeAudioOutGuid);
					Guid guid = new Guid(OVRPlugin._nativeAudioOutGuid.a, OVRPlugin._nativeAudioOutGuid.b, OVRPlugin._nativeAudioOutGuid.c, OVRPlugin._nativeAudioOutGuid.d0, OVRPlugin._nativeAudioOutGuid.d1, OVRPlugin._nativeAudioOutGuid.d2, OVRPlugin._nativeAudioOutGuid.d3, OVRPlugin._nativeAudioOutGuid.d4, OVRPlugin._nativeAudioOutGuid.d5, OVRPlugin._nativeAudioOutGuid.d6, OVRPlugin._nativeAudioOutGuid.d7);
					if (guid != OVRPlugin._cachedAudioOutGuid)
					{
						OVRPlugin._cachedAudioOutGuid = guid;
						OVRPlugin._cachedAudioOutString = OVRPlugin._cachedAudioOutGuid.ToString();
					}
					return OVRPlugin._cachedAudioOutString;
				}
			}
			catch
			{
			}
			return string.Empty;
		}
	}

	public static string audioInId
	{
		get
		{
			try
			{
				if (OVRPlugin._nativeAudioInGuid == null)
				{
					OVRPlugin._nativeAudioInGuid = new OVRPlugin.GUID();
				}
				IntPtr intPtr = OVRPlugin.OVRP_1_1_0.ovrp_GetAudioInId();
				if (intPtr != IntPtr.Zero)
				{
					Marshal.PtrToStructure<OVRPlugin.GUID>(intPtr, OVRPlugin._nativeAudioInGuid);
					Guid guid = new Guid(OVRPlugin._nativeAudioInGuid.a, OVRPlugin._nativeAudioInGuid.b, OVRPlugin._nativeAudioInGuid.c, OVRPlugin._nativeAudioInGuid.d0, OVRPlugin._nativeAudioInGuid.d1, OVRPlugin._nativeAudioInGuid.d2, OVRPlugin._nativeAudioInGuid.d3, OVRPlugin._nativeAudioInGuid.d4, OVRPlugin._nativeAudioInGuid.d5, OVRPlugin._nativeAudioInGuid.d6, OVRPlugin._nativeAudioInGuid.d7);
					if (guid != OVRPlugin._cachedAudioInGuid)
					{
						OVRPlugin._cachedAudioInGuid = guid;
						OVRPlugin._cachedAudioInString = OVRPlugin._cachedAudioInGuid.ToString();
					}
					return OVRPlugin._cachedAudioInString;
				}
			}
			catch
			{
			}
			return string.Empty;
		}
	}

	public static bool hasVrFocus
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetAppHasVrFocus() == OVRPlugin.Bool.True;
		}
	}

	public static bool hasInputFocus
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_18_0.version)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				return OVRPlugin.OVRP_1_18_0.ovrp_GetAppHasInputFocus(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
			}
			return true;
		}
	}

	public static bool shouldQuit
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetAppShouldQuit() == OVRPlugin.Bool.True;
		}
	}

	public static bool shouldRecenter
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetAppShouldRecenter() == OVRPlugin.Bool.True;
		}
	}

	public static string productName
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemProductName();
		}
	}

	public static string latency
	{
		get
		{
			if (!OVRPlugin.initialized)
			{
				return string.Empty;
			}
			return OVRPlugin.OVRP_1_1_0.ovrp_GetAppLatencyTimings();
		}
	}

	public static float eyeDepth
	{
		get
		{
			if (!OVRPlugin.initialized)
			{
				return 0f;
			}
			return OVRPlugin.OVRP_1_1_0.ovrp_GetUserEyeDepth();
		}
		set
		{
			OVRPlugin.OVRP_1_1_0.ovrp_SetUserEyeDepth(value);
		}
	}

	public static float eyeHeight
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetUserEyeHeight();
		}
		set
		{
			OVRPlugin.OVRP_1_1_0.ovrp_SetUserEyeHeight(value);
		}
	}

	[Obsolete("Deprecated. Please use SystemInfo.batteryLevel", false)]
	public static float batteryLevel
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemBatteryLevel();
		}
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static float batteryTemperature
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemBatteryTemperature();
		}
	}

	private static PerformanceLevelHint ProcessorPerformanceLevelToPerformanceLevelHint(OVRPlugin.ProcessorPerformanceLevel level)
	{
		switch (level)
		{
		case OVRPlugin.ProcessorPerformanceLevel.PowerSavings:
			return PerformanceLevelHint.PowerSavings;
		case OVRPlugin.ProcessorPerformanceLevel.SustainedLow:
			return PerformanceLevelHint.SustainedLow;
		case OVRPlugin.ProcessorPerformanceLevel.SustainedHigh:
			return PerformanceLevelHint.SustainedHigh;
		case OVRPlugin.ProcessorPerformanceLevel.Boost:
			return PerformanceLevelHint.Boost;
		default:
			return PerformanceLevelHint.SustainedHigh;
		}
	}

	public static OVRPlugin.ProcessorPerformanceLevel suggestedCpuPerfLevel
	{
		get
		{
			return OVRPlugin.m_suggestedCpuPerfLevelOpenXR;
		}
		set
		{
			OVRPlugin.m_suggestedCpuPerfLevelOpenXR = value;
			XrPerformanceSettingsFeature.SetPerformanceLevelHint(PerformanceDomain.Cpu, OVRPlugin.ProcessorPerformanceLevelToPerformanceLevelHint(value));
		}
	}

	public static OVRPlugin.ProcessorPerformanceLevel suggestedGpuPerfLevel
	{
		get
		{
			return OVRPlugin.m_suggestedGpuPerfLevelOpenXR;
		}
		set
		{
			OVRPlugin.m_suggestedGpuPerfLevelOpenXR = value;
			XrPerformanceSettingsFeature.SetPerformanceLevelHint(PerformanceDomain.Gpu, OVRPlugin.ProcessorPerformanceLevelToPerformanceLevelHint(value));
		}
	}

	[Obsolete("Deprecated. Please use suggestedCpuPerfLevel.", false)]
	public static int cpuLevel
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemCpuLevel();
		}
		set
		{
			OVRPlugin.OVRP_1_1_0.ovrp_SetSystemCpuLevel(value);
		}
	}

	[Obsolete("Deprecated. Please use suggestedGpuPerfLevel.", false)]
	public static int gpuLevel
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemGpuLevel();
		}
		set
		{
			OVRPlugin.OVRP_1_1_0.ovrp_SetSystemGpuLevel(value);
		}
	}

	public static int vsyncCount
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemVSyncCount();
		}
		set
		{
			OVRPlugin.OVRP_1_2_0.ovrp_SetSystemVSyncCount(value);
		}
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static float systemVolume
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemVolume();
		}
	}

	public static float ipd
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetUserIPD();
		}
		set
		{
			OVRPlugin.OVRP_1_1_0.ovrp_SetUserIPD(value);
		}
	}

	public static bool occlusionMesh
	{
		get
		{
			return OVRPlugin.initialized && OVRPlugin.OVRP_1_3_0.ovrp_GetEyeOcclusionMeshEnabled() == OVRPlugin.Bool.True;
		}
		set
		{
			if (!OVRPlugin.initialized)
			{
				return;
			}
			OVRPlugin.OVRP_1_3_0.ovrp_SetEyeOcclusionMeshEnabled(OVRPlugin.ToBool(value));
		}
	}

	public static bool premultipliedAlphaLayersSupported
	{
		get
		{
			return !Application.isMobilePlatform && OVRPlugin.version >= OVRPlugin.OVRP_1_3_0.version;
		}
	}

	public static bool unpremultipliedAlphaLayersSupported
	{
		get
		{
			return Application.isMobilePlatform;
		}
	}

	[Obsolete("Deprecated. Please use SystemInfo.batteryStatus", false)]
	public static OVRPlugin.BatteryStatus batteryStatus
	{
		get
		{
			return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemBatteryStatus();
		}
	}

	public static OVRPlugin.Frustumf GetEyeFrustum(OVRPlugin.Eye eyeId)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_GetNodeFrustum((OVRPlugin.Node)eyeId);
	}

	public static OVRPlugin.Sizei GetEyeTextureSize(OVRPlugin.Eye eyeId)
	{
		return OVRPlugin.OVRP_0_1_0.ovrp_GetEyeTextureSize(eyeId);
	}

	public static OVRPlugin.Posef GetTrackerPose(OVRPlugin.Tracker trackerId)
	{
		return OVRPlugin.GetNodePose((OVRPlugin.Node)(trackerId + 5), OVRPlugin.Step.Render);
	}

	public static OVRPlugin.Frustumf GetTrackerFrustum(OVRPlugin.Tracker trackerId)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_GetNodeFrustum((OVRPlugin.Node)(trackerId + 5));
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static bool ShowUI(OVRPlugin.PlatformUI ui)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_ShowSystemUI(ui) == OVRPlugin.Bool.True;
	}

	public static bool EnqueueSubmitLayer(bool onTop, bool headLocked, bool noDepthBufferTesting, IntPtr leftTexture, IntPtr rightTexture, int layerId, int frameIndex, OVRPlugin.Posef pose, OVRPlugin.Vector3f scale, int layerIndex = 0, OVRPlugin.OverlayShape shape = OVRPlugin.OverlayShape.Quad, bool overrideTextureRectMatrix = false, OVRPlugin.TextureRectMatrixf textureRectMatrix = default(OVRPlugin.TextureRectMatrixf), bool overridePerLayerColorScaleAndOffset = false, Vector4 colorScale = default(Vector4), Vector4 colorOffset = default(Vector4), bool expensiveSuperSample = false, bool bicubic = false, bool efficientSuperSample = false, bool efficientSharpen = false, bool expensiveSharpen = false, bool hidden = false, bool secureContent = false, bool automaticFiltering = false, bool premultipledAlpha = false)
	{
		if (!OVRPlugin.initialized)
		{
			return false;
		}
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version))
		{
			return layerIndex == 0 && OVRPlugin.OVRP_0_1_1.ovrp_SetOverlayQuad2(OVRPlugin.ToBool(onTop), OVRPlugin.ToBool(headLocked), leftTexture, IntPtr.Zero, pose, scale) == OVRPlugin.Bool.True;
		}
		uint num = 0U;
		if (onTop)
		{
			num |= 1U;
		}
		if (headLocked)
		{
			num |= 2U;
		}
		if (noDepthBufferTesting)
		{
			num |= 4U;
		}
		if (expensiveSuperSample)
		{
			num |= 8U;
		}
		if (hidden)
		{
			num |= 512U;
		}
		if (efficientSuperSample)
		{
			num |= 16U;
		}
		if (expensiveSharpen)
		{
			num |= 128U;
		}
		if (efficientSharpen)
		{
			num |= 32U;
		}
		if (bicubic)
		{
			num |= 64U;
		}
		if (secureContent)
		{
			num |= 256U;
		}
		if (automaticFiltering)
		{
			num |= 1024U;
		}
		if (premultipledAlpha)
		{
			num |= 1048576U;
		}
		if (shape == OVRPlugin.OverlayShape.Cylinder || shape == OVRPlugin.OverlayShape.Cubemap)
		{
			if (shape == OVRPlugin.OverlayShape.Cubemap && OVRPlugin.version < OVRPlugin.OVRP_1_10_0.version)
			{
				return false;
			}
			if (shape == OVRPlugin.OverlayShape.Cylinder && OVRPlugin.version < OVRPlugin.OVRP_1_16_0.version)
			{
				return false;
			}
		}
		if (shape == OVRPlugin.OverlayShape.OffcenterCubemap)
		{
			return false;
		}
		if (shape == OVRPlugin.OverlayShape.Equirect)
		{
			return false;
		}
		if (shape == OVRPlugin.OverlayShape.Fisheye)
		{
			return false;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_34_0.version && layerId != -1)
		{
			return OVRPlugin.OVRP_1_34_0.ovrp_EnqueueSubmitLayer2(num, leftTexture, rightTexture, layerId, frameIndex, ref pose, ref scale, layerIndex, overrideTextureRectMatrix ? OVRPlugin.Bool.True : OVRPlugin.Bool.False, ref textureRectMatrix, overridePerLayerColorScaleAndOffset ? OVRPlugin.Bool.True : OVRPlugin.Bool.False, ref colorScale, ref colorOffset) == OVRPlugin.Result.Success;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && layerId != -1)
		{
			return OVRPlugin.OVRP_1_15_0.ovrp_EnqueueSubmitLayer(num, leftTexture, rightTexture, layerId, frameIndex, ref pose, ref scale, layerIndex) == OVRPlugin.Result.Success;
		}
		return OVRPlugin.OVRP_1_6_0.ovrp_SetOverlayQuad3(num, leftTexture, rightTexture, IntPtr.Zero, pose, scale, layerIndex) == OVRPlugin.Bool.True;
	}

	public static OVRPlugin.LayerDesc CalculateLayerDesc(OVRPlugin.OverlayShape shape, OVRPlugin.LayerLayout layout, OVRPlugin.Sizei textureSize, int mipLevels, int sampleCount, OVRPlugin.EyeTextureFormat format, int layerFlags)
	{
		if (!OVRPlugin.initialized || OVRPlugin.version < OVRPlugin.OVRP_1_15_0.version)
		{
			return default(OVRPlugin.LayerDesc);
		}
		OVRPlugin.LayerDesc result = default(OVRPlugin.LayerDesc);
		OVRPlugin.OVRP_1_15_0.ovrp_CalculateLayerDesc(shape, layout, ref textureSize, mipLevels, sampleCount, format, layerFlags, ref result);
		return result;
	}

	public static bool EnqueueSetupLayer(OVRPlugin.LayerDesc desc, int compositionDepth, IntPtr layerID)
	{
		if (!OVRPlugin.initialized)
		{
			return false;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_28_0.version)
		{
			return OVRPlugin.OVRP_1_28_0.ovrp_EnqueueSetupLayer2(ref desc, compositionDepth, layerID) == OVRPlugin.Result.Success;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version)
		{
			if (compositionDepth != 0)
			{
				Debug.LogWarning("Use Oculus Plugin 1.28.0 or above to support non-zero compositionDepth");
			}
			return OVRPlugin.OVRP_1_15_0.ovrp_EnqueueSetupLayer(ref desc, layerID) == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static bool EnqueueDestroyLayer(IntPtr layerID)
	{
		return OVRPlugin.initialized && OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_EnqueueDestroyLayer(layerID) == OVRPlugin.Result.Success;
	}

	public static IntPtr GetLayerTexture(int layerId, int stage, OVRPlugin.Eye eyeId)
	{
		IntPtr zero = IntPtr.Zero;
		if (!OVRPlugin.initialized)
		{
			return zero;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version)
		{
			OVRPlugin.OVRP_1_15_0.ovrp_GetLayerTexturePtr(layerId, stage, eyeId, ref zero);
		}
		return zero;
	}

	public static int GetLayerTextureStageCount(int layerId)
	{
		if (!OVRPlugin.initialized)
		{
			return 1;
		}
		int result = 1;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version)
		{
			OVRPlugin.OVRP_1_15_0.ovrp_GetLayerTextureStageCount(layerId, ref result);
		}
		return result;
	}

	public static IntPtr GetLayerAndroidSurfaceObject(int layerId)
	{
		IntPtr zero = IntPtr.Zero;
		if (!OVRPlugin.initialized)
		{
			return zero;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_29_0.version)
		{
			OVRPlugin.OVRP_1_29_0.ovrp_GetLayerAndroidSurfaceObject(layerId, ref zero);
		}
		return zero;
	}

	public static bool UpdateNodePhysicsPoses(int frameIndex, double predictionSeconds)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && OVRPlugin.OVRP_1_8_0.ovrp_Update2(0, frameIndex, predictionSeconds) == OVRPlugin.Bool.True;
	}

	public static OVRPlugin.Posef GetNodePose(OVRPlugin.Node nodeId, OVRPlugin.Step stepId)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).Pose;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && stepId == OVRPlugin.Step.Physics)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_GetNodePose2(0, nodeId);
		}
		return OVRPlugin.OVRP_0_1_2.ovrp_GetNodePose(nodeId);
	}

	public static OVRPlugin.Vector3f GetNodeVelocity(OVRPlugin.Node nodeId, OVRPlugin.Step stepId)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).Velocity;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && stepId == OVRPlugin.Step.Physics)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_GetNodeVelocity2(0, nodeId).Position;
		}
		return OVRPlugin.OVRP_0_1_3.ovrp_GetNodeVelocity(nodeId).Position;
	}

	public static OVRPlugin.Vector3f GetNodeAngularVelocity(OVRPlugin.Node nodeId, OVRPlugin.Step stepId)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).AngularVelocity;
		}
		return default(OVRPlugin.Vector3f);
	}

	[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
	public static OVRPlugin.Vector3f GetNodeAcceleration(OVRPlugin.Node nodeId, OVRPlugin.Step stepId)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).Acceleration;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && stepId == OVRPlugin.Step.Physics)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_GetNodeAcceleration2(0, nodeId).Position;
		}
		return OVRPlugin.OVRP_0_1_3.ovrp_GetNodeAcceleration(nodeId).Position;
	}

	[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
	public static OVRPlugin.Vector3f GetNodeAngularAcceleration(OVRPlugin.Node nodeId, OVRPlugin.Step stepId)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).AngularAcceleration;
		}
		return default(OVRPlugin.Vector3f);
	}

	public static bool GetNodePresent(OVRPlugin.Node nodeId)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_GetNodePresent(nodeId) == OVRPlugin.Bool.True;
	}

	public static bool GetNodeOrientationTracked(OVRPlugin.Node nodeId)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_GetNodeOrientationTracked(nodeId) == OVRPlugin.Bool.True;
	}

	public static bool GetNodeOrientationValid(OVRPlugin.Node nodeId)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_38_0.ovrp_GetNodeOrientationValid(nodeId, ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return OVRPlugin.GetNodeOrientationTracked(nodeId);
	}

	public static bool GetNodePositionTracked(OVRPlugin.Node nodeId)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_GetNodePositionTracked(nodeId) == OVRPlugin.Bool.True;
	}

	public static bool GetNodePositionValid(OVRPlugin.Node nodeId)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_38_0.ovrp_GetNodePositionValid(nodeId, ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return OVRPlugin.GetNodePositionTracked(nodeId);
	}

	public static OVRPlugin.PoseStatef GetNodePoseStateRaw(OVRPlugin.Node nodeId, OVRPlugin.Step stepId)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_29_0.version)
		{
			OVRPlugin.PoseStatef result;
			if (OVRPlugin.OVRP_1_29_0.ovrp_GetNodePoseStateRaw(stepId, -1, nodeId, out result) == OVRPlugin.Result.Success)
			{
				return result;
			}
			return OVRPlugin.PoseStatef.identity;
		}
		else
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
			{
				return OVRPlugin.OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId);
			}
			return OVRPlugin.PoseStatef.identity;
		}
	}

	public static OVRPlugin.PoseStatef GetNodePoseStateAtTime(double time, OVRPlugin.Node nodeId)
	{
		OVRPlugin.PoseStatef result;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_76_0.version && OVRPlugin.OVRP_1_76_0.ovrp_GetNodePoseStateAtTime(time, nodeId, out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return OVRPlugin.PoseStatef.identity;
	}

	public static OVRPlugin.PoseStatef GetNodePoseStateImmediate(OVRPlugin.Node nodeId)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_69_0.version))
		{
			return OVRPlugin.PoseStatef.identity;
		}
		OVRPlugin.PoseStatef result;
		if (OVRPlugin.OVRP_1_69_0.ovrp_GetNodePoseStateImmediate(nodeId, out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return OVRPlugin.PoseStatef.identity;
	}

	public static bool AreHandPosesGeneratedByControllerData(OVRPlugin.Step stepId, OVRPlugin.Node nodeId)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_86_0.ovrp_AreHandPosesGeneratedByControllerData(stepId, nodeId, ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static bool SetSimultaneousHandsAndControllersEnabled(bool enabled)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_88_0.version && OVRPlugin.OVRP_1_88_0.ovrp_SetSimultaneousHandsAndControllersEnabled(enabled ? OVRPlugin.Bool.True : OVRPlugin.Bool.False) == OVRPlugin.Result.Success;
	}

	public static bool GetControllerIsInHand(OVRPlugin.Step stepId, OVRPlugin.Node nodeId)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.True;
			return OVRPlugin.OVRP_1_86_0.ovrp_GetControllerIsInHand(stepId, nodeId, ref @bool) != OVRPlugin.Result.Success || @bool != OVRPlugin.Bool.False;
		}
		return true;
	}

	public static OVRPlugin.Posef GetCurrentTrackingTransformPose()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_30_0.version))
		{
			return OVRPlugin.Posef.identity;
		}
		OVRPlugin.Posef result;
		if (OVRPlugin.OVRP_1_30_0.ovrp_GetCurrentTrackingTransformPose(out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return OVRPlugin.Posef.identity;
	}

	public static OVRPlugin.Posef GetTrackingTransformRawPose()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_30_0.version))
		{
			return OVRPlugin.Posef.identity;
		}
		OVRPlugin.Posef result;
		if (OVRPlugin.OVRP_1_30_0.ovrp_GetTrackingTransformRawPose(out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return OVRPlugin.Posef.identity;
	}

	public static OVRPlugin.Posef GetTrackingTransformRelativePose(OVRPlugin.TrackingOrigin trackingOrigin)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version))
		{
			return OVRPlugin.Posef.identity;
		}
		OVRPlugin.Posef identity = OVRPlugin.Posef.identity;
		if (OVRPlugin.OVRP_1_38_0.ovrp_GetTrackingTransformRelativePose(ref identity, trackingOrigin) == OVRPlugin.Result.Success)
		{
			return identity;
		}
		return OVRPlugin.Posef.identity;
	}

	public static OVRPlugin.ControllerState GetControllerState(uint controllerMask)
	{
		return OVRPlugin.OVRP_1_1_0.ovrp_GetControllerState(controllerMask);
	}

	public static OVRPlugin.ControllerState2 GetControllerState2(uint controllerMask)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetControllerState2(controllerMask);
		}
		return new OVRPlugin.ControllerState2(OVRPlugin.OVRP_1_1_0.ovrp_GetControllerState(controllerMask));
	}

	public static OVRPlugin.ControllerState4 GetControllerState4(uint controllerMask)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version)
		{
			OVRPlugin.ControllerState4 result = default(OVRPlugin.ControllerState4);
			OVRPlugin.OVRP_1_16_0.ovrp_GetControllerState4(controllerMask, ref result);
			return result;
		}
		return new OVRPlugin.ControllerState4(OVRPlugin.GetControllerState2(controllerMask));
	}

	public static OVRPlugin.ControllerState5 GetControllerState5(uint controllerMask)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version)
		{
			OVRPlugin.ControllerState5 result = default(OVRPlugin.ControllerState5);
			OVRPlugin.OVRP_1_78_0.ovrp_GetControllerState5(controllerMask, ref result);
			return result;
		}
		return new OVRPlugin.ControllerState5(OVRPlugin.GetControllerState4(controllerMask));
	}

	public static OVRPlugin.ControllerState6 GetControllerState6(uint controllerMask)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_83_0.version)
		{
			OVRPlugin.ControllerState6 result = default(OVRPlugin.ControllerState6);
			OVRPlugin.OVRP_1_83_0.ovrp_GetControllerState6(controllerMask, ref result);
			return result;
		}
		return new OVRPlugin.ControllerState6(OVRPlugin.GetControllerState5(controllerMask));
	}

	public static OVRPlugin.InteractionProfile GetCurrentInteractionProfile(OVRPlugin.Hand hand)
	{
		OVRPlugin.InteractionProfile result = OVRPlugin.InteractionProfile.None;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version)
		{
			OVRPlugin.OVRP_1_78_0.ovrp_GetCurrentInteractionProfile(hand, out result);
		}
		return result;
	}

	public static OVRPlugin.InteractionProfile GetCurrentDetachedInteractionProfile(OVRPlugin.Hand hand)
	{
		OVRPlugin.InteractionProfile result = OVRPlugin.InteractionProfile.None;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version)
		{
			OVRPlugin.OVRP_1_86_0.ovrp_GetCurrentDetachedInteractionProfile(hand, out result);
		}
		return result;
	}

	public static string GetCurrentInteractionProfileName(OVRPlugin.Hand hand)
	{
		string result = string.Empty;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_100_0.version)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.AllocHGlobal(256);
				if (OVRPlugin.OVRP_1_100_0.ovrp_GetCurrentInteractionProfileName(hand, intPtr) == OVRPlugin.Result.Success)
				{
					result = Marshal.PtrToStringAnsi(intPtr);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		return result;
	}

	public static bool SetControllerVibration(uint controllerMask, float frequency, float amplitude)
	{
		return OVRPlugin.OVRP_0_1_2.ovrp_SetControllerVibration(controllerMask, frequency, amplitude) == OVRPlugin.Bool.True;
	}

	public static bool SetControllerLocalizedVibration(OVRPlugin.Controller controllerMask, OVRPlugin.HapticsLocation hapticsLocationMask, float frequency, float amplitude)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_SetControllerLocalizedVibration(controllerMask, hapticsLocationMask, frequency, amplitude) == OVRPlugin.Result.Success;
	}

	public static bool SetControllerHapticsAmplitudeEnvelope(OVRPlugin.Controller controllerMask, OVRPlugin.HapticsAmplitudeEnvelopeVibration hapticsVibration)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_SetControllerHapticsAmplitudeEnvelope(controllerMask, hapticsVibration) == OVRPlugin.Result.Success;
	}

	public static bool SetControllerHapticsPcm(OVRPlugin.Controller controllerMask, OVRPlugin.HapticsPcmVibration hapticsVibration)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_SetControllerHapticsPcm(controllerMask, hapticsVibration) == OVRPlugin.Result.Success;
	}

	public static bool GetControllerSampleRateHz(OVRPlugin.Controller controllerMask, out float sampleRateHz)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version)
		{
			return OVRPlugin.OVRP_1_78_0.ovrp_GetControllerSampleRateHz(controllerMask, out sampleRateHz) == OVRPlugin.Result.Success;
		}
		sampleRateHz = 0f;
		return false;
	}

	public static OVRPlugin.HapticsDesc GetControllerHapticsDesc(uint controllerMask)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version)
		{
			return OVRPlugin.OVRP_1_6_0.ovrp_GetControllerHapticsDesc(controllerMask);
		}
		return default(OVRPlugin.HapticsDesc);
	}

	public static OVRPlugin.HapticsState GetControllerHapticsState(uint controllerMask)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version)
		{
			return OVRPlugin.OVRP_1_6_0.ovrp_GetControllerHapticsState(controllerMask);
		}
		return default(OVRPlugin.HapticsState);
	}

	public static bool SetControllerHaptics(uint controllerMask, OVRPlugin.HapticsBuffer hapticsBuffer)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version && OVRPlugin.OVRP_1_6_0.ovrp_SetControllerHaptics(controllerMask, hapticsBuffer) == OVRPlugin.Bool.True;
	}

	public static float GetEyeRecommendedResolutionScale()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version)
		{
			return OVRPlugin.OVRP_1_6_0.ovrp_GetEyeRecommendedResolutionScale();
		}
		return 1f;
	}

	public static float GetAppCpuStartToGpuEndTime()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_6_0.version)
		{
			return OVRPlugin.OVRP_1_6_0.ovrp_GetAppCpuStartToGpuEndTime();
		}
		return 0f;
	}

	public static bool GetBoundaryConfigured()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && OVRPlugin.OVRP_1_8_0.ovrp_GetBoundaryConfigured() == OVRPlugin.Bool.True;
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static OVRPlugin.BoundaryTestResult TestBoundaryNode(OVRPlugin.Node nodeId, OVRPlugin.BoundaryType boundaryType)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_TestBoundaryNode(nodeId, boundaryType);
		}
		return default(OVRPlugin.BoundaryTestResult);
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static OVRPlugin.BoundaryTestResult TestBoundaryPoint(OVRPlugin.Vector3f point, OVRPlugin.BoundaryType boundaryType)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_TestBoundaryPoint(point, boundaryType);
		}
		return default(OVRPlugin.BoundaryTestResult);
	}

	public static OVRPlugin.BoundaryGeometry GetBoundaryGeometry(OVRPlugin.BoundaryType boundaryType)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_GetBoundaryGeometry(boundaryType);
		}
		return default(OVRPlugin.BoundaryGeometry);
	}

	public static bool GetBoundaryGeometry2(OVRPlugin.BoundaryType boundaryType, IntPtr points, ref int pointsCount)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_9_0.version)
		{
			return OVRPlugin.OVRP_1_9_0.ovrp_GetBoundaryGeometry2(boundaryType, points, ref pointsCount) == OVRPlugin.Bool.True;
		}
		pointsCount = 0;
		return false;
	}

	public static OVRPlugin.AppPerfStats GetAppPerfStats()
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR)
		{
			if (!OVRPlugin.perfStatWarningPrinted)
			{
				Debug.LogWarning("GetAppPerfStats is currently unsupported on OpenXR.");
				OVRPlugin.perfStatWarningPrinted = true;
			}
			return default(OVRPlugin.AppPerfStats);
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_9_0.version)
		{
			return OVRPlugin.OVRP_1_9_0.ovrp_GetAppPerfStats();
		}
		return default(OVRPlugin.AppPerfStats);
	}

	public static bool ResetAppPerfStats()
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR)
		{
			if (!OVRPlugin.resetPerfStatWarningPrinted)
			{
				Debug.LogWarning("ResetAppPerfStats is currently unsupported on OpenXR.");
				OVRPlugin.resetPerfStatWarningPrinted = true;
			}
			return false;
		}
		return OVRPlugin.version >= OVRPlugin.OVRP_1_9_0.version && OVRPlugin.OVRP_1_9_0.ovrp_ResetAppPerfStats() == OVRPlugin.Bool.True;
	}

	public static float GetAppFramerate()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_12_0.version)
		{
			return OVRPlugin.OVRP_1_12_0.ovrp_GetAppFramerate();
		}
		return 0f;
	}

	public static bool SetHandNodePoseStateLatency(double latencyInSeconds)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_18_0.version && OVRPlugin.OVRP_1_18_0.ovrp_SetHandNodePoseStateLatency(latencyInSeconds) == OVRPlugin.Result.Success;
	}

	public static double GetHandNodePoseStateLatency()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_18_0.version))
		{
			return 0.0;
		}
		double result = 0.0;
		if (OVRPlugin.OVRP_1_18_0.ovrp_GetHandNodePoseStateLatency(out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0.0;
	}

	public static bool SetControllerDrivenHandPoses(bool controllerDrivenHandPoses)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version && OVRPlugin.OVRP_1_86_0.ovrp_SetControllerDrivenHandPoses(controllerDrivenHandPoses ? OVRPlugin.Bool.True : OVRPlugin.Bool.False) == OVRPlugin.Result.Success;
	}

	public static bool SetControllerDrivenHandPosesAreNatural(bool controllerDrivenHandPosesAreNatural)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_87_0.version && OVRPlugin.OVRP_1_87_0.ovrp_SetControllerDrivenHandPosesAreNatural(controllerDrivenHandPosesAreNatural ? OVRPlugin.Bool.True : OVRPlugin.Bool.False) == OVRPlugin.Result.Success;
	}

	public static bool IsControllerDrivenHandPosesEnabled()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_86_0.ovrp_IsControllerDrivenHandPosesEnabled(ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static bool AreControllerDrivenHandPosesNatural()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_87_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_87_0.ovrp_AreControllerDrivenHandPosesNatural(ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static bool SetHandSkeletonVersion(OVRHandSkeletonVersion skeletonVersion)
	{
		if (skeletonVersion == OVRHandSkeletonVersion.Uninitialized)
		{
			OVRPlugin.HandSkeletonVersion = skeletonVersion;
			return false;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			OVRPlugin.Result result = OVRPlugin.OVRP_1_103_0.ovrp_SetHandSkeletonVersion(skeletonVersion);
			if (result == OVRPlugin.Result.Success)
			{
				OVRPlugin.HandSkeletonVersion = skeletonVersion;
			}
			return result == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static bool GetActionStateBoolean(string actionName, out bool result)
	{
		OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
		result = false;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_95_0.version))
		{
			return false;
		}
		OVRPlugin.Result result2 = OVRPlugin.OVRP_1_95_0.ovrp_GetActionStateBoolean(actionName, ref @bool);
		if (result2 == OVRPlugin.Result.Success)
		{
			result = (@bool == OVRPlugin.Bool.True);
			return true;
		}
		Debug.LogError(string.Format("Error calling GetActionStateBoolean: {0}", result2));
		return false;
	}

	public static bool GetActionStateFloat(string actionName, out float result)
	{
		result = 0f;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_95_0.version))
		{
			return false;
		}
		OVRPlugin.Result result2 = OVRPlugin.OVRP_1_95_0.ovrp_GetActionStateFloat(actionName, ref result);
		if (result2 == OVRPlugin.Result.Success)
		{
			return true;
		}
		Debug.LogError(string.Format("Error calling GetActionStateFloat: {0}", result2));
		return false;
	}

	public static bool GetActionStatePose(string actionName, out OVRPlugin.Posef result)
	{
		result = default(OVRPlugin.Posef);
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_95_0.version))
		{
			return false;
		}
		OVRPlugin.Result result2 = OVRPlugin.OVRP_1_95_0.ovrp_GetActionStatePose(actionName, ref result);
		if (result2 == OVRPlugin.Result.Success)
		{
			return true;
		}
		Debug.LogError(string.Format("Error calling GetActionStatePose: {0}", result2));
		return false;
	}

	public static bool GetActionStatePose(string actionName, OVRPlugin.Hand hand, out OVRPlugin.Posef result)
	{
		result = default(OVRPlugin.Posef);
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_100_0.version))
		{
			return false;
		}
		OVRPlugin.Result result2 = OVRPlugin.OVRP_1_100_0.ovrp_GetActionStatePose2(actionName, hand, ref result);
		if (result2 == OVRPlugin.Result.Success)
		{
			return true;
		}
		Debug.LogError(string.Format("Error calling GetActionStatePose2: {0}", result2));
		return false;
	}

	public static bool TriggerVibrationAction(string actionName, OVRPlugin.Hand hand, float duration, float amplitude)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_100_0.version))
		{
			return false;
		}
		OVRPlugin.Result result = OVRPlugin.OVRP_1_100_0.ovrp_TriggerVibrationAction(actionName, hand, duration, amplitude);
		if (result == OVRPlugin.Result.Success)
		{
			return true;
		}
		Debug.LogError(string.Format("Error calling TriggerVibrationAction: {0}", result));
		return false;
	}

	public static bool SetWideMotionModeHandPoses(bool wideMotionModeFusionHandPoses)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_93_0.version && OVRPlugin.OVRP_1_93_0.ovrp_SetWideMotionModeHandPoses(wideMotionModeFusionHandPoses ? OVRPlugin.Bool.True : OVRPlugin.Bool.False) == OVRPlugin.Result.Success;
	}

	public static bool IsWideMotionModeHandPosesEnabled()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_93_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_93_0.ovrp_IsWideMotionModeHandPosesEnabled(ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static OVRPlugin.EyeTextureFormat GetDesiredEyeTextureFormat()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_11_0.version)
		{
			uint num = (uint)OVRPlugin.OVRP_1_11_0.ovrp_GetDesiredEyeTextureFormat();
			if (num == 1U)
			{
				num = 0U;
			}
			return (OVRPlugin.EyeTextureFormat)num;
		}
		return OVRPlugin.EyeTextureFormat.Default;
	}

	public static bool SetDesiredEyeTextureFormat(OVRPlugin.EyeTextureFormat value)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_11_0.version && OVRPlugin.OVRP_1_11_0.ovrp_SetDesiredEyeTextureFormat(value) == OVRPlugin.Bool.True;
	}

	public static bool InitializeMixedReality()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_InitializeMixedReality() == OVRPlugin.Result.Success;
	}

	public static bool ShutdownMixedReality()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_ShutdownMixedReality() == OVRPlugin.Result.Success;
	}

	public static bool IsMixedRealityInitialized()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_GetMixedRealityInitialized() == OVRPlugin.Bool.True;
	}

	public static int GetExternalCameraCount()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version))
		{
			return 0;
		}
		int result = 0;
		if (OVRPlugin.OVRP_1_15_0.ovrp_GetExternalCameraCount(out result) != OVRPlugin.Result.Success)
		{
			return 0;
		}
		return result;
	}

	public static bool UpdateExternalCamera()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_UpdateExternalCamera() == OVRPlugin.Result.Success;
	}

	public static bool GetMixedRealityCameraInfo(int cameraId, out OVRPlugin.CameraExtrinsics cameraExtrinsics, out OVRPlugin.CameraIntrinsics cameraIntrinsics)
	{
		cameraExtrinsics = default(OVRPlugin.CameraExtrinsics);
		cameraIntrinsics = default(OVRPlugin.CameraIntrinsics);
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version)
		{
			bool result = true;
			if (OVRPlugin.OVRP_1_15_0.ovrp_GetExternalCameraExtrinsics(cameraId, out cameraExtrinsics) != OVRPlugin.Result.Success)
			{
				result = false;
			}
			if (OVRPlugin.OVRP_1_15_0.ovrp_GetExternalCameraIntrinsics(cameraId, out cameraIntrinsics) != OVRPlugin.Result.Success)
			{
				result = false;
			}
			return result;
		}
		return false;
	}

	public static bool OverrideExternalCameraFov(int cameraId, bool useOverriddenFov, OVRPlugin.Fovf fov)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			bool result = true;
			if (OVRPlugin.OVRP_1_44_0.ovrp_OverrideExternalCameraFov(cameraId, useOverriddenFov ? OVRPlugin.Bool.True : OVRPlugin.Bool.False, ref fov) != OVRPlugin.Result.Success)
			{
				result = false;
			}
			return result;
		}
		return false;
	}

	public static bool GetUseOverriddenExternalCameraFov(int cameraId)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			bool result = true;
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			if (OVRPlugin.OVRP_1_44_0.ovrp_GetUseOverriddenExternalCameraFov(cameraId, out @bool) != OVRPlugin.Result.Success)
			{
				result = false;
			}
			if (@bool == OVRPlugin.Bool.False)
			{
				result = false;
			}
			return result;
		}
		return false;
	}

	public static bool OverrideExternalCameraStaticPose(int cameraId, bool useOverriddenPose, OVRPlugin.Posef poseInStageOrigin)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			bool result = true;
			if (OVRPlugin.OVRP_1_44_0.ovrp_OverrideExternalCameraStaticPose(cameraId, useOverriddenPose ? OVRPlugin.Bool.True : OVRPlugin.Bool.False, ref poseInStageOrigin) != OVRPlugin.Result.Success)
			{
				result = false;
			}
			return result;
		}
		return false;
	}

	public static bool GetUseOverriddenExternalCameraStaticPose(int cameraId)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			bool result = true;
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			if (OVRPlugin.OVRP_1_44_0.ovrp_GetUseOverriddenExternalCameraStaticPose(cameraId, out @bool) != OVRPlugin.Result.Success)
			{
				result = false;
			}
			if (@bool == OVRPlugin.Bool.False)
			{
				result = false;
			}
			return result;
		}
		return false;
	}

	public static bool ResetDefaultExternalCamera()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version && OVRPlugin.OVRP_1_44_0.ovrp_ResetDefaultExternalCamera() == OVRPlugin.Result.Success;
	}

	public static bool SetDefaultExternalCamera(string cameraName, ref OVRPlugin.CameraIntrinsics cameraIntrinsics, ref OVRPlugin.CameraExtrinsics cameraExtrinsics)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version && OVRPlugin.OVRP_1_44_0.ovrp_SetDefaultExternalCamera(cameraName, ref cameraIntrinsics, ref cameraExtrinsics) == OVRPlugin.Result.Success;
	}

	public static bool SetExternalCameraProperties(string cameraName, ref OVRPlugin.CameraIntrinsics cameraIntrinsics, ref OVRPlugin.CameraExtrinsics cameraExtrinsics)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_48_0.version && OVRPlugin.OVRP_1_48_0.ovrp_SetExternalCameraProperties(cameraName, ref cameraIntrinsics, ref cameraExtrinsics) == OVRPlugin.Result.Success;
	}

	public static bool SetMultimodalHandsControllersSupported(bool value)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version && OVRPlugin.OVRP_1_86_0.ovrp_SetMultimodalHandsControllersSupported(OVRPlugin.ToBool(value)) == OVRPlugin.Result.Success;
	}

	public static bool IsMultimodalHandsControllersSupported()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_86_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_86_0.ovrp_IsMultimodalHandsControllersSupported(ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static bool IsInsightPassthroughSupported()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version))
		{
			return false;
		}
		OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
		OVRPlugin.Result result = OVRPlugin.OVRP_1_71_0.ovrp_IsInsightPassthroughSupported(ref @bool);
		if (result == OVRPlugin.Result.Success)
		{
			return @bool == OVRPlugin.Bool.True;
		}
		Debug.LogError("Unable to determine whether passthrough is supported. Try calling IsInsightPassthroughSupported() while the XR plug-in is initialized. Failed with reason: " + result.ToString());
		return false;
	}

	public static bool InitializeInsightPassthrough()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_InitializeInsightPassthrough() == OVRPlugin.Result.Success;
	}

	public static bool ShutdownInsightPassthrough()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_ShutdownInsightPassthrough() == OVRPlugin.Result.Success;
	}

	public static bool IsInsightPassthroughInitialized()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_GetInsightPassthroughInitialized() == OVRPlugin.Bool.True;
	}

	public static OVRPlugin.Result GetInsightPassthroughInitializationState()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_66_0.version)
		{
			return OVRPlugin.OVRP_1_66_0.ovrp_GetInsightPassthroughInitializationState();
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static bool CreateInsightTriangleMesh(int layerId, Vector3[] vertices, int[] triangles, out ulong meshHandle)
	{
		meshHandle = 0UL;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version))
		{
			return false;
		}
		if (vertices == null || triangles == null || vertices.Length == 0 || triangles.Length == 0)
		{
			return false;
		}
		int vertexCount = vertices.Length;
		int triangleCount = triangles.Length / 3;
		GCHandle gchandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
		IntPtr vertices2 = gchandle.AddrOfPinnedObject();
		GCHandle gchandle2 = GCHandle.Alloc(triangles, GCHandleType.Pinned);
		IntPtr triangles2 = gchandle2.AddrOfPinnedObject();
		bool flag = OVRPlugin.OVRP_1_63_0.ovrp_CreateInsightTriangleMesh(layerId, vertices2, vertexCount, triangles2, triangleCount, out meshHandle) != OVRPlugin.Result.Success;
		gchandle2.Free();
		gchandle.Free();
		return !flag;
	}

	public static bool DestroyInsightTriangleMesh(ulong meshHandle)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_DestroyInsightTriangleMesh(meshHandle) == OVRPlugin.Result.Success;
	}

	public static bool AddInsightPassthroughSurfaceGeometry(int layerId, ulong meshHandle, Matrix4x4 T_world_model, out ulong geometryInstanceHandle)
	{
		geometryInstanceHandle = 0UL;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_AddInsightPassthroughSurfaceGeometry(layerId, meshHandle, T_world_model, out geometryInstanceHandle) == OVRPlugin.Result.Success;
	}

	public static bool DestroyInsightPassthroughGeometryInstance(ulong geometryInstanceHandle)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_DestroyInsightPassthroughGeometryInstance(geometryInstanceHandle) == OVRPlugin.Result.Success;
	}

	public static bool UpdateInsightPassthroughGeometryTransform(ulong geometryInstanceHandle, Matrix4x4 transform)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_UpdateInsightPassthroughGeometryTransform(geometryInstanceHandle, transform) == OVRPlugin.Result.Success;
	}

	public static bool SetInsightPassthroughStyle(int layerId, OVRPlugin.InsightPassthroughStyle2 style)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_84_0.version)
		{
			return OVRPlugin.OVRP_1_84_0.ovrp_SetInsightPassthroughStyle2(layerId, style).IsSuccess();
		}
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version))
		{
			return false;
		}
		if (style.TextureColorMapType == OVRPlugin.InsightPassthroughColorMapType.ColorLut || style.TextureColorMapType == OVRPlugin.InsightPassthroughColorMapType.InterpolatedColorLut)
		{
			Debug.LogError("Only OVRPlugn version 1.84.0 or higher supports Color LUTs");
			return false;
		}
		OVRPlugin.InsightPassthroughStyle style2 = default(OVRPlugin.InsightPassthroughStyle);
		style.CopyTo(ref style2);
		return OVRPlugin.OVRP_1_63_0.ovrp_SetInsightPassthroughStyle(layerId, style2).IsSuccess();
	}

	public static bool SetInsightPassthroughStyle(int layerId, OVRPlugin.InsightPassthroughStyle style)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_63_0.version && OVRPlugin.OVRP_1_63_0.ovrp_SetInsightPassthroughStyle(layerId, style).IsSuccess();
	}

	public static bool CreatePassthroughColorLut(OVRPlugin.PassthroughColorLutChannels channels, uint resolution, OVRPlugin.PassthroughColorLutData data, out ulong colorLut)
	{
		colorLut = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_84_0.version)
		{
			return OVRPlugin.OVRP_1_84_0.ovrp_CreatePassthroughColorLut(channels, resolution, data, out colorLut).IsSuccess();
		}
		colorLut = 0UL;
		return false;
	}

	public static bool DestroyPassthroughColorLut(ulong colorLut)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_84_0.version && OVRPlugin.OVRP_1_84_0.ovrp_DestroyPassthroughColorLut(colorLut).IsSuccess();
	}

	public static bool UpdatePassthroughColorLut(ulong colorLut, OVRPlugin.PassthroughColorLutData data)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_84_0.version && OVRPlugin.OVRP_1_84_0.ovrp_UpdatePassthroughColorLut(colorLut, data).IsSuccess();
	}

	public static bool SetInsightPassthroughKeyboardHandsIntensity(int layerId, OVRPlugin.InsightPassthroughKeyboardHandsIntensity intensity)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_68_0.version && OVRPlugin.OVRP_1_68_0.ovrp_SetInsightPassthroughKeyboardHandsIntensity(layerId, intensity) == OVRPlugin.Result.Success;
	}

	public static OVRPlugin.PassthroughCapabilityFlags GetPassthroughCapabilityFlags()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version)
		{
			OVRPlugin.PassthroughCapabilityFlags result = (OVRPlugin.PassthroughCapabilityFlags)0;
			OVRPlugin.Result result2 = OVRPlugin.OVRP_1_78_0.ovrp_GetPassthroughCapabilityFlags(ref result);
			if (result2 == OVRPlugin.Result.Success)
			{
				return result;
			}
			Debug.LogError("Unable to retrieve passthrough capability flags. Try calling GetInsightPassthroughCapabilityFlags() while the XR plug-in is initialized. Failed with reason: " + result2.ToString());
		}
		else
		{
			Debug.LogWarning("ovrp_GetPassthroughCapabilityFlags() not yet supported by OVRPlugin. Result of GetInsightPassthroughCapabilityFlags() is not accurate.");
		}
		if (!OVRPlugin.IsInsightPassthroughSupported())
		{
			return (OVRPlugin.PassthroughCapabilityFlags)0;
		}
		return OVRPlugin.PassthroughCapabilityFlags.Passthrough;
	}

	public static OVRPlugin.Result GetPassthroughCapabilities(ref OVRPlugin.PassthroughCapabilities outCapabilities)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_85_0.version)
		{
			outCapabilities.Fields = (OVRPlugin.PassthroughCapabilityFields)3;
			return OVRPlugin.OVRP_1_85_0.ovrp_GetPassthroughCapabilities(ref outCapabilities);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Vector3f GetBoundaryDimensions(OVRPlugin.BoundaryType boundaryType)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version)
		{
			return OVRPlugin.OVRP_1_8_0.ovrp_GetBoundaryDimensions(boundaryType);
		}
		return default(OVRPlugin.Vector3f);
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static bool GetBoundaryVisible()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && OVRPlugin.OVRP_1_8_0.ovrp_GetBoundaryVisible() == OVRPlugin.Bool.True;
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static bool SetBoundaryVisible(bool value)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_8_0.version && OVRPlugin.OVRP_1_8_0.ovrp_SetBoundaryVisible(OVRPlugin.ToBool(value)) == OVRPlugin.Bool.True;
	}

	public static OVRPlugin.SystemHeadset GetSystemHeadsetType()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_9_0.version)
		{
			return OVRPlugin.OVRP_1_9_0.ovrp_GetSystemHeadsetType();
		}
		return OVRPlugin.SystemHeadset.None;
	}

	public static OVRPlugin.Controller GetActiveController()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_9_0.version)
		{
			return OVRPlugin.OVRP_1_9_0.ovrp_GetActiveController();
		}
		return OVRPlugin.Controller.None;
	}

	public static OVRPlugin.Controller GetConnectedControllers()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_9_0.version)
		{
			return OVRPlugin.OVRP_1_9_0.ovrp_GetConnectedControllers();
		}
		return OVRPlugin.Controller.None;
	}

	private static OVRPlugin.Bool ToBool(bool b)
	{
		if (!b)
		{
			return OVRPlugin.Bool.False;
		}
		return OVRPlugin.Bool.True;
	}

	public static OVRPlugin.TrackingOrigin GetTrackingOriginType()
	{
		return OVRPlugin.OVRP_1_0_0.ovrp_GetTrackingOriginType();
	}

	public static bool SetTrackingOriginType(OVRPlugin.TrackingOrigin originType)
	{
		return OVRPlugin.OVRP_1_0_0.ovrp_SetTrackingOriginType(originType) == OVRPlugin.Bool.True;
	}

	public static OVRPlugin.Posef GetTrackingCalibratedOrigin()
	{
		return OVRPlugin.OVRP_1_0_0.ovrp_GetTrackingCalibratedOrigin();
	}

	public static bool SetTrackingCalibratedOrigin()
	{
		return OVRPlugin.OVRP_1_2_0.ovrpi_SetTrackingCalibratedOrigin() == OVRPlugin.Bool.True;
	}

	public static bool RecenterTrackingOrigin(OVRPlugin.RecenterFlags flags)
	{
		return OVRPlugin.OVRP_1_0_0.ovrp_RecenterTrackingOrigin((uint)flags) == OVRPlugin.Bool.True;
	}

	public static bool UpdateCameraDevices()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_UpdateCameraDevices() == OVRPlugin.Result.Success;
	}

	public static bool IsCameraDeviceAvailable(OVRPlugin.CameraDevice cameraDevice)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_IsCameraDeviceAvailable(cameraDevice) == OVRPlugin.Bool.True;
	}

	public static bool SetCameraDevicePreferredColorFrameSize(OVRPlugin.CameraDevice cameraDevice, int width, int height)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_SetCameraDevicePreferredColorFrameSize(cameraDevice, new OVRPlugin.Sizei
		{
			w = width,
			h = height
		}) == OVRPlugin.Result.Success;
	}

	public static bool OpenCameraDevice(OVRPlugin.CameraDevice cameraDevice)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_OpenCameraDevice(cameraDevice) == OVRPlugin.Result.Success;
	}

	public static bool CloseCameraDevice(OVRPlugin.CameraDevice cameraDevice)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_CloseCameraDevice(cameraDevice) == OVRPlugin.Result.Success;
	}

	public static bool HasCameraDeviceOpened(OVRPlugin.CameraDevice cameraDevice)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_HasCameraDeviceOpened(cameraDevice) == OVRPlugin.Bool.True;
	}

	public static bool IsCameraDeviceColorFrameAvailable(OVRPlugin.CameraDevice cameraDevice)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version && OVRPlugin.OVRP_1_16_0.ovrp_IsCameraDeviceColorFrameAvailable(cameraDevice) == OVRPlugin.Bool.True;
	}

	public static Texture2D GetCameraDeviceColorFrameTexture(OVRPlugin.CameraDevice cameraDevice)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_16_0.version))
		{
			return null;
		}
		OVRPlugin.Sizei sizei = default(OVRPlugin.Sizei);
		if (OVRPlugin.OVRP_1_16_0.ovrp_GetCameraDeviceColorFrameSize(cameraDevice, out sizei) != OVRPlugin.Result.Success)
		{
			return null;
		}
		IntPtr data;
		int num;
		if (OVRPlugin.OVRP_1_16_0.ovrp_GetCameraDeviceColorFrameBgraPixels(cameraDevice, out data, out num) != OVRPlugin.Result.Success)
		{
			return null;
		}
		if (num != sizei.w * 4)
		{
			return null;
		}
		if (!OVRPlugin.cachedCameraFrameTexture || OVRPlugin.cachedCameraFrameTexture.width != sizei.w || OVRPlugin.cachedCameraFrameTexture.height != sizei.h)
		{
			OVRPlugin.cachedCameraFrameTexture = new Texture2D(sizei.w, sizei.h, TextureFormat.BGRA32, false);
		}
		OVRPlugin.cachedCameraFrameTexture.LoadRawTextureData(data, num * sizei.h);
		OVRPlugin.cachedCameraFrameTexture.Apply();
		return OVRPlugin.cachedCameraFrameTexture;
	}

	public static bool DoesCameraDeviceSupportDepth(OVRPlugin.CameraDevice cameraDevice)
	{
		OVRPlugin.Bool @bool;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_17_0.version && OVRPlugin.OVRP_1_17_0.ovrp_DoesCameraDeviceSupportDepth(cameraDevice, out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
	}

	public static bool SetCameraDeviceDepthSensingMode(OVRPlugin.CameraDevice camera, OVRPlugin.CameraDeviceDepthSensingMode depthSensoringMode)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_17_0.version && OVRPlugin.OVRP_1_17_0.ovrp_SetCameraDeviceDepthSensingMode(camera, depthSensoringMode) == OVRPlugin.Result.Success;
	}

	public static bool SetCameraDevicePreferredDepthQuality(OVRPlugin.CameraDevice camera, OVRPlugin.CameraDeviceDepthQuality depthQuality)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_17_0.version && OVRPlugin.OVRP_1_17_0.ovrp_SetCameraDevicePreferredDepthQuality(camera, depthQuality) == OVRPlugin.Result.Success;
	}

	public static bool IsCameraDeviceDepthFrameAvailable(OVRPlugin.CameraDevice cameraDevice)
	{
		OVRPlugin.Bool @bool;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_17_0.version && OVRPlugin.OVRP_1_17_0.ovrp_IsCameraDeviceDepthFrameAvailable(cameraDevice, out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
	}

	public static Texture2D GetCameraDeviceDepthFrameTexture(OVRPlugin.CameraDevice cameraDevice)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_17_0.version))
		{
			return null;
		}
		OVRPlugin.Sizei sizei = default(OVRPlugin.Sizei);
		if (OVRPlugin.OVRP_1_17_0.ovrp_GetCameraDeviceDepthFrameSize(cameraDevice, out sizei) != OVRPlugin.Result.Success)
		{
			return null;
		}
		IntPtr data;
		int num;
		if (OVRPlugin.OVRP_1_17_0.ovrp_GetCameraDeviceDepthFramePixels(cameraDevice, out data, out num) != OVRPlugin.Result.Success)
		{
			return null;
		}
		if (num != sizei.w * 4)
		{
			return null;
		}
		if (!OVRPlugin.cachedCameraDepthTexture || OVRPlugin.cachedCameraDepthTexture.width != sizei.w || OVRPlugin.cachedCameraDepthTexture.height != sizei.h)
		{
			OVRPlugin.cachedCameraDepthTexture = new Texture2D(sizei.w, sizei.h, TextureFormat.RFloat, false);
			OVRPlugin.cachedCameraDepthTexture.filterMode = FilterMode.Point;
		}
		OVRPlugin.cachedCameraDepthTexture.LoadRawTextureData(data, num * sizei.h);
		OVRPlugin.cachedCameraDepthTexture.Apply();
		return OVRPlugin.cachedCameraDepthTexture;
	}

	public static Texture2D GetCameraDeviceDepthConfidenceTexture(OVRPlugin.CameraDevice cameraDevice)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_17_0.version))
		{
			return null;
		}
		OVRPlugin.Sizei sizei = default(OVRPlugin.Sizei);
		if (OVRPlugin.OVRP_1_17_0.ovrp_GetCameraDeviceDepthFrameSize(cameraDevice, out sizei) != OVRPlugin.Result.Success)
		{
			return null;
		}
		IntPtr data;
		int num;
		if (OVRPlugin.OVRP_1_17_0.ovrp_GetCameraDeviceDepthConfidencePixels(cameraDevice, out data, out num) != OVRPlugin.Result.Success)
		{
			return null;
		}
		if (num != sizei.w * 4)
		{
			return null;
		}
		if (!OVRPlugin.cachedCameraDepthConfidenceTexture || OVRPlugin.cachedCameraDepthConfidenceTexture.width != sizei.w || OVRPlugin.cachedCameraDepthConfidenceTexture.height != sizei.h)
		{
			OVRPlugin.cachedCameraDepthConfidenceTexture = new Texture2D(sizei.w, sizei.h, TextureFormat.RFloat, false);
		}
		OVRPlugin.cachedCameraDepthConfidenceTexture.LoadRawTextureData(data, num * sizei.h);
		OVRPlugin.cachedCameraDepthConfidenceTexture.Apply();
		return OVRPlugin.cachedCameraDepthConfidenceTexture;
	}

	private static bool foveatedRenderingSupported
	{
		get
		{
			return OVRPlugin.fixedFoveatedRenderingSupported || OVRPlugin.eyeTrackedFoveatedRenderingSupported;
		}
	}

	public static bool eyeTrackedFoveatedRenderingSupported
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version)
			{
				OVRPlugin.Bool @bool;
				OVRPlugin.OVRP_1_78_0.ovrp_GetFoveationEyeTrackedSupported(out @bool);
				return @bool == OVRPlugin.Bool.True;
			}
			return false;
		}
	}

	public static bool eyeTrackedFoveatedRenderingEnabled
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.eyeTrackedFoveatedRenderingSupported)
			{
				OVRPlugin.Bool @bool;
				OVRPlugin.OVRP_1_78_0.ovrp_GetFoveationEyeTracked(out @bool);
				return @bool == OVRPlugin.Bool.True;
			}
			return false;
		}
		set
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.eyeTrackedFoveatedRenderingSupported)
			{
				OVRPlugin.OVRP_1_78_0.ovrp_SetFoveationEyeTracked(value ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
			}
		}
	}

	public static bool fixedFoveatedRenderingSupported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version && OVRPlugin.OVRP_1_21_0.ovrp_GetTiledMultiResSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static OVRPlugin.FoveatedRenderingLevel foveatedRenderingLevel
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version && OVRPlugin.foveatedRenderingSupported)
			{
				OVRPlugin.FoveatedRenderingLevel result;
				OVRPlugin.OVRP_1_21_0.ovrp_GetTiledMultiResLevel(out result);
				return result;
			}
			return OVRPlugin.FoveatedRenderingLevel.Off;
		}
		set
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version && OVRPlugin.foveatedRenderingSupported)
			{
				if (value == OVRPlugin.FoveatedRenderingLevel.HighTop)
				{
					value = OVRPlugin.FoveatedRenderingLevel.High;
					Debug.LogWarning("FoveatedRenderingLevel.HighTop is not supported in OpenXR, changed to FoveatedRenderingLevel.High instead.");
				}
				OVRPlugin.OVRP_1_21_0.ovrp_SetTiledMultiResLevel(value);
			}
		}
	}

	[Obsolete("Please use foveatedRenderingLevel instead", false)]
	public static OVRPlugin.FixedFoveatedRenderingLevel fixedFoveatedRenderingLevel
	{
		get
		{
			return (OVRPlugin.FixedFoveatedRenderingLevel)OVRPlugin.foveatedRenderingLevel;
		}
		set
		{
			OVRPlugin.foveatedRenderingLevel = (OVRPlugin.FoveatedRenderingLevel)value;
		}
	}

	public static bool useDynamicFoveatedRendering
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_46_0.version && OVRPlugin.foveatedRenderingSupported)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				OVRPlugin.OVRP_1_46_0.ovrp_GetTiledMultiResDynamic(out @bool);
				return @bool > OVRPlugin.Bool.False;
			}
			return false;
		}
		set
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_46_0.version && OVRPlugin.foveatedRenderingSupported)
			{
				OVRPlugin.OVRP_1_46_0.ovrp_SetTiledMultiResDynamic(value ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
			}
		}
	}

	[Obsolete("Please use useDynamicFoveatedRendering instead", false)]
	public static bool useDynamicFixedFoveatedRendering
	{
		get
		{
			return OVRPlugin.useDynamicFoveatedRendering;
		}
		set
		{
			OVRPlugin.useDynamicFoveatedRendering = value;
		}
	}

	[Obsolete("Please use fixedFoveatedRenderingSupported instead", false)]
	public static bool tiledMultiResSupported
	{
		get
		{
			return OVRPlugin.fixedFoveatedRenderingSupported;
		}
	}

	[Obsolete("Please use foveatedRenderingLevel instead", false)]
	public static OVRPlugin.TiledMultiResLevel tiledMultiResLevel
	{
		get
		{
			return (OVRPlugin.TiledMultiResLevel)OVRPlugin.foveatedRenderingLevel;
		}
		set
		{
			OVRPlugin.foveatedRenderingLevel = (OVRPlugin.FoveatedRenderingLevel)value;
		}
	}

	public static bool gpuUtilSupported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version && OVRPlugin.OVRP_1_21_0.ovrp_GetGPUUtilSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static float gpuUtilLevel
	{
		get
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version) || !OVRPlugin.gpuUtilSupported)
			{
				return 0f;
			}
			float result;
			if (OVRPlugin.OVRP_1_21_0.ovrp_GetGPUUtilLevel(out result) == OVRPlugin.Result.Success)
			{
				return result;
			}
			return 0f;
		}
	}

	public static float[] systemDisplayFrequenciesAvailable
	{
		get
		{
			if (OVRPlugin._cachedSystemDisplayFrequenciesAvailable == null)
			{
				OVRPlugin._cachedSystemDisplayFrequenciesAvailable = new float[0];
				if (OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version)
				{
					int num = 0;
					if (OVRPlugin.OVRP_1_21_0.ovrp_GetSystemDisplayAvailableFrequencies(IntPtr.Zero, ref num) == OVRPlugin.Result.Success && num > 0)
					{
						int num2 = num;
						OVRPlugin._nativeSystemDisplayFrequenciesAvailable = new OVRNativeBuffer(4 * num2);
						if (OVRPlugin.OVRP_1_21_0.ovrp_GetSystemDisplayAvailableFrequencies(OVRPlugin._nativeSystemDisplayFrequenciesAvailable.GetPointer(0), ref num) == OVRPlugin.Result.Success)
						{
							int num3 = (num <= num2) ? num : num2;
							if (num3 > 0)
							{
								OVRPlugin._cachedSystemDisplayFrequenciesAvailable = new float[num3];
								Marshal.Copy(OVRPlugin._nativeSystemDisplayFrequenciesAvailable.GetPointer(0), OVRPlugin._cachedSystemDisplayFrequenciesAvailable, 0, num3);
							}
						}
					}
				}
			}
			return OVRPlugin._cachedSystemDisplayFrequenciesAvailable;
		}
	}

	public static float systemDisplayFrequency
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version)
			{
				float result;
				if (OVRPlugin.OVRP_1_21_0.ovrp_GetSystemDisplayFrequency2(out result) == OVRPlugin.Result.Success)
				{
					return result;
				}
				return 0f;
			}
			else
			{
				if (OVRPlugin.version >= OVRPlugin.OVRP_1_1_0.version)
				{
					return OVRPlugin.OVRP_1_1_0.ovrp_GetSystemDisplayFrequency();
				}
				return 0f;
			}
		}
		set
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version)
			{
				OVRPlugin.OVRP_1_21_0.ovrp_SetSystemDisplayFrequency(value);
			}
		}
	}

	public static bool eyeFovPremultipliedAlphaModeEnabled
	{
		get
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.True;
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_57_0.version)
			{
				OVRPlugin.OVRP_1_57_0.ovrp_GetEyeFovPremultipliedAlphaMode(ref @bool);
			}
			return @bool == OVRPlugin.Bool.True;
		}
		set
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_57_0.version)
			{
				OVRPlugin.OVRP_1_57_0.ovrp_SetEyeFovPremultipliedAlphaMode(OVRPlugin.ToBool(value));
			}
		}
	}

	public static bool GetNodeFrustum2(OVRPlugin.Node nodeId, out OVRPlugin.Frustumf2 frustum)
	{
		frustum = default(OVRPlugin.Frustumf2);
		return OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_GetNodeFrustum2(nodeId, out frustum) == OVRPlugin.Result.Success;
	}

	public static bool AsymmetricFovEnabled
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_21_0.version)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				return OVRPlugin.OVRP_1_21_0.ovrp_GetAppAsymmetricFov(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
			}
			return false;
		}
	}

	public static bool EyeTextureArrayEnabled
	{
		get
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_15_0.version && OVRPlugin.OVRP_1_15_0.ovrp_GetEyeTextureArrayEnabled() == OVRPlugin.Bool.True;
		}
	}

	public static bool localDimmingSupported
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				return OVRPlugin.OVRP_1_78_0.ovrp_GetLocalDimmingSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
			}
			return false;
		}
	}

	public static bool localDimming
	{
		get
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.localDimmingSupported)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				if (OVRPlugin.OVRP_1_78_0.ovrp_GetLocalDimming(out @bool) == OVRPlugin.Result.Success)
				{
					return @bool == OVRPlugin.Bool.True;
				}
			}
			return false;
		}
		set
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.localDimmingSupported)
			{
				OVRPlugin.OVRP_1_78_0.ovrp_SetLocalDimming(value ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
			}
		}
	}

	public static OVRPlugin.Handedness GetDominantHand()
	{
		OVRPlugin.Handedness result;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_28_0.version && OVRPlugin.OVRP_1_28_0.ovrp_GetDominantHand(out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return OVRPlugin.Handedness.Unsupported;
	}

	public static bool SendEvent(string name, string param = "", string source = "")
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_30_0.version)
		{
			return OVRPlugin.OVRP_1_30_0.ovrp_SendEvent2(name, param, (source.Length == 0) ? "integration" : source) == OVRPlugin.Result.Success;
		}
		return OVRPlugin.version >= OVRPlugin.OVRP_1_28_0.version && OVRPlugin.OVRP_1_28_0.ovrp_SendEvent(name, param) == OVRPlugin.Result.Success;
	}

	public static OVRPlugin.Result SendUnifiedEvent(OVRPlugin.Bool isEssential, string productType, string eventName, string event_metadata_json, string project_name = "", string event_entrypoint = "", string project_guid = "", string event_type = "", string event_target = "", string error_msg = "", string is_internal_build = "", string batch_mode = "")
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_110_0.version)
		{
			return OVRPlugin.OVRP_1_110_0.ovrp_SendUnifiedEventV2(isEssential, productType, eventName, event_metadata_json, project_name, event_entrypoint, project_guid, event_type, event_target, error_msg, is_internal_build, batch_mode);
		}
		if (OVRPlugin.version == OVRPlugin.OVRP_1_109_0.version)
		{
			return OVRPlugin.OVRP_1_109_0.ovrp_SendUnifiedEvent(isEssential, productType, eventName, event_metadata_json, project_name, event_entrypoint, project_guid, event_type, event_target, error_msg, is_internal_build);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static bool SetHeadPoseModifier(ref OVRPlugin.Quatf relativeRotation, ref OVRPlugin.Vector3f relativeTranslation)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_29_0.version && OVRPlugin.OVRP_1_29_0.ovrp_SetHeadPoseModifier(ref relativeRotation, ref relativeTranslation) == OVRPlugin.Result.Success;
	}

	public static bool GetHeadPoseModifier(out OVRPlugin.Quatf relativeRotation, out OVRPlugin.Vector3f relativeTranslation)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_29_0.version)
		{
			return OVRPlugin.OVRP_1_29_0.ovrp_GetHeadPoseModifier(out relativeRotation, out relativeTranslation) == OVRPlugin.Result.Success;
		}
		relativeRotation = OVRPlugin.Quatf.identity;
		relativeTranslation = OVRPlugin.Vector3f.zero;
		return false;
	}

	public static bool IsPerfMetricsSupported(OVRPlugin.PerfMetrics perfMetrics)
	{
		OVRPlugin.Bool @bool;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_30_0.version && OVRPlugin.OVRP_1_30_0.ovrp_IsPerfMetricsSupported(perfMetrics, out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
	}

	public static float? GetPerfMetricsFloat(OVRPlugin.PerfMetrics perfMetrics)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_30_0.version))
		{
			return null;
		}
		float value;
		if (OVRPlugin.OVRP_1_30_0.ovrp_GetPerfMetricsFloat(perfMetrics, out value) == OVRPlugin.Result.Success)
		{
			return new float?(value);
		}
		return null;
	}

	public static int? GetPerfMetricsInt(OVRPlugin.PerfMetrics perfMetrics)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_30_0.version))
		{
			return null;
		}
		int value;
		if (OVRPlugin.OVRP_1_30_0.ovrp_GetPerfMetricsInt(perfMetrics, out value) == OVRPlugin.Result.Success)
		{
			return new int?(value);
		}
		return null;
	}

	public static double GetTimeInSeconds()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_31_0.version))
		{
			return 0.0;
		}
		double result;
		if (OVRPlugin.OVRP_1_31_0.ovrp_GetTimeInSeconds(out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0.0;
	}

	public static bool SetColorScaleAndOffset(Vector4 colorScale, Vector4 colorOffset, bool applyToAllLayers)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_31_0.version)
		{
			OVRPlugin.Bool applyToAllLayers2 = applyToAllLayers ? OVRPlugin.Bool.True : OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_31_0.ovrp_SetColorScaleAndOffset(colorScale, colorOffset, applyToAllLayers2) == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static bool AddCustomMetadata(string name, string param = "")
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_32_0.version && OVRPlugin.OVRP_1_32_0.ovrp_AddCustomMetadata(name, param) == OVRPlugin.Result.Success;
	}

	public static bool SetDeveloperMode(OVRPlugin.Bool active)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_SetDeveloperMode(active) == OVRPlugin.Result.Success;
	}

	[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
	public static float GetAdaptiveGPUPerformanceScale()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_42_0.version))
		{
			return 1f;
		}
		float result = 1f;
		if (OVRPlugin.OVRP_1_42_0.ovrp_GetAdaptiveGpuPerformanceScale2(ref result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 1f;
	}

	public static bool GetHandTrackingEnabled()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_44_0.ovrp_GetHandTrackingEnabled(ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static OVRHandSkeletonVersion HandSkeletonVersion { get; private set; } = OVRHandSkeletonVersion.OVR;

	public static bool GetHandState(OVRPlugin.Step stepId, OVRPlugin.Hand hand, ref OVRPlugin.HandState handState)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version && OVRPlugin.HandSkeletonVersion == OVRHandSkeletonVersion.OpenXR)
		{
			if (OVRPlugin.OVRP_1_103_0.ovrp_GetHandState3(stepId, -1, hand, out OVRPlugin.cachedHandState3) == OVRPlugin.Result.Success)
			{
				if (handState.BoneRotations == null || handState.BoneRotations.Length != 26)
				{
					handState.BoneRotations = new OVRPlugin.Quatf[26];
				}
				if (handState.BonePositions == null || handState.BonePositions.Length != 26)
				{
					handState.BonePositions = new OVRPlugin.Vector3f[26];
				}
				if (handState.PinchStrength == null || handState.PinchStrength.Length != 5)
				{
					handState.PinchStrength = new float[5];
				}
				if (handState.FingerConfidences == null || handState.FingerConfidences.Length != 5)
				{
					handState.FingerConfidences = new OVRPlugin.TrackingConfidence[5];
				}
				handState.Status = OVRPlugin.cachedHandState3.Status;
				handState.RootPose = OVRPlugin.cachedHandState3.RootPose;
				handState.BoneRotations[0] = OVRPlugin.cachedHandState3.BonePoses_0.Orientation;
				handState.BoneRotations[1] = OVRPlugin.cachedHandState3.BonePoses_1.Orientation;
				handState.BoneRotations[2] = OVRPlugin.cachedHandState3.BonePoses_2.Orientation;
				handState.BoneRotations[3] = OVRPlugin.cachedHandState3.BonePoses_3.Orientation;
				handState.BoneRotations[4] = OVRPlugin.cachedHandState3.BonePoses_4.Orientation;
				handState.BoneRotations[5] = OVRPlugin.cachedHandState3.BonePoses_5.Orientation;
				handState.BoneRotations[6] = OVRPlugin.cachedHandState3.BonePoses_6.Orientation;
				handState.BoneRotations[7] = OVRPlugin.cachedHandState3.BonePoses_7.Orientation;
				handState.BoneRotations[8] = OVRPlugin.cachedHandState3.BonePoses_8.Orientation;
				handState.BoneRotations[9] = OVRPlugin.cachedHandState3.BonePoses_9.Orientation;
				handState.BoneRotations[10] = OVRPlugin.cachedHandState3.BonePoses_10.Orientation;
				handState.BoneRotations[11] = OVRPlugin.cachedHandState3.BonePoses_11.Orientation;
				handState.BoneRotations[12] = OVRPlugin.cachedHandState3.BonePoses_12.Orientation;
				handState.BoneRotations[13] = OVRPlugin.cachedHandState3.BonePoses_13.Orientation;
				handState.BoneRotations[14] = OVRPlugin.cachedHandState3.BonePoses_14.Orientation;
				handState.BoneRotations[15] = OVRPlugin.cachedHandState3.BonePoses_15.Orientation;
				handState.BoneRotations[16] = OVRPlugin.cachedHandState3.BonePoses_16.Orientation;
				handState.BoneRotations[17] = OVRPlugin.cachedHandState3.BonePoses_17.Orientation;
				handState.BoneRotations[18] = OVRPlugin.cachedHandState3.BonePoses_18.Orientation;
				handState.BoneRotations[19] = OVRPlugin.cachedHandState3.BonePoses_19.Orientation;
				handState.BoneRotations[20] = OVRPlugin.cachedHandState3.BonePoses_20.Orientation;
				handState.BoneRotations[21] = OVRPlugin.cachedHandState3.BonePoses_21.Orientation;
				handState.BoneRotations[22] = OVRPlugin.cachedHandState3.BonePoses_22.Orientation;
				handState.BoneRotations[23] = OVRPlugin.cachedHandState3.BonePoses_23.Orientation;
				handState.BoneRotations[24] = OVRPlugin.cachedHandState3.BonePoses_24.Orientation;
				handState.BoneRotations[25] = OVRPlugin.cachedHandState3.BonePoses_25.Orientation;
				handState.BonePositions[0] = OVRPlugin.cachedHandState3.BonePoses_0.Position;
				handState.BonePositions[1] = OVRPlugin.cachedHandState3.BonePoses_1.Position;
				handState.BonePositions[2] = OVRPlugin.cachedHandState3.BonePoses_2.Position;
				handState.BonePositions[3] = OVRPlugin.cachedHandState3.BonePoses_3.Position;
				handState.BonePositions[4] = OVRPlugin.cachedHandState3.BonePoses_4.Position;
				handState.BonePositions[5] = OVRPlugin.cachedHandState3.BonePoses_5.Position;
				handState.BonePositions[6] = OVRPlugin.cachedHandState3.BonePoses_6.Position;
				handState.BonePositions[7] = OVRPlugin.cachedHandState3.BonePoses_7.Position;
				handState.BonePositions[8] = OVRPlugin.cachedHandState3.BonePoses_8.Position;
				handState.BonePositions[9] = OVRPlugin.cachedHandState3.BonePoses_9.Position;
				handState.BonePositions[10] = OVRPlugin.cachedHandState3.BonePoses_10.Position;
				handState.BonePositions[11] = OVRPlugin.cachedHandState3.BonePoses_11.Position;
				handState.BonePositions[12] = OVRPlugin.cachedHandState3.BonePoses_12.Position;
				handState.BonePositions[13] = OVRPlugin.cachedHandState3.BonePoses_13.Position;
				handState.BonePositions[14] = OVRPlugin.cachedHandState3.BonePoses_14.Position;
				handState.BonePositions[15] = OVRPlugin.cachedHandState3.BonePoses_15.Position;
				handState.BonePositions[16] = OVRPlugin.cachedHandState3.BonePoses_16.Position;
				handState.BonePositions[17] = OVRPlugin.cachedHandState3.BonePoses_17.Position;
				handState.BonePositions[18] = OVRPlugin.cachedHandState3.BonePoses_18.Position;
				handState.BonePositions[19] = OVRPlugin.cachedHandState3.BonePoses_19.Position;
				handState.BonePositions[20] = OVRPlugin.cachedHandState3.BonePoses_20.Position;
				handState.BonePositions[21] = OVRPlugin.cachedHandState3.BonePoses_21.Position;
				handState.BonePositions[22] = OVRPlugin.cachedHandState3.BonePoses_22.Position;
				handState.BonePositions[23] = OVRPlugin.cachedHandState3.BonePoses_23.Position;
				handState.BonePositions[24] = OVRPlugin.cachedHandState3.BonePoses_24.Position;
				handState.BonePositions[25] = OVRPlugin.cachedHandState3.BonePoses_25.Position;
				handState.Pinches = OVRPlugin.cachedHandState3.Pinches;
				handState.PinchStrength[0] = OVRPlugin.cachedHandState3.PinchStrength_0;
				handState.PinchStrength[1] = OVRPlugin.cachedHandState3.PinchStrength_1;
				handState.PinchStrength[2] = OVRPlugin.cachedHandState3.PinchStrength_2;
				handState.PinchStrength[3] = OVRPlugin.cachedHandState3.PinchStrength_3;
				handState.PinchStrength[4] = OVRPlugin.cachedHandState3.PinchStrength_4;
				handState.PointerPose = OVRPlugin.cachedHandState3.PointerPose;
				handState.HandScale = OVRPlugin.cachedHandState3.HandScale;
				handState.HandConfidence = OVRPlugin.cachedHandState3.HandConfidence;
				handState.FingerConfidences[0] = OVRPlugin.cachedHandState3.FingerConfidences_0;
				handState.FingerConfidences[1] = OVRPlugin.cachedHandState3.FingerConfidences_1;
				handState.FingerConfidences[2] = OVRPlugin.cachedHandState3.FingerConfidences_2;
				handState.FingerConfidences[3] = OVRPlugin.cachedHandState3.FingerConfidences_3;
				handState.FingerConfidences[4] = OVRPlugin.cachedHandState3.FingerConfidences_4;
				handState.RequestedTimeStamp = OVRPlugin.cachedHandState3.RequestedTimeStamp;
				handState.SampleTimeStamp = OVRPlugin.cachedHandState3.SampleTimeStamp;
				return true;
			}
			return false;
		}
		else
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version))
			{
				return false;
			}
			if (OVRPlugin.OVRP_1_44_0.ovrp_GetHandState(stepId, hand, out OVRPlugin.cachedHandState) == OVRPlugin.Result.Success)
			{
				if (handState.BoneRotations == null || handState.BoneRotations.Length != 24)
				{
					handState.BoneRotations = new OVRPlugin.Quatf[24];
				}
				if (handState.PinchStrength == null || handState.PinchStrength.Length != 5)
				{
					handState.PinchStrength = new float[5];
				}
				if (handState.FingerConfidences == null || handState.FingerConfidences.Length != 5)
				{
					handState.FingerConfidences = new OVRPlugin.TrackingConfidence[5];
				}
				handState.Status = OVRPlugin.cachedHandState.Status;
				handState.RootPose = OVRPlugin.cachedHandState.RootPose;
				handState.BoneRotations[0] = OVRPlugin.cachedHandState.BoneRotations_0;
				handState.BoneRotations[1] = OVRPlugin.cachedHandState.BoneRotations_1;
				handState.BoneRotations[2] = OVRPlugin.cachedHandState.BoneRotations_2;
				handState.BoneRotations[3] = OVRPlugin.cachedHandState.BoneRotations_3;
				handState.BoneRotations[4] = OVRPlugin.cachedHandState.BoneRotations_4;
				handState.BoneRotations[5] = OVRPlugin.cachedHandState.BoneRotations_5;
				handState.BoneRotations[6] = OVRPlugin.cachedHandState.BoneRotations_6;
				handState.BoneRotations[7] = OVRPlugin.cachedHandState.BoneRotations_7;
				handState.BoneRotations[8] = OVRPlugin.cachedHandState.BoneRotations_8;
				handState.BoneRotations[9] = OVRPlugin.cachedHandState.BoneRotations_9;
				handState.BoneRotations[10] = OVRPlugin.cachedHandState.BoneRotations_10;
				handState.BoneRotations[11] = OVRPlugin.cachedHandState.BoneRotations_11;
				handState.BoneRotations[12] = OVRPlugin.cachedHandState.BoneRotations_12;
				handState.BoneRotations[13] = OVRPlugin.cachedHandState.BoneRotations_13;
				handState.BoneRotations[14] = OVRPlugin.cachedHandState.BoneRotations_14;
				handState.BoneRotations[15] = OVRPlugin.cachedHandState.BoneRotations_15;
				handState.BoneRotations[16] = OVRPlugin.cachedHandState.BoneRotations_16;
				handState.BoneRotations[17] = OVRPlugin.cachedHandState.BoneRotations_17;
				handState.BoneRotations[18] = OVRPlugin.cachedHandState.BoneRotations_18;
				handState.BoneRotations[19] = OVRPlugin.cachedHandState.BoneRotations_19;
				handState.BoneRotations[20] = OVRPlugin.cachedHandState.BoneRotations_20;
				handState.BoneRotations[21] = OVRPlugin.cachedHandState.BoneRotations_21;
				handState.BoneRotations[22] = OVRPlugin.cachedHandState.BoneRotations_22;
				handState.BoneRotations[23] = OVRPlugin.cachedHandState.BoneRotations_23;
				handState.Pinches = OVRPlugin.cachedHandState.Pinches;
				handState.PinchStrength[0] = OVRPlugin.cachedHandState.PinchStrength_0;
				handState.PinchStrength[1] = OVRPlugin.cachedHandState.PinchStrength_1;
				handState.PinchStrength[2] = OVRPlugin.cachedHandState.PinchStrength_2;
				handState.PinchStrength[3] = OVRPlugin.cachedHandState.PinchStrength_3;
				handState.PinchStrength[4] = OVRPlugin.cachedHandState.PinchStrength_4;
				handState.PointerPose = OVRPlugin.cachedHandState.PointerPose;
				handState.HandScale = OVRPlugin.cachedHandState.HandScale;
				handState.HandConfidence = OVRPlugin.cachedHandState.HandConfidence;
				handState.FingerConfidences[0] = OVRPlugin.cachedHandState.FingerConfidences_0;
				handState.FingerConfidences[1] = OVRPlugin.cachedHandState.FingerConfidences_1;
				handState.FingerConfidences[2] = OVRPlugin.cachedHandState.FingerConfidences_2;
				handState.FingerConfidences[3] = OVRPlugin.cachedHandState.FingerConfidences_3;
				handState.FingerConfidences[4] = OVRPlugin.cachedHandState.FingerConfidences_4;
				handState.RequestedTimeStamp = OVRPlugin.cachedHandState.RequestedTimeStamp;
				handState.SampleTimeStamp = OVRPlugin.cachedHandState.SampleTimeStamp;
				return true;
			}
			return false;
		}
	}

	public static bool GetHandTrackingState(OVRPlugin.Step stepId, OVRPlugin.Hand hand, ref OVRPlugin.HandTrackingState handTrackingState)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version < OVRPlugin.OVRP_1_81_0.version)
		{
			return false;
		}
		if (OVRPlugin.OVRP_1_106_0.ovrp_GetHandTrackingState(stepId, -1, hand, out OVRPlugin.cachedHandTrackingState) != OVRPlugin.Result.Success)
		{
			return false;
		}
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_106_0.version)
		{
			handTrackingState.Microgesture = OVRPlugin.cachedHandTrackingState.Microgesture;
		}
		return true;
	}

	public static bool IsValidBone(OVRPlugin.BoneId bone, OVRPlugin.SkeletonType skeletonType)
	{
		switch (skeletonType)
		{
		case OVRPlugin.SkeletonType.HandLeft:
		case OVRPlugin.SkeletonType.HandRight:
			return bone >= OVRPlugin.BoneId.Hand_Start && bone <= OVRPlugin.BoneId.Hand_End;
		case OVRPlugin.SkeletonType.Body:
			return bone >= OVRPlugin.BoneId.Hand_Start && bone <= OVRPlugin.BoneId.Body_End;
		case OVRPlugin.SkeletonType.FullBody:
			return bone >= OVRPlugin.BoneId.Hand_Start && bone <= OVRPlugin.BoneId.FullBody_End;
		case OVRPlugin.SkeletonType.XRHandLeft:
		case OVRPlugin.SkeletonType.XRHandRight:
			return bone >= OVRPlugin.BoneId.Hand_Start && bone <= OVRPlugin.BoneId.XRHand_Max;
		}
		return false;
	}

	public static bool GetSkeleton(OVRPlugin.SkeletonType skeletonType, out OVRPlugin.Skeleton skeleton)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			return OVRPlugin.OVRP_1_44_0.ovrp_GetSkeleton(skeletonType, out skeleton) == OVRPlugin.Result.Success;
		}
		skeleton = default(OVRPlugin.Skeleton);
		return false;
	}

	public static bool GetSkeleton2(OVRPlugin.SkeletonType skeletonType, ref OVRPlugin.Skeleton2 skeleton)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version)
		{
			OVRPlugin.SkeletonType skeletonType2 = skeletonType;
			if (OVRSkeleton.IsBodySkeleton((OVRSkeleton.SkeletonType)skeletonType))
			{
				switch (OVRPlugin._currentJointSet)
				{
				case OVRPlugin.BodyJointSet.None:
					Debug.LogError("Global joint set is invalid. Ensure that there is an OVRBody instance that is active an enabled with a valid joint set");
					return false;
				case OVRPlugin.BodyJointSet.UpperBody:
					skeletonType2 = OVRPlugin.SkeletonType.Body;
					break;
				case OVRPlugin.BodyJointSet.FullBody:
					skeletonType2 = OVRPlugin.SkeletonType.FullBody;
					break;
				}
			}
			if (OVRPlugin.OVRP_1_92_0.ovrp_GetSkeleton3(skeletonType2, out OVRPlugin.cachedSkeleton3) == OVRPlugin.Result.Success)
			{
				if (skeleton.BoneCapsules == null || skeleton.BoneCapsules.Length != 19)
				{
					skeleton.BoneCapsules = new OVRPlugin.BoneCapsule[19];
				}
				skeleton.Type = OVRPlugin.cachedSkeleton3.Type;
				skeleton.NumBoneCapsules = OVRPlugin.cachedSkeleton3.NumBoneCapsules;
				if (skeletonType == OVRPlugin.SkeletonType.Body && skeletonType2 == OVRPlugin.SkeletonType.FullBody)
				{
					uint num = 0U;
					int num2 = 0;
					while ((long)num2 < (long)((ulong)OVRPlugin.cachedSkeleton3.NumBones))
					{
						if (OVRPlugin.Skeleton3GetBone[num2]().Id < OVRPlugin.BoneId.Body_End)
						{
							num += 1U;
						}
						num2++;
					}
					skeleton.NumBones = num;
					if (skeleton.Bones == null || (long)skeleton.Bones.Length != (long)((ulong)num))
					{
						skeleton.Bones = new OVRPlugin.Bone[num];
					}
					int num3 = 0;
					int num4 = 0;
					while ((long)num4 < (long)((ulong)OVRPlugin.cachedSkeleton3.NumBones))
					{
						if (OVRPlugin.Skeleton3GetBone[num4]().Id < OVRPlugin.BoneId.Body_End)
						{
							skeleton.Bones[num3++] = OVRPlugin.Skeleton3GetBone[num4]();
						}
						num4++;
					}
				}
				else
				{
					skeleton.NumBones = OVRPlugin.cachedSkeleton3.NumBones;
					if (skeleton.Bones == null || (long)skeleton.Bones.Length != (long)((ulong)skeleton.NumBones))
					{
						skeleton.Bones = new OVRPlugin.Bone[skeleton.NumBones];
					}
					int num5 = 0;
					while ((long)num5 < (long)((ulong)skeleton.NumBones))
					{
						skeleton.Bones[num5] = OVRPlugin.Skeleton3GetBone[num5]();
						num5++;
					}
				}
				skeleton.BoneCapsules[0] = OVRPlugin.cachedSkeleton3.BoneCapsules_0;
				skeleton.BoneCapsules[1] = OVRPlugin.cachedSkeleton3.BoneCapsules_1;
				skeleton.BoneCapsules[2] = OVRPlugin.cachedSkeleton3.BoneCapsules_2;
				skeleton.BoneCapsules[3] = OVRPlugin.cachedSkeleton3.BoneCapsules_3;
				skeleton.BoneCapsules[4] = OVRPlugin.cachedSkeleton3.BoneCapsules_4;
				skeleton.BoneCapsules[5] = OVRPlugin.cachedSkeleton3.BoneCapsules_5;
				skeleton.BoneCapsules[6] = OVRPlugin.cachedSkeleton3.BoneCapsules_6;
				skeleton.BoneCapsules[7] = OVRPlugin.cachedSkeleton3.BoneCapsules_7;
				skeleton.BoneCapsules[8] = OVRPlugin.cachedSkeleton3.BoneCapsules_8;
				skeleton.BoneCapsules[9] = OVRPlugin.cachedSkeleton3.BoneCapsules_9;
				skeleton.BoneCapsules[10] = OVRPlugin.cachedSkeleton3.BoneCapsules_10;
				skeleton.BoneCapsules[11] = OVRPlugin.cachedSkeleton3.BoneCapsules_11;
				skeleton.BoneCapsules[12] = OVRPlugin.cachedSkeleton3.BoneCapsules_12;
				skeleton.BoneCapsules[13] = OVRPlugin.cachedSkeleton3.BoneCapsules_13;
				skeleton.BoneCapsules[14] = OVRPlugin.cachedSkeleton3.BoneCapsules_14;
				skeleton.BoneCapsules[15] = OVRPlugin.cachedSkeleton3.BoneCapsules_15;
				skeleton.BoneCapsules[16] = OVRPlugin.cachedSkeleton3.BoneCapsules_16;
				skeleton.BoneCapsules[17] = OVRPlugin.cachedSkeleton3.BoneCapsules_17;
				skeleton.BoneCapsules[18] = OVRPlugin.cachedSkeleton3.BoneCapsules_18;
				return true;
			}
			return false;
		}
		else if (OVRPlugin.version >= OVRPlugin.OVRP_1_55_0.version)
		{
			if (OVRPlugin.OVRP_1_55_0.ovrp_GetSkeleton2(skeletonType, out OVRPlugin.cachedSkeleton2) == OVRPlugin.Result.Success)
			{
				if (skeleton.BoneCapsules == null || skeleton.BoneCapsules.Length != 19)
				{
					skeleton.BoneCapsules = new OVRPlugin.BoneCapsule[19];
				}
				skeleton.Type = OVRPlugin.cachedSkeleton2.Type;
				skeleton.NumBones = OVRPlugin.cachedSkeleton2.NumBones;
				skeleton.NumBoneCapsules = OVRPlugin.cachedSkeleton2.NumBoneCapsules;
				if (skeleton.Bones == null || (long)skeleton.Bones.Length != (long)((ulong)skeleton.NumBones))
				{
					skeleton.Bones = new OVRPlugin.Bone[skeleton.NumBones];
				}
				int num6 = 0;
				while ((long)num6 < (long)((ulong)skeleton.NumBones))
				{
					skeleton.Bones[num6] = OVRPlugin.Skeleton2GetBone[num6]();
					num6++;
				}
				skeleton.BoneCapsules[0] = OVRPlugin.cachedSkeleton2.BoneCapsules_0;
				skeleton.BoneCapsules[1] = OVRPlugin.cachedSkeleton2.BoneCapsules_1;
				skeleton.BoneCapsules[2] = OVRPlugin.cachedSkeleton2.BoneCapsules_2;
				skeleton.BoneCapsules[3] = OVRPlugin.cachedSkeleton2.BoneCapsules_3;
				skeleton.BoneCapsules[4] = OVRPlugin.cachedSkeleton2.BoneCapsules_4;
				skeleton.BoneCapsules[5] = OVRPlugin.cachedSkeleton2.BoneCapsules_5;
				skeleton.BoneCapsules[6] = OVRPlugin.cachedSkeleton2.BoneCapsules_6;
				skeleton.BoneCapsules[7] = OVRPlugin.cachedSkeleton2.BoneCapsules_7;
				skeleton.BoneCapsules[8] = OVRPlugin.cachedSkeleton2.BoneCapsules_8;
				skeleton.BoneCapsules[9] = OVRPlugin.cachedSkeleton2.BoneCapsules_9;
				skeleton.BoneCapsules[10] = OVRPlugin.cachedSkeleton2.BoneCapsules_10;
				skeleton.BoneCapsules[11] = OVRPlugin.cachedSkeleton2.BoneCapsules_11;
				skeleton.BoneCapsules[12] = OVRPlugin.cachedSkeleton2.BoneCapsules_12;
				skeleton.BoneCapsules[13] = OVRPlugin.cachedSkeleton2.BoneCapsules_13;
				skeleton.BoneCapsules[14] = OVRPlugin.cachedSkeleton2.BoneCapsules_14;
				skeleton.BoneCapsules[15] = OVRPlugin.cachedSkeleton2.BoneCapsules_15;
				skeleton.BoneCapsules[16] = OVRPlugin.cachedSkeleton2.BoneCapsules_16;
				skeleton.BoneCapsules[17] = OVRPlugin.cachedSkeleton2.BoneCapsules_17;
				skeleton.BoneCapsules[18] = OVRPlugin.cachedSkeleton2.BoneCapsules_18;
				return true;
			}
			return false;
		}
		else
		{
			if (OVRPlugin.GetSkeleton(skeletonType, out OVRPlugin.cachedSkeleton))
			{
				if (skeleton.Bones == null || skeleton.Bones.Length != 84)
				{
					skeleton.Bones = new OVRPlugin.Bone[84];
				}
				if (skeleton.BoneCapsules == null || skeleton.BoneCapsules.Length != 19)
				{
					skeleton.BoneCapsules = new OVRPlugin.BoneCapsule[19];
				}
				skeleton.Type = OVRPlugin.cachedSkeleton.Type;
				skeleton.NumBones = OVRPlugin.cachedSkeleton.NumBones;
				skeleton.NumBoneCapsules = OVRPlugin.cachedSkeleton.NumBoneCapsules;
				int num7 = 0;
				while ((long)num7 < (long)((ulong)skeleton.NumBones))
				{
					skeleton.Bones[num7] = OVRPlugin.cachedSkeleton.Bones[num7];
					num7++;
				}
				int num8 = 0;
				while ((long)num8 < (long)((ulong)skeleton.NumBoneCapsules))
				{
					skeleton.BoneCapsules[num8] = OVRPlugin.cachedSkeleton.BoneCapsules[num8];
					num8++;
				}
				return true;
			}
			return false;
		}
	}

	public static bool bodyTrackingSupported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_GetBodyTrackingSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool bodyTrackingEnabled
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_GetBodyTrackingEnabled(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool GetBodyState(OVRPlugin.Step stepId, ref OVRPlugin.BodyState bodyState)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (OVRPlugin.version < OVRPlugin.OVRP_1_78_0.version)
		{
			return false;
		}
		OVRPlugin.BodyJointLocation[] jointLocations = bodyState.JointLocations;
		if (jointLocations == null || jointLocations.Length != 70)
		{
			bodyState.JointLocations = new OVRPlugin.BodyJointLocation[70];
		}
		OVRPlugin.BodyStateInternal bodyStateInternal;
		if (OVRPlugin.OVRP_1_78_0.ovrp_GetBodyState(stepId, -1, out bodyStateInternal) != OVRPlugin.Result.Success)
		{
			return false;
		}
		if (bodyStateInternal.IsActive != OVRPlugin.Bool.True)
		{
			return false;
		}
		bodyState.Confidence = bodyStateInternal.Confidence;
		bodyState.SkeletonChangedCount = bodyStateInternal.SkeletonChangedCount;
		bodyState.Time = bodyStateInternal.Time;
		bodyState.JointLocations[0] = bodyStateInternal.JointLocation_0;
		bodyState.JointLocations[1] = bodyStateInternal.JointLocation_1;
		bodyState.JointLocations[2] = bodyStateInternal.JointLocation_2;
		bodyState.JointLocations[3] = bodyStateInternal.JointLocation_3;
		bodyState.JointLocations[4] = bodyStateInternal.JointLocation_4;
		bodyState.JointLocations[5] = bodyStateInternal.JointLocation_5;
		bodyState.JointLocations[6] = bodyStateInternal.JointLocation_6;
		bodyState.JointLocations[7] = bodyStateInternal.JointLocation_7;
		bodyState.JointLocations[8] = bodyStateInternal.JointLocation_8;
		bodyState.JointLocations[9] = bodyStateInternal.JointLocation_9;
		bodyState.JointLocations[10] = bodyStateInternal.JointLocation_10;
		bodyState.JointLocations[11] = bodyStateInternal.JointLocation_11;
		bodyState.JointLocations[12] = bodyStateInternal.JointLocation_12;
		bodyState.JointLocations[13] = bodyStateInternal.JointLocation_13;
		bodyState.JointLocations[14] = bodyStateInternal.JointLocation_14;
		bodyState.JointLocations[15] = bodyStateInternal.JointLocation_15;
		bodyState.JointLocations[16] = bodyStateInternal.JointLocation_16;
		bodyState.JointLocations[17] = bodyStateInternal.JointLocation_17;
		bodyState.JointLocations[18] = bodyStateInternal.JointLocation_18;
		bodyState.JointLocations[19] = bodyStateInternal.JointLocation_19;
		bodyState.JointLocations[20] = bodyStateInternal.JointLocation_20;
		bodyState.JointLocations[21] = bodyStateInternal.JointLocation_21;
		bodyState.JointLocations[22] = bodyStateInternal.JointLocation_22;
		bodyState.JointLocations[23] = bodyStateInternal.JointLocation_23;
		bodyState.JointLocations[24] = bodyStateInternal.JointLocation_24;
		bodyState.JointLocations[25] = bodyStateInternal.JointLocation_25;
		bodyState.JointLocations[26] = bodyStateInternal.JointLocation_26;
		bodyState.JointLocations[27] = bodyStateInternal.JointLocation_27;
		bodyState.JointLocations[28] = bodyStateInternal.JointLocation_28;
		bodyState.JointLocations[29] = bodyStateInternal.JointLocation_29;
		bodyState.JointLocations[30] = bodyStateInternal.JointLocation_30;
		bodyState.JointLocations[31] = bodyStateInternal.JointLocation_31;
		bodyState.JointLocations[32] = bodyStateInternal.JointLocation_32;
		bodyState.JointLocations[33] = bodyStateInternal.JointLocation_33;
		bodyState.JointLocations[34] = bodyStateInternal.JointLocation_34;
		bodyState.JointLocations[35] = bodyStateInternal.JointLocation_35;
		bodyState.JointLocations[36] = bodyStateInternal.JointLocation_36;
		bodyState.JointLocations[37] = bodyStateInternal.JointLocation_37;
		bodyState.JointLocations[38] = bodyStateInternal.JointLocation_38;
		bodyState.JointLocations[39] = bodyStateInternal.JointLocation_39;
		bodyState.JointLocations[40] = bodyStateInternal.JointLocation_40;
		bodyState.JointLocations[41] = bodyStateInternal.JointLocation_41;
		bodyState.JointLocations[42] = bodyStateInternal.JointLocation_42;
		bodyState.JointLocations[43] = bodyStateInternal.JointLocation_43;
		bodyState.JointLocations[44] = bodyStateInternal.JointLocation_44;
		bodyState.JointLocations[45] = bodyStateInternal.JointLocation_45;
		bodyState.JointLocations[46] = bodyStateInternal.JointLocation_46;
		bodyState.JointLocations[47] = bodyStateInternal.JointLocation_47;
		bodyState.JointLocations[48] = bodyStateInternal.JointLocation_48;
		bodyState.JointLocations[49] = bodyStateInternal.JointLocation_49;
		bodyState.JointLocations[50] = bodyStateInternal.JointLocation_50;
		bodyState.JointLocations[51] = bodyStateInternal.JointLocation_51;
		bodyState.JointLocations[52] = bodyStateInternal.JointLocation_52;
		bodyState.JointLocations[53] = bodyStateInternal.JointLocation_53;
		bodyState.JointLocations[54] = bodyStateInternal.JointLocation_54;
		bodyState.JointLocations[55] = bodyStateInternal.JointLocation_55;
		bodyState.JointLocations[56] = bodyStateInternal.JointLocation_56;
		bodyState.JointLocations[57] = bodyStateInternal.JointLocation_57;
		bodyState.JointLocations[58] = bodyStateInternal.JointLocation_58;
		bodyState.JointLocations[59] = bodyStateInternal.JointLocation_59;
		bodyState.JointLocations[60] = bodyStateInternal.JointLocation_60;
		bodyState.JointLocations[61] = bodyStateInternal.JointLocation_61;
		bodyState.JointLocations[62] = bodyStateInternal.JointLocation_62;
		bodyState.JointLocations[63] = bodyStateInternal.JointLocation_63;
		bodyState.JointLocations[64] = bodyStateInternal.JointLocation_64;
		bodyState.JointLocations[65] = bodyStateInternal.JointLocation_65;
		bodyState.JointLocations[66] = bodyStateInternal.JointLocation_66;
		bodyState.JointLocations[67] = bodyStateInternal.JointLocation_67;
		bodyState.JointLocations[68] = bodyStateInternal.JointLocation_68;
		bodyState.JointLocations[69] = bodyStateInternal.JointLocation_69;
		return true;
	}

	public static bool GetBodyState4(OVRPlugin.Step stepId, OVRPlugin.BodyJointSet jointSet, ref OVRPlugin.BodyState bodyState)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version))
		{
			return OVRPlugin.GetBodyState(stepId, ref bodyState);
		}
		int num = (jointSet == OVRPlugin.BodyJointSet.FullBody) ? 84 : 70;
		OVRPlugin.BodyJointLocation[] jointLocations = bodyState.JointLocations;
		if (jointLocations == null || jointLocations.Length != num)
		{
			bodyState.JointLocations = new OVRPlugin.BodyJointLocation[num];
		}
		OVRPlugin.BodyState4Internal bodyState4Internal;
		if (OVRPlugin.OVRP_1_92_0.ovrp_GetBodyState4(stepId, -1, out bodyState4Internal) != OVRPlugin.Result.Success || bodyState4Internal.IsActive != OVRPlugin.Bool.True)
		{
			return false;
		}
		bodyState.Confidence = bodyState4Internal.Confidence;
		bodyState.SkeletonChangedCount = bodyState4Internal.SkeletonChangedCount;
		bodyState.Time = bodyState4Internal.Time;
		bodyState.Fidelity = bodyState4Internal.Fidelity;
		bodyState.CalibrationStatus = bodyState4Internal.CalibrationStatus;
		bodyState.JointLocations[0] = bodyState4Internal.JointLocation_0;
		bodyState.JointLocations[1] = bodyState4Internal.JointLocation_1;
		bodyState.JointLocations[2] = bodyState4Internal.JointLocation_2;
		bodyState.JointLocations[3] = bodyState4Internal.JointLocation_3;
		bodyState.JointLocations[4] = bodyState4Internal.JointLocation_4;
		bodyState.JointLocations[5] = bodyState4Internal.JointLocation_5;
		bodyState.JointLocations[6] = bodyState4Internal.JointLocation_6;
		bodyState.JointLocations[7] = bodyState4Internal.JointLocation_7;
		bodyState.JointLocations[8] = bodyState4Internal.JointLocation_8;
		bodyState.JointLocations[9] = bodyState4Internal.JointLocation_9;
		bodyState.JointLocations[10] = bodyState4Internal.JointLocation_10;
		bodyState.JointLocations[11] = bodyState4Internal.JointLocation_11;
		bodyState.JointLocations[12] = bodyState4Internal.JointLocation_12;
		bodyState.JointLocations[13] = bodyState4Internal.JointLocation_13;
		bodyState.JointLocations[14] = bodyState4Internal.JointLocation_14;
		bodyState.JointLocations[15] = bodyState4Internal.JointLocation_15;
		bodyState.JointLocations[16] = bodyState4Internal.JointLocation_16;
		bodyState.JointLocations[17] = bodyState4Internal.JointLocation_17;
		bodyState.JointLocations[18] = bodyState4Internal.JointLocation_18;
		bodyState.JointLocations[19] = bodyState4Internal.JointLocation_19;
		bodyState.JointLocations[20] = bodyState4Internal.JointLocation_20;
		bodyState.JointLocations[21] = bodyState4Internal.JointLocation_21;
		bodyState.JointLocations[22] = bodyState4Internal.JointLocation_22;
		bodyState.JointLocations[23] = bodyState4Internal.JointLocation_23;
		bodyState.JointLocations[24] = bodyState4Internal.JointLocation_24;
		bodyState.JointLocations[25] = bodyState4Internal.JointLocation_25;
		bodyState.JointLocations[26] = bodyState4Internal.JointLocation_26;
		bodyState.JointLocations[27] = bodyState4Internal.JointLocation_27;
		bodyState.JointLocations[28] = bodyState4Internal.JointLocation_28;
		bodyState.JointLocations[29] = bodyState4Internal.JointLocation_29;
		bodyState.JointLocations[30] = bodyState4Internal.JointLocation_30;
		bodyState.JointLocations[31] = bodyState4Internal.JointLocation_31;
		bodyState.JointLocations[32] = bodyState4Internal.JointLocation_32;
		bodyState.JointLocations[33] = bodyState4Internal.JointLocation_33;
		bodyState.JointLocations[34] = bodyState4Internal.JointLocation_34;
		bodyState.JointLocations[35] = bodyState4Internal.JointLocation_35;
		bodyState.JointLocations[36] = bodyState4Internal.JointLocation_36;
		bodyState.JointLocations[37] = bodyState4Internal.JointLocation_37;
		bodyState.JointLocations[38] = bodyState4Internal.JointLocation_38;
		bodyState.JointLocations[39] = bodyState4Internal.JointLocation_39;
		bodyState.JointLocations[40] = bodyState4Internal.JointLocation_40;
		bodyState.JointLocations[41] = bodyState4Internal.JointLocation_41;
		bodyState.JointLocations[42] = bodyState4Internal.JointLocation_42;
		bodyState.JointLocations[43] = bodyState4Internal.JointLocation_43;
		bodyState.JointLocations[44] = bodyState4Internal.JointLocation_44;
		bodyState.JointLocations[45] = bodyState4Internal.JointLocation_45;
		bodyState.JointLocations[46] = bodyState4Internal.JointLocation_46;
		bodyState.JointLocations[47] = bodyState4Internal.JointLocation_47;
		bodyState.JointLocations[48] = bodyState4Internal.JointLocation_48;
		bodyState.JointLocations[49] = bodyState4Internal.JointLocation_49;
		bodyState.JointLocations[50] = bodyState4Internal.JointLocation_50;
		bodyState.JointLocations[51] = bodyState4Internal.JointLocation_51;
		bodyState.JointLocations[52] = bodyState4Internal.JointLocation_52;
		bodyState.JointLocations[53] = bodyState4Internal.JointLocation_53;
		bodyState.JointLocations[54] = bodyState4Internal.JointLocation_54;
		bodyState.JointLocations[55] = bodyState4Internal.JointLocation_55;
		bodyState.JointLocations[56] = bodyState4Internal.JointLocation_56;
		bodyState.JointLocations[57] = bodyState4Internal.JointLocation_57;
		bodyState.JointLocations[58] = bodyState4Internal.JointLocation_58;
		bodyState.JointLocations[59] = bodyState4Internal.JointLocation_59;
		bodyState.JointLocations[60] = bodyState4Internal.JointLocation_60;
		bodyState.JointLocations[61] = bodyState4Internal.JointLocation_61;
		bodyState.JointLocations[62] = bodyState4Internal.JointLocation_62;
		bodyState.JointLocations[63] = bodyState4Internal.JointLocation_63;
		bodyState.JointLocations[64] = bodyState4Internal.JointLocation_64;
		bodyState.JointLocations[65] = bodyState4Internal.JointLocation_65;
		bodyState.JointLocations[66] = bodyState4Internal.JointLocation_66;
		bodyState.JointLocations[67] = bodyState4Internal.JointLocation_67;
		bodyState.JointLocations[68] = bodyState4Internal.JointLocation_68;
		bodyState.JointLocations[69] = bodyState4Internal.JointLocation_69;
		if (jointSet == OVRPlugin.BodyJointSet.FullBody)
		{
			bodyState.JointLocations[70] = bodyState4Internal.JointLocation_70;
			bodyState.JointLocations[71] = bodyState4Internal.JointLocation_71;
			bodyState.JointLocations[72] = bodyState4Internal.JointLocation_72;
			bodyState.JointLocations[73] = bodyState4Internal.JointLocation_73;
			bodyState.JointLocations[74] = bodyState4Internal.JointLocation_74;
			bodyState.JointLocations[75] = bodyState4Internal.JointLocation_75;
			bodyState.JointLocations[76] = bodyState4Internal.JointLocation_76;
			bodyState.JointLocations[77] = bodyState4Internal.JointLocation_77;
			bodyState.JointLocations[78] = bodyState4Internal.JointLocation_78;
			bodyState.JointLocations[79] = bodyState4Internal.JointLocation_79;
			bodyState.JointLocations[80] = bodyState4Internal.JointLocation_80;
			bodyState.JointLocations[81] = bodyState4Internal.JointLocation_81;
			bodyState.JointLocations[82] = bodyState4Internal.JointLocation_82;
			bodyState.JointLocations[83] = bodyState4Internal.JointLocation_83;
		}
		return true;
	}

	public static bool GetMesh(OVRPlugin.MeshType meshType, out OVRPlugin.Mesh mesh)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version)
		{
			mesh = new OVRPlugin.Mesh();
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf<OVRPlugin.Mesh>(mesh));
			OVRPlugin.Result result = OVRPlugin.OVRP_1_44_0.ovrp_GetMesh(meshType, intPtr);
			if (result == OVRPlugin.Result.Success)
			{
				Marshal.PtrToStructure<OVRPlugin.Mesh>(intPtr, mesh);
			}
			Marshal.FreeHGlobal(intPtr);
			return result == OVRPlugin.Result.Success;
		}
		mesh = new OVRPlugin.Mesh();
		return false;
	}

	public static OVRPlugin.Result CreateVirtualKeyboard(OVRPlugin.VirtualKeyboardCreateInfo createInfo)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_CreateVirtualKeyboard(createInfo);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result DestroyVirtualKeyboard()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_DestroyVirtualKeyboard();
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result SendVirtualKeyboardInput(OVRPlugin.VirtualKeyboardInputInfo inputInfo, ref OVRPlugin.Posef interactorRootPose)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_SendVirtualKeyboardInput(inputInfo, ref interactorRootPose);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result ChangeVirtualKeyboardTextContext(string textContext)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_ChangeVirtualKeyboardTextContext(textContext);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result CreateVirtualKeyboardSpace(OVRPlugin.VirtualKeyboardSpaceCreateInfo createInfo, out ulong keyboardSpace)
	{
		keyboardSpace = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_CreateVirtualKeyboardSpace(createInfo, out keyboardSpace);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result SuggestVirtualKeyboardLocation(OVRPlugin.VirtualKeyboardLocationInfo locationInfo)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_SuggestVirtualKeyboardLocation(locationInfo);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result GetVirtualKeyboardScale(out float scale)
	{
		scale = 0f;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			return OVRPlugin.OVRP_1_74_0.ovrp_GetVirtualKeyboardScale(out scale);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result GetVirtualKeyboardModelAnimationStates(OVRPlugin.VirtualKeyboardModelAnimationStateBufferProvider bufferProvider, OVRPlugin.VirtualKeyboardModelAnimationStateHandler stateHandler)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_83_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		OVRPlugin.VirtualKeyboardModelAnimationStatesInternal virtualKeyboardModelAnimationStatesInternal = new OVRPlugin.VirtualKeyboardModelAnimationStatesInternal
		{
			StateCapacityInput = 0U
		};
		OVRPlugin.Result result = OVRPlugin.OVRP_1_83_0.ovrp_GetVirtualKeyboardModelAnimationStates(ref virtualKeyboardModelAnimationStatesInternal);
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogError("GetVirtualKeyboardModelAnimationStates failed: cannot query animation state data:" + result.ToString());
		}
		if (virtualKeyboardModelAnimationStatesInternal.StateCountOutput == 0U || result != OVRPlugin.Result.Success)
		{
			return result;
		}
		int num = Marshal.SizeOf(typeof(OVRPlugin.VirtualKeyboardModelAnimationState));
		virtualKeyboardModelAnimationStatesInternal.StatesBuffer = bufferProvider((int)(virtualKeyboardModelAnimationStatesInternal.StateCountOutput * (uint)num), (int)virtualKeyboardModelAnimationStatesInternal.StateCountOutput);
		virtualKeyboardModelAnimationStatesInternal.StateCapacityInput = virtualKeyboardModelAnimationStatesInternal.StateCountOutput;
		result = OVRPlugin.OVRP_1_83_0.ovrp_GetVirtualKeyboardModelAnimationStates(ref virtualKeyboardModelAnimationStatesInternal);
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogError("GetVirtualKeyboardModelAnimationStates failed: cannot populate animation state data:" + result.ToString());
		}
		else
		{
			int[] array = new int[1];
			float[] array2 = new float[1];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)virtualKeyboardModelAnimationStatesInternal.StateCountOutput))
			{
				IntPtr intPtr = IntPtr.Add(virtualKeyboardModelAnimationStatesInternal.StatesBuffer, num2 * num);
				Marshal.Copy(intPtr, array, 0, 1);
				Marshal.Copy(IntPtr.Add(intPtr, 4), array2, 0, 1);
				OVRPlugin.VirtualKeyboardModelAnimationState virtualKeyboardModelAnimationState;
				virtualKeyboardModelAnimationState.AnimationIndex = array[0];
				virtualKeyboardModelAnimationState.Fraction = array2[0];
				stateHandler(ref virtualKeyboardModelAnimationState);
				num2++;
			}
		}
		return result;
	}

	[Obsolete("Use GetVirtualKeyboardModelAnimationStates with delegates")]
	public static OVRPlugin.Result GetVirtualKeyboardModelAnimationStates(out OVRPlugin.VirtualKeyboardModelAnimationStates animationStates)
	{
		animationStates = default(OVRPlugin.VirtualKeyboardModelAnimationStates);
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_83_0.version)
		{
			Marshal.SizeOf(typeof(OVRPlugin.VirtualKeyboardModelAnimationState));
			OVRPlugin.VirtualKeyboardModelAnimationState[] states = Array.Empty<OVRPlugin.VirtualKeyboardModelAnimationState>();
			int i = 0;
			IntPtr buffer = IntPtr.Zero;
			try
			{
				OVRPlugin.Result virtualKeyboardModelAnimationStates = OVRPlugin.GetVirtualKeyboardModelAnimationStates(delegate(int bufferSize, int stateCount)
				{
					buffer = Marshal.AllocHGlobal(bufferSize);
					states = new OVRPlugin.VirtualKeyboardModelAnimationState[stateCount];
					return buffer;
				}, delegate(ref OVRPlugin.VirtualKeyboardModelAnimationState state)
				{
					OVRPlugin.VirtualKeyboardModelAnimationState[] states = states;
					int i = i;
					i++;
					states[i] = state;
				});
				animationStates.States = states;
				return virtualKeyboardModelAnimationStates;
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result GetVirtualKeyboardDirtyTextures(out OVRPlugin.VirtualKeyboardTextureIds textureIds)
	{
		textureIds = default(OVRPlugin.VirtualKeyboardTextureIds);
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_83_0.version)
		{
			OVRPlugin.VirtualKeyboardTextureIdsInternal virtualKeyboardTextureIdsInternal = default(OVRPlugin.VirtualKeyboardTextureIdsInternal);
			OVRPlugin.Result result = OVRPlugin.OVRP_1_83_0.ovrp_GetVirtualKeyboardDirtyTextures(ref virtualKeyboardTextureIdsInternal);
			textureIds.TextureIds = new ulong[virtualKeyboardTextureIdsInternal.TextureIdCountOutput];
			if (virtualKeyboardTextureIdsInternal.TextureIdCountOutput == 0U)
			{
				if (result != OVRPlugin.Result.Success)
				{
					Debug.LogError("GetVirtualKeyboardDirtyTextures failed: cannot query dirty textures data:" + result.ToString());
				}
				return result;
			}
			GCHandle gchandle = GCHandle.Alloc(textureIds.TextureIds, GCHandleType.Pinned);
			try
			{
				virtualKeyboardTextureIdsInternal.TextureIdCapacityInput = virtualKeyboardTextureIdsInternal.TextureIdCountOutput;
				virtualKeyboardTextureIdsInternal.TextureIdsBuffer = gchandle.AddrOfPinnedObject();
				result = OVRPlugin.OVRP_1_83_0.ovrp_GetVirtualKeyboardDirtyTextures(ref virtualKeyboardTextureIdsInternal);
				if (result != OVRPlugin.Result.Success)
				{
					Debug.LogError("GetVirtualKeyboardDirtyTextures failed: cannot populate dirty textures data:" + result.ToString());
				}
				return result;
			}
			finally
			{
				gchandle.Free();
			}
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result GetVirtualKeyboardTextureData(ulong textureId, ref OVRPlugin.VirtualKeyboardTextureData textureData)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_83_0.version)
		{
			return OVRPlugin.OVRP_1_83_0.ovrp_GetVirtualKeyboardTextureData(textureId, ref textureData);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static OVRPlugin.Result SetVirtualKeyboardModelVisibility(ref OVRPlugin.VirtualKeyboardModelVisibility visibility)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_83_0.version)
		{
			return OVRPlugin.OVRP_1_83_0.ovrp_SetVirtualKeyboardModelVisibility(ref visibility);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static bool faceTrackingEnabled
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_GetFaceTrackingEnabled(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool faceTrackingSupported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_GetFaceTrackingSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	private static bool GetFaceStateInternal(OVRPlugin.Step stepId, int frameIndex, ref OVRPlugin.FaceState faceState)
	{
		if (OVRPlugin.OVRP_1_78_0.ovrp_GetFaceState(stepId, frameIndex, out OVRPlugin.cachedFaceState) != OVRPlugin.Result.Success)
		{
			return false;
		}
		if (faceState.ExpressionWeights == null || faceState.ExpressionWeights.Length != 63)
		{
			faceState.ExpressionWeights = new float[63];
		}
		if (faceState.ExpressionWeightConfidences == null || faceState.ExpressionWeightConfidences.Length != 2)
		{
			faceState.ExpressionWeightConfidences = new float[2];
		}
		faceState.ExpressionWeights[0] = OVRPlugin.cachedFaceState.ExpressionWeights_0;
		faceState.ExpressionWeights[1] = OVRPlugin.cachedFaceState.ExpressionWeights_1;
		faceState.ExpressionWeights[2] = OVRPlugin.cachedFaceState.ExpressionWeights_2;
		faceState.ExpressionWeights[3] = OVRPlugin.cachedFaceState.ExpressionWeights_3;
		faceState.ExpressionWeights[4] = OVRPlugin.cachedFaceState.ExpressionWeights_4;
		faceState.ExpressionWeights[5] = OVRPlugin.cachedFaceState.ExpressionWeights_5;
		faceState.ExpressionWeights[6] = OVRPlugin.cachedFaceState.ExpressionWeights_6;
		faceState.ExpressionWeights[7] = OVRPlugin.cachedFaceState.ExpressionWeights_7;
		faceState.ExpressionWeights[8] = OVRPlugin.cachedFaceState.ExpressionWeights_8;
		faceState.ExpressionWeights[9] = OVRPlugin.cachedFaceState.ExpressionWeights_9;
		faceState.ExpressionWeights[10] = OVRPlugin.cachedFaceState.ExpressionWeights_10;
		faceState.ExpressionWeights[11] = OVRPlugin.cachedFaceState.ExpressionWeights_11;
		faceState.ExpressionWeights[12] = OVRPlugin.cachedFaceState.ExpressionWeights_12;
		faceState.ExpressionWeights[13] = OVRPlugin.cachedFaceState.ExpressionWeights_13;
		faceState.ExpressionWeights[14] = OVRPlugin.cachedFaceState.ExpressionWeights_14;
		faceState.ExpressionWeights[15] = OVRPlugin.cachedFaceState.ExpressionWeights_15;
		faceState.ExpressionWeights[16] = OVRPlugin.cachedFaceState.ExpressionWeights_16;
		faceState.ExpressionWeights[17] = OVRPlugin.cachedFaceState.ExpressionWeights_17;
		faceState.ExpressionWeights[18] = OVRPlugin.cachedFaceState.ExpressionWeights_18;
		faceState.ExpressionWeights[19] = OVRPlugin.cachedFaceState.ExpressionWeights_19;
		faceState.ExpressionWeights[20] = OVRPlugin.cachedFaceState.ExpressionWeights_20;
		faceState.ExpressionWeights[21] = OVRPlugin.cachedFaceState.ExpressionWeights_21;
		faceState.ExpressionWeights[22] = OVRPlugin.cachedFaceState.ExpressionWeights_22;
		faceState.ExpressionWeights[23] = OVRPlugin.cachedFaceState.ExpressionWeights_23;
		faceState.ExpressionWeights[24] = OVRPlugin.cachedFaceState.ExpressionWeights_24;
		faceState.ExpressionWeights[25] = OVRPlugin.cachedFaceState.ExpressionWeights_25;
		faceState.ExpressionWeights[26] = OVRPlugin.cachedFaceState.ExpressionWeights_26;
		faceState.ExpressionWeights[27] = OVRPlugin.cachedFaceState.ExpressionWeights_27;
		faceState.ExpressionWeights[28] = OVRPlugin.cachedFaceState.ExpressionWeights_28;
		faceState.ExpressionWeights[29] = OVRPlugin.cachedFaceState.ExpressionWeights_29;
		faceState.ExpressionWeights[30] = OVRPlugin.cachedFaceState.ExpressionWeights_30;
		faceState.ExpressionWeights[31] = OVRPlugin.cachedFaceState.ExpressionWeights_31;
		faceState.ExpressionWeights[32] = OVRPlugin.cachedFaceState.ExpressionWeights_32;
		faceState.ExpressionWeights[33] = OVRPlugin.cachedFaceState.ExpressionWeights_33;
		faceState.ExpressionWeights[34] = OVRPlugin.cachedFaceState.ExpressionWeights_34;
		faceState.ExpressionWeights[35] = OVRPlugin.cachedFaceState.ExpressionWeights_35;
		faceState.ExpressionWeights[36] = OVRPlugin.cachedFaceState.ExpressionWeights_36;
		faceState.ExpressionWeights[37] = OVRPlugin.cachedFaceState.ExpressionWeights_37;
		faceState.ExpressionWeights[38] = OVRPlugin.cachedFaceState.ExpressionWeights_38;
		faceState.ExpressionWeights[39] = OVRPlugin.cachedFaceState.ExpressionWeights_39;
		faceState.ExpressionWeights[40] = OVRPlugin.cachedFaceState.ExpressionWeights_40;
		faceState.ExpressionWeights[41] = OVRPlugin.cachedFaceState.ExpressionWeights_41;
		faceState.ExpressionWeights[42] = OVRPlugin.cachedFaceState.ExpressionWeights_42;
		faceState.ExpressionWeights[43] = OVRPlugin.cachedFaceState.ExpressionWeights_43;
		faceState.ExpressionWeights[44] = OVRPlugin.cachedFaceState.ExpressionWeights_44;
		faceState.ExpressionWeights[45] = OVRPlugin.cachedFaceState.ExpressionWeights_45;
		faceState.ExpressionWeights[46] = OVRPlugin.cachedFaceState.ExpressionWeights_46;
		faceState.ExpressionWeights[47] = OVRPlugin.cachedFaceState.ExpressionWeights_47;
		faceState.ExpressionWeights[48] = OVRPlugin.cachedFaceState.ExpressionWeights_48;
		faceState.ExpressionWeights[49] = OVRPlugin.cachedFaceState.ExpressionWeights_49;
		faceState.ExpressionWeights[50] = OVRPlugin.cachedFaceState.ExpressionWeights_50;
		faceState.ExpressionWeights[51] = OVRPlugin.cachedFaceState.ExpressionWeights_51;
		faceState.ExpressionWeights[52] = OVRPlugin.cachedFaceState.ExpressionWeights_52;
		faceState.ExpressionWeights[53] = OVRPlugin.cachedFaceState.ExpressionWeights_53;
		faceState.ExpressionWeights[54] = OVRPlugin.cachedFaceState.ExpressionWeights_54;
		faceState.ExpressionWeights[55] = OVRPlugin.cachedFaceState.ExpressionWeights_55;
		faceState.ExpressionWeights[56] = OVRPlugin.cachedFaceState.ExpressionWeights_56;
		faceState.ExpressionWeights[57] = OVRPlugin.cachedFaceState.ExpressionWeights_57;
		faceState.ExpressionWeights[58] = OVRPlugin.cachedFaceState.ExpressionWeights_58;
		faceState.ExpressionWeights[59] = OVRPlugin.cachedFaceState.ExpressionWeights_59;
		faceState.ExpressionWeights[60] = OVRPlugin.cachedFaceState.ExpressionWeights_60;
		faceState.ExpressionWeights[61] = OVRPlugin.cachedFaceState.ExpressionWeights_61;
		faceState.ExpressionWeights[62] = OVRPlugin.cachedFaceState.ExpressionWeights_62;
		faceState.ExpressionWeightConfidences[0] = OVRPlugin.cachedFaceState.ExpressionWeightConfidences_0;
		faceState.ExpressionWeightConfidences[1] = OVRPlugin.cachedFaceState.ExpressionWeightConfidences_1;
		faceState.Status = OVRPlugin.cachedFaceState.Status.ToFaceExpressionStatus();
		faceState.Time = OVRPlugin.cachedFaceState.Time;
		return true;
	}

	public static bool GetFaceState(OVRPlugin.Step stepId, int frameIndex, ref OVRPlugin.FaceState faceState)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			stepId = OVRPlugin.Step.Render;
		}
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.GetFaceStateInternal(stepId, frameIndex, ref faceState);
	}

	public static bool GetFaceState2(OVRPlugin.Step stepId, int frameIndex, ref OVRPlugin.FaceState faceState)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version))
		{
			return false;
		}
		if (OVRPlugin.OVRP_1_92_0.ovrp_GetFaceState2(stepId, frameIndex, out OVRPlugin.cachedFaceState2) != OVRPlugin.Result.Success)
		{
			return false;
		}
		if (faceState.ExpressionWeights == null || faceState.ExpressionWeights.Length != 70)
		{
			faceState.ExpressionWeights = new float[70];
		}
		if (faceState.ExpressionWeightConfidences == null || faceState.ExpressionWeightConfidences.Length != 2)
		{
			faceState.ExpressionWeightConfidences = new float[2];
		}
		faceState.ExpressionWeights[0] = OVRPlugin.cachedFaceState2.ExpressionWeights_0;
		faceState.ExpressionWeights[1] = OVRPlugin.cachedFaceState2.ExpressionWeights_1;
		faceState.ExpressionWeights[2] = OVRPlugin.cachedFaceState2.ExpressionWeights_2;
		faceState.ExpressionWeights[3] = OVRPlugin.cachedFaceState2.ExpressionWeights_3;
		faceState.ExpressionWeights[4] = OVRPlugin.cachedFaceState2.ExpressionWeights_4;
		faceState.ExpressionWeights[5] = OVRPlugin.cachedFaceState2.ExpressionWeights_5;
		faceState.ExpressionWeights[6] = OVRPlugin.cachedFaceState2.ExpressionWeights_6;
		faceState.ExpressionWeights[7] = OVRPlugin.cachedFaceState2.ExpressionWeights_7;
		faceState.ExpressionWeights[8] = OVRPlugin.cachedFaceState2.ExpressionWeights_8;
		faceState.ExpressionWeights[9] = OVRPlugin.cachedFaceState2.ExpressionWeights_9;
		faceState.ExpressionWeights[10] = OVRPlugin.cachedFaceState2.ExpressionWeights_10;
		faceState.ExpressionWeights[11] = OVRPlugin.cachedFaceState2.ExpressionWeights_11;
		faceState.ExpressionWeights[12] = OVRPlugin.cachedFaceState2.ExpressionWeights_12;
		faceState.ExpressionWeights[13] = OVRPlugin.cachedFaceState2.ExpressionWeights_13;
		faceState.ExpressionWeights[14] = OVRPlugin.cachedFaceState2.ExpressionWeights_14;
		faceState.ExpressionWeights[15] = OVRPlugin.cachedFaceState2.ExpressionWeights_15;
		faceState.ExpressionWeights[16] = OVRPlugin.cachedFaceState2.ExpressionWeights_16;
		faceState.ExpressionWeights[17] = OVRPlugin.cachedFaceState2.ExpressionWeights_17;
		faceState.ExpressionWeights[18] = OVRPlugin.cachedFaceState2.ExpressionWeights_18;
		faceState.ExpressionWeights[19] = OVRPlugin.cachedFaceState2.ExpressionWeights_19;
		faceState.ExpressionWeights[20] = OVRPlugin.cachedFaceState2.ExpressionWeights_20;
		faceState.ExpressionWeights[21] = OVRPlugin.cachedFaceState2.ExpressionWeights_21;
		faceState.ExpressionWeights[22] = OVRPlugin.cachedFaceState2.ExpressionWeights_22;
		faceState.ExpressionWeights[23] = OVRPlugin.cachedFaceState2.ExpressionWeights_23;
		faceState.ExpressionWeights[24] = OVRPlugin.cachedFaceState2.ExpressionWeights_24;
		faceState.ExpressionWeights[25] = OVRPlugin.cachedFaceState2.ExpressionWeights_25;
		faceState.ExpressionWeights[26] = OVRPlugin.cachedFaceState2.ExpressionWeights_26;
		faceState.ExpressionWeights[27] = OVRPlugin.cachedFaceState2.ExpressionWeights_27;
		faceState.ExpressionWeights[28] = OVRPlugin.cachedFaceState2.ExpressionWeights_28;
		faceState.ExpressionWeights[29] = OVRPlugin.cachedFaceState2.ExpressionWeights_29;
		faceState.ExpressionWeights[30] = OVRPlugin.cachedFaceState2.ExpressionWeights_30;
		faceState.ExpressionWeights[31] = OVRPlugin.cachedFaceState2.ExpressionWeights_31;
		faceState.ExpressionWeights[32] = OVRPlugin.cachedFaceState2.ExpressionWeights_32;
		faceState.ExpressionWeights[33] = OVRPlugin.cachedFaceState2.ExpressionWeights_33;
		faceState.ExpressionWeights[34] = OVRPlugin.cachedFaceState2.ExpressionWeights_34;
		faceState.ExpressionWeights[35] = OVRPlugin.cachedFaceState2.ExpressionWeights_35;
		faceState.ExpressionWeights[36] = OVRPlugin.cachedFaceState2.ExpressionWeights_36;
		faceState.ExpressionWeights[37] = OVRPlugin.cachedFaceState2.ExpressionWeights_37;
		faceState.ExpressionWeights[38] = OVRPlugin.cachedFaceState2.ExpressionWeights_38;
		faceState.ExpressionWeights[39] = OVRPlugin.cachedFaceState2.ExpressionWeights_39;
		faceState.ExpressionWeights[40] = OVRPlugin.cachedFaceState2.ExpressionWeights_40;
		faceState.ExpressionWeights[41] = OVRPlugin.cachedFaceState2.ExpressionWeights_41;
		faceState.ExpressionWeights[42] = OVRPlugin.cachedFaceState2.ExpressionWeights_42;
		faceState.ExpressionWeights[43] = OVRPlugin.cachedFaceState2.ExpressionWeights_43;
		faceState.ExpressionWeights[44] = OVRPlugin.cachedFaceState2.ExpressionWeights_44;
		faceState.ExpressionWeights[45] = OVRPlugin.cachedFaceState2.ExpressionWeights_45;
		faceState.ExpressionWeights[46] = OVRPlugin.cachedFaceState2.ExpressionWeights_46;
		faceState.ExpressionWeights[47] = OVRPlugin.cachedFaceState2.ExpressionWeights_47;
		faceState.ExpressionWeights[48] = OVRPlugin.cachedFaceState2.ExpressionWeights_48;
		faceState.ExpressionWeights[49] = OVRPlugin.cachedFaceState2.ExpressionWeights_49;
		faceState.ExpressionWeights[50] = OVRPlugin.cachedFaceState2.ExpressionWeights_50;
		faceState.ExpressionWeights[51] = OVRPlugin.cachedFaceState2.ExpressionWeights_51;
		faceState.ExpressionWeights[52] = OVRPlugin.cachedFaceState2.ExpressionWeights_52;
		faceState.ExpressionWeights[53] = OVRPlugin.cachedFaceState2.ExpressionWeights_53;
		faceState.ExpressionWeights[54] = OVRPlugin.cachedFaceState2.ExpressionWeights_54;
		faceState.ExpressionWeights[55] = OVRPlugin.cachedFaceState2.ExpressionWeights_55;
		faceState.ExpressionWeights[56] = OVRPlugin.cachedFaceState2.ExpressionWeights_56;
		faceState.ExpressionWeights[57] = OVRPlugin.cachedFaceState2.ExpressionWeights_57;
		faceState.ExpressionWeights[58] = OVRPlugin.cachedFaceState2.ExpressionWeights_58;
		faceState.ExpressionWeights[59] = OVRPlugin.cachedFaceState2.ExpressionWeights_59;
		faceState.ExpressionWeights[60] = OVRPlugin.cachedFaceState2.ExpressionWeights_60;
		faceState.ExpressionWeights[61] = OVRPlugin.cachedFaceState2.ExpressionWeights_61;
		faceState.ExpressionWeights[62] = OVRPlugin.cachedFaceState2.ExpressionWeights_62;
		faceState.ExpressionWeights[63] = OVRPlugin.cachedFaceState2.ExpressionWeights_63;
		faceState.ExpressionWeights[64] = OVRPlugin.cachedFaceState2.ExpressionWeights_64;
		faceState.ExpressionWeights[65] = OVRPlugin.cachedFaceState2.ExpressionWeights_65;
		faceState.ExpressionWeights[66] = OVRPlugin.cachedFaceState2.ExpressionWeights_66;
		faceState.ExpressionWeights[67] = OVRPlugin.cachedFaceState2.ExpressionWeights_67;
		faceState.ExpressionWeights[68] = OVRPlugin.cachedFaceState2.ExpressionWeights_68;
		faceState.ExpressionWeights[69] = OVRPlugin.cachedFaceState2.ExpressionWeights_69;
		faceState.ExpressionWeightConfidences[0] = OVRPlugin.cachedFaceState2.ExpressionWeightConfidences_0;
		faceState.ExpressionWeightConfidences[1] = OVRPlugin.cachedFaceState2.ExpressionWeightConfidences_1;
		faceState.Status = OVRPlugin.cachedFaceState2.Status.ToFaceExpressionStatus();
		faceState.Time = OVRPlugin.cachedFaceState2.Time;
		faceState.DataSource = OVRPlugin.cachedFaceState2.DataSource;
		return true;
	}

	public static OVRPlugin.Result GetFaceVisemesState(OVRPlugin.Step stepId, ref OVRPlugin.FaceVisemesState faceVisemesState)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_104_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		OVRPlugin.Result result = OVRPlugin.OVRP_1_104_0.ovrp_GetFaceVisemesState(stepId, -1, out OVRPlugin.cachedFaceVisemesState);
		if (result != OVRPlugin.Result.Success)
		{
			return result;
		}
		if (faceVisemesState.Visemes == null || faceVisemesState.Visemes.Length != 15)
		{
			faceVisemesState.Visemes = new float[15];
		}
		faceVisemesState.Visemes[0] = OVRPlugin.cachedFaceVisemesState.Visemes_0;
		faceVisemesState.Visemes[1] = OVRPlugin.cachedFaceVisemesState.Visemes_1;
		faceVisemesState.Visemes[2] = OVRPlugin.cachedFaceVisemesState.Visemes_2;
		faceVisemesState.Visemes[3] = OVRPlugin.cachedFaceVisemesState.Visemes_3;
		faceVisemesState.Visemes[4] = OVRPlugin.cachedFaceVisemesState.Visemes_4;
		faceVisemesState.Visemes[5] = OVRPlugin.cachedFaceVisemesState.Visemes_5;
		faceVisemesState.Visemes[6] = OVRPlugin.cachedFaceVisemesState.Visemes_6;
		faceVisemesState.Visemes[7] = OVRPlugin.cachedFaceVisemesState.Visemes_7;
		faceVisemesState.Visemes[8] = OVRPlugin.cachedFaceVisemesState.Visemes_8;
		faceVisemesState.Visemes[9] = OVRPlugin.cachedFaceVisemesState.Visemes_9;
		faceVisemesState.Visemes[10] = OVRPlugin.cachedFaceVisemesState.Visemes_10;
		faceVisemesState.Visemes[11] = OVRPlugin.cachedFaceVisemesState.Visemes_11;
		faceVisemesState.Visemes[12] = OVRPlugin.cachedFaceVisemesState.Visemes_12;
		faceVisemesState.Visemes[13] = OVRPlugin.cachedFaceVisemesState.Visemes_13;
		faceVisemesState.Visemes[14] = OVRPlugin.cachedFaceVisemesState.Visemes_14;
		faceVisemesState.IsValid = (OVRPlugin.cachedFaceVisemesState.IsValid == OVRPlugin.Bool.True);
		faceVisemesState.Time = OVRPlugin.cachedFaceVisemesState.Time;
		return OVRPlugin.Result.Success;
	}

	public static OVRPlugin.Result SetFaceTrackingVisemesEnabled(bool enabled)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.OVRP_1_104_0.ovrp_SetFaceTrackingVisemesEnabled(enabled ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static bool eyeTrackingEnabled
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_GetEyeTrackingEnabled(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool eyeTrackingSupported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_GetEyeTrackingSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool GetEyeGazesState(OVRPlugin.Step stepId, int frameIndex, ref OVRPlugin.EyeGazesState eyeGazesState)
	{
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR && stepId == OVRPlugin.Step.Physics)
		{
			Debug.LogWarning("Step.Physics is deprecated when using OpenXR");
			stepId = OVRPlugin.Step.Render;
		}
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version))
		{
			return false;
		}
		if (OVRPlugin.OVRP_1_78_0.ovrp_GetEyeGazesState(stepId, frameIndex, out OVRPlugin.cachedEyeGazesState) == OVRPlugin.Result.Success)
		{
			if (eyeGazesState.EyeGazes == null || eyeGazesState.EyeGazes.Length != 2)
			{
				eyeGazesState.EyeGazes = new OVRPlugin.EyeGazeState[2];
			}
			eyeGazesState.EyeGazes[0] = OVRPlugin.cachedEyeGazesState.EyeGazes_0;
			eyeGazesState.EyeGazes[1] = OVRPlugin.cachedEyeGazesState.EyeGazes_1;
			eyeGazesState.Time = OVRPlugin.cachedEyeGazesState.Time;
			return true;
		}
		return false;
	}

	public static bool StartEyeTracking()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_StartEyeTracking() == OVRPlugin.Result.Success;
	}

	public static bool StopEyeTracking()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_StopEyeTracking() == OVRPlugin.Result.Success;
	}

	public static bool StartFaceTracking()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_StartFaceTracking() == OVRPlugin.Result.Success;
	}

	public static bool StopFaceTracking()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_StopFaceTracking() == OVRPlugin.Result.Success;
	}

	public static bool faceTracking2Enabled
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_GetFaceTracking2Enabled(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool faceTracking2Supported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_GetFaceTracking2Supported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool faceTrackingVisemesSupported
	{
		get
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_104_0.version && OVRPlugin.OVRP_1_104_0.ovrp_GetFaceTrackingVisemesSupported(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
	}

	public static bool StartFaceTracking2(OVRPlugin.FaceTrackingDataSource[] requestedFaceTrackingDataSources)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_StartFaceTracking2(requestedFaceTrackingDataSources, (uint)((requestedFaceTrackingDataSources != null) ? requestedFaceTrackingDataSources.Length : 0)) == OVRPlugin.Result.Success;
	}

	public static bool StopFaceTracking2()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_StopFaceTracking2() == OVRPlugin.Result.Success;
	}

	public static bool StartBodyTracking2(OVRPlugin.BodyJointSet jointSet)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version)
		{
			if (OVRPlugin._currentJointSet != OVRPlugin.BodyJointSet.None)
			{
				return true;
			}
			if (OVRPlugin.OVRP_1_92_0.ovrp_StartBodyTracking2(jointSet) == OVRPlugin.Result.Success)
			{
				OVRPlugin._currentJointSet = jointSet;
				return true;
			}
			return false;
		}
		else
		{
			if (jointSet != OVRPlugin.BodyJointSet.FullBody)
			{
				return OVRPlugin.StartBodyTracking();
			}
			Debug.LogError("Full body joint set is not supported by this version of OVRPlugin.");
			return false;
		}
	}

	public static bool StartBodyTracking()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_StartBodyTracking() == OVRPlugin.Result.Success;
	}

	public static bool RequestBodyTrackingFidelity(OVRPlugin.BodyTrackingFidelity2 fidelity)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_RequestBodyTrackingFidelity(fidelity) == OVRPlugin.Result.Success;
	}

	public static bool SuggestBodyTrackingCalibrationOverride(OVRPlugin.BodyTrackingCalibrationInfo calibrationInfo)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_SuggestBodyTrackingCalibrationOverride(calibrationInfo) == OVRPlugin.Result.Success;
	}

	public static bool ResetBodyTrackingCalibration()
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version && OVRPlugin.OVRP_1_92_0.ovrp_ResetBodyTrackingCalibration() == OVRPlugin.Result.Success;
	}

	public static bool StopBodyTracking()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_92_0.version)
		{
			OVRPlugin._currentJointSet = OVRPlugin.BodyJointSet.None;
		}
		return OVRPlugin.version >= OVRPlugin.OVRP_1_78_0.version && OVRPlugin.OVRP_1_78_0.ovrp_StopBodyTracking() == OVRPlugin.Result.Success;
	}

	public static int GetLocalTrackingSpaceRecenterCount()
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_44_0.version))
		{
			return 0;
		}
		int result = 0;
		if (OVRPlugin.OVRP_1_44_0.ovrp_GetLocalTrackingSpaceRecenterCount(ref result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0;
	}

	public static bool GetSystemHmd3DofModeEnabled()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_45_0.version)
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			return OVRPlugin.OVRP_1_45_0.ovrp_GetSystemHmd3DofModeEnabled(ref @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}
		return false;
	}

	public static bool SetClientColorDesc(OVRPlugin.ColorSpace colorSpace)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_49_0.version)
		{
			if (colorSpace == OVRPlugin.ColorSpace.Unknown)
			{
				Debug.LogWarning("A color gamut of Unknown is not supported. Defaulting to DCI-P3 color space instead.");
				colorSpace = OVRPlugin.ColorSpace.P3;
			}
			return OVRPlugin.OVRP_1_49_0.ovrp_SetClientColorDesc(colorSpace) == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static OVRPlugin.ColorSpace GetHmdColorDesc()
	{
		OVRPlugin.ColorSpace result = OVRPlugin.ColorSpace.Unknown;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_49_0.version)
		{
			if (OVRPlugin.OVRP_1_49_0.ovrp_GetHmdColorDesc(ref result) != OVRPlugin.Result.Success)
			{
				Debug.LogError("GetHmdColorDesc: Failed to get Hmd color description");
			}
			return result;
		}
		Debug.LogError("GetHmdColorDesc: Not supported on this version of OVRPlugin");
		return result;
	}

	public static bool PollEvent(ref OVRPlugin.EventDataBuffer eventDataBuffer)
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_55_1.version)
		{
			IntPtr zero = IntPtr.Zero;
			if (eventDataBuffer.EventData == null)
			{
				eventDataBuffer.EventData = new byte[4000];
			}
			if (OVRPlugin.OVRP_1_55_1.ovrp_PollEvent2(ref eventDataBuffer.EventType, ref zero) != OVRPlugin.Result.Success || zero == IntPtr.Zero)
			{
				return false;
			}
			Marshal.Copy(zero, eventDataBuffer.EventData, 0, 4000);
			return true;
		}
		else
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_55_0.version)
			{
				return OVRPlugin.OVRP_1_55_0.ovrp_PollEvent(ref eventDataBuffer) == OVRPlugin.Result.Success;
			}
			eventDataBuffer = default(OVRPlugin.EventDataBuffer);
			return false;
		}
	}

	public static ulong GetNativeOpenXRInstance()
	{
		ulong result;
		ulong num;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_55_0.version && OVRPlugin.OVRP_1_55_0.ovrp_GetNativeOpenXRHandles(out result, out num) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0UL;
	}

	public static ulong GetNativeOpenXRSession()
	{
		ulong num;
		ulong result;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_55_0.version && OVRPlugin.OVRP_1_55_0.ovrp_GetNativeOpenXRHandles(out num, out result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0UL;
	}

	internal static double GetPredictedDisplayTime()
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_44_0.version)
		{
			return 0.0;
		}
		double result = 0.0;
		if (OVRPlugin.OVRP_1_44_0.ovrp_GetPredictedDisplayTime(-1, ref result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0.0;
	}

	internal static IntPtr GetOpenXRInstanceProcAddrFunc()
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return IntPtr.Zero;
		}
		IntPtr zero = IntPtr.Zero;
		if (OVRPlugin.OVRP_1_104_0.ovrp_GetOpenXRInstanceProcAddrFunc(ref zero) == OVRPlugin.Result.Success)
		{
			return zero;
		}
		return IntPtr.Zero;
	}

	internal static OVRPlugin.Result RegisterOpenXREventHandler(OVRPlugin.OpenXREventDelegateType eventHandler)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_104_0.ovrp_RegisterOpenXREventHandler(eventHandler, IntPtr.Zero);
	}

	internal static OVRPlugin.Result UnregisterOpenXREventHandler(OVRPlugin.OpenXREventDelegateType eventHandler)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_104_0.ovrp_UnregisterOpenXREventHandler(eventHandler);
	}

	internal static ulong GetAppSpace()
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_107_0.version)
		{
			return 0UL;
		}
		ulong result = 0UL;
		if (OVRPlugin.OVRP_1_107_0.ovrp_GetAppSpace(ref result) == OVRPlugin.Result.Success)
		{
			return result;
		}
		return 0UL;
	}

	public static bool SetKeyboardOverlayUV(OVRPlugin.Vector2f uv)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_57_0.version && OVRPlugin.OVRP_1_57_0.ovrp_SetKeyboardOverlayUV(uv) == OVRPlugin.Result.Success;
	}

	public static bool CreateSpatialAnchor(OVRPlugin.SpatialAnchorCreateInfo createInfo, out ulong requestId)
	{
		requestId = 0UL;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version && OVRPlugin.OVRP_1_72_0.ovrp_CreateSpatialAnchor(ref createInfo, out requestId) == OVRPlugin.Result.Success;
	}

	public static bool SetSpaceComponentStatus(ulong space, OVRPlugin.SpaceComponentType componentType, bool enable, double timeout, out ulong requestId)
	{
		requestId = 0UL;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version && OVRPlugin.OVRP_1_72_0.ovrp_SetSpaceComponentStatus(ref space, componentType, OVRPlugin.ToBool(enable), timeout, out requestId) == OVRPlugin.Result.Success;
	}

	public static bool GetSpaceComponentStatus(ulong space, OVRPlugin.SpaceComponentType componentType, out bool enabled, out bool changePending)
	{
		return OVRPlugin.GetSpaceComponentStatusInternal(space, componentType, out enabled, out changePending) == OVRPlugin.Result.Success;
	}

	internal static OVRPlugin.Result GetSpaceComponentStatusInternal(ulong space, OVRPlugin.SpaceComponentType componentType, out bool enabled, out bool changePending)
	{
		enabled = false;
		changePending = false;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version)
		{
			OVRPlugin.Bool @bool;
			OVRPlugin.Bool bool2;
			OVRPlugin.Result result = OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceComponentStatus(ref space, componentType, out @bool, out bool2);
			enabled = (@bool == OVRPlugin.Bool.True);
			changePending = (bool2 == OVRPlugin.Bool.True);
			return result;
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	[Obsolete("Use the overload of EnumerateSpaceSupportedComponents that accepts a pointer rather than a managed array.")]
	public static bool EnumerateSpaceSupportedComponents(ulong space, out uint numSupportedComponents, OVRPlugin.SpaceComponentType[] supportedComponents)
	{
		numSupportedComponents = 0U;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version && OVRPlugin.OVRP_1_72_0.ovrp_EnumerateSpaceSupportedComponents(ref space, (uint)supportedComponents.Length, out numSupportedComponents, supportedComponents) == OVRPlugin.Result.Success;
	}

	public unsafe static OVRPlugin.Result EnumerateSpaceSupportedComponents(ulong space, uint capacityInput, out uint countOutput, OVRPlugin.SpaceComponentType* buffer)
	{
		countOutput = 0U;
		if (!(OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version))
		{
			return OVRPlugin.OVRP_1_72_0.ovrp_EnumerateSpaceSupportedComponents(ref space, capacityInput, out countOutput, buffer);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static bool SaveSpace(ulong space, OVRPlugin.SpaceStorageLocation location, OVRPlugin.SpaceStoragePersistenceMode mode, out ulong requestId)
	{
		requestId = 0UL;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version && OVRPlugin.OVRP_1_72_0.ovrp_SaveSpace(ref space, location, mode, out requestId) == OVRPlugin.Result.Success;
	}

	public static bool EraseSpace(ulong space, OVRPlugin.SpaceStorageLocation location, out ulong requestId)
	{
		return OVRPlugin.EraseSpaceWithResult(space, location, out requestId).IsSuccess();
	}

	public static OVRPlugin.Result EraseSpaceWithResult(ulong space, OVRPlugin.SpaceStorageLocation location, out ulong requestId)
	{
		requestId = 0UL;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_72_0.ovrp_EraseSpace(ref space, location, out requestId);
	}

	public static bool GetSpaceUuid(ulong space, out Guid uuid)
	{
		uuid = default(Guid);
		return OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version && OVRPlugin.OVRP_1_74_0.ovrp_GetSpaceUuid(space, out uuid) == OVRPlugin.Result.Success;
	}

	public static bool QuerySpaces(OVRPlugin.SpaceQueryInfo queryInfo, out ulong requestId)
	{
		return OVRPlugin.QuerySpacesWithResult(queryInfo, out requestId).IsSuccess();
	}

	public static OVRPlugin.Result QuerySpacesWithResult(OVRPlugin.SpaceQueryInfo queryInfo, out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version)
		{
			if (queryInfo.FilterType == OVRPlugin.SpaceQueryFilterType.Ids)
			{
				Guid[] ids = queryInfo.IdInfo.Ids;
				if (ids != null && ids.Length > 1024)
				{
					Debug.LogError("QuerySpaces attempted to query more uuids than the maximum number supported: " + 1024.ToString());
					return OVRPlugin.Result.Failure_InvalidParameter;
				}
			}
			else if (queryInfo.FilterType == OVRPlugin.SpaceQueryFilterType.Components)
			{
				OVRPlugin.SpaceComponentType[] components = queryInfo.ComponentsInfo.Components;
				if (components != null && components.Length > 16)
				{
					Debug.LogError("QuerySpaces attempted to query more components than the maximum number supported: " + 16.ToString());
					return OVRPlugin.Result.Failure_InvalidParameter;
				}
			}
			Guid[] ids2 = queryInfo.IdInfo.Ids;
			if (ids2 == null || ids2.Length != 1024)
			{
				Array.Resize<Guid>(ref queryInfo.IdInfo.Ids, 1024);
			}
			OVRPlugin.SpaceComponentType[] components2 = queryInfo.ComponentsInfo.Components;
			if (components2 == null || components2.Length != 16)
			{
				Array.Resize<OVRPlugin.SpaceComponentType>(ref queryInfo.ComponentsInfo.Components, 16);
			}
			return OVRPlugin.OVRP_1_72_0.ovrp_QuerySpaces(ref queryInfo, out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result QuerySpaces2(OVRPlugin.SpaceQueryInfo2 queryInfo, out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			if (queryInfo.FilterType == OVRPlugin.SpaceQueryFilterType.Ids)
			{
				Guid[] ids = queryInfo.IdInfo.Ids;
				if (ids != null && ids.Length > 1024)
				{
					Debug.LogError("QuerySpaces attempted to query more uuids than the maximum number supported: " + 1024.ToString());
					return OVRPlugin.Result.Failure_InvalidParameter;
				}
			}
			else if (queryInfo.FilterType == OVRPlugin.SpaceQueryFilterType.Components)
			{
				OVRPlugin.SpaceComponentType[] components = queryInfo.ComponentsInfo.Components;
				if (components != null && components.Length > 16)
				{
					Debug.LogError("QuerySpaces attempted to query more components than the maximum number supported: " + 16.ToString());
					return OVRPlugin.Result.Failure_InvalidParameter;
				}
			}
			Guid[] ids2 = queryInfo.IdInfo.Ids;
			if (ids2 == null || ids2.Length != 1024)
			{
				Array.Resize<Guid>(ref queryInfo.IdInfo.Ids, 1024);
			}
			OVRPlugin.SpaceComponentType[] components2 = queryInfo.ComponentsInfo.Components;
			if (components2 == null || components2.Length != 16)
			{
				Array.Resize<OVRPlugin.SpaceComponentType>(ref queryInfo.ComponentsInfo.Components, 16);
			}
			return OVRPlugin.OVRP_1_103_0.ovrp_QuerySpaces2(ref queryInfo, out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static bool RetrieveSpaceQueryResults(ulong requestId, out NativeArray<OVRPlugin.SpaceQueryResult> results, Allocator allocator)
	{
		results = default(NativeArray<OVRPlugin.SpaceQueryResult>);
		if (OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version)
		{
			return false;
		}
		uint length = 0U;
		if (OVRPlugin.OVRP_1_72_0.ovrp_RetrieveSpaceQueryResults(ref requestId, 0U, ref length, (IntPtr)0) != OVRPlugin.Result.Success)
		{
			return false;
		}
		results = new NativeArray<OVRPlugin.SpaceQueryResult>((int)length, allocator, NativeArrayOptions.ClearMemory);
		if (OVRPlugin.OVRP_1_72_0.ovrp_RetrieveSpaceQueryResults(ref requestId, (uint)results.Length, ref length, new IntPtr(results.GetUnsafePtr<OVRPlugin.SpaceQueryResult>())) != OVRPlugin.Result.Success)
		{
			results.Dispose();
			return false;
		}
		return true;
	}

	public static bool RetrieveSpaceQueryResults(ulong requestId, out OVRPlugin.SpaceQueryResult[] results)
	{
		results = null;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version))
		{
			return false;
		}
		IntPtr results2 = new IntPtr(0);
		uint num = 0U;
		if (OVRPlugin.OVRP_1_72_0.ovrp_RetrieveSpaceQueryResults(ref requestId, 0U, ref num, results2) != OVRPlugin.Result.Success)
		{
			return false;
		}
		int num2 = Marshal.SizeOf(typeof(OVRPlugin.SpaceQueryResult));
		IntPtr intPtr = Marshal.AllocHGlobal((int)(num * (uint)num2));
		if (OVRPlugin.OVRP_1_72_0.ovrp_RetrieveSpaceQueryResults(ref requestId, num, ref num, intPtr) != OVRPlugin.Result.Success)
		{
			Marshal.FreeHGlobal(intPtr);
			return false;
		}
		results = new OVRPlugin.SpaceQueryResult[num];
		int num3 = 0;
		while ((long)num3 < (long)((ulong)num))
		{
			OVRPlugin.SpaceQueryResult spaceQueryResult = (OVRPlugin.SpaceQueryResult)Marshal.PtrToStructure(intPtr + num3 * num2, typeof(OVRPlugin.SpaceQueryResult));
			results[num3] = spaceQueryResult;
			num3++;
		}
		Marshal.FreeHGlobal(intPtr);
		return true;
	}

	public unsafe static OVRPlugin.Result SaveSpaceList(NativeArray<ulong> spaces, OVRPlugin.SpaceStorageLocation location, out ulong requestId)
	{
		return OVRPlugin.SaveSpaceList((ulong*)spaces.GetUnsafeReadOnlyPtr<ulong>(), (uint)spaces.Length, location, out requestId);
	}

	public unsafe static OVRPlugin.Result SaveSpaceList(ulong* spaces, uint numSpaces, OVRPlugin.SpaceStorageLocation location, out ulong requestId)
	{
		requestId = 0UL;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_79_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_79_0.ovrp_SaveSpaceList(spaces, numSpaces, location, out requestId);
	}

	public static bool GetSpaceUserId(ulong spaceUserHandle, out ulong spaceUserId)
	{
		spaceUserId = 0UL;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_79_0.version && OVRPlugin.OVRP_1_79_0.ovrp_GetSpaceUserId(spaceUserHandle, out spaceUserId) == OVRPlugin.Result.Success;
	}

	public static bool CreateSpaceUser(ulong spaceUserId, out ulong spaceUserHandle)
	{
		spaceUserHandle = 0UL;
		return OVRPlugin.version >= OVRPlugin.OVRP_1_79_0.version && OVRPlugin.OVRP_1_79_0.ovrp_CreateSpaceUser(spaceUserId, out spaceUserHandle) == OVRPlugin.Result.Success;
	}

	public static bool DestroySpaceUser(ulong spaceUserHandle)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_79_0.version && OVRPlugin.OVRP_1_79_0.ovrp_DestroySpaceUser(spaceUserHandle) == OVRPlugin.Result.Success;
	}

	public unsafe static OVRPlugin.Result ShareSpaces(NativeArray<ulong> spaces, NativeArray<ulong> userHandles, out ulong requestId)
	{
		return OVRPlugin.ShareSpaces((ulong*)spaces.GetUnsafeReadOnlyPtr<ulong>(), (uint)spaces.Length, (ulong*)userHandles.GetUnsafeReadOnlyPtr<ulong>(), (uint)userHandles.Length, out requestId);
	}

	public unsafe static OVRPlugin.Result ShareSpaces(ulong* spaces, uint numSpaces, ulong* userHandles, uint numUsers, out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_79_0.version)
		{
			return OVRPlugin.OVRP_1_79_0.ovrp_ShareSpaces(spaces, numSpaces, userHandles, numUsers, out requestId);
		}
		return OVRPlugin.Result.Failure_Unsupported;
	}

	public static bool TryLocateSpace(ulong space, OVRPlugin.TrackingOrigin baseOrigin, out OVRPlugin.Posef pose)
	{
		bool result;
		using (new OVRProfilerScope("TryLocateSpace"))
		{
			pose = OVRPlugin.Posef.identity;
			result = (OVRPlugin.version >= OVRPlugin.OVRP_1_64_0.version && OVRPlugin.OVRP_1_64_0.ovrp_LocateSpace(ref pose, ref space, baseOrigin) == OVRPlugin.Result.Success);
		}
		return result;
	}

	[Obsolete("LocateSpace unconditionally returns a pose, even if the underlying OpenXR function fails. Instead, use TryLocateSpace, which indicates failure.")]
	public static OVRPlugin.Posef LocateSpace(ulong space, OVRPlugin.TrackingOrigin baseOrigin)
	{
		OVRPlugin.Posef result;
		if (!OVRPlugin.TryLocateSpace(space, baseOrigin, out result))
		{
			return OVRPlugin.Posef.identity;
		}
		return result;
	}

	public static bool TryLocateSpace(ulong space, OVRPlugin.TrackingOrigin baseOrigin, out OVRPlugin.Posef pose, out OVRPlugin.SpaceLocationFlags locationFlags)
	{
		pose = OVRPlugin.Posef.identity;
		locationFlags = (OVRPlugin.SpaceLocationFlags)0UL;
		OVRPlugin.SpaceLocationf spaceLocationf;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_79_0.version && OVRPlugin.OVRP_1_79_0.ovrp_LocateSpace2(out spaceLocationf, space, baseOrigin) == OVRPlugin.Result.Success)
		{
			pose = spaceLocationf.pose;
			locationFlags = spaceLocationf.locationFlags;
			return true;
		}
		return false;
	}

	public static bool DestroySpace(ulong space)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version && OVRPlugin.OVRP_1_65_0.ovrp_DestroySpace(ref space) == OVRPlugin.Result.Success;
	}

	public static bool GetSpaceContainer(ulong space, out Guid[] containerUuids)
	{
		containerUuids = Array.Empty<Guid>();
		if (OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version)
		{
			return false;
		}
		OVRPlugin.SpaceContainerInternal spaceContainerInternal = default(OVRPlugin.SpaceContainerInternal);
		if (OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceContainer(ref space, ref spaceContainerInternal) != OVRPlugin.Result.Success)
		{
			return false;
		}
		Guid[] array = new Guid[spaceContainerInternal.uuidCountOutput];
		using (OVRPlugin.PinnedArray<Guid> pinnedArray = new OVRPlugin.PinnedArray<Guid>(array))
		{
			spaceContainerInternal.uuidCapacityInput = spaceContainerInternal.uuidCountOutput;
			spaceContainerInternal.uuids = pinnedArray;
			if (OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceContainer(ref space, ref spaceContainerInternal) != OVRPlugin.Result.Success)
			{
				return false;
			}
		}
		containerUuids = array;
		return true;
	}

	public static bool GetSpaceBoundingBox2D(ulong space, out OVRPlugin.Rectf rect)
	{
		rect = default(OVRPlugin.Rectf);
		return OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version && OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundingBox2D(ref space, out rect) == OVRPlugin.Result.Success;
	}

	public static bool GetSpaceBoundingBox3D(ulong space, out OVRPlugin.Boundsf bounds)
	{
		bounds = default(OVRPlugin.Boundsf);
		return OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version && OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundingBox3D(ref space, out bounds) == OVRPlugin.Result.Success;
	}

	public static bool GetSpaceSemanticLabels(ulong space, out string labels)
	{
		char[] value = null;
		int length;
		if (OVRPlugin.GetSpaceSemanticLabelsNonAlloc(space, ref value, out length))
		{
			labels = new string(value, 0, length);
			return true;
		}
		labels = null;
		return false;
	}

	internal unsafe static bool GetSpaceSemanticLabelsNonAlloc(ulong space, ref char[] buffer, out int length)
	{
		length = -1;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version)
		{
			OVRPlugin.SpaceSemanticLabelInternal spaceSemanticLabelInternal = new OVRPlugin.SpaceSemanticLabelInternal
			{
				byteCapacityInput = 0,
				byteCountOutput = 0
			};
			OVRPlugin.Result result = OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceSemanticLabels(ref space, ref spaceSemanticLabelInternal);
			if (result == OVRPlugin.Result.Success)
			{
				spaceSemanticLabelInternal.byteCapacityInput = spaceSemanticLabelInternal.byteCountOutput;
				length = spaceSemanticLabelInternal.byteCountOutput;
				spaceSemanticLabelInternal.labels = Marshal.AllocHGlobal(length);
				result = OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceSemanticLabels(ref space, ref spaceSemanticLabelInternal);
				if (buffer == null)
				{
					buffer = new char[length];
				}
				else if (buffer.Length < length)
				{
					buffer = new char[Math.Max(buffer.Length * 2, length)];
				}
				byte* ptr = (byte*)((void*)spaceSemanticLabelInternal.labels);
				for (int i = 0; i < length; i++)
				{
					buffer[i] = (char)ptr[i];
				}
				Marshal.FreeHGlobal(spaceSemanticLabelInternal.labels);
			}
			return result == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static bool GetSpaceRoomLayout(ulong space, out OVRPlugin.RoomLayout roomLayout)
	{
		roomLayout = default(OVRPlugin.RoomLayout);
		if (OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version)
		{
			return false;
		}
		OVRPlugin.RoomLayoutInternal roomLayoutInternal = default(OVRPlugin.RoomLayoutInternal);
		if (OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceRoomLayout(ref space, ref roomLayoutInternal) != OVRPlugin.Result.Success)
		{
			return false;
		}
		Guid[] array = new Guid[roomLayoutInternal.wallUuidCountOutput];
		using (OVRPlugin.PinnedArray<Guid> pinnedArray = new OVRPlugin.PinnedArray<Guid>(array))
		{
			roomLayoutInternal.wallUuidCapacityInput = roomLayoutInternal.wallUuidCountOutput;
			roomLayoutInternal.wallUuids = pinnedArray;
			if (OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceRoomLayout(ref space, ref roomLayoutInternal) != OVRPlugin.Result.Success)
			{
				return false;
			}
		}
		roomLayout.ceilingUuid = roomLayoutInternal.ceilingUuid;
		roomLayout.floorUuid = roomLayoutInternal.floorUuid;
		roomLayout.wallUuids = array;
		return true;
	}

	public static bool GetSpaceBoundary2DCount(ulong space, out int count)
	{
		count = 0;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version)
		{
			return false;
		}
		OVRPlugin.PolygonalBoundary2DInternal polygonalBoundary2DInternal = default(OVRPlugin.PolygonalBoundary2DInternal);
		int num = (int)OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundary2D(ref space, ref polygonalBoundary2DInternal);
		count = polygonalBoundary2DInternal.vertexCountOutput;
		return num == 0;
	}

	public static bool GetSpaceBoundary2D(ulong space, NativeArray<Vector2> boundary)
	{
		int num;
		return OVRPlugin.GetSpaceBoundary2D(space, boundary, out num);
	}

	public static bool GetSpaceBoundary2D(ulong space, NativeArray<Vector2> boundary, out int count)
	{
		count = 0;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version)
		{
			return false;
		}
		OVRPlugin.PolygonalBoundary2DInternal polygonalBoundary2DInternal = new OVRPlugin.PolygonalBoundary2DInternal
		{
			vertexCapacityInput = boundary.Length,
			vertices = new IntPtr(boundary.GetUnsafePtr<Vector2>())
		};
		bool result = OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundary2D(ref space, ref polygonalBoundary2DInternal) == OVRPlugin.Result.Success;
		count = polygonalBoundary2DInternal.vertexCountOutput;
		return result;
	}

	public static NativeArray<Vector2> GetSpaceBoundary2D(ulong space, Allocator allocator)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_72_0.version)
		{
			return default(NativeArray<Vector2>);
		}
		OVRPlugin.PolygonalBoundary2DInternal polygonalBoundary2DInternal = new OVRPlugin.PolygonalBoundary2DInternal
		{
			vertexCapacityInput = 0,
			vertexCountOutput = 0
		};
		if (OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundary2D(ref space, ref polygonalBoundary2DInternal) != OVRPlugin.Result.Success)
		{
			return default(NativeArray<Vector2>);
		}
		NativeArray<Vector2> nativeArray = new NativeArray<Vector2>(polygonalBoundary2DInternal.vertexCountOutput, allocator, NativeArrayOptions.ClearMemory);
		polygonalBoundary2DInternal.vertices = new IntPtr(nativeArray.GetUnsafePtr<Vector2>());
		polygonalBoundary2DInternal.vertexCapacityInput = nativeArray.Length;
		if (OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundary2D(ref space, ref polygonalBoundary2DInternal) == OVRPlugin.Result.Success)
		{
			return nativeArray;
		}
		nativeArray.Dispose();
		return default(NativeArray<Vector2>);
	}

	[Obsolete("This method allocates managed arrays. Use GetSpaceBoundary2D(UInt64, Allocator) to avoid managed allocations.")]
	public static bool GetSpaceBoundary2D(ulong space, out Vector2[] boundary)
	{
		boundary = Array.Empty<Vector2>();
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version)
		{
			OVRPlugin.PolygonalBoundary2DInternal polygonalBoundary2DInternal = new OVRPlugin.PolygonalBoundary2DInternal
			{
				vertexCapacityInput = 0,
				vertexCountOutput = 0
			};
			OVRPlugin.Result result = OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundary2D(ref space, ref polygonalBoundary2DInternal);
			if (result == OVRPlugin.Result.Success)
			{
				polygonalBoundary2DInternal.vertexCapacityInput = polygonalBoundary2DInternal.vertexCountOutput;
				int num = Marshal.SizeOf(typeof(Vector2));
				polygonalBoundary2DInternal.vertices = Marshal.AllocHGlobal(polygonalBoundary2DInternal.vertexCountOutput * num);
				result = OVRPlugin.OVRP_1_72_0.ovrp_GetSpaceBoundary2D(ref space, ref polygonalBoundary2DInternal);
				if (result == OVRPlugin.Result.Success)
				{
					boundary = new Vector2[polygonalBoundary2DInternal.vertexCountOutput];
					IntPtr intPtr = polygonalBoundary2DInternal.vertices;
					for (int i = 0; i < polygonalBoundary2DInternal.vertexCountOutput; i++)
					{
						IntPtr ptr = new IntPtr(num);
						ptr = intPtr;
						intPtr += num;
						boundary[i] = Marshal.PtrToStructure<Vector2>(ptr);
					}
					Marshal.FreeHGlobal(polygonalBoundary2DInternal.vertices);
				}
			}
			return result == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static bool RequestSceneCapture(out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_72_0.version)
		{
			OVRPlugin.SceneCaptureRequestInternal sceneCaptureRequestInternal = new OVRPlugin.SceneCaptureRequestInternal
			{
				requestByteCount = 0
			};
			return OVRPlugin.OVRP_1_72_0.ovrp_RequestSceneCapture(ref sceneCaptureRequestInternal, out requestId) == OVRPlugin.Result.Success;
		}
		return false;
	}

	public static bool GetSpaceTriangleMeshCounts(ulong space, out int vertexCount, out int triangleCount)
	{
		vertexCount = 0;
		triangleCount = 0;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_82_0.version)
		{
			return false;
		}
		bool result;
		using (new OVRProfilerScope("GetSpaceTriangleMeshCounts"))
		{
			OVRPlugin.TriangleMeshInternal triangleMeshInternal = default(OVRPlugin.TriangleMeshInternal);
			int num = (int)OVRPlugin.OVRP_1_82_0.ovrp_GetSpaceTriangleMesh(ref space, ref triangleMeshInternal);
			vertexCount = triangleMeshInternal.vertexCountOutput;
			triangleCount = triangleMeshInternal.indexCountOutput / 3;
			result = (num == 0);
		}
		return result;
	}

	public static bool GetSpaceTriangleMesh(ulong space, NativeArray<Vector3> vertices, NativeArray<int> triangles)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_82_0.version)
		{
			return false;
		}
		bool result;
		using (new OVRProfilerScope("GetSpaceTriangleMesh"))
		{
			OVRPlugin.TriangleMeshInternal triangleMeshInternal = new OVRPlugin.TriangleMeshInternal
			{
				vertices = new IntPtr(vertices.GetUnsafePtr<Vector3>()),
				vertexCapacityInput = vertices.Length,
				indices = new IntPtr(triangles.GetUnsafePtr<int>()),
				indexCapacityInput = triangles.Length
			};
			result = (OVRPlugin.OVRP_1_82_0.ovrp_GetSpaceTriangleMesh(ref space, ref triangleMeshInternal) == OVRPlugin.Result.Success);
		}
		return result;
	}

	public static bool GetLayerRecommendedResolution(int layerId, out OVRPlugin.Sizei recommendedSize)
	{
		recommendedSize = default(OVRPlugin.Sizei);
		return OVRPlugin.version >= OVRPlugin.OVRP_1_84_0.version && OVRPlugin.OVRP_1_84_0.ovrp_GetLayerRecommendedResolution(layerId, out recommendedSize) == OVRPlugin.Result.Success;
	}

	public static bool GetEyeLayerRecommendedResolution(out OVRPlugin.Sizei recommendedSize)
	{
		recommendedSize = default(OVRPlugin.Sizei);
		return OVRPlugin.version >= OVRPlugin.OVRP_1_84_0.version && OVRPlugin.OVRP_1_84_0.ovrp_GetEyeLayerRecommendedResolution(out recommendedSize) == OVRPlugin.Result.Success;
	}

	public static string[] GetRenderModelPaths()
	{
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_68_0.version)
		{
			uint num = 0U;
			List<string> list = new List<string>();
			IntPtr intPtr = Marshal.AllocHGlobal(256);
			while (OVRPlugin.OVRP_1_68_0.ovrp_GetRenderModelPaths(num, intPtr) == OVRPlugin.Result.Success)
			{
				list.Add(Marshal.PtrToStringAnsi(intPtr));
				num += 1U;
			}
			Marshal.FreeHGlobal(intPtr);
			return list.ToArray();
		}
		return null;
	}

	public static bool GetRenderModelProperties(string modelPath, ref OVRPlugin.RenderModelProperties modelProperties)
	{
		OVRPlugin.RenderModelPropertiesInternal renderModelPropertiesInternal;
		OVRPlugin.Result result;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_74_0.version)
		{
			result = OVRPlugin.OVRP_1_74_0.ovrp_GetRenderModelProperties2(modelPath, OVRPlugin.RenderModelFlags.SupportsGltf20Subset2, out renderModelPropertiesInternal);
		}
		else
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_68_0.version))
			{
				return false;
			}
			result = OVRPlugin.OVRP_1_68_0.ovrp_GetRenderModelProperties(modelPath, out renderModelPropertiesInternal);
		}
		if (result != OVRPlugin.Result.Success)
		{
			return false;
		}
		modelProperties.ModelName = Encoding.Default.GetString(renderModelPropertiesInternal.ModelName);
		modelProperties.ModelKey = renderModelPropertiesInternal.ModelKey;
		modelProperties.VendorId = renderModelPropertiesInternal.VendorId;
		modelProperties.ModelVersion = renderModelPropertiesInternal.ModelVersion;
		return true;
	}

	public static byte[] LoadRenderModel(ulong modelKey)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_68_0.version))
		{
			return null;
		}
		uint num = 0U;
		if (OVRPlugin.OVRP_1_68_0.ovrp_LoadRenderModel(modelKey, 0U, ref num, IntPtr.Zero) != OVRPlugin.Result.Success)
		{
			return null;
		}
		if (num == 0U)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal((int)num);
		if (OVRPlugin.OVRP_1_68_0.ovrp_LoadRenderModel(modelKey, num, ref num, intPtr) != OVRPlugin.Result.Success)
		{
			Marshal.FreeHGlobal(intPtr);
			return null;
		}
		byte[] array = new byte[num];
		Marshal.Copy(intPtr, array, 0, (int)num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public static OVRPlugin.Result StartColocationSessionAdvertisement(OVRPlugin.ColocationSessionStartAdvertisementInfo info, out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			return OVRPlugin.OVRP_1_103_0.ovrp_StartColocationAdvertisement(info, out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result StopColocationSessionAdvertisement(out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			return OVRPlugin.OVRP_1_103_0.ovrp_StopColocationAdvertisement(out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result StartColocationSessionDiscovery(out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			return OVRPlugin.OVRP_1_103_0.ovrp_StartColocationDiscovery(out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result StopColocationSessionDiscovery(out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			return OVRPlugin.OVRP_1_103_0.ovrp_StopColocationDiscovery(out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result ShareSpaces(in OVRPlugin.ShareSpacesInfo info, out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version)
		{
			return OVRPlugin.OVRP_1_103_0.ovrp_ShareSpaces2(info, out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result DiscoverSpaces(in OVRPlugin.SpaceDiscoveryInfo info, out ulong requestId)
	{
		requestId = 0UL;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_97_0.version)
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_97_0.ovrp_DiscoverSpaces(info, out requestId);
	}

	public unsafe static OVRPlugin.Result RetrieveSpaceDiscoveryResults(ulong requestId, OVRPlugin.SpaceDiscoveryResult* results, int capacityInput, out int countOutput)
	{
		countOutput = 0;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_97_0.version)
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		OVRPlugin.SpaceDiscoveryResults spaceDiscoveryResults = new OVRPlugin.SpaceDiscoveryResults
		{
			ResultCapacityInput = (uint)capacityInput,
			Results = results
		};
		OVRPlugin.Result result = OVRPlugin.OVRP_1_97_0.ovrp_RetrieveSpaceDiscoveryResults(requestId, ref spaceDiscoveryResults);
		countOutput = (int)spaceDiscoveryResults.ResultCountOutput;
		return result;
	}

	public unsafe static OVRPlugin.Result SaveSpaces(ulong* spaces, int count, out ulong requestId)
	{
		requestId = 0UL;
		if (!(OVRPlugin.version < OVRPlugin.OVRP_1_97_0.version))
		{
			return OVRPlugin.OVRP_1_97_0.ovrp_SaveSpaces((uint)count, spaces, out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public unsafe static OVRPlugin.Result EraseSpaces(uint spaceCount, ulong* spaces, uint uuidCount, Guid* uuids, out ulong requestId)
	{
		requestId = 0UL;
		if (!(OVRPlugin.version < OVRPlugin.OVRP_1_97_0.version))
		{
			return OVRPlugin.OVRP_1_97_0.ovrp_EraseSpaces(spaceCount, spaces, uuidCount, uuids, out requestId);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result RequestBoundaryVisibility(OVRPlugin.BoundaryVisibility boundaryVisibility)
	{
		if (!(OVRPlugin.version < OVRPlugin.OVRP_1_98_0.version))
		{
			return OVRPlugin.OVRP_1_98_0.ovrp_RequestBoundaryVisibility(boundaryVisibility);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result GetBoundaryVisibility(out OVRPlugin.BoundaryVisibility boundaryVisibility)
	{
		boundaryVisibility = (OVRPlugin.BoundaryVisibility)0;
		if (!(OVRPlugin.version < OVRPlugin.OVRP_1_98_0.version))
		{
			return OVRPlugin.OVRP_1_98_0.ovrp_GetBoundaryVisibility(out boundaryVisibility);
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result CreateDynamicObjectTracker(out ulong tracker)
	{
		tracker = 0UL;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_104_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_104_0.ovrp_CreateDynamicObjectTracker(out tracker);
	}

	public static OVRTask<OVRResult<ulong, OVRPlugin.Result>> CreateDynamicObjectTrackerAsync()
	{
		ulong requestId;
		return OVRTask.Build(OVRPlugin.CreateDynamicObjectTracker(out requestId), requestId, OVRPlugin.EventType.CreateDynamicObjectTrackerResult).ToTask<ulong, OVRPlugin.Result>();
	}

	public static OVRPlugin.Result DestroyDynamicObjectTracker(ulong tracker)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_104_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_104_0.ovrp_DestroyDynamicObjectTracker(tracker);
	}

	public unsafe static OVRPlugin.Result SetDynamicObjectTrackedClasses(ulong tracker, ReadOnlySpan<OVRPlugin.DynamicObjectClass> classes)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		fixed (OVRPlugin.DynamicObjectClass* pinnableReference = classes.GetPinnableReference())
		{
			OVRPlugin.DynamicObjectClass* classes2 = pinnableReference;
			OVRPlugin.DynamicObjectTrackedClassesSetInfo dynamicObjectTrackedClassesSetInfo = default(OVRPlugin.DynamicObjectTrackedClassesSetInfo);
			dynamicObjectTrackedClassesSetInfo.Classes = classes2;
			dynamicObjectTrackedClassesSetInfo.ClassCount = (uint)classes.Length;
			return OVRPlugin.OVRP_1_104_0.ovrp_SetDynamicObjectTrackedClasses(tracker, dynamicObjectTrackedClassesSetInfo);
		}
	}

	public static OVRTask<OVRResult<OVRPlugin.Result>> SetDynamicObjectTrackedClassesAsync(ulong tracker, ReadOnlySpan<OVRPlugin.DynamicObjectClass> classes)
	{
		return OVRTask.Build(OVRPlugin.SetDynamicObjectTrackedClasses(tracker, classes), tracker, OVRPlugin.EventType.SetDynamicObjectTrackedClassesResult).ToResultTask<OVRPlugin.Result>();
	}

	public static OVRPlugin.Result GetSpaceDynamicObjectData(ulong space, out OVRPlugin.DynamicObjectData data)
	{
		data = default(OVRPlugin.DynamicObjectData);
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_104_0.version))
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_104_0.ovrp_GetSpaceDynamicObjectData(ref space, out data);
	}

	public static OVRPlugin.Result GetDynamicObjectTrackerSupported(out bool value)
	{
		value = false;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		OVRPlugin.Bool @bool;
		OVRPlugin.Result result = OVRPlugin.OVRP_1_104_0.ovrp_GetDynamicObjectTrackerSupported(out @bool);
		value = (@bool == OVRPlugin.Bool.True);
		return result;
	}

	public static OVRPlugin.Result GetDynamicObjectKeyboardSupported(out bool value)
	{
		value = false;
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		OVRPlugin.Bool @bool;
		OVRPlugin.Result result = OVRPlugin.OVRP_1_104_0.ovrp_GetDynamicObjectKeyboardSupported(out @bool);
		value = (@bool == OVRPlugin.Bool.True);
		return result;
	}

	public static void OnEditorShutdown()
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_85_0.version)
		{
			return;
		}
		OVRPlugin.OVRP_1_85_0.ovrp_OnEditorShutdown();
	}

	internal static OVRPlugin.Result GetPassthroughPreferences(out OVRPlugin.PassthroughPreferences preferences)
	{
		preferences = default(OVRPlugin.PassthroughPreferences);
		if (OVRPlugin.version < OVRPlugin.OVRP_1_87_0.version)
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_87_0.ovrp_GetPassthroughPreferences(out preferences);
	}

	public static bool SetEyeBufferSharpenType(OVRPlugin.LayerSharpenType sharpenType)
	{
		return OVRPlugin.version >= OVRPlugin.OVRP_1_87_0.version && OVRPlugin.OVRP_1_87_0.ovrp_SetEyeBufferSharpenType(sharpenType) == OVRPlugin.Result.Success;
	}

	public unsafe static OVRPlugin.Result CreateMarkerTrackerAsync(ReadOnlySpan<OVRPlugin.MarkerType> markerTypes, out ulong future)
	{
		future = 0UL;
		fixed (OVRPlugin.MarkerType* pinnableReference = markerTypes.GetPinnableReference())
		{
			OVRPlugin.MarkerType* markerTypes2 = pinnableReference;
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_110_0.version))
			{
				return OVRPlugin.Result.Failure_NotYetImplemented;
			}
			OVRPlugin.MarkerTrackerCreateInfo markerTrackerCreateInfo = default(OVRPlugin.MarkerTrackerCreateInfo);
			markerTrackerCreateInfo.MarkerTypes = markerTypes2;
			markerTrackerCreateInfo.MarkerTypeCount = (uint)markerTypes.Length;
			return OVRPlugin.OVRP_1_110_0.ovrp_CreateMarkerTrackerAsync(markerTrackerCreateInfo, out future);
		}
	}

	public static OVRPlugin.Result CreateMarkerTrackerComplete(ulong future, out OVRPlugin.MarkerTrackerCreateCompletion completion)
	{
		completion = default(OVRPlugin.MarkerTrackerCreateCompletion);
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_110_0.version))
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_110_0.ovrp_CreateMarkerTrackerComplete(future, out completion);
	}

	public static OVRPlugin.Result DestroyMarkerTracker(ulong markerTracker)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_110_0.version))
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_110_0.ovrp_DestroyMarkerTracker(markerTracker);
	}

	public static OVRPlugin.Result GetSpaceMarkerPayload(ulong space, ref OVRPlugin.SpaceMarkerPayload payload)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_110_0.version))
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_110_0.ovrp_GetSpaceMarkerPayload(space, ref payload);
	}

	public static OVRPlugin.Result GetMarkerTrackingSupported(out bool markerTrackingSupported)
	{
		markerTrackingSupported = false;
		if (OVRPlugin.version >= OVRPlugin.OVRP_1_110_0.version)
		{
			OVRPlugin.Bool @bool;
			OVRPlugin.Result result = OVRPlugin.OVRP_1_110_0.ovrp_GetMarkerTrackingSupported(out @bool);
			markerTrackingSupported = (@bool == OVRPlugin.Bool.True);
			return result;
		}
		return OVRPlugin.Result.Failure_NotYetImplemented;
	}

	public static OVRPlugin.Result PollFuture(ulong future, out OVRPlugin.FutureState state)
	{
		state = (OVRPlugin.FutureState)0;
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version))
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_103_0.ovrp_PollFuture(future, out state);
	}

	public static OVRPlugin.Result CancelFuture(ulong future)
	{
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_103_0.version))
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_103_0.ovrp_CancelFuture(future);
	}

	public static OVRPlugin.Result SetExternalLayerDynresEnabled(OVRPlugin.Bool enabled)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_104_0.version)
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_104_0.ovrp_SetExternalLayerDynresEnabled(enabled);
	}

	public static OVRPlugin.Result SetDeveloperTelemetryConsent(OVRPlugin.Bool consent)
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_95_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		return OVRPlugin.OVRP_1_95_0.ovrp_SetDeveloperTelemetryConsent(consent);
	}

	public static bool BeginProfilingRegion(string regionName)
	{
		return OVRPlugin.OVRP_1_104_0.ovrp_BeginProfilingRegion(regionName) == OVRPlugin.Result.Success;
	}

	public static bool EndProfilingRegion()
	{
		return OVRPlugin.OVRP_1_104_0.ovrp_EndProfilingRegion() == OVRPlugin.Result.Success;
	}

	public static OVRPlugin.Result SendMicrogestureHint()
	{
		if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
		{
			return OVRPlugin.Result.Failure_Unsupported;
		}
		OVRPlugin.OVRP_1_106_0.ovrp_SendMicrogestureHint();
		return OVRPlugin.Result.Success;
	}

	public static OVRPlugin.Result GetStationaryReferenceSpaceId(out Guid generationId)
	{
		generationId = default(Guid);
		if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_109_0.version))
		{
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}
		return OVRPlugin.OVRP_1_109_0.ovrp_GetStationaryReferenceSpaceId(out generationId);
	}

	public const bool isSupportedPlatform = true;

	public static readonly Version wrapperVersion = OVRPlugin.OVRP_1_110_0.version;

	private static Version _version;

	private static Version _nativeSDKVersion;

	public static int MAX_CPU_CORES = 8;

	private const int OverlayShapeFlagShift = 4;

	public const int AppPerfFrameStatsMaxCount = 5;

	private const int EventDataBufferSize = 4000;

	public const int RENDER_MODEL_NULL_KEY = 0;

	public const int SpaceFilterInfoIdsMaxSize = 1024;

	public const int SpaceFilterInfoComponentsMaxSize = 16;

	public const int SpatialEntityMaxQueryResultsPerEvent = 128;

	public const int MaxQuerySpacesByGroup = 1024;

	private static OVRPlugin.XrApi? _nativeXrApi = null;

	private static OVRPlugin.GUID _nativeAudioOutGuid = new OVRPlugin.GUID();

	private static Guid _cachedAudioOutGuid;

	private static string _cachedAudioOutString;

	private static OVRPlugin.GUID _nativeAudioInGuid = new OVRPlugin.GUID();

	private static Guid _cachedAudioInGuid;

	private static string _cachedAudioInString;

	private static OVRPlugin.ProcessorPerformanceLevel m_suggestedCpuPerfLevelOpenXR = OVRPlugin.ProcessorPerformanceLevel.SustainedHigh;

	private static OVRPlugin.ProcessorPerformanceLevel m_suggestedGpuPerfLevelOpenXR = OVRPlugin.ProcessorPerformanceLevel.SustainedHigh;

	private static bool perfStatWarningPrinted = false;

	private static bool resetPerfStatWarningPrinted = false;

	private static Texture2D cachedCameraFrameTexture = null;

	private static Texture2D cachedCameraDepthTexture = null;

	private static Texture2D cachedCameraDepthConfidenceTexture = null;

	private static OVRNativeBuffer _nativeSystemDisplayFrequenciesAvailable = null;

	private static float[] _cachedSystemDisplayFrequenciesAvailable = null;

	private static OVRPlugin.HandStateInternal cachedHandState = default(OVRPlugin.HandStateInternal);

	private static OVRPlugin.HandState3Internal cachedHandState3 = default(OVRPlugin.HandState3Internal);

	private static Quaternion LeftBoneRotator = Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(270f, Vector3.up);

	private static Quaternion RightBoneRotator = Quaternion.AngleAxis(270f, Vector3.up);

	private static OVRPlugin.HandTrackingStateInternal cachedHandTrackingState = default(OVRPlugin.HandTrackingStateInternal);

	private static OVRPlugin.Skeleton cachedSkeleton = default(OVRPlugin.Skeleton);

	private static OVRPlugin.Skeleton2Internal cachedSkeleton2 = default(OVRPlugin.Skeleton2Internal);

	private static OVRPlugin.GetBoneSkeleton2Delegate[] Skeleton2GetBone = new OVRPlugin.GetBoneSkeleton2Delegate[]
	{
		() => OVRPlugin.cachedSkeleton2.Bones_0,
		() => OVRPlugin.cachedSkeleton2.Bones_1,
		() => OVRPlugin.cachedSkeleton2.Bones_2,
		() => OVRPlugin.cachedSkeleton2.Bones_3,
		() => OVRPlugin.cachedSkeleton2.Bones_4,
		() => OVRPlugin.cachedSkeleton2.Bones_5,
		() => OVRPlugin.cachedSkeleton2.Bones_6,
		() => OVRPlugin.cachedSkeleton2.Bones_7,
		() => OVRPlugin.cachedSkeleton2.Bones_8,
		() => OVRPlugin.cachedSkeleton2.Bones_9,
		() => OVRPlugin.cachedSkeleton2.Bones_10,
		() => OVRPlugin.cachedSkeleton2.Bones_11,
		() => OVRPlugin.cachedSkeleton2.Bones_12,
		() => OVRPlugin.cachedSkeleton2.Bones_13,
		() => OVRPlugin.cachedSkeleton2.Bones_14,
		() => OVRPlugin.cachedSkeleton2.Bones_15,
		() => OVRPlugin.cachedSkeleton2.Bones_16,
		() => OVRPlugin.cachedSkeleton2.Bones_17,
		() => OVRPlugin.cachedSkeleton2.Bones_18,
		() => OVRPlugin.cachedSkeleton2.Bones_19,
		() => OVRPlugin.cachedSkeleton2.Bones_20,
		() => OVRPlugin.cachedSkeleton2.Bones_21,
		() => OVRPlugin.cachedSkeleton2.Bones_22,
		() => OVRPlugin.cachedSkeleton2.Bones_23,
		() => OVRPlugin.cachedSkeleton2.Bones_24,
		() => OVRPlugin.cachedSkeleton2.Bones_25,
		() => OVRPlugin.cachedSkeleton2.Bones_26,
		() => OVRPlugin.cachedSkeleton2.Bones_27,
		() => OVRPlugin.cachedSkeleton2.Bones_28,
		() => OVRPlugin.cachedSkeleton2.Bones_29,
		() => OVRPlugin.cachedSkeleton2.Bones_30,
		() => OVRPlugin.cachedSkeleton2.Bones_31,
		() => OVRPlugin.cachedSkeleton2.Bones_32,
		() => OVRPlugin.cachedSkeleton2.Bones_33,
		() => OVRPlugin.cachedSkeleton2.Bones_34,
		() => OVRPlugin.cachedSkeleton2.Bones_35,
		() => OVRPlugin.cachedSkeleton2.Bones_36,
		() => OVRPlugin.cachedSkeleton2.Bones_37,
		() => OVRPlugin.cachedSkeleton2.Bones_38,
		() => OVRPlugin.cachedSkeleton2.Bones_39,
		() => OVRPlugin.cachedSkeleton2.Bones_40,
		() => OVRPlugin.cachedSkeleton2.Bones_41,
		() => OVRPlugin.cachedSkeleton2.Bones_42,
		() => OVRPlugin.cachedSkeleton2.Bones_43,
		() => OVRPlugin.cachedSkeleton2.Bones_44,
		() => OVRPlugin.cachedSkeleton2.Bones_45,
		() => OVRPlugin.cachedSkeleton2.Bones_46,
		() => OVRPlugin.cachedSkeleton2.Bones_47,
		() => OVRPlugin.cachedSkeleton2.Bones_48,
		() => OVRPlugin.cachedSkeleton2.Bones_49,
		() => OVRPlugin.cachedSkeleton2.Bones_50,
		() => OVRPlugin.cachedSkeleton2.Bones_51,
		() => OVRPlugin.cachedSkeleton2.Bones_52,
		() => OVRPlugin.cachedSkeleton2.Bones_53,
		() => OVRPlugin.cachedSkeleton2.Bones_54,
		() => OVRPlugin.cachedSkeleton2.Bones_55,
		() => OVRPlugin.cachedSkeleton2.Bones_56,
		() => OVRPlugin.cachedSkeleton2.Bones_57,
		() => OVRPlugin.cachedSkeleton2.Bones_58,
		() => OVRPlugin.cachedSkeleton2.Bones_59,
		() => OVRPlugin.cachedSkeleton2.Bones_60,
		() => OVRPlugin.cachedSkeleton2.Bones_61,
		() => OVRPlugin.cachedSkeleton2.Bones_62,
		() => OVRPlugin.cachedSkeleton2.Bones_63,
		() => OVRPlugin.cachedSkeleton2.Bones_64,
		() => OVRPlugin.cachedSkeleton2.Bones_65,
		() => OVRPlugin.cachedSkeleton2.Bones_66,
		() => OVRPlugin.cachedSkeleton2.Bones_67,
		() => OVRPlugin.cachedSkeleton2.Bones_68,
		() => OVRPlugin.cachedSkeleton2.Bones_69
	};

	private static OVRPlugin.Skeleton3Internal cachedSkeleton3 = default(OVRPlugin.Skeleton3Internal);

	private static OVRPlugin.GetBoneSkeleton3Delegate[] Skeleton3GetBone = new OVRPlugin.GetBoneSkeleton3Delegate[]
	{
		() => OVRPlugin.cachedSkeleton3.Bones_0,
		() => OVRPlugin.cachedSkeleton3.Bones_1,
		() => OVRPlugin.cachedSkeleton3.Bones_2,
		() => OVRPlugin.cachedSkeleton3.Bones_3,
		() => OVRPlugin.cachedSkeleton3.Bones_4,
		() => OVRPlugin.cachedSkeleton3.Bones_5,
		() => OVRPlugin.cachedSkeleton3.Bones_6,
		() => OVRPlugin.cachedSkeleton3.Bones_7,
		() => OVRPlugin.cachedSkeleton3.Bones_8,
		() => OVRPlugin.cachedSkeleton3.Bones_9,
		() => OVRPlugin.cachedSkeleton3.Bones_10,
		() => OVRPlugin.cachedSkeleton3.Bones_11,
		() => OVRPlugin.cachedSkeleton3.Bones_12,
		() => OVRPlugin.cachedSkeleton3.Bones_13,
		() => OVRPlugin.cachedSkeleton3.Bones_14,
		() => OVRPlugin.cachedSkeleton3.Bones_15,
		() => OVRPlugin.cachedSkeleton3.Bones_16,
		() => OVRPlugin.cachedSkeleton3.Bones_17,
		() => OVRPlugin.cachedSkeleton3.Bones_18,
		() => OVRPlugin.cachedSkeleton3.Bones_19,
		() => OVRPlugin.cachedSkeleton3.Bones_20,
		() => OVRPlugin.cachedSkeleton3.Bones_21,
		() => OVRPlugin.cachedSkeleton3.Bones_22,
		() => OVRPlugin.cachedSkeleton3.Bones_23,
		() => OVRPlugin.cachedSkeleton3.Bones_24,
		() => OVRPlugin.cachedSkeleton3.Bones_25,
		() => OVRPlugin.cachedSkeleton3.Bones_26,
		() => OVRPlugin.cachedSkeleton3.Bones_27,
		() => OVRPlugin.cachedSkeleton3.Bones_28,
		() => OVRPlugin.cachedSkeleton3.Bones_29,
		() => OVRPlugin.cachedSkeleton3.Bones_30,
		() => OVRPlugin.cachedSkeleton3.Bones_31,
		() => OVRPlugin.cachedSkeleton3.Bones_32,
		() => OVRPlugin.cachedSkeleton3.Bones_33,
		() => OVRPlugin.cachedSkeleton3.Bones_34,
		() => OVRPlugin.cachedSkeleton3.Bones_35,
		() => OVRPlugin.cachedSkeleton3.Bones_36,
		() => OVRPlugin.cachedSkeleton3.Bones_37,
		() => OVRPlugin.cachedSkeleton3.Bones_38,
		() => OVRPlugin.cachedSkeleton3.Bones_39,
		() => OVRPlugin.cachedSkeleton3.Bones_40,
		() => OVRPlugin.cachedSkeleton3.Bones_41,
		() => OVRPlugin.cachedSkeleton3.Bones_42,
		() => OVRPlugin.cachedSkeleton3.Bones_43,
		() => OVRPlugin.cachedSkeleton3.Bones_44,
		() => OVRPlugin.cachedSkeleton3.Bones_45,
		() => OVRPlugin.cachedSkeleton3.Bones_46,
		() => OVRPlugin.cachedSkeleton3.Bones_47,
		() => OVRPlugin.cachedSkeleton3.Bones_48,
		() => OVRPlugin.cachedSkeleton3.Bones_49,
		() => OVRPlugin.cachedSkeleton3.Bones_50,
		() => OVRPlugin.cachedSkeleton3.Bones_51,
		() => OVRPlugin.cachedSkeleton3.Bones_52,
		() => OVRPlugin.cachedSkeleton3.Bones_53,
		() => OVRPlugin.cachedSkeleton3.Bones_54,
		() => OVRPlugin.cachedSkeleton3.Bones_55,
		() => OVRPlugin.cachedSkeleton3.Bones_56,
		() => OVRPlugin.cachedSkeleton3.Bones_57,
		() => OVRPlugin.cachedSkeleton3.Bones_58,
		() => OVRPlugin.cachedSkeleton3.Bones_59,
		() => OVRPlugin.cachedSkeleton3.Bones_60,
		() => OVRPlugin.cachedSkeleton3.Bones_61,
		() => OVRPlugin.cachedSkeleton3.Bones_62,
		() => OVRPlugin.cachedSkeleton3.Bones_63,
		() => OVRPlugin.cachedSkeleton3.Bones_64,
		() => OVRPlugin.cachedSkeleton3.Bones_65,
		() => OVRPlugin.cachedSkeleton3.Bones_66,
		() => OVRPlugin.cachedSkeleton3.Bones_67,
		() => OVRPlugin.cachedSkeleton3.Bones_68,
		() => OVRPlugin.cachedSkeleton3.Bones_69,
		() => OVRPlugin.cachedSkeleton3.Bones_70,
		() => OVRPlugin.cachedSkeleton3.Bones_71,
		() => OVRPlugin.cachedSkeleton3.Bones_72,
		() => OVRPlugin.cachedSkeleton3.Bones_73,
		() => OVRPlugin.cachedSkeleton3.Bones_74,
		() => OVRPlugin.cachedSkeleton3.Bones_75,
		() => OVRPlugin.cachedSkeleton3.Bones_76,
		() => OVRPlugin.cachedSkeleton3.Bones_77,
		() => OVRPlugin.cachedSkeleton3.Bones_78,
		() => OVRPlugin.cachedSkeleton3.Bones_79,
		() => OVRPlugin.cachedSkeleton3.Bones_80,
		() => OVRPlugin.cachedSkeleton3.Bones_81,
		() => OVRPlugin.cachedSkeleton3.Bones_82,
		() => OVRPlugin.cachedSkeleton3.Bones_83
	};

	private static OVRPlugin.FaceStateInternal cachedFaceState = default(OVRPlugin.FaceStateInternal);

	private static OVRPlugin.FaceState2Internal cachedFaceState2 = default(OVRPlugin.FaceState2Internal);

	private static OVRPlugin.FaceVisemesStateInternal cachedFaceVisemesState = default(OVRPlugin.FaceVisemesStateInternal);

	private static OVRPlugin.EyeGazesStateInternal cachedEyeGazesState = default(OVRPlugin.EyeGazesStateInternal);

	private static OVRPlugin.BodyJointSet _currentJointSet = OVRPlugin.BodyJointSet.None;

	private const string pluginName = "OVRPlugin";

	private static Version _versionZero = new Version(0, 0, 0);

	[StructLayout(LayoutKind.Sequential)]
	private class GUID
	{
		public int a;

		public short b;

		public short c;

		public byte d0;

		public byte d1;

		public byte d2;

		public byte d3;

		public byte d4;

		public byte d5;

		public byte d6;

		public byte d7;
	}

	public enum Bool
	{
		False,
		True
	}

	public enum OptionalBool
	{
		False,
		True,
		Unknown
	}

	[OVRResultStatus]
	public enum Result
	{
		Success,
		Success_EventUnavailable,
		Success_Pending,
		Success_ColocationSessionAlreadyAdvertising = 3001,
		Success_ColocationSessionAlreadyDiscovering,
		Failure = -1000,
		Failure_InvalidParameter = -1001,
		Failure_NotInitialized = -1002,
		Failure_InvalidOperation = -1003,
		Failure_Unsupported = -1004,
		Failure_NotYetImplemented = -1005,
		Failure_OperationFailed = -1006,
		Failure_InsufficientSize = -1007,
		Failure_DataIsInvalid = -1008,
		Failure_DeprecatedOperation = -1009,
		Failure_ErrorLimitReached = -1010,
		Failure_ErrorInitializationFailed = -1011,
		Failure_RuntimeUnavailable = -1012,
		Failure_HandleInvalid = -1013,
		Failure_SpaceCloudStorageDisabled = -2000,
		Failure_SpaceMappingInsufficient = -2001,
		Failure_SpaceLocalizationFailed = -2002,
		Failure_SpaceNetworkTimeout = -2003,
		Failure_SpaceNetworkRequestFailed = -2004,
		Failure_SpaceComponentNotSupported = -2005,
		Failure_SpaceComponentNotEnabled = -2006,
		Failure_SpaceComponentStatusPending = -2007,
		Failure_SpaceComponentStatusAlreadySet = -2008,
		Failure_SpaceGroupNotFound = -2009,
		Failure_ColocationSessionNetworkFailed = -3002,
		Failure_ColocationSessionNoDiscoveryMethodAvailable = -3003,
		Failure_SpaceInsufficientResources = -9000,
		Failure_SpaceStorageAtCapacity = -9001,
		Failure_SpaceInsufficientView = -9002,
		Failure_SpacePermissionInsufficient = -9003,
		Failure_SpaceRateLimited = -9004,
		Failure_SpaceTooDark = -9005,
		Failure_SpaceTooBright = -9006,
		Warning_BoundaryVisibilitySuppressionNotAllowed = 9030,
		Failure_FuturePending = -10000,
		Failure_FutureInvalid = -10001
	}

	public enum LogLevel
	{
		Debug,
		Info,
		Error
	}

	public delegate void LogCallback2DelegateType(OVRPlugin.LogLevel logLevel, IntPtr message, int size);

	public enum CameraStatus
	{
		CameraStatus_None,
		CameraStatus_Connected,
		CameraStatus_Calibrating,
		CameraStatus_CalibrationFailed,
		CameraStatus_Calibrated,
		CameraStatus_ThirdPerson,
		CameraStatus_EnumSize = 2147483647
	}

	public enum CameraAnchorType
	{
		CameraAnchorType_PreDefined,
		CameraAnchorType_Custom,
		CameraAnchorType_Count,
		CameraAnchorType_EnumSize = 2147483647
	}

	public enum XrApi
	{
		Unknown,
		CAPI,
		VRAPI,
		OpenXR,
		EnumSize = 2147483647
	}

	public enum Eye
	{
		None = -1,
		Left,
		Right,
		Count
	}

	public enum Tracker
	{
		None = -1,
		Zero,
		One,
		Two,
		Three,
		Count
	}

	public enum Node
	{
		None = -1,
		EyeLeft,
		EyeRight,
		EyeCenter,
		HandLeft,
		HandRight,
		TrackerZero,
		TrackerOne,
		TrackerTwo,
		TrackerThree,
		Head,
		DeviceObjectZero,
		TrackedKeyboard,
		ControllerLeft,
		ControllerRight,
		Count
	}

	public enum ActionTypes
	{
		Boolean = 1,
		Float,
		Vector2,
		Pose,
		Vibration = 100
	}

	public enum Controller
	{
		None,
		LTouch,
		RTouch,
		Touch,
		Remote,
		Gamepad = 16,
		LHand = 32,
		RHand = 64,
		Hands = 96,
		Active = -2147483648,
		All = -1
	}

	public enum InteractionProfile
	{
		None,
		Touch,
		TouchPro,
		TouchPlus = 4
	}

	public enum Handedness
	{
		Unsupported,
		LeftHanded,
		RightHanded
	}

	public enum TrackingOrigin
	{
		EyeLevel,
		FloorLevel,
		Stage,
		View = 4,
		Stationary = 6,
		Count
	}

	public enum SpaceFlags
	{
		None,
		AllowRecentering
	}

	public enum RecenterFlags
	{
		Default,
		IgnoreAll = -2147483648,
		Count
	}

	public enum BatteryStatus
	{
		Charging,
		Discharging,
		Full,
		NotCharging,
		Unknown
	}

	public enum EyeTextureFormat
	{
		Default,
		R8G8B8A8_sRGB = 0,
		R8G8B8A8,
		R16G16B16A16_FP,
		R11G11B10_FP,
		B8G8R8A8_sRGB,
		B8G8R8A8,
		R5G6B5 = 11,
		EnumSize = 2147483647
	}

	public enum PlatformUI
	{
		None = -1,
		ConfirmQuit = 1,
		GlobalMenuTutorial
	}

	public enum SystemRegion
	{
		Unspecified,
		Japan,
		China
	}

	public enum SystemHeadset
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

	public enum OverlayShape
	{
		Quad,
		Cylinder,
		Cubemap,
		OffcenterCubemap = 4,
		Equirect,
		ReconstructionPassthrough = 7,
		SurfaceProjectedPassthrough,
		Fisheye,
		KeyboardHandsPassthrough,
		KeyboardMaskedHandsPassthrough
	}

	public enum LayerSuperSamplingType
	{
		None,
		Normal = 4096,
		Quality = 256
	}

	public enum LayerSharpenType
	{
		None,
		Normal = 8192,
		Quality = 65536,
		Automatic = 262144
	}

	public enum Step
	{
		Render = -1,
		Physics
	}

	public enum CameraDevice
	{
		None,
		WebCamera0 = 100,
		WebCamera1,
		ZEDCamera = 300
	}

	public enum CameraDeviceDepthSensingMode
	{
		Standard,
		Fill
	}

	public enum CameraDeviceDepthQuality
	{
		Low,
		Medium,
		High
	}

	public enum FoveatedRenderingLevel
	{
		Off,
		Low,
		Medium,
		High,
		HighTop,
		EnumSize = 2147483647
	}

	[Obsolete("Please use FoveatedRenderingLevel instead", false)]
	public enum FixedFoveatedRenderingLevel
	{
		Off,
		Low,
		Medium,
		High,
		HighTop,
		EnumSize = 2147483647
	}

	[Obsolete("Please use FixedFoveatedRenderingLevel instead", false)]
	public enum TiledMultiResLevel
	{
		Off,
		LMSLow,
		LMSMedium,
		LMSHigh,
		LMSHighTop,
		EnumSize = 2147483647
	}

	public enum PerfMetrics
	{
		App_CpuTime_Float,
		App_GpuTime_Float,
		Compositor_CpuTime_Float = 3,
		Compositor_GpuTime_Float,
		Compositor_DroppedFrameCount_Int,
		System_GpuUtilPercentage_Float = 7,
		System_CpuUtilAveragePercentage_Float,
		System_CpuUtilWorstPercentage_Float,
		Device_CpuClockFrequencyInMHz_Float,
		Device_GpuClockFrequencyInMHz_Float,
		Device_CpuClockLevel_Int,
		Device_GpuClockLevel_Int,
		Compositor_SpaceWarp_Mode_Int,
		Device_CpuCore0UtilPercentage_Float = 32,
		Device_CpuCore1UtilPercentage_Float,
		Device_CpuCore2UtilPercentage_Float,
		Device_CpuCore3UtilPercentage_Float,
		Device_CpuCore4UtilPercentage_Float,
		Device_CpuCore5UtilPercentage_Float,
		Device_CpuCore6UtilPercentage_Float,
		Device_CpuCore7UtilPercentage_Float,
		Count,
		EnumSize = 2147483647
	}

	public enum ProcessorPerformanceLevel
	{
		PowerSavings,
		SustainedLow,
		SustainedHigh,
		Boost,
		EnumSize = 2147483647
	}

	public enum FeatureType
	{
		HandTracking,
		KeyboardTracking,
		EyeTracking,
		FaceTracking,
		BodyTracking,
		Passthrough,
		GazeBasedFoveatedRendering,
		Count,
		EnumSize = 2147483647
	}

	public struct CameraDeviceIntrinsicsParameters
	{
		private float fx;

		private float fy;

		private float cx;

		private float cy;

		private double disto0;

		private double disto1;

		private double disto2;

		private double disto3;

		private double disto4;

		private float v_fov;

		private float h_fov;

		private float d_fov;

		private int w;

		private int h;
	}

	private enum OverlayFlag
	{
		None,
		OnTop,
		HeadLocked,
		NoDepth = 4,
		ExpensiveSuperSample = 8,
		EfficientSuperSample = 16,
		EfficientSharpen = 32,
		BicubicFiltering = 64,
		ExpensiveSharpen = 128,
		SecureContent = 256,
		ShapeFlag_Quad = 0,
		ShapeFlag_Cylinder = 16,
		ShapeFlag_Cubemap = 32,
		ShapeFlag_OffcenterCubemap = 64,
		ShapeFlagRangeMask = 240,
		Hidden = 512,
		AutoFiltering = 1024,
		PremultipliedAlpha = 1048576
	}

	public struct Vector2f
	{
		public float x;

		public float y;
	}

	public struct Vector3f
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", this.x, this.y, this.z);
		}

		public float x;

		public float y;

		public float z;

		public static readonly OVRPlugin.Vector3f zero = new OVRPlugin.Vector3f
		{
			x = 0f,
			y = 0f,
			z = 0f
		};
	}

	public struct Vector4f
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.w
			});
		}

		public float x;

		public float y;

		public float z;

		public float w;

		public static readonly OVRPlugin.Vector4f zero = new OVRPlugin.Vector4f
		{
			x = 0f,
			y = 0f,
			z = 0f,
			w = 0f
		};
	}

	public struct Vector4s
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.w
			});
		}

		public short x;

		public short y;

		public short z;

		public short w;

		public static readonly OVRPlugin.Vector4s zero = new OVRPlugin.Vector4s
		{
			x = 0,
			y = 0,
			z = 0,
			w = 0
		};
	}

	public struct Quatf
	{
		public Quatf(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.w
			});
		}

		public float x;

		public float y;

		public float z;

		public float w;

		public static readonly OVRPlugin.Quatf identity = new OVRPlugin.Quatf
		{
			x = 0f,
			y = 0f,
			z = 0f,
			w = 1f
		};
	}

	public struct Posef
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Position ({0}), Orientation({1})", this.Position, this.Orientation);
		}

		public OVRPlugin.Quatf Orientation;

		public OVRPlugin.Vector3f Position;

		public static readonly OVRPlugin.Posef identity = new OVRPlugin.Posef
		{
			Orientation = OVRPlugin.Quatf.identity,
			Position = OVRPlugin.Vector3f.zero
		};
	}

	public struct TextureRectMatrixf
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Rect Left ({0}), Rect Right({1}), Scale Bias Left ({2}), Scale Bias Right({3})", new object[]
			{
				this.leftRect,
				this.rightRect,
				this.leftScaleBias,
				this.rightScaleBias
			});
		}

		public Rect leftRect;

		public Rect rightRect;

		public Vector4 leftScaleBias;

		public Vector4 rightScaleBias;

		public static readonly OVRPlugin.TextureRectMatrixf zero = new OVRPlugin.TextureRectMatrixf
		{
			leftRect = new Rect(0f, 0f, 1f, 1f),
			rightRect = new Rect(0f, 0f, 1f, 1f),
			leftScaleBias = new Vector4(1f, 1f, 0f, 0f),
			rightScaleBias = new Vector4(1f, 1f, 0f, 0f)
		};
	}

	public struct PoseStatef
	{
		public OVRPlugin.Posef Pose;

		public OVRPlugin.Vector3f Velocity;

		[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
		public OVRPlugin.Vector3f Acceleration;

		public OVRPlugin.Vector3f AngularVelocity;

		[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
		public OVRPlugin.Vector3f AngularAcceleration;

		public double Time;

		public static readonly OVRPlugin.PoseStatef identity = new OVRPlugin.PoseStatef
		{
			Pose = OVRPlugin.Posef.identity,
			Velocity = OVRPlugin.Vector3f.zero,
			AngularVelocity = OVRPlugin.Vector3f.zero,
			Acceleration = OVRPlugin.Vector3f.zero,
			AngularAcceleration = OVRPlugin.Vector3f.zero
		};
	}

	public enum HapticsLocation
	{
		None,
		Hand,
		Thumb,
		Index = 4
	}

	public struct ControllerState6
	{
		public ControllerState6(OVRPlugin.ControllerState5 cs)
		{
			this.ConnectedControllers = cs.ConnectedControllers;
			this.Buttons = cs.Buttons;
			this.Touches = cs.Touches;
			this.NearTouches = cs.NearTouches;
			this.LIndexTrigger = cs.LIndexTrigger;
			this.RIndexTrigger = cs.RIndexTrigger;
			this.LHandTrigger = cs.LHandTrigger;
			this.RHandTrigger = cs.RHandTrigger;
			this.LThumbstick = cs.LThumbstick;
			this.RThumbstick = cs.RThumbstick;
			this.LTouchpad = cs.LTouchpad;
			this.RTouchpad = cs.RTouchpad;
			this.LBatteryPercentRemaining = cs.LBatteryPercentRemaining;
			this.RBatteryPercentRemaining = cs.RBatteryPercentRemaining;
			this.LRecenterCount = cs.LRecenterCount;
			this.RRecenterCount = cs.RRecenterCount;
			this.LThumbRestForce = cs.LThumbRestForce;
			this.RThumbRestForce = cs.RThumbRestForce;
			this.LStylusForce = cs.LStylusForce;
			this.RStylusForce = cs.RStylusForce;
			this.LIndexTriggerCurl = cs.LIndexTriggerCurl;
			this.RIndexTriggerCurl = cs.RIndexTriggerCurl;
			this.LIndexTriggerSlide = cs.LIndexTriggerSlide;
			this.RIndexTriggerSlide = cs.RIndexTriggerSlide;
			this.LIndexTriggerForce = 0f;
			this.RIndexTriggerForce = 0f;
		}

		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public OVRPlugin.Vector2f LThumbstick;

		public OVRPlugin.Vector2f RThumbstick;

		public OVRPlugin.Vector2f LTouchpad;

		public OVRPlugin.Vector2f RTouchpad;

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public byte LBatteryPercentRemaining;

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public byte RBatteryPercentRemaining;

		public byte LRecenterCount;

		public byte RRecenterCount;

		public float LThumbRestForce;

		public float RThumbRestForce;

		public float LStylusForce;

		public float RStylusForce;

		public float LIndexTriggerCurl;

		public float RIndexTriggerCurl;

		public float LIndexTriggerSlide;

		public float RIndexTriggerSlide;

		public float LIndexTriggerForce;

		public float RIndexTriggerForce;
	}

	public struct ControllerState5
	{
		public ControllerState5(OVRPlugin.ControllerState4 cs)
		{
			this.ConnectedControllers = cs.ConnectedControllers;
			this.Buttons = cs.Buttons;
			this.Touches = cs.Touches;
			this.NearTouches = cs.NearTouches;
			this.LIndexTrigger = cs.LIndexTrigger;
			this.RIndexTrigger = cs.RIndexTrigger;
			this.LHandTrigger = cs.LHandTrigger;
			this.RHandTrigger = cs.RHandTrigger;
			this.LThumbstick = cs.LThumbstick;
			this.RThumbstick = cs.RThumbstick;
			this.LTouchpad = cs.LTouchpad;
			this.RTouchpad = cs.RTouchpad;
			this.LBatteryPercentRemaining = cs.LBatteryPercentRemaining;
			this.RBatteryPercentRemaining = cs.RBatteryPercentRemaining;
			this.LRecenterCount = cs.LRecenterCount;
			this.RRecenterCount = cs.RRecenterCount;
			this.LThumbRestForce = 0f;
			this.RThumbRestForce = 0f;
			this.LStylusForce = 0f;
			this.RStylusForce = 0f;
			this.LIndexTriggerCurl = 0f;
			this.RIndexTriggerCurl = 0f;
			this.LIndexTriggerSlide = 0f;
			this.RIndexTriggerSlide = 0f;
		}

		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public OVRPlugin.Vector2f LThumbstick;

		public OVRPlugin.Vector2f RThumbstick;

		public OVRPlugin.Vector2f LTouchpad;

		public OVRPlugin.Vector2f RTouchpad;

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public byte LBatteryPercentRemaining;

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public byte RBatteryPercentRemaining;

		public byte LRecenterCount;

		public byte RRecenterCount;

		public float LThumbRestForce;

		public float RThumbRestForce;

		public float LStylusForce;

		public float RStylusForce;

		public float LIndexTriggerCurl;

		public float RIndexTriggerCurl;

		public float LIndexTriggerSlide;

		public float RIndexTriggerSlide;
	}

	public struct ControllerState4
	{
		public ControllerState4(OVRPlugin.ControllerState2 cs)
		{
			this.ConnectedControllers = cs.ConnectedControllers;
			this.Buttons = cs.Buttons;
			this.Touches = cs.Touches;
			this.NearTouches = cs.NearTouches;
			this.LIndexTrigger = cs.LIndexTrigger;
			this.RIndexTrigger = cs.RIndexTrigger;
			this.LHandTrigger = cs.LHandTrigger;
			this.RHandTrigger = cs.RHandTrigger;
			this.LThumbstick = cs.LThumbstick;
			this.RThumbstick = cs.RThumbstick;
			this.LTouchpad = cs.LTouchpad;
			this.RTouchpad = cs.RTouchpad;
			this.LBatteryPercentRemaining = 0;
			this.RBatteryPercentRemaining = 0;
			this.LRecenterCount = 0;
			this.RRecenterCount = 0;
			this.Reserved_27 = 0;
			this.Reserved_26 = 0;
			this.Reserved_25 = 0;
			this.Reserved_24 = 0;
			this.Reserved_23 = 0;
			this.Reserved_22 = 0;
			this.Reserved_21 = 0;
			this.Reserved_20 = 0;
			this.Reserved_19 = 0;
			this.Reserved_18 = 0;
			this.Reserved_17 = 0;
			this.Reserved_16 = 0;
			this.Reserved_15 = 0;
			this.Reserved_14 = 0;
			this.Reserved_13 = 0;
			this.Reserved_12 = 0;
			this.Reserved_11 = 0;
			this.Reserved_10 = 0;
			this.Reserved_09 = 0;
			this.Reserved_08 = 0;
			this.Reserved_07 = 0;
			this.Reserved_06 = 0;
			this.Reserved_05 = 0;
			this.Reserved_04 = 0;
			this.Reserved_03 = 0;
			this.Reserved_02 = 0;
			this.Reserved_01 = 0;
			this.Reserved_00 = 0;
		}

		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public OVRPlugin.Vector2f LThumbstick;

		public OVRPlugin.Vector2f RThumbstick;

		public OVRPlugin.Vector2f LTouchpad;

		public OVRPlugin.Vector2f RTouchpad;

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public byte LBatteryPercentRemaining;

		[Obsolete("Deprecated. The controller battery percentage data is no longer supported in OpenXR", false)]
		public byte RBatteryPercentRemaining;

		public byte LRecenterCount;

		public byte RRecenterCount;

		public byte Reserved_27;

		public byte Reserved_26;

		public byte Reserved_25;

		public byte Reserved_24;

		public byte Reserved_23;

		public byte Reserved_22;

		public byte Reserved_21;

		public byte Reserved_20;

		public byte Reserved_19;

		public byte Reserved_18;

		public byte Reserved_17;

		public byte Reserved_16;

		public byte Reserved_15;

		public byte Reserved_14;

		public byte Reserved_13;

		public byte Reserved_12;

		public byte Reserved_11;

		public byte Reserved_10;

		public byte Reserved_09;

		public byte Reserved_08;

		public byte Reserved_07;

		public byte Reserved_06;

		public byte Reserved_05;

		public byte Reserved_04;

		public byte Reserved_03;

		public byte Reserved_02;

		public byte Reserved_01;

		public byte Reserved_00;
	}

	public struct ControllerState2
	{
		public ControllerState2(OVRPlugin.ControllerState cs)
		{
			this.ConnectedControllers = cs.ConnectedControllers;
			this.Buttons = cs.Buttons;
			this.Touches = cs.Touches;
			this.NearTouches = cs.NearTouches;
			this.LIndexTrigger = cs.LIndexTrigger;
			this.RIndexTrigger = cs.RIndexTrigger;
			this.LHandTrigger = cs.LHandTrigger;
			this.RHandTrigger = cs.RHandTrigger;
			this.LThumbstick = cs.LThumbstick;
			this.RThumbstick = cs.RThumbstick;
			this.LTouchpad = new OVRPlugin.Vector2f
			{
				x = 0f,
				y = 0f
			};
			this.RTouchpad = new OVRPlugin.Vector2f
			{
				x = 0f,
				y = 0f
			};
		}

		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public OVRPlugin.Vector2f LThumbstick;

		public OVRPlugin.Vector2f RThumbstick;

		public OVRPlugin.Vector2f LTouchpad;

		public OVRPlugin.Vector2f RTouchpad;
	}

	public struct ControllerState
	{
		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public OVRPlugin.Vector2f LThumbstick;

		public OVRPlugin.Vector2f RThumbstick;
	}

	public struct HapticsBuffer
	{
		public IntPtr Samples;

		public int SamplesCount;
	}

	public struct HapticsState
	{
		public int SamplesAvailable;

		public int SamplesQueued;
	}

	public struct HapticsDesc
	{
		public int SampleRateHz;

		public int SampleSizeInBytes;

		public int MinimumSafeSamplesQueued;

		public int MinimumBufferSamplesCount;

		public int OptimalBufferSamplesCount;

		public int MaximumBufferSamplesCount;
	}

	public struct HapticsAmplitudeEnvelopeVibration
	{
		public float Duration;

		public uint AmplitudeCount;

		public IntPtr Amplitudes;
	}

	public struct HapticsPcmVibration
	{
		public uint BufferSize;

		public IntPtr Buffer;

		public float SampleRateHz;

		public OVRPlugin.Bool Append;

		public IntPtr SamplesConsumed;
	}

	public enum HapticsConstants
	{
		ParametricHapticsUnspecifiedFrequency,
		MaxSamples = 4000
	}

	public struct AppPerfFrameStats
	{
		public int HmdVsyncIndex;

		public int AppFrameIndex;

		public int AppDroppedFrameCount;

		public float AppMotionToPhotonLatency;

		public float AppQueueAheadTime;

		public float AppCpuElapsedTime;

		public float AppGpuElapsedTime;

		public int CompositorFrameIndex;

		public int CompositorDroppedFrameCount;

		public float CompositorLatency;

		public float CompositorCpuElapsedTime;

		public float CompositorGpuElapsedTime;

		public float CompositorCpuStartToGpuEndElapsedTime;

		public float CompositorGpuEndToVsyncElapsedTime;
	}

	public struct AppPerfStats
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public OVRPlugin.AppPerfFrameStats[] FrameStats;

		public int FrameStatsCount;

		public OVRPlugin.Bool AnyFrameStatsDropped;

		public float AdaptiveGpuPerformanceScale;
	}

	public struct Sizei : IEquatable<OVRPlugin.Sizei>
	{
		public bool Equals(OVRPlugin.Sizei other)
		{
			return this.w == other.w && this.h == other.h;
		}

		public override bool Equals(object obj)
		{
			if (obj is OVRPlugin.Sizei)
			{
				OVRPlugin.Sizei other = (OVRPlugin.Sizei)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.w * 397 ^ this.h;
		}

		public int w;

		public int h;

		public static readonly OVRPlugin.Sizei zero = new OVRPlugin.Sizei
		{
			w = 0,
			h = 0
		};
	}

	public struct Sizef
	{
		public float w;

		public float h;

		public static readonly OVRPlugin.Sizef zero = new OVRPlugin.Sizef
		{
			w = 0f,
			h = 0f
		};
	}

	public struct Size3f
	{
		public float w;

		public float h;

		public float d;

		public static readonly OVRPlugin.Size3f zero = new OVRPlugin.Size3f
		{
			w = 0f,
			h = 0f,
			d = 0f
		};
	}

	public struct Vector2i
	{
		public int x;

		public int y;
	}

	public struct Recti
	{
		public OVRPlugin.Vector2i Pos;

		public OVRPlugin.Sizei Size;
	}

	public struct RectiPair
	{
		public OVRPlugin.Recti this[int i]
		{
			get
			{
				if (i == 0)
				{
					return this.Rect0;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException(string.Format("{0} was not in range [0,2)", i));
				}
				return this.Rect1;
			}
			set
			{
				if (i == 0)
				{
					this.Rect0 = value;
					return;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException(string.Format("{0} was not in range [0,2)", i));
				}
				this.Rect1 = value;
			}
		}

		public OVRPlugin.Recti Rect0;

		public OVRPlugin.Recti Rect1;
	}

	public struct Rectf
	{
		public OVRPlugin.Vector2f Pos;

		public OVRPlugin.Sizef Size;
	}

	public struct RectfPair
	{
		public OVRPlugin.Rectf this[int i]
		{
			get
			{
				if (i == 0)
				{
					return this.Rect0;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException(string.Format("{0} was not in range [0,2)", i));
				}
				return this.Rect1;
			}
			set
			{
				if (i == 0)
				{
					this.Rect0 = value;
					return;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException(string.Format("{0} was not in range [0,2)", i));
				}
				this.Rect1 = value;
			}
		}

		public OVRPlugin.Rectf Rect0;

		public OVRPlugin.Rectf Rect1;
	}

	public struct Boundsf
	{
		public OVRPlugin.Vector3f Pos;

		public OVRPlugin.Size3f Size;
	}

	public struct Frustumf
	{
		public float zNear;

		public float zFar;

		public float fovX;

		public float fovY;
	}

	public struct Frustumf2
	{
		public float zNear;

		public float zFar;

		public OVRPlugin.Fovf Fov;
	}

	public enum BoundaryType
	{
		[Obsolete("Deprecated. This enum value will not be supported in OpenXR", false)]
		OuterBoundary = 1,
		PlayArea = 256
	}

	[Obsolete("Deprecated. This struct will not be supported in OpenXR", false)]
	public struct BoundaryTestResult
	{
		public OVRPlugin.Bool IsTriggering;

		public float ClosestDistance;

		public OVRPlugin.Vector3f ClosestPoint;

		public OVRPlugin.Vector3f ClosestPointNormal;
	}

	public struct BoundaryGeometry
	{
		public OVRPlugin.BoundaryType BoundaryType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public OVRPlugin.Vector3f[] Points;

		public int PointsCount;
	}

	public struct Colorf
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "R:{0:F3} G:{1:F3} B:{2:F3} A:{3:F3}", new object[]
			{
				this.r,
				this.g,
				this.b,
				this.a
			});
		}

		public float r;

		public float g;

		public float b;

		public float a;
	}

	public struct Fovf
	{
		public float UpTan;

		public float DownTan;

		public float LeftTan;

		public float RightTan;
	}

	public struct FovfPair
	{
		public OVRPlugin.Fovf this[int i]
		{
			get
			{
				if (i == 0)
				{
					return this.Fov0;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException(string.Format("{0} was not in range [0,2)", i));
				}
				return this.Fov1;
			}
			set
			{
				if (i == 0)
				{
					this.Fov0 = value;
					return;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException(string.Format("{0} was not in range [0,2)", i));
				}
				this.Fov1 = value;
			}
		}

		public OVRPlugin.Fovf Fov0;

		public OVRPlugin.Fovf Fov1;
	}

	public struct CameraIntrinsics
	{
		public OVRPlugin.Bool IsValid;

		public double LastChangedTimeSeconds;

		public OVRPlugin.Fovf FOVPort;

		public float VirtualNearPlaneDistanceMeters;

		public float VirtualFarPlaneDistanceMeters;

		public OVRPlugin.Sizei ImageSensorPixelResolution;
	}

	public struct CameraExtrinsics
	{
		public OVRPlugin.Bool IsValid;

		public double LastChangedTimeSeconds;

		public OVRPlugin.CameraStatus CameraStatusData;

		public OVRPlugin.Node AttachedToNode;

		public OVRPlugin.Posef RelativePose;
	}

	public enum LayerLayout
	{
		Stereo,
		Mono,
		DoubleWide,
		Array,
		EnumSize = 15
	}

	public enum LayerFlags
	{
		Static = 1,
		LoadingScreen,
		SymmetricFov = 4,
		TextureOriginAtBottomLeft = 8,
		ChromaticAberrationCorrection = 16,
		NoAllocation = 32,
		ProtectedContent = 64,
		AndroidSurfaceSwapChain = 128,
		BicubicFiltering = 16384
	}

	public struct LayerDesc
	{
		public override string ToString()
		{
			string text = ", ";
			return string.Concat(new string[]
			{
				this.Shape.ToString(),
				text,
				this.Layout.ToString(),
				text,
				this.TextureSize.w.ToString(),
				"x",
				this.TextureSize.h.ToString(),
				text,
				this.MipLevels.ToString(),
				text,
				this.SampleCount.ToString(),
				text,
				this.Format.ToString(),
				text,
				this.LayerFlags.ToString()
			});
		}

		public OVRPlugin.OverlayShape Shape;

		public OVRPlugin.LayerLayout Layout;

		public OVRPlugin.Sizei TextureSize;

		public int MipLevels;

		public int SampleCount;

		public OVRPlugin.EyeTextureFormat Format;

		public int LayerFlags;

		public OVRPlugin.FovfPair Fov;

		public OVRPlugin.RectfPair VisibleRect;

		public OVRPlugin.Sizei MaxViewportSize;

		public OVRPlugin.EyeTextureFormat DepthFormat;

		public OVRPlugin.EyeTextureFormat MotionVectorFormat;

		public OVRPlugin.EyeTextureFormat MotionVectorDepthFormat;

		public OVRPlugin.Sizei MotionVectorTextureSize;
	}

	public enum BlendFactor
	{
		Zero,
		One,
		SrcAlpha,
		OneMinusSrcAlpha,
		DstAlpha,
		OneMinusDstAlpha
	}

	public struct LayerSubmit
	{
		private int LayerId;

		private int TextureStage;

		private OVRPlugin.RectiPair ViewportRect;

		private OVRPlugin.Posef Pose;

		private int LayerSubmitFlags;
	}

	public enum TrackingConfidence
	{
		Low,
		High = 1065353216
	}

	public enum Hand
	{
		None = -1,
		HandLeft,
		HandRight
	}

	[Flags]
	public enum HandStatus
	{
		HandTracked = 1,
		InputStateValid = 2,
		SystemGestureInProgress = 64,
		DominantHand = 128,
		MenuPressed = 256
	}

	public enum BoneId
	{
		Invalid = -1,
		Hand_Start,
		Hand_WristRoot = 0,
		Hand_ForearmStub,
		Hand_Thumb0,
		Hand_Thumb1,
		Hand_Thumb2,
		Hand_Thumb3,
		Hand_Index1,
		Hand_Index2,
		Hand_Index3,
		Hand_Middle1,
		Hand_Middle2,
		Hand_Middle3,
		Hand_Ring1,
		Hand_Ring2,
		Hand_Ring3,
		Hand_Pinky0,
		Hand_Pinky1,
		Hand_Pinky2,
		Hand_Pinky3,
		Hand_MaxSkinnable,
		Hand_ThumbTip = 19,
		Hand_IndexTip,
		Hand_MiddleTip,
		Hand_RingTip,
		Hand_PinkyTip,
		Hand_End,
		XRHand_Start = 0,
		XRHand_Palm = 0,
		XRHand_Wrist,
		XRHand_ThumbMetacarpal,
		XRHand_ThumbProximal,
		XRHand_ThumbDistal,
		XRHand_ThumbTip,
		XRHand_IndexMetacarpal,
		XRHand_IndexProximal,
		XRHand_IndexIntermediate,
		XRHand_IndexDistal,
		XRHand_IndexTip,
		XRHand_MiddleMetacarpal,
		XRHand_MiddleProximal,
		XRHand_MiddleIntermediate,
		XRHand_MiddleDistal,
		XRHand_MiddleTip,
		XRHand_RingMetacarpal,
		XRHand_RingProximal,
		XRHand_RingIntermediate,
		XRHand_RingDistal,
		XRHand_RingTip,
		XRHand_LittleMetacarpal,
		XRHand_LittleProximal,
		XRHand_LittleIntermediate,
		XRHand_LittleDistal,
		XRHand_LittleTip,
		XRHand_Max,
		XRHand_End = 26,
		Body_Start = 0,
		Body_Root = 0,
		Body_Hips,
		Body_SpineLower,
		Body_SpineMiddle,
		Body_SpineUpper,
		Body_Chest,
		Body_Neck,
		Body_Head,
		Body_LeftShoulder,
		Body_LeftScapula,
		Body_LeftArmUpper,
		Body_LeftArmLower,
		Body_LeftHandWristTwist,
		Body_RightShoulder,
		Body_RightScapula,
		Body_RightArmUpper,
		Body_RightArmLower,
		Body_RightHandWristTwist,
		Body_LeftHandPalm,
		Body_LeftHandWrist,
		Body_LeftHandThumbMetacarpal,
		Body_LeftHandThumbProximal,
		Body_LeftHandThumbDistal,
		Body_LeftHandThumbTip,
		Body_LeftHandIndexMetacarpal,
		Body_LeftHandIndexProximal,
		Body_LeftHandIndexIntermediate,
		Body_LeftHandIndexDistal,
		Body_LeftHandIndexTip,
		Body_LeftHandMiddleMetacarpal,
		Body_LeftHandMiddleProximal,
		Body_LeftHandMiddleIntermediate,
		Body_LeftHandMiddleDistal,
		Body_LeftHandMiddleTip,
		Body_LeftHandRingMetacarpal,
		Body_LeftHandRingProximal,
		Body_LeftHandRingIntermediate,
		Body_LeftHandRingDistal,
		Body_LeftHandRingTip,
		Body_LeftHandLittleMetacarpal,
		Body_LeftHandLittleProximal,
		Body_LeftHandLittleIntermediate,
		Body_LeftHandLittleDistal,
		Body_LeftHandLittleTip,
		Body_RightHandPalm,
		Body_RightHandWrist,
		Body_RightHandThumbMetacarpal,
		Body_RightHandThumbProximal,
		Body_RightHandThumbDistal,
		Body_RightHandThumbTip,
		Body_RightHandIndexMetacarpal,
		Body_RightHandIndexProximal,
		Body_RightHandIndexIntermediate,
		Body_RightHandIndexDistal,
		Body_RightHandIndexTip,
		Body_RightHandMiddleMetacarpal,
		Body_RightHandMiddleProximal,
		Body_RightHandMiddleIntermediate,
		Body_RightHandMiddleDistal,
		Body_RightHandMiddleTip,
		Body_RightHandRingMetacarpal,
		Body_RightHandRingProximal,
		Body_RightHandRingIntermediate,
		Body_RightHandRingDistal,
		Body_RightHandRingTip,
		Body_RightHandLittleMetacarpal,
		Body_RightHandLittleProximal,
		Body_RightHandLittleIntermediate,
		Body_RightHandLittleDistal,
		Body_RightHandLittleTip,
		Body_End,
		FullBody_Start = 0,
		FullBody_Root = 0,
		FullBody_Hips,
		FullBody_SpineLower,
		FullBody_SpineMiddle,
		FullBody_SpineUpper,
		FullBody_Chest,
		FullBody_Neck,
		FullBody_Head,
		FullBody_LeftShoulder,
		FullBody_LeftScapula,
		FullBody_LeftArmUpper,
		FullBody_LeftArmLower,
		FullBody_LeftHandWristTwist,
		FullBody_RightShoulder,
		FullBody_RightScapula,
		FullBody_RightArmUpper,
		FullBody_RightArmLower,
		FullBody_RightHandWristTwist,
		FullBody_LeftHandPalm,
		FullBody_LeftHandWrist,
		FullBody_LeftHandThumbMetacarpal,
		FullBody_LeftHandThumbProximal,
		FullBody_LeftHandThumbDistal,
		FullBody_LeftHandThumbTip,
		FullBody_LeftHandIndexMetacarpal,
		FullBody_LeftHandIndexProximal,
		FullBody_LeftHandIndexIntermediate,
		FullBody_LeftHandIndexDistal,
		FullBody_LeftHandIndexTip,
		FullBody_LeftHandMiddleMetacarpal,
		FullBody_LeftHandMiddleProximal,
		FullBody_LeftHandMiddleIntermediate,
		FullBody_LeftHandMiddleDistal,
		FullBody_LeftHandMiddleTip,
		FullBody_LeftHandRingMetacarpal,
		FullBody_LeftHandRingProximal,
		FullBody_LeftHandRingIntermediate,
		FullBody_LeftHandRingDistal,
		FullBody_LeftHandRingTip,
		FullBody_LeftHandLittleMetacarpal,
		FullBody_LeftHandLittleProximal,
		FullBody_LeftHandLittleIntermediate,
		FullBody_LeftHandLittleDistal,
		FullBody_LeftHandLittleTip,
		FullBody_RightHandPalm,
		FullBody_RightHandWrist,
		FullBody_RightHandThumbMetacarpal,
		FullBody_RightHandThumbProximal,
		FullBody_RightHandThumbDistal,
		FullBody_RightHandThumbTip,
		FullBody_RightHandIndexMetacarpal,
		FullBody_RightHandIndexProximal,
		FullBody_RightHandIndexIntermediate,
		FullBody_RightHandIndexDistal,
		FullBody_RightHandIndexTip,
		FullBody_RightHandMiddleMetacarpal,
		FullBody_RightHandMiddleProximal,
		FullBody_RightHandMiddleIntermediate,
		FullBody_RightHandMiddleDistal,
		FullBody_RightHandMiddleTip,
		FullBody_RightHandRingMetacarpal,
		FullBody_RightHandRingProximal,
		FullBody_RightHandRingIntermediate,
		FullBody_RightHandRingDistal,
		FullBody_RightHandRingTip,
		FullBody_RightHandLittleMetacarpal,
		FullBody_RightHandLittleProximal,
		FullBody_RightHandLittleIntermediate,
		FullBody_RightHandLittleDistal,
		FullBody_RightHandLittleTip,
		FullBody_LeftUpperLeg,
		FullBody_LeftLowerLeg,
		FullBody_LeftFootAnkleTwist,
		FullBody_LeftFootAnkle,
		FullBody_LeftFootSubtalar,
		FullBody_LeftFootTransverse,
		FullBody_LeftFootBall,
		FullBody_RightUpperLeg,
		FullBody_RightLowerLeg,
		FullBody_RightFootAnkleTwist,
		FullBody_RightFootAnkle,
		FullBody_RightFootSubtalar,
		FullBody_RightFootTransverse,
		FullBody_RightFootBall,
		FullBody_End,
		FullBody_Invalid,
		Max = 84
	}

	public enum HandFinger
	{
		Thumb,
		Index,
		Middle,
		Ring,
		Pinky,
		Max
	}

	public enum MicrogestureType
	{
		NoGesture,
		SwipeLeft,
		SwipeRight,
		SwipeForward,
		SwipeBackward,
		ThumbTap,
		Invalid = -1
	}

	[Flags]
	public enum HandFingerPinch
	{
		Thumb = 1,
		Index = 2,
		Middle = 4,
		Ring = 8,
		Pinky = 16
	}

	public struct HandState
	{
		public OVRPlugin.HandStatus Status;

		public OVRPlugin.Posef RootPose;

		public OVRPlugin.Quatf[] BoneRotations;

		public OVRPlugin.Vector3f[] BonePositions;

		public OVRPlugin.HandFingerPinch Pinches;

		public float[] PinchStrength;

		public OVRPlugin.Posef PointerPose;

		public float HandScale;

		public OVRPlugin.TrackingConfidence HandConfidence;

		public OVRPlugin.TrackingConfidence[] FingerConfidences;

		public double RequestedTimeStamp;

		public double SampleTimeStamp;
	}

	public struct HandTrackingState
	{
		public OVRPlugin.MicrogestureType Microgesture;
	}

	private struct HandTrackingStateInternal
	{
		public OVRPlugin.MicrogestureType Microgesture;
	}

	private struct HandStateInternal
	{
		public OVRPlugin.HandStatus Status;

		public OVRPlugin.Posef RootPose;

		public OVRPlugin.Quatf BoneRotations_0;

		public OVRPlugin.Quatf BoneRotations_1;

		public OVRPlugin.Quatf BoneRotations_2;

		public OVRPlugin.Quatf BoneRotations_3;

		public OVRPlugin.Quatf BoneRotations_4;

		public OVRPlugin.Quatf BoneRotations_5;

		public OVRPlugin.Quatf BoneRotations_6;

		public OVRPlugin.Quatf BoneRotations_7;

		public OVRPlugin.Quatf BoneRotations_8;

		public OVRPlugin.Quatf BoneRotations_9;

		public OVRPlugin.Quatf BoneRotations_10;

		public OVRPlugin.Quatf BoneRotations_11;

		public OVRPlugin.Quatf BoneRotations_12;

		public OVRPlugin.Quatf BoneRotations_13;

		public OVRPlugin.Quatf BoneRotations_14;

		public OVRPlugin.Quatf BoneRotations_15;

		public OVRPlugin.Quatf BoneRotations_16;

		public OVRPlugin.Quatf BoneRotations_17;

		public OVRPlugin.Quatf BoneRotations_18;

		public OVRPlugin.Quatf BoneRotations_19;

		public OVRPlugin.Quatf BoneRotations_20;

		public OVRPlugin.Quatf BoneRotations_21;

		public OVRPlugin.Quatf BoneRotations_22;

		public OVRPlugin.Quatf BoneRotations_23;

		public OVRPlugin.HandFingerPinch Pinches;

		public float PinchStrength_0;

		public float PinchStrength_1;

		public float PinchStrength_2;

		public float PinchStrength_3;

		public float PinchStrength_4;

		public OVRPlugin.Posef PointerPose;

		public float HandScale;

		public OVRPlugin.TrackingConfidence HandConfidence;

		public OVRPlugin.TrackingConfidence FingerConfidences_0;

		public OVRPlugin.TrackingConfidence FingerConfidences_1;

		public OVRPlugin.TrackingConfidence FingerConfidences_2;

		public OVRPlugin.TrackingConfidence FingerConfidences_3;

		public OVRPlugin.TrackingConfidence FingerConfidences_4;

		public double RequestedTimeStamp;

		public double SampleTimeStamp;
	}

	private struct HandState3Internal
	{
		public OVRPlugin.HandStatus Status;

		public OVRPlugin.Posef RootPose;

		public OVRPlugin.Posef BonePoses_0;

		public OVRPlugin.Posef BonePoses_1;

		public OVRPlugin.Posef BonePoses_2;

		public OVRPlugin.Posef BonePoses_3;

		public OVRPlugin.Posef BonePoses_4;

		public OVRPlugin.Posef BonePoses_5;

		public OVRPlugin.Posef BonePoses_6;

		public OVRPlugin.Posef BonePoses_7;

		public OVRPlugin.Posef BonePoses_8;

		public OVRPlugin.Posef BonePoses_9;

		public OVRPlugin.Posef BonePoses_10;

		public OVRPlugin.Posef BonePoses_11;

		public OVRPlugin.Posef BonePoses_12;

		public OVRPlugin.Posef BonePoses_13;

		public OVRPlugin.Posef BonePoses_14;

		public OVRPlugin.Posef BonePoses_15;

		public OVRPlugin.Posef BonePoses_16;

		public OVRPlugin.Posef BonePoses_17;

		public OVRPlugin.Posef BonePoses_18;

		public OVRPlugin.Posef BonePoses_19;

		public OVRPlugin.Posef BonePoses_20;

		public OVRPlugin.Posef BonePoses_21;

		public OVRPlugin.Posef BonePoses_22;

		public OVRPlugin.Posef BonePoses_23;

		public OVRPlugin.Posef BonePoses_24;

		public OVRPlugin.Posef BonePoses_25;

		public OVRPlugin.HandFingerPinch Pinches;

		public float PinchStrength_0;

		public float PinchStrength_1;

		public float PinchStrength_2;

		public float PinchStrength_3;

		public float PinchStrength_4;

		public OVRPlugin.Posef PointerPose;

		public float HandScale;

		public OVRPlugin.TrackingConfidence HandConfidence;

		public OVRPlugin.TrackingConfidence FingerConfidences_0;

		public OVRPlugin.TrackingConfidence FingerConfidences_1;

		public OVRPlugin.TrackingConfidence FingerConfidences_2;

		public OVRPlugin.TrackingConfidence FingerConfidences_3;

		public OVRPlugin.TrackingConfidence FingerConfidences_4;

		public double RequestedTimeStamp;

		public double SampleTimeStamp;
	}

	public struct BoneCapsule
	{
		public short BoneIndex;

		public OVRPlugin.Vector3f StartPoint;

		public OVRPlugin.Vector3f EndPoint;

		public float Radius;
	}

	public struct Bone
	{
		public OVRPlugin.BoneId Id;

		public short ParentBoneIndex;

		public OVRPlugin.Posef Pose;
	}

	public enum SkeletonConstants
	{
		MaxHandBones = 24,
		MaxXRHandBones = 26,
		MaxBodyBones = 70,
		MaxBones = 84,
		MaxBoneCapsules = 19
	}

	public enum SkeletonType
	{
		None = -1,
		HandLeft,
		HandRight,
		Body,
		FullBody,
		XRHandLeft,
		XRHandRight
	}

	public struct Skeleton
	{
		public OVRPlugin.SkeletonType Type;

		public uint NumBones;

		public uint NumBoneCapsules;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		public OVRPlugin.Bone[] Bones;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
		public OVRPlugin.BoneCapsule[] BoneCapsules;
	}

	public struct Skeleton2
	{
		public OVRPlugin.SkeletonType Type;

		public uint NumBones;

		public uint NumBoneCapsules;

		public OVRPlugin.Bone[] Bones;

		public OVRPlugin.BoneCapsule[] BoneCapsules;
	}

	private struct Skeleton2Internal
	{
		public OVRPlugin.SkeletonType Type;

		public uint NumBones;

		public uint NumBoneCapsules;

		public OVRPlugin.Bone Bones_0;

		public OVRPlugin.Bone Bones_1;

		public OVRPlugin.Bone Bones_2;

		public OVRPlugin.Bone Bones_3;

		public OVRPlugin.Bone Bones_4;

		public OVRPlugin.Bone Bones_5;

		public OVRPlugin.Bone Bones_6;

		public OVRPlugin.Bone Bones_7;

		public OVRPlugin.Bone Bones_8;

		public OVRPlugin.Bone Bones_9;

		public OVRPlugin.Bone Bones_10;

		public OVRPlugin.Bone Bones_11;

		public OVRPlugin.Bone Bones_12;

		public OVRPlugin.Bone Bones_13;

		public OVRPlugin.Bone Bones_14;

		public OVRPlugin.Bone Bones_15;

		public OVRPlugin.Bone Bones_16;

		public OVRPlugin.Bone Bones_17;

		public OVRPlugin.Bone Bones_18;

		public OVRPlugin.Bone Bones_19;

		public OVRPlugin.Bone Bones_20;

		public OVRPlugin.Bone Bones_21;

		public OVRPlugin.Bone Bones_22;

		public OVRPlugin.Bone Bones_23;

		public OVRPlugin.Bone Bones_24;

		public OVRPlugin.Bone Bones_25;

		public OVRPlugin.Bone Bones_26;

		public OVRPlugin.Bone Bones_27;

		public OVRPlugin.Bone Bones_28;

		public OVRPlugin.Bone Bones_29;

		public OVRPlugin.Bone Bones_30;

		public OVRPlugin.Bone Bones_31;

		public OVRPlugin.Bone Bones_32;

		public OVRPlugin.Bone Bones_33;

		public OVRPlugin.Bone Bones_34;

		public OVRPlugin.Bone Bones_35;

		public OVRPlugin.Bone Bones_36;

		public OVRPlugin.Bone Bones_37;

		public OVRPlugin.Bone Bones_38;

		public OVRPlugin.Bone Bones_39;

		public OVRPlugin.Bone Bones_40;

		public OVRPlugin.Bone Bones_41;

		public OVRPlugin.Bone Bones_42;

		public OVRPlugin.Bone Bones_43;

		public OVRPlugin.Bone Bones_44;

		public OVRPlugin.Bone Bones_45;

		public OVRPlugin.Bone Bones_46;

		public OVRPlugin.Bone Bones_47;

		public OVRPlugin.Bone Bones_48;

		public OVRPlugin.Bone Bones_49;

		public OVRPlugin.Bone Bones_50;

		public OVRPlugin.Bone Bones_51;

		public OVRPlugin.Bone Bones_52;

		public OVRPlugin.Bone Bones_53;

		public OVRPlugin.Bone Bones_54;

		public OVRPlugin.Bone Bones_55;

		public OVRPlugin.Bone Bones_56;

		public OVRPlugin.Bone Bones_57;

		public OVRPlugin.Bone Bones_58;

		public OVRPlugin.Bone Bones_59;

		public OVRPlugin.Bone Bones_60;

		public OVRPlugin.Bone Bones_61;

		public OVRPlugin.Bone Bones_62;

		public OVRPlugin.Bone Bones_63;

		public OVRPlugin.Bone Bones_64;

		public OVRPlugin.Bone Bones_65;

		public OVRPlugin.Bone Bones_66;

		public OVRPlugin.Bone Bones_67;

		public OVRPlugin.Bone Bones_68;

		public OVRPlugin.Bone Bones_69;

		public OVRPlugin.BoneCapsule BoneCapsules_0;

		public OVRPlugin.BoneCapsule BoneCapsules_1;

		public OVRPlugin.BoneCapsule BoneCapsules_2;

		public OVRPlugin.BoneCapsule BoneCapsules_3;

		public OVRPlugin.BoneCapsule BoneCapsules_4;

		public OVRPlugin.BoneCapsule BoneCapsules_5;

		public OVRPlugin.BoneCapsule BoneCapsules_6;

		public OVRPlugin.BoneCapsule BoneCapsules_7;

		public OVRPlugin.BoneCapsule BoneCapsules_8;

		public OVRPlugin.BoneCapsule BoneCapsules_9;

		public OVRPlugin.BoneCapsule BoneCapsules_10;

		public OVRPlugin.BoneCapsule BoneCapsules_11;

		public OVRPlugin.BoneCapsule BoneCapsules_12;

		public OVRPlugin.BoneCapsule BoneCapsules_13;

		public OVRPlugin.BoneCapsule BoneCapsules_14;

		public OVRPlugin.BoneCapsule BoneCapsules_15;

		public OVRPlugin.BoneCapsule BoneCapsules_16;

		public OVRPlugin.BoneCapsule BoneCapsules_17;

		public OVRPlugin.BoneCapsule BoneCapsules_18;
	}

	private struct Skeleton3Internal
	{
		public OVRPlugin.SkeletonType Type;

		public uint NumBones;

		public uint NumBoneCapsules;

		public OVRPlugin.Bone Bones_0;

		public OVRPlugin.Bone Bones_1;

		public OVRPlugin.Bone Bones_2;

		public OVRPlugin.Bone Bones_3;

		public OVRPlugin.Bone Bones_4;

		public OVRPlugin.Bone Bones_5;

		public OVRPlugin.Bone Bones_6;

		public OVRPlugin.Bone Bones_7;

		public OVRPlugin.Bone Bones_8;

		public OVRPlugin.Bone Bones_9;

		public OVRPlugin.Bone Bones_10;

		public OVRPlugin.Bone Bones_11;

		public OVRPlugin.Bone Bones_12;

		public OVRPlugin.Bone Bones_13;

		public OVRPlugin.Bone Bones_14;

		public OVRPlugin.Bone Bones_15;

		public OVRPlugin.Bone Bones_16;

		public OVRPlugin.Bone Bones_17;

		public OVRPlugin.Bone Bones_18;

		public OVRPlugin.Bone Bones_19;

		public OVRPlugin.Bone Bones_20;

		public OVRPlugin.Bone Bones_21;

		public OVRPlugin.Bone Bones_22;

		public OVRPlugin.Bone Bones_23;

		public OVRPlugin.Bone Bones_24;

		public OVRPlugin.Bone Bones_25;

		public OVRPlugin.Bone Bones_26;

		public OVRPlugin.Bone Bones_27;

		public OVRPlugin.Bone Bones_28;

		public OVRPlugin.Bone Bones_29;

		public OVRPlugin.Bone Bones_30;

		public OVRPlugin.Bone Bones_31;

		public OVRPlugin.Bone Bones_32;

		public OVRPlugin.Bone Bones_33;

		public OVRPlugin.Bone Bones_34;

		public OVRPlugin.Bone Bones_35;

		public OVRPlugin.Bone Bones_36;

		public OVRPlugin.Bone Bones_37;

		public OVRPlugin.Bone Bones_38;

		public OVRPlugin.Bone Bones_39;

		public OVRPlugin.Bone Bones_40;

		public OVRPlugin.Bone Bones_41;

		public OVRPlugin.Bone Bones_42;

		public OVRPlugin.Bone Bones_43;

		public OVRPlugin.Bone Bones_44;

		public OVRPlugin.Bone Bones_45;

		public OVRPlugin.Bone Bones_46;

		public OVRPlugin.Bone Bones_47;

		public OVRPlugin.Bone Bones_48;

		public OVRPlugin.Bone Bones_49;

		public OVRPlugin.Bone Bones_50;

		public OVRPlugin.Bone Bones_51;

		public OVRPlugin.Bone Bones_52;

		public OVRPlugin.Bone Bones_53;

		public OVRPlugin.Bone Bones_54;

		public OVRPlugin.Bone Bones_55;

		public OVRPlugin.Bone Bones_56;

		public OVRPlugin.Bone Bones_57;

		public OVRPlugin.Bone Bones_58;

		public OVRPlugin.Bone Bones_59;

		public OVRPlugin.Bone Bones_60;

		public OVRPlugin.Bone Bones_61;

		public OVRPlugin.Bone Bones_62;

		public OVRPlugin.Bone Bones_63;

		public OVRPlugin.Bone Bones_64;

		public OVRPlugin.Bone Bones_65;

		public OVRPlugin.Bone Bones_66;

		public OVRPlugin.Bone Bones_67;

		public OVRPlugin.Bone Bones_68;

		public OVRPlugin.Bone Bones_69;

		public OVRPlugin.Bone Bones_70;

		public OVRPlugin.Bone Bones_71;

		public OVRPlugin.Bone Bones_72;

		public OVRPlugin.Bone Bones_73;

		public OVRPlugin.Bone Bones_74;

		public OVRPlugin.Bone Bones_75;

		public OVRPlugin.Bone Bones_76;

		public OVRPlugin.Bone Bones_77;

		public OVRPlugin.Bone Bones_78;

		public OVRPlugin.Bone Bones_79;

		public OVRPlugin.Bone Bones_80;

		public OVRPlugin.Bone Bones_81;

		public OVRPlugin.Bone Bones_82;

		public OVRPlugin.Bone Bones_83;

		public OVRPlugin.BoneCapsule BoneCapsules_0;

		public OVRPlugin.BoneCapsule BoneCapsules_1;

		public OVRPlugin.BoneCapsule BoneCapsules_2;

		public OVRPlugin.BoneCapsule BoneCapsules_3;

		public OVRPlugin.BoneCapsule BoneCapsules_4;

		public OVRPlugin.BoneCapsule BoneCapsules_5;

		public OVRPlugin.BoneCapsule BoneCapsules_6;

		public OVRPlugin.BoneCapsule BoneCapsules_7;

		public OVRPlugin.BoneCapsule BoneCapsules_8;

		public OVRPlugin.BoneCapsule BoneCapsules_9;

		public OVRPlugin.BoneCapsule BoneCapsules_10;

		public OVRPlugin.BoneCapsule BoneCapsules_11;

		public OVRPlugin.BoneCapsule BoneCapsules_12;

		public OVRPlugin.BoneCapsule BoneCapsules_13;

		public OVRPlugin.BoneCapsule BoneCapsules_14;

		public OVRPlugin.BoneCapsule BoneCapsules_15;

		public OVRPlugin.BoneCapsule BoneCapsules_16;

		public OVRPlugin.BoneCapsule BoneCapsules_17;

		public OVRPlugin.BoneCapsule BoneCapsules_18;
	}

	public enum MeshConstants
	{
		MaxVertices = 3000,
		MaxIndices = 18000
	}

	public enum MeshType
	{
		None = -1,
		HandLeft,
		HandRight,
		XRHandLeft = 4,
		XRHandRight
	}

	[StructLayout(LayoutKind.Sequential)]
	public class Mesh
	{
		public OVRPlugin.MeshType Type;

		public uint NumVertices;

		public uint NumIndices;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
		public OVRPlugin.Vector3f[] VertexPositions;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 18000)]
		public short[] Indices;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
		public OVRPlugin.Vector3f[] VertexNormals;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
		public OVRPlugin.Vector2f[] VertexUV0;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
		public OVRPlugin.Vector4s[] BlendIndices;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
		public OVRPlugin.Vector4f[] BlendWeights;
	}

	[Flags]
	public enum SpaceLocationFlags : ulong
	{
		OrientationValid = 1UL,
		PositionValid = 2UL,
		OrientationTracked = 4UL,
		PositionTracked = 8UL
	}

	public struct SpaceLocationf
	{
		public OVRPlugin.SpaceLocationFlags locationFlags;

		public OVRPlugin.Posef pose;
	}

	public enum BodyJointSet
	{
		[InspectorName(null)]
		None = -1,
		UpperBody,
		FullBody
	}

	public enum BodyTrackingFidelity2
	{
		Low = 1,
		High
	}

	public enum BodyTrackingCalibrationState
	{
		Valid = 1,
		Calibrating,
		Invalid
	}

	public struct BodyTrackingCalibrationInfo
	{
		public float BodyHeight;
	}

	public struct BodyJointLocation
	{
		public bool OrientationValid
		{
			get
			{
				return (this.LocationFlags & OVRPlugin.SpaceLocationFlags.OrientationValid) > (OVRPlugin.SpaceLocationFlags)0UL;
			}
		}

		public bool PositionValid
		{
			get
			{
				return (this.LocationFlags & OVRPlugin.SpaceLocationFlags.PositionValid) > (OVRPlugin.SpaceLocationFlags)0UL;
			}
		}

		public bool OrientationTracked
		{
			get
			{
				return (this.LocationFlags & OVRPlugin.SpaceLocationFlags.OrientationTracked) > (OVRPlugin.SpaceLocationFlags)0UL;
			}
		}

		public bool PositionTracked
		{
			get
			{
				return (this.LocationFlags & OVRPlugin.SpaceLocationFlags.PositionTracked) > (OVRPlugin.SpaceLocationFlags)0UL;
			}
		}

		public OVRPlugin.SpaceLocationFlags LocationFlags;

		public OVRPlugin.Posef Pose;

		public static readonly OVRPlugin.BodyJointLocation invalid = new OVRPlugin.BodyJointLocation
		{
			LocationFlags = (OVRPlugin.SpaceLocationFlags)0UL,
			Pose = OVRPlugin.Posef.identity
		};
	}

	public struct BodyState
	{
		public OVRPlugin.BodyJointLocation[] JointLocations;

		public float Confidence;

		public uint SkeletonChangedCount;

		public double Time;

		public OVRPlugin.BodyJointSet JointSet;

		public OVRPlugin.BodyTrackingCalibrationState CalibrationStatus;

		public OVRPlugin.BodyTrackingFidelity2 Fidelity;
	}

	private struct BodyStateInternal
	{
		public OVRPlugin.Bool IsActive;

		public float Confidence;

		public uint SkeletonChangedCount;

		public double Time;

		public OVRPlugin.BodyJointLocation JointLocation_0;

		public OVRPlugin.BodyJointLocation JointLocation_1;

		public OVRPlugin.BodyJointLocation JointLocation_2;

		public OVRPlugin.BodyJointLocation JointLocation_3;

		public OVRPlugin.BodyJointLocation JointLocation_4;

		public OVRPlugin.BodyJointLocation JointLocation_5;

		public OVRPlugin.BodyJointLocation JointLocation_6;

		public OVRPlugin.BodyJointLocation JointLocation_7;

		public OVRPlugin.BodyJointLocation JointLocation_8;

		public OVRPlugin.BodyJointLocation JointLocation_9;

		public OVRPlugin.BodyJointLocation JointLocation_10;

		public OVRPlugin.BodyJointLocation JointLocation_11;

		public OVRPlugin.BodyJointLocation JointLocation_12;

		public OVRPlugin.BodyJointLocation JointLocation_13;

		public OVRPlugin.BodyJointLocation JointLocation_14;

		public OVRPlugin.BodyJointLocation JointLocation_15;

		public OVRPlugin.BodyJointLocation JointLocation_16;

		public OVRPlugin.BodyJointLocation JointLocation_17;

		public OVRPlugin.BodyJointLocation JointLocation_18;

		public OVRPlugin.BodyJointLocation JointLocation_19;

		public OVRPlugin.BodyJointLocation JointLocation_20;

		public OVRPlugin.BodyJointLocation JointLocation_21;

		public OVRPlugin.BodyJointLocation JointLocation_22;

		public OVRPlugin.BodyJointLocation JointLocation_23;

		public OVRPlugin.BodyJointLocation JointLocation_24;

		public OVRPlugin.BodyJointLocation JointLocation_25;

		public OVRPlugin.BodyJointLocation JointLocation_26;

		public OVRPlugin.BodyJointLocation JointLocation_27;

		public OVRPlugin.BodyJointLocation JointLocation_28;

		public OVRPlugin.BodyJointLocation JointLocation_29;

		public OVRPlugin.BodyJointLocation JointLocation_30;

		public OVRPlugin.BodyJointLocation JointLocation_31;

		public OVRPlugin.BodyJointLocation JointLocation_32;

		public OVRPlugin.BodyJointLocation JointLocation_33;

		public OVRPlugin.BodyJointLocation JointLocation_34;

		public OVRPlugin.BodyJointLocation JointLocation_35;

		public OVRPlugin.BodyJointLocation JointLocation_36;

		public OVRPlugin.BodyJointLocation JointLocation_37;

		public OVRPlugin.BodyJointLocation JointLocation_38;

		public OVRPlugin.BodyJointLocation JointLocation_39;

		public OVRPlugin.BodyJointLocation JointLocation_40;

		public OVRPlugin.BodyJointLocation JointLocation_41;

		public OVRPlugin.BodyJointLocation JointLocation_42;

		public OVRPlugin.BodyJointLocation JointLocation_43;

		public OVRPlugin.BodyJointLocation JointLocation_44;

		public OVRPlugin.BodyJointLocation JointLocation_45;

		public OVRPlugin.BodyJointLocation JointLocation_46;

		public OVRPlugin.BodyJointLocation JointLocation_47;

		public OVRPlugin.BodyJointLocation JointLocation_48;

		public OVRPlugin.BodyJointLocation JointLocation_49;

		public OVRPlugin.BodyJointLocation JointLocation_50;

		public OVRPlugin.BodyJointLocation JointLocation_51;

		public OVRPlugin.BodyJointLocation JointLocation_52;

		public OVRPlugin.BodyJointLocation JointLocation_53;

		public OVRPlugin.BodyJointLocation JointLocation_54;

		public OVRPlugin.BodyJointLocation JointLocation_55;

		public OVRPlugin.BodyJointLocation JointLocation_56;

		public OVRPlugin.BodyJointLocation JointLocation_57;

		public OVRPlugin.BodyJointLocation JointLocation_58;

		public OVRPlugin.BodyJointLocation JointLocation_59;

		public OVRPlugin.BodyJointLocation JointLocation_60;

		public OVRPlugin.BodyJointLocation JointLocation_61;

		public OVRPlugin.BodyJointLocation JointLocation_62;

		public OVRPlugin.BodyJointLocation JointLocation_63;

		public OVRPlugin.BodyJointLocation JointLocation_64;

		public OVRPlugin.BodyJointLocation JointLocation_65;

		public OVRPlugin.BodyJointLocation JointLocation_66;

		public OVRPlugin.BodyJointLocation JointLocation_67;

		public OVRPlugin.BodyJointLocation JointLocation_68;

		public OVRPlugin.BodyJointLocation JointLocation_69;
	}

	private struct BodyState4Internal
	{
		public OVRPlugin.Bool IsActive;

		public float Confidence;

		public uint SkeletonChangedCount;

		public double Time;

		public OVRPlugin.BodyJointLocation JointLocation_0;

		public OVRPlugin.BodyJointLocation JointLocation_1;

		public OVRPlugin.BodyJointLocation JointLocation_2;

		public OVRPlugin.BodyJointLocation JointLocation_3;

		public OVRPlugin.BodyJointLocation JointLocation_4;

		public OVRPlugin.BodyJointLocation JointLocation_5;

		public OVRPlugin.BodyJointLocation JointLocation_6;

		public OVRPlugin.BodyJointLocation JointLocation_7;

		public OVRPlugin.BodyJointLocation JointLocation_8;

		public OVRPlugin.BodyJointLocation JointLocation_9;

		public OVRPlugin.BodyJointLocation JointLocation_10;

		public OVRPlugin.BodyJointLocation JointLocation_11;

		public OVRPlugin.BodyJointLocation JointLocation_12;

		public OVRPlugin.BodyJointLocation JointLocation_13;

		public OVRPlugin.BodyJointLocation JointLocation_14;

		public OVRPlugin.BodyJointLocation JointLocation_15;

		public OVRPlugin.BodyJointLocation JointLocation_16;

		public OVRPlugin.BodyJointLocation JointLocation_17;

		public OVRPlugin.BodyJointLocation JointLocation_18;

		public OVRPlugin.BodyJointLocation JointLocation_19;

		public OVRPlugin.BodyJointLocation JointLocation_20;

		public OVRPlugin.BodyJointLocation JointLocation_21;

		public OVRPlugin.BodyJointLocation JointLocation_22;

		public OVRPlugin.BodyJointLocation JointLocation_23;

		public OVRPlugin.BodyJointLocation JointLocation_24;

		public OVRPlugin.BodyJointLocation JointLocation_25;

		public OVRPlugin.BodyJointLocation JointLocation_26;

		public OVRPlugin.BodyJointLocation JointLocation_27;

		public OVRPlugin.BodyJointLocation JointLocation_28;

		public OVRPlugin.BodyJointLocation JointLocation_29;

		public OVRPlugin.BodyJointLocation JointLocation_30;

		public OVRPlugin.BodyJointLocation JointLocation_31;

		public OVRPlugin.BodyJointLocation JointLocation_32;

		public OVRPlugin.BodyJointLocation JointLocation_33;

		public OVRPlugin.BodyJointLocation JointLocation_34;

		public OVRPlugin.BodyJointLocation JointLocation_35;

		public OVRPlugin.BodyJointLocation JointLocation_36;

		public OVRPlugin.BodyJointLocation JointLocation_37;

		public OVRPlugin.BodyJointLocation JointLocation_38;

		public OVRPlugin.BodyJointLocation JointLocation_39;

		public OVRPlugin.BodyJointLocation JointLocation_40;

		public OVRPlugin.BodyJointLocation JointLocation_41;

		public OVRPlugin.BodyJointLocation JointLocation_42;

		public OVRPlugin.BodyJointLocation JointLocation_43;

		public OVRPlugin.BodyJointLocation JointLocation_44;

		public OVRPlugin.BodyJointLocation JointLocation_45;

		public OVRPlugin.BodyJointLocation JointLocation_46;

		public OVRPlugin.BodyJointLocation JointLocation_47;

		public OVRPlugin.BodyJointLocation JointLocation_48;

		public OVRPlugin.BodyJointLocation JointLocation_49;

		public OVRPlugin.BodyJointLocation JointLocation_50;

		public OVRPlugin.BodyJointLocation JointLocation_51;

		public OVRPlugin.BodyJointLocation JointLocation_52;

		public OVRPlugin.BodyJointLocation JointLocation_53;

		public OVRPlugin.BodyJointLocation JointLocation_54;

		public OVRPlugin.BodyJointLocation JointLocation_55;

		public OVRPlugin.BodyJointLocation JointLocation_56;

		public OVRPlugin.BodyJointLocation JointLocation_57;

		public OVRPlugin.BodyJointLocation JointLocation_58;

		public OVRPlugin.BodyJointLocation JointLocation_59;

		public OVRPlugin.BodyJointLocation JointLocation_60;

		public OVRPlugin.BodyJointLocation JointLocation_61;

		public OVRPlugin.BodyJointLocation JointLocation_62;

		public OVRPlugin.BodyJointLocation JointLocation_63;

		public OVRPlugin.BodyJointLocation JointLocation_64;

		public OVRPlugin.BodyJointLocation JointLocation_65;

		public OVRPlugin.BodyJointLocation JointLocation_66;

		public OVRPlugin.BodyJointLocation JointLocation_67;

		public OVRPlugin.BodyJointLocation JointLocation_68;

		public OVRPlugin.BodyJointLocation JointLocation_69;

		public OVRPlugin.BodyJointLocation JointLocation_70;

		public OVRPlugin.BodyJointLocation JointLocation_71;

		public OVRPlugin.BodyJointLocation JointLocation_72;

		public OVRPlugin.BodyJointLocation JointLocation_73;

		public OVRPlugin.BodyJointLocation JointLocation_74;

		public OVRPlugin.BodyJointLocation JointLocation_75;

		public OVRPlugin.BodyJointLocation JointLocation_76;

		public OVRPlugin.BodyJointLocation JointLocation_77;

		public OVRPlugin.BodyJointLocation JointLocation_78;

		public OVRPlugin.BodyJointLocation JointLocation_79;

		public OVRPlugin.BodyJointLocation JointLocation_80;

		public OVRPlugin.BodyJointLocation JointLocation_81;

		public OVRPlugin.BodyJointLocation JointLocation_82;

		public OVRPlugin.BodyJointLocation JointLocation_83;

		public OVRPlugin.BodyTrackingCalibrationState CalibrationStatus;

		public OVRPlugin.BodyTrackingFidelity2 Fidelity;
	}

	public struct FaceExpressionStatus
	{
		public bool IsValid;

		public bool IsEyeFollowingBlendshapesValid;
	}

	public struct FaceVisemesState
	{
		public bool IsValid;

		public float[] Visemes;

		public double Time;
	}

	public struct FaceState
	{
		public float[] ExpressionWeights;

		public float[] ExpressionWeightConfidences;

		public OVRPlugin.FaceExpressionStatus Status;

		public OVRPlugin.FaceTrackingDataSource DataSource;

		public double Time;
	}

	private struct FaceExpressionStatusInternal
	{
		public OVRPlugin.FaceExpressionStatus ToFaceExpressionStatus()
		{
			return new OVRPlugin.FaceExpressionStatus
			{
				IsValid = (this.IsValid == OVRPlugin.Bool.True),
				IsEyeFollowingBlendshapesValid = (this.IsEyeFollowingBlendshapesValid == OVRPlugin.Bool.True)
			};
		}

		public OVRPlugin.Bool IsValid;

		public OVRPlugin.Bool IsEyeFollowingBlendshapesValid;
	}

	private struct FaceStateInternal
	{
		public float ExpressionWeights_0;

		public float ExpressionWeights_1;

		public float ExpressionWeights_2;

		public float ExpressionWeights_3;

		public float ExpressionWeights_4;

		public float ExpressionWeights_5;

		public float ExpressionWeights_6;

		public float ExpressionWeights_7;

		public float ExpressionWeights_8;

		public float ExpressionWeights_9;

		public float ExpressionWeights_10;

		public float ExpressionWeights_11;

		public float ExpressionWeights_12;

		public float ExpressionWeights_13;

		public float ExpressionWeights_14;

		public float ExpressionWeights_15;

		public float ExpressionWeights_16;

		public float ExpressionWeights_17;

		public float ExpressionWeights_18;

		public float ExpressionWeights_19;

		public float ExpressionWeights_20;

		public float ExpressionWeights_21;

		public float ExpressionWeights_22;

		public float ExpressionWeights_23;

		public float ExpressionWeights_24;

		public float ExpressionWeights_25;

		public float ExpressionWeights_26;

		public float ExpressionWeights_27;

		public float ExpressionWeights_28;

		public float ExpressionWeights_29;

		public float ExpressionWeights_30;

		public float ExpressionWeights_31;

		public float ExpressionWeights_32;

		public float ExpressionWeights_33;

		public float ExpressionWeights_34;

		public float ExpressionWeights_35;

		public float ExpressionWeights_36;

		public float ExpressionWeights_37;

		public float ExpressionWeights_38;

		public float ExpressionWeights_39;

		public float ExpressionWeights_40;

		public float ExpressionWeights_41;

		public float ExpressionWeights_42;

		public float ExpressionWeights_43;

		public float ExpressionWeights_44;

		public float ExpressionWeights_45;

		public float ExpressionWeights_46;

		public float ExpressionWeights_47;

		public float ExpressionWeights_48;

		public float ExpressionWeights_49;

		public float ExpressionWeights_50;

		public float ExpressionWeights_51;

		public float ExpressionWeights_52;

		public float ExpressionWeights_53;

		public float ExpressionWeights_54;

		public float ExpressionWeights_55;

		public float ExpressionWeights_56;

		public float ExpressionWeights_57;

		public float ExpressionWeights_58;

		public float ExpressionWeights_59;

		public float ExpressionWeights_60;

		public float ExpressionWeights_61;

		public float ExpressionWeights_62;

		public float ExpressionWeightConfidences_0;

		public float ExpressionWeightConfidences_1;

		public OVRPlugin.FaceExpressionStatusInternal Status;

		public double Time;
	}

	private struct FaceState2Internal
	{
		public float ExpressionWeights_0;

		public float ExpressionWeights_1;

		public float ExpressionWeights_2;

		public float ExpressionWeights_3;

		public float ExpressionWeights_4;

		public float ExpressionWeights_5;

		public float ExpressionWeights_6;

		public float ExpressionWeights_7;

		public float ExpressionWeights_8;

		public float ExpressionWeights_9;

		public float ExpressionWeights_10;

		public float ExpressionWeights_11;

		public float ExpressionWeights_12;

		public float ExpressionWeights_13;

		public float ExpressionWeights_14;

		public float ExpressionWeights_15;

		public float ExpressionWeights_16;

		public float ExpressionWeights_17;

		public float ExpressionWeights_18;

		public float ExpressionWeights_19;

		public float ExpressionWeights_20;

		public float ExpressionWeights_21;

		public float ExpressionWeights_22;

		public float ExpressionWeights_23;

		public float ExpressionWeights_24;

		public float ExpressionWeights_25;

		public float ExpressionWeights_26;

		public float ExpressionWeights_27;

		public float ExpressionWeights_28;

		public float ExpressionWeights_29;

		public float ExpressionWeights_30;

		public float ExpressionWeights_31;

		public float ExpressionWeights_32;

		public float ExpressionWeights_33;

		public float ExpressionWeights_34;

		public float ExpressionWeights_35;

		public float ExpressionWeights_36;

		public float ExpressionWeights_37;

		public float ExpressionWeights_38;

		public float ExpressionWeights_39;

		public float ExpressionWeights_40;

		public float ExpressionWeights_41;

		public float ExpressionWeights_42;

		public float ExpressionWeights_43;

		public float ExpressionWeights_44;

		public float ExpressionWeights_45;

		public float ExpressionWeights_46;

		public float ExpressionWeights_47;

		public float ExpressionWeights_48;

		public float ExpressionWeights_49;

		public float ExpressionWeights_50;

		public float ExpressionWeights_51;

		public float ExpressionWeights_52;

		public float ExpressionWeights_53;

		public float ExpressionWeights_54;

		public float ExpressionWeights_55;

		public float ExpressionWeights_56;

		public float ExpressionWeights_57;

		public float ExpressionWeights_58;

		public float ExpressionWeights_59;

		public float ExpressionWeights_60;

		public float ExpressionWeights_61;

		public float ExpressionWeights_62;

		public float ExpressionWeights_63;

		public float ExpressionWeights_64;

		public float ExpressionWeights_65;

		public float ExpressionWeights_66;

		public float ExpressionWeights_67;

		public float ExpressionWeights_68;

		public float ExpressionWeights_69;

		public float ExpressionWeightConfidences_0;

		public float ExpressionWeightConfidences_1;

		public OVRPlugin.FaceExpressionStatusInternal Status;

		public OVRPlugin.FaceTrackingDataSource DataSource;

		public double Time;
	}

	private struct FaceVisemesStateInternal
	{
		public OVRPlugin.Bool IsValid;

		public float Visemes_0;

		public float Visemes_1;

		public float Visemes_2;

		public float Visemes_3;

		public float Visemes_4;

		public float Visemes_5;

		public float Visemes_6;

		public float Visemes_7;

		public float Visemes_8;

		public float Visemes_9;

		public float Visemes_10;

		public float Visemes_11;

		public float Visemes_12;

		public float Visemes_13;

		public float Visemes_14;

		public double Time;
	}

	public enum FaceRegionConfidence
	{
		Lower,
		Upper,
		Max
	}

	public enum FaceExpression
	{
		Invalid = -1,
		Brow_Lowerer_L,
		Brow_Lowerer_R,
		Cheek_Puff_L,
		Cheek_Puff_R,
		Cheek_Raiser_L,
		Cheek_Raiser_R,
		Cheek_Suck_L,
		Cheek_Suck_R,
		Chin_Raiser_B,
		Chin_Raiser_T,
		Dimpler_L,
		Dimpler_R,
		Eyes_Closed_L,
		Eyes_Closed_R,
		Eyes_Look_Down_L,
		Eyes_Look_Down_R,
		Eyes_Look_Left_L,
		Eyes_Look_Left_R,
		Eyes_Look_Right_L,
		Eyes_Look_Right_R,
		Eyes_Look_Up_L,
		Eyes_Look_Up_R,
		Inner_Brow_Raiser_L,
		Inner_Brow_Raiser_R,
		Jaw_Drop,
		Jaw_Sideways_Left,
		Jaw_Sideways_Right,
		Jaw_Thrust,
		Lid_Tightener_L,
		Lid_Tightener_R,
		Lip_Corner_Depressor_L,
		Lip_Corner_Depressor_R,
		Lip_Corner_Puller_L,
		Lip_Corner_Puller_R,
		Lip_Funneler_LB,
		Lip_Funneler_LT,
		Lip_Funneler_RB,
		Lip_Funneler_RT,
		Lip_Pressor_L,
		Lip_Pressor_R,
		Lip_Pucker_L,
		Lip_Pucker_R,
		Lip_Stretcher_L,
		Lip_Stretcher_R,
		Lip_Suck_LB,
		Lip_Suck_LT,
		Lip_Suck_RB,
		Lip_Suck_RT,
		Lip_Tightener_L,
		Lip_Tightener_R,
		Lips_Toward,
		Lower_Lip_Depressor_L,
		Lower_Lip_Depressor_R,
		Mouth_Left,
		Mouth_Right,
		Nose_Wrinkler_L,
		Nose_Wrinkler_R,
		Outer_Brow_Raiser_L,
		Outer_Brow_Raiser_R,
		Upper_Lid_Raiser_L,
		Upper_Lid_Raiser_R,
		Upper_Lip_Raiser_L,
		Upper_Lip_Raiser_R,
		Max
	}

	public enum FaceExpression2
	{
		Invalid = -1,
		Brow_Lowerer_L,
		Brow_Lowerer_R,
		Cheek_Puff_L,
		Cheek_Puff_R,
		Cheek_Raiser_L,
		Cheek_Raiser_R,
		Cheek_Suck_L,
		Cheek_Suck_R,
		Chin_Raiser_B,
		Chin_Raiser_T,
		Dimpler_L,
		Dimpler_R,
		Eyes_Closed_L,
		Eyes_Closed_R,
		Eyes_Look_Down_L,
		Eyes_Look_Down_R,
		Eyes_Look_Left_L,
		Eyes_Look_Left_R,
		Eyes_Look_Right_L,
		Eyes_Look_Right_R,
		Eyes_Look_Up_L,
		Eyes_Look_Up_R,
		Inner_Brow_Raiser_L,
		Inner_Brow_Raiser_R,
		Jaw_Drop,
		Jaw_Sideways_Left,
		Jaw_Sideways_Right,
		Jaw_Thrust,
		Lid_Tightener_L,
		Lid_Tightener_R,
		Lip_Corner_Depressor_L,
		Lip_Corner_Depressor_R,
		Lip_Corner_Puller_L,
		Lip_Corner_Puller_R,
		Lip_Funneler_LB,
		Lip_Funneler_LT,
		Lip_Funneler_RB,
		Lip_Funneler_RT,
		Lip_Pressor_L,
		Lip_Pressor_R,
		Lip_Pucker_L,
		Lip_Pucker_R,
		Lip_Stretcher_L,
		Lip_Stretcher_R,
		Lip_Suck_LB,
		Lip_Suck_LT,
		Lip_Suck_RB,
		Lip_Suck_RT,
		Lip_Tightener_L,
		Lip_Tightener_R,
		Lips_Toward,
		Lower_Lip_Depressor_L,
		Lower_Lip_Depressor_R,
		Mouth_Left,
		Mouth_Right,
		Nose_Wrinkler_L,
		Nose_Wrinkler_R,
		Outer_Brow_Raiser_L,
		Outer_Brow_Raiser_R,
		Upper_Lid_Raiser_L,
		Upper_Lid_Raiser_R,
		Upper_Lip_Raiser_L,
		Upper_Lip_Raiser_R,
		Tongue_Tip_Interdental,
		Tongue_Tip_Alveolar,
		Tongue_Front_Dorsal_Palate,
		Tongue_Mid_Dorsal_Palate,
		Tongue_Back_Dorsal_Velar,
		Tongue_Out,
		Tongue_Retreat,
		Max
	}

	public enum FaceTrackingDataSource
	{
		Visual,
		Audio,
		Count
	}

	public enum FaceViseme
	{
		Invalid = -1,
		SIL,
		PP,
		FF,
		TH,
		DD,
		KK,
		CH,
		SS,
		NN,
		RR,
		AA,
		E,
		IH,
		OH,
		OU,
		Count
	}

	public enum FaceConstants
	{
		MaxFaceExpressions = 63,
		MaxFaceRegionConfidences = 2,
		MaxFaceExpressions2 = 70,
		FaceVisemesCount = 15
	}

	public struct EyeGazeState
	{
		public bool IsValid
		{
			get
			{
				return this._isValid == OVRPlugin.Bool.True;
			}
		}

		public OVRPlugin.Posef Pose;

		public float Confidence;

		internal OVRPlugin.Bool _isValid;
	}

	public struct EyeGazesState
	{
		public OVRPlugin.EyeGazeState[] EyeGazes;

		public double Time;
	}

	private struct EyeGazesStateInternal
	{
		public OVRPlugin.EyeGazeState EyeGazes_0;

		public OVRPlugin.EyeGazeState EyeGazes_1;

		public double Time;
	}

	public enum ColorSpace
	{
		Unknown,
		Unmanaged,
		Rec_2020,
		Rec_709,
		Rift_CV1,
		Rift_S,
		Quest,
		P3,
		Adobe_RGB
	}

	public enum EventType
	{
		Unknown,
		DisplayRefreshRateChanged,
		SpatialAnchorCreateComplete = 49,
		SpaceSetComponentStatusComplete,
		SpaceQueryResults,
		SpaceQueryComplete,
		SpaceSaveComplete,
		SpaceEraseComplete,
		SpaceShareResult = 56,
		SpaceListSaveResult,
		SpaceShareToGroupsComplete,
		SceneCaptureComplete = 100,
		VirtualKeyboardCommitText = 201,
		VirtualKeyboardBackspace,
		VirtualKeyboardEnter,
		VirtualKeyboardShown,
		VirtualKeyboardHidden,
		SpaceDiscoveryResultsAvailable = 300,
		SpaceDiscoveryComplete,
		SpacesSaveResult,
		SpacesEraseResult,
		ColocationSessionStartAdvertisementComplete = 370,
		ColocationSessionAdvertisementComplete,
		ColocationSessionStopAdvertisementComplete,
		ColocationSessionStartDiscoveryComplete,
		ColocationSessionDiscoveryResult,
		ColocationSessionDiscoveryComplete,
		ColocationSessionStopDiscoveryComplete,
		PassthroughLayerResumed = 500,
		BoundaryVisibilityChanged = 510,
		CreateDynamicObjectTrackerResult = 650,
		SetDynamicObjectTrackedClassesResult,
		ReferenceSpaceChangePending = 1160
	}

	public struct EventDataBuffer
	{
		public OVRPlugin.EventType EventType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4000)]
		public byte[] EventData;
	}

	public struct RenderModelProperties
	{
		public string ModelName;

		public ulong ModelKey;

		public uint VendorId;

		public uint ModelVersion;
	}

	private struct RenderModelPropertiesInternal
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] ModelName;

		public ulong ModelKey;

		public uint VendorId;

		public uint ModelVersion;
	}

	[Flags]
	public enum RenderModelFlags
	{
		SupportsGltf20Subset1 = 1,
		SupportsGltf20Subset2 = 2
	}

	public enum VirtualKeyboardLocationType
	{
		Custom,
		Far,
		Direct
	}

	public struct VirtualKeyboardSpaceCreateInfo
	{
		public OVRPlugin.VirtualKeyboardLocationType locationType;

		public OVRPlugin.Posef pose;

		public OVRPlugin.TrackingOrigin trackingOriginType;
	}

	public struct VirtualKeyboardLocationInfo
	{
		public OVRPlugin.VirtualKeyboardLocationType locationType;

		public OVRPlugin.Posef pose;

		public float scale;

		public OVRPlugin.TrackingOrigin trackingOriginType;
	}

	public struct VirtualKeyboardCreateInfo
	{
	}

	public enum VirtualKeyboardInputSource
	{
		Invalid,
		ControllerRayLeft,
		ControllerRayRight,
		HandRayLeft,
		HandRayRight,
		ControllerDirectLeft,
		ControllerDirectRight,
		HandDirectIndexTipLeft,
		HandDirectIndexTipRight,
		EnumSize = 2147483647
	}

	[Flags]
	public enum VirtualKeyboardInputStateFlags : ulong
	{
		IsPressed = 1UL
	}

	public struct VirtualKeyboardInputInfo
	{
		public OVRPlugin.VirtualKeyboardInputSource inputSource;

		public OVRPlugin.Posef inputPose;

		public OVRPlugin.VirtualKeyboardInputStateFlags inputState;

		public OVRPlugin.TrackingOrigin inputTrackingOriginType;
	}

	public struct VirtualKeyboardModelAnimationState
	{
		public int AnimationIndex;

		public float Fraction;
	}

	public struct VirtualKeyboardModelAnimationStates
	{
		public OVRPlugin.VirtualKeyboardModelAnimationState[] States;
	}

	public struct VirtualKeyboardModelAnimationStatesInternal
	{
		public uint StateCapacityInput;

		public uint StateCountOutput;

		public IntPtr StatesBuffer;
	}

	public struct VirtualKeyboardTextureIds
	{
		public ulong[] TextureIds;
	}

	public struct VirtualKeyboardTextureIdsInternal
	{
		public uint TextureIdCapacityInput;

		public uint TextureIdCountOutput;

		public IntPtr TextureIdsBuffer;
	}

	public struct VirtualKeyboardTextureData
	{
		public uint TextureWidth;

		public uint TextureHeight;

		public uint BufferCapacityInput;

		public uint BufferCountOutput;

		public IntPtr Buffer;
	}

	public struct VirtualKeyboardModelVisibility
	{
		public bool Visible
		{
			get
			{
				return this._visible == OVRPlugin.Bool.True;
			}
			set
			{
				this._visible = (value ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
			}
		}

		internal OVRPlugin.Bool _visible;
	}

	public enum InsightPassthroughColorMapType
	{
		None,
		MonoToRgba,
		MonoToMono,
		BrightnessContrastSaturation = 4,
		ColorLut = 6,
		InterpolatedColorLut
	}

	public enum InsightPassthroughStyleFlags
	{
		HasTextureOpacityFactor = 1,
		HasEdgeColor,
		HasTextureColorMap = 4
	}

	public struct InsightPassthroughStyle
	{
		public OVRPlugin.InsightPassthroughStyleFlags Flags;

		public float TextureOpacityFactor;

		public OVRPlugin.Colorf EdgeColor;

		public OVRPlugin.InsightPassthroughColorMapType TextureColorMapType;

		public uint TextureColorMapDataSize;

		public IntPtr TextureColorMapData;
	}

	public struct InsightPassthroughStyle2
	{
		public void CopyTo(ref OVRPlugin.InsightPassthroughStyle target)
		{
			target.Flags = this.Flags;
			target.TextureOpacityFactor = this.TextureOpacityFactor;
			target.EdgeColor = this.EdgeColor;
			target.TextureColorMapType = this.TextureColorMapType;
			target.TextureColorMapDataSize = this.TextureColorMapDataSize;
			target.TextureColorMapData = this.TextureColorMapData;
		}

		public OVRPlugin.InsightPassthroughStyleFlags Flags;

		public float TextureOpacityFactor;

		public OVRPlugin.Colorf EdgeColor;

		public OVRPlugin.InsightPassthroughColorMapType TextureColorMapType;

		public uint TextureColorMapDataSize;

		public IntPtr TextureColorMapData;

		public ulong LutSource;

		public ulong LutTarget;

		public float LutWeight;
	}

	public enum PassthroughColorLutChannels
	{
		Rgb = 1,
		Rgba
	}

	public struct PassthroughColorLutData
	{
		public uint BufferSize;

		public IntPtr Buffer;
	}

	public struct InsightPassthroughKeyboardHandsIntensity
	{
		public float LeftHandIntensity;

		public float RightHandIntensity;
	}

	public enum PassthroughCapabilityFlags
	{
		Passthrough = 1,
		Color,
		Depth = 4
	}

	public enum PassthroughCapabilityFields
	{
		Flags = 1,
		MaxColorLutResolution
	}

	public struct PassthroughCapabilities
	{
		public OVRPlugin.PassthroughCapabilityFields Fields;

		public OVRPlugin.PassthroughCapabilityFlags Flags;

		public uint MaxColorLutResolution;
	}

	public enum SpaceComponentType
	{
		Locatable,
		Storable,
		Sharable,
		Bounded2D,
		Bounded3D,
		SemanticLabels,
		RoomLayout,
		SpaceContainer,
		MarkerPayload = 1000576000,
		TriangleMesh = 1000269000,
		DynamicObject = 1000288007
	}

	public enum SpaceStorageLocation
	{
		Invalid,
		Local,
		Cloud
	}

	public enum SpaceStoragePersistenceMode
	{
		Invalid,
		Indefinite
	}

	public enum SpaceQueryActionType
	{
		Load
	}

	public enum SpaceQueryType
	{
		Action
	}

	public enum SpaceQueryFilterType
	{
		None,
		Ids,
		Components,
		Group
	}

	public struct SpatialAnchorCreateInfo
	{
		public OVRPlugin.TrackingOrigin BaseTracking;

		public OVRPlugin.Posef PoseInSpace;

		public double Time;
	}

	public struct SpaceFilterInfoIds
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		public Guid[] Ids;

		public int NumIds;
	}

	public struct SpaceFilterInfoComponents
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public OVRPlugin.SpaceComponentType[] Components;

		public int NumComponents;
	}

	public struct SpaceQueryInfo
	{
		public OVRPlugin.SpaceQueryType QueryType;

		public int MaxQuerySpaces;

		public double Timeout;

		public OVRPlugin.SpaceStorageLocation Location;

		public OVRPlugin.SpaceQueryActionType ActionType;

		public OVRPlugin.SpaceQueryFilterType FilterType;

		public OVRPlugin.SpaceFilterInfoIds IdInfo;

		public OVRPlugin.SpaceFilterInfoComponents ComponentsInfo;
	}

	public struct SpaceQueryInfo2
	{
		public OVRPlugin.SpaceQueryType QueryType;

		public int MaxQuerySpaces;

		public double Timeout;

		public OVRPlugin.SpaceStorageLocation Location;

		public OVRPlugin.SpaceQueryActionType ActionType;

		public OVRPlugin.SpaceQueryFilterType FilterType;

		public OVRPlugin.SpaceFilterInfoIds IdInfo;

		public OVRPlugin.SpaceFilterInfoComponents ComponentsInfo;

		public Guid GroupUuidInfo;
	}

	public struct SpaceQueryResult
	{
		public ulong space;

		public Guid uuid;
	}

	public struct ColocationSessionStartAdvertisementInfo
	{
		public uint PeerMetadataCount;

		public unsafe byte* GroupMetadata;
	}

	public enum ShareSpacesRecipientType
	{
		Group = 1
	}

	public struct ShareSpacesRecipientInfoBase
	{
	}

	public struct ShareSpacesInfo
	{
		public OVRPlugin.ShareSpacesRecipientType RecipientType;

		public unsafe OVRPlugin.ShareSpacesRecipientInfoBase* RecipientInfo;

		public uint SpaceCount;

		public unsafe ulong* Spaces;
	}

	public struct ShareSpacesGroupRecipientInfo
	{
		public uint GroupCount;

		public unsafe Guid* GroupUuids;
	}

	public enum SpaceDiscoveryFilterType
	{
		None,
		Ids = 2,
		Component
	}

	public class Media
	{
		public static bool Initialize()
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_Initialize() == OVRPlugin.Result.Success;
		}

		public static bool Shutdown()
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_Shutdown() == OVRPlugin.Result.Success;
		}

		public static bool GetInitialized()
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				return OVRPlugin.OVRP_1_38_0.ovrp_Media_GetInitialized(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
			}
			return false;
		}

		public static bool Update()
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_Update() == OVRPlugin.Result.Success;
		}

		public static OVRPlugin.Media.MrcActivationMode GetMrcActivationMode()
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version))
			{
				return OVRPlugin.Media.MrcActivationMode.Automatic;
			}
			OVRPlugin.Media.MrcActivationMode result;
			if (OVRPlugin.OVRP_1_38_0.ovrp_Media_GetMrcActivationMode(out result) == OVRPlugin.Result.Success)
			{
				return result;
			}
			return OVRPlugin.Media.MrcActivationMode.Automatic;
		}

		public static bool SetMrcActivationMode(OVRPlugin.Media.MrcActivationMode mode)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_SetMrcActivationMode(mode) == OVRPlugin.Result.Success;
		}

		public static bool SetPlatformInitialized()
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_54_0.version && OVRPlugin.OVRP_1_54_0.ovrp_Media_SetPlatformInitialized() == OVRPlugin.Result.Success;
		}

		public static OVRPlugin.Media.PlatformCameraMode GetPlatformCameraMode()
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_57_0.version))
			{
				return OVRPlugin.Media.PlatformCameraMode.Initialized;
			}
			OVRPlugin.Media.PlatformCameraMode result;
			if (OVRPlugin.OVRP_1_57_0.ovrp_Media_GetPlatformCameraMode(out result) == OVRPlugin.Result.Success)
			{
				return result;
			}
			return OVRPlugin.Media.PlatformCameraMode.Initialized;
		}

		public static bool SetPlatformCameraMode(OVRPlugin.Media.PlatformCameraMode mode)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_57_0.version && OVRPlugin.OVRP_1_57_0.ovrp_Media_SetPlatformCameraMode(mode) == OVRPlugin.Result.Success;
		}

		public static bool IsMrcEnabled()
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_IsMrcEnabled(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}

		public static bool IsMrcActivated()
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_IsMrcActivated(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}

		public static bool UseMrcDebugCamera()
		{
			OVRPlugin.Bool @bool;
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_UseMrcDebugCamera(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
		}

		public static bool SetMrcInputVideoBufferType(OVRPlugin.Media.InputVideoBufferType videoBufferType)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_SetMrcInputVideoBufferType(videoBufferType) == OVRPlugin.Result.Success;
		}

		public static OVRPlugin.Media.InputVideoBufferType GetMrcInputVideoBufferType()
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
			{
				OVRPlugin.Media.InputVideoBufferType result = OVRPlugin.Media.InputVideoBufferType.Memory;
				OVRPlugin.OVRP_1_38_0.ovrp_Media_GetMrcInputVideoBufferType(ref result);
				return result;
			}
			return OVRPlugin.Media.InputVideoBufferType.Memory;
		}

		public static bool SetMrcFrameSize(int frameWidth, int frameHeight)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_SetMrcFrameSize(frameWidth, frameHeight) == OVRPlugin.Result.Success;
		}

		public static void GetMrcFrameSize(out int frameWidth, out int frameHeight)
		{
			frameWidth = -1;
			frameHeight = -1;
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
			{
				OVRPlugin.OVRP_1_38_0.ovrp_Media_GetMrcFrameSize(ref frameWidth, ref frameHeight);
			}
		}

		public static bool SetMrcAudioSampleRate(int sampleRate)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_SetMrcAudioSampleRate(sampleRate) == OVRPlugin.Result.Success;
		}

		public static int GetMrcAudioSampleRate()
		{
			int result = 0;
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
			{
				OVRPlugin.OVRP_1_38_0.ovrp_Media_GetMrcAudioSampleRate(ref result);
			}
			return result;
		}

		public static bool SetMrcFrameImageFlipped(bool imageFlipped)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_SetMrcFrameImageFlipped(imageFlipped ? OVRPlugin.Bool.True : OVRPlugin.Bool.False) == OVRPlugin.Result.Success;
		}

		public static bool GetMrcFrameImageFlipped()
		{
			OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version)
			{
				OVRPlugin.OVRP_1_38_0.ovrp_Media_GetMrcFrameImageFlipped(ref @bool);
			}
			return @bool == OVRPlugin.Bool.True;
		}

		public static bool EncodeMrcFrame(IntPtr textureHandle, IntPtr fgTextureHandle, float[] audioData, int audioFrames, int audioChannels, double timestamp, double poseTime, ref int outSyncId)
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version))
			{
				return false;
			}
			if (textureHandle == IntPtr.Zero)
			{
				Debug.LogError("EncodeMrcFrame: textureHandle is null");
				return false;
			}
			if (OVRPlugin.Media.GetMrcInputVideoBufferType() != OVRPlugin.Media.InputVideoBufferType.TextureHandle)
			{
				Debug.LogError("EncodeMrcFrame: videoBufferType mismatch");
				return false;
			}
			GCHandle gchandle = default(GCHandle);
			IntPtr intPtr = IntPtr.Zero;
			int audioDataLen = 0;
			if (audioData != null)
			{
				gchandle = GCHandle.Alloc(audioData, GCHandleType.Pinned);
				intPtr = gchandle.AddrOfPinnedObject();
				audioDataLen = audioFrames * 4;
			}
			OVRPlugin.Result result;
			if (fgTextureHandle == IntPtr.Zero)
			{
				if (OVRPlugin.version >= OVRPlugin.OVRP_1_49_0.version)
				{
					result = OVRPlugin.OVRP_1_49_0.ovrp_Media_EncodeMrcFrameWithPoseTime(textureHandle, intPtr, audioDataLen, audioChannels, timestamp, poseTime, ref outSyncId);
				}
				else
				{
					result = OVRPlugin.OVRP_1_38_0.ovrp_Media_EncodeMrcFrame(textureHandle, intPtr, audioDataLen, audioChannels, timestamp, ref outSyncId);
				}
			}
			else if (OVRPlugin.version >= OVRPlugin.OVRP_1_49_0.version)
			{
				result = OVRPlugin.OVRP_1_49_0.ovrp_Media_EncodeMrcFrameDualTexturesWithPoseTime(textureHandle, fgTextureHandle, intPtr, audioDataLen, audioChannels, timestamp, poseTime, ref outSyncId);
			}
			else
			{
				result = OVRPlugin.OVRP_1_38_0.ovrp_Media_EncodeMrcFrameWithDualTextures(textureHandle, fgTextureHandle, intPtr, audioDataLen, audioChannels, timestamp, ref outSyncId);
			}
			if (audioData != null)
			{
				gchandle.Free();
			}
			return result == OVRPlugin.Result.Success;
		}

		public static bool EncodeMrcFrame(RenderTexture frame, float[] audioData, int audioFrames, int audioChannels, double timestamp, double poseTime, ref int outSyncId)
		{
			if (!(OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version))
			{
				return false;
			}
			if (frame == null)
			{
				Debug.LogError("EncodeMrcFrame: frame is null");
				return false;
			}
			if (OVRPlugin.Media.GetMrcInputVideoBufferType() != OVRPlugin.Media.InputVideoBufferType.Memory)
			{
				Debug.LogError("EncodeMrcFrame: videoBufferType mismatch");
				return false;
			}
			GCHandle gchandle = default(GCHandle);
			IntPtr rawBuffer = IntPtr.Zero;
			if (OVRPlugin.Media.cachedTexture == null || OVRPlugin.Media.cachedTexture.width != frame.width || OVRPlugin.Media.cachedTexture.height != frame.height)
			{
				OVRPlugin.Media.cachedTexture = new Texture2D(frame.width, frame.height, TextureFormat.ARGB32, false);
			}
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = frame;
			OVRPlugin.Media.cachedTexture.ReadPixels(new Rect(0f, 0f, (float)frame.width, (float)frame.height), 0, 0);
			RenderTexture.active = active;
			gchandle = GCHandle.Alloc(OVRPlugin.Media.cachedTexture.GetPixels32(0), GCHandleType.Pinned);
			rawBuffer = gchandle.AddrOfPinnedObject();
			GCHandle gchandle2 = default(GCHandle);
			IntPtr audioDataPtr = IntPtr.Zero;
			int audioDataLen = 0;
			if (audioData != null)
			{
				gchandle2 = GCHandle.Alloc(audioData, GCHandleType.Pinned);
				audioDataPtr = gchandle2.AddrOfPinnedObject();
				audioDataLen = audioFrames * 4;
			}
			OVRPlugin.Result result;
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_49_0.version)
			{
				result = OVRPlugin.OVRP_1_49_0.ovrp_Media_EncodeMrcFrameWithPoseTime(rawBuffer, audioDataPtr, audioDataLen, audioChannels, timestamp, poseTime, ref outSyncId);
			}
			else
			{
				result = OVRPlugin.OVRP_1_38_0.ovrp_Media_EncodeMrcFrame(rawBuffer, audioDataPtr, audioDataLen, audioChannels, timestamp, ref outSyncId);
			}
			gchandle.Free();
			if (audioData != null)
			{
				gchandle2.Free();
			}
			return result == OVRPlugin.Result.Success;
		}

		public static bool SyncMrcFrame(int syncId)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_38_0.version && OVRPlugin.OVRP_1_38_0.ovrp_Media_SyncMrcFrame(syncId) == OVRPlugin.Result.Success;
		}

		public static bool SetAvailableQueueIndexVulkan(uint queueIndexVk)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_45_0.version && OVRPlugin.OVRP_1_45_0.ovrp_Media_SetAvailableQueueIndexVulkan(queueIndexVk) == OVRPlugin.Result.Success;
		}

		public static bool SetMrcHeadsetControllerPose(OVRPlugin.Posef headsetPose, OVRPlugin.Posef leftControllerPose, OVRPlugin.Posef rightControllerPose)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_49_0.version && OVRPlugin.OVRP_1_49_0.ovrp_Media_SetHeadsetControllerPose(headsetPose, leftControllerPose, rightControllerPose) == OVRPlugin.Result.Success;
		}

		public static bool IsCastingToRemoteClient()
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_66_0.version)
			{
				OVRPlugin.Bool @bool = OVRPlugin.Bool.False;
				return OVRPlugin.OVRP_1_66_0.ovrp_Media_IsCastingToRemoteClient(out @bool) == OVRPlugin.Result.Success && @bool == OVRPlugin.Bool.True;
			}
			return false;
		}

		private static Texture2D cachedTexture;

		public enum MrcActivationMode
		{
			Automatic,
			Disabled,
			EnumSize = 2147483647
		}

		public enum PlatformCameraMode
		{
			Disabled = -1,
			Initialized,
			UserControlled,
			SmartNavigated,
			StabilizedPoV,
			RemoteDroneControlled,
			RemoteSpatialMapped,
			SpectatorMode,
			MobileMRC,
			EnumSize = 2147483647
		}

		public enum InputVideoBufferType
		{
			Memory,
			TextureHandle,
			EnumSize = 2147483647
		}
	}

	private delegate OVRPlugin.Bone GetBoneSkeleton2Delegate();

	private delegate OVRPlugin.Bone GetBoneSkeleton3Delegate();

	public delegate IntPtr VirtualKeyboardModelAnimationStateBufferProvider(int minimumBufferLength, int stateCount);

	public delegate void VirtualKeyboardModelAnimationStateHandler(ref OVRPlugin.VirtualKeyboardModelAnimationState state);

	public delegate void OpenXREventDelegateType(IntPtr data, IntPtr context);

	private struct SpaceContainerInternal
	{
		public int uuidCapacityInput;

		public int uuidCountOutput;

		public IntPtr uuids;
	}

	private struct SpaceSemanticLabelInternal
	{
		public int byteCapacityInput;

		public int byteCountOutput;

		public IntPtr labels;
	}

	public struct RoomLayout
	{
		public Guid floorUuid;

		public Guid ceilingUuid;

		public Guid[] wallUuids;
	}

	internal struct RoomLayoutInternal
	{
		public Guid floorUuid;

		public Guid ceilingUuid;

		public int wallUuidCapacityInput;

		public int wallUuidCountOutput;

		public IntPtr wallUuids;
	}

	private struct PolygonalBoundary2DInternal
	{
		public int vertexCapacityInput;

		public int vertexCountOutput;

		public IntPtr vertices;
	}

	private struct SceneCaptureRequestInternal
	{
		public int requestByteCount;

		[MarshalAs(UnmanagedType.LPStr)]
		public string request;
	}

	private struct PinnedArray<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		public PinnedArray(T[] array)
		{
			this._handle = GCHandle.Alloc(array, GCHandleType.Pinned);
		}

		public void Dispose()
		{
			this._handle.Free();
		}

		public static implicit operator IntPtr(OVRPlugin.PinnedArray<T> pinnedArray)
		{
			return pinnedArray._handle.AddrOfPinnedObject();
		}

		private GCHandle _handle;
	}

	public struct SpaceDiscoveryResult
	{
		public ulong Space;

		public Guid Uuid;
	}

	public struct SpaceDiscoveryResults
	{
		public uint ResultCapacityInput;

		public uint ResultCountOutput;

		public unsafe OVRPlugin.SpaceDiscoveryResult* Results;
	}

	public struct SpaceDiscoveryFilterInfoHeader
	{
		public OVRPlugin.SpaceDiscoveryFilterType Type;
	}

	public struct SpaceDiscoveryFilterInfoIds
	{
		public OVRPlugin.SpaceDiscoveryFilterType Type;

		public int NumIds;

		public unsafe Guid* Ids;
	}

	public struct SpaceDiscoveryFilterInfoComponents
	{
		public OVRPlugin.SpaceDiscoveryFilterType Type;

		public OVRPlugin.SpaceComponentType Component;
	}

	public struct SpaceDiscoveryInfo
	{
		public uint NumFilters;

		public unsafe OVRPlugin.SpaceDiscoveryFilterInfoHeader** Filters;
	}

	private struct TriangleMeshInternal
	{
		public int vertexCapacityInput;

		public int vertexCountOutput;

		public IntPtr vertices;

		public int indexCapacityInput;

		public int indexCountOutput;

		public IntPtr indices;
	}

	[Flags]
	public enum PassthroughPreferenceFields
	{
		Flags = 1
	}

	[Flags]
	public enum PassthroughPreferenceFlags : long
	{
		DefaultToActive = 1L
	}

	public struct PassthroughPreferences
	{
		public OVRPlugin.PassthroughPreferenceFields Fields;

		public OVRPlugin.PassthroughPreferenceFlags Flags;
	}

	public class Ktx
	{
		public static IntPtr LoadKtxFromMemory(IntPtr dataPtr, uint length)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return IntPtr.Zero;
			}
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version)
			{
				IntPtr zero = IntPtr.Zero;
				OVRPlugin.OVRP_1_65_0.ovrp_KtxLoadFromMemory(ref dataPtr, length, ref zero);
				return zero;
			}
			return IntPtr.Zero;
		}

		public static uint GetKtxTextureWidth(IntPtr texture)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return 0U;
			}
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version)
			{
				uint result = 0U;
				OVRPlugin.OVRP_1_65_0.ovrp_KtxTextureWidth(texture, ref result);
				return result;
			}
			return 0U;
		}

		public static uint GetKtxTextureHeight(IntPtr texture)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return 0U;
			}
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version)
			{
				uint result = 0U;
				OVRPlugin.OVRP_1_65_0.ovrp_KtxTextureHeight(texture, ref result);
				return result;
			}
			return 0U;
		}

		public static bool TranscodeKtxTexture(IntPtr texture, uint format)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return false;
			}
			return OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version && OVRPlugin.OVRP_1_65_0.ovrp_KtxTranscode(texture, format) == OVRPlugin.Result.Success;
		}

		public static uint GetKtxTextureSize(IntPtr texture)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return 0U;
			}
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version)
			{
				uint result = 0U;
				OVRPlugin.OVRP_1_65_0.ovrp_KtxTextureSize(texture, ref result);
				return result;
			}
			return 0U;
		}

		public static bool GetKtxTextureData(IntPtr texture, IntPtr textureData, uint bufferSize)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return false;
			}
			return OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version && OVRPlugin.OVRP_1_65_0.ovrp_KtxGetTextureData(texture, textureData, bufferSize) == OVRPlugin.Result.Success;
		}

		public static bool DestroyKtxTexture(IntPtr texture)
		{
			if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
			{
				Debug.LogWarning("KTX features are only supported in OpenXR.");
				return false;
			}
			return OVRPlugin.version >= OVRPlugin.OVRP_1_65_0.version && OVRPlugin.OVRP_1_65_0.ovrp_KtxDestroy(texture) == OVRPlugin.Result.Success;
		}
	}

	public enum BoundaryVisibility
	{
		NotSuppressed = 1,
		Suppressed
	}

	public enum DynamicObjectClass
	{
		None,
		Keyboard = 1000587000
	}

	public struct DynamicObjectTrackedClassesSetInfo
	{
		public unsafe OVRPlugin.DynamicObjectClass* Classes;

		public uint ClassCount;
	}

	public struct DynamicObjectData
	{
		public OVRPlugin.DynamicObjectClass ClassType;
	}

	public class UnityOpenXR
	{
		public static void SetClientVersion()
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_SetClientVersion(OVRPlugin.wrapperVersion.Major, OVRPlugin.wrapperVersion.Minor, OVRPlugin.wrapperVersion.Build);
			}
		}

		public static IntPtr HookGetInstanceProcAddr(IntPtr func)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				return OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_HookGetInstanceProcAddr(func);
			}
			return func;
		}

		public static bool OnInstanceCreate(ulong xrInstance)
		{
			return OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version && OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnInstanceCreate(xrInstance) == OVRPlugin.Result.Success;
		}

		public static void OnInstanceDestroy(ulong xrInstance)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnInstanceDestroy(xrInstance);
			}
		}

		public static void OnSessionCreate(ulong xrSession)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnSessionCreate(xrSession);
			}
		}

		public static void OnAppSpaceChange(ulong xrSpace)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnAppSpaceChange(xrSpace);
			}
		}

		public static void OnAppSpaceChange2(ulong xrSpace, int spaceFlags)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_108_0.version)
			{
				OVRPlugin.OVRP_1_108_0.ovrp_UnityOpenXR_OnAppSpaceChange2(xrSpace, spaceFlags);
			}
		}

		public static void AllowVisibilityMesh(bool enabled)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_109_0.version)
			{
				OVRPlugin.OVRP_1_109_0.ovrp_AllowVisibilityMask(enabled ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
			}
		}

		public static void OnSessionStateChange(int oldState, int newState)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnSessionStateChange(oldState, newState);
			}
		}

		public static void OnSessionBegin(ulong xrSession)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnSessionBegin(xrSession);
			}
		}

		public static void OnSessionEnd(ulong xrSession)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnSessionEnd(xrSession);
			}
		}

		public static void OnSessionExiting(ulong xrSession)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnSessionExiting(xrSession);
			}
		}

		public static void OnSessionDestroy(ulong xrSession)
		{
			if (OVRPlugin.version >= OVRPlugin.OVRP_1_71_0.version)
			{
				OVRPlugin.OVRP_1_71_0.ovrp_UnityOpenXR_OnSessionDestroy(xrSession);
			}
		}

		public static bool Enabled;
	}

	public enum MarkerType
	{
		QRCode = 1
	}

	public enum SpaceMarkerPayloadType
	{
		InvalidQRCode = 1,
		StringQRCode,
		BinaryQRCode
	}

	public struct MarkerTrackerCreateInfo
	{
		public uint MarkerTypeCount;

		public unsafe OVRPlugin.MarkerType* MarkerTypes;
	}

	public struct MarkerTrackerCreateCompletion
	{
		public OVRPlugin.Result FutureResult;

		public ulong MarkerTracker;
	}

	public struct SpaceMarkerPayload
	{
		public uint BufferCapacityInput;

		public uint BufferCountOutput;

		public unsafe byte* Buffer;

		public OVRPlugin.SpaceMarkerPayloadType PayloadType;
	}

	public enum FutureState
	{
		Pending = 1,
		Ready
	}

	public static class Qpl
	{
		public static void SetConsent(OVRPlugin.Bool consent)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_92_0.version)
			{
				return;
			}
			OVRPlugin.OVRP_1_92_0.ovrp_QplSetConsent(consent);
		}

		public static void MarkerStart(int markerId, int instanceKey = 0, long timestampMs = -1L)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version)
			{
				return;
			}
			OVRPlugin.OVRP_1_84_0.ovrp_QplMarkerStart(markerId, instanceKey, timestampMs);
		}

		public static void MarkerStartForJoin(int markerId, string joinId, OVRPlugin.Bool cancelMarkerIfAppBackgrounded, int instanceKey = 0, long timestampMs = -1L)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_105_0.version)
			{
				return;
			}
			OVRPlugin.OVRP_1_105_0.ovrp_QplMarkerStartForJoin(markerId, joinId, cancelMarkerIfAppBackgrounded, instanceKey, timestampMs);
		}

		public static void MarkerEnd(int markerId, OVRPlugin.Qpl.ResultType resultTypeId = OVRPlugin.Qpl.ResultType.Success, int instanceKey = 0, long timestampMs = -1L)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version)
			{
				return;
			}
			OVRPlugin.OVRP_1_84_0.ovrp_QplMarkerEnd(markerId, resultTypeId, instanceKey, timestampMs);
		}

		public static OVRPlugin.Result MarkerPoint(int markerId, string name, int instanceKey, long timestampMs)
		{
			if (!(OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version))
			{
				return OVRPlugin.OVRP_1_84_0.ovrp_QplMarkerPoint(markerId, name, instanceKey, timestampMs);
			}
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}

		public unsafe static OVRPlugin.Result MarkerPoint(int markerId, string name, OVRPlugin.Qpl.Annotation* annotations, int annotationCount, int instanceKey, long timestampMs)
		{
			if (!(OVRPlugin.version < OVRPlugin.OVRP_1_96_0.version))
			{
				return OVRPlugin.OVRP_1_96_0.ovrp_QplMarkerPointData(markerId, name, annotations, annotationCount, instanceKey, timestampMs);
			}
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}

		public static void MarkerPointCached(int markerId, int nameHandle, int instanceKey = 0, long timestampMs = -1L)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version)
			{
				return;
			}
			OVRPlugin.OVRP_1_84_0.ovrp_QplMarkerPointCached(markerId, nameHandle, instanceKey, timestampMs);
		}

		public static void MarkerAnnotation(int markerId, string annotationKey, string annotationValue, int instanceKey = 0)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version)
			{
				return;
			}
			OVRPlugin.OVRP_1_84_0.ovrp_QplMarkerAnnotation(markerId, annotationKey, annotationValue, instanceKey);
		}

		public static OVRPlugin.Result MarkerAnnotation(int markerId, string annotationKey, OVRPlugin.Qpl.Variant annotationValue, int instanceKey)
		{
			if (!(OVRPlugin.version < OVRPlugin.OVRP_1_96_0.version))
			{
				return OVRPlugin.OVRP_1_96_0.ovrp_QplMarkerAnnotationVariant(markerId, annotationKey, annotationValue, instanceKey);
			}
			return OVRPlugin.Result.Failure_NotYetImplemented;
		}

		public static bool CreateMarkerHandle(string name, out int nameHandle)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version)
			{
				nameHandle = 0;
				return false;
			}
			return OVRPlugin.OVRP_1_84_0.ovrp_QplCreateMarkerHandle(name, out nameHandle) == OVRPlugin.Result.Success;
		}

		public static bool DestroyMarkerHandle(int nameHandle)
		{
			return !(OVRPlugin.version < OVRPlugin.OVRP_1_84_0.version) && OVRPlugin.OVRP_1_84_0.ovrp_QplDestroyMarkerHandle(nameHandle) == OVRPlugin.Result.Success;
		}

		public const int DefaultInstanceKey = 0;

		public const long AutoSetTimestampMs = -1L;

		public const int AutoSetTimeoutMs = 0;

		public enum ResultType : short
		{
			Success = 2,
			Fail,
			Cancel
		}

		public enum VariantType
		{
			None,
			String,
			Int,
			Double,
			Bool,
			StringArray,
			IntArray,
			DoubleArray,
			BoolArray
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Variant
		{
			public unsafe static OVRPlugin.Qpl.Variant From(byte* value)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.String,
					StringValue = value
				};
			}

			public static OVRPlugin.Qpl.Variant From(long value)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.Int,
					LongValue = value
				};
			}

			public static OVRPlugin.Qpl.Variant From(double value)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.Double,
					DoubleValue = value
				};
			}

			public static OVRPlugin.Qpl.Variant From(bool value)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.Bool,
					BoolValue = (value ? OVRPlugin.Bool.True : OVRPlugin.Bool.False)
				};
			}

			public unsafe static OVRPlugin.Qpl.Variant From(byte** values, int count)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.StringArray,
					Count = count,
					StringValues = values
				};
			}

			public unsafe static OVRPlugin.Qpl.Variant From(long* values, int count)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.IntArray,
					Count = count,
					LongValues = values
				};
			}

			public unsafe static OVRPlugin.Qpl.Variant From(double* values, int count)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.DoubleArray,
					Count = count,
					DoubleValues = values
				};
			}

			public unsafe static OVRPlugin.Qpl.Variant From(OVRPlugin.Bool* values, int count)
			{
				return new OVRPlugin.Qpl.Variant
				{
					Type = OVRPlugin.Qpl.VariantType.BoolArray,
					Count = count,
					BoolValues = values
				};
			}

			[FieldOffset(0)]
			public OVRPlugin.Qpl.VariantType Type;

			[FieldOffset(4)]
			public int Count;

			[FieldOffset(8)]
			public unsafe byte* StringValue;

			[FieldOffset(8)]
			public long LongValue;

			[FieldOffset(8)]
			public double DoubleValue;

			[FieldOffset(8)]
			public OVRPlugin.Bool BoolValue;

			[FieldOffset(8)]
			public unsafe byte** StringValues;

			[FieldOffset(8)]
			public unsafe long* LongValues;

			[FieldOffset(8)]
			public unsafe double* DoubleValues;

			[FieldOffset(8)]
			public unsafe OVRPlugin.Bool* BoolValues;
		}

		public readonly struct Annotation
		{
			public unsafe string KeyStr
			{
				get
				{
					return Marshal.PtrToStringUTF8(new IntPtr((void*)this.Key));
				}
			}

			public unsafe Annotation(byte* key, OVRPlugin.Qpl.Variant value)
			{
				this.Key = key;
				this.Value = value;
			}

			public unsafe readonly byte* Key;

			public readonly OVRPlugin.Qpl.Variant Value;

			public struct Builder : IDisposable
			{
				private IntPtr Copy(string str)
				{
					IntPtr intPtr = Marshal.StringToCoTaskMemUTF8(str);
					this._ownedStrings.Add(intPtr);
					return intPtr;
				}

				public int Count
				{
					get
					{
						List<OVRPlugin.Qpl.Annotation.Builder.Entry> entries = this._entries;
						if (entries == null)
						{
							return 0;
						}
						return entries.Count;
					}
				}

				public static OVRPlugin.Qpl.Annotation.Builder Create()
				{
					return new OVRPlugin.Qpl.Annotation.Builder
					{
						_entries = OVRObjectPool.List<OVRPlugin.Qpl.Annotation.Builder.Entry>(),
						_ownedStrings = OVRObjectPool.List<IntPtr>()
					};
				}

				public OVRPlugin.Qpl.Annotation.Builder Add(string key, OVRPlugin.Qpl.Variant value)
				{
					this._entries.Add(new OVRPlugin.Qpl.Annotation.Builder.Entry
					{
						Key = this.Copy(key),
						Value = value
					});
					return this;
				}

				public unsafe OVRPlugin.Qpl.Annotation.Builder Add(string key, string value)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From((byte*)((void*)this.Copy(value))));
				}

				public unsafe OVRPlugin.Qpl.Annotation.Builder Add(string key, byte* value)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value));
				}

				public OVRPlugin.Qpl.Annotation.Builder Add(string key, long value)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value));
				}

				public OVRPlugin.Qpl.Annotation.Builder Add(string key, double value)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value));
				}

				public OVRPlugin.Qpl.Annotation.Builder Add(string key, bool value)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value));
				}

				public unsafe OVRPlugin.Qpl.Annotation.Builder Add(string key, byte** value, int count)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value, count));
				}

				public unsafe OVRPlugin.Qpl.Annotation.Builder Add(string key, long* value, int count)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value, count));
				}

				public unsafe OVRPlugin.Qpl.Annotation.Builder Add(string key, double* value, int count)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value, count));
				}

				public unsafe OVRPlugin.Qpl.Annotation.Builder Add(string key, OVRPlugin.Bool* value, int count)
				{
					return this.Add(key, OVRPlugin.Qpl.Variant.From(value, count));
				}

				public unsafe NativeArray<OVRPlugin.Qpl.Annotation> ToNativeArray(Allocator allocator = Allocator.Temp)
				{
					NativeArray<OVRPlugin.Qpl.Annotation> result = new NativeArray<OVRPlugin.Qpl.Annotation>(this.Count, allocator, NativeArrayOptions.ClearMemory);
					if (this._entries != null)
					{
						int num = 0;
						foreach (OVRPlugin.Qpl.Annotation.Builder.Entry entry in this._entries)
						{
							result[num++] = new OVRPlugin.Qpl.Annotation((byte*)((void*)entry.Key), entry.Value);
						}
					}
					return result;
				}

				public void Dispose()
				{
					if (this._ownedStrings != null)
					{
						foreach (IntPtr ptr in this._ownedStrings)
						{
							Marshal.FreeCoTaskMem(ptr);
						}
						OVRObjectPool.Return<List<IntPtr>>(this._ownedStrings);
						this._ownedStrings = null;
					}
					if (this._entries != null)
					{
						OVRObjectPool.Return<List<OVRPlugin.Qpl.Annotation.Builder.Entry>>(this._entries);
						this._entries = null;
					}
				}

				private List<OVRPlugin.Qpl.Annotation.Builder.Entry> _entries;

				private List<IntPtr> _ownedStrings;

				private struct Entry
				{
					public IntPtr Key;

					public OVRPlugin.Qpl.Variant Value;
				}
			}
		}
	}

	public static class UnifiedConsent
	{
		public static OVRPlugin.Result SaveUnifiedConsent(bool consentValue)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return OVRPlugin.Result.Failure_Unsupported;
			}
			return OVRPlugin.OVRP_1_106_0.ovrp_SaveUnifiedConsent(1, consentValue ? OVRPlugin.Bool.True : OVRPlugin.Bool.False);
		}

		public static OVRPlugin.Result SaveUnifiedConsentWithOlderVersion(bool consentValue, int consentVersion)
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return OVRPlugin.Result.Failure_Unsupported;
			}
			return OVRPlugin.OVRP_1_106_0.ovrp_SaveUnifiedConsentWithOlderVersion(1, consentValue ? OVRPlugin.Bool.True : OVRPlugin.Bool.False, consentVersion);
		}

		public static bool? GetUnifiedConsent()
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return new bool?(false);
			}
			OVRPlugin.OptionalBool optionalBool = OVRPlugin.OVRP_1_106_0.ovrp_GetUnifiedConsent(1);
			bool? result;
			if (optionalBool != OVRPlugin.OptionalBool.False)
			{
				if (optionalBool == OVRPlugin.OptionalBool.True)
				{
					result = new bool?(true);
				}
				else
				{
					result = null;
				}
			}
			else
			{
				result = new bool?(false);
			}
			return result;
		}

		public static string GetConsentTitle()
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return "";
			}
			IntPtr intPtr = Marshal.AllocHGlobal(256);
			if (OVRPlugin.OVRP_1_106_0.ovrp_GetConsentTitle(intPtr) != OVRPlugin.Result.Success)
			{
				Marshal.FreeHGlobal(intPtr);
				return "";
			}
			string result = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public static string GetConsentMarkdownText()
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return "";
			}
			IntPtr intPtr = Marshal.AllocHGlobal(2048);
			if (OVRPlugin.OVRP_1_106_0.ovrp_GetConsentMarkdownText(intPtr) != OVRPlugin.Result.Success)
			{
				Marshal.FreeHGlobal(intPtr);
				return "";
			}
			string result = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public static string GetConsentNotificationMarkdownText()
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return "";
			}
			IntPtr intPtr = Marshal.AllocHGlobal(1024);
			IntPtr intPtr2 = Marshal.StringToHGlobalAnsi("Edit > Preferences > Meta XR");
			if (OVRPlugin.OVRP_1_106_0.ovrp_GetConsentNotificationMarkdownText(intPtr2, intPtr) != OVRPlugin.Result.Success)
			{
				Marshal.FreeHGlobal(intPtr);
				return "";
			}
			string result = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		public static string GetConsentSettingsChangeText()
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return "";
			}
			IntPtr intPtr = Marshal.AllocHGlobal(1024);
			if (OVRPlugin.OVRP_1_106_0.ovrp_GetConsentSettingsChangeText(intPtr) != OVRPlugin.Result.Success)
			{
				Marshal.FreeHGlobal(intPtr);
				return "";
			}
			string result = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public static bool ShouldShowTelemetryConsentWindow()
		{
			return !(OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version) && OVRPlugin.OVRP_1_106_0.ovrp_ShouldShowTelemetryConsentWindow(1) == OVRPlugin.Bool.True;
		}

		public static bool IsConsentSettingsChangeEnabled()
		{
			return OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version || OVRPlugin.OVRP_1_106_0.ovrp_IsConsentSettingsChangeEnabled(1) == OVRPlugin.Bool.True;
		}

		public static bool ShouldShowTelemetryNotification()
		{
			return !(OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version) && OVRPlugin.OVRP_1_106_0.ovrp_ShouldShowTelemetryNotification(1) == OVRPlugin.Bool.True;
		}

		public static OVRPlugin.Result SetNotificationShown()
		{
			if (OVRPlugin.version < OVRPlugin.OVRP_1_106_0.version)
			{
				return OVRPlugin.Result.Failure_Unsupported;
			}
			return OVRPlugin.OVRP_1_106_0.ovrp_SetNotificationShown(1);
		}

		private const int ToolId = 1;
	}

	private static class OVRP_0_1_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Sizei ovrp_GetEyeTextureSize(OVRPlugin.Eye eyeId);

		public static readonly Version version = new Version(0, 1, 0);
	}

	private static class OVRP_0_1_1
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetOverlayQuad2(OVRPlugin.Bool onTop, OVRPlugin.Bool headLocked, IntPtr texture, IntPtr device, OVRPlugin.Posef pose, OVRPlugin.Vector3f scale);

		public static readonly Version version = new Version(0, 1, 1);
	}

	private static class OVRP_0_1_2
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetNodePose(OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetControllerVibration(uint controllerMask, float frequency, float amplitude);

		public static readonly Version version = new Version(0, 1, 2);
	}

	private static class OVRP_0_1_3
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetNodeVelocity(OVRPlugin.Node nodeId);

		[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetNodeAcceleration(OVRPlugin.Node nodeId);

		public static readonly Version version = new Version(0, 1, 3);
	}

	private static class OVRP_0_5_0
	{
		public static readonly Version version = new Version(0, 5, 0);
	}

	private static class OVRP_1_0_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.TrackingOrigin ovrp_GetTrackingOriginType();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetTrackingOriginType(OVRPlugin.TrackingOrigin originType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetTrackingCalibratedOrigin();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_RecenterTrackingOrigin(uint flags);

		public static readonly Version version = new Version(1, 0, 0);
	}

	private static class OVRP_1_1_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetInitialized();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetVersion")]
		private static extern IntPtr _ovrp_GetVersion();

		public static string ovrp_GetVersion()
		{
			return Marshal.PtrToStringAnsi(OVRPlugin.OVRP_1_1_0._ovrp_GetVersion());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetNativeSDKVersion")]
		private static extern IntPtr _ovrp_GetNativeSDKVersion();

		public static string ovrp_GetNativeSDKVersion()
		{
			return Marshal.PtrToStringAnsi(OVRPlugin.OVRP_1_1_0._ovrp_GetNativeSDKVersion());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovrp_GetAudioOutId();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovrp_GetAudioInId();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetEyeTextureScale();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetEyeTextureScale(float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetTrackingOrientationSupported();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetTrackingOrientationEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetTrackingOrientationEnabled(OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetTrackingPositionSupported();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetTrackingPositionEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetTrackingPositionEnabled(OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetNodePresent(OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetNodeOrientationTracked(OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetNodePositionTracked(OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Frustumf ovrp_GetNodeFrustum(OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.ControllerState ovrp_GetControllerState(uint controllerMask);

		[Obsolete("Deprecated. Replaced by ovrp_GetSuggestedCpuPerformanceLevel", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemCpuLevel();

		[Obsolete("Deprecated. Replaced by ovrp_SetSuggestedCpuPerformanceLevel", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetSystemCpuLevel(int value);

		[Obsolete("Deprecated. Replaced by ovrp_GetSuggestedGpuPerformanceLevel", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemGpuLevel();

		[Obsolete("Deprecated. Replaced by ovrp_SetSuggestedGpuPerformanceLevel", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetSystemGpuLevel(int value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetSystemPowerSavingMode();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemDisplayFrequency();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemVSyncCount();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemVolume();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.BatteryStatus ovrp_GetSystemBatteryStatus();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemBatteryLevel();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemBatteryTemperature();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetSystemProductName")]
		private static extern IntPtr _ovrp_GetSystemProductName();

		public static string ovrp_GetSystemProductName()
		{
			return Marshal.PtrToStringAnsi(OVRPlugin.OVRP_1_1_0._ovrp_GetSystemProductName());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_ShowSystemUI(OVRPlugin.PlatformUI ui);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetAppMonoscopic();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetAppMonoscopic(OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetAppHasVrFocus();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetAppShouldQuit();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetAppShouldRecenter();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetAppLatencyTimings")]
		private static extern IntPtr _ovrp_GetAppLatencyTimings();

		public static string ovrp_GetAppLatencyTimings()
		{
			return Marshal.PtrToStringAnsi(OVRPlugin.OVRP_1_1_0._ovrp_GetAppLatencyTimings());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetUserPresent();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetUserIPD();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetUserIPD(float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetUserEyeDepth();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetUserEyeDepth(float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetUserEyeHeight();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetUserEyeHeight(float value);

		public static readonly Version version = new Version(1, 1, 0);
	}

	private static class OVRP_1_2_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetSystemVSyncCount(int vsyncCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrpi_SetTrackingCalibratedOrigin();

		public static readonly Version version = new Version(1, 2, 0);
	}

	private static class OVRP_1_3_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetEyeOcclusionMeshEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetEyeOcclusionMeshEnabled(OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetSystemHeadphonesPresent();

		public static readonly Version version = new Version(1, 3, 0);
	}

	private static class OVRP_1_5_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.SystemRegion ovrp_GetSystemRegion();

		public static readonly Version version = new Version(1, 5, 0);
	}

	private static class OVRP_1_6_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetTrackingIPDEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetTrackingIPDEnabled(OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.HapticsDesc ovrp_GetControllerHapticsDesc(uint controllerMask);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.HapticsState ovrp_GetControllerHapticsState(uint controllerMask);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetControllerHaptics(uint controllerMask, OVRPlugin.HapticsBuffer hapticsBuffer);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetOverlayQuad3(uint flags, IntPtr textureLeft, IntPtr textureRight, IntPtr device, OVRPlugin.Posef pose, OVRPlugin.Vector3f scale, int layerIndex);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetEyeRecommendedResolutionScale();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetAppCpuStartToGpuEndTime();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemRecommendedMSAALevel();

		public static readonly Version version = new Version(1, 6, 0);
	}

	private static class OVRP_1_7_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetAppChromaticCorrection();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetAppChromaticCorrection(OVRPlugin.Bool value);

		public static readonly Version version = new Version(1, 7, 0);
	}

	private static class OVRP_1_8_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetBoundaryConfigured();

		[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.BoundaryTestResult ovrp_TestBoundaryNode(OVRPlugin.Node nodeId, OVRPlugin.BoundaryType boundaryType);

		[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.BoundaryTestResult ovrp_TestBoundaryPoint(OVRPlugin.Vector3f point, OVRPlugin.BoundaryType boundaryType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.BoundaryGeometry ovrp_GetBoundaryGeometry(OVRPlugin.BoundaryType boundaryType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Vector3f ovrp_GetBoundaryDimensions(OVRPlugin.BoundaryType boundaryType);

		[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetBoundaryVisible();

		[Obsolete("Deprecated. This function will not be supported in OpenXR", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetBoundaryVisible(OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_Update2(int stateId, int frameIndex, double predictionSeconds);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetNodePose2(int stateId, OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetNodeVelocity2(int stateId, OVRPlugin.Node nodeId);

		[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Posef ovrp_GetNodeAcceleration2(int stateId, OVRPlugin.Node nodeId);

		public static readonly Version version = new Version(1, 8, 0);
	}

	private static class OVRP_1_9_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.SystemHeadset ovrp_GetSystemHeadsetType();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Controller ovrp_GetActiveController();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Controller ovrp_GetConnectedControllers();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetBoundaryGeometry2(OVRPlugin.BoundaryType boundaryType, IntPtr points, ref int pointsCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.AppPerfStats ovrp_GetAppPerfStats();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_ResetAppPerfStats();

		public static readonly Version version = new Version(1, 9, 0);
	}

	private static class OVRP_1_10_0
	{
		public static readonly Version version = new Version(1, 10, 0);
	}

	private static class OVRP_1_11_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_SetDesiredEyeTextureFormat(OVRPlugin.EyeTextureFormat value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.EyeTextureFormat ovrp_GetDesiredEyeTextureFormat();

		public static readonly Version version = new Version(1, 11, 0);
	}

	private static class OVRP_1_12_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetAppFramerate();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.PoseStatef ovrp_GetNodePoseState(OVRPlugin.Step stepId, OVRPlugin.Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.ControllerState2 ovrp_GetControllerState2(uint controllerMask);

		public static readonly Version version = new Version(1, 12, 0);
	}

	private static class OVRP_1_15_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_InitializeMixedReality();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ShutdownMixedReality();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetMixedRealityInitialized();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_UpdateExternalCamera();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetExternalCameraCount(out int cameraCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetExternalCameraName(int cameraId, [MarshalAs(UnmanagedType.LPArray, SizeConst = 32)] char[] cameraName);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetExternalCameraIntrinsics(int cameraId, out OVRPlugin.CameraIntrinsics cameraIntrinsics);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetExternalCameraExtrinsics(int cameraId, out OVRPlugin.CameraExtrinsics cameraExtrinsics);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CalculateLayerDesc(OVRPlugin.OverlayShape shape, OVRPlugin.LayerLayout layout, ref OVRPlugin.Sizei textureSize, int mipLevels, int sampleCount, OVRPlugin.EyeTextureFormat format, int layerFlags, ref OVRPlugin.LayerDesc layerDesc);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EnqueueSetupLayer(ref OVRPlugin.LayerDesc desc, IntPtr layerId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EnqueueDestroyLayer(IntPtr layerId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLayerTextureStageCount(int layerId, ref int layerTextureStageCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLayerTexturePtr(int layerId, int stage, OVRPlugin.Eye eyeId, ref IntPtr textureHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EnqueueSubmitLayer(uint flags, IntPtr textureLeft, IntPtr textureRight, int layerId, int frameIndex, ref OVRPlugin.Posef pose, ref OVRPlugin.Vector3f scale, int layerIndex);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNodeFrustum2(OVRPlugin.Node nodeId, out OVRPlugin.Frustumf2 nodeFrustum);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetEyeTextureArrayEnabled();

		public static readonly Version version = new Version(1, 15, 0);

		public const int OVRP_EXTERNAL_CAMERA_NAME_SIZE = 32;
	}

	private static class OVRP_1_16_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_UpdateCameraDevices();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_IsCameraDeviceAvailable(OVRPlugin.CameraDevice cameraDevice);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetCameraDevicePreferredColorFrameSize(OVRPlugin.CameraDevice cameraDevice, OVRPlugin.Sizei preferredColorFrameSize);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_OpenCameraDevice(OVRPlugin.CameraDevice cameraDevice);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CloseCameraDevice(OVRPlugin.CameraDevice cameraDevice);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_HasCameraDeviceOpened(OVRPlugin.CameraDevice cameraDevice);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_IsCameraDeviceColorFrameAvailable(OVRPlugin.CameraDevice cameraDevice);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceColorFrameSize(OVRPlugin.CameraDevice cameraDevice, out OVRPlugin.Sizei colorFrameSize);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceColorFrameBgraPixels(OVRPlugin.CameraDevice cameraDevice, out IntPtr colorFrameBgraPixels, out int colorFrameRowPitch);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetControllerState4(uint controllerMask, ref OVRPlugin.ControllerState4 controllerState);

		public static readonly Version version = new Version(1, 16, 0);
	}

	private static class OVRP_1_17_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetExternalCameraPose(OVRPlugin.CameraDevice camera, out OVRPlugin.Posef cameraPose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ConvertPoseToCameraSpace(OVRPlugin.CameraDevice camera, ref OVRPlugin.Posef trackingSpacePose, out OVRPlugin.Posef cameraSpacePose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceIntrinsicsParameters(OVRPlugin.CameraDevice camera, out OVRPlugin.Bool supportIntrinsics, out OVRPlugin.CameraDeviceIntrinsicsParameters intrinsicsParameters);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DoesCameraDeviceSupportDepth(OVRPlugin.CameraDevice camera, out OVRPlugin.Bool supportDepth);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceDepthSensingMode(OVRPlugin.CameraDevice camera, out OVRPlugin.CameraDeviceDepthSensingMode depthSensoringMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetCameraDeviceDepthSensingMode(OVRPlugin.CameraDevice camera, OVRPlugin.CameraDeviceDepthSensingMode depthSensoringMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDevicePreferredDepthQuality(OVRPlugin.CameraDevice camera, out OVRPlugin.CameraDeviceDepthQuality depthQuality);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetCameraDevicePreferredDepthQuality(OVRPlugin.CameraDevice camera, OVRPlugin.CameraDeviceDepthQuality depthQuality);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_IsCameraDeviceDepthFrameAvailable(OVRPlugin.CameraDevice camera, out OVRPlugin.Bool available);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceDepthFrameSize(OVRPlugin.CameraDevice camera, out OVRPlugin.Sizei depthFrameSize);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceDepthFramePixels(OVRPlugin.CameraDevice cameraDevice, out IntPtr depthFramePixels, out int depthFrameRowPitch);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCameraDeviceDepthConfidencePixels(OVRPlugin.CameraDevice cameraDevice, out IntPtr depthConfidencePixels, out int depthConfidenceRowPitch);

		public static readonly Version version = new Version(1, 17, 0);
	}

	private static class OVRP_1_18_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetHandNodePoseStateLatency(double latencyInSeconds);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHandNodePoseStateLatency(out double latencyInSeconds);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetAppHasInputFocus(out OVRPlugin.Bool appHasInputFocus);

		public static readonly Version version = new Version(1, 18, 0);
	}

	private static class OVRP_1_19_0
	{
		public static readonly Version version = new Version(1, 19, 0);
	}

	private static class OVRP_1_21_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTiledMultiResSupported(out OVRPlugin.Bool foveationSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTiledMultiResLevel(out OVRPlugin.FoveatedRenderingLevel level);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetTiledMultiResLevel(OVRPlugin.FoveatedRenderingLevel level);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetGPUUtilSupported(out OVRPlugin.Bool gpuUtilSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetGPUUtilLevel(out float gpuUtil);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSystemDisplayFrequency2(out float systemDisplayFrequency);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSystemDisplayAvailableFrequencies(IntPtr systemDisplayAvailableFrequencies, ref int numFrequencies);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetSystemDisplayFrequency(float requestedFrequency);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetAppAsymmetricFov(out OVRPlugin.Bool useAsymmetricFov);

		public static readonly Version version = new Version(1, 21, 0);
	}

	private static class OVRP_1_28_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetDominantHand(out OVRPlugin.Handedness dominantHand);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SendEvent(string name, string param);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EnqueueSetupLayer2(ref OVRPlugin.LayerDesc desc, int compositionDepth, IntPtr layerId);

		public static readonly Version version = new Version(1, 28, 0);
	}

	private static class OVRP_1_29_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLayerAndroidSurfaceObject(int layerId, ref IntPtr surfaceObject);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetHeadPoseModifier(ref OVRPlugin.Quatf relativeRotation, ref OVRPlugin.Vector3f relativeTranslation);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHeadPoseModifier(out OVRPlugin.Quatf relativeRotation, out OVRPlugin.Vector3f relativeTranslation);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNodePoseStateRaw(OVRPlugin.Step stepId, int frameIndex, OVRPlugin.Node nodeId, out OVRPlugin.PoseStatef nodePoseState);

		public static readonly Version version = new Version(1, 29, 0);
	}

	private static class OVRP_1_30_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCurrentTrackingTransformPose(out OVRPlugin.Posef trackingTransformPose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTrackingTransformRawPose(out OVRPlugin.Posef trackingTransformRawPose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SendEvent2(string name, string param, string source);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_IsPerfMetricsSupported(OVRPlugin.PerfMetrics perfMetrics, out OVRPlugin.Bool isSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetPerfMetricsFloat(OVRPlugin.PerfMetrics perfMetrics, out float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetPerfMetricsInt(OVRPlugin.PerfMetrics perfMetrics, out int value);

		public static readonly Version version = new Version(1, 30, 0);
	}

	private static class OVRP_1_31_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTimeInSeconds(out double value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetColorScaleAndOffset(Vector4 colorScale, Vector4 colorOffset, OVRPlugin.Bool applyToAllLayers);

		public static readonly Version version = new Version(1, 31, 0);
	}

	private static class OVRP_1_32_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_AddCustomMetadata(string name, string param);

		public static readonly Version version = new Version(1, 32, 0);
	}

	private static class OVRP_1_34_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EnqueueSubmitLayer2(uint flags, IntPtr textureLeft, IntPtr textureRight, int layerId, int frameIndex, ref OVRPlugin.Posef pose, ref OVRPlugin.Vector3f scale, int layerIndex, OVRPlugin.Bool overrideTextureRectMatrix, ref OVRPlugin.TextureRectMatrixf textureRectMatrix, OVRPlugin.Bool overridePerLayerColorScaleAndOffset, ref Vector4 colorScale, ref Vector4 colorOffset);

		public static readonly Version version = new Version(1, 34, 0);
	}

	private static class OVRP_1_35_0
	{
		public static readonly Version version = new Version(1, 35, 0);
	}

	private static class OVRP_1_36_0
	{
		public static readonly Version version = new Version(1, 36, 0);
	}

	private static class OVRP_1_37_0
	{
		public static readonly Version version = new Version(1, 37, 0);
	}

	private static class OVRP_1_38_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTrackingTransformRelativePose(ref OVRPlugin.Posef trackingTransformRelativePose, OVRPlugin.TrackingOrigin trackingOrigin);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_Initialize();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_Shutdown();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetInitialized(out OVRPlugin.Bool initialized);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_Update();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetMrcActivationMode(out OVRPlugin.Media.MrcActivationMode activationMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetMrcActivationMode(OVRPlugin.Media.MrcActivationMode activationMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_IsMrcEnabled(out OVRPlugin.Bool mrcEnabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_IsMrcActivated(out OVRPlugin.Bool mrcActivated);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_UseMrcDebugCamera(out OVRPlugin.Bool useMrcDebugCamera);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetMrcInputVideoBufferType(OVRPlugin.Media.InputVideoBufferType inputVideoBufferType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetMrcInputVideoBufferType(ref OVRPlugin.Media.InputVideoBufferType inputVideoBufferType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetMrcFrameSize(int frameWidth, int frameHeight);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetMrcFrameSize(ref int frameWidth, ref int frameHeight);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetMrcAudioSampleRate(int sampleRate);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetMrcAudioSampleRate(ref int sampleRate);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetMrcFrameImageFlipped(OVRPlugin.Bool flipped);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetMrcFrameImageFlipped(ref OVRPlugin.Bool flipped);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_EncodeMrcFrame(IntPtr rawBuffer, IntPtr audioDataPtr, int audioDataLen, int audioChannels, double timestamp, ref int outSyncId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_EncodeMrcFrameWithDualTextures(IntPtr backgroundTextureHandle, IntPtr foregroundTextureHandle, IntPtr audioData, int audioDataLen, int audioChannels, double timestamp, ref int outSyncId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SyncMrcFrame(int syncId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetDeveloperMode(OVRPlugin.Bool active);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNodeOrientationValid(OVRPlugin.Node nodeId, ref OVRPlugin.Bool nodeOrientationValid);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNodePositionValid(OVRPlugin.Node nodeId, ref OVRPlugin.Bool nodePositionValid);

		public static readonly Version version = new Version(1, 38, 0);
	}

	private static class OVRP_1_39_0
	{
		public static readonly Version version = new Version(1, 39, 0);
	}

	private static class OVRP_1_40_0
	{
		public static readonly Version version = new Version(1, 40, 0);
	}

	private static class OVRP_1_41_0
	{
		public static readonly Version version = new Version(1, 41, 0);
	}

	private static class OVRP_1_42_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetAdaptiveGpuPerformanceScale2(ref float adaptiveGpuPerformanceScale);

		public static readonly Version version = new Version(1, 42, 0);
	}

	private static class OVRP_1_43_0
	{
		public static readonly Version version = new Version(1, 43, 0);
	}

	private static class OVRP_1_44_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHandTrackingEnabled(ref OVRPlugin.Bool handTrackingEnabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHandState(OVRPlugin.Step stepId, OVRPlugin.Hand hand, out OVRPlugin.HandStateInternal handState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSkeleton(OVRPlugin.SkeletonType skeletonType, out OVRPlugin.Skeleton skeleton);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetMesh(OVRPlugin.MeshType meshType, IntPtr meshPtr);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_OverrideExternalCameraFov(int cameraId, OVRPlugin.Bool useOverriddenFov, ref OVRPlugin.Fovf fov);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetUseOverriddenExternalCameraFov(int cameraId, out OVRPlugin.Bool useOverriddenFov);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_OverrideExternalCameraStaticPose(int cameraId, OVRPlugin.Bool useOverriddenPose, ref OVRPlugin.Posef poseInStageOrigin);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetUseOverriddenExternalCameraStaticPose(int cameraId, out OVRPlugin.Bool useOverriddenStaticPose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ResetDefaultExternalCamera();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetDefaultExternalCamera(string cameraName, ref OVRPlugin.CameraIntrinsics cameraIntrinsics, ref OVRPlugin.CameraExtrinsics cameraExtrinsics);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLocalTrackingSpaceRecenterCount(ref int recenterCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetPredictedDisplayTime(int frameIndex, ref double predictedDisplayTime);

		public static readonly Version version = new Version(1, 44, 0);
	}

	private static class OVRP_1_45_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSystemHmd3DofModeEnabled(ref OVRPlugin.Bool enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetAvailableQueueIndexVulkan(uint queueIndexVk);

		public static readonly Version version = new Version(1, 45, 0);
	}

	private static class OVRP_1_46_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTiledMultiResDynamic(out OVRPlugin.Bool isDynamic);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetTiledMultiResDynamic(OVRPlugin.Bool isDynamic);

		public static readonly Version version = new Version(1, 46, 0);
	}

	private static class OVRP_1_47_0
	{
		public static readonly Version version = new Version(1, 47, 0);
	}

	private static class OVRP_1_48_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetExternalCameraProperties(string cameraName, ref OVRPlugin.CameraIntrinsics cameraIntrinsics, ref OVRPlugin.CameraExtrinsics cameraExtrinsics);

		public static readonly Version version = new Version(1, 48, 0);
	}

	private static class OVRP_1_49_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetClientColorDesc(OVRPlugin.ColorSpace colorSpace);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHmdColorDesc(ref OVRPlugin.ColorSpace colorSpace);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_EncodeMrcFrameWithPoseTime(IntPtr rawBuffer, IntPtr audioDataPtr, int audioDataLen, int audioChannels, double timestamp, double poseTime, ref int outSyncId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_EncodeMrcFrameDualTexturesWithPoseTime(IntPtr backgroundTextureHandle, IntPtr foregroundTextureHandle, IntPtr audioData, int audioDataLen, int audioChannels, double timestamp, double poseTime, ref int outSyncId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetHeadsetControllerPose(OVRPlugin.Posef headsetPose, OVRPlugin.Posef leftControllerPose, OVRPlugin.Posef rightControllerPose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_EnumerateCameraAnchorHandles(ref int anchorCount, ref IntPtr CameraAnchorHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetCurrentCameraAnchorHandle(ref IntPtr anchorHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetCameraAnchorName(IntPtr anchorHandle, [MarshalAs(UnmanagedType.LPArray, SizeConst = 32)] char[] cameraName);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetCameraAnchorHandle(IntPtr anchorName, ref IntPtr anchorHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetCameraAnchorType(IntPtr anchorHandle, ref OVRPlugin.CameraAnchorType anchorType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_CreateCustomCameraAnchor(IntPtr anchorName, ref IntPtr anchorHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_DestroyCustomCameraAnchor(IntPtr anchorHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetCustomCameraAnchorPose(IntPtr anchorHandle, ref OVRPlugin.Posef pose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetCustomCameraAnchorPose(IntPtr anchorHandle, OVRPlugin.Posef pose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetCameraMinMaxDistance(IntPtr anchorHandle, ref double minDistance, ref double maxDistance);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetCameraMinMaxDistance(IntPtr anchorHandle, double minDistance, double maxDistance);

		public static readonly Version version = new Version(1, 49, 0);

		public const int OVRP_ANCHOR_NAME_SIZE = 32;
	}

	private static class OVRP_1_50_0
	{
		public static readonly Version version = new Version(1, 50, 0);
	}

	private static class OVRP_1_51_0
	{
		public static readonly Version version = new Version(1, 51, 0);
	}

	private static class OVRP_1_52_0
	{
		public static readonly Version version = new Version(1, 52, 0);
	}

	private static class OVRP_1_53_0
	{
		public static readonly Version version = new Version(1, 53, 0);
	}

	private static class OVRP_1_54_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetPlatformInitialized();

		public static readonly Version version = new Version(1, 54, 0);
	}

	private static class OVRP_1_55_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSkeleton2(OVRPlugin.SkeletonType skeletonType, out OVRPlugin.Skeleton2Internal skeleton);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_PollEvent(ref OVRPlugin.EventDataBuffer eventDataBuffer);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNativeXrApiType(out OVRPlugin.XrApi xrApi);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNativeOpenXRHandles(out ulong xrInstance, out ulong xrSession);

		public static readonly Version version = new Version(1, 55, 0);
	}

	private static class OVRP_1_55_1
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_PollEvent2(ref OVRPlugin.EventType eventType, ref IntPtr eventData);

		public static readonly Version version = new Version(1, 55, 1);
	}

	private static class OVRP_1_56_0
	{
		public static readonly Version version = new Version(1, 56, 0);
	}

	private static class OVRP_1_57_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_GetPlatformCameraMode(out OVRPlugin.Media.PlatformCameraMode platformCameraMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_SetPlatformCameraMode(OVRPlugin.Media.PlatformCameraMode platformCameraMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetEyeFovPremultipliedAlphaMode(OVRPlugin.Bool enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetEyeFovPremultipliedAlphaMode(ref OVRPlugin.Bool enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetKeyboardOverlayUV(OVRPlugin.Vector2f uv);

		public static readonly Version version = new Version(1, 57, 0);
	}

	private static class OVRP_1_58_0
	{
		public static readonly Version version = new Version(1, 58, 0);
	}

	private static class OVRP_1_59_0
	{
		public static readonly Version version = new Version(1, 59, 0);
	}

	private static class OVRP_1_60_0
	{
		public static readonly Version version = new Version(1, 60, 0);
	}

	private static class OVRP_1_61_0
	{
		public static readonly Version version = new Version(1, 61, 0);
	}

	private static class OVRP_1_62_0
	{
		public static readonly Version version = new Version(1, 62, 0);
	}

	private static class OVRP_1_63_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_InitializeInsightPassthrough();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ShutdownInsightPassthrough();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_GetInsightPassthroughInitialized();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetInsightPassthroughStyle(int layerId, OVRPlugin.InsightPassthroughStyle style);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateInsightTriangleMesh(int layerId, IntPtr vertices, int vertexCount, IntPtr triangles, int triangleCount, out ulong meshHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroyInsightTriangleMesh(ulong meshHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_AddInsightPassthroughSurfaceGeometry(int layerId, ulong meshHandle, Matrix4x4 T_world_model, out ulong geometryInstanceHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroyInsightPassthroughGeometryInstance(ulong geometryInstanceHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_UpdateInsightPassthroughGeometryTransform(ulong geometryInstanceHandle, Matrix4x4 T_world_model);

		public static readonly Version version = new Version(1, 63, 0);
	}

	private static class OVRP_1_64_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_LocateSpace(ref OVRPlugin.Posef location, ref ulong space, OVRPlugin.TrackingOrigin trackingOrigin);

		public static readonly Version version = new Version(1, 64, 0);
	}

	private static class OVRP_1_65_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxLoadFromMemory(ref IntPtr data, uint length, ref IntPtr texture);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxTextureWidth(IntPtr texture, ref uint width);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxTextureHeight(IntPtr texture, ref uint height);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxTranscode(IntPtr texture, uint format);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxGetTextureData(IntPtr texture, IntPtr data, uint bufferSize);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxTextureSize(IntPtr texture, ref uint size);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_KtxDestroy(IntPtr texture);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroySpace(ref ulong space);

		public static readonly Version version = new Version(1, 65, 0);
	}

	private static class OVRP_1_66_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetInsightPassthroughInitializationState();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_Media_IsCastingToRemoteClient(out OVRPlugin.Bool isCasting);

		public static readonly Version version = new Version(1, 66, 0);
	}

	private static class OVRP_1_67_0
	{
		public static readonly Version version = new Version(1, 67, 0);
	}

	private static class OVRP_1_68_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_LoadRenderModel(ulong modelKey, uint bufferInputCapacity, ref uint bufferCountOutput, IntPtr buffer);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetRenderModelPaths(uint index, IntPtr path);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetRenderModelProperties(string path, out OVRPlugin.RenderModelPropertiesInternal properties);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetInsightPassthroughKeyboardHandsIntensity(int layerId, OVRPlugin.InsightPassthroughKeyboardHandsIntensity intensity);

		public static readonly Version version = new Version(1, 68, 0);

		public const int OVRP_RENDER_MODEL_MAX_PATH_LENGTH = 256;

		public const int OVRP_RENDER_MODEL_MAX_NAME_LENGTH = 64;
	}

	private static class OVRP_1_69_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNodePoseStateImmediate(OVRPlugin.Node nodeId, out OVRPlugin.PoseStatef nodePoseState);

		public static readonly Version version = new Version(1, 69, 0);
	}

	private static class OVRP_1_70_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetLogCallback2(OVRPlugin.LogCallback2DelegateType logCallback);

		public static readonly Version version = new Version(1, 70, 0);
	}

	private static class OVRP_1_71_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_IsInsightPassthroughSupported(ref OVRPlugin.Bool supported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_SetClientVersion(int majorVersion, int minorVersion, int patchVersion);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovrp_UnityOpenXR_HookGetInstanceProcAddr(IntPtr func);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_UnityOpenXR_OnInstanceCreate(ulong xrInstance);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnInstanceDestroy(ulong xrInstance);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnSessionCreate(ulong xrSession);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnAppSpaceChange(ulong xrSpace);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnSessionStateChange(int oldState, int newState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnSessionBegin(ulong xrSession);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnSessionEnd(ulong xrSession);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnSessionExiting(ulong xrSession);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnSessionDestroy(ulong xrSession);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetSuggestedCpuPerformanceLevel(OVRPlugin.ProcessorPerformanceLevel perfLevel);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSuggestedCpuPerformanceLevel(out OVRPlugin.ProcessorPerformanceLevel perfLevel);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetSuggestedGpuPerformanceLevel(OVRPlugin.ProcessorPerformanceLevel perfLevel);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSuggestedGpuPerformanceLevel(out OVRPlugin.ProcessorPerformanceLevel perfLevel);

		public static readonly Version version = new Version(1, 71, 0);
	}

	private static class OVRP_1_72_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateSpatialAnchor(ref OVRPlugin.SpatialAnchorCreateInfo createInfo, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetSpaceComponentStatus(ref ulong space, OVRPlugin.SpaceComponentType componentType, OVRPlugin.Bool enable, double timeout, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceComponentStatus(ref ulong space, OVRPlugin.SpaceComponentType componentType, out OVRPlugin.Bool enabled, out OVRPlugin.Bool changePending);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EnumerateSpaceSupportedComponents(ref ulong space, uint componentTypesCapacityInput, out uint componentTypesCountOutput, [MarshalAs(UnmanagedType.LPArray)] [In] [Out] OVRPlugin.SpaceComponentType[] componentTypes);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern OVRPlugin.Result ovrp_EnumerateSpaceSupportedComponents(ref ulong space, uint componentTypesCapacityInput, out uint componentTypesCountOutput, OVRPlugin.SpaceComponentType* componentTypes);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SaveSpace(ref ulong space, OVRPlugin.SpaceStorageLocation location, OVRPlugin.SpaceStoragePersistenceMode mode, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QuerySpaces(ref OVRPlugin.SpaceQueryInfo queryInfo, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_RetrieveSpaceQueryResults(ref ulong requestId, uint resultCapacityInput, ref uint resultCountOutput, IntPtr results);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EraseSpace(ref ulong space, OVRPlugin.SpaceStorageLocation location, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceContainer(ref ulong space, ref OVRPlugin.SpaceContainerInternal containerInternal);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceBoundingBox2D(ref ulong space, out OVRPlugin.Rectf rect);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceBoundingBox3D(ref ulong space, out OVRPlugin.Boundsf bounds);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceSemanticLabels(ref ulong space, ref OVRPlugin.SpaceSemanticLabelInternal labelsInternal);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceRoomLayout(ref ulong space, ref OVRPlugin.RoomLayoutInternal roomLayoutInternal);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceBoundary2D(ref ulong space, ref OVRPlugin.PolygonalBoundary2DInternal boundaryInternal);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_RequestSceneCapture(ref OVRPlugin.SceneCaptureRequestInternal request, out ulong requestId);

		public static readonly Version version = new Version(1, 72, 0);
	}

	private static class OVRP_1_73_0
	{
		public static readonly Version version = new Version(1, 73, 0);
	}

	private static class OVRP_1_74_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceUuid(in ulong space, out Guid uuid);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateVirtualKeyboard(OVRPlugin.VirtualKeyboardCreateInfo createInfo);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroyVirtualKeyboard();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SendVirtualKeyboardInput(OVRPlugin.VirtualKeyboardInputInfo inputInfo, ref OVRPlugin.Posef interactorRootPose);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ChangeVirtualKeyboardTextContext(string textContext);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateVirtualKeyboardSpace(OVRPlugin.VirtualKeyboardSpaceCreateInfo createInfo, out ulong keyboardSpace);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SuggestVirtualKeyboardLocation(OVRPlugin.VirtualKeyboardLocationInfo locationInfo);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetVirtualKeyboardScale(out float location);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetRenderModelProperties2(string path, OVRPlugin.RenderModelFlags flags, out OVRPlugin.RenderModelPropertiesInternal properties);

		public static readonly Version version = new Version(1, 74, 0);
	}

	private static class OVRP_1_75_0
	{
		public static readonly Version version = new Version(1, 75, 0);
	}

	private static class OVRP_1_76_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetNodePoseStateAtTime(double time, OVRPlugin.Node nodeId, out OVRPlugin.PoseStatef nodePoseState);

		public static readonly Version version = new Version(1, 76, 0);
	}

	private static class OVRP_1_78_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetPassthroughCapabilityFlags(ref OVRPlugin.PassthroughCapabilityFlags capabilityFlags);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFoveationEyeTrackedSupported(out OVRPlugin.Bool foveationSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFoveationEyeTracked(out OVRPlugin.Bool isEyeTrackedFoveation);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetFoveationEyeTracked(OVRPlugin.Bool isEyeTrackedFoveation);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartFaceTracking();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StopFaceTracking();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartBodyTracking();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StopBodyTracking();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartEyeTracking();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StopEyeTracking();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetEyeTrackingSupported(out OVRPlugin.Bool eyeTrackingSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceTrackingSupported(out OVRPlugin.Bool faceTrackingSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetBodyTrackingEnabled(out OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetBodyTrackingSupported(out OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetBodyState(OVRPlugin.Step stepId, int frameIndex, out OVRPlugin.BodyStateInternal bodyState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceTrackingEnabled(out OVRPlugin.Bool faceTrackingEnabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceState(OVRPlugin.Step stepId, int frameIndex, out OVRPlugin.FaceStateInternal faceState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetEyeTrackingEnabled(out OVRPlugin.Bool eyeTrackingEnabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetEyeGazesState(OVRPlugin.Step stepId, int frameIndex, out OVRPlugin.EyeGazesStateInternal eyeGazesState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetControllerState5(uint controllerMask, ref OVRPlugin.ControllerState5 controllerState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetControllerLocalizedVibration(OVRPlugin.Controller controllerMask, OVRPlugin.HapticsLocation hapticsLocationMask, float frequency, float amplitude);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLocalDimmingSupported(out OVRPlugin.Bool localDimmingSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetLocalDimming(OVRPlugin.Bool localDimmingMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLocalDimming(out OVRPlugin.Bool localDimmingMode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCurrentInteractionProfile(OVRPlugin.Hand hand, out OVRPlugin.InteractionProfile interactionProfile);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetControllerHapticsAmplitudeEnvelope(OVRPlugin.Controller controllerMask, OVRPlugin.HapticsAmplitudeEnvelopeVibration hapticsVibration);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetControllerHapticsPcm(OVRPlugin.Controller controllerMask, OVRPlugin.HapticsPcmVibration hapticsVibration);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetControllerSampleRateHz(OVRPlugin.Controller controller, out float sampleRateHz);

		public static readonly Version version = new Version(1, 78, 0);
	}

	private static class OVRP_1_79_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern OVRPlugin.Result ovrp_ShareSpaces(ulong* spaces, uint numSpaces, ulong* userHandles, uint numUsers, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern OVRPlugin.Result ovrp_SaveSpaceList(ulong* spaces, uint numSpaces, OVRPlugin.SpaceStorageLocation location, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceUserId(in ulong spaceUserHandle, out ulong spaceUserId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateSpaceUser(in ulong spaceUserId, out ulong spaceUserHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroySpaceUser(in ulong userHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_LocateSpace2(out OVRPlugin.SpaceLocationf location, in ulong space, OVRPlugin.TrackingOrigin trackingOrigin);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DeclareUser(in ulong userId, out ulong userHandle);

		public static readonly Version version = new Version(1, 79, 0);
	}

	private static class OVRP_1_81_0
	{
		public static readonly Version version = new Version(1, 81, 0);
	}

	private static class OVRP_1_82_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceTriangleMesh(ref ulong space, ref OVRPlugin.TriangleMeshInternal triangleMeshInternal);

		public static readonly Version version = new Version(1, 82, 0);
	}

	private static class OVRP_1_83_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetControllerState6(uint controllerMask, ref OVRPlugin.ControllerState6 controllerState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetVirtualKeyboardModelAnimationStates(ref OVRPlugin.VirtualKeyboardModelAnimationStatesInternal animationStates);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetVirtualKeyboardDirtyTextures(ref OVRPlugin.VirtualKeyboardTextureIdsInternal textureIds);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetVirtualKeyboardTextureData(ulong textureId, ref OVRPlugin.VirtualKeyboardTextureData textureData);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetVirtualKeyboardModelVisibility(ref OVRPlugin.VirtualKeyboardModelVisibility visibility);

		public static readonly Version version = new Version(1, 83, 0);
	}

	private static class OVRP_1_84_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreatePassthroughColorLut(OVRPlugin.PassthroughColorLutChannels channels, uint resolution, OVRPlugin.PassthroughColorLutData data, out ulong colorLut);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroyPassthroughColorLut(ulong colorLut);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_UpdatePassthroughColorLut(ulong colorLut, OVRPlugin.PassthroughColorLutData data);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetInsightPassthroughStyle2(int layerId, in OVRPlugin.InsightPassthroughStyle2 style);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetLayerRecommendedResolution(int layerId, out OVRPlugin.Sizei recommendedDimensions);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetEyeLayerRecommendedResolution(out OVRPlugin.Sizei recommendedDimensions);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerStart(int markerId, int instanceKey, long timestampMs);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerEnd(int markerId, OVRPlugin.Qpl.ResultType resultTypeId, int instanceKey, long timestampMs);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerPoint(int markerId, [MarshalAs(UnmanagedType.LPStr)] string name, int instanceKey, long timestampMs);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerPointCached(int markerId, int nameHandle, int instanceKey, long timestampMs);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerAnnotation(int markerId, [MarshalAs(UnmanagedType.LPStr)] string annotationKey, [MarshalAs(UnmanagedType.LPStr)] string annotationValue, int instanceKey);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplCreateMarkerHandle([MarshalAs(UnmanagedType.LPStr)] string name, out int nameHandle);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplDestroyMarkerHandle(int nameHandle);

		public static readonly Version version = new Version(1, 84, 0);
	}

	private static class OVRP_1_85_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_OnEditorShutdown();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetPassthroughCapabilities(ref OVRPlugin.PassthroughCapabilities capabilityFlags);

		public static readonly Version version = new Version(1, 85, 0);
	}

	private static class OVRP_1_86_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetControllerDrivenHandPoses(OVRPlugin.Bool controllerDrivenHandPoses);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_IsControllerDrivenHandPosesEnabled(ref OVRPlugin.Bool enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_AreHandPosesGeneratedByControllerData(OVRPlugin.Step stepId, OVRPlugin.Node nodeId, ref OVRPlugin.Bool isGeneratedByControllerData);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetMultimodalHandsControllersSupported(OVRPlugin.Bool supported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_IsMultimodalHandsControllersSupported(ref OVRPlugin.Bool supported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCurrentDetachedInteractionProfile(OVRPlugin.Hand hand, out OVRPlugin.InteractionProfile interactionProfile);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetControllerIsInHand(OVRPlugin.Step stepId, OVRPlugin.Node nodeId, ref OVRPlugin.Bool isInHand);

		public static readonly Version version = new Version(1, 86, 0);
	}

	private static class OVRP_1_87_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetPassthroughPreferences(out OVRPlugin.PassthroughPreferences preferences);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetEyeBufferSharpenType(OVRPlugin.LayerSharpenType sharpenType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetControllerDrivenHandPosesAreNatural(OVRPlugin.Bool controllerDrivenHandPosesAreNatural);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_AreControllerDrivenHandPosesNatural(ref OVRPlugin.Bool natural);

		public static readonly Version version = new Version(1, 87, 0);
	}

	private static class OVRP_1_88_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetSimultaneousHandsAndControllersEnabled(OVRPlugin.Bool enabled);

		public static readonly Version version = new Version(1, 88, 0);
	}

	private static class OVRP_1_89_0
	{
		public static readonly Version version = new Version(1, 89, 0);
	}

	private static class OVRP_1_90_0
	{
		public static readonly Version version = new Version(1, 90, 0);
	}

	private static class OVRP_1_91_0
	{
		public static readonly Version version = new Version(1, 91, 0);
	}

	private static class OVRP_1_92_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceState2(OVRPlugin.Step stepId, int frameIndex, out OVRPlugin.FaceState2Internal faceState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartFaceTracking2(OVRPlugin.FaceTrackingDataSource[] requestedDataSources, uint requestedDataSourcesCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StopFaceTracking2();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceTracking2Enabled(out OVRPlugin.Bool faceTracking2Enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceTracking2Supported(out OVRPlugin.Bool faceTracking2Enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_RequestBodyTrackingFidelity(OVRPlugin.BodyTrackingFidelity2 fidelity);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SuggestBodyTrackingCalibrationOverride(OVRPlugin.BodyTrackingCalibrationInfo calibrationInfo);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ResetBodyTrackingCalibration();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetBodyState4(OVRPlugin.Step stepId, int frameIndex, out OVRPlugin.BodyState4Internal bodyState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSkeleton3(OVRPlugin.SkeletonType skeletonType, out OVRPlugin.Skeleton3Internal skeleton);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartBodyTracking2(OVRPlugin.BodyJointSet jointSet);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplSetConsent(OVRPlugin.Bool consent);

		public static readonly Version version = new Version(1, 92, 0);
	}

	private static class OVRP_1_93_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetWideMotionModeHandPoses(OVRPlugin.Bool wideMotionModeHandPoses);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_IsWideMotionModeHandPosesEnabled(ref OVRPlugin.Bool enabled);

		public static readonly Version version = new Version(1, 93, 0);
	}

	private static class OVRP_1_94_0
	{
		public static readonly Version version = new Version(1, 94, 0);
	}

	private static class OVRP_1_95_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetActionStateBoolean(string path, ref OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetActionStateFloat(string path, ref float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetActionStatePose(string path, ref OVRPlugin.Posef value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetDeveloperTelemetryConsent(OVRPlugin.Bool consent);

		public static readonly Version version = new Version(1, 95, 0);
	}

	private static class OVRP_1_96_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerAnnotationVariant(int markerId, [MarshalAs(UnmanagedType.LPStr)] string annotationKey, in OVRPlugin.Qpl.Variant annotationValue, int instanceKey);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern OVRPlugin.Result ovrp_QplMarkerPointData(int markerId, [MarshalAs(UnmanagedType.LPStr)] string name, OVRPlugin.Qpl.Annotation* annotations, int annotationCount, int instanceKey, long timestampMs);

		public static readonly Version version = new Version(1, 96, 0);
	}

	private static class OVRP_1_97_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DiscoverSpaces(in OVRPlugin.SpaceDiscoveryInfo info, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_RetrieveSpaceDiscoveryResults(ulong requestId, ref OVRPlugin.SpaceDiscoveryResults results);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern OVRPlugin.Result ovrp_SaveSpaces(uint spaceCount, ulong* spaces, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern OVRPlugin.Result ovrp_EraseSpaces(uint spaceCount, ulong* spaces, uint uuidCount, Guid* uuids, out ulong requestId);

		public static readonly Version version = new Version(1, 97, 0);
	}

	private static class OVRP_1_98_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_RequestBoundaryVisibility(OVRPlugin.BoundaryVisibility boundaryVisibility);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetBoundaryVisibility(out OVRPlugin.BoundaryVisibility boundaryVisibility);

		public static readonly Version version = new Version(1, 98, 0);
	}

	private static class OVRP_1_99_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetTrackingPoseEnabledForInvisibleSession(out OVRPlugin.Bool trackingPoseEnabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetTrackingPoseEnabledForInvisibleSession(OVRPlugin.Bool trackingPoseEnabled);

		public static readonly Version version = new Version(1, 99, 0);
	}

	private static class OVRP_1_100_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetCurrentInteractionProfileName(OVRPlugin.Hand hand, IntPtr interactionProfile);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetActionStatePose2(string path, OVRPlugin.Hand hand, ref OVRPlugin.Posef value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_TriggerVibrationAction(string actionName, OVRPlugin.Hand hand, float duration, float amplitude);

		public static readonly Version version = new Version(1, 100, 0);
	}

	private static class OVRP_1_101_0
	{
		public static readonly Version version = new Version(1, 101, 0);
	}

	private static class OVRP_1_102_0
	{
		public static readonly Version version = new Version(1, 102, 0);
	}

	private static class OVRP_1_103_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetHandSkeletonVersion(OVRHandSkeletonVersion handSkeletonVersion);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHandState3(OVRPlugin.Step stepId, int frameIndex, OVRPlugin.Hand hand, out OVRPlugin.HandState3Internal handState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_PollFuture(ulong future, out OVRPlugin.FutureState state);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CancelFuture(ulong future);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartColocationAdvertisement(in OVRPlugin.ColocationSessionStartAdvertisementInfo info, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StopColocationAdvertisement(out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StartColocationDiscovery(out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_StopColocationDiscovery(out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_ShareSpaces2(in OVRPlugin.ShareSpacesInfo info, out ulong requestId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QuerySpaces2(ref OVRPlugin.SpaceQueryInfo2 queryInfo, out ulong requestId);

		public static readonly Version version = new Version(1, 103, 0);
	}

	private static class OVRP_1_104_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetOpenXRInstanceProcAddrFunc(ref IntPtr func);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_RegisterOpenXREventHandler(OVRPlugin.OpenXREventDelegateType eventHandler, IntPtr context);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_UnregisterOpenXREventHandler(OVRPlugin.OpenXREventDelegateType eventHandler);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceVisemesState(OVRPlugin.Step stepId, int frameIndex, out OVRPlugin.FaceVisemesStateInternal faceVisemesState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetFaceTrackingVisemesSupported(out OVRPlugin.Bool faceTrackingVisemesSupported);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetFaceTrackingVisemesEnabled(OVRPlugin.Bool enabled);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateDynamicObjectTracker(out ulong tracker);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroyDynamicObjectTracker(ulong tracker);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetDynamicObjectTrackedClasses(ulong tracker, in OVRPlugin.DynamicObjectTrackedClassesSetInfo setInfo);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceDynamicObjectData(ref ulong space, out OVRPlugin.DynamicObjectData data);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetDynamicObjectTrackerSupported(out OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetDynamicObjectKeyboardSupported(out OVRPlugin.Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_BeginProfilingRegion(string regionName);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_EndProfilingRegion();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetExternalLayerDynresEnabled(OVRPlugin.Bool enabled);

		public static readonly Version version = new Version(1, 104, 0);
	}

	private static class OVRP_1_105_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_QplMarkerStartForJoin(int markerId, string joinId, OVRPlugin.Bool cancelMarkerIfAppBackgrounded, int instanceKey, long timestampMs);

		public static readonly Version version = new Version(1, 105, 0);
	}

	private static class OVRP_1_106_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetHandTrackingState(OVRPlugin.Step stepId, int frameIndex, OVRPlugin.Hand hand, out OVRPlugin.HandTrackingStateInternal handState);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SaveUnifiedConsent(int toolId, OVRPlugin.Bool consentValue);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SaveUnifiedConsentWithOlderVersion(int toolId, OVRPlugin.Bool consentValue, int consentVersion);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.OptionalBool ovrp_GetUnifiedConsent(int toolId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetConsentTitle(IntPtr title);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetConsentMarkdownText(IntPtr markdownText);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetConsentNotificationMarkdownText(IntPtr consentChangeLocationMarkdown, IntPtr markDownText);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_ShouldShowTelemetryConsentWindow(int toolId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_IsConsentSettingsChangeEnabled(int toolId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Bool ovrp_ShouldShowTelemetryNotification(int toolId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SendMicrogestureHint();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SetNotificationShown(int tool);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetConsentSettingsChangeText(IntPtr consentSettingsChangeText);

		public const int OVRP_CONSENT_TITLE_MAX_LENGTH = 256;

		public const int OVRP_CONSENT_TEXT_MAX_LENGTH = 2048;

		public const int OVRP_CONSENT_NOTIFICATION_MAX_LENGTH = 1024;

		public const int OVRP_CONSENT_SETTINGS_CHANGE_MAX_LENGTH = 1024;

		public static readonly Version version = new Version(1, 106, 0);
	}

	private static class OVRP_1_107_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetAppSpace(ref ulong appSpace);

		public static readonly Version version = new Version(1, 107, 0);
	}

	private static class OVRP_1_108_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_UnityOpenXR_OnAppSpaceChange2(ulong xrSpace, int spaceFlags);

		public static readonly Version version = new Version(1, 108, 0);
	}

	private static class OVRP_1_109_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetStationaryReferenceSpaceId(out Guid generationId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SendUnifiedEvent(OVRPlugin.Bool isEssential, string productType, string eventName, string event_metadata_json, string project_name, string event_entrypoint, string project_guid, string event_type, string event_target, string error_msg, string is_internal);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovrp_AllowVisibilityMask(OVRPlugin.Bool enabled);

		public static readonly Version version = new Version(1, 109, 0);
	}

	private static class OVRP_1_110_0
	{
		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_SendUnifiedEventV2(OVRPlugin.Bool isEssential, string productType, string eventName, string event_metadata_json, string project_name, string event_entrypoint, string project_guid, string event_type, string event_target, string error_msg, string is_internal_build, string batch_mode);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateMarkerTrackerAsync(in OVRPlugin.MarkerTrackerCreateInfo createInfo, out ulong future);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_CreateMarkerTrackerComplete(ulong future, out OVRPlugin.MarkerTrackerCreateCompletion completion);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_DestroyMarkerTracker(ulong tracker);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetSpaceMarkerPayload(ulong space, ref OVRPlugin.SpaceMarkerPayload payload);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern OVRPlugin.Result ovrp_GetMarkerTrackingSupported(out OVRPlugin.Bool value);

		public static readonly Version version = new Version(1, 110, 0);
	}

	private static class OVRP_1_111_0
	{
		public static readonly Version version = new Version(1, 111, 0);
	}

	private static class OVRP_1_112_0
	{
		public static readonly Version version = new Version(1, 112, 0);
	}

	private static class OVRP_1_113_0
	{
		public static readonly Version version = new Version(1, 113, 0);
	}

	private static class OVRP_1_114_0
	{
		public static readonly Version version = new Version(1, 114, 0);
	}

	private static class OVRP_1_115_0
	{
		public static readonly Version version = new Version(1, 115, 0);
	}

	private static class OVRP_1_116_0
	{
		public static readonly Version version = new Version(1, 116, 0);
	}

	private static class OVRP_1_117_0
	{
		public static readonly Version version = new Version(1, 117, 0);
	}

	private static class OVRP_1_118_0
	{
		public static readonly Version version = new Version(1, 118, 0);
	}

	private static class OVRP_1_119_0
	{
		public static readonly Version version = new Version(1, 119, 0);
	}

	private static class OVRP_1_120_0
	{
		public static readonly Version version = new Version(1, 120, 0);
	}

	private static class OVRP_1_121_0
	{
		public static readonly Version version = new Version(1, 121, 0);
	}

	private static class OVRP_1_122_0
	{
		public static readonly Version version = new Version(1, 122, 0);
	}

	private static class OVRP_1_123_0
	{
		public static readonly Version version = new Version(1, 123, 0);
	}

	private static class OVRP_1_124_0
	{
		public static readonly Version version = new Version(1, 124, 0);
	}

	private static class OVRP_1_125_0
	{
		public static readonly Version version = new Version(1, 125, 0);
	}

	private static class OVRP_1_126_0
	{
		public static readonly Version version = new Version(1, 126, 0);
	}

	private static class OVRP_1_127_0
	{
		public static readonly Version version = new Version(1, 127, 0);
	}

	private static class OVRP_1_128_0
	{
		public static readonly Version version = new Version(1, 128, 0);
	}

	private static class OVRP_1_129_0
	{
		public static readonly Version version = new Version(1, 129, 0);
	}
}
