using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class CopyColorPass : ScriptableRenderPass
	{
		private RTHandle source { get; set; }

		private RTHandle destination { get; set; }

		public CopyColorPass(RenderPassEvent evt, Material samplingMaterial, Material copyColorMaterial = null, string customPassName = null)
		{
			base.profilingSampler = ((customPassName != null) ? new ProfilingSampler(customPassName) : ProfilingSampler.Get<URPProfileId>(URPProfileId.CopyColor));
			this.m_PassData = new CopyColorPass.PassData();
			this.m_SamplingMaterial = samplingMaterial;
			this.m_CopyColorMaterial = copyColorMaterial;
			this.m_SampleOffsetShaderHandle = Shader.PropertyToID("_SampleOffset");
			base.renderPassEvent = evt;
			this.m_DownsamplingMethod = Downsampling.None;
			base.useNativeRenderPass = false;
		}

		public static void ConfigureDescriptor(Downsampling downsamplingMethod, ref RenderTextureDescriptor descriptor, out FilterMode filterMode)
		{
			descriptor.msaaSamples = 1;
			descriptor.depthStencilFormat = GraphicsFormat.None;
			if (downsamplingMethod == Downsampling._2xBilinear)
			{
				descriptor.width = Mathf.Max(1, descriptor.width / 2);
				descriptor.height = Mathf.Max(1, descriptor.height / 2);
			}
			else if (downsamplingMethod == Downsampling._4xBox || downsamplingMethod == Downsampling._4xBilinear)
			{
				descriptor.width = Mathf.Max(1, descriptor.width / 4);
				descriptor.height = Mathf.Max(1, descriptor.height / 4);
			}
			filterMode = ((downsamplingMethod == Downsampling.None) ? FilterMode.Point : FilterMode.Bilinear);
		}

		[Obsolete("Use RTHandles for source and destination.", true)]
		public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination, Downsampling downsampling)
		{
			throw new NotSupportedException("Setup with RenderTargetIdentifier has been deprecated. Use it with RTHandles instead.");
		}

		public void Setup(RTHandle source, RTHandle destination, Downsampling downsampling)
		{
			this.source = source;
			this.destination = destination;
			this.m_DownsamplingMethod = downsampling;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			cmd.SetGlobalTexture(this.destination.name, this.destination.nameID);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			this.m_PassData.samplingMaterial = this.m_SamplingMaterial;
			this.m_PassData.copyColorMaterial = this.m_CopyColorMaterial;
			this.m_PassData.downsamplingMethod = this.m_DownsamplingMethod;
			this.m_PassData.sampleOffsetShaderHandle = this.m_SampleOffsetShaderHandle;
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			if (this.source == renderingData.cameraData.renderer->GetCameraColorFrontBuffer(commandBuffer))
			{
				this.source = renderingData.cameraData.renderer->cameraColorTargetHandle;
			}
			if (renderingData.cameraData.xr.supportsFoveatedRendering)
			{
				commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
			}
			ScriptableRenderer.SetRenderTarget(commandBuffer, this.destination, ScriptableRenderPass.k_CameraTarget, base.clearFlag, base.clearColor);
			CopyColorPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), this.m_PassData, this.source, renderingData.cameraData.xr.enabled);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, CopyColorPass.PassData passData, RTHandle source, bool useDrawProceduralBlit)
		{
			Material samplingMaterial = passData.samplingMaterial;
			Material copyColorMaterial = passData.copyColorMaterial;
			Downsampling downsamplingMethod = passData.downsamplingMethod;
			int sampleOffsetShaderHandle = passData.sampleOffsetShaderHandle;
			if (samplingMaterial == null)
			{
				Debug.LogErrorFormat("Missing {0}. Copy Color render pass will not execute. Check for missing reference in the renderer resources.", new object[]
				{
					samplingMaterial
				});
				return;
			}
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.CopyColor)))
			{
				Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
				switch (downsamplingMethod)
				{
				case Downsampling.None:
					Blitter.BlitTexture(cmd, source, v, copyColorMaterial, 0);
					break;
				case Downsampling._2xBilinear:
					Blitter.BlitTexture(cmd, source, v, copyColorMaterial, 1);
					break;
				case Downsampling._4xBox:
					samplingMaterial.SetFloat(sampleOffsetShaderHandle, 2f);
					Blitter.BlitTexture(cmd, source, v, samplingMaterial, 0);
					break;
				case Downsampling._4xBilinear:
					Blitter.BlitTexture(cmd, source, v, copyColorMaterial, 1);
					break;
				}
			}
		}

		internal TextureHandle Render(RenderGraph renderGraph, ContextContainer frameData, out TextureHandle destination, in TextureHandle source, Downsampling downsampling)
		{
			this.m_DownsamplingMethod = downsampling;
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
			FilterMode filterMode;
			CopyColorPass.ConfigureDescriptor(downsampling, ref cameraTargetDescriptor, out filterMode);
			destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_CameraOpaqueTexture", true, filterMode, TextureWrapMode.Clamp);
			this.RenderInternal(renderGraph, destination, source, universalCameraData.xr.enabled);
			return destination;
		}

		internal void RenderToExistingTexture(RenderGraph renderGraph, ContextContainer frameData, in TextureHandle destination, in TextureHandle source, Downsampling downsampling = Downsampling.None)
		{
			this.m_DownsamplingMethod = downsampling;
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			this.RenderInternal(renderGraph, destination, source, universalCameraData.xr.enabled);
		}

		private void RenderInternal(RenderGraph renderGraph, in TextureHandle destination, in TextureHandle source, bool useProceduralBlit)
		{
			CopyColorPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<CopyColorPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\CopyColorPass.cs", 216))
			{
				passData.destination = destination;
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.WriteAll);
				passData.source = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				passData.useProceduralBlit = useProceduralBlit;
				passData.samplingMaterial = this.m_SamplingMaterial;
				passData.copyColorMaterial = this.m_CopyColorMaterial;
				passData.downsamplingMethod = this.m_DownsamplingMethod;
				passData.sampleOffsetShaderHandle = this.m_SampleOffsetShaderHandle;
				TextureHandle textureHandle = destination;
				if (textureHandle.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(destination, Shader.PropertyToID("_CameraOpaqueTexture"));
				}
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<CopyColorPass.PassData>(delegate(CopyColorPass.PassData data, RasterGraphContext context)
				{
					CopyColorPass.ExecutePass(context.cmd, data, data.source, data.useProceduralBlit);
				});
			}
		}

		private int m_SampleOffsetShaderHandle;

		private Material m_SamplingMaterial;

		private Downsampling m_DownsamplingMethod;

		private Material m_CopyColorMaterial;

		private CopyColorPass.PassData m_PassData;

		private class PassData
		{
			internal TextureHandle source;

			internal TextureHandle destination;

			internal bool useProceduralBlit;

			internal Material samplingMaterial;

			internal Material copyColorMaterial;

			internal Downsampling downsamplingMethod;

			internal int sampleOffsetShaderHandle;
		}
	}
}
