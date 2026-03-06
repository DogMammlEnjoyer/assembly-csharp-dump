using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderHDRPLit : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("HDRP/Lit");
		}

		private TextureBlenderHDRPLit.MaterialType _MapFloatToMaterialType(float materialType)
		{
			if (materialType == 0f)
			{
				return TextureBlenderHDRPLit.MaterialType.subsurfaceScattering;
			}
			if (materialType == 1f)
			{
				return TextureBlenderHDRPLit.MaterialType.standard;
			}
			if (materialType == 2f)
			{
				return TextureBlenderHDRPLit.MaterialType.anisotropy;
			}
			if (materialType == 3f)
			{
				return TextureBlenderHDRPLit.MaterialType.iridescence;
			}
			if (materialType == 4f)
			{
				return TextureBlenderHDRPLit.MaterialType.specularColor;
			}
			if (materialType == 5f)
			{
				return TextureBlenderHDRPLit.MaterialType.translucent;
			}
			return TextureBlenderHDRPLit.MaterialType.unknown;
		}

		private float _MapMaterialTypeToFloat(TextureBlenderHDRPLit.MaterialType materialType)
		{
			if (materialType == TextureBlenderHDRPLit.MaterialType.subsurfaceScattering)
			{
				return 0f;
			}
			if (materialType == TextureBlenderHDRPLit.MaterialType.standard)
			{
				return 1f;
			}
			if (materialType == TextureBlenderHDRPLit.MaterialType.anisotropy)
			{
				return 2f;
			}
			if (materialType == TextureBlenderHDRPLit.MaterialType.iridescence)
			{
				return 3f;
			}
			if (materialType == TextureBlenderHDRPLit.MaterialType.specularColor)
			{
				return 4f;
			}
			if (materialType == TextureBlenderHDRPLit.MaterialType.translucent)
			{
				return 5f;
			}
			return -1f;
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (this.m_materialType == TextureBlenderHDRPLit.MaterialType.unknown)
			{
				if (sourceMat.HasProperty("_MaterialID"))
				{
					this.m_materialType = this._MapFloatToMaterialType(sourceMat.GetFloat("_MaterialID"));
				}
			}
			else if (sourceMat.HasProperty("_MaterialID") && this._MapFloatToMaterialType(sourceMat.GetFloat("_MaterialID")) != this.m_materialType)
			{
				Debug.LogError("Using the High Definition Render Pipeline TextureBlender to blend non-texture-properties. Some of the source materials use different 'MaterialType'. These  cannot be blended properly. Results will be unpredictable.");
			}
			if (shaderTexturePropertyName.Equals("_BaseColorMap"))
			{
				this.propertyToDo = TextureBlenderHDRPLit.Prop.doColor;
				if (sourceMat.HasProperty("_BaseColor"))
				{
					this.m_tintColor = sourceMat.GetColor("_BaseColor");
					return;
				}
				this.m_tintColor = this.m_notGeneratingAtlasDefaultColor;
				return;
			}
			else
			{
				if (!shaderTexturePropertyName.Equals("_MaskMap"))
				{
					if (shaderTexturePropertyName.Equals("_SpecularColorMap") && this.m_materialType == TextureBlenderHDRPLit.MaterialType.specularColor)
					{
						this.propertyToDo = TextureBlenderHDRPLit.Prop.doSpecular;
						if (sourceMat.HasProperty("_SpecularColorMap") && sourceMat.GetTexture("_SpecularColorMap") != null)
						{
							this.m_hasSpecMap = true;
						}
						else
						{
							this.m_hasSpecMap = false;
						}
						if (sourceMat.HasProperty("_SpecularColor"))
						{
							this.m_specularColor = sourceMat.GetColor("_SpecularColor");
							return;
						}
					}
					else if (shaderTexturePropertyName.Equals("_EmissiveColorMap"))
					{
						this.propertyToDo = TextureBlenderHDRPLit.Prop.doEmission;
						if (sourceMat.HasProperty("_EmissiveColor"))
						{
							this.m_emissiveColor = sourceMat.GetColor("_EmissiveColor");
							return;
						}
						this.m_emissiveColor = this.m_notGeneratingAtlasDefaultEmissiveColor;
						return;
					}
					else
					{
						this.propertyToDo = TextureBlenderHDRPLit.Prop.doNone;
					}
					return;
				}
				this.propertyToDo = TextureBlenderHDRPLit.Prop.doMask;
				if (sourceMat.HasProperty("_MaskMap") && sourceMat.GetTexture("_MaskMap") != null)
				{
					this.m_hasMaskMap = true;
					return;
				}
				this.m_hasMaskMap = false;
				if (this.m_materialType == TextureBlenderHDRPLit.MaterialType.standard && sourceMat.HasProperty("_Metallic"))
				{
					this.m_metallic = sourceMat.GetFloat("_Metallic");
				}
				else
				{
					this.m_metallic = this.m_notGeneratingAtlasDefaultMetallic;
				}
				if (sourceMat.HasProperty("_Smoothness"))
				{
					this.m_smoothness = sourceMat.GetFloat("_Smoothness");
					return;
				}
				this.m_smoothness = this.m_notGeneratingAtlasDefaultSmoothness;
				return;
			}
		}

		public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
		{
			if (this.propertyToDo == TextureBlenderHDRPLit.Prop.doColor)
			{
				return new Color(pixelColor.r * this.m_tintColor.r, pixelColor.g * this.m_tintColor.g, pixelColor.b * this.m_tintColor.b, pixelColor.a * this.m_tintColor.a);
			}
			if (this.propertyToDo == TextureBlenderHDRPLit.Prop.doMask)
			{
				if (this.m_hasMaskMap)
				{
					return new Color(pixelColor.r * this.m_metallic, pixelColor.g, pixelColor.b, pixelColor.a * this.m_smoothness);
				}
				return new Color(this.m_metallic, 0f, 0f, this.m_smoothness);
			}
			else if (this.propertyToDo == TextureBlenderHDRPLit.Prop.doSpecular)
			{
				if (this.m_hasSpecMap)
				{
					return new Color(pixelColor.r * this.m_specularColor.r, pixelColor.g * this.m_specularColor.g, pixelColor.b * this.m_specularColor.g, pixelColor.a * this.m_specularColor.a);
				}
				return this.m_specularColor;
			}
			else
			{
				if (this.propertyToDo == TextureBlenderHDRPLit.Prop.doEmission)
				{
					return new Color(pixelColor.r * this.m_emissiveColor.r, pixelColor.g * this.m_emissiveColor.g, pixelColor.b * this.m_emissiveColor.b, pixelColor.a * this.m_emissiveColor.a);
				}
				return pixelColor;
			}
		}

		public bool NonTexturePropertiesAreEqual(Material a, Material b)
		{
			if (!TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaColor, "_BaseColor"))
			{
				return false;
			}
			bool flag = a.HasProperty("_MaskMap") && a.GetTexture("_MaskMap") != null;
			bool flag2 = b.HasProperty("_MaskMap") && b.GetTexture("_MaskMap") != null;
			return (flag || flag2 || TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultMetallic, "_Metallic") || TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultSmoothness, "_Smoothness")) && (this.m_materialType != TextureBlenderHDRPLit.MaterialType.specularColor || TextureBlenderFallback._compareColor(a, b, this.m_notGeneratingAtlasDefaultSpecular, "_SpecularColor")) && TextureBlenderFallback._compareColor(a, b, this.m_notGeneratingAtlasDefaultEmissiveColor, "_EmissiveColor");
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			if (this.m_materialType != TextureBlenderHDRPLit.MaterialType.unknown)
			{
				resultMaterial.SetFloat("_MaterialID", this._MapMaterialTypeToFloat(this.m_materialType));
			}
			if (resultMaterial.GetTexture("_BaseColorMap") != null)
			{
				resultMaterial.SetColor("_BaseColor", this.m_generatingTintedAtlaColor);
			}
			else
			{
				resultMaterial.SetColor("_BaseColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_BaseColor", this.m_notGeneratingAtlasDefaultColor));
			}
			if (!(resultMaterial.GetTexture("_MaskMap") != null))
			{
				resultMaterial.SetFloat("_Metallic", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Metallic", this.m_notGeneratingAtlasDefaultMetallic));
				resultMaterial.SetFloat("_Smoothness", this.m_notGeneratingAtlasDefaultSmoothness);
			}
			if (this.m_materialType == TextureBlenderHDRPLit.MaterialType.specularColor)
			{
				if (resultMaterial.GetTexture("_SpecularColorMap") != null)
				{
					resultMaterial.SetColor("_SpecularColor", this.m_generatingTintedAtlaSpecular);
					resultMaterial.SetFloat("_AORemapMin", 1f);
				}
				else
				{
					resultMaterial.SetColor("_SpecularColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_SpecularColor", this.m_notGeneratingAtlasDefaultSpecular));
					resultMaterial.SetFloat("_AORemapMin", 1f);
				}
			}
			if (resultMaterial.GetTexture("_EmissiveColorMap") != null)
			{
				resultMaterial.SetColor("_EmissiveColor", this.m_generatingTintedAtlaEmission);
				return;
			}
			resultMaterial.SetColor("_EmissiveColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_EmissiveColor", this.m_notGeneratingAtlasDefaultEmissiveColor));
		}

		public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
		{
			if (texPropertyName.name.Equals("_BaseColorMap"))
			{
				if (mat != null && mat.HasProperty("_BaseColor"))
				{
					return this.m_notGeneratingAtlasDefaultColor;
				}
			}
			else
			{
				if (texPropertyName.name.Equals("_BumpMap"))
				{
					return TextureBlenderFallback.GetDefaultNormalMapColor();
				}
				if (texPropertyName.name.Equals("_Metallic"))
				{
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
						return new Color(0f, 0f, 0f, this.m_notGeneratingAtlasDefaultSmoothness);
					}
					return new Color(0f, 0f, 0f, this.m_notGeneratingAtlasDefaultSmoothness);
				}
				else if (texPropertyName.name.Equals("_Smoothness"))
				{
					if (mat != null && mat.HasProperty("_Smoothness"))
					{
						try
						{
							float float2 = mat.GetFloat("_Smoothness");
							this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Smoothness", float2);
						}
						catch (Exception)
						{
						}
						return new Color(0f, 0f, 0f, this.m_notGeneratingAtlasDefaultSmoothness);
					}
					return new Color(0f, 0f, 0f, this.m_notGeneratingAtlasDefaultSmoothness);
				}
				else
				{
					if (texPropertyName.name.Equals("_SpecularColorMap"))
					{
						if (mat != null && mat.HasProperty("_SpecularColor"))
						{
							try
							{
								Color color = mat.GetColor("_SpecularColor");
								this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_SpecularColor", color);
							}
							catch (Exception)
							{
							}
						}
						return this.m_notGeneratingAtlasDefaultSpecular;
					}
					if (texPropertyName.name.Equals("_EmissiveColorMap") && mat != null)
					{
						if (mat.HasProperty("_EmissiveColor"))
						{
							try
							{
								Color color2 = mat.GetColor("_EmissiveColor");
								this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_EmissiveColor", color2);
								goto IL_207;
							}
							catch (Exception)
							{
								goto IL_207;
							}
						}
						return this.m_notGeneratingAtlasDefaultEmissiveColor;
					}
				}
			}
			IL_207:
			return new Color(1f, 1f, 1f, 0f);
		}

		private TextureBlenderMaterialPropertyCacheHelper sourceMaterialPropertyCache = new TextureBlenderMaterialPropertyCacheHelper();

		private TextureBlenderHDRPLit.MaterialType m_materialType;

		private Color m_tintColor;

		private bool m_hasMaskMap;

		private float m_smoothness;

		private float m_metallic;

		private bool m_hasSpecMap;

		private Color m_specularColor;

		private Color m_emissiveColor;

		private TextureBlenderHDRPLit.Prop propertyToDo = TextureBlenderHDRPLit.Prop.doNone;

		private Color m_generatingTintedAtlaColor = Color.white;

		private Color m_generatingTintedAtlaSpecular = Color.white;

		private Color m_generatingTintedAtlaEmission = Color.white;

		private Color m_notGeneratingAtlasDefaultColor = Color.white;

		private float m_notGeneratingAtlasDefaultMetallic;

		private float m_notGeneratingAtlasDefaultSmoothness = 0.5f;

		private Color m_notGeneratingAtlasDefaultSpecular = Color.white;

		private Color m_notGeneratingAtlasDefaultEmissiveColor = Color.black;

		private enum Prop
		{
			doColor,
			doMask,
			doSpecular,
			doEmission,
			doNone
		}

		private enum MaterialType
		{
			unknown,
			subsurfaceScattering,
			standard,
			anisotropy,
			iridescence,
			specularColor,
			translucent
		}
	}
}
