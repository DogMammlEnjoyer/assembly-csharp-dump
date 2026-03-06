using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderFallback : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return true;
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (shaderTexturePropertyName.Equals("_MainTex"))
			{
				this.m_doTintColor = true;
				this.m_tintColor = Color.white;
				if (sourceMat.HasProperty("_Color"))
				{
					this.m_tintColor = sourceMat.GetColor("_Color");
					return;
				}
				if (sourceMat.HasProperty("_TintColor"))
				{
					this.m_tintColor = sourceMat.GetColor("_TintColor");
					return;
				}
			}
			else
			{
				this.m_doTintColor = false;
			}
		}

		public Color OnBlendTexturePixel(string shaderPropertyName, Color pixelColor)
		{
			if (this.m_doTintColor)
			{
				return new Color(pixelColor.r * this.m_tintColor.r, pixelColor.g * this.m_tintColor.g, pixelColor.b * this.m_tintColor.b, pixelColor.a * this.m_tintColor.a);
			}
			return pixelColor;
		}

		public bool NonTexturePropertiesAreEqual(Material a, Material b)
		{
			if (a.HasProperty("_Color"))
			{
				if (TextureBlenderFallback._compareColor(a, b, this.m_defaultColor, "_Color"))
				{
					return true;
				}
			}
			else if (a.HasProperty("_TintColor") && TextureBlenderFallback._compareColor(a, b, this.m_defaultColor, "_TintColor"))
			{
				return true;
			}
			return false;
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			if (resultMaterial.HasProperty("_Color"))
			{
				resultMaterial.SetColor("_Color", this.m_defaultColor);
				return;
			}
			if (resultMaterial.HasProperty("_TintColor"))
			{
				resultMaterial.SetColor("_TintColor", this.m_defaultColor);
			}
		}

		public static Color GetDefaultNormalMapColor()
		{
			if (MBVersion.IsSwizzledNormalMapPlatform())
			{
				return new Color(1f, 0.5f, 0.5f, 0.5f);
			}
			return new Color(0.5f, 0.5f, 1f, 0.5f);
		}

		public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texProperty)
		{
			if (texProperty.isNormalMap)
			{
				return TextureBlenderFallback.GetDefaultNormalMapColor();
			}
			if (!texProperty.name.Equals("_MainTex"))
			{
				if (texProperty.name.Equals("_SpecGlossMap"))
				{
					if (!(mat != null) || !mat.HasProperty("_SpecColor"))
					{
						goto IL_2A9;
					}
					try
					{
						Color color = mat.GetColor("_SpecColor");
						if (mat.HasProperty("_Glossiness"))
						{
							try
							{
								color.a = mat.GetFloat("_Glossiness");
							}
							catch (Exception)
							{
							}
						}
						return color;
					}
					catch (Exception)
					{
						goto IL_2A9;
					}
				}
				if (texProperty.name.Equals("_MetallicGlossMap"))
				{
					if (!(mat != null) || !mat.HasProperty("_Metallic"))
					{
						goto IL_2A9;
					}
					try
					{
						float @float = mat.GetFloat("_Metallic");
						Color result = new Color(@float, @float, @float);
						if (mat.HasProperty("_Glossiness"))
						{
							try
							{
								result.a = mat.GetFloat("_Glossiness");
							}
							catch (Exception)
							{
							}
						}
						return result;
					}
					catch (Exception)
					{
						goto IL_2A9;
					}
				}
				if (texProperty.name.Equals("_ParallaxMap"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
				if (texProperty.name.Equals("_OcclusionMap"))
				{
					return new Color(1f, 1f, 1f, 1f);
				}
				if (texProperty.name.Equals("_EmissionMap"))
				{
					if (!(mat != null) || !mat.HasProperty("_EmissionScaleUI"))
					{
						goto IL_2A9;
					}
					if (mat.HasProperty("_EmissionColor") && mat.HasProperty("_EmissionColorUI"))
					{
						try
						{
							Color color2 = mat.GetColor("_EmissionColor");
							Color color3 = mat.GetColor("_EmissionColorUI");
							float float2 = mat.GetFloat("_EmissionScaleUI");
							if (color2 == new Color(0f, 0f, 0f, 0f) && color3 == new Color(1f, 1f, 1f, 1f))
							{
								return new Color(float2, float2, float2, float2);
							}
							return color3;
						}
						catch (Exception)
						{
							goto IL_2A9;
						}
					}
					try
					{
						float float3 = mat.GetFloat("_EmissionScaleUI");
						return new Color(float3, float3, float3, float3);
					}
					catch (Exception)
					{
						goto IL_2A9;
					}
				}
				if (texProperty.name.Equals("_DetailMask"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
				goto IL_2A9;
			}
			if (mat != null && mat.HasProperty("_Color"))
			{
				return Color.white;
			}
			if (mat != null && mat.HasProperty("_TintColor"))
			{
				return Color.white;
			}
			IL_2A9:
			return new Color(1f, 1f, 1f, 0f);
		}

		public static bool _compareColor(Material a, Material b, Color defaultVal, string propertyName)
		{
			Color lhs = defaultVal;
			Color rhs = defaultVal;
			if (a.HasProperty(propertyName))
			{
				lhs = a.GetColor(propertyName);
			}
			if (b.HasProperty(propertyName))
			{
				rhs = b.GetColor(propertyName);
			}
			return !(lhs != rhs);
		}

		public static bool _compareFloat(Material a, Material b, float defaultVal, string propertyName)
		{
			float num = defaultVal;
			float num2 = defaultVal;
			if (a.HasProperty(propertyName))
			{
				num = a.GetFloat(propertyName);
			}
			if (b.HasProperty(propertyName))
			{
				num2 = b.GetFloat(propertyName);
			}
			return num == num2;
		}

		private bool m_doTintColor;

		private Color m_tintColor;

		private Color m_defaultColor = Color.white;
	}
}
