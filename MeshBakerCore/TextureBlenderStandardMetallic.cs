using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderStandardMetallic : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("Standard") || shaderName.EndsWith("StandardTextureArray");
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (shaderTexturePropertyName.Equals("_MainTex"))
			{
				this.propertyToDo = TextureBlenderStandardMetallic.Prop.doColor;
				if (sourceMat.HasProperty("_Color"))
				{
					this.m_tintColor = sourceMat.GetColor("_Color");
				}
				else
				{
					this.m_tintColor = this.m_generatingTintedAtlasColor;
				}
				if (sourceMat.HasProperty("_Mode") && sourceMat.HasProperty("_Cutoff") && sourceMat.GetFloat("_Mode") == 1f)
				{
					this.m_doScaleAlphaCutoff = true;
					this.m_alphaCutoff = sourceMat.GetFloat("_Cutoff");
					this.m_alphaCutoff = Mathf.Clamp(this.m_alphaCutoff, 0.0001f, 0.9999f);
					return;
				}
				this.m_doScaleAlphaCutoff = false;
				this.m_alphaCutoff = 0.5f;
				return;
			}
			else
			{
				if (!shaderTexturePropertyName.Equals("_MetallicGlossMap"))
				{
					if (shaderTexturePropertyName.Equals("_BumpMap"))
					{
						this.propertyToDo = TextureBlenderStandardMetallic.Prop.doBump;
						if (!sourceMat.HasProperty(shaderTexturePropertyName))
						{
							this.m_bumpScale = this.m_generatingTintedAtlasBumpScale;
							return;
						}
						if (sourceMat.HasProperty("_BumpScale"))
						{
							this.m_bumpScale = sourceMat.GetFloat("_BumpScale");
							return;
						}
					}
					else if (shaderTexturePropertyName.Equals("_EmissionMap"))
					{
						this.propertyToDo = TextureBlenderStandardMetallic.Prop.doEmission;
						this.m_shaderDoesEmission = sourceMat.IsKeywordEnabled("_EMISSION");
						if (sourceMat.HasProperty("_EmissionColor"))
						{
							this.m_emissionColor = sourceMat.GetColor("_EmissionColor");
							return;
						}
						this.m_emissionColor = this.m_notGeneratingAtlasDefaultEmisionColor;
						return;
					}
					else
					{
						this.propertyToDo = TextureBlenderStandardMetallic.Prop.doNone;
					}
					return;
				}
				this.propertyToDo = TextureBlenderStandardMetallic.Prop.doMetallic;
				this.m_metallic = this.m_generatingTintedAtlasMetallic;
				if (sourceMat.GetTexture("_MetallicGlossMap") != null)
				{
					this.m_hasMetallicGlossMap = true;
				}
				else
				{
					this.m_hasMetallicGlossMap = false;
				}
				if (sourceMat.HasProperty("_Metallic"))
				{
					this.m_metallic = sourceMat.GetFloat("_Metallic");
				}
				else
				{
					this.m_metallic = 0f;
				}
				if (sourceMat.HasProperty("_GlossMapScale"))
				{
					this.m_glossMapScale = sourceMat.GetFloat("_GlossMapScale");
				}
				else
				{
					this.m_glossMapScale = 1f;
				}
				if (sourceMat.HasProperty("_Glossiness"))
				{
					this.m_glossiness = sourceMat.GetFloat("_Glossiness");
					return;
				}
				this.m_glossiness = 0f;
				return;
			}
		}

		public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
		{
			if (this.propertyToDo == TextureBlenderStandardMetallic.Prop.doColor)
			{
				Color color = new Color(pixelColor.r * this.m_tintColor.r, pixelColor.g * this.m_tintColor.g, pixelColor.b * this.m_tintColor.b, pixelColor.a * this.m_tintColor.a);
				if (this.m_doScaleAlphaCutoff)
				{
					if (color.a >= this.m_alphaCutoff)
					{
						color.a = 0.5f + 0.5f * (color.a - this.m_alphaCutoff) / (1f - this.m_alphaCutoff);
					}
					else
					{
						color.a = 0.5f * color.a / this.m_alphaCutoff;
					}
				}
				return color;
			}
			if (this.propertyToDo == TextureBlenderStandardMetallic.Prop.doMetallic)
			{
				if (this.m_hasMetallicGlossMap)
				{
					pixelColor = new Color(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a * this.m_glossMapScale);
					return pixelColor;
				}
				return new Color(this.m_metallic, 0f, 0f, this.m_glossiness);
			}
			else
			{
				if (this.propertyToDo == TextureBlenderStandardMetallic.Prop.doBump)
				{
					return Color.Lerp(TextureBlenderStandardMetallic.NeutralNormalMap, pixelColor, this.m_bumpScale);
				}
				if (this.propertyToDo != TextureBlenderStandardMetallic.Prop.doEmission)
				{
					return pixelColor;
				}
				if (this.m_shaderDoesEmission)
				{
					return new Color(pixelColor.r * this.m_emissionColor.r, pixelColor.g * this.m_emissionColor.g, pixelColor.b * this.m_emissionColor.b, pixelColor.a * this.m_emissionColor.a);
				}
				return Color.black;
			}
		}

		public bool NonTexturePropertiesAreEqual(Material a, Material b)
		{
			if (!TextureBlenderFallback._compareColor(a, b, this.m_notGeneratingAtlasDefaultColor, "_Color"))
			{
				return false;
			}
			if (a.HasProperty("_Mode") && b.HasProperty("_Mode") && a.GetFloat("_Mode") == 1f && b.GetFloat("_Mode") == 1f && a.HasProperty("_Cutoff") && b.HasProperty("_Cutoff") && a.HasProperty("_Cutoff") != b.HasProperty("_Cutoff"))
			{
				return false;
			}
			if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultGlossiness, "_Glossiness"))
			{
				return false;
			}
			bool flag = a.HasProperty("_MetallicGlossMap") && a.GetTexture("_MetallicGlossMap") != null;
			bool flag2 = b.HasProperty("_MetallicGlossMap") && b.GetTexture("_MetallicGlossMap") != null;
			if (flag && flag2)
			{
				if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultMetallic, "_GlossMapScale"))
				{
					return false;
				}
			}
			else
			{
				if (flag || flag2)
				{
					return false;
				}
				if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultMetallic, "_Metallic"))
				{
					return false;
				}
			}
			return a.IsKeywordEnabled("_EMISSION") == b.IsKeywordEnabled("_EMISSION") && (!a.IsKeywordEnabled("_EMISSION") || TextureBlenderFallback._compareColor(a, b, this.m_notGeneratingAtlasDefaultEmisionColor, "_EmissionColor"));
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			if (resultMaterial.GetTexture("_MainTex") != null)
			{
				resultMaterial.SetColor("_Color", this.m_generatingTintedAtlasColor);
				if (resultMaterial.GetFloat("_Mode") == 1f)
				{
					resultMaterial.SetFloat("_Cutoff", 0.5f);
				}
			}
			else
			{
				resultMaterial.SetColor("_Color", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Color", this.m_notGeneratingAtlasDefaultColor));
			}
			if (resultMaterial.GetTexture("_MetallicGlossMap") != null)
			{
				resultMaterial.SetFloat("_Metallic", this.m_generatingTintedAtlasMetallic);
				resultMaterial.SetFloat("_GlossMapScale", this.m_generatingTintedAtlasGlossMapScale);
				resultMaterial.SetFloat("_Glossiness", this.m_generatingTintedAtlasGlossiness);
			}
			else
			{
				resultMaterial.SetFloat("_Metallic", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Metallic", this.m_notGeneratingAtlasDefaultMetallic));
				resultMaterial.SetFloat("_Glossiness", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Glossiness", this.m_notGeneratingAtlasDefaultGlossiness));
			}
			if (resultMaterial.GetTexture("_BumpMap") != null)
			{
				resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlasBumpScale);
			}
			if (resultMaterial.GetTexture("_EmissionMap") != null)
			{
				resultMaterial.EnableKeyword("_EMISSION");
				resultMaterial.SetColor("_EmissionColor", this.m_generatingTintedAtlasEmission);
				return;
			}
			resultMaterial.DisableKeyword("_EMISSION");
			resultMaterial.SetColor("_EmissionColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_EmissionColor", this.m_notGeneratingAtlasDefaultEmisionColor));
		}

		public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
		{
			if (texPropertyName.name.Equals("_BumpMap"))
			{
				return TextureBlenderFallback.GetDefaultNormalMapColor();
			}
			if (!texPropertyName.name.Equals("_MainTex"))
			{
				if (texPropertyName.name.Equals("_MetallicGlossMap"))
				{
					if (mat != null && mat.HasProperty("_Metallic"))
					{
						try
						{
							float @float = mat.GetFloat("_Metallic");
							Color color = new Color(@float, @float, @float);
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
							this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Metallic", @float);
							this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Glossiness", color.a);
							return color;
						}
						catch (Exception)
						{
						}
						return new Color(0f, 0f, 0f, 0.5f);
					}
					return new Color(0f, 0f, 0f, 0.5f);
				}
				else
				{
					if (texPropertyName.name.Equals("_ParallaxMap"))
					{
						return new Color(0f, 0f, 0f, 0f);
					}
					if (texPropertyName.name.Equals("_OcclusionMap"))
					{
						return new Color(1f, 1f, 1f, 1f);
					}
					if (texPropertyName.name.Equals("_EmissionMap"))
					{
						if (!(mat != null))
						{
							goto IL_217;
						}
						if (mat.IsKeywordEnabled("_EMISSION"))
						{
							if (mat.HasProperty("_EmissionColor"))
							{
								try
								{
									Color color2 = mat.GetColor("_EmissionColor");
									this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_EmissionColor", color2);
									return color2;
								}
								catch (Exception)
								{
									goto IL_217;
								}
							}
							return Color.black;
						}
						return Color.black;
					}
					else
					{
						if (texPropertyName.name.Equals("_DetailMask"))
						{
							return new Color(0f, 0f, 0f, 0f);
						}
						goto IL_217;
					}
				}
				Color result;
				return result;
			}
			if (mat != null && mat.HasProperty("_Color"))
			{
				return Color.white;
			}
			IL_217:
			return new Color(1f, 1f, 1f, 0f);
		}

		private static Color NeutralNormalMap = new Color(0.5f, 0.5f, 1f);

		private TextureBlenderMaterialPropertyCacheHelper sourceMaterialPropertyCache = new TextureBlenderMaterialPropertyCacheHelper();

		private Color m_tintColor;

		private bool m_doScaleAlphaCutoff;

		private float m_alphaCutoff;

		private float m_glossiness;

		private float m_glossMapScale;

		private float m_metallic;

		private bool m_hasMetallicGlossMap;

		private float m_bumpScale;

		private bool m_shaderDoesEmission;

		private Color m_emissionColor;

		private TextureBlenderStandardMetallic.Prop propertyToDo = TextureBlenderStandardMetallic.Prop.doNone;

		private Color m_generatingTintedAtlasColor = Color.white;

		private float m_generatingTintedAtlasMetallic;

		private float m_generatingTintedAtlasGlossiness = 1f;

		private float m_generatingTintedAtlasGlossMapScale = 1f;

		private float m_generatingTintedAtlasBumpScale = 1f;

		private Color m_generatingTintedAtlasEmission = Color.white;

		private const float m_generatedAlphaCutoff = 0.5f;

		private Color m_notGeneratingAtlasDefaultColor = Color.white;

		private float m_notGeneratingAtlasDefaultMetallic;

		private float m_notGeneratingAtlasDefaultGlossiness = 0.5f;

		private Color m_notGeneratingAtlasDefaultEmisionColor = Color.black;

		private enum Prop
		{
			doColor,
			doMetallic,
			doEmission,
			doBump,
			doNone
		}
	}
}
