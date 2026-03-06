using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderURPLit : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("Universal Render Pipeline/Lit") || shaderName.Equals("Universal Render Pipeline/Simple Lit") || shaderName.Equals("Universal Render Pipeline/Baked Lit") || shaderName.Equals("Universal Render Pipeline/Unlit") || shaderName.Equals("Universal Render Pipeline/Complex Lit") || shaderName.Equals("Universal Render Pipeline/Particles/Lit") || shaderName.Equals("Universal Render Pipeline/Particles/Unlit") || shaderName.Equals("Universal Render Pipeline/Particles/Simple Lit");
		}

		private TextureBlenderURPLit.WorkflowMode _MapFloatToWorkflowMode(float workflowMode)
		{
			if (workflowMode == 0f)
			{
				return TextureBlenderURPLit.WorkflowMode.specular;
			}
			return TextureBlenderURPLit.WorkflowMode.metallic;
		}

		private float _MapWorkflowModeToFloat(TextureBlenderURPLit.WorkflowMode workflowMode)
		{
			if (workflowMode == TextureBlenderURPLit.WorkflowMode.specular)
			{
				return 0f;
			}
			return 1f;
		}

		private TextureBlenderURPLit.SmoothnessTextureChannel _MapFloatToTextureChannel(float texChannel)
		{
			if (texChannel == 0f)
			{
				return TextureBlenderURPLit.SmoothnessTextureChannel.metallicSpecular;
			}
			return TextureBlenderURPLit.SmoothnessTextureChannel.albedo;
		}

		private float _MapTextureChannelToFloat(TextureBlenderURPLit.SmoothnessTextureChannel workflowMode)
		{
			if (workflowMode == TextureBlenderURPLit.SmoothnessTextureChannel.metallicSpecular)
			{
				return 0f;
			}
			return 1f;
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.unknown)
			{
				if (sourceMat.HasProperty("_WorkflowMode"))
				{
					this.m_workflowMode = this._MapFloatToWorkflowMode(sourceMat.GetFloat("_WorkflowMode"));
				}
			}
			else if (sourceMat.HasProperty("_WorkflowMode") && this._MapFloatToWorkflowMode(sourceMat.GetFloat("_WorkflowMode")) != this.m_workflowMode)
			{
				Debug.LogError("Using the Universal Render Pipeline TextureBlender to blend non-texture-propertyes. Some of the source materials used different 'WorkflowModes'. These  cannot be blended properly. Results will be unpredictable.");
			}
			if (this.m_smoothnessTextureChannel == TextureBlenderURPLit.SmoothnessTextureChannel.unknown)
			{
				if (sourceMat.HasProperty("_SmoothnessTextureChannel"))
				{
					this.m_smoothnessTextureChannel = this._MapFloatToTextureChannel(sourceMat.GetFloat("_SmoothnessTextureChannel"));
				}
			}
			else if (sourceMat.HasProperty("_SmoothnessTextureChannel") && this._MapFloatToTextureChannel(sourceMat.GetFloat("_SmoothnessTextureChannel")) != this.m_smoothnessTextureChannel)
			{
				Debug.LogError("Using the Universal Render Pipeline TextureBlender to blend non-texture-properties. Some of the source materials store smoothness in the Albedo texture alpha and some source materials store smoothness in the Metallic/Specular texture alpha channel. The result material can only read smoothness from one or the other. Results will be unpredictable.");
			}
			if (!shaderTexturePropertyName.Equals("_BaseMap"))
			{
				if (shaderTexturePropertyName.Equals("_SpecGlossMap"))
				{
					this.propertyToDo = TextureBlenderURPLit.Prop.doSpecular;
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
					if (sourceMat.HasProperty("_Smoothness") && this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.specular)
					{
						this.m_smoothness = sourceMat.GetFloat("_Smoothness");
						return;
					}
					if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.specular)
					{
						this.m_smoothness = 1f;
						return;
					}
				}
				else if (shaderTexturePropertyName.Equals("_MetallicGlossMap"))
				{
					this.propertyToDo = TextureBlenderURPLit.Prop.doMetallic;
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
					if (sourceMat.HasProperty("_Smoothness") && this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.metallic)
					{
						this.m_smoothness = sourceMat.GetFloat("_Smoothness");
						return;
					}
					if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.metallic)
					{
						this.m_smoothness = 0f;
						return;
					}
				}
				else if (shaderTexturePropertyName.Equals("_BumpMap"))
				{
					this.propertyToDo = TextureBlenderURPLit.Prop.doBump;
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
					this.propertyToDo = TextureBlenderURPLit.Prop.doEmission;
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
					this.propertyToDo = TextureBlenderURPLit.Prop.doNone;
				}
				return;
			}
			this.propertyToDo = TextureBlenderURPLit.Prop.doColor;
			if (sourceMat.HasProperty("_BaseColor"))
			{
				this.m_tintColor = sourceMat.GetColor("_BaseColor");
			}
			else
			{
				this.m_tintColor = this.m_generatingTintedAtlaColor;
			}
			if (sourceMat.HasProperty("_Surface") && sourceMat.HasProperty("_AlphaClip") && sourceMat.HasProperty("_Cutoff") && sourceMat.GetFloat("_Surface") == 1f && sourceMat.GetFloat("_AlphaClip") == 1f)
			{
				this.m_doScaleAlphaCutoff = true;
				this.m_alphaCutoff = sourceMat.GetFloat("_Cutoff");
				this.m_alphaCutoff = Mathf.Clamp(this.m_alphaCutoff, 0.0001f, 0.9999f);
				return;
			}
			this.m_doScaleAlphaCutoff = false;
			this.m_alphaCutoff = 0.5f;
		}

		public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
		{
			if (this.propertyToDo == TextureBlenderURPLit.Prop.doColor)
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
			if (this.propertyToDo == TextureBlenderURPLit.Prop.doMetallic)
			{
				if (this.m_hasMetallicGlossMap)
				{
					pixelColor = new Color(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a * this.m_smoothness);
					return pixelColor;
				}
				return new Color(this.m_metallic, 0f, 0f, this.m_smoothness);
			}
			else if (this.propertyToDo == TextureBlenderURPLit.Prop.doSpecular)
			{
				if (this.m_hasSpecGlossMap)
				{
					pixelColor = new Color(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a * this.m_smoothness);
					return pixelColor;
				}
				Color specColor = this.m_specColor;
				specColor.a = this.m_smoothness;
				return specColor;
			}
			else
			{
				if (this.propertyToDo == TextureBlenderURPLit.Prop.doBump)
				{
					return Color.Lerp(TextureBlenderURPLit.NeutralNormalMap, pixelColor, this.m_bumpScale);
				}
				if (this.propertyToDo != TextureBlenderURPLit.Prop.doEmission)
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
			if (!TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaColor, "_BaseColor"))
			{
				return false;
			}
			if (!TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaSpecular, "_SpecColor"))
			{
				return false;
			}
			if (a.HasProperty("_Surface") && b.HasProperty("_Surface") && a.GetFloat("_AlphaClip") == 1f && b.GetFloat("_AlphaClip") == 1f && a.HasProperty("_Cutoff") && b.HasProperty("_Cutoff") && a.HasProperty("_Cutoff") != b.HasProperty("_Cutoff"))
			{
				return false;
			}
			if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.specular)
			{
				bool flag = a.HasProperty("_SpecGlossMap") && a.GetTexture("_SpecGlossMap") != null;
				bool flag2 = b.HasProperty("_SpecGlossMap") && b.GetTexture("_SpecGlossMap") != null;
				if (flag && flag2)
				{
					if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultSmoothness_SpecularWorkflow, "_Smoothness"))
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
					if (!TextureBlenderFallback._compareColor(a, b, this.m_notGeneratingAtlasDefaultSpecularColor, "_SpecColor") && !TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultSmoothness_SpecularWorkflow, "_Smoothness"))
					{
						return false;
					}
				}
			}
			if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.metallic)
			{
				bool flag3 = a.HasProperty("_MetallicGlossMap") && a.GetTexture("_MetallicGlossMap") != null;
				bool flag4 = b.HasProperty("_MetallicGlossMap") && b.GetTexture("_MetallicGlossMap") != null;
				if (flag3 && flag4)
				{
					if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultSmoothness_MetallicWorkflow, "_Smoothness"))
					{
						return false;
					}
				}
				else
				{
					if (flag3 || flag4)
					{
						return false;
					}
					if (!TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultMetallic, "_Metallic") && !TextureBlenderFallback._compareFloat(a, b, this.m_notGeneratingAtlasDefaultSmoothness_MetallicWorkflow, "_Smoothness"))
					{
						return false;
					}
				}
			}
			return TextureBlenderFallback._compareFloat(a, b, this.m_generatingTintedAtlaBumpScale, "_BumpScale") && a.IsKeywordEnabled("_EMISSION") == b.IsKeywordEnabled("_EMISSION") && (!a.IsKeywordEnabled("_EMISSION") || TextureBlenderFallback._compareColor(a, b, this.m_generatingTintedAtlaEmission, "_EmissionColor"));
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			if (this.m_workflowMode != TextureBlenderURPLit.WorkflowMode.unknown)
			{
				resultMaterial.SetFloat("_WorkflowMode", this._MapWorkflowModeToFloat(this.m_workflowMode));
			}
			if (this.m_smoothnessTextureChannel != TextureBlenderURPLit.SmoothnessTextureChannel.unknown)
			{
				resultMaterial.SetFloat("_SmoothnessTextureChannel", this._MapTextureChannelToFloat(this.m_smoothnessTextureChannel));
			}
			if (resultMaterial.GetTexture("_BaseMap") != null)
			{
				resultMaterial.SetColor("_BaseColor", this.m_generatingTintedAtlaColor);
				if (resultMaterial.GetFloat("_Surface") == 1f && resultMaterial.GetFloat("_AlphaClip") == 1f && resultMaterial.HasProperty("_Cutoff"))
				{
					resultMaterial.SetFloat("_Cutoff", 0.5f);
				}
			}
			else
			{
				resultMaterial.SetColor("_BaseColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_BaseColor", this.m_notGeneratingAtlasDefaultColor));
			}
			if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.specular && resultMaterial.HasProperty("_SpecGlossMap"))
			{
				if (resultMaterial.GetTexture("_SpecGlossMap") != null)
				{
					resultMaterial.SetColor("_SpecColor", this.m_generatingTintedAtlaSpecular);
					resultMaterial.SetFloat("_Smoothness", this.m_generatingTintedAtlasSpecular_somoothness);
				}
				else
				{
					resultMaterial.SetColor("_SpecColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_SpecColor", this.m_notGeneratingAtlasDefaultSpecularColor));
					resultMaterial.SetFloat("_Smoothness", this.m_smoothness);
				}
			}
			if (this.m_workflowMode == TextureBlenderURPLit.WorkflowMode.metallic && resultMaterial.HasProperty("_MetallicGlossMap"))
			{
				if (resultMaterial.GetTexture("_MetallicGlossMap") != null)
				{
					resultMaterial.SetFloat("_Metallic", this.m_generatingTintedAtlasMetallic);
					resultMaterial.SetFloat("_Smoothness", this.m_generatingTintedAtlasMetallic_smoothness);
				}
				else
				{
					resultMaterial.SetFloat("_Metallic", (float)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_Metallic", this.m_notGeneratingAtlasDefaultMetallic));
					resultMaterial.SetFloat("_Smoothness", this.m_smoothness);
				}
			}
			if (resultMaterial.HasProperty("_BumpMap") && resultMaterial.HasProperty("_BumpScale"))
			{
				if (resultMaterial.GetTexture("_BumpMap") != null)
				{
					resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlaBumpScale);
				}
				else
				{
					resultMaterial.SetFloat("_BumpScale", this.m_generatingTintedAtlaBumpScale);
				}
			}
			if (resultMaterial.HasProperty("_EmissionMap") && resultMaterial.HasProperty("_EmissionColor"))
			{
				if (resultMaterial.GetTexture("_EmissionMap") != null)
				{
					resultMaterial.EnableKeyword("_EMISSION");
					resultMaterial.SetColor("_EmissionColor", Color.white);
					return;
				}
				resultMaterial.DisableKeyword("_EMISSION");
				resultMaterial.SetColor("_EmissionColor", (Color)this.sourceMaterialPropertyCache.GetValueIfAllSourceAreTheSameOrDefault("_EmissionColor", this.m_notGeneratingAtlasDefaultEmisionColor));
			}
		}

		public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
		{
			if (texPropertyName.name.Equals("_BumpMap"))
			{
				return TextureBlenderFallback.GetDefaultNormalMapColor();
			}
			if (texPropertyName.name.Equals("_BaseMap"))
			{
				return Color.white;
			}
			if (texPropertyName.name.Equals("_SpecGlossMap"))
			{
				if (mat != null && mat.HasProperty("_SpecColor"))
				{
					try
					{
						Color color = mat.GetColor("_SpecColor");
						this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_SpecColor", color);
					}
					catch (Exception)
					{
					}
				}
				return new Color(0f, 0f, 0f, 0.5f);
			}
			if (texPropertyName.name.Equals("_MetallicGlossMap"))
			{
				if (mat != null && mat.HasProperty("_Metallic"))
				{
					try
					{
						float @float = mat.GetFloat("_Metallic");
						Color color2 = new Color(@float, @float, @float);
						if (mat.HasProperty("_Smoothness"))
						{
							try
							{
								color2.a = mat.GetFloat("_Smoothness");
							}
							catch (Exception)
							{
							}
						}
						this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Metallic", @float);
						this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_Smoothness", color2.a);
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
				if (texPropertyName.name.Equals("_OcclusionMap"))
				{
					return new Color(1f, 1f, 1f, 1f);
				}
				if (texPropertyName.name.Equals("_EmissionMap"))
				{
					if (mat != null)
					{
						if (mat.IsKeywordEnabled("_EMISSION"))
						{
							if (mat.HasProperty("_EmissionColor"))
							{
								try
								{
									Color color3 = mat.GetColor("_EmissionColor");
									this.sourceMaterialPropertyCache.CacheMaterialProperty(mat, "_EmissionColor", color3);
									goto IL_232;
								}
								catch (Exception)
								{
									goto IL_232;
								}
							}
							return Color.black;
						}
						return Color.black;
					}
				}
				else if (texPropertyName.name.Equals("_DetailMask"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
				IL_232:
				return new Color(1f, 1f, 1f, 0f);
			}
		}

		private static Color NeutralNormalMap = new Color(0.5f, 0.5f, 1f);

		private TextureBlenderMaterialPropertyCacheHelper sourceMaterialPropertyCache = new TextureBlenderMaterialPropertyCacheHelper();

		private TextureBlenderURPLit.WorkflowMode m_workflowMode;

		private TextureBlenderURPLit.SmoothnessTextureChannel m_smoothnessTextureChannel;

		private Color m_tintColor;

		private bool m_doScaleAlphaCutoff;

		private float m_alphaCutoff;

		private float m_smoothness;

		private Color m_specColor;

		private bool m_hasSpecGlossMap;

		private float m_metallic;

		private bool m_hasMetallicGlossMap;

		private float m_bumpScale;

		private bool m_shaderDoesEmission;

		private Color m_emissionColor;

		private TextureBlenderURPLit.Prop propertyToDo = TextureBlenderURPLit.Prop.doNone;

		private Color m_generatingTintedAtlaColor = Color.white;

		private float m_generatingTintedAtlasMetallic;

		private Color m_generatingTintedAtlaSpecular = Color.black;

		private float m_generatingTintedAtlasMetallic_smoothness = 1f;

		private float m_generatingTintedAtlasSpecular_somoothness = 1f;

		private float m_generatingTintedAtlaBumpScale = 1f;

		private Color m_generatingTintedAtlaEmission = Color.white;

		private const float m_generatedAlphaCutoff = 0.5f;

		private Color m_notGeneratingAtlasDefaultColor = Color.white;

		private float m_notGeneratingAtlasDefaultMetallic;

		private float m_notGeneratingAtlasDefaultSmoothness_MetallicWorkflow;

		private float m_notGeneratingAtlasDefaultSmoothness_SpecularWorkflow = 1f;

		private Color m_notGeneratingAtlasDefaultSpecularColor = new Color(0.2f, 0.2f, 0.2f, 1f);

		private Color m_notGeneratingAtlasDefaultEmisionColor = Color.black;

		private enum Prop
		{
			doColor,
			doSpecular,
			doMetallic,
			doEmission,
			doBump,
			doNone
		}

		private enum WorkflowMode
		{
			unknown,
			metallic,
			specular
		}

		private enum SmoothnessTextureChannel
		{
			unknown,
			albedo,
			metallicSpecular
		}
	}
}
