using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal static class StyleSheetUtility
	{
		public static Dimension ToDimension(this Length length)
		{
			bool flag = length.IsAuto() || length.IsNone();
			if (flag)
			{
				throw new InvalidCastException(string.Format("Can't convert a Length to a Dimension because it contains the '{0}' keyword.", length));
			}
			return new Dimension(length.value, length.unit.ToDimensionUnit());
		}

		public static Dimension.Unit ToDimensionUnit(this LengthUnit unit)
		{
			if (!true)
			{
			}
			Dimension.Unit result;
			if (unit != LengthUnit.Pixel)
			{
				if (unit != LengthUnit.Percent)
				{
					throw new InvalidCastException(string.Format("Can't convert a LengthUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 'px' or '%', but was {0}", unit));
				}
				result = Dimension.Unit.Percent;
			}
			else
			{
				result = Dimension.Unit.Pixel;
			}
			if (!true)
			{
			}
			return result;
		}

		public static Dimension ToDimension(this Angle angle)
		{
			bool flag = angle.IsNone();
			if (flag)
			{
				throw new InvalidCastException(string.Format("Can't convert a Rotate to a Dimension because it contains the '{0}' keyword.", angle));
			}
			return new Dimension(angle.value, angle.unit.ToDimensionUnit());
		}

		public static Dimension.Unit ToDimensionUnit(this AngleUnit unit)
		{
			if (!true)
			{
			}
			Dimension.Unit result;
			switch (unit)
			{
			case AngleUnit.Degree:
				result = Dimension.Unit.Degree;
				break;
			case AngleUnit.Gradian:
				result = Dimension.Unit.Gradian;
				break;
			case AngleUnit.Radian:
				result = Dimension.Unit.Radian;
				break;
			case AngleUnit.Turn:
				result = Dimension.Unit.Turn;
				break;
			default:
				throw new InvalidCastException(string.Format("Can't convert a AngleUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 'deg', 'grad', 'rad' or 'turn', but was {0}", unit));
			}
			if (!true)
			{
			}
			return result;
		}

		public static Dimension ToDimension(this TimeValue timeValue)
		{
			return new Dimension(timeValue.value, timeValue.unit.ToDimensionUnit());
		}

		public static Dimension.Unit ToDimensionUnit(this TimeUnit unit)
		{
			if (!true)
			{
			}
			Dimension.Unit result;
			if (unit != TimeUnit.Second)
			{
				if (unit != TimeUnit.Millisecond)
				{
					throw new InvalidCastException(string.Format("Can't convert a TimeUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 's' or 'ms', but was {0}", unit));
				}
				result = Dimension.Unit.Millisecond;
			}
			else
			{
				result = Dimension.Unit.Second;
			}
			if (!true)
			{
			}
			return result;
		}

		public static StyleValueKeyword ToStyleValueKeyword(this StyleKeyword keyword)
		{
			if (!true)
			{
			}
			StyleValueKeyword result;
			switch (keyword)
			{
			case StyleKeyword.Auto:
				result = StyleValueKeyword.Auto;
				break;
			case StyleKeyword.None:
				result = StyleValueKeyword.None;
				break;
			case StyleKeyword.Initial:
				result = StyleValueKeyword.Initial;
				break;
			default:
				throw new InvalidCastException(string.Format("Can't convert a StyleKeyword to a StyleValueKeyword because it does not contain a valid keyword. Expected 'auto', 'none' or 'initial', but was {0}.", keyword));
			}
			if (!true)
			{
			}
			return result;
		}

		public static void TransferStylePropertyHandles(StyleSheet fromStyleSheet, StyleProperty fromStyleProperty, StyleSheet toStyleSheet, StyleProperty toStyleProperty)
		{
			Assert.IsNotNull<StyleSheet>(fromStyleSheet);
			Assert.IsNotNull<StyleSheet>(toStyleSheet);
			Assert.IsNotNull<StyleProperty>(fromStyleProperty);
			Assert.IsNotNull<StyleProperty>(toStyleProperty);
			Assert.IsFalse(fromStyleProperty == toStyleProperty, "Cannot transfer a StyleProperty unto itself.");
			List<StyleValueHandle> list;
			using (CollectionPool<List<StyleValueHandle>, StyleValueHandle>.Get(out list))
			{
				list.AddRange(toStyleProperty.values);
				foreach (StyleValueHandle styleValueHandle in fromStyleProperty.values)
				{
					StyleValueType valueType = styleValueHandle.valueType;
					if (!true)
					{
					}
					int num;
					switch (valueType)
					{
					case StyleValueType.Invalid:
						num = styleValueHandle.valueIndex;
						break;
					case StyleValueType.Keyword:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadKeyword(styleValueHandle));
						break;
					case StyleValueType.Float:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadFloat(styleValueHandle));
						break;
					case StyleValueType.Dimension:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadDimension(styleValueHandle));
						break;
					case StyleValueType.Color:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadColor(styleValueHandle));
						break;
					case StyleValueType.ResourcePath:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadResourcePath(styleValueHandle));
						break;
					case StyleValueType.AssetReference:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadAssetReference(styleValueHandle));
						break;
					case StyleValueType.Enum:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadEnum(styleValueHandle));
						break;
					case StyleValueType.Variable:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadVariable(styleValueHandle));
						break;
					case StyleValueType.String:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadString(styleValueHandle));
						break;
					case StyleValueType.Function:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadFunction(styleValueHandle));
						break;
					case StyleValueType.CommaSeparator:
						num = styleValueHandle.valueIndex;
						break;
					case StyleValueType.ScalableImage:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadScalableImage(styleValueHandle));
						break;
					case StyleValueType.MissingAssetReference:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadMissingAssetReferenceUrl(styleValueHandle));
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!true)
					{
					}
					int valueIndex = num;
					list.Add(new StyleValueHandle(valueIndex, valueType));
				}
				toStyleProperty.isCustomProperty |= toStyleProperty.isCustomProperty;
				toStyleProperty.requireVariableResolve |= toStyleProperty.requireVariableResolve;
				toStyleProperty.values = list.ToArray();
			}
		}

		public static string GetEnumExportString(Enum value)
		{
			return StyleSheetUtility.ConvertCamelToDash(value.ToString());
		}

		public static string ConvertCamelToDash(string camel)
		{
			string text = Regex.Replace(Regex.Replace(camel, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1-$2"), "(\\p{Ll})(\\P{Ll})", "$1-$2");
			string text2 = text.ToLowerInvariant();
			return StyleSheetUtility.SpecialEnumToStringCases.GetValueOrDefault(text2, text2);
		}

		public static string ConvertDashToHungarian(string dash)
		{
			return StyleSheetUtility.ConvertDashToUpperNoSpace(dash, true, false);
		}

		public static string ConvertDashToUpperNoSpace(string dash, bool firstCase, bool addSpace)
		{
			string text;
			bool flag = StyleSheetUtility.SpecialStringToEnumCases.TryGetValue(dash, out text);
			string result;
			if (flag)
			{
				result = text;
			}
			else
			{
				StringBuilder stringBuilder = GenericPool<StringBuilder>.Get();
				try
				{
					bool flag2 = firstCase;
					foreach (char c in dash)
					{
						bool flag3 = c == '-';
						if (flag3)
						{
							if (addSpace)
							{
								stringBuilder.Append(' ');
							}
							flag2 = true;
						}
						else
						{
							bool flag4 = flag2;
							if (flag4)
							{
								stringBuilder.Append(char.ToUpper(c, CultureInfo.InvariantCulture));
								flag2 = false;
							}
							else
							{
								stringBuilder.Append(char.ToLowerInvariant(c));
							}
						}
					}
					result = stringBuilder.ToString();
				}
				finally
				{
					GenericPool<StringBuilder>.Release(stringBuilder.Clear());
				}
			}
			return result;
		}

		private static readonly Dictionary<string, string> SpecialEnumToStringCases = new Dictionary<string, string>
		{
			{
				"no-wrap",
				"nowrap"
			}
		};

		private static readonly Dictionary<string, string> SpecialStringToEnumCases = new Dictionary<string, string>
		{
			{
				"nowrap",
				"NoWrap"
			},
			{
				"sdf",
				"SDF"
			}
		};
	}
}
