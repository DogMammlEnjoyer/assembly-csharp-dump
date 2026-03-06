using System;
using Unity.Collections;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	internal class GBufferPass : ScriptableRenderPass
	{
		public GBufferPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference, DeferredLights deferredLights)
		{
			base.profilingSampler = new ProfilingSampler("Draw GBuffer");
			base.renderPassEvent = evt;
			this.m_PassData = new GBufferPass.PassData();
			this.m_DeferredLights = deferredLights;
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(renderQueueRange), layerMask, uint.MaxValue, 0);
			this.m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			this.m_RenderStateBlock.stencilState = stencilState;
			this.m_RenderStateBlock.stencilReference = stencilReference;
			this.m_RenderStateBlock.mask = RenderStateMask.Stencil;
			if (GBufferPass.s_ShaderTagValues == null)
			{
				GBufferPass.s_ShaderTagValues = new ShaderTagId[5];
				GBufferPass.s_ShaderTagValues[0] = GBufferPass.s_ShaderTagLit;
				GBufferPass.s_ShaderTagValues[1] = GBufferPass.s_ShaderTagSimpleLit;
				GBufferPass.s_ShaderTagValues[2] = GBufferPass.s_ShaderTagUnlit;
				GBufferPass.s_ShaderTagValues[3] = GBufferPass.s_ShaderTagComplexLit;
				GBufferPass.s_ShaderTagValues[4] = default(ShaderTagId);
			}
			if (GBufferPass.s_RenderStateBlocks == null)
			{
				GBufferPass.s_RenderStateBlocks = new RenderStateBlock[5];
				GBufferPass.s_RenderStateBlocks[0] = DeferredLights.OverwriteStencil(this.m_RenderStateBlock, 96, 32);
				GBufferPass.s_RenderStateBlocks[1] = DeferredLights.OverwriteStencil(this.m_RenderStateBlock, 96, 64);
				GBufferPass.s_RenderStateBlocks[2] = DeferredLights.OverwriteStencil(this.m_RenderStateBlock, 96, 0);
				GBufferPass.s_RenderStateBlocks[3] = DeferredLights.OverwriteStencil(this.m_RenderStateBlock, 96, 0);
				GBufferPass.s_RenderStateBlocks[4] = GBufferPass.s_RenderStateBlocks[0];
			}
		}

		public void Dispose()
		{
			DeferredLights deferredLights = this.m_DeferredLights;
			if (deferredLights == null)
			{
				return;
			}
			deferredLights.ReleaseGbufferResources();
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			RTHandle[] gbufferAttachments = this.m_DeferredLights.GbufferAttachments;
			if (cmd != null)
			{
				bool flag = true;
				if (this.m_DeferredLights.UseFramebufferFetch && this.m_DeferredLights.DepthCopyTexture != null && this.m_DeferredLights.DepthCopyTexture.rt != null)
				{
					this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GbufferDepthIndex] = this.m_DeferredLights.DepthCopyTexture;
					flag = false;
				}
				for (int i = 0; i < gbufferAttachments.Length; i++)
				{
					if (i != this.m_DeferredLights.GBufferLightingIndex && (i != this.m_DeferredLights.GBufferNormalSmoothnessIndex || !this.m_DeferredLights.HasNormalPrepass) && (i != this.m_DeferredLights.GbufferDepthIndex || flag) && (!this.m_DeferredLights.UseFramebufferFetch || i == this.m_DeferredLights.GbufferDepthIndex || this.m_DeferredLights.HasDepthPrepass))
					{
						this.m_DeferredLights.ReAllocateGBufferIfNeeded(cameraTextureDescriptor, i);
						cmd.SetGlobalTexture(this.m_DeferredLights.GbufferAttachments[i].name, this.m_DeferredLights.GbufferAttachments[i].nameID);
					}
				}
			}
			if (this.m_DeferredLights.UseFramebufferFetch)
			{
				this.m_DeferredLights.UpdateDeferredInputAttachments();
			}
			base.ConfigureTarget(this.m_DeferredLights.GbufferAttachments, this.m_DeferredLights.DepthAttachment, this.m_DeferredLights.GbufferFormats);
			base.ConfigureClear(ClearFlag.None, Color.black);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			this.m_PassData.deferredLights = this.m_DeferredLights;
			this.InitRendererLists(ref this.m_PassData, context, null, renderingData2, cameraData, lightData, false, uint.MaxValue);
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			using (new ProfilingScope(commandBuffer, base.profilingSampler))
			{
				GBufferPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), this.m_PassData, this.m_PassData.rendererList, this.m_PassData.objectsWithErrorRendererList);
				if (!this.m_DeferredLights.UseFramebufferFetch)
				{
					renderingData.commandBuffer->SetGlobalTexture(GBufferPass.s_CameraNormalsTextureID, this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GBufferNormalSmoothnessIndex]);
				}
			}
		}

		private static void ExecutePass(RasterCommandBuffer cmd, GBufferPass.PassData data, RendererList rendererList, RendererList errorRendererList)
		{
			bool flag = data.deferredLights.UseRenderingLayers && !data.deferredLights.HasRenderingLayerPrepass;
			if (flag)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, true);
			}
			cmd.DrawRendererList(rendererList);
			if (flag)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, false);
			}
		}

		private void InitRendererLists(ref GBufferPass.PassData passData, ScriptableRenderContext context, RenderGraph renderGraph, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, bool useRenderGraph, uint batchLayerMask = 4294967295U)
		{
			ShaderTagId shaderTagId = GBufferPass.s_ShaderTagUniversalGBuffer;
			DrawingSettings drawSettings = base.CreateDrawingSettings(shaderTagId, renderingData, cameraData, lightData, cameraData.defaultOpaqueSortFlags);
			FilteringSettings filteringSettings = this.m_FilteringSettings;
			filteringSettings.batchLayerMask = batchLayerMask;
			NativeArray<ShaderTagId> value = new NativeArray<ShaderTagId>(GBufferPass.s_ShaderTagValues, Allocator.Temp);
			NativeArray<RenderStateBlock> value2 = new NativeArray<RenderStateBlock>(GBufferPass.s_RenderStateBlocks, Allocator.Temp);
			RendererListParams rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings)
			{
				tagValues = new NativeArray<ShaderTagId>?(value),
				stateBlocks = new NativeArray<RenderStateBlock>?(value2),
				tagName = GBufferPass.s_ShaderTagUniversalMaterialType,
				isPassTagName = false
			};
			if (useRenderGraph)
			{
				passData.rendererListHdl = renderGraph.CreateRendererList(rendererListParams);
			}
			else
			{
				passData.rendererList = context.CreateRendererList(ref rendererListParams);
			}
			value.Dispose();
			value2.Dispose();
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle cameraColor, TextureHandle cameraDepth, bool setGlobalTextures, uint batchLayerMask = 4294967295U)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			GBufferPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<GBufferPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\GBufferPass.cs", 232))
			{
				bool flag = this.m_DeferredLights.UseRenderingLayers && !this.m_DeferredLights.UseLightLayers;
				passData.gbuffer = this.m_DeferredLights.GbufferTextureHandles;
				for (int i = 0; i < this.m_DeferredLights.GBufferSliceCount; i++)
				{
					rasterRenderGraphBuilder.SetRenderAttachment(passData.gbuffer[i], i, AccessFlags.Write);
				}
				RenderGraphUtils.UseDBufferIfValid(rasterRenderGraphBuilder, universalResourceData);
				passData.depth = cameraDepth;
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(cameraDepth, AccessFlags.Write);
				passData.deferredLights = this.m_DeferredLights;
				this.InitRendererLists(ref passData, default(ScriptableRenderContext), renderGraph, renderingData, cameraData, lightData, true, uint.MaxValue);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererListHdl);
				rasterRenderGraphBuilder.UseRendererList(passData.objectsWithErrorRendererListHdl);
				if (setGlobalTextures)
				{
					IBaseRenderGraphBuilder baseRenderGraphBuilder = rasterRenderGraphBuilder;
					TextureHandle textureHandle = universalResourceData.cameraNormalsTexture;
					baseRenderGraphBuilder.SetGlobalTextureAfterPass(textureHandle, GBufferPass.s_CameraNormalsTextureID);
					if (flag)
					{
						IBaseRenderGraphBuilder baseRenderGraphBuilder2 = rasterRenderGraphBuilder;
						textureHandle = universalResourceData.renderingLayersTexture;
						baseRenderGraphBuilder2.SetGlobalTextureAfterPass(textureHandle, GBufferPass.s_CameraRenderingLayersTextureID);
					}
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<GBufferPass.PassData>(delegate(GBufferPass.PassData data, RasterGraphContext context)
				{
					GBufferPass.ExecutePass(context.cmd, data, data.rendererListHdl, data.objectsWithErrorRendererListHdl);
				});
			}
		}

		private static readonly int s_CameraNormalsTextureID = Shader.PropertyToID("_CameraNormalsTexture");

		private static readonly int s_CameraRenderingLayersTextureID = Shader.PropertyToID("_CameraRenderingLayersTexture");

		private static readonly ShaderTagId s_ShaderTagLit = new ShaderTagId("Lit");

		private static readonly ShaderTagId s_ShaderTagSimpleLit = new ShaderTagId("SimpleLit");

		private static readonly ShaderTagId s_ShaderTagUnlit = new ShaderTagId("Unlit");

		private static readonly ShaderTagId s_ShaderTagComplexLit = new ShaderTagId("ComplexLit");

		private static readonly ShaderTagId s_ShaderTagUniversalGBuffer = new ShaderTagId("UniversalGBuffer");

		private static readonly ShaderTagId s_ShaderTagUniversalMaterialType = new ShaderTagId("UniversalMaterialType");

		private DeferredLights m_DeferredLights;

		private static ShaderTagId[] s_ShaderTagValues;

		private static RenderStateBlock[] s_RenderStateBlocks;

		private FilteringSettings m_FilteringSettings;

		private RenderStateBlock m_RenderStateBlock;

		private GBufferPass.PassData m_PassData;

		private class PassData
		{
			internal TextureHandle[] gbuffer;

			internal TextureHandle depth;

			internal DeferredLights deferredLights;

			internal RendererListHandle rendererListHdl;

			internal RendererListHandle objectsWithErrorRendererListHdl;

			internal RendererList rendererList;

			internal RendererList objectsWithErrorRendererList;
		}
	}
}
