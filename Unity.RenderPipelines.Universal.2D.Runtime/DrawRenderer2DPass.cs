using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DrawRenderer2DPass : ScriptableRenderPass
	{
		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			throw new NotImplementedException();
		}

		private static void Execute(RasterGraphContext context, DrawRenderer2DPass.PassData passData)
		{
			RasterCommandBuffer cmd = context.cmd;
			int num = passData.blendStyleIndices.Length;
			cmd.SetGlobalFloat(DrawRenderer2DPass.k_HDREmulationScaleID, passData.hdrEmulationScale);
			cmd.SetGlobalColor(DrawRenderer2DPass.k_RendererColorID, Color.white);
			RendererLighting.SetLightShaderGlobals(cmd, passData.lightBlendStyles, passData.blendStyleIndices);
			if (passData.layerUseLights)
			{
				for (int i = 0; i < num; i++)
				{
					int blendStyleIndex = passData.blendStyleIndices[i];
					RendererLighting.EnableBlendStyle(cmd, blendStyleIndex, true);
				}
			}
			else if (passData.isSceneLit)
			{
				RendererLighting.EnableBlendStyle(cmd, 0, true);
			}
			if (passData.activeDebugHandler)
			{
				passData.debugRendererLists.DrawWithRendererList(cmd);
			}
			else
			{
				cmd.DrawRendererList(passData.rendererList);
			}
			RendererLighting.DisableAllKeywords(cmd);
		}

		public void Render(RenderGraph graph, ContextContainer frameData, Renderer2DData rendererData, ref LayerBatch[] layerBatches, int batchIndex, ref FilteringSettings filterSettings)
		{
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			Universal2DResourceData universal2DResourceData = frameData.Get<Universal2DResourceData>();
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			LayerBatch layerBatch = layerBatches[batchIndex];
			bool isLitView = true;
			if (batchIndex == 0)
			{
				DrawRenderer2DPass.SetGlobalPassData setGlobalPassData;
				using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<DrawRenderer2DPass.SetGlobalPassData>(DrawRenderer2DPass.k_SetLightBlendTexture, out setGlobalPassData, DrawRenderer2DPass.m_SetLightBlendTextureProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\DrawRenderer2DPass.cs", 116))
				{
					if (layerBatch.lightStats.useLights)
					{
						setGlobalPassData.lightTextures = universal2DResourceData.lightTextures[batchIndex];
						for (int i = 0; i < setGlobalPassData.lightTextures.Length; i++)
						{
							rasterRenderGraphBuilder.UseTexture(setGlobalPassData.lightTextures[i], AccessFlags.Read);
						}
					}
					this.SetGlobalLightTextures(graph, rasterRenderGraphBuilder, setGlobalPassData.lightTextures, ref layerBatch, rendererData, isLitView);
					rasterRenderGraphBuilder.AllowGlobalStateModification(true);
					rasterRenderGraphBuilder.SetRenderFunc<DrawRenderer2DPass.SetGlobalPassData>(delegate(DrawRenderer2DPass.SetGlobalPassData data, RasterGraphContext context)
					{
					});
				}
			}
			DrawRenderer2DPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder2 = graph.AddRasterRenderPass<DrawRenderer2DPass.PassData>(DrawRenderer2DPass.k_RenderPass, out passData, DrawRenderer2DPass.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\DrawRenderer2DPass.cs", 136))
			{
				passData.lightBlendStyles = rendererData.lightBlendStyles;
				passData.blendStyleIndices = layerBatch.activeBlendStylesIndices;
				passData.hdrEmulationScale = rendererData.hdrEmulationScale;
				passData.isSceneLit = rendererData.lightCullResult.IsSceneLit();
				passData.layerUseLights = layerBatch.lightStats.useLights;
				DrawingSettings drawSettings = base.CreateDrawingSettings(DrawRenderer2DPass.k_ShaderTags, universalRenderingData, universalCameraData, lightData, SortingCriteria.CommonTransparent);
				SortingSettings sortingSettings = drawSettings.sortingSettings;
				RendererLighting.GetTransparencySortingMode(rendererData, universalCameraData.camera, ref sortingSettings);
				drawSettings.sortingSettings = sortingSettings;
				DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
				passData.activeDebugHandler = (activeDebugHandler != null);
				if (activeDebugHandler != null)
				{
					RenderStateBlock renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
					passData.debugRendererLists = activeDebugHandler.CreateRendererListsWithDebugRenderState(graph, ref universalRenderingData.cullResults, ref drawSettings, ref filterSettings, ref renderStateBlock);
					passData.debugRendererLists.PrepareRendererListForRasterPass(rasterRenderGraphBuilder2);
				}
				else
				{
					RendererListParams rendererListParams = new RendererListParams(universalRenderingData.cullResults, drawSettings, filterSettings);
					passData.rendererList = graph.CreateRendererList(rendererListParams);
					rasterRenderGraphBuilder2.UseRendererList(passData.rendererList);
				}
				if (passData.layerUseLights)
				{
					passData.lightTextures = universal2DResourceData.lightTextures[batchIndex];
					for (int j = 0; j < passData.lightTextures.Length; j++)
					{
						rasterRenderGraphBuilder2.UseTexture(passData.lightTextures[j], AccessFlags.Read);
					}
				}
				if (rendererData.useCameraSortingLayerTexture)
				{
					IBaseRenderGraphBuilder baseRenderGraphBuilder = rasterRenderGraphBuilder2;
					TextureHandle cameraSortingLayerTexture = universal2DResourceData.cameraSortingLayerTexture;
					baseRenderGraphBuilder.UseTexture(cameraSortingLayerTexture, AccessFlags.Read);
				}
				rasterRenderGraphBuilder2.SetRenderAttachment(universalResourceData.activeColorTexture, 0, AccessFlags.Write);
				if (rendererData.useDepthStencilBuffer)
				{
					rasterRenderGraphBuilder2.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Write);
				}
				rasterRenderGraphBuilder2.AllowGlobalStateModification(true);
				int num = batchIndex + 1;
				if (num < universal2DResourceData.lightTextures.Length)
				{
					this.SetGlobalLightTextures(graph, rasterRenderGraphBuilder2, universal2DResourceData.lightTextures[num], ref layerBatches[num], rendererData, isLitView);
				}
				rasterRenderGraphBuilder2.SetRenderFunc<DrawRenderer2DPass.PassData>(delegate(DrawRenderer2DPass.PassData data, RasterGraphContext context)
				{
					DrawRenderer2DPass.Execute(context, data);
				});
			}
		}

		private void SetGlobalLightTextures(RenderGraph graph, IRasterRenderGraphBuilder builder, TextureHandle[] lightTextures, ref LayerBatch layerBatch, Renderer2DData rendererData, bool isLitView)
		{
			if (isLitView)
			{
				if (layerBatch.lightStats.useLights)
				{
					for (int i = 0; i < lightTextures.Length; i++)
					{
						int num = layerBatch.activeBlendStylesIndices[i];
						builder.SetGlobalTextureAfterPass(lightTextures[i], Shader.PropertyToID(RendererLighting.k_ShapeLightTextureIDs[num]));
					}
					return;
				}
				if (rendererData.lightCullResult.IsSceneLit())
				{
					for (int j = 0; j < RendererLighting.k_ShapeLightTextureIDs.Length; j++)
					{
						TextureHandle blackTexture = graph.defaultResources.blackTexture;
						builder.SetGlobalTextureAfterPass(blackTexture, Shader.PropertyToID(RendererLighting.k_ShapeLightTextureIDs[j]));
					}
				}
			}
		}

		private static readonly string k_RenderPass = "Renderer2D Pass";

		private static readonly string k_SetLightBlendTexture = "SetLightBlendTextures";

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(DrawRenderer2DPass.k_RenderPass);

		private static readonly ProfilingSampler m_SetLightBlendTextureProfilingSampler = new ProfilingSampler(DrawRenderer2DPass.k_SetLightBlendTexture);

		private static readonly ShaderTagId k_CombinedRenderingPassName = new ShaderTagId("Universal2D");

		private static readonly ShaderTagId k_LegacyPassName = new ShaderTagId("SRPDefaultUnlit");

		private static readonly List<ShaderTagId> k_ShaderTags = new List<ShaderTagId>
		{
			DrawRenderer2DPass.k_LegacyPassName,
			DrawRenderer2DPass.k_CombinedRenderingPassName
		};

		private static readonly int k_HDREmulationScaleID = Shader.PropertyToID("_HDREmulationScale");

		private static readonly int k_RendererColorID = Shader.PropertyToID("_RendererColor");

		private class SetGlobalPassData
		{
			internal TextureHandle[] lightTextures;
		}

		private class PassData
		{
			internal Light2DBlendStyle[] lightBlendStyles;

			internal int[] blendStyleIndices;

			internal float hdrEmulationScale;

			internal bool isSceneLit;

			internal bool layerUseLights;

			internal TextureHandle[] lightTextures;

			internal RendererListHandle rendererList;

			internal DebugRendererLists debugRendererLists;

			internal bool activeDebugHandler;
		}
	}
}
