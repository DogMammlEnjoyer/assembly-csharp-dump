using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal class StyleValueCollection
	{
		public StyleLength GetStyleLength(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleLength result;
			if (flag)
			{
				result = new StyleLength(styleValue.length, styleValue.keyword);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleFloat GetStyleFloat(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleFloat result;
			if (flag)
			{
				result = new StyleFloat(styleValue.number, styleValue.keyword);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleInt GetStyleInt(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleInt result;
			if (flag)
			{
				result = new StyleInt((int)styleValue.number, styleValue.keyword);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleColor GetStyleColor(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleColor result;
			if (flag)
			{
				result = new StyleColor(styleValue.color, styleValue.keyword);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleBackground GetStyleBackground(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				Texture2D texture2D = styleValue.resource.IsAllocated ? (styleValue.resource.Target as Texture2D) : null;
				bool flag2 = texture2D != null;
				if (flag2)
				{
					return new StyleBackground(texture2D, styleValue.keyword);
				}
				Sprite sprite = styleValue.resource.IsAllocated ? (styleValue.resource.Target as Sprite) : null;
				bool flag3 = sprite != null;
				if (flag3)
				{
					return new StyleBackground(sprite, styleValue.keyword);
				}
				VectorImage vectorImage = styleValue.resource.IsAllocated ? (styleValue.resource.Target as VectorImage) : null;
				bool flag4 = vectorImage != null;
				if (flag4)
				{
					return new StyleBackground(vectorImage, styleValue.keyword);
				}
			}
			return StyleKeyword.Null;
		}

		public StyleBackgroundPosition GetStyleBackgroundPosition(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleBackgroundPosition result;
			if (flag)
			{
				result = new StyleBackgroundPosition(styleValue.position);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleBackgroundRepeat GetStyleBackgroundRepeat(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleBackgroundRepeat result;
			if (flag)
			{
				result = new StyleBackgroundRepeat(styleValue.repeat);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleFont GetStyleFont(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleFont result;
			if (flag)
			{
				Font v = styleValue.resource.IsAllocated ? (styleValue.resource.Target as Font) : null;
				result = new StyleFont(v, styleValue.keyword);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public StyleFontDefinition GetStyleFontDefinition(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleFontDefinition result;
			if (flag)
			{
				object obj = styleValue.resource.IsAllocated ? styleValue.resource.Target : null;
				result = new StyleFontDefinition(obj, styleValue.keyword);
			}
			else
			{
				result = StyleKeyword.Null;
			}
			return result;
		}

		public bool TryGetStyleValue(StylePropertyId id, ref StyleValue value)
		{
			value.id = StylePropertyId.Unknown;
			foreach (StyleValue styleValue in this.m_Values)
			{
				bool flag = styleValue.id == id;
				if (flag)
				{
					value = styleValue;
					return true;
				}
			}
			return false;
		}

		public void SetStyleValue(StyleValue value)
		{
			for (int i = 0; i < this.m_Values.Count; i++)
			{
				bool flag = this.m_Values[i].id == value.id;
				if (flag)
				{
					bool flag2 = value.keyword == StyleKeyword.Null;
					if (flag2)
					{
						this.m_Values.RemoveAt(i);
					}
					else
					{
						this.m_Values[i] = value;
					}
					return;
				}
			}
			this.m_Values.Add(value);
		}

		internal List<StyleValue> m_Values = new List<StyleValue>();
	}
}
