using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.Universal", "Unity.RenderPipelines.Universal.Runtime", null)]
	[Serializable]
	public struct Light2DBlendStyle
	{
		internal Vector2 blendFactors
		{
			get
			{
				Vector2 result = default(Vector2);
				switch (this.blendMode)
				{
				case Light2DBlendStyle.BlendMode.Additive:
					result.x = 0f;
					result.y = 1f;
					break;
				case Light2DBlendStyle.BlendMode.Multiply:
					result.x = 1f;
					result.y = 0f;
					break;
				case Light2DBlendStyle.BlendMode.Subtractive:
					result.x = 0f;
					result.y = -1f;
					break;
				default:
					result.x = 1f;
					result.y = 0f;
					break;
				}
				return result;
			}
		}

		internal Light2DBlendStyle.MaskChannelFilter maskTextureChannelFilter
		{
			get
			{
				switch (this.maskTextureChannel)
				{
				case Light2DBlendStyle.TextureChannel.R:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));
				case Light2DBlendStyle.TextureChannel.G:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));
				case Light2DBlendStyle.TextureChannel.B:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 0f));
				case Light2DBlendStyle.TextureChannel.A:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(0f, 0f, 0f, 1f), new Vector4(0f, 0f, 0f, 0f));
				case Light2DBlendStyle.TextureChannel.OneMinusR:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(1f, 0f, 0f, 0f), new Vector4(1f, 0f, 0f, 0f));
				case Light2DBlendStyle.TextureChannel.OneMinusG:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f));
				case Light2DBlendStyle.TextureChannel.OneMinusB:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 1f, 0f));
				case Light2DBlendStyle.TextureChannel.OneMinusA:
					return new Light2DBlendStyle.MaskChannelFilter(new Vector4(0f, 0f, 0f, 1f), new Vector4(0f, 0f, 0f, 1f));
				}
				return new Light2DBlendStyle.MaskChannelFilter(Vector4.zero, Vector4.zero);
			}
		}

		internal bool isDirty { readonly get; set; }

		internal bool hasRenderTarget { readonly get; set; }

		public string name;

		[SerializeField]
		internal Light2DBlendStyle.TextureChannel maskTextureChannel;

		[SerializeField]
		internal Light2DBlendStyle.BlendMode blendMode;

		internal int renderTargetHandleId;

		internal RTHandle renderTargetHandle;

		internal enum TextureChannel
		{
			None,
			R,
			G,
			B,
			A,
			OneMinusR,
			OneMinusG,
			OneMinusB,
			OneMinusA
		}

		internal struct MaskChannelFilter
		{
			public Vector4 mask { readonly get; private set; }

			public Vector4 inverted { readonly get; private set; }

			public MaskChannelFilter(Vector4 m, Vector4 i)
			{
				this.mask = m;
				this.inverted = i;
			}
		}

		internal enum BlendMode
		{
			Additive,
			Multiply,
			Subtractive
		}

		[Serializable]
		internal struct BlendFactors
		{
			public float multiplicative;

			public float additive;
		}
	}
}
