using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public class XROcclusionMeshPass : ScriptableRenderPass
	{
		public XROcclusionMeshPass(RenderPassEvent evt)
		{
			base.profilingSampler = new ProfilingSampler("Draw XR Occlusion Mesh");
			base.renderPassEvent = evt;
			this.m_PassData = new XROcclusionMeshPass.PassData();
			this.m_IsActiveTargetBackBuffer = false;
		}

		private static void ExecutePass(RasterCommandBuffer cmd, XROcclusionMeshPass.PassData data)
		{
			if (data.xr.hasValidOcclusionMesh)
			{
				if (data.isActiveTargetBackBuffer)
				{
					cmd.SetViewport(data.xr.GetViewport(0));
				}
				data.xr.RenderOcclusionMesh(cmd, !data.isActiveTargetBackBuffer);
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			this.m_PassData.xr = renderingData.cameraData.xr;
			this.m_PassData.isActiveTargetBackBuffer = this.m_IsActiveTargetBackBuffer;
			XROcclusionMeshPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData);
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, in TextureHandle cameraColorAttachment, in TextureHandle cameraDepthAttachment)
		{
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			XROcclusionMeshPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<XROcclusionMeshPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\XROcclusionMeshPass.cs", 61))
			{
				passData.xr = universalCameraData.xr;
				passData.cameraColorAttachment = cameraColorAttachment;
				rasterRenderGraphBuilder.SetRenderAttachment(cameraColorAttachment, 0, AccessFlags.Write);
				passData.cameraDepthAttachment = cameraDepthAttachment;
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(cameraDepthAttachment, AccessFlags.Write);
				passData.isActiveTargetBackBuffer = universalResourceData.isActiveTargetBackBuffer;
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (universalCameraData.xr.enabled)
				{
					bool flag = universalCameraData.xrUniversal.canFoveateIntermediatePasses || universalResourceData.isActiveTargetBackBuffer;
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && flag);
				}
				rasterRenderGraphBuilder.SetRenderFunc<XROcclusionMeshPass.PassData>(delegate(XROcclusionMeshPass.PassData data, RasterGraphContext context)
				{
					XROcclusionMeshPass.ExecutePass(context.cmd, data);
				});
			}
		}

		private XROcclusionMeshPass.PassData m_PassData;

		public bool m_IsActiveTargetBackBuffer;

		private class PassData
		{
			internal XRPass xr;

			internal TextureHandle cameraColorAttachment;

			internal TextureHandle cameraDepthAttachment;

			internal bool isActiveTargetBackBuffer;
		}
	}
}
