using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class HDRDebugViewPass : ScriptableRenderPass
	{
		public HDRDebugViewPass(Material mat)
		{
			base.profilingSampler = new ProfilingSampler("Blit HDR Debug Data");
			base.renderPassEvent = (RenderPassEvent)1003;
			this.m_PassDataCIExy = new HDRDebugViewPass.PassDataCIExy
			{
				material = mat
			};
			this.m_PassDataDebugView = new HDRDebugViewPass.PassDataDebugView
			{
				material = mat
			};
			this.m_material = mat;
			base.useNativeRenderPass = false;
		}

		public static void ConfigureDescriptorForCIEPrepass(ref RenderTextureDescriptor descriptor)
		{
			descriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
			descriptor.width = (descriptor.height = HDRDebugViewPass.ShaderConstants._SizeOfHDRXYMapping);
			descriptor.useMipMap = false;
			descriptor.autoGenerateMips = false;
			descriptor.useDynamicScale = true;
			descriptor.depthStencilFormat = GraphicsFormat.None;
			descriptor.enableRandomWrite = true;
			descriptor.msaaSamples = 1;
			descriptor.dimension = TextureDimension.Tex2D;
			descriptor.vrUsage = VRTextureUsage.None;
		}

		internal static Vector4 GetLuminanceParameters(UniversalCameraData cameraData)
		{
			Vector4 zero = Vector4.zero;
			if (cameraData.isHDROutputActive)
			{
				Tonemapping component = VolumeManager.instance.stack.GetComponent<Tonemapping>();
				UniversalRenderPipeline.GetHDROutputLuminanceParameters(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, component, out zero);
			}
			else
			{
				zero.z = 1f;
			}
			return zero;
		}

		private static void ExecuteCIExyPrepass(CommandBuffer cmd, HDRDebugViewPass.PassDataCIExy data, RTHandle sourceTexture, RTHandle xyTarget, RTHandle destTexture)
		{
			CoreUtils.SetRenderTarget(cmd, destTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			Vector4 value = new Vector4((float)HDRDebugViewPass.ShaderConstants._SizeOfHDRXYMapping, (float)HDRDebugViewPass.ShaderConstants._SizeOfHDRXYMapping, 0f, 0f);
			cmd.SetRandomWriteTarget(HDRDebugViewPass.ShaderConstants._CIExyUAVIndex, xyTarget);
			data.material.SetVector(HDRDebugViewPass.ShaderConstants._HDRDebugParamsId, value);
			data.material.SetVector(ShaderPropertyId.hdrOutputLuminanceParams, data.luminanceParameters);
			Vector2 v = sourceTexture.useScaling ? new Vector2(sourceTexture.rtHandleProperties.rtHandleScale.x, sourceTexture.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			Blitter.BlitTexture(cmd, sourceTexture, v, data.material, 0);
			cmd.ClearRandomWriteTargets();
		}

		private static void ExecuteHDRDebugViewFinalPass(RasterCommandBuffer cmd, HDRDebugViewPass.PassDataDebugView data, RTHandle sourceTexture, RTHandle destination, RTHandle xyTarget)
		{
			if (data.cameraData.isHDROutputActive)
			{
				HDROutputUtils.ConfigureHDROutput(data.material, data.cameraData.hdrDisplayColorGamut, HDROutputUtils.Operation.ColorEncoding);
				CoreUtils.SetKeyword(data.material, "_HDR_OVERLAY", data.cameraData.rendersOverlayUI);
			}
			data.material.SetTexture(HDRDebugViewPass.ShaderConstants._xyTextureId, xyTarget);
			Vector4 value = new Vector4((float)HDRDebugViewPass.ShaderConstants._SizeOfHDRXYMapping, (float)HDRDebugViewPass.ShaderConstants._SizeOfHDRXYMapping, 0f, 0f);
			data.material.SetVector(HDRDebugViewPass.ShaderConstants._HDRDebugParamsId, value);
			data.material.SetVector(ShaderPropertyId.hdrOutputLuminanceParams, data.luminanceParameters);
			data.material.SetInteger(HDRDebugViewPass.ShaderConstants._DebugHDRModeId, (int)data.hdrDebugMode);
			Vector4 finalBlitScaleBias = RenderingUtils.GetFinalBlitScaleBias(sourceTexture, destination, data.cameraData);
			RenderTargetIdentifier rhs = BuiltinRenderTextureType.CameraTarget;
			if (data.cameraData.xr.enabled)
			{
				rhs = data.cameraData.xr.renderTarget;
			}
			if (destination.nameID == rhs || data.cameraData.targetTexture != null)
			{
				cmd.SetViewport(data.cameraData.pixelRect);
			}
			Blitter.BlitTexture(cmd, sourceTexture, finalBlitScaleBias, data.material, 1);
		}

		public void Dispose()
		{
			RTHandle ciexyTarget = this.m_CIExyTarget;
			if (ciexyTarget != null)
			{
				ciexyTarget.Release();
			}
			RTHandle passthroughRT = this.m_PassthroughRT;
			if (passthroughRT == null)
			{
				return;
			}
			passthroughRT.Release();
		}

		public void Setup(UniversalCameraData cameraData, HDRDebugMode hdrdebugMode)
		{
			this.m_PassDataDebugView.hdrDebugMode = hdrdebugMode;
			RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
			DebugHandler.ConfigureColorDescriptorForDebugScreen(ref cameraTargetDescriptor, cameraData.pixelWidth, cameraData.pixelHeight);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PassthroughRT, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_HDRDebugDummyRT");
			RenderTextureDescriptor cameraTargetDescriptor2 = cameraData.cameraTargetDescriptor;
			HDRDebugViewPass.ConfigureDescriptorForCIEPrepass(ref cameraTargetDescriptor2);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_CIExyTarget, cameraTargetDescriptor2, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_xyBuffer");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			CommandBuffer cmd = *renderingData.commandBuffer;
			this.m_PassDataCIExy.luminanceParameters = (this.m_PassDataDebugView.luminanceParameters = HDRDebugViewPass.GetLuminanceParameters(cameraData));
			this.m_PassDataDebugView.cameraData = cameraData;
			RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer->cameraColorTargetHandle;
			RTHandleStaticHelpers.SetRTHandleStaticWrapper(RenderingUtils.GetCameraTargetIdentifier(ref renderingData));
			RTHandle s_RTHandleWrapper = RTHandleStaticHelpers.s_RTHandleWrapper;
			this.m_material.enabledKeywords = null;
			CoreUtils.SetRenderTarget(cmd, this.m_CIExyTarget, ClearFlag.Color, Color.clear, 0, CubemapFace.Unknown, -1);
			this.ExecutePass(cmd, this.m_PassDataCIExy, this.m_PassDataDebugView, cameraColorTargetHandle, this.m_CIExyTarget, s_RTHandleWrapper);
		}

		private void ExecutePass(CommandBuffer cmd, HDRDebugViewPass.PassDataCIExy dataCIExy, HDRDebugViewPass.PassDataDebugView dataDebugView, RTHandle sourceTexture, RTHandle xyTarget, RTHandle destTexture)
		{
			RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(cmd);
			if (dataDebugView.hdrDebugMode != HDRDebugMode.ValuesAbovePaperWhite)
			{
				using (new ProfilingScope(cmd, base.profilingSampler))
				{
					HDRDebugViewPass.ExecuteCIExyPrepass(cmd, dataCIExy, sourceTexture, xyTarget, this.m_PassthroughRT);
				}
			}
			CoreUtils.SetRenderTarget(cmd, destTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			using (new ProfilingScope(cmd, base.profilingSampler))
			{
				HDRDebugViewPass.ExecuteHDRDebugViewFinalPass(rasterCommandBuffer, dataDebugView, sourceTexture, destTexture, xyTarget);
			}
			dataDebugView.cameraData.renderer.ConfigureCameraTarget(destTexture, destTexture);
		}

		internal void RenderHDRDebug(RenderGraph renderGraph, UniversalCameraData cameraData, TextureHandle srcColor, TextureHandle overlayUITexture, TextureHandle dstColor, HDRDebugMode hdrDebugMode)
		{
			bool flag = hdrDebugMode != HDRDebugMode.ValuesAbovePaperWhite;
			Vector4 luminanceParameters = HDRDebugViewPass.GetLuminanceParameters(cameraData);
			TextureHandle passThrough = srcColor;
			TextureHandle xyBuffer = TextureHandle.nullHandle;
			if (flag)
			{
				RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureColorDescriptorForDebugScreen(ref cameraTargetDescriptor, cameraData.pixelWidth, cameraData.pixelHeight);
				passThrough = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_HDRDebugDummyRT", false, FilterMode.Point, TextureWrapMode.Clamp);
				HDRDebugViewPass.ConfigureDescriptorForCIEPrepass(ref cameraTargetDescriptor);
				xyBuffer = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_xyBuffer", true, FilterMode.Point, TextureWrapMode.Clamp);
				HDRDebugViewPass.PassDataCIExy passDataCIExy;
				using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<HDRDebugViewPass.PassDataCIExy>("Blit HDR DebugView CIExy", out passDataCIExy, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\HDRDebugViewPass.cs", 234))
				{
					passDataCIExy.material = this.m_material;
					passDataCIExy.luminanceParameters = luminanceParameters;
					passDataCIExy.srcColor = srcColor;
					unsafeRenderGraphBuilder.UseTexture(srcColor, AccessFlags.Read);
					passDataCIExy.xyBuffer = xyBuffer;
					unsafeRenderGraphBuilder.UseTexture(xyBuffer, AccessFlags.Write);
					passDataCIExy.passThrough = passThrough;
					unsafeRenderGraphBuilder.UseTexture(passThrough, AccessFlags.Write);
					unsafeRenderGraphBuilder.SetRenderFunc<HDRDebugViewPass.PassDataCIExy>(delegate(HDRDebugViewPass.PassDataCIExy data, UnsafeGraphContext context)
					{
						HDRDebugViewPass.ExecuteCIExyPrepass(CommandBufferHelpers.GetNativeCommandBuffer(context.cmd), data, data.srcColor, data.xyBuffer, data.passThrough);
					});
				}
			}
			HDRDebugViewPass.PassDataDebugView passDataDebugView;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<HDRDebugViewPass.PassDataDebugView>("Blit HDR DebugView", out passDataDebugView, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\HDRDebugViewPass.cs", 252))
			{
				passDataDebugView.material = this.m_material;
				passDataDebugView.hdrDebugMode = hdrDebugMode;
				passDataDebugView.luminanceParameters = luminanceParameters;
				passDataDebugView.cameraData = cameraData;
				if (flag)
				{
					passDataDebugView.xyBuffer = xyBuffer;
					rasterRenderGraphBuilder.UseTexture(xyBuffer, AccessFlags.Read);
				}
				passDataDebugView.srcColor = srcColor;
				rasterRenderGraphBuilder.UseTexture(srcColor, AccessFlags.Read);
				passDataDebugView.dstColor = dstColor;
				rasterRenderGraphBuilder.SetRenderAttachment(dstColor, 0, AccessFlags.WriteAll);
				if (overlayUITexture.IsValid())
				{
					passDataDebugView.overlayUITexture = overlayUITexture;
					rasterRenderGraphBuilder.UseTexture(overlayUITexture, AccessFlags.Read);
				}
				rasterRenderGraphBuilder.SetRenderFunc<HDRDebugViewPass.PassDataDebugView>(delegate(HDRDebugViewPass.PassDataDebugView data, RasterGraphContext context)
				{
					data.material.enabledKeywords = null;
					HDRDebugViewPass.ExecuteHDRDebugViewFinalPass(context.cmd, data, data.srcColor, data.dstColor, data.xyBuffer);
				});
			}
		}

		private HDRDebugViewPass.PassDataCIExy m_PassDataCIExy;

		private HDRDebugViewPass.PassDataDebugView m_PassDataDebugView;

		private RTHandle m_CIExyTarget;

		private RTHandle m_PassthroughRT;

		private Material m_material;

		private enum HDRDebugPassId
		{
			CIExyPrepass,
			DebugViewPass
		}

		private class PassDataCIExy
		{
			internal Material material;

			internal Vector4 luminanceParameters;

			internal TextureHandle srcColor;

			internal TextureHandle xyBuffer;

			internal TextureHandle passThrough;
		}

		private class PassDataDebugView
		{
			internal Material material;

			internal HDRDebugMode hdrDebugMode;

			internal UniversalCameraData cameraData;

			internal Vector4 luminanceParameters;

			internal TextureHandle overlayUITexture;

			internal TextureHandle xyBuffer;

			internal TextureHandle srcColor;

			internal TextureHandle dstColor;
		}

		internal class ShaderConstants
		{
			public static readonly int _DebugHDRModeId = Shader.PropertyToID("_DebugHDRMode");

			public static readonly int _HDRDebugParamsId = Shader.PropertyToID("_HDRDebugParams");

			public static readonly int _xyTextureId = Shader.PropertyToID("_xyBuffer");

			public static readonly int _SizeOfHDRXYMapping = 512;

			public static readonly int _CIExyUAVIndex = 1;
		}
	}
}
