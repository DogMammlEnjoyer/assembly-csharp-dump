using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	public struct CameraData
	{
		internal CameraData(ContextContainer frameData)
		{
			this.frameData = frameData;
		}

		internal UniversalCameraData universalCameraData
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>();
			}
		}

		internal void SetViewAndProjectionMatrix(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
		{
			this.frameData.Get<UniversalCameraData>().SetViewAndProjectionMatrix(viewMatrix, projectionMatrix);
		}

		internal void SetViewProjectionAndJitterMatrix(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, Matrix4x4 jitterMatrix)
		{
			this.frameData.Get<UniversalCameraData>().SetViewProjectionAndJitterMatrix(viewMatrix, projectionMatrix, jitterMatrix);
		}

		internal void PushBuiltinShaderConstantsXR(RasterCommandBuffer cmd, bool renderIntoTexture)
		{
			this.frameData.Get<UniversalCameraData>().PushBuiltinShaderConstantsXR(cmd, renderIntoTexture);
		}

		public Matrix4x4 GetViewMatrix(int viewIndex = 0)
		{
			return this.frameData.Get<UniversalCameraData>().GetViewMatrix(viewIndex);
		}

		public Matrix4x4 GetProjectionMatrix(int viewIndex = 0)
		{
			return this.frameData.Get<UniversalCameraData>().GetProjectionMatrix(viewIndex);
		}

		internal Matrix4x4 GetProjectionMatrixNoJitter(int viewIndex = 0)
		{
			return this.frameData.Get<UniversalCameraData>().GetProjectionMatrixNoJitter(viewIndex);
		}

		public Matrix4x4 GetGPUProjectionMatrix(int viewIndex = 0)
		{
			return this.frameData.Get<UniversalCameraData>().GetGPUProjectionMatrix(viewIndex);
		}

		public Matrix4x4 GetGPUProjectionMatrixNoJitter(int viewIndex = 0)
		{
			return this.frameData.Get<UniversalCameraData>().GetGPUProjectionMatrixNoJitter(viewIndex);
		}

		internal Matrix4x4 GetGPUProjectionMatrix(bool renderIntoTexture, int viewIndex = 0)
		{
			return this.frameData.Get<UniversalCameraData>().GetGPUProjectionMatrix(renderIntoTexture, viewIndex);
		}

		public ref Camera camera
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().camera;
			}
		}

		public ref UniversalCameraHistory historyManager
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().m_HistoryManager;
			}
		}

		public ref CameraRenderType renderType
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().renderType;
			}
		}

		public ref RenderTexture targetTexture
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().targetTexture;
			}
		}

		public ref RenderTextureDescriptor cameraTargetDescriptor
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().cameraTargetDescriptor;
			}
		}

		internal ref Rect pixelRect
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().pixelRect;
			}
		}

		internal ref bool useScreenCoordOverride
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().useScreenCoordOverride;
			}
		}

		internal ref Vector4 screenSizeOverride
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().screenSizeOverride;
			}
		}

		internal ref Vector4 screenCoordScaleBias
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().screenCoordScaleBias;
			}
		}

		internal ref int pixelWidth
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().pixelWidth;
			}
		}

		internal ref int pixelHeight
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().pixelHeight;
			}
		}

		internal ref float aspectRatio
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().aspectRatio;
			}
		}

		public ref float renderScale
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().renderScale;
			}
		}

		internal ref ImageScalingMode imageScalingMode
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().imageScalingMode;
			}
		}

		internal ref ImageUpscalingFilter upscalingFilter
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().upscalingFilter;
			}
		}

		internal ref bool fsrOverrideSharpness
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().fsrOverrideSharpness;
			}
		}

		internal ref float fsrSharpness
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().fsrSharpness;
			}
		}

		internal ref HDRColorBufferPrecision hdrColorBufferPrecision
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().hdrColorBufferPrecision;
			}
		}

		public ref bool clearDepth
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().clearDepth;
			}
		}

		public ref CameraType cameraType
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().cameraType;
			}
		}

		public ref bool isDefaultViewport
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().isDefaultViewport;
			}
		}

		public ref bool isHdrEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().isHdrEnabled;
			}
		}

		public ref bool allowHDROutput
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().allowHDROutput;
			}
		}

		public ref bool isAlphaOutputEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().isAlphaOutputEnabled;
			}
		}

		public ref bool requiresDepthTexture
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().requiresDepthTexture;
			}
		}

		public ref bool requiresOpaqueTexture
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().requiresOpaqueTexture;
			}
		}

		public ref bool postProcessingRequiresDepthTexture
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().postProcessingRequiresDepthTexture;
			}
		}

		public ref bool xrRendering
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().xrRendering;
			}
		}

		internal bool requireSrgbConversion
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().requireSrgbConversion;
			}
		}

		public bool isSceneViewCamera
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().isSceneViewCamera;
			}
		}

		public bool isPreviewCamera
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().isPreviewCamera;
			}
		}

		internal bool isRenderPassSupportedCamera
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().isRenderPassSupportedCamera;
			}
		}

		internal bool resolveToScreen
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().resolveToScreen;
			}
		}

		public bool isHDROutputActive
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().isHDROutputActive;
			}
		}

		public HDROutputUtils.HDRDisplayInformation hdrDisplayInformation
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().hdrDisplayInformation;
			}
		}

		public ColorGamut hdrDisplayColorGamut
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().hdrDisplayColorGamut;
			}
		}

		public bool rendersOverlayUI
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().rendersOverlayUI;
			}
		}

		public bool IsHandleYFlipped(RTHandle handle)
		{
			return this.frameData.Get<UniversalCameraData>().IsHandleYFlipped(handle);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public bool IsCameraProjectionMatrixFlipped()
		{
			return this.frameData.Get<UniversalCameraData>().IsCameraProjectionMatrixFlipped();
		}

		public bool IsRenderTargetProjectionMatrixFlipped(RTHandle color, RTHandle depth = null)
		{
			return this.frameData.Get<UniversalCameraData>().IsRenderTargetProjectionMatrixFlipped(color, depth);
		}

		internal bool IsTemporalAAEnabled()
		{
			return this.frameData.Get<UniversalCameraData>().IsTemporalAAEnabled();
		}

		public ref SortingCriteria defaultOpaqueSortFlags
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().defaultOpaqueSortFlags;
			}
		}

		public XRPass xr
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().xr;
			}
			internal set
			{
				this.frameData.Get<UniversalCameraData>().xr = value;
			}
		}

		internal XRPassUniversal xrUniversal
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().xrUniversal;
			}
		}

		public ref float maxShadowDistance
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().maxShadowDistance;
			}
		}

		public ref bool postProcessEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().postProcessEnabled;
			}
		}

		public ref IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().captureActions;
			}
		}

		public ref LayerMask volumeLayerMask
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().volumeLayerMask;
			}
		}

		public ref Transform volumeTrigger
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().volumeTrigger;
			}
		}

		public ref bool isStopNaNEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().isStopNaNEnabled;
			}
		}

		public ref bool isDitheringEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().isDitheringEnabled;
			}
		}

		public ref AntialiasingMode antialiasing
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().antialiasing;
			}
		}

		public ref AntialiasingQuality antialiasingQuality
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().antialiasingQuality;
			}
		}

		public ref ScriptableRenderer renderer
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().renderer;
			}
		}

		public ref bool resolveFinalTarget
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().resolveFinalTarget;
			}
		}

		public ref Vector3 worldSpaceCameraPos
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().worldSpaceCameraPos;
			}
		}

		public ref Color backgroundColor
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().backgroundColor;
			}
		}

		internal ref TaaHistory taaHistory
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().taaHistory;
			}
		}

		internal ref TemporalAA.Settings taaSettings
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().taaSettings;
			}
		}

		internal bool resetHistory
		{
			get
			{
				return this.frameData.Get<UniversalCameraData>().resetHistory;
			}
		}

		public ref Camera baseCamera
		{
			get
			{
				return ref this.frameData.Get<UniversalCameraData>().baseCamera;
			}
		}

		private ContextContainer frameData;
	}
}
