using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public class DrawSkyboxPass : ScriptableRenderPass
	{
		public DrawSkyboxPass(RenderPassEvent evt)
		{
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(URPProfileId.DrawSkybox);
			base.renderPassEvent = evt;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			if (activeDebugHandler != null && activeDebugHandler.IsScreenClearNeeded)
			{
				return;
			}
			RendererList rendererList = this.CreateSkyboxRendererList(context, universalCameraData);
			DrawSkyboxPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), universalCameraData.xr, rendererList);
		}

		private RendererList CreateSkyboxRendererList(ScriptableRenderContext context, UniversalCameraData cameraData)
		{
			RendererList result = default(RendererList);
			if (cameraData.xr.enabled)
			{
				if (cameraData.xr.singlePassEnabled)
				{
					result = context.CreateSkyboxRendererList(cameraData.camera, cameraData.GetProjectionMatrix(0), cameraData.GetViewMatrix(0), cameraData.GetProjectionMatrix(1), cameraData.GetViewMatrix(1));
				}
				else
				{
					result = context.CreateSkyboxRendererList(cameraData.camera, cameraData.GetProjectionMatrix(0), cameraData.GetViewMatrix(0));
				}
			}
			else
			{
				result = context.CreateSkyboxRendererList(cameraData.camera);
			}
			return result;
		}

		private RendererListHandle CreateSkyBoxRendererList(RenderGraph renderGraph, UniversalCameraData cameraData)
		{
			RendererListHandle result = default(RendererListHandle);
			if (cameraData.xr.enabled)
			{
				if (cameraData.xr.singlePassEnabled)
				{
					result = renderGraph.CreateSkyboxRendererList(cameraData.camera, cameraData.GetProjectionMatrix(0), cameraData.GetViewMatrix(0), cameraData.GetProjectionMatrix(1), cameraData.GetViewMatrix(1));
				}
				else
				{
					result = renderGraph.CreateSkyboxRendererList(cameraData.camera, cameraData.GetProjectionMatrix(0), cameraData.GetViewMatrix(0));
				}
			}
			else
			{
				result = renderGraph.CreateSkyboxRendererList(cameraData.camera);
			}
			return result;
		}

		private static void ExecutePass(RasterCommandBuffer cmd, XRPass xr, RendererList rendererList)
		{
			if (xr.enabled && xr.singlePassEnabled)
			{
				cmd.SetSinglePassStereo(SystemInfo.supportsMultiview ? SinglePassStereoMode.Multiview : SinglePassStereoMode.Instancing);
			}
			cmd.DrawRendererList(rendererList);
			if (xr.enabled && xr.singlePassEnabled)
			{
				cmd.SetSinglePassStereo(SinglePassStereoMode.None);
			}
		}

		private void InitPassData(ref DrawSkyboxPass.PassData passData, in XRPass xr, in RendererListHandle handle)
		{
			passData.xr = xr;
			passData.skyRendererListHandle = handle;
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, ScriptableRenderContext context, TextureHandle colorTarget, TextureHandle depthTarget, Material skyboxMaterial)
		{
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			if (activeDebugHandler != null && activeDebugHandler.IsScreenClearNeeded)
			{
				return;
			}
			DrawSkyboxPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DrawSkyboxPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawSkyboxPass.cs", 148))
			{
				RendererListHandle rendererListHandle = this.CreateSkyBoxRendererList(renderGraph, universalCameraData);
				XRPass xr = universalCameraData.xr;
				this.InitPassData(ref passData, xr, rendererListHandle);
				passData.material = skyboxMaterial;
				rasterRenderGraphBuilder.UseRendererList(rendererListHandle);
				rasterRenderGraphBuilder.SetRenderAttachment(colorTarget, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(depthTarget, AccessFlags.Write);
				rasterRenderGraphBuilder.AllowPassCulling(false);
				if (universalCameraData.xr.enabled)
				{
					bool flag = universalCameraData.xrUniversal.canFoveateIntermediatePasses || universalResourceData.isActiveTargetBackBuffer;
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && flag);
				}
				rasterRenderGraphBuilder.SetRenderFunc<DrawSkyboxPass.PassData>(delegate(DrawSkyboxPass.PassData data, RasterGraphContext context)
				{
					DrawSkyboxPass.ExecutePass(context.cmd, data.xr, data.skyRendererListHandle);
				});
			}
		}

		private class PassData
		{
			internal XRPass xr;

			internal RendererListHandle skyRendererListHandle;

			internal Material material;
		}
	}
}
