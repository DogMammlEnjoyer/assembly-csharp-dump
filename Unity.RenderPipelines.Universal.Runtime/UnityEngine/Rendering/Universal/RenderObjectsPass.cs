using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.Universal", null, null)]
	public class RenderObjectsPass : ScriptableRenderPass
	{
		public Material overrideMaterial { get; set; }

		public int overrideMaterialPassIndex { get; set; }

		public Shader overrideShader { get; set; }

		public int overrideShaderPassIndex { get; set; }

		[Obsolete("Use SetDepthState instead", true)]
		public void SetDetphState(bool writeEnabled, CompareFunction function = CompareFunction.Less)
		{
			this.SetDepthState(writeEnabled, function);
		}

		public void SetDepthState(bool writeEnabled, CompareFunction function = CompareFunction.Less)
		{
			this.m_RenderStateBlock.mask = (this.m_RenderStateBlock.mask | RenderStateMask.Depth);
			this.m_RenderStateBlock.depthState = new DepthState(writeEnabled, function);
		}

		public void SetStencilState(int reference, CompareFunction compareFunction, StencilOp passOp, StencilOp failOp, StencilOp zFailOp)
		{
			StencilState defaultValue = StencilState.defaultValue;
			defaultValue.enabled = true;
			defaultValue.SetCompareFunction(compareFunction);
			defaultValue.SetPassOperation(passOp);
			defaultValue.SetFailOperation(failOp);
			defaultValue.SetZFailOperation(zFailOp);
			this.m_RenderStateBlock.mask = (this.m_RenderStateBlock.mask | RenderStateMask.Stencil);
			this.m_RenderStateBlock.stencilReference = reference;
			this.m_RenderStateBlock.stencilState = defaultValue;
		}

		public RenderObjectsPass(string profilerTag, RenderPassEvent renderPassEvent, string[] shaderTags, RenderQueueType renderQueueType, int layerMask, RenderObjects.CustomCameraSettings cameraSettings)
		{
			base.profilingSampler = new ProfilingSampler(profilerTag);
			this.Init(renderPassEvent, shaderTags, renderQueueType, layerMask, cameraSettings);
		}

		internal RenderObjectsPass(URPProfileId profileId, RenderPassEvent renderPassEvent, string[] shaderTags, RenderQueueType renderQueueType, int layerMask, RenderObjects.CustomCameraSettings cameraSettings)
		{
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(profileId);
			this.Init(renderPassEvent, shaderTags, renderQueueType, layerMask, cameraSettings);
		}

		internal void Init(RenderPassEvent renderPassEvent, string[] shaderTags, RenderQueueType renderQueueType, int layerMask, RenderObjects.CustomCameraSettings cameraSettings)
		{
			this.m_PassData = new RenderObjectsPass.PassData();
			base.renderPassEvent = renderPassEvent;
			this.renderQueueType = renderQueueType;
			this.overrideMaterial = null;
			this.overrideMaterialPassIndex = 0;
			this.overrideShader = null;
			this.overrideShaderPassIndex = 0;
			RenderQueueRange value = (renderQueueType == RenderQueueType.Transparent) ? RenderQueueRange.transparent : RenderQueueRange.opaque;
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(value), layerMask, uint.MaxValue, 0);
			if (shaderTags != null && shaderTags.Length != 0)
			{
				foreach (string name in shaderTags)
				{
					this.m_ShaderTagIdList.Add(new ShaderTagId(name));
				}
			}
			else
			{
				this.m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
				this.m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
				this.m_ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
			}
			this.m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			this.m_CameraSettings = cameraSettings;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalRenderingData renderingData2 = renderingData.frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
			RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer);
			using (new ProfilingScope(rasterCommandBuffer, base.profilingSampler))
			{
				this.InitPassData(cameraData, ref this.m_PassData);
				this.InitRendererLists(renderingData2, lightData, ref this.m_PassData, context, null, false);
				RenderObjectsPass.ExecutePass(this.m_PassData, rasterCommandBuffer, this.m_PassData.rendererList, renderingData.cameraData.IsCameraProjectionMatrixFlipped());
			}
		}

		private static void ExecutePass(RenderObjectsPass.PassData passData, RasterCommandBuffer cmd, RendererList rendererList, bool isYFlipped)
		{
			Camera camera = passData.cameraData.camera;
			Rect pixelRect = passData.cameraData.pixelRect;
			float aspect = pixelRect.width / pixelRect.height;
			if (passData.cameraSettings.overrideCamera)
			{
				if (passData.cameraData.xr.enabled)
				{
					Debug.LogWarning("RenderObjects pass is configured to override camera matrices. While rendering in stereo camera matrices cannot be overridden.");
				}
				else
				{
					Matrix4x4 matrix4x = Matrix4x4.Perspective(passData.cameraSettings.cameraFieldOfView, aspect, camera.nearClipPlane, camera.farClipPlane);
					matrix4x = GL.GetGPUProjectionMatrix(matrix4x, isYFlipped);
					Matrix4x4 viewMatrix = passData.cameraData.GetViewMatrix(0);
					Vector4 column = viewMatrix.GetColumn(3);
					viewMatrix.SetColumn(3, column + passData.cameraSettings.offset);
					RenderingUtils.SetViewAndProjectionMatrices(cmd, viewMatrix, matrix4x, false);
				}
			}
			if (ScriptableRenderPass.GetActiveDebugHandler(passData.cameraData) != null)
			{
				passData.debugRendererLists.DrawWithRendererList(cmd);
			}
			else
			{
				cmd.DrawRendererList(rendererList);
			}
			if (passData.cameraSettings.overrideCamera && passData.cameraSettings.restoreCamera && !passData.cameraData.xr.enabled)
			{
				RenderingUtils.SetViewAndProjectionMatrices(cmd, passData.cameraData.GetViewMatrix(0), GL.GetGPUProjectionMatrix(passData.cameraData.GetProjectionMatrix(0), isYFlipped), false);
			}
		}

		private void InitPassData(UniversalCameraData cameraData, ref RenderObjectsPass.PassData passData)
		{
			passData.cameraSettings = this.m_CameraSettings;
			passData.renderPassEvent = base.renderPassEvent;
			passData.cameraData = cameraData;
		}

		private void InitRendererLists(UniversalRenderingData renderingData, UniversalLightData lightData, ref RenderObjectsPass.PassData passData, ScriptableRenderContext context, RenderGraph renderGraph, bool useRenderGraph)
		{
			SortingCriteria sortingCriteria = (this.renderQueueType == RenderQueueType.Transparent) ? SortingCriteria.CommonTransparent : passData.cameraData.defaultOpaqueSortFlags;
			DrawingSettings ds = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, renderingData, passData.cameraData, lightData, sortingCriteria);
			ds.overrideMaterial = this.overrideMaterial;
			ds.overrideMaterialPassIndex = this.overrideMaterialPassIndex;
			ds.overrideShader = this.overrideShader;
			ds.overrideShaderPassIndex = this.overrideShaderPassIndex;
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(passData.cameraData);
			FilteringSettings filteringSettings = this.m_FilteringSettings;
			if (useRenderGraph)
			{
				if (activeDebugHandler != null)
				{
					passData.debugRendererLists = activeDebugHandler.CreateRendererListsWithDebugRenderState(renderGraph, ref renderingData.cullResults, ref ds, ref this.m_FilteringSettings, ref this.m_RenderStateBlock);
					return;
				}
				RenderingUtils.CreateRendererListWithRenderStateBlock(renderGraph, ref renderingData.cullResults, ds, this.m_FilteringSettings, this.m_RenderStateBlock, ref passData.rendererListHdl);
				return;
			}
			else
			{
				if (activeDebugHandler != null)
				{
					passData.debugRendererLists = activeDebugHandler.CreateRendererListsWithDebugRenderState(context, ref renderingData.cullResults, ref ds, ref this.m_FilteringSettings, ref this.m_RenderStateBlock);
					return;
				}
				RenderingUtils.CreateRendererListWithRenderStateBlock(context, ref renderingData.cullResults, ds, this.m_FilteringSettings, this.m_RenderStateBlock, ref passData.rendererList);
				return;
			}
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			RenderObjectsPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<RenderObjectsPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\RenderObjectsPass.cs", 274))
			{
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				this.InitPassData(universalCameraData, ref passData);
				passData.color = universalResourceData.activeColorTexture;
				rasterRenderGraphBuilder.SetRenderAttachment(universalResourceData.activeColorTexture, 0, AccessFlags.Write);
				if (universalCameraData.imageScalingMode != ImageScalingMode.Upscaling || passData.renderPassEvent != RenderPassEvent.AfterRenderingPostProcessing)
				{
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Write);
				}
				TextureHandle mainShadowsTexture = universalResourceData.mainShadowsTexture;
				TextureHandle additionalShadowsTexture = universalResourceData.additionalShadowsTexture;
				if (mainShadowsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(mainShadowsTexture, AccessFlags.Read);
				}
				if (additionalShadowsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(additionalShadowsTexture, AccessFlags.Read);
				}
				foreach (TextureHandle textureHandle in universalResourceData.dBuffer)
				{
					if (textureHandle.IsValid())
					{
						rasterRenderGraphBuilder.UseTexture(textureHandle, AccessFlags.Read);
					}
				}
				TextureHandle ssaoTexture = universalResourceData.ssaoTexture;
				if (ssaoTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(ssaoTexture, AccessFlags.Read);
				}
				this.InitRendererLists(renderingData, lightData, ref passData, default(ScriptableRenderContext), renderGraph, true);
				if (ScriptableRenderPass.GetActiveDebugHandler(passData.cameraData) != null)
				{
					passData.debugRendererLists.PrepareRendererListForRasterPass(rasterRenderGraphBuilder);
				}
				else
				{
					rasterRenderGraphBuilder.UseRendererList(passData.rendererListHdl);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (universalCameraData.xr.enabled)
				{
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && universalCameraData.xrUniversal.canFoveateIntermediatePasses);
				}
				rasterRenderGraphBuilder.SetRenderFunc<RenderObjectsPass.PassData>(delegate(RenderObjectsPass.PassData data, RasterGraphContext rgContext)
				{
					bool isYFlipped = data.cameraData.IsRenderTargetProjectionMatrixFlipped(data.color, null);
					RenderObjectsPass.ExecutePass(data, rgContext.cmd, data.rendererListHdl, isYFlipped);
				});
			}
		}

		private RenderQueueType renderQueueType;

		private FilteringSettings m_FilteringSettings;

		private RenderObjects.CustomCameraSettings m_CameraSettings;

		private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

		private RenderObjectsPass.PassData m_PassData;

		private RenderStateBlock m_RenderStateBlock;

		private class PassData
		{
			internal RenderObjects.CustomCameraSettings cameraSettings;

			internal RenderPassEvent renderPassEvent;

			internal TextureHandle color;

			internal RendererListHandle rendererListHdl;

			internal DebugRendererLists debugRendererLists;

			internal UniversalCameraData cameraData;

			internal RendererList rendererList;
		}
	}
}
