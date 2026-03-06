using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class DepthNormalOnlyPass : ScriptableRenderPass
	{
		internal List<ShaderTagId> shaderTagIds { get; set; }

		private RTHandle depthHandle { get; set; }

		private RTHandle normalHandle { get; set; }

		private RTHandle renderingLayersHandle { get; set; }

		internal bool enableRenderingLayers { get; set; }

		internal RenderingLayerUtils.MaskSize renderingLayersMaskSize { get; set; }

		public DepthNormalOnlyPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
		{
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(URPProfileId.DrawDepthNormalPrepass);
			this.m_PassData = new DepthNormalOnlyPass.PassData();
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(renderQueueRange), layerMask, uint.MaxValue, 0);
			base.renderPassEvent = evt;
			base.useNativeRenderPass = false;
			this.shaderTagIds = DepthNormalOnlyPass.k_DepthNormals;
		}

		public static GraphicsFormat GetGraphicsFormat()
		{
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R8G8B8A8_SNorm, GraphicsFormatUsage.Render))
			{
				return GraphicsFormat.R8G8B8A8_SNorm;
			}
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.Render))
			{
				return GraphicsFormat.R16G16B16A16_SFloat;
			}
			return GraphicsFormat.R32G32B32A32_SFloat;
		}

		public void Setup(RTHandle depthHandle, RTHandle normalHandle)
		{
			this.depthHandle = depthHandle;
			this.normalHandle = normalHandle;
			this.enableRenderingLayers = false;
		}

		public void Setup(RTHandle depthHandle, RTHandle normalHandle, RTHandle decalLayerHandle)
		{
			this.Setup(depthHandle, normalHandle);
			this.renderingLayersHandle = decalLayerHandle;
			this.enableRenderingLayers = true;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RTHandle[] colorAttachments;
			if (this.enableRenderingLayers)
			{
				DepthNormalOnlyPass.k_ColorAttachment2[0] = this.normalHandle;
				DepthNormalOnlyPass.k_ColorAttachment2[1] = this.renderingLayersHandle;
				colorAttachments = DepthNormalOnlyPass.k_ColorAttachment2;
			}
			else
			{
				DepthNormalOnlyPass.k_ColorAttachment1[0] = this.normalHandle;
				colorAttachments = DepthNormalOnlyPass.k_ColorAttachment1;
			}
			if (renderingData.cameraData.renderer->useDepthPriming && (*renderingData.cameraData.renderType == CameraRenderType.Base || *renderingData.cameraData.clearDepth))
			{
				base.ConfigureTarget(colorAttachments, renderingData.cameraData.renderer->cameraDepthTargetHandle);
			}
			else
			{
				base.ConfigureTarget(colorAttachments, this.depthHandle);
			}
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, DepthNormalOnlyPass.PassData passData, RendererList rendererList)
		{
			if (passData.enableRenderingLayers)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, true);
			}
			cmd.DrawRendererList(rendererList);
			if (passData.enableRenderingLayers)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.WriteRenderingLayers, false);
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			this.m_PassData.enableRenderingLayers = this.enableRenderingLayers;
			RendererListParams rendererListParams = this.InitRendererListParams(renderingData2, cameraData, lightData);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer);
			using (new ProfilingScope(rasterCommandBuffer, base.profilingSampler))
			{
				DepthNormalOnlyPass.ExecutePass(rasterCommandBuffer, this.m_PassData, rendererList);
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}
			this.normalHandle = null;
			this.depthHandle = null;
			this.renderingLayersHandle = null;
			this.shaderTagIds = DepthNormalOnlyPass.k_DepthNormals;
		}

		private RendererListParams InitRendererListParams(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			SortingCriteria defaultOpaqueSortFlags = cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.shaderTagIds, renderingData, cameraData, lightData, defaultOpaqueSortFlags);
			drawSettings.perObjectData = PerObjectData.None;
			return new RendererListParams(renderingData.cullResults, drawSettings, this.m_FilteringSettings);
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle cameraNormalsTexture, TextureHandle cameraDepthTexture, TextureHandle renderingLayersTexture, uint batchLayerMask, bool setGlobalDepth, bool setGlobalTextures)
		{
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			DepthNormalOnlyPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DepthNormalOnlyPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DepthNormalOnlyPass.cs", 196))
			{
				passData.cameraNormalsTexture = cameraNormalsTexture;
				rasterRenderGraphBuilder.SetRenderAttachment(cameraNormalsTexture, 0, AccessFlags.Write);
				passData.cameraDepthTexture = cameraDepthTexture;
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(cameraDepthTexture, AccessFlags.Write);
				passData.enableRenderingLayers = this.enableRenderingLayers;
				if (passData.enableRenderingLayers)
				{
					rasterRenderGraphBuilder.SetRenderAttachment(renderingLayersTexture, 1, AccessFlags.Write);
					passData.maskSize = this.renderingLayersMaskSize;
				}
				RendererListParams rendererListParams = this.InitRendererListParams(renderingData, universalCameraData, lightData);
				rendererListParams.filteringSettings.batchLayerMask = batchLayerMask;
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				if (universalCameraData.xr.enabled)
				{
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && universalCameraData.xrUniversal.canFoveateIntermediatePasses);
				}
				if (setGlobalTextures)
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(cameraNormalsTexture, DepthNormalOnlyPass.s_CameraNormalsTextureID);
					if (passData.enableRenderingLayers)
					{
						rasterRenderGraphBuilder.SetGlobalTextureAfterPass(renderingLayersTexture, DepthNormalOnlyPass.s_CameraRenderingLayersTextureID);
					}
				}
				if (setGlobalDepth)
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(cameraDepthTexture, DepthNormalOnlyPass.s_CameraDepthTextureID);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<DepthNormalOnlyPass.PassData>(delegate(DepthNormalOnlyPass.PassData data, RasterGraphContext context)
				{
					RenderingLayerUtils.SetupProperties(context.cmd, data.maskSize);
					DepthNormalOnlyPass.ExecutePass(context.cmd, data, data.rendererList);
				});
			}
		}

		private FilteringSettings m_FilteringSettings;

		private DepthNormalOnlyPass.PassData m_PassData;

		private static readonly List<ShaderTagId> k_DepthNormals = new List<ShaderTagId>
		{
			new ShaderTagId("DepthNormals"),
			new ShaderTagId("DepthNormalsOnly")
		};

		private static readonly RTHandle[] k_ColorAttachment1 = new RTHandle[1];

		private static readonly RTHandle[] k_ColorAttachment2 = new RTHandle[2];

		internal static readonly string k_CameraNormalsTextureName = "_CameraNormalsTexture";

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private static readonly int s_CameraNormalsTextureID = Shader.PropertyToID(DepthNormalOnlyPass.k_CameraNormalsTextureName);

		private static readonly int s_CameraRenderingLayersTextureID = Shader.PropertyToID("_CameraRenderingLayersTexture");

		private class PassData
		{
			internal TextureHandle cameraDepthTexture;

			internal TextureHandle cameraNormalsTexture;

			internal bool enableRenderingLayers;

			internal RenderingLayerUtils.MaskSize maskSize;

			internal RendererListHandle rendererList;
		}
	}
}
