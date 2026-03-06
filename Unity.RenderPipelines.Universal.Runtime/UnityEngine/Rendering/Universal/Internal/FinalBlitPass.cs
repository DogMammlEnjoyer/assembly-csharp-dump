using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class FinalBlitPass : ScriptableRenderPass
	{
		public FinalBlitPass(RenderPassEvent evt, Material blitMaterial, Material blitHDRMaterial)
		{
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(URPProfileId.BlitFinalToBackBuffer);
			base.useNativeRenderPass = false;
			this.m_PassData = new FinalBlitPass.PassData();
			base.renderPassEvent = evt;
			this.m_BlitMaterialData = new FinalBlitPass.BlitMaterialData[2];
			for (int i = 0; i < 2; i++)
			{
				this.m_BlitMaterialData[i].material = ((i == 0) ? blitMaterial : blitHDRMaterial);
				FinalBlitPass.BlitMaterialData[] blitMaterialData = this.m_BlitMaterialData;
				int num = i;
				Material material = this.m_BlitMaterialData[i].material;
				blitMaterialData[num].nearestSamplerPass = ((material != null) ? material.FindPass("NearestDebugDraw") : -1);
				FinalBlitPass.BlitMaterialData[] blitMaterialData2 = this.m_BlitMaterialData;
				int num2 = i;
				Material material2 = this.m_BlitMaterialData[i].material;
				blitMaterialData2[num2].bilinearSamplerPass = ((material2 != null) ? material2.FindPass("BilinearDebugDraw") : -1);
			}
		}

		public void Dispose()
		{
		}

		[Obsolete("Use RTHandles for colorHandle", true)]
		public void Setup(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorHandle)
		{
			throw new NotSupportedException("Setup with RenderTargetHandle has been deprecated. Use it with RTHandles instead.");
		}

		public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle colorHandle)
		{
			this.m_Source = colorHandle;
		}

		private static void SetupHDROutput(ColorGamut hdrDisplayColorGamut, Material material, HDROutputUtils.Operation hdrOperation, Vector4 hdrOutputParameters, bool rendersOverlayUI)
		{
			material.SetVector(ShaderPropertyId.hdrOutputLuminanceParams, hdrOutputParameters);
			HDROutputUtils.ConfigureHDROutput(material, hdrDisplayColorGamut, hdrOperation);
			CoreUtils.SetKeyword(material, "_HDR_OVERLAY", rendersOverlayUI);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			if (activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget))
			{
				base.ConfigureTarget(*activeDebugHandler.DebugScreenColorHandle, *activeDebugHandler.DebugScreenDepthHandle);
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			bool isHDROutputActive = renderingData.cameraData.isHDROutputActive;
			bool enableAlphaOutput = false;
			this.InitPassData(universalCameraData, ref this.m_PassData, isHDROutputActive ? FinalBlitPass.BlitType.HDR : FinalBlitPass.BlitType.Core, enableAlphaOutput);
			if (this.m_PassData.blitMaterialData.material == null)
			{
				Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", new object[]
				{
					this.m_PassData.blitMaterialData,
					base.GetType().Name
				});
				return;
			}
			RenderTargetIdentifier cameraTargetIdentifier = RenderingUtils.GetCameraTargetIdentifier(ref renderingData);
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			bool flag = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget);
			RTHandleStaticHelpers.SetRTHandleStaticWrapper(cameraTargetIdentifier);
			RTHandle s_RTHandleWrapper = RTHandleStaticHelpers.s_RTHandleWrapper;
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			if (this.m_Source == universalCameraData.renderer.GetCameraColorFrontBuffer(commandBuffer))
			{
				this.m_Source = renderingData.cameraData.renderer->cameraColorTargetHandle;
			}
			using (new ProfilingScope(commandBuffer, base.profilingSampler))
			{
				this.m_PassData.blitMaterialData.material.enabledKeywords = null;
				commandBuffer.SetKeyword(ShaderGlobalKeywords.LinearToSRGBConversion, universalCameraData.requireSrgbConversion);
				if (isHDROutputActive)
				{
					Tonemapping component = VolumeManager.instance.stack.GetComponent<Tonemapping>();
					Vector4 hdrOutputParameters;
					UniversalRenderPipeline.GetHDROutputLuminanceParameters(universalCameraData.hdrDisplayInformation, universalCameraData.hdrDisplayColorGamut, component, out hdrOutputParameters);
					HDROutputUtils.Operation operation = HDROutputUtils.Operation.None;
					if (activeDebugHandler == null || !activeDebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget))
					{
						operation |= HDROutputUtils.Operation.ColorEncoding;
					}
					if (!universalCameraData.postProcessEnabled)
					{
						operation |= HDROutputUtils.Operation.ColorConversion;
					}
					FinalBlitPass.SetupHDROutput(universalCameraData.hdrDisplayColorGamut, this.m_PassData.blitMaterialData.material, operation, hdrOutputParameters, universalCameraData.rendersOverlayUI);
				}
				if (flag)
				{
					RenderTexture rt = this.m_Source.rt;
					int pass = (rt != null && rt.filterMode == FilterMode.Bilinear) ? this.m_PassData.blitMaterialData.bilinearSamplerPass : this.m_PassData.blitMaterialData.nearestSamplerPass;
					Vector2 v = this.m_Source.useScaling ? new Vector2(this.m_Source.rtHandleProperties.rtHandleScale.x, this.m_Source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(commandBuffer, this.m_Source, v, this.m_PassData.blitMaterialData.material, pass);
					universalCameraData.renderer.ConfigureCameraTarget(*activeDebugHandler.DebugScreenColorHandle, *activeDebugHandler.DebugScreenDepthHandle);
				}
				else if (GL.wireframe && universalCameraData.isSceneViewCamera)
				{
					commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
					commandBuffer.Blit(this.m_Source.nameID, s_RTHandleWrapper.nameID);
				}
				else
				{
					RenderBufferLoadAction loadAction = RenderBufferLoadAction.DontCare;
					if (!universalCameraData.isSceneViewCamera && !universalCameraData.isDefaultViewport)
					{
						loadAction = RenderBufferLoadAction.Load;
					}
					if (universalCameraData.xr.enabled)
					{
						loadAction = RenderBufferLoadAction.Load;
					}
					CoreUtils.SetRenderTarget(*renderingData.commandBuffer, s_RTHandleWrapper.nameID, loadAction, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear);
					FinalBlitPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, this.m_Source, s_RTHandleWrapper, universalCameraData);
					universalCameraData.renderer.ConfigureCameraTarget(s_RTHandleWrapper, s_RTHandleWrapper);
				}
			}
		}

		private static void ExecutePass(RasterCommandBuffer cmd, FinalBlitPass.PassData data, RTHandle source, RTHandle destination, UniversalCameraData cameraData)
		{
			bool flag = !cameraData.isSceneViewCamera;
			if (cameraData.xr.enabled)
			{
				flag = (new RenderTargetIdentifier(destination.nameID, 0, CubemapFace.Unknown, -1) == new RenderTargetIdentifier(cameraData.xr.renderTarget, 0, CubemapFace.Unknown, -1));
			}
			Vector4 finalBlitScaleBias = RenderingUtils.GetFinalBlitScaleBias(source, destination, cameraData);
			if (flag)
			{
				cmd.SetViewport(cameraData.pixelRect);
			}
			cmd.SetWireframe(false);
			CoreUtils.SetKeyword(data.blitMaterialData.material, "_ENABLE_ALPHA_OUTPUT", data.enableAlphaOutput);
			RenderTexture rt = source.rt;
			int pass = (rt != null && rt.filterMode == FilterMode.Bilinear) ? data.blitMaterialData.bilinearSamplerPass : data.blitMaterialData.nearestSamplerPass;
			Blitter.BlitTexture(cmd, source, finalBlitScaleBias, data.blitMaterialData.material, pass);
		}

		private void InitPassData(UniversalCameraData cameraData, ref FinalBlitPass.PassData passData, FinalBlitPass.BlitType blitType, bool enableAlphaOutput)
		{
			passData.cameraData = cameraData;
			passData.requireSrgbConversion = cameraData.requireSrgbConversion;
			passData.enableAlphaOutput = enableAlphaOutput;
			passData.blitMaterialData = this.m_BlitMaterialData[(int)blitType];
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, UniversalCameraData cameraData, in TextureHandle src, in TextureHandle dest, TextureHandle overlayUITexture)
		{
			FinalBlitPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<FinalBlitPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\FinalBlitPass.cs", 270))
			{
				frameData.Get<UniversalResourceData>();
				bool flag = cameraData.renderer is UniversalRenderer;
				if (cameraData.requiresDepthTexture && flag)
				{
					rasterRenderGraphBuilder.UseGlobalTexture(FinalBlitPass.s_CameraDepthTextureID, AccessFlags.Read);
				}
				bool isHDROutputActive = cameraData.isHDROutputActive;
				bool isAlphaOutputEnabled = cameraData.isAlphaOutputEnabled;
				this.InitPassData(cameraData, ref passData, isHDROutputActive ? FinalBlitPass.BlitType.HDR : FinalBlitPass.BlitType.Core, isAlphaOutputEnabled);
				passData.sourceID = ShaderPropertyId.sourceTex;
				passData.source = src;
				rasterRenderGraphBuilder.UseTexture(src, AccessFlags.Read);
				passData.destination = dest;
				AccessFlags flags = AccessFlags.Write;
				bool flag2 = !XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster);
				rasterRenderGraphBuilder.EnableFoveatedRasterization(cameraData.xr.supportsFoveatedRendering && flag2);
				if (cameraData.xr.enabled && cameraData.isDefaultViewport && !isAlphaOutputEnabled)
				{
					flags = AccessFlags.WriteAll;
				}
				rasterRenderGraphBuilder.SetRenderAttachment(dest, 0, flags);
				if (isHDROutputActive && overlayUITexture.IsValid())
				{
					Tonemapping component = VolumeManager.instance.stack.GetComponent<Tonemapping>();
					UniversalRenderPipeline.GetHDROutputLuminanceParameters(passData.cameraData.hdrDisplayInformation, passData.cameraData.hdrDisplayColorGamut, component, out passData.hdrOutputLuminanceParams);
					rasterRenderGraphBuilder.UseTexture(overlayUITexture, AccessFlags.Read);
				}
				else
				{
					passData.hdrOutputLuminanceParams = new Vector4(-1f, -1f, -1f, -1f);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<FinalBlitPass.PassData>(delegate(FinalBlitPass.PassData data, RasterGraphContext context)
				{
					data.blitMaterialData.material.enabledKeywords = null;
					context.cmd.SetKeyword(ShaderGlobalKeywords.LinearToSRGBConversion, data.requireSrgbConversion);
					data.blitMaterialData.material.SetTexture(data.sourceID, data.source);
					DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(data.cameraData);
					bool flag3 = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(data.cameraData.resolveFinalTarget);
					if (data.hdrOutputLuminanceParams.w >= 0f)
					{
						HDROutputUtils.Operation operation = HDROutputUtils.Operation.None;
						if (activeDebugHandler == null || !activeDebugHandler.HDRDebugViewIsActive(data.cameraData.resolveFinalTarget))
						{
							operation |= HDROutputUtils.Operation.ColorEncoding;
						}
						if (!data.cameraData.postProcessEnabled)
						{
							operation |= HDROutputUtils.Operation.ColorConversion;
						}
						FinalBlitPass.SetupHDROutput(data.cameraData.hdrDisplayColorGamut, data.blitMaterialData.material, operation, data.hdrOutputLuminanceParams, data.cameraData.rendersOverlayUI);
					}
					if (flag3)
					{
						RTHandle rthandle = data.source;
						Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
						RenderTexture rt = rthandle.rt;
						int pass = (rt != null && rt.filterMode == FilterMode.Bilinear) ? data.blitMaterialData.bilinearSamplerPass : data.blitMaterialData.nearestSamplerPass;
						Blitter.BlitTexture(context.cmd, rthandle, v, data.blitMaterialData.material, pass);
						return;
					}
					FinalBlitPass.ExecutePass(context.cmd, data, data.source, data.destination, data.cameraData);
				});
			}
		}

		private RTHandle m_Source;

		private FinalBlitPass.PassData m_PassData;

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private FinalBlitPass.BlitMaterialData[] m_BlitMaterialData;

		private static class BlitPassNames
		{
			public const string NearestSampler = "NearestDebugDraw";

			public const string BilinearSampler = "BilinearDebugDraw";
		}

		private enum BlitType
		{
			Core,
			HDR,
			Count
		}

		private struct BlitMaterialData
		{
			public Material material;

			public int nearestSamplerPass;

			public int bilinearSamplerPass;
		}

		private class PassData
		{
			internal TextureHandle source;

			internal TextureHandle destination;

			internal int sourceID;

			internal Vector4 hdrOutputLuminanceParams;

			internal bool requireSrgbConversion;

			internal bool enableAlphaOutput;

			internal FinalBlitPass.BlitMaterialData blitMaterialData;

			internal UniversalCameraData cameraData;
		}
	}
}
