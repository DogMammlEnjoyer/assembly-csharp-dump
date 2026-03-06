using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalGBufferRenderPass : ScriptableRenderPass
	{
		public DecalGBufferRenderPass(DecalScreenSpaceSettings settings, DecalDrawGBufferSystem drawSystem, bool decalLayers)
		{
			base.renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
			this.m_DrawSystem = drawSystem;
			this.m_Settings = settings;
			base.profilingSampler = new ProfilingSampler("Draw Decal To GBuffer");
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
			this.m_DecalLayers = decalLayers;
			this.m_ShaderTagIdList = new List<ShaderTagId>();
			if (drawSystem == null)
			{
				this.m_ShaderTagIdList.Add(new ShaderTagId("DecalGBufferProjector"));
			}
			else
			{
				this.m_ShaderTagIdList.Add(new ShaderTagId("DecalGBufferMesh"));
			}
			this.m_PassData = new DecalGBufferRenderPass.PassData();
			this.m_GbufferAttachments = new RTHandle[4];
		}

		internal void Setup(DeferredLights deferredLights)
		{
			this.m_DeferredLights = deferredLights;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			if (this.m_DeferredLights.UseFramebufferFetch)
			{
				this.m_GbufferAttachments[0] = this.m_DeferredLights.GbufferAttachments[0];
				this.m_GbufferAttachments[1] = this.m_DeferredLights.GbufferAttachments[1];
				this.m_GbufferAttachments[2] = this.m_DeferredLights.GbufferAttachments[2];
				this.m_GbufferAttachments[3] = this.m_DeferredLights.GbufferAttachments[3];
				if (this.m_DecalLayers)
				{
					RTHandle[] inputs = new RTHandle[]
					{
						this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GbufferDepthIndex],
						this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GBufferRenderingLayers]
					};
					bool[] array = new bool[2];
					array[0] = true;
					bool[] isTransient = array;
					base.ConfigureInputAttachments(inputs, isTransient);
				}
				else
				{
					RTHandle[] inputs2 = new RTHandle[]
					{
						this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GbufferDepthIndex]
					};
					bool[] isTransient2 = new bool[]
					{
						true
					};
					base.ConfigureInputAttachments(inputs2, isTransient2);
				}
			}
			else
			{
				this.m_GbufferAttachments[0] = this.m_DeferredLights.GbufferAttachments[0];
				this.m_GbufferAttachments[1] = this.m_DeferredLights.GbufferAttachments[1];
				this.m_GbufferAttachments[2] = this.m_DeferredLights.GbufferAttachments[2];
				this.m_GbufferAttachments[3] = this.m_DeferredLights.GbufferAttachments[3];
			}
			base.ConfigureTarget(this.m_GbufferAttachments, this.m_DeferredLights.DepthAttachmentHandle);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			this.InitPassData(cameraData, ref this.m_PassData);
			SortingCriteria sortingCriteria = *renderingData.cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, ref renderingData, sortingCriteria);
			RendererListParams rendererListParams = new RendererListParams(*renderingData.cullResults, drawSettings, this.m_FilteringSettings);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			using (new ProfilingScope(*renderingData.commandBuffer, base.profilingSampler))
			{
				DecalGBufferRenderPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, rendererList);
			}
		}

		private void InitPassData(UniversalCameraData cameraData, ref DecalGBufferRenderPass.PassData passData)
		{
			passData.drawSystem = this.m_DrawSystem;
			passData.settings = this.m_Settings;
			passData.decalLayers = this.m_DecalLayers;
			passData.cameraData = cameraData;
		}

		private static void ExecutePass(RasterCommandBuffer cmd, DecalGBufferRenderPass.PassData passData, RendererList rendererList)
		{
			NormalReconstruction.SetupProperties(cmd, passData.cameraData);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendLow, passData.settings.normalBlend == DecalNormalBlend.Low);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendMedium, passData.settings.normalBlend == DecalNormalBlend.Medium);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalNormalBlendHigh, passData.settings.normalBlend == DecalNormalBlend.High);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalLayers, passData.decalLayers);
			DecalDrawGBufferSystem drawSystem = passData.drawSystem;
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
			DecalGBufferRenderPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DecalGBufferRenderPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Decal\\ScreenSpace\\DecalGBufferRenderPass.cs", 166))
			{
				UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
				UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
				UniversalLightData lightData = frameData.Get<UniversalLightData>();
				this.InitPassData(cameraData, ref passData);
				for (int i = 0; i <= this.m_DeferredLights.GBufferLightingIndex; i++)
				{
					if (universalResourceData.gBuffer[i].IsValid())
					{
						rasterRenderGraphBuilder.SetRenderAttachment(universalResourceData.gBuffer[i], i, AccessFlags.Write);
					}
				}
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Read);
				if (renderGraph.nativeRenderPassesEnabled)
				{
					if (universalResourceData.gBuffer[4].IsValid())
					{
						rasterRenderGraphBuilder.SetInputAttachment(universalResourceData.gBuffer[4], 0, AccessFlags.Read);
					}
					if (this.m_DecalLayers && universalResourceData.gBuffer[5].IsValid())
					{
						rasterRenderGraphBuilder.SetInputAttachment(universalResourceData.gBuffer[5], 1, AccessFlags.Read);
					}
				}
				else
				{
					if (cameraDepthTexture.IsValid())
					{
						rasterRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
					}
					if (this.m_DecalLayers && renderingLayersTexture.IsValid())
					{
						rasterRenderGraphBuilder.UseTexture(renderingLayersTexture, AccessFlags.Read);
					}
				}
				SortingCriteria defaultOpaqueSortFlags = passData.cameraData.defaultOpaqueSortFlags;
				DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, universalRenderingData, passData.cameraData, lightData, defaultOpaqueSortFlags);
				RendererListParams rendererListParams = new RendererListParams(universalRenderingData.cullResults, drawSettings, this.m_FilteringSettings);
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<DecalGBufferRenderPass.PassData>(delegate(DecalGBufferRenderPass.PassData data, RasterGraphContext rgContext)
				{
					DecalGBufferRenderPass.ExecutePass(rgContext.cmd, data, data.rendererList);
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

		private DecalDrawGBufferSystem m_DrawSystem;

		private DecalScreenSpaceSettings m_Settings;

		private DeferredLights m_DeferredLights;

		private RTHandle[] m_GbufferAttachments;

		private bool m_DecalLayers;

		private DecalGBufferRenderPass.PassData m_PassData;

		private class PassData
		{
			internal DecalDrawGBufferSystem drawSystem;

			internal DecalScreenSpaceSettings settings;

			internal bool decalLayers;

			internal UniversalCameraData cameraData;

			internal RendererListHandle rendererList;
		}
	}
}
