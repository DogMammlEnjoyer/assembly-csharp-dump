using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalPreviewPass : ScriptableRenderPass
	{
		public DecalPreviewPass()
		{
			base.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
			base.ConfigureInput(ScriptableRenderPassInput.Depth);
			this.m_ProfilingSampler = new ProfilingSampler("Decal Preview Render");
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
			this.m_ShaderTagIdList = new List<ShaderTagId>();
			this.m_ShaderTagIdList.Add(new ShaderTagId("DecalScreenSpaceMesh"));
			this.m_PassData = new DecalPreviewPass.PassData();
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalRenderingData universalRenderingData = renderingData.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
			SortingCriteria defaultOpaqueSortFlags = universalCameraData.defaultOpaqueSortFlags;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, universalRenderingData, universalCameraData, lightData, defaultOpaqueSortFlags);
			RendererListParams rendererListParams = new RendererListParams(universalRenderingData.cullResults, drawSettings, this.m_FilteringSettings);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			using (new ProfilingScope(universalRenderingData.commandBuffer, this.m_ProfilingSampler))
			{
				DecalPreviewPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(universalRenderingData.commandBuffer), this.m_PassData, rendererList);
			}
		}

		private static void ExecutePass(RasterCommandBuffer cmd, DecalPreviewPass.PassData passData, RendererList rendererList)
		{
			cmd.DrawRendererList(rendererList);
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			DecalPreviewPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DecalPreviewPass.PassData>("Decal Preview Pass", out passData, this.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Decal\\DecalPreviewPass.cs", 62))
			{
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
				UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
				UniversalLightData lightData = frameData.Get<UniversalLightData>();
				UniversalRenderer universalRenderer = (UniversalRenderer)universalCameraData.renderer;
				rasterRenderGraphBuilder.SetRenderAttachment(universalResourceData.activeColorTexture, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Read);
				SortingCriteria defaultOpaqueSortFlags = universalCameraData.defaultOpaqueSortFlags;
				DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, universalRenderingData, universalCameraData, lightData, defaultOpaqueSortFlags);
				RendererListParams rendererListParams = new RendererListParams(universalRenderingData.cullResults, drawSettings, this.m_FilteringSettings);
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				rasterRenderGraphBuilder.SetRenderFunc<DecalPreviewPass.PassData>(delegate(DecalPreviewPass.PassData data, RasterGraphContext rgContext)
				{
					DecalPreviewPass.ExecutePass(rgContext.cmd, data, data.rendererList);
				});
			}
		}

		private FilteringSettings m_FilteringSettings;

		private List<ShaderTagId> m_ShaderTagIdList;

		private ProfilingSampler m_ProfilingSampler;

		private DecalPreviewPass.PassData m_PassData;

		private class PassData
		{
			internal RendererListHandle rendererList;
		}
	}
}
