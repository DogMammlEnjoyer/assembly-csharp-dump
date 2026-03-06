using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class CopyCameraSortingLayerPass : ScriptableRenderPass
	{
		public CopyCameraSortingLayerPass(Material blitMaterial)
		{
			CopyCameraSortingLayerPass.m_BlitMaterial = blitMaterial;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			throw new NotImplementedException();
		}

		public static void ConfigureDescriptor(Downsampling downsamplingMethod, ref RenderTextureDescriptor descriptor, out FilterMode filterMode)
		{
			descriptor.msaaSamples = 1;
			descriptor.depthStencilFormat = GraphicsFormat.None;
			if (downsamplingMethod == Downsampling._2xBilinear)
			{
				descriptor.width /= 2;
				descriptor.height /= 2;
			}
			else if (downsamplingMethod == Downsampling._4xBox || downsamplingMethod == Downsampling._4xBilinear)
			{
				descriptor.width /= 4;
				descriptor.height /= 4;
			}
			filterMode = ((downsamplingMethod == Downsampling.None || downsamplingMethod == Downsampling._4xBox) ? FilterMode.Point : FilterMode.Bilinear);
		}

		private static void Execute(RasterCommandBuffer cmd, RTHandle source)
		{
			using (new ProfilingScope(cmd, CopyCameraSortingLayerPass.m_ExecuteProfilingSampler))
			{
				Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
				Blitter.BlitTexture(cmd, source, v, CopyCameraSortingLayerPass.m_BlitMaterial, (source.rt.filterMode == FilterMode.Bilinear) ? 1 : 0);
			}
		}

		public void Render(RenderGraph graph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			Universal2DResourceData universal2DResourceData = frameData.Get<Universal2DResourceData>();
			CopyCameraSortingLayerPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<CopyCameraSortingLayerPass.PassData>(CopyCameraSortingLayerPass.k_CopyCameraSortingLayerPass, out passData, CopyCameraSortingLayerPass.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\CopyCameraSortingLayerPass.cs", 65))
			{
				passData.source = universalResourceData.activeColorTexture;
				rasterRenderGraphBuilder.SetRenderAttachment(universal2DResourceData.cameraSortingLayerTexture, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.UseTexture(passData.source, AccessFlags.Read);
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<CopyCameraSortingLayerPass.PassData>(delegate(CopyCameraSortingLayerPass.PassData data, RasterGraphContext context)
				{
					CopyCameraSortingLayerPass.Execute(context.cmd, data.source);
				});
			}
		}

		private static readonly string k_CopyCameraSortingLayerPass = "CopyCameraSortingLayer Pass";

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(CopyCameraSortingLayerPass.k_CopyCameraSortingLayerPass);

		private static readonly ProfilingSampler m_ExecuteProfilingSampler = new ProfilingSampler("Copy");

		internal static readonly string k_CameraSortingLayerTexture = "_CameraSortingLayerTexture";

		internal static readonly int k_CameraSortingLayerTextureId = Shader.PropertyToID(CopyCameraSortingLayerPass.k_CameraSortingLayerTexture);

		private static Material m_BlitMaterial;

		private class PassData
		{
			internal TextureHandle source;
		}
	}
}
