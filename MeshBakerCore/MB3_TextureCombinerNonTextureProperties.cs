using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB3_TextureCombinerNonTextureProperties
	{
		public MB3_TextureCombinerNonTextureProperties(MB2_LogLevel ll, bool considerNonTextureProps)
		{
			this._considerNonTextureProperties = considerNonTextureProps;
			this.textureProperty2DefaultColorMap = new Dictionary<string, Color>();
			for (int i = 0; i < this.defaultTextureProperty2DefaultColorMap.Length; i++)
			{
				this.textureProperty2DefaultColorMap.Add(this.defaultTextureProperty2DefaultColorMap[i].name, this.defaultTextureProperty2DefaultColorMap[i].color);
				this._nonTexturePropertiesBlender = new MB3_TextureCombinerNonTextureProperties.NonTexturePropertiesDontBlendProps(this);
			}
		}

		internal void CollectAverageValuesOfNonTextureProperties(Material resultMaterial, Material mat)
		{
			for (int i = 0; i < this._nonTextureProperties.Length; i++)
			{
				MB3_TextureCombinerNonTextureProperties.MaterialProperty materialProperty = this._nonTextureProperties[i];
				if (resultMaterial.HasProperty(materialProperty.PropertyName))
				{
					materialProperty.GetAverageCalculator().TryGetPropValueFromMaterialAndBlendIntoAverage(mat, materialProperty);
				}
			}
		}

		internal void LoadTextureBlendersIfNeeded(Material resultMaterial)
		{
			if (this._considerNonTextureProperties)
			{
				this.LoadTextureBlenders();
				this.FindBestTextureBlender(resultMaterial);
			}
		}

		private static bool InterfaceFilter(Type typeObj, object criteriaObj)
		{
			return typeObj.ToString() == criteriaObj.ToString();
		}

		private void FindBestTextureBlender(Material resultMaterial)
		{
			this.resultMaterialTextureBlender = this.FindMatchingTextureBlender(resultMaterial.shader.name);
			if (this.resultMaterialTextureBlender != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					string str = "Using Consider Non-Texture Properties found a TextureBlender for result material. Using: ";
					TextureBlender textureBlender = this.resultMaterialTextureBlender;
					Debug.Log(str + ((textureBlender != null) ? textureBlender.ToString() : null));
				}
			}
			else
			{
				this.resultMaterialTextureBlender = new TextureBlenderFallback();
			}
			if (this.resultMaterialTextureBlender is TextureBlenderFallback && this.LOG_LEVEL >= MB2_LogLevel.error)
			{
				Debug.LogWarning("Using _considerNonTextureProperties could not find a TextureBlender that matches the shader on the result material (" + resultMaterial.shader.name + "). Using the Fallback Texture Blender.");
			}
			this._nonTexturePropertiesBlender = new MB3_TextureCombinerNonTextureProperties.NonTexturePropertiesBlendProps(this, this.resultMaterialTextureBlender);
		}

		private void LoadTextureBlenders()
		{
			string filterCriteria = "DigitalOpus.MB.Core.TextureBlender";
			TypeFilter filter = new TypeFilter(MB3_TextureCombinerNonTextureProperties.InterfaceFilter);
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				IEnumerable enumerable = null;
				try
				{
					enumerable = assembly.GetTypes();
				}
				catch (Exception ex)
				{
					ex.Equals(null);
				}
				if (enumerable != null)
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (type.FindInterfaces(filter, filterCriteria).Length != 0)
						{
							list.Add(type);
						}
					}
				}
			}
			TextureBlender textureBlender = null;
			List<TextureBlender> list2 = new List<TextureBlender>();
			foreach (Type type2 in list)
			{
				if (!type2.IsAbstract && !type2.IsInterface)
				{
					TextureBlender textureBlender2 = (TextureBlender)Activator.CreateInstance(type2);
					if (textureBlender2 is TextureBlenderFallback)
					{
						textureBlender = textureBlender2;
					}
					else
					{
						list2.Add(textureBlender2);
					}
				}
			}
			if (textureBlender != null)
			{
				list2.Add(textureBlender);
			}
			this.textureBlenders = list2.ToArray();
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Format("Loaded {0} TextureBlenders.", this.textureBlenders.Length));
			}
		}

		internal bool NonTexturePropertiesAreEqual(Material a, Material b)
		{
			return this._nonTexturePropertiesBlender.NonTexturePropertiesAreEqual(a, b);
		}

		internal Texture2D TintTextureWithTextureCombiner(Texture2D t, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName)
		{
			return this._nonTexturePropertiesBlender.TintTextureWithTextureCombiner(t, sourceMaterial, shaderPropertyName);
		}

		internal void AdjustNonTextureProperties(Material resultMat, List<ShaderTextureProperty> texPropertyNames, MB2_EditorMethodsInterface editorMethods)
		{
			if (resultMat == null || texPropertyNames == null)
			{
				return;
			}
			this._nonTexturePropertiesBlender.AdjustNonTextureProperties(resultMat, texPropertyNames, editorMethods);
		}

		internal Color GetColorAsItWouldAppearInAtlasIfNoTexture(Material matIfBlender, ShaderTextureProperty texProperty)
		{
			return this._nonTexturePropertiesBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(matIfBlender, texProperty);
		}

		internal Color GetColorForTemporaryTexture(Material matIfBlender, ShaderTextureProperty texProperty)
		{
			return this._nonTexturePropertiesBlender.GetColorForTemporaryTexture(matIfBlender, texProperty);
		}

		private TextureBlender FindMatchingTextureBlender(string shaderName)
		{
			for (int i = 0; i < this.textureBlenders.Length; i++)
			{
				if (this.textureBlenders[i].DoesShaderNameMatch(shaderName))
				{
					return this.textureBlenders[i];
				}
			}
			return null;
		}

		private MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair[] defaultTextureProperty2DefaultColorMap = new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair[]
		{
			new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair("_MainTex", new Color(1f, 1f, 1f, 0f)),
			new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair("_MetallicGlossMap", new Color(0f, 0f, 0f, 1f)),
			new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair("_ParallaxMap", new Color(0f, 0f, 0f, 0f)),
			new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair("_OcclusionMap", new Color(1f, 1f, 1f, 1f)),
			new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair("_EmissionMap", new Color(0f, 0f, 0f, 0f)),
			new MB3_TextureCombinerNonTextureProperties.TexPropertyNameColorPair("_DetailMask", new Color(0f, 0f, 0f, 0f))
		};

		private MB3_TextureCombinerNonTextureProperties.MaterialProperty[] _nonTextureProperties = new MB3_TextureCombinerNonTextureProperties.MaterialProperty[]
		{
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyColor("_Color", Color.white),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyFloat("_Glossiness", 0.5f),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyFloat("_GlossMapScale", 1f),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyFloat("_Metallic", 0f),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyFloat("_BumpScale", 0.1f),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyFloat("_Parallax", 0.02f),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyFloat("_OcclusionStrength", 1f),
			new MB3_TextureCombinerNonTextureProperties.MaterialPropertyColor("_EmissionColor", Color.black)
		};

		private MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		private bool _considerNonTextureProperties;

		private TextureBlender resultMaterialTextureBlender;

		private TextureBlender[] textureBlenders = new TextureBlender[0];

		private Dictionary<string, Color> textureProperty2DefaultColorMap = new Dictionary<string, Color>();

		private MB3_TextureCombinerNonTextureProperties.NonTextureProperties _nonTexturePropertiesBlender;

		public interface MaterialProperty
		{
			string PropertyName { get; set; }

			MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveraged GetAverageCalculator();

			object GetDefaultValue();
		}

		public class MaterialPropertyFloat : MB3_TextureCombinerNonTextureProperties.MaterialProperty
		{
			public string PropertyName { get; set; }

			public MaterialPropertyFloat(string name, float defValue)
			{
				this._averageCalc = new MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveragedFloat();
				this._defaultValue = defValue;
				this.PropertyName = name;
			}

			public MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveraged GetAverageCalculator()
			{
				return this._averageCalc;
			}

			public object GetDefaultValue()
			{
				return this._defaultValue;
			}

			private MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveragedFloat _averageCalc;

			private float _defaultValue;
		}

		public class MaterialPropertyColor : MB3_TextureCombinerNonTextureProperties.MaterialProperty
		{
			public string PropertyName { get; set; }

			public MaterialPropertyColor(string name, Color defaultVal)
			{
				this._averageCalc = new MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveragedColor();
				this._defaultValue = defaultVal;
				this.PropertyName = name;
			}

			public MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveraged GetAverageCalculator()
			{
				return this._averageCalc;
			}

			public object GetDefaultValue()
			{
				return this._defaultValue;
			}

			private MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveragedColor _averageCalc;

			private Color _defaultValue;
		}

		public interface MaterialPropertyValueAveraged
		{
			void TryGetPropValueFromMaterialAndBlendIntoAverage(Material mat, MB3_TextureCombinerNonTextureProperties.MaterialProperty property);

			object GetAverage();

			int NumValues();

			void SetAverageValueOrDefaultOnMaterial(Material mat, MB3_TextureCombinerNonTextureProperties.MaterialProperty property);
		}

		public class MaterialPropertyValueAveragedFloat : MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveraged
		{
			public void TryGetPropValueFromMaterialAndBlendIntoAverage(Material mat, MB3_TextureCombinerNonTextureProperties.MaterialProperty property)
			{
				if (mat.HasProperty(property.PropertyName))
				{
					float @float = mat.GetFloat(property.PropertyName);
					this.averageVal = this.averageVal * (float)this.numValues / (float)(this.numValues + 1) + @float / (float)(this.numValues + 1);
					this.numValues++;
				}
			}

			public object GetAverage()
			{
				return this.averageVal;
			}

			public int NumValues()
			{
				return this.numValues;
			}

			public void SetAverageValueOrDefaultOnMaterial(Material mat, MB3_TextureCombinerNonTextureProperties.MaterialProperty property)
			{
				if (mat.HasProperty(property.PropertyName))
				{
					if (this.numValues > 0)
					{
						mat.SetFloat(property.PropertyName, this.averageVal);
						return;
					}
					mat.SetFloat(property.PropertyName, (float)property.GetDefaultValue());
				}
			}

			public float averageVal;

			public int numValues;
		}

		public class MaterialPropertyValueAveragedColor : MB3_TextureCombinerNonTextureProperties.MaterialPropertyValueAveraged
		{
			public void TryGetPropValueFromMaterialAndBlendIntoAverage(Material mat, MB3_TextureCombinerNonTextureProperties.MaterialProperty property)
			{
				if (mat.HasProperty(property.PropertyName))
				{
					Color color = mat.GetColor(property.PropertyName);
					this.averageVal = this.averageVal * (float)this.numValues / (float)(this.numValues + 1) + color / (float)(this.numValues + 1);
					this.numValues++;
				}
			}

			public object GetAverage()
			{
				return this.averageVal;
			}

			public int NumValues()
			{
				return this.numValues;
			}

			public void SetAverageValueOrDefaultOnMaterial(Material mat, MB3_TextureCombinerNonTextureProperties.MaterialProperty property)
			{
				if (mat.HasProperty(property.PropertyName))
				{
					if (this.numValues > 0)
					{
						mat.SetColor(property.PropertyName, this.averageVal);
						return;
					}
					mat.SetColor(property.PropertyName, (Color)property.GetDefaultValue());
				}
			}

			public Color averageVal;

			public int numValues;
		}

		public struct TexPropertyNameColorPair
		{
			public TexPropertyNameColorPair(string nm, Color col)
			{
				this.name = nm;
				this.color = col;
			}

			public string name;

			public Color color;
		}

		private interface NonTextureProperties
		{
			bool NonTexturePropertiesAreEqual(Material a, Material b);

			Texture2D TintTextureWithTextureCombiner(Texture2D t, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName);

			void AdjustNonTextureProperties(Material resultMat, List<ShaderTextureProperty> texPropertyNames, MB2_EditorMethodsInterface editorMethods);

			Color GetColorForTemporaryTexture(Material matIfBlender, ShaderTextureProperty texProperty);

			Color GetColorAsItWouldAppearInAtlasIfNoTexture(Material matIfBlender, ShaderTextureProperty texProperty);
		}

		private class NonTexturePropertiesDontBlendProps : MB3_TextureCombinerNonTextureProperties.NonTextureProperties
		{
			public NonTexturePropertiesDontBlendProps(MB3_TextureCombinerNonTextureProperties textureProperties)
			{
				this._textureProperties = textureProperties;
			}

			public bool NonTexturePropertiesAreEqual(Material a, Material b)
			{
				return true;
			}

			public Texture2D TintTextureWithTextureCombiner(Texture2D t, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName)
			{
				Debug.LogError("TintTextureWithTextureCombiner should never be called if resultMaterialTextureBlender is null");
				return t;
			}

			public void AdjustNonTextureProperties(Material resultMat, List<ShaderTextureProperty> texPropertyNames, MB2_EditorMethodsInterface editorMethods)
			{
				if (resultMat == null || texPropertyNames == null)
				{
					return;
				}
				for (int i = 0; i < this._textureProperties._nonTextureProperties.Length; i++)
				{
					MB3_TextureCombinerNonTextureProperties.MaterialProperty materialProperty = this._textureProperties._nonTextureProperties[i];
					if (resultMat.HasProperty(materialProperty.PropertyName))
					{
						materialProperty.GetAverageCalculator().SetAverageValueOrDefaultOnMaterial(resultMat, materialProperty);
					}
				}
				if (editorMethods != null)
				{
					editorMethods.CommitChangesToAssets();
				}
			}

			public Color GetColorAsItWouldAppearInAtlasIfNoTexture(Material matIfBlender, ShaderTextureProperty texProperty)
			{
				return Color.white;
			}

			public Color GetColorForTemporaryTexture(Material matIfBlender, ShaderTextureProperty texProperty)
			{
				if (texProperty.isNormalMap)
				{
					if (MBVersion.IsSwizzledNormalMapPlatform())
					{
						return MB3_TextureCombiner.NEUTRAL_NORMAL_MAP_COLOR_SWIZZLED;
					}
					return MB3_TextureCombiner.NEUTRAL_NORMAL_MAP_COLOR_NON_SWIZZLED;
				}
				else
				{
					if (this._textureProperties.textureProperty2DefaultColorMap.ContainsKey(texProperty.name))
					{
						return this._textureProperties.textureProperty2DefaultColorMap[texProperty.name];
					}
					return new Color(1f, 1f, 1f, 0f);
				}
			}

			private MB3_TextureCombinerNonTextureProperties _textureProperties;
		}

		private class NonTexturePropertiesBlendProps : MB3_TextureCombinerNonTextureProperties.NonTextureProperties
		{
			public NonTexturePropertiesBlendProps(MB3_TextureCombinerNonTextureProperties textureProperties, TextureBlender resultMats)
			{
				this.resultMaterialTextureBlender = resultMats;
				this._textureProperties = textureProperties;
			}

			public bool NonTexturePropertiesAreEqual(Material a, Material b)
			{
				return this.resultMaterialTextureBlender.NonTexturePropertiesAreEqual(a, b);
			}

			public Texture2D TintTextureWithTextureCombiner(Texture2D t, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName)
			{
				this.resultMaterialTextureBlender.OnBeforeTintTexture(sourceMaterial.matsAndGOs.mats[0].mat, shaderPropertyName.name);
				if (this._textureProperties.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("Blending texture {0} mat {1} with non-texture properties using TextureBlender {2}", t.name, sourceMaterial.matsAndGOs.mats[0].mat, this.resultMaterialTextureBlender));
				}
				for (int i = 0; i < t.height; i++)
				{
					Color[] pixels = t.GetPixels(0, i, t.width, 1);
					for (int j = 0; j < pixels.Length; j++)
					{
						pixels[j] = this.resultMaterialTextureBlender.OnBlendTexturePixel(shaderPropertyName.name, pixels[j]);
					}
					t.SetPixels(0, i, t.width, 1, pixels);
				}
				t.Apply();
				return t;
			}

			public void AdjustNonTextureProperties(Material resultMat, List<ShaderTextureProperty> texPropertyNames, MB2_EditorMethodsInterface editorMethods)
			{
				if (resultMat == null || texPropertyNames == null)
				{
					return;
				}
				if (this._textureProperties.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("Adjusting non texture properties using TextureBlender for shader: " + resultMat.shader.name);
				}
				this.resultMaterialTextureBlender.SetNonTexturePropertyValuesOnResultMaterial(resultMat);
				if (editorMethods != null)
				{
					editorMethods.CommitChangesToAssets();
				}
			}

			public Color GetColorAsItWouldAppearInAtlasIfNoTexture(Material matIfBlender, ShaderTextureProperty texProperty)
			{
				this.resultMaterialTextureBlender.OnBeforeTintTexture(matIfBlender, texProperty.name);
				Color colorForTemporaryTexture = this.GetColorForTemporaryTexture(matIfBlender, texProperty);
				return this.resultMaterialTextureBlender.OnBlendTexturePixel(texProperty.name, colorForTemporaryTexture);
			}

			public Color GetColorForTemporaryTexture(Material matIfBlender, ShaderTextureProperty texProperty)
			{
				return this.resultMaterialTextureBlender.GetColorIfNoTexture(matIfBlender, texProperty);
			}

			private MB3_TextureCombinerNonTextureProperties _textureProperties;

			private TextureBlender resultMaterialTextureBlender;
		}
	}
}
