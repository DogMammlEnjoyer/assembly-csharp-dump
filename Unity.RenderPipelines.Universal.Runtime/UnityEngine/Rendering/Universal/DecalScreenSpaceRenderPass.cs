using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalScreenSpaceRenderPass : ScriptableRenderPass
	{
		public DecalScreenSpaceRenderPass(DecalScreenSpaceSettings settings, DecalDrawScreenSpaceSystem drawSystem, bool decalLayers)
		{
			base.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
			ScriptableRenderPassInput passInput = ScriptableRenderPassInput.Depth;
			base.ConfigureInput(passInput);
			this.m_DrawSystem = drawSystem;
			this.m_Settings = settings;
			base.profilingSampler = new ProfilingSampler("Draw Decal Screen Space");
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
			this.m_DecalLayers = decalLayers;
			this.m_ShaderTagIdList = new List<ShaderTagId>();
			if (this.m_DrawSystem == null)
			{
				this.m_ShaderTagIdList.Add(new ShaderTagId("DecalScreenSpaceProjector"));
			}
			else
			{
				this.m_ShaderTagIdList.Add(new ShaderTagId("DecalScreenSpaceMesh"));
			}
			this.m_PassData = new DecalScreenSpaceRenderPass.PassData();
		}

		private RendererListParams CreateRenderListParams(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			SortingCriteria sortingCriteria = SortingCriteria.None;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, renderingData, cameraData, lightData, sortingCriteria);
			return new RendererListParams(renderingData.cullResults, drawSettings, this.m_FilteringSettings);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			this.InitPassData(cameraData, ref this.m_PassData);
			RenderingUtils.SetScaleBiasRt(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), renderingData);
			UniversalRenderingData renderingData2 = renderingData.frameData.Get<UniversalRenderingData>();
			UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
			RendererListParams rendererListParams = this.CreateRenderListParams(renderingData2, cameraData, lightData);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			using (new ProfilingScope(*renderingData.commandBuffer, base.profilingSampler))
			{
				DecalScreenSpaceRenderPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, rendererList);
			}
		}

		private void InitPassData(UniversalCameraData cameraData, ref DecalScreenSpaceRenderPass.PassData passData)
		{
			passData.drawSystem = this.m_DrawSystem;
			passData.settings = this.m_Settings;
			passData.decalLayers = this.m_DecalLayers;
			passData.isGLDevice = DecalRendererFeature.isGLDevice;
			passData.cameraData = cameraData;
		}

		private static void ExecutePass(RasterCommandBuffer cmd, DecalScreenSpaceRenderPass.PassData passData, RendererList rendererList)
		{
			NormalReconstruction.SetupProperties(cmd, passData.cameraData);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendLow, passData.settings.normalBlend == DecalNormalBlend.Low);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendMedium, passData.settings.normalBlend == DecalNormalBlend.Medium);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendHigh, passData.settings.normalBlend == DecalNormalBlend.High);
			if (!passData.isGLDevice)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.DecalLayers, passData.decalLayers);
			}
			DecalDrawScreenSpaceSystem drawSystem = passData.drawSystem;
			if (drawSystem != null)
			{
				drawSystem.Execute(cmd);
			}
			cmd.DrawRendererList(rendererList);
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			TextureHandle cameraDepthTexture = universalResourceData.cameraDepthTexture;
			TextureHandle renderingLayersTexture = universalResourceData.renderingLayersTexture;
			DecalScreenSpaceRenderPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DecalScreenSpaceRenderPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Decal\\ScreenSpace\\DecalScreenSpaceRenderPass.cs", 114))
			{
				UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
				UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
				UniversalLightData lightData = frameData.Get<UniversalLightData>();
				this.InitPassData(cameraData, ref passData);
				passData.colorTarget = universalResourceData.cameraColor;
				rasterRenderGraphBuilder.SetRenderAttachment(universalResourceData.activeColorTexture, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Read);
				RendererListParams rendererListParams = this.CreateRenderListParams(renderingData, passData.cameraData, lightData);
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				if (cameraDepthTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				}
				if (passData.decalLayers && renderingLayersTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(renderingLayersTexture, AccessFlags.Read);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<DecalScreenSpaceRenderPass.PassData>(delegate(DecalScreenSpaceRenderPass.PassData data, RasterGraphContext rgContext)
				{
					RenderingUtils.SetScaleBiasRt(rgContext.cmd, data.cameraData, data.colorTarget);
					DecalScreenSpaceRenderPass.ExecutePass(rgContext.cmd, data, data.rendererList);
				});
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendLow, false);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendMedium, false);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendHigh, false);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalLayers, false);
		}

		private FilteringSettings m_FilteringSettings;

		private List<ShaderTagId> m_ShaderTagIdList;

		private DecalDrawScreenSpaceSystem m_DrawSystem;

		private DecalScreenSpaceSettings m_Settings;

		private bool m_DecalLayers;

		private DecalScreenSpaceRenderPass.PassData m_PassData;

		private class PassData
		{
			internal DecalDrawScreenSpaceSystem drawSystem;

			internal DecalScreenSpaceSettings settings;

			internal bool decalLayers;

			internal bool isGLDevice;

			internal TextureHandle colorTarget;

			internal UniversalCameraData cameraData;

			internal RendererListHandle rendererList;
		}
	}
}
