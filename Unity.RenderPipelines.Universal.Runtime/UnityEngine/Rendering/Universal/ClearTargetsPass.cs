using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class ClearTargetsPass
	{
		internal static void Render(RenderGraph graph, TextureHandle colorHandle, TextureHandle depthHandle, UniversalCameraData cameraData)
		{
			RTClearFlags rtclearFlags = RTClearFlags.None;
			if (cameraData.renderType == CameraRenderType.Base)
			{
				rtclearFlags = RTClearFlags.All;
			}
			else if (cameraData.clearDepth)
			{
				rtclearFlags = RTClearFlags.Depth;
			}
			if (rtclearFlags != RTClearFlags.None)
			{
				ClearTargetsPass.Render(graph, colorHandle, depthHandle, rtclearFlags, cameraData.backgroundColor);
			}
		}

		internal static void Render(RenderGraph graph, TextureHandle colorHandle, TextureHandle depthHandle, RTClearFlags clearFlags, Color clearColor)
		{
			ClearTargetsPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<ClearTargetsPass.PassData>("Clear Targets Pass", out passData, ClearTargetsPass.s_ClearProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\UniversalRendererRenderGraph.cs", 2048))
			{
				if (colorHandle.IsValid())
				{
					passData.color = colorHandle;
					rasterRenderGraphBuilder.SetRenderAttachment(colorHandle, 0, AccessFlags.Write);
				}
				if (depthHandle.IsValid())
				{
					passData.depth = depthHandle;
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(depthHandle, AccessFlags.Write);
				}
				passData.clearFlags = clearFlags;
				passData.clearColor = clearColor;
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<ClearTargetsPass.PassData>(delegate(ClearTargetsPass.PassData data, RasterGraphContext context)
				{
					context.cmd.ClearRenderTarget(data.clearFlags, data.clearColor, 1f, 0U);
				});
			}
		}

		private static ProfilingSampler s_ClearProfilingSampler = new ProfilingSampler("Clear Targets");

		private class PassData
		{
			internal TextureHandle color;

			internal TextureHandle depth;

			internal RTClearFlags clearFlags;

			internal Color clearColor;
		}
	}
}
