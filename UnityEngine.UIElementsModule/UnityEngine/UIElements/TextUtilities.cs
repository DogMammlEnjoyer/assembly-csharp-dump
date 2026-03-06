using System;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal static class TextUtilities
	{
		public static TextSettings textSettings
		{
			get
			{
				bool flag = TextUtilities.s_TextSettings == null;
				if (flag)
				{
					TextUtilities.s_TextSettings = TextUtilities.getEditorTextSettings();
				}
				return TextUtilities.s_TextSettings;
			}
		}

		internal static Vector2 MeasureVisualElementTextSize(TextElement te, in RenderedText textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			bool flag = !TextUtilities.IsFontAssigned(te);
			Vector2 result;
			if (flag)
			{
				result = new Vector2(num, num2);
			}
			else
			{
				float num3 = 1f;
				bool flag2 = te.panel != null;
				if (flag2)
				{
					num3 = te.scaledPixelsPerPoint;
				}
				bool flag3 = num3 <= 0f;
				if (flag3)
				{
					result = Vector2.zero;
				}
				else
				{
					bool flag4 = widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly;
					if (flag4)
					{
						Vector2 vector = te.uitkTextHandle.ComputeTextSize(textToMeasure, width, height, fontsize);
						num = vector.x;
						num2 = vector.y;
					}
					bool flag5 = widthMode == VisualElement.MeasureMode.Exactly;
					if (flag5)
					{
						num = width;
					}
					else
					{
						bool flag6 = widthMode == VisualElement.MeasureMode.AtMost;
						if (flag6)
						{
							num = Mathf.Min(num, width);
						}
					}
					bool flag7 = heightMode == VisualElement.MeasureMode.Exactly;
					if (flag7)
					{
						num2 = height;
					}
					else
					{
						bool flag8 = heightMode == VisualElement.MeasureMode.AtMost;
						if (flag8)
						{
							num2 = Mathf.Min(num2, height);
						}
					}
					float num4 = AlignmentUtils.CeilToPixelGrid(num, num3, 0f);
					float y = AlignmentUtils.CeilToPixelGrid(num2, num3, 0f);
					Vector2 vector2 = new Vector2(num4, y);
					bool flag9 = TextUtilities.IsAdvancedTextEnabledForElement(te);
					if (flag9)
					{
						te.uitkTextHandle.ATGMeasuredSizes = new Vector2(num, num2);
						te.uitkTextHandle.ATGRoundedSizes = vector2;
						te.uitkTextHandle.LastPixelPerPoint = num3;
					}
					else
					{
						te.uitkTextHandle.MeasuredWidth = new float?(num);
						te.uitkTextHandle.RoundedWidth = num4;
						te.uitkTextHandle.LastPixelPerPoint = num3;
					}
					result = vector2;
				}
			}
			return result;
		}

		internal static FontAsset GetFontAsset(VisualElement ve)
		{
			bool flag = ve.computedStyle.unityFontDefinition.fontAsset != null;
			FontAsset result;
			if (flag)
			{
				result = ve.computedStyle.unityFontDefinition.fontAsset;
			}
			else
			{
				TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(ve);
				bool flag2 = ve.computedStyle.unityFontDefinition.font != null;
				if (flag2)
				{
					result = textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFontDefinition.font);
				}
				else
				{
					bool flag3 = ve.computedStyle.unityFont != null;
					if (flag3)
					{
						result = textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFont);
					}
					else
					{
						bool flag4 = textSettingsFrom != null;
						if (flag4)
						{
							result = textSettingsFrom.defaultFontAsset;
						}
						else
						{
							result = null;
						}
					}
				}
			}
			return result;
		}

		internal static bool IsFontAssigned(VisualElement ve)
		{
			return ve.computedStyle.unityFont != null || !ve.computedStyle.unityFontDefinition.IsEmpty();
		}

		internal static TextSettings GetTextSettingsFrom(VisualElement ve)
		{
			RuntimePanel runtimePanel = ve.panel as RuntimePanel;
			bool flag = runtimePanel != null;
			TextSettings result;
			if (flag)
			{
				result = (runtimePanel.panelSettings.textSettings ?? PanelTextSettings.defaultPanelTextSettings);
			}
			else
			{
				result = PanelTextSettings.defaultPanelTextSettings;
			}
			return result;
		}

		internal static bool IsAdvancedTextEnabledForElement(VisualElement ve)
		{
			bool flag = ve == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = ve.computedStyle.unityTextGenerator == TextGeneratorType.Advanced;
				bool flag3 = false;
				bool flag4 = ve.panel == null;
				if (flag4)
				{
					result = false;
				}
				else
				{
					RuntimePanel runtimePanel = ve.panel as RuntimePanel;
					bool flag5 = runtimePanel != null;
					if (flag5)
					{
						PanelSettings panelSettings = runtimePanel.panelSettings;
						flag3 = (((panelSettings != null) ? panelSettings.m_ICUDataAsset : null) != null);
					}
					result = (flag2 && flag3);
				}
			}
			return result;
		}

		internal unsafe static TextCoreSettings GetTextCoreSettingsForElement(VisualElement ve, bool ignoreColors)
		{
			FontAsset fontAsset = TextUtilities.GetFontAsset(ve);
			bool flag = fontAsset == null;
			TextCoreSettings result;
			if (flag)
			{
				result = default(TextCoreSettings);
			}
			else
			{
				IResolvedStyle resolvedStyle = ve.resolvedStyle;
				ComputedStyle computedStyle = *ve.computedStyle;
				TextShadow textShadow = computedStyle.textShadow;
				float num = TextHandle.ConvertPixelUnitsToTextCoreRelativeUnits(computedStyle.fontSize.value, fontAsset);
				float num2 = Mathf.Clamp(resolvedStyle.unityTextOutlineWidth * num, 0f, 1f);
				float underlaySoftness = Mathf.Clamp(textShadow.blurRadius * num, 0f, 1f);
				float x = (textShadow.offset.x < 0f) ? Mathf.Max(textShadow.offset.x * num, -1f) : Mathf.Min(textShadow.offset.x * num, 1f);
				float y = (textShadow.offset.y < 0f) ? Mathf.Max(textShadow.offset.y * num, -1f) : Mathf.Min(textShadow.offset.y * num, 1f);
				Vector2 underlayOffset = new Vector2(x, y);
				Color color;
				Color color3;
				if (ignoreColors)
				{
					color = Color.white;
					Color color2 = Color.white;
					color3 = Color.white;
				}
				else
				{
					bool flag2 = ((Texture2D)fontAsset.material.mainTexture).format != TextureFormat.Alpha8;
					color = resolvedStyle.color;
					color3 = resolvedStyle.unityTextOutlineColor;
					bool flag3 = num2 < 1E-30f;
					if (flag3)
					{
						color3.a = 0f;
					}
					Color color2 = textShadow.color;
					bool flag4 = flag2;
					if (flag4)
					{
						color = new Color(1f, 1f, 1f, color.a);
					}
					else
					{
						color2.r *= color.a;
						color2.g *= color.a;
						color2.b *= color.a;
						color3.r *= color3.a;
						color3.g *= color3.a;
						color3.b *= color3.a;
					}
				}
				result = new TextCoreSettings
				{
					faceColor = color,
					outlineColor = color3,
					outlineWidth = num2,
					underlayColor = textShadow.color,
					underlayOffset = underlayOffset,
					underlaySoftness = underlaySoftness
				};
			}
			return result;
		}

		public static TextWrappingMode toTextWrappingMode(this WhiteSpace whiteSpace, bool isSingleLineInputField)
		{
			TextWrappingMode result;
			if (isSingleLineInputField)
			{
				if (!true)
				{
				}
				TextWrappingMode textWrappingMode;
				if (whiteSpace > WhiteSpace.NoWrap)
				{
					if (whiteSpace - WhiteSpace.Pre > 1)
					{
						textWrappingMode = TextWrappingMode.NoWrap;
					}
					else
					{
						textWrappingMode = TextWrappingMode.PreserveWhitespaceNoWrap;
					}
				}
				else
				{
					textWrappingMode = TextWrappingMode.NoWrap;
				}
				if (!true)
				{
				}
				result = textWrappingMode;
			}
			else
			{
				if (!true)
				{
				}
				TextWrappingMode textWrappingMode;
				switch (whiteSpace)
				{
				case WhiteSpace.Normal:
					textWrappingMode = TextWrappingMode.Normal;
					break;
				case WhiteSpace.NoWrap:
					textWrappingMode = TextWrappingMode.NoWrap;
					break;
				case WhiteSpace.Pre:
					textWrappingMode = TextWrappingMode.PreserveWhitespaceNoWrap;
					break;
				case WhiteSpace.PreWrap:
					textWrappingMode = TextWrappingMode.PreserveWhitespace;
					break;
				default:
					textWrappingMode = TextWrappingMode.Normal;
					break;
				}
				if (!true)
				{
				}
				result = textWrappingMode;
			}
			return result;
		}

		public static WhiteSpace toTextCore(this WhiteSpace whiteSpace, bool isInputField)
		{
			WhiteSpace result;
			if (isInputField)
			{
				if (!true)
				{
				}
				WhiteSpace whiteSpace2;
				switch (whiteSpace)
				{
				case WhiteSpace.Normal:
				case WhiteSpace.PreWrap:
					whiteSpace2 = WhiteSpace.PreWrap;
					break;
				case WhiteSpace.NoWrap:
				case WhiteSpace.Pre:
					whiteSpace2 = WhiteSpace.Pre;
					break;
				default:
					whiteSpace2 = WhiteSpace.Pre;
					break;
				}
				if (!true)
				{
				}
				result = whiteSpace2;
			}
			else
			{
				if (!true)
				{
				}
				WhiteSpace whiteSpace2;
				switch (whiteSpace)
				{
				case WhiteSpace.Normal:
					whiteSpace2 = WhiteSpace.Normal;
					break;
				case WhiteSpace.NoWrap:
					whiteSpace2 = WhiteSpace.NoWrap;
					break;
				case WhiteSpace.Pre:
					whiteSpace2 = WhiteSpace.Pre;
					break;
				case WhiteSpace.PreWrap:
					whiteSpace2 = WhiteSpace.PreWrap;
					break;
				default:
					whiteSpace2 = WhiteSpace.Normal;
					break;
				}
				if (!true)
				{
				}
				result = whiteSpace2;
			}
			return result;
		}

		public static TextOverflow toTextCore(this TextOverflow textOverflow, OverflowInternal overflow)
		{
			if (!true)
			{
			}
			TextOverflow result;
			if (textOverflow == TextOverflow.Ellipsis)
			{
				if (overflow == OverflowInternal.Hidden)
				{
					result = TextOverflow.Ellipsis;
					goto IL_1B;
				}
			}
			result = TextOverflow.Clip;
			IL_1B:
			if (!true)
			{
			}
			return result;
		}

		public static Func<TextSettings> getEditorTextSettings;

		internal static Func<bool> IsAdvancedTextEnabled;

		private static TextSettings s_TextSettings;
	}
}
