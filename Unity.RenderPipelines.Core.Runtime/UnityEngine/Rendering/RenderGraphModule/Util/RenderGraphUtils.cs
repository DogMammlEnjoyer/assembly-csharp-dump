using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RenderGraphModule.Util
{
	public static class RenderGraphUtils
	{
		public static bool CanAddCopyPassMSAA()
		{
			return RenderGraphUtils.IsFramebufferFetchEmulationMSAASupportedOnCurrentPlatform() && Blitter.CanCopyMSAA();
		}

		public static bool CanAddCopyPassMSAA(in TextureDesc sourceDesc)
		{
			return RenderGraphUtils.IsFramebufferFetchEmulationMSAASupportedOnCurrentPlatform() && Blitter.CanCopyMSAA(sourceDesc);
		}

		internal static bool IsFramebufferFetchEmulationSupportedOnCurrentPlatform()
		{
			return true;
		}

		internal static bool IsFramebufferFetchEmulationMSAASupportedOnCurrentPlatform()
		{
			return SystemInfo.graphicsDeviceType != GraphicsDeviceType.PlayStation4 && SystemInfo.graphicsDeviceType != GraphicsDeviceType.PlayStation5 && SystemInfo.graphicsDeviceType != GraphicsDeviceType.PlayStation5NGGC;
		}

		public static bool IsFramebufferFetchSupportedOnCurrentPlatform(this RenderGraph graph, in TextureHandle tex)
		{
			if (!RenderGraphUtils.IsFramebufferFetchEmulationSupportedOnCurrentPlatform())
			{
				return false;
			}
			if (!RenderGraphUtils.IsFramebufferFetchEmulationMSAASupportedOnCurrentPlatform())
			{
				RenderTargetInfo renderTargetInfo = graph.GetRenderTargetInfo(tex);
				if (renderTargetInfo.msaaSamples > 1)
				{
					return renderTargetInfo.bindMS;
				}
			}
			return true;
		}

		public static void AddCopyPass(this RenderGraph graph, TextureHandle source, TextureHandle destination, string passName = "Copy Pass Utility", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			if (!graph.nativeRenderPassesEnabled)
			{
				throw new ArgumentException("CopyPass only supported for native render pass. Please use the blit functions instead for non native render pass platforms.");
			}
			TextureDesc textureDesc = graph.GetTextureDesc(source);
			TextureDesc textureDesc2 = graph.GetTextureDesc(destination);
			if (textureDesc.msaaSamples != textureDesc2.msaaSamples)
			{
				throw new ArgumentException("MSAA samples from source and destination texture doesn't match.");
			}
			if (textureDesc.width != textureDesc2.width || textureDesc.height != textureDesc2.height)
			{
				throw new ArgumentException("Dimensions for source and destination texture doesn't match.");
			}
			if (textureDesc.slices != textureDesc2.slices)
			{
				throw new ArgumentException("Slice count for source and destination texture doesn't match.");
			}
			bool flag = textureDesc.msaaSamples > MSAASamples.None;
			if (flag && !RenderGraphUtils.CanAddCopyPassMSAA(textureDesc))
			{
				throw new ArgumentException("Target does not support MSAA for AddCopyPass. Please use the blit alternative or use non MSAA textures.");
			}
			RenderGraphUtils.CopyPassData copyPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<RenderGraphUtils.CopyPassData>(passName, out copyPassData, file, line))
			{
				copyPassData.isMSAA = flag;
				bool useTexArray = TextureXR.useTexArray;
				bool flag2 = textureDesc.slices > 1;
				copyPassData.force2DForXR = (useTexArray && !flag2);
				rasterRenderGraphBuilder.SetInputAttachment(source, 0, AccessFlags.Read);
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderFunc<RenderGraphUtils.CopyPassData>(delegate(RenderGraphUtils.CopyPassData data, RasterGraphContext context)
				{
					RenderGraphUtils.CopyRenderFunc(data, context);
				});
				if (copyPassData.force2DForXR)
				{
					rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				}
			}
		}

		public static void AddCopyPass(this RenderGraph graph, TextureHandle source, TextureHandle destination, int sourceSlice, int destinationSlice = 0, int sourceMip = 0, int destinationMip = 0, string passName = "Copy Pass Utility", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			graph.AddCopyPass(source, destination, passName, file, line);
		}

		private static void CopyRenderFunc(RenderGraphUtils.CopyPassData data, RasterGraphContext rgContext)
		{
			Blitter.CopyTexture(rgContext.cmd, data.isMSAA, data.force2DForXR);
		}

		internal static bool IsTextureXR(ref TextureDesc destDesc, int sourceSlice, int destinationSlice, int numSlices, int numMips)
		{
			return TextureXR.useTexArray && destDesc.dimension == TextureDimension.Tex2DArray && destDesc.slices == TextureXR.slices && sourceSlice == 0 && destinationSlice == 0 && numSlices == TextureXR.slices && numMips == 1;
		}

		public static void AddBlitPass(this RenderGraph graph, TextureHandle source, TextureHandle destination, Vector2 scale, Vector2 offset, int sourceSlice = 0, int destinationSlice = 0, int numSlices = -1, int sourceMip = 0, int destinationMip = 0, int numMips = 1, RenderGraphUtils.BlitFilterMode filterMode = RenderGraphUtils.BlitFilterMode.ClampBilinear, string passName = "Blit Pass Utility", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			TextureDesc textureDesc = graph.GetTextureDesc(source);
			TextureDesc textureDesc2 = graph.GetTextureDesc(destination);
			int num = (int)math.log2((float)math.max(math.max(textureDesc.width, textureDesc.height), textureDesc.slices)) + 1;
			int num2 = (int)math.log2((float)math.max(math.max(textureDesc2.width, textureDesc2.height), textureDesc2.slices)) + 1;
			if (numSlices == -1)
			{
				numSlices = textureDesc.slices - sourceSlice;
			}
			if (numSlices > textureDesc.slices - sourceSlice || numSlices > textureDesc2.slices - destinationSlice)
			{
				throw new ArgumentException("BlitPass: " + passName + " attempts to blit too many slices. The pass will be skipped.");
			}
			if (numMips == -1)
			{
				numMips = num - sourceMip;
			}
			if (numMips > num - sourceMip || numMips > num2 - destinationMip)
			{
				throw new ArgumentException("BlitPass: " + passName + " attempts to blit too many mips. The pass will be skipped.");
			}
			RenderGraphUtils.BlitPassData blitPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = graph.AddUnsafePass<RenderGraphUtils.BlitPassData>(passName, out blitPassData, file, line))
			{
				blitPassData.source = source;
				blitPassData.destination = destination;
				blitPassData.scale = scale;
				blitPassData.offset = offset;
				blitPassData.sourceSlice = sourceSlice;
				blitPassData.destinationSlice = destinationSlice;
				blitPassData.numSlices = numSlices;
				blitPassData.sourceMip = sourceMip;
				blitPassData.destinationMip = destinationMip;
				blitPassData.numMips = numMips;
				blitPassData.filterMode = filterMode;
				blitPassData.isXR = RenderGraphUtils.IsTextureXR(ref textureDesc2, sourceSlice, destinationSlice, numSlices, numMips);
				unsafeRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				unsafeRenderGraphBuilder.UseTexture(destination, AccessFlags.Write);
				unsafeRenderGraphBuilder.SetRenderFunc<RenderGraphUtils.BlitPassData>(delegate(RenderGraphUtils.BlitPassData data, UnsafeGraphContext context)
				{
					RenderGraphUtils.BlitRenderFunc(data, context);
				});
			}
		}

		private static void BlitRenderFunc(RenderGraphUtils.BlitPassData data, UnsafeGraphContext context)
		{
			RenderGraphUtils.s_BlitScaleBias.x = data.scale.x;
			RenderGraphUtils.s_BlitScaleBias.y = data.scale.y;
			RenderGraphUtils.s_BlitScaleBias.z = data.offset.x;
			RenderGraphUtils.s_BlitScaleBias.w = data.offset.y;
			CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
			if (data.isXR)
			{
				context.cmd.SetRenderTarget(data.destination, 0, CubemapFace.Unknown, -1);
				Blitter.BlitTexture(nativeCommandBuffer, data.source, RenderGraphUtils.s_BlitScaleBias, (float)data.sourceMip, data.filterMode == RenderGraphUtils.BlitFilterMode.ClampBilinear);
				return;
			}
			for (int i = 0; i < data.numSlices; i++)
			{
				for (int j = 0; j < data.numMips; j++)
				{
					context.cmd.SetRenderTarget(data.destination, data.destinationMip + j, CubemapFace.Unknown, data.destinationSlice + i);
					Blitter.BlitTexture(nativeCommandBuffer, data.source, RenderGraphUtils.s_BlitScaleBias, (float)(data.sourceMip + j), data.sourceSlice + i, data.filterMode == RenderGraphUtils.BlitFilterMode.ClampBilinear);
				}
			}
		}

		public static void AddBlitPass(this RenderGraph graph, RenderGraphUtils.BlitMaterialParameters blitParameters, string passName = "Blit Pass Utility w. Material", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			if (!blitParameters.destination.IsValid())
			{
				throw new ArgumentException("BlitPass: " + passName + " destination needs to be a valid texture handle.");
			}
			TextureDesc textureDesc = graph.GetTextureDesc(blitParameters.destination);
			int num = (int)math.log2((float)math.max(math.max(textureDesc.width, textureDesc.height), textureDesc.slices)) + 1;
			if (blitParameters.numSlices == -1)
			{
				blitParameters.numSlices = textureDesc.slices - blitParameters.destinationSlice;
			}
			if (blitParameters.numMips == -1)
			{
				blitParameters.numMips = num - blitParameters.destinationMip;
			}
			if (blitParameters.source.IsValid())
			{
				TextureDesc textureDesc2 = graph.GetTextureDesc(blitParameters.source);
				int num2 = (int)math.log2((float)math.max(math.max(textureDesc2.width, textureDesc2.height), textureDesc2.slices)) + 1;
				if (blitParameters.sourceSlice != -1 && blitParameters.numSlices > textureDesc2.slices - blitParameters.sourceSlice)
				{
					throw new ArgumentException("BlitPass: " + passName + " attempts to blit too many slices. There are not enough slices in the source array. The pass will be skipped.");
				}
				if (blitParameters.sourceMip != -1 && blitParameters.numMips > num2 - blitParameters.sourceMip)
				{
					throw new ArgumentException("BlitPass: " + passName + " attempts to blit too many mips. There are not enough mips in the source texture. The pass will be skipped.");
				}
			}
			if (blitParameters.numSlices > textureDesc.slices - blitParameters.destinationSlice)
			{
				throw new ArgumentException("BlitPass: " + passName + " attempts to blit too many slices. There are not enough slices in the destination array. The pass will be skipped.");
			}
			if (blitParameters.numMips > num - blitParameters.destinationMip)
			{
				throw new ArgumentException("BlitPass: " + passName + " attempts to blit too many mips. There are not enough mips in the destination texture. The pass will be skipped.");
			}
			RenderGraphUtils.BlitMaterialPassData blitMaterialPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = graph.AddUnsafePass<RenderGraphUtils.BlitMaterialPassData>(passName, out blitMaterialPassData, file, line))
			{
				blitMaterialPassData.sourceTexturePropertyID = blitParameters.sourceTexturePropertyID;
				blitMaterialPassData.source = blitParameters.source;
				blitMaterialPassData.destination = blitParameters.destination;
				blitMaterialPassData.scale = blitParameters.scale;
				blitMaterialPassData.offset = blitParameters.offset;
				blitMaterialPassData.material = blitParameters.material;
				blitMaterialPassData.shaderPass = blitParameters.shaderPass;
				blitMaterialPassData.propertyBlock = blitParameters.propertyBlock;
				blitMaterialPassData.sourceSlice = blitParameters.sourceSlice;
				blitMaterialPassData.destinationSlice = blitParameters.destinationSlice;
				blitMaterialPassData.numSlices = blitParameters.numSlices;
				blitMaterialPassData.sourceMip = blitParameters.sourceMip;
				blitMaterialPassData.destinationMip = blitParameters.destinationMip;
				blitMaterialPassData.numMips = blitParameters.numMips;
				blitMaterialPassData.geometry = blitParameters.geometry;
				blitMaterialPassData.sourceSlicePropertyID = blitParameters.sourceSlicePropertyID;
				blitMaterialPassData.sourceMipPropertyID = blitParameters.sourceMipPropertyID;
				blitMaterialPassData.scaleBiasPropertyID = blitParameters.scaleBiasPropertyID;
				blitMaterialPassData.isXR = RenderGraphUtils.IsTextureXR(ref textureDesc, (blitMaterialPassData.sourceSlice == -1) ? 0 : blitMaterialPassData.sourceSlice, blitMaterialPassData.destinationSlice, blitMaterialPassData.numSlices, blitMaterialPassData.numMips);
				if (blitParameters.source.IsValid())
				{
					unsafeRenderGraphBuilder.UseTexture(blitParameters.source, AccessFlags.Read);
				}
				unsafeRenderGraphBuilder.UseTexture(blitParameters.destination, AccessFlags.Write);
				unsafeRenderGraphBuilder.SetRenderFunc<RenderGraphUtils.BlitMaterialPassData>(delegate(RenderGraphUtils.BlitMaterialPassData data, UnsafeGraphContext context)
				{
					RenderGraphUtils.BlitMaterialRenderFunc(data, context);
				});
			}
		}

		private static void BlitMaterialRenderFunc(RenderGraphUtils.BlitMaterialPassData data, UnsafeGraphContext context)
		{
			RenderGraphUtils.s_BlitScaleBias.x = data.scale.x;
			RenderGraphUtils.s_BlitScaleBias.y = data.scale.y;
			RenderGraphUtils.s_BlitScaleBias.z = data.offset.x;
			RenderGraphUtils.s_BlitScaleBias.w = data.offset.y;
			CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
			if (data.propertyBlock == null)
			{
				data.propertyBlock = RenderGraphUtils.s_PropertyBlock;
			}
			if (data.source.IsValid())
			{
				data.propertyBlock.SetTexture(data.sourceTexturePropertyID, data.source);
			}
			data.propertyBlock.SetVector(data.scaleBiasPropertyID, RenderGraphUtils.s_BlitScaleBias);
			if (!data.isXR)
			{
				for (int i = 0; i < data.numSlices; i++)
				{
					for (int j = 0; j < data.numMips; j++)
					{
						if (data.sourceSlice != -1)
						{
							data.propertyBlock.SetInt(data.sourceSlicePropertyID, data.sourceSlice + i);
						}
						if (data.sourceMip != -1)
						{
							data.propertyBlock.SetInt(data.sourceMipPropertyID, data.sourceMip + j);
						}
						context.cmd.SetRenderTarget(data.destination, data.destinationMip + j, CubemapFace.Unknown, data.destinationSlice + i);
						switch (data.geometry)
						{
						case RenderGraphUtils.FullScreenGeometryType.Mesh:
							Blitter.DrawQuadMesh(nativeCommandBuffer, data.material, data.shaderPass, data.propertyBlock);
							break;
						case RenderGraphUtils.FullScreenGeometryType.ProceduralTriangle:
							Blitter.DrawTriangle(nativeCommandBuffer, data.material, data.shaderPass, data.propertyBlock);
							break;
						case RenderGraphUtils.FullScreenGeometryType.ProceduralQuad:
							Blitter.DrawQuad(nativeCommandBuffer, data.material, data.shaderPass, data.propertyBlock);
							break;
						}
					}
				}
				return;
			}
			if (data.sourceSlice != -1)
			{
				data.propertyBlock.SetInt(data.sourceSlicePropertyID, 0);
			}
			if (data.sourceMip != -1)
			{
				data.propertyBlock.SetInt(data.sourceMipPropertyID, data.sourceMip);
			}
			context.cmd.SetRenderTarget(data.destination, 0, CubemapFace.Unknown, -1);
			switch (data.geometry)
			{
			case RenderGraphUtils.FullScreenGeometryType.Mesh:
				Blitter.DrawQuadMesh(nativeCommandBuffer, data.material, data.shaderPass, data.propertyBlock);
				return;
			case RenderGraphUtils.FullScreenGeometryType.ProceduralTriangle:
				Blitter.DrawTriangle(nativeCommandBuffer, data.material, data.shaderPass, data.propertyBlock);
				return;
			case RenderGraphUtils.FullScreenGeometryType.ProceduralQuad:
				Blitter.DrawQuad(nativeCommandBuffer, data.material, data.shaderPass, data.propertyBlock);
				return;
			default:
				return;
			}
		}

		private static MaterialPropertyBlock s_PropertyBlock = new MaterialPropertyBlock();

		private const string DisableTexture2DXArray = "DISABLE_TEXTURE2D_X_ARRAY";

		private static Vector4 s_BlitScaleBias = default(Vector4);

		private class CopyPassData
		{
			public bool isMSAA;

			public bool force2DForXR;
		}

		public enum BlitFilterMode
		{
			ClampNearest,
			ClampBilinear
		}

		private class BlitPassData
		{
			public TextureHandle source;

			public TextureHandle destination;

			public Vector2 scale;

			public Vector2 offset;

			public int sourceSlice;

			public int destinationSlice;

			public int numSlices;

			public int sourceMip;

			public int destinationMip;

			public int numMips;

			public RenderGraphUtils.BlitFilterMode filterMode;

			public bool isXR;
		}

		public enum FullScreenGeometryType
		{
			Mesh,
			ProceduralTriangle,
			ProceduralQuad
		}

		public struct BlitMaterialParameters
		{
			public BlitMaterialParameters(TextureHandle source, TextureHandle destination, Material material, int shaderPass)
			{
				this = new RenderGraphUtils.BlitMaterialParameters(source, destination, Vector2.one, Vector2.zero, material, shaderPass);
			}

			public BlitMaterialParameters(TextureHandle source, TextureHandle destination, Vector2 scale, Vector2 offset, Material material, int shaderPass)
			{
				this.source = source;
				this.destination = destination;
				this.scale = scale;
				this.offset = offset;
				this.sourceSlice = -1;
				this.destinationSlice = 0;
				this.numSlices = -1;
				this.sourceMip = -1;
				this.destinationMip = 0;
				this.numMips = 1;
				this.material = material;
				this.shaderPass = shaderPass;
				this.propertyBlock = null;
				this.sourceTexturePropertyID = RenderGraphUtils.BlitMaterialParameters.blitTextureProperty;
				this.sourceSlicePropertyID = RenderGraphUtils.BlitMaterialParameters.blitSliceProperty;
				this.sourceMipPropertyID = RenderGraphUtils.BlitMaterialParameters.blitMipProperty;
				this.scaleBiasPropertyID = RenderGraphUtils.BlitMaterialParameters.blitScaleBias;
				this.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralTriangle;
			}

			public BlitMaterialParameters(TextureHandle source, TextureHandle destination, Material material, int shaderPass, MaterialPropertyBlock mpb, int destinationSlice, int destinationMip, int numSlices = -1, int numMips = 1, int sourceSlice = -1, int sourceMip = -1, RenderGraphUtils.FullScreenGeometryType geometry = RenderGraphUtils.FullScreenGeometryType.Mesh, int sourceTexturePropertyID = -1, int sourceSlicePropertyID = -1, int sourceMipPropertyID = -1)
			{
				this = new RenderGraphUtils.BlitMaterialParameters(source, destination, Vector2.one, Vector2.zero, material, shaderPass, mpb, destinationSlice, destinationMip, numSlices, numMips, sourceSlice, sourceMip, geometry, sourceTexturePropertyID, sourceSlicePropertyID, sourceMipPropertyID, -1);
			}

			public BlitMaterialParameters(TextureHandle source, TextureHandle destination, Vector2 scale, Vector2 offset, Material material, int shaderPass, MaterialPropertyBlock mpb, int destinationSlice, int destinationMip, int numSlices = -1, int numMips = 1, int sourceSlice = -1, int sourceMip = -1, RenderGraphUtils.FullScreenGeometryType geometry = RenderGraphUtils.FullScreenGeometryType.Mesh, int sourceTexturePropertyID = -1, int sourceSlicePropertyID = -1, int sourceMipPropertyID = -1, int scaleBiasPropertyID = -1)
			{
				this = new RenderGraphUtils.BlitMaterialParameters(source, destination, scale, offset, material, shaderPass);
				this.propertyBlock = mpb;
				this.sourceSlice = sourceSlice;
				this.destinationSlice = destinationSlice;
				this.numSlices = numSlices;
				this.sourceMip = sourceMip;
				this.destinationMip = destinationMip;
				this.numMips = numMips;
				if (sourceTexturePropertyID != -1)
				{
					this.sourceTexturePropertyID = sourceTexturePropertyID;
				}
				if (sourceSlicePropertyID != -1)
				{
					this.sourceSlicePropertyID = sourceSlicePropertyID;
				}
				if (sourceMipPropertyID != -1)
				{
					this.sourceMipPropertyID = sourceMipPropertyID;
				}
				if (scaleBiasPropertyID != -1)
				{
					this.scaleBiasPropertyID = scaleBiasPropertyID;
				}
				this.geometry = geometry;
			}

			public BlitMaterialParameters(TextureHandle source, TextureHandle destination, Material material, int shaderPass, MaterialPropertyBlock mpb, RenderGraphUtils.FullScreenGeometryType geometry = RenderGraphUtils.FullScreenGeometryType.Mesh, int sourceTexturePropertyID = -1, int sourceSlicePropertyID = -1, int sourceMipPropertyID = -1)
			{
				this = new RenderGraphUtils.BlitMaterialParameters(source, destination, Vector2.one, Vector2.zero, material, shaderPass, mpb, geometry, sourceTexturePropertyID, sourceSlicePropertyID, sourceMipPropertyID, -1);
			}

			public BlitMaterialParameters(TextureHandle source, TextureHandle destination, Vector2 scale, Vector2 offset, Material material, int shaderPass, MaterialPropertyBlock mpb, RenderGraphUtils.FullScreenGeometryType geometry = RenderGraphUtils.FullScreenGeometryType.Mesh, int sourceTexturePropertyID = -1, int sourceSlicePropertyID = -1, int sourceMipPropertyID = -1, int scaleBiasPropertyID = -1)
			{
				this = new RenderGraphUtils.BlitMaterialParameters(source, destination, scale, offset, material, shaderPass);
				this.propertyBlock = mpb;
				if (sourceTexturePropertyID != -1)
				{
					this.sourceTexturePropertyID = sourceTexturePropertyID;
				}
				if (sourceSlicePropertyID != -1)
				{
					this.sourceSlicePropertyID = sourceSlicePropertyID;
				}
				if (sourceMipPropertyID != -1)
				{
					this.sourceMipPropertyID = sourceMipPropertyID;
				}
				if (scaleBiasPropertyID != -1)
				{
					this.scaleBiasPropertyID = scaleBiasPropertyID;
				}
				this.geometry = geometry;
			}

			private static readonly int blitTextureProperty = Shader.PropertyToID("_BlitTexture");

			private static readonly int blitSliceProperty = Shader.PropertyToID("_BlitTexArraySlice");

			private static readonly int blitMipProperty = Shader.PropertyToID("_BlitMipLevel");

			private static readonly int blitScaleBias = Shader.PropertyToID("_BlitScaleBias");

			public TextureHandle source;

			public TextureHandle destination;

			public Vector2 scale;

			public Vector2 offset;

			public int sourceSlice;

			public int destinationSlice;

			public int numSlices;

			public int sourceMip;

			public int destinationMip;

			public int numMips;

			public Material material;

			public int shaderPass;

			public MaterialPropertyBlock propertyBlock;

			public int sourceTexturePropertyID;

			public int sourceSlicePropertyID;

			public int sourceMipPropertyID;

			public int scaleBiasPropertyID;

			public RenderGraphUtils.FullScreenGeometryType geometry;
		}

		private class BlitMaterialPassData
		{
			public int sourceTexturePropertyID;

			public TextureHandle source;

			public TextureHandle destination;

			public Vector2 scale;

			public Vector2 offset;

			public Material material;

			public int shaderPass;

			public MaterialPropertyBlock propertyBlock;

			public int sourceSlice;

			public int destinationSlice;

			public int numSlices;

			public int sourceMip;

			public int destinationMip;

			public int numMips;

			public RenderGraphUtils.FullScreenGeometryType geometry;

			public int sourceSlicePropertyID;

			public int sourceMipPropertyID;

			public int scaleBiasPropertyID;

			public bool isXR;
		}
	}
}
