using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public static class TextureXR
	{
		public static int maxViews
		{
			set
			{
				TextureXR.m_MaxViews = value;
			}
		}

		public static int slices
		{
			get
			{
				return TextureXR.m_MaxViews;
			}
		}

		public static bool useTexArray
		{
			get
			{
				GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
				if (graphicsDeviceType <= GraphicsDeviceType.Metal)
				{
					if (graphicsDeviceType != GraphicsDeviceType.Direct3D11 && graphicsDeviceType != GraphicsDeviceType.PlayStation4 && graphicsDeviceType != GraphicsDeviceType.Metal)
					{
						return false;
					}
				}
				else if (graphicsDeviceType != GraphicsDeviceType.Direct3D12 && graphicsDeviceType != GraphicsDeviceType.Vulkan && graphicsDeviceType - GraphicsDeviceType.PlayStation5 > 1)
				{
					return false;
				}
				return true;
			}
		}

		public static TextureDimension dimension
		{
			get
			{
				if (!TextureXR.useTexArray)
				{
					return TextureDimension.Tex2D;
				}
				return TextureDimension.Tex2DArray;
			}
		}

		public static RTHandle GetBlackUIntTexture()
		{
			if (!TextureXR.useTexArray)
			{
				return TextureXR.m_BlackUIntTextureRTH;
			}
			return TextureXR.m_BlackUIntTexture2DArrayRTH;
		}

		public static RTHandle GetClearTexture()
		{
			if (!TextureXR.useTexArray)
			{
				return TextureXR.m_ClearTextureRTH;
			}
			return TextureXR.m_ClearTexture2DArrayRTH;
		}

		public static RTHandle GetMagentaTexture()
		{
			if (!TextureXR.useTexArray)
			{
				return TextureXR.m_MagentaTextureRTH;
			}
			return TextureXR.m_MagentaTexture2DArrayRTH;
		}

		public static RTHandle GetBlackTexture()
		{
			if (!TextureXR.useTexArray)
			{
				return TextureXR.m_BlackTextureRTH;
			}
			return TextureXR.m_BlackTexture2DArrayRTH;
		}

		public static RTHandle GetBlackTextureArray()
		{
			return TextureXR.m_BlackTexture2DArrayRTH;
		}

		public static RTHandle GetBlackTexture3D()
		{
			return TextureXR.m_BlackTexture3DRTH;
		}

		public static RTHandle GetWhiteTexture()
		{
			if (!TextureXR.useTexArray)
			{
				return TextureXR.m_WhiteTextureRTH;
			}
			return TextureXR.m_WhiteTexture2DArrayRTH;
		}

		public static void Initialize(CommandBuffer cmd, ComputeShader clearR32_UIntShader)
		{
			if (TextureXR.m_BlackUIntTexture2DArray == null)
			{
				RTHandles.Release(TextureXR.m_BlackUIntTexture2DArrayRTH);
				TextureXR.m_BlackUIntTexture2DArray = TextureXR.CreateBlackUIntTextureArray(cmd, clearR32_UIntShader);
				TextureXR.m_BlackUIntTexture2DArrayRTH = RTHandles.Alloc(TextureXR.m_BlackUIntTexture2DArray);
				RTHandles.Release(TextureXR.m_BlackUIntTextureRTH);
				TextureXR.m_BlackUIntTexture = TextureXR.CreateBlackUintTexture(cmd, clearR32_UIntShader);
				TextureXR.m_BlackUIntTextureRTH = RTHandles.Alloc(TextureXR.m_BlackUIntTexture);
				RTHandles.Release(TextureXR.m_ClearTextureRTH);
				TextureXR.m_ClearTexture = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None)
				{
					name = "Clear Texture"
				};
				TextureXR.m_ClearTexture.SetPixel(0, 0, Color.clear);
				TextureXR.m_ClearTexture.Apply();
				TextureXR.m_ClearTextureRTH = RTHandles.Alloc(TextureXR.m_ClearTexture);
				RTHandles.Release(TextureXR.m_ClearTexture2DArrayRTH);
				TextureXR.m_ClearTexture2DArray = TextureXR.CreateTexture2DArrayFromTexture2D(TextureXR.m_ClearTexture, "Clear Texture2DArray");
				TextureXR.m_ClearTexture2DArrayRTH = RTHandles.Alloc(TextureXR.m_ClearTexture2DArray);
				RTHandles.Release(TextureXR.m_MagentaTextureRTH);
				TextureXR.m_MagentaTexture = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None)
				{
					name = "Magenta Texture"
				};
				TextureXR.m_MagentaTexture.SetPixel(0, 0, Color.magenta);
				TextureXR.m_MagentaTexture.Apply();
				TextureXR.m_MagentaTextureRTH = RTHandles.Alloc(TextureXR.m_MagentaTexture);
				RTHandles.Release(TextureXR.m_MagentaTexture2DArrayRTH);
				TextureXR.m_MagentaTexture2DArray = TextureXR.CreateTexture2DArrayFromTexture2D(TextureXR.m_MagentaTexture, "Magenta Texture2DArray");
				TextureXR.m_MagentaTexture2DArrayRTH = RTHandles.Alloc(TextureXR.m_MagentaTexture2DArray);
				RTHandles.Release(TextureXR.m_BlackTextureRTH);
				TextureXR.m_BlackTexture = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None)
				{
					name = "Black Texture"
				};
				TextureXR.m_BlackTexture.SetPixel(0, 0, Color.black);
				TextureXR.m_BlackTexture.Apply();
				TextureXR.m_BlackTextureRTH = RTHandles.Alloc(TextureXR.m_BlackTexture);
				RTHandles.Release(TextureXR.m_BlackTexture2DArrayRTH);
				TextureXR.m_BlackTexture2DArray = TextureXR.CreateTexture2DArrayFromTexture2D(TextureXR.m_BlackTexture, "Black Texture2DArray");
				TextureXR.m_BlackTexture2DArrayRTH = RTHandles.Alloc(TextureXR.m_BlackTexture2DArray);
				RTHandles.Release(TextureXR.m_BlackTexture3DRTH);
				TextureXR.m_BlackTexture3D = TextureXR.CreateBlackTexture3D("Black Texture3D");
				TextureXR.m_BlackTexture3DRTH = RTHandles.Alloc(TextureXR.m_BlackTexture3D);
				RTHandles.Release(TextureXR.m_WhiteTextureRTH);
				TextureXR.m_WhiteTextureRTH = RTHandles.Alloc(Texture2D.whiteTexture);
				RTHandles.Release(TextureXR.m_WhiteTexture2DArrayRTH);
				TextureXR.m_WhiteTexture2DArray = TextureXR.CreateTexture2DArrayFromTexture2D(Texture2D.whiteTexture, "White Texture2DArray");
				TextureXR.m_WhiteTexture2DArrayRTH = RTHandles.Alloc(TextureXR.m_WhiteTexture2DArray);
			}
		}

		private static Texture2DArray CreateTexture2DArrayFromTexture2D(Texture2D source, string name)
		{
			Texture2DArray texture2DArray = new Texture2DArray(source.width, source.height, TextureXR.slices, source.format, false)
			{
				name = name
			};
			for (int i = 0; i < TextureXR.slices; i++)
			{
				Graphics.CopyTexture(source, 0, 0, texture2DArray, i, 0);
			}
			return texture2DArray;
		}

		private static Texture CreateBlackUIntTextureArray(CommandBuffer cmd, ComputeShader clearR32_UIntShader)
		{
			RenderTexture renderTexture = new RenderTexture(1, 1, 0, GraphicsFormat.R32_UInt)
			{
				dimension = TextureDimension.Tex2DArray,
				volumeDepth = TextureXR.slices,
				useMipMap = false,
				autoGenerateMips = false,
				enableRandomWrite = true,
				name = "Black UInt Texture Array"
			};
			renderTexture.Create();
			int kernelIndex = clearR32_UIntShader.FindKernel("ClearUIntTextureArray");
			cmd.SetComputeTextureParam(clearR32_UIntShader, kernelIndex, "_TargetArray", renderTexture);
			cmd.DispatchCompute(clearR32_UIntShader, kernelIndex, 1, 1, TextureXR.slices);
			return renderTexture;
		}

		private static Texture CreateBlackUintTexture(CommandBuffer cmd, ComputeShader clearR32_UIntShader)
		{
			RenderTexture renderTexture = new RenderTexture(1, 1, 0, GraphicsFormat.R32_UInt)
			{
				dimension = TextureDimension.Tex2D,
				volumeDepth = 1,
				useMipMap = false,
				autoGenerateMips = false,
				enableRandomWrite = true,
				name = "Black UInt Texture"
			};
			renderTexture.Create();
			int kernelIndex = clearR32_UIntShader.FindKernel("ClearUIntTexture");
			cmd.SetComputeTextureParam(clearR32_UIntShader, kernelIndex, "_Target", renderTexture);
			cmd.DispatchCompute(clearR32_UIntShader, kernelIndex, 1, 1, 1);
			return renderTexture;
		}

		private static Texture3D CreateBlackTexture3D(string name)
		{
			Texture3D texture3D = new Texture3D(1, 1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
			texture3D.name = name;
			texture3D.SetPixel(0, 0, 0, Color.black, 0);
			texture3D.Apply(false);
			return texture3D;
		}

		private static int m_MaxViews = 1;

		private static Texture m_BlackUIntTexture2DArray;

		private static Texture m_BlackUIntTexture;

		private static RTHandle m_BlackUIntTexture2DArrayRTH;

		private static RTHandle m_BlackUIntTextureRTH;

		private static Texture2DArray m_ClearTexture2DArray;

		private static Texture2D m_ClearTexture;

		private static RTHandle m_ClearTexture2DArrayRTH;

		private static RTHandle m_ClearTextureRTH;

		private static Texture2DArray m_MagentaTexture2DArray;

		private static Texture2D m_MagentaTexture;

		private static RTHandle m_MagentaTexture2DArrayRTH;

		private static RTHandle m_MagentaTextureRTH;

		private static Texture2D m_BlackTexture;

		private static Texture3D m_BlackTexture3D;

		private static Texture2DArray m_BlackTexture2DArray;

		private static RTHandle m_BlackTexture2DArrayRTH;

		private static RTHandle m_BlackTextureRTH;

		private static RTHandle m_BlackTexture3DRTH;

		private static Texture2DArray m_WhiteTexture2DArray;

		private static RTHandle m_WhiteTexture2DArrayRTH;

		private static RTHandle m_WhiteTextureRTH;
	}
}
