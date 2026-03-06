using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;
using UnityEngine.XR.OpenXR.Features;

namespace UnityEngine.XR.OpenXR
{
	[Serializable]
	public class OpenXRSettings : ScriptableObject, ISerializationCallbackReceiver
	{
		public int featureCount
		{
			get
			{
				return this.features.Length;
			}
		}

		public TFeature GetFeature<TFeature>() where TFeature : OpenXRFeature
		{
			return (TFeature)((object)this.GetFeature(typeof(TFeature)));
		}

		public OpenXRFeature GetFeature(Type featureType)
		{
			foreach (OpenXRFeature openXRFeature in this.features)
			{
				if (featureType.IsInstanceOfType(openXRFeature))
				{
					return openXRFeature;
				}
			}
			return null;
		}

		public OpenXRFeature[] GetFeatures<TFeature>()
		{
			return this.GetFeatures(typeof(TFeature));
		}

		public OpenXRFeature[] GetFeatures(Type featureType)
		{
			List<OpenXRFeature> list = new List<OpenXRFeature>();
			foreach (OpenXRFeature openXRFeature in this.features)
			{
				if (featureType.IsInstanceOfType(openXRFeature))
				{
					list.Add(openXRFeature);
				}
			}
			return list.ToArray();
		}

		public int GetFeatures<TFeature>(List<TFeature> featuresOut) where TFeature : OpenXRFeature
		{
			featuresOut.Clear();
			OpenXRFeature[] array = this.features;
			for (int i = 0; i < array.Length; i++)
			{
				TFeature tfeature = array[i] as TFeature;
				if (tfeature != null)
				{
					featuresOut.Add(tfeature);
				}
			}
			return featuresOut.Count;
		}

		public int GetFeatures(Type featureType, List<OpenXRFeature> featuresOut)
		{
			featuresOut.Clear();
			foreach (OpenXRFeature openXRFeature in this.features)
			{
				if (featureType.IsInstanceOfType(openXRFeature))
				{
					featuresOut.Add(openXRFeature);
				}
			}
			return featuresOut.Count;
		}

		public OpenXRFeature[] GetFeatures()
		{
			OpenXRFeature[] array = this.features;
			return ((OpenXRFeature[])((array != null) ? array.Clone() : null)) ?? new OpenXRFeature[0];
		}

		public int GetFeatures(List<OpenXRFeature> featuresOut)
		{
			featuresOut.Clear();
			featuresOut.AddRange(this.features);
			return featuresOut.Count;
		}

		private void ApplyPermissionSettings()
		{
		}

		[DllImport("UnityOpenXR", EntryPoint = "OculusFoveation_SetHasEyeTrackingPermissions")]
		internal static extern void Internal_SetHasEyeTrackingPermissions([MarshalAs(UnmanagedType.I1)] bool value);

