using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class TextureGradient : IDisposable
	{
		public int textureSize { get; private set; }

		[HideInInspector]
		public GradientColorKey[] colorKeys
		{
			get
			{
				Gradient gradient = this.m_Gradient;
				if (gradient == null)
				{
					return null;
				}
				return gradient.colorKeys;
			}
		}

		[HideInInspector]
		public GradientAlphaKey[] alphaKeys
		{
			get
			{
				Gradient gradient = this.m_Gradient;
				if (gradient == null)
				{
					return null;
				}
				return gradient.alphaKeys;
			}
		}

		public TextureGradient(Gradient baseCurve) : this(baseCurve.colorKeys, baseCurve.alphaKeys, GradientMode.PerceptualBlend, ColorSpace.Uninitialized, -1, false)
		{
			this.mode = baseCurve.mode;
			this.colorSpace = baseCurve.colorSpace;
			this.m_Gradient.mode = baseCurve.mode;
			this.m_Gradient.colorSpace = baseCurve.colorSpace;
		}

		public TextureGradient(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys, GradientMode mode = GradientMode.PerceptualBlend, ColorSpace colorSpace = ColorSpace.Uninitialized, int requestedTextureSize = -1, bool precise = false)
		{
			this.Rebuild(colorKeys, alphaKeys, mode, colorSpace, requestedTextureSize, precise);
		}

		private void Rebuild(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys, GradientMode mode, ColorSpace colorSpace, int requestedTextureSize, bool precise)
		{
			this.m_Gradient = new Gradient();
			this.m_Gradient.mode = mode;
			this.m_Gradient.colorSpace = colorSpace;
			this.m_Gradient.SetKeys(colorKeys, alphaKeys);
			this.m_Precise = precise;
			this.m_RequestedTextureSize = requestedTextureSize;
			if (requestedTextureSize > 0)
			{
				this.textureSize = requestedTextureSize;
			}
			else
			{
				float num = 1f;
				float[] array = new float[colorKeys.Length + alphaKeys.Length];
				for (int i = 0; i < colorKeys.Length; i++)
				{
					array[i] = colorKeys[i].time;
				}
				for (int j = 0; j < alphaKeys.Length; j++)
				{
					array[colorKeys.Length + j] = alphaKeys[j].time;
				}
				Array.Sort<float>(array);
				for (int k = 1; k < array.Length; k++)
				{
					int num2 = Math.Max(k - 1, 0);
					int num3 = Math.Min(k, array.Length - 1);
					float num4 = Mathf.Abs(array[num2] - array[num3]);
					if (num4 > 0f && num4 < num)
					{
						num = num4;
					}
				}
				float num5;
				if (precise || mode == GradientMode.Fixed)
				{
					num5 = 4f;
				}
				else
				{
					num5 = 2f;
				}
				float f = num5 * Mathf.Ceil(1f / num + 1f);
				this.textureSize = Mathf.RoundToInt(f);
				this.textureSize = Math.Min(this.textureSize, 1024);
			}
			this.SetDirty();
		}

		public void Dispose()
		{
		}

		public void Release()
		{
			if (this.m_Texture != null)
			{
				CoreUtils.Destroy(this.m_Texture);
			}
			this.m_Texture = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDirty()
		{
			this.m_IsTextureDirty = true;
		}

		private static GraphicsFormat GetTextureFormat()
		{
			return GraphicsFormat.R8G8B8A8_UNorm;
		}

		public Texture2D GetTexture()
		{
			float num = 1f / (float)(this.textureSize - 1);
			if (this.m_Texture != null && this.m_Texture.width != this.textureSize)
			{
				Object.DestroyImmediate(this.m_Texture);
				this.m_Texture = null;
			}
			if (this.m_Texture == null)
			{
				this.m_Texture = new Texture2D(this.textureSize, 1, TextureGradient.GetTextureFormat(), TextureCreationFlags.None);
				this.m_Texture.name = "GradientTexture";
				this.m_Texture.hideFlags = HideFlags.HideAndDontSave;
				this.m_Texture.filterMode = FilterMode.Bilinear;
				this.m_Texture.wrapMode = TextureWrapMode.Clamp;
				this.m_Texture.anisoLevel = 0;
				this.m_IsTextureDirty = true;
			}
			if (this.m_IsTextureDirty)
			{
				Color[] array = new Color[this.textureSize];
				for (int i = 0; i < this.textureSize; i++)
				{
					array[i] = this.Evaluate((float)i * num);
				}
				this.m_Texture.SetPixels(array);
				this.m_Texture.Apply(false, false);
				this.m_IsTextureDirty = false;
				this.m_Texture.IncrementUpdateCount();
			}
			return this.m_Texture;
		}

		public Color Evaluate(float time)
		{
			if (this.textureSize <= 0)
			{
				return Color.black;
			}
			return this.m_Gradient.Evaluate(time);
		}

		public void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys, GradientMode mode, ColorSpace colorSpace)
		{
			this.m_Gradient.SetKeys(colorKeys, alphaKeys);
			this.m_Gradient.mode = mode;
			this.m_Gradient.colorSpace = colorSpace;
			this.Rebuild(colorKeys, alphaKeys, mode, colorSpace, this.m_RequestedTextureSize, this.m_Precise);
		}

		[SerializeField]
		private Gradient m_Gradient;

		private Texture2D m_Texture;

		private int m_RequestedTextureSize = -1;

		private bool m_IsTextureDirty;

		private bool m_Precise;

		[SerializeField]
		[HideInInspector]
		public GradientMode mode = GradientMode.PerceptualBlend;

		[SerializeField]
		[HideInInspector]
		public ColorSpace colorSpace = ColorSpace.Uninitialized;
	}
}
