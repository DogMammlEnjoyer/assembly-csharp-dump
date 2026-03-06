using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DrawScreenSpaceUIPass : ScriptableRenderPass
	{
		public DrawScreenSpaceUIPass(RenderPassEvent evt, bool renderOffscreen)
		{
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(URPProfileId.DrawScreenSpaceUI);
			base.renderPassEvent = evt;
			base.useNativeRenderPass = false;
			this.m_RenderOffscreen = renderOffscreen;
			this.m_PassData = new DrawScreenSpaceUIPass.PassData();
		}

		public static void ConfigureColorDescriptor(ref RenderTextureDescriptor descriptor, int cameraWidth, int cameraHeight)
		{
			descriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
			descriptor.depthStencilFormat = GraphicsFormat.None;
			descriptor.width = cameraWidth;
			descriptor.height = cameraHeight;
		}

		public static void ConfigureDepthDescriptor(ref RenderTextureDescriptor descriptor, GraphicsFormat depthStencilFormat, int cameraWidth, int cameraHeight)
		{
			descriptor.graphicsFormat = GraphicsFormat.None;
			descriptor.depthStencilFormat = depthStencilFormat;
			descriptor.width = cameraWidth;
			descriptor.height = cameraHeight;
		}

		private static void ExecutePass(RasterCommandBuffer commandBuffer, DrawScreenSpaceUIPass.PassData passData, RendererList rendererList)
		{
			commandBuffer.DrawRendererList(rendererList);
		}

		private static void ExecutePass(UnsafeCommandBuffer commandBuffer, DrawScreenSpaceUIPass.UnsafePassData passData, RendererList rendererList)
		{
			commandBuffer.DrawRendererList(rendererList);
		}

		public void Dispose()
		{
			RTHandle colorTarget = this.m_ColorTarget;
			if (colorTarget != null)
			{
				colorTarget.Release();
			}
			RTHandle depthTarget = this.m_DepthTarget;
			if (depthTarget == null)
			{
				return;
			}
			depthTarget.Release();
		}

		public void Setup(UniversalCameraData cameraData, GraphicsFormat depthStencilFormat)
		{
			if (this.m_RenderOffscreen)
			{
				RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
				DrawScreenSpaceUIPass.ConfigureColorDescriptor(ref cameraTargetDescriptor, cameraData.pixelWidth, cameraData.pixelHeight);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_ColorTarget, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_OverlayUITexture");
				RenderTextureDescriptor cameraTargetDescriptor2 = cameraData.cameraTargetDescriptor;
				DrawScreenSpaceUIPass.ConfigureDepthDescriptor(ref cameraTargetDescriptor2, depthStencilFormat, cameraData.pixelWidth, cameraData.pixelHeight);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_DepthTarget, cameraTargetDescriptor2, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_OverlayUITexture_Depth");
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			if (this.m_RenderOffscreen)
			{
				base.ConfigureTarget(this.m_ColorTarget, this.m_DepthTarget);
				base.ConfigureClear(ClearFlag.Color, Color.clear);
				if (cmd != null)
				{
					cmd.SetGlobalTexture(ShaderPropertyId.overlayUITexture, this.m_ColorTarget);
					return;
				}
			}
			else
			{
				UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
				DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
				if (activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget))
				{
					base.ConfigureTarget(*activeDebugHandler.DebugScreenColorHandle, *activeDebugHandler.DebugScreenDepthHandle);
					return;
				}
				RTHandleStaticHelpers.SetRTHandleStaticWrapper(RenderingUtils.GetCameraTargetIdentifier(ref renderingData));
				RTHandle s_RTHandleWrapper = RTHandleStaticHelpers.s_RTHandleWrapper;
				base.ConfigureTarget(s_RTHandleWrapper);
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			using (new ProfilingScope(*renderingData.commandBuffer, base.profilingSampler))
			{
				RendererList rendererList = context.CreateUIOverlayRendererList(*renderingData.cameraData.camera);
				DrawScreenSpaceUIPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, rendererList);
			}
		}

		internal void RenderOffscreen(RenderGraph renderGraph, ContextContainer frameData, GraphicsFormat depthStencilFormat, out TextureHandle output)
		{
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
			DrawScreenSpaceUIPass.ConfigureColorDescriptor(ref cameraTargetDescriptor, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
			output = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_OverlayUITexture", true, FilterMode.Point, TextureWrapMode.Clamp);
			RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
			DrawScreenSpaceUIPass.ConfigureDepthDescriptor(ref cameraTargetDescriptor2, depthStencilFormat, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
			TextureHandle tex = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor2, "_OverlayUITexture_Depth", false, FilterMode.Point, TextureWrapMode.Clamp);
			DrawScreenSpaceUIPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DrawScreenSpaceUIPass.PassData>("Draw Screen Space UIToolkit/uGUI - Offscreen", out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawScreenSpaceUIPass.cs", 181))
			{
				rasterRenderGraphBuilder.UseAllGlobalTextures(true);
				rasterRenderGraphBuilder.SetRenderAttachment(output, 0, AccessFlags.Write);
				DrawScreenSpaceUIPass.PassData passData2 = passData;
				UniversalCameraData universalCameraData2 = universalCameraData;
				UISubset uisubset = UISubset.UIToolkit_UGUI;
				passData2.rendererList = renderGraph.CreateUIOverlayRendererList(universalCameraData2.camera, uisubset);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(tex, AccessFlags.ReadWrite);
				if (output.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(output, ShaderPropertyId.overlayUITexture);
				}
				rasterRenderGraphBuilder.SetRenderFunc<DrawScreenSpaceUIPass.PassData>(delegate(DrawScreenSpaceUIPass.PassData data, RasterGraphContext context)
				{
					DrawScreenSpaceUIPass.ExecutePass(context.cmd, data, data.rendererList);
				});
			}
			DrawScreenSpaceUIPass.UnsafePassData unsafePassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<DrawScreenSpaceUIPass.UnsafePassData>("Draw Screen Space IMGUI/SoftwareCursor - Offscreen", out unsafePassData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawScreenSpaceUIPass.cs", 205))
			{
				unsafePassData.colorTarget = output;
				unsafeRenderGraphBuilder.UseTexture(output, AccessFlags.Write);
				DrawScreenSpaceUIPass.UnsafePassData unsafePassData2 = unsafePassData;
				UniversalCameraData universalCameraData3 = universalCameraData;
				UISubset uisubset = UISubset.LowLevel;
				unsafePassData2.rendererList = renderGraph.CreateUIOverlayRendererList(universalCameraData3.camera, uisubset);
				unsafeRenderGraphBuilder.UseRendererList(unsafePassData.rendererList);
				unsafeRenderGraphBuilder.SetRenderFunc<DrawScreenSpaceUIPass.UnsafePassData>(delegate(DrawScreenSpaceUIPass.UnsafePassData data, UnsafeGraphContext context)
				{
					context.cmd.SetRenderTarget(data.colorTarget);
					DrawScreenSpaceUIPass.ExecutePass(context.cmd, data, data.rendererList);
				});
			}
		}

		internal void RenderOverlay(RenderGraph renderGraph, ContextContainer frameData, in TextureHandle colorBuffer, in TextureHandle depthBuffer)
		{
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			frameData.Get<UniversalResourceData>();
			ScriptableRenderer renderer = universalCameraData.renderer;
			DrawScreenSpaceUIPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DrawScreenSpaceUIPass.PassData>("Draw UIToolkit/uGUI Overlay", out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawScreenSpaceUIPass.cs", 228))
			{
				rasterRenderGraphBuilder.UseAllGlobalTextures(true);
				rasterRenderGraphBuilder.SetRenderAttachment(colorBuffer, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(depthBuffer, AccessFlags.ReadWrite);
				DrawScreenSpaceUIPass.PassData passData2 = passData;
				UniversalCameraData universalCameraData2 = universalCameraData;
				UISubset uisubset = UISubset.UIToolkit_UGUI;
				passData2.rendererList = renderGraph.CreateUIOverlayRendererList(universalCameraData2.camera, uisubset);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				rasterRenderGraphBuilder.SetRenderFunc<DrawScreenSpaceUIPass.PassData>(delegate(DrawScreenSpaceUIPass.PassData data, RasterGraphContext context)
				{
					DrawScreenSpaceUIPass.ExecutePass(context.cmd, data, data.rendererList);
				});
			}
			DrawScreenSpaceUIPass.UnsafePassData unsafePassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<DrawScreenSpaceUIPass.UnsafePassData>("Draw IMGUI/SoftwareCursor Overlay", out unsafePassData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawScreenSpaceUIPass.cs", 248))
			{
				unsafePassData.colorTarget = colorBuffer;
				unsafeRenderGraphBuilder.UseTexture(colorBuffer, AccessFlags.Write);
				DrawScreenSpaceUIPass.UnsafePassData unsafePassData2 = unsafePassData;
				UniversalCameraData universalCameraData3 = universalCameraData;
				UISubset uisubset = UISubset.LowLevel;
				unsafePassData2.rendererList = renderGraph.CreateUIOverlayRendererList(universalCameraData3.camera, uisubset);
				unsafeRenderGraphBuilder.UseRendererList(unsafePassData.rendererList);
				unsafeRenderGraphBuilder.SetRenderFunc<DrawScreenSpaceUIPass.UnsafePassData>(delegate(DrawScreenSpaceUIPass.UnsafePassData data, UnsafeGraphContext context)
				{
					context.cmd.SetRenderTarget(data.colorTarget);
					DrawScreenSpaceUIPass.ExecutePass(context.cmd, data, data.rendererList);
				});
			}
		}

		private DrawScreenSpaceUIPass.PassData m_PassData;

		private RTHandle m_ColorTarget;

		private RTHandle m_DepthTarget;

		private bool m_RenderOffscreen;

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private static readonly int s_CameraOpaqueTextureID = Shader.PropertyToID("_CameraOpaqueTexture");

		private class PassData
		{
			internal RendererListHandle rendererList;
		}

		private class UnsafePassData
		{
			internal RendererListHandle rendererList;

			internal TextureHandle colorTarget;
		}
	}
}
