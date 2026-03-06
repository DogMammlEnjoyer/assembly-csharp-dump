using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderStandardMetallicRoughness : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("Standard (Roughness setup)");
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (shaderTexturePropertyName.Equals("_MainTex"))
			{
				this.propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doColor;
				if (sourceMat.HasProperty("_Color"))
				{
					this.m_tintColor = sourceMat.GetColor("_Color");
					return;
				}
				this.m_tintColor = this.m_generatingTintedAtlasColor;
				return;
			}
			else if (shaderTexturePropertyName.Equals("_MetallicGlossMap"))
			{
				this.propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doMetallic;
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
					return;
				}
				this.m_metallic = 0f;
				return;
			}
			else
			{
				if (!shaderTexturePropertyName.Equals("_SpecGlossMap"))
				{
					if (shaderTexturePropertyName.Equals("_BumpMap"))
					{
						this.propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doBump;
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
						this.propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doEmission;
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
						this.propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doNone;
					}
					return;
				}
				this.propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doRoughness;
				this.m_roughness = this.m_generatingTintedAtlasRoughness;
				if (sourceMat.GetTexture("_SpecGlossMap") != null)
				{
					this.m_hasSpecGlossMap = true;
				}
				else
				{
					this.m_hasSpecGlossMap = false;
				}
				if (sourceMat.HasProperty("_Glossiness"))
				{
					this.m_roughness = sourceMat.GetFloat("_Glossiness");
					return;
				}
				this.m_roughness = 1f;
				return;
			}
		}

		public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
		{
			if (this.propertyToDo == TextureBlenderStandardMetallicRoughness.Prop.doColor)
			{
				return new Color(pixelColor.r * this.m_tintColor.r, pixelColor.g * this.m_tintColor.g, pixelColor.b * this.m_tintColor.b, pixelColor.a * this.m_tintColor.a);
			}
			if (this.propertyToDo == TextureBlenderStandardMetallicRoughness.Prop.doMetallic)
			{
				if (this.m_hasMetallicGlossMap)
				{
					return pixelColor;
				}
				return new Color(this.m_metallic, 0f, 0f, this.m_roughness);
			}
			else if (this.propertyToDo == TextureBlenderStandardMetallicRoughness.Prop.doRoughness)
			{
				if (this.m_hasSpecGlossMap)
				{
					return pixelColor;
				}
				return new Color(this.m_roughness, 0f, 0f, 0f);
			}
			else
			{
				if (this.propertyToDo == TextureBlenderStandardMetallicRoughness.Prop.doBump)
				{
					return Color.Lerp(TextureBlenderStandardMetallicRoughness.NeutralNormalMap, pixelColor, this.m_bumpScale);
				}
				if (this.propertyToDo != TextureBlenderStandardMetallicRoughness.Prop.doEmission)
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
			bool flag = a.HasProperty("_MetallicGlossMap") && a.GetTexture("_MetallicGlossMap") != null;
			bool flag2 = b.HasProperty("_MetallicGlossMap") && b.GetTexture("_MetallicGlossMap") != null;
			if (flag || flag2)
			{
				return false;
			}
			if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultMetallic, "_Metallic"))
			{
				return false;
			}
			bool flag3 = a.HasProperty("_SpecGlossMap") && a.GetTexture("_SpecGlossMap") != null;
			bool flag4 = b.HasProperty("_SpecGlossMap") && b.GetTexture("_SpecGlossMap") != null;
			return !flag3 && !flag4 && TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlasRoughness, "_Glossiness") && TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlasBumpScale, "_bumpScale") && TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlasRoughness, "_Glossiness") && a.IsKeywordEnabled("_EMISSION") == b.IsKeywordEnabled("_EMISSION") && (!a.IsKeywordEnabled("_EMISSION") || TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlasEmission, "_EmissionColor"));
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			if (resultMaterial.GetTexture("_MainTex") != null)
			{
				resultMaterial.SetColor("_Color", this.m_generatingTintedAtlasColor);
			}
			else
			{
				resultMaterial.SetColor("_Color", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Color", this.m_notGeneratingAtlasDefaultColor));
			}
			if (resultMaterial.GetTexture("_MetallicGlossMap") != null)
			{
				resultMaterial.SetFloat("_Metallic", this.m_generatingTintedAtlasMetallic);
			}
			else
			{
				resultMaterial.SetFloat("_Metallic", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Metallic", this.m_notGeneratingAtlasDefaultMetallic));
			}
			if (!(resultMaterial.GetTexture("_SpecGlossMap") != null))
			{
				resultMaterial.SetFloat("_Glossiness", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Glossiness", this.m_notGeneratingAtlasDefaultGlossiness));
			}
			if (resultMaterial.GetTexture("_BumpMap") != null)
			{
				resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlasBumpScale);
			}
			else
			{
				resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlasBumpScale);
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
			if (texPropertyName.name.Equals("_MainTex"))
			{
				if (mat != null && mat.HasProperty("_Color"))
				{
					return Color.white;
				}
			}
			else
			{
				if (!texPropertyName.name.Equals("_MetallicGlossMap"))
				{
					if (texPropertyName.name.Equals("_SpecGlossMap"))
					{
						bool flag = false;
						try
						{
							Color color = new Color(0f, 0f, 0f, 0.5f);
							if (mat.HasProperty("_Glossiness"))
							{
								try
								{
									flag = true;
									color.a = mat.GetFloat("_Glossiness");
								}
								catch (Exception)
								{
								}
							}
							this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Glossiness", color.a);
							return new Color(0f, 0f, 0f, 0.5f);
						}
						catch (Exception)
						{
						}
						if (!flag)
						{
							return new Color(0f, 0f, 0f, 0.5f);
						}
						goto IL_278;
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
								goto IL_278;
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
										goto IL_278;
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
							goto IL_278;
						}
					}
					Color result;
					return result;
				}
				if (mat != null && mat.HasProperty("_Metallic"))
				{
					try
					{
						float @float = mat.GetFloat("_Metallic");
						this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Metallic", @float);
					}
					catch (Exception)
					{
					}
					return new Color(0f, 0f, 0f, 0.5f);
				}
				return new Color(0f, 0f, 0f, 0.5f);
			}
			IL_278:
			return new Color(1f, 1f, 1f, 0f);
		}

		private static Color NeutralNormalMap = new Color(0.5f, 0.5f, 1f);

		private TextureBlenderMaterialPropertyCacheHelper sourceMaterialPropertyCache = new TextureBlenderMaterialPropertyCacheHelper();

		private Color m_tintColor;

		private float m_roughness;

		private float m_metallic;

		private bool m_hasMetallicGlossMap;

		private bool m_hasSpecGlossMap;

		private float m_bumpScale;

		private bool m_shaderDoesEmission;

		private Color m_emissionColor;

		private TextureBlenderStandardMetallicRoughness.Prop propertyToDo = TextureBlenderStandardMetallicRoughness.Prop.doNone;

		private Color m_generatingTintedAtlasColor = Color.white;

		private float m_generatingTintedAtlasMetallic;

		private float m_generatingTintedAtlasRoughness = 0.5f;

		private float m_generatingTintedAtlasBumpScale = 1f;

		private Color m_generatingTintedAtlasEmission = Color.white;

		private Color m_notGeneratingAtlasDefaultColor = Color.white;

		private float m_notGeneratingAtlasDefaultMetallic;

		private float m_notGeneratingAtlasDefaultGlossiness = 0.5f;

		private Color m_notGeneratingAtlasDefaultEmisionColor = Color.black;

		private enum Prop
		{
			doColor,
			doMetallic,
			doRoughness,
			doEmission,
			doBump,
			doNone
		}
	}
}
