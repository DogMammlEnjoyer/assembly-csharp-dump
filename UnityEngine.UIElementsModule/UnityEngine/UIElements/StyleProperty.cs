using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class StyleProperty
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			internal set
			{
				this.m_Name = value;
			}
		}

		public int line
		{
			get
			{
				return this.m_Line;
			}
			internal set
			{
				this.m_Line = value;
			}
		}

		public StyleValueHandle[] values
		{
			get
			{
				return this.m_Values;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			internal set
			{
				this.m_Values = value;
			}
		}

		internal int handleCount
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				StyleValueHandle[] values = this.m_Values;
				return (values != null) ? values.Length : 0;
			}
		}

		public bool ContainsVariable()
		{
			foreach (StyleValueHandle styleValueHandle in this.values)
			{
				bool flag = styleValueHandle.IsVarFunction();
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasValue()
		{
			return this.handleCount != 0;
		}

		public void ClearValue()
		{
			this.m_Values = Array.Empty<StyleValueHandle>();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void SetKeyword(StyleSheet styleSheet, StyleValueKeyword value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteKeyword(ref this.m_Values[0], value);
		}

		public bool TryGetKeyword(StyleSheet styleSheet, out StyleValueKeyword value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadKeyword(this.m_Values[0], out value);
			}
			else
			{
				value = StyleValueKeyword.Inherit;
				result = false;
			}
			return result;
		}

		public void SetFloat(StyleSheet styleSheet, float value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteFloat(ref this.m_Values[0], value);
		}

		public bool TryGetFloat(StyleSheet styleSheet, out float value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadFloat(this.m_Values[0], out value);
			}
			else
			{
				value = 0f;
				result = false;
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void SetDimension(StyleSheet styleSheet, Dimension value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteDimension(ref this.m_Values[0], value);
		}

		public bool TryGetDimension(StyleSheet styleSheet, out Dimension value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadDimension(this.m_Values[0], out value);
			}
			else
			{
				value = default(Dimension);
				result = false;
			}
			return result;
		}

		public void SetColor(StyleSheet styleSheet, Color value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteColor(ref this.m_Values[0], value);
		}

		public bool TryGetColor(StyleSheet styleSheet, out Color value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadColor(this.m_Values[0], out value);
			}
			else
			{
				value = default(Color);
				result = false;
			}
			return result;
		}

		public void SetString(StyleSheet styleSheet, string value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteString(ref this.values[0], value);
		}

		public bool TryGetString(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadString(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				result = false;
			}
			return result;
		}

		public void SetEnum(StyleSheet styleSheet, Enum value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteEnum<Enum>(ref this.m_Values[0], value);
		}

		public void SetEnum<TEnum>(StyleSheet styleSheet, TEnum value) where TEnum : struct, Enum
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteEnum<TEnum>(ref this.m_Values[0], value);
		}

		public bool TryGetEnumString(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadEnum(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				result = false;
			}
			return result;
		}

		public bool TryGetEnum<TEnum>(StyleSheet styleSheet, out TEnum value) where TEnum : struct, Enum
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadEnum<TEnum>(this.m_Values[0], out value);
			}
			else
			{
				value = default(TEnum);
				result = false;
			}
			return result;
		}

		public void SetVariableReference(StyleSheet styleSheet, string variableName)
		{
			StyleProperty.SetSize(ref this.m_Values, 3);
			styleSheet.WriteFunction(ref this.m_Values[0], StyleValueFunction.Var);
			styleSheet.WriteFloat(ref this.m_Values[1], 1f);
			styleSheet.WriteVariable(ref this.m_Values[2], variableName);
		}

		public bool TryGetVariableReference(StyleSheet styleSheet, out string variableName)
		{
			StyleValueFunction styleValueFunction;
			float num;
			bool flag = this.handleCount == 3 && styleSheet.TryReadFunction(this.m_Values[0], out styleValueFunction) && styleValueFunction == StyleValueFunction.Var && styleSheet.TryReadFloat(this.m_Values[1], out num) && (int)num == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadVariable(this.m_Values[2], out variableName);
			}
			else
			{
				variableName = null;
				result = false;
			}
			return result;
		}

		public void SetResourcePath(StyleSheet styleSheet, string value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteResourcePath(ref this.m_Values[0], value);
		}

		public bool TryGetResourcePath(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadResourcePath(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				result = false;
			}
			return result;
		}

		public void SetAssetReference(StyleSheet styleSheet, Object value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteAssetReference(ref this.m_Values[0], value);
		}

		public bool TryGetAssetReference(StyleSheet styleSheet, out Object value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadAssetReference(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				result = false;
			}
			return result;
		}

		public bool TryGetAssetReference<TObject>(StyleSheet styleSheet, out TObject value) where TObject : Object
		{
			Object @object;
			TObject tobject;
			bool flag;
			if (this.TryGetAssetReference(styleSheet, out @object))
			{
				tobject = (@object as TObject);
				flag = (tobject != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				value = tobject;
				result = true;
			}
			else
			{
				value = default(TObject);
				result = false;
			}
			return result;
		}

		public void SetMissingAssetReferenceUrl(StyleSheet styleSheet, string value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteMissingAssetReferenceUrl(ref this.m_Values[0], value);
		}

		public bool TryGetMissingAssetReferenceUrl(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadMissingAssetReferenceUrl(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				result = false;
			}
			return result;
		}

		public void SetScalableImage(StyleSheet styleSheet, ScalableImage value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteScalableImage(ref this.m_Values[0], value);
		}

		public bool TryGetScalableImage(StyleSheet styleSheet, out ScalableImage value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = styleSheet.TryReadScalableImage(this.m_Values[0], out value);
			}
			else
			{
				value = default(ScalableImage);
				result = false;
			}
			return result;
		}

		public void SetKeyword(StyleSheet styleSheet, StyleKeyword value)
		{
			this.SetKeyword(styleSheet, value.ToStyleValueKeyword());
		}

		public bool TryGetKeyword(StyleSheet styleSheet, out StyleKeyword value)
		{
			bool flag = this.handleCount == 1;
			bool result;
			if (flag)
			{
				result = StyleProperty.TryReadSetKeyword(styleSheet, ref this.m_Values[0], out value);
			}
			else
			{
				value = StyleKeyword.Undefined;
				result = false;
			}
			return result;
		}

		public void SetBackgroundRepeat(StyleSheet styleSheet, BackgroundRepeat value)
		{
			StyleProperty.SetSize(ref this.m_Values, 2);
			styleSheet.WriteEnum<Repeat>(ref this.values[0], value.x);
			styleSheet.WriteEnum<Repeat>(ref this.values[1], value.y);
		}

		public bool TryGetBackgroundRepeat(StyleSheet styleSheet, out BackgroundRepeat value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 2;
			bool result;
			if (flag)
			{
				value = default(BackgroundRepeat);
				result = false;
			}
			else
			{
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (this.handleCount > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadBackgroundRepeat(this.handleCount, val, val2);
				result = true;
			}
			return result;
		}

		public void SetBackgroundSize(StyleSheet styleSheet, BackgroundSize value)
		{
			switch (value.sizeType)
			{
			case BackgroundSizeType.Length:
				StyleProperty.SetSize(ref this.m_Values, 2);
				styleSheet.WriteLength(ref this.values[0], value.x);
				styleSheet.WriteLength(ref this.values[1], value.y);
				break;
			case BackgroundSizeType.Cover:
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.Cover);
				break;
			case BackgroundSizeType.Contain:
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.Contain);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public bool TryGetBackgroundSize(StyleSheet styleSheet, out BackgroundSize value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 2;
			bool result;
			if (flag)
			{
				value = default(BackgroundSize);
				result = false;
			}
			else
			{
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (this.handleCount > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadBackgroundSize(this.handleCount, val, val2);
				result = true;
			}
			return result;
		}

		public void SetBackgroundPosition(StyleSheet styleSheet, BackgroundPosition value)
		{
			bool flag = value.keyword == BackgroundPositionKeyword.Center;
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteEnum<BackgroundPositionKeyword>(ref this.values[0], value.keyword);
			}
			else
			{
				StyleProperty.SetSize(ref this.m_Values, 2);
				styleSheet.WriteEnum<BackgroundPositionKeyword>(ref this.values[0], value.keyword);
				styleSheet.WriteDimension(ref this.values[1], value.offset.ToDimension());
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool TryGetBackgroundPosition(StyleSheet styleSheet, out BackgroundPosition value, BackgroundPosition.Axis axis)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 2;
			bool result;
			if (flag)
			{
				value = default(BackgroundPosition);
				result = false;
			}
			else
			{
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (this.handleCount > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadBackgroundPosition(this.handleCount, val, val2, (axis == BackgroundPosition.Axis.Horizontal) ? BackgroundPositionKeyword.Left : BackgroundPositionKeyword.Top);
				result = true;
			}
			return result;
		}

		public void SetInt(StyleSheet styleSheet, int value)
		{
			this.SetFloat(styleSheet, (float)value);
		}

		public bool TryGetInt(StyleSheet styleSheet, out int value)
		{
			float num;
			bool flag = this.TryGetFloat(styleSheet, out num);
			bool result;
			if (flag)
			{
				value = (int)num;
				result = true;
			}
			else
			{
				value = 0;
				result = false;
			}
			return result;
		}

		public void SetLength(StyleSheet styleSheet, Length value)
		{
			bool flag = value.IsAuto();
			if (flag)
			{
				this.SetKeyword(styleSheet, StyleValueKeyword.Auto);
			}
			else
			{
				bool flag2 = value.IsNone();
				if (flag2)
				{
					this.SetKeyword(styleSheet, StyleValueKeyword.None);
				}
				else
				{
					this.SetDimension(styleSheet, value.ToDimension());
				}
			}
		}

		public bool TryGetLength(StyleSheet styleSheet, out Length value)
		{
			bool flag = this.handleCount != 1;
			bool result;
			if (flag)
			{
				value = default(Length);
				result = false;
			}
			else
			{
				StyleValueKeyword styleValueKeyword;
				bool flag2 = styleSheet.TryReadKeyword(this.m_Values[0], out styleValueKeyword);
				if (flag2)
				{
					StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
					StyleValueKeyword styleValueKeyword3 = styleValueKeyword2;
					if (styleValueKeyword3 != StyleValueKeyword.Initial)
					{
						if (styleValueKeyword3 != StyleValueKeyword.Auto)
						{
							if (styleValueKeyword3 != StyleValueKeyword.None)
							{
								value = default(Length);
								result = false;
							}
							else
							{
								value = Length.None();
								result = true;
							}
						}
						else
						{
							value = Length.Auto();
							result = true;
						}
					}
					else
					{
						value = default(Length);
						result = true;
					}
				}
				else
				{
					Dimension dimension;
					bool flag3 = styleSheet.TryReadDimension(this.m_Values[0], out dimension) && dimension.IsLength();
					if (flag3)
					{
						value = dimension.ToLength();
						result = true;
					}
					else
					{
						value = default(Length);
						result = false;
					}
				}
			}
			return result;
		}

		public void SetTranslate(StyleSheet styleSheet, Translate value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.m_Values[0], StyleValueKeyword.None);
			}
			else
			{
				bool flag2 = value.z == 0f;
				if (flag2)
				{
					StyleProperty.SetSize(ref this.m_Values, 2);
					styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
					styleSheet.WriteDimension(ref this.m_Values[1], value.y.ToDimension());
				}
				else
				{
					StyleProperty.SetSize(ref this.m_Values, 3);
					styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
					styleSheet.WriteDimension(ref this.m_Values[1], value.y.ToDimension());
					styleSheet.WriteDimension(ref this.m_Values[2], new Length(value.z).ToDimension());
				}
			}
		}

		public bool TryGetTranslate(StyleSheet styleSheet, out Translate value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool result;
			if (flag)
			{
				value = default(Translate);
				result = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val3 = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadTranslate(handleCount2, val, val2, val3);
				result = true;
			}
			return result;
		}

		public void SetRotate(StyleSheet styleSheet, Rotate value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.None);
			}
			else
			{
				bool flag2 = value.axis == Vector3.forward;
				if (flag2)
				{
					StyleProperty.SetSize(ref this.m_Values, 1);
					styleSheet.WriteAngle(ref this.values[0], value.angle);
				}
				else
				{
					StyleProperty.SetSize(ref this.m_Values, 4);
					Vector3 axis = value.axis;
					styleSheet.WriteFloat(ref this.values[0], axis.x);
					styleSheet.WriteFloat(ref this.values[1], axis.y);
					styleSheet.WriteFloat(ref this.values[2], axis.z);
					styleSheet.WriteAngle(ref this.values[3], value.angle);
				}
			}
		}

		public bool TryGetRotate(StyleSheet styleSheet, out Rotate value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 4;
			bool result;
			if (flag)
			{
				value = default(Rotate);
				result = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val3 = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val4 = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[3],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadRotate(handleCount2, val, val2, val3, val4);
				result = true;
			}
			return result;
		}

		public void SetScale(StyleSheet styleSheet, Scale value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.None);
			}
			else
			{
				bool flag2 = Mathf.Approximately(value.value.z, 1f);
				if (flag2)
				{
					StyleProperty.SetSize(ref this.m_Values, 2);
					styleSheet.WriteFloat(ref this.values[0], value.value.x);
					styleSheet.WriteFloat(ref this.values[1], value.value.y);
				}
				else
				{
					StyleProperty.SetSize(ref this.m_Values, 3);
					styleSheet.WriteFloat(ref this.values[0], value.value.x);
					styleSheet.WriteFloat(ref this.values[1], value.value.y);
					styleSheet.WriteFloat(ref this.values[2], value.value.z);
				}
			}
		}

		public bool TryGetScale(StyleSheet styleSheet, out Scale value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool result;
			if (flag)
			{
				value = default(Scale);
				result = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val3 = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadScale(handleCount2, val, val2, val3);
				result = true;
			}
			return result;
		}

		public void SetTextShadow(StyleSheet styleSheet, TextShadow value)
		{
			StyleProperty.SetSize(ref this.m_Values, 4);
			styleSheet.WriteDimension(ref this.values[0], new Dimension
			{
				value = value.offset.x,
				unit = Dimension.Unit.Pixel
			});
			styleSheet.WriteDimension(ref this.values[1], new Dimension
			{
				value = value.offset.y,
				unit = Dimension.Unit.Pixel
			});
			styleSheet.WriteDimension(ref this.values[2], new Dimension
			{
				value = value.blurRadius,
				unit = Dimension.Unit.Pixel
			});
			styleSheet.WriteColor(ref this.values[3], value.color);
		}

		public bool TryGetTextShadow(StyleSheet styleSheet, out TextShadow value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 4;
			bool result;
			if (flag)
			{
				value = default(TextShadow);
				result = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val3 = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val4 = (handleCount2 > 3) ? new StylePropertyValue
				{
					handle = this.values[3],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadTextShadow(handleCount2, val, val2, val3, val4);
				result = true;
			}
			return result;
		}

		public void SetTextAutoSize(StyleSheet styleSheet, TextAutoSize value)
		{
			bool flag = value.mode == TextAutoSizeMode.None;
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteEnum<TextAutoSizeMode>(ref this.m_Values[0], value.mode);
			}
			else
			{
				StyleProperty.SetSize(ref this.m_Values, 3);
				styleSheet.WriteEnum<TextAutoSizeMode>(ref this.m_Values[0], value.mode);
				styleSheet.WriteDimension(ref this.values[1], new Dimension
				{
					value = value.minSize.value,
					unit = Dimension.Unit.Pixel
				});
				styleSheet.WriteDimension(ref this.values[2], new Dimension
				{
					value = value.maxSize.value,
					unit = Dimension.Unit.Pixel
				});
			}
		}

		public bool TryGetTextAutoSize(StyleSheet styleSheet, out TextAutoSize value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool result;
			if (flag)
			{
				value = TextAutoSize.None();
				result = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue val3 = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadTextAutoSize(handleCount2, val, val2, val3);
				result = true;
			}
			return result;
		}

		public void SetTransformOrigin(StyleSheet styleSheet, TransformOrigin value)
		{
			TransformOriginOffset? transformOriginOffset = StyleProperty.GetTransformOriginOffset(value.x, true);
			TransformOriginOffset? transformOriginOffset2 = StyleProperty.GetTransformOriginOffset(value.y, false);
			bool flag = value.z != 0f;
			bool flag2 = !flag;
			if (flag2)
			{
				bool flag3 = transformOriginOffset2 == TransformOriginOffset.Center;
				if (flag3)
				{
					StyleProperty.SetSize(ref this.m_Values, 1);
					bool flag4 = transformOriginOffset != null;
					if (flag4)
					{
						styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[0], transformOriginOffset.Value);
					}
					else
					{
						styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
					}
					return;
				}
				bool flag5 = transformOriginOffset != null && transformOriginOffset2 != null && transformOriginOffset.Value == TransformOriginOffset.Center;
				if (flag5)
				{
					StyleProperty.SetSize(ref this.m_Values, 1);
					styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[0], transformOriginOffset2.Value);
					return;
				}
			}
			StyleProperty.SetSize(ref this.m_Values, 2 + (flag ? 1 : 0));
			bool flag6 = transformOriginOffset != null;
			if (flag6)
			{
				styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[0], transformOriginOffset.Value);
			}
			else
			{
				styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
			}
			bool flag7 = transformOriginOffset2 != null;
			if (flag7)
			{
				styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[1], transformOriginOffset2.Value);
			}
			else
			{
				styleSheet.WriteDimension(ref this.m_Values[1], value.y.ToDimension());
			}
			bool flag8 = flag;
			if (flag8)
			{
				styleSheet.WriteDimension(ref this.m_Values[2], new Dimension(value.z, Dimension.Unit.Pixel));
			}
		}

		public bool TryGetTransformOrigin(StyleSheet styleSheet, out TransformOrigin value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool result;
			if (flag)
			{
				value = default(TransformOrigin);
				result = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue val = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue val2 = (handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue);
				StylePropertyValue zVvalue = (handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue);
				value = StylePropertyReader.ReadTransformOrigin(handleCount2, val, val2, zVvalue);
				result = true;
			}
			return result;
		}

		public void SetTimeValue(StyleSheet styleSheet, List<TimeValue> value)
		{
			StyleProperty.SetSize(ref this.m_Values, value.Count * 2 - 1);
			for (int i = 0; i < value.Count; i++)
			{
				int num = i * 2;
				styleSheet.WriteDimension(ref this.values[num], value[i].ToDimension());
				bool flag = i < value.Count - 1;
				if (flag)
				{
					styleSheet.WriteCommaSeparator(ref this.values[num + 1]);
				}
			}
		}

		public bool TryGetTimeValue(StyleSheet styleSheet, out List<TimeValue> value)
		{
			bool flag = this.ContainsVariable();
			bool result;
			if (flag)
			{
				value = null;
				result = false;
			}
			else
			{
				value = new List<TimeValue>();
				result = this.TryGetTimeValue(styleSheet, value);
			}
			return result;
		}

		public bool TryGetTimeValue(StyleSheet styleSheet, List<TimeValue> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.m_Values.Length; i += 2)
				{
					int num = i + 1;
					TimeValue item;
					bool flag3 = !styleSheet.TryReadTimeValue(this.m_Values[i], out item) || (num < this.m_Values.Length && this.values[num].valueType != StyleValueType.CommaSeparator);
					if (flag3)
					{
						value.Clear();
						return false;
					}
					value.Add(item);
				}
				result = true;
			}
			return result;
		}

		public void SetStylePropertyName(StyleSheet styleSheet, List<StylePropertyName> value)
		{
			StyleProperty.SetSize(ref this.m_Values, value.Count * 2 - 1);
			for (int i = 0; i < value.Count; i++)
			{
				int num = i * 2;
				styleSheet.WriteStylePropertyName(ref this.values[num], value[i]);
				bool flag = i < value.Count - 1;
				if (flag)
				{
					styleSheet.WriteCommaSeparator(ref this.values[num + 1]);
				}
			}
		}

		public bool TryGetStylePropertyName(StyleSheet styleSheet, out List<StylePropertyName> value)
		{
			bool flag = this.ContainsVariable();
			bool result;
			if (flag)
			{
				value = null;
				result = false;
			}
			else
			{
				value = new List<StylePropertyName>();
				result = this.TryGetStylePropertyName(styleSheet, value);
			}
			return result;
		}

		public bool TryGetStylePropertyName(StyleSheet styleSheet, List<StylePropertyName> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.m_Values.Length; i += 2)
				{
					int num = i + 1;
					StylePropertyName item;
					bool flag3 = !styleSheet.TryReadStylePropertyName(this.m_Values[i], out item) || (num < this.m_Values.Length && this.values[num].valueType != StyleValueType.CommaSeparator);
					if (flag3)
					{
						value.Clear();
						return false;
					}
					value.Add(item);
				}
				result = true;
			}
			return result;
		}

		public void SetEasingFunction(StyleSheet styleSheet, List<EasingFunction> value)
		{
			StyleProperty.SetSize(ref this.m_Values, value.Count * 2 - 1);
			for (int i = 0; i < value.Count; i++)
			{
				int num = i * 2;
				styleSheet.WriteEnum<EasingMode>(ref this.values[num], value[i].mode);
				bool flag = i < value.Count - 1;
				if (flag)
				{
					styleSheet.WriteCommaSeparator(ref this.values[num + 1]);
				}
			}
		}

		public bool TryGetEasingFunction(StyleSheet styleSheet, out List<EasingFunction> value)
		{
			bool flag = this.ContainsVariable();
			bool result;
			if (flag)
			{
				value = null;
				result = false;
			}
			else
			{
				value = new List<EasingFunction>();
				result = this.TryGetEasingFunction(styleSheet, value);
			}
			return result;
		}

		public bool TryGetEasingFunction(StyleSheet styleSheet, List<EasingFunction> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.m_Values.Length; i += 2)
				{
					int num = i + 1;
					EasingMode mode;
					bool flag3 = !styleSheet.TryReadEnum<EasingMode>(this.m_Values[i], out mode) || (num < this.m_Values.Length && this.values[num].valueType != StyleValueType.CommaSeparator);
					if (flag3)
					{
						value.Clear();
						return false;
					}
					value.Add(new EasingFunction(mode));
				}
				result = true;
			}
			return result;
		}

		private static void SetSize(ref StyleValueHandle[] store, int size)
		{
			StyleValueHandle[] array = store;
			bool flag = array != null && array.Length == size;
			if (!flag)
			{
				store = new StyleValueHandle[size];
			}
		}

		internal static bool TryReadSetKeyword(StyleSheet styleSheet, ref StyleValueHandle handle, out StyleKeyword value)
		{
			bool flag = handle.valueType == StyleValueType.Keyword;
			if (flag)
			{
				StyleValueKeyword valueIndex = (StyleValueKeyword)handle.valueIndex;
				StyleValueKeyword styleValueKeyword = valueIndex;
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				if (styleValueKeyword2 == StyleValueKeyword.Initial)
				{
					value = StyleKeyword.Initial;
					return true;
				}
				if (styleValueKeyword2 == StyleValueKeyword.Auto)
				{
					value = StyleKeyword.Auto;
					return true;
				}
				if (styleValueKeyword2 == StyleValueKeyword.None)
				{
					value = StyleKeyword.None;
					return true;
				}
			}
			value = StyleKeyword.Undefined;
			return false;
		}

		private static TransformOriginOffset? GetTransformOriginOffset(Length dim, bool horizontal)
		{
			TransformOriginOffset? result = null;
			bool flag = Mathf.Approximately(dim.value, 0f);
			if (flag)
			{
				result = new TransformOriginOffset?(horizontal ? TransformOriginOffset.Left : TransformOriginOffset.Top);
			}
			else
			{
				bool flag2 = dim.unit == LengthUnit.Percent;
				if (flag2)
				{
					bool flag3 = Mathf.Approximately(dim.value, 50f);
					if (flag3)
					{
						result = new TransformOriginOffset?(TransformOriginOffset.Center);
					}
					else
					{
						bool flag4 = Mathf.Approximately(dim.value, 100f);
						if (flag4)
						{
							result = new TransformOriginOffset?(horizontal ? TransformOriginOffset.Right : TransformOriginOffset.Bottom);
						}
					}
				}
			}
			return result;
		}

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private int m_Line;

		[SerializeField]
		private StyleValueHandle[] m_Values = Array.Empty<StyleValueHandle>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[NonSerialized]
		internal bool isCustomProperty;

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[NonSerialized]
		internal bool requireVariableResolve;
	}
}
