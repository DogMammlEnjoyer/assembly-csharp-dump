using System;

namespace UnityEngine.UIElements
{
	public struct StyleTextShadow : IStyleValue<TextShadow>, IEquatable<StyleTextShadow>
	{
		public TextShadow value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(TextShadow);
			}
			set
			{
				this.m_Value = value;
				this.m_Keyword = StyleKeyword.Undefined;
			}
		}

		public StyleKeyword keyword
		{
			get
			{
				return this.m_Keyword;
			}
			set
			{
				this.m_Keyword = value;
			}
		}

		public StyleTextShadow(TextShadow v)
		{
			this = new StyleTextShadow(v, StyleKeyword.Undefined);
		}

		public StyleTextShadow(StyleKeyword keyword)
		{
			this = new StyleTextShadow(default(TextShadow), keyword);
		}

		internal StyleTextShadow(TextShadow v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleTextShadow lhs, StyleTextShadow rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleTextShadow lhs, StyleTextShadow rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleTextShadow(StyleKeyword keyword)
		{
			return new StyleTextShadow(keyword);
		}

		public static implicit operator StyleTextShadow(TextShadow v)
		{
			return new StyleTextShadow(v);
		}

		public bool Equals(StyleTextShadow other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is StyleTextShadow);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				StyleTextShadow lhs = (StyleTextShadow)obj;
				result = (lhs == this);
			}
			return result;
		}

		public override int GetHashCode()
		{
			int num = 917506989;
			num = num * -1521134295 + this.m_Keyword.GetHashCode();
			return num * -1521134295 + this.m_Value.GetHashCode();
		}

		public override string ToString()
		{
			return this.DebugString<TextShadow>();
		}

		private StyleKeyword m_Keyword;

		private TextShadow m_Value;
	}
}
