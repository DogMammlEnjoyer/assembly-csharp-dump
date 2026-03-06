using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class DepthOnlyPass : ScriptableRenderPass
	{
		private RTHandle destination { get; set; }

		internal ShaderTagId shaderTagId { get; set; } = DepthOnlyPass.k_ShaderTagId;

		public DepthOnlyPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
		{
			base.profilingSampler = new ProfilingSampler("Draw Depth Only");
			this.m_PassData = new DepthOnlyPass.PassData();
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(renderQueueRange), layerMask, uint.MaxValue, 0);
			base.renderPassEvent = evt;
			base.useNativeRenderPass = false;
			this.shaderTagId = DepthOnlyPass.k_ShaderTagId;
		}

		public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle depthAttachmentHandle)
		{
			this.destination = depthAttachmentHandle;
			this.depthStencilFormat = baseDescriptor.depthStencilFormat;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			ref RenderTextureDescriptor cameraTargetDescriptor = ref renderingData.cameraData.cameraTargetDescriptor;
			if (renderingData.cameraData.renderer->useDepthPriming && (*renderingData.cameraData.renderType == CameraRenderType.Base || *renderingData.cameraData.clearDepth))
			{
				base.ConfigureTarget(renderingData.cameraData.renderer->cameraDepthTargetHandle);
				base.ConfigureClear(ClearFlag.Depth, Color.black);
				return;
			}
			base.useNativeRenderPass = true;
			base.ConfigureTarget(this.destination);
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, RendererList rendererList)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.DepthPrepass)))
			{
				cmd.DrawRendererList(rendererList);
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			RendererListParams rendererListParams = this.InitRendererListParams(renderingData2, cameraData, lightData);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			DepthOnlyPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), rendererList);
		}

		private RendererListParams InitRendererListParams(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			SortingCriteria defaultOpaqueSortFlags = cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.shaderTagId, renderingData, cameraData, lightData, defaultOpaqueSortFlags);
			drawSettings.perObjectData = PerObjectData.None;
			drawSettings.lodCrossFadeStencilMask = 0;
			return new RendererListParams(renderingData.cullResults, drawSettings, this.m_FilteringSettings);
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, ref TextureHandle cameraDepthTexture, uint batchLayerMask, bool setGlobalDepth)
		{
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			DepthOnlyPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DepthOnlyPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DepthOnlyPass.cs", 131))
			{
				RendererListParams rendererListParams = this.InitRendererListParams(renderingData, universalCameraData, lightData);
				rendererListParams.filteringSettings.batchLayerMask = batchLayerMask;
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(cameraDepthTexture, AccessFlags.Write);
				if (setGlobalDepth)
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(cameraDepthTexture, DepthOnlyPass.s_CameraDepthTextureID);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (universalCameraData.xr.enabled)
				{
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && universalCameraData.xrUniversal.canFoveateIntermediatePasses);
				}
				rasterRenderGraphBuilder.SetRenderFunc<DepthOnlyPass.PassData>(delegate(DepthOnlyPass.PassData data, RasterGraphContext context)
				{
					DepthOnlyPass.ExecutePass(context.cmd, data.rendererList);
				});
			}
		}

		private GraphicsFormat depthStencilFormat;

		private DepthOnlyPass.PassData m_PassData;

		private FilteringSettings m_FilteringSettings;

		private static readonly ShaderTagId k_ShaderTagId = new ShaderTagId("DepthOnly");

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private class PassData
		{
			internal RendererListHandle rendererList;
		}
	}
}
