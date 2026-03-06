using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Color Lookup")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class ColorLookup : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.contribution.value > 0f && this.ValidateLUT();
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		public bool ValidateLUT()
		{
			UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
			if (asset == null || this.texture.value == null)
			{
				return false;
			}
			int colorGradingLutSize = asset.colorGradingLutSize;
			if (this.texture.value.height != colorGradingLutSize)
			{
				return false;
			}
			bool flag = false;
			Texture value = this.texture.value;
			Texture2D texture2D = value as Texture2D;
			if (texture2D == null)
			{
				RenderTexture renderTexture = value as RenderTexture;
				if (renderTexture != null)
				{
					flag |= (renderTexture.dimension == TextureDimension.Tex2D && renderTexture.width == colorGradingLutSize * colorGradingLutSize && !renderTexture.sRGB);
				}
			}
			else
			{
				flag |= (texture2D.width == colorGradingLutSize * colorGradingLutSize && !GraphicsFormatUtility.IsSRGBFormat(texture2D.graphicsFormat));
			}
			return flag;
		}

		[Tooltip("A 2D Lookup Texture (LUT) to use for color grading.")]
		public TextureParameter texture = new TextureParameter(null, false);

		[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
		public ClampedFloatParameter contribution = new ClampedFloatParameter(0f, 0f, 1f, false);
	}
}
