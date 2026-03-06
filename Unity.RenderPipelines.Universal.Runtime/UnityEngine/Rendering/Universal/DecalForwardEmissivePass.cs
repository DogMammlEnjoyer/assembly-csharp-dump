using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalForwardEmissivePass : ScriptableRenderPass
	{
		public DecalForwardEmissivePass(DecalDrawFowardEmissiveSystem drawSystem)
		{
			base.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
			base.ConfigureInput(ScriptableRenderPassInput.Depth);
			this.m_DrawSystem = drawSystem;
			base.profilingSampler = new ProfilingSampler("Draw Decal Forward Emissive");
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
			this.m_ShaderTagIdList = new List<ShaderTagId>();
			this.m_ShaderTagIdList.Add(new ShaderTagId("DecalMeshForwardEmissive"));
			this.m_ShaderTagIdList.Add(new ShaderTagId("DecalProjectorForwardEmissive"));
			this.m_PassData = new DecalForwardEmissivePass.PassData();
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			this.InitPassData(ref this.m_PassData);
			UniversalRenderingData universalRenderingData = renderingData.frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
			RendererListParams rendererListParams = this.InitRendererListParams(universalRenderingData, cameraData, lightData);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			using (new ProfilingScope(universalRenderingData.commandBuffer, base.profilingSampler))
			{
				DecalForwardEmissivePass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(universalRenderingData.commandBuffer), this.m_PassData, rendererList);
			}
		}

		private void InitPassData(ref DecalForwardEmissivePass.PassData passData)
		{
			passData.drawSystem = this.m_DrawSystem;
		}

		private RendererListParams InitRendererListParams(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			SortingCriteria defaultOpaqueSortFlags = cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, renderingData, cameraData, lightData, defaultOpaqueSortFlags);
			return new RendererListParams(renderingData.cullResults, drawSettings, this.m_FilteringSettings);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, DecalForwardEmissivePass.PassData passData, RendererList rendererList)
		{
			passData.drawSystem.Execute(cmd);
			cmd.DrawRendererList(rendererList);
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			DecalForwardEmissivePass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DecalForwardEmissivePass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Decal\\DBuffer\\DecalForwardEmissivePass.cs", 82))
			{
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
				UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
				UniversalLightData lightData = frameData.Get<UniversalLightData>();
				this.InitPassData(ref passData);
				RendererListParams rendererListParams = this.InitRendererListParams(renderingData, universalCameraData, lightData);
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				UniversalRenderer universalRenderer = (UniversalRenderer)universalCameraData.renderer;
				rasterRenderGraphBuilder.SetRenderAttachment(universalResourceData.activeColorTexture, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Read);
				rasterRenderGraphBuilder.SetRenderFunc<DecalForwardEmissivePass.PassData>(delegate(DecalForwardEmissivePass.PassData data, RasterGraphContext rgContext)
				{
					DecalForwardEmissivePass.ExecutePass(rgContext.cmd, data, data.rendererList);
				});
			}
		}

		private FilteringSettings m_FilteringSettings;

		private List<ShaderTagId> m_ShaderTagIdList;

		private DecalDrawFowardEmissiveSystem m_DrawSystem;

		private DecalForwardEmissivePass.PassData m_PassData;

		private class PassData
		{
			internal DecalDrawFowardEmissiveSystem drawSystem;

			internal RendererListHandle rendererList;
		}
	}
}
