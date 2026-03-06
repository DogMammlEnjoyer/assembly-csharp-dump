using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class ProbeVolumeDebugPass : ScriptableRenderPass
	{
		public ProbeVolumeDebugPass(RenderPassEvent evt, ComputeShader computeShader)
		{
			base.profilingSampler = new ProfilingSampler("Dispatch APV Debug");
			base.renderPassEvent = evt;
			this.m_ComputeShader = computeShader;
		}

		public void Setup(RTHandle depthBuffer, RTHandle normalBuffer)
		{
			this.m_DepthTexture = depthBuffer;
			this.m_NormalTexture = normalBuffer;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (!ProbeReferenceVolume.instance.isInitialized)
			{
				return;
			}
			ref CameraData ptr = ref renderingData.cameraData;
			GraphicsBuffer buffer;
			Vector2 vector;
			if (ProbeReferenceVolume.instance.GetProbeSamplingDebugResources(*ptr.camera, out buffer, out vector))
			{
				CommandBuffer commandBuffer = *renderingData.commandBuffer;
				int kernelIndex = this.m_ComputeShader.FindKernel("ComputePositionNormal");
				commandBuffer.SetComputeTextureParam(this.m_ComputeShader, kernelIndex, "_CameraDepthTexture", this.m_DepthTexture);
				commandBuffer.SetComputeTextureParam(this.m_ComputeShader, kernelIndex, "_NormalBufferTexture", this.m_NormalTexture);
				commandBuffer.SetComputeVectorParam(this.m_ComputeShader, "_positionSS", new Vector4(vector.x, vector.y, 0f, 0f));
				commandBuffer.SetComputeBufferParam(this.m_ComputeShader, kernelIndex, "_ResultBuffer", buffer);
				commandBuffer.DispatchCompute(this.m_ComputeShader, kernelIndex, 1, 1, 1);
			}
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle depthPyramidBuffer, TextureHandle normalBuffer)
		{
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			if (!ProbeReferenceVolume.instance.isInitialized)
			{
				return;
			}
			GraphicsBuffer graphicsBuffer;
			Vector2 clickCoordinates;
			if (ProbeReferenceVolume.instance.GetProbeSamplingDebugResources(universalCameraData.camera, out graphicsBuffer, out clickCoordinates))
			{
				ProbeVolumeDebugPass.WriteApvData writeApvData;
				using (IComputeRenderGraphBuilder computeRenderGraphBuilder = renderGraph.AddComputePass<ProbeVolumeDebugPass.WriteApvData>(base.passName, out writeApvData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\ProbeVolumeDebugPass.cs", 81))
				{
					writeApvData.clickCoordinates = clickCoordinates;
					writeApvData.computeShader = this.m_ComputeShader;
					writeApvData.resultBuffer = renderGraph.ImportBuffer(graphicsBuffer, false);
					writeApvData.depthBuffer = depthPyramidBuffer;
					writeApvData.normalBuffer = normalBuffer;
					computeRenderGraphBuilder.UseBuffer(writeApvData.resultBuffer, AccessFlags.Write);
					computeRenderGraphBuilder.UseTexture(writeApvData.depthBuffer, AccessFlags.Read);
					computeRenderGraphBuilder.UseTexture(writeApvData.normalBuffer, AccessFlags.Read);
					computeRenderGraphBuilder.SetRenderFunc<ProbeVolumeDebugPass.WriteApvData>(delegate(ProbeVolumeDebugPass.WriteApvData data, ComputeGraphContext ctx)
					{
						int kernelIndex = data.computeShader.FindKernel("ComputePositionNormal");
						ctx.cmd.SetComputeTextureParam(data.computeShader, kernelIndex, "_CameraDepthTexture", data.depthBuffer);
						ctx.cmd.SetComputeTextureParam(data.computeShader, kernelIndex, "_NormalBufferTexture", data.normalBuffer);
						ctx.cmd.SetComputeVectorParam(data.computeShader, "_positionSS", new Vector4(data.clickCoordinates.x, data.clickCoordinates.y, 0f, 0f));
						ctx.cmd.SetComputeBufferParam(data.computeShader, kernelIndex, "_ResultBuffer", data.resultBuffer);
						ctx.cmd.DispatchCompute(data.computeShader, kernelIndex, 1, 1, 1);
					});
				}
			}
		}

		private ComputeShader m_ComputeShader;

		private RTHandle m_DepthTexture;

		private RTHandle m_NormalTexture;

		private class WriteApvData
		{
			public ComputeShader computeShader;

			public BufferHandle resultBuffer;

			public Vector2 clickCoordinates;

			public TextureHandle depthBuffer;

			public TextureHandle normalBuffer;
		}
	}
}
