using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal class StylePropertyReader
	{
		private int m_CurrentValueIndex { get; set; }

		public StyleProperty property { get; private set; }

		public StylePropertyId propertyId { get; private set; }

		public int valueCount { get; private set; }

		public float dpiScaling { get; private set; }

		public void SetContext(StyleSheet sheet, StyleComplexSelector selector, StyleVariableContext varContext, float dpiScaling = 1f)
		{
			this.m_Sheet = sheet;
			this.m_Properties = selector.rule.properties;
			this.m_PropertyIds = StyleSheetCache.GetPropertyIds(sheet, selector.ruleIndex);
			this.m_Resolver.variableContext = varContext;
			this.dpiScaling = dpiScaling;
			this.LoadProperties();
		}

		public void SetInlineContext(StyleSheet sheet, StyleProperty[] properties, StylePropertyId[] propertyIds, float dpiScaling = 1f)
		{
			this.m_Sheet = sheet;
			this.m_Properties = properties;
			this.m_PropertyIds = propertyIds;
			this.dpiScaling = dpiScaling;
			this.LoadProperties();
		}

		public StylePropertyId MoveNextProperty()
		{
			this.m_CurrentPropertyIndex++;
			this.m_CurrentValueIndex += this.valueCount;
			this.SetCurrentProperty();
			return this.propertyId;
		}

		public StylePropertyValue GetValue(int index)
		{
			return this.m_Values[this.m_CurrentValueIndex + index];
		}

		public StyleValueType GetValueType(int index)
		{
			return this.m_Values[this.m_CurrentValueIndex + index].handle.valueType;
		}

		public bool IsValueType(int index, StyleValueType type)
		{
			return this.m_Values[this.m_CurrentValueIndex + index].handle.valueType == type;
		}

		public bool IsKeyword(int index, StyleValueKeyword keyword)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.handle.valueType == StyleValueType.Keyword && stylePropertyValue.handle.valueIndex == (int)keyword;
		}

		public string ReadAsString(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
		}

		public Length ReadLength(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			bool flag = stylePropertyValue.handle.valueType == StyleValueType.Keyword;
			Length result;
			if (flag)
			{
				StyleValueKeyword valueIndex = (StyleValueKeyword)stylePropertyValue.handle.valueIndex;
				StyleValueKeyword styleValueKeyword = valueIndex;
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				if (styleValueKeyword2 != StyleValueKeyword.Auto)
				{
					if (styleValueKeyword2 != StyleValueKeyword.None)
					{
						result = default(Length);
					}
					else
					{
						result = Length.None();
					}
				}
				else
				{
					result = Length.Auto();
				}
			}
			else
			{
				result = stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToLength();
			}
			return result;
		}

		public TimeValue ReadTimeValue(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToTime();
		}

		public Translate ReadTranslate(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue val3 = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			return StylePropertyReader.ReadTranslate(this.valueCount, val, val2, val3);
		}

		public TransformOrigin ReadTransformOrigin(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue zVvalue = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			return StylePropertyReader.ReadTransformOrigin(this.valueCount, val, val2, zVvalue);
		}

		public Rotate ReadRotate(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue val3 = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			StylePropertyValue val4 = (this.valueCount > 3) ? this.m_Values[this.m_CurrentValueIndex + index + 3] : default(StylePropertyValue);
			return StylePropertyReader.ReadRotate(this.valueCount, val, val2, val3, val4);
		}

		public Scale ReadScale(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue val3 = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			return StylePropertyReader.ReadScale(this.valueCount, val, val2, val3);
		}

		public float ReadFloat(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.sheet.ReadFloat(stylePropertyValue.handle);
		}

		public int ReadInt(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return (int)stylePropertyValue.sheet.ReadFloat(stylePropertyValue.handle);
		}

		public Color ReadColor(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.sheet.ReadColor(stylePropertyValue.handle);
		}

		public int ReadEnum(StyleEnumType enumType, int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			StyleValueHandle handle = stylePropertyValue.handle;
			bool flag = handle.valueType == StyleValueType.Keyword;
			string value;
			if (flag)
			{
				StyleValueKeyword svk = stylePropertyValue.sheet.ReadKeyword(handle);
				value = svk.ToUssString();
			}
			else
			{
				value = stylePropertyValue.sheet.ReadEnum(handle);
			}
			int result;
			StylePropertyUtil.TryGetEnumIntValue(enumType, value, out result);
			return result;
		}

		public Object ReadAsset(int index)
		{
			Object result = null;
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			StyleValueType valueType = stylePropertyValue.handle.valueType;
			StyleValueType styleValueType = valueType;
			if (styleValueType != StyleValueType.ResourcePath)
			{
				if (styleValueType == StyleValueType.AssetReference)
				{
					result = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle);
				}
			}
			else
			{
				string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
				bool flag = !string.IsNullOrEmpty(text);
				if (flag)
				{
					result = Panel.LoadResource(text, typeof(Object), this.dpiScaling);
				}
			}
			return result;
		}

		public FontDefinition ReadFontDefinition(int index)
		{
			FontAsset fontAsset = null;
			Font font = null;
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			StyleValueType valueType = stylePropertyValue.handle.valueType;
			StyleValueType styleValueType = valueType;
			if (styleValueType != StyleValueType.Keyword)
			{
				if (styleValueType != StyleValueType.ResourcePath)
				{
					if (styleValueType != StyleValueType.AssetReference)
					{
						Debug.LogWarning("Invalid value for font " + stylePropertyValue.handle.valueType.ToString());
					}
					else
					{
						font = (stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Font);
						bool flag = font == null;
						if (flag)
						{
							fontAsset = (stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as FontAsset);
						}
					}
				}
				else
				{
					string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
					bool flag2 = !string.IsNullOrEmpty(text);
					if (flag2)
					{
						font = (Panel.LoadResource(text, typeof(Font), this.dpiScaling) as Font);
						bool flag3 = font == null;
						if (flag3)
						{
							fontAsset = (Panel.LoadResource(text, typeof(FontAsset), this.dpiScaling) as FontAsset);
						}
					}
					bool flag4 = fontAsset == null && font == null;
					if (flag4)
					{
						Debug.LogWarning(string.Format(CultureInfo.InvariantCulture, "Font not found for path: {0}", text));
					}
				}
			}
			else
			{
				bool flag5 = stylePropertyValue.handle.valueIndex != 6;
				if (flag5)
				{
					string str = "Invalid keyword for font ";
					StyleValueKeyword valueIndex = (StyleValueKeyword)stylePropertyValue.handle.valueIndex;
					Debug.LogWarning(str + valueIndex.ToString());
				}
			}
			bool flag6 = font != null;
			FontDefinition result;
			if (flag6)
			{
				result = FontDefinition.FromFont(font);
			}
			else
			{
				bool flag7 = fontAsset != null;
				if (flag7)
				{
					result = FontDefinition.FromSDFFont(fontAsset);
				}
				else
				{
					result = default(FontDefinition);
				}
			}
			return result;
		}

		public Font ReadFont(int index)
		{
			Font font = null;
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			StyleValueType valueType = stylePropertyValue.handle.valueType;
			StyleValueType styleValueType = valueType;
			if (styleValueType != StyleValueType.Keyword)
			{
				if (styleValueType != StyleValueType.ResourcePath)
				{
					if (styleValueType != StyleValueType.AssetReference)
					{
						Debug.LogWarning("Invalid value for font " + stylePropertyValue.handle.valueType.ToString());
					}
					else
					{
						font = (stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Font);
					}
				}
				else
				{
					string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
					bool flag = !string.IsNullOrEmpty(text);
					if (flag)
					{
						font = (Panel.LoadResource(text, typeof(Font), this.dpiScaling) as Font);
					}
					bool flag2 = font == null;
					if (flag2)
					{
						Debug.LogWarning(string.Format(CultureInfo.InvariantCulture, "Font not found for path: {0}", text));
					}
				}
			}
			else
			{
				bool flag3 = stylePropertyValue.handle.valueIndex != 6;
				if (flag3)
				{
					string str = "Invalid keyword for font ";
					StyleValueKeyword valueIndex = (StyleValueKeyword)stylePropertyValue.handle.valueIndex;
					Debug.LogWarning(str + valueIndex.ToString());
				}
			}
			return font;
		}

		public Material ReadMaterial(int index)
		{
			Material material = null;
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			StyleValueType valueType = stylePropertyValue.handle.valueType;
			StyleValueType styleValueType = valueType;
			if (styleValueType != StyleValueType.Keyword)
			{
				if (styleValueType != StyleValueType.ResourcePath)
				{
					if (styleValueType != StyleValueType.AssetReference)
					{
						Debug.LogWarning("Invalid value for material " + stylePropertyValue.handle.valueType.ToString());
					}
					else
					{
						material = (stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Material);
					}
				}
				else
				{
					string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
					bool flag = !string.IsNullOrEmpty(text);
					if (flag)
					{
						material = (Panel.LoadResource(text, typeof(Material), this.dpiScaling) as Material);
					}
					bool flag2 = material == null;
					if (flag2)
					{
						Debug.LogWarning(string.Format(CultureInfo.InvariantCulture, "Material not found for path: {0}", text));
					}
				}
			}
			else
			{
				bool flag3 = stylePropertyValue.handle.valueIndex != 6;
				if (flag3)
				{
					string str = "Invalid keyword for material ";
					StyleValueKeyword valueIndex = (StyleValueKeyword)stylePropertyValue.handle.valueIndex;
					Debug.LogWarning(str + valueIndex.ToString());
				}
			}
			return material;
		}

		public Background ReadBackground(int index)
		{
			ImageSource imageSource = default(ImageSource);
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			bool flag = stylePropertyValue.handle.valueType == StyleValueType.Keyword;
			if (flag)
			{
				bool flag2 = stylePropertyValue.handle.valueIndex != 6;
				if (flag2)
				{
					string str = "Invalid keyword for image source ";
					StyleValueKeyword valueIndex = (StyleValueKeyword)stylePropertyValue.handle.valueIndex;
					Debug.LogWarning(str + valueIndex.ToString());
				}
			}
			else
			{
				bool flag3 = !StylePropertyReader.TryGetImageSourceFromValue(stylePropertyValue, this.dpiScaling, out imageSource);
				if (flag3)
				{
				}
			}
			bool flag4 = imageSource.texture != null;
			Background result;
			if (flag4)
			{
				result = Background.FromTexture2D(imageSource.texture);
			}
			else
			{
				bool flag5 = imageSource.sprite != null;
				if (flag5)
				{
					result = Background.FromSprite(imageSource.sprite);
				}
				else
				{
					bool flag6 = imageSource.vectorImage != null;
					if (flag6)
					{
						result = Background.FromVectorImage(imageSource.vectorImage);
					}
					else
					{
						bool flag7 = imageSource.renderTexture != null;
						if (flag7)
						{
							result = Background.FromRenderTexture(imageSource.renderTexture);
						}
						else
						{
							result = default(Background);
						}
					}
				}
			}
			return result;
		}

		public Cursor ReadCursor(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue val3 = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			return StylePropertyReader.ReadCursor(this.valueCount, val, val2, val3, this.dpiScaling);
		}

		public TextShadow ReadTextShadow(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue val3 = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			StylePropertyValue val4 = (this.valueCount > 3) ? this.m_Values[this.m_CurrentValueIndex + index + 3] : default(StylePropertyValue);
			return StylePropertyReader.ReadTextShadow(this.valueCount, val, val2, val3, val4);
		}

		public TextAutoSize ReadTextAutoSize(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			StylePropertyValue val3 = (this.valueCount > 2) ? this.m_Values[this.m_CurrentValueIndex + index + 2] : default(StylePropertyValue);
			return StylePropertyReader.ReadTextAutoSize(this.valueCount, val, val2, val3);
		}

		public BackgroundPosition ReadBackgroundPositionX(int index)
		{
			return this.ReadBackgroundPosition(index, BackgroundPositionKeyword.Left);
		}

		public BackgroundPosition ReadBackgroundPositionY(int index)
		{
			return this.ReadBackgroundPosition(index, BackgroundPositionKeyword.Top);
		}

		private BackgroundPosition ReadBackgroundPosition(int index, BackgroundPositionKeyword keyword)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			return StylePropertyReader.ReadBackgroundPosition(this.valueCount, val, val2, keyword);
		}

		public BackgroundRepeat ReadBackgroundRepeat(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			return StylePropertyReader.ReadBackgroundRepeat(this.valueCount, val, val2);
		}

		public BackgroundSize ReadBackgroundSize(int index)
		{
			StylePropertyValue val = this.m_Values[this.m_CurrentValueIndex + index];
			StylePropertyValue val2 = (this.valueCount > 1) ? this.m_Values[this.m_CurrentValueIndex + index + 1] : default(StylePropertyValue);
			return StylePropertyReader.ReadBackgroundSize(this.valueCount, val, val2);
		}

		public void ReadListEasingFunction(List<EasingFunction> list, int index)
		{
			list.Clear();
			do
			{
				StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
				StyleValueHandle handle = stylePropertyValue.handle;
				bool flag = handle.valueType == StyleValueType.Enum;
				if (flag)
				{
					string value = stylePropertyValue.sheet.ReadEnum(handle);
					int mode;
					StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.EasingMode, value, out mode);
					list.Add(new EasingFunction((EasingMode)mode));
					index++;
				}
				bool flag2 = index < this.valueCount;
				if (flag2)
				{
					bool flag3 = this.m_Values[this.m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator;
					if (flag3)
					{
						index++;
					}
				}
			}
			while (index < this.valueCount);
		}

		public void ReadListTimeValue(List<TimeValue> list, int index)
		{
			list.Clear();
			do
			{
				StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
				TimeValue item = stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToTime();
				list.Add(item);
				index++;
				bool flag = index < this.valueCount;
				if (flag)
				{
					bool flag2 = this.m_Values[this.m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator;
					if (flag2)
					{
						index++;
					}
				}
			}
			while (index < this.valueCount);
		}

		private FilterFunctionType ToFilterFunctionType(StyleValueFunction function)
		{
			FilterFunctionType result;
			switch (function)
			{
			case StyleValueFunction.CustomFilter:
				result = FilterFunctionType.Custom;
				break;
			case StyleValueFunction.FilterTint:
				result = FilterFunctionType.Tint;
				break;
			case StyleValueFunction.FilterOpacity:
				result = FilterFunctionType.Opacity;
				break;
			case StyleValueFunction.FilterInvert:
				result = FilterFunctionType.Invert;
				break;
			case StyleValueFunction.FilterGrayscale:
				result = FilterFunctionType.Grayscale;
				break;
			case StyleValueFunction.FilterSepia:
				result = FilterFunctionType.Sepia;
				break;
			case StyleValueFunction.FilterBlur:
				result = FilterFunctionType.Blur;
				break;
			default:
				result = FilterFunctionType.None;
				break;
			}
			return result;
		}

		public unsafe void ReadListFilterFunction(List<FilterFunction> list, int index)
		{
			list.Clear();
			do
			{
				StyleValueFunction valueIndex = (StyleValueFunction)this.GetValue(index++).handle.valueIndex;
				int num = this.ReadInt(index++);
				bool flag = false;
				FilterFunctionDefinition customDefinition = null;
				bool flag2 = valueIndex == StyleValueFunction.CustomFilter && num > 0;
				if (flag2)
				{
					flag = true;
					customDefinition = (this.ReadAsset(index++) as FilterFunctionDefinition);
					num--;
				}
				FixedBuffer4<FilterParameter> parameters = default(FixedBuffer4<FilterParameter>);
				int i = 0;
				while (i < num)
				{
					StyleValueType valueType = this.GetValueType(index);
					bool flag3 = valueType == StyleValueType.Color || valueType == StyleValueType.Enum;
					if (flag3)
					{
						Color colorValue = this.ReadColor(index++);
						*parameters[i] = new FilterParameter
						{
							type = FilterParameterType.Color,
							colorValue = colorValue
						};
					}
					else
					{
						bool flag4 = valueType == StyleValueType.Dimension || valueType == StyleValueType.Float;
						if (flag4)
						{
							StylePropertyValue value = this.GetValue(index++);
							Dimension dim = value.sheet.ReadDimension(value.handle);
							*parameters[i] = new FilterParameter
							{
								type = FilterParameterType.Float,
								floatValue = this.ConvertDimensionToFilterFloat(dim)
							};
						}
						else
						{
							bool flag5 = valueType == StyleValueType.CommaSeparator;
							if (!flag5)
							{
								Debug.LogError(string.Format("Unexpected value type {0} in filter function argument", valueType));
							}
						}
					}
					IL_15D:
					i++;
					continue;
					goto IL_15D;
				}
				bool flag6 = flag;
				if (flag6)
				{
					list.Add(new FilterFunction(customDefinition, parameters, num));
				}
				else
				{
					list.Add(new FilterFunction(this.ToFilterFunctionType(valueIndex), parameters, num));
				}
			}
			while (index < this.valueCount);
		}

		private float ConvertDimensionToFilterFloat(Dimension dim)
		{
			switch (dim.unit)
			{
			case Dimension.Unit.Percent:
				return dim.value * 0.01f;
			case Dimension.Unit.Millisecond:
				return dim.value * 0.001f;
			case Dimension.Unit.Degree:
				return dim.value * 0.017453292f;
			case Dimension.Unit.Gradian:
				return dim.value * 3.1415927f / 200f;
			case Dimension.Unit.Turn:
				return dim.value * 3.1415927f * 2f;
			}
			return dim.value;
		}

		public void ReadListStylePropertyName(List<StylePropertyName> list, int index)
		{
			list.Clear();
			do
			{
				StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
				bool flag = stylePropertyValue.handle.valueType == StyleValueType.Keyword;
				StylePropertyName item;
				if (flag)
				{
					StyleValueKeyword svk = stylePropertyValue.sheet.ReadKeyword(stylePropertyValue.handle);
					item = new StylePropertyName(svk.ToUssString());
				}
				else
				{
					item = stylePropertyValue.sheet.ReadStylePropertyName(stylePropertyValue.handle);
				}
				list.Add(item);
				index++;
				bool flag2 = index < this.valueCount;
				if (flag2)
				{
					bool flag3 = this.m_Values[this.m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator;
					if (flag3)
					{
						index++;
					}
				}
			}
			while (index < this.valueCount);
		}

		public void ReadListString(List<string> list, int index)
		{
			list.Clear();
			do
			{
				StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
				string item = stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
				list.Add(item);
				index++;
				bool flag = index < this.valueCount;
				if (flag)
				{
					bool flag2 = this.m_Values[this.m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator;
					if (flag2)
					{
						index++;
					}
				}
			}
			while (index < this.valueCount);
		}

		private void LoadProperties()
		{
			this.m_CurrentPropertyIndex = 0;
			this.m_CurrentValueIndex = 0;
			this.m_Values.Clear();
			this.m_ValueCount.Clear();
			foreach (StyleProperty styleProperty in this.m_Properties)
			{
				int num = 0;
				bool flag = true;
				bool requireVariableResolve = styleProperty.requireVariableResolve;
				if (requireVariableResolve)
				{
					this.m_Resolver.Init(styleProperty, this.m_Sheet, styleProperty.values);
					int num2 = 0;
					while (num2 < styleProperty.values.Length && flag)
					{
						StyleValueHandle handle = styleProperty.values[num2];
						bool flag2 = handle.IsVarFunction();
						if (flag2)
						{
							flag = this.m_Resolver.ResolveVarFunction(ref num2);
						}
						else
						{
							this.m_Resolver.AddValue(handle);
						}
						num2++;
					}
					bool flag3 = flag && this.m_Resolver.ValidateResolvedValues();
					if (flag3)
					{
						this.m_Values.AddRange(this.m_Resolver.resolvedValues);
						num += this.m_Resolver.resolvedValues.Count;
					}
					else
					{
						StyleValueHandle handle2 = new StyleValueHandle
						{
							valueType = StyleValueType.Keyword,
							valueIndex = 3
						};
						this.m_Values.Add(new StylePropertyValue
						{
							sheet = this.m_Sheet,
							handle = handle2
						});
						num++;
					}
				}
				else
				{
					num = styleProperty.values.Length;
					for (int j = 0; j < num; j++)
					{
						this.m_Values.Add(new StylePropertyValue
						{
							sheet = this.m_Sheet,
							handle = styleProperty.values[j]
						});
					}
				}
				this.m_ValueCount.Add(num);
			}
			this.SetCurrentProperty();
		}

		private void SetCurrentProperty()
		{
			bool flag = this.m_CurrentPropertyIndex < this.m_PropertyIds.Length;
			if (flag)
			{
				this.property = this.m_Properties[this.m_CurrentPropertyIndex];
				this.propertyId = this.m_PropertyIds[this.m_CurrentPropertyIndex];
				this.valueCount = this.m_ValueCount[this.m_CurrentPropertyIndex];
			}
			else
			{
				this.property = null;
				this.propertyId = StylePropertyId.Unknown;
				this.valueCount = 0;
			}
		}

		public static TransformOrigin ReadTransformOrigin(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue zVvalue)
		{
			Length x = Length.Percent(50f);
			Length y = Length.Percent(50f);
			float z = 0f;
			switch (valCount)
			{
			case 1:
			{
				bool flag;
				bool flag2;
				Length length = StylePropertyReader.ReadTransformOriginEnum(val1, out flag, out flag2);
				bool flag3 = flag2;
				if (flag3)
				{
					x = length;
				}
				else
				{
					y = length;
				}
				goto IL_F3;
			}
			case 2:
				break;
			case 3:
			{
				bool flag4 = zVvalue.handle.valueType == StyleValueType.Dimension || zVvalue.handle.valueType == StyleValueType.Float;
				if (flag4)
				{
					Dimension dimension = zVvalue.sheet.ReadDimension(zVvalue.handle);
					z = dimension.value;
				}
				break;
			}
			default:
				goto IL_F3;
			}
			bool flag5;
			bool flag6;
			Length length2 = StylePropertyReader.ReadTransformOriginEnum(val1, out flag5, out flag6);
			bool flag7;
			bool flag8;
			Length length3 = StylePropertyReader.ReadTransformOriginEnum(val2, out flag7, out flag8);
			bool flag9 = !flag6 || !flag7;
			if (flag9)
			{
				bool flag10 = flag8 && flag5;
				if (flag10)
				{
					x = length3;
					y = length2;
				}
			}
			else
			{
				x = length2;
				y = length3;
			}
			IL_F3:
			return new TransformOrigin(x, y, z);
		}

		private static Length ReadTransformOriginEnum(StylePropertyValue value, out bool isVertical, out bool isHorizontal)
		{
			bool flag = value.handle.valueType == StyleValueType.Enum;
			if (flag)
			{
				switch (StylePropertyReader.ReadEnum(StyleEnumType.TransformOriginOffset, value))
				{
				case 1:
					isVertical = false;
					isHorizontal = true;
					return Length.Percent(0f);
				case 2:
					isVertical = false;
					isHorizontal = true;
					return Length.Percent(100f);
				case 3:
					isVertical = true;
					isHorizontal = false;
					return Length.Percent(0f);
				case 4:
					isVertical = true;
					isHorizontal = false;
					return Length.Percent(100f);
				case 5:
					isVertical = true;
					isHorizontal = true;
					return Length.Percent(50f);
				}
			}
			else
			{
				bool flag2 = value.handle.valueType == StyleValueType.Dimension || value.handle.valueType == StyleValueType.Float;
				if (flag2)
				{
					isVertical = true;
					isHorizontal = true;
					return value.sheet.ReadDimension(value.handle).ToLength();
				}
			}
			isVertical = false;
			isHorizontal = false;
			return Length.Percent(50f);
		}

		public static Translate ReadTranslate(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3)
		{
			bool flag = val1.handle.valueType == StyleValueType.Keyword && val1.handle.valueIndex == 6;
			Translate result;
			if (flag)
			{
				result = Translate.None();
			}
			else
			{
				Length x = 0f;
				Length y = 0f;
				float z = 0f;
				switch (valCount)
				{
				case 1:
				{
					bool flag2 = val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float;
					if (flag2)
					{
						x = val1.sheet.ReadDimension(val1.handle).ToLength();
						y = val1.sheet.ReadDimension(val1.handle).ToLength();
					}
					goto IL_1A6;
				}
				case 2:
					break;
				case 3:
				{
					bool flag3 = val3.handle.valueType == StyleValueType.Dimension || val3.handle.valueType == StyleValueType.Float;
					if (flag3)
					{
						Dimension dimension = val3.sheet.ReadDimension(val3.handle);
						z = dimension.value;
					}
					break;
				}
				default:
					goto IL_1A6;
				}
				bool flag4 = val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float;
				if (flag4)
				{
					x = val1.sheet.ReadDimension(val1.handle).ToLength();
				}
				bool flag5 = val2.handle.valueType == StyleValueType.Dimension || val2.handle.valueType == StyleValueType.Float;
				if (flag5)
				{
					y = val2.sheet.ReadDimension(val2.handle).ToLength();
				}
				IL_1A6:
				result = new Translate(x, y, z);
			}
			return result;
		}

		public static Scale ReadScale(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3)
		{
			bool flag = val1.handle.valueType == StyleValueType.Keyword && val1.handle.valueIndex == 6;
			Scale result;
			if (flag)
			{
				result = Scale.None();
			}
			else
			{
				Vector3 one = Vector3.one;
				switch (valCount)
				{
				case 1:
				{
					bool flag2 = val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float;
					if (flag2)
					{
						one.x = val1.sheet.ReadFloat(val1.handle);
						one.y = one.x;
					}
					goto IL_173;
				}
				case 2:
					break;
				case 3:
				{
					bool flag3 = val3.handle.valueType == StyleValueType.Dimension || val3.handle.valueType == StyleValueType.Float;
					if (flag3)
					{
						one.z = val3.sheet.ReadFloat(val3.handle);
					}
					break;
				}
				default:
					goto IL_173;
				}
				bool flag4 = val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float;
				if (flag4)
				{
					one.x = val1.sheet.ReadFloat(val1.handle);
				}
				bool flag5 = val2.handle.valueType == StyleValueType.Dimension || val2.handle.valueType == StyleValueType.Float;
				if (flag5)
				{
					one.y = val2.sheet.ReadFloat(val2.handle);
				}
				IL_173:
				result = new Scale(one);
			}
			return result;
		}

		public static Rotate ReadRotate(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3, StylePropertyValue val4)
		{
			bool flag = val1.handle.valueType == StyleValueType.Keyword && val1.handle.valueIndex == 6;
			Rotate result;
			if (flag)
			{
				result = Rotate.None();
			}
			else
			{
				Rotate rotate = Rotate.Initial();
				switch (valCount)
				{
				case 1:
				{
					bool flag2 = val1.handle.valueType == StyleValueType.Dimension;
					if (flag2)
					{
						rotate.angle = StylePropertyReader.ReadAngle(val1);
					}
					break;
				}
				case 2:
					rotate.angle = StylePropertyReader.ReadAngle(val2);
					switch (StylePropertyReader.ReadEnum(StyleEnumType.Axis, val1))
					{
					case 0:
						rotate.axis = new Vector3(1f, 0f, 0f);
						break;
					case 1:
						rotate.axis = new Vector3(0f, 1f, 0f);
						break;
					case 2:
						rotate.axis = new Vector3(0f, 0f, 1f);
						break;
					}
					break;
				case 4:
					rotate.angle = StylePropertyReader.ReadAngle(val4);
					rotate.axis = new Vector3(val1.sheet.ReadFloat(val1.handle), val1.sheet.ReadFloat(val2.handle), val1.sheet.ReadFloat(val3.handle));
					break;
				}
				result = rotate;
			}
			return result;
		}

		private static bool TryReadEnum(StyleEnumType enumType, StylePropertyValue value, out int intValue)
		{
			StyleValueHandle handle = value.handle;
			bool flag = handle.valueType == StyleValueType.Keyword;
			string value2;
			if (flag)
			{
				StyleValueKeyword svk = value.sheet.ReadKeyword(handle);
				value2 = svk.ToUssString();
			}
			else
			{
				value2 = value.sheet.ReadEnum(handle);
			}
			return StylePropertyUtil.TryGetEnumIntValue(enumType, value2, out intValue);
		}

		private static int ReadEnum(StyleEnumType enumType, StylePropertyValue value)
		{
			StyleValueHandle handle = value.handle;
			bool flag = handle.valueType == StyleValueType.Keyword;
			string value2;
			if (flag)
			{
				StyleValueKeyword svk = value.sheet.ReadKeyword(handle);
				value2 = svk.ToUssString();
			}
			else
			{
				value2 = value.sheet.ReadEnum(handle);
			}
			int result;
			StylePropertyUtil.TryGetEnumIntValue(enumType, value2, out result);
			return result;
		}

		public static Angle ReadAngle(StylePropertyValue value)
		{
			bool flag = value.handle.valueType == StyleValueType.Keyword;
			Angle result;
			if (flag)
			{
				StyleValueKeyword valueIndex = (StyleValueKeyword)value.handle.valueIndex;
				StyleValueKeyword styleValueKeyword = valueIndex;
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				if (styleValueKeyword2 != StyleValueKeyword.None)
				{
					result = default(Angle);
				}
				else
				{
					result = Angle.None();
				}
			}
			else
			{
				result = value.sheet.ReadDimension(value.handle).ToAngle();
			}
			return result;
		}

		public static BackgroundPosition ReadBackgroundPosition(int valCount, StylePropertyValue val1, StylePropertyValue val2, BackgroundPositionKeyword keyword)
		{
			bool flag = valCount == 1;
			if (flag)
			{
				bool flag2 = val1.handle.valueType == StyleValueType.Enum;
				if (flag2)
				{
					return new BackgroundPosition((BackgroundPositionKeyword)StylePropertyReader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, val1));
				}
				bool flag3 = val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float;
				if (flag3)
				{
					return new BackgroundPosition(keyword, val1.sheet.ReadDimension(val1.handle).ToLength());
				}
			}
			else
			{
				bool flag4 = valCount == 2;
				if (flag4)
				{
					bool flag5 = val1.handle.valueType == StyleValueType.Enum && (val2.handle.valueType == StyleValueType.Dimension || val2.handle.valueType == StyleValueType.Float);
					if (flag5)
					{
						return new BackgroundPosition((BackgroundPositionKeyword)StylePropertyReader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, val1), val1.sheet.ReadDimension(val2.handle).ToLength());
					}
				}
			}
			return default(BackgroundPosition);
		}

		public static BackgroundRepeat ReadBackgroundRepeat(int valCount, StylePropertyValue val1, StylePropertyValue val2)
		{
			BackgroundRepeat backgroundRepeat = default(BackgroundRepeat);
			bool flag = valCount == 1;
			if (flag)
			{
				int num;
				bool flag2 = StylePropertyReader.TryReadEnum(StyleEnumType.RepeatXY, val1, out num);
				if (flag2)
				{
					bool flag3 = num == 0;
					if (flag3)
					{
						backgroundRepeat.x = Repeat.Repeat;
						backgroundRepeat.y = Repeat.NoRepeat;
					}
					else
					{
						bool flag4 = num == 1;
						if (flag4)
						{
							backgroundRepeat.x = Repeat.NoRepeat;
							backgroundRepeat.y = Repeat.Repeat;
						}
					}
				}
				else
				{
					backgroundRepeat.x = (Repeat)StylePropertyReader.ReadEnum(StyleEnumType.Repeat, val1);
					backgroundRepeat.y = backgroundRepeat.x;
				}
			}
			else
			{
				backgroundRepeat.x = (Repeat)StylePropertyReader.ReadEnum(StyleEnumType.Repeat, val1);
				backgroundRepeat.y = (Repeat)StylePropertyReader.ReadEnum(StyleEnumType.Repeat, val2);
			}
			return backgroundRepeat;
		}

		public static BackgroundSize ReadBackgroundSize(int valCount, StylePropertyValue val1, StylePropertyValue val2)
		{
			BackgroundSize result = default(BackgroundSize);
			bool flag = valCount == 1;
			if (flag)
			{
				bool flag2 = val1.handle.valueType == StyleValueType.Keyword;
				if (flag2)
				{
					bool flag3 = val1.handle.valueIndex == 2;
					if (flag3)
					{
						result.x = Length.Auto();
						result.y = Length.Auto();
					}
					else
					{
						bool flag4 = val1.handle.valueIndex == 7;
						if (flag4)
						{
							result.sizeType = BackgroundSizeType.Cover;
						}
						else
						{
							bool flag5 = val1.handle.valueIndex == 8;
							if (flag5)
							{
								result.sizeType = BackgroundSizeType.Contain;
							}
						}
					}
				}
				else
				{
					bool flag6 = val1.handle.valueType == StyleValueType.Enum;
					if (flag6)
					{
						result.sizeType = (BackgroundSizeType)StylePropertyReader.ReadEnum(StyleEnumType.BackgroundSizeType, val1);
					}
					else
					{
						bool flag7 = val1.handle.valueType == StyleValueType.Dimension;
						if (flag7)
						{
							result.x = val1.sheet.ReadDimension(val1.handle).ToLength();
							result.y = Length.Auto();
						}
					}
				}
			}
			else
			{
				bool flag8 = valCount == 2;
				if (flag8)
				{
					bool flag9 = val1.handle.valueType == StyleValueType.Keyword;
					if (flag9)
					{
						bool flag10 = val1.handle.valueIndex == 2;
						if (flag10)
						{
							result.x = Length.Auto();
						}
					}
					else
					{
						bool flag11 = val1.handle.valueType == StyleValueType.Dimension;
						if (flag11)
						{
							result.x = val1.sheet.ReadDimension(val1.handle).ToLength();
						}
					}
					bool flag12 = val2.handle.valueType == StyleValueType.Keyword;
					if (flag12)
					{
						bool flag13 = val2.handle.valueIndex == 2;
						if (flag13)
						{
							result.y = Length.Auto();
						}
					}
					else
					{
						bool flag14 = val2.handle.valueType == StyleValueType.Dimension;
						if (flag14)
						{
							result.y = val2.sheet.ReadDimension(val2.handle).ToLength();
						}
					}
				}
			}
			return result;
		}

		public static TextShadow ReadTextShadow(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3, StylePropertyValue val4)
		{
			bool flag = valCount < 2;
			TextShadow result;
			if (flag)
			{
				result = default(TextShadow);
			}
			else
			{
				StyleValueType valueType = val1.handle.valueType;
				bool flag2 = valueType == StyleValueType.Color || valueType == StyleValueType.Enum;
				TextShadow textShadow = new TextShadow
				{
					color = Color.clear,
					offset = Vector2.zero,
					blurRadius = 0f
				};
				if (valCount < 4)
				{
					if (valCount != 2)
					{
						if (valCount == 3)
						{
							StylePropertyValue stylePropertyValue = flag2 ? val1 : val3;
							StylePropertyValue stylePropertyValue2 = flag2 ? val2 : val1;
							StylePropertyValue stylePropertyValue3 = flag2 ? val3 : val2;
							Color color;
							bool flag3 = stylePropertyValue.sheet.TryReadColor(stylePropertyValue.handle, out color);
							if (flag3)
							{
								textShadow.color = color;
							}
							Vector2 offset = default(Vector2);
							Dimension dimension;
							bool flag4 = stylePropertyValue2.sheet.TryReadDimension(stylePropertyValue2.handle, out dimension);
							if (flag4)
							{
								offset.x = dimension.value;
							}
							Dimension dimension2;
							bool flag5 = stylePropertyValue3.sheet.TryReadDimension(stylePropertyValue3.handle, out dimension2);
							if (flag5)
							{
								offset.y = dimension2.value;
							}
							textShadow.offset = offset;
						}
					}
					else
					{
						Vector2 offset2 = textShadow.offset;
						Dimension dimension3;
						bool flag6 = val1.sheet.TryReadDimension(val1.handle, out dimension3);
						if (flag6)
						{
							offset2.x = dimension3.value;
						}
						Dimension dimension4;
						bool flag7 = val2.sheet.TryReadDimension(val2.handle, out dimension4);
						if (flag7)
						{
							offset2.y = dimension4.value;
						}
						textShadow.offset = offset2;
					}
				}
				else
				{
					StylePropertyValue stylePropertyValue4 = flag2 ? val1 : val4;
					StylePropertyValue stylePropertyValue5 = flag2 ? val2 : val1;
					StylePropertyValue stylePropertyValue6 = flag2 ? val3 : val2;
					StylePropertyValue stylePropertyValue7 = flag2 ? val4 : val3;
					Color color2;
					bool flag8 = stylePropertyValue4.sheet.TryReadColor(stylePropertyValue4.handle, out color2);
					if (flag8)
					{
						textShadow.color = color2;
					}
					Vector2 offset3 = default(Vector2);
					Dimension dimension5;
					bool flag9 = stylePropertyValue5.sheet.TryReadDimension(stylePropertyValue5.handle, out dimension5);
					if (flag9)
					{
						offset3.x = dimension5.value;
					}
					Dimension dimension6;
					bool flag10 = stylePropertyValue6.sheet.TryReadDimension(stylePropertyValue6.handle, out dimension6);
					if (flag10)
					{
						offset3.y = dimension6.value;
					}
					textShadow.offset = offset3;
					Dimension dimension7;
					bool flag11 = stylePropertyValue7.sheet.TryReadDimension(stylePropertyValue7.handle, out dimension7);
					if (flag11)
					{
						textShadow.blurRadius = dimension7.value;
					}
				}
				result = textShadow;
			}
			return result;
		}

		public static TextAutoSize ReadTextAutoSize(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3)
		{
			TextAutoSize result2;
			if (valCount > 2)
			{
				if (valCount == 3)
				{
					bool flag = val2.handle.valueType == StyleValueType.Keyword && val2.handle.valueIndex == 6;
					if (flag)
					{
						return TextAutoSize.None();
					}
					bool flag2 = val1.handle.valueType == StyleValueType.Enum;
					if (flag2)
					{
						TextAutoSizeMode textAutoSizeMode = (TextAutoSizeMode)StylePropertyReader.ReadEnum(StyleEnumType.TextAutoSizeMode, val1);
						bool flag3 = textAutoSizeMode == TextAutoSizeMode.None;
						if (flag3)
						{
							return TextAutoSize.None();
						}
						bool flag4 = textAutoSizeMode == TextAutoSizeMode.BestFit;
						if (flag4)
						{
							TextAutoSize result = default(TextAutoSize);
							result.mode = textAutoSizeMode;
							Dimension dimension;
							bool flag5 = val2.sheet.TryReadDimension(val2.handle, out dimension);
							if (flag5)
							{
								result.minSize = dimension.ToLength();
							}
							Dimension dimension2;
							bool flag6 = val3.sheet.TryReadDimension(val3.handle, out dimension2);
							if (flag6)
							{
								result.maxSize = dimension2.ToLength();
							}
							return result;
						}
					}
				}
				result2 = TextAutoSize.None();
			}
			else
			{
				result2 = TextAutoSize.None();
			}
			return result2;
		}

		internal static Cursor ReadCursor(int valueCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3, float dpiScaling = 1f)
		{
			Cursor result = default(Cursor);
			StyleValueType valueType = val1.handle.valueType;
			bool flag = valueType == StyleValueType.ResourcePath || valueType == StyleValueType.AssetReference || valueType == StyleValueType.ScalableImage || valueType == StyleValueType.MissingAssetReference;
			bool flag2 = flag;
			if (flag2)
			{
				ImageSource imageSource = default(ImageSource);
				bool flag3 = StylePropertyReader.TryGetImageSourceFromValue(val1, dpiScaling, out imageSource);
				if (flag3)
				{
					result.texture = imageSource.texture;
					bool flag4 = valueCount >= 3;
					if (flag4)
					{
						bool flag5 = val2.handle.valueType != StyleValueType.Float || val3.handle.valueType != StyleValueType.Float;
						if (flag5)
						{
							Debug.LogWarning("USS 'cursor' property requires two integers for the hot spot value.");
						}
						else
						{
							result.hotspot = new Vector2(val2.sheet.ReadFloat(val2.handle), val3.sheet.ReadFloat(val3.handle));
						}
					}
				}
			}
			else
			{
				bool flag6 = StylePropertyReader.getCursorIdFunc != null;
				if (flag6)
				{
					result.defaultCursorId = StylePropertyReader.getCursorIdFunc(val1.sheet, val1.handle);
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal static bool TryGetImageSourceFromValue(StylePropertyValue propertyValue, float dpiScaling, out ImageSource source)
		{
			source = default(ImageSource);
			StyleValueType valueType = propertyValue.handle.valueType;
			StyleValueType styleValueType = valueType;
			if (styleValueType <= StyleValueType.AssetReference)
			{
				if (styleValueType != StyleValueType.ResourcePath)
				{
					if (styleValueType == StyleValueType.AssetReference)
					{
						Object @object = propertyValue.sheet.ReadAssetReference(propertyValue.handle);
						source.texture = (@object as Texture2D);
						source.sprite = (@object as Sprite);
						source.vectorImage = (@object as VectorImage);
						source.renderTexture = (@object as RenderTexture);
						bool flag = source.IsNull();
						if (flag)
						{
							Debug.LogWarning("Invalid image specified");
							return false;
						}
						goto IL_254;
					}
				}
				else
				{
					string text = propertyValue.sheet.ReadResourcePath(propertyValue.handle);
					bool flag2 = !string.IsNullOrEmpty(text);
					if (flag2)
					{
						source.sprite = (Panel.LoadResource(text, typeof(Sprite), dpiScaling) as Sprite);
						bool flag3 = source.IsNull();
						if (flag3)
						{
							source.texture = (Panel.LoadResource(text, typeof(Texture2D), dpiScaling) as Texture2D);
						}
						bool flag4 = source.IsNull();
						if (flag4)
						{
							source.vectorImage = (Panel.LoadResource(text, typeof(VectorImage), dpiScaling) as VectorImage);
						}
						bool flag5 = source.IsNull();
						if (flag5)
						{
							source.renderTexture = (Panel.LoadResource(text, typeof(RenderTexture), dpiScaling) as RenderTexture);
						}
					}
					bool flag6 = source.IsNull();
					if (flag6)
					{
						Debug.LogWarning(string.Format("Image not found for path: {0}", text));
						return false;
					}
					goto IL_254;
				}
			}
			else if (styleValueType != StyleValueType.ScalableImage)
			{
				if (styleValueType == StyleValueType.MissingAssetReference)
				{
					return false;
				}
			}
			else
			{
				ScalableImage scalableImage = propertyValue.sheet.ReadScalableImage(propertyValue.handle);
				bool flag7 = scalableImage.normalImage == null && scalableImage.highResolutionImage == null;
				if (flag7)
				{
					Debug.LogWarning("Invalid scalable image specified");
					return false;
				}
				source.texture = scalableImage.normalImage;
				bool flag8 = !Mathf.Approximately(dpiScaling % 1f, 0f);
				if (flag8)
				{
					source.texture.filterMode = FilterMode.Bilinear;
				}
				goto IL_254;
			}
			Debug.LogWarning("Invalid value for image texture " + propertyValue.handle.valueType.ToString());
			return false;
			IL_254:
			return true;
		}

		internal static StylePropertyReader.GetCursorIdFunction getCursorIdFunc;

		private List<StylePropertyValue> m_Values = new List<StylePropertyValue>();

		private List<int> m_ValueCount = new List<int>();

		private StyleVariableResolver m_Resolver = new StyleVariableResolver();

		private StyleSheet m_Sheet;

		private StyleProperty[] m_Properties;

		private StylePropertyId[] m_PropertyIds;

		private int m_CurrentPropertyIndex;

		internal delegate int GetCursorIdFunction(StyleSheet sheet, StyleValueHandle handle);
	}
}
