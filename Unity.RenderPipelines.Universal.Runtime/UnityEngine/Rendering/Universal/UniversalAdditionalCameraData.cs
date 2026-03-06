using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.Universal
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[ExecuteAlways]
	public class UniversalAdditionalCameraData : MonoBehaviour, ISerializationCallbackReceiver, IAdditionalData
	{
		internal static UniversalAdditionalCameraData defaultAdditionalCameraData
		{
			get
			{
				if (UniversalAdditionalCameraData.s_DefaultAdditionalCameraData == null)
				{
					UniversalAdditionalCameraData.s_DefaultAdditionalCameraData = new UniversalAdditionalCameraData();
				}
				return UniversalAdditionalCameraData.s_DefaultAdditionalCameraData;
			}
		}

		internal Camera camera
		{
			get
			{
				if (!this.m_Camera)
				{
					base.gameObject.TryGetComponent<Camera>(out this.m_Camera);
				}
				return this.m_Camera;
			}
		}

		private void Start()
		{
			if (this.m_CameraType == CameraRenderType.Overlay)
			{
				this.camera.clearFlags = CameraClearFlags.Nothing;
			}
		}

		public bool renderShadows
		{
			get
			{
				return this.m_RenderShadows;
			}
			set
			{
				this.m_RenderShadows = value;
			}
		}

		public CameraOverrideOption requiresDepthOption
		{
			get
			{
				return this.m_RequiresDepthTextureOption;
			}
			set
			{
				this.m_RequiresDepthTextureOption = value;
			}
		}

		public CameraOverrideOption requiresColorOption
		{
			get
			{
				return this.m_RequiresOpaqueTextureOption;
			}
			set
			{
				this.m_RequiresOpaqueTextureOption = value;
			}
		}

		public CameraRenderType renderType
		{
			get
			{
				return this.m_CameraType;
			}
			set
			{
				this.m_CameraType = value;
			}
		}

		public List<Camera> cameraStack
		{
			get
			{
				if (this.renderType != CameraRenderType.Base)
				{
					Camera component = base.gameObject.GetComponent<Camera>();
					Debug.LogWarning(string.Format("{0}: This camera is of {1} type. Only Base cameras can have a camera stack.", component.name, this.renderType));
					return null;
				}
				if (!this.scriptableRenderer.SupportsCameraStackingType(CameraRenderType.Base))
				{
					Camera component2 = base.gameObject.GetComponent<Camera>();
					Debug.LogWarning(string.Format("{0}: This camera has a ScriptableRenderer that doesn't support camera stacking. Camera stack is null.", component2.name));
					return null;
				}
				return this.m_Cameras;
			}
		}

		internal void UpdateCameraStack()
		{
			int count = this.m_Cameras.Count;
			this.m_Cameras.RemoveAll((Camera cam) => cam == null);
			int count2 = this.m_Cameras.Count;
			int num = count - count2;
			if (num != 0)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					base.name,
					": ",
					num.ToString(),
					" camera overlay",
					(num > 1) ? "s" : "",
					" no longer exists and will be removed from the camera stack."
				}));
			}
		}

		public bool clearDepth
		{
			get
			{
				return this.m_ClearDepth;
			}
		}

		public bool requiresDepthTexture
		{
			get
			{
				if (this.m_RequiresDepthTextureOption == CameraOverrideOption.UsePipelineSettings)
				{
					return UniversalRenderPipeline.asset.supportsCameraDepthTexture;
				}
				return this.m_RequiresDepthTextureOption == CameraOverrideOption.On;
			}
			set
			{
				this.m_RequiresDepthTextureOption = (value ? CameraOverrideOption.On : CameraOverrideOption.Off);
			}
		}

		public bool requiresColorTexture
		{
			get
			{
				if (this.m_RequiresOpaqueTextureOption == CameraOverrideOption.UsePipelineSettings)
				{
					return UniversalRenderPipeline.asset.supportsCameraOpaqueTexture;
				}
				return this.m_RequiresOpaqueTextureOption == CameraOverrideOption.On;
			}
			set
			{
				this.m_RequiresOpaqueTextureOption = (value ? CameraOverrideOption.On : CameraOverrideOption.Off);
			}
		}

		public ScriptableRenderer scriptableRenderer
		{
			get
			{
				if (UniversalRenderPipeline.asset == null)
				{
					return null;
				}
				if (!UniversalRenderPipeline.asset.ValidateRendererData(this.m_RendererIndex))
				{
					int defaultRendererIndex = UniversalRenderPipeline.asset.m_DefaultRendererIndex;
					Debug.LogWarning(string.Concat(new string[]
					{
						"Renderer at <b>index ",
						this.m_RendererIndex.ToString(),
						"</b> is missing for camera <b>",
						this.camera.name,
						"</b>, falling back to Default Renderer. <b>",
						UniversalRenderPipeline.asset.m_RendererDataList[defaultRendererIndex].name,
						"</b>"
					}), UniversalRenderPipeline.asset);
					return UniversalRenderPipeline.asset.GetRenderer(defaultRendererIndex);
				}
				return UniversalRenderPipeline.asset.GetRenderer(this.m_RendererIndex);
			}
		}

		public void SetRenderer(int index)
		{
			this.m_RendererIndex = index;
		}

		public LayerMask volumeLayerMask
		{
			get
			{
				return this.m_VolumeLayerMask;
			}
			set
			{
				this.m_VolumeLayerMask = value;
			}
		}

		public Transform volumeTrigger
		{
			get
			{
				return this.m_VolumeTrigger;
			}
			set
			{
				this.m_VolumeTrigger = value;
			}
		}

		internal VolumeFrameworkUpdateMode volumeFrameworkUpdateMode
		{
			get
			{
				return this.m_VolumeFrameworkUpdateModeOption;
			}
			set
			{
				this.m_VolumeFrameworkUpdateModeOption = value;
			}
		}

		public bool requiresVolumeFrameworkUpdate
		{
			get
			{
				if (this.m_VolumeFrameworkUpdateModeOption == VolumeFrameworkUpdateMode.UsePipelineSettings)
				{
					return UniversalRenderPipeline.asset.volumeFrameworkUpdateMode != VolumeFrameworkUpdateMode.ViaScripting;
				}
				return this.m_VolumeFrameworkUpdateModeOption == VolumeFrameworkUpdateMode.EveryFrame;
			}
		}

		public VolumeStack volumeStack
		{
			get
			{
				return this.m_VolumeStack;
			}
			set
			{
				if (value == null && this.m_VolumeStack != null && this.m_VolumeStack.isValid)
				{
					if (UniversalAdditionalCameraData.s_CachedVolumeStacks == null)
					{
						UniversalAdditionalCameraData.s_CachedVolumeStacks = new List<VolumeStack>(4);
					}
					UniversalAdditionalCameraData.s_CachedVolumeStacks.Add(this.m_VolumeStack);
				}
				this.m_VolumeStack = value;
			}
		}

		internal void GetOrCreateVolumeStack()
		{
			if (UniversalAdditionalCameraData.s_CachedVolumeStacks != null && UniversalAdditionalCameraData.s_CachedVolumeStacks.Count > 0)
			{
				int index = UniversalAdditionalCameraData.s_CachedVolumeStacks.Count - 1;
				VolumeStack volumeStack = UniversalAdditionalCameraData.s_CachedVolumeStacks[index];
				UniversalAdditionalCameraData.s_CachedVolumeStacks.RemoveAt(index);
				if (volumeStack.isValid)
				{
					this.volumeStack = volumeStack;
				}
			}
			if (this.volumeStack == null)
			{
				this.volumeStack = VolumeManager.instance.CreateStack();
			}
		}

		public bool renderPostProcessing
		{
			get
			{
				return this.m_RenderPostProcessing;
			}
			set
			{
				this.m_RenderPostProcessing = value;
			}
		}

		public AntialiasingMode antialiasing
		{
			get
			{
				return this.m_Antialiasing;
			}
			set
			{
				this.m_Antialiasing = value;
			}
		}

		public AntialiasingQuality antialiasingQuality
		{
			get
			{
				return this.m_AntialiasingQuality;
			}
			set
			{
				this.m_AntialiasingQuality = value;
			}
		}

		public ref TemporalAA.Settings taaSettings
		{
			get
			{
				return ref this.m_TaaSettings;
			}
		}

		public ICameraHistoryReadAccess history
		{
			get
			{
				return this.m_History;
			}
		}

		internal UniversalCameraHistory historyManager
		{
			get
			{
				return this.m_History;
			}
		}

		internal MotionVectorsPersistentData motionVectorsPersistentData
		{
			get
			{
				return this.m_MotionVectorsPersistentData;
			}
		}

		public bool resetHistory
		{
			get
			{
				return this.m_TaaSettings.resetHistoryFrames != 0;
			}
			set
			{
				this.m_TaaSettings.resetHistoryFrames = this.m_TaaSettings.resetHistoryFrames + (value ? 1 : 0);
				this.m_MotionVectorsPersistentData.Reset();
				this.m_TaaSettings.jitterFrameCountOffset = -Time.frameCount;
			}
		}

		public bool stopNaN
		{
			get
			{
				return this.m_StopNaN;
			}
			set
			{
				this.m_StopNaN = value;
			}
		}

		public bool dithering
		{
			get
			{
				return this.m_Dithering;
			}
			set
			{
				this.m_Dithering = value;
			}
		}

		public bool allowXRRendering
		{
			get
			{
				return this.m_AllowXRRendering;
			}
			set
			{
				this.m_AllowXRRendering = value;
			}
		}

		public bool useScreenCoordOverride
		{
			get
			{
				return this.m_UseScreenCoordOverride;
			}
			set
			{
				this.m_UseScreenCoordOverride = value;
			}
		}

		public Vector4 screenSizeOverride
		{
			get
			{
				return this.m_ScreenSizeOverride;
			}
			set
			{
				this.m_ScreenSizeOverride = value;
			}
		}

		public Vector4 screenCoordScaleBias
		{
			get
			{
				return this.m_ScreenCoordScaleBias;
			}
			set
			{
				this.m_ScreenCoordScaleBias = value;
			}
		}

		public bool allowHDROutput
		{
			get
			{
				return this.m_AllowHDROutput;
			}
			set
			{
				this.m_AllowHDROutput = value;
			}
		}

		public void OnValidate()
		{
			if (this.m_CameraType == CameraRenderType.Overlay && this.m_Camera != null)
			{
				this.m_Camera.clearFlags = CameraClearFlags.Nothing;
			}
		}

		public void OnDrawGizmos()
		{
			string text = "";
			Color white = Color.white;
			if (this.m_CameraType == CameraRenderType.Base)
			{
				text = "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/Camera_Base.png";
			}
			else if (this.m_CameraType == CameraRenderType.Overlay)
			{
				text = "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/Camera_Base.png";
			}
			if (!string.IsNullOrEmpty(text))
			{
				Gizmos.DrawIcon(base.transform.position, text, true, white);
			}
			if (this.renderPostProcessing)
			{
				Gizmos.DrawIcon(base.transform.position, "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/Camera_PostProcessing.png", true, white);
			}
		}

		public void OnDestroy()
		{
			this.m_Camera.DestroyVolumeStack(this);
			if (this.camera.cameraType != CameraType.SceneView)
			{
				ScriptableRenderer rawRenderer = this.GetRawRenderer();
				if (rawRenderer != null)
				{
					rawRenderer.ReleaseRenderTargets();
				}
			}
			UniversalCameraHistory history = this.m_History;
			if (history != null)
			{
				history.Dispose();
			}
			this.m_History = null;
		}

		private unsafe ScriptableRenderer GetRawRenderer()
		{
			if (UniversalRenderPipeline.asset == null)
			{
				return null;
			}
			ReadOnlySpan<ScriptableRenderer> renderers = UniversalRenderPipeline.asset.renderers;
			if (renderers == null || renderers.IsEmpty)
			{
				return null;
			}
			if (this.m_RendererIndex >= renderers.Length || this.m_RendererIndex < 0)
			{
				return null;
			}
			return *renderers[this.m_RendererIndex];
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this.m_Version == UniversalAdditionalCameraData.Version.Count)
			{
				this.m_Version = UniversalAdditionalCameraData.Version.DepthAndOpaqueTextureOptions;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_Version == UniversalAdditionalCameraData.Version.Count)
			{
				this.m_Version = UniversalAdditionalCameraData.Version.Initial;
			}
			if (this.m_Version < UniversalAdditionalCameraData.Version.DepthAndOpaqueTextureOptions)
			{
				this.m_RequiresDepthTextureOption = (this.m_RequiresDepthTexture ? CameraOverrideOption.On : CameraOverrideOption.Off);
				this.m_RequiresOpaqueTextureOption = (this.m_RequiresColorTexture ? CameraOverrideOption.On : CameraOverrideOption.Off);
				this.m_Version = UniversalAdditionalCameraData.Version.DepthAndOpaqueTextureOptions;
			}
		}

		[Obsolete("This field has been deprecated. #from(6000.2)", false)]
		public float version
		{
			get
			{
				return (float)this.m_Version;
			}
		}

		private const string k_GizmoPath = "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/";

		private const string k_BaseCameraGizmoPath = "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/Camera_Base.png";

		private const string k_OverlayCameraGizmoPath = "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/Camera_Base.png";

		private const string k_PostProcessingGizmoPath = "Packages/com.unity.render-pipelines.universal/Editor/Gizmos/Camera_PostProcessing.png";

		[FormerlySerializedAs("renderShadows")]
		[SerializeField]
		private bool m_RenderShadows = true;

		[SerializeField]
		private CameraOverrideOption m_RequiresDepthTextureOption = CameraOverrideOption.UsePipelineSettings;

		[SerializeField]
		private CameraOverrideOption m_RequiresOpaqueTextureOption = CameraOverrideOption.UsePipelineSettings;

		[SerializeField]
		private CameraRenderType m_CameraType;

		[SerializeField]
		private List<Camera> m_Cameras = new List<Camera>();

		[SerializeField]
		private int m_RendererIndex = -1;

		[SerializeField]
		private LayerMask m_VolumeLayerMask = 1;

		[SerializeField]
		private Transform m_VolumeTrigger;

		[SerializeField]
		private VolumeFrameworkUpdateMode m_VolumeFrameworkUpdateModeOption = VolumeFrameworkUpdateMode.UsePipelineSettings;

		[SerializeField]
		private bool m_RenderPostProcessing;

		[SerializeField]
		private AntialiasingMode m_Antialiasing;

		[SerializeField]
		private AntialiasingQuality m_AntialiasingQuality = AntialiasingQuality.High;

		[SerializeField]
		private bool m_StopNaN;

		[SerializeField]
		private bool m_Dithering;

		[SerializeField]
		private bool m_ClearDepth = true;

		[SerializeField]
		private bool m_AllowXRRendering = true;

		[SerializeField]
		private bool m_AllowHDROutput = true;

		[SerializeField]
		private bool m_UseScreenCoordOverride;

		[SerializeField]
		private Vector4 m_ScreenSizeOverride;

		[SerializeField]
		private Vector4 m_ScreenCoordScaleBias;

		[NonSerialized]
		private Camera m_Camera;

		[FormerlySerializedAs("requiresDepthTexture")]
		[SerializeField]
		private bool m_RequiresDepthTexture;

		[FormerlySerializedAs("requiresColorTexture")]
		[SerializeField]
		private bool m_RequiresColorTexture;

		[NonSerialized]
		private MotionVectorsPersistentData m_MotionVectorsPersistentData = new MotionVectorsPersistentData();

		[NonSerialized]
		internal UniversalCameraHistory m_History = new UniversalCameraHistory();

		[SerializeField]
		internal TemporalAA.Settings m_TaaSettings = TemporalAA.Settings.Create();

		private static UniversalAdditionalCameraData s_DefaultAdditionalCameraData;

		private static List<VolumeStack> s_CachedVolumeStacks;

		private VolumeStack m_VolumeStack;

		[SerializeField]
		private UniversalAdditionalCameraData.Version m_Version = UniversalAdditionalCameraData.Version.Count;

		private enum Version
		{
			Initial,
			DepthAndOpaqueTextureOptions = 2,
			Count
		}
	}
}