		public OpenXRSettings.RenderMode renderMode
		{
			get
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					return OpenXRSettings.Internal_GetRenderMode();
				}
				return this.m_renderMode;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetRenderMode(value);
					return;
				}
				this.m_renderMode = value;
			}
		}

		public OpenXRSettings.LatencyOptimization latencyOptimization
		{
			get
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					return OpenXRSettings.Internal_GetLatencyOptimization();
				}
				return this.m_latencyOptimization;
			}
			set
			{
				this.m_latencyOptimization = value;
			}
		}

		public bool autoColorSubmissionMode
		{
			get
			{
				return this.m_autoColorSubmissionMode;
			}
			set
			{
				this.m_autoColorSubmissionMode = value;
			}
		}

		public OpenXRSettings.ColorSubmissionModeGroup[] colorSubmissionModes
		{
			get
			{
				if (this.m_autoColorSubmissionMode)
				{
					return new OpenXRSettings.ColorSubmissionModeGroup[]
					{
						OpenXRSettings.kDefaultColorMode
					};
				}
				if (OpenXRLoaderBase.Instance != null)
				{
					int num = OpenXRSettings.Internal_GetColorSubmissionModes(null, 0);
					int[] array = new int[num];
					OpenXRSettings.Internal_GetColorSubmissionModes(array, num);
					return (from i in array
					select (OpenXRSettings.ColorSubmissionModeGroup)i).ToArray<OpenXRSettings.ColorSubmissionModeGroup>();
				}
				return this.m_colorSubmissionModes.m_List;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetColorSubmissionModes((from e in value
					select (int)e).ToArray<int>(), value.Length);
					return;
				}
				this.m_colorSubmissionModes.m_List = value;
			}
		}

		public OpenXRSettings.DepthSubmissionMode depthSubmissionMode
		{
			get
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					return OpenXRSettings.Internal_GetDepthSubmissionMode();
				}
				return this.m_depthSubmissionMode;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetDepthSubmissionMode(value);
					return;
				}
				this.m_depthSubmissionMode = value;
			}
		}

		public OpenXRSettings.SpaceWarpMotionVectorTextureFormat spacewarpMotionVectorTextureFormat
		{
			get
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					return OpenXRSettings.Internal_GetSpaceWarpMotionVectorTextureFormat();
				}
				return this.m_spacewarpMotionVectorTextureFormat;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetSpaceWarpMotionVectorTextureFormat(value);
					return;
				}
				this.m_spacewarpMotionVectorTextureFormat = value;
			}
		}

		public bool optimizeBufferDiscards
		{
			get
			{
				return this.m_optimizeBufferDiscards;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetOptimizeBufferDiscards(value);
					return;
				}
				this.m_optimizeBufferDiscards = value;
			}
		}

		private void ApplyRenderSettings()
		{
			OpenXRSettings.Internal_SetSymmetricProjection(this.m_symmetricProjection);
			OpenXRSettings.Internal_SetMultiviewRenderRegionsOptimizationMode(this.m_multiviewRenderRegionsOptimizationMode);
			OpenXRSettings.Internal_SetUseOpenXRPredictedTime(this.m_useOpenXRPredictedTime);
			OpenXRSettings.Internal_SetUsedFoveatedRenderingApi(this.m_foveatedRenderingApi);
			OpenXRSettings.Internal_SetRenderMode(this.m_renderMode);
			OpenXRSettings.Internal_SetLatencyOptimization(this.m_latencyOptimization);
			OpenXRSettings.Internal_SetColorSubmissionModes((from e in this.m_colorSubmissionModes.m_List
			select (int)e).ToArray<int>(), this.m_colorSubmissionModes.m_List.Length);
			OpenXRSettings.Internal_SetDepthSubmissionMode(this.m_depthSubmissionMode);
			OpenXRSettings.Internal_SetSpaceWarpMotionVectorTextureFormat(this.m_spacewarpMotionVectorTextureFormat);
			OpenXRSettings.Internal_SetOptimizeBufferDiscards(this.m_optimizeBufferDiscards);
		}

		public void OnBeforeSerialize()
		{
			this.m_optimizeMultiviewRenderRegions = (this.m_multiviewRenderRegionsOptimizationMode > OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None);
		}

		public void OnAfterDeserialize()
		{
			if (!this.m_hasMigratedMultiviewRenderRegionSetting)
			{
				if (this.m_optimizeMultiviewRenderRegions)
				{
					this.m_multiviewRenderRegionsOptimizationMode = OpenXRSettings.MultiviewRenderRegionsOptimizationMode.FinalPass;
				}
				else
				{
					this.m_multiviewRenderRegionsOptimizationMode = OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None;
				}
				this.m_hasMigratedMultiviewRenderRegionSetting = true;
			}
		}

		public bool symmetricProjection
		{
			get
			{
				return this.m_symmetricProjection;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetSymmetricProjection(value);
					return;
				}
				this.m_symmetricProjection = value;
			}
		}

		[Obsolete("optimizeMultiviewRenderRegions is deprecated. Use multiviewRenderRegionsMode instead.", false)]
		public bool optimizeMultiviewRenderRegions
		{
			get
			{
				return this.m_multiviewRenderRegionsOptimizationMode == OpenXRSettings.MultiviewRenderRegionsOptimizationMode.FinalPass || this.m_multiviewRenderRegionsOptimizationMode == OpenXRSettings.MultiviewRenderRegionsOptimizationMode.AllPasses;
			}
			set
			{
				OpenXRSettings.MultiviewRenderRegionsOptimizationMode multiviewRenderRegionsOptimizationMode = value ? OpenXRSettings.MultiviewRenderRegionsOptimizationMode.FinalPass : OpenXRSettings.MultiviewRenderRegionsOptimizationMode.None;
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetMultiviewRenderRegionsOptimizationMode(multiviewRenderRegionsOptimizationMode);
					return;
				}
				this.m_optimizeMultiviewRenderRegions = value;
				this.m_multiviewRenderRegionsOptimizationMode = multiviewRenderRegionsOptimizationMode;
			}
		}

		public OpenXRSettings.MultiviewRenderRegionsOptimizationMode multiviewRenderRegionsOptimizationMode
		{
			get
			{
				return this.m_multiviewRenderRegionsOptimizationMode;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetMultiviewRenderRegionsOptimizationMode(value);
					return;
				}
				this.m_multiviewRenderRegionsOptimizationMode = value;
			}
		}

		public OpenXRSettings.BackendFovationApi foveatedRenderingApi
		{
			get
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					return OpenXRSettings.Internal_GetUsedFoveatedRenderingApi();
				}
				return this.m_foveatedRenderingApi;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetUsedFoveatedRenderingApi(value);
					return;
				}
				this.m_foveatedRenderingApi = value;
			}
		}

		public bool useOpenXRPredictedTime
		{
			get
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					return OpenXRSettings.Internal_GetUseOpenXRPredictedTime();
				}
				return this.m_useOpenXRPredictedTime;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null)
				{
					OpenXRSettings.Internal_SetUseOpenXRPredictedTime(value);
					return;
				}
				this.m_useOpenXRPredictedTime = value;
			}
		}

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetRenderMode")]
		private static extern void Internal_SetRenderMode(OpenXRSettings.RenderMode renderMode);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetRenderMode")]
		private static extern OpenXRSettings.RenderMode Internal_GetRenderMode();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetLatencyOptimization")]
		private static extern void Internal_SetLatencyOptimization(OpenXRSettings.LatencyOptimization latencyOptimzation);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetLatencyOptimization")]
		private static extern OpenXRSettings.LatencyOptimization Internal_GetLatencyOptimization();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetDepthSubmissionMode")]
		private static extern void Internal_SetDepthSubmissionMode(OpenXRSettings.DepthSubmissionMode depthSubmissionMode);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetDepthSubmissionMode")]
		private static extern OpenXRSettings.DepthSubmissionMode Internal_GetDepthSubmissionMode();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetSpaceWarpMotionVectorTextureFormat")]
		private static extern void Internal_SetSpaceWarpMotionVectorTextureFormat(OpenXRSettings.SpaceWarpMotionVectorTextureFormat spaceWarpMotionVectorTextureFormat);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetSpaceWarpMotionVectorTextureFormat")]
		private static extern OpenXRSettings.SpaceWarpMotionVectorTextureFormat Internal_GetSpaceWarpMotionVectorTextureFormat();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetSymmetricProjection")]
		private static extern void Internal_SetSymmetricProjection([MarshalAs(UnmanagedType.I1)] bool enabled);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetMultiviewRenderRegionsOptimizationMode")]
		private static extern void Internal_SetMultiviewRenderRegionsOptimizationMode(OpenXRSettings.MultiviewRenderRegionsOptimizationMode mode);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetOptimizeBufferDiscards")]
		private static extern void Internal_SetOptimizeBufferDiscards([MarshalAs(UnmanagedType.I1)] bool enabled);

		[DllImport("UnityOpenXR", EntryPoint = "OculusFoveation_SetUsedApi")]
		private static extern void Internal_SetUsedFoveatedRenderingApi(OpenXRSettings.BackendFovationApi api);

		[DllImport("UnityOpenXR", EntryPoint = "OculusFoveation_GetUsedApi")]
		internal static extern OpenXRSettings.BackendFovationApi Internal_GetUsedFoveatedRenderingApi();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetColorSubmissionMode")]
		private static extern void Internal_SetColorSubmissionMode(OpenXRSettings.ColorSubmissionModeGroup[] colorSubmissionMode);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetColorSubmissionModes")]
		private static extern void Internal_SetColorSubmissionModes(int[] colorSubmissionMode, int arraySize);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetColorSubmissionModes")]
		private static extern int Internal_GetColorSubmissionModes([Out] int[] colorSubmissionMode, int arraySize);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetIsUsingLegacyXRDisplay")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetIsUsingLegacyXRDisplay();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetUseOpenXRPredictedTime")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetUseOpenXRPredictedTime();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetUseOpenXRPredictedTime")]
		private static extern void Internal_SetUseOpenXRPredictedTime([MarshalAs(UnmanagedType.I1)] bool enabled);

		private void Awake()
		{
			OpenXRSettings.s_RuntimeInstance = this;
		}

		internal void ApplySettings()
		{
			this.ApplyRenderSettings();
			this.ApplyPermissionSettings();
		}

		private static OpenXRSettings GetInstance(bool useActiveBuildTarget)
		{
			OpenXRSettings openXRSettings = OpenXRSettings.s_RuntimeInstance;
			if (openXRSettings == null)
			{
				openXRSettings = ScriptableObject.CreateInstance<OpenXRSettings>();
			}
			return openXRSettings;
		}

		public static OpenXRSettings ActiveBuildTargetInstance
		{
			get
			{
				return OpenXRSettings.GetInstance(true);
			}
		}

		public static OpenXRSettings Instance
		{
			get
			{
				return OpenXRSettings.GetInstance(false);
			}
		}

		public static void SetAllowRecentering(bool allowRecentering, float floorOffset = 1.5f)
		{
			OpenXRSettings.Internal_SetAllowRecentering(allowRecentering, floorOffset);
		}

		public static void RefreshRecenterSpace()
		{
			OpenXRSettings.Internal_RegenerateTrackingOrigin();
		}

		public static bool AllowRecentering
		{
			get
			{
				return OpenXRSettings.Internal_GetAllowRecentering();
			}
		}

		public static float FloorOffset
		{
			get
			{
				return OpenXRSettings.Internal_GetFloorOffset();
			}
		}

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetAllowRecentering")]
		private static extern void Internal_SetAllowRecentering([MarshalAs(UnmanagedType.U1)] bool active, float height);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_RegenerateTrackingOrigin")]
		private static extern void Internal_RegenerateTrackingOrigin();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetAllowRecentering")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetAllowRecentering();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetFloorOffsetHeight")]
		private static extern float Internal_GetFloorOffset();

		[FormerlySerializedAs("extensions")]
		[HideInInspector]
		[SerializeField]
		internal OpenXRFeature[] features = new OpenXRFeature[0];

		public static readonly OpenXRSettings.ColorSubmissionModeGroup kDefaultColorMode;

		[SerializeField]
		private OpenXRSettings.RenderMode m_renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;

		[SerializeField]
		private OpenXRSettings.LatencyOptimization m_latencyOptimization;

		[SerializeField]
		private bool m_autoColorSubmissionMode = true;

		[SerializeField]
		private OpenXRSettings.ColorSubmissionModeList m_colorSubmissionModes = new OpenXRSettings.ColorSubmissionModeList();

		[SerializeField]
		private OpenXRSettings.DepthSubmissionMode m_depthSubmissionMode;

		[SerializeField]
		private OpenXRSettings.SpaceWarpMotionVectorTextureFormat m_spacewarpMotionVectorTextureFormat;

		[SerializeField]
		private bool m_optimizeBufferDiscards;

		[SerializeField]
		private bool m_symmetricProjection;

		[SerializeField]
		[HideInInspector]
		[Obsolete("m_optimizeMultiviewRenderRegions is deprecated. Use m_multiviewRenderRegionsOptimizationMode instead.", false)]
		private bool m_optimizeMultiviewRenderRegions;

		[SerializeField]
		[HideInInspector]
		private OpenXRSettings.MultiviewRenderRegionsOptimizationMode m_multiviewRenderRegionsOptimizationMode;

		[SerializeField]
		[HideInInspector]
		private bool m_hasMigratedMultiviewRenderRegionSetting;

		[SerializeField]
		private OpenXRSettings.BackendFovationApi m_foveatedRenderingApi;

		[SerializeField]
		private bool m_useOpenXRPredictedTime;

		private const string LibraryName = "UnityOpenXR";

		private static OpenXRSettings s_RuntimeInstance;

		public enum ColorSubmissionModeGroup
		{
			[InspectorName("8 bits per channel (LDR, default)")]
			kRenderTextureFormatGroup8888,
			[InspectorName("10 bits floating-point per color channel, 2 bit alpha (HDR)")]
			kRenderTextureFormatGroup1010102_Float,
			[InspectorName("16 bits floating-point per channel (HDR)")]
			kRenderTextureFormatGroup16161616_Float,
			[InspectorName("5,6,5 bit packed (LDR, mobile)")]
			kRenderTextureFormatGroup565,
			[InspectorName("11,11,10 bit packed floating-point (HDR)")]
			kRenderTextureFormatGroup111110_Float
		}

		[Serializable]
		public class ColorSubmissionModeList
		{
			public OpenXRSettings.ColorSubmissionModeGroup[] m_List = new OpenXRSettings.ColorSubmissionModeGroup[1];
		}

		public enum RenderMode
		{
			MultiPass,
			SinglePassInstanced
		}

		public enum LatencyOptimization
		{
			PrioritizeRendering,
			PrioritizeInputPolling
		}

		public enum DepthSubmissionMode
		{
			None,
			Depth16Bit,
			Depth24Bit
		}

		public enum BackendFovationApi : byte
		{
			Legacy,
			SRPFoveation
		}

		public enum SpaceWarpMotionVectorTextureFormat
		{
			RGBA16f,
			RG16f
		}

		public enum MultiviewRenderRegionsOptimizationMode : byte
		{
			None,
			FinalPass,
			AllPasses
		}
	}
}
