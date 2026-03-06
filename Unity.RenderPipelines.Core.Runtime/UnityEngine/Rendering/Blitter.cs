using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace UnityEngine.Rendering
{
	public static class Blitter
	{
		public static void Initialize(Shader blitPS, Shader blitColorAndDepthPS)
		{
			if (Blitter.s_Blit != null)
			{
				throw new Exception("Blitter is already initialized. Please only initialize the blitter once or you will leak engine resources. If you need to re-initialize the blitter with different shaders destroy & recreate it.");
			}
			Blitter.s_Copy = CoreUtils.CreateEngineMaterial(GraphicsSettings.GetRenderPipelineSettings<RenderGraphUtilsResources>().coreCopyPS);
			Blitter.s_Blit = CoreUtils.CreateEngineMaterial(blitPS);
			Blitter.s_BlitColorAndDepth = CoreUtils.CreateEngineMaterial(blitColorAndDepthPS);
			Blitter.s_DecodeHdrKeyword = new LocalKeyword(blitPS, "BLIT_DECODE_HDR");
			if (TextureXR.useTexArray)
			{
				Blitter.s_Blit.EnableKeyword("DISABLE_TEXTURE2D_X_ARRAY");
				Blitter.s_BlitTexArray = CoreUtils.CreateEngineMaterial(blitPS);
				Blitter.s_BlitTexArraySingleSlice = CoreUtils.CreateEngineMaterial(blitPS);
				Blitter.s_BlitTexArraySingleSlice.EnableKeyword("BLIT_SINGLE_SLICE");
			}
			float z = -1f;
			if (SystemInfo.usesReversedZBuffer)
			{
				z = 1f;
			}
			if (SystemInfo.graphicsShaderLevel < 30 && !Blitter.s_TriangleMesh)
			{
				Blitter.s_TriangleMesh = new Mesh();
				Blitter.s_TriangleMesh.vertices = Blitter.<Initialize>g__GetFullScreenTriangleVertexPosition|14_0(z);
				Blitter.s_TriangleMesh.uv = Blitter.<Initialize>g__GetFullScreenTriangleTexCoord|14_1();
				Blitter.s_TriangleMesh.triangles = new int[]
				{
					0,
					1,
					2
				};
			}
			if (!Blitter.s_QuadMesh)
			{
				Blitter.s_QuadMesh = new Mesh();
				Blitter.s_QuadMesh.vertices = Blitter.<Initialize>g__GetQuadVertexPosition|14_2(z);
				Blitter.s_QuadMesh.uv = Blitter.<Initialize>g__GetQuadTexCoord|14_3();
				Blitter.s_QuadMesh.triangles = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3
				};
			}
			string[] names = Enum.GetNames(typeof(Blitter.BlitShaderPassNames));
			Blitter.s_BlitShaderPassIndicesMap = new int[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				Blitter.s_BlitShaderPassIndicesMap[i] = Blitter.s_Blit.FindPass(names[i]);
			}
			names = Enum.GetNames(typeof(Blitter.BlitColorAndDepthPassNames));
			Blitter.s_BlitColorAndDepthShaderPassIndicesMap = new int[names.Length];
			for (int j = 0; j < names.Length; j++)
			{
				Blitter.s_BlitColorAndDepthShaderPassIndicesMap[j] = Blitter.s_BlitColorAndDepth.FindPass(names[j]);
			}
		}

		public static void Cleanup()
		{
			CoreUtils.Destroy(Blitter.s_Copy);
			Blitter.s_Copy = null;
			CoreUtils.Destroy(Blitter.s_Blit);
			Blitter.s_Blit = null;
			CoreUtils.Destroy(Blitter.s_BlitColorAndDepth);
			Blitter.s_BlitColorAndDepth = null;
			CoreUtils.Destroy(Blitter.s_BlitTexArray);
			Blitter.s_BlitTexArray = null;
			CoreUtils.Destroy(Blitter.s_BlitTexArraySingleSlice);
			Blitter.s_BlitTexArraySingleSlice = null;
			CoreUtils.Destroy(Blitter.s_TriangleMesh);
			Blitter.s_TriangleMesh = null;
			CoreUtils.Destroy(Blitter.s_QuadMesh);
			Blitter.s_QuadMesh = null;
		}

		public static Material GetBlitMaterial(TextureDimension dimension, bool singleSlice = false)
		{
			Material material = (dimension == TextureDimension.Tex2DArray) ? (singleSlice ? Blitter.s_BlitTexArraySingleSlice : Blitter.s_BlitTexArray) : null;
			if (!(material == null))
			{
				return material;
			}
			return Blitter.s_Blit;
		}

		internal static void DrawTriangle(RasterCommandBuffer cmd, Material material, int shaderPass)
		{
			Blitter.DrawTriangle(cmd.m_WrappedCommandBuffer, material, shaderPass);
		}

		internal static void DrawTriangle(CommandBuffer cmd, Material material, int shaderPass)
		{
			Blitter.DrawTriangle(cmd, material, shaderPass, Blitter.s_PropertyBlock);
		}

		internal static void DrawTriangle(CommandBuffer cmd, Material material, int shaderPass, MaterialPropertyBlock propertyBlock)
		{
			if (SystemInfo.graphicsShaderLevel < 30)
			{
				cmd.DrawMesh(Blitter.s_TriangleMesh, Matrix4x4.identity, material, 0, shaderPass, propertyBlock);
				return;
			}
			cmd.DrawProcedural(Matrix4x4.identity, material, shaderPass, MeshTopology.Triangles, 3, 1, propertyBlock);
		}

		internal static void DrawQuadMesh(CommandBuffer cmd, Material material, int shaderPass, MaterialPropertyBlock propertyBlock)
		{
			cmd.DrawMesh(Blitter.s_QuadMesh, Matrix4x4.identity, material, 0, shaderPass, propertyBlock);
		}

		internal static void DrawQuad(RasterCommandBuffer cmd, Material material, int shaderPass, MaterialPropertyBlock propertyBlock)
		{
			Blitter.DrawQuad(cmd.m_WrappedCommandBuffer, material, shaderPass, propertyBlock);
		}

		internal static void DrawQuad(CommandBuffer cmd, Material material, int shaderPass)
		{
			Blitter.DrawQuad(cmd, material, shaderPass, Blitter.s_PropertyBlock);
		}

		internal static void DrawQuad(CommandBuffer cmd, Material material, int shaderPass, MaterialPropertyBlock propertyBlock)
		{
			if (SystemInfo.graphicsShaderLevel < 30)
			{
				cmd.DrawMesh(Blitter.s_QuadMesh, Matrix4x4.identity, material, 0, shaderPass, propertyBlock);
				return;
			}
			cmd.DrawProcedural(Matrix4x4.identity, material, shaderPass, MeshTopology.Quads, 4, 1, propertyBlock);
		}

		internal static bool CanCopyMSAA()
		{
			return SystemInfo.graphicsDeviceType != GraphicsDeviceType.PlayStation4 && Blitter.s_Copy.passCount == 2;
		}

		internal static bool CanCopyMSAA(in TextureDesc sourceDesc)
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12;
			return (!SystemInfo.supportsMultisampleAutoResolve || flag || sourceDesc.bindTextureMS) && Blitter.CanCopyMSAA();
		}

		internal static void CopyTexture(RasterCommandBuffer cmd, bool isMSAA, bool force2DForXR = false)
		{
			if (force2DForXR)
			{
				cmd.EnableShaderKeyword("DISABLE_TEXTURE2D_X_ARRAY");
			}
			Blitter.DrawTriangle(cmd, Blitter.s_Copy, isMSAA ? 1 : 0);
			if (force2DForXR)
			{
				cmd.DisableShaderKeyword("DISABLE_TEXTURE2D_X_ARRAY");
			}
		}

		internal static void BlitTexture(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, float sourceMipLevel, int sourceDepthSlice, bool bilinear)
		{
			Blitter.BlitTexture(cmd, source, scaleBias, Blitter.GetBlitMaterial(TextureDimension.Tex2D, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 1 : 0], sourceMipLevel, sourceDepthSlice);
		}

		internal static void BlitTexture(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, Material material, int pass, float sourceMipLevel, int sourceDepthSlice)
		{
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, sourceMipLevel);
			Blitter.s_PropertyBlock.SetInt(Blitter.BlitShaderIDs._BlitTexArraySlice, sourceDepthSlice);
			Blitter.BlitTexture(cmd, source, scaleBias, material, pass);
		}

		public static void BlitTexture(RasterCommandBuffer cmd, RTHandle source, Vector4 scaleBias, float mipLevel, bool bilinear)
		{
			Blitter.BlitTexture(cmd.m_WrappedCommandBuffer, source, scaleBias, mipLevel, bilinear);
		}

		public static void BlitTexture(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, float mipLevel, bool bilinear)
		{
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, mipLevel);
			Blitter.BlitTexture(cmd, source, scaleBias, Blitter.GetBlitMaterial(TextureXR.dimension, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 1 : 0]);
		}

		public static void BlitTexture2D(RasterCommandBuffer cmd, RTHandle source, Vector4 scaleBias, float mipLevel, bool bilinear)
		{
			Blitter.BlitTexture2D(cmd.m_WrappedCommandBuffer, source, scaleBias, mipLevel, bilinear);
		}

		public static void BlitTexture2D(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, float mipLevel, bool bilinear)
		{
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, mipLevel);
			Blitter.BlitTexture(cmd, source, scaleBias, Blitter.GetBlitMaterial(TextureDimension.Tex2D, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 1 : 0]);
		}

		public static void BlitColorAndDepth(RasterCommandBuffer cmd, Texture sourceColor, RenderTexture sourceDepth, Vector4 scaleBias, float mipLevel, bool blitDepth)
		{
			Blitter.BlitColorAndDepth(cmd.m_WrappedCommandBuffer, sourceColor, sourceDepth, scaleBias, mipLevel, blitDepth);
		}

		public static void BlitColorAndDepth(CommandBuffer cmd, Texture sourceColor, RenderTexture sourceDepth, Vector4 scaleBias, float mipLevel, bool blitDepth)
		{
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, mipLevel);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBias);
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, sourceColor);
			if (blitDepth)
			{
				Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._InputDepth, sourceDepth, RenderTextureSubElement.Depth);
			}
			Blitter.DrawTriangle(cmd, Blitter.s_BlitColorAndDepth, Blitter.s_BlitColorAndDepthShaderPassIndicesMap[blitDepth ? 1 : 0]);
		}

		public static void BlitTexture(RasterCommandBuffer cmd, RTHandle source, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.BlitTexture(cmd.m_WrappedCommandBuffer, source, scaleBias, material, pass);
		}

		public static void BlitTexture(UnsafeCommandBuffer cmd, RTHandle source, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.BlitTexture(cmd.m_WrappedCommandBuffer, source, scaleBias, material, pass);
		}

		public static void BlitTexture(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBias);
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.DrawTriangle(cmd, material, pass);
		}

		public static void BlitTexture(RasterCommandBuffer cmd, RenderTargetIdentifier source, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.BlitTexture(cmd.m_WrappedCommandBuffer, source, scaleBias, material, pass);
		}

		public static void BlitTexture(CommandBuffer cmd, RenderTargetIdentifier source, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.s_PropertyBlock.Clear();
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBias);
			cmd.SetGlobalTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.DrawTriangle(cmd, material, pass);
		}

		public static void BlitTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass)
		{
			Blitter.s_PropertyBlock.Clear();
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, Vector2.one);
			cmd.SetGlobalTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			cmd.SetRenderTarget(destination);
			Blitter.DrawTriangle(cmd, material, pass);
		}

		public static void BlitTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, Material material, int pass)
		{
			Blitter.s_PropertyBlock.Clear();
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, Vector2.one);
			cmd.SetGlobalTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			cmd.SetRenderTarget(destination, loadAction, storeAction);
			Blitter.DrawTriangle(cmd, material, pass);
		}

		public static void BlitTexture(CommandBuffer cmd, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBias);
			Blitter.DrawTriangle(cmd, material, pass);
		}

		public static void BlitTexture(RasterCommandBuffer cmd, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBias);
			Blitter.DrawTriangle(cmd, material, pass);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, float mipLevel = 0f, bool bilinear = false)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destination, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.BlitTexture(cmd, source, v, mipLevel, bilinear);
		}

		public static void BlitCameraTexture2D(CommandBuffer cmd, RTHandle source, RTHandle destination, float mipLevel = 0f, bool bilinear = false)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destination, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.BlitTexture2D(cmd, source, v, mipLevel, bilinear);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, Material material, int pass)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destination, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.BlitTexture(cmd, source, v, material, pass);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, Material material, int pass)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destination, loadAction, storeAction, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			Blitter.BlitTexture(cmd, source, v, material, pass);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, Vector4 scaleBias, float mipLevel = 0f, bool bilinear = false)
		{
			CoreUtils.SetRenderTarget(cmd, destination, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			Blitter.BlitTexture(cmd, source, scaleBias, mipLevel, bilinear);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, Rect destViewport, float mipLevel = 0f, bool bilinear = false)
		{
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			CoreUtils.SetRenderTarget(cmd, destination, ClearFlag.None, 0, CubemapFace.Unknown, -1);
			cmd.SetViewport(destViewport);
			Blitter.BlitTexture(cmd, source, v, mipLevel, bilinear);
		}

		public static void BlitQuad(CommandBuffer cmd, Texture source, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear)
		{
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBiasTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 3 : 2]);
		}

		public static void BlitQuadWithPadding(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBiasTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitTextureSize, textureSize);
			Blitter.s_PropertyBlock.SetInt(Blitter.BlitShaderIDs._BlitPaddingSize, paddingInPixels);
			if (source.wrapMode == TextureWrapMode.Repeat)
			{
				Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 7 : 6]);
				return;
			}
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 5 : 4]);
		}

		public static void BlitQuadWithPaddingMultiply(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBiasTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitTextureSize, textureSize);
			Blitter.s_PropertyBlock.SetInt(Blitter.BlitShaderIDs._BlitPaddingSize, paddingInPixels);
			if (source.wrapMode == TextureWrapMode.Repeat)
			{
				Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 12 : 11]);
				return;
			}
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[bilinear ? 10 : 9]);
		}

		public static void BlitOctahedralWithPadding(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBiasTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitTextureSize, textureSize);
			Blitter.s_PropertyBlock.SetInt(Blitter.BlitShaderIDs._BlitPaddingSize, paddingInPixels);
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[8]);
		}

		public static void BlitOctahedralWithPaddingMultiply(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBiasTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitTextureSize, textureSize);
			Blitter.s_PropertyBlock.SetInt(Blitter.BlitShaderIDs._BlitPaddingSize, paddingInPixels);
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[13]);
		}

		public static void BlitCubeToOctahedral2DQuad(CommandBuffer cmd, Texture source, Vector4 scaleBiasRT, int mipLevelTex)
		{
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitCubeTexture, source);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, new Vector4(1f, 1f, 0f, 0f));
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[14]);
		}

		public static void BlitCubeToOctahedral2DQuadWithPadding(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels, Vector4? decodeInstructions = null)
		{
			Material blitMaterial = Blitter.GetBlitMaterial(source.dimension, false);
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitCubeTexture, source);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, new Vector4(1f, 1f, 0f, 0f));
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitTextureSize, textureSize);
			Blitter.s_PropertyBlock.SetInt(Blitter.BlitShaderIDs._BlitPaddingSize, paddingInPixels);
			cmd.SetKeyword(blitMaterial, Blitter.s_DecodeHdrKeyword, decodeInstructions != null);
			if (decodeInstructions != null)
			{
				Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitDecodeInstructions, decodeInstructions.Value);
			}
			Blitter.DrawQuad(cmd, blitMaterial, Blitter.s_BlitShaderPassIndicesMap[bilinear ? 22 : 21]);
			cmd.SetKeyword(blitMaterial, Blitter.s_DecodeHdrKeyword, false);
		}

		public static void BlitCubeToOctahedral2DQuadSingleChannel(CommandBuffer cmd, Texture source, Vector4 scaleBiasRT, int mipLevelTex)
		{
			int num = 15;
			if (GraphicsFormatUtility.GetComponentCount(source.graphicsFormat) == 1U)
			{
				if (GraphicsFormatUtility.IsAlphaOnlyFormat(source.graphicsFormat))
				{
					num = 16;
				}
				if (GraphicsFormatUtility.GetSwizzleR(source.graphicsFormat) == FormatSwizzle.FormatSwizzleR)
				{
					num = 17;
				}
			}
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitCubeTexture, source);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, new Vector4(1f, 1f, 0f, 0f));
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[num]);
		}

		public static void BlitQuadSingleChannel(CommandBuffer cmd, Texture source, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex)
		{
			int num = 18;
			if (GraphicsFormatUtility.GetComponentCount(source.graphicsFormat) == 1U)
			{
				if (GraphicsFormatUtility.IsAlphaOnlyFormat(source.graphicsFormat))
				{
					num = 19;
				}
				if (GraphicsFormatUtility.GetSwizzleR(source.graphicsFormat) == FormatSwizzle.FormatSwizzleR)
				{
					num = 20;
				}
			}
			Blitter.s_PropertyBlock.SetTexture(Blitter.BlitShaderIDs._BlitTexture, source);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBias, scaleBiasTex);
			Blitter.s_PropertyBlock.SetVector(Blitter.BlitShaderIDs._BlitScaleBiasRt, scaleBiasRT);
			Blitter.s_PropertyBlock.SetFloat(Blitter.BlitShaderIDs._BlitMipLevel, (float)mipLevelTex);
			Blitter.DrawQuad(cmd, Blitter.GetBlitMaterial(source.dimension, false), Blitter.s_BlitShaderPassIndicesMap[num]);
		}

		[CompilerGenerated]
		internal static Vector3[] <Initialize>g__GetFullScreenTriangleVertexPosition|14_0(float z)
		{
			Vector3[] array = new Vector3[3];
			for (int i = 0; i < 3; i++)
			{
				Vector2 vector = new Vector2((float)(i << 1 & 2), (float)(i & 2));
				array[i] = new Vector3(vector.x * 2f - 1f, vector.y * 2f - 1f, z);
			}
			return array;
		}

		[CompilerGenerated]
		internal static Vector2[] <Initialize>g__GetFullScreenTriangleTexCoord|14_1()
		{
			Vector2[] array = new Vector2[3];
			for (int i = 0; i < 3; i++)
			{
				if (SystemInfo.graphicsUVStartsAtTop)
				{
					array[i] = new Vector2((float)(i << 1 & 2), 1f - (float)(i & 2));
				}
				else
				{
					array[i] = new Vector2((float)(i << 1 & 2), (float)(i & 2));
				}
			}
			return array;
		}

		[CompilerGenerated]
		internal static Vector3[] <Initialize>g__GetQuadVertexPosition|14_2(float z)
		{
			Vector3[] array = new Vector3[4];
			for (uint num = 0U; num < 4U; num += 1U)
			{
				uint num2 = num >> 1;
				uint num3 = num & 1U;
				float x = num2;
				float y = 1U - (num2 + num3) & 1U;
				array[(int)num] = new Vector3(x, y, z);
			}
			return array;
		}

		[CompilerGenerated]
		internal static Vector2[] <Initialize>g__GetQuadTexCoord|14_3()
		{
			Vector2[] array = new Vector2[4];
			for (uint num = 0U; num < 4U; num += 1U)
			{
				uint num2 = num >> 1;
				uint num3 = num & 1U;
				float x = num2;
				float num4 = num2 + num3 & 1U;
				if (SystemInfo.graphicsUVStartsAtTop)
				{
					num4 = 1f - num4;
				}
				array[(int)num] = new Vector2(x, num4);
			}
			return array;
		}

		private static Material s_Copy;

		private static Material s_Blit;

		private static Material s_BlitTexArray;

		private static Material s_BlitTexArraySingleSlice;

		private static Material s_BlitColorAndDepth;

		private static MaterialPropertyBlock s_PropertyBlock = new MaterialPropertyBlock();

		private static Mesh s_TriangleMesh;

		private static Mesh s_QuadMesh;

		private static LocalKeyword s_DecodeHdrKeyword;

		private static int[] s_BlitShaderPassIndicesMap;

		private static int[] s_BlitColorAndDepthShaderPassIndicesMap;

		private static class BlitShaderIDs
		{
			public static readonly int _BlitTexture = Shader.PropertyToID("_BlitTexture");

			public static readonly int _BlitCubeTexture = Shader.PropertyToID("_BlitCubeTexture");

			public static readonly int _BlitScaleBias = Shader.PropertyToID("_BlitScaleBias");

			public static readonly int _BlitScaleBiasRt = Shader.PropertyToID("_BlitScaleBiasRt");

			public static readonly int _BlitMipLevel = Shader.PropertyToID("_BlitMipLevel");

			public static readonly int _BlitTexArraySlice = Shader.PropertyToID("_BlitTexArraySlice");

			public static readonly int _BlitTextureSize = Shader.PropertyToID("_BlitTextureSize");

			public static readonly int _BlitPaddingSize = Shader.PropertyToID("_BlitPaddingSize");

			public static readonly int _BlitDecodeInstructions = Shader.PropertyToID("_BlitDecodeInstructions");

			public static readonly int _InputDepth = Shader.PropertyToID("_InputDepthTexture");
		}

		private enum BlitShaderPassNames
		{
			Nearest,
			Bilinear,
			NearestQuad,
			BilinearQuad,
			NearestQuadPadding,
			BilinearQuadPadding,
			NearestQuadPaddingRepeat,
			BilinearQuadPaddingRepeat,
			BilinearQuadPaddingOctahedral,
			NearestQuadPaddingAlphaBlend,
			BilinearQuadPaddingAlphaBlend,
			NearestQuadPaddingAlphaBlendRepeat,
			BilinearQuadPaddingAlphaBlendRepeat,
			BilinearQuadPaddingAlphaBlendOctahedral,
			CubeToOctahedral,
			CubeToOctahedralLuminance,
			CubeToOctahedralAlpha,
			CubeToOctahedralRed,
			BilinearQuadLuminance,
			BilinearQuadAlpha,
			BilinearQuadRed,
			NearestCubeToOctahedralPadding,
			BilinearCubeToOctahedralPadding
		}

		private enum BlitColorAndDepthPassNames
		{
			ColorOnly,
			ColorAndDepth
		}
	}
}
