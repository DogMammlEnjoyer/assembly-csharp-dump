using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	internal static class StyleValueExtensions
	{
		internal static string DebugString<T>(this IStyleValue<T> styleValue)
		{
			return (styleValue.keyword != StyleKeyword.Undefined) ? string.Format("{0}", styleValue.keyword) : string.Format("{0}", styleValue.value);
		}

		internal static LayoutValue ToLayoutValue(this Length length)
		{
			bool flag = length.IsAuto();
			LayoutValue result;
			if (flag)
			{
				result = LayoutValue.Auto();
			}
			else
			{
				bool flag2 = length.IsNone();
				if (flag2)
				{
					result = float.NaN;
				}
				else
				{
					LengthUnit unit = length.unit;
					LengthUnit lengthUnit = unit;
					if (lengthUnit != LengthUnit.Pixel)
					{
						if (lengthUnit != LengthUnit.Percent)
						{
							Debug.LogAssertion(string.Format("Unexpected unit '{0}'", length.unit));
							result = float.NaN;
						}
						else
						{
							result = LayoutValue.Percent(length.value);
						}
					}
					else
					{
						result = LayoutValue.Point(length.value);
					}
				}
			}
			return result;
		}

		internal static Length ToLength(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Length result;
			if (styleKeyword2 != StyleKeyword.Auto)
			{
				if (styleKeyword2 != StyleKeyword.None)
				{
					Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
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
			return result;
		}

		internal static Rotate ToRotate(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Rotate result;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				result = default(Rotate);
			}
			else
			{
				result = Rotate.None();
			}
			return result;
		}

		internal static Scale ToScale(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Scale result;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				result = default(Scale);
			}
			else
			{
				result = Scale.None();
			}
			return result;
		}

		internal static Translate ToTranslate(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Translate result;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				result = default(Translate);
			}
			else
			{
				result = Translate.None();
			}
			return result;
		}

		internal static TextAutoSize ToTextAutoSize(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			TextAutoSize result;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				result = default(TextAutoSize);
			}
			else
			{
				result = TextAutoSize.None();
			}
			return result;
		}

		internal static Length ToLength(this StyleLength styleLength)
		{
			StyleKeyword keyword = styleLength.keyword;
			StyleKeyword styleKeyword = keyword;
			Length result;
			if (styleKeyword - StyleKeyword.Auto > 1)
			{
				result = styleLength.value;
			}
			else
			{
				result = styleLength.keyword.ToLength();
			}
			return result;
		}

		internal static void CopyFrom<T>(this List<T> list, List<T> other)
		{
			list.Clear();
			list.AddRange(other);
		}
	}
}
