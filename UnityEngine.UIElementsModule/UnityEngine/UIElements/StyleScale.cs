using System;

namespace UnityEngine.UIElements
{
	public struct StyleScale : IStyleValue<Scale>, IEquatable<StyleScale>
	{
		public Scale value
		{
			get
			{
				StyleKeyword keyword = this.m_Keyword;
				if (!true)
				{
				}
				Scale result;
				switch (keyword)
				{
				case StyleKeyword.Undefined:
					result = this.m_Value;
					goto IL_4F;
				case StyleKeyword.Null:
					result = Scale.None();
					goto IL_4F;
				case StyleKeyword.None:
					result = Scale.None();
					goto IL_4F;
				case StyleKeyword.Initial:
					result = Scale.Initial();
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

		public StyleScale(Scale v)
		{
			this = new StyleScale(v, StyleKeyword.Undefined);
		}

		public StyleScale(StyleKeyword keyword)
		{
			this = new StyleScale(default(Scale), keyword);
		}

		public StyleScale(Vector2 scale)
		{
			this = new StyleScale(new Scale(scale));
		}

		internal StyleScale(Vector3 scale)
		{
			this = new StyleScale(new Scale(scale));
		}

		internal StyleScale(Scale v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static implicit operator StyleScale(Vector2 scale)
		{
			return new Scale(scale);
		}

		public static implicit operator StyleScale(Vector3 scale)
		{
			return new Scale(scale);
		}

		public static bool operator ==(StyleScale lhs, StyleScale rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleScale lhs, StyleScale rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleScale(StyleKeyword keyword)
		{
			return new StyleScale(keyword);
		}

		public static implicit operator StyleScale(Scale v)
		{
			return new StyleScale(v);
		}

		public bool Equals(StyleScale other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is StyleScale)
			{
				StyleScale other = (StyleScale)obj;
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
			return this.DebugString<Scale>();
		}

		private Scale m_Value;

		private StyleKeyword m_Keyword;
	}
}
