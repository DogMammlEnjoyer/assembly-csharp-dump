using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public static class CoreUtils
	{
		public static Cubemap blackCubeTexture
		{
			get
			{
				if (CoreUtils.m_BlackCubeTexture == null)
				{
					CoreUtils.m_BlackCubeTexture = new Cubemap(1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
					for (int i = 0; i < 6; i++)
					{
						CoreUtils.m_BlackCubeTexture.SetPixel((CubemapFace)i, 0, 0, Color.black);
					}
					CoreUtils.m_BlackCubeTexture.Apply();
				}
				return CoreUtils.m_BlackCubeTexture;
			}
		}

		public static Cubemap magentaCubeTexture
		{
			get
			{
				if (CoreUtils.m_MagentaCubeTexture == null)
				{
					CoreUtils.m_MagentaCubeTexture = new Cubemap(1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
					for (int i = 0; i < 6; i++)
					{
						CoreUtils.m_MagentaCubeTexture.SetPixel((CubemapFace)i, 0, 0, Color.magenta);
					}
					CoreUtils.m_MagentaCubeTexture.Apply();
				}
				return CoreUtils.m_MagentaCubeTexture;
			}
		}

		public static CubemapArray magentaCubeTextureArray
		{
			get
			{
				if (CoreUtils.m_MagentaCubeTextureArray == null)
				{
					CoreUtils.m_MagentaCubeTextureArray = new CubemapArray(1, 1, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
					for (int i = 0; i < 6; i++)
					{
						Color[] colors = new Color[]
						{
							Color.magenta
						};
						CoreUtils.m_MagentaCubeTextureArray.SetPixels(colors, (CubemapFace)i, 0);
					}
					CoreUtils.m_MagentaCubeTextureArray.Apply();
				}
				return CoreUtils.m_MagentaCubeTextureArray;
			}
		}

		public static Cubemap whiteCubeTexture
		{
			get
			{
				if (CoreUtils.m_WhiteCubeTexture == null)
				{
					CoreUtils.m_WhiteCubeTexture = new Cubemap(1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
					for (int i = 0; i < 6; i++)
					{
						CoreUtils.m_WhiteCubeTexture.SetPixel((CubemapFace)i, 0, 0, Color.white);
					}
					CoreUtils.m_WhiteCubeTexture.Apply();
				}
				return CoreUtils.m_WhiteCubeTexture;
			}
		}

		public static RenderTexture emptyUAV
		{
			get
			{
				if (CoreUtils.m_EmptyUAV == null)
				{
					CoreUtils.m_EmptyUAV = new RenderTexture(1, 1, 0);
					CoreUtils.m_EmptyUAV.enableRandomWrite = true;
					CoreUtils.m_EmptyUAV.Create();
				}
				return CoreUtils.m_EmptyUAV;
			}
		}

		public static GraphicsBuffer emptyBuffer
		{
			get
			{
				if (CoreUtils.m_EmptyBuffer == null || !CoreUtils.m_EmptyBuffer.IsValid())
				{
					CoreUtils.m_EmptyBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 1, 4);
				}
				return CoreUtils.m_EmptyBuffer;
			}
		}

		public static Texture3D blackVolumeTexture
		{
			get
			{
				if (CoreUtils.m_BlackVolumeTexture == null)
				{
					Color[] colors = new Color[]
					{
						Color.black
					};
					CoreUtils.m_BlackVolumeTexture = new Texture3D(1, 1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
					CoreUtils.m_BlackVolumeTexture.SetPixels(colors, 0);
					CoreUtils.m_BlackVolumeTexture.Apply();
				}
				return CoreUtils.m_BlackVolumeTexture;
			}
		}

		internal static Texture3D whiteVolumeTexture
		{
			get
			{
				if (CoreUtils.m_WhiteVolumeTexture == null)
				{
					Color[] colors = new Color[]
					{
						Color.white
					};
					CoreUtils.m_WhiteVolumeTexture = new Texture3D(1, 1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
					CoreUtils.m_WhiteVolumeTexture.SetPixels(colors, 0);
					CoreUtils.m_WhiteVolumeTexture.Apply();
				}
				return CoreUtils.m_WhiteVolumeTexture;
			}
		}

		public static void ClearRenderTarget(CommandBuffer cmd, ClearFlag clearFlag, Color clearColor)
		{
			if (clearFlag != ClearFlag.None)
			{
				cmd.ClearRenderTarget((RTClearFlags)clearFlag, clearColor, 1f, 0U);
			}
		}

		private static int FixupDepthSlice(int depthSlice, RTHandle buffer)
		{
			if (depthSlice == -1)
			{
				RenderTexture rt = buffer.rt;
				if (rt != null && rt.dimension == TextureDimension.Cube)
				{
					depthSlice = 0;
				}
			}
			return depthSlice;
		}

		private static int FixupDepthSlice(int depthSlice, CubemapFace cubemapFace)
		{
			if (depthSlice == -1 && cubemapFace != CubemapFace.Unknown)
			{
				depthSlice = 0;
			}
			return depthSlice;
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, cubemapFace);
			cmd.SetRenderTarget(buffer, miplevel, cubemapFace, depthSlice);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, ClearFlag clearFlag = ClearFlag.None, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, buffer, clearFlag, Color.clear, miplevel, cubemapFace, depthSlice);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderTargetIdentifier depthBuffer, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer, depthBuffer, ClearFlag.None, Color.clear, miplevel, cubemapFace, depthSlice);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderTargetIdentifier depthBuffer, ClearFlag clearFlag, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer, depthBuffer, clearFlag, Color.clear, miplevel, cubemapFace, depthSlice);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderTargetIdentifier depthBuffer, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, cubemapFace);
			cmd.SetRenderTarget(colorBuffer, depthBuffer, miplevel, cubemapFace, depthSlice);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier[] colorBuffers, RenderTargetIdentifier depthBuffer)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffers, depthBuffer, ClearFlag.None, Color.clear);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier[] colorBuffers, RenderTargetIdentifier depthBuffer, ClearFlag clearFlag = ClearFlag.None)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffers, depthBuffer, clearFlag, Color.clear);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier[] colorBuffers, RenderTargetIdentifier depthBuffer, ClearFlag clearFlag, Color clearColor)
		{
			cmd.SetRenderTarget(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, -1);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, ClearFlag clearFlag, Color clearColor)
		{
			cmd.SetRenderTarget(buffer, loadAction, storeAction);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, cubemapFace);
			buffer = new RenderTargetIdentifier(buffer, miplevel, cubemapFace, depthSlice);
			cmd.SetRenderTarget(buffer, loadAction, storeAction);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, cubemapFace);
			buffer = new RenderTargetIdentifier(buffer, miplevel, cubemapFace, depthSlice);
			CoreUtils.SetRenderTarget(cmd, buffer, loadAction, storeAction, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, ClearFlag clearFlag)
		{
			CoreUtils.SetRenderTarget(cmd, buffer, loadAction, storeAction, clearFlag, Color.clear);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depthBuffer, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, ClearFlag clearFlag, Color clearColor)
		{
			cmd.SetRenderTarget(colorBuffer, colorLoadAction, colorStoreAction, depthBuffer, depthLoadAction, depthStoreAction);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depthBuffer, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, cubemapFace);
			colorBuffer = new RenderTargetIdentifier(colorBuffer, miplevel, cubemapFace, depthSlice);
			depthBuffer = new RenderTargetIdentifier(depthBuffer, miplevel, cubemapFace, depthSlice);
			cmd.SetRenderTarget(colorBuffer, colorLoadAction, colorStoreAction, depthBuffer, depthLoadAction, depthStoreAction);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depthBuffer, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, cubemapFace);
			colorBuffer = new RenderTargetIdentifier(colorBuffer, miplevel, cubemapFace, depthSlice);
			depthBuffer = new RenderTargetIdentifier(depthBuffer, miplevel, cubemapFace, depthSlice);
			CoreUtils.SetRenderTarget(cmd, colorBuffer, colorLoadAction, colorStoreAction, depthBuffer, depthLoadAction, depthStoreAction, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier buffer, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, ClearFlag clearFlag, Color clearColor)
		{
			cmd.SetRenderTarget(buffer, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier colorBuffer, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depthBuffer, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, ClearFlag clearFlag)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer, colorLoadAction, colorStoreAction, depthBuffer, depthLoadAction, depthStoreAction, clearFlag, Color.clear);
		}

		private static void SetViewportAndClear(CommandBuffer cmd, RTHandle buffer, ClearFlag clearFlag, Color clearColor)
		{
			CoreUtils.SetViewport(cmd, buffer);
			CoreUtils.ClearRenderTarget(cmd, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle buffer, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			depthSlice = CoreUtils.FixupDepthSlice(depthSlice, buffer);
			cmd.SetRenderTarget(buffer.nameID, miplevel, cubemapFace, depthSlice);
			CoreUtils.SetViewportAndClear(cmd, buffer, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle buffer, ClearFlag clearFlag = ClearFlag.None, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, buffer, clearFlag, Color.clear, miplevel, cubemapFace, depthSlice);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle colorBuffer, RTHandle depthBuffer, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer, depthBuffer, ClearFlag.None, Color.clear, miplevel, cubemapFace, depthSlice);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle colorBuffer, RTHandle depthBuffer, ClearFlag clearFlag, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer, depthBuffer, clearFlag, Color.clear, miplevel, cubemapFace, depthSlice);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle colorBuffer, RTHandle depthBuffer, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer.nameID, depthBuffer.nameID, miplevel, cubemapFace, depthSlice);
			CoreUtils.SetViewportAndClear(cmd, colorBuffer, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle buffer, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, buffer.nameID, loadAction, storeAction, miplevel, cubemapFace, depthSlice);
			CoreUtils.SetViewportAndClear(cmd, buffer, clearFlag, clearColor);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RTHandle colorBuffer, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RTHandle depthBuffer, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, ClearFlag clearFlag, Color clearColor, int miplevel = 0, CubemapFace cubemapFace = CubemapFace.Unknown, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffer.nameID, colorLoadAction, colorStoreAction, depthBuffer.nameID, depthLoadAction, depthStoreAction, miplevel, cubemapFace, depthSlice);
			CoreUtils.SetViewportAndClear(cmd, colorBuffer, clearFlag, clearColor);
		}

		public static void SetShadingRateFragmentSize(CommandBuffer cmd, ShadingRateFragmentSize baseShadingRateFragmentSize)
		{
			cmd.SetShadingRateFragmentSize(baseShadingRateFragmentSize);
		}

		public static void SetShadingRateCombiner(CommandBuffer cmd, ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
		{
			cmd.SetShadingRateCombiner(stage, combiner);
		}

		public static void SetShadingRateImage(CommandBuffer cmd, in RenderTargetIdentifier shadingRateImage)
		{
			cmd.SetShadingRateImage(shadingRateImage);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier[] colorBuffers, RTHandle depthBuffer)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffers, depthBuffer.nameID, ClearFlag.None, Color.clear);
			CoreUtils.SetViewport(cmd, depthBuffer);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier[] colorBuffers, RTHandle depthBuffer, ClearFlag clearFlag = ClearFlag.None)
		{
			CoreUtils.SetRenderTarget(cmd, colorBuffers, depthBuffer.nameID);
			CoreUtils.SetViewportAndClear(cmd, depthBuffer, clearFlag, Color.clear);
		}

		public static void SetRenderTarget(CommandBuffer cmd, RenderTargetIdentifier[] colorBuffers, RTHandle depthBuffer, ClearFlag clearFlag, Color clearColor)
		{
			cmd.SetRenderTarget(colorBuffers, depthBuffer.nameID, 0, CubemapFace.Unknown, -1);
			CoreUtils.SetViewportAndClear(cmd, depthBuffer, clearFlag, clearColor);
		}

		public static void SetViewport(CommandBuffer cmd, RTHandle target)
		{
			if (target.useScaling)
			{
				Vector2Int scaledSize = target.GetScaledSize(target.rtHandleProperties.currentViewportSize);
				cmd.SetViewport(new Rect(0f, 0f, (float)scaledSize.x, (float)scaledSize.y));
			}
		}

		public static string GetRenderTargetAutoName(int width, int height, int depth, RenderTextureFormat format, string name, bool mips = false, bool enableMSAA = false, MSAASamples msaaSamples = MSAASamples.None)
		{
			return CoreUtils.GetRenderTargetAutoName(width, height, depth, format.ToString(), TextureDimension.None, name, mips, enableMSAA, msaaSamples, false, false);
		}

		public static string GetRenderTargetAutoName(int width, int height, int depth, GraphicsFormat format, string name, bool mips = false, bool enableMSAA = false, MSAASamples msaaSamples = MSAASamples.None)
		{
			return CoreUtils.GetRenderTargetAutoName(width, height, depth, format.ToString(), TextureDimension.None, name, mips, enableMSAA, msaaSamples, false, false);
		}

		public static string GetRenderTargetAutoName(int width, int height, int depth, GraphicsFormat format, TextureDimension dim, string name, bool mips = false, bool enableMSAA = false, MSAASamples msaaSamples = MSAASamples.None, bool dynamicRes = false, bool dynamicResExplicit = false)
		{
			return CoreUtils.GetRenderTargetAutoName(width, height, depth, format.ToString(), dim, name, mips, enableMSAA, msaaSamples, dynamicRes, dynamicResExplicit);
		}

		private static string GetRenderTargetAutoName(int width, int height, int depth, string format, TextureDimension dim, string name, bool mips, bool enableMSAA, MSAASamples msaaSamples, bool dynamicRes, bool dynamicResExplicit)
		{
			string text = string.Format("{0}_{1}x{2}", name, width, height);
			if (depth > 1)
			{
				text = string.Format("{0}x{1}", text, depth);
			}
			if (mips)
			{
				text = string.Format("{0}_{1}", text, "Mips");
			}
			text = string.Format("{0}_{1}", text, format);
			if (dim != TextureDimension.None)
			{
				text = string.Format("{0}_{1}", text, dim);
			}
			if (enableMSAA)
			{
				text = string.Format("{0}_{1}", text, msaaSamples.ToString());
			}
			if (dynamicRes)
			{
				text = string.Format("{0}_{1}", text, "Dynamic");
			}
			if (dynamicResExplicit)
			{
				text = string.Format("{0}_{1}", text, "DynamicExplicit");
			}
			return text;
		}

		public static string GetTextureAutoName(int width, int height, TextureFormat format, TextureDimension dim = TextureDimension.None, string name = "", bool mips = false, int depth = 0)
		{
			return CoreUtils.GetTextureAutoName(width, height, format.ToString(), dim, name, mips, depth);
		}

		public static string GetTextureAutoName(int width, int height, GraphicsFormat format, TextureDimension dim = TextureDimension.None, string name = "", bool mips = false, int depth = 0)
		{
			return CoreUtils.GetTextureAutoName(width, height, format.ToString(), dim, name, mips, depth);
		}

		private static string GetTextureAutoName(int width, int height, string format, TextureDimension dim = TextureDimension.None, string name = "", bool mips = false, int depth = 0)
		{
			string arg;
			if (depth == 0)
			{
				arg = string.Format("{0}x{1}{2}_{3}", new object[]
				{
					width,
					height,
					mips ? "_Mips" : "",
					format
				});
			}
			else
			{
				arg = string.Format("{0}x{1}x{2}{3}_{4}", new object[]
				{
					width,
					height,
					depth,
					mips ? "_Mips" : "",
					format
				});
			}
			return string.Format("{0}_{1}_{2}", (name != null && name.Length == 0) ? "Texture" : name, (dim == TextureDimension.None) ? "" : dim.ToString(), arg);
		}

		public static void ClearCubemap(CommandBuffer cmd, RenderTexture renderTexture, Color clearColor, bool clearMips = false)
		{
			int num = 1;
			if (renderTexture.useMipMap && clearMips)
			{
				num = (int)Mathf.Log((float)renderTexture.width, 2f) + 1;
			}
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < num; j++)
				{
					CoreUtils.SetRenderTarget(cmd, new RenderTargetIdentifier(renderTexture), ClearFlag.Color, clearColor, j, (CubemapFace)i, -1);
				}
			}
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(RasterCommandBuffer commandBuffer, Material material, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			CoreUtils.DrawFullScreen(commandBuffer.m_WrappedCommandBuffer, material, properties, shaderPassId);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RenderTargetIdentifier colorBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			commandBuffer.SetRenderTarget(colorBuffer, 0, CubemapFace.Unknown, -1);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RenderTargetIdentifier colorBuffer, RenderTargetIdentifier depthStencilBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			commandBuffer.SetRenderTarget(colorBuffer, depthStencilBuffer, 0, CubemapFace.Unknown, -1);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RenderTargetIdentifier[] colorBuffers, RenderTargetIdentifier depthStencilBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			commandBuffer.SetRenderTarget(colorBuffers, depthStencilBuffer, 0, CubemapFace.Unknown, -1);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RenderTargetIdentifier[] colorBuffers, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			CoreUtils.DrawFullScreen(commandBuffer, material, colorBuffers, colorBuffers[0], properties, shaderPassId);
		}

		public static Color ConvertSRGBToActiveColorSpace(Color color)
		{
			if (QualitySettings.activeColorSpace != ColorSpace.Linear)
			{
				return color;
			}
			return color.linear;
		}

		public static Color ConvertLinearToActiveColorSpace(Color color)
		{
			if (QualitySettings.activeColorSpace != ColorSpace.Linear)
			{
				return color.gamma;
			}
			return color;
		}

		public static Material CreateEngineMaterial(string shaderPath)
		{
			if (string.IsNullOrEmpty(shaderPath))
			{
				throw new ArgumentException("shaderPath");
			}
			Shader shader = Shader.Find(shaderPath);
			if (shader == null)
			{
				Debug.LogError("Cannot create required material because shader " + shaderPath + " could not be found");
				return null;
			}
			return CoreUtils.CreateEngineMaterial(shader);
		}

		public static Material CreateEngineMaterial(Shader shader)
		{
			if (shader == null)
			{
				Debug.LogError("Cannot create required material because shader is null");
				return null;
			}
			return new Material(shader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}

		public static bool HasFlag<T>(T mask, T flag) where T : IConvertible
		{
			return (mask.ToUInt32(null) & flag.ToUInt32(null)) > 0U;
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		public static void SetKeyword(CommandBuffer cmd, string keyword, bool state)
		{
			if (state)
			{
				cmd.EnableShaderKeyword(keyword);
				return;
			}
			cmd.DisableShaderKeyword(keyword);
		}

		public static void SetKeyword(CommandBuffer cmd, ComputeShader cs, string keyword, bool state)
		{
			LocalKeyword localKeyword = new LocalKeyword(cs, keyword);
			if (state)
			{
				cmd.EnableKeyword(cs, localKeyword);
				return;
			}
			cmd.DisableKeyword(cs, localKeyword);
		}

		public static void SetKeyword(BaseCommandBuffer cmd, string keyword, bool state)
		{
			if (state)
			{
				cmd.m_WrappedCommandBuffer.EnableShaderKeyword(keyword);
				return;
			}
			cmd.m_WrappedCommandBuffer.DisableShaderKeyword(keyword);
		}

		public static void SetKeyword(Material material, string keyword, bool state)
		{
			if (state)
			{
				material.EnableKeyword(keyword);
				return;
			}
			material.DisableKeyword(keyword);
		}

		public static void SetKeyword(Material material, LocalKeyword keyword, bool state)
		{
			if (state)
			{
				material.EnableKeyword(keyword);
				return;
			}
			material.DisableKeyword(keyword);
		}

		public static void SetKeyword(ComputeShader cs, string keyword, bool state)
		{
			if (state)
			{
				cs.EnableKeyword(keyword);
				return;
			}
			cs.DisableKeyword(keyword);
		}

		public static void Destroy(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		public static IEnumerable<Type> GetAllAssemblyTypes()
		{
			if (CoreUtils.m_AssemblyTypes == null)
			{
				CoreUtils.m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly t)
				{
					Type[] result = new Type[0];
					try
					{
						result = t.GetTypes();
					}
					catch
					{
					}
					return result;
				});
			}
			return CoreUtils.m_AssemblyTypes;
		}

		public static IEnumerable<Type> GetAllTypesDerivedFrom<T>()
		{
			return from t in CoreUtils.GetAllAssemblyTypes()
			where t.IsSubclassOf(typeof(T))
			select t;
		}

		public static void SafeRelease(GraphicsBuffer buffer)
		{
			if (buffer != null)
			{
				buffer.Release();
			}
		}

		public static void SafeRelease(ComputeBuffer buffer)
		{
			if (buffer != null)
			{
				buffer.Release();
			}
		}

		public static Mesh CreateCubeMesh(Vector3 min, Vector3 max)
		{
			return new Mesh
			{
				vertices = new Vector3[]
				{
					new Vector3(min.x, min.y, min.z),
					new Vector3(max.x, min.y, min.z),
					new Vector3(max.x, max.y, min.z),
					new Vector3(min.x, max.y, min.z),
					new Vector3(min.x, min.y, max.z),
					new Vector3(max.x, min.y, max.z),
					new Vector3(max.x, max.y, max.z),
					new Vector3(min.x, max.y, max.z)
				},
				triangles = new int[]
				{
					0,
					2,
					1,
					0,
					3,
					2,
					1,
					6,
					5,
					1,
					2,
					6,
					5,
					7,
					4,
					5,
					6,
					7,
					4,
					3,
					0,
					4,
					7,
					3,
					3,
					6,
					2,
					3,
					7,
					6,
					4,
					1,
					5,
					4,
					0,
					1
				}
			};
		}

		public static bool ArePostProcessesEnabled(Camera camera)
		{
			return true;
		}

		public static bool AreAnimatedMaterialsEnabled(Camera camera)
		{
			return true;
		}

		public static bool IsSceneLightingDisabled(Camera camera)
		{
			return false;
		}

		public static bool IsLightOverlapDebugEnabled(Camera camera)
		{
			return false;
		}

		public static bool IsSceneViewFogEnabled(Camera camera)
		{
			return true;
		}

		public static bool IsSceneFilteringEnabled()
		{
			return false;
		}

		public static bool IsSceneViewPrefabStageContextHidden()
		{
			return false;
		}

		public static void DrawRendererList(ScriptableRenderContext renderContext, CommandBuffer cmd, RendererList rendererList)
		{
			cmd.DrawRendererList(rendererList);
		}

		public static int GetTextureHash(Texture texture)
		{
			int num = texture.GetHashCode();
			num = 23 * num + texture.GetInstanceID().GetHashCode();
			num = 23 * num + texture.graphicsFormat.GetHashCode();
			num = 23 * num + texture.wrapMode.GetHashCode();
			num = 23 * num + texture.width.GetHashCode();
			num = 23 * num + texture.height.GetHashCode();
			num = 23 * num + texture.filterMode.GetHashCode();
			num = 23 * num + texture.anisoLevel.GetHashCode();
			num = 23 * num + texture.mipmapCount.GetHashCode();
			return 23 * num + texture.updateCount.GetHashCode();
		}

		public static int PreviousPowerOfTwo(int size)
		{
			if (size <= 0)
			{
				return 0;
			}
			size |= size >> 1;
			size |= size >> 2;
			size |= size >> 4;
			size |= size >> 8;
			size |= size >> 16;
			return size - (size >> 1);
		}

		public static int GetMipCount(int size)
		{
			return Mathf.FloorToInt(Mathf.Log((float)size, 2f)) + 1;
		}

		public static int GetMipCount(float size)
		{
			return Mathf.FloorToInt(Mathf.Log(size, 2f)) + 1;
		}

		public static int DivRoundUp(int value, int divisor)
		{
			return (value + (divisor - 1)) / divisor;
		}

		public static T GetLastEnumValue<T>() where T : Enum
		{
			return typeof(T).GetEnumValues().Cast<T>().Last<T>();
		}

		internal static string GetCorePath()
		{
			return "Packages/com.unity.render-pipelines.core/";
		}

		public static Vector3[] CalculateViewSpaceCorners(Matrix4x4 proj, float z)
		{
			Vector3[] array = new Vector3[4];
			Matrix4x4 matrix4x = Matrix4x4.Inverse(proj);
			array[0] = matrix4x.MultiplyPoint(new Vector3(-1f, -1f, 0.95f));
			array[1] = matrix4x.MultiplyPoint(new Vector3(1f, -1f, 0.95f));
			array[2] = matrix4x.MultiplyPoint(new Vector3(1f, 1f, 0.95f));
			array[3] = matrix4x.MultiplyPoint(new Vector3(-1f, 1f, 0.95f));
			for (int i = 0; i < 4; i++)
			{
				array[i] *= z / -array[i].z;
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GraphicsFormat GetDefaultDepthStencilFormat()
		{
			return GraphicsFormat.D32_SFloat_S8_UInt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GraphicsFormat GetDefaultDepthOnlyFormat()
		{
			return GraphicsFormat.D32_SFloat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DepthBits GetDefaultDepthBufferBits()
		{
			return DepthBits.Depth32;
		}

		public static readonly Vector3[] lookAtList = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, -1f)
		};

		public static readonly Vector3[] upVectorList = new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0f)
		};

		private const string obsoletePriorityMessage = "Use CoreUtils.Priorities instead";

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int editMenuPriority1 = 320;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int editMenuPriority2 = 331;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int editMenuPriority3 = 342;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int editMenuPriority4 = 353;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int assetCreateMenuPriority1 = 230;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int assetCreateMenuPriority2 = 241;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int assetCreateMenuPriority3 = 300;

		[Obsolete("Use CoreUtils.Priorities instead", false)]
		public const int gameObjectMenuPriority = 10;

		private static Cubemap m_BlackCubeTexture;

		private static Cubemap m_MagentaCubeTexture;

		private static CubemapArray m_MagentaCubeTextureArray;

		private static Cubemap m_WhiteCubeTexture;

		private static RenderTexture m_EmptyUAV;

		private static GraphicsBuffer m_EmptyBuffer;

		private static Texture3D m_BlackVolumeTexture;

		internal static Texture3D m_WhiteVolumeTexture;

		private static IEnumerable<Type> m_AssemblyTypes;

		public static class Sections
		{
			public const int section1 = 10000;

			public const int section2 = 20000;

			public const int section3 = 30000;

			public const int section4 = 40000;

			public const int section5 = 50000;

			public const int section6 = 60000;

			public const int section7 = 70000;

			public const int section8 = 80000;
		}

		public static class Priorities
		{
			public const int assetsCreateShaderMenuPriority = 83;

			public const int assetsCreateRenderingMenuPriority = 308;

			public const int editMenuPriority = 320;

			public const int gameObjectMenuPriority = 10;

			public const int srpLensFlareMenuPriority = 9;

			public const int scriptingPriority = 40;
		}
	}
}
