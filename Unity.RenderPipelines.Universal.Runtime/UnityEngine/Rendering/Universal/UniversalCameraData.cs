using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalCameraData : ContextItem
	{
		internal void SetViewAndProjectionMatrix(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
		{
			this.m_ViewMatrix = viewMatrix;
			this.m_ProjectionMatrix = projectionMatrix;
			this.m_JitterMatrix = Matrix4x4.identity;
		}

		internal void SetViewProjectionAndJitterMatrix(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, Matrix4x4 jitterMatrix)
		{
			this.m_ViewMatrix = viewMatrix;
			this.m_ProjectionMatrix = projectionMatrix;
			this.m_JitterMatrix = jitterMatrix;
		}

		internal void PushBuiltinShaderConstantsXR(RasterCommandBuffer cmd, bool renderIntoTexture)
		{
			if ((!this.m_InitBuiltinXRConstants || this.m_CachedRenderIntoTextureXR != renderIntoTexture || !this.xr.singlePassEnabled) && this.xr.enabled)
			{
				Matrix4x4 projectionMatrix = this.GetProjectionMatrix(0);
				Matrix4x4 viewMatrix = this.GetViewMatrix(0);
				cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
				if (this.xr.singlePassEnabled)
				{
					Matrix4x4 projectionMatrix2 = this.GetProjectionMatrix(1);
					Matrix4x4 viewMatrix2 = this.GetViewMatrix(1);
					XRBuiltinShaderConstants.UpdateBuiltinShaderConstants(viewMatrix, projectionMatrix, renderIntoTexture, 0);
					XRBuiltinShaderConstants.UpdateBuiltinShaderConstants(viewMatrix2, projectionMatrix2, renderIntoTexture, 1);
					XRBuiltinShaderConstants.SetBuiltinShaderConstants(cmd);
				}
				else
				{
					Vector3 v = Matrix4x4.Inverse(this.GetViewMatrix(0)).GetColumn(3);
					cmd.SetGlobalVector(ShaderPropertyId.worldSpaceCameraPos, v);
					Matrix4x4 gpuprojectionMatrix = this.GetGPUProjectionMatrix(renderIntoTexture, 0);
					Matrix4x4 matrix4x = Matrix4x4.Inverse(viewMatrix);
					Matrix4x4 matrix4x2 = Matrix4x4.Inverse(gpuprojectionMatrix);
					Matrix4x4 value = matrix4x * matrix4x2;
					Matrix4x4 value2 = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * viewMatrix;
					Matrix4x4 inverse = value2.inverse;
					cmd.SetGlobalMatrix(ShaderPropertyId.worldToCameraMatrix, value2);
					cmd.SetGlobalMatrix(ShaderPropertyId.cameraToWorldMatrix, inverse);
					cmd.SetGlobalMatrix(ShaderPropertyId.inverseViewMatrix, matrix4x);
					cmd.SetGlobalMatrix(ShaderPropertyId.inverseProjectionMatrix, matrix4x2);
					cmd.SetGlobalMatrix(ShaderPropertyId.inverseViewAndProjectionMatrix, value);
				}
				this.m_CachedRenderIntoTextureXR = renderIntoTexture;
				this.m_InitBuiltinXRConstants = true;
			}
		}

		public Matrix4x4 GetViewMatrix(int viewIndex = 0)
		{
			if (this.xr.enabled)
			{
				return this.xr.GetViewMatrix(viewIndex);
			}
			return this.m_ViewMatrix;
		}

		public Matrix4x4 GetProjectionMatrix(int viewIndex = 0)
		{
			if (this.xr.enabled)
			{
				return this.m_JitterMatrix * this.xr.GetProjMatrix(viewIndex);
			}
			return this.m_JitterMatrix * this.m_ProjectionMatrix;
		}

		internal Matrix4x4 GetProjectionMatrixNoJitter(int viewIndex = 0)
		{
			if (this.xr.enabled)
			{
				return this.xr.GetProjMatrix(viewIndex);
			}
			return this.m_ProjectionMatrix;
		}

		public Matrix4x4 GetGPUProjectionMatrix(int viewIndex = 0)
		{
			return this.m_JitterMatrix * GL.GetGPUProjectionMatrix(this.GetProjectionMatrixNoJitter(viewIndex), this.IsCameraProjectionMatrixFlipped());
		}

		public Matrix4x4 GetGPUProjectionMatrixNoJitter(int viewIndex = 0)
		{
			return GL.GetGPUProjectionMatrix(this.GetProjectionMatrixNoJitter(viewIndex), this.IsCameraProjectionMatrixFlipped());
		}

		internal Matrix4x4 GetGPUProjectionMatrix(bool renderIntoTexture, int viewIndex = 0)
		{
			return GL.GetGPUProjectionMatrix(this.GetProjectionMatrix(viewIndex), renderIntoTexture);
		}

		public int scaledWidth
		{
			get
			{
				return Mathf.Max(1, (int)((float)this.camera.pixelWidth * this.renderScale));
			}
		}

		public int scaledHeight
		{
			get
			{
				return Mathf.Max(1, (int)((float)this.camera.pixelHeight * this.renderScale));
			}
		}

		public UniversalCameraHistory historyManager
		{
			get
			{
				return this.m_HistoryManager;
			}
			set
			{
				this.m_HistoryManager = value;
			}
		}

		internal bool requireSrgbConversion
		{
			get
			{
				if (this.xr.enabled)
				{
					return !this.xr.renderTargetDesc.sRGB && (this.xr.renderTargetDesc.graphicsFormat == GraphicsFormat.R8G8B8A8_UNorm || this.xr.renderTargetDesc.graphicsFormat == GraphicsFormat.B8G8R8A8_UNorm) && QualitySettings.activeColorSpace == ColorSpace.Linear;
				}
				return this.targetTexture == null && Display.main.requiresSrgbBlitToBackbuffer;
			}
		}

		public bool isGameCamera
		{
			get
			{
				return this.cameraType == CameraType.Game;
			}
		}

		public bool isSceneViewCamera
		{
			get
			{
				return this.cameraType == CameraType.SceneView;
			}
		}

		public bool isPreviewCamera
		{
			get
			{
				return this.cameraType == CameraType.Preview;
			}
		}

		internal bool isRenderPassSupportedCamera
		{
			get
			{
				return this.cameraType == CameraType.Game || this.cameraType == CameraType.Reflection;
			}
		}

		internal bool resolveToScreen
		{
			get
			{
				return this.targetTexture == null && this.resolveFinalTarget && (this.cameraType == CameraType.Game || this.camera.cameraType == CameraType.VR);
			}
		}

		public bool isHDROutputActive
		{
			get
			{
				bool flag = UniversalRenderPipeline.HDROutputForMainDisplayIsActive();
				if (this.xr.enabled)
				{
					flag = this.xr.isHDRDisplayOutputActive;
				}
				return flag && this.allowHDROutput && this.resolveToScreen;
			}
		}

		public HDROutputUtils.HDRDisplayInformation hdrDisplayInformation
		{
			get
			{
				HDROutputUtils.HDRDisplayInformation hdrDisplayOutputInformation;
				if (this.xr.enabled)
				{
					hdrDisplayOutputInformation = this.xr.hdrDisplayOutputInformation;
				}
				else
				{
					HDROutputSettings main = HDROutputSettings.main;
					hdrDisplayOutputInformation = new HDROutputUtils.HDRDisplayInformation(main.maxFullFrameToneMapLuminance, main.maxToneMapLuminance, main.minToneMapLuminance, main.paperWhiteNits);
				}
				return hdrDisplayOutputInformation;
			}
		}

		public ColorGamut hdrDisplayColorGamut
		{
			get
			{
				if (this.xr.enabled)
				{
					return this.xr.hdrDisplayOutputColorGamut;
				}
				return HDROutputSettings.main.displayColorGamut;
			}
		}

		public bool rendersOverlayUI
		{
			get
			{
				return SupportedRenderingFeatures.active.rendersUIOverlay && this.resolveToScreen;
			}
		}

		public bool IsHandleYFlipped(RTHandle handle)
		{
			if (!SystemInfo.graphicsUVStartsAtTop)
			{
				return true;
			}
			if (this.cameraType == CameraType.SceneView || this.cameraType == CameraType.Preview)
			{
				return true;
			}
			RenderTargetIdentifier lhs = new RenderTargetIdentifier(handle.nameID, 0, CubemapFace.Unknown, 0);
			bool flag = lhs == BuiltinRenderTextureType.CameraTarget || lhs == BuiltinRenderTextureType.Depth;
			if (this.xr.enabled)
			{
				flag |= (lhs == new RenderTargetIdentifier(this.xr.renderTarget, 0, CubemapFace.Unknown, 0));
			}
			return !flag;
		}

		public bool IsCameraProjectionMatrixFlipped()
		{
			if (!SystemInfo.graphicsUVStartsAtTop)
			{
				return false;
			}
			ScriptableRenderer current = ScriptableRenderer.current;
			return current == null || this.IsHandleYFlipped(current.cameraColorTargetHandle) || this.targetTexture != null;
		}

		public bool IsRenderTargetProjectionMatrixFlipped(RTHandle color, RTHandle depth = null)
		{
			return !SystemInfo.graphicsUVStartsAtTop || this.targetTexture != null || this.IsHandleYFlipped(color ?? depth);
		}

		internal bool IsTemporalAARequested()
		{
			return this.antialiasing == AntialiasingMode.TemporalAntiAliasing;
		}

		internal bool IsTemporalAAEnabled()
		{
			UniversalAdditionalCameraData universalAdditionalCameraData;
			this.camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
			return this.IsTemporalAARequested() && this.postProcessEnabled && this.taaHistory != null && this.cameraTargetDescriptor.msaaSamples == 1 && (universalAdditionalCameraData == null || universalAdditionalCameraData.renderType != CameraRenderType.Overlay) && (universalAdditionalCameraData == null || universalAdditionalCameraData.cameraStack.Count <= 0) && !this.camera.allowDynamicResolution && this.renderer.SupportsMotionVectors();
		}

		internal bool IsSTPRequested()
		{
			return this.imageScalingMode == ImageScalingMode.Upscaling && this.upscalingFilter == ImageUpscalingFilter.STP;
		}

		internal bool IsSTPEnabled()
		{
			return this.IsSTPRequested() && this.IsTemporalAAEnabled();
		}

		public XRPass xr { get; internal set; }

		internal XRPassUniversal xrUniversal
		{
			get
			{
				return this.xr as XRPassUniversal;
			}
		}

		internal bool resetHistory
		{
			get
			{
				return this.taaSettings.resetHistoryFrames != 0;
			}
		}

		public override void Reset()
		{
			this.m_ViewMatrix = default(Matrix4x4);
			this.m_ProjectionMatrix = default(Matrix4x4);
			this.m_JitterMatrix = default(Matrix4x4);
			this.m_CachedRenderIntoTextureXR = false;
			this.m_InitBuiltinXRConstants = false;
			this.camera = null;
			this.renderType = CameraRenderType.Base;
			this.targetTexture = null;
			this.cameraTargetDescriptor = default(RenderTextureDescriptor);
			this.pixelRect = default(Rect);
			this.useScreenCoordOverride = false;
			this.screenSizeOverride = default(Vector4);
			this.screenCoordScaleBias = default(Vector4);
			this.pixelWidth = 0;
			this.pixelHeight = 0;
			this.aspectRatio = 0f;
			this.renderScale = 1f;
			this.imageScalingMode = ImageScalingMode.None;
			this.upscalingFilter = ImageUpscalingFilter.Point;
			this.fsrOverrideSharpness = false;
			this.fsrSharpness = 0f;
			this.hdrColorBufferPrecision = HDRColorBufferPrecision._32Bits;
			this.clearDepth = false;
			this.cameraType = CameraType.Game;
			this.isDefaultViewport = false;
			this.isHdrEnabled = false;
			this.allowHDROutput = false;
			this.isAlphaOutputEnabled = false;
			this.requiresDepthTexture = false;
			this.requiresOpaqueTexture = false;
			this.postProcessingRequiresDepthTexture = false;
			this.xrRendering = false;
			this.useGPUOcclusionCulling = false;
			this.defaultOpaqueSortFlags = SortingCriteria.None;
			this.xr = null;
			this.maxShadowDistance = 0f;
			this.postProcessEnabled = false;
			this.captureActions = null;
			this.volumeLayerMask = 0;
			this.volumeTrigger = null;
			this.isStopNaNEnabled = false;
			this.isDitheringEnabled = false;
			this.antialiasing = AntialiasingMode.None;
			this.antialiasingQuality = AntialiasingQuality.Low;
			this.renderer = null;
			this.resolveFinalTarget = false;
			this.worldSpaceCameraPos = default(Vector3);
			this.backgroundColor = Color.black;
			this.taaHistory = null;
			this.stpHistory = null;
			this.taaSettings = default(TemporalAA.Settings);
			this.baseCamera = null;
			this.isLastBaseCamera = false;
			this.stackAnyPostProcessingEnabled = false;
			this.stackLastCameraOutputToHDR = false;
		}

		private Matrix4x4 m_ViewMatrix;

		private Matrix4x4 m_ProjectionMatrix;

		private Matrix4x4 m_JitterMatrix;

		private bool m_CachedRenderIntoTextureXR;

		private bool m_InitBuiltinXRConstants;

		public Camera camera;

		internal UniversalCameraHistory m_HistoryManager;

		public CameraRenderType renderType;

		public RenderTexture targetTexture;

		public RenderTextureDescriptor cameraTargetDescriptor;

		internal Rect pixelRect;

		internal bool useScreenCoordOverride;

		internal Vector4 screenSizeOverride;

		internal Vector4 screenCoordScaleBias;

		internal int pixelWidth;

		internal int pixelHeight;

		internal float aspectRatio;

		public float renderScale;

		internal ImageScalingMode imageScalingMode;

		internal ImageUpscalingFilter upscalingFilter;

		internal bool fsrOverrideSharpness;

		internal float fsrSharpness;

		internal HDRColorBufferPrecision hdrColorBufferPrecision;

		public bool clearDepth;

		public CameraType cameraType;

		public bool isDefaultViewport;

		public bool isHdrEnabled;

		public bool allowHDROutput;

		public bool isAlphaOutputEnabled;

		public bool requiresDepthTexture;

		public bool requiresOpaqueTexture;

		public bool postProcessingRequiresDepthTexture;

		public bool xrRendering;

		internal bool useGPUOcclusionCulling;

		internal bool stackLastCameraOutputToHDR;

		public SortingCriteria defaultOpaqueSortFlags;

		public float maxShadowDistance;

		public bool postProcessEnabled;

		internal bool stackAnyPostProcessingEnabled;

		public IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions;

		public LayerMask volumeLayerMask;

		public Transform volumeTrigger;

		public bool isStopNaNEnabled;

		public bool isDitheringEnabled;

		public AntialiasingMode antialiasing;

		public AntialiasingQuality antialiasingQuality;

		public ScriptableRenderer renderer;

		public bool resolveFinalTarget;

		public Vector3 worldSpaceCameraPos;

		public Color backgroundColor;

		internal TaaHistory taaHistory;

		internal StpHistory stpHistory;

		internal TemporalAA.Settings taaSettings;

		public Camera baseCamera;

		internal bool isLastBaseCamera;
	}
}
