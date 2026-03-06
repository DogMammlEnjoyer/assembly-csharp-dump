using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public class DynamicResolutionHandler
	{
		private void Reset()
		{
			this.m_Enabled = false;
			this.m_UseMipBias = false;
			this.m_MinScreenFraction = 1f;
			this.m_MaxScreenFraction = 1f;
			this.m_CurrentFraction = 1f;
			this.m_ForcingRes = false;
			this.m_CurrentCameraRequest = true;
			this.m_PrevFraction = -1f;
			this.m_ForceSoftwareFallback = false;
			this.m_RunUpscalerFilterOnFullResolution = false;
			this.m_PrevHWScaleWidth = 1f;
			this.m_PrevHWScaleHeight = 1f;
			this.m_LastScaledSize = new Vector2Int(0, 0);
			this.filter = DynamicResUpscaleFilter.CatmullRom;
		}

		public DynamicResUpscaleFilter filter { get; private set; }

		public Vector2Int finalViewport { get; set; }

		public bool runUpscalerFilterOnFullResolution
		{
			get
			{
				return this.m_RunUpscalerFilterOnFullResolution || this.filter == DynamicResUpscaleFilter.EdgeAdaptiveScalingUpres;
			}
			set
			{
				this.m_RunUpscalerFilterOnFullResolution = value;
			}
		}

		public bool forcingResolution
		{
			get
			{
				return this.m_ForcingRes;
			}
		}

		private bool FlushScalableBufferManagerState()
		{
			if (DynamicResolutionHandler.s_GlobalHwUpresActive == this.HardwareDynamicResIsEnabled() && DynamicResolutionHandler.s_GlobalHwFraction == this.m_CurrentFraction)
			{
				return false;
			}
			DynamicResolutionHandler.s_GlobalHwUpresActive = this.HardwareDynamicResIsEnabled();
			DynamicResolutionHandler.s_GlobalHwFraction = this.m_CurrentFraction;
			float num = DynamicResolutionHandler.s_GlobalHwUpresActive ? DynamicResolutionHandler.s_GlobalHwFraction : 1f;
			ScalableBufferManager.ResizeBuffers(num, num);
			return true;
		}

		private static DynamicResolutionHandler GetOrCreateDrsInstanceHandler(Camera camera)
		{
			if (camera == null)
			{
				return null;
			}
			DynamicResolutionHandler dynamicResolutionHandler = null;
			int instanceID = camera.GetInstanceID();
			if (!DynamicResolutionHandler.s_CameraInstances.TryGetValue(instanceID, out dynamicResolutionHandler))
			{
				if (DynamicResolutionHandler.s_CameraInstances.Count >= 32)
				{
					int key = 0;
					DynamicResolutionHandler dynamicResolutionHandler2 = null;
					foreach (KeyValuePair<int, DynamicResolutionHandler> keyValuePair in DynamicResolutionHandler.s_CameraInstances)
					{
						if (keyValuePair.Value.m_OwnerCameraWeakRef == null || !keyValuePair.Value.m_OwnerCameraWeakRef.IsAlive)
						{
							dynamicResolutionHandler2 = keyValuePair.Value;
							key = keyValuePair.Key;
							break;
						}
					}
					if (dynamicResolutionHandler2 != null)
					{
						dynamicResolutionHandler = dynamicResolutionHandler2;
						DynamicResolutionHandler.s_CameraInstances.Remove(key);
						DynamicResolutionHandler.s_CameraUpscaleFilters.Remove(key);
					}
				}
				if (dynamicResolutionHandler == null)
				{
					dynamicResolutionHandler = new DynamicResolutionHandler();
					dynamicResolutionHandler.m_OwnerCameraWeakRef = new WeakReference(camera);
				}
				else
				{
					dynamicResolutionHandler.Reset();
					dynamicResolutionHandler.m_OwnerCameraWeakRef.Target = camera;
				}
				DynamicResolutionHandler.s_CameraInstances.Add(instanceID, dynamicResolutionHandler);
			}
			return dynamicResolutionHandler;
		}

		public DynamicResolutionHandler.UpsamplerScheduleType upsamplerSchedule
		{
			get
			{
				return this.m_UpsamplerSchedule;
			}
			set
			{
				this.m_UpsamplerSchedule = value;
			}
		}

		public static DynamicResolutionHandler instance
		{
			get
			{
				return DynamicResolutionHandler.s_ActiveInstance;
			}
		}

		private DynamicResolutionHandler()
		{
			this.Reset();
		}

		private static float DefaultDynamicResMethod()
		{
			return 1f;
		}

		private void ProcessSettings(GlobalDynamicResolutionSettings settings)
		{
			this.m_Enabled = (settings.enabled && (Application.isPlaying || settings.forceResolution));
			if (!this.m_Enabled)
			{
				this.m_CurrentFraction = 1f;
			}
			else
			{
				this.type = settings.dynResType;
				this.m_UseMipBias = settings.useMipBias;
				float minScreenFraction = Mathf.Clamp(settings.minPercentage / 100f, 0.1f, 1f);
				this.m_MinScreenFraction = minScreenFraction;
				float maxScreenFraction = Mathf.Clamp(settings.maxPercentage / 100f, this.m_MinScreenFraction, 3f);
				this.m_MaxScreenFraction = maxScreenFraction;
				DynamicResUpscaleFilter dynamicResUpscaleFilter;
				this.filter = (DynamicResolutionHandler.s_CameraUpscaleFilters.TryGetValue(DynamicResolutionHandler.s_ActiveCameraId, out dynamicResUpscaleFilter) ? dynamicResUpscaleFilter : settings.upsampleFilter);
				this.m_ForcingRes = settings.forceResolution;
				if (this.m_ForcingRes)
				{
					float currentFraction = Mathf.Clamp(settings.forcedPercentage / 100f, 0.1f, 1.5f);
					this.m_CurrentFraction = currentFraction;
				}
			}
			this.m_CachedSettings = settings;
		}

		public Vector2 GetResolvedScale()
		{
			if (!this.m_Enabled || !this.m_CurrentCameraRequest)
			{
				return new Vector2(1f, 1f);
			}
			float x = this.m_CurrentFraction;
			float y = this.m_CurrentFraction;
			if (!this.m_ForceSoftwareFallback && this.type == DynamicResolutionType.Hardware)
			{
				x = ScalableBufferManager.widthScaleFactor;
				y = ScalableBufferManager.heightScaleFactor;
			}
			return new Vector2(x, y);
		}

		public float CalculateMipBias(Vector2Int inputResolution, Vector2Int outputResolution, bool forceApply = false)
		{
			if (!this.m_UseMipBias && !forceApply)
			{
				return 0f;
			}
			return (float)Math.Log((double)inputResolution.x / (double)outputResolution.x, 2.0);
		}

		public static void SetDynamicResScaler(PerformDynamicRes scaler, DynamicResScalePolicyType scalerType = DynamicResScalePolicyType.ReturnsMinMaxLerpFactor)
		{
			DynamicResolutionHandler.s_ScalerContainers[0] = new DynamicResolutionHandler.ScalerContainer
			{
				type = scalerType,
				method = scaler
			};
		}

		public static void SetSystemDynamicResScaler(PerformDynamicRes scaler, DynamicResScalePolicyType scalerType = DynamicResScalePolicyType.ReturnsMinMaxLerpFactor)
		{
			DynamicResolutionHandler.s_ScalerContainers[1] = new DynamicResolutionHandler.ScalerContainer
			{
				type = scalerType,
				method = scaler
			};
		}

		public static void SetActiveDynamicScalerSlot(DynamicResScalerSlot slot)
		{
			DynamicResolutionHandler.s_ActiveScalerSlot = slot;
		}

		public static void ClearSelectedCamera()
		{
			DynamicResolutionHandler.s_ActiveInstance = DynamicResolutionHandler.s_DefaultInstance;
			DynamicResolutionHandler.s_ActiveCameraId = 0;
			DynamicResolutionHandler.s_ActiveInstanceDirty = true;
		}

		public static void SetUpscaleFilter(Camera camera, DynamicResUpscaleFilter filter)
		{
			int instanceID = camera.GetInstanceID();
			if (DynamicResolutionHandler.s_CameraUpscaleFilters.ContainsKey(instanceID))
			{
				DynamicResolutionHandler.s_CameraUpscaleFilters[instanceID] = filter;
				return;
			}
			DynamicResolutionHandler.s_CameraUpscaleFilters.Add(instanceID, filter);
		}

		public void SetCurrentCameraRequest(bool cameraRequest)
		{
			this.m_CurrentCameraRequest = cameraRequest;
		}

		public static void UpdateAndUseCamera(Camera camera, GlobalDynamicResolutionSettings? settings = null, Action OnResolutionChange = null)
		{
			int num;
			if (camera == null)
			{
				DynamicResolutionHandler.s_ActiveInstance = DynamicResolutionHandler.s_DefaultInstance;
				num = 0;
			}
			else
			{
				DynamicResolutionHandler.s_ActiveInstance = DynamicResolutionHandler.GetOrCreateDrsInstanceHandler(camera);
				num = camera.GetInstanceID();
			}
			DynamicResolutionHandler.s_ActiveInstanceDirty = (num != DynamicResolutionHandler.s_ActiveCameraId);
			DynamicResolutionHandler.s_ActiveCameraId = num;
			DynamicResolutionHandler.s_ActiveInstance.Update((settings != null) ? settings.Value : DynamicResolutionHandler.s_ActiveInstance.m_CachedSettings, OnResolutionChange);
		}

		public void Update(GlobalDynamicResolutionSettings settings, Action OnResolutionChange = null)
		{
			this.ProcessSettings(settings);
			if (!this.m_Enabled || !DynamicResolutionHandler.s_ActiveInstanceDirty)
			{
				this.FlushScalableBufferManagerState();
				DynamicResolutionHandler.s_ActiveInstanceDirty = false;
				return;
			}
			if (!this.m_ForcingRes)
			{
				ref DynamicResolutionHandler.ScalerContainer ptr = ref DynamicResolutionHandler.s_ScalerContainers[(int)DynamicResolutionHandler.s_ActiveScalerSlot];
				if (ptr.type == DynamicResScalePolicyType.ReturnsMinMaxLerpFactor)
				{
					float t = Mathf.Clamp(ptr.method(), 0f, 1f);
					this.m_CurrentFraction = Mathf.Lerp(this.m_MinScreenFraction, this.m_MaxScreenFraction, t);
				}
				else if (ptr.type == DynamicResScalePolicyType.ReturnsPercentage)
				{
					float num = Mathf.Max(ptr.method(), 5f);
					this.m_CurrentFraction = Mathf.Clamp(num / 100f, this.m_MinScreenFraction, this.m_MaxScreenFraction);
				}
			}
			bool flag = false;
			bool flag2 = this.m_CurrentFraction != this.m_PrevFraction;
			this.m_PrevFraction = this.m_CurrentFraction;
			if (!this.m_ForceSoftwareFallback && this.type == DynamicResolutionType.Hardware)
			{
				flag = this.FlushScalableBufferManagerState();
				if (ScalableBufferManager.widthScaleFactor != this.m_PrevHWScaleWidth || ScalableBufferManager.heightScaleFactor != this.m_PrevHWScaleHeight)
				{
					flag = true;
				}
			}
			if ((flag2 || flag) && OnResolutionChange != null)
			{
				OnResolutionChange();
			}
			DynamicResolutionHandler.s_ActiveInstanceDirty = false;
			this.m_PrevHWScaleWidth = ScalableBufferManager.widthScaleFactor;
			this.m_PrevHWScaleHeight = ScalableBufferManager.heightScaleFactor;
		}

		public bool SoftwareDynamicResIsEnabled()
		{
			return this.m_CurrentCameraRequest && this.m_Enabled && (this.m_CurrentFraction != 1f || this.runUpscalerFilterOnFullResolution) && (this.m_ForceSoftwareFallback || this.type == DynamicResolutionType.Software);
		}

		public bool HardwareDynamicResIsEnabled()
		{
			return !this.m_ForceSoftwareFallback && this.m_CurrentCameraRequest && this.m_Enabled && this.type == DynamicResolutionType.Hardware;
		}

		public bool RequestsHardwareDynamicResolution()
		{
			return !this.m_ForceSoftwareFallback && this.type == DynamicResolutionType.Hardware;
		}

		public bool DynamicResolutionEnabled()
		{
			return this.m_CurrentCameraRequest && this.m_Enabled && (this.m_CurrentFraction != 1f || this.runUpscalerFilterOnFullResolution);
		}

		public void ForceSoftwareFallback()
		{
			this.m_ForceSoftwareFallback = true;
		}

		public Vector2Int GetScaledSize(Vector2Int size)
		{
			this.cachedOriginalSize = size;
			if (!this.m_Enabled || !this.m_CurrentCameraRequest)
			{
				return size;
			}
			Vector2Int vector2Int = this.ApplyScalesOnSize(size);
			this.m_LastScaledSize = vector2Int;
			return vector2Int;
		}

		public Vector2Int ApplyScalesOnSize(Vector2Int size)
		{
			return this.ApplyScalesOnSize(size, this.GetResolvedScale());
		}

		internal Vector2Int ApplyScalesOnSize(Vector2Int size, Vector2 scales)
		{
			Vector2Int result = new Vector2Int(Mathf.CeilToInt((float)size.x * scales.x), Mathf.CeilToInt((float)size.y * scales.y));
			if (this.m_ForceSoftwareFallback || this.type != DynamicResolutionType.Hardware)
			{
				result.x += (1 & result.x);
				result.y += (1 & result.y);
			}
			result.x = Math.Min(result.x, size.x);
			result.y = Math.Min(result.y, size.y);
			return result;
		}

		public float GetCurrentScale()
		{
			if (!this.m_Enabled || !this.m_CurrentCameraRequest)
			{
				return 1f;
			}
			return this.m_CurrentFraction;
		}

		public Vector2Int GetLastScaledSize()
		{
			return this.m_LastScaledSize;
		}

		public float GetLowResMultiplier(float targetLowRes)
		{
			return this.GetLowResMultiplier(targetLowRes, this.m_CachedSettings.lowResTransparencyMinimumThreshold);
		}

		public float GetLowResMultiplier(float targetLowRes, float minimumThreshold)
		{
			if (!this.m_Enabled)
			{
				return targetLowRes;
			}
			float num = Math.Min(minimumThreshold / 100f, targetLowRes);
			if (targetLowRes * this.m_CurrentFraction >= num)
			{
				return targetLowRes;
			}
			return Mathf.Clamp(num / this.m_CurrentFraction, 0f, 1f);
		}

		private bool m_Enabled;

		private bool m_UseMipBias;

		private float m_MinScreenFraction;

		private float m_MaxScreenFraction;

		private float m_CurrentFraction;

		private bool m_ForcingRes;

		private bool m_CurrentCameraRequest;

		private float m_PrevFraction;

		private bool m_ForceSoftwareFallback;

		private bool m_RunUpscalerFilterOnFullResolution;

		private float m_PrevHWScaleWidth;

		private float m_PrevHWScaleHeight;

		private Vector2Int m_LastScaledSize;

		private static DynamicResScalerSlot s_ActiveScalerSlot = DynamicResScalerSlot.User;

		private static DynamicResolutionHandler.ScalerContainer[] s_ScalerContainers = new DynamicResolutionHandler.ScalerContainer[]
		{
			new DynamicResolutionHandler.ScalerContainer
			{
				type = DynamicResScalePolicyType.ReturnsMinMaxLerpFactor,
				method = new PerformDynamicRes(DynamicResolutionHandler.DefaultDynamicResMethod)
			},
			new DynamicResolutionHandler.ScalerContainer
			{
				type = DynamicResScalePolicyType.ReturnsMinMaxLerpFactor,
				method = new PerformDynamicRes(DynamicResolutionHandler.DefaultDynamicResMethod)
			}
		};

		private Vector2Int cachedOriginalSize;

		private static Dictionary<int, DynamicResUpscaleFilter> s_CameraUpscaleFilters = new Dictionary<int, DynamicResUpscaleFilter>();

		private DynamicResolutionType type;

		private GlobalDynamicResolutionSettings m_CachedSettings = GlobalDynamicResolutionSettings.NewDefault();

		private const int CameraDictionaryMaxcCapacity = 32;

		private WeakReference m_OwnerCameraWeakRef;

		private static Dictionary<int, DynamicResolutionHandler> s_CameraInstances = new Dictionary<int, DynamicResolutionHandler>(32);

		private static DynamicResolutionHandler s_DefaultInstance = new DynamicResolutionHandler();

		private static int s_ActiveCameraId = 0;

		private static DynamicResolutionHandler s_ActiveInstance = DynamicResolutionHandler.s_DefaultInstance;

		private static bool s_ActiveInstanceDirty = true;

		private static float s_GlobalHwFraction = 1f;

		private static bool s_GlobalHwUpresActive = false;

		private DynamicResolutionHandler.UpsamplerScheduleType m_UpsamplerSchedule = DynamicResolutionHandler.UpsamplerScheduleType.AfterPost;

		private struct ScalerContainer
		{
			public DynamicResScalePolicyType type;

			public PerformDynamicRes method;
		}

		public enum UpsamplerScheduleType
		{
			BeforePost,
			AfterDepthOfField,
			AfterPost
		}
	}
}
