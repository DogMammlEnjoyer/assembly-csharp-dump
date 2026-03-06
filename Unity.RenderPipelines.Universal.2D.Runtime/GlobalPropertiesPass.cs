using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class GlobalPropertiesPass : ScriptableRenderPass
	{
		internal static void Setup(RenderGraph graph, ContextContainer frameData, Renderer2DData rendererData, UniversalCameraData cameraData, bool useLights)
		{
			Universal2DResourceData universal2DResourceData = frameData.Get<Universal2DResourceData>();
			GlobalPropertiesPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<GlobalPropertiesPass.PassData>(GlobalPropertiesPass.k_SetGlobalProperties, out passData, GlobalPropertiesPass.m_SetGlobalPropertiesProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\GlobalPropertiesPass.cs", 19))
			{
				passData.screenParams = Vector2Int.zero;
				PixelPerfectCamera pixelPerfectCamera;
				cameraData.camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
				if (pixelPerfectCamera != null && pixelPerfectCamera.enabled && pixelPerfectCamera.offscreenRTSize != Vector2Int.zero)
				{
					passData.screenParams = pixelPerfectCamera.offscreenRTSize;
				}
				if (useLights)
				{
					TextureHandle textureHandle = graph.ImportTexture(Light2DLookupTexture.GetLightLookupTexture_Rendergraph());
					TextureHandle textureHandle2 = graph.ImportTexture(Light2DLookupTexture.GetFallOffLookupTexture_Rendergraph());
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(textureHandle, Light2DLookupTexture.k_LightLookupID);
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(textureHandle2, Light2DLookupTexture.k_FalloffLookupID);
				}
				if (rendererData.useCameraSortingLayerTexture)
				{
					IBaseRenderGraphBuilder baseRenderGraphBuilder = rasterRenderGraphBuilder;
					TextureHandle cameraSortingLayerTexture = universal2DResourceData.cameraSortingLayerTexture;
					baseRenderGraphBuilder.SetGlobalTextureAfterPass(cameraSortingLayerTexture, CopyCameraSortingLayerPass.k_CameraSortingLayerTextureId);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<GlobalPropertiesPass.PassData>(delegate(GlobalPropertiesPass.PassData data, RasterGraphContext context)
				{
					if (data.screenParams != Vector2Int.zero)
					{
						int x = data.screenParams.x;
						int y = data.screenParams.y;
						context.cmd.SetGlobalVector(ShaderPropertyId.screenParams, new Vector4((float)x, (float)y, 1f + 1f / (float)x, 1f + 1f / (float)y));
					}
				});
			}
		}

		private static readonly string k_SetGlobalProperties = "SetGlobalProperties";

		private static readonly ProfilingSampler m_SetGlobalPropertiesProfilingSampler = new ProfilingSampler(GlobalPropertiesPass.k_SetGlobalProperties);

		private class PassData
		{
			internal Vector2Int screenParams;
		}
	}
}
