using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/ShadingRateImage.h")]
	public static class ShadingRateImage
	{
		[FreeFunction("ShadingRateImage::GetAllocSizeInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetAllocSizeInternal(int pixelWidth, int pixelHeight, out int tileWidth, out int tileHeight);

		public static Vector2Int GetAllocTileSize(Vector2Int pixelSize)
		{
			return ShadingRateImage.GetAllocTileSize(pixelSize.x, pixelSize.y);
		}

		public static Vector2Int GetAllocTileSize(int pixelWidth, int pixelHeight)
		{
			int x;
			int y;
			ShadingRateImage.GetAllocSizeInternal(pixelWidth, pixelHeight, out x, out y);
			return new Vector2Int(x, y);
		}

		public static RenderTexture AllocFromPixelSize(in RenderTextureDescriptor rtDesc)
		{
			Vector2Int allocTileSize = ShadingRateImage.GetAllocTileSize(rtDesc.width, rtDesc.height);
			RenderTextureDescriptor desc = rtDesc;
			desc.width = allocTileSize.x;
			desc.height = allocTileSize.y;
			return new RenderTexture(desc);
		}

		public static RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height, int volumeDepth = 1, TextureDimension textureDimension = TextureDimension.Tex2D)
		{
			bool supportsPerImageTile = ShadingRateInfo.supportsPerImageTile;
			RenderTextureDescriptor result;
			if (supportsPerImageTile)
			{
				RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height)
				{
					msaaSamples = 1,
					autoGenerateMips = false,
					volumeDepth = volumeDepth,
					dimension = textureDimension,
					graphicsFormat = ShadingRateInfo.graphicsFormat,
					enableRandomWrite = true,
					enableShadingRate = true
				};
				result = renderTextureDescriptor;
			}
			else
			{
				RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(0, 0)
				{
					msaaSamples = 0,
					autoGenerateMips = false,
					volumeDepth = 0,
					dimension = TextureDimension.None,
					graphicsFormat = GraphicsFormat.None
				};
				result = renderTextureDescriptor;
			}
			return result;
		}
	}
}
