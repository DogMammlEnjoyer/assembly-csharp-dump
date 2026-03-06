using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules]
	internal static class ShorthandApplicator
	{
		public static void ApplyBackgroundPosition(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			BackgroundPosition backgroundPositionX;
			BackgroundPosition backgroundPositionY;
			ShorthandApplicator.CompileBackgroundPosition(reader, out backgroundPositionX, out backgroundPositionY);
			computedStyle.visualData.Write().backgroundPositionX = backgroundPositionX;
			computedStyle.visualData.Write().backgroundPositionY = backgroundPositionY;
		}

		public static void ApplyBorderColor(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Color borderTopColor;
			Color borderRightColor;
			Color borderBottomColor;
			Color borderLeftColor;
			ShorthandApplicator.CompileBoxArea(reader, out borderTopColor, out borderRightColor, out borderBottomColor, out borderLeftColor);
			computedStyle.visualData.Write().borderTopColor = borderTopColor;
			computedStyle.visualData.Write().borderRightColor = borderRightColor;
			computedStyle.visualData.Write().borderBottomColor = borderBottomColor;
			computedStyle.visualData.Write().borderLeftColor = borderLeftColor;
		}

		public static void ApplyBorderRadius(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Length borderTopLeftRadius;
			Length borderTopRightRadius;
			Length borderBottomRightRadius;
			Length borderBottomLeftRadius;
			ShorthandApplicator.CompileBorderRadius(reader, out borderTopLeftRadius, out borderTopRightRadius, out borderBottomRightRadius, out borderBottomLeftRadius);
			computedStyle.visualData.Write().borderTopLeftRadius = borderTopLeftRadius;
			computedStyle.visualData.Write().borderTopRightRadius = borderTopRightRadius;
			computedStyle.visualData.Write().borderBottomRightRadius = borderBottomRightRadius;
			computedStyle.visualData.Write().borderBottomLeftRadius = borderBottomLeftRadius;
		}

		public static void ApplyBorderWidth(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			float borderTopWidth;
			float borderRightWidth;
			float borderBottomWidth;
			float borderLeftWidth;
			ShorthandApplicator.CompileBoxArea(reader, out borderTopWidth, out borderRightWidth, out borderBottomWidth, out borderLeftWidth);
			computedStyle.layoutData.Write().borderTopWidth = borderTopWidth;
			computedStyle.layoutData.Write().borderRightWidth = borderRightWidth;
			computedStyle.layoutData.Write().borderBottomWidth = borderBottomWidth;
			computedStyle.layoutData.Write().borderLeftWidth = borderLeftWidth;
		}

		public static void ApplyFlex(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			float flexGrow;
			float flexShrink;
			Length flexBasis;
			ShorthandApplicator.CompileFlexShorthand(reader, out flexGrow, out flexShrink, out flexBasis);
			computedStyle.layoutData.Write().flexGrow = flexGrow;
			computedStyle.layoutData.Write().flexShrink = flexShrink;
			computedStyle.layoutData.Write().flexBasis = flexBasis;
		}

		public static void ApplyMargin(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Length marginTop;
			Length marginRight;
			Length marginBottom;
			Length marginLeft;
			ShorthandApplicator.CompileBoxArea(reader, out marginTop, out marginRight, out marginBottom, out marginLeft);
			computedStyle.layoutData.Write().marginTop = marginTop;
			computedStyle.layoutData.Write().marginRight = marginRight;
			computedStyle.layoutData.Write().marginBottom = marginBottom;
			computedStyle.layoutData.Write().marginLeft = marginLeft;
		}

		public static void ApplyPadding(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Length paddingTop;
			Length paddingRight;
			Length paddingBottom;
			Length paddingLeft;
			ShorthandApplicator.CompileBoxArea(reader, out paddingTop, out paddingRight, out paddingBottom, out paddingLeft);
			computedStyle.layoutData.Write().paddingTop = paddingTop;
			computedStyle.layoutData.Write().paddingRight = paddingRight;
			computedStyle.layoutData.Write().paddingBottom = paddingBottom;
			computedStyle.layoutData.Write().paddingLeft = paddingLeft;
		}

		public static void ApplyTransition(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			List<TimeValue> other;
			List<TimeValue> other2;
			List<StylePropertyName> other3;
			List<EasingFunction> other4;
			ShorthandApplicator.CompileTransition(reader, out other, out other2, out other3, out other4);
			computedStyle.transitionData.Write().transitionDelay.CopyFrom(other);
			computedStyle.transitionData.Write().transitionDuration.CopyFrom(other2);
			computedStyle.transitionData.Write().transitionProperty.CopyFrom(other3);
			computedStyle.transitionData.Write().transitionTimingFunction.CopyFrom(other4);
		}

		public static void ApplyUnityBackgroundScaleMode(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			BackgroundPosition backgroundPositionX;
			BackgroundPosition backgroundPositionY;
			BackgroundRepeat backgroundRepeat;
			BackgroundSize backgroundSize;
			ShorthandApplicator.CompileUnityBackgroundScaleMode(reader, out backgroundPositionX, out backgroundPositionY, out backgroundRepeat, out backgroundSize);
			computedStyle.visualData.Write().backgroundPositionX = backgroundPositionX;
			computedStyle.visualData.Write().backgroundPositionY = backgroundPositionY;
			computedStyle.visualData.Write().backgroundRepeat = backgroundRepeat;
			computedStyle.visualData.Write().backgroundSize = backgroundSize;
		}

		public static void ApplyUnityTextOutline(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Color unityTextOutlineColor;
			float unityTextOutlineWidth;
			ShorthandApplicator.CompileTextOutline(reader, out unityTextOutlineColor, out unityTextOutlineWidth);
			computedStyle.inheritedData.Write().unityTextOutlineColor = unityTextOutlineColor;
			computedStyle.inheritedData.Write().unityTextOutlineWidth = unityTextOutlineWidth;
		}

		private static bool CompileFlexShorthand(StylePropertyReader reader, out float grow, out float shrink, out Length basis)
		{
			grow = 0f;
			shrink = 1f;
			basis = Length.Auto();
			bool flag = false;
			int valueCount = reader.valueCount;
			bool flag2 = valueCount == 1 && reader.IsValueType(0, StyleValueType.Keyword);
			if (flag2)
			{
				bool flag3 = reader.IsKeyword(0, StyleValueKeyword.None);
				if (flag3)
				{
					flag = true;
					grow = 0f;
					shrink = 0f;
					basis = Length.Auto();
				}
				else
				{
					bool flag4 = reader.IsKeyword(0, StyleValueKeyword.Auto);
					if (flag4)
					{
						flag = true;
						grow = 1f;
						shrink = 1f;
						basis = Length.Auto();
					}
				}
			}
			else
			{
				bool flag5 = valueCount <= 3;
				if (flag5)
				{
					flag = true;
					grow = 0f;
					shrink = 1f;
					basis = Length.Percent(0f);
					bool flag6 = false;
					bool flag7 = false;
					int num = 0;
					while (num < valueCount && flag)
					{
						StyleValueType valueType = reader.GetValueType(num);
						bool flag8 = valueType == StyleValueType.Dimension || valueType == StyleValueType.Keyword;
						if (flag8)
						{
							bool flag9 = flag7;
							if (flag9)
							{
								flag = false;
								break;
							}
							flag7 = true;
							bool flag10 = valueType == StyleValueType.Keyword;
							if (flag10)
							{
								bool flag11 = reader.IsKeyword(num, StyleValueKeyword.Auto);
								if (flag11)
								{
									basis = Length.Auto();
								}
							}
							else
							{
								bool flag12 = valueType == StyleValueType.Dimension;
								if (flag12)
								{
									basis = reader.ReadLength(num);
								}
							}
							bool flag13 = flag6 && num != valueCount - 1;
							if (flag13)
							{
								flag = false;
							}
						}
						else
						{
							bool flag14 = valueType == StyleValueType.Float;
							if (flag14)
							{
								float num2 = reader.ReadFloat(num);
								bool flag15 = !flag6;
								if (flag15)
								{
									flag6 = true;
									grow = num2;
								}
								else
								{
									shrink = num2;
								}
							}
							else
							{
								flag = false;
							}
						}
						num++;
					}
				}
			}
			return flag;
		}

		private static void CompileBorderRadius(StylePropertyReader reader, out Length top, out Length right, out Length bottom, out Length left)
		{
			ShorthandApplicator.CompileBoxArea(reader, out top, out right, out bottom, out left);
			bool flag = top.IsAuto() || top.IsNone();
			if (flag)
			{
				top = 0f;
			}
			bool flag2 = right.IsAuto() || right.IsNone();
			if (flag2)
			{
				right = 0f;
			}
			bool flag3 = bottom.IsAuto() || bottom.IsNone();
			if (flag3)
			{
				bottom = 0f;
			}
			bool flag4 = left.IsAuto() || left.IsNone();
			if (flag4)
			{
				left = 0f;
			}
		}

		private static void CompileBackgroundPosition(StylePropertyReader reader, out BackgroundPosition backgroundPositionX, out BackgroundPosition backgroundPositionY)
		{
			int valueCount = reader.valueCount;
			StylePropertyValue value = reader.GetValue(0);
			StylePropertyValue stylePropertyValue = (valueCount > 1) ? reader.GetValue(1) : default(StylePropertyValue);
			StylePropertyValue stylePropertyValue2 = (valueCount > 2) ? reader.GetValue(2) : default(StylePropertyValue);
			StylePropertyValue stylePropertyValue3 = (valueCount > 3) ? reader.GetValue(3) : default(StylePropertyValue);
			backgroundPositionX = default(BackgroundPosition);
			backgroundPositionY = default(BackgroundPosition);
			bool flag = valueCount == 1;
			if (flag)
			{
				BackgroundPositionKeyword backgroundPositionKeyword = (BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0);
				bool flag2 = backgroundPositionKeyword == BackgroundPositionKeyword.Left;
				if (flag2)
				{
					backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Left);
					backgroundPositionY = BackgroundPosition.Initial();
				}
				else
				{
					bool flag3 = backgroundPositionKeyword == BackgroundPositionKeyword.Right;
					if (flag3)
					{
						backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Right);
						backgroundPositionY = BackgroundPosition.Initial();
					}
					else
					{
						bool flag4 = backgroundPositionKeyword == BackgroundPositionKeyword.Top;
						if (flag4)
						{
							backgroundPositionX = BackgroundPosition.Initial();
							backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top);
						}
						else
						{
							bool flag5 = backgroundPositionKeyword == BackgroundPositionKeyword.Bottom;
							if (flag5)
							{
								backgroundPositionX = BackgroundPosition.Initial();
								backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Bottom);
							}
							else
							{
								bool flag6 = backgroundPositionKeyword == BackgroundPositionKeyword.Center;
								if (flag6)
								{
									backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
									backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag7 = valueCount == 2;
				if (flag7)
				{
					bool flag8 = (value.handle.valueType == StyleValueType.Dimension || value.handle.valueType == StyleValueType.Float) && (value.handle.valueType == StyleValueType.Dimension || value.handle.valueType == StyleValueType.Float);
					if (flag8)
					{
						backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Left, value.sheet.ReadDimension(value.handle).ToLength());
						backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top, stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToLength());
					}
					else
					{
						bool flag9 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Enum;
						if (flag9)
						{
							BackgroundPositionKeyword backgroundPositionKeyword2 = (BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0);
							BackgroundPositionKeyword backgroundPositionKeyword3 = (BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 1);
							bool flag10 = backgroundPositionKeyword3 == BackgroundPositionKeyword.Left;
							if (flag10)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							bool flag11 = backgroundPositionKeyword3 == BackgroundPositionKeyword.Right;
							if (flag11)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							bool flag12 = backgroundPositionKeyword2 == BackgroundPositionKeyword.Top;
							if (flag12)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							bool flag13 = backgroundPositionKeyword2 == BackgroundPositionKeyword.Bottom;
							if (flag13)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							backgroundPositionX = new BackgroundPosition(backgroundPositionKeyword2);
							backgroundPositionY = new BackgroundPosition(backgroundPositionKeyword3);
						}
					}
				}
				else
				{
					bool flag14 = valueCount == 3;
					if (flag14)
					{
						bool flag15 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Enum && stylePropertyValue2.handle.valueType == StyleValueType.Dimension;
						if (flag15)
						{
							backgroundPositionX = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0));
							backgroundPositionY = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 1), reader.ReadLength(2));
						}
						else
						{
							bool flag16 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Dimension && stylePropertyValue2.handle.valueType == StyleValueType.Enum;
							if (flag16)
							{
								backgroundPositionX = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0), reader.ReadLength(1));
								backgroundPositionY = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 2));
							}
						}
					}
					else
					{
						bool flag17 = valueCount == 4;
						if (flag17)
						{
							bool flag18 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Dimension && stylePropertyValue2.handle.valueType == StyleValueType.Enum && stylePropertyValue3.handle.valueType == StyleValueType.Dimension;
							if (flag18)
							{
								backgroundPositionX = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0), reader.ReadLength(1));
								backgroundPositionY = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 2), reader.ReadLength(3));
							}
						}
					}
				}
			}
		}

		public static void CompileUnityBackgroundScaleMode(StylePropertyReader reader, out BackgroundPosition backgroundPositionX, out BackgroundPosition backgroundPositionY, out BackgroundRepeat backgroundRepeat, out BackgroundSize backgroundSize)
		{
			ScaleMode scaleMode = (ScaleMode)reader.ReadEnum(StyleEnumType.ScaleMode, 0);
			backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(scaleMode);
			backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(scaleMode);
			backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(scaleMode);
			backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(scaleMode);
		}

		private static void CompileBoxArea(StylePropertyReader reader, out Length top, out Length right, out Length bottom, out Length left)
		{
			top = 0f;
			right = 0f;
			bottom = 0f;
			left = 0f;
			switch (reader.valueCount)
			{
			case 0:
				break;
			case 1:
				top = (right = (bottom = (left = reader.ReadLength(0))));
				break;
			case 2:
				top = (bottom = reader.ReadLength(0));
				left = (right = reader.ReadLength(1));
				break;
			case 3:
				top = reader.ReadLength(0);
				left = (right = reader.ReadLength(1));
				bottom = reader.ReadLength(2);
				break;
			default:
				top = reader.ReadLength(0);
				right = reader.ReadLength(1);
				bottom = reader.ReadLength(2);
				left = reader.ReadLength(3);
				break;
			}
		}

		private static void CompileBoxArea(StylePropertyReader reader, out float top, out float right, out float bottom, out float left)
		{
			Length length;
			Length length2;
			Length length3;
			Length length4;
			ShorthandApplicator.CompileBoxArea(reader, out length, out length2, out length3, out length4);
			top = length.value;
			right = length2.value;
			bottom = length3.value;
			left = length4.value;
		}

		private static void CompileBoxArea(StylePropertyReader reader, out Color top, out Color right, out Color bottom, out Color left)
		{
			top = Color.clear;
			right = Color.clear;
			bottom = Color.clear;
			left = Color.clear;
			switch (reader.valueCount)
			{
			case 0:
				break;
			case 1:
				top = (right = (bottom = (left = reader.ReadColor(0))));
				break;
			case 2:
				top = (bottom = reader.ReadColor(0));
				left = (right = reader.ReadColor(1));
				break;
			case 3:
				top = reader.ReadColor(0);
				left = (right = reader.ReadColor(1));
				bottom = reader.ReadColor(2);
				break;
			default:
				top = reader.ReadColor(0);
				right = reader.ReadColor(1);
				bottom = reader.ReadColor(2);
				left = reader.ReadColor(3);
				break;
			}
		}

		private static void CompileTextOutline(StylePropertyReader reader, out Color outlineColor, out float outlineWidth)
		{
			outlineColor = Color.clear;
			outlineWidth = 0f;
			int valueCount = reader.valueCount;
			for (int i = 0; i < valueCount; i++)
			{
				StyleValueType valueType = reader.GetValueType(i);
				bool flag = valueType == StyleValueType.Dimension;
				if (flag)
				{
					outlineWidth = reader.ReadFloat(i);
				}
				else
				{
					bool flag2 = valueType == StyleValueType.Enum || valueType == StyleValueType.Color;
					if (flag2)
					{
						outlineColor = reader.ReadColor(i);
					}
				}
			}
		}

		private static void CompileTransition(StylePropertyReader reader, out List<TimeValue> outDelay, out List<TimeValue> outDuration, out List<StylePropertyName> outProperty, out List<EasingFunction> outTimingFunction)
		{
			ShorthandApplicator.s_TransitionDelayList.Clear();
			ShorthandApplicator.s_TransitionDurationList.Clear();
			ShorthandApplicator.s_TransitionPropertyList.Clear();
			ShorthandApplicator.s_TransitionTimingFunctionList.Clear();
			bool flag = true;
			bool flag2 = false;
			int valueCount = reader.valueCount;
			int num = 0;
			int num2 = 0;
			for (;;)
			{
				bool flag3 = flag2;
				if (flag3)
				{
					break;
				}
				StylePropertyName item = InitialStyle.transitionProperty[0];
				TimeValue item2 = InitialStyle.transitionDuration[0];
				TimeValue item3 = InitialStyle.transitionDelay[0];
				EasingFunction item4 = InitialStyle.transitionTimingFunction[0];
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				bool flag8 = false;
				while (num2 < valueCount && !flag8)
				{
					StyleValueType valueType = reader.GetValueType(num2);
					StyleValueType styleValueType = valueType;
					StyleValueType styleValueType2 = styleValueType;
					if (styleValueType2 <= StyleValueType.Dimension)
					{
						if (styleValueType2 != StyleValueType.Keyword)
						{
							if (styleValueType2 != StyleValueType.Dimension)
							{
								goto IL_1A3;
							}
							TimeValue timeValue = reader.ReadTimeValue(num2);
							bool flag9 = !flag4;
							if (flag9)
							{
								flag4 = true;
								item2 = timeValue;
							}
							else
							{
								bool flag10 = !flag5;
								if (flag10)
								{
									flag5 = true;
									item3 = timeValue;
								}
								else
								{
									flag = false;
								}
							}
						}
						else
						{
							bool flag11 = reader.IsKeyword(num2, StyleValueKeyword.None) && num == 0;
							if (flag11)
							{
								flag2 = true;
								flag6 = true;
								item = new StylePropertyName("none");
							}
							else
							{
								flag = false;
							}
						}
					}
					else if (styleValueType2 != StyleValueType.Enum)
					{
						if (styleValueType2 != StyleValueType.CommaSeparator)
						{
							goto IL_1A3;
						}
						flag8 = true;
						num++;
					}
					else
					{
						string text = reader.ReadAsString(num2);
						int easingMode;
						bool flag12 = !flag7 && StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.EasingMode, text, out easingMode);
						if (flag12)
						{
							flag7 = true;
							item4 = (EasingMode)easingMode;
						}
						else
						{
							bool flag13 = !flag6;
							if (flag13)
							{
								flag6 = true;
								item = new StylePropertyName(text);
							}
							else
							{
								flag = false;
							}
						}
					}
					IL_1A7:
					num2++;
					continue;
					IL_1A3:
					flag = false;
					goto IL_1A7;
				}
				ShorthandApplicator.s_TransitionDelayList.Add(item3);
				ShorthandApplicator.s_TransitionDurationList.Add(item2);
				ShorthandApplicator.s_TransitionPropertyList.Add(item);
				ShorthandApplicator.s_TransitionTimingFunctionList.Add(item4);
				if (num2 >= valueCount || !flag)
				{
					goto IL_209;
				}
			}
			flag = false;
			IL_209:
			bool flag14 = flag;
			if (flag14)
			{
				outProperty = ShorthandApplicator.s_TransitionPropertyList;
				outDelay = ShorthandApplicator.s_TransitionDelayList;
				outDuration = ShorthandApplicator.s_TransitionDurationList;
				outTimingFunction = ShorthandApplicator.s_TransitionTimingFunctionList;
			}
			else
			{
				outProperty = InitialStyle.transitionProperty;
				outDelay = InitialStyle.transitionDelay;
				outDuration = InitialStyle.transitionDuration;
				outTimingFunction = InitialStyle.transitionTimingFunction;
			}
		}

		[CompilerGenerated]
		internal static void <CompileBackgroundPosition>g__SwapKeyword|16_0(ref BackgroundPositionKeyword a, ref BackgroundPositionKeyword b)
		{
			BackgroundPositionKeyword backgroundPositionKeyword = a;
			a = b;
			b = backgroundPositionKeyword;
		}

		private static List<TimeValue> s_TransitionDelayList = new List<TimeValue>();

		private static List<TimeValue> s_TransitionDurationList = new List<TimeValue>();

		private static List<StylePropertyName> s_TransitionPropertyList = new List<StylePropertyName>();

		private static List<EasingFunction> s_TransitionTimingFunctionList = new List<EasingFunction>();
	}
}
