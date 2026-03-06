using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	internal class DrawObjectsWithRenderingLayersPass : DrawObjectsPass
	{
		public DrawObjectsWithRenderingLayersPass(URPProfileId profilerTag, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference) : base(profilerTag, opaque, evt, renderQueueRange, layerMask, stencilState, stencilReference)
		{
			this.m_ColorTargetIndentifiers = new RTHandle[2];
		}

		public void Setup(RTHandle colorAttachment, RTHandle renderingLayersTexture, RTHandle depthAttachment)
		{
			if (colorAttachment == null)
			{
				throw new ArgumentException("Color attachment can not be null", "colorAttachment");
			}
			if (renderingLayersTexture == null)
			{
				throw new ArgumentException("Rendering layers attachment can not be null", "renderingLayersTexture");
			}
			if (depthAttachment == null)
			{
				throw new ArgumentException("Depth attachment can not be null", "depthAttachment");
			}
			this.m_ColorTargetIndentifiers[0] = colorAttachment;
			this.m_ColorTargetIndentifiers[1] = renderingLayersTexture;
			this.m_DepthTargetIndentifiers = depthAttachment;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			base.ConfigureTarget(this.m_ColorTargetIndentifiers, this.m_DepthTargetIndentifiers);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			commandBuffer.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, true);
			base.Execute(context, ref renderingData);
			commandBuffer.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, false);
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle colorTarget, TextureHandle renderingLayersTexture, TextureHandle depthTarget, TextureHandle mainShadowsTexture, TextureHandle additionalShadowsTexture, RenderingLayerUtils.MaskSize maskSize, uint batchLayerMask = 4294967295U)
		{
			DrawObjectsWithRenderingLayersPass.RenderingLayersPassData renderingLayersPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DrawObjectsWithRenderingLayersPass.RenderingLayersPassData>(base.passName, out renderingLayersPassData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawObjectsPass.cs", 411))
			{
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
				UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
				UniversalLightData lightData = frameData.Get<UniversalLightData>();
				base.InitPassData(universalCameraData, ref renderingLayersPassData.basePassData, batchLayerMask, false);
				renderingLayersPassData.maskSize = maskSize;
				renderingLayersPassData.basePassData.albedoHdl = colorTarget;
				rasterRenderGraphBuilder.SetRenderAttachment(colorTarget, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachment(renderingLayersTexture, 1, AccessFlags.Write);
				renderingLayersPassData.basePassData.depthHdl = depthTarget;
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(depthTarget, AccessFlags.Write);
				if (mainShadowsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(mainShadowsTexture, AccessFlags.Read);
				}
				if (additionalShadowsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(additionalShadowsTexture, AccessFlags.Read);
				}
				if (universalCameraData.renderer is UniversalRenderer)
				{
					TextureHandle ssaoTexture = universalResourceData.ssaoTexture;
					if (ssaoTexture.IsValid())
					{
						rasterRenderGraphBuilder.UseTexture(ssaoTexture, AccessFlags.Read);
					}
					RenderGraphUtils.UseDBufferIfValid(rasterRenderGraphBuilder, universalResourceData);
				}
				base.InitRendererLists(renderingData, universalCameraData, lightData, ref renderingLayersPassData.basePassData, default(ScriptableRenderContext), renderGraph, true);
				if (ScriptableRenderPass.GetActiveDebugHandler(universalCameraData) != null)
				{
					renderingLayersPassData.basePassData.debugRendererLists.PrepareRendererListForRasterPass(rasterRenderGraphBuilder);
				}
				else
				{
					rasterRenderGraphBuilder.UseRendererList(renderingLayersPassData.basePassData.rendererListHdl);
					rasterRenderGraphBuilder.UseRendererList(renderingLayersPassData.basePassData.objectsWithErrorRendererListHdl);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (universalCameraData.xr.enabled)
				{
					bool flag = universalCameraData.xrUniversal.canFoveateIntermediatePasses || universalResourceData.isActiveTargetBackBuffer;
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && flag);
				}
				rasterRenderGraphBuilder.SetRenderFunc<DrawObjectsWithRenderingLayersPass.RenderingLayersPassData>(delegate(DrawObjectsWithRenderingLayersPass.RenderingLayersPassData data, RasterGraphContext context)
				{
					context.cmd.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, true);
					RenderingLayerUtils.SetupProperties(context.cmd, data.maskSize);
					if (!data.basePassData.isOpaque && !data.basePassData.shouldTransparentsReceiveShadows)
					{
						TransparentSettingsPass.ExecutePass(context.cmd);
					}
					bool yFlip = data.basePassData.cameraData.IsRenderTargetProjectionMatrixFlipped(data.basePassData.albedoHdl, data.basePassData.depthHdl);
					DrawObjectsPass.ExecutePass(context.cmd, data.basePassData, data.basePassData.rendererListHdl, data.basePassData.objectsWithErrorRendererListHdl, yFlip);
					context.cmd.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, false);
				});
			}
		}

		private RTHandle[] m_ColorTargetIndentifiers;

		private RTHandle m_DepthTargetIndentifiers;

		private class RenderingLayersPassData
		{
			public RenderingLayersPassData()
			{
				this.basePassData = new DrawObjectsPass.PassData();
			}

			internal DrawObjectsPass.PassData basePassData;

			internal RenderingLayerUtils.MaskSize maskSize;
		}
	}
}
