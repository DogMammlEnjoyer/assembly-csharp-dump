using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class UpscalePass : ScriptableRenderPass
	{
		public UpscalePass(RenderPassEvent evt, Material blitMaterial)
		{
			base.renderPassEvent = evt;
			UpscalePass.m_BlitMaterial = blitMaterial;
		}

		public void Setup(RTHandle colorTargetHandle, int width, int height, FilterMode mode, RenderTextureDescriptor cameraTargetDescriptor, out RTHandle upscaleHandle)
		{
			this.source = colorTargetHandle;
			RenderTextureDescriptor renderTextureDescriptor = cameraTargetDescriptor;
			renderTextureDescriptor.width = width;
			renderTextureDescriptor.height = height;
			renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.destination, renderTextureDescriptor, mode, TextureWrapMode.Clamp, 1, 0f, "_UpscaleTexture");
			upscaleHandle = this.destination;
		}

		public void Dispose()
		{
			RTHandle rthandle = this.destination;
			if (rthandle == null)
			{
				return;
			}
			rthandle.Release();
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			commandBuffer.SetRenderTarget(this.destination);
			UpscalePass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), this.source);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, RTHandle source)
		{
			using (new ProfilingScope(cmd, UpscalePass.m_ExecuteProfilingSampler))
			{
				Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
				Blitter.BlitTexture(cmd, source, v, UpscalePass.m_BlitMaterial, (source.rt.filterMode == FilterMode.Bilinear) ? 1 : 0);
			}
		}

		public void Render(RenderGraph graph, Camera camera, in TextureHandle cameraColorAttachment, in TextureHandle upscaleHandle)
		{
			PixelPerfectCamera pixelPerfectCamera;
			camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
			if (pixelPerfectCamera == null || !pixelPerfectCamera.enabled || !pixelPerfectCamera.requiresUpscalePass)
			{
				return;
			}
			UpscalePass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<UpscalePass.PassData>(UpscalePass.k_UpscalePass, out passData, UpscalePass.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\UpscalePass.cs", 71))
			{
				passData.source = cameraColorAttachment;
				rasterRenderGraphBuilder.SetRenderAttachment(upscaleHandle, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.UseTexture(cameraColorAttachment, AccessFlags.Read);
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<UpscalePass.PassData>(delegate(UpscalePass.PassData data, RasterGraphContext context)
				{
					UpscalePass.ExecutePass(context.cmd, data.source);
				});
			}
		}

		private static readonly string k_UpscalePass = "Upscale2D Pass";

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(UpscalePass.k_UpscalePass);

		private static readonly ProfilingSampler m_ExecuteProfilingSampler = new ProfilingSampler("Draw Upscale");

		private static Material m_BlitMaterial;

		private RTHandle source;

		private RTHandle destination;

		private class PassData
		{
			internal TextureHandle source;
		}
	}
}
