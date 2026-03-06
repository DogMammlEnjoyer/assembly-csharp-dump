using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Drawing
{
	public class AlineURPRenderPassFeature : ScriptableRendererFeature
	{
		public override void Create()
		{
			this.m_ScriptablePass = new AlineURPRenderPassFeature.AlineURPRenderPass();
			this.m_ScriptablePass.renderPassEvent = (RenderPassEvent)549;
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			this.AddRenderPasses(renderer);
		}

		public void AddRenderPasses(ScriptableRenderer renderer)
		{
			renderer.EnqueuePass(this.m_ScriptablePass);
		}

		private AlineURPRenderPassFeature.AlineURPRenderPass m_ScriptablePass;

		public class AlineURPRenderPass : ScriptableRenderPass
		{
			[Obsolete]
			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
			}

			[Obsolete]
			public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				DrawingManager.instance.ExecuteCustomRenderPass(context, *renderingData.cameraData.camera);
			}

			public AlineURPRenderPass()
			{
				base.profilingSampler = new ProfilingSampler("ALINE");
			}

			public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
			{
				UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				AlineURPRenderPassFeature.AlineURPRenderPass.PassData passData;
				using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<AlineURPRenderPassFeature.AlineURPRenderPass.PassData>("ALINE", out passData, base.profilingSampler, "C:\\Users\\root\\GT\\Assets\\ALINE\\AlineURPRenderPassFeature.cs", 41))
				{
					bool allowDisablingWireframe = false;
					if (Application.isEditor && (universalCameraData.cameraType & (CameraType.SceneView | CameraType.Preview)) != (CameraType)0)
					{
						rasterRenderGraphBuilder.AllowGlobalStateModification(true);
						allowDisablingWireframe = true;
					}
					rasterRenderGraphBuilder.SetRenderAttachment(universalResourceData.activeColorTexture, 0, AccessFlags.Write);
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Write);
					passData.camera = universalCameraData.camera;
					rasterRenderGraphBuilder.SetRenderFunc<AlineURPRenderPassFeature.AlineURPRenderPass.PassData>(delegate(AlineURPRenderPassFeature.AlineURPRenderPass.PassData data, RasterGraphContext context)
					{
						DrawingManager.instance.ExecuteCustomRenderGraphPass(new DrawingData.CommandBufferWrapper
						{
							cmd2 = context.cmd,
							allowDisablingWireframe = allowDisablingWireframe
						}, data.camera);
					});
				}
			}

			public override void FrameCleanup(CommandBuffer cmd)
			{
			}

			private class PassData
			{
				public Camera camera;
			}
		}
	}
}
