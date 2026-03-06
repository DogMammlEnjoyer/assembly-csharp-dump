using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class CopyDepthPass : ScriptableRenderPass
	{
		private RTHandle source { get; set; }

		private RTHandle destination { get; set; }

		internal int MsaaSamples { get; set; }

		internal bool CopyToDepth { get; set; }

		internal bool CopyToDepthXR { get; set; }

		internal bool CopyToBackbuffer { get; set; }

		public CopyDepthPass(RenderPassEvent evt, Shader copyDepthShader, bool shouldClear = false, bool copyToDepth = false, bool copyResolvedDepth = false, string customPassName = null)
		{
			base.profilingSampler = ((customPassName != null) ? new ProfilingSampler(customPassName) : ProfilingSampler.Get<URPProfileId>(URPProfileId.CopyDepth));
			this.m_PassData = new CopyDepthPass.PassData();
			this.CopyToDepth = copyToDepth;
			this.m_CopyDepthMaterial = ((copyDepthShader != null) ? CoreUtils.CreateEngineMaterial(copyDepthShader) : null);
			base.renderPassEvent = evt;
			this.m_CopyResolvedDepth = copyResolvedDepth;
			this.m_ShouldClear = shouldClear;
			this.CopyToDepthXR = false;
			this.CopyToBackbuffer = false;
		}

		public void Setup(RTHandle source, RTHandle destination)
		{
			this.source = source;
			this.destination = destination;
			this.MsaaSamples = -1;
		}

		public void Dispose()
		{
			CoreUtils.Destroy(this.m_CopyDepthMaterial);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			base.ConfigureTarget(this.destination);
			if (this.m_ShouldClear)
			{
				base.ConfigureClear(ClearFlag.All, Color.black);
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			this.m_PassData.copyDepthMaterial = this.m_CopyDepthMaterial;
			this.m_PassData.msaaSamples = this.MsaaSamples;
			this.m_PassData.copyResolvedDepth = this.m_CopyResolvedDepth;
			this.m_PassData.copyToDepth = (this.CopyToDepth || this.CopyToDepthXR);
			this.m_PassData.isDstBackbuffer = (this.CopyToBackbuffer || this.CopyToDepthXR);
			this.m_PassData.cameraData = cameraData;
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			commandBuffer.SetGlobalTexture(CopyDepthPass.ShaderConstants._CameraDepthAttachment, this.source.nameID);
			if (this.m_PassData.cameraData.xr.enabled && this.m_PassData.cameraData.xr.supportsFoveatedRendering)
			{
				commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
			}
			CopyDepthPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), this.m_PassData, this.source);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, CopyDepthPass.PassData passData, RTHandle source)
		{
			Material copyDepthMaterial = passData.copyDepthMaterial;
			int msaaSamples = passData.msaaSamples;
			bool copyResolvedDepth = passData.copyResolvedDepth;
			bool copyToDepth = passData.copyToDepth;
			if (copyDepthMaterial == null)
			{
				Debug.LogErrorFormat("Missing {0}. Copy Depth render pass will not execute. Check for missing reference in the renderer resources.", new object[]
				{
					copyDepthMaterial
				});
				return;
			}
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.CopyDepth)))
			{
				int num;
				if (copyResolvedDepth || SystemInfo.supportsMultisampledTextures == 0)
				{
					num = 1;
				}
				else if (msaaSamples == -1)
				{
					num = source.rt.antiAliasing;
				}
				else
				{
					num = msaaSamples;
				}
				if (num != 2)
				{
					if (num != 4)
					{
						if (num == 8)
						{
							cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa2, false);
							cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa4, false);
							cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa8, true);
						}
						else
						{
							cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa2, false);
							cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa4, false);
							cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa8, false);
						}
					}
					else
					{
						cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa2, false);
						cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa4, true);
						cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa8, false);
					}
				}
				else
				{
					cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa2, true);
					cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa4, false);
					cmd.SetKeyword(ShaderGlobalKeywords.DepthMsaa8, false);
				}
				cmd.SetKeyword(ShaderGlobalKeywords._OUTPUT_DEPTH, copyToDepth);
				bool flag = passData.isDstBackbuffer && passData.cameraData.IsHandleYFlipped(source);
				Vector2 vector = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
				Vector4 scaleBias = flag ? new Vector4(vector.x, -vector.y, 0f, vector.y) : new Vector4(vector.x, vector.y, 0f, 0f);
				if (passData.isDstBackbuffer)
				{
					cmd.SetViewport(passData.cameraData.pixelRect);
				}
				copyDepthMaterial.SetTexture(CopyDepthPass.ShaderConstants._CameraDepthAttachment, source);
				copyDepthMaterial.SetFloat(CopyDepthPass.ShaderConstants._ZWriteShaderHandle, copyToDepth ? 1f : 0f);
				Blitter.BlitTexture(cmd, source, scaleBias, copyDepthMaterial, 0);
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}
			this.destination = ScriptableRenderPass.k_CameraTarget;
		}

		public void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle destination, TextureHandle source, bool bindAsCameraDepth = false, string passName = "Copy Depth")
		{
			UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			this.Render(renderGraph, destination, source, resourceData, cameraData, bindAsCameraDepth, passName);
		}

		public void Render(RenderGraph renderGraph, TextureHandle destination, TextureHandle source, UniversalResourceData resourceData, UniversalCameraData cameraData, bool bindAsCameraDepth = false, string passName = "Copy Depth")
		{
			this.MsaaSamples = -1;
			CopyDepthPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<CopyDepthPass.PassData>(passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\CopyDepthPass.cs", 266))
			{
				passData.copyDepthMaterial = this.m_CopyDepthMaterial;
				passData.msaaSamples = this.MsaaSamples;
				passData.cameraData = cameraData;
				passData.copyResolvedDepth = this.m_CopyResolvedDepth;
				passData.copyToDepth = (this.CopyToDepth || this.CopyToDepthXR);
				passData.isDstBackbuffer = (this.CopyToBackbuffer || this.CopyToDepthXR);
				if (this.CopyToDepth)
				{
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(destination, AccessFlags.WriteAll);
				}
				else if (this.CopyToDepthXR)
				{
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(destination, AccessFlags.WriteAll);
					if (cameraData.xr.enabled && cameraData.xr.copyDepth)
					{
						RenderTargetInfo renderTargetInfo = renderGraph.GetRenderTargetInfo(resourceData.backBufferColor);
						if (renderTargetInfo.msaaSamples > 1)
						{
							TextureDesc textureDesc = new TextureDesc(renderTargetInfo.width, renderTargetInfo.height, false, true);
							textureDesc.name = "XR Copy Depth Dummy Render Target";
							textureDesc.slices = renderTargetInfo.volumeDepth;
							textureDesc.format = renderTargetInfo.format;
							textureDesc.msaaSamples = (MSAASamples)renderTargetInfo.msaaSamples;
							textureDesc.clearBuffer = false;
							textureDesc.bindTextureMS = renderTargetInfo.bindMS;
							TextureHandle tex = renderGraph.CreateTexture(textureDesc);
							rasterRenderGraphBuilder.SetRenderAttachment(tex, 0, AccessFlags.Write);
						}
						else
						{
							rasterRenderGraphBuilder.SetRenderAttachment(resourceData.backBufferColor, 0, AccessFlags.Write);
						}
					}
				}
				else
				{
					rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.WriteAll);
				}
				passData.source = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				if (bindAsCameraDepth && destination.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(destination, CopyDepthPass.ShaderConstants._CameraDepthTexture);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<CopyDepthPass.PassData>(delegate(CopyDepthPass.PassData data, RasterGraphContext context)
				{
					CopyDepthPass.ExecutePass(context.cmd, data, data.source);
				});
			}
		}

		private Material m_CopyDepthMaterial;

		internal bool m_CopyResolvedDepth;

		internal bool m_ShouldClear;

		private CopyDepthPass.PassData m_PassData;

		private static class ShaderConstants
		{
			public static readonly int _CameraDepthAttachment = Shader.PropertyToID("_CameraDepthAttachment");

			public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");

			public static readonly int _ZWriteShaderHandle = Shader.PropertyToID("_ZWrite");
		}

		private class PassData
		{
			internal TextureHandle source;

			internal UniversalCameraData cameraData;

			internal Material copyDepthMaterial;

			internal int msaaSamples;

			internal bool copyResolvedDepth;

			internal bool copyToDepth;

			internal bool isDstBackbuffer;
		}
	}
}
