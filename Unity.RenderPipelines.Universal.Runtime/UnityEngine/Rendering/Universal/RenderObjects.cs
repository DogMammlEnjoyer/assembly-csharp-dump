using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
	[ExcludeFromPreset]
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.Universal", null, null)]
	[Tooltip("Render Objects simplifies the injection of additional render passes by exposing a selection of commonly used settings.")]
	public class RenderObjects : ScriptableRendererFeature
	{
		public override void Create()
		{
			RenderObjects.FilterSettings filterSettings = this.settings.filterSettings;
			if (this.settings.Event < RenderPassEvent.BeforeRenderingPrePasses)
			{
				this.settings.Event = RenderPassEvent.BeforeRenderingPrePasses;
			}
			this.renderObjectsPass = new RenderObjectsPass(this.settings.passTag, this.settings.Event, filterSettings.PassNames, filterSettings.RenderQueueType, filterSettings.LayerMask, this.settings.cameraSettings);
			switch (this.settings.overrideMode)
			{
			case RenderObjects.RenderObjectsSettings.OverrideMaterialMode.None:
				this.renderObjectsPass.overrideMaterial = null;
				this.renderObjectsPass.overrideShader = null;
				break;
			case RenderObjects.RenderObjectsSettings.OverrideMaterialMode.Material:
				this.renderObjectsPass.overrideMaterial = this.settings.overrideMaterial;
				this.renderObjectsPass.overrideMaterialPassIndex = this.settings.overrideMaterialPassIndex;
				this.renderObjectsPass.overrideShader = null;
				break;
			case RenderObjects.RenderObjectsSettings.OverrideMaterialMode.Shader:
				this.renderObjectsPass.overrideMaterial = null;
				this.renderObjectsPass.overrideShader = this.settings.overrideShader;
				this.renderObjectsPass.overrideShaderPassIndex = this.settings.overrideShaderPassIndex;
				break;
			}
			if (this.settings.overrideDepthState)
			{
				this.renderObjectsPass.SetDepthState(this.settings.enableWrite, this.settings.depthCompareFunction);
			}
			if (this.settings.stencilSettings.overrideStencilState)
			{
				this.renderObjectsPass.SetStencilState(this.settings.stencilSettings.stencilReference, this.settings.stencilSettings.stencilCompareFunction, this.settings.stencilSettings.passOperation, this.settings.stencilSettings.failOperation, this.settings.stencilSettings.zFailOperation);
			}
		}

		public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (*renderingData.cameraData.cameraType == CameraType.Preview || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
			{
				return;
			}
			renderer.EnqueuePass(this.renderObjectsPass);
		}

		internal override bool SupportsNativeRenderPass()
		{
			return true;
		}

		public RenderObjects.RenderObjectsSettings settings = new RenderObjects.RenderObjectsSettings();

		private RenderObjectsPass renderObjectsPass;

		[Serializable]
		public class RenderObjectsSettings
		{
			public string passTag = "RenderObjectsFeature";

			public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

			public RenderObjects.FilterSettings filterSettings = new RenderObjects.FilterSettings();

			public Material overrideMaterial;

			public int overrideMaterialPassIndex;

			public Shader overrideShader;

			public int overrideShaderPassIndex;

			public RenderObjects.RenderObjectsSettings.OverrideMaterialMode overrideMode = RenderObjects.RenderObjectsSettings.OverrideMaterialMode.Material;

			public bool overrideDepthState;

			public CompareFunction depthCompareFunction = CompareFunction.LessEqual;

			public bool enableWrite = true;

			public StencilStateData stencilSettings = new StencilStateData();

			public RenderObjects.CustomCameraSettings cameraSettings = new RenderObjects.CustomCameraSettings();

			public enum OverrideMaterialMode
			{
				None,
				Material,
				Shader
			}
		}

		[Serializable]
		public class FilterSettings
		{
			public FilterSettings()
			{
				this.RenderQueueType = RenderQueueType.Opaque;
				this.LayerMask = 0;
			}

			public RenderQueueType RenderQueueType;

			public LayerMask LayerMask;

			public string[] PassNames;
		}

		[Serializable]
		public class CustomCameraSettings
		{
			public bool overrideCamera;

			public bool restoreCamera = true;

			public Vector4 offset;

			public float cameraFieldOfView = 60f;
		}
	}
}
