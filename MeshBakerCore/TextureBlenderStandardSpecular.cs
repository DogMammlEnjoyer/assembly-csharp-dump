using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderStandardSpecular : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("Standard (Specular setup)");
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (shaderTexturePropertyName.Equals("_MainTex"))
			{
				this.propertyToDo = TextureBlenderStandardSpecular.Prop.doColor;
				if (sourceMat.HasProperty("_Color"))
				{
					this.m_tintColor = sourceMat.GetColor("_Color");
				}
				else
				{
					this.m_tintColor = this.m_generatingTintedAtlaColor;
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
				if (!shaderTexturePropertyName.Equals("_SpecGlossMap"))
				{
					if (shaderTexturePropertyName.Equals("_BumpMap"))
					{
						this.propertyToDo = TextureBlenderStandardSpecular.Prop.doBump;
						if (!sourceMat.HasProperty(shaderTexturePropertyName))
						{
							this.m_bumpScale = this.m_generatingTintedAtlaBumpScale;
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
						this.propertyToDo = TextureBlenderStandardSpecular.Prop.doEmission;
						this.m_shaderDoesEmission = sourceMat.IsKeywordEnabled("_EMISSION");
						if (sourceMat.HasProperty("_EmissionColor"))
						{
							this.m_emissionColor = sourceMat.GetColor("_EmissionColor");
							return;
						}
						this.m_generatingTintedAtlaColor = this.m_notGeneratingAtlasDefaultEmisionColor;
						return;
					}
					else
					{
						this.propertyToDo = TextureBlenderStandardSpecular.Prop.doNone;
					}
					return;
				}
				this.propertyToDo = TextureBlenderStandardSpecular.Prop.doSpecular;
				this.m_specColor = this.m_generatingTintedAtlaSpecular;
				if (sourceMat.GetTexture("_SpecGlossMap") != null)
				{
					this.m_hasSpecGlossMap = true;
				}
				else
				{
					this.m_hasSpecGlossMap = false;
				}
				if (sourceMat.HasProperty("_SpecColor"))
				{
					this.m_specColor = sourceMat.GetColor("_SpecColor");
				}
				else
				{
					this.m_specColor = new Color(0f, 0f, 0f, 1f);
				}
				if (sourceMat.HasProperty("_GlossMapScale"))
				{
					this.m_SpecGlossMapScale = sourceMat.GetFloat("_GlossMapScale");
				}
				else
				{
					this.m_SpecGlossMapScale = 1f;
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
			if (this.propertyToDo == TextureBlenderStandardSpecular.Prop.doColor)
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
			if (this.propertyToDo == TextureBlenderStandardSpecular.Prop.doSpecular)
			{
				if (this.m_hasSpecGlossMap)
				{
					pixelColor = new Color(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a * this.m_SpecGlossMapScale);
					return pixelColor;
				}
				Color specColor = this.m_specColor;
				specColor.a = this.m_glossiness;
				return specColor;
			}
			else
			{
				if (this.propertyToDo == TextureBlenderStandardSpecular.Prop.doBump)
				{
					return Color.Lerp(TextureBlenderStandardSpecular.NeutralNormalMap, pixelColor, this.m_bumpScale);
				}
				if (this.propertyToDo != TextureBlenderStandardSpecular.Prop.doEmission)
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
			if (!TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaColor, "_Color"))
			{
				return false;
			}
			if (!TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaSpecular, "_SpecColor"))
			{
				return false;
			}
			if (a.HasProperty("_Mode") && b.HasProperty("_Mode") && a.GetFloat("_Mode") == 1f && b.GetFloat("_Mode") == 1f && a.HasProperty("_Cutoff") && b.HasProperty("_Cutoff") && a.HasProperty("_Cutoff") != b.HasProperty("_Cutoff"))
			{
				return false;
			}
			bool flag = a.HasProperty("_SpecGlossMap") && a.GetTexture("_SpecGlossMap") != null;
			bool flag2 = b.HasProperty("_SpecGlossMap") && b.GetTexture("_SpecGlossMap") != null;
			if (flag && flag2)
			{
				if (!TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlaSpecGlossMapScale, "_GlossMapScale"))
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
				if (!TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlaGlossiness, "_Glossiness"))
				{
					return false;
				}
			}
			return TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlaBumpScale, "_BumpScale") && a.IsKeywordEnabled("_EMISSION") == b.IsKeywordEnabled("_EMISSION") && (!a.IsKeywordEnabled("_EMISSION") || TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaEmission, "_EmissionColor"));
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			if (resultMaterial.GetTexture("_MainTex") != null)
			{
				resultMaterial.SetColor("_Color", this.m_generatingTintedAtlaColor);
				if (resultMaterial.GetFloat("_Mode") == 1f)
				{
					resultMaterial.SetFloat("_Cutoff", 0.5f);
				}
			}
			else
			{
				resultMaterial.SetColor("_Color", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Color", this.m_notGeneratingAtlasDefaultColor));
			}
			if (resultMaterial.GetTexture("_SpecGlossMap") != null)
			{
				resultMaterial.SetColor("_SpecColor", this.m_generatingTintedAtlaSpecular);
				resultMaterial.SetFloat("_GlossMapScale", this.m_generatingTintedAtlaSpecGlossMapScale);
				resultMaterial.SetFloat("_Glossiness", this.m_generatingTintedAtlaGlossiness);
			}
			else
			{
				resultMaterial.SetColor("_SpecColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_SpecColor", this.m_notGeneratingAtlasDefaultSpecularColor));
				resultMaterial.SetFloat("_Glossiness", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Glossiness", this.m_notGeneratingAtlasDefaultGlossiness));
			}
			if (resultMaterial.GetTexture("_BumpMap") != null)
			{
				resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlaBumpScale);
			}
			else
			{
				resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlaBumpScale);
			}
			if (resultMaterial.GetTexture("_EmissionMap") != null)
			{
				resultMaterial.EnableKeyword("_EMISSION");
				resultMaterial.SetColor("_EmissionColor", Color.white);
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
				if (texPropertyName.name.Equals("_SpecGlossMap"))
				{
					if (mat != null && mat.HasProperty("_SpecColor"))
					{
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
							this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_SpecColor", color);
							this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Glossiness", color.a);
							return color;
						}
						catch (Exception)
						{
						}
					}
					return new Color(0f, 0f, 0f, 0.5f);
				}
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
						goto IL_1ED;
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
								goto IL_1ED;
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
					goto IL_1ED;
				}
				Color result;
				return result;
			}
			if (mat != null && mat.HasProperty("_Color"))
			{
				return Color.white;
			}
			IL_1ED:
			return new Color(1f, 1f, 1f, 0f);
		}

		private static Color NeutralNormalMap = new Color(0.5f, 0.5f, 1f);

		private TextureBlenderMaterialPropertyCacheHelper sourceMaterialPropertyCache = new TextureBlenderMaterialPropertyCacheHelper();

		private Color m_tintColor;

		private bool m_doScaleAlphaCutoff;

		private float m_alphaCutoff;

		private float m_glossiness;

		private float m_SpecGlossMapScale;

		private Color m_specColor;

		private bool m_hasSpecGlossMap;

		private float m_bumpScale;

		private bool m_shaderDoesEmission;

		private Color m_emissionColor;

		private TextureBlenderStandardSpecular.Prop propertyToDo = TextureBlenderStandardSpecular.Prop.doNone;

		private Color m_generatingTintedAtlaColor = Color.white;

		private Color m_generatingTintedAtlaSpecular = Color.black;

		private float m_generatingTintedAtlaGlossiness = 1f;

		private float m_generatingTintedAtlaSpecGlossMapScale = 1f;

		private float m_generatingTintedAtlaBumpScale = 1f;

		private Color m_generatingTintedAtlaEmission = Color.white;

		private const float m_generatedAlphaCutoff = 0.5f;

		private Color m_notGeneratingAtlasDefaultColor = Color.white;

		private Color m_notGeneratingAtlasDefaultSpecularColor = new Color(0f, 0f, 0f, 1f);

		private float m_notGeneratingAtlasDefaultGlossiness = 0.5f;

		private Color m_notGeneratingAtlasDefaultEmisionColor = Color.black;

		private enum Prop
		{
			doColor,
			doSpecular,
			doEmission,
			doBump,
			doNone
		}
	}
}
