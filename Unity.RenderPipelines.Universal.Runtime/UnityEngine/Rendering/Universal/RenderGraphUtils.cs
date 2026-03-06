using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal static class RenderGraphUtils
	{
		internal static void UseDBufferIfValid(IRasterRenderGraphBuilder builder, UniversalResourceData resourceData)
		{
			TextureHandle[] dBuffer = resourceData.dBuffer;
			for (int i = 0; i < 3; i++)
			{
				TextureHandle textureHandle = dBuffer[i];
				if (textureHandle.IsValid())
				{
					builder.UseTexture(textureHandle, AccessFlags.Read);
				}
			}
		}

		public static void SetGlobalTexture(RenderGraph graph, int nameId, TextureHandle handle, string passName = "Set Global Texture", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			RenderGraphUtils.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<RenderGraphUtils.PassData>(passName, out passData, RenderGraphUtils.s_SetGlobalTextureProfilingSampler, file, line))
			{
				passData.nameID = nameId;
				passData.texture = handle;
				rasterRenderGraphBuilder.UseTexture(handle, AccessFlags.Read);
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetGlobalTextureAfterPass(handle, nameId);
				rasterRenderGraphBuilder.SetRenderFunc<RenderGraphUtils.PassData>(delegate(RenderGraphUtils.PassData data, RasterGraphContext context)
				{
				});
			}
		}

		private static ProfilingSampler s_SetGlobalTextureProfilingSampler = new ProfilingSampler("Set Global Texture");

		internal const int GBufferSize = 7;

		internal const int DBufferSize = 3;

		internal const int LightTextureSize = 4;

		private class PassData
		{
			internal TextureHandle texture;

			internal int nameID;
		}
	}
}
