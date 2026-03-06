using System;

namespace UnityEngine.UIElements
{
	public struct StyleTranslate : IStyleValue<Translate>, IEquatable<StyleTranslate>
	{
		public Translate value
		{
			get
			{
				StyleKeyword keyword = this.m_Keyword;
				if (!true)
				{
				}
				Translate result;
				switch (keyword)
				{
				case StyleKeyword.Undefined:
					result = this.m_Value;
					goto IL_4F;
				case StyleKeyword.Null:
					result = Translate.None();
					goto IL_4F;
				case StyleKeyword.None:
					result = Translate.None();
					goto IL_4F;
				case StyleKeyword.Initial:
					result = Translate.None();
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

		public StyleTranslate(Translate v)
		{
			this = new StyleTranslate(v, StyleKeyword.Undefined);
		}

		public StyleTranslate(StyleKeyword keyword)
		{
			this = new StyleTranslate(default(Translate), keyword);
		}

		internal StyleTranslate(Translate v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleTranslate lhs, StyleTranslate rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleTranslate lhs, StyleTranslate rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleTranslate(StyleKeyword keyword)
		{
			return new StyleTranslate(keyword);
		}

		public static implicit operator StyleTranslate(Translate v)
		{
			return new StyleTranslate(v);
		}

		public static implicit operator StyleTranslate(Vector3 v)
		{
			return new StyleTranslate(v);
		}

		public static implicit operator StyleTranslate(Vector2 v)
		{
			return new StyleTranslate(v);
		}

		public bool Equals(StyleTranslate other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is StyleTranslate)
			{
				StyleTranslate other = (StyleTranslate)obj;
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
			return this.DebugString<Translate>();
		}

		private Translate m_Value;

		private StyleKeyword m_Keyword;
	}
}
