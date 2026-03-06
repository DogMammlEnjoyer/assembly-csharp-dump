using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	internal class DeferredPass : ScriptableRenderPass
	{
		public DeferredPass(RenderPassEvent evt, DeferredLights deferredLights)
		{
			base.profilingSampler = new ProfilingSampler("Render Deferred Lighting");
			base.renderPassEvent = evt;
			this.m_DeferredLights = deferredLights;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescripor)
		{
			RTHandle colorAttachment = this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GBufferLightingIndex];
			RTHandle depthAttachmentHandle = this.m_DeferredLights.DepthAttachmentHandle;
			if (this.m_DeferredLights.UseFramebufferFetch)
			{
				base.ConfigureInputAttachments(this.m_DeferredLights.DeferredInputAttachments, this.m_DeferredLights.DeferredInputIsTransient);
			}
			base.ConfigureTarget(colorAttachment, depthAttachmentHandle);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			this.m_DeferredLights.ExecuteDeferredPass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), cameraData, lightData, shadowData);
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle color, TextureHandle depth, TextureHandle[] gbuffer)
		{
			frameData.Get<UniversalResourceData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			DeferredPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DeferredPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DeferredPass.cs", 82))
			{
				passData.cameraData = cameraData;
				passData.lightData = lightData;
				passData.shadowData = shadowData;
				passData.color = color;
				rasterRenderGraphBuilder.SetRenderAttachment(color, 0, AccessFlags.Write);
				passData.depth = depth;
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(depth, AccessFlags.Write);
				passData.deferredLights = this.m_DeferredLights;
				if (!this.m_DeferredLights.UseFramebufferFetch)
				{
					for (int i = 0; i < gbuffer.Length; i++)
					{
						if (i != this.m_DeferredLights.GBufferLightingIndex)
						{
							rasterRenderGraphBuilder.UseTexture(gbuffer[i], AccessFlags.Read);
						}
					}
				}
				else
				{
					int num = 0;
					for (int j = 0; j < gbuffer.Length; j++)
					{
						if (j != this.m_DeferredLights.GBufferLightingIndex)
						{
							rasterRenderGraphBuilder.SetInputAttachment(gbuffer[j], num, AccessFlags.Read);
							num++;
						}
					}
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<DeferredPass.PassData>(delegate(DeferredPass.PassData data, RasterGraphContext context)
				{
					data.deferredLights.ExecuteDeferredPass(context.cmd, data.cameraData, data.lightData, data.shadowData);
				});
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			this.m_DeferredLights.OnCameraCleanup(cmd);
		}

		private DeferredLights m_DeferredLights;

		private class PassData
		{
			internal UniversalCameraData cameraData;

			internal UniversalLightData lightData;

			internal UniversalShadowData shadowData;

			internal TextureHandle color;

			internal TextureHandle depth;

			internal TextureHandle[] gbuffer;

			internal DeferredLights deferredLights;
		}
	}
}
