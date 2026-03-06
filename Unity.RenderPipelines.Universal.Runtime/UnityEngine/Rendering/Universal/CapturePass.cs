using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class CapturePass : ScriptableRenderPass
	{
		public CapturePass(RenderPassEvent evt)
		{
			base.profilingSampler = new ProfilingSampler("Capture Camera output");
			base.renderPassEvent = evt;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = *renderingData.commandBuffer;
			this.m_CameraColorHandle = renderingData.cameraData.renderer->GetCameraColorBackBuffer(cmd);
			using (new ProfilingScope(cmd, base.profilingSampler))
			{
				RenderTargetIdentifier nameID = this.m_CameraColorHandle.nameID;
				IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> enumerator = *renderingData.cameraData.captureActions;
				enumerator.Reset();
				while (enumerator.MoveNext())
				{
					Action<RenderTargetIdentifier, CommandBuffer> action = enumerator.Current;
					action(nameID, *renderingData.commandBuffer);
				}
			}
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			CapturePass.UnsafePassData unsafePassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<CapturePass.UnsafePassData>(base.passName, out unsafePassData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\CapturePass.cs", 55))
			{
				unsafePassData.source = universalResourceData.cameraColor;
				unsafePassData.captureActions = universalCameraData.captureActions;
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				IBaseRenderGraphBuilder baseRenderGraphBuilder = unsafeRenderGraphBuilder;
				TextureHandle cameraColor = universalResourceData.cameraColor;
				baseRenderGraphBuilder.UseTexture(cameraColor, AccessFlags.Read);
				unsafeRenderGraphBuilder.SetRenderFunc<CapturePass.UnsafePassData>(delegate(CapturePass.UnsafePassData data, UnsafeGraphContext unsafeContext)
				{
					CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(unsafeContext.cmd);
					IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions = data.captureActions;
					data.captureActions.Reset();
					while (data.captureActions.MoveNext())
					{
						captureActions.Current(data.source, nativeCommandBuffer);
					}
				});
			}
		}

		private RTHandle m_CameraColorHandle;

		private class UnsafePassData
		{
			internal TextureHandle source;

			public IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions;
		}
	}
}
