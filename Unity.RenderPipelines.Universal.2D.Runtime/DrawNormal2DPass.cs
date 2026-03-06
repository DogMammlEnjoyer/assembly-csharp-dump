using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DrawNormal2DPass : ScriptableRenderPass
	{
		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			throw new NotImplementedException();
		}

		private static void Execute(RasterCommandBuffer cmd, DrawNormal2DPass.PassData passData)
		{
			cmd.DrawRendererList(passData.rendererList);
		}

		public void Render(RenderGraph graph, ContextContainer frameData, Renderer2DData rendererData, ref LayerBatch layerBatch, int batchIndex)
		{
			Universal2DResourceData universal2DResourceData = frameData.Get<Universal2DResourceData>();
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			if (!layerBatch.useNormals)
			{
				return;
			}
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			DrawNormal2DPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<DrawNormal2DPass.PassData>(DrawNormal2DPass.k_NormalPass, out passData, DrawNormal2DPass.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\DrawNormal2DPass.cs", 42))
			{
				FilteringSettings filteringSettings;
				LayerUtility.GetFilterSettings(rendererData, ref layerBatch, out filteringSettings);
				DrawingSettings drawSettings = base.CreateDrawingSettings(DrawNormal2DPass.k_NormalsRenderingPassName, universalRenderingData, universalCameraData, lightData, SortingCriteria.CommonTransparent);
				SortingSettings sortingSettings = drawSettings.sortingSettings;
				RendererLighting.GetTransparencySortingMode(rendererData, universalCameraData.camera, ref sortingSettings);
				drawSettings.sortingSettings = sortingSettings;
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderAttachment(universal2DResourceData.normalsTexture[batchIndex], 0, AccessFlags.Write);
				if (rendererData.useDepthStencilBuffer)
				{
					TextureHandle tex = universal2DResourceData.normalsDepth.IsValid() ? universal2DResourceData.normalsDepth : universalResourceData.activeDepthTexture;
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(tex, AccessFlags.Write);
				}
				RendererListParams rendererListParams = new RendererListParams(universalRenderingData.cullResults, drawSettings, filteringSettings);
				passData.rendererList = graph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				rasterRenderGraphBuilder.SetRenderFunc<DrawNormal2DPass.PassData>(delegate(DrawNormal2DPass.PassData data, RasterGraphContext context)
				{
					DrawNormal2DPass.Execute(context.cmd, data);
				});
			}
		}

		private static readonly string k_NormalPass = "Normal2D Pass";

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(DrawNormal2DPass.k_NormalPass);

		private static readonly ShaderTagId k_NormalsRenderingPassName = new ShaderTagId("NormalsRendering");

		private class PassData
		{
			internal RendererListHandle rendererList;
		}
	}
}
