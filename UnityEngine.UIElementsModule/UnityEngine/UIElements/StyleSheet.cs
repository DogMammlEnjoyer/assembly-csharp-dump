using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-USS")]
	[Serializable]
	public class StyleSheet : ScriptableObject
	{
		public bool importedWithErrors
		{
			get
			{
				return this.m_ImportedWithErrors;
			}
			internal set
			{
				this.m_ImportedWithErrors = value;
			}
		}

		public bool importedWithWarnings
		{
			get
			{
				return this.m_ImportedWithWarnings;
			}
			internal set
			{
				this.m_ImportedWithWarnings = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal StyleRule[] rules
		{
			get
			{
				return this.m_Rules;
			}
			set
			{
				this.m_Rules = value;
				this.SetupReferences();
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal StyleComplexSelector[] complexSelectors
		{
			get
			{
				return this.m_ComplexSelectors;
			}
			set
			{
				this.m_ComplexSelectors = value;
				this.SetupReferences();
			}
		}

		internal List<StyleSheet> flattenedRecursiveImports
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_FlattenedImportedStyleSheets;
			}
		}

		public int contentHash
		{
			get
			{
				return this.m_ContentHash;
			}
			set
			{
				this.m_ContentHash = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool isDefaultStyleSheet
		{
			get
			{
				return this.m_IsDefaultStyleSheet;
			}
			set
			{
				this.m_IsDefaultStyleSheet = value;
				bool flag = this.flattenedRecursiveImports != null;
				if (flag)
				{
					foreach (StyleSheet styleSheet in this.flattenedRecursiveImports)
					{
						styleSheet.isDefaultStyleSheet = value;
					}
				}
			}
		}

		private bool TryCheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle, out T value)
		{
			bool flag = handle.valueType != type || handle.valueIndex < 0 || handle.valueIndex >= list.Length;
			bool result;
			if (flag)
			{
				value = default(T);
				result = false;
			}
			else
			{
				value = list[handle.valueIndex];
				result = true;
			}
			return result;
		}

		private T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
		{
			T result = default(T);
			bool flag = handle.valueType != type;
			if (flag)
			{
				Debug.LogErrorFormat(this, "Trying to read value of type {0} while reading a value of type {1}", new object[]
				{
					type,
					handle.valueType
				});
			}
			else
			{
				bool flag2 = list == null || handle.valueIndex < 0 || handle.valueIndex >= list.Length;
				if (flag2)
				{
					Debug.LogError("Accessing invalid property", this);
				}
				else
				{
					result = list[handle.valueIndex];
				}
			}
			return result;
		}

		internal virtual void OnEnable()
		{
			this.SetupReferences();
		}

		internal void FlattenImportedStyleSheetsRecursive()
		{
			this.m_FlattenedImportedStyleSheets = new List<StyleSheet>();
			this.FlattenImportedStyleSheetsRecursive(this);
		}

		private void FlattenImportedStyleSheetsRecursive(StyleSheet sheet)
		{
			bool flag = sheet.imports == null;
			if (!flag)
			{
				for (int i = 0; i < sheet.imports.Length; i++)
				{
					StyleSheet styleSheet = sheet.imports[i].styleSheet;
					bool flag2 = styleSheet == null;
					if (!flag2)
					{
						styleSheet.isDefaultStyleSheet = this.isDefaultStyleSheet;
						this.FlattenImportedStyleSheetsRecursive(styleSheet);
						this.m_FlattenedImportedStyleSheets.Add(styleSheet);
					}
				}
			}
		}

		private void SetupReferences()
		{
			bool flag = this.complexSelectors == null || this.rules == null || (this.complexSelectors.Length == 0 && this.rules.Length == 0);
			if (!flag)
			{
				foreach (StyleRule styleRule in this.rules)
				{
					foreach (StyleProperty styleProperty in styleRule.properties)
					{
						bool flag2 = StyleSheet.CustomStartsWith(styleProperty.name, StyleSheet.kCustomPropertyMarker);
						if (flag2)
						{
							styleRule.customPropertiesCount++;
							styleProperty.isCustomProperty = true;
						}
						foreach (StyleValueHandle styleValueHandle in styleProperty.values)
						{
							bool flag3 = styleValueHandle.IsVarFunction();
							if (flag3)
							{
								styleProperty.requireVariableResolve = true;
								break;
							}
						}
					}
				}
				int l = 0;
				int num = this.complexSelectors.Length;
				while (l < num)
				{
					this.complexSelectors[l].CachePseudoStateMasks(this);
					l++;
				}
				this.tables = new Dictionary<string, StyleComplexSelector>[3];
				this.tables[0] = new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal);
				this.tables[1] = new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal);
				this.tables[2] = new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal);
				this.nonEmptyTablesMask = 0;
				this.firstRootSelector = null;
				this.firstWildCardSelector = null;
				int m = 0;
				while (m < this.complexSelectors.Length)
				{
					StyleComplexSelector styleComplexSelector = this.complexSelectors[m];
					bool flag4 = styleComplexSelector.ruleIndex < this.rules.Length;
					if (flag4)
					{
						styleComplexSelector.rule = this.rules[styleComplexSelector.ruleIndex];
					}
					styleComplexSelector.CalculateHashes();
					styleComplexSelector.orderInStyleSheet = m;
					StyleSelector styleSelector = styleComplexSelector.selectors[styleComplexSelector.selectors.Length - 1];
					StyleSelectorPart styleSelectorPart = styleSelector.parts[0];
					string value = styleSelectorPart.value;
					StyleSheet.OrderedSelectorType orderedSelectorType = StyleSheet.OrderedSelectorType.None;
					switch (styleSelectorPart.type)
					{
					case StyleSelectorType.Wildcard:
					{
						bool flag5 = this.firstWildCardSelector != null;
						if (flag5)
						{
							styleComplexSelector.nextInTable = this.firstWildCardSelector;
						}
						this.firstWildCardSelector = styleComplexSelector;
						break;
					}
					case StyleSelectorType.Type:
						value = styleSelectorPart.value;
						orderedSelectorType = StyleSheet.OrderedSelectorType.Type;
						break;
					case StyleSelectorType.Class:
						orderedSelectorType = StyleSheet.OrderedSelectorType.Class;
						break;
					case StyleSelectorType.PseudoClass:
					{
						bool flag6 = (styleSelector.pseudoStateMask & 128) != 0;
						if (flag6)
						{
							bool flag7 = this.firstRootSelector != null;
							if (flag7)
							{
								styleComplexSelector.nextInTable = this.firstRootSelector;
							}
							this.firstRootSelector = styleComplexSelector;
						}
						else
						{
							bool flag8 = this.firstWildCardSelector != null;
							if (flag8)
							{
								styleComplexSelector.nextInTable = this.firstWildCardSelector;
							}
							this.firstWildCardSelector = styleComplexSelector;
						}
						break;
					}
					case StyleSelectorType.RecursivePseudoClass:
						goto IL_2E2;
					case StyleSelectorType.ID:
						orderedSelectorType = StyleSheet.OrderedSelectorType.Name;
						break;
					default:
						goto IL_2E2;
					}
					IL_301:
					bool flag9 = orderedSelectorType != StyleSheet.OrderedSelectorType.None;
					if (flag9)
					{
						Dictionary<string, StyleComplexSelector> dictionary = this.tables[(int)orderedSelectorType];
						StyleComplexSelector nextInTable;
						bool flag10 = dictionary.TryGetValue(value, out nextInTable);
						if (flag10)
						{
							styleComplexSelector.nextInTable = nextInTable;
						}
						this.nonEmptyTablesMask |= 1 << (int)orderedSelectorType;
						dictionary[value] = styleComplexSelector;
					}
					m++;
					continue;
					IL_2E2:
					Debug.LogError(string.Format("Invalid first part type {0}", styleSelectorPart.type), this);
					goto IL_301;
				}
			}
		}

		private int AddValueToArray<T>(ref T[] array, T value)
		{
			CollectionExtensions.AddToArray<T>(ref array, value);
			this.SetTemporaryContentHash();
			return array.Length - 1;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(StyleValueKeyword keyword)
		{
			this.SetTemporaryContentHash();
			return (int)keyword;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(StyleValueFunction function)
		{
			this.SetTemporaryContentHash();
			return (int)function;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(float value)
		{
			return this.AddValueToArray<float>(ref this.floats, value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(Dimension value)
		{
			return this.AddValueToArray<Dimension>(ref this.dimensions, value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(Color value)
		{
			return this.AddValueToArray<Color>(ref this.colors, value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(ScalableImage value)
		{
			return this.AddValueToArray<ScalableImage>(ref this.scalableImages, value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(string value)
		{
			return this.AddValueToArray<string>(ref this.strings, value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(Object value)
		{
			return this.AddValueToArray<Object>(ref this.assets, value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int AddValue(Enum value)
		{
			string enumExportString = StyleSheetUtility.GetEnumExportString(value);
			return this.AddValueToArray<string>(ref this.strings, enumExportString);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal StyleValueKeyword ReadKeyword(StyleValueHandle handle)
		{
			return (StyleValueKeyword)handle.valueIndex;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool TryReadKeyword(StyleValueHandle handle, out StyleValueKeyword value)
		{
			value = (StyleValueKeyword)handle.valueIndex;
			return handle.valueType == StyleValueType.Keyword;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal float ReadFloat(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Dimension;
			float result;
			if (flag)
			{
				Dimension dimension = this.CheckAccess<Dimension>(this.dimensions, StyleValueType.Dimension, handle);
				result = dimension.value;
			}
			else
			{
				result = this.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
			}
			return result;
		}

		internal bool TryReadFloat(StyleValueHandle handle, out float value)
		{
			bool flag = this.TryCheckAccess<float>(this.floats, StyleValueType.Float, handle, out value);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Dimension dimension;
				bool flag2 = this.TryCheckAccess<Dimension>(this.dimensions, StyleValueType.Float, handle, out dimension);
				value = dimension.value;
				result = flag2;
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal Dimension ReadDimension(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Float;
			Dimension result;
			if (flag)
			{
				float value = this.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
				result = new Dimension(value, Dimension.Unit.Unitless);
			}
			else
			{
				result = this.CheckAccess<Dimension>(this.dimensions, StyleValueType.Dimension, handle);
			}
			return result;
		}

		internal bool TryReadDimension(StyleValueHandle handle, out Dimension value)
		{
			bool flag = this.TryCheckAccess<Dimension>(this.dimensions, StyleValueType.Dimension, handle, out value);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				float value2;
				bool flag2 = this.TryCheckAccess<float>(this.floats, StyleValueType.Float, handle, out value2);
				value = new Dimension(value2, Dimension.Unit.Unitless);
				result = flag2;
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal Color ReadColor(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Enum;
			Color result;
			if (flag)
			{
				string text = this.ReadEnum(handle);
				Color color;
				StyleSheetColor.TryGetColor(text.ToLowerInvariant(), out color);
				result = color;
			}
			else
			{
				result = this.CheckAccess<Color>(this.colors, StyleValueType.Color, handle);
			}
			return result;
		}

		internal bool TryReadColor(StyleValueHandle handle, out Color value)
		{
			bool flag = this.TryCheckAccess<Color>(this.colors, StyleValueType.Color, handle, out value);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				string text;
				bool flag2 = this.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handle, out text);
				if (flag2)
				{
					result = StyleSheetColor.TryGetColor(text.ToLowerInvariant(), out value);
				}
				else
				{
					value = default(Color);
					result = false;
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal string ReadString(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.String, handle);
		}

		internal bool TryReadString(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.String, handle, out value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal string ReadEnum(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.Enum, handle);
		}

		internal bool TryReadEnum(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handle, out value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal TEnum ReadEnum<TEnum>(StyleValueHandle handle) where TEnum : struct, Enum
		{
			string dash = this.ReadEnum(handle);
			TEnum tenum;
			return Enum.TryParse<TEnum>(StyleSheetUtility.ConvertDashToHungarian(dash), out tenum) ? tenum : default(TEnum);
		}

		internal bool TryReadEnum<TEnum>(StyleValueHandle handle, out TEnum value) where TEnum : struct, Enum
		{
			string dash;
			bool flag = this.TryReadEnum(handle, out dash) && Enum.TryParse<TEnum>(StyleSheetUtility.ConvertDashToHungarian(dash), out value);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				value = default(TEnum);
				result = false;
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal string ReadVariable(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.Variable, handle);
		}

		internal bool TryReadVariable(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.Variable, handle, out value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal string ReadResourcePath(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle);
		}

		internal bool TryReadResourcePath(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle, out value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal Object ReadAssetReference(StyleValueHandle handle)
		{
			return this.CheckAccess<Object>(this.assets, StyleValueType.AssetReference, handle);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal string ReadMissingAssetReferenceUrl(StyleValueHandle handle)
		{
			return this.CheckAccess<string>(this.strings, StyleValueType.MissingAssetReference, handle);
		}

		internal bool TryReadMissingAssetReferenceUrl(StyleValueHandle handle, out string value)
		{
			return this.TryCheckAccess<string>(this.strings, StyleValueType.MissingAssetReference, handle, out value);
		}

		internal bool TryReadAssetReference(StyleValueHandle handle, out Object value)
		{
			return this.TryCheckAccess<Object>(this.assets, StyleValueType.AssetReference, handle, out value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal StyleValueFunction ReadFunction(StyleValueHandle handle)
		{
			return (StyleValueFunction)handle.valueIndex;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool TryReadFunction(StyleValueHandle handle, out StyleValueFunction value)
		{
			value = (StyleValueFunction)handle.valueIndex;
			return handle.valueType == StyleValueType.Function;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal string ReadFunctionName(StyleValueHandle handle)
		{
			bool flag = handle.valueType != StyleValueType.Function;
			string result;
			if (flag)
			{
				Debug.LogErrorFormat(this, string.Format("Trying to read value of type {0} while reading a value of type {1}", StyleValueType.Function, handle.valueType), Array.Empty<object>());
				result = string.Empty;
			}
			else
			{
				StyleValueFunction valueIndex = (StyleValueFunction)handle.valueIndex;
				result = valueIndex.ToUssString();
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal ScalableImage ReadScalableImage(StyleValueHandle handle)
		{
			return this.CheckAccess<ScalableImage>(this.scalableImages, StyleValueType.ScalableImage, handle);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool TryReadScalableImage(StyleValueHandle handle, out ScalableImage value)
		{
			return this.TryCheckAccess<ScalableImage>(this.scalableImages, StyleValueType.ScalableImage, handle, out value);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal StylePropertyName ReadStylePropertyName(StyleValueHandle handle)
		{
			return new StylePropertyName(this.CheckAccess<string>(this.strings, StyleValueType.Enum, handle));
		}

		internal bool TryReadStylePropertyName(StyleValueHandle handle, out StylePropertyName value)
		{
			string name;
			bool flag = this.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handle, out name);
			bool result;
			if (flag)
			{
				value = new StylePropertyName(name);
				result = true;
			}
			else
			{
				value = default(StylePropertyName);
				result = false;
			}
			return result;
		}

		internal Length ReadLength(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Keyword;
			Length result;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword = this.ReadKeyword(handle);
				if (!true)
				{
				}
				Length length;
				if (styleValueKeyword != StyleValueKeyword.Auto)
				{
					if (styleValueKeyword != StyleValueKeyword.None)
					{
						length = default(Length);
					}
					else
					{
						length = Length.None();
					}
				}
				else
				{
					length = Length.Auto();
				}
				if (!true)
				{
				}
				result = length;
			}
			else
			{
				Dimension dimension = this.ReadDimension(handle);
				result = (dimension.IsLength() ? dimension.ToLength() : default(Length));
			}
			return result;
		}

		internal bool TryReadLength(StyleValueHandle handle, out Length value)
		{
			StyleValueKeyword styleValueKeyword;
			bool flag = this.TryReadKeyword(handle, out styleValueKeyword);
			bool result;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				StyleValueKeyword styleValueKeyword3 = styleValueKeyword2;
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
				Dimension dimension;
				bool flag2 = this.TryReadDimension(handle, out dimension) && dimension.IsLength();
				if (flag2)
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
			return result;
		}

		internal Angle ReadAngle(StyleValueHandle handle)
		{
			bool flag = handle.valueType == StyleValueType.Keyword;
			Angle result;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword = this.ReadKeyword(handle);
				if (!true)
				{
				}
				Angle angle;
				if (styleValueKeyword != StyleValueKeyword.None)
				{
					angle = default(Angle);
				}
				else
				{
					angle = Angle.None();
				}
				if (!true)
				{
				}
				result = angle;
			}
			else
			{
				Dimension dimension = this.ReadDimension(handle);
				result = (dimension.IsAngle() ? dimension.ToAngle() : default(Angle));
			}
			return result;
		}

		internal bool TryReadAngle(StyleValueHandle handle, out Angle value)
		{
			StyleValueKeyword styleValueKeyword;
			bool flag = this.TryReadKeyword(handle, out styleValueKeyword);
			bool result;
			if (flag)
			{
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				StyleValueKeyword styleValueKeyword3 = styleValueKeyword2;
				if (styleValueKeyword3 != StyleValueKeyword.None)
				{
					value = default(Angle);
					result = false;
				}
				else
				{
					value = Angle.None();
					result = true;
				}
			}
			else
			{
				Dimension dimension;
				bool flag2 = this.TryReadDimension(handle, out dimension) && dimension.IsAngle();
				if (flag2)
				{
					value = dimension.ToAngle();
					result = true;
				}
				else
				{
					value = default(Angle);
					result = false;
				}
			}
			return result;
		}

		internal TimeValue ReadTimeValue(StyleValueHandle handle)
		{
			Dimension dimension = this.ReadDimension(handle);
			return dimension.IsTimeValue() ? dimension.ToTime() : default(TimeValue);
		}

		internal bool TryReadTimeValue(StyleValueHandle handle, out TimeValue value)
		{
			Dimension dimension;
			bool flag = this.TryReadDimension(handle, out dimension) && dimension.IsTimeValue();
			bool result;
			if (flag)
			{
				value = dimension.ToTime();
				result = true;
			}
			else
			{
				value = default(TimeValue);
				result = false;
			}
			return result;
		}

		private static bool CustomStartsWith(string originalString, string pattern)
		{
			int length = originalString.Length;
			int length2 = pattern.Length;
			int num = 0;
			int num2 = 0;
			while (num < length && num2 < length2 && originalString[num] == pattern[num2])
			{
				num++;
				num2++;
			}
			return (num2 == length2 && length >= length2) || (num == length && length2 >= length);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteKeyword(ref StyleValueHandle handle, StyleValueKeyword value)
		{
			handle.valueType = StyleValueType.Keyword;
			handle.valueIndex = (int)value;
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteFloat(ref StyleValueHandle handle, float value)
		{
			bool flag = handle.valueType == StyleValueType.Float;
			if (flag)
			{
				this.floats[handle.valueIndex] = value;
			}
			else
			{
				int valueIndex = this.AddValue(value);
				handle.valueType = StyleValueType.Float;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteDimension(ref StyleValueHandle handle, Dimension dimension)
		{
			bool flag = handle.valueType == StyleValueType.Dimension;
			if (flag)
			{
				this.dimensions[handle.valueIndex] = dimension;
			}
			else
			{
				int valueIndex = this.AddValue(dimension);
				handle.valueType = StyleValueType.Dimension;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteColor(ref StyleValueHandle handle, Color color)
		{
			bool flag = handle.valueType == StyleValueType.Color;
			if (flag)
			{
				this.colors[handle.valueIndex] = color;
			}
			else
			{
				int valueIndex = this.AddValue(color);
				handle.valueType = StyleValueType.Color;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteString(ref StyleValueHandle handle, string value)
		{
			bool flag = handle.valueType == StyleValueType.String;
			if (flag)
			{
				this.strings[handle.valueIndex] = value;
			}
			else
			{
				int valueIndex = this.AddValue(value);
				handle.valueType = StyleValueType.String;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteEnum<TEnum>(ref StyleValueHandle handle, TEnum value) where TEnum : Enum
		{
			string enumExportString = StyleSheetUtility.GetEnumExportString(value);
			this.WriteEnumAsString(ref handle, enumExportString);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteEnumAsString(ref StyleValueHandle handle, string valueStr)
		{
			bool flag = handle.valueType == StyleValueType.Enum;
			if (flag)
			{
				this.strings[handle.valueIndex] = valueStr;
			}
			else
			{
				int valueIndex = this.AddValue(valueStr);
				handle.valueType = StyleValueType.Enum;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteVariable(ref StyleValueHandle handle, string variableName)
		{
			bool flag = handle.valueType == StyleValueType.Variable;
			if (flag)
			{
				this.strings[handle.valueIndex] = variableName;
			}
			else
			{
				int valueIndex = this.AddValue(variableName);
				handle.valueType = StyleValueType.Variable;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteResourcePath(ref StyleValueHandle handle, string resourcePath)
		{
			bool flag = handle.valueType == StyleValueType.ResourcePath;
			if (flag)
			{
				this.strings[handle.valueIndex] = resourcePath;
			}
			else
			{
				int valueIndex = this.AddValue(resourcePath);
				handle.valueType = StyleValueType.ResourcePath;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteAssetReference(ref StyleValueHandle handle, Object value)
		{
			bool flag = handle.valueType == StyleValueType.AssetReference;
			if (flag)
			{
				this.assets[handle.valueIndex] = value;
			}
			else
			{
				int valueIndex = this.AddValue(value);
				handle.valueType = StyleValueType.AssetReference;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteMissingAssetReferenceUrl(ref StyleValueHandle handle, string assetReference)
		{
			bool flag = handle.valueType == StyleValueType.MissingAssetReference;
			if (flag)
			{
				this.strings[handle.valueIndex] = assetReference;
			}
			else
			{
				int valueIndex = this.AddValue(assetReference);
				handle.valueType = StyleValueType.MissingAssetReference;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteFunction(ref StyleValueHandle handle, StyleValueFunction function)
		{
			handle.valueType = StyleValueType.Function;
			handle.valueIndex = (int)function;
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteScalableImage(ref StyleValueHandle handle, ScalableImage scalableImage)
		{
			bool flag = handle.valueType == StyleValueType.ScalableImage;
			if (flag)
			{
				this.scalableImages[handle.valueIndex] = scalableImage;
			}
			else
			{
				int valueIndex = this.AddValue(scalableImage);
				handle.valueType = StyleValueType.ScalableImage;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteStylePropertyName(ref StyleValueHandle handle, StylePropertyName propertyName)
		{
			bool flag = handle.valueType == StyleValueType.Enum;
			if (flag)
			{
				this.strings[handle.valueIndex] = propertyName.ToString();
			}
			else
			{
				int valueIndex = this.AddValue(propertyName.ToString());
				handle.valueType = StyleValueType.Enum;
				handle.valueIndex = valueIndex;
			}
			this.SetTemporaryContentHash();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void WriteCommaSeparator(ref StyleValueHandle handle)
		{
			handle.valueIndex = 0;
			handle.valueType = StyleValueType.CommaSeparator;
			this.SetTemporaryContentHash();
		}

		internal void WriteLength(ref StyleValueHandle handle, Length value)
		{
			bool flag = value.IsAuto();
			if (flag)
			{
				this.WriteKeyword(ref handle, StyleValueKeyword.Auto);
			}
			else
			{
				bool flag2 = value.IsNone();
				if (flag2)
				{
					this.WriteKeyword(ref handle, StyleValueKeyword.None);
				}
				else
				{
					this.WriteDimension(ref handle, value.ToDimension());
				}
			}
		}

		internal void WriteAngle(ref StyleValueHandle handle, Angle value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				this.WriteKeyword(ref handle, StyleValueKeyword.None);
			}
			else
			{
				this.WriteDimension(ref handle, value.ToDimension());
			}
		}

		internal void WriteTimeValue(ref StyleValueHandle handle, TimeValue value)
		{
			this.WriteDimension(ref handle, value.ToDimension());
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void SetTemporaryContentHash()
		{
			bool flag = this.rules == null || this.rules.Length == 0;
			if (flag)
			{
				this.contentHash = 0;
			}
			else
			{
				this.contentHash = Random.Range(1, int.MaxValue);
			}
		}

		[SerializeField]
		private bool m_ImportedWithErrors;

		[SerializeField]
		private bool m_ImportedWithWarnings;

		[SerializeField]
		private StyleRule[] m_Rules = Array.Empty<StyleRule>();

		[SerializeField]
		private StyleComplexSelector[] m_ComplexSelectors = Array.Empty<StyleComplexSelector>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[SerializeField]
		internal float[] floats = Array.Empty<float>();

		[SerializeField]
		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal Dimension[] dimensions = Array.Empty<Dimension>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[SerializeField]
		internal Color[] colors = Array.Empty<Color>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[SerializeField]
		internal string[] strings = Array.Empty<string>();

		[SerializeField]
		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal Object[] assets = Array.Empty<Object>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[SerializeField]
		internal StyleSheet.ImportStruct[] imports = Array.Empty<StyleSheet.ImportStruct>();

		[SerializeField]
		private List<StyleSheet> m_FlattenedImportedStyleSheets = new List<StyleSheet>();

		[SerializeField]
		private int m_ContentHash;

		[SerializeField]
		internal ScalableImage[] scalableImages = Array.Empty<ScalableImage>();

		[NonSerialized]
		internal Dictionary<string, StyleComplexSelector>[] tables;

		[NonSerialized]
		internal int nonEmptyTablesMask;

		[NonSerialized]
		internal StyleComplexSelector firstRootSelector;

		[NonSerialized]
		internal StyleComplexSelector firstWildCardSelector;

		[NonSerialized]
		private bool m_IsDefaultStyleSheet;

		private static string kCustomPropertyMarker = "--";

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[Serializable]
		internal struct ImportStruct
		{
			public StyleSheet styleSheet;

			public string[] mediaQueries;
		}

		internal enum OrderedSelectorType
		{
			None = -1,
			Name,
			Type,
			Class,
			Length
		}
	}
}
