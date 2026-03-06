using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	public static class Vrs
	{
		public static bool IsColorMaskTextureConversionSupported()
		{
			return SystemInfo.supportsComputeShaders && ShadingRateInfo.supportsPerImageTile && Vrs.IsInitialized();
		}

		public static bool IsInitialized()
		{
			return Vrs.s_VrsResources != null && Vrs.s_VrsResources.textureComputeShader != null && Vrs.s_VrsResources.textureReduceKernel != -1 && Vrs.s_VrsResources.textureCopyKernel != -1;
		}

		public static void InitializeResources()
		{
			bool flag = SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3;
			if (SystemInfo.supportsComputeShaders && flag)
			{
				Vrs.s_VrsResources = new VrsResources(GraphicsSettings.GetRenderPipelineSettings<VrsRenderPipelineRuntimeResources>());
			}
		}

		public static void DisposeResources()
		{
			VrsResources vrsResources = Vrs.s_VrsResources;
			if (vrsResources != null)
			{
				vrsResources.Dispose();
			}
			Vrs.s_VrsResources = null;
		}

		public static TextureHandle ColorMaskTextureToShadingRateImage(RenderGraph renderGraph, RTHandle sriRtHandle, RTHandle colorMaskRtHandle, bool yFlip)
		{
			if (renderGraph == null || sriRtHandle == null || colorMaskRtHandle == null)
			{
				Debug.LogError("TextureToShadingRateImage: invalid argument.");
				return TextureHandle.nullHandle;
			}
			TextureHandle sriTextureHandle = renderGraph.ImportShadingRateImageTexture(sriRtHandle);
			TextureHandle colorMaskHandle = renderGraph.ImportTexture(colorMaskRtHandle);
			return Vrs.ColorMaskTextureToShadingRateImage(renderGraph, sriTextureHandle, colorMaskHandle, colorMaskRtHandle.dimension, yFlip);
		}

		public static TextureHandle ColorMaskTextureToShadingRateImage(RenderGraph renderGraph, TextureHandle sriTextureHandle, TextureHandle colorMaskHandle, TextureDimension colorMaskDimension, bool yFlip)
		{
			if (!Vrs.IsColorMaskTextureConversionSupported())
			{
				Debug.LogError("ColorMaskTextureToShadingRateImage: conversion not supported.");
				return TextureHandle.nullHandle;
			}
			TextureDesc descriptor = sriTextureHandle.GetDescriptor(renderGraph);
			if (descriptor.dimension != TextureDimension.Tex2D)
			{
				Debug.LogError("ColorMaskTextureToShadingRateImage: Vrs image not a texture 2D.");
				return TextureHandle.nullHandle;
			}
			if (colorMaskDimension != TextureDimension.Tex2D && colorMaskDimension != TextureDimension.Tex2DArray)
			{
				Debug.LogError("ColorMaskTextureToShadingRateImage: Input texture dimension not supported.");
				return TextureHandle.nullHandle;
			}
			Vrs.ConversionPassData conversionPassData;
			TextureHandle sriTextureHandle2;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder = renderGraph.AddComputePass<Vrs.ConversionPassData>("TextureToShadingRateImage", out conversionPassData, Vrs.s_VrsResources.conversionProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Vrs\\Vrs.cs", 159))
			{
				conversionPassData.sriTextureHandle = sriTextureHandle;
				conversionPassData.mainTexHandle = colorMaskHandle;
				conversionPassData.mainTexDimension = colorMaskDimension;
				conversionPassData.mainTexLutHandle = renderGraph.ImportBuffer(Vrs.s_VrsResources.conversionLutBuffer, false);
				conversionPassData.validatedShadingRateFragmentSizeHandle = renderGraph.ImportBuffer(Vrs.s_VrsResources.validatedShadingRateFragmentSizeBuffer, false);
				conversionPassData.computeShader = Vrs.s_VrsResources.textureComputeShader;
				conversionPassData.kernelIndex = Vrs.s_VrsResources.textureReduceKernel;
				conversionPassData.scaleBias = new Vector4
				{
					x = 1f / (float)(descriptor.width * Vrs.s_VrsResources.tileSize.x),
					y = 1f / (float)(descriptor.height * Vrs.s_VrsResources.tileSize.y),
					z = (float)descriptor.width,
					w = (float)descriptor.height
				};
				conversionPassData.dispatchSize = new Vector2Int(descriptor.width, descriptor.height);
				conversionPassData.yFlip = yFlip;
				computeRenderGraphBuilder.UseTexture(conversionPassData.sriTextureHandle, AccessFlags.Write);
				computeRenderGraphBuilder.UseTexture(conversionPassData.mainTexHandle, AccessFlags.Read);
				computeRenderGraphBuilder.UseBuffer(conversionPassData.mainTexLutHandle, AccessFlags.Read);
				computeRenderGraphBuilder.AllowGlobalStateModification(true);
				computeRenderGraphBuilder.SetRenderFunc<Vrs.ConversionPassData>(delegate(Vrs.ConversionPassData innerPassData, ComputeGraphContext context)
				{
					Vrs.ConversionDispatch(context.cmd, innerPassData);
				});
				sriTextureHandle2 = conversionPassData.sriTextureHandle;
			}
			return sriTextureHandle2;
		}

		public static void ShadingRateImageToColorMaskTexture(RenderGraph renderGraph, in TextureHandle sriTextureHandle, in TextureHandle colorMaskHandle)
		{
			if (Vrs.s_VrsResources == null)
			{
				Debug.LogError("ShadingRateImageToColorMaskTexture: VRS not initialized.");
				return;
			}
			TextureHandle textureHandle = colorMaskHandle;
			if (!textureHandle.IsValid())
			{
				Debug.LogError("ShadingRateImageToColorMaskTexture: Output target handle is not valid.");
				return;
			}
			Vrs.VisualizationPassData visualizationPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<Vrs.VisualizationPassData>("ShadingRateImageToTexture", out visualizationPassData, Vrs.s_VrsResources.visualizationProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Vrs\\Vrs.cs", 214))
			{
				visualizationPassData.material = Vrs.s_VrsResources.visualizationMaterial;
				textureHandle = sriTextureHandle;
				if (textureHandle.IsValid())
				{
					visualizationPassData.source = sriTextureHandle;
				}
				else
				{
					visualizationPassData.source = renderGraph.defaultResources.blackTexture;
				}
				visualizationPassData.lut = renderGraph.ImportBuffer(Vrs.s_VrsResources.visualizationLutBuffer, false);
				visualizationPassData.dummy = renderGraph.defaultResources.blackTexture;
				visualizationPassData.visualizationParams = new Vector4(1f / (float)Vrs.s_VrsResources.tileSize.x, 1f / (float)Vrs.s_VrsResources.tileSize.y, 0f, 0f);
				rasterRenderGraphBuilder.UseTexture(visualizationPassData.source, AccessFlags.Read);
				rasterRenderGraphBuilder.UseBuffer(visualizationPassData.lut, AccessFlags.Read);
				rasterRenderGraphBuilder.UseTexture(visualizationPassData.dummy, AccessFlags.Read);
				rasterRenderGraphBuilder.SetRenderAttachment(colorMaskHandle, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<Vrs.VisualizationPassData>(delegate(Vrs.VisualizationPassData innerPassData, RasterGraphContext context)
				{
					innerPassData.material.SetTexture(VrsShaders.s_ShadingRateImage, innerPassData.source);
					innerPassData.material.SetBuffer(VrsShaders.s_VisualizationLut, innerPassData.lut);
					innerPassData.material.SetVector(VrsShaders.s_VisualizationParams, innerPassData.visualizationParams);
					Blitter.BlitTexture(context.cmd, innerPassData.dummy, new Vector4(1f, 1f, 0f, 0f), innerPassData.material, 0);
				});
			}
		}

		private static void ConversionDispatch(ComputeCommandBuffer cmd, Vrs.ConversionPassData conversionPassData)
		{
			LocalKeyword localKeyword = new LocalKeyword(conversionPassData.computeShader, "DISABLE_TEXTURE2D_X_ARRAY");
			if (conversionPassData.mainTexDimension == TextureDimension.Tex2DArray)
			{
				cmd.DisableKeyword(conversionPassData.computeShader, localKeyword);
			}
			else
			{
				cmd.EnableKeyword(conversionPassData.computeShader, localKeyword);
			}
			LocalKeyword localKeyword2 = new LocalKeyword(conversionPassData.computeShader, "APPLY_Y_FLIP");
			if (conversionPassData.yFlip)
			{
				cmd.EnableKeyword(conversionPassData.computeShader, localKeyword2);
			}
			else
			{
				cmd.DisableKeyword(conversionPassData.computeShader, localKeyword2);
			}
			cmd.SetComputeTextureParam(conversionPassData.computeShader, conversionPassData.kernelIndex, VrsShaders.s_MainTex, conversionPassData.mainTexHandle);
			cmd.SetComputeBufferParam(conversionPassData.computeShader, conversionPassData.kernelIndex, VrsShaders.s_MainTexLut, conversionPassData.mainTexLutHandle);
			cmd.SetComputeBufferParam(conversionPassData.computeShader, conversionPassData.kernelIndex, VrsShaders.s_ShadingRateNativeValues, conversionPassData.validatedShadingRateFragmentSizeHandle);
			cmd.SetComputeTextureParam(conversionPassData.computeShader, conversionPassData.kernelIndex, VrsShaders.s_ShadingRateImage, conversionPassData.sriTextureHandle);
			cmd.SetComputeVectorParam(conversionPassData.computeShader, VrsShaders.s_ScaleBias, conversionPassData.scaleBias);
			cmd.DispatchCompute(conversionPassData.computeShader, conversionPassData.kernelIndex, conversionPassData.dispatchSize.x, conversionPassData.dispatchSize.y, 1);
		}

		public static void ColorMaskTextureToShadingRateImageDispatch(CommandBuffer cmd, RTHandle sriDestination, Texture colorMaskSource, bool yFlip = true)
		{
			if (sriDestination == null)
			{
				Debug.LogError("ColorMaskTextureToShadingRateImageDispatch: VRS destination shading rate texture is null.");
				return;
			}
			if (colorMaskSource == null)
			{
				Debug.LogError("ColorMaskTextureToShadingRateImageDispatch: VRS source color texture is null.");
				return;
			}
			if (!Vrs.IsInitialized())
			{
				Debug.LogError("ColorMaskTextureToShadingRateImageDispatch: VRS is not initialized.");
				return;
			}
			ComputeShader textureComputeShader = Vrs.s_VrsResources.textureComputeShader;
			int textureReduceKernel = Vrs.s_VrsResources.textureReduceKernel;
			GraphicsBuffer conversionLutBuffer = Vrs.s_VrsResources.conversionLutBuffer;
			GraphicsBuffer validatedShadingRateFragmentSizeBuffer = Vrs.s_VrsResources.validatedShadingRateFragmentSizeBuffer;
			int width = sriDestination.rt.width;
			int height = sriDestination.rt.height;
			Vector4 val = new Vector4
			{
				x = 1f / (float)(width * Vrs.s_VrsResources.tileSize.x),
				y = 1f / (float)(height * Vrs.s_VrsResources.tileSize.y),
				z = (float)width,
				w = (float)height
			};
			Vector2Int vector2Int = new Vector2Int(width, height);
			LocalKeyword localKeyword = new LocalKeyword(textureComputeShader, "DISABLE_TEXTURE2D_X_ARRAY");
			if (colorMaskSource != null && colorMaskSource.dimension == TextureDimension.Tex2DArray)
			{
				cmd.DisableKeyword(textureComputeShader, localKeyword);
			}
			else
			{
				cmd.EnableKeyword(textureComputeShader, localKeyword);
			}
			LocalKeyword localKeyword2 = new LocalKeyword(textureComputeShader, "APPLY_Y_FLIP");
			if (yFlip)
			{
				cmd.EnableKeyword(textureComputeShader, localKeyword2);
			}
			else
			{
				cmd.DisableKeyword(textureComputeShader, localKeyword2);
			}
			cmd.SetComputeTextureParam(textureComputeShader, textureReduceKernel, VrsShaders.s_MainTex, colorMaskSource);
			cmd.SetComputeBufferParam(textureComputeShader, textureReduceKernel, VrsShaders.s_MainTexLut, conversionLutBuffer);
			cmd.SetComputeBufferParam(textureComputeShader, textureReduceKernel, VrsShaders.s_ShadingRateNativeValues, validatedShadingRateFragmentSizeBuffer);
			cmd.SetComputeTextureParam(textureComputeShader, textureReduceKernel, VrsShaders.s_ShadingRateImage, sriDestination);
			cmd.SetComputeVectorParam(textureComputeShader, VrsShaders.s_ScaleBias, val);
			cmd.DispatchCompute(textureComputeShader, textureReduceKernel, vector2Int.x, vector2Int.y, 1);
		}

		public static void ShadingRateImageToColorMaskTextureBlit(CommandBuffer cmd, RTHandle sriSource, RTHandle colorMaskDestination)
		{
			if (sriSource == null)
			{
				Debug.LogError("ShadingRateImageToColorMaskTextureBlit: VRS source shading rate texture is null.");
				return;
			}
			if (colorMaskDestination == null)
			{
				Debug.LogError("ShadingRateImageToColorMaskTextureBlit: VRS destination color texture is null.");
				return;
			}
			if (!Vrs.IsInitialized())
			{
				Debug.LogError("ShadingRateImageToColorMaskTextureBlit: VRS is not initialized.");
				return;
			}
			Material visualizationMaterial = Vrs.s_VrsResources.visualizationMaterial;
			GraphicsBuffer visualizationLutBuffer = Vrs.s_VrsResources.visualizationLutBuffer;
			Vector4 value = new Vector4(1f / (float)Vrs.s_VrsResources.tileSize.x, 1f / (float)Vrs.s_VrsResources.tileSize.y, 0f, 0f);
			visualizationMaterial.SetTexture(VrsShaders.s_ShadingRateImage, sriSource);
			visualizationMaterial.SetBuffer(VrsShaders.s_VisualizationLut, visualizationLutBuffer);
			visualizationMaterial.SetVector(VrsShaders.s_VisualizationParams, value);
			CoreUtils.SetRenderTarget(cmd, colorMaskDestination, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.BlitTexture(cmd, new Vector4(1f, 1f, 0f, 0f), visualizationMaterial, 0);
		}

		internal static readonly int shadingRateFragmentSizeCount = Enum.GetNames(typeof(ShadingRateFragmentSize)).Length;

		private static VrsResources s_VrsResources;

		private class ConversionPassData
		{
			public TextureHandle sriTextureHandle;

			public TextureHandle mainTexHandle;

			public TextureDimension mainTexDimension;

			public BufferHandle mainTexLutHandle;

			public BufferHandle validatedShadingRateFragmentSizeHandle;

			public ComputeShader computeShader;

			public int kernelIndex;

			public Vector4 scaleBias;

			public Vector2Int dispatchSize;

			public bool yFlip;
		}

		private class VisualizationPassData
		{
			public Material material;

			public TextureHandle source;

			public BufferHandle lut;

			public TextureHandle dummy;

			public Vector4 visualizationParams;
		}
	}
}
