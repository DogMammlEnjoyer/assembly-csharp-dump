using System;

namespace UnityEngine.UIElements
{
	public struct StyleTransformOrigin : IStyleValue<TransformOrigin>, IEquatable<StyleTransformOrigin>
	{
		public TransformOrigin value
		{
			get
			{
				StyleKeyword keyword = this.m_Keyword;
				if (!true)
				{
				}
				TransformOrigin result;
				switch (keyword)
				{
				case StyleKeyword.Undefined:
					result = this.m_Value;
					goto IL_4F;
				case StyleKeyword.Null:
					result = TransformOrigin.Initial();
					goto IL_4F;
				case StyleKeyword.None:
					result = TransformOrigin.Initial();
					goto IL_4F;
				case StyleKeyword.Initial:
					result = TransformOrigin.Initial();
					goto IL_4F;
				}
				throw new NotImplementedException();
				IL_4F:
				if (!true)
				{
				}
				return result;
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

		public StyleTransformOrigin(TransformOrigin v)
		{
			this = new StyleTransformOrigin(v, StyleKeyword.Undefined);
		}

		public StyleTransformOrigin(StyleKeyword keyword)
		{
			this = new StyleTransformOrigin(default(TransformOrigin), keyword);
		}

		internal StyleTransformOrigin(TransformOrigin v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleTransformOrigin lhs, StyleTransformOrigin rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleTransformOrigin lhs, StyleTransformOrigin rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleTransformOrigin(StyleKeyword keyword)
		{
			return new StyleTransformOrigin(keyword);
		}

		public static implicit operator StyleTransformOrigin(TransformOrigin v)
		{
			return new StyleTransformOrigin(v);
		}

		public bool Equals(StyleTransformOrigin other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is StyleTransformOrigin)
			{
				StyleTransformOrigin other = (StyleTransformOrigin)obj;
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
			return this.m_Value.GetHashCode() * 397 ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<TransformOrigin>();
		}

		private TransformOrigin m_Value;

		private StyleKeyword m_Keyword;
	}
}
