using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal struct SelectorMatchRecord : IEquatable<SelectorMatchRecord>
	{
		public SelectorMatchRecord(StyleSheet sheet, int styleSheetIndexInStack)
		{
			this = default(SelectorMatchRecord);
			this.sheet = sheet;
			this.styleSheetIndexInStack = styleSheetIndexInStack;
		}

		public static int Compare(SelectorMatchRecord a, SelectorMatchRecord b)
		{
			bool flag = a.sheet.isDefaultStyleSheet != b.sheet.isDefaultStyleSheet;
			int result;
			if (flag)
			{
				result = (a.sheet.isDefaultStyleSheet ? -1 : 1);
			}
			else
			{
				int num = a.complexSelector.specificity.CompareTo(b.complexSelector.specificity);
				bool flag2 = num == 0;
				if (flag2)
				{
					num = a.styleSheetIndexInStack.CompareTo(b.styleSheetIndexInStack);
				}
				bool flag3 = num == 0;
				if (flag3)
				{
					num = a.complexSelector.orderInStyleSheet.CompareTo(b.complexSelector.orderInStyleSheet);
				}
				result = num;
			}
			return result;
		}

		public bool Equals(SelectorMatchRecord other)
		{
			return object.Equals(this.sheet, other.sheet) && this.styleSheetIndexInStack == other.styleSheetIndexInStack && object.Equals(this.complexSelector, other.complexSelector);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is SelectorMatchRecord)
			{
				SelectorMatchRecord other = (SelectorMatchRecord)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<StyleSheet, int, StyleComplexSelector>(this.sheet, this.styleSheetIndexInStack, this.complexSelector);
		}

		public StyleSheet sheet;

		public int styleSheetIndexInStack;

		public StyleComplexSelector complexSelector;
	}
}
